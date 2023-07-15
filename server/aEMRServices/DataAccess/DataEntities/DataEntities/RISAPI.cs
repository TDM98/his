using System;
using System.Linq;

namespace DataEntities
{
    public partial class HL7_INP
    {
        public HL7_INP(PatientPCLRequest aPatientPCLRequest, byte OrderStatus = 0, int? aHeight = 150, double? aWeight = 70)
        {
            if (aPatientPCLRequest.Patient == null
                || aPatientPCLRequest.PatientPCLRequestIndicators == null
                || aPatientPCLRequest.PatientPCLRequestIndicators.Count != 1
                || (OrderStatus == 0 && aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType == null)
                || (OrderStatus == 0 && string.IsNullOrEmpty(aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ModalityCode)))
            {
                return;
            }
            this.StudyId = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLReqItemID.ToString();
            if (OrderStatus == 0)
            {
                this.VisitNumber = aPatientPCLRequest.PatientPCLReqID;
                this.AccessionNumber = aPatientPCLRequest.PCLRequestNumID;
                this.PatientId = aPatientPCLRequest.Patient.PatientID.ToString();
                this.FirstName = aPatientPCLRequest.Patient.FirstName;
                this.MiddleName = aPatientPCLRequest.Patient.MiddleName;
                this.LastName = aPatientPCLRequest.Patient.LastName;
                this.PatientAdress = aPatientPCLRequest.Patient.PatientFullStreetAddress;
                this.Gender = aPatientPCLRequest.Patient.Gender;
                this.DateOfBirth = aPatientPCLRequest.Patient.DOB;
                this.Height = aHeight;
                this.Weight = aWeight;
                this.PatientLocation = aPatientPCLRequest.RequestedDepartment == null ? "" : aPatientPCLRequest.RequestedDepartment.DeptName;
                this.ReferringPhysician = aPatientPCLRequest.DoctorStaff == null ? "" : aPatientPCLRequest.DoctorStaff.FullName;
                this.OrderingProvider = aPatientPCLRequest.RequestedDoctorName;
                this.PatientClass = aPatientPCLRequest.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU ? "I" : "O";
                this.Diagnose = aPatientPCLRequest.Diagnosis;
                this.FillerOrder = aPatientPCLRequest.ReqFromDeptLocIDName;
                this.ExamRoom = aPatientPCLRequest.PCLDeptLocIDName;
                this.ExamCode = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.PCLExamTypeCode;
                this.ExamDescription = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.PCLExamTypeName;
                this.OrderDateTime = aPatientPCLRequest.CreatedDate;
                this.OrderEffectiveDate = aPatientPCLRequest.CreatedDate;
                this.Modality = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ModalityCode;
                this.ExamDateAndTime = aPatientPCLRequest.ExamDate;
            }
            this.OrderStatus = OrderStatus.ToString();
        }
        public string StudyId { get; set; }
        public long? VisitNumber { get; set; }
        public string AccessionNumber { get; set; }
        public string PatientId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PatientAdress { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Height { get; set; }
        public double? Weight { get; set; }
        public string PatientLocation { get; set; }
        public string ReferringPhysician { get; set; }
        public string OrderingProvider { get; set; }
        public string PatientClass { get; set; }
        public string Diagnose { get; set; }
        public string FillerOrder { get; set; }
        public string ExamRoom { get; set; }
        public string ExamCode { get; set; }
        public string ExamDescription { get; set; }
        public DateTime? OrderDateTime { get; set; }
        public DateTime? OrderEffectiveDate { get; set; }
        public string Modality { get; set; }
        public DateTime? ExamDateAndTime { get; set; }
        public string OrderStatus { get; set; }
        public string StudyUUID { get; set; }
        public string PatientNameE { get; set; }
        public string TechnicianText { get; set; }
        public string ResultDescriptionText { get; set; }
        public string ConclusionText { get; set; }
        public string AssistantInterpreter { get; set; }
        public string PrimaryInterpreter { get; set; }
        public string ResultDatetimeCreated { get; set; }
        public string URLReportFullPath { get; set; }
        public string ImageReportFullPath { get; set; }
    }
}