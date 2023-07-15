using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HealthInsuranceHistory : NotifyChangedBase, IEditableObject
    {
        public HealthInsuranceHistory()
            : base()
        {

        }

        private HealthInsuranceHistory _tempHealthInsuranceHistory;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHealthInsuranceHistory = (HealthInsuranceHistory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHealthInsuranceHistory)
                CopyFrom(_tempHealthInsuranceHistory);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HealthInsuranceHistory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new HealthInsuranceHistory object.

        /// <param name="hisID">Initial value of the HisID property.</param>
        public static HealthInsuranceHistory CreateHealthInsuranceHistory(long hisID)
        {
            HealthInsuranceHistory healthInsuranceHistory = new HealthInsuranceHistory();
            healthInsuranceHistory.HisID = hisID;
            return healthInsuranceHistory;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long HisID
        {
            get
            {
                return _HisID;
            }
            set
            {
                if (_HisID != value)
                {
                    OnHisIDChanging(value);
                    ////ReportPropertyChanging("HisID");
                    _HisID = value;
                    RaisePropertyChanged("HisID");
                    OnHisIDChanged();
                }
            }
        }
        private long _HisID;
        partial void OnHisIDChanging(long value);
        partial void OnHisIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> HIID
        {
            get
            {
                return _HIID;
            }
            set
            {
                OnHIIDChanging(value);
                ////ReportPropertyChanging("HIID");
                _HIID = value;
                RaisePropertyChanged("HIID");
                OnHIIDChanged();
            }
        }
        private Nullable<long> _HIID;
        partial void OnHIIDChanging(Nullable<long> value);
        partial void OnHIIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> FromAppliedDate
        {
            get
            {
                return _FromAppliedDate;
            }
            set
            {
                OnFromAppliedDateChanging(value);
                ////ReportPropertyChanging("FromAppliedDate");
                _FromAppliedDate = value;
                RaisePropertyChanged("FromAppliedDate");
                OnFromAppliedDateChanged();
            }
        }
        private Nullable<DateTime> _FromAppliedDate;
        partial void OnFromAppliedDateChanging(Nullable<DateTime> value);
        partial void OnFromAppliedDateChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> ToAppliedDate
        {
            get
            {
                return _ToAppliedDate;
            }
            set
            {
                OnToAppliedDateChanging(value);
                ////ReportPropertyChanging("ToAppliedDate");
                _ToAppliedDate = value;
                RaisePropertyChanged("ToAppliedDate");
                OnToAppliedDateChanged();
            }
        }
        private Nullable<DateTime> _ToAppliedDate;
        partial void OnToAppliedDateChanging(Nullable<DateTime> value);
        partial void OnToAppliedDateChanged();


       


        [DataMemberAttribute()]
        public Nullable<Boolean> IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                ////ReportPropertyChanging("IsActive");
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private Nullable<Boolean> _IsActive;
        partial void OnIsActiveChanging(Nullable<Boolean> value);
        partial void OnIsActiveChanged();

        #endregion

        #region Navigation Properties


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HEALTHIN_REL_PTINF_HEALTHIN", "HealthInsurance")]
        public HealthInsurance HealthInsurance
        {
            get;
            set;
        }

        #endregion
    }
}
