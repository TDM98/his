using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class StaffAllowance : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new StaffAllowance object.

        /// <param name="sAlID">Initial value of the SAlID property.</param>
        public static StaffAllowance CreateStaffAllowance(long sAlID)
        {
            StaffAllowance staffAllowance = new StaffAllowance();
            staffAllowance.SAlID = sAlID;
            return staffAllowance;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long SAlID
        {
            get
            {
                return _SAlID;
            }
            set
            {
                if (_SAlID != value)
                {
                    OnSAlIDChanging(value);
                    ////ReportPropertyChanging("SAlID");
                    _SAlID = value;
                    RaisePropertyChanged("SAlID");
                    OnSAlIDChanged();
                }
            }
        }
        private long _SAlID;
        partial void OnSAlIDChanging(long value);
        partial void OnSAlIDChanged();





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
        public Nullable<long> AllowanceID
        {
            get
            {
                return _AllowanceID;
            }
            set
            {
                OnAllowanceIDChanging(value);
                ////ReportPropertyChanging("AllowanceID");
                _AllowanceID = value;
                RaisePropertyChanged("AllowanceID");
                OnAllowanceIDChanged();
            }
        }
        private Nullable<long> _AllowanceID;
        partial void OnAllowanceIDChanging(Nullable<long> value);
        partial void OnAllowanceIDChanged();





        [DataMemberAttribute()]
        public String SAName
        {
            get
            {
                return _SAName;
            }
            set
            {
                OnSANameChanging(value);
                ////ReportPropertyChanging("SAName");
                _SAName = value;
                RaisePropertyChanged("SAName");
                OnSANameChanged();
            }
        }
        private String _SAName;
        partial void OnSANameChanging(String value);
        partial void OnSANameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFALL_REL_HR20_REFALLOW", "RefAllowances")]
        public RefAllowance RefAllowance
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFALL_REL_HR17_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
