using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class StaffContract : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new StaffContract object.

        /// <param name="staffContractID">Initial value of the StaffContractID property.</param>
        /// <param name="staffID">Initial value of the StaffID property.</param>
        /// <param name="lContracID">Initial value of the LContracID property.</param>
        public static StaffContract CreateStaffContract(Int64 staffContractID, Int64 staffID, long lContracID)
        {
            StaffContract staffContract = new StaffContract();
            staffContract.StaffContractID = staffContractID;
            staffContract.StaffID = staffID;
            staffContract.LContracID = lContracID;
            return staffContract;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 StaffContractID
        {
            get
            {
                return _StaffContractID;
            }
            set
            {
                if (_StaffContractID != value)
                {
                    OnStaffContractIDChanging(value);
                    ////ReportPropertyChanging("StaffContractID");
                    _StaffContractID = value;
                    RaisePropertyChanged("StaffContractID");
                    OnStaffContractIDChanged();
                }
            }
        }
        private Int64 _StaffContractID;
        partial void OnStaffContractIDChanging(Int64 value);
        partial void OnStaffContractIDChanged();





        [DataMemberAttribute()]
        public Int64 StaffID
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
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public long LContracID
        {
            get
            {
                return _LContracID;
            }
            set
            {
                OnLContracIDChanging(value);
                ////ReportPropertyChanging("LContracID");
                _LContracID = value;
                RaisePropertyChanged("LContracID");
                OnLContracIDChanged();
            }
        }
        private long _LContracID;
        partial void OnLContracIDChanging(long value);
        partial void OnLContracIDChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> ContractDate
        {
            get
            {
                return _ContractDate;
            }
            set
            {
                OnContractDateChanging(value);
                ////ReportPropertyChanging("ContractDate");
                _ContractDate = value;
                RaisePropertyChanged("ContractDate");
                OnContractDateChanged();
            }
        }
        private Nullable<DateTime> _ContractDate;
        partial void OnContractDateChanging(Nullable<DateTime> value);
        partial void OnContractDateChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> ClosedDate
        {
            get
            {
                return _ClosedDate;
            }
            set
            {
                OnClosedDateChanging(value);
                ////ReportPropertyChanging("ClosedDate");
                _ClosedDate = value;
                RaisePropertyChanged("ClosedDate");
                OnClosedDateChanged();
            }
        }
        private Nullable<DateTime> _ClosedDate;
        partial void OnClosedDateChanging(Nullable<DateTime> value);
        partial void OnClosedDateChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> MonthlySalary
        {
            get
            {
                return _MonthlySalary;
            }
            set
            {
                OnMonthlySalaryChanging(value);
                ////ReportPropertyChanging("MonthlySalary");
                _MonthlySalary = value;
                RaisePropertyChanged("MonthlySalary");
                OnMonthlySalaryChanged();
            }
        }
        private Nullable<Decimal> _MonthlySalary;
        partial void OnMonthlySalaryChanging(Nullable<Decimal> value);
        partial void OnMonthlySalaryChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> DaylySalary
        {
            get
            {
                return _DaylySalary;
            }
            set
            {
                OnDaylySalaryChanging(value);
                ////ReportPropertyChanging("DaylySalary");
                _DaylySalary = value;
                RaisePropertyChanged("DaylySalary");
                OnDaylySalaryChanged();
            }
        }
        private Nullable<Decimal> _DaylySalary;
        partial void OnDaylySalaryChanging(Nullable<Decimal> value);
        partial void OnDaylySalaryChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsGrossSalary
        {
            get
            {
                return _IsGrossSalary;
            }
            set
            {
                OnIsGrossSalaryChanging(value);
                ////ReportPropertyChanging("IsGrossSalary");
                _IsGrossSalary = value;
                RaisePropertyChanged("IsGrossSalary");
                OnIsGrossSalaryChanged();
            }
        }
        private Nullable<Boolean> _IsGrossSalary;
        partial void OnIsGrossSalaryChanging(Nullable<Boolean> value);
        partial void OnIsGrossSalaryChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> AllowancesBenefit
        {
            get
            {
                return _AllowancesBenefit;
            }
            set
            {
                OnAllowancesBenefitChanging(value);
                ////ReportPropertyChanging("AllowancesBenefit");
                _AllowancesBenefit = value;
                RaisePropertyChanged("AllowancesBenefit");
                OnAllowancesBenefitChanged();
            }
        }
        private Nullable<Decimal> _AllowancesBenefit;
        partial void OnAllowancesBenefitChanging(Nullable<Decimal> value);
        partial void OnAllowancesBenefitChanged();





        [DataMemberAttribute()]
        public String ContractNotes
        {
            get
            {
                return _ContractNotes;
            }
            set
            {
                OnContractNotesChanging(value);
                ////ReportPropertyChanging("ContractNotes");
                _ContractNotes = value;
                RaisePropertyChanged("ContractNotes");
                OnContractNotesChanged();
            }
        }
        private String _ContractNotes;
        partial void OnContractNotesChanging(String value);
        partial void OnContractNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFCON_REL_HR14_REFLABOR", "RefLaborContracts")]
        public RefLaborContract RefLaborContract
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFCON_REL_HR15_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }


        #endregion
    }
}
