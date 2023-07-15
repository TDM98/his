using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefAllowance : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefAllowance object.

        /// <param name="allowanceID">Initial value of the AllowanceID property.</param>
        /// <param name="allowancesName">Initial value of the AllowancesName property.</param>
        /// <param name="alAmount">Initial value of the AlAmount property.</param>
        /// <param name="alCoefficient">Initial value of the AlCoefficient property.</param>
        /// <param name="alActive">Initial value of the AlActive property.</param>
        public static RefAllowance CreateRefAllowance(long allowanceID, String allowancesName, Decimal alAmount, Double alCoefficient, Boolean alActive)
        {
            RefAllowance refAllowance = new RefAllowance();
            refAllowance.AllowanceID = allowanceID;
            refAllowance.AllowancesName = allowancesName;
            refAllowance.AlAmount = alAmount;
            refAllowance.AlCoefficient = alCoefficient;
            refAllowance.AlActive = alActive;
            return refAllowance;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long AllowanceID
        {
            get
            {
                return _AllowanceID;
            }
            set
            {
                if (_AllowanceID != value)
                {
                    OnAllowanceIDChanging(value);
                    ////ReportPropertyChanging("AllowanceID");
                    _AllowanceID = value;
                    RaisePropertyChanged("AllowanceID");
                    OnAllowanceIDChanged();
                }
            }
        }
        private long _AllowanceID;
        partial void OnAllowanceIDChanging(long value);
        partial void OnAllowanceIDChanged();





        [DataMemberAttribute()]
        public String AllowancesName
        {
            get
            {
                return _AllowancesName;
            }
            set
            {
                OnAllowancesNameChanging(value);
                ////ReportPropertyChanging("AllowancesName");
                _AllowancesName = value;
                RaisePropertyChanged("AllowancesName");
                OnAllowancesNameChanged();
            }
        }
        private String _AllowancesName;
        partial void OnAllowancesNameChanging(String value);
        partial void OnAllowancesNameChanged();





        [DataMemberAttribute()]
        public Decimal AlAmount
        {
            get
            {
                return _AlAmount;
            }
            set
            {
                OnAlAmountChanging(value);
                ////ReportPropertyChanging("AlAmount");
                _AlAmount = value;
                RaisePropertyChanged("AlAmount");
                OnAlAmountChanged();
            }
        }
        private Decimal _AlAmount;
        partial void OnAlAmountChanging(Decimal value);
        partial void OnAlAmountChanged();





        [DataMemberAttribute()]
        public Double AlCoefficient
        {
            get
            {
                return _AlCoefficient;
            }
            set
            {
                OnAlCoefficientChanging(value);
                ////ReportPropertyChanging("AlCoefficient");
                _AlCoefficient = value;
                RaisePropertyChanged("AlCoefficient");
                OnAlCoefficientChanged();
            }
        }
        private Double _AlCoefficient;
        partial void OnAlCoefficientChanging(Double value);
        partial void OnAlCoefficientChanged();





        [DataMemberAttribute()]
        public String AlNotes
        {
            get
            {
                return _AlNotes;
            }
            set
            {
                OnAlNotesChanging(value);
                ////ReportPropertyChanging("AlNotes");
                _AlNotes = value;
                RaisePropertyChanged("AlNotes");
                OnAlNotesChanged();
            }
        }
        private String _AlNotes;
        partial void OnAlNotesChanging(String value);
        partial void OnAlNotesChanged();





        [DataMemberAttribute()]
        public Boolean AlActive
        {
            get
            {
                return _AlActive;
            }
            set
            {
                OnAlActiveChanging(value);
                ////ReportPropertyChanging("AlActive");
                _AlActive = value;
                RaisePropertyChanged("AlActive");
                OnAlActiveChanged();
            }
        }
        private Boolean _AlActive;
        partial void OnAlActiveChanging(Boolean value);
        partial void OnAlActiveChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFALL_REL_HR20_REFALLOW", "StaffAllowance")]
        public ObservableCollection<StaffAllowance> StaffAllowances
        {
            get;
            set;
        }

        #endregion
    }
}
