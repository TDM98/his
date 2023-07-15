using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class RiskFactors : EntityBase, IEditableObject
    {
        #region Factory Method

        /// Create a new RiskFactors object.

        /// <param name="RiskFactorID">Initial value of the RiskFactorID property.</param>
        /// <param name="CreatedDate">Initial value of the RecordDate property.</param>
        public static RiskFactors CreateRiskFactors(long RiskFactorID, DateTime CreatedDate)
        {
            RiskFactors RiskFactors = new RiskFactors();
            RiskFactors.RiskFactorID = RiskFactorID;
            RiskFactors.CreatedDate = CreatedDate;
            return RiskFactors;
        }

        public RiskFactors()
        {
            this.CreatedDate = DateTime.Now;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long RiskFactorID
        {
            get
            {
                return _RiskFactorID;
            }
            set
            {
                if (_RiskFactorID != value)
                {
                    OnRiskFactorIDChanging(value);
                    _RiskFactorID = value;
                    RaisePropertyChanged("RiskFactorID");
                    OnRiskFactorIDChanged();
                }
            }
        }
        private long _RiskFactorID;
        partial void OnRiskFactorIDChanging(long value);
        partial void OnRiskFactorIDChanged();

        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                OnPatientIDChanging(value);
                _PatientID = value;
                RaisePropertyChanged("PatientID");
                OnPatientIDChanged();
            }
        }
        private long _PatientID;
        partial void OnPatientIDChanging(long value);
        partial void OnPatientIDChanged();

        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private long _StaffID;
        partial void OnStaffIDChanging(long value);
        partial void OnStaffIDChanged();



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
        public Nullable<DateTime> CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                OnCreatedDateChanging(value);
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
                OnCreatedDateChanged();
            }
        }
        private Nullable<DateTime> _CreatedDate;
        partial void OnCreatedDateChanging(Nullable<DateTime> value);
        partial void OnCreatedDateChanged();

	
        [DataMemberAttribute()]
        public bool Smoking
        {
            get
            {
                return _Smoking;
            }
            set
            {
                if (_Smoking != value)
                {
                    OnSmokingChanging(value);
                    _Smoking = value;
                    RaisePropertyChanged("Smoking");
                    OnSmokingChanged();
                }
            }
        }
        private bool _Smoking;
        partial void OnSmokingChanging(bool value);
        partial void OnSmokingChanged();

	
        [DataMemberAttribute()]
        public string SmokingDescr
        {
            get
            {
                return _SmokingDescr;
            }
            set
            {
                if (_SmokingDescr != value)
                {
                    OnSmokingDescrChanging(value);
                    _SmokingDescr = value;
                    RaisePropertyChanged("SmokingDescr");
                    OnSmokingDescrChanged();
                }
            }
        }
        private string _SmokingDescr;
        partial void OnSmokingDescrChanging(string value);
        partial void OnSmokingDescrChanged();

	
        [DataMemberAttribute()]
        public bool Drinking
        {
            get
            {
                return _Drinking;
            }
            set
            {
                if (_Drinking != value)
                {
                    OnDrinkingChanging(value);
                    _Drinking = value;
                    RaisePropertyChanged("Drinking");
                    OnDrinkingChanged();
                }
            }
        }
        private bool _Drinking;
        partial void OnDrinkingChanging(bool value);
        partial void OnDrinkingChanged();

	
        [DataMemberAttribute()]
        public string DrinkingDescr
        {
            get
            {
                return _DrinkingDescr;
            }
            set
            {
                if (_DrinkingDescr != value)
                {
                    OnDrinkingDescrChanging(value);
                    _DrinkingDescr = value;
                    RaisePropertyChanged("DrinkingDescr");
                    OnDrinkingDescrChanged();
                }
            }
        }
        private string _DrinkingDescr;
        partial void OnDrinkingDescrChanging(string value);
        partial void OnDrinkingDescrChanged();

	
        [DataMemberAttribute()]
        public bool Diabetics
        {
            get
            {
                return _Diabetics;
            }
            set
            {
                if (_Diabetics != value)
                {
                    OnDiabeticsChanging(value);
                    _Diabetics = value;
                    RaisePropertyChanged("Diabetics");
                    OnDiabeticsChanged();
                }
            }
        }
        private bool _Diabetics;
        partial void OnDiabeticsChanging(bool value);
        partial void OnDiabeticsChanged();
	
        [DataMemberAttribute()]
        public string DiabeticsDescr
        {
            get
            {
                return _DiabeticsDescr;
            }
            set
            {
                if (_DiabeticsDescr != value)
                {
                    OnDiabeticsDescrChanging(value);
                    _DiabeticsDescr = value;
                    RaisePropertyChanged("DiabeticsDescr");
                    OnDiabeticsDescrChanged();
                }
            }
        }
        private string _DiabeticsDescr;
        partial void OnDiabeticsDescrChanging(string value);
        partial void OnDiabeticsDescrChanged();
	
        [DataMemberAttribute()]
        public bool Dyslipidemia
        {
            get
            {
                return _Dyslipidemia;
            }
            set
            {
                if (_Dyslipidemia != value)
                {
                    OnDyslipidemiaChanging(value);
                    _Dyslipidemia = value;
                    RaisePropertyChanged("Dyslipidemia");
                    OnDyslipidemiaChanged();
                }
            }
        }
        private bool _Dyslipidemia;
        partial void OnDyslipidemiaChanging(bool value);
        partial void OnDyslipidemiaChanged();

	
        [DataMemberAttribute()]
        public string DyslipidemiaDescr
        {
            get
            {
                return _DyslipidemiaDescr;
            }
            set
            {
                if (_DyslipidemiaDescr != value)
                {
                    OnDyslipidemiaDescrChanging(value);
                    _DyslipidemiaDescr = value;
                    RaisePropertyChanged("DyslipidemiaDescr");
                    OnDyslipidemiaDescrChanged();
                }
            }
        }
        private string _DyslipidemiaDescr;
        partial void OnDyslipidemiaDescrChanging(string value);
        partial void OnDyslipidemiaDescrChanged();
	
        [DataMemberAttribute()]
        public bool Obesity
        {
            get
            {
                return _Obesity;
            }
            set
            {
                if (_Obesity != value)
                {
                    OnObesityChanging(value);
                    _Obesity = value;
                    RaisePropertyChanged("Obesity");
                    OnObesityChanged();
                }
            }
        }
        private bool _Obesity;
        partial void OnObesityChanging(bool value);
        partial void OnObesityChanged();
	
        [DataMemberAttribute()]
        public string ObesityDescr
        {
            get
            {
                return _ObesityDescr;
            }
            set
            {
                if (_ObesityDescr != value)
                {
                    OnObesityDescrChanging(value);
                    _ObesityDescr = value;
                    RaisePropertyChanged("ObesityDescr");
                    OnObesityDescrChanged();
                }
            }
        }
        private string _ObesityDescr;
        partial void OnObesityDescrChanging(string value);
        partial void OnObesityDescrChanged();

        [DataMemberAttribute()]
        public bool Hypertension
        {
            get
            {
                return _Hypertension;
            }
            set
            {
                if (_Hypertension != value)
                {
                    OnHypertensionChanging(value);
                    _Hypertension = value;
                    RaisePropertyChanged("Hypertension");
                    OnHypertensionChanged();
                }
            }
        }
        private bool _Hypertension;
        partial void OnHypertensionChanging(bool value);
        partial void OnHypertensionChanged();
	
        [DataMemberAttribute()]
        public string HypertensionDescr
        {
            get
            {
                return _HypertensionDescr;
            }
            set
            {
                if (_HypertensionDescr != value)
                {
                    OnHypertensionDescrChanging(value);
                    _HypertensionDescr = value;
                    RaisePropertyChanged("HypertensionDescr");
                    OnHypertensionDescrChanged();
                }
            }
        }
        private string _HypertensionDescr;
        partial void OnHypertensionDescrChanging(string value);
        partial void OnHypertensionDescrChanged();
	
        [DataMemberAttribute()]
        public string Other
        {
            get
            {
                return _Other;
            }
            set
            {
                if (_Other != value)
                {
                    OnOtherChanging(value);
                    _Other = value;
                    RaisePropertyChanged("Other");
                    OnOtherChanged();
                }
            }
        }
        private string _Other;
        partial void OnOtherChanging(string value);
        partial void OnOtherChanged();

        
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

        #endregion
        private RiskFactors _tempRiskFactors;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRiskFactors = (RiskFactors)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRiskFactors)
                CopyFrom(_tempRiskFactors);
        }

        public void EndEdit()
        {
            //Dinh cap nhat thong tin o day ne
        }

        public void CopyFrom(RiskFactors p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

    }
}
