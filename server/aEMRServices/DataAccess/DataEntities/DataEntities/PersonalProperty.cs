using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PersonalProperty : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PersonalProperty object.

        /// <param name="pPID">Initial value of the PPID property.</param>
        public static PersonalProperty CreatePersonalProperty(Int64 pPID)
        {
            PersonalProperty personalProperty = new PersonalProperty();
            personalProperty.PPID = pPID;
            return personalProperty;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 PPID
        {
            get
            {
                return _PPID;
            }
            set
            {
                if (_PPID != value)
                {
                    OnPPIDChanging(value);
                    ////ReportPropertyChanging("PPID");
                    _PPID = value;
                    RaisePropertyChanged("PPID");
                    OnPPIDChanged();
                }
            }
        }
        private Int64 _PPID;
        partial void OnPPIDChanging(Int64 value);
        partial void OnPPIDChanged();





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
        public String PPTaste
        {
            get
            {
                return _PPTaste;
            }
            set
            {
                OnPPTasteChanging(value);
                ////ReportPropertyChanging("PPTaste");
                _PPTaste = value;
                RaisePropertyChanged("PPTaste");
                OnPPTasteChanged();
            }
        }
        private String _PPTaste;
        partial void OnPPTasteChanging(String value);
        partial void OnPPTasteChanged();





        [DataMemberAttribute()]
        public Nullable<Double> PPHeight
        {
            get
            {
                return _PPHeight;
            }
            set
            {
                OnPPHeightChanging(value);
                ////ReportPropertyChanging("PPHeight");
                _PPHeight = value;
                RaisePropertyChanged("PPHeight");
                OnPPHeightChanged();
            }
        }
        private Nullable<Double> _PPHeight;
        partial void OnPPHeightChanging(Nullable<Double> value);
        partial void OnPPHeightChanged();





        [DataMemberAttribute()]
        public Nullable<Double> PPWeight
        {
            get
            {
                return _PPWeight;
            }
            set
            {
                OnPPWeightChanging(value);
                ////ReportPropertyChanging("PPWeight");
                _PPWeight = value;
                RaisePropertyChanged("PPWeight");
                OnPPWeightChanged();
            }
        }
        private Nullable<Double> _PPWeight;
        partial void OnPPWeightChanging(Nullable<Double> value);
        partial void OnPPWeightChanged();





        [DataMemberAttribute()]
        public String PPHealthStatus
        {
            get
            {
                return _PPHealthStatus;
            }
            set
            {
                OnPPHealthStatusChanging(value);
                ////ReportPropertyChanging("PPHealthStatus");
                _PPHealthStatus = value;
                RaisePropertyChanged("PPHealthStatus");
                OnPPHealthStatusChanged();
            }
        }
        private String _PPHealthStatus;
        partial void OnPPHealthStatusChanging(String value);
        partial void OnPPHealthStatusChanged();





        [DataMemberAttribute()]
        public String PPPersonality
        {
            get
            {
                return _PPPersonality;
            }
            set
            {
                OnPPPersonalityChanging(value);
                ////ReportPropertyChanging("PPPersonality");
                _PPPersonality = value;
                RaisePropertyChanged("PPPersonality");
                OnPPPersonalityChanged();
            }
        }
        private String _PPPersonality;
        partial void OnPPPersonalityChanging(String value);
        partial void OnPPPersonalityChanged();





        [DataMemberAttribute()]
        public String PPPhysiognomy
        {
            get
            {
                return _PPPhysiognomy;
            }
            set
            {
                OnPPPhysiognomyChanging(value);
                ////ReportPropertyChanging("PPPhysiognomy");
                _PPPhysiognomy = value;
                RaisePropertyChanged("PPPhysiognomy");
                OnPPPhysiognomyChanged();
            }
        }
        private String _PPPhysiognomy;
        partial void OnPPPhysiognomyChanging(String value);
        partial void OnPPPhysiognomyChanged();





        [DataMemberAttribute()]
        public String PPBloodGroup
        {
            get
            {
                return _PPBloodGroup;
            }
            set
            {
                OnPPBloodGroupChanging(value);
                ////ReportPropertyChanging("PPBloodGroup");
                _PPBloodGroup = value;
                RaisePropertyChanged("PPBloodGroup");
                OnPPBloodGroupChanged();
            }
        }
        private String _PPBloodGroup;
        partial void OnPPBloodGroupChanging(String value);
        partial void OnPPBloodGroupChanged();





        [DataMemberAttribute()]
        public String PPAptitude
        {
            get
            {
                return _PPAptitude;
            }
            set
            {
                OnPPAptitudeChanging(value);
                ////ReportPropertyChanging("PPAptitude");
                _PPAptitude = value;
                RaisePropertyChanged("PPAptitude");
                OnPPAptitudeChanged();
            }
        }
        private String _PPAptitude;
        partial void OnPPAptitudeChanging(String value);
        partial void OnPPAptitudeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PERSONAL_REL_HR12_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }


        #endregion
    }
}
