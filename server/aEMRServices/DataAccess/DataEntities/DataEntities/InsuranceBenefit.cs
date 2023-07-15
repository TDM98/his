using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class InsuranceBenefit : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new InsuranceBenefit object.

        /// <param name="iBID">Initial value of the IBID property.</param>
        /// <param name="rebatePercentage">Initial value of the RebatePercentage property.</param>
        /// <param name="maxPayable">Initial value of the MaxPayable property.</param>
        /// <param name="maxPayableRemark">Initial value of the MaxPayableRemark property.</param>
        /// <param name="percentageOnMaxPayable">Initial value of the PercentageOnMaxPayable property.</param>
        public static InsuranceBenefit CreateInsuranceBenefit(Int32 iBID, Double rebatePercentage, Decimal maxPayable, String maxPayableRemark, Double percentageOnMaxPayable)
        {
            InsuranceBenefit insuranceBenefit = new InsuranceBenefit();
            insuranceBenefit.IBID = iBID;
            insuranceBenefit.RebatePercentage = rebatePercentage;
            insuranceBenefit.MaxPayable = maxPayable;
            insuranceBenefit.MaxPayableRemark = maxPayableRemark;
            insuranceBenefit.PercentageOnMaxPayable = percentageOnMaxPayable;
            return insuranceBenefit;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int32 IBID
        {
            get
            {
                return _IBID;
            }
            set
            {
                if (_IBID != value)
                {
                    OnIBIDChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _IBID = value;
                    RaisePropertyChanged("IBID");
                    OnIBIDChanged();
                }
            }
        }
        private Int32 _IBID;
        partial void OnIBIDChanging(Int32 value);
        partial void OnIBIDChanged();





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
        public Double RebatePercentage
        {
            get
            {
                return _RebatePercentage;
            }
            set
            {
                OnRebatePercentageChanging(value);
                ////ReportPropertyChanging("RebatePercentage");
                _RebatePercentage = value;
                RaisePropertyChanged("RebatePercentage");
                OnRebatePercentageChanged();
            }
        }
        private Double _RebatePercentage;
        partial void OnRebatePercentageChanging(Double value);
        partial void OnRebatePercentageChanged();





        [DataMemberAttribute()]
        public Decimal MaxPayable
        {
            get
            {
                return _MaxPayable;
            }
            set
            {
                OnMaxPayableChanging(value);
                ////ReportPropertyChanging("MaxPayable");
                _MaxPayable = value;
                RaisePropertyChanged("MaxPayable");
                OnMaxPayableChanged();
            }
        }
        private Decimal _MaxPayable;
        partial void OnMaxPayableChanging(Decimal value);
        partial void OnMaxPayableChanged();





        [DataMemberAttribute()]
        public String MaxPayableRemark
        {
            get
            {
                return _MaxPayableRemark;
            }
            set
            {
                OnMaxPayableRemarkChanging(value);
                ////ReportPropertyChanging("MaxPayableRemark");
                _MaxPayableRemark = value;
                RaisePropertyChanged("MaxPayableRemark");
                OnMaxPayableRemarkChanged();
            }
        }
        private String _MaxPayableRemark;
        partial void OnMaxPayableRemarkChanging(String value);
        partial void OnMaxPayableRemarkChanged();





        [DataMemberAttribute()]
        public Double PercentageOnMaxPayable
        {
            get
            {
                return _PercentageOnMaxPayable;
            }
            set
            {
                OnPercentageOnMaxPayableChanging(value);
                ////ReportPropertyChanging("PercentageOnMaxPayable");
                _PercentageOnMaxPayable = value;
                RaisePropertyChanged("PercentageOnMaxPayable");
                OnPercentageOnMaxPayableChanged();
            }
        }
        private Double _PercentageOnMaxPayable;
        partial void OnPercentageOnMaxPayableChanging(Double value);
        partial void OnPercentageOnMaxPayableChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> UpdateDate
        {
            get
            {
                return _UpdateDate;
            }
            set
            {
                OnUpdateDateChanging(value);
                ////ReportPropertyChanging("UpdateDate");
                _UpdateDate = value;
                RaisePropertyChanged("UpdateDate");
                OnUpdateDateChanged();
            }
        }
        private Nullable<DateTime> _UpdateDate;
        partial void OnUpdateDateChanging(Nullable<DateTime> value);
        partial void OnUpdateDateChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HI_REL_PTINF_IBENF", "HealthInsurance")]
        public ObservableCollection<HealthInsurance> HealthInsurances
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HEALTHIN_REL_PTINF_INSURANC", "HealthInsurancePolicy")]
        public ObservableCollection<HealthInsurancePolicy> HealthInsurancePolicies
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INSURANC_REL_PTINF_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }



        #endregion
    }
}
