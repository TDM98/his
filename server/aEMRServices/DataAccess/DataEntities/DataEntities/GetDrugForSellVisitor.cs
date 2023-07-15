
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class GetDrugForSellVisitor: NotifyChangedBase
    {
        public GetDrugForSellVisitor()
        {
        }
        #region Factory Method
        /// Create a new GetDrugForSellVisitor object.
        public static GetDrugForSellVisitor CreateGetDrugForSellVisitor(int Remaining, long DrugID, int RequiredNumber)
        {
            GetDrugForSellVisitor GetDrugForSellVisitor = new GetDrugForSellVisitor();
            GetDrugForSellVisitor.Remaining = Remaining;
            GetDrugForSellVisitor.DrugID = DrugID;
            GetDrugForSellVisitor.RequiredNumber = RequiredNumber;
            return GetDrugForSellVisitor;
        }
        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public int Remaining
        {
            get
            {
                return _remaining;
            }
            set
            {
                OnRemainingChanging(value);
                _remaining = value;
                RaisePropertyChanged("Remaining");
                OnRemainingChanged();
            }
        }
        private int _remaining;
        partial void OnRemainingChanging(int value);
        partial void OnRemainingChanged();

        [DataMemberAttribute()]
        public double RequiredNumber
        {
            get
            {
                return _requiredNumber;
            }
            set
            {
                OnRequiredNumberChanging(value);
                _requiredNumber = value;
                RaisePropertyChanged("RequiredNumber");
                OnRequiredNumberChanged();
            }
        }
        private double _requiredNumber;
        partial void OnRequiredNumberChanging(double value);
        partial void OnRequiredNumberChanged();
       
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

        [DataMemberAttribute()]
        public String BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                OnBrandNameChanging(value);
                _BrandName = value;
                RaisePropertyChanged("BrandName");
                OnBrandNameChanged();
            }
        }
        private String _BrandName;
        partial void OnBrandNameChanging(String value);
        partial void OnBrandNameChanged();

        [DataMemberAttribute()]
        public long GenericID
        {
            get
            {
                return _GenericID;
            }
            set
            {
                if (_GenericID != value)
                {
                    _GenericID = value;
                    RaisePropertyChanged("GenericID");
                }
            }
        }
        private long _GenericID;

        [DataMemberAttribute()]
        public String GenericName
        {
            get
            {
                return _GenericName;
            }
            set
            {
                OnGenericNameChanging(value);
                _GenericName = value;
                RaisePropertyChanged("GenericName");
                OnGenericNameChanged();
            }
        }
        private String _GenericName;
        partial void OnGenericNameChanging(String value);
        partial void OnGenericNameChanged();

        [DataMemberAttribute()]
        public String InBatchNumber
        {
            get
            {
                return _InBatchNumber;
            }
            set
            {
                OnInBatchNumberChanging(value);
                _InBatchNumber = value;
                RaisePropertyChanged("InBatchNumber");
                OnInBatchNumberChanged();
            }
        }
        private String _InBatchNumber;
        partial void OnInBatchNumberChanging(String value);
        partial void OnInBatchNumberChanged();
     
        [DataMemberAttribute()]
        public String UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                OnUnitNameChanging(value);
                _UnitName = value;
                RaisePropertyChanged("UnitName");
                OnUnitNameChanged();
            }
        }
        private String _UnitName;
        partial void OnUnitNameChanging(String value);
        partial void OnUnitNameChanged();

        [DataMemberAttribute()]
        public Decimal SellingPrice
        {
            get
            {
                return _SellingPrice;
            }
            set
            {
                OnSellingPriceChanging(value);
                _SellingPrice = value;
                RaisePropertyChanged("SellingPrice");
                OnSellingPriceChanged();
            }
        }
        private Decimal _SellingPrice;
        partial void OnSellingPriceChanging(Decimal value);
        partial void OnSellingPriceChanged();

        [DataMemberAttribute()]
        public Decimal InCost
        {
            get
            {
                return _InCost;
            }
            set
            {
                _InCost = value;
                RaisePropertyChanged("InCost");
            }
        }
        private Decimal _InCost;

        [DataMemberAttribute()]
        public Decimal OutPrice
        {
            get
            {
                return _OutPrice;
            }
            set
            {
                _OutPrice = value;
                RaisePropertyChanged("OutPrice");
            }
        }
        private Decimal _OutPrice;

        #region Sell Price Member
        [DataMemberAttribute()]
        public decimal NormalPrice
        {
            get
            {
                return _NormalPrice;
            }
            set
            {
                if (_NormalPrice != value)
                {
                    _NormalPrice = value;
                    RaisePropertyChanged("NormalPrice");
                }
            }
        }
        private decimal _NormalPrice;

        [DataMemberAttribute()]
        public decimal PriceForHIPatient
        {
            get
            {
                return _PriceForHIPatient;
            }
            set
            {
                if (_PriceForHIPatient != value)
                {
                    _PriceForHIPatient = value;
                    RaisePropertyChanged("PriceForHIPatient");
                }
            }
        }
        private decimal _PriceForHIPatient;

        //cot nay thay doi tuy theo gia tri truyen vao
        [DataMemberAttribute()]
        public Decimal HIAllowedPrice
        {
            get
            {
                return _HIAllowedPrice;
            }
            set
            {
                _HIAllowedPrice = value;
                RaisePropertyChanged("HIAllowedPrice");
            }
        }
        private Decimal _HIAllowedPrice;

        //cot nay co dinh vi lay trong bang gia ra,khong dung de luu
        [DataMemberAttribute()]
        public Decimal HIAllowedPriceNoChange
        {
            get
            {
                return _HIAllowedPriceNoChange;
            }
            set
            {
                _HIAllowedPriceNoChange = value;
                RaisePropertyChanged("HIAllowedPriceNoChange");
            }
        }
        private Decimal _HIAllowedPriceNoChange;
        #endregion

        [DataMemberAttribute()]
        public long STT
        {
            get
            {
                return _STT;
            }
            set
            {
                OnSTTChanging(value);
                _STT = value;
                RaisePropertyChanged("STT");
                OnSTTChanged();
            }
        }
        private long _STT;
        partial void OnSTTChanging(long value);
        partial void OnSTTChanged();

        [DataMemberAttribute()]
        public long InID
        {
            get
            {
                return _InID;
            }
            set
            {
                OnInIDChanging(value);
                _InID = value;
                RaisePropertyChanged("InID");
                OnInIDChanged();
            }
        }
        private long _InID;
        partial void OnInIDChanging(long value);
        partial void OnInIDChanged();


        [DataMemberAttribute()]
        public String UnitUse
        {
            get
            {
                return _UnitUse;
            }
            set
            {
                _UnitUse = value;
                RaisePropertyChanged("UnitUse");
            }
        }
        private String _UnitUse;


        [DataMemberAttribute()]
        public String Dosage
        {
            get
            {
                return _Dosage;
            }
            set
            {
                OnDosageChanging(value);
                _Dosage = value;
                RaisePropertyChanged("Dosage");
                OnDosageChanged();
            }
        }
        private String _Dosage;
        partial void OnDosageChanging(String value);
        partial void OnDosageChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> InsuranceCover
        {
            get
            {
                return _InsuranceCover;
            }
            set
            {
                OnInsuranceCoverChanging(value);
                _InsuranceCover = value;
                RaisePropertyChanged("InsuranceCover");
                OnInsuranceCoverChanged();
            }
        }
        private Nullable<Boolean> _InsuranceCover;
        partial void OnInsuranceCoverChanging(Nullable<Boolean> value);
        partial void OnInsuranceCoverChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsConsult
        {
            get
            {
                return _IsConsult;
            }
            set
            {
                OnIsConsultChanging(value);
                _IsConsult = value;
                RaisePropertyChanged("IsConsult");
                OnIsConsultChanged();
            }
        }
        private Nullable<Boolean> _IsConsult;
        partial void OnIsConsultChanging(Nullable<Boolean> value);
        partial void OnIsConsultChanged();


        [DataMemberAttribute()]
        public Double DispenseVolume
        {
            get
            {
                return _DispenseVolume;
            }
            set
            {
                OnDispenseVolumeChanging(value);
                _DispenseVolume = value;
                RaisePropertyChanged("DispenseVolume");
                OnDispenseVolumeChanged();
            }
        }
        private Double _DispenseVolume;
        partial void OnDispenseVolumeChanging(Double value);
        partial void OnDispenseVolumeChanged();


        [DataMemberAttribute()]
        public Double UnitVolume
        {
            get
            {
                return _UnitVolume;
            }
            set
            {
                OnUnitVolumeChanging(value);
                _UnitVolume = value;
                RaisePropertyChanged("UnitVolume");
                OnUnitVolumeChanged();
            }
        }
        private Double _UnitVolume;
        partial void OnUnitVolumeChanging(Double value);
        partial void OnUnitVolumeChanged();

        [DataMemberAttribute()]
        public Int16? MaxDayPrescribed
        {
            get
            {
                return _MaxDayPrescribed;
            }
            set
            {
                _MaxDayPrescribed = value;
                RaisePropertyChanged("MaxDayPrescribed");
            }
        }
        private Int16? _MaxDayPrescribed;


        [DataMemberAttribute()]
        public bool IsSearchByGenName
        {
            get
            {
                return _isSearchByGenName;
            }
            set
            {
                OnIsSearchByGenNameChanging(value);
                _isSearchByGenName = value;
                RaisePropertyChanged("IsSearchByGenName");
                OnIsSearchByGenNameChanged();
            }
        }
        private bool _isSearchByGenName;
        partial void OnIsSearchByGenNameChanging(bool value);
        partial void OnIsSearchByGenNameChanged();

        
        [DataMemberAttribute()]
        public double? HIPaymentPercent
        {
            get
            {
                return _HIPaymentPercent;
            }
            set
            {
                _HIPaymentPercent = value;
                RaisePropertyChanged("HIPaymentPercent");
            }
        }
        private double? _HIPaymentPercent;

        [DataMemberAttribute()]
        public int HIRemaining
        {
            get
            {
                return _HIRemaining;
            }
            set
            {
                _HIRemaining = value;
                RaisePropertyChanged("HIRemaining");
            }
        }
        private int _HIRemaining;

        [DataMemberAttribute()]
        public String Indication
        {
            get
            {
                return _Indication;
            }
            set
            {
                _Indication = value;
                RaisePropertyChanged("Indication");
            }
        }
        private String _Indication;

        [DataMemberAttribute()]
        public long DrugClassID
        {
            get
            {
                return _DrugClassID;
            }
            set
            {
                if (_DrugClassID != value)
                {
                    _DrugClassID = value;
                    RaisePropertyChanged("DrugClassID");
                }
            }
        }
        private long _DrugClassID;

        [DataMemberAttribute()]
        public string DrugClassName
        {
            get
            {
                return _DrugClassName;
            }
            set
            {
                if (_DrugClassName != value)
                {
                    _DrugClassName = value;
                    RaisePropertyChanged("DrugClassName");
                }
            }
        }
        private string _DrugClassName;

        [DataMemberAttribute()]
        public decimal? VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                if (_VAT != value)
                {
                    _VAT = value;
                    RaisePropertyChanged("VAT");
                }
            }
        }
        private decimal? _VAT;
        private bool _IsNotVat;
        [DataMemberAttribute()]
        public bool IsNotVat
        {
            get
            {
                return _IsNotVat;
            }
            set
            {
                if (_IsNotVat != value)
                {
                    _IsNotVat = value;
                    RaisePropertyChanged("IsNotVat");
                }
            }
        }

        private long? _DrugVersionID;
        [DataMemberAttribute()]
        public long? DrugVersionID
        {
            get
            {
                return _DrugVersionID;
            }
            set
            {
                if (_DrugVersionID != value)
                {
                    _DrugVersionID = value;
                    RaisePropertyChanged("DrugVersionID");
                }
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            GetDrugForSellVisitor seletedStore = obj as GetDrugForSellVisitor;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.DrugID == seletedStore.DrugID && this.BrandName == seletedStore.BrandName
                && this.BrandName!=null && seletedStore.BrandName!=null;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    public partial class RefGenericDrugSimple : NotifyChangedBase
    {
        public RefGenericDrugSimple()
        {
        }
        #region Factory Method
        /// Create a new GetDrugForSellVisitor object.
        public static RefGenericDrugSimple CreateRefGenericDrugSimple(long DrugID)
        {
            RefGenericDrugSimple GetDrugForSellVisitor = new RefGenericDrugSimple();
            GetDrugForSellVisitor.DrugID = DrugID;
            return GetDrugForSellVisitor;
        }
        #endregion

        #region Primitive Properties
      
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

        [DataMemberAttribute()]
        public String DrugCode
        {
            get
            {
                return _DrugCode;
            }
            set
            {
                _DrugCode = value;
                RaisePropertyChanged("DrugCode");
            }
        }
        private String _DrugCode;

        [DataMemberAttribute()]
        public String BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                OnBrandNameChanging(value);
                _BrandName = value;
                RaisePropertyChanged("BrandName");
                OnBrandNameChanged();
            }
        }
        private String _BrandName;
        partial void OnBrandNameChanging(String value);
        partial void OnBrandNameChanged();

        [DataMemberAttribute()]
        public String GenericName
        {
            get
            {
                return _GenericName;
            }
            set
            {
                _GenericName = value;
                RaisePropertyChanged("GenericName");
            }
        }
        private String _GenericName;

        [DataMemberAttribute()]
        public String Content
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
                RaisePropertyChanged("Content");
            }
        }
        private String _Content;

        [DataMemberAttribute()]
        public String UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                OnUnitNameChanging(value);
                _UnitName = value;
                RaisePropertyChanged("UnitName");
                OnUnitNameChanged();
            }
        }
        private String _UnitName;
        partial void OnUnitNameChanging(String value);
        partial void OnUnitNameChanged();

        #endregion

        public override bool Equals(object obj)
        {
            RefGenericDrugSimple seletedStore = obj as RefGenericDrugSimple;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.DrugID == seletedStore.DrugID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
