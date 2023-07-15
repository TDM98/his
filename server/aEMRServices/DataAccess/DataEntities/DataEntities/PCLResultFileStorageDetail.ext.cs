using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class PCLResultFileStorageDetail : NotifyChangedBase
    {
        #region PatientPCLRequest
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
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private Nullable<long> _ServiceRecID;
        partial void OnServiceRecIDChanging(Nullable<long> value);
        partial void OnServiceRecIDChanged();

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
        public String DoctorComments
        {
            get
            {
                return _DoctorComments;
            }
            set
            {
                OnDoctorCommentsChanging(value);
                _DoctorComments = value;
                RaisePropertyChanged("DoctorComments");
                OnDoctorCommentsChanged();
            }
        }
        private String _DoctorComments;
        partial void OnDoctorCommentsChanging(String value);
        partial void OnDoctorCommentsChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsExternalExam
        {
            get
            {
                return _IsExternalExam;
            }
            set
            {
                OnIsExternalExamChanging(value);
                _IsExternalExam = value;
                RaisePropertyChanged("IsExternalExam");
                OnIsExternalExamChanged();
            }
        }
        private Nullable<Boolean> _IsExternalExam;
        partial void OnIsExternalExamChanging(Nullable<Boolean> value);
        partial void OnIsExternalExamChanged();
        #endregion

        #region PatientPCLExamResult
        [DataMemberAttribute()]
        public Nullable<long> AgencyID
        {
            get
            {
                return _AgencyID;
            }
            set
            {
                OnAgencyIDChanging(value);
                _AgencyID = value;
                RaisePropertyChanged("AgencyID");
                OnAgencyIDChanged();
            }
        }
        private Nullable<long> _AgencyID;
        partial void OnAgencyIDChanging(Nullable<long> value);
        partial void OnAgencyIDChanged();

        [DataMemberAttribute()]
        public string  AgencyNameAddress
        {
            get
            {
                return _AgencyNameAddress;
            }
            set
            {
                OnAgencyNameAddressChanging(value);
                _AgencyNameAddress = value;
                RaisePropertyChanged("AgencyNameAddress");
                OnAgencyNameAddressChanged();
            }
        }
        private string _AgencyNameAddress;
        partial void OnAgencyNameAddressChanging(string value);
        partial void OnAgencyNameAddressChanged();

        [DataMemberAttribute()]
        public DateTime PCLExamDate
        {
            get
            {
                return _PCLExamDate;
            }
            set
            {
                OnPCLExamDateChanging(value);
                _PCLExamDate = value;
                RaisePropertyChanged("PCLExamDate");
                OnPCLExamDateChanged();
            }
        }
        private DateTime _PCLExamDate;
        partial void OnPCLExamDateChanging(DateTime value);
        partial void OnPCLExamDateChanged();

        [DataMemberAttribute()]
        public String DiagnoseOnPCLExam
        {
            get
            {
                return _DiagnoseOnPCLExam;
            }
            set
            {
                OnDiagnoseOnPCLExamChanging(value);
                _DiagnoseOnPCLExam = value;
                RaisePropertyChanged("DiagnoseOnPCLExam");
                OnDiagnoseOnPCLExamChanged();
            }
        }
        private String _DiagnoseOnPCLExam;
        partial void OnDiagnoseOnPCLExamChanging(String value);
        partial void OnDiagnoseOnPCLExamChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> ExamDoctorID
        {
            get
            {
                return _ExamDoctorID;
            }
            set
            {
                OnExamDoctorIDChanging(value);
                _ExamDoctorID = value;
                RaisePropertyChanged("ExamDoctorID");
                OnExamDoctorIDChanged();
            }
        }
        private Nullable<Int64> _ExamDoctorID;
        partial void OnExamDoctorIDChanging(Nullable<Int64> value);
        partial void OnExamDoctorIDChanged();

        [DataMemberAttribute()]
        public String ExamDoctorFullName
        {
            get
            {
                return _ExamDoctorFullName;
            }
            set
            {
                OnExamDoctorFullNameChanging(value);
                _ExamDoctorFullName =value;
                RaisePropertyChanged("ExamDoctorFullName");
                OnExamDoctorFullNameChanged();
            }
        }
        private String _ExamDoctorFullName;
        partial void OnExamDoctorFullNameChanging(String value);
        partial void OnExamDoctorFullNameChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> PCLExamForOutPatient
        {
            get
            {
                return _PCLExamForOutPatient;
            }
            set
            {
                OnPCLExamForOutPatientChanging(value);
                _PCLExamForOutPatient = value;
                RaisePropertyChanged("PCLExamForOutPatient");
                OnPCLExamForOutPatientChanged();
            }
        }
        private Nullable<Boolean> _PCLExamForOutPatient;
        partial void OnPCLExamForOutPatientChanging(Nullable<Boolean> value);
        partial void OnPCLExamForOutPatientChanged();

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
        public String PCLExamTypeCode
        {
            get
            {
                return _PCLExamTypeCode;
            }
            set
            {
                OnPCLExamTypeCodeChanging(value);
                ////ReportPropertyChanging("PCLExamTypeCode");
                _PCLExamTypeCode = value;
                RaisePropertyChanged("PCLExamTypeCode");
                OnPCLExamTypeCodeChanged();
            }
        }
        private String _PCLExamTypeCode;
        partial void OnPCLExamTypeCodeChanging(String value);
        partial void OnPCLExamTypeCodeChanged();

        [DataMemberAttribute()]
        public long? PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {

                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
            }
        }
        private long? _PCLExamTypeID;

        [DataMemberAttribute()]
        public long? PCLExamGroupID
        {
            get
            {
                return _PCLExamGroupID;
            }
            set
            {

                _PCLExamGroupID = value;
                RaisePropertyChanged("PCLExamGroupID");
            }
        }
        private long? _PCLExamGroupID;

        [DataMemberAttribute()]
        public String ResultType
        {
            get
            {
                return _ResultType;
            }
            set
            {
                OnResultTypeChanging(value);
                _ResultType = value;
                RaisePropertyChanged("ResultType");
                OnResultTypeChanged();
            }
        }
        private String _ResultType;
        partial void OnResultTypeChanging(String value);
        partial void OnResultTypeChanged();

        [DataMemberAttribute()]
        public String PathNameOfResource
        {
            get
            {
                return _PathNameOfResource;
            }
            set
            {
                OnPathNameOfResourceChanging(value);
                _PathNameOfResource = value;
                RaisePropertyChanged("PathNameOfResource");
                OnPathNameOfResourceChanged();
            }
        }
        private String _PathNameOfResource;
        partial void OnPathNameOfResourceChanging(String value);
        partial void OnPathNameOfResourceChanged();

        [DataMemberAttribute()]
        public bool IsImage
        {
            get
            {
                return _IsImage;
            }
            set
            {
                OnIsImageChanging(value);
                _IsImage = value;
                RaisePropertyChanged("IsImage");
                OnIsImageChanged();
            }
        }
        private bool _IsImage;
        partial void OnIsImageChanging(bool value);
        partial void OnIsImageChanged();

        [DataMemberAttribute()]
        public bool IsVideo
        {
            get
            {
                return _IsVideo;
            }
            set
            {
                OnIsVideoChanging(value);
                _IsVideo = value;
                RaisePropertyChanged("IsVideo");
                OnIsVideoChanged();
            }
        }
        private bool _IsVideo;
        partial void OnIsVideoChanging(bool value);
        partial void OnIsVideoChanged();

        [DataMemberAttribute()]
        public bool IsDocument
        {
            get
            {
                return _IsDocument;
            }
            set
            {
                OnIsDocumentChanging(value);
                _IsDocument = value;
                RaisePropertyChanged("IsDocument");
                OnIsDocumentChanged();
            }
        }
        private bool _IsDocument;
        partial void OnIsDocumentChanging(bool value);
        partial void OnIsDocumentChanged();

        [DataMemberAttribute()]
        public bool IsOthers
        {
            get
            {
                return _IsOthers;
            }
            set
            {
                OnIsOthersChanging(value);
                _IsOthers = value;
                RaisePropertyChanged("IsOthers");
                OnIsOthersChanged();
            }
        }
        private bool _IsOthers;
        partial void OnIsOthersChanging(bool value);
        partial void OnIsOthersChanged();

        #endregion

        #region PatientServiceRecord
        [DataMemberAttribute()]
        public DateTime PCLRequestDate
        {
            get
            {
                return _PCLRequestDate;
            }
            set
            {
                OnPCLRequestDateChanging(value);
                _PCLRequestDate = value;
                RaisePropertyChanged("PCLRequestDate");
                OnPCLRequestDateChanged();
            }
        }
        private DateTime _PCLRequestDate;
        partial void OnPCLRequestDateChanging(DateTime value);
        partial void OnPCLRequestDateChanged();

        #endregion

    }
}
