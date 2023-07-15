using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefStaffCategory : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefStaffCategory object.

        /// <param name="staffCatgID">Initial value of the StaffCatgID property.</param>
        /// <param name="staffCatgDescription">Initial value of the StaffCatgDescription property.</param>
        public static RefStaffCategory CreateRefStaffCategory(Int64 staffCatgID, String staffCatgDescription)
        {
            RefStaffCategory refStaffCategory = new RefStaffCategory();
            refStaffCategory.StaffCatgID = staffCatgID;
            refStaffCategory.StaffCatgDescription = staffCatgDescription;
            return refStaffCategory;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 StaffCatgID
        {
            get
            {
                return _StaffCatgID;
            }
            set
            {
                if (_StaffCatgID != value)
                {
                    OnStaffCatgIDChanging(value);
                    ////ReportPropertyChanging("StaffCatgID");
                    _StaffCatgID = value;
                    RaisePropertyChanged("StaffCatgID");
                    OnStaffCatgIDChanged();
                }
            }
        }
        private Int64 _StaffCatgID;
        partial void OnStaffCatgIDChanging(Int64 value);
        partial void OnStaffCatgIDChanged();





        [DataMemberAttribute()]
        public String StaffCatgDescription
        {
            get
            {
                return _StaffCatgDescription;
            }
            set
            {
                OnStaffCatgDescriptionChanging(value);
                ////ReportPropertyChanging("StaffCatgDescription");
                _StaffCatgDescription = value;
                RaisePropertyChanged("StaffCatgDescription");
                OnStaffCatgDescriptionChanged();
            }
        }
        private String _StaffCatgDescription;
        partial void OnStaffCatgDescriptionChanging(String value);
        partial void OnStaffCatgDescriptionChanged();


        [DataMemberAttribute()]
        public long V_StaffCatType
        {
            get
            {
                return _V_StaffCatType;
            }
            set
            {
                OnV_StaffCatTypeChanging(value);
                ////ReportPropertyChanging("V_StaffCatType");
                _V_StaffCatType = value;
                RaisePropertyChanged("V_StaffCatType");
                OnV_StaffCatTypeChanged();
            }
        }
        private long _V_StaffCatType;
        partial void OnV_StaffCatTypeChanging(long value);
        partial void OnV_StaffCatTypeChanged();
        

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFS_REL_HR06_REFSTAFF", "Staffs")]
        public ObservableCollection<Staff> Staffs
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            RefStaffCategory info = obj as RefStaffCategory;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.StaffCatgID > 0 && this.StaffCatgID == info.StaffCatgID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
