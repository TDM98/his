
/*
 * 20161231 #001 CMN: Add variable for VAT
 * 20170308 #002 CMN: Add HIStore Service
 * 20181002 #003 TTM: Thêm cấu hình điều khiển việc tìm kiếm đăng ký của bệnh nhân. Nếu bật lên thì có quyền tìm kiếm đăng ký bằng tên bệnh nhân và ngược lại.
 * 20181004 #004 TBL: Them cau hinh dieu khien viec tao moi dua tren cu chan doan
 * 20181005 #005 TBL: Them cau hinh rang buoc so chu cho cac truong cua chan doan
 * 20181005 #006 TNHX: [BM0000034] Add config Mode for Printting&Show PhieuChiDinh Report
 * 20181015 #007 TBL: Them cau hinh rang buoc truong phai nhap cua chan doan
 * 20181023 #008 TNHX: [BM0003221] Add config for Report (HospitalName, HospitalAddress, DepartmentOfHealth, LogoUrl)
 * 20181201 #009 TNHX: [BM0005312] Add config For PrintingMode of PhieuMienGiam
 * 20190520 #010 TNHX: [BM0006874] Add config For PrintingReceiptWithDrugBill
 * 20190522 #011 TNHX: [BM0006500] Add config For PrintingPhieuChiDinhForService
 * 20190613 #012 TNHX: [BM0006826] Add config For AllowTimeUpdatePrescription
 * 20190704 #013 TNHX: [BM0011926] Add config For DQGUnitname
 * 20190917 #014 TNHX: [BM0013247] Add config For AllowSearchInReport
 * 20190929 #015 TNHX: [BM] Add config For ShowInCostInInternalInwardPharmacy
 * 20191109 #016 TNHX: [BM 0013015] Add config For BlockPaymentWhenNotSettlement
 * 20191109 #017 TNHX: [BM ] Add config For ViewPrintAllImagingPCLRequestSeparate
 * 20200328 #018 TNHX: [BM ] Add config For BlockAddictiveAndPsychotropicDrugRequest
 * 20200329 #019 TNHX: [BM ] Add config For SecondExportBlockFormTheRequestForm
 * 20200519 #020 TNHX: [BM ] Add config For PrescriptionOutPtVersion + PrescriptionInPtVersion + PrescriptionMainRightHeader + PrescriptionSubRightHeader + LaboratoryResultVersion
 * 20200717 #021 TNHX: [BM ] Add config For BlockPrescriptionMaxHIPay
 * 20200807 #022 TNHX: [BM ] Add config For AllowConfirmEmergencyOutPt
 * 20200811 #023 TNHX: [BM ] Add config For PhieuNhanThuocPrintingModeInConfirmHIView
 * 20200903 #024 TNHX: [BM ] Add config For PharmacySearchByGenericName + PrintingWithoutExportPDF
 * 20200926 #025 TNHX: [BM ] Add config For ApplyFilterPrescriptionsHasHIPayTable
 * 20201026 #026 TNHX: [BM ] Add config For DisableBtnCheckCountPatientInPt + BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty
 * 20201110 #027 TNHX: [BM ] Add config For WhichHospitalUseThisApp
 * 20201117 #028 TNHX: [BM ] Add config For NumDayHIAgreeToPayAfterHIExpiresInPt
 * 20201127 #029 TNHX: [BM ] Add config For AutoGetHICardDataFromHIPortal
 * 20201211 #030 TNHX: [BM ] Add config AutoCreatePACWorklist + PACSAPIAddress + PACUserName + PACPassword + BlockInteractionSeverityLevelInPt + FilterDoctorByDeptResponsibilitiesInPt
 * 20201218 #031 TNHX: [BM ] Add config ApplyTemp12Version6556
 * 20201228 #032 TNHX: [BM ] Add config ApplyNewFuncForExportExcel
 * 20210206 #033 TNHX: [BM ] Add config NgayNhapLaiTDK
 * 20210220 #034 TNHX: [214] Add config ThuocDuocXuatThapPhan
 * 20210228 #035 TNHX: 219 Add config AllowFirstHIExaminationWithoutPay
 * 20210323 #036 TNHX: 240 Add config AutoSavePhysicalExamination
 * 20210411 #037 TNHX:  Add config AllowReSelectRoomWhenLeaveDept
 * 20210430 #038 TNHX:  Add config ListICDShowAdvisoryVotes
 * 20210710 #039 TNHX:  260 Add config AllowToBorrowDoctorAccount
 * 20210722 #040 TNHX:  Add config AgeMustHasDHST
 * 20210508 #041 TNHX:  428 Add config RefGenDrugCatID_2ForDrug
 * 20210809 #042 TNHX:  Add config MedProServiceStaffID
 * 20210921 #043 TNHX:  Add config AutoAddBedService
 * 20210923 #044 QTD:  Add config IsEnableFilterStorage
 * 20210925 #045 QTD:  Add config EnableCheckbox XCD
 * 20220103 #046 TNHX:  Add config AllowEditDiagnosisFinalForPatientCOVID
 * 20220225 #047 QTD:  Add config TimeForAllowUpdateMedicalInstruction
 * 20220330 #048 QTD: Add config QMS For PCL/Prescription
 * 20220414 #049 TNHX: Add config MinimumPopulateDelay For time delay search drug in outpt
 * 20220416 #050 DatTB: Thêm cấu hình xác nhận hoãn tạm ứng
 * 20220521 #051 BaoLQ: Thêm cấu hình in toa thuốc khi xem mẫu 12
 * 20220530 #052 BLQ: Thêm cấu hình thời gian thao tác của bác sĩ
 * 20220815 #053 BLQ: Thêm cấu hình kiểm tra thông tin bệnh nhân khi lưu toa thuốc
 * 20220829 #054 TNHX: Thêm cấu hình số ngày tìm kiếm mặc định cho KSK NumDayFindOutRegistrationMedicalExamination
 * 20220903 #055 BLQ: Thêm cấu hình in mẫu 12 màn hình duyệt toa
 * 20220922 #056 BLQ: Thêm cấu hình số lượng thuốc cho phép khi cấp toa
 * 20220924 #057 BLQ:
 * + Thêm cấu hình tách toa thuốc mua ngoài
 * + Thêm cấu hình Số điện thoại Bệnh viện
 * 20221008 #058 TNHX: 2344 Thêm cấu hình load dsach mã thẻ ND70 (TT_5149_List_HIPCode)
 * 20221010 #059 QTD: Thêm cấu hình ẩn tạo mã HSBA nội trú
 * 20221018 #060 QTD: Thêm cấu hình in biên lai và phiếu chỉ định khi lưu và trả tiền
 * 20221020 #061 QTD: Thêm cấu hình in mẫu 01/KBCB màn hình Xuất viện/Ra viện
 * 20221028 #062 QTD: Thêm Cấu hình triệu chứng không tự đẩy qua màn hình đề nghị nhập viện
 * 20221128 #063 TNHX: Thêm cấu hình đường dẫn kết quả xét nghiệm chữ ký số 
 * 20221205 #064 QTD: Thêm cấu hình HIAPI và đẩy cổng tự động
 *                   + Tách cấu hình Ngoại trú - Nội trú
 * 20221212 #065 QTD: Thêm cấu hình cho phép nhập y lệnh diễn tiến không có Thuốc/CLS/DV Nội trú
 * 20221209 #066 TNHX: 994 Thêm cấu hình đường dẫn kết quả xét nghiệm chữ ký số
 * 20221228 #067 QTD: Thêm cấu hình chỉ định gói dịch vụ từ màn hình khám bệnh bác sĩ
 * 20230108 #068 THNHX:  944 Thêm cấu hình đẩy đơn thuốc điện tử tự động khi xác nhận BHYT - ngoại trú
 * 20230103 #069 QTD  Thêm cấu hình lập phiếu dự trù màn hình Lập phiếu lĩnh VPP/VTTH
 * 20230211 #070 BLQ: Thêm cấu hình số toa thuốc điện tử cho phép xác nhận 1 lần
 * 20230314 #071 BLQ: Thêm cấu hình áp dụng xml theo 130
 * 20230603 #072 DatTB: Thêm chức năng song ngữ mẫu kết quả xét nghiệm
 * 20230617 #073 DatTB: Thêm biết cấu hình tên BV viết tắt
 * 20230706 #074 DatTB: Thêm cấu hình cho quét QRCode của CCCD
 */
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace eHCMS.Configurations
{
    [DataContract]
    public class CommonItemElement
    {
        //▼====: #068
        [DataMemberAttribute]
        public bool IsApplyAutoCreateDTDTReportWhenConfirmHI
        {
            get;
            set;
        }
        //▲====: #068
        //▼====: #066
        [DataMemberAttribute]
        public bool ApplyDTDT
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string DTDTUsername
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string DTDTPassword
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string DonThuocQuocGiaAPIUrl
        {
            get;
            set;
        }
        //▲====: #066
        //▼====: #063
        [DataMemberAttribute]
        public string FTPServerSighHashUrl
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string HISSighHashSmartCAUrl
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string ServicePool
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string ServiceUrl
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string PDFFileResultSignedPath
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string PDFFileResultToSignPath
        {
            get;
            set;
        }
        //▲====: #063
        //▼====: #049
        [DataMemberAttribute]
        public int MinimumPopulateDelay
        {
            get;
            set;
        }
        //▲====: #049
        //▼====: #043
        [DataMemberAttribute]
        public bool AutoAddBedService
        {
            get;
            set;
        }
        //▲====: #043
        //▼====: #042
        [DataMemberAttribute]
        public long MedProServiceStaffID
        {
            get;
            set;
        }
        //▲====: #042
        //▼====: #040
        [DataMemberAttribute]
        public long AgeMustHasDHST
        {
            get;
            set;
        }
        //▲====: #040
        //▼====: #039
        [DataMemberAttribute]
        public bool AllowToBorrowDoctorAccount
        {
            get;
            set;
        }
        //▲====: #039
        //▼====: #037
        [DataMemberAttribute]
        public bool AllowReSelectRoomWhenLeaveDept
        {
            get;
            set;
        }
        //▲====: #037
        //▼====: #036
        [DataMemberAttribute]
        public bool AutoSavePhysicalExamination
        {
            get;
            set;
        }
        //▲====: #036
        [DataMemberAttribute]
        public int ExaminationResultVersion
        {
            get;
            set;
        }
		//▼====: #035
		[DataMemberAttribute]
        public bool AllowFirstHIExaminationWithoutPay
        {
            get;
            set;
        }
        //▲====: #035
        [DataMemberAttribute]
        public bool ApplyCheckInPtRegistration
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public bool ApplyAutoCodeForCirculars56
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string SuffixAutoCodeForCirculars56
        {
            get;
            set;
        }
        //▼====: #033
        [DataMemberAttribute]
        public string NgayNhapLaiTDK
        {
            get;
            set;
        }
        //▲====: #033
        [DataMemberAttribute]
        public bool ApplyOtherDiagnosis
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string ApplyCheckV_TreatmentType
        {
            get;
            set;
        }
        //▼====: #032
        [DataMemberAttribute]
        public bool ApplyNewFuncExportExcel
        {
            get;
            set;
        }
        //▲====: #032
        //▼====: #031
        [DataMemberAttribute]
        public bool ApplyTemp12Version6556
        {
            get;
            set;
        }
        //▲====: #031
        //▼====: #029
        [DataMemberAttribute]
        public bool AutoGetHICardDataFromHIPortal
        {
            get;
            set;
        }
        //▲====: #029
        //▼====: #027
        [DataMemberAttribute]
        public int WhichHospitalUseThisApp
        {
            get;
            set;
        }
        //▲====: #027
        //▼====: #011
        [DataMemberAttribute]
        public bool ViewPrintAllImagingPCLRequestSeparate
        {
            get;
            set;
        }
        //▲====: #011
        //▼====: #015
        [DataMemberAttribute()]
        public bool ShowInCostInInternalInwardPharmacy { get; set; }
        //▲====: #015    
        //▼====: #014
        [DataMemberAttribute()]
        public bool AllowSearchInReport { get; set; }
        //▲====: #014
        //▼====: #011
        [DataMemberAttribute]
        public bool PrintingPhieuChiDinhForService
        {
            get;
            set;
        }
        //▲====: #011
        //▼====: #010
        [DataMemberAttribute]
        public bool PrintingReceiptWithDrugBill
        {
            get;
            set;
        }
        //▲====: #010
        //▼====: #009
        [DataMemberAttribute]
        public int PhieuMienGiamPrintingMode
        {
            get;
            set;
        }
        //▲====: #009
        /*▼====: #006*/
        [DataMemberAttribute]
        public int PhieuChiDinhPrintingMode
        {
            get;
            set;
        }
        /*▲====: #006*/
        [DataMemberAttribute]
        public int ReceiptPrintingMode
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public int ReceiptVersion
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public bool IsPrintReceiptPatientNoPay
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public bool IsPrintReceiptHINoPay
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public int EditHIBenefit
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public int ReceiptForEachLocationPrintingMode
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int RefundOrCancelCashReceipt //0: la Refund;1:la cancel hoa don
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public long StaffCatTypeBAC_SI 
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int NumberOfCopiesPrescription
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int DefaultNumOfCopyPrescriptNormalPT
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int DefaultNumOfCopyPrescriptHIPT
        {
            get;
            set;
        }


        [DataMemberAttribute]
        public int OrganizationUseSoftware
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public string OrganizationName
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public string OrganizationAddress
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public string OrganizationNotes
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public bool ShowApptCheck
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public bool ShowAddRegisButton
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int AllowDuplicateMedicalServiceItems
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int NumberTypePrescriptions_Rule
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int ExpRelAndBuildVersion { get; set; }

        [DataMemberAttribute]
        public bool ShowLoginNameOnReport38
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public byte PrintPatientInfoOption
        {
            get;
            set;
        }

        //==== #001
        [DataMemberAttribute()]
        public float DefaultVATPercent { get; set; }
        [DataMemberAttribute()]
        public bool VATAlreadyInPrice { get; set; }
        [DataMemberAttribute()]
        public bool UseVATOnBill { get; set; }
        [DataMemberAttribute()]
        public bool UseQRCode { get; set; }
        [DataMemberAttribute()]
        public int BorrowTimeLimit { get; set; }
        [DataMemberAttribute()]
        public bool EnableMedicalFileManagement { get; set; }
        //==== #001
        //==== #074
        [DataMemberAttribute()]
        public bool UseIDCardQRCode { get; set; }
        //==== #074
        //==== #002
        [DataMemberAttribute()]
        public bool EnableHIStore { get; set; }
        [DataMemberAttribute()]
        public bool EnablePayAfter { get; set; }
        [DataMemberAttribute()]
        public int HIStorageTypeID { get; set; }
        [DataMemberAttribute()]
        public int StoreIDForHIPrescription { get; set; }
        //==== #002
        [DataMemberAttribute()]
        public bool IsUseDailyDiagnostic { get; set; }
        [DataMemberAttribute()]
        public bool EnableTestFunction { get; set; }
        [DataMemberAttribute()]
        public bool IsEnalbeInDeptChangeLocFucn { get; set; }
        [DataMemberAttribute()]
        public bool IsEnalbeTempInDeptFuction { get; set; }
        [DataMemberAttribute()]
        public int CurrentHIReportVersion { get; set; }
        [DataMemberAttribute()]
        public bool AllowZeroHIPriceWithFlag { get; set; }
        [DataMemberAttribute()]
        public bool EnableNotiReExPatient { get; set; }
        [DataMemberAttribute()]
        public bool EnableOutPtCashAdvance { get; set; }
        [DataMemberAttribute()]
        public bool MixedHIPharmacyStores { get; set; }
        /*▼====: #006*/
        [DataMemberAttribute()]
        public string ReportDepartmentOfHealth { get; set; }
        [DataMemberAttribute()]
        public string ReportLogoUrl { get; set; }
        [DataMemberAttribute()]
        public string ReportHospitalAddress { get; set; }
        [DataMemberAttribute()]
        public string ReportHospitalName { get; set; }
        /*▲====: #006*/
        //▼====: #013
        [DataMemberAttribute()]
        public string DQGUnitname { get; set; }
        //▲====: #013
        [DataMemberAttribute]
        public string ReportTemplatesLocation { get; set; }
        [DataMemberAttribute]
        public string ValidHIPattern { get; set; }
        [DataMemberAttribute]
        public List<string[]> InsuranceBenefitCategories { get; set; }
        [DataMemberAttribute]
        public bool PayOnComfirmHI { get; set; }
        [DataMemberAttribute]
        public string ServerPublicAddress { get; set; }
        [DataMemberAttribute]
        public decimal AddingServicesPercent { get; set; }
        [DataMemberAttribute]
        public string eInvoicePatern { get; set; }
        [DataMemberAttribute]
        public string eInvoiceSerial { get; set; }
        [DataMemberAttribute]
        public string eInvoiceAdminUserName { get; set; }
        [DataMemberAttribute]
        public string eInvoiceAdminUserPass { get; set; }
        [DataMemberAttribute]
        public string eInvoiceAccountUserName { get; set; }
        [DataMemberAttribute]
        public string eInvoiceAccountUserPass { get; set; }
        [DataMemberAttribute]
        public string DQGUsername { get; set; }
        [DataMemberAttribute]
        public string DQGPassword { get; set; }
        [DataMemberAttribute]
        public string DQGUnitcode { get; set; }
        [DataMemberAttribute]
        public string DQGHUsername { get; set; }
        [DataMemberAttribute]
        public string DQGHPassword { get; set; }
        [DataMemberAttribute]
        public bool UserCanEditInvoicePatern { get; set; }
        [DataMemberAttribute]
        public int MaxEInvoicePaternLength { get; set; }
        [DataMemberAttribute]
        public long DefaultVIPServiceItemID { get; set; }
        [DataMemberAttribute]
        public bool ShowAddressPKBSHuan { get; set; }
        [DataMemberAttribute]
        public decimal AddingHIServicesPercent { get; set; }
        [DataMemberAttribute()]
        public bool ChangeHIAfterSaveAndPayRule { get; set; }

        [DataMemberAttribute()]
        public int MaxTimeForSmallProcedure { get; set; }
        [DataMemberAttribute()]
        public int LIDForConsultationAtHome { get; set; }

        [DataMemberAttribute()]
        public string CSSUrlPattern { get; set; }

        [DataMemberAttribute]
        public bool UpdateTicketStatusAfterRegister { get; set; }
        [DataMemberAttribute]
        public bool ReportTwoRegistrationSameTime { get; set; }
        [DataMemberAttribute]
        public bool UseQMSSystem { get; set; }
        [DataMemberAttribute]
        public bool CheckPatientInfoQMSSystem { get; set; }
        [DataMemberAttribute]
        public int BlockRegNoTicket { get; set; }
        [DataMemberAttribute]
        public long DefaultStoreIDForQuotation { get; set; }
        //▼====: #023
        [DataMemberAttribute]
        public int PhieuNhanThuocPrintingModeInConfirmHIView { get; set; }
        //▲====: #023

        [DataMemberAttribute]
        public bool SpecialHIRegistration { get; set; }
        [DataMemberAttribute]
        public bool NewMethodToReport4210 { get; set; }
        [DataMemberAttribute]
        public bool ChangeVATCreditOnInwardInvoice { get; set; }
        [DataMemberAttribute]
        public decimal InwardDifferenceValue { get; set; }

        // VuTTM - QMS Service
        [DataMemberAttribute]
        public string QMS_API_Url { get; set; }
        [DataMemberAttribute]
        public bool ApplyQMSAPI { get; set; }
        [DataMemberAttribute]
        public long Cashier1 { get; set; }
        [DataMemberAttribute]
        public long Cashier2 { get; set; }
        [DataMemberAttribute]
        public string QMSDepts { get; set; }
        [DataMemberAttribute]
        public string ApplyingQMSDepts { get; set; }
        [DataMemberAttribute]
        public string FloorDeptLocation_0 { get; set; }
        [DataMemberAttribute]
        public string FloorDeptLocation_1 { get; set; }
        [DataMemberAttribute]
        public string FloorDeptLocation_2 { get; set; }
        [DataMemberAttribute]
        public string OutpatientDept { get; set; }
        [DataMemberAttribute]
        public long MedDepartment { get; set; }
        [DataMemberAttribute]
        public long PharmacyDepartment { get; set; }

        [DataMemberAttribute]
        public long KioskStaffID { get; set; }
        [DataMemberAttribute]
        public string Excluded_Room { get; set; }

        [DataMemberAttribute]
        public string ICDCategorySearchUrl { get; set; }

        [DataMemberAttribute]
        public bool ApplyFixReCalcHIBenefit { get; set; }

        [DataMemberAttribute]
        public bool ApplyFloorNumberKiosk { get; set; }

        [DataMemberAttribute]
        public bool ApplyTemplatePCLResultNew { get; set; }

        [DataMemberAttribute]
        public string RuntimeUrl { get; set; }

        [DataMemberAttribute]
        public string RuntimeLocation { get; set; }

        [DataMemberAttribute]
        public string RuntimeReg64 { get; set; }

        [DataMemberAttribute]
        public string RuntimeReg32 { get; set; }

        
        //▼====: #045
        [DataMemberAttribute]
        public bool EnableCheckboxXCD{ get; set; }
        //▲====: #045

        [DataMemberAttribute]
        public string KBYTLink { get; set; }

        [DataMemberAttribute]
        public int WarningOutTimeSegments { get; set; }

        [DataMemberAttribute]
        public bool IsApplyTimeSegments { get; set; }

        [DataMemberAttribute]
        public string BearerToken { get; set; }

        [DataMemberAttribute]
        public string ExamCovidAPIBaseURL { get; set; }

        [DataMemberAttribute]
        public string ExamCovidAPIGetHistory { get; set; }

        [DataMemberAttribute]
        public string ExamCovidAPIGetPrintPreview { get; set; }


        [DataMemberAttribute]
        public bool IsApplyPCRDual { get; set; }

        [DataMemberAttribute]
        public bool IsApplyAutoCreateHIReport { get; set; }

        [DataMemberAttribute]
        public bool IsApplyUpdateInstruction { get; set; }

        //▼====: #047
        [DataMemberAttribute]
        public int TimeForAllowUpdateMedicalInstruction { get; set; }

        [DataMemberAttribute]
        public bool IsApplyTimeForAllowUpdateMedicalInstruction { get; set; }
        //▲====: #047
        //▼====: #048
        [DataMemberAttribute]
        public bool IsEnableQMSForPCL { get; set; }
        [DataMemberAttribute]
        public bool IsEnableQMSForPrescription { get; set; }
        [DataMemberAttribute]
        public bool IsEnableCreateOrderFromAccountant { get; set; }
        //▲====: #048

        //▼====: #050
        [DataMemberAttribute]
        public bool EnablePostponementAdvancePayment { get; set; }
        //▲====: #050
        //▼====: #052
        [DataMemberAttribute]
        public int DoctorContactPatientTime { get; set; }
        [DataMemberAttribute]
        public string LocationNotCheckDoctorContactPatientTime { get; set; }
        //▲====: #052
        //▼====: #051
        [DataMemberAttribute]
        public bool PrintPrescriptionWithTemp12 { get; set; }
        //▲====: #051
        [DataMemberAttribute]
        public string NotesKhongCheckTocDoTruyen { get; set; }

        [DataMemberAttribute]
        public string LinkKhaoSatNgoaiTru { get; set; }

        [DataMemberAttribute]
        public string LinkKhaoSatNoiTru { get; set; }

        [DataMemberAttribute]
        public string DeptCheckPainLevel { get; set; }
        //▼====: #053
        [DataMemberAttribute]
        public bool CheckPatientInfoWhenSavePrescript { get; set; }
        //▲====: #053
        //▼====: #055
        [DataMemberAttribute]
        public int Temp12PrintingMode { get; set; }
        //▲====: #055
        [DataMemberAttribute]
        public int CountSendTransaction { get; set; }
        //▼====: #056
        [DataMemberAttribute]
        public int MaxNumDayPrescriptAllow { get; set; }
        [DataMemberAttribute]
        public int MaxNumDayPrescriptAllow_InPt { get; set; }
        //▲====: #056 
        //▼====: #057
        [DataMemberAttribute]
        public bool IsSeparatePrescription { get; set; }
        [DataMemberAttribute]
        public string ReportHospitalPhone { get; set; }
        //▲====: #057
        //▼====: #059
        [DataMemberAttribute]
        public bool IsDisableCreateMedicalFile { get; set; }
        //▲====: #059
        //▼====: #060
        [DataMemberAttribute]
        public bool IsEnablePrintReceiptAndRequest { get; set; }
        //▲====: #060
        //▼====: #061
        [DataMemberAttribute]
        public bool PrintTemp01KBCB { get; set; }
        //▲====: #061
        //▼====: #062
        [DataMemberAttribute]
        public string SymptomNotUseForAdmission { get; set; }
        //▲====: #062
        //▼====: #064
        [DataMemberAttribute]
        public string APISendHIReportAddress { get; set; }
        [DataMemberAttribute]
        public bool IsApplyAutoCreateHIReportWhenConfirmHI { get; set; }
        [DataMemberAttribute]
        public bool IsApplyAutoCreateHIReportWhenSettlement { get; set; }
        //▲====: #064
        //▼====: #065
        [DataMemberAttribute]
        public bool IsSaveMedicalInstructionWithoutPrescription { get; set; }
        //▲====: #065
        //▼====: #069
        [DataMemberAttribute]
        public bool IsApplyCreateRequestForEstimation { get; set; }
        //▲====: #069
        //▼====: #067
        [DataMemberAttribute]
        public bool IsEnableAddRegPackByDoctor { get; set; }
        //▲====: #067 
        //▼====: #070
        [DataMemberAttribute]
        public int ElectronicPrescriptionMaxReport { get; set; }
        //▲====: #070
        //▼====: #071
        [DataMemberAttribute]
        public bool ApplyReport130 { get; set; }
        //▲====: #071
        [DataMemberAttribute]
        public string DeptLocIDApplyQMS { get; set; }
        [DataMemberAttribute]
        public bool IsEnableFilterPerformStaff { get; set; }
        [DataMemberAttribute]
        public string LocationAllowPrenatalCertificates { get; set; }
        [DataMemberAttribute]
        public string InsuranceCertificatePrefix { get; set; }
        [DataMemberAttribute]
        public long DeptIDKhoaSan { get; set; }
        [DataMemberAttribute]
        public string ListObjectTypeIDForMngt { get; set; }

        [DataMemberAttribute]
        public bool EnableCheckPaymentCeilingForTechService { get; set; }
        [DataMemberAttribute]
        public bool CheckHIWhenConfirm { get; set; }

        //▼==== #072
        [DataMemberAttribute()]
        public string ReportDepartmentOfHealthEng { get; set; }
        [DataMemberAttribute()]
        public string ReportHospitalAddressEng { get; set; }
        [DataMemberAttribute()]
        public string ReportHospitalNameEng { get; set; }
        //▲==== #072

        //▼==== #073
        [DataMemberAttribute()]
        public string ReportHospitalNameShort { get; set; }
        //▲==== #073

        [DataMemberAttribute]
        public string SMS_API_Url { get; set; }

        [DataMemberAttribute]
        public bool IsEnableSendSMSLab { get; set; }

        [DataMemberAttribute]
        public string BacSiTruongPhoKhoa { get; set; }

        [DataMemberAttribute]
        public string ThuTruongDonVi { get; set; }
    }
    [DataContract]
    public class HospitalElement
    {
        /// <summary>
        /// Mã Nơi khám chữa bệnh ban đầu của bệnh viện.
        /// </summary>
        [DataMemberAttribute]
        public string HospitalCode
        {
            get;
            set;
        }

        /// <summary>
        /// Đường dẫn tới file chứa logo của bệnh viện.
        /// </summary>
        [DataMemberAttribute]
        public string LogoImagePath
        {
            get;
            set;
        }
        
        [DataMemberAttribute]
        public string SequenceNumberType_5
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string SequenceNumberType_10
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string SequenceNumberType_25
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string SequenceNumberType_30
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string SequenceNumberType_35
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int KhoaPhongKham
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int FindRegistrationInDays_NgoaiTru
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int EffectedDiagHours
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int EffectedPCLHours
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int EditDiagDays
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int RoomFunction
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int LaboRmTp
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public bool IsConfirmHI
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public double MinPatientCashAdvance
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public string PCLResourcePool
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public string PCLStorePool
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public string PCLThumbTemp
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int RegistrationVIP
        {
            get;
            set;
        }

        [DataMemberAttribute]
        public int NeedICD10
        {
            get;
            set;
        }

        private bool _IsDirectorSignature;
        [DataMemberAttribute()]
        public bool IsDirectorSignature
        {
            get
            {
                return _IsDirectorSignature;
            }
            set
            {
                _IsDirectorSignature = value;
            }
        }

        private long _SurgeryDeptID;
        [DataMemberAttribute()]
        public long SurgeryDeptID
        {
            get
            {
                return _SurgeryDeptID;
            }
            set
            {
                _SurgeryDeptID = value;
            }
        }

        [DataMemberAttribute()]
        public string HIAPILoginAccount { get; set; }
        [DataMemberAttribute()]
        public string HIAPILoginPassword { get; set; }

        //▼====: #018
        private bool _BlockAddictiveAndPsychotropicDrugRequest;
        [DataMemberAttribute()]
        public bool BlockAddictiveAndPsychotropicDrugRequest
        {
            get
            {
                return _BlockAddictiveAndPsychotropicDrugRequest;
            }
            set
            {
                _BlockAddictiveAndPsychotropicDrugRequest = value;
            }
        }
        [DataMemberAttribute()]
        public string PrescriptionMainRightHeader { get; set; }
        [DataMemberAttribute()]
        public string PrescriptionSubRightHeader { get; set; }
        //▲====: #020

        [DataMemberAttribute]
        public string PDFStorePool { get; set;}

        [DataMemberAttribute]
        public string FTPLinkKQXN { get; set;}

        [DataMemberAttribute]
        public string FTPAdminUserName { get; set;}

        [DataMemberAttribute]
        public string FTPAdminPassword { get; set;}

    }
    [DataContract]
    public class HealthInsuranceElement
    {
        private long _hiPolicyMinSalary = (long)0;
        /// <summary>
        /// Giá trị tháng lương tối thiểu
        /// </summary>
        [DataMemberAttribute()]
        public long HiPolicyMinSalary
        {
            get { return _hiPolicyMinSalary; }
            set { _hiPolicyMinSalary = value; }
        }

        public double _rebatePercentage2015Level1_InPt = 0.4D;
        /// <summary>
        /// Phần trăm Bảo hiểm chi trả cho trường hợp trái tuyến mức 1 
        /// </summary>
        [DataMemberAttribute()]
        public double RebatePercentage2015Level1_InPt
        {
            get { return _rebatePercentage2015Level1_InPt; }
            set { _rebatePercentage2015Level1_InPt = value; }
        }

        public double _rebatePercentageLevel1 = 0.3D;
        /// <summary>
        /// Phần trăm Bảo hiểm chi trả cho trường hợp trái tuyến mức 1 (mức 1: 30%, mức 2: 50% ...)
        /// </summary>
        [DataMemberAttribute()]
        public double RebatePercentageLevel1
        {
            get { return _rebatePercentageLevel1; }
            set { _rebatePercentageLevel1 = value; }
        }


        public float _hiPolicyPercentageOnPayable = 0.15F;
        /// <summary>
        /// Giá trị % tháng lương tối thiểu mà từ đó mới tính đồng chi trả
        /// </summary>
        [DataMemberAttribute()]
        public float HiPolicyPercentageOnPayable
        {
            get { return _hiPolicyPercentageOnPayable; }
            set { _hiPolicyPercentageOnPayable = value; }
        }

        private List<string> _crossRegionCodeAcceptedList = new List<string>();
        /// <summary>
        /// Danh sách các bệnh viện được chấp nhận thẻ bảo hiểm cùng tuyến nếu có giấy chuyển viện
        /// </summary>
        [DataMemberAttribute()]
        public List<string> CrossRegionCodeAcceptedList
        {
            get
            {
                return _crossRegionCodeAcceptedList;
            }
            set
            {
                _crossRegionCodeAcceptedList = value;
            }
        }

        private int _paperReferalMaxDays = 30;
        /// <summary>
        /// Số ngày tối đa giấy chuyển viện còn hợp lệ kể từ ngày ký
        /// </summary>
        [DataMemberAttribute()]
        public int PaperReferalMaxDays
        {
            get { return _paperReferalMaxDays; }
            set { _paperReferalMaxDays = value; }
        }

        private bool _specialRuleForHIConsultationApplied = true;
        /// <summary>
        /// Có áp dụng luật Bảo hiểm đối với dịch vụ KCB hay không 
        /// (Đối với tất cả các dịch vụ KCB, bảo hiểm chỉ tính 1 dịch vụ thôi, còn lại là không có BH)
        /// </summary>
        [DataMemberAttribute()]
        public bool SpecialRuleForHIConsultationApplied
        {
            get { return _specialRuleForHIConsultationApplied; }
            set { _specialRuleForHIConsultationApplied = value; }
        }

        private double _hiConsultationServiceHIAllowedPrice = 3000;
        /// <summary>
        /// Số tiền BH quy định đối với dịch vụ KCB
        /// </summary>
        [DataMemberAttribute()]
        public double HIConsultationServiceHIAllowedPrice
        {
            get { return _hiConsultationServiceHIAllowedPrice; }
            set { _hiConsultationServiceHIAllowedPrice = value; }
        }

        private int _Apply15HIPercent = 0; //0:khong ap dung 15% BH,1:co ap dung
        [DataMemberAttribute()]
        public int Apply15HIPercent
        {
            get { return _Apply15HIPercent; }
            set { _Apply15HIPercent = value; }
        }
        
        private int _PharmacyMaxDaysHIRebate_NgoaiTru = 0;
        [DataMemberAttribute()]
        public int PharmacyMaxDaysHIRebate_NgoaiTru
        {
            get { return _PharmacyMaxDaysHIRebate_NgoaiTru; }
            set { _PharmacyMaxDaysHIRebate_NgoaiTru = value; }
        }

        private int _PharmacyMaxDaysHIRebate_NoiTru = 0;
        [DataMemberAttribute()]
        public int PharmacyMaxDaysHIRebate_NoiTru
        {
            get { return _PharmacyMaxDaysHIRebate_NoiTru;  }
            set { _PharmacyMaxDaysHIRebate_NoiTru = value;  }
        }

        private int _DifferenceDayPrecriptHI = 0;
        [DataMemberAttribute()]
        public int DifferenceDayPrecriptHI
        {
            get { return _DifferenceDayPrecriptHI;  }
            set { _DifferenceDayPrecriptHI = value;  }
        }

        private int _DifferenceDayRegistrationHI = 0;
        [DataMemberAttribute()]
        public int DifferenceDayRegistrationHI
        {
            get { return _DifferenceDayRegistrationHI;  }
            set { _DifferenceDayRegistrationHI = value;  }
        }

        [DataMemberAttribute()]
        public int MaxDaySellPrescriptInsurance { get; set; }

        private bool _applyHINewRule20150101;
        [DataMemberAttribute()]
        public bool ApplyHINewRule20150101
        {
            get 
            {
                return _applyHINewRule20150101;
            }
            set
            {
                _applyHINewRule20150101 = value;
            }
        }

        //Huyen 12/08/2015: Thêm thuộc tính để kích hoạt các view theo luật bảo hiểm mới
        private bool _applyHINew_Report20150701;
        [DataMemberAttribute()]
        public bool ApplyHINew_Report20150701
        {
            get
            {
                return _applyHINew_Report20150701;
            }
            set
            {
                _applyHINew_Report20150701 = value;
            }
        }
        //Huyen
        //HPT: Cấu hình có hay không ràng buộc mã code trong giấy chuyển viện
        private bool _IsCheckHICodeInPaperReferal;
        [DataMemberAttribute()]
        public bool IsCheckHICodeInPaperReferal
        {
            get
            {
                return _IsCheckHICodeInPaperReferal;
            }
            set
            {
                _IsCheckHICodeInPaperReferal = value;
            }
        }
        // Bắt buộc nhập địa chỉ thường trú của bệnh nhân vào thẻ BHYT
        private bool _CheckAddressInHealthInsurance;
        [DataMemberAttribute()]
        public bool CheckAddressInHealthInsurance
        {
            get
            {
                return _CheckAddressInHealthInsurance;
            }
            set
            {
                _CheckAddressInHealthInsurance = value;
            }
        }

        private bool _allowOutPtCrossRegion = false;
        [DataMemberAttribute()]
        public bool AllowOutPtCrossRegion
        {
            get
            {
                return _allowOutPtCrossRegion;
            }
            set
            {
                _allowOutPtCrossRegion = value;
            }
        }

        private bool _allowInPtCrossRegion = false;
        [DataMemberAttribute()]
        public bool AllowInPtCrossRegion
        {
            get
            {
                return _allowInPtCrossRegion;
            }
            set
            {
                _allowInPtCrossRegion = value;
            }
        }

        private bool _ValidateApplyingHIBenefit;
        [DataMemberAttribute()]
        public bool ValidateApplyingHIBenefit
        {
            get
            {
                return _ValidateApplyingHIBenefit;
            }
            set
            {
                _ValidateApplyingHIBenefit = value;
            }
        }

        private bool _calcHIBenefitBaseOnPatientClassType;
        [DataMemberAttribute()]
        public bool CalcHIBenefitBaseOnPatientClassType
        {
            get
            {
                return _calcHIBenefitBaseOnPatientClassType;
            }
            set
            {
                _calcHIBenefitBaseOnPatientClassType = value;
            }
        }

        private bool _FiveYearNotPaidEnough;
        [DataMemberAttribute()]
        public bool FiveYearNotPaidEnough
        {
            get
            {
                return _FiveYearNotPaidEnough;
            }
            set
            {
                _FiveYearNotPaidEnough = value;
            }
        }

        private long _MaxHIPaidOnMoreAddedItem;
        [DataMemberAttribute()]
        public long MaxHIPaidOnMoreAddedItem
        {
            get
            {
                return _MaxHIPaidOnMoreAddedItem;
            }
            set
            {
                _MaxHIPaidOnMoreAddedItem = value;
            }
        }
        [DataMemberAttribute()]
        public string NotPermittedHICard { get; set; }
        [DataMemberAttribute()]
        public float[] HIPercentOnDifDept { get; set; }
        [DataMemberAttribute()]
        public bool FullHIBenefitForConfirm { get; set; }
        [DataMemberAttribute()]
        public bool FullHIOfServicesForConfirm { get; set; }

        /// <summary>
        /// Cấu hình để quyết định sử dụng màn hình nào cho việc tính toán quyền lợi bảo hiểm cho bệnh nhân ngoại trú
        /// True: Sử dụng màn hình tính toán quyền lợi.
        /// False: Sử dụng màn hình xác nhận quyền lợi.
        /// </summary>
        [DataMemberAttribute]
        public bool UseConfirmRecalcHIOutPt { get; set; }
        [DataMemberAttribute]
        public float PercentForEkip { get; set; }
        [DataMemberAttribute]
        public float PercentForOtherEkip { get; set; }
    }
    [DataContract]
    public class PCLElement
    {
        [DataMemberAttribute()]
        public string PclImageStoragePath { get; set; }
        [DataMemberAttribute()]
        public string NetWorkMapDriver { get; set; }
        [DataMemberAttribute()]
        public string NWMDUser { get; set; }
        [DataMemberAttribute()]
        public string NWMDPass { get; set; }
        [DataMemberAttribute()]
        public string LocalFolderName { get; set; }
        [DataMemberAttribute()]
        public int RequireDiagnosisForPCLReq { get; set; }
        //20161014 CMN Begin: Add Permit for FileStore
        [DataMemberAttribute()]
        public short MaxEchogramImageFile { get; set; }
        //20161014 CMN End.
        [DataMemberAttribute()]
        public string Ab_Liver { get; set; }
        [DataMemberAttribute()]
        public string Ab_Gallbladder { get; set; }
        [DataMemberAttribute()]
        public string Ab_Pancreas { get; set; }
        [DataMemberAttribute()]
        public string Ab_Spleen { get; set; }
        [DataMemberAttribute()]
        public string Ab_RightKidney { get; set; }
        [DataMemberAttribute()]
        public string Ab_LeftKidney { get; set; }
        [DataMemberAttribute()]
        public string Ab_Bladder { get; set; }
        [DataMemberAttribute()]
        public string Ab_Prostate { get; set; }
        [DataMemberAttribute()]
        public string Ab_Uterus { get; set; }
        [DataMemberAttribute()]
        public string Ab_RightOvary { get; set; }
        [DataMemberAttribute()]
        public string Ab_LeftOvary { get; set; }
        [DataMemberAttribute()]
        public string Ab_PeritonealFluid { get; set; }
        [DataMemberAttribute()]
        public string Ab_PleuralFluid { get; set; }
        [DataMemberAttribute()]
        public string Ab_AbdominalAortic { get; set; }
        [DataMemberAttribute()]
        public string Ab_Conclusion { get; set; }
        [DataMemberAttribute()]
        public string ImageCaptureFileLocalStorePath { get; set; }
        [DataMemberAttribute()]
        public bool SaveImgWhenCapturing_Local { get; set; }
        [DataMemberAttribute]
        public bool AutoCreateRISWorklist { get; set; }
        [DataMemberAttribute]
        public string RISAPIAddress { get; set; }
        [DataMemberAttribute()]
        public string PCLImageURL { get; set; }
        [DataMemberAttribute]
        public string PCLImageFolder { get; set; }
        [DataMemberAttribute]
        public string PCLImageResultFolder { get; set; }
        //▼====: #030
        [DataMemberAttribute]
        public bool AutoCreatePACWorklist { get; set; }
        [DataMemberAttribute]
        public string PACSAPIAddress { get; set; }
        [DataMemberAttribute]
        public string PACUserName { get; set; }
        [DataMemberAttribute]
        public string PACPassword { get; set; }
        //▲====: #030
    }
    [DataContract]
    public class PharmacyConfigElement
    {
        //▼====: #024
        [DataMemberAttribute]
        public bool PharmacySearchByGenericName { get; set; }
        //▲====: #024

        [DataMemberAttribute()]
        public double PharmacyDefaultVATInward { get; set; }
        [DataMemberAttribute()]
        public int PharmacyCountMoneyIndependent { get; set; }
        [DataMemberAttribute()]
        public int AllowedPharmacyChangeHIPrescript { get; set; }
        [DataMemberAttribute()]
        public int AllowTimeUpdateOutInvoice { get; set; }
        [DataMemberAttribute()]
        public bool OnlyRoundResultForOutward { get; set; }
        [DataMemberAttribute()]
        public int HIStorageID { get; set; }
        [DataMemberAttribute()]
        public bool CalForPriceProfitScale { get; set; }
    }
    [DataContract]
    public class MedDeptConfigElement
    {
        [DataMemberAttribute()]
        public bool AutoCreateMedCode { get; set; }

        [DataMemberAttribute()]
        public bool MedDeptCanGetCash { get; set; }

        [DataMemberAttribute()]
        public string PrefixCodeMedical { get; set; }

        [DataMemberAttribute()]
        public string PrefixCodeMachine { get; set; }

        [DataMemberAttribute()]
        public string PrefixCodeChemical { get; set; }

        [DataMemberAttribute()]
        public bool CheckValueBuyPriceOnImportTempInward { get; set; }

        [DataMemberAttribute()]
        public long IntravenousCatID { get; set; }

        [DataMemberAttribute]
        public bool IsEnableMedSubStorage { get; set; }
        [DataMemberAttribute]
        public bool IsComplChkPointAfterStockTake { get; set; }
        [DataMemberAttribute]
        public bool UseBidDetailOnInward { get; set; }
        [DataMemberAttribute]
        public bool UseDrugDeptAs2DistinctParts { get; set; }
        [DataMemberAttribute()]
        public bool CalForPriceProfitScale_DrugDept { get; set; }
        //▼====: #019
        [DataMemberAttribute()]
        public bool SecondExportBlockFormTheRequestForm { get; set; }
        //▲====: #019
        //▼====: #026
        [DataMemberAttribute()]
        public bool BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty { get; set; }
        //▲====: #026

        //▼====: #044
        [DataMemberAttribute()]
        public bool IsEnableFilterStorage { get; set; }
        //▲====: #044
    }
    [DataContract]
    public class ClinicDeptConfigElement
    {
        //▼====: #034
        [DataMemberAttribute()]
        public string ThuocDuocXuatThapPhan { get; set; }
        //▲====: #034
        [DataMemberAttribute()]
        public bool UpdateOutwardToPatientNew { get; set; }

        [DataMemberAttribute()]
        public bool RequireDoctorAndDateForMed { get; set; }

        [DataMemberAttribute()]
        public bool RequireDoctorAndDateForMat { get; set; }

        [DataMemberAttribute()]
        public bool RequireDoctorAndDateForLab { get; set; }

        [DataMemberAttribute()]
        public long FileEmployeeID { get; set; }

        [DataMemberAttribute()]
        public bool LamTronSLXuatNoiTru { get; set; }
        [DataMemberAttribute]
        public bool RoundDownInwardOutQty { get; set; }
        [DataMemberAttribute]
        public string ProductTypeNotDocAndDateReq { get; set; }
        [DataMemberAttribute]
        public short DrugDeptOutDrugExpiryDateRule { get; set; }
        [DataMemberAttribute]
        public byte LoadOutwardTempBy { get; set; }        
    }
    [DataContract]
    public class OutRegisConfigElement
    {
        //▼====: #054
        [DataMemberAttribute()]
        public int NumDayFindOutRegistrationMedicalExamination { get; set; }
        //▲====: #054
        //▼====: #024
        [DataMemberAttribute()]
        public bool PrintingWithoutExportPDF { get; set; }
        //▲====: #024

        [DataMemberAttribute()]
        public bool AssignSequenceNumberManually { get; set; }

        [DataMemberAttribute()]
        public bool AllowToChooseTypeOf01Form { get; set; }

        [DataMemberAttribute()]
        public int MaxNumberOfServicesAllowForOutPatient { get; set; }

        [DataMemberAttribute()]
        public bool AutoLocationAllocation { get; set; }

        [DataMemberAttribute()]
        public bool IsPerformingTMVFunctionsA { get; set; }

        [DataMemberAttribute()]
        public bool CheckDoctorStaffID { get; set; }

        [DataMemberAttribute()]
        public int DayStartAndEndFindAppointment { get; set; }
        //▼===== #022
        [DataMemberAttribute]
        public bool AllowConfirmEmergencyOutPt { get; set; }
        //▲===== #022
    }
    [DataContract]
    public class InRegisConfigElement
    {
        //▼====: #045
        [DataMemberAttribute]
        public bool AllowEditDiagnosisFinalForPatientCOVID
        {
            get;
            set;
        }
        //▲====: #045
        //▼====: #041
        [DataMemberAttribute]
        public long RefGenDrugCatID_2ForDrug
        {
            get;
            set;
        }
        //▲====: #041
        //▼====: #028
        [DataMemberAttribute]
        public int NumDayHIAgreeToPayAfterHIExpiresInPt
        {
            get;
            set;
        }
        //▲====: #028
        [DataMemberAttribute()]
        public bool AddMedProductToBillDirectly { get; set; }

        [DataMemberAttribute()]
        public bool OnlyInsertToCashAdvance { get; set; }

        // TxD 19/01/2015 Added this config to enable discharging InPt using 2 separate steps:
        //          1. Doctor or someone creates and fills in the Discharge Detail record in the DischargeInfoViewModel
        //          2. Nurse or someone responsible to confirm that the discharging process is now completed: ie. patient has already left the hospital.

        [DataMemberAttribute()]
        public bool DischargeInPtWith2Steps { get; set; }


        //KMx: Không hiển thị bill của khoa A, B vào màn hình tạm ứng bill, quyết toán (07/03/2015 14:41).
        [DataMemberAttribute()]
        public bool ExcludeDeptAAndB { get; set; }

        //KMx: Số ngày được phép tạo bill cho bệnh nhân sau ngày xuất viện tạm. Nếu <= 0: Không kiểm tra. (30/05/2015 10:57).
        [DataMemberAttribute()]
        public int NumOfDayAllowSaveBillAfterDischarge { get; set; }

        //HPT: Số ngày tối đa một đăng ký vãng lai chờ được phục vụ hoặc nhập viện. Sau số ngày này, muốn đăng ký các dịch vụ cho bệnh nhân đó, phải tạo đăng ký mới
        [DataMemberAttribute()]
        public int NumOfDayAllowPending_CasualReg { get; set; }
        
        //HPT: Số ngày tối đa một đăng ký tiền giải phẫu chờ được phục vụ hoặc nhập viện. Sau số ngày này, muốn đăng ký các dịch vụ cho bệnh nhân đó, phải tạo đăng ký mới
        [DataMemberAttribute()]
        public int NumOfDayAllowPending_PreOpReg { get; set; }

        //KMx: Số tiền BH thanh toán tối đa cho bill DVKTC
        [DataMemberAttribute()]
        public decimal MaxHIPayForHighTechServiceBill { get; set; }

        //HPT: cấu hình kiểm tra giá trị RegLockFlag để khóa các đăng ký đã báo cáo theo mẫu mới công văn 87 (02/06/2016)
        [DataMemberAttribute()]
        public int CheckToLockReportedRegistration { get; set; }

        //HPT: cấu hình cho phép trẻ em dưới 6 tuổi sử dụng thẻ BHYT hết hạn trước và trong ngày 30/9
        [DataMemberAttribute()]
        public bool AllowChildUnder6YearsOldUseHIOverDate { get; set; }

        //HPT: cấu hình cho phép hiển thị checkbox Bệnh nhân cấp cứu tái khám sau xuất viện (Ngoại trú)
        [DataMemberAttribute()]
        public bool ShowEmergInPtReExamination { get; set; }

        //HPT: ID khoa cấp cứu
        [DataMemberAttribute()]
        public long EmerDeptID { get; set; }

        //HPT cấu hình số ngày nhập viện trong tương lai không vượt quá n ngày tính từ ngày hiện tại
        [DataMemberAttribute()]
        public int NumOfOverDaysInDischargeForm { get; set; }

        [DataMemberAttribute()]
        public bool CheckDischargeDate { get; set; }

        [DataMemberAttribute()]
        public int NumOfOverDaysForMedicalInstructDate { get; set; }

        [DataMemberAttribute()]
        public bool CheckMedicalInstructDate { get; set; }
        [DataMemberAttribute()]
        public bool NotCountHIOnPackItem { get; set; }
        [DataMemberAttribute()]
        public bool Use_SaveRegisThenPay { get; set; }
        [DataMemberAttribute()]
        public bool ShowMessageBoxForLockReportedRegistration { get; set; }
        [DataMemberAttribute()]
        public bool BlockPaymentWhenNotSettlement { get; set; }
        [DataMemberAttribute()]
        public int MergerPatientRegistration { get; set; }
        //▼====: #026
        [DataMemberAttribute]
        public bool DisableBtnCheckCountPatientInPt { get; set; }
        //▲====: #026
    }
    [DataContract]
    public class ConsultationConfigElement
    {
        //▼====: #058
        [DataMemberAttribute]
        public string TT5149ListHIPCode
        {
            get;
            set;
        }
        //▲====: #058
        //▼====: #038
        [DataMemberAttribute]
        public string ListICDShowAdvisoryVotes
        {
            get;
            set;
        }
        //▲====: #038
        [DataMemberAttribute()]
        public bool DefSearchByGenericName { get; set; }
        [DataMemberAttribute()]
        public bool AllowToUpdateDiagnosisIntoPCLReq { get; set; }
        [DataMemberAttribute()]
        public bool DisalbeSequenceNumberInAppointment { get; set; }
        /*TMA 25/11/2017*/
        private bool _IsSeparatePsychotropicPrescription;
        [DataMemberAttribute()]
        public bool IsSeparatePsychotropicPrescription
        {
            get
            {
                return _IsSeparatePsychotropicPrescription;
            }
            set
            {
                _IsSeparatePsychotropicPrescription = value;
            }
        }

        private bool _IsSeparatePsychotropicPrescription_Inpt;
        [DataMemberAttribute()]
        public bool IsSeparatePsychotropicPrescription_Inpt
        {
            get
            {
                return _IsSeparatePsychotropicPrescription_Inpt;
            }
            set
            {
                _IsSeparatePsychotropicPrescription_Inpt = value;
            }
        }
        /*TMA 25/11/2017*/
        [DataMemberAttribute()]
        public bool EnableTreatmentRegimen { get; set; }
        //▼====== #003
        [DataMemberAttribute()]
        public bool IsAllowSearchingPtByName { get; set; }
        //▼====== #003
        /*▼====: #004*/
        [DataMemberAttribute()]
        public int IsAllowCopyDiagTrmt { get; set; }
        /*▲====: #004*/
        /*▼====: #005*/
        [DataMemberAttribute()]
        public int MinNumOfChar { get; set; }
        /*▲====: #005*/
        /*▼====: #007*/
        [DataMemberAttribute()]
        public int IsAllowInputDiagTrmt { get; set; }
        /*▲====: #007*/

        [DataMemberAttribute()]
        public bool AllowWorkingOnSunday { get; set; }

        [DataMemberAttribute()]
        public bool DiagnosisTreatmentForDrug { get; set; }

        [DataMemberAttribute()]
        public bool AllowSaveQuantityNotEnough { get; set; }
        [DataMemberAttribute()]
        public bool AllowBlockContraIndicator { get; set; }
        [DataMemberAttribute()]
        public bool AllowBlockContraIndicatorInDay { get; set; }
        [DataMemberAttribute()]
        public bool CheckToaThuocBiTrungTheoHoatChat { get; set; }
        [DataMemberAttribute()]
        public bool CheckToaThuocBiTrungNhomThuoc { get; set; }
        [DataMemberAttribute()]
        public bool KiemTraQuanHeHoatChat { get; set; }

        //▼====: #012
        [DataMemberAttribute()]
        public int AllowTimeUpdatePrescription { get; set; }
        //▲====: #012
        [DataMemberAttribute()]
        public bool AppointmentAuto { get; set; }
        [DataMemberAttribute()]
        public string ParamAppointmentAuto { get; set; }
        [DataMemberAttribute()]
        public bool CheckedTreatmentRegimen { get; set; }
        [DataMemberAttribute]
        public long[] HealthExamDeptLocIDArray { get; set; }
        [DataMemberAttribute()]
        public long ModeShowInforDrugForAutoCompleteInstruction { get; set; }
        [DataMemberAttribute]
        public bool UseOnlyDailyDiagnosis { get; set; }
        [DataMemberAttribute()]
        public int LevelWarningWhenCreateNewAndCopy { get; set; }
        [DataMemberAttribute]
        public decimal PrescriptionMaxHIPay { get; set; }
        [DataMemberAttribute()]
        public int IsAllowCopyDiagTrmtInstruction { get; set; }
        [DataMemberAttribute]
        public int CheckMonitoringVitalSigns { get; set; }
        [DataMemberAttribute()]
        public int IsAllowCopyInstruction { get; set; }
        [DataMemberAttribute]
        public string ConsultMinTimeReqBeforeExit { get; set; }
        [DataMemberAttribute]
        public bool IsCheckApmtOnPrescription { get; set; }
        [DataMemberAttribute]
        public int PrescriptionOutPtVersion { get; set; }
        [DataMemberAttribute]
        public int PrescriptionInPtVersion { get; set; }
        [DataMemberAttribute]
        public int LaboratoryResultVersion { get; set; }
        [DataMemberAttribute]
        public double PercentPrescriptionForHI { get; set; }
        [DataMemberAttribute]
        public bool BlockPrescriptionMaxHIPay { get; set; }
        //▼====: #025
        [DataMemberAttribute]
        public bool ApplyFilterPrescriptionsHasHIPayTable { get; set; }
        //▲====: #025
        //▼====: #030
        [DataMemberAttribute]
        public long BlockInteractionSeverityLevelInPt { get; set; }
        [DataMemberAttribute]
        public bool FilterDoctorByDeptResponsibilitiesInPt { get; set; }
        //▲====: #030
        [DataMemberAttribute()]
        public bool CheckToaThuocBiTrungTheoHoatChatVaNgayThuocBaoHiem { get; set; }
    }
    [DataContract]
    public class ServerElement
    {
        [DataMemberAttribute()]
        public string OutstandingServerIP { get; set; }
        [DataMemberAttribute()]
        public long OutstandingServerPort { get; set; }
        [DataMemberAttribute()]
        public string ExcelStorePool { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatisticsQuarter1 { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatisticsQuarter2 { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatisticsQuarter3 { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatisticsQuarter4 { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_HRStatistics { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_MedExamActivity { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_TreatmentActivity { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_SpecialistTreatmentActivity { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_SurgeryActivity { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_ReproductiveHealthActivity { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_PCLActivity { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_PharmacyDeptStatistics { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_MedicalEquipmentStatistics { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_ScientificResearchActivity { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_FinancialActivityTemp1 { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_FinancialActivityTemp2 { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_FinancialActivityTemp3 { get; set; }
        [DataMemberAttribute()]
        public string HospitalStatistics_ICD10Statistics { get; set; }
        [DataMemberAttribute()]
        public bool UseDataAccessLayerProvider { get; set; }
        [DataMemberAttribute]
        public string ImageCaptureFilePublicStorePath { get; set; }
    }
    [DataContract]
    public class AxServerConfigSection
    {
        private CommonItemElement _commonItems = new CommonItemElement();
        [DataMemberAttribute()]
        public CommonItemElement CommonItems
        {
            get { return _commonItems; }
            set { _commonItems = value; }
        }

        private HospitalElement _hospitals = new HospitalElement();
        [DataMemberAttribute()]
        public HospitalElement Hospitals
        {
            get { return _hospitals; }
            set { _hospitals = value; }
        }

        private PharmacyConfigElement _pharmacyElements = new PharmacyConfigElement();
        [DataMemberAttribute()]
        public PharmacyConfigElement PharmacyElements
        {
            get { return _pharmacyElements; }
            set { _pharmacyElements = value; }
        }

        private MedDeptConfigElement _medDeptElements = new MedDeptConfigElement();
        [DataMemberAttribute()]
        public MedDeptConfigElement MedDeptElements
        {
            get { return _medDeptElements; }
            set { _medDeptElements = value; }
        }

        private ClinicDeptConfigElement _clinicDeptElements = new ClinicDeptConfigElement();
        [DataMemberAttribute()]
        public ClinicDeptConfigElement ClinicDeptElements
        {
            get { return _clinicDeptElements; }
            set { _clinicDeptElements = value; }
        }

        private OutRegisConfigElement _outRegisElements = new OutRegisConfigElement();
        [DataMemberAttribute()]
        public OutRegisConfigElement OutRegisElements
        {
            get { return _outRegisElements; }
            set { _outRegisElements = value; }
        }

        private InRegisConfigElement _inRegisElements = new InRegisConfigElement();
        [DataMemberAttribute()]
        public InRegisConfigElement InRegisElements
        {
            get { return _inRegisElements; }
            set { _inRegisElements = value; }
        }

        private ConsultationConfigElement _consultationElements = new ConsultationConfigElement();
        [DataMemberAttribute()]
        public ConsultationConfigElement ConsultationElements
        {
            get { return _consultationElements; }
            set { _consultationElements = value; }
        }

        private HealthInsuranceElement _healthInsurances = new HealthInsuranceElement();
        [DataMemberAttribute()]
        public HealthInsuranceElement HealthInsurances
        {
            get { return _healthInsurances; }
            set { _healthInsurances = value; }
        }

        private PCLElement _pcls = new PCLElement();
        [DataMemberAttribute()]
        public PCLElement Pcls
        {
            get { return _pcls; }
            set { _pcls = value; }
        }

        private ServerElement _servers = new ServerElement();
        [DataMemberAttribute()]
        public ServerElement Servers
        {
            get
            {
                return _servers;
            }
            set
            {
                _servers = value;
            }
        }

    }
}
