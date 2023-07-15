using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HistorySalary : NotifyChangedBase, IEditableObject
    {
        public HistorySalary()
            : base()
        {

        }

        private HistorySalary _tempHistorySalary;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHistorySalary = (HistorySalary)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHistorySalary)
                CopyFrom(_tempHistorySalary);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HistorySalary p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new HistorySalary object.

        /// <param name="hSID">Initial value of the HSID property.</param>
        /// <param name="hSCreateDate">Initial value of the HSCreateDate property.</param>
        /// <param name="hSOldSalary">Initial value of the HSOldSalary property.</param>
        /// <param name="hSNewSalary">Initial value of the HSNewSalary property.</param>
        /// <param name="hSActive">Initial value of the HSActive property.</param>
        public static HistorySalary CreateHistorySalary(long hSID, DateTime hSCreateDate, Decimal hSOldSalary, Decimal hSNewSalary, Boolean hSActive)
        {
            HistorySalary historySalary = new HistorySalary();
            historySalary.HSID = hSID;
            historySalary.HSCreateDate = hSCreateDate;
            historySalary.HSOldSalary = hSOldSalary;
            historySalary.HSNewSalary = hSNewSalary;
            historySalary.HSActive = hSActive;
            return historySalary;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long HSID
        {
            get
            {
                return _HSID;
            }
            set
            {
                if (_HSID != value)
                {
                    OnHSIDChanging(value);
                    ////ReportPropertyChanging("HSID");
                    _HSID = value;
                    RaisePropertyChanged("HSID");
                    OnHSIDChanged();
                }
            }
        }
        private long _HSID;
        partial void OnHSIDChanging(long value);
        partial void OnHSIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public DateTime HSCreateDate
        {
            get
            {
                return _HSCreateDate;
            }
            set
            {
                OnHSCreateDateChanging(value);
                ////ReportPropertyChanging("HSCreateDate");
                _HSCreateDate = value;
                RaisePropertyChanged("HSCreateDate");
                OnHSCreateDateChanged();
            }
        }
        private DateTime _HSCreateDate;
        partial void OnHSCreateDateChanging(DateTime value);
        partial void OnHSCreateDateChanged();





        [DataMemberAttribute()]
        public Decimal HSOldSalary
        {
            get
            {
                return _HSOldSalary;
            }
            set
            {
                OnHSOldSalaryChanging(value);
                ////ReportPropertyChanging("HSOldSalary");
                _HSOldSalary = value;
                RaisePropertyChanged("HSOldSalary");
                OnHSOldSalaryChanged();
            }
        }
        private Decimal _HSOldSalary;
        partial void OnHSOldSalaryChanging(Decimal value);
        partial void OnHSOldSalaryChanged();





        [DataMemberAttribute()]
        public Decimal HSNewSalary
        {
            get
            {
                return _HSNewSalary;
            }
            set
            {
                OnHSNewSalaryChanging(value);
                ////ReportPropertyChanging("HSNewSalary");
                _HSNewSalary = value;
                RaisePropertyChanged("HSNewSalary");
                OnHSNewSalaryChanged();
            }
        }
        private Decimal _HSNewSalary;
        partial void OnHSNewSalaryChanging(Decimal value);
        partial void OnHSNewSalaryChanged();





        [DataMemberAttribute()]
        public String HSNotes
        {
            get
            {
                return _HSNotes;
            }
            set
            {
                OnHSNotesChanging(value);
                ////ReportPropertyChanging("HSNotes");
                _HSNotes = value;
                RaisePropertyChanged("HSNotes");
                OnHSNotesChanged();
            }
        }
        private String _HSNotes;
        partial void OnHSNotesChanging(String value);
        partial void OnHSNotesChanged();





        [DataMemberAttribute()]
        public Boolean HSActive
        {
            get
            {
                return _HSActive;
            }
            set
            {
                OnHSActiveChanging(value);
                ////ReportPropertyChanging("HSActive");
                _HSActive = value;
                RaisePropertyChanged("HSActive");
                OnHSActiveChanged();
            }
        }
        private Boolean _HSActive;
        partial void OnHSActiveChanging(Boolean value);
        partial void OnHSActiveChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HISTORYS_REL_HR16_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
