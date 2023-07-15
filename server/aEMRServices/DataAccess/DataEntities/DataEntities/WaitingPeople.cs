using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class WaitingPeople : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new WaitingPeople object.

        /// <param name="wPID">Initial value of the WPID property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        /// <param name="phone">Initial value of the Phone property.</param>
        public static WaitingPeople CreateWaitingPeople(Int32 wPID, String name, String phone)
        {
            WaitingPeople waitingPeople = new WaitingPeople();
            waitingPeople.WPID = wPID;
            waitingPeople.Name = name;
            waitingPeople.Phone = phone;
            return waitingPeople;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int32 WPID
        {
            get
            {
                return _WPID;
            }
            set
            {
                if (_WPID != value)
                {
                    OnWPIDChanging(value);
                    ////ReportPropertyChanging("WPID");
                    _WPID = value;
                    RaisePropertyChanged("WPID");
                    OnWPIDChanged();
                }
            }
        }
        private Int32 _WPID;
        partial void OnWPIDChanging(Int32 value);
        partial void OnWPIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                OnPatientIDChanging(value);
                ////ReportPropertyChanging("PatientID");
                _PatientID = value;
                RaisePropertyChanged("PatientID");
                OnPatientIDChanged();
            }
        }
        private Nullable<long> _PatientID;
        partial void OnPatientIDChanging(Nullable<long> value);
        partial void OnPatientIDChanged();





        [DataMemberAttribute()]
        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ////ReportPropertyChanging("Name");
                _Name = value;
                RaisePropertyChanged("Name");
                OnNameChanged();
            }
        }
        private String _Name;
        partial void OnNameChanging(String value);
        partial void OnNameChanged();





        [DataMemberAttribute()]
        public String Phone
        {
            get
            {
                return _Phone;
            }
            set
            {
                OnPhoneChanging(value);
                ////ReportPropertyChanging("Phone");
                _Phone = value;
                RaisePropertyChanged("Phone");
                OnPhoneChanged();
            }
        }
        private String _Phone;
        partial void OnPhoneChanging(String value);
        partial void OnPhoneChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_WAITINGP_REL_PTAPP_PATIENTS", "Patients")]
        public Patient Patient
        {
            get;
            set;
        }




        #endregion
    }
}
