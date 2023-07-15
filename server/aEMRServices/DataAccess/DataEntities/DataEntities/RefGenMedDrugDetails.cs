using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    [DataContract]
    public partial class RefGenMedDrugDetails : NotifyChangedBase//,IChargeableItemPrice
    {
        public static RefGenMedDrugDetails CreateRefGenericDrugDetail(long drugID)
        {
            RefGenMedDrugDetails refGenericDrugDetail = new RefGenMedDrugDetails();
            refGenericDrugDetail.GenMedProductID = drugID;
            return refGenericDrugDetail;
        }

        
        private Int64 _MedGenDrugID;
        [DataMemberAttribute()]
        public Int64 MedGenDrugID
        {
            get 
            { 
                return _MedGenDrugID; 
            }
            set 
            {
                if (_MedGenDrugID != value)
                {
                    OnMedGenDrugIDChanging(value);
                    _MedGenDrugID = value;
                    RaisePropertyChanged("MedGenDrugID");
                    OnMedGenDrugIDChanged();
                }
            }
        }
        partial void OnMedGenDrugIDChanging(Int64 value);
        partial void OnMedGenDrugIDChanged();

        
        private Int64 _GenMedProductID;
        [DataMemberAttribute()]
        public Int64 GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                if (_GenMedProductID != value)
                {
                    OnGenMedProductIDChanging(value);
                    _GenMedProductID = value;
                    RaisePropertyChanged("GenMedProductID");
                }
            }
        }
        partial void OnGenMedProductIDChanging(Int64 value);
        partial void OnGenMedProductIDChanged();


       
        private string _Content;
         [DataMemberAttribute()]
        public string Content
        {
            get 
            { 
                return _Content; 
            }
            set 
            {
                if (_Content != value)
                {
                    OnContentChanging(value);
                    _Content = value;
                    RaisePropertyChanged("Content");
                    OnContentChanged();
                }
            }
        }
        partial void OnContentChanging(string value);
        partial void OnContentChanged();

       
        private string _Dosage;
         [DataMemberAttribute()]
        public string Dosage
        {
            get 
            { 
                return _Dosage; 
            }
            set 
            {
                if (_Dosage != value)
                {
                    OnDosageChanging(value);
                    _Dosage = value;
                    RaisePropertyChanged("Dosage");
                    OnDosageChanged();
                }
            }
        }
        partial void OnDosageChanging(string value);
        partial void OnDosageChanged();

       
        private string _Composition;
         [DataMemberAttribute()]
        public string Composition
        {
            get 
            { 
                return _Composition; 
            }
            set 
            {
                if (_Composition != value)
                {
                    OnCompositionChanging(value);
                    _Composition = value;
                    RaisePropertyChanged("Composition");
                    OnCompositionChanged();
                }
            }
        }
        partial void OnCompositionChanging(string value);
        partial void OnCompositionChanged();


       
        private string _ActiveIngredient;
         [DataMemberAttribute()]
        public string ActiveIngredient
        {
            get 
            { 
                return _ActiveIngredient; 
            }
            set 
            {
                if (_ActiveIngredient != value)
                {
                    OnActiveIngredientChanging(value);
                    _ActiveIngredient = value;
                    RaisePropertyChanged("ActiveIngredient");
                    OnActiveIngredientChanged();
                }
            }
        }
        partial void OnActiveIngredientChanging(string value);
        partial void OnActiveIngredientChanged();


       
        private string _Indication;
         [DataMemberAttribute()]
        public string Indication
        {
            get 
            { 
                return _Indication; 
            }
            set 
            {
                if (_Indication != value)
                {
                    OnIndicationChanging(value);
                    _Indication = value;
                    RaisePropertyChanged("Indication");
                    OnIndicationChanged();
                }
            }
        }
        partial void OnIndicationChanging(string value);
        partial void OnIndicationChanged();


     
        private string _Contraindication;
           [DataMemberAttribute()]
        public string Contraindication
        {
            get 
            { 
                return _Contraindication; 
            }
            set 
            {
                if (_Contraindication != value)
                {
                    OnContraindicationChanging(value);
                    _Contraindication = value;
                    RaisePropertyChanged("Contraindication");
                    OnContraindicationChanged();
                }
            }
        }
        partial void OnContraindicationChanging(string value);
        partial void OnContraindicationChanged();


       
        private string _Precaution_Warn;
         [DataMemberAttribute()]
        public string Precaution_Warn
        {
            get 
            {
                return _Precaution_Warn; 
            }
            set 
            {
                if (_Precaution_Warn != value)
                {
                    OnPrecaution_WarnChanging(value);
                    _Precaution_Warn = value;
                    RaisePropertyChanged("Precaution_Warn");
                    OnPrecaution_WarnChanged();
                }
            }
        }
        partial void OnPrecaution_WarnChanging(string value);
        partial void OnPrecaution_WarnChanged();


      
        private string _SideEffects;
          [DataMemberAttribute()]
        public string SideEffects
        {
            get 
            {
                return _SideEffects; 
            }
            set 
            {
                if (_SideEffects != value)
                {
                    OnSideEffectsChanging(value);
                    _SideEffects = value;
                    RaisePropertyChanged("SideEffects");
                    OnSideEffectsChanged();
                }
            }
        }
        partial void OnSideEffectsChanging(string value);
        partial void OnSideEffectsChanged();


       
        private string _Interaction;
         [DataMemberAttribute()]
        public string Interaction
        {
            get 
            { 
                return _Interaction; 
            }
            set 
            {
                if (_Interaction != value)
                {
                    OnInteractionChanging(value);
                    _Interaction = value;
                    RaisePropertyChanged("Interaction");
                    OnInteractionChanged();
                }
            }
        }
        partial void OnInteractionChanging(string value);
        partial void OnInteractionChanged();

       
        private Nullable<Int32> _AdvTimeBeforeExpire;
         [DataMemberAttribute()]
        public Nullable<Int32> AdvTimeBeforeExpire
        {
            get 
            { 
                return _AdvTimeBeforeExpire; 
            }
            set 
            {
                if (_AdvTimeBeforeExpire != value)
                {
                    OnAdvTimeBeforeExpireChanging(value);
                    _AdvTimeBeforeExpire = value;
                    RaisePropertyChanged("AdvTimeBeforeExpire");
                    OnAdvTimeBeforeExpireChanged();
                }
            }
        }
        partial void OnAdvTimeBeforeExpireChanging(Nullable<Int32> value);
        partial void OnAdvTimeBeforeExpireChanged();

        private Nullable<bool> _IsConsult;
         [DataMemberAttribute()]
        public Nullable<bool> IsConsult
        {
            get 
            { 
                return _IsConsult; 
            }
            set 
            {
                if (_IsConsult != value)
                {
                    OnIsConsultChanging(value);
                    _IsConsult = value;
                    RaisePropertyChanged("IsConsult");
                    OnIsConsultChanged();
                }
            }
        }
        partial void OnIsConsultChanging(Nullable<bool> value);
        partial void OnIsConsultChanged();

     
        private long? _RefGenDrugBHYT_CatID;
           [DataMemberAttribute()]
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
        public string VidalGroup
        {
            get
            {
                return _VidalGroup;
            }
            set
            {
                _VidalGroup = value;
                RaisePropertyChanged("VidalGroup");
            }
        }
        private string _VidalGroup;


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

        private string _HowToUse;
        [DataMemberAttribute()]
        public string HowToUse
        {
            get
            {
                return _HowToUse;
            }
            set
            {
                if (_HowToUse != value)
                {
                    OnHowToUseChanging(value);
                    _HowToUse = value;
                    RaisePropertyChanged("HowToUse");
                    OnHowToUseChanged();
                }
            }
        }
        partial void OnHowToUseChanging(string value);
        partial void OnHowToUseChanged();

        private string _ReferencesDocument;
        [DataMemberAttribute()]
        public string ReferencesDocument
        {
            get
            {
                return _ReferencesDocument;
            }
            set
            {
                if (_ReferencesDocument != value)
                {
                    OnReferencesDocumentChanging(value);
                    _ReferencesDocument = value;
                    RaisePropertyChanged("ReferencesDocument");
                    OnReferencesDocumentChanged();
                }
            }
        }
        partial void OnReferencesDocumentChanging(string value);
        partial void OnReferencesDocumentChanged();

        private string _IndicationInfo;
        [DataMemberAttribute()]
        public string IndicationInfo
        {
            get
            {
                return _IndicationInfo;
            }
            set
            {
                if (_IndicationInfo != value)
                {
                    OnIndicationInfoChanging(value);
                    _IndicationInfo = value;
                    RaisePropertyChanged("IndicationInfo");
                    OnIndicationInfoChanged();
                }
            }
        }
        partial void OnIndicationInfoChanging(string value);
        partial void OnIndicationInfoChanged();

        private string _PrivateContent;
        [DataMemberAttribute]
        public string PrivateContent
        {
            get
            {
                return _PrivateContent;
            }
            set
            {
                if (_PrivateContent == value)
                {
                    return;
                }
                _PrivateContent = value;
                RaisePropertyChanged("PrivateContent");
            }
        }
        private bool _IsAcceptRoundValue;
        [DataMemberAttribute]
        public bool IsAcceptRoundValue
        {
            get
            {
                return _IsAcceptRoundValue;
            }
            set
            {
                if (_IsAcceptRoundValue == value)
                {
                    return;
                }
                _IsAcceptRoundValue = value;
                RaisePropertyChanged("IsAcceptRoundValue");
            }
        }


        #region Navigation Properties

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

        #endregion

        public override bool Equals(object obj)
        {
            RefGenMedDrugDetails info = obj as RefGenMedDrugDetails;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.MedGenDrugID == info.MedGenDrugID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
