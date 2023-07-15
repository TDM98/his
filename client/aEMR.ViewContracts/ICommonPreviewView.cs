/*
 *  20170215 #001 CMN Begin: Add report TransferForm
 *  20181101 #002 TTM: BM 0004220: Bổ sung param cho report toa thuốc.
 *  20181129 #003 TNHX: [BM0005312]: Bổ sung param cho report PhieuMienGiam.
 *  20210430 #004 BLQ: thêm OutPtTreatmentProgramID cho xem in hồ sơ bệnh án ngoại trú
* 20230603 #005 DatTB: Thêm chức năng song ngữ mẫu kết quả xét nghiệm
 */
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ICommonPreviewView
    {
        long ID { get; set; }
        long PatientID { get; set; }
        string FileCodeNumber { get; set; }
        string PatientCode { get; set; }
        string PatientName { get; set; }
        string PatientFullAddress { get; set; }
        string PatientShortAddress { get; set; }
        string DOB { get; set; }
        string Age { get; set; }
        string Gender { get; set; }
        DateTime? AdmissionDate { get; set; }
        long IssueID { get; set; }
        ReportName eItem { get; set; }
        long PatientPCLReqID { get; set; }
        long PaymentID { get; set; }
        int FindPatient { get; set; }
        long RegistrationID { get; set; }
        long RegistrationDetailID { get; set; }

        bool? IsAppointment { get; set; }
        bool ViewByDate { get; set; }
        long DoctorStaffID { get; set; }
        DateTime? FromDate { get; set; }
        DateTime? ToDate { get; set; }

        long V_MedProductType { get; set; }
        string Result { get; set; }
        string TieuDeRpt { get; set; }
        long DrugDeptSellingPriceListID { get; set; }
        string TenYCuHoaChat { get; set; }
        string TenYCuHoaChatTiengViet { get; set; }

        long PharmacySellingPriceListID { get; set; }
        long? DeptID { get; set; }
        string DeptName { get; set; }

        long? DeptLocID { get; set; }
        string LocationName { get; set; }
        string TieuDeRpt1 { get; set; }

        long AppointmentID { get; set; }
        long? ServiceRecID { get; set; }
        long RepPaymentRecvID { get; set; }
        string StaffName { get; set; }
        string strIDList { get; set; }

        List<long> RegistrationDetailIDList { get; set; }
        List<long> PclRequestIDList { get; set; }
        int? ReceiptForEachLocationPrintingMode { get; set; }
        SeachPtRegistrationCriteria SeachRegistrationCriteria { get; set; }
        AppointmentSearchCriteria AppointmentCriteria { get; set; }

        long StaffID { get; set; }

        long RepPaymtRecptByStaffID { get; set; }

        byte flag { get; set; }

        GenericPayment CurGenPaymt { get; set; }
        long V_RegistrationType { get; set; }

        // TxD 08/10/2016 The following parameter was added for future use when DevExpress Xtrareport can accept an array type ie. byte[] parameter
        //                  so we can pass the image content directly to the report wihtout saving it first (not yet a requirement)
        // For now we save the captured image(s) then pass the file path to DevExpress 
        void SetHeartUltraImgageResult1(WriteableBitmap imgResult);

        string EchoCardioType1ImageResultFile1 { get; set; }

        string EchoCardioType1ImageResultFile2 { get; set; }

        ReceiptType ReceiptType { get; set; }
        //==== #001
        long TransferFormID { get; set; }
        int TransferFormType { get; set; }
        int FindBy { get; set; }
        long V_PCLRequestType { get; set; }
        //==== #001

        string IssueName { get; set; }
        string ReceiveName { get; set; }
        long? MedicalFileStorageCheckID { get; set; }
        int parTypeOfForm { get; set; }
        long? PrimaryID { get; set; }
        Int64 IntPtDiagDrInstructionID { get; set; }
        //▼====== #002
        bool IsPsychotropicDrugs { get; set; }
        bool IsFuncfoodsOrCosmetics { get; set; }
        //▲====== #002
        //▼====== #003
        long TotalMienGiam { get; set; }
        long PromoDiscProgID { get; set; }
        //▲====== #003
        bool IsDetails { get; set; }
        bool HasDischarge { get; set; }
        Patient CurPatient { get; set; }
        bool IsAddictive { get; set; }
        bool HasPharmacyDrug { get; set; }
        long TreatmentProcessID { get; set; }
        //▼====== #004
        long OutPtTreatmentProgramID { get; set; }
        //▲====== #004
        long NutritionalRatingID { get; set; }
        bool IsYHCTPrescript { get; set; }
        string ImageUrl { get; set; }

        //▼==== #005
        bool IsBilingual { get; set; }
        //▲==== #005
    }
}
