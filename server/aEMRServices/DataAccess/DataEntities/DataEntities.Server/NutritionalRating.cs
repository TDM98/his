using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static DataEntities.AllLookupValues;

namespace DataEntities
{
    public partial class NutritionalRating : EntityBase, IEditableObject
    {
        public void BeginEdit()
        {
            throw new NotImplementedException();
        }
        #region Primitive Properties
        [DataMemberAttribute()]
        public Int64 NutritionalRatingID
        {
            get
            {
                return _NutritionalRatingID;
            }
            set
            {
                if (_NutritionalRatingID != value)
                {
                    OnNutritionalRatingIDChanging(value);
                    _NutritionalRatingID = value;
                    RaisePropertyChanged("NutritionalRatingID");
                    OnNutritionalRatingIDChanged();
                }
            }
        }
        private Int64 _NutritionalRatingID;
        partial void OnNutritionalRatingIDChanging(Int64 value);
        partial void OnNutritionalRatingIDChanged();

        [DataMemberAttribute()]
        public Int64 PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID != value)
                {
                    OnPtRegistrationIDChanging(value);
                    _PtRegistrationID = value;
                    RaisePropertyChanged("PtRegistrationID");
                    OnPtRegistrationIDChanged();
                }
            }
        }
        private Int64 _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Int64 value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public bool ROM_BMI
        {
            get
            {
                return _ROM_BMI;
            }
            set
            {
                OnROM_BMIChanging(value);
                _ROM_BMI = value;
                RaisePropertyChanged("ROM_BMI");
                OnROM_BMIChanged();
            }
        }
        private bool _ROM_BMI;
        partial void OnROM_BMIChanging(bool value);
        partial void OnROM_BMIChanged();

        [DataMemberAttribute()]
        public bool ROM_WeightLoss
        {
            get
            {
                return _ROM_WeightLoss;
            }
            set
            {
                OnROM_WeightLossChanging(value);
                _ROM_WeightLoss = value;
                RaisePropertyChanged("ROM_WeightLoss");
                OnROM_WeightLossChanged();
            }
        }
        private bool _ROM_WeightLoss;
        partial void OnROM_WeightLossChanging(bool value);
        partial void OnROM_WeightLossChanged();

        [DataMemberAttribute()]
        public bool ROM_ReduceEat
        {
            get
            {
                return _ROM_ReduceEat;
            }
            set
            {
                OnROM_ReduceEatChanging(value);
                _ROM_ReduceEat = value;
                RaisePropertyChanged("ROM_ReduceEat");
                OnROM_ReduceEatChanged();
            }
        }
        private bool _ROM_ReduceEat;
        partial void OnROM_ReduceEatChanging(bool value);
        partial void OnROM_ReduceEatChanged();

        [DataMemberAttribute()]
        public bool ROM_SevereIllness
        {
            get
            {
                return _ROM_SevereIllness;
            }
            set
            {
                OnROM_SevereIllnessChanging(value);
                _ROM_SevereIllness = value;
                RaisePropertyChanged("ROM_SevereIllness");
                OnROM_SevereIllnessChanged();
            }
        }
        private bool _ROM_SevereIllness;
        partial void OnROM_SevereIllnessChanging(bool value);
        partial void OnROM_SevereIllnessChanged();

        [DataMemberAttribute()]
        public bool RiskOfMalnutrition
        {
            get
            {
                return _RiskOfMalnutrition;
            }
            set
            {
                OnRiskOfMalnutritionChanging(value);
                _RiskOfMalnutrition = value;
                RaisePropertyChanged("RiskOfMalnutrition");
                OnRiskOfMalnutritionChanged();
            }
        }
        private bool _RiskOfMalnutrition;
        partial void OnRiskOfMalnutritionChanging(bool value);
        partial void OnRiskOfMalnutritionChanged();

        [DataMemberAttribute()]
        public bool WeightLossHospitalStay
        {
            get
            {
                return _WeightLossHospitalStay;
            }
            set
            {
                OnWeightLossHospitalStayChanging(value);
                _WeightLossHospitalStay = value;
                RaisePropertyChanged("WeightLossHospitalStay");
                OnWeightLossHospitalStayChanged();
            }
        }
        private bool _WeightLossHospitalStay;
        partial void OnWeightLossHospitalStayChanging(bool value);
        partial void OnWeightLossHospitalStayChanged();

        [DataMemberAttribute()]
        public Byte WL_Weight
        {
            get
            {
                return _WL_Weight;
            }
            set
            {
                OnWL_WeightChanging(value);
                _WL_Weight = value;
                RaisePropertyChanged("WL_Weight");
                OnWL_WeightChanged();
            }
        }
        private Byte _WL_Weight;
        partial void OnWL_WeightChanging(Byte value);
        partial void OnWL_WeightChanged();

        [DataMemberAttribute()]
        public Byte WL_Month
        {
            get
            {
                return _WL_Month;
            }
            set
            {
                OnWL_MonthChanging(value);
                _WL_Month = value;
                RaisePropertyChanged("WL_Month");
                OnWL_MonthChanged();
            }
        }
        private Byte _WL_Month;
        partial void OnWL_MonthChanging(Byte value);
        partial void OnWL_MonthChanged();

        [DataMemberAttribute()]
        public Byte WL_Percent
        {
            get
            {
                return _WL_Percent;
            }
            set
            {
                OnWL_PercentChanging(value);
                _WL_Percent = value;
                RaisePropertyChanged("WL_Percent");
                OnWL_PercentChanged();
            }
        }
        private Byte _WL_Percent;
        partial void OnWL_PercentChanging(Byte value);
        partial void OnWL_PercentChanged();

        [DataMemberAttribute()]
        public Int64 V_EatingType
        {
            get
            {
                return _V_EatingType;
            }
            set
            {
                if (_V_EatingType != value)
                {
                    OnV_EatingTypeChanging(value);
                    _V_EatingType = value;
                    RaisePropertyChanged("V_EatingType");
                    OnV_EatingTypeChanged();
                }
            }
        }
        private Int64 _V_EatingType;
        partial void OnV_EatingTypeChanging(Int64 value);
        partial void OnV_EatingTypeChanged();

        [DataMemberAttribute()]
        public Int64 AtrophySubcutaneousFatLayer
        {
            get
            {
                return _AtrophySubcutaneousFatLayer;
            }
            set
            {
                if (_AtrophySubcutaneousFatLayer != value)
                {
                    OnAtrophySubcutaneousFatLayerChanging(value);
                    _AtrophySubcutaneousFatLayer = value;
                    RaisePropertyChanged("AtrophySubcutaneousFatLayer");
                    OnAtrophySubcutaneousFatLayerChanged();
                }
            }
        }
        private Int64 _AtrophySubcutaneousFatLayer;
        partial void OnAtrophySubcutaneousFatLayerChanging(Int64 value);
        partial void OnAtrophySubcutaneousFatLayerChanged();

        [DataMemberAttribute()]
        public Int64 AmyotrophicLateralSclerosis
        {
            get
            {
                return _AmyotrophicLateralSclerosis;
            }
            set
            {
                if (_AmyotrophicLateralSclerosis != value)
                {
                    OnAmyotrophicLateralSclerosisChanging(value);
                    _AmyotrophicLateralSclerosis = value;
                    RaisePropertyChanged("AmyotrophicLateralSclerosis");
                    OnAmyotrophicLateralSclerosisChanged();
                }
            }
        }
        private Int64 _AmyotrophicLateralSclerosis;
        partial void OnAmyotrophicLateralSclerosisChanging(Int64 value);
        partial void OnAmyotrophicLateralSclerosisChanged();

        [DataMemberAttribute()]
        public Int64 PeripheralEdema
        {
            get
            {
                return _PeripheralEdema;
            }
            set
            {
                if (_PeripheralEdema != value)
                {
                    OnPeripheralEdemaChanging(value);
                    _PeripheralEdema = value;
                    RaisePropertyChanged("PeripheralEdema");
                    OnPeripheralEdemaChanged();
                }
            }
        }
        private Int64 _PeripheralEdema;
        partial void OnPeripheralEdemaChanging(Int64 value);
        partial void OnPeripheralEdemaChanged();

        [DataMemberAttribute()]
        public Int64 BellyFlap
        {
            get
            {
                return _BellyFlap;
            }
            set
            {
                if (_BellyFlap != value)
                {
                    OnBellyFlapChanging(value);
                    _BellyFlap = value;
                    RaisePropertyChanged("BellyFlap");
                    OnBellyFlapChanged();
                }
            }
        }
        private Int64 _BellyFlap;
        partial void OnBellyFlapChanging(Int64 value);
        partial void OnBellyFlapChanged();

        [DataMemberAttribute()]
        public Int64 V_SGAType
        {
            get
            {
                return _V_SGAType;
            }
            set
            {
                if (_V_SGAType != value)
                {
                    OnV_SGATypeChanging(value);
                    _V_SGAType = value;
                    RaisePropertyChanged("V_SGAType");
                    OnV_SGATypeChanged();
                }
            }
        }
        private Int64 _V_SGAType;
        partial void OnV_SGATypeChanging(Int64 value);
        partial void OnV_SGATypeChanged();

        [DataMemberAttribute()]
        public Int64 V_NutritionalRequire
        {
            get
            {
                return _V_NutritionalRequire;
            }
            set
            {
                if (_V_NutritionalRequire != value)
                {
                    OnV_NutritionalRequireChanging(value);
                    _V_NutritionalRequire = value;
                    RaisePropertyChanged("V_NutritionalRequire");
                    OnV_NutritionalRequireChanged();
                }
            }
        }
        private Int64 _V_NutritionalRequire;
        partial void OnV_NutritionalRequireChanging(Int64 value);
        partial void OnV_NutritionalRequireChanged();

        [DataMemberAttribute()]
        public Int32 ONT_Kcal
        {
            get
            {
                return _ONT_Kcal;
            }
            set
            {
                if (_ONT_Kcal != value)
                {
                    OnONT_KcalChanging(value);
                    _ONT_Kcal = value;
                    RaisePropertyChanged("ONT_Kcal");
                    OnONT_KcalChanged();
                }
            }
        }
        private Int32 _ONT_Kcal;
        partial void OnONT_KcalChanging(Int32 value);
        partial void OnONT_KcalChanged();

        [DataMemberAttribute()]
        public Int32 ONT_Protein
        {
            get
            {
                return _ONT_Protein;
            }
            set
            {
                if (_ONT_Protein != value)
                {
                    OnONT_ProteinChanging(value);
                    _ONT_Protein = value;
                    RaisePropertyChanged("ONT_Protein");
                    OnONT_ProteinChanged();
                }
            }
        }
        private Int32 _ONT_Protein;
        partial void OnONT_ProteinChanging(Int32 value);
        partial void OnONT_ProteinChanged();

        [DataMemberAttribute()]
        public Int32 ONT_Fat
        {
            get
            {
                return _ONT_Fat;
            }
            set
            {
                if (_ONT_Fat != value)
                {
                    OnONT_FatChanging(value);
                    _ONT_Fat = value;
                    RaisePropertyChanged("ONT_Fat");
                    OnONT_FatChanged();
                }
            }
        }
        private Int32 _ONT_Fat;
        partial void OnONT_FatChanging(Int32 value);
        partial void OnONT_FatChanged();

        [DataMemberAttribute()]
        public String ONT_Other
        {
            get
            {
                return _ONT_Other;
            }
            set
            {
                if (_ONT_Other != value)
                {
                    OnONT_OtherChanging(value);
                    _ONT_Other = value;
                    RaisePropertyChanged("ONT_Other");
                    OnONT_OtherChanged();
                }
            }
        }
        private String _ONT_Other;
        partial void OnONT_OtherChanging(String value);
        partial void OnONT_OtherChanged();

        [DataMemberAttribute()]
        public Int64 V_NutritionalMethods
        {
            get
            {
                return _V_NutritionalMethods;
            }
            set
            {
                if (_V_NutritionalMethods != value)
                {
                    OnV_NutritionalMethodsChanging(value);
                    _V_NutritionalMethods = value;
                    RaisePropertyChanged("V_NutritionalMethods");
                    OnV_NutritionalMethodsChanged();
                }
            }
        }
        private Int64 _V_NutritionalMethods;
        partial void OnV_NutritionalMethodsChanging(Int64 value);
        partial void OnV_NutritionalMethodsChanged();

        [DataMemberAttribute()]
        public bool ConsultationNutritional
        {
            get
            {
                return _ConsultationNutritional;
            }
            set
            {
                if (_ConsultationNutritional != value)
                {
                    OnConsultationNutritionalChanging(value);
                    _ConsultationNutritional = value;
                    RaisePropertyChanged("ConsultationNutritional");
                    OnConsultationNutritionalChanged();
                }
            }
        }
        private bool _ConsultationNutritional;
        partial void OnConsultationNutritionalChanging(bool value);
        partial void OnConsultationNutritionalChanged();
        [DataMemberAttribute()]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    _CreatedDate = value;
                    RaisePropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
                }
            }
        }
        private DateTime _CreatedDate;
        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();
        [DataMemberAttribute()]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                if (_CreatedStaff != value)
                {
                    OnCreatedStaffChanging(value);
                    _CreatedStaff = value;
                    RaisePropertyChanged("CreatedStaff");
                    OnCreatedStaffChanged();
                }
            }
        }
        private Staff _CreatedStaff;
        partial void OnCreatedStaffChanging(Staff value);
        partial void OnCreatedStaffChanged();
        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();
        #endregion
        public void CancelEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            throw new NotImplementedException();
        }
        #region Navigation Properties
        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_LOCATION_REL_RM29_ROOMTYPE", "Locations")]
        public ObservableCollection<NutritionalRating> NutritionalRatings
        {
            get;
            set;
        }
        #endregion
    }
}
