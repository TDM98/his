using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
namespace DataEntities
{
    public partial class PCLExamTypeCombo : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLExamParamResult object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static PCLExamTypeCombo CreatePCLExamTypeCombo(string ComboName, string PCLExamTestItemCode)
        {
            PCLExamTypeCombo PCLExamTestItem = new PCLExamTypeCombo();
            PCLExamTestItem.ComboName = ComboName;
            return PCLExamTestItem;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PCLExamTypeComboID
        {
            get
            {
                return _PCLExamTypeComboID;
            }
            set
            {
                if (_PCLExamTypeComboID != value)
                {
                    OnPCLExamTypeComboIDChanging(value);
                    _PCLExamTypeComboID = value;
                    RaisePropertyChanged("PCLExamTypeComboID");
                    OnPCLExamTypeComboIDChanged();
                }
            }
        }
        private Int64 _PCLExamTypeComboID;
        partial void OnPCLExamTypeComboIDChanging(Int64 value);
        partial void OnPCLExamTypeComboIDChanged();

        [DataMemberAttribute()]
        public String ComboName
        {
            get
            {
                return _ComboName;
            }
            set
            {
                OnComboNameChanging(value);
                
                _ComboName = value;
                RaisePropertyChanged("ComboName");
                OnComboNameChanged();
            }
        }
        private String _ComboName;
        partial void OnComboNameChanging(String value);
        partial void OnComboNameChanged();

        [DataMemberAttribute()]
        public String ComboDescription
        {
            get
            {
                return _ComboDescription;
            }
            set
            {
                OnComboDescriptionChanging(value);
                _ComboDescription = value;
                RaisePropertyChanged("ComboDescription");
                OnComboDescriptionChanged();
            }
        }
        private String _ComboDescription;
        partial void OnComboDescriptionChanging(String value);
        partial void OnComboDescriptionChanged();

        [DataMemberAttribute()]
        public Int64 CreatorStaffID
        {
            get
            {
                return _CreatorStaffID;
            }
            set
            {
                if (_CreatorStaffID != value)
                {
                    OnCreatorStaffIDChanging(value);
                    _CreatorStaffID = value;
                    RaisePropertyChanged("CreatorStaffID");
                    OnCreatorStaffIDChanged();
                }
            }
        }
        private Int64 _CreatorStaffID;
        partial void OnCreatorStaffIDChanging(Int64 value);
        partial void OnCreatorStaffIDChanged();

        [DataMemberAttribute()]
        public String StaffName
        {
            get
            {
                return _StaffName;
            }
            set
            {
                OnStaffNameChanging(value);
                _StaffName = value;
                RaisePropertyChanged("StaffName");
                OnStaffNameChanged();
            }
        }
        private String _StaffName;
        partial void OnStaffNameChanging(String value);
        partial void OnStaffNameChanged();


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
        private DateTime _RecCreatedDate=DateTime.Now;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();
        #endregion

        #region Navigation Properties

        private ObservableCollection<PCLExamTypeComboItem> _PCLExamTypeComboItems;
        [DataMemberAttribute()]
        public ObservableCollection<PCLExamTypeComboItem> PCLExamTypeComboItems
        {
            get
            {
                return _PCLExamTypeComboItems;
            }
            set
            {
                _PCLExamTypeComboItems = value;
                RaisePropertyChanged("PCLExamTypeComboItems");
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            PCLExamTypeCombo SelectedItem = obj as PCLExamTypeCombo;
            if (SelectedItem == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamTypeComboID > 0 && this.PCLExamTypeComboID == SelectedItem.PCLExamTypeComboID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public partial class PCLExamTypeComboItem : NotifyChangedBase
    {
        #region Factory Method

        /// Create a new PCLExamParamResult object.
        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static PCLExamTypeComboItem CreatePCLExamTypeComboItem(Int64 PCLExamTypeComboID)
        {
            PCLExamTypeComboItem PCLExamTestItem = new PCLExamTypeComboItem();
            PCLExamTestItem.PCLExamTypeComboID = PCLExamTypeComboID;
            return PCLExamTestItem;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PCLExamTypeComboItemID
        {
            get
            {
                return _PCLExamTypeComboItemID;
            }
            set
            {
                if (_PCLExamTypeComboItemID != value)
                {
                    OnPCLExamTypeComboItemIDChanging(value);
                    _PCLExamTypeComboItemID = value;
                    RaisePropertyChanged("PCLExamTypeComboItemID");
                    OnPCLExamTypeComboItemIDChanged();
                }
            }
        }
        private Int64 _PCLExamTypeComboItemID;
        partial void OnPCLExamTypeComboItemIDChanging(Int64 value);
        partial void OnPCLExamTypeComboItemIDChanged();

        [DataMemberAttribute()]
        public Int64 PCLExamTypeComboID
        {
            get
            {
                return _PCLExamTypeComboID;
            }
            set
            {
                OnPCLExamTypeComboIDChanging(value);

                _PCLExamTypeComboID = value;
                RaisePropertyChanged("PCLExamTypeComboID");
                OnPCLExamTypeComboIDChanged();
            }
        }
        private Int64 _PCLExamTypeComboID;
        partial void OnPCLExamTypeComboIDChanging(Int64 value);
        partial void OnPCLExamTypeComboIDChanged();

        [DataMemberAttribute()]
        public Int64 PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                OnPCLExamTypeIDChanging(value);
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
                OnPCLExamTypeIDChanged();
            }
        }
        private Int64 _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(Int64 value);
        partial void OnPCLExamTypeIDChanged();

        #endregion

        #region Navigation Properties
        private PCLExamType _pclExamType;
        [DataMemberAttribute()]
        public PCLExamType PCLExamType
        {
            get
            {
                return _pclExamType;
            }
            set
            {
                _pclExamType = value;
                RaisePropertyChanged("PCLExamType");
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            PCLExamTypeComboItem SelectedItem = obj as PCLExamTypeComboItem;
            if (SelectedItem == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamTypeComboItemID > 0 && this.PCLExamTypeComboItemID == SelectedItem.PCLExamTypeComboItemID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
