using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Reward : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Reward object.

        /// <param name="rDID">Initial value of the RDID property.</param>
        public static Reward CreateReward(long rDID)
        {
            Reward reward = new Reward();
            reward.RDID = rDID;
            return reward;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long RDID
        {
            get
            {
                return _RDID;
            }
            set
            {
                if (_RDID != value)
                {
                    OnRDIDChanging(value);
                    ////ReportPropertyChanging("RDID");
                    _RDID = value;
                    RaisePropertyChanged("RDID");
                    OnRDIDChanged();
                }
            }
        }
        private long _RDID;
        partial void OnRDIDChanging(long value);
        partial void OnRDIDChanged();





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
        public String RDAchievement
        {
            get
            {
                return _RDAchievement;
            }
            set
            {
                OnRDAchievementChanging(value);
                ////ReportPropertyChanging("RDAchievement");
                _RDAchievement = value;
                RaisePropertyChanged("RDAchievement");
                OnRDAchievementChanged();
            }
        }
        private String _RDAchievement;
        partial void OnRDAchievementChanging(String value);
        partial void OnRDAchievementChanged();





        [DataMemberAttribute()]
        public String RDType
        {
            get
            {
                return _RDType;
            }
            set
            {
                OnRDTypeChanging(value);
                ////ReportPropertyChanging("RDType");
                _RDType = value;
                RaisePropertyChanged("RDType");
                OnRDTypeChanged();
            }
        }
        private String _RDType;
        partial void OnRDTypeChanging(String value);
        partial void OnRDTypeChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> RDAmount
        {
            get
            {
                return _RDAmount;
            }
            set
            {
                OnRDAmountChanging(value);
                ////ReportPropertyChanging("RDAmount");
                _RDAmount = value;
                RaisePropertyChanged("RDAmount");
                OnRDAmountChanged();
            }
        }
        private Nullable<Decimal> _RDAmount;
        partial void OnRDAmountChanging(Nullable<Decimal> value);
        partial void OnRDAmountChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> RDDate
        {
            get
            {
                return _RDDate;
            }
            set
            {
                OnRDDateChanging(value);
                ////ReportPropertyChanging("RDDate");
                _RDDate = value;
                RaisePropertyChanged("RDDate");
                OnRDDateChanged();
            }
        }
        private Nullable<DateTime> _RDDate;
        partial void OnRDDateChanging(Nullable<DateTime> value);
        partial void OnRDDateChanged();





        [DataMemberAttribute()]
        public String RDNotes
        {
            get
            {
                return _RDNotes;
            }
            set
            {
                OnRDNotesChanging(value);
                ////ReportPropertyChanging("RDNotes");
                _RDNotes = value;
                RaisePropertyChanged("RDNotes");
                OnRDNotesChanged();
            }
        }
        private String _RDNotes;
        partial void OnRDNotesChanging(String value);
        partial void OnRDNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REWARD_REL_HR07_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
