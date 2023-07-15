using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class PCLForExternalRef : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLForExternalRef object.

        /// <param name="PCLForExternalRef">Initial value of the PCLForExternalRef property.</param>
        /// <param name="request">Initial value of the Request property.</param>
        /// <param name="requestDate">Initial value of the RequestDate property.</param>
        public static PCLForExternalRef CreatePCLForExternalRef(long pCLExtRefID, String request, DateTime requestDate)
        {
            PCLForExternalRef PCLForExternalRef = new PCLForExternalRef();
            PCLForExternalRef.PCLExtRefID = pCLExtRefID;
            PCLForExternalRef.Request = request;
            PCLForExternalRef.RequestDate = requestDate;
            return PCLForExternalRef;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PCLExtRefID
        {
            get
            {
                return _PCLExtRefID;
            }
            set
            {
                if (_PCLExtRefID != value)
                {
                    OnPCLExtRefIDChanging(value);
                    _PCLExtRefID = value;
                    RaisePropertyChanged("PCLExtRefID");
                    OnPCLExtRefIDChanged();
                }
            }
        }
        private long _PCLExtRefID;
        partial void OnPCLExtRefIDChanging(long value);
        partial void OnPCLExtRefIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PCLFormID
        {
            get
            {
                return _PCLFormID;
            }
            set
            {
                OnPCLFormIDChanging(value);
                _PCLFormID = value;
                RaisePropertyChanged("PCLFormID");
                OnPCLFormIDChanged();
            }
        }
        private Nullable<long> _PCLFormID;
        partial void OnPCLFormIDChanging(Nullable<long> value);
        partial void OnPCLFormIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                OnPtRegDetailIDChanging(value);
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
                OnPtRegDetailIDChanged();
            }
        }
        private Nullable<Int64> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<Int64> value);
        partial void OnPtRegDetailIDChanged();

        [DataMemberAttribute()]
        public String RequestDoctor
        {
            get
            {
                return _RequestDoctor;
            }
            set
            {
                OnRequestDoctorChanging(value);
                _RequestDoctor = value;
                RaisePropertyChanged("RequestDoctor");
                OnRequestDoctorChanged();
            }
        }
        private String _RequestDoctor;
        partial void OnRequestDoctorChanging(String value);
        partial void OnRequestDoctorChanged();


        [DataMemberAttribute()]
        public String FromHospitalOrClinic
        {
            get
            {
                return _FromHospitalOrClinic;
            }
            set
            {
                OnFromHospitalOrClinicChanging(value);
                _FromHospitalOrClinic = value;
                RaisePropertyChanged("FromHospitalOrClinic");
                OnFromHospitalOrClinicChanged();
            }
        }
        private String _FromHospitalOrClinic;
        partial void OnFromHospitalOrClinicChanging(String value);
        partial void OnFromHospitalOrClinicChanged();

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
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
                OnDiagnosisChanged();
            }
        }
        private String _Diagnosis;
        partial void OnDiagnosisChanging(String value);
        partial void OnDiagnosisChanged();

        [DataMemberAttribute()]
        public String Request
        {
            get
            {
                return _Request;
            }
            set
            {
                OnRequestChanging(value);
                _Request = value;
                RaisePropertyChanged("Request");
                OnRequestChanged();
            }
        }
        private String _Request;
        partial void OnRequestChanging(String value);
        partial void OnRequestChanged();

        [DataMemberAttribute()]
        public String TestSample
        {
            get
            {
                return _TestSample;
            }
            set
            {
                OnTestSampleChanging(value);
                _TestSample = value;
                RaisePropertyChanged("TestSample");
                OnTestSampleChanged();
            }
        }
        private String _TestSample;
        partial void OnTestSampleChanging(String value);
        partial void OnTestSampleChanged();

        [DataMemberAttribute()]
        public DateTime RequestDate
        {
            get
            {
                return _RequestDate;
            }
            set
            {
                OnRequestDateChanging(value);
                _RequestDate = value;
                RaisePropertyChanged("RequestDate");
                OnRequestDateChanged();
            }
        }
        private DateTime _RequestDate;
        partial void OnRequestDateChanging(DateTime value);
        partial void OnRequestDateChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLImagingResult> PatientPCLExamResults
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public PatientRegistrationDetail PatientRegistrationDetail
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public PCLForm PCLForm
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 31-08-2012 Dinh
        /// Thêm trạng thái để phân biệt nội trú và ngoại trú
        /// </summary>
        private RegistrationType _RegistrationType;
        [DataMemberAttribute()]
        public RegistrationType RegistrationType
        {
            get
            {
                return _RegistrationType;
            }
            set
            {
                _RegistrationType = value;
                RaisePropertyChanged("RegistrationType");
            }
        }


        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
    }
}
