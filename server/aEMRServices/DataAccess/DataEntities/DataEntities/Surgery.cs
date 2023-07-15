using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Surgery : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Surgery object.

        /// <param name="surgeryID">Initial value of the SurgeryID property.</param>
        /// <param name="startSDateTime">Initial value of the StartSDateTime property.</param>
        /// <param name="endSDateTime">Initial value of the EndSDateTime property.</param>
        /// <param name="realitySurgicalMethod">Initial value of the RealitySurgicalMethod property.</param>
        public static Surgery CreateSurgery(long surgeryID, DateTime startSDateTime, DateTime endSDateTime, String realitySurgicalMethod)
        {
            Surgery surgery = new Surgery();
            surgery.SurgeryID = surgeryID;
            surgery.StartSDateTime = startSDateTime;
            surgery.EndSDateTime = endSDateTime;
            surgery.RealitySurgicalMethod = realitySurgicalMethod;
            return surgery;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long SurgeryID
        {
            get
            {
                return _SurgeryID;
            }
            set
            {
                if (_SurgeryID != value)
                {
                    OnSurgeryIDChanging(value);
                    ////ReportPropertyChanging("SurgeryID");
                    _SurgeryID = value;
                    RaisePropertyChanged("SurgeryID");
                    OnSurgeryIDChanged();
                }
            }
        }
        private long _SurgeryID;
        partial void OnSurgeryIDChanging(long value);
        partial void OnSurgeryIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                OnServiceRecIDChanging(value);
                ////ReportPropertyChanging("ServiceRecID");
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private Nullable<long> _ServiceRecID;
        partial void OnServiceRecIDChanging(Nullable<long> value);
        partial void OnServiceRecIDChanged();





        [DataMemberAttribute()]
        public String SMethodCode
        {
            get
            {
                return _SMethodCode;
            }
            set
            {
                OnSMethodCodeChanging(value);
                ////ReportPropertyChanging("SMethodCode");
                _SMethodCode = value;
                RaisePropertyChanged("SMethodCode");
                OnSMethodCodeChanged();
            }
        }
        private String _SMethodCode;
        partial void OnSMethodCodeChanging(String value);
        partial void OnSMethodCodeChanged();





        [DataMemberAttribute()]
        public DateTime StartSDateTime
        {
            get
            {
                return _StartSDateTime;
            }
            set
            {
                OnStartSDateTimeChanging(value);
                ////ReportPropertyChanging("StartSDateTime");
                _StartSDateTime = value;
                RaisePropertyChanged("StartSDateTime");
                OnStartSDateTimeChanged();
            }
        }
        private DateTime _StartSDateTime;
        partial void OnStartSDateTimeChanging(DateTime value);
        partial void OnStartSDateTimeChanged();





        [DataMemberAttribute()]
        public DateTime EndSDateTime
        {
            get
            {
                return _EndSDateTime;
            }
            set
            {
                OnEndSDateTimeChanging(value);
                ////ReportPropertyChanging("EndSDateTime");
                _EndSDateTime = value;
                RaisePropertyChanged("EndSDateTime");
                OnEndSDateTimeChanged();
            }
        }
        private DateTime _EndSDateTime;
        partial void OnEndSDateTimeChanging(DateTime value);
        partial void OnEndSDateTimeChanged();





        [DataMemberAttribute()]
        public String DiagnoseCode
        {
            get
            {
                return _DiagnoseCode;
            }
            set
            {
                OnDiagnoseCodeChanging(value);
                ////ReportPropertyChanging("DiagnoseCode");
                _DiagnoseCode = value;
                RaisePropertyChanged("DiagnoseCode");
                OnDiagnoseCodeChanged();
            }
        }
        private String _DiagnoseCode;
        partial void OnDiagnoseCodeChanging(String value);
        partial void OnDiagnoseCodeChanged();





        [DataMemberAttribute()]
        public String DiagnosedInDept
        {
            get
            {
                return _DiagnosedInDept;
            }
            set
            {
                OnDiagnosedInDeptChanging(value);
                ////ReportPropertyChanging("DiagnosedInDept");
                _DiagnosedInDept = value;
                RaisePropertyChanged("DiagnosedInDept");
                OnDiagnosedInDeptChanged();
            }
        }
        private String _DiagnosedInDept;
        partial void OnDiagnosedInDeptChanging(String value);
        partial void OnDiagnosedInDeptChanged();





        [DataMemberAttribute()]
        public String DiagnoseBefSCode
        {
            get
            {
                return _DiagnoseBefSCode;
            }
            set
            {
                OnDiagnoseBefSCodeChanging(value);
                ////ReportPropertyChanging("DiagnoseBefSCode");
                _DiagnoseBefSCode = value;
                RaisePropertyChanged("DiagnoseBefSCode");
                OnDiagnoseBefSCodeChanged();
            }
        }
        private String _DiagnoseBefSCode;
        partial void OnDiagnoseBefSCodeChanging(String value);
        partial void OnDiagnoseBefSCodeChanged();





        [DataMemberAttribute()]
        public String DiagnosedBefSurgical
        {
            get
            {
                return _DiagnosedBefSurgical;
            }
            set
            {
                OnDiagnosedBefSurgicalChanging(value);
                ////ReportPropertyChanging("DiagnosedBefSurgical");
                _DiagnosedBefSurgical = value;
                RaisePropertyChanged("DiagnosedBefSurgical");
                OnDiagnosedBefSurgicalChanged();
            }
        }
        private String _DiagnosedBefSurgical;
        partial void OnDiagnosedBefSurgicalChanging(String value);
        partial void OnDiagnosedBefSurgicalChanged();





        [DataMemberAttribute()]
        public String DiagnoseAftSCode
        {
            get
            {
                return _DiagnoseAftSCode;
            }
            set
            {
                OnDiagnoseAftSCodeChanging(value);
                ////ReportPropertyChanging("DiagnoseAftSCode");
                _DiagnoseAftSCode = value;
                RaisePropertyChanged("DiagnoseAftSCode");
                OnDiagnoseAftSCodeChanged();
            }
        }
        private String _DiagnoseAftSCode;
        partial void OnDiagnoseAftSCodeChanging(String value);
        partial void OnDiagnoseAftSCodeChanged();





        [DataMemberAttribute()]
        public String DiagnosedAftSurgical
        {
            get
            {
                return _DiagnosedAftSurgical;
            }
            set
            {
                OnDiagnosedAftSurgicalChanging(value);
                ////ReportPropertyChanging("DiagnosedAftSurgical");
                _DiagnosedAftSurgical = value;
                RaisePropertyChanged("DiagnosedAftSurgical");
                OnDiagnosedAftSurgicalChanged();
            }
        }
        private String _DiagnosedAftSurgical;
        partial void OnDiagnosedAftSurgicalChanging(String value);
        partial void OnDiagnosedAftSurgicalChanged();





        [DataMemberAttribute()]
        public String RealitySurgicalMethod
        {
            get
            {
                return _RealitySurgicalMethod;
            }
            set
            {
                OnRealitySurgicalMethodChanging(value);
                ////ReportPropertyChanging("RealitySurgicalMethod");
                _RealitySurgicalMethod = value;
                RaisePropertyChanged("RealitySurgicalMethod");
                OnRealitySurgicalMethodChanged();
            }
        }
        private String _RealitySurgicalMethod;
        partial void OnRealitySurgicalMethodChanging(String value);
        partial void OnRealitySurgicalMethodChanged();





        [DataMemberAttribute()]
        public String InsentitiveMethodCode
        {
            get
            {
                return _InsentitiveMethodCode;
            }
            set
            {
                OnInsentitiveMethodCodeChanging(value);
                ////ReportPropertyChanging("InsentitiveMethodCode");
                _InsentitiveMethodCode = value;
                RaisePropertyChanged("InsentitiveMethodCode");
                OnInsentitiveMethodCodeChanged();
            }
        }
        private String _InsentitiveMethodCode;
        partial void OnInsentitiveMethodCodeChanging(String value);
        partial void OnInsentitiveMethodCodeChanged();





        [DataMemberAttribute()]
        public String InsentitiveMethod
        {
            get
            {
                return _InsentitiveMethod;
            }
            set
            {
                OnInsentitiveMethodChanging(value);
                ////ReportPropertyChanging("InsentitiveMethod");
                _InsentitiveMethod = value;
                RaisePropertyChanged("InsentitiveMethod");
                OnInsentitiveMethodChanged();
            }
        }
        private String _InsentitiveMethod;
        partial void OnInsentitiveMethodChanging(String value);
        partial void OnInsentitiveMethodChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_SurgerySituation
        {
            get
            {
                return _V_SurgerySituation;
            }
            set
            {
                OnV_SurgerySituationChanging(value);
                ////ReportPropertyChanging("V_SurgerySituation");
                _V_SurgerySituation = value;
                RaisePropertyChanged("V_SurgerySituation");
                OnV_SurgerySituationChanged();
            }
        }
        private Nullable<Int64> _V_SurgerySituation;
        partial void OnV_SurgerySituationChanging(Nullable<Int64> value);
        partial void OnV_SurgerySituationChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_SurgicalResults
        {
            get
            {
                return _V_SurgicalResults;
            }
            set
            {
                OnV_SurgicalResultsChanging(value);
                ////ReportPropertyChanging("V_SurgicalResults");
                _V_SurgicalResults = value;
                RaisePropertyChanged("V_SurgicalResults");
                OnV_SurgicalResultsChanged();
            }
        }
        private Nullable<Int64> _V_SurgicalResults;
        partial void OnV_SurgicalResultsChanging(Nullable<Int64> value);
        partial void OnV_SurgicalResultsChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_SurgicalStatus
        {
            get
            {
                return _V_SurgicalStatus;
            }
            set
            {
                OnV_SurgicalStatusChanging(value);
                ////ReportPropertyChanging("V_SurgicalStatus");
                _V_SurgicalStatus = value;
                RaisePropertyChanged("V_SurgicalStatus");
                OnV_SurgicalStatusChanged();
            }
        }
        private Nullable<Int64> _V_SurgicalStatus;
        partial void OnV_SurgicalStatusChanging(Nullable<Int64> value);
        partial void OnV_SurgicalStatusChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALS_REL_PCMD2_SURGERIE", "MedicalSurgicalConsumables")]
        public ObservableCollection<MedicalSurgicalConsumable> MedicalSurgicalConsumables
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SURGERIE_REL_PMR22_PATIENTS", "PatientServiceRecords")]
        public PatientServiceRecord PatientServiceRecord
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SURGERIE_REL_PCMD0_SURGERYM", "SurgeryMethods")]
        public SurgeryMethod SurgeryMethod
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SURGERYT_REL_PCMD0_SURGERIE", "SurgeryTeam")]
        public ObservableCollection<SurgeryTeam> SurgeryTeams
        {
            get;
            set;
        }

        #endregion
    }
}
