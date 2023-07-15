
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefGenDrugBHYT_Category : NotifyChangedBase
    {

        #region Factory Method


        /// Create a new RefGenDrugBHYT_Category object.

        /// <param name="RefGenDrugBHYT_CatID">Initial value of the RefGenDrugBHYT_CatID property.</param>
        /// <param name="CategoryName">Initial value of the CategoryName property.</param>
        /// <param name="CategoryDescription">Initial value of the CategoryDescription property.</param>
        public static RefGenDrugBHYT_Category CreateRefGenDrugBHYT_Category(long RefGenDrugBHYT_CatID, String CategoryName, String CategoryDescription)
        {
            RefGenDrugBHYT_Category RefGenDrugBHYT_Category = new RefGenDrugBHYT_Category();
            RefGenDrugBHYT_Category.RefGenDrugBHYT_CatID = RefGenDrugBHYT_CatID;
            RefGenDrugBHYT_Category.CategoryName = CategoryName;
            RefGenDrugBHYT_Category.CategoryDescription = CategoryDescription;
            return RefGenDrugBHYT_Category;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long RefGenDrugBHYT_CatID
        {
            get
            {
                return _RefGenDrugBHYT_CatID;
            }
            set
            {
                if (_RefGenDrugBHYT_CatID != value)
                {
                    OnRefGenDrugBHYT_CatIDChanging(value);
                    _RefGenDrugBHYT_CatID = value;
                    RaisePropertyChanged("RefGenDrugBHYT_CatID");
                    OnRefGenDrugBHYT_CatIDChanged();
                }
            }
        }
        private long _RefGenDrugBHYT_CatID;
        partial void OnRefGenDrugBHYT_CatIDChanging(long value);
        partial void OnRefGenDrugBHYT_CatIDChanged();

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
        public Nullable<Int64> IngredientOrderNo
        {
            get
            {
                return _IngredientOrderNo;
            }
            set
            {
                if (_IngredientOrderNo != value)
                {
                    OnIngredientOrderNoChanging(value);
                    _IngredientOrderNo = value;
                    RaisePropertyChanged("IngredientOrderNo");
                    OnIngredientOrderNoChanged();
                }
            }
        }
        private Nullable<Int64> _IngredientOrderNo;
        partial void OnIngredientOrderNoChanging(Nullable<Int64> value);
        partial void OnIngredientOrderNoChanged();

        //KMx: Chuyển từ kiểu Int64 thành string. Vì có thể có nhiều code trên 1 dòng. VD: "123 + 456" (01/07/2014 14:15).
        //[DataMemberAttribute()]
        //public Nullable<Int64> DrugOrderNo
        //{
        //    get
        //    {
        //        return _DrugOrderNo;
        //    }
        //    set
        //    {
        //        if (_DrugOrderNo != value)
        //        {
        //            OnDrugOrderNoChanging(value);
        //            _DrugOrderNo = value;
        //            RaisePropertyChanged("DrugOrderNo");
        //            OnDrugOrderNoChanged();
        //        }
        //    }
        //}
        //private Nullable<Int64> _DrugOrderNo;
        //partial void OnDrugOrderNoChanging(Nullable<Int64> value);
        //partial void OnDrugOrderNoChanged();

        [DataMemberAttribute()]
        public string DrugOrderNo
        {
            get
            {
                return _DrugOrderNo;
            }
            set
            {
                if (_DrugOrderNo != value)
                {
                    OnDrugOrderNoChanging(value);
                    _DrugOrderNo = value;
                    RaisePropertyChanged("DrugOrderNo");
                    OnDrugOrderNoChanged();
                }
            }
        }
        private string _DrugOrderNo;
        partial void OnDrugOrderNoChanging(string value);
        partial void OnDrugOrderNoChanged();


        [DataMemberAttribute()]
        public Int64 GroupParentID
        {
            get
            {
                return _GroupParentID;
            }
            set
            {
                if (_GroupParentID != value)
                {
                    OnGroupParentIDChanging(value);
                    _GroupParentID = value;
                    RaisePropertyChanged("GroupParentID");
                    OnGroupParentIDChanged();
                }
            }
        }
        private Int64 _GroupParentID;
        partial void OnGroupParentIDChanging(Int64 value);
        partial void OnGroupParentIDChanged();

        [DataMemberAttribute()]
        public Int64 GroupID
        {
            get
            {
                return _GroupID;
            }
            set
            {
                if (_GroupID != value)
                {
                    OnGroupIDChanging(value);
                    _GroupID = value;
                    RaisePropertyChanged("GroupID");
                    OnGroupIDChanged();
                }
            }
        }
        private Int64 _GroupID;
        partial void OnGroupIDChanging(Int64 value);
        partial void OnGroupIDChanged();


        [DataMemberAttribute()]
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    RaisePropertyChanged("IsChecked");
                }
            }
        }
        private bool _IsChecked;

        [DataMemberAttribute()]
        public bool IsCombined
        {
            get
            {
                return _IsCombined;
            }
            set
            {
                if (_IsCombined != value)
                {
                    _IsCombined = value;
                    RaisePropertyChanged("IsCombined");
                }
            }
        }
        private bool _IsCombined;

        #endregion

     
        public override bool Equals(object obj)
        {
            RefGenDrugBHYT_Category currentRefGenDrugBHYT_Category = obj as RefGenDrugBHYT_Category;
            if (currentRefGenDrugBHYT_Category == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.RefGenDrugBHYT_CatID == currentRefGenDrugBHYT_Category.RefGenDrugBHYT_CatID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region ext member

        public string DrugOrderNoAndCategoryName
        {
            get
            {
                if (this.DrugOrderNo != null)
                {
                    return this.CategoryName + "( " + this.DrugOrderNo.ToString() + " )";
                }
                else return this.CategoryName;
            }
        }

        #endregion
    }
}
