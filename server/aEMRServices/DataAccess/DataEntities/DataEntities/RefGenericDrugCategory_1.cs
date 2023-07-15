
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefGenericDrugCategory_1 : NotifyChangedBase
    {

        #region Factory Method


        /// Create a new RefGenericDrugCategory_1 object.

        /// <param name="RefGenDrugCatID_1">Initial value of the RefGenDrugCatID_1 property.</param>
        /// <param name="CategoryName">Initial value of the CategoryName property.</param>
        /// <param name="CategoryDescription">Initial value of the CategoryDescription property.</param>
        public static RefGenericDrugCategory_1 CreateRefGenericDrugCategory_1(long RefGenDrugCatID_1, String CategoryName, String CategoryDescription)
        {
            RefGenericDrugCategory_1 RefGenericDrugCategory_1 = new RefGenericDrugCategory_1();
            RefGenericDrugCategory_1.RefGenDrugCatID_1 = RefGenDrugCatID_1;
            RefGenericDrugCategory_1.CategoryName = CategoryName;
            RefGenericDrugCategory_1.CategoryDescription = CategoryDescription;
            return RefGenericDrugCategory_1;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long RefGenDrugCatID_1
        {
            get
            {
                return _RefGenDrugCatID_1;
            }
            set
            {
                if (_RefGenDrugCatID_1 != value)
                {
                    OnRefGenDrugCatID_1Changing(value);
                    _RefGenDrugCatID_1 = value;
                    RaisePropertyChanged("RefGenDrugCatID_1");
                    OnRefGenDrugCatID_1Changed();
                }
            }
        }
        private long _RefGenDrugCatID_1;
        partial void OnRefGenDrugCatID_1Changing(long value);
        partial void OnRefGenDrugCatID_1Changed();

        [DataMemberAttribute()]
        public String CategoryName
        {
            get
            {
                return _CategoryName;
            }
            set
            {
                OnCategoryNameChanging(value);
                _CategoryName = value;
                RaisePropertyChanged("CategoryName");
                OnCategoryNameChanged();
            }
        }
        private String _CategoryName;
        partial void OnCategoryNameChanging(String value);
        partial void OnCategoryNameChanged();

        [DataMemberAttribute()]
        public String CategoryDescription
        {
            get
            {
                return _CategoryDescription;
            }
            set
            {
                OnCategoryDescriptionChanging(value);
                _CategoryDescription = value;
                RaisePropertyChanged("CategoryDescription");
                OnCategoryDescriptionChanged();
            }
        }
        private String _CategoryDescription;
        partial void OnCategoryDescriptionChanging(String value);
        partial void OnCategoryDescriptionChanged();

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
        #endregion


        public override bool Equals(object obj)
        {
            RefGenericDrugCategory_1 currentRefGenericDrugCategory_1 = obj as RefGenericDrugCategory_1;
            if (currentRefGenericDrugCategory_1 == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.RefGenDrugCatID_1 == currentRefGenericDrugCategory_1.RefGenDrugCatID_1;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        #region ext member

        public string CategoryNameAndCategoryDescription
        {
            get
            {
                string results = "";
                results = this.CategoryName;
                if (!string.IsNullOrEmpty(this.CategoryDescription))
                {
                    results = results + " (" + this.CategoryDescription + ") ";
                }
                return results;
            }
        }

        #endregion
    }

     public partial class RefGenericDrugCategory_2 : NotifyChangedBase
    {

        #region Factory Method


        /// Create a new RefGenericDrugCategory_1 object.

        /// <param name="RefGenDrugCatID_1">Initial value of the RefGenDrugCatID_1 property.</param>
        /// <param name="CategoryName">Initial value of the CategoryName property.</param>
        /// <param name="CategoryDescription">Initial value of the CategoryDescription property.</param>
        public static RefGenericDrugCategory_2 CreateRefGenericDrugCategory_1(long RefGenDrugCatID_1, String CategoryName, String CategoryDescription)
        {
            RefGenericDrugCategory_2 RefGenericDrugCategory_2 = new RefGenericDrugCategory_2();
            RefGenericDrugCategory_2.RefGenDrugCatID_2 = RefGenDrugCatID_1;
            RefGenericDrugCategory_2.CategoryName = CategoryName;
            RefGenericDrugCategory_2.CategoryDescription = CategoryDescription;
            return RefGenericDrugCategory_2;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long RefGenDrugCatID_2
        {
            get
            {
                return _RefGenDrugCatID_2;
            }
            set
            {
                if (_RefGenDrugCatID_2 != value)
                {
                    OnRefGenDrugCatID_2Changing(value);
                    _RefGenDrugCatID_2 = value;
                    RaisePropertyChanged("RefGenDrugCatID_2");
                    OnRefGenDrugCatID_2Changed();
                }
            }
        }
        private long _RefGenDrugCatID_2;
        partial void OnRefGenDrugCatID_2Changing(long value);
        partial void OnRefGenDrugCatID_2Changed();

        [DataMemberAttribute()]
        public String CategoryName
        {
            get
            {
                return _CategoryName;
            }
            set
            {
                OnCategoryNameChanging(value);
                _CategoryName = value;
                RaisePropertyChanged("CategoryName");
                OnCategoryNameChanged();
            }
        }
        private String _CategoryName;
        partial void OnCategoryNameChanging(String value);
        partial void OnCategoryNameChanged();

        [DataMemberAttribute()]
        public String CategoryDescription
        {
            get
            {
                return _CategoryDescription;
            }
            set
            {
                OnCategoryDescriptionChanging(value);
                _CategoryDescription = value;
                RaisePropertyChanged("CategoryDescription");
                OnCategoryDescriptionChanged();
            }
        }
        private String _CategoryDescription;
        partial void OnCategoryDescriptionChanging(String value);
        partial void OnCategoryDescriptionChanged();

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

        #endregion


        public override bool Equals(object obj)
        {
            RefGenericDrugCategory_2 currentRefGenericDrugCategory_1 = obj as RefGenericDrugCategory_2;
            if (currentRefGenericDrugCategory_1 == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.RefGenDrugCatID_2 == currentRefGenericDrugCategory_1.RefGenDrugCatID_2;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region ext member

        public string CategoryNameAndCategoryDescription
        {
            get
            {
                string results = "";
                results = this.CategoryName;
                if (!string.IsNullOrEmpty(this.CategoryDescription))
                {
                    results = results + " (" + this.CategoryDescription + ") ";
                }
                return results;
            }
        }

        #endregion
    }

     public partial class RefPharmacyDrugCategory : NotifyChangedBase
     {

         #region Factory Method


         /// Create a new RefPharmacyDrugCategory object.

         /// <param name="RefPharmacyDrugCatID">Initial value of the RefPharmacyDrugCatID property.</param>
         /// <param name="CategoryName">Initial value of the CategoryName property.</param>
         /// <param name="CategoryDescription">Initial value of the CategoryDescription property.</param>
         public static RefPharmacyDrugCategory CreateRefPharmacyDrugCategory(long RefPharmacyDrugCatID, String CategoryName, String CategoryDescription)
         {
             RefPharmacyDrugCategory RefPharmacyDrugCategory = new RefPharmacyDrugCategory();
             RefPharmacyDrugCategory.RefPharmacyDrugCatID = RefPharmacyDrugCatID;
             RefPharmacyDrugCategory.CategoryName = CategoryName;
             RefPharmacyDrugCategory.CategoryDescription = CategoryDescription;
             return RefPharmacyDrugCategory;
         }

         #endregion

         #region Primitive Properties

         [DataMemberAttribute()]
         public long RefPharmacyDrugCatID
         {
             get
             {
                 return _RefPharmacyDrugCatID;
             }
             set
             {
                 if (_RefPharmacyDrugCatID != value)
                 {
                     OnRefPharmacyDrugCatIDChanging(value);
                     _RefPharmacyDrugCatID = value;
                     RaisePropertyChanged("RefPharmacyDrugCatID");
                     OnRefPharmacyDrugCatIDChanged();
                 }
             }
         }
         private long _RefPharmacyDrugCatID;
         partial void OnRefPharmacyDrugCatIDChanging(long value);
         partial void OnRefPharmacyDrugCatIDChanged();

         [DataMemberAttribute()]
         public String CategoryName
         {
             get
             {
                 return _CategoryName;
             }
             set
             {
                 OnCategoryNameChanging(value);
                 _CategoryName = value;
                 RaisePropertyChanged("CategoryName");
                 OnCategoryNameChanged();
             }
         }
         private String _CategoryName;
         partial void OnCategoryNameChanging(String value);
         partial void OnCategoryNameChanged();

         [DataMemberAttribute()]
         public String CategoryDescription
         {
             get
             {
                 return _CategoryDescription;
             }
             set
             {
                 OnCategoryDescriptionChanging(value);
                 _CategoryDescription = value;
                 RaisePropertyChanged("CategoryDescription");
                 OnCategoryDescriptionChanged();
             }
         }
         private String _CategoryDescription;
         partial void OnCategoryDescriptionChanging(String value);
         partial void OnCategoryDescriptionChanged();

         #endregion


         public override bool Equals(object obj)
         {
             RefPharmacyDrugCategory currentRefPharmacyDrugCategory = obj as RefPharmacyDrugCategory;
             if (currentRefPharmacyDrugCategory == null)
                 return false;

             if (Object.ReferenceEquals(this, obj))
                 return true;

             return this.RefPharmacyDrugCatID == currentRefPharmacyDrugCategory.RefPharmacyDrugCatID;
         }
         public override int GetHashCode()
         {
             return base.GetHashCode();
         }
     }

}
