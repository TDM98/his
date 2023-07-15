using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
/*
 * 20182510 #001 TTM: BM 002173: Thêm mới PtRegistrationID và V_RegistrationType
 * 20220125 #002 Thêm thông tin nước tiểu
 * 20230626 #003 DatTB: Thêm thông tin khoa/phòng lưu sinh hiệu
 */
namespace DataEntities
{
    public partial class PhysicalExamination : EntityBase, IEditableObject
    {
        #region Factory Method

        /// Create a new PhysicalExamination object.

        /// <param name="phyExamID">Initial value of the PhyExamID property.</param>
        /// <param name="recordDate">Initial value of the RecordDate property.</param>
        public static PhysicalExamination CreatePhysicalExamination(long phyExamID, DateTime recordDate)
        {
            PhysicalExamination physicalExamination = new PhysicalExamination();
            physicalExamination.PhyExamID = phyExamID;
            physicalExamination.RecordDate = recordDate;
            return physicalExamination;
        }

        public PhysicalExamination()
        {
            this.RecordDate = DateTime.Now;
        }
        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PhyExamID
        {
            get
            {
                return _PhyExamID;
            }
            set
            {
                if (_PhyExamID != value)
                {
                    OnPhyExamIDChanging(value);
                    _PhyExamID = value;
                    RaisePropertyChanged("PhyExamID");
                    OnPhyExamIDChanged();
                }
            }
        }
        private long _PhyExamID;
        partial void OnPhyExamIDChanging(long value);
        partial void OnPhyExamIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> CommonMedRecID
        {
            get
            {
                return _CommonMedRecID;
            }
            set
            {
                OnCommonMedRecIDChanging(value);
                _CommonMedRecID = value;
                RaisePropertyChanged("CommonMedRecID");
                OnCommonMedRecIDChanged();
            }
        }
        private Nullable<long> _CommonMedRecID;
        partial void OnCommonMedRecIDChanging(Nullable<long> value);
        partial void OnCommonMedRecIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> RecordDate
        {
            get
            {
                return _RecordDate;
            }
            set
            {
                OnRecordDateChanging(value);
                _RecordDate = value;
                RaisePropertyChanged("RecordDate");
                OnRecordDateChanged();
            }
        }
        private Nullable<DateTime> _RecordDate;
        partial void OnRecordDateChanging(Nullable<DateTime> value);
        partial void OnRecordDateChanged();

        [DataMemberAttribute()]
        [Range(0.1, 9999999, ErrorMessage = "Chiều cao > 0")]
        public Nullable<Double> Height
        {
            get
            {
                return _Height;
            }
            set
            {
                OnHeightChanging(value);
                _Height = value;
                RaisePropertyChanged("Height");
                OnHeightChanged();
                CountBMI();
            }
        }
        private Nullable<Double> _Height;
        partial void OnHeightChanging(Nullable<Double> value);
        partial void OnHeightChanged();

        [DataMemberAttribute()]
        [Range(1, 9999999, ErrorMessage = "Cân nặng > 1")]
        public Nullable<Double> Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                OnWeightChanging(value);
                _Weight = value;
                RaisePropertyChanged("Weight");
                OnWeightChanged();
                CountBMI();
            }
        }
        private Nullable<Double> _Weight;
        partial void OnWeightChanging(Nullable<Double> value);
        partial void OnWeightChanged();

        [DataMemberAttribute()]
        public double? BMI
        {
            get
            {
                return _BMI;
            }
            set
            {
                if (_BMI != value)
                    _BMI = value;
                RaisePropertyChanged("BMI");
            }
        }
        private double? _BMI;


        [DataMemberAttribute()]
        [Range(1, 9999999, ErrorMessage = "Systolic Pressure > 0")]
        public Nullable<Double> SystolicPressure
        {
            get
            {
                return _SystolicPressure;
            }
            set
            {
                OnSystolicPressureChanging(value);
                _SystolicPressure = value;
                RaisePropertyChanged("SystolicPressure");
                OnSystolicPressureChanged();
            }
        }
        private Nullable<Double> _SystolicPressure;
        partial void OnSystolicPressureChanging(Nullable<Double> value);
        partial void OnSystolicPressureChanged();

        [DataMemberAttribute()]
        [Range(1, 9999999, ErrorMessage = "Diastolic Pressure > 0")]
        public Nullable<Double> DiastolicPressure
        {
            get
            {
                return _DiastolicPressure;
            }
            set
            {
                OnDiastolicPressureChanging(value);
                _DiastolicPressure = value;
                RaisePropertyChanged("DiastolicPressure");
                OnDiastolicPressureChanged();
            }
        }
        private Nullable<Double> _DiastolicPressure;
        partial void OnDiastolicPressureChanging(Nullable<Double> value);
        partial void OnDiastolicPressureChanged();

        [DataMemberAttribute()]
        [Range(1, 9999999, ErrorMessage = "Pulse > 0")]
        public Nullable<Double> Pulse
        {
            get
            {
                return _Pulse;
            }
            set
            {
                OnPulseChanging(value);
                _Pulse = value;
                RaisePropertyChanged("Pulse");
                OnPulseChanged();
            }
        }
        private Nullable<Double> _Pulse;
        partial void OnPulseChanging(Nullable<Double> value);
        partial void OnPulseChanged();

        [DataMemberAttribute()]
        [Range(1, 9999999, ErrorMessage = "Cholesterol > 0")]
        public Nullable<Double> Cholesterol
        {
            get
            {
                return _Cholesterol;
            }
            set
            {
                OnCholesterolChanging(value);
                _Cholesterol = value;
                RaisePropertyChanged("Cholesterol");
                OnCholesterolChanged();
            }
        }
        private Nullable<Double> _Cholesterol;
        partial void OnCholesterolChanging(Nullable<Double> value);
        partial void OnCholesterolChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> Smoke_EveryDay
        {
            get
            {
                return _Smoke_EveryDay;
            }
            set
            {
                if (_Smoke_EveryDay!=value)
                {
                    OnSmoke_EveryDayChanging(value);
                    _Smoke_EveryDay = value;
                    RaisePropertyChanged("Smoke_EveryDay");
                    OnSmoke_EveryDayChanged();
                }
                
            }
        }
        private Nullable<Boolean> _Smoke_EveryDay;
        partial void OnSmoke_EveryDayChanging(Nullable<Boolean> value);
        partial void OnSmoke_EveryDayChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> Smoke_OnOccasion
        {
            get
            {
                return _Smoke_OnOccasion;
            }
            set
            {
                if (_Smoke_OnOccasion != value)
                {
                    OnSmoke_OnOccasionChanging(value);
                    _Smoke_OnOccasion = value;
                    RaisePropertyChanged("Smoke_OnOccasion");
                    OnSmoke_OnOccasionChanged();
                }
            }
        }
        private Nullable<Boolean> _Smoke_OnOccasion;
        partial void OnSmoke_OnOccasionChanging(Nullable<Boolean> value);
        partial void OnSmoke_OnOccasionChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> Smoke_Never
        {
            get
            {
                return _Smoke_Never;
            }
            set
            {
                OnSmoke_NeverChanging(value);
                _Smoke_Never = value;
                RaisePropertyChanged("Smoke_Never");
                OnSmoke_NeverChanged();
            }
        }
        private Nullable<Boolean> _Smoke_Never;
        partial void OnSmoke_NeverChanging(Nullable<Boolean> value);
        partial void OnSmoke_NeverChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> Alcohol_CurrentHeavy
        {
            get
            {
                return _Alcohol_CurrentHeavy;
            }
            set
            {
                OnAlcohol_CurrentHeavyChanging(value);
                _Alcohol_CurrentHeavy = value;
                RaisePropertyChanged("Alcohol_CurrentHeavy");
                OnAlcohol_CurrentHeavyChanged();
            }
        }
        private Nullable<Boolean> _Alcohol_CurrentHeavy;
        partial void OnAlcohol_CurrentHeavyChanging(Nullable<Boolean> value);
        partial void OnAlcohol_CurrentHeavyChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> Alcohol_HeavyInThePast
        {
            get
            {
                return _Alcohol_HeavyInThePast;
            }
            set
            {
                OnAlcohol_HeavyInThePastChanging(value);
                _Alcohol_HeavyInThePast = value;
                RaisePropertyChanged("Alcohol_HeavyInThePast");
                OnAlcohol_HeavyInThePastChanged();
            }
        }
        private Nullable<Boolean> _Alcohol_HeavyInThePast;
        partial void OnAlcohol_HeavyInThePastChanging(Nullable<Boolean> value);
        partial void OnAlcohol_HeavyInThePastChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> Alcohol_CurrentLight
        {
            get
            {
                return _Alcohol_CurrentLight;
            }
            set
            {
                OnAlcohol_CurrentLightChanging(value);
                _Alcohol_CurrentLight = value;
                RaisePropertyChanged("Alcohol_CurrentLight");
                OnAlcohol_CurrentLightChanged();
            }
        }
        private Nullable<Boolean> _Alcohol_CurrentLight;
        partial void OnAlcohol_CurrentLightChanging(Nullable<Boolean> value);
        partial void OnAlcohol_CurrentLightChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> Alcohol_Never
        {
            get
            {
                return _Alcohol_Never;
            }
            set
            {
                OnAlcohol_NeverChanging(value);
                _Alcohol_Never = value;
                RaisePropertyChanged("Alcohol_Never");
                OnAlcohol_NeverChanged();
            }
        }
        private Nullable<Boolean> _Alcohol_Never;
        partial void OnAlcohol_NeverChanging(Nullable<Boolean> value);
        partial void OnAlcohol_NeverChanged();

        [DataMemberAttribute()]
        public Nullable<Double> CVRisk
        {
            get
            {
                return _CVRisk;
            }
            set
            {
                OnCVRiskChanging(value);
                _CVRisk = value;
                RaisePropertyChanged("CVRisk");
                OnCVRiskChanged();
            }
        }
        private Nullable<Double> _CVRisk;
        partial void OnCVRiskChanging(Nullable<Double> value);
        partial void OnCVRiskChanged();
        //Dinh them
        [DataMemberAttribute()]
        public Nullable<Int64> V_SmokeStatus
        {
            get
            {
                return _V_SmokeStatus;
            }
            set
            {
                OnV_SmokeStatusChanging(value);
                _V_SmokeStatus = value;
                RaisePropertyChanged("V_SmokeStatus");
                OnV_SmokeStatusChanged();
            }
        }
        private Nullable<Int64> _V_SmokeStatus;
        partial void OnV_SmokeStatusChanging(Nullable<Int64> value);
        partial void OnV_SmokeStatusChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_AlcoholDrikingStatus
        {
            get
            {
                return _V_AlcoholDrikingStatus;
            }
            set
            {
                OnV_AlcoholDrikingStatusChanging(value);
                _V_AlcoholDrikingStatus = value;
                RaisePropertyChanged("V_AlcoholDrikingStatus");
                OnV_AlcoholDrikingStatusChanged();
            }
        }
        private Nullable<Int64> _V_AlcoholDrikingStatus;
        partial void OnV_AlcoholDrikingStatusChanging(Nullable<Int64> value);
        partial void OnV_AlcoholDrikingStatusChanged();


        [DataMemberAttribute()]
        private bool _isDeleted=true;
        public bool isDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                if (_isDeleted == value)
                    return;
                _isDeleted = value;
                RaisePropertyChanged("isDeleted");
            }
        }

        [DataMemberAttribute()]
        private bool _isEdit=true;
        public bool isEdit
        {
            get
            {
                return _isEdit;
            }
            set
            {
                if (_isEdit == value)
                    return;
                _isEdit = value;
                RaisePropertyChanged("isEdit");
                if(isEdit==false)
                {
                    isSave = true;
                    isCancel = true;
                }else
                {
                    isSave = false;
                    isCancel = false;
                }

            }
        }

        [DataMemberAttribute()]
        private bool _isCancel=false;
        public bool isCancel
        {
            get
            {
                return _isCancel;
            }
            set
            {
                if (_isCancel == value)
                    return;
                _isCancel = value;
                RaisePropertyChanged("isCancel");
            }
        }

        [DataMemberAttribute()]
        private bool _isSave=false;
        public bool isSave
        {
            get
            {
                return _isSave;
            }
            set
            {
                if (_isSave == value)
                    return;
                _isSave = value;
                RaisePropertyChanged("isSave");
            }
        }


        [DataMemberAttribute()]
        private int _SmokeCigarettePerDay;
        public int SmokeCigarettePerDay
        {
            get
            {
                return _SmokeCigarettePerDay;
            }
            set
            {
                if (_SmokeCigarettePerDay == value)
                    return;
                _SmokeCigarettePerDay = value;
                RaisePropertyChanged("SmokeCigarettePerDay");
            }
        }

        
        [DataMemberAttribute()]
        private double _MonthHaveSmoked;
        public double MonthHaveSmoked
        {
            get
            {
                return _MonthHaveSmoked;
            }
            set
            {
                if (_MonthHaveSmoked == value)
                    return;
                _MonthHaveSmoked = value;
                RaisePropertyChanged("MonthHaveSmoked");
            }
        }
        
        [DataMemberAttribute()]
        private double _MonthQuitSmoking;
        public double MonthQuitSmoking
        {
            get
            {
                return _MonthQuitSmoking;
            }
            set
            {
                if (_MonthQuitSmoking == value)
                    return;
                _MonthQuitSmoking = value;
                RaisePropertyChanged("MonthQuitSmoking");
            }
        }

        
        [DataMemberAttribute()]
        private double? _RespiratoryRate;
        public double? RespiratoryRate
        {
            get
            {
                return _RespiratoryRate;
            }
            set
            {
                if (_RespiratoryRate == value)
                    return;
                _RespiratoryRate = value;
                RaisePropertyChanged("RespiratoryRate");
            }
        }

        [DataMemberAttribute()]
        private double? _Temperature;
        public double? Temperature
        {
            get
            {
                return _Temperature;
            }
            set
            {
                if (_Temperature == value)
                    return;
                _Temperature = value;
                RaisePropertyChanged("Temperature");
            }
        }
            
        [DataMemberAttribute()]
        private double? _SpO2;
        public double? SpO2
        {
            get
            {
                return _SpO2;
            }
            set
            {
                if (_SpO2 == value)
                    return;
                _SpO2 = value;
                RaisePropertyChanged("SpO2");
            }
        }
        [DataMemberAttribute()]
        private bool? _OxygenBreathing;
        public bool? OxygenBreathing
        {
            get
            {
                return _OxygenBreathing;
            }
            set
            {
                if (_OxygenBreathing == value)
                    return;
                _OxygenBreathing = value;
                RaisePropertyChanged("OxygenBreathing");
            }
        }
        [DataMemberAttribute()]
        private int? _V_ConsciousnessLevel;
        public int? V_ConsciousnessLevel
        {
            get
            {
                return _V_ConsciousnessLevel;
            }
            set
            {
                if (_V_ConsciousnessLevel == value)
                    return;
                _V_ConsciousnessLevel = value;
                RaisePropertyChanged("V_ConsciousnessLevel");
            }
        }
        [DataMemberAttribute()]
        private int? _V_PainLevel;
        public int? V_PainLevel
        {
            get
            {
                return _V_PainLevel;
            }
            set
            {
                if (_V_PainLevel == value)
                    return;
                _V_PainLevel = value;
                RaisePropertyChanged("V_PainLevel");
            }
        }
        [DataMemberAttribute()]
        private string _OxygenBreathingStr;
        public string OxygenBreathingStr
        {
            get
            {
                return _OxygenBreathingStr;
            }
            set
            {
                if (_OxygenBreathingStr == value)
                    return;
                _OxygenBreathingStr = value;
                RaisePropertyChanged("OxygenBreathingStr");
            }
        }
        [DataMemberAttribute()]
        private string _ConsciousnessLevelStr;
        public string ConsciousnessLevelStr
        {
            get
            {
                return _ConsciousnessLevelStr;
            }
            set
            {
                if (_ConsciousnessLevelStr == value)
                    return;
                _ConsciousnessLevelStr = value;
                RaisePropertyChanged("ConsciousnessLevelStr");
            }
        }
        [DataMemberAttribute()]
        private string _PainLevelStr;
        public string PainLevelStr
        {
            get
            {
                return _PainLevelStr;
            }
            set
            {
                if (_PainLevelStr == value)
                    return;
                _PainLevelStr = value;
                RaisePropertyChanged("PainLevelStr");
            }
        }

        //▼==== #003
        [DataMemberAttribute()]
        private long _DeptLocID;
        public long DeptLocID
        {
            get
            {
                return _DeptLocID;
            }
            set
            {
                if (_DeptLocID == value)
                    return;
                _DeptLocID = value;
                RaisePropertyChanged("DeptLocID");
            }
        }
        //▲==== #003

        private void CountBMI()
        {
            if (Weight > 0 && Height > 0)
            {
                BMI = Math.Round((double)Weight / Math.Pow((double)Height / 100, 2), 2);
            }
            else
            {
                BMI = 0;
            }
        }
        #endregion
        #region Navigation Properties
        private CommonMedicalRecord _CommonMedicalRecord;
        [DataMemberAttribute()]
        public CommonMedicalRecord CommonMedicalRecord
        {
            get
            {
                return _CommonMedicalRecord;
            }
            set
            {
                if (_CommonMedicalRecord != value)
                {
                    _CommonMedicalRecord = value;
                    RaisePropertyChanged("CommonMedicalRecord");
                }
            }
        }
        [DataMemberAttribute()]
        public Lookup RefAlcohol
        {
            get
            {
                return _RefAlcohol;
            }
            set
            {
                OnRefAlcoholChanging(value);
                _RefAlcohol = value;
                RaisePropertyChanged("RefAlcohol");
                OnRefAlcoholChanged();
            }
        }
        private Lookup _RefAlcohol;
        partial void OnRefAlcoholChanging(Lookup value);
        partial void OnRefAlcoholChanged();

        [DataMemberAttribute()]
        public Lookup RefSmoke
        {
            get
            {
                return _RefSmoke;
            }
            set
            {
                OnRefSmokeChanging(value);
                _RefSmoke = value;
                RaisePropertyChanged("RefSmoke");
                OnRefSmokeChanged();
            }
        }
        private Lookup _RefSmoke;
        partial void OnRefSmokeChanging(Lookup value);
        partial void OnRefSmokeChanged();


        //▼====== #001
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
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
        //▲====== #001

        private byte _BustSize;
        private long? _V_HealthyClassification;
        [DataMemberAttribute]
        public byte BustSize
        {
            get
            {
                return _BustSize;
            }
            set
            {
                _BustSize = value;
                RaisePropertyChanged("BustSize");
            }
        }
        [DataMemberAttribute]
        public long? V_HealthyClassification
        {
            get
            {
                return _V_HealthyClassification;
            }
            set
            {
                _V_HealthyClassification = value;
                RaisePropertyChanged("V_HealthyClassification");
            }
        }
        private string _Diet;
        [DataMemberAttribute]
        public string Diet
        {
            get
            {
                return _Diet;
            }
            set
            {
                _Diet = value;
                RaisePropertyChanged("Diet");
            }
        }

        private double? _Urine;
        [DataMemberAttribute]
        public double? Urine
        {
            get
            {
                return _Urine;
            }
            set
            {
                _Urine = value;
                RaisePropertyChanged("Urine");
            }
        }
        #endregion
        private PhysicalExamination _tempPhysicalExamination;
        #region IEditableObject Members
        public void BeginEdit()
        {
            _tempPhysicalExamination = (PhysicalExamination)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPhysicalExamination)
                CopyFrom(_tempPhysicalExamination);
        }

        public void EndEdit()
        {
            //Dinh cap nhat thong tin o day ne
        }

        public void CopyFrom(PhysicalExamination p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        #endregion
    }
}