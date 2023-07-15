using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class RefItem : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefItem object.

        /// <param name="itemID">Initial value of the ItemID property.</param>
        /// <param name="refItemID">Initial value of the RefItemID property.</param>
        /// <param name="itemFileLocation">Initial value of the ItemFileLocation property.</param>
        /// <param name="itemIssueDate">Initial value of the ItemIssueDate property.</param>
        public static RefItem CreateRefItem(long itemID, Int32 refItemID, String itemFileLocation, DateTime itemIssueDate)
        {
            RefItem refItem = new RefItem();
            refItem.ItemID = itemID;
            refItem.RefItemID = refItemID;
            refItem.ItemFileLocation = itemFileLocation;
            refItem.ItemIssueDate = itemIssueDate;
            return refItem;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Nullable<long> ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                OnServiceRecIDChanging(value);
                ////ReportPropertyChanging("ServiceRecID");
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private Nullable<long> _ServiceRecID;
        partial void OnServiceRecIDChanging(Nullable<long> value);
        partial void OnServiceRecIDChanged();





        [DataMemberAttribute()]
        public long ItemID
        {
            get
            {
                return _ItemID;
            }
            set
            {
                OnItemIDChanging(value);
                ////ReportPropertyChanging("ItemID");
                _ItemID = value;
                RaisePropertyChanged("ItemID");
                OnItemIDChanged();
            }
        }
        private long _ItemID;
        partial void OnItemIDChanging(long value);
        partial void OnItemIDChanged();





        [DataMemberAttribute()]
        public Int32 RefItemID
        {
            get
            {
                return _RefItemID;
            }
            set
            {
                if (_RefItemID != value)
                {
                    OnRefItemIDChanging(value);
                    ////ReportPropertyChanging("RefItemID");
                    _RefItemID = value;
                    RaisePropertyChanged("RefItemID");
                    OnRefItemIDChanged();
                }
            }
        }
        private Int32 _RefItemID;
        partial void OnRefItemIDChanging(Int32 value);
        partial void OnRefItemIDChanged();





        [DataMemberAttribute()]
        public String ItemFileLocation
        {
            get
            {
                return _ItemFileLocation;
            }
            set
            {
                OnItemFileLocationChanging(value);
                ////ReportPropertyChanging("ItemFileLocation");
                _ItemFileLocation = value;
                RaisePropertyChanged("ItemFileLocation");
                OnItemFileLocationChanged();
            }
        }
        private String _ItemFileLocation;
        partial void OnItemFileLocationChanging(String value);
        partial void OnItemFileLocationChanged();





        [DataMemberAttribute()]
        public DateTime ItemIssueDate
        {
            get
            {
                return _ItemIssueDate;
            }
            set
            {
                OnItemIssueDateChanging(value);
                ////ReportPropertyChanging("ItemIssueDate");
                _ItemIssueDate = value;
                RaisePropertyChanged("ItemIssueDate");
                OnItemIssueDateChanged();
            }
        }
        private DateTime _ItemIssueDate;
        partial void OnItemIssueDateChanging(DateTime value);
        partial void OnItemIssueDateChanged();





        [DataMemberAttribute()]
        public String Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                OnNotesChanging(value);
                ////ReportPropertyChanging("Notes");
                _Notes = value;
                RaisePropertyChanged("Notes");
                OnNotesChanged();
            }
        }
        private String _Notes;
        partial void OnNotesChanging(String value);
        partial void OnNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFITEMS_REL_PMR31_PATIENTS", "PatientServiceRecords")]
        public PatientServiceRecord PatientServiceRecord
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFITEMS_REL_PMR16_REFITEMT", "RefItemType")]
        public RefItemType RefItemType
        {
            get;
            set;
        }


        #endregion
    }
}
