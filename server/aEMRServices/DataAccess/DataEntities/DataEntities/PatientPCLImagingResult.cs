/*
 * 20170413 #001 CMN: Add Attribute for Manual Diagnose
 * 20170512 #002 CMN: Added LocationID
 * 20180613 #003 TBLD: Added HIRepResourceCode
 * 20181207 #004 TTM: BM 0005339: Thêm trường BS thực hiện, Ngày thực hiện và họ tên người thực hiện
 * 20210701 #005 TNHX: 260 Thêm trường bsi mượn user
 * 2023067 #006 DatTB: Thêm các trường lưu bệnh phẩm xét nghiệm
*/
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace DataEntities
{
    public partial class PatientPCLImagingResult : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientPCLImagingResults object.

        /// <param name="pLCImgResultID">Initial value of the PLCImgResultID property.</param>
        /// <param name="pCLExamTypeID">Initial value of the PCLExamTypeID property.</param>
        /// <param name="serviceRecID">Initial value of the ServiceRecID property.</param>
        /// <param name="pLCExamDate">Initial value of the PCLExamDate property.</param>
        /// <param name="diagnoseOnPCLExam">Initial value of the DiagnoseOnPCLExam property.</param>
        public static PatientPCLImagingResult CreatePatientPCLImagingResults(long pCLImgResultID, Int64 pCLExamTypeID, DateTime pLCExamDate, String diagnoseOnPCLExam)
        {
            PatientPCLImagingResult patientPCLImagingResults = new PatientPCLImagingResult();
            patientPCLImagingResults.PCLImgResultID = pCLImgResultID;
            patientPCLImagingResults.PCLExamTypeID = pCLExamTypeID;
            patientPCLImagingResults.PCLExamDate = pLCExamDate;
            patientPCLImagingResults.DiagnoseOnPCLExam = diagnoseOnPCLExam;
            return patientPCLImagingResults;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PCLImgResultID
        {
            get
            {
                return _PCLImgResultID;
            }
            set
            {
                if (_PCLImgResultID != value)
                {
                    OnPCLImgResultIDChanging(value);
                    _PCLImgResultID = value;
                    RaisePropertyChanged("PCLImgResultID");
                    OnPCLImgResultIDChanged();
                }
            }
        }
        private long _PCLImgResultID;
        partial void OnPCLImgResultIDChanging(long value);
        partial void OnPCLImgResultIDChanged();

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
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PCLExtRefID
        {
            get
            {
                return _PCLExtRefID;
            }
            set
            {
                OnPCLExtRefIDChanging(value);
                _PCLExtRefID = value;
                RaisePropertyChanged("PCLExtRefID");
                OnPCLExtRefIDChanged();
            }
        }
        private Nullable<long> _PCLExtRefID;
        partial void OnPCLExtRefIDChanging(Nullable<long> value);
        partial void OnPCLExtRefIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                OnPatientPCLReqIDChanging(value);
                _PatientPCLReqID = value;
                RaisePropertyChanged("PatientPCLReqID");
                OnPatientPCLReqIDChanged();
            }
        }
        private Nullable<long> _PatientPCLReqID;
        partial void OnPatientPCLReqIDChanging(Nullable<long> value);
        partial void OnPatientPCLReqIDChanged();

        [DataMemberAttribute()]
        public Int64 PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                OnPCLExamTypeIDChanging(value);
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
                OnPCLExamTypeIDChanged();
            }
        }
        private Int64 _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(Int64 value);
        partial void OnPCLExamTypeIDChanged();

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

        [StringLength(512, MinimumLength = 0, ErrorMessage = "Chẩn đoán <= 512 Ký Tự")]
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

        [StringLength(512, MinimumLength = 0, ErrorMessage = "Chẩn đoán <= 512 Ký Tự")]
        [DataMemberAttribute()]
        public String ResultExplanation
        {
            get
            {
                return _ResultExplanation;
            }
            set
            {
                OnResultExplanationChanging(value);
                _ResultExplanation = value;
                RaisePropertyChanged("ResultExplanation");
                OnResultExplanationChanged();
            }
        }
        private String _ResultExplanation;
        partial void OnResultExplanationChanging(String value);
        partial void OnResultExplanationChanged();

        /*==== #001 ====*/
        [StringLength(50, MinimumLength = 0, ErrorMessage = "Danh sách ICD10 <= 50 Ký Tự")]
        [DataMemberAttribute()]
        public String ICD10List
        {
            get
            {
                return _ICD10List;
            }
            set
            {
                if (_ICD10List != value)
                    _ICD10List = value;
                RaisePropertyChanged("ICD10List");
            }
        }
        private String _ICD10List;

        [DataMemberAttribute()]
        public Double Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                if (_Weight != value)
                    _Weight = value;
                RaisePropertyChanged("Weight");
                CountBSA();
            }
        }
        private Double _Weight;

        [DataMemberAttribute()]
        public Double Height
        {
            get
            {
                return _Height;
            }
            set
            {
                if (_Height != value)
                    _Height = value;
                RaisePropertyChanged("Height");
                CountBSA();
            }
        }
        private Double _Height;

        [DataMemberAttribute()]
        public Double BSA
        {
            get
            {
                return _BSA;
            }
            set
            {
                if (_BSA != value)
                    _BSA = value;
                RaisePropertyChanged("BSA");
            }
        }
        private Double _BSA;

        /*==== #001 ====*/

        /*==== #002 ====*/
        [DataMemberAttribute()]
        public long? DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                _DeptLocationID = value;
                RaisePropertyChanged("DeptLocationID");
            }
        }
        
        private long? _DeptLocationID;
        /*==== #002 ====*/
        /*▼====: #003*/
        [DataMemberAttribute()]
        public string HIRepResourceCode
        {
            get
            {
                return _HIRepResourceCode;
            }
            set
            {
                if (_HIRepResourceCode != value)
                    _HIRepResourceCode = value;
                RaisePropertyChanged("HIRepResourceCode");
            }
        }
        private string _HIRepResourceCode;
        /*▲====: #003*/
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public PatientPCLRequest PatientPCLRequest
        {
            get { return _PatientPCLRequest; }
            set
            {
                if (_PatientPCLRequest != value)
                {
                    _PatientPCLRequest = value;
                    RaisePropertyChanged("PatientPCLRequest");
                }
            }
        }
        private PatientPCLRequest _PatientPCLRequest;

        [DataMemberAttribute()]
        public PCLExamType PCLExamType
        {
            get { return _PCLExamType; }
            set
            {
                if (_PCLExamType != value)
                {
                    _PCLExamType = value;
                    RaisePropertyChanged("PCLExamType");
                }
            }
        }
        private PCLExamType _PCLExamType;


        [DataMemberAttribute()]
        public PCLForExternalRef ForExternalRef
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public Staff Staff
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        private TestingAgency _TestingAgency;
        public TestingAgency TestingAgency
        {
            get
            {
                return _TestingAgency;
            }
            set
            {
                if (_TestingAgency != value)
                {
                    _TestingAgency = value;
                }
                RaisePropertyChanged("TestingAgency");
            }
        }
        
        #endregion
        /*==== #001 ====*/
        private void CountBSA()
        {
            if (Weight > 0 && Height > 0)
            {
                BSA = Math.Round(Math.Pow(Weight, 0.425) * Math.Pow(Height, 0.725) * 71.84 / 10000.0, 2);
            }
            else
                BSA = 0;
        }
        /*==== #001 ====*/

        private string _TemplateResultString;
        [DataMemberAttribute]
        public string TemplateResultString
        {
            get => _TemplateResultString; set
            {
                _TemplateResultString = value;
                RaisePropertyChanged("TemplateResultString");
            }
        }

        private Int16 _NumberOfFilmsReceived;
        [DataMemberAttribute]
        public  Int16 NumberOfFilmsReceived
        {
            get
            {
                return _NumberOfFilmsReceived;
            }
            set
            {
                if(_NumberOfFilmsReceived != value)
                {
                    _NumberOfFilmsReceived = value;
                    RaisePropertyChanged("NumberOfFilmsReceived");
                }
            }
        }

        private string _TemplateResultFileName;
        [DataMemberAttribute]
        public string TemplateResultFileName
        {
            get => _TemplateResultFileName; set
            {
                _TemplateResultFileName = value;
                RaisePropertyChanged("TemplateResultFileName");
            }
        }

        private string _TemplateResultDescription;
        [DataMemberAttribute]
        public string TemplateResultDescription
        {
            get => _TemplateResultDescription; set
            {
                _TemplateResultDescription = value;
                RaisePropertyChanged("TemplateResultDescription");
            }
        }

        private string _TemplateResult;
        [DataMemberAttribute]
        public string TemplateResult
        {
            get => _TemplateResult; set
            {
                _TemplateResult = value;
                RaisePropertyChanged("TemplateResult");
            }
        }

        private string _PtRegistrationCode;
        [DataMemberAttribute]
        public string PtRegistrationCode
        {
            get => _PtRegistrationCode; set
            {
                _PtRegistrationCode = value;
                RaisePropertyChanged("PtRegistrationCode");
            }
        }

        //▼====== #004
        private long? _PerformStaffID;
        [DataMemberAttribute]
        public long? PerformStaffID
        {
            get { return _PerformStaffID; }
            set
            {
                if (_PerformStaffID != value)
                {
                    _PerformStaffID = value;
                }
                RaisePropertyChanged("PerformStaffID");
            }
        }
        private string _PerformStaffFullName;
        [DataMemberAttribute]
        public string PerformStaffFullName
        {
            get { return _PerformStaffFullName; }
            set
            {
                if (_PerformStaffFullName != value)
                {
                    _PerformStaffFullName = value;
                }
                RaisePropertyChanged("PerformStaffFullName");
            }
        }
        private DateTime _PerformedDate;
        [DataMemberAttribute]
        public DateTime PerformedDate
        {
            get { return _PerformedDate; }
            set
            {
                if (_PerformedDate != value)
                {
                    _PerformedDate = value;
                }
                RaisePropertyChanged("PerformedDate");
            }
        }
        //▲====== #004
        [DataMemberAttribute()]
        public string Suggest
        {
            get
            {
                return _Suggest;
            }
            set
            {
                if (_Suggest != value)
                    _Suggest = value;
                RaisePropertyChanged("Suggest");
            }
        }
        private string _Suggest;

        private long? _ResultStaffID;
        [DataMemberAttribute]
        public long? ResultStaffID
        {
            get { return _ResultStaffID; }
            set
            {
                if (_ResultStaffID != value)
                {
                    _ResultStaffID = value;
                }
                RaisePropertyChanged("ResultStaffID");
            }
        }
        private string _ResultStaffFullName;
        [DataMemberAttribute]
        public string ResultStaffFullName
        {
            get { return _ResultStaffFullName; }
            set
            {
                if (_ResultStaffFullName != value)
                {
                    _ResultStaffFullName = value;
                }
                RaisePropertyChanged("ResultStaffFullName");
            }
        }
        //▼====: #005
        private long? _UserOfficialAccountID;
        [DataMemberAttribute]
        public long? UserOfficialAccountID
        {
            get { return _UserOfficialAccountID; }
            set
            {
                if (_UserOfficialAccountID != value)
                {
                    _UserOfficialAccountID = value;
                }
                RaisePropertyChanged("UserOfficialAccountID");
            }
        }
        //▲====: #005
        [DataMemberAttribute()]
        public List<PatientPCLImagingResultDetail> PatientPCLImagingResultDetail
        {
            get
            {
                return _PatientPCLImagingResultDetail;
            }
            set
            {
                if (_PatientPCLImagingResultDetail != value)
                    _PatientPCLImagingResultDetail = value;
                RaisePropertyChanged("PatientPCLImagingResultDetail");
            }
        }
        private List<PatientPCLImagingResultDetail> _PatientPCLImagingResultDetail;

        //▼==== #006
        private long _SpecimenID;
        [DataMemberAttribute]
        public long SpecimenID
        {
            get { return _SpecimenID; }
            set
            {
                if (_SpecimenID != value)
                {
                    _SpecimenID = value;
                    RaisePropertyChanged("SpecimenID");
                }
            }
        }

        private string _SampleQuality;
        [DataMemberAttribute]
        public string SampleQuality
        {
            get { return _SampleQuality; }
            set
            {
                if (_SampleQuality != value)
                {
                    _SampleQuality = value;
                    RaisePropertyChanged("SampleQuality");
                }
            }
        }
        //▲==== #006
    }
}