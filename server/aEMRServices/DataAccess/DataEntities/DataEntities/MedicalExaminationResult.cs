using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
/*
 * 20200717 #001 TNHX: Thêm khám sản
 * 20220602 #002 DattB:  IssueID: 1619 | Thêm các biến kết luận
 */
namespace DataEntities
{
    public class MedicalExaminationResult : NotifyChangedBase
    {
        private long _MedicalExaminationResultID;
        private string _MedicalExaminationHistory;
        private string _CirculationTestResult;
        private long _Circulation_V_HealthClass = 83200;
        private string _RespiratoryTestResult;
        private long _Respiratory_V_HealthClass = 83200;
        private string _DigestionTestResult;
        private long _Digestion_V_HealthClass = 83200;
        private string _UrologyTestResult;
        private long _Urology_V_HealthClass = 83200;
        private string _EndocrineTestResult;
        private long _Endocrine_V_HealthClass = 83200;
        private string _OrthopaedicsTestResult;
        private long _Orthopaedics_V_HealthClass = 83200;
        private string _NeurologyTestResult;
        private long _Neurology_V_HealthClass = 83200;
        private string _NeuropsychiatricTestResult;
        private long _Neuropsychiatric_V_HealthClass = 83200;
        private string _GeneralSugeryTestResult;
        private long _GeneralSugery_V_HealthClass = 83200;
        private string _LeftOptometryTestResult;
        private string _RightOptometryTestResult;
        private string _LeftSightedOptometryTestResult;
        private string _RightSightedOptometryTestResult;
        private string _OptometryDecreases;
        private long _Optometry_V_HealthClass = 83200;
        private string _LeftHearingTestResult;
        private string _RightHearingTestResult;
        private string _LeftSilentlyHearingTestResult;
        private string _RightSilentlyHearingTestResult;
        private string _EarAndThroatDecreases;
        private long _EarAndThroatDecreases_V_HealthClass = 83200;
        private string _UpperJawTestResult;
        private string _LowerJawTestResult;
        private string _DentalAndJawDecreases;
        private long _DentalAndJaw_V_HealthClass = 83200;
        private string _DermatologyTestResult;
        private long _Dermatology_V_HealthClass = 83200;
        private Staff _InternalMedicalResultStaff;
        private Staff _GeneralSugeryResultStaff;
        private Staff _OptometryResultStaff;
        private Staff _EarAndThroatResultStaff;
        private Staff _DentalAndJawResultStaff;
        private Staff _DermatologyResultStaff;
        private long _PtRegDetailID;
        private long _PtRegistrationID;

        private string _ObstetricTestResult;
        private long _Obstetric_V_HealthClass = 83200;
        private Staff _ObstetricResultStaff;
        //▼==== #002
        private long _HealthClassification;
        private string _Diseases;
        private string _Record;
        private string _CurrentHealth;
        private DateTime _HealthCheckUpDate;
        private DateTime _ExpiryDateHealthCertificate;
        //▲==== #002

        [DataMemberAttribute]
        public long MedicalExaminationResultID
        {
            get
            {
                return _MedicalExaminationResultID;
            }
            set
            {
                _MedicalExaminationResultID = value;
                RaisePropertyChanged("MedicalExaminationResultID");
            }
        }
        [DataMemberAttribute]
        public string MedicalExaminationHistory
        {
            get
            {
                return _MedicalExaminationHistory;
            }
            set
            {
                _MedicalExaminationHistory = value;
                RaisePropertyChanged("MedicalExaminationHistory");
            }
        }
        [DataMemberAttribute]
        public string CirculationTestResult
        {
            get
            {
                return _CirculationTestResult;
            }
            set
            {
                _CirculationTestResult = value;
                RaisePropertyChanged("CirculationTestResult");
            }
        }
        [DataMemberAttribute]
        public long Circulation_V_HealthClass
        {
            get
            {
                return _Circulation_V_HealthClass;
            }
            set
            {
                _Circulation_V_HealthClass = value;
                RaisePropertyChanged("Circulation_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string RespiratoryTestResult
        {
            get
            {
                return _RespiratoryTestResult;
            }
            set
            {
                _RespiratoryTestResult = value;
                RaisePropertyChanged("RespiratoryTestResult");
            }
        }
        [DataMemberAttribute]
        public long Respiratory_V_HealthClass
        {
            get
            {
                return _Respiratory_V_HealthClass;
            }
            set
            {
                _Respiratory_V_HealthClass = value;
                RaisePropertyChanged("Respiratory_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string DigestionTestResult
        {
            get
            {
                return _DigestionTestResult;
            }
            set
            {
                _DigestionTestResult = value;
                RaisePropertyChanged("DigestionTestResult");
            }
        }
        [DataMemberAttribute]
        public long Digestion_V_HealthClass
        {
            get
            {
                return _Digestion_V_HealthClass;
            }
            set
            {
                _Digestion_V_HealthClass = value;
                RaisePropertyChanged("Digestion_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string UrologyTestResult
        {
            get
            {
                return _UrologyTestResult;
            }
            set
            {
                _UrologyTestResult = value;
                RaisePropertyChanged("UrologyTestResult");
            }
        }
        [DataMemberAttribute]
        public long Urology_V_HealthClass
        {
            get
            {
                return _Urology_V_HealthClass;
            }
            set
            {
                _Urology_V_HealthClass = value;
                RaisePropertyChanged("Urology_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string EndocrineTestResult
        {
            get
            {
                return _EndocrineTestResult;
            }
            set
            {
                _EndocrineTestResult = value;
                RaisePropertyChanged("EndocrineTestResult");
            }
        }
        [DataMemberAttribute]
        public long Endocrine_V_HealthClass
        {
            get
            {
                return _Endocrine_V_HealthClass;
            }
            set
            {
                _Endocrine_V_HealthClass = value;
                RaisePropertyChanged("Endocrine_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string OrthopaedicsTestResult
        {
            get
            {
                return _OrthopaedicsTestResult;
            }
            set
            {
                _OrthopaedicsTestResult = value;
                RaisePropertyChanged("OrthopaedicsTestResult");
            }
        }
        [DataMemberAttribute]
        public long Orthopaedics_V_HealthClass
        {
            get
            {
                return _Orthopaedics_V_HealthClass;
            }
            set
            {
                _Orthopaedics_V_HealthClass = value;
                RaisePropertyChanged("Orthopaedics_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string NeurologyTestResult
        {
            get
            {
                return _NeurologyTestResult;
            }
            set
            {
                _NeurologyTestResult = value;
                RaisePropertyChanged("NeurologyTestResult");
            }
        }
        [DataMemberAttribute]
        public long Neurology_V_HealthClass
        {
            get
            {
                return _Neurology_V_HealthClass;
            }
            set
            {
                _Neurology_V_HealthClass = value;
                RaisePropertyChanged("Neurology_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string NeuropsychiatricTestResult
        {
            get
            {
                return _NeuropsychiatricTestResult;
            }
            set
            {
                _NeuropsychiatricTestResult = value;
                RaisePropertyChanged("NeuropsychiatricTestResult");
            }
        }
        [DataMemberAttribute]
        public long Neuropsychiatric_V_HealthClass
        {
            get
            {
                return _Neuropsychiatric_V_HealthClass;
            }
            set
            {
                _Neuropsychiatric_V_HealthClass = value;
                RaisePropertyChanged("Neuropsychiatric_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string GeneralSugeryTestResult
        {
            get
            {
                return _GeneralSugeryTestResult;
            }
            set
            {
                _GeneralSugeryTestResult = value;
                RaisePropertyChanged("GeneralSugeryTestResult");
            }
        }
        [DataMemberAttribute]
        public long GeneralSugery_V_HealthClass
        {
            get
            {
                return _GeneralSugery_V_HealthClass;
            }
            set
            {
                _GeneralSugery_V_HealthClass = value;
                RaisePropertyChanged("GeneralSugery_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string LeftOptometryTestResult
        {
            get
            {
                return _LeftOptometryTestResult;
            }
            set
            {
                _LeftOptometryTestResult = value;
                RaisePropertyChanged("LeftOptometryTestResult");
            }
        }
        [DataMemberAttribute]
        public string RightOptometryTestResult
        {
            get
            {
                return _RightOptometryTestResult;
            }
            set
            {
                _RightOptometryTestResult = value;
                RaisePropertyChanged("RightOptometryTestResult");
            }
        }
        [DataMemberAttribute]
        public string LeftSightedOptometryTestResult
        {
            get
            {
                return _LeftSightedOptometryTestResult;
            }
            set
            {
                _LeftSightedOptometryTestResult = value;
                RaisePropertyChanged("LeftSightedOptometryTestResult");
            }
        }
        [DataMemberAttribute]
        public string RightSightedOptometryTestResult
        {
            get
            {
                return _RightSightedOptometryTestResult;
            }
            set
            {
                _RightSightedOptometryTestResult = value;
                RaisePropertyChanged("RightSightedOptometryTestResult");
            }
        }
        [DataMemberAttribute]
        public string OptometryDecreases
        {
            get
            {
                return _OptometryDecreases;
            }
            set
            {
                _OptometryDecreases = value;
                RaisePropertyChanged("OptometryDecreases");
            }
        }
        [DataMemberAttribute]
        public long Optometry_V_HealthClass
        {
            get
            {
                return _Optometry_V_HealthClass;
            }
            set
            {
                _Optometry_V_HealthClass = value;
                RaisePropertyChanged("Optometry_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string LeftHearingTestResult
        {
            get
            {
                return _LeftHearingTestResult;
            }
            set
            {
                _LeftHearingTestResult = value;
                RaisePropertyChanged("LeftHearingTestResult");
            }
        }
        [DataMemberAttribute]
        public string RightHearingTestResult
        {
            get
            {
                return _RightHearingTestResult;
            }
            set
            {
                _RightHearingTestResult = value;
                RaisePropertyChanged("RightHearingTestResult");
            }
        }
        [DataMemberAttribute]
        public string LeftSilentlyHearingTestResult
        {
            get
            {
                return _LeftSilentlyHearingTestResult;
            }
            set
            {
                _LeftSilentlyHearingTestResult = value;
                RaisePropertyChanged("LeftSilentlyHearingTestResult");
            }
        }
        [DataMemberAttribute]
        public string RightSilentlyHearingTestResult
        {
            get
            {
                return _RightSilentlyHearingTestResult;
            }
            set
            {
                _RightSilentlyHearingTestResult = value;
                RaisePropertyChanged("RightSilentlyHearingTestResult");
            }
        }
        [DataMemberAttribute]
        public string EarAndThroatDecreases
        {
            get
            {
                return _EarAndThroatDecreases;
            }
            set
            {
                _EarAndThroatDecreases = value;
                RaisePropertyChanged("EarAndThroatDecreases");
            }
        }
        [DataMemberAttribute]
        public long EarAndThroatDecreases_V_HealthClass
        {
            get
            {
                return _EarAndThroatDecreases_V_HealthClass;
            }
            set
            {
                _EarAndThroatDecreases_V_HealthClass = value;
                RaisePropertyChanged("EarAndThroatDecreases_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string UpperJawTestResult
        {
            get
            {
                return _UpperJawTestResult;
            }
            set
            {
                _UpperJawTestResult = value;
                RaisePropertyChanged("UpperJawTestResult");
            }
        }
        [DataMemberAttribute]
        public string LowerJawTestResult
        {
            get
            {
                return _LowerJawTestResult;
            }
            set
            {
                _LowerJawTestResult = value;
                RaisePropertyChanged("LowerJawTestResult");
            }
        }
        [DataMemberAttribute]
        public string DentalAndJawDecreases
        {
            get
            {
                return _DentalAndJawDecreases;
            }
            set
            {
                _DentalAndJawDecreases = value;
                RaisePropertyChanged("DentalAndJawDecreases");
            }
        }
        [DataMemberAttribute]
        public long DentalAndJaw_V_HealthClass
        {
            get
            {
                return _DentalAndJaw_V_HealthClass;
            }
            set
            {
                _DentalAndJaw_V_HealthClass = value;
                RaisePropertyChanged("DentalAndJaw_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string DermatologyTestResult
        {
            get
            {
                return _DermatologyTestResult;
            }
            set
            {
                _DermatologyTestResult = value;
                RaisePropertyChanged("DermatologyTestResult");
            }
        }
        [DataMemberAttribute]
        public long Dermatology_V_HealthClass
        {
            get
            {
                return _Dermatology_V_HealthClass;
            }
            set
            {
                _Dermatology_V_HealthClass = value;
                RaisePropertyChanged("DermatologyTestResult");
            }
        }
        [DataMemberAttribute]
        public Staff InternalMedicalResultStaff
        {
            get
            {
                return _InternalMedicalResultStaff;
            }
            set
            {
                _InternalMedicalResultStaff = value;
                RaisePropertyChanged("InternalMedicalResultStaff");
            }
        }
        [DataMemberAttribute]
        public Staff GeneralSugeryResultStaff
        {
            get
            {
                return _GeneralSugeryResultStaff;
            }
            set
            {
                _GeneralSugeryResultStaff = value;
                RaisePropertyChanged("GeneralSugeryResultStaff");
            }
        }
        [DataMemberAttribute]
        public Staff OptometryResultStaff
        {
            get
            {
                return _OptometryResultStaff;
            }
            set
            {
                _OptometryResultStaff = value;
                RaisePropertyChanged("OptometryResultStaff");
            }
        }
        [DataMemberAttribute]
        public Staff EarAndThroatResultStaff
        {
            get
            {
                return _EarAndThroatResultStaff;
            }
            set
            {
                _EarAndThroatResultStaff = value;
                RaisePropertyChanged("EarAndThroatResultStaff");
            }
        }
        [DataMemberAttribute]
        public Staff DentalAndJawResultStaff
        {
            get
            {
                return _DentalAndJawResultStaff;
            }
            set
            {
                _DentalAndJawResultStaff = value;
                RaisePropertyChanged("DentalAndJawResultStaff");
            }
        }
        [DataMemberAttribute]
        public Staff DermatologyResultStaff
        {
            get
            {
                return _DermatologyResultStaff;
            }
            set
            {
                _DermatologyResultStaff = value;
                RaisePropertyChanged("DermatologyResultStaff");
            }
        }
        [DataMemberAttribute]
        public long PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
            }
        }
        [DataMemberAttribute]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        public string ToXML()
        {
            if (this == null)
            {
                return null;
            }
            var mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("MedicalExaminationResult",
                    new XElement("MedicalExaminationResultID", MedicalExaminationResultID),
                    new XElement("PtRegDetailID", PtRegDetailID),
                    new XElement("PtRegistrationID", PtRegistrationID),
                    new XElement("MedicalExaminationHistory", MedicalExaminationHistory),
                    new XElement("CirculationTestResult", CirculationTestResult),
                    new XElement("Circulation_V_HealthClass", Circulation_V_HealthClass),
                    new XElement("RespiratoryTestResult", RespiratoryTestResult),
                    new XElement("Respiratory_V_HealthClass", Respiratory_V_HealthClass),
                    new XElement("DigestionTestResult", DigestionTestResult),
                    new XElement("Digestion_V_HealthClass", Digestion_V_HealthClass),
                    new XElement("UrologyTestResult", UrologyTestResult),
                    new XElement("Urology_V_HealthClass", Urology_V_HealthClass),
                    new XElement("EndocrineTestResult", EndocrineTestResult),
                    new XElement("Endocrine_V_HealthClass", Endocrine_V_HealthClass),
                    new XElement("OrthopaedicsTestResult", OrthopaedicsTestResult),
                    new XElement("Orthopaedics_V_HealthClass", Orthopaedics_V_HealthClass),
                    new XElement("NeurologyTestResult", NeurologyTestResult),
                    new XElement("Neurology_V_HealthClass", Neurology_V_HealthClass),
                    new XElement("NeuropsychiatricTestResult", NeuropsychiatricTestResult),
                    new XElement("Neuropsychiatric_V_HealthClass", Neuropsychiatric_V_HealthClass),
                    new XElement("GeneralSugeryTestResult", GeneralSugeryTestResult),
                    new XElement("GeneralSugery_V_HealthClass", GeneralSugery_V_HealthClass),
                    new XElement("LeftOptometryTestResult", LeftOptometryTestResult),
                    new XElement("RightOptometryTestResult", RightOptometryTestResult),
                    new XElement("LeftSightedOptometryTestResult", LeftSightedOptometryTestResult),
                    new XElement("RightSightedOptometryTestResult", RightSightedOptometryTestResult),
                    new XElement("OptometryDecreases", OptometryDecreases),
                    new XElement("Optometry_V_HealthClass", Optometry_V_HealthClass),
                    new XElement("LeftHearingTestResult", LeftHearingTestResult),
                    new XElement("RightHearingTestResult", RightHearingTestResult),
                    new XElement("LeftSilentlyHearingTestResult", LeftSilentlyHearingTestResult),
                    new XElement("RightSilentlyHearingTestResult", RightSilentlyHearingTestResult),
                    new XElement("EarAndThroatDecreases", EarAndThroatDecreases),
                    new XElement("EarAndThroatDecreases_V_HealthClass", EarAndThroatDecreases_V_HealthClass),
                    new XElement("UpperJawTestResult", UpperJawTestResult),
                    new XElement("LowerJawTestResult", LowerJawTestResult),
                    new XElement("DentalAndJawDecreases", DentalAndJawDecreases),
                    new XElement("DentalAndJaw_V_HealthClass", DentalAndJaw_V_HealthClass),
                    new XElement("DermatologyTestResult", DermatologyTestResult),
                    new XElement("Dermatology_V_HealthClass", Dermatology_V_HealthClass),
                    new XElement("InternalMedicalResultStaffID", InternalMedicalResultStaff == null ? null : (long?)InternalMedicalResultStaff.StaffID),
                    new XElement("GeneralSugeryResultStaffID", GeneralSugeryResultStaff == null ? null : (long?)GeneralSugeryResultStaff.StaffID),
                    new XElement("OptometryResultStaffID", OptometryResultStaff == null ? null : (long?)OptometryResultStaff.StaffID),
                    new XElement("EarAndThroatResultStaffID", EarAndThroatResultStaff == null ? null : (long?)EarAndThroatResultStaff.StaffID),
                    new XElement("DentalAndJawResultStaffID", DentalAndJawResultStaff == null ? null : (long?)DentalAndJawResultStaff.StaffID),
                    new XElement("DermatologyResultStaffID", DermatologyResultStaff == null ? null : (long?)DermatologyResultStaff.StaffID),

                    new XElement("ObstetricResultStaffID", ObstetricResultStaff == null ? null : (long?)ObstetricResultStaff.StaffID),
                    new XElement("Obstetric_V_HealthClass", Obstetric_V_HealthClass),
                    new XElement("ObstetricTestResult", ObstetricTestResult)));
            return mXDocument.ToString();
        }

        [DataMemberAttribute]
        public Staff ObstetricResultStaff
        {
            get
            {
                return _ObstetricResultStaff;
            }
            set
            {
                _ObstetricResultStaff = value;
                RaisePropertyChanged("ObstetricResultStaff");
            }
        }
        [DataMemberAttribute]
        public long Obstetric_V_HealthClass
        {
            get
            {
                return _Obstetric_V_HealthClass;
            }
            set
            {
                _Obstetric_V_HealthClass = value;
                RaisePropertyChanged("Obstetric_V_HealthClass");
            }
        }
        [DataMemberAttribute]
        public string ObstetricTestResult
        {
            get
            {
                return _ObstetricTestResult;
            }
            set
            {
                _ObstetricTestResult = value;
                RaisePropertyChanged("ObstetricTestResult");
            }
        }

        //▼==== #002
        [DataMemberAttribute]
        public long HealthClassification
        {
            get
            {
                return _HealthClassification;
            }
            set
            {
                _HealthClassification = value;
                RaisePropertyChanged("HealthClassification");
            }
        }
        
        [DataMemberAttribute]
        public string Diseases
        {
            get
            {
                return _Diseases;
            }
            set
            {
                _Diseases = value;
                RaisePropertyChanged("Diseases");
            }
        }

        [DataMemberAttribute]
        public string Record
        {
            get
            {
                return _Record;
            }
            set
            {
                _Record = value;
                RaisePropertyChanged("Record");
            }
        }
        
        [DataMemberAttribute]
        public string CurrentHealth
        {
            get
            {
                return _CurrentHealth;
            }
            set
            {
                _CurrentHealth = value;
                RaisePropertyChanged("CurrentHealth");
            }
        }
        
        [DataMemberAttribute]
        public DateTime HealthCheckUpDate
        {
            get
            {
                return _HealthCheckUpDate;
            }
            set
            {
                _HealthCheckUpDate = value;
                RaisePropertyChanged("HealthCheckUpDate");
            }
        }
        
        [DataMemberAttribute]
        public DateTime ExpiryDateHealthCertificate
        {
            get
            {
                return _ExpiryDateHealthCertificate;
            }
            set
            {
                _ExpiryDateHealthCertificate = value;
                RaisePropertyChanged("ExpiryDateHealthCertificate");
            }
        }
        //▲==== #002
    }
}