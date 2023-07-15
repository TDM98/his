using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;

/*
 * 20170524 #001 TNHX: Merge Pharmacy&DrugStore
 * 20180728 #002 TTM: Add New Property: V_CatDrugType
 */

namespace DataEntities
{
    public partial class RefGenericDrugDetail : NotifyChangedBase, IEditableObject
    {
        #region Factory Method


        /// Create a new RefGenericDrugDetail object.

        /// <param name="drugID">Initial value of the DrugID property.</param>
        /// <param name="brandName">Initial value of the BrandName property.</param>
        /// <param name="genericName">Initial value of the GenericName property.</param>
        public static RefGenericDrugDetail CreateRefGenericDrugDetail(long drugID, String brandName, String genericName)
        {
            RefGenericDrugDetail refGenericDrugDetail = new RefGenericDrugDetail();
            refGenericDrugDetail.DrugID = drugID;
            refGenericDrugDetail.BrandName = brandName;
            refGenericDrugDetail.GenericName = genericName;
            return refGenericDrugDetail;
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
                if (_DrugID != value)
                {
                    OnDrugIDChanging(value);
                    _DrugID = value;
                    RaisePropertyChanged("DrugID");
                    OnDrugIDChanged();
                }
            }
        }
        private long _DrugID;
        partial void OnDrugIDChanging(long value);
        partial void OnDrugIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> CountryID
        {
            get
            {
                return _CountryID;
            }
            set
            {
                OnCountryIDChanging(value);
                _CountryID = value;
                RaisePropertyChanged("CountryID");
                OnCountryIDChanged();
            }
        }
        private Nullable<long> _CountryID;
        partial void OnCountryIDChanging(Nullable<long> value);
        partial void OnCountryIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> DrugClassID
        {
            get
            {
                return _DrugClassID;
            }
            set
            {
                OnDrugClassIDChanging(value);
                _DrugClassID = value;
                RaisePropertyChanged("DrugClassID");
                OnDrugClassIDChanged();
            }
        }
        private Nullable<long> _DrugClassID;
        partial void OnDrugClassIDChanging(Nullable<long> value);
        partial void OnDrugClassIDChanged();

        [Required(ErrorMessage = "Bạn phải nhập tên thương mại!")]
        [StringLength(128, ErrorMessage = "Tên thương mại quá dài!")]
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
                ValidateProperty("BrandName", value);
                _BrandName = value;
                RaisePropertyChanged("BrandName");
                OnBrandNameChanged();
            }
        }
        private String _BrandName;
        partial void OnBrandNameChanging(String value);
        partial void OnBrandNameChanged();

        //[Required(ErrorMessage = "Bạn phải nhập tên chung!")]
        //[StringLength(128, ErrorMessage = "Tên chung quá dài!")]
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
                ValidateProperty("GenericName", value);
                _GenericName = value;
                RaisePropertyChanged("GenericName");
                OnGenericNameChanged();
            }
        }
        private String _GenericName;
        partial void OnGenericNameChanging(String value);
        partial void OnGenericNameChanged();

        [DataMemberAttribute()]
        public Nullable<long> PCOID
        {
            get
            {
                return _PCOID;
            }
            set
            {
                _PCOID = value;
                RaisePropertyChanged("PCOID");
            }
        }
        private Nullable<long> _PCOID;


        [DataMemberAttribute()]
        public String SdlDescription
        {
            get
            {
                return _SdlDescription;
            }
            set
            {
                OnSdlDescriptionChanging(value);
                _SdlDescription = value;
                RaisePropertyChanged("SdlDescription");
                OnSdlDescriptionChanged();
            }
        }
        private String _SdlDescription;
        partial void OnSdlDescriptionChanging(String value);
        partial void OnSdlDescriptionChanged();

        [DataMemberAttribute()]
        public String Composition
        {
            get
            {
                return _Composition;
            }
            set
            {
                OnCompositionChanging(value);
                _Composition = value;
                RaisePropertyChanged("Composition");
                OnCompositionChanged();
            }
        }
        private String _Composition;
        partial void OnCompositionChanging(String value);
        partial void OnCompositionChanged();

        [DataMemberAttribute()]
        public String ActiveIngredient
        {
            get
            {
                return _ActiveIngredient;
            }
            set
            {
                OnActiveIngredientChanging(value);
                _ActiveIngredient = value;
                RaisePropertyChanged("ActiveIngredient");
                OnActiveIngredientChanged();
            }
        }
        private String _ActiveIngredient;
        partial void OnActiveIngredientChanging(String value);
        partial void OnActiveIngredientChanged();

        [DataMemberAttribute()]
        public String Indication
        {
            get
            {
                return _Indication;
            }
            set
            {
                OnIndicationChanging(value);
                _Indication = value;
                RaisePropertyChanged("Indication");
                OnIndicationChanged();
            }
        }
        private String _Indication;
        partial void OnIndicationChanging(String value);
        partial void OnIndicationChanged();

        [DataMemberAttribute()]
        public String Contraindication
        {
            get
            {
                return _Contraindication;
            }
            set
            {
                OnContraindicationChanging(value);
                _Contraindication = value;
                RaisePropertyChanged("Contraindication");
                OnContraindicationChanged();
            }
        }
        private String _Contraindication;
        partial void OnContraindicationChanging(String value);
        partial void OnContraindicationChanged();

        [DataMemberAttribute()]
        public String Content
        {
            get
            {
                return _Content;
            }
            set
            {
                OnContentChanging(value);
                _Content = value;
                RaisePropertyChanged("Content");
                OnContentChanged();
            }
        }
        private String _Content;
        partial void OnContentChanging(String value);
        partial void OnContentChanged();

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

        [Required(ErrorMessage = "Bạn phải nhập cách dùng!")]
        [DataMemberAttribute()]
        public String Administration
        {
            get
            {
                return _Administration;
            }
            set
            {
                OnAdministrationChanging(value);
                ValidateProperty("Administration", value);
                _Administration = value;
                RaisePropertyChanged("Administration");
                OnAdministrationChanged();
            }
        }
        private String _Administration;
        partial void OnAdministrationChanging(String value);
        partial void OnAdministrationChanged();


        [DataMemberAttribute()]
        public String Precaution_Warn
        {
            get
            {
                return _Precaution_Warn;
            }
            set
            {
                OnPrecaution_WarnChanging(value);
                _Precaution_Warn = value;
                RaisePropertyChanged("Precaution_Warn");
                OnPrecaution_WarnChanged();
            }
        }
        private String _Precaution_Warn;
        partial void OnPrecaution_WarnChanging(String value);
        partial void OnPrecaution_WarnChanged();

        [DataMemberAttribute()]
        public String SideEffects
        {
            get
            {
                return _SideEffects;
            }
            set
            {
                OnSideEffectsChanging(value);
                _SideEffects = value;
                RaisePropertyChanged("SideEffects");
                OnSideEffectsChanged();
            }
        }
        private String _SideEffects;
        partial void OnSideEffectsChanging(String value);
        partial void OnSideEffectsChanged();

        [DataMemberAttribute()]
        public String Interaction
        {
            get
            {
                return _Interaction;
            }
            set
            {
                OnInteractionChanging(value);
                _Interaction = value;
                RaisePropertyChanged("Interaction");
                OnInteractionChanged();
            }
        }
        private String _Interaction;
        partial void OnInteractionChanging(String value);
        partial void OnInteractionChanged();

        [DataMemberAttribute()]
        public Nullable<Int32> AdvTimeBeforeExpire
        {
            get
            {
                return _AdvTimeBeforeExpire;
            }
            set
            {
                OnAdvTimeBeforeExpireChanging(value);
                _AdvTimeBeforeExpire = value;
                RaisePropertyChanged("AdvTimeBeforeExpire");
                OnAdvTimeBeforeExpireChanged();
            }
        }
        private Nullable<Int32> _AdvTimeBeforeExpire;
        partial void OnAdvTimeBeforeExpireChanging(Nullable<Int32> value);
        partial void OnAdvTimeBeforeExpireChanged();

        [DataMemberAttribute()]
        public String OutNotes
        {
            get
            {
                return _OutNotes;
            }
            set
            {
                OnOutNotesChanging(value);
                _OutNotes = value;
                RaisePropertyChanged("OutNotes");
                OnOutNotesChanged();
            }
        }
        private String _OutNotes;
        partial void OnOutNotesChanging(String value);
        partial void OnOutNotesChanged();

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
        public Nullable<Boolean> DIsActive
        {
            get
            {
                return _DIsActive;
            }
            set
            {
                OnDIsActiveChanging(value);
                _DIsActive = value;
                RaisePropertyChanged("DIsActive");
                OnDIsActiveChanged();
            }
        }
        private Nullable<Boolean> _DIsActive;
        partial void OnDIsActiveChanging(Nullable<Boolean> value);
        partial void OnDIsActiveChanged();

        //KMx: Biến dùng để ẩn thuốc, không chổ nào được thấy.
        [DataMemberAttribute()]
        public Nullable<Boolean> IsNotShow
        {
            get
            {
                return _IsNotShow;
            }
            set
            {
                OnIsNotShowChanging(value);
                _IsNotShow = value;
                RaisePropertyChanged("IsNotShow");
                OnIsNotShowChanged();
            }
        }
        private Nullable<Boolean> _IsNotShow;
        partial void OnIsNotShowChanging(Nullable<Boolean> value);
        partial void OnIsNotShowChanged();


        //KMx: Biến dùng để hiện cảnh báo đối với BN BHYT (03/06/2014 11:20).
        [DataMemberAttribute()]
        public bool IsWarningHI
        {
            get
            {
                return _IsWarningHI;
            }
            set
            {
                OnIsWarningHIChanging(value);
                _IsWarningHI = value;
                RaisePropertyChanged("IsWarningHI");
                OnIsWarningHIChanged();
            }
        }
        private bool _IsWarningHI;
        partial void OnIsWarningHIChanging(bool value);
        partial void OnIsWarningHIChanged();


        [Required(ErrorMessage = "Bạn phải nhập Quy Cách Đóng Gói!")]
        [DataMemberAttribute()]
        public string Packaging
        {
            get
            {
                return _Packaging;
            }
            set
            {
                ValidateProperty("Packaging", value);
                _Packaging = value;
                RaisePropertyChanged("Packaging");
            }
        }
        private string _Packaging;


        [DataMemberAttribute()]
        public string Visa
        {
            get
            {
                return _Visa;
            }
            set
            {
                _Visa = value;
                RaisePropertyChanged("Visa");
            }
        }
        private string _Visa;

        [DataMemberAttribute()]
        public string HIDrugCode5084
        {
            get
            {
                return _HIDrugCode5084;
            }
            set
            {
                //  ValidateProperty("DrugCode", value);
                _HIDrugCode5084 = value;
                RaisePropertyChanged("HIDrugCode5084");
            }
        }
        private string _HIDrugCode5084;

        //[Required(ErrorMessage = "Bạn phải nhập Code!")]
        [DataMemberAttribute()]
        public string DrugCode
        {
            get
            {
                return _DrugCode;
            }
            set
            {
                //  ValidateProperty("DrugCode", value);
                _DrugCode = value;
                RaisePropertyChanged("DrugCode");
            }
        }
        private string _DrugCode;

        [DataMemberAttribute()]
        public string HIDrugCode
        {
            get
            {
                return _HIDrugCode;
            }
            set
            {
                _HIDrugCode = value;
                RaisePropertyChanged("HIDrugCode");
            }
        }
        private string _HIDrugCode;

        [DataMemberAttribute()]
        public Nullable<long> UnitID
        {
            get
            {
                return _UnitID;
            }
            set
            {
                _UnitID = value;
                RaisePropertyChanged("UnitID");
            }
        }
        private Nullable<long> _UnitID;


        [DataMemberAttribute()]
        public Nullable<long> UnitUseID
        {
            get
            {
                return _UnitUseID;
            }
            set
            {
                _UnitUseID = value;
                RaisePropertyChanged("UnitUseID");
            }
        }
        private Nullable<long> _UnitUseID;


        [Required(ErrorMessage = "Bạn phải nhập Hệ Số Nhân!")]
        [Range(1, 9999999, ErrorMessage = "Hệ Số Nhân phải > 1")]
        [DataMemberAttribute()]
        private double _NumberOfEstimatedMonths_F;
        public double NumberOfEstimatedMonths_F
        {
            get
            {
                return _NumberOfEstimatedMonths_F;
            }
            set
            {
                //ValidateProperty("NumberOfEstimatedMonths_F", value);
                _NumberOfEstimatedMonths_F = value;
                RaisePropertyChanged("NumberOfEstimatedMonths_F");
            }
        }

        [Required(ErrorMessage = "Bạn phải nhập Số Lượng theo Quy Cách!")]
        [Range(1, 9999999, ErrorMessage = "Số lượng theo Quy Cách phải > 1")]
        [DataMemberAttribute()]
        private Nullable<int> _UnitPackaging;
        public Nullable<int> UnitPackaging
        {
            get
            {
                return _UnitPackaging;
            }
            set
            {
                OnUnitPackagingChanging(value);
                ValidateProperty("UnitPackaging", value);
                _UnitPackaging = value;
                RaisePropertyChanged("UnitPackaging");
                OnUnitPackagingChanged();
            }
        }
        partial void OnUnitPackagingChanging(Nullable<int> value);
        partial void OnUnitPackagingChanged();

        [Required(ErrorMessage = "Bạn phải nhập Hệ Số Nhân!")]
        [Range(1, 9999999, ErrorMessage = "Hệ Số Nhân phải > 1")]
        [DataMemberAttribute()]
        private int _FactorSafety;
        public int FactorSafety
        {
            get
            {
                return _FactorSafety;
            }
            set
            {
                ValidateProperty("FactorSafety", value);
                _FactorSafety = value;
                RaisePropertyChanged("FactorSafety");
            }
        }

        [DataMemberAttribute()]
        private string _ActiveIngredientCode;
        public string ActiveIngredientCode
        {
            get
            {
                return _ActiveIngredientCode;
            }
            set
            {
                _ActiveIngredientCode = value;
                RaisePropertyChanged("ActiveIngredientCode");
            }
        }

        [DataMemberAttribute()]
        private string _ProductCodeRefNum;
        public string ProductCodeRefNum
        {
            get
            {
                return _ProductCodeRefNum;
            }
            set
            {
                _ProductCodeRefNum = value;
                RaisePropertyChanged("ProductCodeRefNum");
            }
        }

        [DataMemberAttribute()]
        private long? _HosID;
        public long? HosID
        {
            get
            {
                return _HosID;
            }
            set
            {
                _HosID = value;
                RaisePropertyChanged("HosID");
            }
        }
        //▼===== #002
        [DataMemberAttribute()]
        private long? _V_CatDrugType;
        public long? V_CatDrugType
        {
            get
            {
                return _V_CatDrugType;
            }
            set
            {
                _V_CatDrugType = value;
                RaisePropertyChanged("V_CatDrugType");
            }
        }
        //▲===== #002
        [DataMemberAttribute()]
        private long? _RefGenDrugCatID_1;
        public long? RefGenDrugCatID_1
        {
            get
            {
                return _RefGenDrugCatID_1;
            }
            set
            {
                _RefGenDrugCatID_1 = value;
                RaisePropertyChanged("RefGenDrugCatID_1");
            }
        }

        [DataMemberAttribute()]
        private long? _RefPharmacyDrugCatID = 0;
        public long? RefPharmacyDrugCatID
        {
            get
            {
                return _RefPharmacyDrugCatID;
            }
            set
            {
                _RefPharmacyDrugCatID = value;
                RaisePropertyChanged("RefPharmacyDrugCatID");
            }
        }

        [DataMemberAttribute()]
        private long? _RefGenDrugBHYT_CatID;
        public long? RefGenDrugBHYT_CatID
        {
            get
            {
                return _RefGenDrugBHYT_CatID;
            }
            set
            {
                _RefGenDrugBHYT_CatID = value;
                RaisePropertyChanged("RefGenDrugBHYT_CatID");
            }
        }

        [Required(ErrorMessage = "Nhập DispenseVolume!")]
        [Range(0, 99999999999.0, ErrorMessage = "DispenseVolume > 0")]
        [DataMemberAttribute()]
        public double DispenseVolume
        {
            get
            {
                return _DispenseVolume;
            }
            set
            {
                ValidateProperty("DispenseVolume", value);
                _DispenseVolume = value;
                RaisePropertyChanged("DispenseVolume");
            }
        }
        private double _DispenseVolume = 0;


        [DataMemberAttribute()]
        public Nullable<Boolean> KeepRefrigerated
        {
            get
            {
                return _KeepRefrigerated;
            }
            set
            {
                OnKeepRefrigeratedChanging(value);
                _KeepRefrigerated = value;
                RaisePropertyChanged("KeepRefrigerated");
                OnKeepRefrigeratedChanged();
            }
        }
        private Nullable<Boolean> _KeepRefrigerated;
        partial void OnKeepRefrigeratedChanging(Nullable<Boolean> value);
        partial void OnKeepRefrigeratedChanged();

        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        private long _StaffID;

        [Range(0, 9999999, ErrorMessage = "Số ngày ra toa tối đa không hợp lệ")]
        [DataMemberAttribute()]
        public Int16? MaxDayPrescribed
        {
            get
            {
                return _MaxDayPrescribed;
            }
            set
            {
                ValidateProperty("MaxDayPrescribed", value);
                _MaxDayPrescribed = value;
                RaisePropertyChanged("MaxDayPrescribed");
            }
        }
        private Int16? _MaxDayPrescribed;


        [DataMemberAttribute()]
        public string ReportBrandName
        {
            get
            {
                return _ReportBrandName;
            }
            set
            {
                _ReportBrandName = value;
                RaisePropertyChanged("ReportBrandName");
            }
        }
        private string _ReportBrandName;


        [DataMemberAttribute()]
        public Lookup RouteOfAdministration
        {
            get
            {
                return _RouteOfAdministration;
            }
            set
            {
                _RouteOfAdministration = value;
                RaisePropertyChanged("RouteOfAdministration");
            }
        }
        private Lookup _RouteOfAdministration;

        [DataMemberAttribute()]
        public Lookup CatDrugType
        {
            get
            {
                return _CatDrugType;
            }
            set
            {
                _CatDrugType = value;
                RaisePropertyChanged("CatDrugType");
            }
        }
        private Lookup _CatDrugType;


        [DataMemberAttribute()]
        public string DosageForm
        {
            get
            {
                return _DosageForm;
            }
            set
            {
                _DosageForm = value;
                RaisePropertyChanged("DosageForm");
            }
        }
        private string _DosageForm;


        [DataMemberAttribute()]
        public string DrugForm
        {
            get
            {
                return _DrugForm;
            }
            set
            {
                _DrugForm = value;
                RaisePropertyChanged("DrugForm");
            }
        }
        private string _DrugForm;

        [DataMemberAttribute()]
        public long V_VENType
        {
            get
            {
                return _V_VENType;
            }
            set
            {
                _V_VENType = value;
                RaisePropertyChanged("V_VENType");
            }
        }
        private long _V_VENType;


        [DataMemberAttribute()]
        public long V_GroupTypeForReport20
        {
            get
            {
                return _V_GroupTypeForReport20;
            }
            set
            {
                _V_GroupTypeForReport20 = value;
                RaisePropertyChanged("V_GroupTypeForReport20");
            }
        }
        private long _V_GroupTypeForReport20;

        [DataMemberAttribute()]
        public string TCKTAndTCCNGroup
        {
            get
            {
                return _TCKTAndTCCNGroup;
            }
            set
            {
                _TCKTAndTCCNGroup = value;
                RaisePropertyChanged("TCKTAndTCCNGroup");
            }
        }
        private string _TCKTAndTCCNGroup;

        [DataMemberAttribute()]
        public string BiddingHospital
        {
            get
            {
                return _BiddingHospital;
            }
            set
            {
                _BiddingHospital = value;
                RaisePropertyChanged("BiddingHospital");
            }
        }
        private string _BiddingHospital;

        [DataMemberAttribute()]
        public string BidDecisionNumAndOrdinalNum
        {
            get
            {
                return _BidDecisionNumAndOrdinalNum;
            }
            set
            {
                _BidDecisionNumAndOrdinalNum = value;
                RaisePropertyChanged("BidDecisionNumAndOrdinalNum");
            }
        }
        private string _BidDecisionNumAndOrdinalNum;

        [DataMemberAttribute()]
        public string BidEffectiveDate
        {
            get
            {
                return _BidEffectiveDate;
            }
            set
            {
                _BidEffectiveDate = value;
                RaisePropertyChanged("BidEffectiveDate");
            }
        }
        private string _BidEffectiveDate;

        [DataMemberAttribute()]
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
        private string _Note;


        [DataMemberAttribute()]
        public RefGenMedProductSimple MatchRefGenMedProduct
        {
            get
            {
                return _MatchRefGenMedProduct;
            }
            set
            {
                _MatchRefGenMedProduct = value;
                RaisePropertyChanged("MatchRefGenMedProduct");
            }
        }
        private RefGenMedProductSimple _MatchRefGenMedProduct;

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Supplier SupplierMain
        {
            get
            {
                return _SupplierMain;
            }
            set
            {
                _SupplierMain = value;
                RaisePropertyChanged("SupplierMain");
            }
        }
        private Supplier _SupplierMain;

        [Required(ErrorMessage = "Vui lòng chọn đơn vị tính")]
        [DataMemberAttribute()]
        public RefUnit SeletedUnit
        {
            get
            {
                return _seletedUnit;
            }
            set
            {
                OnSeletedUnitChanging(value);
                ValidateProperty("SeletedUnit", value);
                _seletedUnit = value;
                RaisePropertyChanged("SeletedUnit");
                OnSeletedUnitChanged();
            }
        }
        private RefUnit _seletedUnit;
        partial void OnSeletedUnitChanging(RefUnit unit);
        partial void OnSeletedUnitChanged();

        [Required(ErrorMessage = "Vui lòng chọn đơn vị dùng")]
        [DataMemberAttribute()]
        public RefUnit SeletedUnitUse
        {
            get
            {
                return _SeletedUnitUse;
            }
            set
            {
                ValidateProperty("SeletedUnitUse", value);
                _SeletedUnitUse = value;
                RaisePropertyChanged("SeletedUnitUse");
            }
        }
        private RefUnit _SeletedUnitUse;


        [Required(ErrorMessage = "Vui lòng chọn quốc gia")]
        [DataMemberAttribute()]
        public RefCountry SeletedCountry
        {
            get
            {
                return _seletedCountry;
            }
            set
            {
                if (_seletedCountry != value)
                {
                    OnCountryChanging(value);
                    ValidateProperty("SeletedCountry", value);
                    _seletedCountry = value;
                    RaisePropertyChanged("SeletedCountry");
                    OnCountryChanged();
                }
            }
        }
        private RefCountry _seletedCountry;
        partial void OnCountryChanging(RefCountry unit);
        partial void OnCountryChanged();


        [Required(ErrorMessage = "Vui lòng chọn họ thuốc")]
        [DataMemberAttribute()]
        public DrugClass SeletedDrugClass
        {
            get
            {
                return _seletedFamilyTherapy;
            }
            set
            {
                if (_seletedFamilyTherapy != value)
                {
                    OnFamilyTherapyChanging(value);
                    ValidateProperty("SeletedDrugClass", value);
                    _seletedFamilyTherapy = value;
                    RaisePropertyChanged("SeletedDrugClass");
                    OnFamilyTherapyChanged();
                }
            }
        }
        private DrugClass _seletedFamilyTherapy;
        partial void OnFamilyTherapyChanging(DrugClass unit);
        partial void OnFamilyTherapyChanged();

        [Required(ErrorMessage = "Vui lòng chọn NSX")]
        [DataMemberAttribute()]
        public PharmaceuticalCompany PharmaceuticalCompany
        {
            get
            {
                return _PharmaceuticalCompany;
            }
            set
            {
                if (_PharmaceuticalCompany != value)
                {
                    OnPharmaceuticalCompanyChanging(value);
                    ValidateProperty("PharmaceuticalCompany", value);
                    _PharmaceuticalCompany = value;
                    RaisePropertyChanged("PharmaceuticalCompany");
                    OnPharmaceuticalCompanyChanged();
                }
            }
        }
        private PharmaceuticalCompany _PharmaceuticalCompany;
        partial void OnPharmaceuticalCompanyChanging(PharmaceuticalCompany unit);
        partial void OnPharmaceuticalCompanyChanged();

        [DataMemberAttribute()]
        public Hospital CurrentHospital
        {
            get
            {
                return _CurrentHospital;
            }
            set
            {
                if (_CurrentHospital != value)
                {
                    OnCurrentHospitalChanging(value);
                    _CurrentHospital = value;
                    RaisePropertyChanged("CurrentHospital");
                    OnCurrentHospitalChanged();
                }
            }
        }
        private Hospital _CurrentHospital;
        partial void OnCurrentHospitalChanging(Hospital CurrentHospital);
        partial void OnCurrentHospitalChanged();


        [DataMemberAttribute()]
        public string WinningHospitals
        {
            get
            {
                return _winningHospitals;
            }
            set
            {
                if (_winningHospitals != value)
                {
                    OnWinningHospitalsChanging(value);
                    _winningHospitals = value;
                    RaisePropertyChanged("WinningHospitals");
                    OnWinningHospitalsChanged();
                }
            }
        }
        private string _winningHospitals;
        partial void OnWinningHospitalsChanging(string WinningHospitals);
        partial void OnWinningHospitalsChanged();


        [DataMemberAttribute()]
        public RefGenDrugBHYT_Category CurrentRefGenDrugBHYT_Category
        {
            get
            {
                return _CurrentRefGenDrugBHYT_Category;
            }
            set
            {
                _CurrentRefGenDrugBHYT_Category = value;
                RaisePropertyChanged("CurrentRefGenDrugBHYT_Category");
            }
        }
        private RefGenDrugBHYT_Category _CurrentRefGenDrugBHYT_Category;


        [DataMemberAttribute()]
        public bool MonitorOutQty
        {
            get
            {
                return _monitorOutQty;
            }
            set
            {
                if (_monitorOutQty != value)
                {
                    OnMonitorOutQtyChanging(value);
                    _monitorOutQty = value;
                    RaisePropertyChanged("MonitorOutQty");
                    OnMonitorOutQtyChanged();
                }
            }
        }
        private bool _monitorOutQty;
        partial void OnMonitorOutQtyChanging(bool MonitorOutQty);
        partial void OnMonitorOutQtyChanged();


        [DataMemberAttribute()]
        public int LimitedOutQty
        {
            get
            {
                return _limitedOutQty;
            }
            set
            {
                OnLimitedOutQtyChanging(value);
                _limitedOutQty = value;
                RaisePropertyChanged("LimitedOutQty");
                OnLimitedOutQtyChanged();
            }
        }
        private int _limitedOutQty;
        partial void OnLimitedOutQtyChanging(int LimitedOutQty);
        partial void OnLimitedOutQtyChanged();


        [DataMemberAttribute()]
        public int RemainWarningLevel1
        {
            get
            {
                return _remainWarningLevel1;
            }
            set
            {
                OnRemainWarningLevel1Changing(value);
                _remainWarningLevel1 = value;
                RaisePropertyChanged("RemainWarningLevel1");
                OnRemainWarningLevel1Changed();
            }
        }
        private int _remainWarningLevel1;
        partial void OnRemainWarningLevel1Changing(int RemainWarningLevel1);
        partial void OnRemainWarningLevel1Changed();


        [DataMemberAttribute()]
        public int RemainWarningLevel2
        {
            get
            {
                return _remainWarningLevel2;
            }
            set
            {
                OnRemainWarningLevel2Changing(value);
                _remainWarningLevel2 = value;
                RaisePropertyChanged("RemainWarningLevel2");
                OnRemainWarningLevel2Changed();
            }
        }
        private int _remainWarningLevel2;
        partial void OnRemainWarningLevel2Changing(int RemainWarningLevel2);
        partial void OnRemainWarningLevel2Changed();

        #endregion

        #region Extention Member
        //gia mua 
        //gia mua gan nhat
        [DataMemberAttribute()]
        public decimal InBuyingPrice
        {
            get
            {
                return _InBuyingPrice;
            }
            set
            {
                if (_InBuyingPrice != value)
                {
                    _InBuyingPrice = value;
                    RaisePropertyChanged("InBuyingPrice");
                }
            }
        }
        private decimal _InBuyingPrice;

        [DataMemberAttribute()]
        public decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                if (_UnitPrice != value)
                {
                    _UnitPrice = value;
                    RaisePropertyChanged("UnitPrice");
                }
            }
        }
        private decimal _UnitPrice;

        [DataMemberAttribute()]
        public decimal PackagePrice
        {
            get
            {
                return _PackagePrice;
            }
            set
            {
                if (_PackagePrice != value)
                {
                    _PackagePrice = value;
                    RaisePropertyChanged("PackagePrice");
                }
            }
        }
        private decimal _PackagePrice;

        [DataMemberAttribute()]
        public int? Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                if (_Qty != value)
                {
                    _Qty = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }
        private int? _Qty;

        //dung cho dat hang,so luong da dat
        [DataMemberAttribute()]
        public int? Ordered
        {
            get
            {
                return _Ordered;
            }
            set
            {
                if (_Ordered != value)
                {
                    _Ordered = value;
                    RaisePropertyChanged("Ordered");
                }
            }
        }
        private int? _Ordered;

        //ton kho tai thoi diem dat
        [DataMemberAttribute()]
        public int Remaining
        {
            get
            {
                return _Remaining;
            }
            set
            {
                if (_Remaining != value)
                {
                    _Remaining = value;
                    RaisePropertyChanged("Remaining");
                }
            }
        }
        private int _Remaining;


        [DataMemberAttribute()]
        public int WaitingDeliveryQty
        {
            get
            {
                return _WaitingDeliveryQty;
            }
            set
            {
                if (_WaitingDeliveryQty != value)
                {
                    _WaitingDeliveryQty = value;
                    RaisePropertyChanged("WaitingDeliveryQty");
                }
            }
        }
        private int _WaitingDeliveryQty;

        #endregion

        //gia ban
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

        [DataMemberAttribute()]
        public decimal HIAllowedPrice
        {
            get
            {
                return _HIAllowedPrice;
            }
            set
            {
                if (_HIAllowedPrice != value)
                {
                    _HIAllowedPrice = value;
                    RaisePropertyChanged("HIAllowedPrice");
                }
            }
        }
        private decimal _HIAllowedPrice;


        [DataMemberAttribute()]
        public PharmacySellingItemPrices ObjPharmacySellingItemPrices
        {
            get { return _ObjPharmacySellingItemPrices; }
            set
            {
                OnObjPharmacySellingItemPricesChanging(value);
                _ObjPharmacySellingItemPrices = value;
                RaisePropertyChanged("ObjPharmacySellingItemPrices");
                OnObjPharmacySellingItemPricesChanged();
            }
        }
        private PharmacySellingItemPrices _ObjPharmacySellingItemPrices;
        partial void OnObjPharmacySellingItemPricesChanging(PharmacySellingItemPrices value);
        partial void OnObjPharmacySellingItemPricesChanged();


        #endregion

        public override bool Equals(object obj)
        {
            RefGenericDrugDetail seletedStore = obj as RefGenericDrugDetail;
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

        #region IEditableObject Members
        private RefGenericDrugDetail _tempRefGenericDetails;
        public void BeginEdit()
        {
            _tempRefGenericDetails = (RefGenericDrugDetail)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRefGenericDetails)
                CopyFrom(_tempRefGenericDetails);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RefGenericDrugDetail p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        #region Supplier member

        [DataMemberAttribute()]
        public ObservableCollection<SupplierGenericDrug> SupplierGenericDrugs
        {
            get
            {
                return _SupplierGenericDrugs;
            }
            set
            {
                _SupplierGenericDrugs = value;
                RaisePropertyChanged("SupplierGenericDrugs");
            }
        }
        private ObservableCollection<SupplierGenericDrug> _SupplierGenericDrugs;

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_SupplierGenericDrugs);
        }
        public string ConvertDetailsListToXml(IEnumerable<SupplierGenericDrug> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<SupplierGenericDrugs>");
                foreach (SupplierGenericDrug details in items)
                {

                    if (details.SupplierID > 0)
                    {
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<SupplierID>{0}</SupplierID>", details.SupplierID);
                        sb.AppendFormat("<DrugID>{0}</DrugID>", details.DrugID);
                        sb.AppendFormat("<IsMain>{0}</IsMain>", details.IsMain);
                        sb.AppendFormat("<UnitPrice>{0}</UnitPrice>", details.UnitPrice);
                        sb.AppendFormat("<VAT>{0}</VAT>", details.VAT);
                        sb.AppendFormat("<PackagePrice>{0}</PackagePrice>", details.PackagePrice);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</SupplierGenericDrugs>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #endregion

        [DataMemberAttribute]
        public decimal TLThanhToan
        {
            get
            {
                return _TLThanhToan;
            }
            set
            {
                _TLThanhToan = value;
                RaisePropertyChanged("TLThanhToan");
            }
        }
        private decimal _TLThanhToan;

        [DataMemberAttribute]
        public string BidName
        {
            get
            {
                return _BidName;
            }
            set
            {
                _BidName = value;
                RaisePropertyChanged("BidName");
            }
        }
        private string _BidName;

        [DataMemberAttribute]
        public long BidID
        {
            get
            {
                return _BidID;
            }
            set
            {
                _BidID = value;
                RaisePropertyChanged("BidID");
            }
        }
        private long _BidID;

        [DataMemberAttribute]
        public long HITTypeID
        {
            get
            {
                return _HITTypeID;
            }
            set
            {
                _HITTypeID = value;
                RaisePropertyChanged("HITTypeID");
            }
        }
        private long _HITTypeID;

        [DataMemberAttribute]
        public string BidCode
        {
            get
            {
                return _BidCode;
            }
            set
            {
                _BidCode = value;
                RaisePropertyChanged("BidCode");
            }
        }
        private string _BidCode;

        [DataMemberAttribute]
        public long V_ProductScope
        {
            get
            {
                return _V_ProductScope;
            }
            set
            {
                _V_ProductScope = value;
                RaisePropertyChanged("V_ProductScope");
            }
        }
        private long _V_ProductScope;

        /*==== #001 ====*/
        [DataMemberAttribute()]
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                OnV_MedProductTypeChanging(value);
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
                OnV_MedProductTypeChanged();
            }
        }
        private long _V_MedProductType;
        partial void OnV_MedProductTypeChanging(long value);
        partial void OnV_MedProductTypeChanged();

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
        public DrugClass SelectedGeneric
        {
            get
            {
                return _SelectedGeneric;
            }
            set
            {
                _SelectedGeneric = value;
                RaisePropertyChanged("SelectedGeneric");
            }
        }
        private DrugClass _SelectedGeneric;
        [DataMemberAttribute]
        public string RefGeneralReportCode
        {
            get
            {
                return _RefGeneralReportCode;
            }
            set
            {
                _RefGeneralReportCode = value;
                RaisePropertyChanged("RefGeneralReportCode");
            }
        }
        private string _RefGeneralReportCode;
        [DataMemberAttribute]
        public string RefGeneralReportName
        {
            get
            {
                return _RefGeneralReportName;
            }
            set
            {
                _RefGeneralReportName = value;
                RaisePropertyChanged("RefGeneralReportName");
            }
        }
        private string _RefGeneralReportName;


        [DataMemberAttribute]
        public string RefGeneralReportContent
        {
            get
            {
                return _RefGeneralReportContent;
            }
            set
            {
                _RefGeneralReportContent = value;
                RaisePropertyChanged("RefGeneralReportContent");
            }
        }
        private string _RefGeneralReportContent;


        [DataMemberAttribute]
        public string HowToUse
        {
            get
            {
                return _HowToUse;
            }
            set
            {
                _HowToUse = value;
                RaisePropertyChanged("HowToUse");
            }
        }
        private string _HowToUse;

        [DataMemberAttribute]
        public string ReferencesDocument
        {
            get
            {
                return _ReferencesDocument;
            }
            set
            {
                _ReferencesDocument = value;
                RaisePropertyChanged("ReferencesDocument");
            }
        }
        private string _ReferencesDocument;

        [DataMemberAttribute]
        public Int16 SellingPriceVATDef
        {
            get
            {
                return _SellingPriceVATDef;
            }
            set
            {
                _SellingPriceVATDef = value;
                RaisePropertyChanged("SellingPriceVATDef");
            }
        }
        private Int16 _SellingPriceVATDef;


        private string _strVAT = "";
        [DataMemberAttribute()]
        public string StrVAT
        {
            get
            {
                return _strVAT;
            }
            set
            {
                _strVAT = value;
                _VAT = 0;
                if (_strVAT != null && _strVAT.Length > 0)
                {
                    try
                    {
                        if (_strVAT.IndexOf(".") == 0) // example when user types .05 then make it 0.05
                        {
                            _strVAT = "0" + _strVAT;
                        }
                        _VAT = Convert.ToDecimal(_strVAT);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                RaisePropertyChanged("StrVAT");
            }
        }

        private decimal? _VAT;
        //[Range(0.0, 1.0, ErrorMessage = "VAT nằm trong khoảng 0 đến 1")]
        [DataMemberAttribute]
        public decimal? VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                _VAT = value;
                _strVAT = "";
                if (VAT.HasValue)
                {
                    _strVAT = _VAT.ToString();
                }
                RaisePropertyChanged("StrVAT");
                ValidateProperty("VAT", value);
                RaisePropertyChanged("VAT");
            }
        }

        private bool _IsNotVat;
        [DataMemberAttribute]
        public bool IsNotVat
        {
            get
            {
                return _IsNotVat;
            }
            set
            {
                _IsNotVat = value;
                if (_IsNotVat)
                {
                    VAT = null;
                }
                else
                {
                    VAT = 0;
                }
                RaisePropertyChanged("IsNotVat");
            }
        }

    }
}