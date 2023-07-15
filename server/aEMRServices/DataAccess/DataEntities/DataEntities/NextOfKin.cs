using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class NextOfKin : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new NextOfKin object.

        /// <param name="rID">Initial value of the RID property.</param>
        public static NextOfKin CreateNextOfKin(long rID)
        {
            NextOfKin nextOfKin = new NextOfKin();
            nextOfKin.RID = rID;
            return nextOfKin;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long RID
        {
            get
            {
                return _RID;
            }
            set
            {
                if (_RID != value)
                {
                    OnRIDChanging(value);
                    ////ReportPropertyChanging("RID");
                    _RID = value;
                    RaisePropertyChanged("RID");
                    OnRIDChanged();
                }
            }
        }
        private long _RID;
        partial void OnRIDChanging(long value);
        partial void OnRIDChanged();





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
        public String RRelationship
        {
            get
            {
                return _RRelationship;
            }
            set
            {
                OnRRelationshipChanging(value);
                ////ReportPropertyChanging("RRelationship");
                _RRelationship = value;
                RaisePropertyChanged("RRelationship");
                OnRRelationshipChanged();
            }
        }
        private String _RRelationship;
        partial void OnRRelationshipChanging(String value);
        partial void OnRRelationshipChanged();





        [DataMemberAttribute()]
        public String RFirstName
        {
            get
            {
                return _RFirstName;
            }
            set
            {
                OnRFirstNameChanging(value);
                ////ReportPropertyChanging("RFirstName");
                _RFirstName = value;
                RaisePropertyChanged("RFirstName");
                OnRFirstNameChanged();
            }
        }
        private String _RFirstName;
        partial void OnRFirstNameChanging(String value);
        partial void OnRFirstNameChanged();





        [DataMemberAttribute()]
        public String RMiddleName
        {
            get
            {
                return _RMiddleName;
            }
            set
            {
                OnRMiddleNameChanging(value);
                ////ReportPropertyChanging("RMiddleName");
                _RMiddleName = value;
                RaisePropertyChanged("RMiddleName");
                OnRMiddleNameChanged();
            }
        }
        private String _RMiddleName;
        partial void OnRMiddleNameChanging(String value);
        partial void OnRMiddleNameChanged();





        [DataMemberAttribute()]
        public String RLastName
        {
            get
            {
                return _RLastName;
            }
            set
            {
                OnRLastNameChanging(value);
                ////ReportPropertyChanging("RLastName");
                _RLastName = value;
                RaisePropertyChanged("RLastName");
                OnRLastNameChanged();
            }
        }
        private String _RLastName;
        partial void OnRLastNameChanging(String value);
        partial void OnRLastNameChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> RDOB
        {
            get
            {
                return _RDOB;
            }
            set
            {
                OnRDOBChanging(value);
                ////ReportPropertyChanging("RDOB");
                _RDOB = value;
                RaisePropertyChanged("RDOB");
                OnRDOBChanged();
            }
        }
        private Nullable<DateTime> _RDOB;
        partial void OnRDOBChanging(Nullable<DateTime> value);
        partial void OnRDOBChanged();





        [DataMemberAttribute()]
        public String RStreetAddress
        {
            get
            {
                return _RStreetAddress;
            }
            set
            {
                OnRStreetAddressChanging(value);
                ////ReportPropertyChanging("RStreetAddress");
                _RStreetAddress = value;
                RaisePropertyChanged("RStreetAddress");
                OnRStreetAddressChanged();
            }
        }
        private String _RStreetAddress;
        partial void OnRStreetAddressChanging(String value);
        partial void OnRStreetAddressChanged();





        [DataMemberAttribute()]
        public String RSurburb
        {
            get
            {
                return _RSurburb;
            }
            set
            {
                OnRSurburbChanging(value);
                ////ReportPropertyChanging("RSurburb");
                _RSurburb = value;
                RaisePropertyChanged("RSurburb");
                OnRSurburbChanged();
            }
        }
        private String _RSurburb;
        partial void OnRSurburbChanging(String value);
        partial void OnRSurburbChanged();





        [DataMemberAttribute()]
        public String RState
        {
            get
            {
                return _RState;
            }
            set
            {
                OnRStateChanging(value);
                ////ReportPropertyChanging("RState");
                _RState = value;
                RaisePropertyChanged("RState");
                OnRStateChanged();
            }
        }
        private String _RState;
        partial void OnRStateChanging(String value);
        partial void OnRStateChanged();





        [DataMemberAttribute()]
        public String RPhoneNumber
        {
            get
            {
                return _RPhoneNumber;
            }
            set
            {
                OnRPhoneNumberChanging(value);
                ////ReportPropertyChanging("RPhoneNumber");
                _RPhoneNumber = value;
                RaisePropertyChanged("RPhoneNumber");
                OnRPhoneNumberChanged();
            }
        }
        private String _RPhoneNumber;
        partial void OnRPhoneNumberChanging(String value);
        partial void OnRPhoneNumberChanged();





        [DataMemberAttribute()]
        public String RMobiPhoneNumber
        {
            get
            {
                return _RMobiPhoneNumber;
            }
            set
            {
                OnRMobiPhoneNumberChanging(value);
                ////ReportPropertyChanging("RMobiPhoneNumber");
                _RMobiPhoneNumber = value;
                RaisePropertyChanged("RMobiPhoneNumber");
                OnRMobiPhoneNumberChanged();
            }
        }
        private String _RMobiPhoneNumber;
        partial void OnRMobiPhoneNumberChanging(String value);
        partial void OnRMobiPhoneNumberChanged();





        [DataMemberAttribute()]
        public String REmailAddress
        {
            get
            {
                return _REmailAddress;
            }
            set
            {
                OnREmailAddressChanging(value);
                ////ReportPropertyChanging("REmailAddress");
                _REmailAddress = value;
                RaisePropertyChanged("REmailAddress");
                OnREmailAddressChanged();
            }
        }
        private String _REmailAddress;
        partial void OnREmailAddressChanging(String value);
        partial void OnREmailAddressChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_NEXTOFKI_REL_HR11_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
