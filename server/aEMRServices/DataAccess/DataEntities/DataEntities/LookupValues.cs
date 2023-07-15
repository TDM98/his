using System.ComponentModel;
using System;
using System.Reflection;
using System.Windows;
using eHCMSLanguage;
/*
 * 20170622 #002 CMN: Added lookup value for ConsultingDiagnosys
 * 20180728 #003 TTM: Added lookup value V_CatDrugType 
 * 20181027 #004 TBL: Added enum V_TreatmentType
 * 20190603 #005 TNHX:  [BM0011782] Add enum StoreID for middle store
 * 20200110 #006 TNHX:  [] Add lookup value for V_RangeOfWareHouses
 * 20200319 #007 TTM:   BM 0027022: [79A] Bổ sung tích chọn xuất Excel toàn bộ dữ liệu, đã xác nhận, chưa xác nhận. 
 * 20200609 #008 TNHX: BM: Chỉnh lại StaffPositions_Enum
 * 20200928 #009 TNHX: BM: Thêm V_DiscountTypeCount
 * 20210323 #010 BLQ: 243 Thêm V_CheckMedicalFilesStatus
 * 20210531 #011 TNHX: 320 Thêm loại cho V_PrescriptionNoteTempType
 * 20220517 #012 DatTB: Thêm trường chọn CLS Đã/Chưa thực hiện
 * 20220620 #013 QTD:   Thêm Lookup PatientClassification
 * 20220725 #014 DatTB: Thêm giá trị "Ngưng theo dõi" cho ListV_ReconmendTime
 * 20220807 #015 DatTB: Báo cáo thống kê số lượng hồ sơ điều trị ngoại trú
 * + Tạo màn hình, thêm các trường lọc dữ liệu.
 * + Thêm trường phòng khám sau khi chọn khoa.
 * + Validate các trường lọc.
 * + Thêm điều kiện để lấy khoa theo list DeptID.
 * 20220901 #016 BLQ: Issue:2174 Chỉnh lại mẫu hình ảnh theo cách mới
 * 20220926 #017 DatTB: Thêm Lookup khoa để check cấu hình
 * 20220928 #018 BLQ: Thêm Lookup loại toa thuốc
 * 20221005 #019 BLQ: Thêm hạng thẻ khách hàng
 * 20221117 #020 DatTB: - IssueID: 2241 | Thêm trường chọn mẫu cho các sổ khám bệnh.
 * 20221121 #021 BLQ: Thêm tình trạng của lịch ngoài giờ
 * 20221201 #022 QTD: Thêm loại loại hình điều trị cho đẩy cổng XML tự động màn hình quyết toán TV
 * 20230201 #023 DatTB: Thêm trường dữ liệu về KSNK trong phần Thông tin chung NB nội trú
 * 20230213 #024 QTD: Thêm Lookup trạng thái đơn thuốc Nhà thuốc
 * 20230311 #025 DatTB: Thêm Lookup mẫu report kết quả xét nghiệm
 * 20230314 #026 DatTB: Tách Lookup mẫu report kết quả xét nghiệm sử dụng chung mẫu CDHA
 * 20230313 #027 BLQ: Thêm trường lookup cho giấy chứng sinh
 * 20230321 #028 QTD: Thêm dữ liệu 130
 * 20230424 #029 DatTB: Thêm lookup tình trạng thiết bị
 * 20230502 #030 QTD: Thêm Lookup tiên lượng
 * 20230513 #031 DatTB: Thêm Báo cáo thời gian chờ tại bệnh viện
 * 20230526 #032 DatTB: Thêm Báo cáo thống kê đơn thuốc điện tử
 * 20230626 #033 QTD: Thêm Lookup trạng thái gửi SMS
*/
namespace DataEntities
{
    //==== 20161126 CMN Begin: Add method for define description from resources
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _resourceKey;
        public LocalizedDescriptionAttribute(string resourceKey)
        {
            _resourceKey = resourceKey;
        }
        public override string Description
        {
            get
            {
                string displayName = eHCMSResources.ResourceManager.GetString(_resourceKey);
                return string.IsNullOrEmpty(displayName)
                    ? string.Format("[[{0}]]", _resourceKey)
                    : displayName;
            }
        }
    }
    //==== 20161126 CMN End.
    public enum LookupValues
    {
        ALL_VALUE_TYPE = 0,
        REF_DOC_TYPE = 1,
        STATUS = 2,
        HOSPITAL_TYPE = 3,
        MARITAL_STATUS = 4,
        ETHNIC = 5,
        FAMILY_RELATIONSHIP = 6,
        DOCUMENT_TYPE_ON_HOLD = 7,
        EXAM_REG_STATUS = 8,
        PAST_MED_HISTORY_STATUS = 9,
        PROCESSING_TYPE = 10,
        CATEGORY_OF_DECEASE = 11,
        ADMISSION_TYPE = 12,
        ADMISSION_REASON = 13,
        REFERRAL_TYPE = 14,
        TREATMENT_RESULT = 15,
        DISCHARGE_TYPE = 16,
        MEDICAL_SERVICE_STATUS = 17,
        TIME_UNIT = 18,
        RELIGION = 19,
        BANK_NAME = 20,
        ALERGY_ITEM_TYPE = 23,
        PRESCRIPTION_ISSUED_CASE = 24,
        PRESCRIPTION_TYPE = 25,
        VITAL_SIGN_CONTEXT = 26,
        STAFF_ROLE = 27,
        SURGERY_SITUATION = 28,
        SURGICAL_RESULTS = 29,
        SURGICAL_STATUS = 30,
        INJURED_PARTS_OF_BODY = 31,
        CHANGES_AFTER_AC = 32,
        BEHAVE_AFTER_AC = 33,
        BEHAVING = 34,
        DRUG_INSTRUCTION = 35,
        VSIGN_DATA_TYPE = 36,
        RSCR_UNIT = 37,
        BED_LOCATION_TYPE = 38,
        APPOINTMENT_TYPE = 39,
        SERIOUSNESS = 40,
        INVOICE_REASON = 41,
        SESSION_ACTIVITY = 45,
        QUEUE_TYPE = 46,
        FILE_STORAGE_RESULT_TYPE = 47,
        PATIENT_PCLREQUEST_CATEGORY = 48,
        PAYMENT_MODE = 49,
        CURRENCY = 50,
        PAYMENT_TYPE = 51,
        TRAN_HI_PAYMENT = 54,
        TRAN_PATIENT_PAYMENT = 55,
        REGISTRATION_STATUS = 56,
        SMOKE_STATUS = 52,
        ALCOHOL_DRINKING_STATUS = 53,
        APPT_STATUS = 57,

        //dinh them            
        RESOURCE_UNIT = 58,
        RESOURCE_ALLOC_STATUS = 59,
        HI_CARD_TYPE = 60,
        RESOURCE_STORE_REASON = 61,
        //
        PURCHASE_ORDER_STATUS = 63,
        PAYMENT_REQ_STATUS = 64,

        V_DeptTypeOperation = 71,

        //ResourceMaintenanceLog            
        V_RscrInitialStatus = 80,
        V_CurrentStatus = 90,
        V_RscrFinalStatus = 100,
        V_StatusIssueMaintenance = 200,//trang thai 1 van de di bao tri: Open, Inprocessing, Close
        //ResourceMaintenanceLog
        V_GoodsType = 130,
        V_MedProductType = 110,

        V_ResGroupCategory = 72,
        V_SupplierType = 73,
        V_Operation = 91,
        V_RegistrationType = 230,

        V_RoomFunction = 42,

        V_PCLRequestType = 240,
        V_PCLRequestStatus = 241,

        V_DiagIcdStatus = 141,
        V_RegForPatientOfType = 235,
        V_PCLCategory = 271,
        V_OutDrugInvStatus = 150,

        V_PeriodOfDay = 280,
        PatientQueueItemsStatus = 210,

        V_PCLMainCategory = 282,

        REGISTRATION_PAYMENT_STATUS = 281,

        V_ValveOpen = 284,

        V_PCLExamTypeUnit = 285,

        V_RefMedServiceItemsUnit = 286,

        V_RefMedicalServiceTypes = 287,


        V_CardialStatus = 288,
        V_CardialResult = 289,
        V_Situs = 290,
        V_RefMedicalServiceInOutOthers = 300,
        V_BillingInvType = 401,
        V_InPatientBillingInvStatus = 402,
        V_RefGenDrugUnitType = 403,
        V_OutputTo = 404,
        InPatientDeptStatus = 405,
        RegistrationClosingStatus = 406,
        DischargeCondition = 408,
        V_PharmacyOutRepType = 409,
        V_PaymentReason = 500,
        ServiceItemType = 501,
        V_ByOutPrice = 510,
        V_ByOutPriceMedDept = 520,
        V_TradingPlaces = 530,
        V_DrugType = 550,
        V_CashAdvanceType = 560,
        V_DiagnosisType = 570,
        //V_HospitalType=3
        V_StockTakeType = 601,
        V_GroupTypeForReport20 = 602,
        V_VENType = 603,
        //▼===== #003
        V_CatDrugType = 822,
        //▲===== #003
        V_AccidentCode = 609,
        V_GenPaymtReasonTK = 610,
        V_GenericPaymentType = 611,
        V_RouteOfAdministration = 615,
        V_HIReportType = 616,
        V_ReqPaymentStatus = 617,
        V_RefundPaymentReasonInPt = 618,
        //==== 20161213 CMN Begin: Add lookup for FetalEchocardiography
        V_EchographyPosture = 619,
        V_MomMedHis = 620,
        //#001
        V_InfusionProcessType = 622,
        V_InfusionType = 623,
        V_TimeIntervalUnit = 624,
        V_LevelCare = 625,
        //#001
        V_TransferType = 626,
        V_TreatmentResult = 627,
        V_CMKT = 628,
        V_TransferReason = 629,
        V_PatientStatus = 630,
        V_TransferFormType = 631,
        CriterialTypes = 632,
        V_AcademicRank = 690,
        V_AcademicDegree = 691,
        V_Education = 692,
        V_ExpenditureSource = 700,
        V_PayReson = 710,
        V_ActivityType = 720,/*DPT*/
        V_Surgery_Tips_Type = 730, /*TMA*/
        V_DeadReason = 740,
        V_TrainingType = 750,
        V_CatastropheType = 760,
        V_Surgery_Tips_Item = 770, /*TMA*/
        /*TMA 10/11/2017 V_CharityObjectType*/
        V_CharityObjectType = 790,
        V_ActivityMethodType = 800, /*DPT*/
        V_ActivityClassType = 810,/*DPT*/
        V_ProductScope = 820,
        V_PatientSubject = 821,
        V_TreatmentPeriodic = 823,
        /*▼====: #004*/
        V_TreatmentType = 825, //Cach dieu tri
        /*▲====: #004*/
        V_DiscountReason = 826,
        V_Ekip = 827,
        V_WarningLevel = 828,
        V_InteractionSeverityLevel = 829,
        V_HosClientType = 830,
        V_EkipIndex = 831,
        V_HealthyClassification = 832,
        V_AnesthesiaType = 833,
        V_SurgicalMode = 834,
        V_CatactropheType = 835,
        V_SpecialistType = 836,
        V_InfectionType = 838,
        V_BloodSmearResult = 839,
        V_BloodSmearMethod = 840,
        V_InfectionCaseStatus = 841,
        V_DiagnosysConsultationType = 842,
        V_ReconmendTime = 843,
        V_OutwardTemplateType = 844,
        V_AntibioticTreatmentType = 845,
        V_InstructionOrdinalType = 846,
        V_RangeOfWareHouses = 847,
        //▼===== #007
        V_79AExportType = 848,
        //▲===== #007
        //▼===== #009
        V_DiscountTypeCount = 851,
        //▲===== #009
        V_FastReportType = 855,
        //▼====: #010
        V_CheckMedicalFilesStatus = 856,
        //▲====: #010
        V_BedType = 857,
        //▼====: #011
        V_HospitalClass = 858,
        //▲====: #011
        V_EatingType = 859,
        V_ExaminationType = 860,
        V_SGAType = 861,
        V_NutritionalRequire = 862,
        V_NutritionalMethods = 863,
        V_Job = 854,
        V_Ethnic = 5,
        V_SpecialistClinicType = 864,
        V_AdmissionCriteriaType = 866,
        V_ReportForm = 867,
        V_SymptomType = 869,
        V_ReconmendTimeUsageDistance = 870,
        V_TransferRateUnit = 871,
        V_AgeUnit = 872,
        V_MedicalServiceGroupType = 873,
        V_ObjectType = 874,
        V_MedicalServiceParentGroup = 875,
        V_PertrolCode = 876,
        V_ConsciousnessLevel = 877,
        V_PainLevel = 878,
        V_EInvoiceStatus = 879,
        //▼==== #015
        V_OutPtTreatmentStatus = 880,
        //▲==== #015
        V_MedicalFileType = 881,
        V_ExpiryTime = 882,
        V_OutDischargeType = 883,
        V_OutDischargeCondition = 884,
        V_MedicalFileStatus = 885,
        V_ReasonType = 886,
        V_TransactionStatus = 887,
        //▼==== #017
        V_PatientClass = 890,
        //▲==== #017
        //▼====: #017
        V_PrescriptionIssuedType = 889,
        //▲====: #017
        //▼==== #020
        V_HealthRecordsType = 891,
        //▲==== #020
        //▼====: #021
        V_OvertimeWorkingWeekStatus = 892,
        //▲====: #021
        //▼====: #027
        V_SurgicalBirth = 893,
        V_BirthUnder32 = 894,
        //▲====: #027
        V_ReasonHospitalStay = 895,
        V_ReasonAdmission = 896,
        V_ReceiveMethod = 897,
        V_ObjectMedicalExamination = 898,
        V_JobPosition = 899,
        //▼==== #023
        V_MRBacteria_Level = 900,
        V_HosInfection_Type = 901,
        V_PaymentMethods = 902,
        //▲==== #023
        V_MedicalExaminationType = 903,
        //▼==== #028
        V_DrugFormulationMethod = 904,
        V_PaymentSource = 905,
        V_SurgicalSite = 906,
        //▼==== #029
        V_RscrStatus = 908,
        //▲==== #029
        V_U22WPregnancy = 909,
        V_O22WPregnancy = 910,
        //▲==== #028
        //▲==== #027
        //▼==== #030
        V_Prognosis = 911,
        V_ResultsEvaluation = 912,
        //▲==== #030
        //▼==== #031
        V_ExaminationProcess = 913,
        //▲==== #031
        //▼==== #032
        V_HIReportStatus = 914,
        //▲==== #032
        //▼==== #033
        V_SendSMSStatus = 917
        //▲==== #033
    }

    // 20210610 TNHX: 331 Dựa vào mạch, huyết áp của "y lệnh theo dõi sinh hiệu" của y lệnh gần nhất để biết có cần nhập lại DHST không
    public enum V_ReconmendTime
    {
        [Description("5 phút")]
        FIVE_MINUTES = 84300,
        [Description("10 phút")]
        TEN_MINUTES = 84301,
        [Description("15 phút")]
        FIFTY_MINUTES = 84302,
        [Description("30 phút")]
        THIRTY_MINUTES = 84303,
        [Description("1 tiếng")]
        ONE_HOUR = 84304,
        [Description("2 tiếng")]
        TWO_HOURS = 84305,
        [Description("4 tiếng")]
        FOUR_HOURS = 84306,
        [Description("6 tiếng")]
        SIX_HOURS = 84307,
        [Description("12 tiếng")]
        TWELVE_HOURS = 84308,
        [Description("24 tiếng")]
        TWENTYFOUR_HOURS = 84309,
        [Description("3 tiếng")]
        THREE_HOURS = 84310,
        [Description("5 tiếng")]
        FIVE_HOURS = 84311,
        [Description("8 tiếng")]
        EIGHT_HOURS = 84312,
        [Description("20 phút")]
        TWENTY_MINUTES = 84313,
        //▼==== #014
        [Description("Ngưng theo dõi")]
        UNFOLLOW = 84314
        //▲==== #014
    }
    public enum RoomFunction
    {
        CHUA_XAC_DINH = 4100,
        KHAM_BENH = 4101,
        LUU_BENH = 4102
    }
    public enum VRoomType
    {
        Khoa = 7000,
        Phòng = 7001,
        Kho = 7002,
        Khoa_Phong_kho = 7003
    }

    public enum StaffRegistrationType
    {
        NORMAL = 0,
        INSURANCE = 1
    }
    public enum PaymentProcessorType
    {
        Normal = 1,
        HealthInsurance = 2
    }
    public enum StaffCatg
    {
        None = 0,
        Bs = 1,
        BsDongY = 2,
        BsRHM = 3,
        BsSan = 4,
        BsTMH = 5,
        BaoVe = 6,
        ChuyenVien = 7,
        DieuDuong = 8,
        DuocSy = 9,
        DuocTa = 10,
        HoLy = 11,
        KSXetNghiem = 12,
        KySu = 13,
        KyThuatVien = 14,
        LaiXe = 15,
        NhaSy = 16,
        NvHanhChanh = 17,
        NuHoSinh = 18,
        TroThuNha = 19,
        YCong = 20,
        YSy = 21,
        YTa = 22,
        NhanVienDangKy = 23,
        DuocTrung = 24,
        TS_BS = 25,
        ThS_BS = 26,
        ThuKyYKhoa = 37
    }

    public enum V_AppointmentType
    {
        HenTaiKham = 3801,
    }

    public enum RegDetailStatus
    {
        None = 0,
        NotDiagYet = 1,
        HasDiag = 2,
        DiagForAnotherDept = 3,
        AnotherDeptGetDiag = 4
    }
    public enum V_DeptTypeOperation
    {
        KhoaNgoaiTru = 7010,
        KhoaNoi = 7011,
        KhoaNgoai = 7012,
        KhoaCanLamSang = 7013,
        PhongDangKy = 7015
    }

    public enum V_StaffCatType
    {
        NhanVienKyThuat = 6102,
        BacSi = 6100,
        NhanVienQuayDangKy = 6101,
        PhuTa = 6103,
        Duoc = 6106
    }
    public enum V_TimeSegment
    {
        Ca1 = 56100,
        Ca2 = 56101,
        Ca3 = 56102,
        Ca4 = 56103,
        Ca5 = 56104,
        CaNgay = 56105
    }


    public enum V_ExamRegStatus
    {
        mDangKyKham = 701,
        mChoKham = 702,
        mBatDauThucHien = 703,
        mHoanTat = 704,
        mKhongXacDinh = 700,
        mNgungTraTienLai = 707,
    }
    public enum V_OutPtEntStatus
    {
        mChoKham = 85100,
        mChoCLS = 85101,
        mChoThuThuat = 85102,
        mHoanTat = 85103
    }
    public enum V_RegistrationError
    {
        mNone = 0,
        mRefresh = 1,
    }

    public enum V_AppointmentError
    {
        mNone = 0,
        [Description("Phòng không có chỉ tiêu")]
        mNotTarget = 1,
        [Description("Vượt quá chỉ tiêu phòng")]
        mOverTarget = 2,
    }

    public enum eFirePharmacySupplierEvent
    {
        EstimationPharmacy = 1
    }

    public enum eFirePharmacieucalCompanyEvent
    {
        EstimationPharmacy = 1
    }

    //KMx: Copy trong class Helpers bên client (11/05/2016 11:53).
    public class EnumTool
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes
              (typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }

    public static class AllLookupValues
    {
        public enum Dosage
        {
            MDose = 1,
            ADose = 2,
            EDose = 3,
            NDose = 4
        }

        //HPT: Mã KV cho thẻ bảo hiểm (15/03/2016)
        public enum KVCode
        {
            KV1 = 61131,
            KV2 = 61132,
            KV3 = 61133,
        }
        public enum SessionActivity
        {
            DANG_KY_KHAM_BENH = 4400,
            THANH_TOAN_TIEN_KHAM_BENH = 4401,
            KHAM_BENH = 4402,
            MUA_THUOC = 4403
        }

        public enum QueueType
        {
            THANH_TOAN_TIEN_KHAM = 4500,
            KHAM_BENH = 4501,
            PCL = 4502,
            MUA_THUOC = 4503,
            CHO_DANG_KY = 4504,
            DANG_KY_HEN_BENH = 4505,
        }

        public enum FamilyRelationship
        {
            VO = 501,
            CHONG = 502,
            CON = 503,
            CHA = 504,
            ME = 505,
            ANH = 506,
            CHI = 507,
            ONG = 508,
            BA = 509,
            KHAC = 510
        }

        public enum DocumentTypeOnHold
        {
            THE_BAO_HIEM = 601,
            CHUNG_MINH_THU = 602,
            GIAY_GIOI_THIEU = 603,
            CAC_GIAY_TO_KHAC = 604
        }
        public enum FileStorageResultType
        {
            UNKNOW = 4600,
            IMAGES = 4601,
            VIDEO_RECORDING = 4602,
            DOCUMENTS = 4603,
            OTHERS = 4604
        }
        public enum PatientPCLRequestCategory
        {
            PCL_IMAGING = 4700,
            PCL_LABORATORY = 4701
        }
        public enum TranHIPayment
        {
            OPENED = 5300,
            DISPUTING = 5301,
            BALANCED = 5302,
            CONSOLIDATED = 5303,
            CLOSED = 5304
        }
        public enum TranPatientPayment
        {
            OPENED = 5400,
            DISPUTING = 5401,
            BALANCED = 5402,
            CLOSED = 5403
        }

        public enum InPtAdmissionStatusEnum
        {
            [Description("Chưa Xác Định")]
            UNKNOWN = 0,
            [Description("Chờ Nhập Viện")]
            WAITING_FOR_ADMISSION = 1,
            [Description("Nhập Viện")]
            ADMITTED = 2,
            [Description("Da Lam Giay Xuat Vien")]
            DISCHARED_PAPER_DONE = 3,
            [Description("Đã Xuất Viện")]
            DISCHARED = 4,
            [Description("Tạm - Xuất Viện")]
            TEMPORARY_DISCHARGED = 5
        }

        public enum RegistrationStatus
        {
            [Description("Đang mở")]
            OPENED = 5500,
            [Description("Đang xử lý")]
            PROCESSING = 5501,
            [Description("Hoàn tất")]
            COMPLETED = 5502,
            [Description("Không hợp lệ")]
            INVALID = 5503,
            //[Description("Bị khóa")]
            //LOCKED = 5504,
            [Description("Ngưng trả tiền lại")]
            REFUND = 5505,
            [Description("Đang chờ")]
            PENDING = 5506,
            //[Description("Đang chờ xác nhận khóa lại")]
            //PENDINGCLOSE = 5507,
            [Description("Benh Nhan Noi Tru Da Dang Ky Cho Nhap Vien")]
            PENDING_INPT = 5510,
            [Description("Benh Nhan Noi Tru Da Nhap Vien")]
            ADMITTED_INPT = 5515,
            [Description("Benh Nhan Noi Tru Da Xuat Vien")]
            DISCHARGED_INPT = 5520,
            [Description("Benh Nhan Noi Tru Da Xuat Vien Hoan Tat Moi Thu Tuc")]
            COMPLETED_INPT = 5525
        }
        public enum RegistrationPaymentStatus
        {
            //[Description("Chưa biết")]
            //NIL = 28100,//281,28100
            [Description("Thẻ tín dụng")]
            CREDIT = 28101,
            [Description("Ghi nợ")]
            DEBIT = 28102,
            //[Description("Đang mở")]
            //OPENED=28103,
        }
        public enum ResourceUnit
        {
            Rscr_Cai = 5700,
            Rscr_Chiec = 5701,
            Rscr_Tam = 5702,
            Rscr_To = 5703,
            Rscr_Tep = 5704,
            Rscr_Vien = 5705,
            Rscr_Mieng = 5706,
            Rscr_Que = 5707,
            Rscr_Chai = 5708,
            Rscr_Ong = 5709,
            Rscr_Soi = 5710,

            Rscr_Cay = 5711,
            Rscr_Cap = 5712,
            Rscr_Cuon = 5713,
            Rscr_Binh = 5714,
            Rscr_Bo = 5715,
            Rscr_Goi = 5716,
            Rscr_Hop = 5717,
            Rscr_Xap = 5718,
            Rscr_Thung = 5719,
            Rscr_Bang = 5720
        }
        public enum PaymentMode
        {
            [Description("Tiền mặt")]
            TIEN_MAT = 4800,
            [Description("Thẻ tín dụng")]
            THE_TIN_DUNG = 4801,
            [Description("Chuyển khoản")]
            CHUYEN_KHOAN = 4802,
            [Description("Thẻ khám bệnh")]
            THE_KHAM_BENH = 4803,
            [Description("Thanh toán online")]
            THANH_TOAN_ONLINE = 4804,
            [Description("Thanh toán qua ứng dung")]
            THANH_TOAN_QUA_APP = 4805
        }
        public enum PaymentType
        {
            [Description("Tạm ứng")]
            TAM_UNG = 5000,
            [Description("Trả đủ")]
            TRA_DU = 5001,
            [Description("Trả nhiều đợt")]
            TRA_NHIEU_DOT = 5002,
            [Description("Hoàn tiền")]
            HOAN_TIEN = 5003
        }
        public enum Currency
        {
            VND = 4900,
            USD = 4901
        }
        public enum CurrencyTable
        {
            VND = 129
        }

        public enum ApptStatus : long
        {
            [Description("Đã xác nhận")]
            BOOKED = 5600,
            [Description("Nhắc")]
            ALERTED = 5601,
            [Description("Bệnh nhân xác nhận")]
            CONFIRMED = 5602,
            [Description("Đã thực hiện")]
            ACTIONED = 5603,
            [Description("Đã hủy")]
            CANCELED = 5604,
            [Description("Chờ xác nhận")]
            PENDING = 5605,
            [Description("Chưa Xác Định")]
            INVALID = 5606,
            [Description("Đang chờ duyệt")]
            WAITING = 5607,
            [Description("Chưa xác định")]
            UNKNOWN = 5608
        }
        public enum AppointmentType
        {
            [Description("Chưa xác định")]
            UNKNOWN = 3800,
            [Description("Tái khám")]
            HEN_TAI_KHAM = 3801,
            [Description("Khám bệnh")]
            HEN_KHAM_BENH = 3802,
            [Description("Khám định kỳ")]
            HEN_KHAM_DINH_KY = 3803,
            [Description("Tái khám sau phẫu thuật")]
            HEN_TAI_KHAM_SAU_PHAU_THUAT = 3804,
            [Description("Gặp BS tư vấn")]
            HEN_GAP_BAC_SI_TU_VAN = 3805,
            [Description("Lấy KQ xét nghiệm")]
            HEN_LAY_KET_QUA_XET_NGHIEM = 3806,
            [Description("Khám sức khỏe")]
            HEN_KHAM_SUC_KHOE = 3807,
            [Description("Khám cận lâm sàng sổ")]
            HEN_CAN_LAM_SANG_SO = 3808,
            [Description("Điều trị ngoại trú nhiều đăng ký")]
            HEN_DTNT_NHIEU_DK = 3809,
            [Description("Điều trị ngoại trú một đăng ký")]
            HEN_DTNT_MOT_DK = 3810

        }
        public enum HICardType
        {
            MAU_2008 = 5901,
            MAU_2011 = 5904,
            MAU_2013_TPHCM = 5905
        }

        public enum ProcessingType
        {
            MEDICAL_CONDITIONS = 901,
            MEDICAL_HISTORY = 902,
            IMMUNIZATION = 903,
            VITAL_SIGNS = 904,
            FAMILY_HISTORY = 905,
            HOSPITALIZATION_HISTORY = 906,
            DEATH_SITUATION_INFO = 907,
            PHYSICAL_EXAMINATION = 908,
            PARA_CLINICAL_EXAMINATION = 909,
            EXAMINE_AND_TEST = 910,
            PRESCRIPTION = 911,
            CLINICAL_INDICATOR = 912,
            MEDICAL_ALLERGY = 913,
            DIAGNOSIS_AND_TREATMENT = 914,
            MEDICAL_WARNING = 915,
            UNKNOWN = 916,
            OTHERS = 917
        }

        public enum Behaving
        {
            KHAM_DIEU_TRI = 3301,
            RATOA = 3302,
            KHAM_DIEU_TRI_NGOAI_TRU = 3303,
            TAI_KHAM = 3304,
            KHAM_VE = 3305,
            YEU_CAU_NHAP_VIEN = 3306,
            CHUYEN_VIEN = 3307,
            CHUYEN_PHONG_KHAM = 3308,
            THAY_DOI_DU_LIEU = 3309,
            KHONG_XAC_DINH = 3310,
            CHI_DINH_XET_NGHIEM_CLS = 3311
        }
        public enum ExamRegStatus
        {
            [Description("Chưa xác định")]
            KHONG_XAC_DINH = 700,
            [Description("Đăng ký khám")]
            DANG_KY_KHAM = 701,
            [Description("Bắt đầu thực hiện")]
            BAT_DAU_THUC_HIEN = 703,
            [Description("Hoàn tất")]
            HOAN_TAT = 704,
            [Description("Ngưng trả tiền lại")]
            NGUNG_TRA_TIEN_LAI = 707,
            [Description("Xóa trả tiền lại")]
            XOA_TRA_TIEN_LAI = 708
        }
        public enum StaffCatType
        {
            BAC_SI = 6100,
            NHAN_VIEN_DANG_KY = 6101
        }
        public enum PatientSummary
        {
            [Description("--Tất Cả--")]
            ChonLoaiDV = 1,
            [Description("Chẩn Đoán")]
            KhamBenh_ChanDoan = 2,
            [Description("Toa Thuốc")]
            ToaThuoc = 3,
            [Description("Cận Lâm Sàng Xét Nghiệm")]
            CanLamSang_XetNghiem = 4,
            [Description("Cận Lâm Sàng Hình Ảnh")]
            CanLamSang_HinhAnh = 5,
            [Description("Cận Lâm Sàng Hình Ảnh Ngoại Viện")]
            CanLamSang_HinhAnh_NgoaiVien = 6,
            [Description("Hội Chẩn")]
            HoiChan = 7,
            [Description("Giải Phẫu Kỹ Thuật Cao")]
            GiaiPhauKyThuatCao = 8,
            [Description("Nội Trú")]
            NoiTru = 9,
            [Description("Thủ thuật")]
            ThuThuat = 10,
            [Description("Cận Lâm Sàng Hình Ảnh")]
            CanLamSang_TieuChiNV = 11,
            count = 12,
        }

        public enum ResGroupCategory
        {
            THIET_BI_Y_TE = 7100,
            THIET_BI_VAN_PHONG = 7101,
            KHAC = 7102
        }
        public enum SupplierType
        {
            CUNG_CAP_THIET_BI_Y_TE = 7200,
            CUNG_CAP_THIET_BI_VAN_PHONG = 7201,
            KHAC = 7202
        }

        public enum V_Operation
        {
            Full_Control = 7301,
            View = 7302,
            Add = 7303,
            Update = 7304,
            Delete = 7305,
            Report = 7306,
            Print = 7307
        }

        public enum MedProductType
        {
            [Description("Chưa xác định")]
            Unknown = 11000,
            [Description("Thuốc")]
            THUOC = 11001,
            [Description("Y cụ")]
            Y_CU = 11002,
            [Description("Hóa chất")]
            HOA_CHAT = 11003,
            [Description("KCB")]
            KCB = 11004,           //may gia tri nay them vao cho tong quat. Khong co trong DB.
            [Description("CLS")]
            CAN_LAM_SANG = 11005,
            [Description("Tiền giường")]
            TIEN_GIUONG = 11006,
            [Description("Hoạt chất")]
            HOAT_CHAT = 11007,
            // 20190117 Add for new "Kho"
            [Description("Vật tư y tế tiêu hao")]
            VTYT_TIEUHAO = 11008,
            [Description("Tiêm ngừa")]
            TIEM_NGUA = 11009,
            [Description("Máu")]
            MAU = 11010,
            [Description("Văn phòng phẩm")]
            VAN_PHONG_PHAM = 11011,
            [Description("Vật tư tiêu hao")]
            VATTU_TIEUHAO = 11012,
            [Description("Thanh trùng")]
            THANHTRUNG = 11013,
            [Description("Dinh Dưỡng")]
            NUTRITION = 11014,
        }

        /*TMA 18/10/2017 CBO cho sổ kiểm nhập thuốc hoá chất ....*/
        public enum MedProductType2 : long
        {
            [Description("Thuốc")]
            THUOC = 11001,
            [Description("Y cụ")]
            Y_CU = 11002,
            [Description("Hóa chất")]
            HOA_CHAT = 11003
        }
        /*TMA 18/10/2017 CBO cho sổ kiểm nhập thuốc hoá chất ....*/
        public enum RegistrationType
        {
            [Description("Chưa xác định")]
            Unknown = 24000,
            [Description("Ngoại Trú")]
            NGOAI_TRU = 24001,
            //[Description("Ngoại Trú -> Nội Trú")]
            //NGOAI_TRU_NOI_TRU = 24002,
            [Description("Nội Trú")]
            NOI_TRU = 24003,
            //[Description("VIP")]
            //DANGKY_VIP=24004
            NOI_TRU_BHYT = 24005,
            CAP_CUU = 24006,
            CAP_CUU_BHYT = 24007,
            XAC_NHAN_LAI_BHYT = 24008,
            DIEU_TRI_NGOAI_TRU = 24009
        }


        public enum V_RegForPatientOfType
        {
            [Description("Chua xac dinh")]
            Unknown = 0,
            [Description("DK BN Moi")]
            DK_BN_MOI = 24100,
            [Description("DK BN NGOAI TRU")]
            DKBN_NGTRU = 24102,
            [Description("DK BN Tai Kham")]
            DK_BN_TAI_KHAM = 24105,
            [Description("DK BN Tai Kham Sau GP")]
            DK_BN_TAI_KHAM_SAU_GP = 24110,
            [Description("Điều trị Nội trú")]
            NBNT_BN_KHONG_BHYT = 24115,
            [Description("Điều trị Nội trú BHYT")]
            NBNT_BN_CO_BHYT = 24120,
            [Description("NBNT BN Cap Cuu Khong BHYT")]
            NBNT_BN_CAP_CUU_KHONG_BHYT = 24125,
            [Description("NBNT BN Cap Cuu Co BHYT")]
            NBNT_BN_CAP_CUU_CO_BHYT = 24130,
            [Description("NBNT BN Tien Phau Khong BHYT")]
            NBNT_BN_TIEN_PHAU_KHONG_BHYT = 24135,
            [Description("NBNT BN Tien Phau Co BHYT")]
            NBNT_BN_TIEN_PHAU_CO_BHYT = 24140,
            [Description("Tiền giải phẫu")]
            DKNT_BN_GP_DEPOSIT = 24145,
            [Description("DKNT BN KTC DEPOSIT")]
            DKNT_BN_KTC_DEPOSIT = 24148,
            [Description("Vãng lai")]
            DKBN_VANG_LAI = 24150,
            [Description("Điều trị Ngoại trú")]
            DKBN_DT_NGOAI_TRU = 24151
        }

        public enum V_FindPatientType
        {
            NGOAI_TRU = 0,
            NOI_TRU = 1,
        }

        public enum V_EstimateType
        {
            FIRSTMONTH = 12001,
            MODIFYMONTH = 12002,
            FIRSTYEAR = 12003,
            MODIFYYEAR = 12004,
            ADDITION_FIRSTMONTH = 12005
        }

        public enum V_MedicalMaterial : long
        {
            VTYT_THAYTHE = 59001,
            VTYT_TIEUHAO = 59002,
        }

        public enum V_GoodsType
        {
            HANGMUA = 13001,
            HANGTANG = 13002,
            HANGMAU = 13003,
            HANG_NHAP_TU_LUAN_CHUYEN_KHO = 13004
        }

        public enum V_PCLRequestType : long
        {
            UNKNOWN = 25000,
            NGOAI_TRU = 25001,
            NOI_TRU = 25002
        }

        public enum V_PCLRequestStatus : long
        {
            OPEN = 26000,
            CLOSE = 26001,
            CANCEL = 26002,
            PROCESSING = 26003,
            SIGNING = 26004,
            SIGNED = 26005,
            ALL = -2
        }

        public enum StoreType
        {
            STORAGE_EXTERNAL = 1,
            STORAGE_DRUGDEPT = 2,
            STORAGE_CLINIC = 3,
            STORAGE_CLINIC_OTHER = 4,
            STORAGE_OUT_HOSPITAL = 5,
            STORAGE_FILES = 6,
            STORAGE_HIDRUGs = 7
        }

        //▼====: #005
        public enum StoreID
        {
            KHO_LE_THUOC_NOI_TRU = 174,
            KHO_LE_VTYT = 175
            //20191002 [UPDATE] [BM0017396] Change AllLookupValues.KHO_LE_THUOC_BHYT_NGOAI_TRU -> RefApplicationConfig.HIStoreID
            //KHO_LE_THUOC_BHYT_NGOAI_TRU = 160
        }
        //▲====: #005

        public enum V_DiagIcdStatus : long
        {
            DANGDIEUTRI = 14001,
            HETBENH = 14002

        }
        public enum V_PrescriptionNotes : long
        {
            TOAGOC = 4301,
            COPYTUTOAKHAC = 4302,
            EDITTUTOAKHAC = 4303
        }
        public enum V_PrescriptionNoteTempType : long
        {
            [Description("Lời Dặn Của Bác Sĩ")]
            PrescriptionNoteGen = 53101,
            [Description("Ghi Chú Cách Dùng")]
            PrescriptionNoteItem = 53102,
            [Description("Cách Dùng")]
            PrescriptionAdministration = 53103,
            //[Description("Đơn Vị Dùng")]
            //PrescriptionUnitUse = 53104,     
            [Description("Hẹn Cận Lâm Sàng")]
            AppointmentPCLTemplate = 53105,
            [Description("Trình tự thủ thuật")]
            SmallProceduresSequence = 53106,
            //▼====: #011
            [Description("Hồ sơ điều trị ngoại trú - phương pháp điều trị")]
            Treatments = 53107,
            [Description("Hồ sơ điều trị ngoại trú - hướng điều trị và chế độ tiếp theo")]
            TreatmentDirectionFollowupRegimen = 53108
            //▲====: #011
        }

        public enum V_PrescriptionStatus
        {
            New = 1,
            Edit = 2,
            NewOnOld = 3,
        }

        //public enum V_PCLCategory : long
        //{
        //    Imaging = 27001,
        //    Laboratory = 27002,
        //    Pathology = 27003
        //}
        public enum V_DeptType : long
        {
            Khoa = 7000,
            Phong = 7001,
            Kho = 7002
        }
        public enum RefOutputType : long
        {
            HOANTRATHUOC = 1,
            XUATNOIBO = 2,
            CANBANGKHO = 3,
            BANTHEOTOA = 4,
            BANLE = 5,
            HUYHANG = 6,
            THUHOITHUOC = 7,
            HUYPHIEUXUAT = 8,
            XUATNOIBO_CHOMUON = 14,
            XUATCHO_BIEU = 17,
            NHAP_TU_NCC = 9,
            NHAP_TU_NGUON_KHAC = 10,
            NHAP_KHAC = 12,
            XUAT_HANGKYGOI = 18,
            NHAP_HANGKYGOI = 19,
            //KMx: Created Date: 14/02/2014 16:51
            XUATNOIBO_LUANCHUYENKHO = 20,
            NHAP_TU_LUANCHUYENKHO = 21,
            //KMx: Created Date: 27/04/2015 10:12
            XUAT_KHOPHONG = 22,
            XUAT_BN = 23,
            XUAT_TRA_KHOADUOC = 24,
            XUAT_DUNGCHUNG = 25,
            XUAT_TRA_NCC = 26,
            NHAP_TU_KHO_BHYT_NHA_THUOC = 27,
            NHAP_TU_KHO_DUOC_CHO_KHO_BHYT_NHA_THUOC = 28,
            THANHLY = 29,
            XUAT_DIEUCHUYEN = 30
        }
        public enum V_OutDrugInvStatus : long
        {
            SAVE = 15000,
            PAID = 15001,
            DRUGCOLLECTED = 15002,
            CANCELED = 15003,
            REFUNDED = 15004,
            RETURN = 15005
        }
        public enum V_PurchaseOrderStatus : long
        {
            NEW = 6200,
            ORDERED = 6201,
            FULL_DELIVERY = 6202,
            PART_DELIVERY = 6203,
            NO_WAITING = 6204
        }
        public enum V_PaymentReqStatus : long
        {
            NEW = 6300,
            WAITING_APPROVED = 6301,
            APPROVED = 6302
        }

        public enum V_PeriodOfDay : long
        {
            SANG = 28000,
            TRUA = 28001,
            CHIEU = 28002,
            TOI = 28003
        }

        public enum PatientQueueItemsStatus : long
        {
            WAITING = 21000,
            PROCESSING = 21001,
            DONE = 21002
        }


        public enum V_PCLMainCategory : long
        {
            Unknown = 0,
            Imaging = 28200,
            Laboratory = 28201,
            Pathology = 28202,
            Laboratory_External = 28203,
            GeneralSugery = 28204
        }

        public enum PCLResultParamImpID
        {
            [Description("Siêu âm tim màu")]
            SIEUAM_TIMMAU = 1,
            [Description("Siêu âm mạch máu")]
            SIEUAM_MACHMAU = 2,
            [Description("Siêu âm gắng sức Dipyridamole")]
            SIEUAM_GANGSUC_Dipyridamole = 3,
            [Description("Siêu âm gắng sức Dobutamine")]
            SIEUAM_GANGSUC_Dobutamine = 4,
            [Description("Siêu âm tim thai")]
            SIEUAM_TIMTHAI = 5,
            [Description("Siêu âm thực quản")]
            SIEUAM_THUCQUAN = 6,
            [Description("Siêu âm bụng")]
            ABDOMINAL_ULTRASOUND = 14,
            [Description("Khác")]
            KHAC = 7,
            [Description("Liệu trình điều trị")]
            GeneralSurgery = 28
            //count = 9
        }


        #region Loại Danh Sách Phiếu CLâm Sàng
        public enum PatientPCLRequestListType
        {
            DANHSACHPHIEUYEUCAU = 0
            //DANHSACHPHIEU_CHOTHUCTHIEN = 2,
            //DANHSACHPHIEU_DANGTHUCTHIEN = 3,
            //DANHSACHPHIEU_THUCHIENXONG = 4
        }
        #endregion


        public enum V_PCLExamTypeUnit
        {
            LAN = 28500
        }

        public enum V_PCLExamTypeExt
        {
            [Description("Kết Quả CLS Hình Ảnh Ngoại Viện")]
            NgoaiVien = 931
        }

        public enum V_RefMedServiceItemsUnit
        {
            LAN = 28600,
            NGAY = 28601
        }

        /*TMA*/
        public enum V_Surgery_Tips_Type
        {
            [Description("Loại Đặc biệt")]
            LOAIDACBIET = 73001,
            [Description("Loại 1A")]
            LOAI1A = 73002,
            [Description("Loại 1B")]
            LOAI1B = 73003,
            [Description("Loại 1C")]
            LOAI1C = 73004,
            [Description("Loại 2A")]
            LOAI2A = 73005,
            [Description("Loại 2B")]
            LOAI2B = 73006,
            [Description("Loại 2C")]
            LOAI2C = 73007,
            [Description("Loại 3")]
            LOAI3 = 73008
        }

        public enum V_Surgery_Tips_Item
        {
            [Description("Phẫu thuật")]
            PHAUTHUAT = 77001,
            [Description("Thủ thuật")]
            THUTHUAT = 77002
        }
        /*TMA*/

        public enum V_RefMedicalServiceTypes
        {
            KHAMBENH = 28700,
            CANLAMSANG = 28701,
            GIUONGBENH = 28702,
            DICHVUHANHCHANH = 28703,
            HOICHAN = 28704,
            THUTHUAT = 28705,
            KYTHUATCAO = 28706,
            PHUCHOICHUCNANG = 28707,
            MAU = 56008,
        }

        public enum ChoiceEnum
        {
            //Khong=0,
            //Co=1,
            //NghiNgo=2,
            KhongBiet = 0,
            Khong = 1,
            Co = 2,
            NghiNgo = 3,
        }
        public enum V_TranRefType : long
        {
            NONE = 0,
            DRUG_NGOAITRU = 40000,
            DRUG_NOITRU_KHODUOC = 40001,
            DRUG_NOITRU_KHOCUAKHOA = 40002,
            PAY_CLS = 40004,
            BILL_NOI_TRU = 40005,
            BILL_THANH_TOAN = 40006
        }
        public enum V_RefMedicalServiceInOutOthers
        {
            NGOAITRU = 30000,
            NOITRU = 30001,
            NOITRU_NGOAITRU = 30002,
            HANHCHANH_NGOAITRU = 30003,
        }

        public enum V_BillingInvType : long
        {
            [Description("Chưa xác định")]
            UNKNOWN = 40110,
            [Description("Tính tiền nội trú")]
            TINH_TIEN_NOI_TRU = 40100,
            [Description("Tính tiền giải phẫu")]
            TINH_TIEN_GIAI_PHAU = 40101
        }
        public enum V_InPatientBillingInvStatus : long
        {
            [LocalizedDescription("Z0013_G1_Moi")]
            NEW = 40200,
            [Description("Ngưng trả tiền lại")]
            NGUNG_TRA_TIEN_LAI = 40201
        }

        public enum V_RefGenDrugUnitType
        {
            BAN_CHO_BENHNHAN = 40300,
            BENHNHAN_UONG = 40301
        }

        public enum V_DrugType : long
        {
            [Description("Thuốc Thông Thường")]
            THUOC_THONGTHUONG = 53200,
            //[Description("Thuốc Ngoài Danh Mục")]
            //THUOC_NGOAIDANH_MUC = 53201,
            [Description("Thuốc Uống Khi Cần")]
            THUOC_UONGKHICAN = 53202,
            [Description("Thuốc Uống Theo Lịch Tuần")]
            THUOC_UONGLICHTUAN = 53203,
        }

        public enum V_Color
        {
            Normal = 1,
            Red = 2,
            Green = 3,
            Blue = 4,
            Pink = 5,
            Brown = 6,
            Yellow = 7,
        }

        public enum V_OutputTo
        {
            [Description("Kho khác")]
            KHO_KHAC = 40401,
            [Description("Bác sĩ")]
            BACSI = 40402,
            [Description("BV bán")]
            BVBAN = 40403,
            [Description("Bệnh nhân")]
            BENHNHAN = 40404,
            [Description("Khách vãng lai")]
            KHACH_VANG_LAI = 40405
        }
        public enum RefGenDrugCatID_1
        {
            [Description("Khác")]
            KHAC = 1,
            [Description("Hướng thần")]
            HUONGTHAN = 2,
            [Description("Gây nghiện")]
            GAYNGHIEN = 3
        }
        public enum V_DischargeType : long
        {
            [Description("Ra viện")]
            RA_VIEN = 1501,
            [Description("Xin ra viện")]
            XIN_VE = 1502,
            [Description("Trốn về")]
            BO_VE = 1503,
            //[Description("Đưa về")]
            //DUA_VE = 1504,
            [Description("Chuyển tuyến theo yêu cầu chuyên môn")]
            CHUYEN_TUYEN_CHUYEN_MON = 1505,
            [Description("Chuyển tuyến theo yêu cầu người bệnh")]
            CHUYEN_VIEN_NGUOI_BENH = 1506
        }

        public enum InPatientDeptStatus : long
        {
            //KMx: Chỉnh sửa lại cho đơn giản (09/09/2014 17:38).
            [Description("Nhập")]
            NHAP_KHOA_PHONG = 40501,
            [Description("Xuất")]
            XUAT_KHOA_PHONG = 40502

            //[Description("Đang chờ")]
            //PENDING = 40501 ,
            //[Description("Nhập khoa")]
            //NHAP_KHOA = 40502,
            //[Description("Chuyển khoa")]
            //CHUYEN_KHOA = 40503,
            //[Description("Xuất khoa")]
            //XUAT_KHOA = 40504
        }
        public enum RegistrationClosingStatus : long
        {
            [Description("Đã cân bằng")]
            BALANCED = 40601,
            [Description("Chưa cân bằng - Trả thiếu")]
            NOTBALANCED_CREDIT = 40602,
            [Description("Chưa trả tiền")]
            NOPAYMENT = 40603,
            [Description("Chưa cân bằng - Trả dư")]
            NOTBALANCED_DEBIT = 40604
        }

        public enum PatientFindBy : long
        {
            [Description("Ngoại trú")]
            NGOAITRU = 0,
            [Description("Nội trú")]
            NOITRU = 1,
            [Description("Cả hai")]
            CAHAI = 2,
            [Description("Điều trị ngoại trú")]
            DTNGOAITRU = 3,
            [Description("Khám sức khỏe")]
            KSK = 4
        }

        public enum DischargeCondition : long
        {
            [Description("Khỏi")]
            KHOI = 40701,
            [Description("Đỡ, giảm")]
            DO_GIAM = 40702,
            [Description("Không thay đổi")]
            KHONG_THAY_DOI = 40703,
            [Description("Nặng hơn")]
            NANG_HON = 40704,
            [Description("Tử vong")]
            TU_VONG = 40705,
            [Description("Tiên lượng năng xin về")]
            TIEN_LUONG_NANG = 40706,
            [Description("Chưa xác định")]
            UN_KNOWN = 40707,
        }

        public enum DeadReason : long
        {
            [Description("Tử vong trên bàn phẫu thuật")]
            InSurgery = 74001,
            [Description("Tử vong sau khi phẫu thuật 24 giờ")]
            After24Hour = 74002
        }

        public enum CategoryOfDecease : long
        {
            [Description("Do bệnh")]
            DO_BENH = 1001,
            [Description("Do tai biến điều trị")]
            DO_TAI_BIEN_DIEU_TRI = 1002,
            [Description("Lý do khác")]
            LY_DO_KHAC = 1003,
            [Description("Chưa xác định")]
            CHUA_XAC_DINH = 1000
        }

        public enum V_AItemType : long
        {
            [Description("Loại dị ứng - dị ứng bởi thuốc")]
            DRUG = 22001,
            [Description("Loại dị ứng - dị ứng bởi họ thuốc")]
            DRUG_CLASS = 22002,
            [Description("Loại dị ứng - dị ứng bởi thứ khác như thực phẩm, phấn hoa…")]
            LY_DO_KHAC = 22003
        }
        public enum V_PharmacyOutRepType : long
        {
            ALL = 40801,
            TUNG_NGUOI = 40802
        }

        public enum V_PaymentReason : long
        {
            [Description("Thu tiền ngoại trú")]
            THU_TIEN_NGOAI_TRU = 40900,
            [Description("Thu tiền tạm ứng nội trú")]
            TAM_UNG_NOI_TRU = 40901,
            [Description("Thu tiền giải phẫu")]
            THU_TIEN_GIAI_PHAU = 40902,
            [Description("Thu tiền thông tim can thiệp")]
            THU_TIEN_THONG_TIM = 40903
        }

        public enum V_GenericPaymentType : long
        {
            [LocalizedDescription("G0729_G1_ThuTien")]
            THU_TIEN = 61100,
            [LocalizedDescription("K3575_G1_DoiBienLai")]
            DOI_BIEN_LAI = 61101,
        }
        public enum ServiceItemType : long
        {
            [Description("KCB")]
            CHI_TIET_KCB = 50000,
            [Description("CLS")]
            CHI_TIET_CLS = 50001,
            [Description("THUOC")]
            CHI_TIET_THUOC = 50002,
            [Description("BILLING")]
            BILLING = 50003
        }

        public enum StatusForm : long
        {
            [Description("Tạo Mới")]
            TAOMOI = 0,

            [Description("Hiệu Chỉnh")]
            HIEUCHINH = 1,
            [Description("Xem")]
            XEM = 2
        }

        public enum ButtonClicked : long
        {
            [Description("Tạo Mới")]
            TAOMOI = 0,

            [Description("Hiệu Chỉnh")]
            HIEUCHINH = 1,

            [Description("Xóa")]
            XOA = 2,

            [Description("Bỏ Qua")]
            BOQUA = 3,

            [Description("Lưu")]
            LUU = 4,

            [Description("In")]
            IN = 5
        }

        public enum V_ByOutPrice : long
        {
            [Description("Giá Thông Thường")]
            GIATHONGTHUONG = 51000,

            [Description("Giá Vốn")]
            GIAVON = 51001,

            [Description("Giá Khác")]
            KHAC = 51002
        }

        public enum V_ByOutPriceMedDept : long
        {
            [Description("Giá Thông Thường")]
            GIATHONGTHUONG = 52000,

            [Description("Giá Vốn")]
            GIAVON = 52001,

            [Description("Giá Khác")]
            KHAC = 52002
        }
        public enum V_TradingPlaces : long
        {
            [Description("Quầy đăng ký")]
            DANG_KY = 53000,
            [Description("Nhà Thuốc")]
            NHA_THUOC = 53001,
            [Description("Quầy xác nhận")]
            QUAY_XAC_NHAN = 53002
        }
        public enum V_ServicePrice
        {
            None = 0,
            Changeable = 1

        }
        public enum DocTypeRequired_Status
        {
            CHUA_HOAN_THANH = 60550,
            DA_HOAN_THANH = 60551
        }

        public enum V_DocTypeRequired
        {
            CD_XUAT_KHOA = 60500,
            DN_CHUYEN_KHOA = 60501
        }

        public enum V_CashAdvanceType : long
        {
            [Description("de nghi tam ung")]
            DE_NGHI_TAM_UNG = 54000,

            [Description("de nghi thanh toan")]
            DE_NGHI_THANH_TOAN = 54001
        }

        public enum V_DiagnosisType : long
        {
            [Description("CHAN DOAN THONG THUONG")]
            DIAGNOSIS_NORMAL = 55000,
            [Description("CHAN DOAN NHAP VIEN")]
            DIAGNOSIS_IN = 55001,
            [Description("CHAN DOAN XUAT VIEN")]
            DIAGNOSIS_OUTHOS = 55002,
            [Description("CHAN DOAN XUAT KHOA")]
            DIAGNOSIS_OUTDEPT = 55003,
            [Description("CHẨN ĐOÁN HÀNG NGÀY")]
            DIAGNOSIS_DAILY = 55004,
            [Description("CHẨN ĐOÁN NHẬP KHOA")]
            DIAGNOSIS_INDEPT = 55005,
            [Description("Kết quả Thủ thuật")]
            Diagnosis_SmallProcedure = 55006,
        }
        //▼====: #008
        public enum StaffPositions_Enum : int
        {
            [Description("Giám Đốc")]
            GIAM_DOC = 84901,
            [Description("Phó Giám Đốc Hành Chánh")]
            PGD_HANH_CHANH = 84902,
            [Description("Phó Giám Đốc Kỹ Thuật")]
            PGD_KY_THUAT = 84903,
            [Description("Phó Giám Đốc")]
            PHO_GIAM_DOC = 84904,
            [Description("Trưởng nhà thuốc")]
            TRUONG_NHA_THUOC = 84910,
            [Description("Thủ kho nhà thuốc")]
            THU_KHO_NHA_THUOC = 84911,
            [Description("Thống kê dược nhà thuốc")]
            THONG_KE_DUOC_NHA_THUOC = 84912,
            [Description("Trưởng khoa dược")]
            TRUONG_KHOA_DUOC = 84920,
            [Description("Thủ kho thuốc - khoa dược")]
            THU_KHO_THUOC_KHOA_dUOC = 84921,
            [Description("Thủ kho y cụ")]
            THU_KHO_Y_CU = 84922,
            [Description("Thủ kho hóa chất")]
            THU_KHO_HOA_CHAT = 84923,
            [Description("Kế toán kho - khoa dược")]
            KE_TOAN_KHO_KHOA_DUOC = 84924,
            [Description("Thống kê dược - Thuốc")]
            THONG_KE_DUOC_THUOC = 84925,
            [Description("Thống kê dược - Y Cụ")]
            THONG_KE_DUOC_YCU = 84926,
            [Description("Thống kê dược - Hóa Chất")]
            THONG_KE_DUOC_HOACHAT = 84927,
            [Description("Trưởng khoa vật tư")]
            TRUONG_KHOA_VATTU = 84928,
            [Description("Thống kê khoa vật tư")]
            THONG_KE_VATTU = 84929,
            [Description("Điều dưỡng trưởng khoa")]
            DIEU_DUONG_TRUONG_KHOA = 84930,
            [Description("Trưởng khoa xét nghiệm")]
            TRUONG_KHOA_XET_NGHIEM = 84931,
            [Description("Kế toán trưởng")]
            KE_TOAN_TRUONG = 84940,
            [Description("TP.Kế hoạch")]
            TP_KE_HOACH = 84950
        }
        //▲====: #008

        public enum V_PrescriptionIssuedCase : long
        {
            [Description("Toa thuốc được cập nhật từ toa đã bán rồi")]
            UPDATE_FROM_PRESCRIPT_WAS_SOLD = 2305
        }

        public enum V_Form02Type : long
        {
            [Description("Mẫu 02 tất cả khoa")]
            ALL_DEPT = 60001,
            [Description("Mẫu 02 từng khoa")]
            ONE_DEPT = 60002
        }

        public enum PriceListType : int
        {
            [Description("Bảng giá thuốc")]
            BANG_GIA_THUOC = 1,
            [Description("Bảng giá dịch vụ khám chữa bệnh")]
            BANG_GIA_DV = 2,
            [Description("Bảng giá cận lâm sàng")]
            BANG_GIA_PCL = 3
        }

        // Hpt 11/11/2015: enum cho loại giá dịch vụ
        public enum V_NewPriceType : int
        {
            [Description("Không có giá")]
            Unknown_PriceType = 60430,
            [Description("Có giá cố định")]
            Fixed_PriceType = 60410,
            [Description("Có giá thay đổi")]
            Updatable_PriceType = 60420
        }

        public enum PopupModifyPrice_Type : int
        {
            INSERT_DICHVU = 60450,
            INSERT_PCL_HINHANH = 60451,
            INSERT_PCL_XETNGHIEM = 60452,
            UPDATE_DV = 60453,
            UPDATE_PCL = 60454,
            INSERT_GIUONG = 60455,
            INSERT_MAU = 60456,
            INSERT_PHAUTHUAT_THUTHUAT = 60457
        }
        public enum V_SupplierType : long
        {
            [Description("Cung cấp thiết bị y tế")]
            CUNGCAP_THIETBI_YTE = 7200,
            [Description("Cung cấp thiết bị văn phòng")]
            CUNGCAP_THIETBI_VANPHONG = 7201,
            [Description("Khác")]
            KHAC = 7202
        }

        public enum V_StockTakeType : long
        {
            [Description("Kiểm Kê Kết Chuyển")]
            KIEMKE_KETCHUYEN = 60101,
            [Description("Kiểm Kê Bổ Sung")]
            KIEMKE_BOSUNG = 60102
        }

        public enum V_EchoCardio_2D_Situs
        {
            [Description("EchoCardio 2D Situs Solitus")]
            V_Solitus = 29000,
            [Description("EchoCardio 2D Situs Inversus")]
            V_Inversus = 29001,
            [Description("EchoCardio 2D Situs Ambiguous")]
            V_Ambiguous = 29002
        }

        public enum AppServiceDetailPrintType
        {
            [Description("Không in")]
            None = 0,
            [Description("In giấy hẹn thông thường")]
            NormalApp = 1,
            [Description("In giấy hẹn BHYT ngoại trú")]
            HIApp = 2,
            [Description("In giấy hẹn BHYT nội trú")]
            HIApp_InPt = 3,
            [Description("In giấy hẹn BHYT theo mẫu của Bộ Y Tế")]
            HIApp_New = 4
        }

        public enum V_RecordState
        {
            [Description("Không có gì thay đổi")]
            UNCHANGE = 60801,
            [Description("Thêm dòng")]
            ADD = 60802,
            [Description("Cập nhật dòng")]
            UPDATE = 60803,
            [Description("Xóa dòng")]
            DELETE = 60804
        }
        public enum V_GenPaymtReasonTK
        {
            [Description("Viện phí mổ")]
            VIEN_PHI_MO = 61001,
            [Description("Viện phí mạch vành")]
            VIEN_PHI_MACH_VANH = 61002,
            [Description("Viện phí khoa A")]
            VIEN_PHI_KHOA_A = 61003,
            [Description("Viện phí khoa B")]
            VIEN_PHI_KHOA_B = 61004,
            [Description("Khoản thu khác")]
            THU_KHAC = 61005,
            [Description("Chi phí điều trị")]
            CHI_PHI_DIEU_TRI = 61006,
            [Description("Viện phí trại")]
            VIEN_PHI_TRAI = 61007,
            [Description("Chi phí Khám chữa bệnh")]
            CHI_PHI_KCB = 61008,
            [Description("Viện phí chụp mạch vành")]
            VP_CHUP_MACH_VANH = 61009,
            [Description("Đóng thông liên nhĩ")]
            DONG_THONG_LIEN_NHI = 61010,
            [Description("Đóng thông liên thất")]
            DONG_THONG_LIEN_THAT = 61011,
            [Description("Chi phí đặt máy tạo nhịp")]
            CP_MAY_TAO_NHIP = 61012

            // HPT: Chờ viện tim bổ sung rồi thêm vô
        }

        public enum V_GetReportMethod : long
        {
            [Description("Xem report")]
            VIEW_REPORT = 61201,
            [Description("Xuất excel")]
            EXPORT_EXCEL = 61202
        }

        public enum V_HIReportType : long
        {
            [Description("Xem báo cáo theo ngày")]
            DATE = 61501,
            [Description("Xem báo cáo theo tháng")]
            MONTH = 61502,
            [Description("Xem báo cáo theo quý")]
            QUARTER = 61503,
            [Description("Xem báo cáo theo năm")]
            YEAR = 61504,
            [Description("Báo cáo theo thời gian")]
            TIME = 61505,
            [Description("Báo cáo theo mã đợt điều trị")]
            REGID = 61506
        }
        public enum PCLRequestCreatedFrom
        {
            [Description("Từ phiếu chỉ định")]
            FROM_PCLREQUEST = 0,
            [Description("Từ tạo bill")]
            FROM_BILLINGINV = 1
        }

        public enum V_ReqPaymentStatus
        {
            [Description("Chưa tạo bill")]
            NO_BILL = 61600,
            [Description("Bill mới")]
            NEW_BILL = 61601,
            [Description("Đã trả tiền")]
            PAID_ALL = 61602,
            [Description("Đã hoàn tiền")]
            REFUND_ALL = 61603
        }
        public enum LoadPCLRequestType
        {
            [Description("Không load PCLRequest vào Bill")]
            NO_LOAD_PCL = 0,
            [Description("Load phiếu đã thực hiện")]
            LOAD_COMPLETED_PCL = 1,
            [Description("Load tất cả phiếu chỉ định")]
            LOAD_ALL = 2
        }

        public enum V_RefundPaymentReasonInPt : long
        {
            [Description("Tiền thừa đã trừ BHYT")]
            THUA_DA_TRU_BHYT = 61600,
            [Description("Nhận lại tiền viện phí mổ còn thừa")]
            THUA_VIEN_PHI_MO = 61601,
            [Description("Nhận lại tiền viện phí mạch vành còn thừa")]
            THUA_VP_MACH_VANH = 61602,
            [Description("Nhận lại chi phí đặt máy tạo nhịp còn thừa")]
            THUA_DAT_MAY_TAO_NHIP = 61603,
            [Description("Chi phí điều trị còn thừa")]
            THUA_CP_DIEU_TRI = 61604,
            [Description("Nhận lại tiền viện phí mổ")]
            NL_VIEN_PHI_MO = 61605,
            [Description("Nhận lại tiền viện phí mạch vành")]
            NL_VP_MACH_VANH = 61606,
            [Description("Nhận lại chi phí đặt máy tạo nhịp")]
            NL_DAT_MAY_TAO_NHIP = 61607,
        }

        public enum RequireDiagnosisForPCLReq : int
        {
            [Description("Yêu cầu CĐ cho PCĐ CLS ngoại trú")]
            DIAG_FOR_PCLREQ_OUTPATIENT = 1,
            [Description("Yêu cầu CĐ cho PCĐ CLS nội trú")]
            DIAG_FOR_PCLREQ_INPATIENT = 2,
            [Description("Yêu cầu CĐ cho PCĐ CLS cả nội và ngoại trú")]
            DIAG_FOR_PCLREQ = 3,
        }
        //==== 20161213 CMN Begin: Add lookup for FetalEchocardiography
        public enum V_EchographyPosture : long
        {
            [Description("Ngả trước")]
            Front = 1,
            [Description("Ngả sau")]
            After = 2,
            [Description("Trung gian")]
            Middle = 3
        }
        public enum V_MomMedHis : long
        {
            [Description("0000")]
            Zero = 0,
            [Description("0001")]
            Once = 1,
            [Description("0002")]
            Twice = 2,
            [Description("0003")]
            Thrice = 3,
            [Description("0004")]
            Four = 4
        }
        //==== #001
        public enum V_InfusionProcessType : long
        {
            [Description("Liên tục")]
            Continuous = 62201,
            [Description("Ngắt quãng")]
            Interrupted = 62202
        }
        public enum V_InfusionType : long
        {
            [Description("Truyền TM")]
            Infusion = 62301,
            [Description("Tiêm TM")]
            Inject = 62302,
            [Description("Tiêm TM chậm")]
            SlowInject = 62303,
            [Description("Bolus TM")]
            Bolus = 62304
        }
        public enum V_TimeIntervalUnit : long
        {
            [Description("Dung tích")]
            Volume = 62401,
            [Description("Thời gian")]
            Time = 62402
        }
        public enum V_LevelCare : long
        {
            [Description("Cấp 1")]
            One = 1,
            [Description("Cấp 2")]
            Two = 2,
            [Description("Cấp 3")]
            Three = 3
        }
        //==== #001

        public enum V_TransferFormType : int
        {
            [Description("Giấy chuyển đi")]
            CHUYEN_Di = 1,
            [Description("Giấy chuyển đến")]
            CHUYEN_DEN = 2,
            [Description("Giấy chuyển đi làm cận lâm sàng")]
            CHUYEN_DI_CLS = 3
        }

        public enum CriterialTypes : int
        {
            [Description("Mã bệnh nhân")]
            MA_BN = 1,
            [Description("Tên bệnh nhân")]
            TEN_BN = 2,
            [Description("Mã chuyển tuyến")]
            MA_CHUYEN_TUYEN = 3,
            [Description("BV chuyển")]
            BV_CHUYEN = 4,
            [Description("Khoa chuyển")]
            KHOA_CHUYEN = 5
        }
        /*▼====: #002*/
        public enum V_HeartSurgicalType
        {
            [Description("Mổ tim kín")]
            Closed = 64001,
            [Description("Mổ tim hở")]
            Opened = 64002
        }
        public enum V_DiagnosticType
        {
            [Description("Bệnh bẩm sinh")]
            Congenital = 65001,
            [Description("Bệnh van tim")]
            Valve = 65002,
            [Description("Bệnh mạch vành tim")]
            Coronary = 65003
        }
        public enum V_TreatmentMethod
        {
            [Description("Điều trị nội")]
            InPtExam = 66001,
            [Description("Thông tim")]
            Cardiac = 66002,
            [Description("Phẫu thuật")]
            Surgery = 66003
        }
        public enum V_ProcessStep
        {
            [Description("Chỉ định mổ")]
            Opened = 67001,
            [Description("Nhận hồ sơ")]
            Approved = 67002,
            [Description("Từ chối hồ sơ")]
            Rejected = 67003,
            [Description("Bắt đầu xét nghiệm")]
            BeginExam = 67004,
            [Description("Hoàn tất xét nghiệm")]
            CompleteExam = 67005,
            [Description("Nhập viện")]
            Admission = 67006,
            [Description("Đã mổ")]
            Surgery = 67007,
            [Description("Hoàn tất")]
            Completed = 67008
        }
        public enum V_ValveType
        {
            [Description("Van cơ học")]
            Mechanical = 68001,
            [Description("Van sinh học")]
            Biological = 68002
        }
        /*▲====: #002*/

        /*TMA 10/11/2017*/
        public enum V_CharityObjectType
        {
            [Description("Người nghèo không thể nộp được")]
            NGUOINGHEO = 79001,
            [Description("Người bệnh trong diện ưu đãi")]
            UUDAI = 79002,
            [Description("Người bệnh không có người nhận")]
            KONGUOINHAN = 79003,
            [Description("Không thu được vì các lý do khác")]
            KOTHUDUOC = 79004
        }
        /*TMA 10/11/2017*/

        public enum V_ProductScope : long
        {
            InHIScope = 82001,
            NotInHIScope = 82002
        }

        public enum V_TreatmentPeriodic : long
        {
            TreatmentPeriodic1 = 82300,
            TreatmentPeriodic2 = 82301,
            TreatmentPeriodic3 = 82302
        }

        public enum V_ReportStatus : long
        {
            NotReported = 82400,
            Pending = 82401,
            Completed = 82402,
            Errored = 82404
        }

        public enum V_CatDrugType : long
        {
            All = 0,
            Shared = 82201,
            DrugDept = 82202,
            Pharmacy = 82203
        }

        public enum HIType : int
        {
            NoHI = 0,
            HI = 1,
            All = 2,
        }

        public enum V_TreatmentType : long
        {
            [Description("Cấp toa cho về")]
            IssuedPrescription = 82500,
            [Description("Nhập viện")]
            InPtAdmission = 82501,
            [Description("Thực hiện CLS")]
            PCLExam = 82502,
            [Description("Điều trị ngoại trú")]
            OutPtTreatment = 82506,
            [Description("Chuyển tuyến")]
            Transfer = 82507,
            [Description("Thực hiện thủ thuật")]
            SmallProcedure = 82508
        }

        public enum V_WarningLevel : long
        {
            [Description("Bình thường")]
            Normal = 82800,
            [Description("Cảnh báo")]
            Warning = 82801,
            [Description("Ngăn chặn")]
            Block = 82802
        }

        public enum V_InteractionSeverityLevel : long
        {
            Level0 = 82900,
            Level1 = 82901,
            Level2 = 82902,
            Level3 = 82903,
            Level4 = 82904,
            Level5 = 82905,
        }

        public enum V_HosClientType : long
        {
            [Description("Công ty")]
            Company = 83000,
            [Description("Công ty")]
            HealthyOrganization = 83001
        }

        public enum V_Ekip : long
        {
            EkipSo1 = 82701,
            EkipSo2 = 82702,
            EkipSo3 = 82703
        }

        public enum V_EkipIndex : long
        {
            DauTien = 83100,
            CungEkip = 83101,
            KhacEkip = 83102
        }

        public enum DeptID : long
        {
            KHOA_DUOC = 23,
            CAP_CUU = 30,
            //▼==== #015 ID trên không giống như SQL nhưng không rõ đang dùng như thế nào, nên giữ lại
            CapCuu = 15,
            KhoaKham = 19,
            //▲==== #015
            //▼==== #017
            GayMeHoiSuc = 25,
            KhoaNoi = 35
            //▲==== #017
        }
        public enum HITTypeID : long
        {
            DVKT = 9
        }
        public enum TypeChangePrice : long
        {
            DRUGDEPT = 1,
            PHARMACY = 2
        }
        public enum SupplierDrugDeptPharmOthers : long
        {
            DRUGDEPT = 1,
            PHARMACY = 2,
            ALL = 3
        }

        public enum V_AdmissionType : long
        {
            [Description("Cấp Cứu")]
            Emergency = 1100,
            [Description("Trực tiếp vào Cấp Cứu")]
            EmergencyIncome = 1101,
            [Description("Từ Phòng Khám vào Cấp Cứu")]
            FromOutPtDept = 1102,
            [Description("Trực tiếp vào Khoa")]
            DeptIncome = 1103,
            [Description("Chuyển viện")]
            FromOutHos = 1104
        }
        public enum V_SpecialistType : long
        {
            Tim = 83601,
            Noi = 83602,
            Ngoai = 83603,
            Nhi = 83604,
            DaLieu = 83605,
            TMH = 83606,
            RHM = 83607,
            San = 83608,
            Mat = 83609
        }
        public enum V_OutwardTemplateType : long
        {
            OutwardTemplate = 84400,
            RequestTemplate = 84401
        }
        public enum V_AntibioticTreatmentType : long
        {
            InfectionCase = 84500,
            Instruction = 84501
        }
        public enum V_InstructionOrdinalType : long
        {
            Thuong = 84600,
            KhangSinh = 84601,
            GayNghien = 84602,
            KhangViemCorticoid = 84603,
            HuongThan = 84604
        }

        public enum V_RangeOfWareHouses : long
        {
            WholeHospital = 84700,
            DrugDept = 84701,
            StoreDept = 84702,
            BaseNumStoreDept = 84703
        }
        public enum MedicalServiceTypeID : long
        {
            KHAMBENH = 1
        }
        public enum HospitalCode : long
        {
            VIEN_TIM = 79443,
            THANH_VU_1 = 95076,
            THANH_VU_2 = 95078
        }
        //▼===== #009
        public enum V_DiscountTypeCount : long
        {
            [Description("Tất cả")]
            All = 851001,
            [Description("Chỉ tính BN trả")]
            PriceDifference = 851002,
            [Description("Chỉ tình cùng chi trả")]
            AmountCoPay = 851003
        }
        //▲===== #009

        //--▼-- 28/12/2020 DatTB Thêm biến Lookup loại kho V_GroupTypes
        public enum V_GroupTypes : long
        {
            [Description("Kho được khấu trừ thuế GTGT")]
            TINH_GTGT = 85200,
            [Description("Kho không được khấu trừ thuế GTGT")]
            KHONG_TINH_GTGT = 85201
        }
        //--▲-- 28/12/2020 DatTB

        public enum V_FastReportType : long
        {
            [Description("Biên lai thu tiền, hòa ứng")]
            Bien_Lai_Hoa_Ung = 85501,
            //[Description("Hóa đơn")]
            //Hoa_Don = 85502,
            [Description("Nhập dược, trả dược")]
            Nhap_Tra_Duoc = 85502,
            [Description("Xuất dược")]
            Xuat_Duoc = 85503
        }
        //▼====: #010
        public enum V_CheckMedicalFilesStatus : long
        {
            [Description("Chờ duyệt")]
            Tao_Moi = 85601,
            [Description("Trả hồ sơ")]
            Tra_Ho_So = 85602,
            [Description("Đã duyệt")]
            Da_Duyet = 85603,
            [Description("Chờ duyệt lại")]
            Cho_Duyet_Lai = 85604
        }
        //▲====: #010
        public enum V_BedType : long
        {
            [Description("Giường kế hoạch")]
            Ke_Hoach = 85701,
            [Description("Giường kê thêm")]
            Ke_Them = 85702,
            [Description("Giường tự chọn")]
            Tu_Chon = 85703,
            [Description("Giường khác")]
            Khac = 85704
        }
        public enum V_EatingType : long
        {
            [Description("Bình thường")]
            Binh_Thuong = 85901,
            [Description("Trên 50% bình thường")]
            Tren_50 = 85902,
            [Description("Dưới 50% bình thường")]
            Duoi_50 = 85903
        }
        public enum V_ExaminationType : long
        {
            [Description("Không")]
            Binh_Thuong = 86001,
            [Description("Nhẹ/vừa")]
            Nhe_Vua = 86002,
            [Description("Nặng")]
            Nang = 86003
        }
        public enum V_SGAType : long
        {
            [Description("SGA-A (Bình thường): Sụt cân <5%, ăn uống vá khám bình thường")]
            SGA_A = 86101,
            [Description("SGA-B (Suy dinh dưỡng nhẹ/vừa): Sụt cân 5-10%, ăn > 50%, teo mỡ và cơ mức độ nhẹ hay vừa")]
            SGA_B = 86102,
            [Description("SGA_C (Suy dinh dưỡng nặng): Sụt cân >10%, ăn <50%, khám teo mỡ và cơ nặng hay có phù chi báng bụng(trừ bệnh gan, thận)")]
            SGA_C = 86103
        }
        public enum V_NutritionalRequire : long
        {
            [Description("1,45-1,50m &#x09; 1500-1600 kcal &#x09; 55-58g đạm")]
            NutritionalRequire145 = 86201,
            [Description("1,51-1,55m &#x09; 1600-1700 kcal &#x09;&#x09; 59-62g đạm")]
            NutritionalRequire151 = 86202,
            [Description("1,56-1,60m &#x09; 1700-1800 kcal &#x09;&#x09; 63-66g đạm")]
            NutritionalRequire156 = 86203,
            [Description("1,61-1,65m &#x09; 1800-1900 kcal &#x09;&#x09; 67-70g đạm")]
            NutritionalRequire161 = 86204,
            [Description("1,66-1,70m &#x09; 1900-2000 kcal &#x09;&#x09; 71-74g đạm")]
            NutritionalRequire166 = 86205,
            [Description("1,71-1,75m &#x09; 2000-2100 kcal &#x09;&#x09; 75-78g đạm")]
            NutritionalRequire171 = 86206,
            [Description(">1,75m &#x09; 2100-2200 kcal &#x09;&#x09; 79-82g đạm")]
            NutritionalRequire175 = 86207,
            [Description("Khác")]
            NutritionalRequireOther = 86208,
        }
        public enum V_NutritionalMethods : long
        {
            [Description("Đường miệng")]
            Duong_Mieng = 86301,
            [Description("Qua ống thông")]
            Qua_Ong_Thong = 86302,
            [Description("Qua tĩnh mạch")]
            Qua_Tinh_Mach = 86303
        }
        public enum V_AdmissionCriteriaType : long
        {
            [Description("IA")]
            Loai_A = 86601,
            [Description("IB")]
            Loai_B = 86602
        }
		
        public enum LoadBillType : int
        {
            [Description("Bình thường")]
            BINHTHUONG = 0,
            [Description("Xuất khoa")]
            XUATKHOA = 1,
            [Description("Xuất viện")]
			XUATVIEN = 2
		}
		
        public enum V_ReportForm : long
        {
            Mau_0_Hinh = 86701,
            Mau_2_Hinh = 86702,
            Mau_3_Hinh = 86703,
            Mau_4_Hinh = 86704,
            Mau_6_Hinh = 86705,
            Mau_1_Hinh = 86706,
            Mau_Realtime_PCR = 86707,
            Mau_Test_Nhanh = 86708,
            Mau_Xet_Nghiem = 86709,
            Mau_Helicobacter_Pylori = 86710,
            Mau_Sieu_Am_Tim = 86711,
            Mau_Sieu_Am_San_4D = 86712,
            Mau_Dien_Tim = 86713,
            Mau_Dien_Nao = 86714,
            Mau_ABI = 86715,
            Mau_Dien_Tim_Gang_Suc = 86716,
            Mau_CLVT_Hai_Ham = 86717,
            //▼====: #016
            Mau_Sieu_Am_San_4D_New = 86718,
            Mau_6_Hinh_2_New = 86719,
            Mau_6_Hinh_1_New = 86720,
            Mau_Sieu_Am_Tim_New = 86721,
            Mau_Noi_Soi_9_Hinh = 86722,

            //▲====: #016
            //▼==== #027
            Mau_0_Hinh_V2 = 86724,
            Mau_Xet_Nghiem_V2 = 86725,
            //▲==== #025
            //▼==== #026
            Mau_0_Hinh_XN = 86726,
            Mau_1_Hinh_XN = 86727,
            //▲==== #027
        }

        public enum V_SymptomType : long
        {
            Bat_Buoc = 86900,
            Khong_BatBuoc = 86901
        }

        public enum V_AgeUnit : long
        {
            Nam = 87200,
            Thang = 87201,
            Ngay = 87202
        }

        public enum V_ObjectType : long
        {
            BenhNhan = 874001,
            DoiTac = 874002,
            NhanVien = 874003
        }
        public enum V_MedicalServiceGroupType : long
        {
            Kham_Benh = 87300,
            Kiosk = 87301
        }
        public enum V_MedicalServiceParentGroup : long
        {
            Goi_Ung_Thu = 87500,
            Goi_Theo_Tuoi = 87501,
            Goi_Benh_Ly = 87502,
            Goi_Gen = 87503,
        }

        //▼==== #012
        public enum PCLStatus
        {
            DaThucHien = 0,
            ChuaThucHien = 1
        }
        //▲==== #012

        //▼==== #013
        public enum PatientClassification
        {
            BN_KHONG_BHYT = 1,
            BN_CO_BHYT = 2,
            DICH_VU = 3,
            MIEN_PHI = 4,
            NGHIEN_CUU = 5,
            NGHIEN_CUU_BRAMHS = 6,
            NHAN_VIEN = 7,
            TRA_SAU = 8,
            KHAM_SUC_KHOE = 9
        }
        //▲==== #013
        public enum V_EInvoiceStatus : long
        {
            Dang_Ky_Hoan_Tat = 87900,
            DTNT_Da_Ket_Thuc = 87901,
            DTNT_Chua_Ket_Thuc = 87902
        }

        public enum V_MedicalFileType: long
        {
            Noi_Tru = 881001,
            DT_Ngoai_Tru = 881002
        }
        public enum V_TransactionStatus : long
        {
            Thanh_Cong = 88700,
            Cho_Ky = 88701,
            Tu_Choi = 88702,
            Het_Gio = 88703,
            Khac = 88704,
        }
        //▼==== #017
        public enum V_PatientClass : long
        {
            Thong_Thuong = 89000,
            Bac = 89001,
            Vang = 89002,
            Kim_Cuong = 89003
        }
        //▲==== #017
        //▼====: #018
        public enum V_PrescriptionIssuedType : long
        {
            Ngoai_Tru = 88900,
            Gay_Nghien = 88901,
            Huong_Than = 88902,
            Dong_Y = 88903
        }
        //▲====: #018
        //▼====: #020
        public enum V_OvertimeWorkingWeekStatus : long
        {
            Luu = 89200,
            Gui_Phong_TC = 89201,
            Gui_Phong_KHTH = 89202,
            Xac_Nhan = 89203,
            Khoa = 89204
        }
        //▲====: #020
        //▼====: #022
        public enum RegTypeID
        {
            NGOAI_TRU = 1,
            NOI_TRU = 3,
        }
        //▲====: #022
        //▼====: #024
        public enum V_ReportStatusPrescription : long
        {
            NotReported = 892000,
            Reported = 892001,
            Deleted = 892002
        }
        //▲====: #024
        //▼====: #025
        public enum V_SurgicalBirth : long
        {
            Sinh_Con_Khong_PT = 89300,
            Sinh_Con_PT = 89301
        }
        public enum V_BirthUnder32 : long
        {
            Sinh_Con_Khong_Duoi_32_Tuan = 89400,
            Sinh_Con_Duoi_32_Tuan = 89401
        }
        //▲====: #025

        public enum V_ExamTypeSubCategoryID: long
        {
            XRAY = 1,
            ULTRASOUND = 2,
            SCAN = 3,
            MRI = 4,
            TEMPLATE_1 = 7,
            TEMPLATE_2 = 8,
            ECG = 9,
            ANGIOGRAPHY = 10, 
            CLS_HA_NGOAIVIEN = 11,
            TDCN = 12,
            NOISOI = 13,
            GIAIPHAUBENH = 14,
            XETNGHIEM = 15,
            LIEUTRINH = 16
        }
        public enum V_ReasonHospitalStay: long
        {
            Khac = 89515
        }
        public enum V_ReasonAdmission : long
        {
            Khac = 89618
        }
        public enum V_ObjectMedicalExamination: long
        {
            Dung_Tuyen_1 = 89800,
            Dung_Tuyen_1_1 = 89801,
            Dung_Tuyen_1_2 = 89802,
            Dung_Tuyen_1_3 = 89803,
            Dung_Tuyen_1_4 = 89804,
            Dung_Tuyen_1_5 = 89805,
            Dung_Tuyen_1_6 = 89806,
            Dung_Tuyen_1_7 = 89807,
            Dung_Tuyen_1_8 = 89808,
            Dung_Tuyen_1_9 = 89809,
            Dung_Tuyen_1_10 = 89810,
            Cap_Cuu_2 = 89811,
            Trai_Tuyen_3 = 89812,
            Trai_Tuyen_3_1 = 89813,
            Trai_Tuyen_3_2 = 89814,
            Trai_Tuyen_3_3 = 89815,
            Trai_Tuyen_3_4 = 89816,
            Trai_Tuyen_3_5 = 89817,
            Trai_Tuyen_3_6 = 89818,
            Trai_Tuyen_3_7 = 89819,
            Linh_Thuoc_Theo_Giay_Hen_7 = 89820,
            Linh_Thuoc_Theo_Giay_Hen_7_1 = 89821,
            Linh_Thuoc_Theo_Giay_Hen_7_2 = 89822,
            Linh_Thuoc_Theo_Giay_Hen_7_3 = 89823,
            Linh_Thuoc_Theo_Giay_Hen_7_4 = 89824,
            Thu_Hoi_De_Nghi_Thanh_Toan_8 = 89825,
            Kham_Chua_Benh_Dich_Vu_9 = 89826,
        }
    }
}
