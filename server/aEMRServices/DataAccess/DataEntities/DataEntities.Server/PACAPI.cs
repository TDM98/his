using System;
using System.Linq;

namespace DataEntities
{
    //class PACAPI
    //{
    //}
    public partial class HL7_ITEM
    {
        public HL7_ITEM(PatientPCLRequest aPatientPCLRequest, int? aHeight = 150, double? aWeight = 70)
        {
            if (aPatientPCLRequest.Patient == null
            || aPatientPCLRequest.PatientPCLRequestIndicators == null
            || aPatientPCLRequest.PatientPCLRequestIndicators.Count != 1
            || (aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType == null)
            || (aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID == null)
            || (!aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID.IsSendToPAC))
            {
                return;
            }
            MaChiDinh = aPatientPCLRequest.PCLRequestNumID;
            ThoiGianChiDinh = aPatientPCLRequest.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
            MaBenhNhan = aPatientPCLRequest.Patient.PatientCode;
            TenBenhNhan = aPatientPCLRequest.Patient.FullName;
            NgaySinh = ((DateTime)aPatientPCLRequest.Patient.DOB).ToString("yyyyMMdd");
            GioiTinh = aPatientPCLRequest.Patient.Gender;
            DiaChi = aPatientPCLRequest.Patient.PatientFullStreetAddress;
            SDT = aPatientPCLRequest.Patient.PatientCellPhoneNumber;
            NoiChiDinh = aPatientPCLRequest.RequestedDepartment == null ? aPatientPCLRequest.ReqFromDeptLocIDName : aPatientPCLRequest.RequestedDepartment.DeptName;
            MaBacSiChiDinh = aPatientPCLRequest.RequestedDoctorCode;
            TenBacSiChiDinh = aPatientPCLRequest.RequestedDoctorName ?? aPatientPCLRequest.StaffIDName ?? "";
            MaDichVu = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.PCLExamTypeCode;
            TenDichVu = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.PCLExamTypeName;
            ChanDoan = aPatientPCLRequest.Diagnosis;
            NhomDichVu = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID == null ? "" : aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID.SubCategoryCodeToPAC;
            //PatientClass = aPatientPCLRequest.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU ? "I" : "O";
            TrangThai = aPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN ? "NEW" : (aPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL ? "CANCEL" : "");
        }

        public string MaChiDinh { get; set; }
        //  2020-10-07 16:08:47.373
        public string ThoiGianChiDinh { get; set; }
        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }

        // 19910101
        public string NgaySinh { get; set; }
        // M: Male F: Female
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public string NoiChiDinh { get; set; }
        public string MaBacSiChiDinh { get; set; }
        public string TenBacSiChiDinh { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string ChanDoan { get; set; }
        public string NhomDichVu { get; set; }
        // NEW, DELETED, MODIFIED
        public string TrangThai { get; set; }
        // defaul 0
        public string PhanLoai{ get; set; }
    }

    public partial class PACResult
    {
        public PACResult(PatientPCLImagingResult aPatientPCLImagingResult)
        {
            if (aPatientPCLImagingResult == null
            || aPatientPCLImagingResult.PatientPCLRequest == null
            || aPatientPCLImagingResult.PatientPCLRequest.Patient == null)
            {
                return;
            }
            Accession_Number = aPatientPCLImagingResult.PatientPCLRequest.PCLRequestNumID;
            Patient_ID = aPatientPCLImagingResult.PatientPCLRequest.Patient.PatientCode;
            Report_Date = aPatientPCLImagingResult.PerformedDate.ToString("yyyy-MM-dd HH:mm:ss");
            Report_Desc = aPatientPCLImagingResult.TemplateResultDescription;
            Report_Result = aPatientPCLImagingResult.TemplateResult;
            Reading_Physician_ID = (long)aPatientPCLImagingResult.PerformStaffID;
            Reading_Physician = aPatientPCLImagingResult.PerformStaffFullName;
            Status = aPatientPCLImagingResult.PatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE ? "COMPLETED" : "";
        }

        public string Accession_Number { get; set; }
        public string Patient_ID { get; set; }
        public string Report_Date { get; set; }
        public string Report_Desc { get; set; }
        public string Report_Result { get; set; }
        public long Reading_Physician_ID { get; set; }
        public string Reading_Physician { get; set; }
        // COMPLETED/READ/FINAL/PRINT
        public string Status { get; set; }
    }
}
