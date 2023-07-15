using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class EmergencyRecord : NotifyChangedBase, IEditableObject
    {
        public EmergencyRecord()
            : base()
        {

        }

        private EmergencyRecord _tempEmergencyRecord;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempEmergencyRecord = (EmergencyRecord)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempEmergencyRecord)
                CopyFrom(_tempEmergencyRecord);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(EmergencyRecord p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new EmergencyRecord object.

        /// <param name="emergRecID">Initial value of the EmergRecID property.</param>
        /// <param name="acDateTime">Initial value of the AcDateTime property.</param>
        /// <param name="cause">Initial value of the Cause property.</param>
        /// <param name="v_InjuredPartsOfBody">Initial value of the V_InjuredPartsOfBody property.</param>
        public static EmergencyRecord CreateEmergencyRecord(long emergRecID, DateTime acDateTime, String cause, Int64 v_InjuredPartsOfBody)
        {
            EmergencyRecord emergencyRecord = new EmergencyRecord();
            emergencyRecord.EmergRecID = emergRecID;
            emergencyRecord.AcDateTime = acDateTime;
            emergencyRecord.Cause = cause;
            emergencyRecord.V_InjuredPartsOfBody = v_InjuredPartsOfBody;
            return emergencyRecord;
        }

        #endregion
        #region Primitive Properties



        private long? _PatientID;
        [DataMemberAttribute()]
        public long? PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }

        private Patient _Patient;
        [DataMemberAttribute()]
        public Patient Patient
        {
            get
            {
                return _Patient;
            }
            set
            {
                _Patient = value;
                RaisePropertyChanged("Patient");
            }
        }

        [DataMemberAttribute()]
        public long EmergRecID
        {
            get
            {
                return _EmergRecID;
            }
            set
            {
                if (_EmergRecID != value)
                {
                    OnEmergRecIDChanging(value);
                    ////ReportPropertyChanging("EmergRecID");
                    _EmergRecID = value;
                    RaisePropertyChanged("EmergRecID");
                    OnEmergRecIDChanged();
                }
            }
        }
        private long _EmergRecID;
        partial void OnEmergRecIDChanging(long value);
        partial void OnEmergRecIDChanged();





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
        public Nullable<long> CityProvinceID
        {
            get
            {
                return _CityProvinceID;
            }
            set
            {
                OnCityProvinceIDChanging(value);
                ////ReportPropertyChanging("CityProvinceID");
                _CityProvinceID = value;
                RaisePropertyChanged("CityProvinceID");
                OnCityProvinceIDChanged();
            }
        }
        private Nullable<long> _CityProvinceID;
        partial void OnCityProvinceIDChanging(Nullable<long> value);
        partial void OnCityProvinceIDChanged();





        [DataMemberAttribute()]
        public DateTime AcDateTime
        {
            get
            {
                return _AcDateTime;
            }
            set
            {
                OnAcDateTimeChanging(value);
                ////ReportPropertyChanging("AcDateTime");
                _AcDateTime = value;
                RaisePropertyChanged("AcDateTime");
                OnAcDateTimeChanged();
            }
        }
        private DateTime _AcDateTime;
        partial void OnAcDateTimeChanging(DateTime value);
        partial void OnAcDateTimeChanged();





        [DataMemberAttribute()]
        public String AcPlaceStreetAddress
        {
            get
            {
                return _AcPlaceStreetAddress;
            }
            set
            {
                OnAcPlaceStreetAddressChanging(value);
                ////ReportPropertyChanging("AcPlaceStreetAddress");
                _AcPlaceStreetAddress = value;
                RaisePropertyChanged("AcPlaceStreetAddress");
                OnAcPlaceStreetAddressChanged();
            }
        }
        private String _AcPlaceStreetAddress;
        partial void OnAcPlaceStreetAddressChanging(String value);
        partial void OnAcPlaceStreetAddressChanged();





        [DataMemberAttribute()]
        public String AcPlaceSurburb
        {
            get
            {
                return _AcPlaceSurburb;
            }
            set
            {
                OnAcPlaceSurburbChanging(value);
                ////ReportPropertyChanging("AcPlaceSurburb");
                _AcPlaceSurburb = value;
                RaisePropertyChanged("AcPlaceSurburb");
                OnAcPlaceSurburbChanged();
            }
        }
        private String _AcPlaceSurburb;
        partial void OnAcPlaceSurburbChanging(String value);
        partial void OnAcPlaceSurburbChanged();





        [DataMemberAttribute()]
        public String AccidentPlace
        {
            get
            {
                return _AccidentPlace;
            }
            set
            {
                OnAccidentPlaceChanging(value);
                ////ReportPropertyChanging("AccidentPlace");
                _AccidentPlace = value;
                RaisePropertyChanged("AccidentPlace");
                OnAccidentPlaceChanged();
            }
        }
        private String _AccidentPlace;
        partial void OnAccidentPlaceChanging(String value);
        partial void OnAccidentPlaceChanged();





        [DataMemberAttribute()]
        public String Cause
        {
            get
            {
                return _Cause;
            }
            set
            {
                OnCauseChanging(value);
                ////ReportPropertyChanging("Cause");
                _Cause = value;
                RaisePropertyChanged("Cause");
                OnCauseChanged();
            }
        }
        private String _Cause;
        partial void OnCauseChanging(String value);
        partial void OnCauseChanged();





        [DataMemberAttribute()]
        public Int64 V_InjuredPartsOfBody
        {
            get
            {
                return _V_InjuredPartsOfBody;
            }
            set
            {
                OnV_InjuredPartsOfBodyChanging(value);
                ////ReportPropertyChanging("V_InjuredPartsOfBody");
                _V_InjuredPartsOfBody = value;
                RaisePropertyChanged("V_InjuredPartsOfBody");
                OnV_InjuredPartsOfBodyChanged();
            }
        }
        private Int64 _V_InjuredPartsOfBody;
        partial void OnV_InjuredPartsOfBodyChanging(Int64 value);
        partial void OnV_InjuredPartsOfBodyChanged();





        [DataMemberAttribute()]
        public String BePoisoned
        {
            get
            {
                return _BePoisoned;
            }
            set
            {
                OnBePoisonedChanging(value);
                ////ReportPropertyChanging("BePoisoned");
                _BePoisoned = value;
                RaisePropertyChanged("BePoisoned");
                OnBePoisonedChanged();
            }
        }
        private String _BePoisoned;
        partial void OnBePoisonedChanging(String value);
        partial void OnBePoisonedChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_ChangesAfterAc
        {
            get
            {
                return _V_ChangesAfterAc;
            }
            set
            {
                OnV_ChangesAfterAcChanging(value);
                ////ReportPropertyChanging("V_ChangesAfterAc");
                _V_ChangesAfterAc = value;
                RaisePropertyChanged("V_ChangesAfterAc");
                OnV_ChangesAfterAcChanged();
            }
        }
        private Nullable<Int64> _V_ChangesAfterAc;
        partial void OnV_ChangesAfterAcChanging(Nullable<Int64> value);
        partial void OnV_ChangesAfterAcChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_BehaveAfterAc
        {
            get
            {
                return _V_BehaveAfterAc;
            }
            set
            {
                OnV_BehaveAfterAcChanging(value);
                ////ReportPropertyChanging("V_BehaveAfterAc");
                _V_BehaveAfterAc = value;
                RaisePropertyChanged("V_BehaveAfterAc");
                OnV_BehaveAfterAcChanged();
            }
        }
        private Nullable<Int64> _V_BehaveAfterAc;
        partial void OnV_BehaveAfterAcChanging(Nullable<Int64> value);
        partial void OnV_BehaveAfterAcChanged();





        [DataMemberAttribute()]
        public String HospitalizationReason
        {
            get
            {
                return _HospitalizationReason;
            }
            set
            {
                OnHospitalizationReasonChanging(value);
                ////ReportPropertyChanging("HospitalizationReason");
                _HospitalizationReason = value;
                RaisePropertyChanged("HospitalizationReason");
                OnHospitalizationReasonChanged();
            }
        }
        private String _HospitalizationReason;
        partial void OnHospitalizationReasonChanging(String value);
        partial void OnHospitalizationReasonChanged();





        [DataMemberAttribute()]
        public String Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                OnDiagnosisChanging(value);
                ////ReportPropertyChanging("Diagnosis");
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
                OnDiagnosisChanged();
            }
        }
        private String _Diagnosis;
        partial void OnDiagnosisChanging(String value);
        partial void OnDiagnosisChanged();





        [DataMemberAttribute()]
        public String Treatment
        {
            get
            {
                return _Treatment;
            }
            set
            {
                OnTreatmentChanging(value);
                ////ReportPropertyChanging("Treatment");
                _Treatment = value;
                RaisePropertyChanged("Treatment");
                OnTreatmentChanged();
            }
        }
        private String _Treatment;
        partial void OnTreatmentChanging(String value);
        partial void OnTreatmentChanged();





        [DataMemberAttribute()]
        public String InjuryStatusBefHos
        {
            get
            {
                return _InjuryStatusBefHos;
            }
            set
            {
                OnInjuryStatusBefHosChanging(value);
                ////ReportPropertyChanging("InjuryStatusBefHos");
                _InjuryStatusBefHos = value;
                RaisePropertyChanged("InjuryStatusBefHos");
                OnInjuryStatusBefHosChanged();
            }
        }
        private String _InjuryStatusBefHos;
        partial void OnInjuryStatusBefHosChanging(String value);
        partial void OnInjuryStatusBefHosChanged();





        [DataMemberAttribute()]
        public String InjuryStatusAftDischarge
        {
            get
            {
                return _InjuryStatusAftDischarge;
            }
            set
            {
                OnInjuryStatusAftDischargeChanging(value);
                ////ReportPropertyChanging("InjuryStatusAftDischarge");
                _InjuryStatusAftDischarge = value;
                RaisePropertyChanged("InjuryStatusAftDischarge");
                OnInjuryStatusAftDischargeChanged();
            }
        }
        private String _InjuryStatusAftDischarge;
        partial void OnInjuryStatusAftDischargeChanging(String value);
        partial void OnInjuryStatusAftDischargeChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EMERGENC_REL_AEREC_CITIESPR", "CitiesProvinces")]
        public CitiesProvince CitiesProvince
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EMERGENC_REL_AEREC_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTR_REL_AEREC_EMERGENC", "PatientRegistration")]
        public ObservableCollection<PatientRegistration> PatientRegistrations
        {
            get;
            set;
        }

        #endregion
    }
}
