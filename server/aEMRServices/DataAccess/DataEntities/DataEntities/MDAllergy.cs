using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class MDAllergy : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new MDAllergy object.

        /// <param name="aItemID">Initial value of the AItemID property.</param>
        /// <param name="allergiesItems">Initial value of the AllergiesItems property.</param>
        public static MDAllergy CreateMDAllergy(long aItemID, String allergiesItems)
        {
            MDAllergy mDAllergy = new MDAllergy();
            mDAllergy.AItemID = aItemID;
            mDAllergy.AllergiesItems = allergiesItems;
            return mDAllergy;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long AItemID
        {
            get
            {
                return _AItemID;
            }
            set
            {
                if (_AItemID != value)
                {
                    OnAItemIDChanging(value);
                    _AItemID = value;
                    RaisePropertyChanged("AItemID");
                    OnAItemIDChanged();
                }
            }
        }
        private long _AItemID;
        partial void OnAItemIDChanging(long value);
        partial void OnAItemIDChanged();


        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                if (_RecCreatedDate != value)
                {
                    OnRecCreatedDateChanging(value);
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                    OnRecCreatedDateChanged();
                }
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();


        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    OnPatientIDChanging(value);
                    _PatientID = value;
                    RaisePropertyChanged("PatientID");
                    OnPatientIDChanged();
                }
            }
        }
        private long _PatientID;
        partial void OnPatientIDChanging(long value);
        partial void OnPatientIDChanged();


        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    OnStaffIDChanging(value);
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                    OnStaffIDChanged();
                }
            }
        }
        private long _StaffID;
        partial void OnStaffIDChanging(long value);
        partial void OnStaffIDChanged();
    
        
        [DataMemberAttribute()]
        public String AllergiesItems
        {
            get
            {
                return _AllergiesItems;
            }
            set
            {
                OnAllergiesItemsChanging(value);
                _AllergiesItems = value;
                RaisePropertyChanged("AllergiesItems");
                OnAllergiesItemsChanged();
            }
        }
        private String _AllergiesItems;
        partial void OnAllergiesItemsChanging(String value);
        partial void OnAllergiesItemsChanged();

        [DataMemberAttribute()]
        public String Reactions
        {
            get
            {
                return _Reactions;
            }
            set
            {
                OnReactionsChanging(value);
                _Reactions = value;
                RaisePropertyChanged("Reactions");
                OnReactionsChanged();
            }
        }
        private String _Reactions;
        partial void OnReactionsChanging(String value);
        partial void OnReactionsChanged();
        

        [DataMemberAttribute()]
        public Nullable<Int64> V_AItemType
        {
            get
            {
                return _V_AItemType;
            }
            set
            {
                OnV_AItemTypeChanging(value);
                _V_AItemType = value;
                RaisePropertyChanged("V_AItemType");
                OnV_AItemTypeChanged();
            }
        }
        private Nullable<Int64> _V_AItemType;
        partial void OnV_AItemTypeChanging(Nullable<Int64> value);
        partial void OnV_AItemTypeChanged();


        
        [DataMemberAttribute()]
        public Lookup ObjV_AItemType
        {
            get
            {
                return _ObjV_AItemType;
            }
            set
            {
                OnObjV_AItemTypeChanging(value);
                _ObjV_AItemType = value;
                RaisePropertyChanged("ObjV_AItemType");
                OnObjV_AItemTypeChanged();
            }
        }
        private Lookup _ObjV_AItemType;
        partial void OnObjV_AItemTypeChanging(Lookup value);
        partial void OnObjV_AItemTypeChanged();



        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted != value)
                {
                    OnIsDeletedChanging(value);
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                    OnIsDeletedChanged();
                }
            }
        }
        private bool _IsDeleted;
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();


        [DataMemberAttribute()]
        public DateTime DateDeleted
        {
            get
            {
                return _DateDeleted;
            }
            set
            {
                if (_DateDeleted != value)
                {
                    OnDateDeletedChanging(value);
                    _DateDeleted = value;
                    RaisePropertyChanged("DateDeleted");
                    OnDateDeletedChanged();
                }
            }
        }
        private DateTime _DateDeleted;
        partial void OnDateDeletedChanging(DateTime value);
        partial void OnDateDeletedChanged();

        #endregion


        

    }
}
