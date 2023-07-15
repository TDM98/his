using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class SocialAndHealthInsurance : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new SocialAndHealthInsurance object.

        /// <param name="sHIID">Initial value of the SHIID property.</param>
        public static SocialAndHealthInsurance CreateSocialAndHealthInsurance(Int64 sHIID)
        {
            SocialAndHealthInsurance socialAndHealthInsurance = new SocialAndHealthInsurance();
            socialAndHealthInsurance.SHIID = sHIID;
            return socialAndHealthInsurance;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 SHIID
        {
            get
            {
                return _SHIID;
            }
            set
            {
                if (_SHIID != value)
                {
                    OnSHIIDChanging(value);
                    ////ReportPropertyChanging("SHIID");
                    _SHIID = value;
                    RaisePropertyChanged("SHIID");
                    OnSHIIDChanged();
                }
            }
        }
        private Int64 _SHIID;
        partial void OnSHIIDChanging(Int64 value);
        partial void OnSHIIDChanged();





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
        public String SINumber
        {
            get
            {
                return _SINumber;
            }
            set
            {
                OnSINumberChanging(value);
                ////ReportPropertyChanging("SINumber");
                _SINumber = value;
                RaisePropertyChanged("SINumber");
                OnSINumberChanged();
            }
        }
        private String _SINumber;
        partial void OnSINumberChanging(String value);
        partial void OnSINumberChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> SIIssuedDate
        {
            get
            {
                return _SIIssuedDate;
            }
            set
            {
                OnSIIssuedDateChanging(value);
                ////ReportPropertyChanging("SIIssuedDate");
                _SIIssuedDate = value;
                RaisePropertyChanged("SIIssuedDate");
                OnSIIssuedDateChanged();
            }
        }
        private Nullable<DateTime> _SIIssuedDate;
        partial void OnSIIssuedDateChanging(Nullable<DateTime> value);
        partial void OnSIIssuedDateChanged();





        [DataMemberAttribute()]
        public String SIIssuesPlace
        {
            get
            {
                return _SIIssuesPlace;
            }
            set
            {
                OnSIIssuesPlaceChanging(value);
                ////ReportPropertyChanging("SIIssuesPlace");
                _SIIssuesPlace = value;
                RaisePropertyChanged("SIIssuesPlace");
                OnSIIssuesPlaceChanged();
            }
        }
        private String _SIIssuesPlace;
        partial void OnSIIssuesPlaceChanging(String value);
        partial void OnSIIssuesPlaceChanged();





        [DataMemberAttribute()]
        public String HINumber
        {
            get
            {
                return _HINumber;
            }
            set
            {
                OnHINumberChanging(value);
                ////ReportPropertyChanging("HINumber");
                _HINumber = value;
                RaisePropertyChanged("HINumber");
                OnHINumberChanged();
            }
        }
        private String _HINumber;
        partial void OnHINumberChanging(String value);
        partial void OnHINumberChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> HIIssuedDate
        {
            get
            {
                return _HIIssuedDate;
            }
            set
            {
                OnHIIssuedDateChanging(value);
                ////ReportPropertyChanging("HIIssuedDate");
                _HIIssuedDate = value;
                RaisePropertyChanged("HIIssuedDate");
                OnHIIssuedDateChanged();
            }
        }
        private Nullable<DateTime> _HIIssuedDate;
        partial void OnHIIssuedDateChanging(Nullable<DateTime> value);
        partial void OnHIIssuedDateChanged();





        [DataMemberAttribute()]
        public String HIIssuedPlace
        {
            get
            {
                return _HIIssuedPlace;
            }
            set
            {
                OnHIIssuedPlaceChanging(value);
                ////ReportPropertyChanging("HIIssuedPlace");
                _HIIssuedPlace = value;
                RaisePropertyChanged("HIIssuedPlace");
                OnHIIssuedPlaceChanged();
            }
        }
        private String _HIIssuedPlace;
        partial void OnHIIssuedPlaceChanging(String value);
        partial void OnHIIssuedPlaceChanged();





        [DataMemberAttribute()]
        public String AINumber
        {
            get
            {
                return _AINumber;
            }
            set
            {
                OnAINumberChanging(value);
                ////ReportPropertyChanging("AINumber");
                _AINumber = value;
                RaisePropertyChanged("AINumber");
                OnAINumberChanged();
            }
        }
        private String _AINumber;
        partial void OnAINumberChanging(String value);
        partial void OnAINumberChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> AIIssedDate
        {
            get
            {
                return _AIIssedDate;
            }
            set
            {
                OnAIIssedDateChanging(value);
                ////ReportPropertyChanging("AIIssedDate");
                _AIIssedDate = value;
                RaisePropertyChanged("AIIssedDate");
                OnAIIssedDateChanged();
            }
        }
        private Nullable<DateTime> _AIIssedDate;
        partial void OnAIIssedDateChanging(Nullable<DateTime> value);
        partial void OnAIIssedDateChanged();





        [DataMemberAttribute()]
        public String AIIssuedPlace
        {
            get
            {
                return _AIIssuedPlace;
            }
            set
            {
                OnAIIssuedPlaceChanging(value);
                ////ReportPropertyChanging("AIIssuedPlace");
                _AIIssuedPlace = value;
                RaisePropertyChanged("AIIssuedPlace");
                OnAIIssuedPlaceChanged();
            }
        }
        private String _AIIssuedPlace;
        partial void OnAIIssuedPlaceChanging(String value);
        partial void OnAIIssuedPlaceChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SOCIALAN_REL_HR18_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }


        #endregion
    }
}
