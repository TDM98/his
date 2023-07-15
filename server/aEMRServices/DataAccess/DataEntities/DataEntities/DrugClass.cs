using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DrugClass : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new DrugClass object.

        /// <param name="drugClassID">Initial value of the DrugClassID property.</param>
        /// <param name="faName">Initial value of the FaName property.</param>
        /// <param name="faActive">Initial value of the FaActive property.</param>
        public static DrugClass CreateDrugClass(long drugClassID, String faName, Boolean faActive)
        {
            DrugClass drugClass = new DrugClass();
            drugClass.DrugClassID = drugClassID;
            drugClass.FaName = faName;
            drugClass.FaActive = faActive;
            return drugClass;
        }

        #endregion
        #region Primitive Properties
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
                    OnDrugClassIDChanging(value);
                    ////ReportPropertyChanging("DrugClassID");
                    _DrugClassID = value;
                    RaisePropertyChanged("DrugClassID");
                    OnDrugClassIDChanged();
                }
            }
        }
        private long _DrugClassID;
        partial void OnDrugClassIDChanging(long value);
        partial void OnDrugClassIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> ParDrugClassID
        {
            get
            {
                return _ParDrugClassID;
            }
            set
            {
                OnParDrugClassIDChanging(value);
                ////ReportPropertyChanging("ParDrugClassID");
                _ParDrugClassID = value;
                RaisePropertyChanged("ParDrugClassID");
                OnParDrugClassIDChanged();
            }
        }
        private Nullable<long> _ParDrugClassID;
        partial void OnParDrugClassIDChanging(Nullable<long> value);
        partial void OnParDrugClassIDChanged();

        [DataMemberAttribute()]
        public String FaName
        {
            get
            {
                return _FaName;
            }
            set
            {
                OnFaNameChanging(value);
                _FaName = value;
                RaisePropertyChanged("FaName");
                OnFaNameChanged();
            }
        }
        private String _FaName;
        partial void OnFaNameChanging(String value);
        partial void OnFaNameChanged();

        [DataMemberAttribute()]
        public String FaDescription
        {
            get
            {
                return _FaDescription;
            }
            set
            {
                OnFaDescriptionChanging(value);
                _FaDescription = value;
                RaisePropertyChanged("FaDescription");
                OnFaDescriptionChanged();
            }
        }
        private String _FaDescription;
        partial void OnFaDescriptionChanging(String value);
        partial void OnFaDescriptionChanged();

        [DataMemberAttribute()]
        public Boolean FaActive
        {
            get
            {
                return _FaActive;
            }
            set
            {
                OnFaActiveChanging(value);
                _FaActive = value;
                RaisePropertyChanged("FaActive");
                OnFaActiveChanged();
            }
        }
        private Boolean _FaActive;
        partial void OnFaActiveChanging(Boolean value);
        partial void OnFaActiveChanged();

        [DataMemberAttribute()]
        public String DrugClassCode
        {
            get
            {
                return _DrugClassCode;
            }
            set
            {
                OnDrugClassCodeChanging(value);
                _DrugClassCode = value;
                RaisePropertyChanged("DrugClassCode");
                OnDrugClassCodeChanged();
            }
        }
        private String _DrugClassCode;
        partial void OnDrugClassCodeChanging(String value);
        partial void OnDrugClassCodeChanged();

        [DataMemberAttribute()]
        public long? V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
            }
        }
        private long? _V_MedProductType;

        private string _ParentText;
        public string ParentText
        {
            get
            {
                return _ParentText;
            }
            set
            {
                _ParentText = value;
                RaisePropertyChanged("ParentText");
            }
        }
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFGENER_REL_DMGMT_DRUGCLAS", "RefGenericDrugDetails")]
        public ObservableCollection<RefGenericDrugDetail> RefGenericDrugDetails
        {
            get;
            set;
        }
        #endregion

        public override bool Equals(object obj)
        {
            DrugClass seletedFamilyTherapy = obj as DrugClass;
            if (seletedFamilyTherapy == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.DrugClassID == seletedFamilyTherapy.DrugClassID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}