using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class ContraIndicatorDrugsRelToMedCond : NotifyChangedBase, IEditableObject
    {
        public ContraIndicatorDrugsRelToMedCond()
            : base()
        {

        }

        private ContraIndicatorDrugsRelToMedCond _tempContraIndicatorDrugsRelToMedCond;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempContraIndicatorDrugsRelToMedCond = (ContraIndicatorDrugsRelToMedCond)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempContraIndicatorDrugsRelToMedCond)
                CopyFrom(_tempContraIndicatorDrugsRelToMedCond);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ContraIndicatorDrugsRelToMedCond p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new ContraIndicatorDrugsRelToMedCond object.

        /// <param name="drugsMCTypeID">Initial value of the DrugsMCTypeID property.</param>
        /// <param name="mCTypeID">Initial value of the MCTypeID property.</param>
        /// <param name="drugID">Initial value of the DrugID property.</param>
        public static ContraIndicatorDrugsRelToMedCond CreateContraIndicatorDrugsRelToMedCond(long drugsMCTypeID, Byte mCTypeID, long drugID)
        {
            ContraIndicatorDrugsRelToMedCond contraIndicatorDrugsRelToMedCond = new ContraIndicatorDrugsRelToMedCond();
            contraIndicatorDrugsRelToMedCond.DrugsMCTypeID = drugsMCTypeID;
            contraIndicatorDrugsRelToMedCond.MCTypeID = mCTypeID;
            contraIndicatorDrugsRelToMedCond.DrugID = drugID;
            return contraIndicatorDrugsRelToMedCond;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long DrugsMCTypeID
        {
            get
            {
                return _DrugsMCTypeID;
            }
            set
            {
                if (_DrugsMCTypeID != value)
                {
                    OnDrugsMCTypeIDChanging(value);
                    _DrugsMCTypeID = value;
                    RaisePropertyChanged("DrugsMCTypeID");
                    OnDrugsMCTypeIDChanged();
                }
            }
        }
        private long _DrugsMCTypeID;
        partial void OnDrugsMCTypeIDChanging(long value);
        partial void OnDrugsMCTypeIDChanged();

        [DataMemberAttribute()]
        public int MCTypeID
        {
            get
            {
                return _MCTypeID;
            }
            set
            {
                OnMCTypeIDChanging(value);
                _MCTypeID = value;
                RaisePropertyChanged("MCTypeID");
                OnMCTypeIDChanged();
            }
        }
        private int _MCTypeID;
        partial void OnMCTypeIDChanging(int value);
        partial void OnMCTypeIDChanged();

        [DataMemberAttribute()]
        public long DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                OnDrugIDChanging(value);
                _DrugID = value;
                RaisePropertyChanged("DrugID");
                OnDrugIDChanged();
            }
        }
        private long _DrugID;
        partial void OnDrugIDChanging(long value);
        partial void OnDrugIDChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public RefGenericDrugDetail RefGenericDrugDetail
        {
            get
            {
                return _RefGenericDrugDetail;
            }
            set
            {
                OnRefGenericDrugDetailChanging(value);
                _RefGenericDrugDetail = value;
                RaisePropertyChanged("RefGenericDrugDetail");
                if (_RefGenericDrugDetail != null)
                {
                    DrugID = _RefGenericDrugDetail.DrugID;
                }
                OnRefGenericDrugDetailChanged();
            }
        }
        private RefGenericDrugDetail _RefGenericDrugDetail;
        partial void OnRefGenericDrugDetailChanging(RefGenericDrugDetail value);
        partial void OnRefGenericDrugDetailChanged();

        [DataMemberAttribute()]
        public RefMedContraIndicationTypes RefMedicalConditionType
        {
            get
            {
                return _RefMedicalConditionType;
            }
            set
            {
                OnRefMedicalConditionTypeChanging(value);
                _RefMedicalConditionType = value;
                RaisePropertyChanged("RefMedicalConditionType");
                if (_RefMedicalConditionType != null)
                {
                    MCTypeID = (int)_RefMedicalConditionType.MedContraTypeID;   
                }
                OnRefMedicalConditionTypeChanged();
            }
        }
        private RefMedContraIndicationTypes _RefMedicalConditionType;
        partial void OnRefMedicalConditionTypeChanging(RefMedContraIndicationTypes value);
        partial void OnRefMedicalConditionTypeChanged();

        [DataMemberAttribute()]
        public RefMedContraIndicationICD RefMedicalCondition
        {
            get
            {
                return _RefMedicalCondition;
            }
            set
            {
                _RefMedicalCondition = value;
                RaisePropertyChanged("RefMedicalCondition");
            }
        }
        private RefMedContraIndicationICD _RefMedicalCondition;

        [DataMemberAttribute()]
        public bool IsWarning
        {
            get
            {
                return _IsWarning;
            }
            set
            {
                _IsWarning = value;
                RaisePropertyChanged("IsWarning");
            }
        }
        private bool _IsWarning;
        #endregion
    }

    public partial class ContraIndicatorMedProductsRelToMedCond : NotifyChangedBase, IEditableObject
    {
        public ContraIndicatorMedProductsRelToMedCond()
            : base()
        {

        }

        private ContraIndicatorMedProductsRelToMedCond _tempContraIndicatorMedProductsRelToMedCond;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempContraIndicatorMedProductsRelToMedCond = (ContraIndicatorMedProductsRelToMedCond)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempContraIndicatorMedProductsRelToMedCond)
                CopyFrom(_tempContraIndicatorMedProductsRelToMedCond);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ContraIndicatorMedProductsRelToMedCond p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new ContraIndicatorMedProductsRelToMedCond object.

        /// <param name="drugsMCTypeID">Initial value of the DrugsMCTypeID property.</param>
        /// <param name="mCTypeID">Initial value of the MCTypeID property.</param>
        /// <param name="drugID">Initial value of the DrugID property.</param>
        public static ContraIndicatorMedProductsRelToMedCond CreateContraIndicatorMedProductsRelToMedCond(long drugsMCTypeID, Byte mCTypeID, long drugID)
        {
            ContraIndicatorMedProductsRelToMedCond ContraIndicatorMedProductsRelToMedCond = new ContraIndicatorMedProductsRelToMedCond();
            ContraIndicatorMedProductsRelToMedCond.MedProductsMCTypeID = drugsMCTypeID;
            ContraIndicatorMedProductsRelToMedCond.MCTypeID = mCTypeID;
            ContraIndicatorMedProductsRelToMedCond.GenMedProductID = drugID;
            return ContraIndicatorMedProductsRelToMedCond;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long MedProductsMCTypeID
        {
            get
            {
                return _MedProductsMCTypeID;
            }
            set
            {
                if (_MedProductsMCTypeID != value)
                {
                    OnMedProductsMCTypeIDChanging(value);
                    _MedProductsMCTypeID = value;
                    RaisePropertyChanged("MedProductsMCTypeID");
                    OnMedProductsMCTypeIDChanged();
                }
            }
        }
        private long _MedProductsMCTypeID;
        partial void OnMedProductsMCTypeIDChanging(long value);
        partial void OnMedProductsMCTypeIDChanged();

        [DataMemberAttribute()]
        public long MCTypeID
        {
            get
            {
                return _MCTypeID;
            }
            set
            {
                OnMCTypeIDChanging(value);
                _MCTypeID = value;
                RaisePropertyChanged("MCTypeID");
                OnMCTypeIDChanged();
            }
        }
        private long _MCTypeID;
        partial void OnMCTypeIDChanging(long value);
        partial void OnMCTypeIDChanged();

        [DataMemberAttribute()]
        public long GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                OnGenMedProductIDChanging(value);
                _GenMedProductID = value;
                RaisePropertyChanged("GenMedProductID");
                OnGenMedProductIDChanged();
            }
        }
        private long _GenMedProductID;
        partial void OnGenMedProductIDChanging(long value);
        partial void OnGenMedProductIDChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public RefGenMedProductDetails RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                OnRefGenMedProductDetailsChanging(value);
                _RefGenMedProductDetails = value;
                RaisePropertyChanged("RefGenMedProductDetails");
                if (_RefGenMedProductDetails != null)
                {
                    GenMedProductID = _RefGenMedProductDetails.GenMedProductID;
                }
                OnRefGenMedProductDetailsChanged();
            }
        }
        private RefGenMedProductDetails _RefGenMedProductDetails;
        partial void OnRefGenMedProductDetailsChanging(RefGenMedProductDetails value);
        partial void OnRefGenMedProductDetailsChanged();

        [DataMemberAttribute()]
        public RefMedContraIndicationTypes RefMedicalConditionType
        {
            get
            {
                return _RefMedicalConditionType;
            }
            set
            {
                OnRefMedicalConditionTypeChanging(value);
                _RefMedicalConditionType = value;
                RaisePropertyChanged("RefMedicalConditionType");
                if (_RefMedicalConditionType != null)
                {
                    MCTypeID = _RefMedicalConditionType.MedContraTypeID;
                }
                OnRefMedicalConditionTypeChanged();
            }
        }
        private RefMedContraIndicationTypes _RefMedicalConditionType;
        partial void OnRefMedicalConditionTypeChanging(RefMedContraIndicationTypes value);
        partial void OnRefMedicalConditionTypeChanged();

        [DataMemberAttribute()]
        public bool IsWarning
        {
            get
            {
                return _IsWarning;
            }
            set
            {
                _IsWarning = value;
                RaisePropertyChanged("IsWarning");
            }
        }
        private bool _IsWarning;
        #endregion
    }

    public partial class MedProductContraIndicatorRelToMedCond : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public long DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                _DrugID = value;
                RaisePropertyChanged("DrugID");
            }
        }
        private long _DrugID;

        [DataMemberAttribute()]
        public String BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                _BrandName = value;
                RaisePropertyChanged("BrandName");
            }
        }
        private String _BrandName;

        [DataMemberAttribute()]
        public bool IsWarning
        {
            get
            {
                return _IsWarning;
            }
            set
            {
                _IsWarning = value;
                RaisePropertyChanged("IsWarning");
            }
        }
        private bool _IsWarning;

        [DataMemberAttribute()]
        public RefMedContraIndicationICD RefMedicalCondition
        {
            get
            {
                return _RefMedicalCondition;
            }
            set
            {
                _RefMedicalCondition = value;
                RaisePropertyChanged("RefMedicalCondition");
            }
        }
        private RefMedContraIndicationICD _RefMedicalCondition;

        [DataMemberAttribute()]
        public RefMedContraIndicationTypes RefMedicalConditionType
        {
            get
            {
                return _RefMedicalConditionType;
            }
            set
            {
                _RefMedicalConditionType = value;
                RaisePropertyChanged("RefMedicalConditionType");
            }
        }
        private RefMedContraIndicationTypes _RefMedicalConditionType;

    }
    public partial class ContraAndLstICDs : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public long MedConTraTypeID
        {
            get
            {
                return _MedConTraTypeID;
            }
            set
            {
                _MedConTraTypeID = value;
                RaisePropertyChanged("MedConTraTypeID");
            }
        }
        private long _MedConTraTypeID;

        [DataMemberAttribute()]
        public int? AgeFrom
        {
            get
            {
                return _AgeFrom;
            }
            set
            {
                _AgeFrom = value;
                RaisePropertyChanged("AgeFrom");
            }
        }
        private int? _AgeFrom;

        [DataMemberAttribute()]
        public int? AgeTo
        {
            get
            {
                return _AgeTo;
            }
            set
            {
                _AgeTo = value;
                RaisePropertyChanged("AgeTo");
            }
        }
        private int? _AgeTo;

        private long _V_AgeUnit;
        [DataMemberAttribute]
        public long V_AgeUnit
        {
            get
            {
                return _V_AgeUnit;
            }
            set
            {
                if (_V_AgeUnit == value)
                {
                    return;
                }
                _V_AgeUnit = value;
                RaisePropertyChanged("V_AgeUnit");
            }
        }

        [DataMemberAttribute()]
        public bool IsWarning
        {
            get
            {
                return _IsWarning;
            }
            set
            {
                _IsWarning = value;
                RaisePropertyChanged("IsWarning");
            }
        }
        private bool _IsWarning;

        [DataMemberAttribute()]
        public List<String> ListICD10Code
        {
            get
            {
                return _ListICD10Code;
            }
            set
            {
                _ListICD10Code = value;
                RaisePropertyChanged("ListICD10Code");
            }
        }
        private List<String> _ListICD10Code;

    }

    public partial class DrugAndConTra : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public long DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                _DrugID = value;
                RaisePropertyChanged("DrugID");
            }
        }
        private long _DrugID;

        [DataMemberAttribute()]
        public String BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                _BrandName = value;
                RaisePropertyChanged("BrandName");
            }
        }
        private String _BrandName;

        [DataMemberAttribute()]
        public List<ContraAndLstICDs> ListConTraAndLstICDs
        {
            get
            {
                return _ListConTraAndLstICDs;
            }
            set
            {
                _ListConTraAndLstICDs = value;
                RaisePropertyChanged("ListConTraAndLstICDs");
            }
        }
        private List<ContraAndLstICDs> _ListConTraAndLstICDs;
    }
}
