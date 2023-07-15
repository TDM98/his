using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class RefItemType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefItemType object.

        /// <param name="itemID">Initial value of the ItemID property.</param>
        /// <param name="itemName">Initial value of the ItemName property.</param>
        /// <param name="itemDescription">Initial value of the ItemDescription property.</param>
        public static RefItemType CreateRefItemType(long itemID, String itemName, String itemDescription)
        {
            RefItemType refItemType = new RefItemType();
            refItemType.ItemID = itemID;
            refItemType.ItemName = itemName;
            refItemType.ItemDescription = itemDescription;
            return refItemType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long ItemID
        {
            get
            {
                return _ItemID;
            }
            set
            {
                if (_ItemID != value)
                {
                    OnItemIDChanging(value);
                    ////ReportPropertyChanging("ItemID");
                    _ItemID = value;
                    RaisePropertyChanged("ItemID");
                    OnItemIDChanged();
                }
            }
        }
        private long _ItemID;
        partial void OnItemIDChanging(long value);
        partial void OnItemIDChanged();





        [DataMemberAttribute()]
        public String ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                OnItemNameChanging(value);
                ////ReportPropertyChanging("ItemName");
                _ItemName = value;
                RaisePropertyChanged("ItemName");
                OnItemNameChanged();
            }
        }
        private String _ItemName;
        partial void OnItemNameChanging(String value);
        partial void OnItemNameChanged();





        [DataMemberAttribute()]
        public String ItemDescription
        {
            get
            {
                return _ItemDescription;
            }
            set
            {
                OnItemDescriptionChanging(value);
                ////ReportPropertyChanging("ItemDescription");
                _ItemDescription = value;
                RaisePropertyChanged("ItemDescription");
                OnItemDescriptionChanged();
            }
        }
        private String _ItemDescription;
        partial void OnItemDescriptionChanging(String value);
        partial void OnItemDescriptionChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFITEMS_REL_PMR16_REFITEMT", "RefItems")]
        public ObservableCollection<RefItem> RefItems
        {
            get;
            set;
        }

        #endregion
    }
}
