using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class SurgeryTeam : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new SurgeryTeam object.

        /// <param name="surgeyTeamID">Initial value of the SurgeyTeamID property.</param>
        /// <param name="staffID">Initial value of the StaffID property.</param>
        /// <param name="surgeryID">Initial value of the SurgeryID property.</param>
        public static SurgeryTeam CreateSurgeryTeam(long surgeyTeamID, Int64 staffID, long surgeryID)
        {
            SurgeryTeam surgeryTeam = new SurgeryTeam();
            surgeryTeam.SurgeyTeamID = surgeyTeamID;
            surgeryTeam.StaffID = staffID;
            surgeryTeam.SurgeryID = surgeryID;
            return surgeryTeam;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long SurgeyTeamID
        {
            get
            {
                return _SurgeyTeamID;
            }
            set
            {
                if (_SurgeyTeamID != value)
                {
                    OnSurgeyTeamIDChanging(value);
                    ////ReportPropertyChanging("SurgeyTeamID");
                    _SurgeyTeamID = value;
                    RaisePropertyChanged("SurgeyTeamID");
                    OnSurgeyTeamIDChanged();
                }
            }
        }
        private long _SurgeyTeamID;
        partial void OnSurgeyTeamIDChanging(long value);
        partial void OnSurgeyTeamIDChanged();





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
        public long SurgeryID
        {
            get
            {
                return _SurgeryID;
            }
            set
            {
                OnSurgeryIDChanging(value);
                ////ReportPropertyChanging("SurgeryID");
                _SurgeryID = value;
                RaisePropertyChanged("SurgeryID");
                OnSurgeryIDChanged();
            }
        }
        private long _SurgeryID;
        partial void OnSurgeryIDChanging(long value);
        partial void OnSurgeryIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_SRole
        {
            get
            {
                return _V_SRole;
            }
            set
            {
                OnV_SRoleChanging(value);
                ////ReportPropertyChanging("V_SRole");
                _V_SRole = value;
                RaisePropertyChanged("V_SRole");
                OnV_SRoleChanged();
            }
        }
        private Nullable<Int64> _V_SRole;
        partial void OnV_SRoleChanging(Nullable<Int64> value);
        partial void OnV_SRoleChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> StartDateTime
        {
            get
            {
                return _StartDateTime;
            }
            set
            {
                OnStartDateTimeChanging(value);
                ////ReportPropertyChanging("StartDateTime");
                _StartDateTime = value;
                RaisePropertyChanged("StartDateTime");
                OnStartDateTimeChanged();
            }
        }
        private Nullable<DateTime> _StartDateTime;
        partial void OnStartDateTimeChanging(Nullable<DateTime> value);
        partial void OnStartDateTimeChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> EndDateTime
        {
            get
            {
                return _EndDateTime;
            }
            set
            {
                OnEndDateTimeChanging(value);
                ////ReportPropertyChanging("EndDateTime");
                _EndDateTime = value;
                RaisePropertyChanged("EndDateTime");
                OnEndDateTimeChanged();
            }
        }
        private Nullable<DateTime> _EndDateTime;
        partial void OnEndDateTimeChanging(Nullable<DateTime> value);
        partial void OnEndDateTimeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SURGERYT_REL_PCMD0_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SURGERYT_REL_PCMD0_SURGERIE", "Surgeries")]
        public Surgery Surgery
        {
            get;
            set;
        }

        #endregion
    }
}
