using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

/*
 * 20180906 #001 TBL:   Xuat excel mau 79a-80a theo dinh dang 3360
 * 20181005 #002 TNHX:  [BM0000034] Add Report PhieuChiDinh
 * 20181103 #003 TNHX:  [BM0005214] Add Report PhieuNhanThuocSummary
 * 20190608 #004 TNHX:  [BM0006715] Add function to export excel for Accountant
 * 20200319 #005 TTM:   BM 0027022: [79A] Bổ sung tích chọn xuất Excel toàn bộ dữ liệu, đã xác nhận, chưa xác nhận. 
 * 20200610 #006 TNHX:  Thêm báo cáo toa thuoc KD + DLS, báo cáo thuốc, y cụ dỡ dang cho kế toán
 * 20200914 #007 TNHX:  Thêm _StoreType
 * 20210910 #008 QTD: Thêm báo cáo kho tổng hợp
 * 20211004 #009 TNHX: Thêm báo cao 80a cho kế toán doanh thu
 * 20211011 #010 TNHX: Thêm báo cao bn chờ kết quả xét nghiệm
 * 20220305 #011 QTD:  Thêm báo cáo đo DHST
 * 20220330 #012 DatTB:  Thêm báo cáo quản lý kiểm duyệt hồ sơ KHTH
 * 20220407 #013 DatTB:  Thêm Báo cáo kiểm tra lịch sử KCB DLS
 * 20220409 #014 DatTB:  Thêm Báo cáo thông tin danh mục thuốc DLS
 * 20220516 #015 DatTB: Báo cáo tình hình thực hiện CLS - Đã thực hiện
 * 20220517 #016 DatTB: Báo cáo máu sắp hết hạn dùng
 * 20220517 #017 DatTB: Thêm trường chọn CLS Đã/Chưa thực hiện
 * 20220517 #018 DatTB: Báo cáo danh sách dịch vụ kỹ thuật xét nghiệm
 * 20220518 #019 DatTB: Báo cáo lượt xét nghiệm
 * 20220523 #020 DatTB: Báo cáo Doanh thu theo Khoa
 * 20220524 #021 DatTB: Thêm ShowDischarge chọn đã/chưa xuất viện
 * 20220525 #022 DatTB: Thêm Báo Cáo Hoãn/Miễn Tạm Ứng Nội Trú
 * 20220527 #023 DatTB: Thêm Giấy Miễn Tạm Ứng Nội Trú
 * 20220527 #024 DatTB: Thêm Giấy Hoãn Tạm Ứng Nội Trú
 * 20220528 #025 BLQ: Báo cáo tổng hợp theo dõi tiền viện phí
 * 20220608 #026 DatTB: Thêm in report hướng dẫn sử dụng theo toa thuốc
 * 20220624 #027 QTD:   BC BN trễ hẹn điều trị ngoại trú
 * 20220709 #028 BLQ: Báo cáo thang điểm cảnh báo sớm
 * 20220728 #029 DatTB:
 * + Báo cáo giao ban Khoa khám bệnh
 * + Báo cáo chỉ định cận lâm sàng – khoa khám
 * 20220807 #030 DatTB: Báo cáo thống kê số lượng hồ sơ điều trị ngoại trú
 * + Tạo màn hình, thêm các trường lọc dữ liệu.
 * + Thêm trường phòng khám sau khi chọn khoa.
 * + Validate các trường lọc.
 * + Thêm điều kiện để lấy khoa theo list DeptID.
 * 20220808 #031 QTD: Báo cáo DS BN điều trị ngoại trú
 * 20220823 #032 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
 * 20220901 #033 BLQ: Issue:2174 Chỉnh lại mẫu hình ảnh theo cách mới
 * 20220910 #034 DatTB: Thêm phiếu công khai thuốc KK
 * 20220927 #035 DatTB: Báo cáo danh sách bệnh nhân ĐTNT
 * 20221013 #036 BLQ: Thêm mẫu giấy đề nghị mở thẻ, Báo cáo Phát hành thẻ Khám chữa bệnh, BÁO CÁO THỐNG KÊ SỐ LƯỢNG THẺ KHÁM CHỮA BỆNH
 * 20221117 #037 DatTB: - IssueID: 2241 | Thêm trường chọn mẫu cho các sổ khám bệnh.
 * 20221124 #038 BLQ: Thêm báo cáo lịch làm việc ngoài giờ
 * 20221128 #039 DatTB: Thêm báo cáo thao tác người dùng
 * 20221201 #040 DatTB: Thêm báo cáo thống kê hồ sơ ĐTNT
 * 20230131 #041 QTD:   Thêm Report toa thuốc GN_HT nội trú
 * 20230109 #042 DatTB: Thêm phiếu tự khai và cam kết điều trị
 * 20220131 #043 BaoLQ: Thêm báo cáo phiếu theo dõi dịch truyền
 * 20230207 #044 DatTB: 
 * + Thêm báo cáo KSNK
 * + Thêm bộ lọc cho báo cáo KSNK
 * 20230209 #045 QTD:   Thêm Report xuất sử dụng thuốc cản quang
 * 20230224 #046 QTD:   Thêm BC hủy đẩy cổng nhà thuốc
 * 20220220 #047 BaoLQ: Thêm báo cáo hủy DTDT
 * 20230218 #048 DatTB: Thêm Báo cáo thời gian tư vấn cấp toa/chỉ định
 * 20230304 #049 DatTB:   Báo cáo xuất thuốc Khoa/Phòng
 * 20230009 #050 BaoLQ: Thêm Báo cáo DLS
 * 20230311 #051 DatTB: Thêm report phiếu kết quả xét nghiệm mới
 * 20230314 #052 DatTB: Tách Lookup mẫu report kết quả xét nghiệm sử dụng chung mẫu CDHA
* 20230504 #053 DatTB: Thêm report Kết quả tính tuổi động mạch
* 20230513 #054 DatTB: Thêm Báo cáo thời gian chờ tại bệnh viện
* 20230526 #055 DatTB: Thêm Báo cáo thống kê đơn thuốc điện tử
* 20230626 #056 QTD:   Thêm báo cáo danh sách BN gửi SMS
* 20230630 #057 QTD:   Thêm báo cáo tổng hợp dự trù theo khoa/phòng
*/
namespace DataEntities
{
    public enum ReportName : int
    {
        None = 0,
        [Description("Mẫu 20 ngoại trú")]
        TEMP20_NGOAITRU = 1,
        [Description("Mẫu 20 nội trú")]
        TEMP20_NOITRU = 2,
        [Description("Mẫu 21 ngoại trú")]
        TEMP21_NGOAITRU = 3,
        [Description("Mẫu 21 nội trú")]
        TEMP21_NOITRU = 4,
        [Description("Mẫu 25 chi tiết")]
        TEMP25a_CHITIET = 5,
        TEMP25a_TONGHOP = 6,
        [Description("Mẫu 26 chi tiết")]
        TEMP26a_CHITIET = 7,
        TEMP26a_TONGHOP = 8,
        TEMP38a = 9,
        [Description("Thống kê doanh thu")]
        THONGKEDOANHTHU = 10,

        PHARMACY_NHAPTHUOCTUNCC = 11,
        PHARMACY_PHIEUNHANTHUOC = 12,
        PHARMACY_TRATHUOCBH = 13,
        PHARMACY_PHIEUCHI = 14,
        PHARMACY_TRATHUOC = 15,
        PHARMACY_NHAPXUATTON = 16,
        PHARMACY_NHAPXUATTONTHEOTUNGTHUOC = 17,
        PHARMACY_THEKHO = 18,
        PHARMACY_THUOCHETHANDUNG = 19,
        PHARMACY_VISITORPHIEUTHU = 25,
        PHARMACY_ESTIMATTION = 26,


        PHARMACY_BCHANGNGAY_NOPTIEN = 27,
        PHARMACY_BCHANGNGAY_PHATTHUOC = 28,
        PHARMACY_PHIEUKIEMKE = 29,
        PHARMACY_PHIEUHUYHANG = 30,
        PHARMACY_NHAPTHUOCHANGTHANG = 31,
        PHARMACY_NHAPHANGTHANGTHEOSOPHIEU = 32,
        PHARMACY_XUATNOIBO = 33,
        PHARMACY_REQUESTDRUGPHARMACY = 34,
        PHARMACY_BANGKECHUNGTUTHANHTOAN = 35,
        PHARMACY_PHIEUDENGHITHANHTOAN = 36,
        PHARMACY_XUATNOIBOTHEONGUOIBAN = 37,
        PHARMACY_XUATNOIBOTHEOTENTHUOC = 38,
        PHARMACY_XUATTHUOCCHOBH = 39,
        PHARMACY_BANTHUOCTH = 40,
        PHARMACY_TRATHUOCTH = 41,
        PHARMACY_BAOCAOTONGHOPDOANHTHU = 42,
        PHARMACY_PHIEUDATHANG = 43,
        PHARMACY_BCKIEMKEVADUTRU = 44,
        PHARMACY_PHARMACYSUPPLIERTEMPLATE = 45,
        PHARMACY_DUTRUDUATRENHESOANTOAN = 46,
        PHARMACY_TONGHOPDUTRU_SOPHIEU = 47,
        PHARMACY_TONGHOPDUTRU_TENTHUOC = 48,
        PHARMACY_HESOANTOANBAN = 49,
        PHARMACY_THEODOICONGNO = 50,

        CONSULTATION_TOATHUOC = 20,
        CONSULTATION_REQUESTPCLFROM = 21,
        CONSULTATION_REQUESTPCL = 22,
        CONSULTATION_LABRESULTS = 23,
        CONSULTATION_PMR = 24,
        PCLDEPARTMENT_LABORATORY_RESULT = 51,

        //drugdept 

        DRUGDEPT_OUTINTERNAL = 53,
        DRUGDEPT_ESTIMATION = 54,
        DRUGDEPT_ORDER = 55,
        DRUGDEPT_INWARD_MEDFORCLINIC = 56,
        DRUGDEPT_INWARD_MEDDEPTSUPPLIER = 57,
        DRUGDEPT_BANGKECHUNGTUTHANHTOAN = 58,
        DRUGDEPT_PHIEUDENGHITHANHTOAN = 59,
        DRUGDEPT_OUTADDICTIVE = 60,
        //Khám Bệnh
        DiagnosisTreatmentByDoctorStaffID = 61,
        AllDiagnosisTreatmentGroupByDoctorStaffID = 62,

        //Khoa Dược
        RptBangGiaThuocKhoaDuoc_AutoCreate = 63,
        RptBangGiaThuocKhoaDuoc = 64,

        RptBangGiaYCuHoaChatKhoaDuoc = 65,
        RptBangGiaYCuHoaChatKhoaDuoc_AutoCreate = 66,

        RptDanhSachXuatThuocYCuHoaChatKhoaDuoc = 67,

        //Nhà Thuốc
        RptBangGiaThuocNhaThuoc = 68,
        RptBangGiaThuocNhaThuoc_AutoCreate = 69,
        RptDanhSachXuatThuocNhaThuoc = 70,
        //Phiếu YC CLS
        RptPatientPCLRequestDetailsByPatientPCLReqID = 71,

        //Kiểm Kê Khoa Dược
        RptDrugDeptStockTakes_ThuocYCuHoaChatKhoaDuoc = 72,
        DRUGDEPT_RETURN_MEDDEPT = 73,
        DRUGDEPT_REPORT_MEDDEPTINVOICE = 74,
        DRUGDEPT_REPORT_INOUTSTOCK_ADDICTIVE = 75,
        DRUGDEPT_REPORT_THEODOICONGNO = 76,
        DRUGDEPT_INWARD_NHAPCHIPHI = 77,
        //Kiểm Kê Khoa Dược
        DRUGDEPT_KIEMKE_KHOADUOC = 78,
        [Description("Báo cáo chi tiết viện phí")]
        TRANSACTION_VIENPHICHITIET = 79,
        [Description("Báo cáo chi tiết viện phí phòng khám")]
        TRANSACTION_VIENPHICHITIET_PK = 80,
        [Description("Tổng hợp doanh thu ngoại trú")]
        RptTongHopDoanhThu = 81,
        [Description("Tổng hợp doanh thu nội trú")]
        RptTongHopDoanhThuNoiTru = 82,
        TEMP38aNoiTru = 83,
        DRUGDEPT_HUYTHUOC = 84,
        DRUGDEPT_ESTIMATIONTHUKHO = 85,
        DRUGDEPT_ESTIMATIONKETOAN = 86,
        DRUGDEPT_OUTINTERNAL_TOCLINICDEPT = 87,
        DRUGDEPT_INWARD_MEDDEPTFROMCLINICDEPT = 88,
        RptThuTienHangNgay = 89,
        REGISTRATION_OUT_PATIENT_RECEIPT = 90,
        DRUGDEPT_INWARD_MEDDEPTSUPPLIER_TRONGNUOC = 91,
        REGISTRATION_OUT_PATIENT_HI_CONFIRMATION = 92,
        PHARMACY_THEODOISOLUONGTHUOC = 93,

        RptPatientApptServiceDetails = 94,
        RptPatientApptPCLRequests = 95,

        CONSULTATION_BANGKECHITIETKHAM = 96,

        DRUGDEPT_REQUEST = 52,
        DRUGDEPT_REQUEST_DETAILS = 97,
        DRUGDEPT_REQUEST_APPROVED = 98,
        DRUGDEPT_REQUEST_DETAILS_APPROVED = 99,
        CLINICDEPT_OUTWARD_DRUGDEPT = 100,
        CLINICDEPT_OUTWARD_PATIENT = 101,
        PHARMACY_BCHANGNGAY_NOPTIENCHITIET = 102,
        PATIENTCASHADVANCE_REPORT = 103,
        BAOCAOCHITIETTHANHTOAN = 104,
        PHIEUDENGHITAMUNG = 105,

        REGISTRATION_OUT_PATIENT_RECEIPT_XML = 106,
        PHARMACY_PHIEUNHANTHUOC_BH = 107,
        REGISTRATIOBLIST = 108,
        REGISTRATION_IN_PATIENT_HI_CONFIRMATION = 109,
        THONG_TIN_XUAT_VIEN = 110,
        REGISTRATION_HUY_HOADON = 111,
        REGISTRATION_HUY_HOADON_CHITIET = 112,
        DANHSACH_DANGKY_BHYT = 113,
        BAOCAONHANH_KHUKHAMBENH = 114,

        BAOCAOHENBENH = 115,

        BANGKETHUPHI_XN_THEONGAY = 116,
        BANGLETHUPHI_KB_CDHA_THEONGAY = 117,
        //Phiếu YC CLS (in gộp)
        PatientPCLRequestDetailsByPatientPCLReqID_XML = 118,
        AppointmentReport = 119,
        CLINICDEPT_KIEMKE_KHOAPHONG = 120,
        RptPatientApptPCLRequests_XML = 121,
        REGISTRATIONDETAILLIST = 122,
        XRptPharmacySellingPriceList_Detail_Simple = 123,
        PCLExamTypeTarget = 124,

        REGISTRATION_HI_APPOINTMENT = 125,

        BANGLETHUPHICHITIET_KB_CDHA_THEONGAY = 126,
        PHIEUNOPTIEN = 127,
        PHIEUTHUTIENTONGHOP = 128,
        RptHoatDongQuayDK = 129,
        [Description("Mẫu 25 trả thuốc chi tiết")]
        TEMP25aTRATHUOC_CHITIET = 130,
        TEMP25aTRATHUOC_TONGHOP = 131,
        [Description("Mẫu 20 ngoại trú trả thuốc")]
        TEMP20_NGOAITRU_TRATHUOC = 132,
        //Mẫu toa thuốc của phòng mạch bác Huân.
        CONSULTATION_TOATHUOC_PRIVATE = 133,
        //Mẫu phiếu nhận thuốc của phòng mạch bác Huân.
        PHARMACY_PHIEUNHANTHUOC_PRIVATE = 134,
        //Toa thuốc nội trú
        CONSULTATION_TOATHUOC_INPT = 135,
        REGISTRATION_CASH_ADVANCE_BILL_INPT = 136,
        PHIEUDENGHI_THANHTOAN = 137,
        PHARMACY_THEODOITHUOCCOGIOIHANSL = 138,
        MEDDEPT_OUTWARD_FROM_PRESCRIPTION = 139,
        MEDDEPT_PRINT_BILL = 140,

        BAOCAO_THUTIEN_TAMUNG = 141,

        INPATIENT_SETTLEMENT = 142,

        REPORT_IMPORT_EXPORT_DEPARTMENT = 143,

        REPORT_PATIENT_ARE_TREATED = 144,

        REGISTRATION_IN_PT_REPAYCASHADVANCE = 145,

        REPORT_PATIENT_SETTLEMENT = 146,

        REPORT_OUTWARD_MEDDEPT_INFLOW = 147,

        REPORT_CLINIC_OUTWARD_TO_PATIENT = 148,

        REPORT_INPT_NOT_PAY_CASH_ADVANCE = 149,

        REGISTRATION_HI_APPOINTMENT_INPT = 150, //In giấy hẹn tái khám BHYT nội trú.

        [Description("Mẫu 20 VTYT TH")]
        TEMP20_VTYTTH = 151,

        FORM_02_NoiTru = 152,
        [Description("Báo cáo bệnh nhân điều trị nội trú")]
        REPORT_INPATIENT = 153,
        [Description("Tổng hợp chi phí điều trị nội trú")]
        REPORT_GENERAL_TEMP02 = 154,
        [Description("Mẫu 79 chi tiết")]
        TEMP79a_CHITIET = 155,
        TEMP79a_TONGHOP = 156,
        [Description("Mẫu 79 trả thuốc chi tiết")]
        TEMP79aTRATHUOC_CHITIET = 157,
        TEMP79aTRATHUOC_TONGHOP = 158,
        [Description("Mẫu 80 chi tiết")]
        TEMP80a_CHITIET = 159,
        TEMP80a_TONGHOP = 160,

        [Description("Mẫu 19")]
        TEMP19 = 161,
        [Description("Mẫu 20 nội trú - New")]
        TEMP20_NOITRU_NEW = 162,
        [Description("Mẫu 21 - New")]
        TEMP21_NEW = 163,


        PCLDEPARTMENT_IMAGERESULT_HEART_ULTRASOUND = 164,

        HI_APPOINTMENT = 165,
        [Description("Sổ kiểm nhập thuốc")]
        PHARMACY_SOKIEMNHAPTHUOC = 166,

        REGISTRATION_OUT_PATIENT_RECEIPT_XML_V2 = 167,
        INPATIENT_SETTLEMENT_V2 = 168,
        PHIEU_THU_KHAC = 169,
        BAOCAO_THUTIEN_KHAC = 170,
        REPORT_INPT_NOT_PAY_ALL_BILL = 171,
        TEMP19_V2 = 172,
        [Description("Mẫu 20 BHYT")]
        TEMP20_V2 = 173,
        [Description("Mẫu 21 BHYT")]
        TEMP21_V2 = 174,
        [Description("Mẫu 79 BHYT")]
        TEMP79_V2 = 175,
        [Description("Mẫu 79 trả thuốc BHYT")]
        TEMP79_TRATHUOC_V2 = 176,
        [Description("Mẫu 80 BHYT")]
        TEMP80_V2 = 177,
        [Description("Công văn 9324 - Bảng 1")]
        TEMP9324_BANG_1 = 178,
        [Description("Công văn 9324 - Bảng 2")]
        TEMP9324_BANG_2 = 179,
        [Description("Công văn 9324 - Bảng 3")]
        TEMP9324_BANG_3 = 180,
        [Description("Thông tin bệnh nhân")]
        PATIENT_INFO = 181,
        [Description("Báo cáo viện phí")]
        HOSPITAL_FEES_REPORT = 182,
        [Description("Báo cáo viện phí KTC")]
        HIGH_TECH_FEES_REPORT = 183,
        [Description("Báo cáo xuất VTYT KTC")]
        OUT_MEDICAL_MATERIAL_REPORT = 184,

        [Description("Báo cáo hàng có giới hạn SL xuất")]
        DRUG_DEPT_WATCH_OUT_QTY = 185,

        [Description("Báo cáo Viện phí mổ trẻ em dưới 6 tuổi")]
        HIGH_TECH_FEES_CHILD_REPORT = 186,

        [Description("Siêu âm bụng")]
        ABDOMINAL_ULTRASOUND_RESULT = 187,

        [Description("Siêu âm tim thai")]
        FETAL_ECHOCARDIOGRAPHY = 188,

        [Description("Danh sách bệnh nhân siêu âm")]
        ULTRASOUND_STATISTICS = 189,

        HR_STATISTICS_BY_DEPT = 190,

        MED_EXAM_ACTIVITY = 191,

        TREATMENT_ACTIVITY = 192,

        SPECIALIST_TREATMENT_ACTIVITY = 193,

        SURGERY_ACTIVITY = 194,

        REPRODUCTIVE_HEALTH_ACTIVITY = 195,

        PCL_ACTIVITY = 196,

        PHARMACY_DEPT_STATISTICS = 197,

        MEDICAL_EQUIPMENT_STATISTICS = 198,

        SCIENTIFIC_RESEARCH_ACTIVITY = 199,

        FINANCIAL_ACTIVITY_TEMP1 = 200,

        FINANCIAL_ACTIVITY_TEMP2 = 201,

        FINANCIAL_ACTIVITY_TEMP3 = 202,

        ICD10_STATISTICS = 203,

        [Description("Chỉ tiêu nhân sự")]
        STAFFDEPTPRESENCE = 204,

        REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4 = 205,
        INPATIENT_SETTLEMENT_V4 = 206,
        PHIEU_THU_KHAC_V4 = 207,
        INPT_INSTRUCTION = 208,
        [Description("Y lệnh")]
        PATIENTINSTRUCTION = 209,
        [Description("Giấy chuyển tuyến")]
        TRANSFERFORM = 210,
        MEDICALFILESLIST = 211,
        MEDICALFILECHECKOUTCONFIRM = 212,
        MEDICALFILECHECKOUTHISTORY = 213,
        CONSULTINGDIAGNOSYSHISTORY = 214,
        [Description("Hoạt động cận lâm sàn")]
        TRANSACTION_CANLAMSAN = 215,
        [Description("Dược bệnh viện")]
        TRANSACTION_DUOCBV = 216,
        [Description("Hoạt động khám bệnh")]
        TRANSACTION_HOATDONGKB = 217,
        [Description("Thông tin bệnh nhân")]
        CLINIC_INFO = 218,
        [Description("Tình hình bệnh tật theo ICD")]
        FollowICD = 219,
        [Description("Tình hình cán bộ, công chức, viên chức")]
        EmployeesReport = 220,
        HoatDongChiDaoTuyen = 221,
        [Description("Tổng hợp thông tin các tuyến chuyển đến")]
        TransferFormType2Rpt = 222,
        [Description("Tổng hợp thông tin chuyển đi các tuyến")]
        TransferFormType2_1Rpt = 223,
        [Description("Báo cáo công tác chuyển tuyến")]
        TransferFormType5Rpt = 224,
        [Description("Báo cáo 15 ngày sử dụng thuốc,hoá chất, vật tư tiêu hao")]
        BaoCao15Ngay = 225,
        [Description("Sổ vào viện - ra viện - chuyển viện")]
        InPtAdmDisStatistics = 226,
        [Description("Sổ kiểm nhập thuốc/hoá chất/vật dụng y tế tiêu hao")]
        RptDrugMedDept = 227,
        [Description("Sổ xét nghiệm")]
        SoXetNghiem = 228,
        [Description("Theo dõi hàng xuất ký gởi")]
        THEODOIHANGXUATKYGOI = 229,
        TRANSFERFORMDATA = 230,
        EstimationPriceReport = 231,
        /*▼====: #002*/
        [Description("Phiếu chỉ định")]
        REGISTRATION_SPECIFY_VOTES_XML = 232,
        /*▲====: #002*/
        [Description("Bien lai thu tien TV")]
        REGISTRATION_OUT_PATIENT_RECEIPTS_TV = 233,
        [Description("Phieu De Nghi Tam Ung TV")]
        PHIEUDENGHITAMUNG_TV = 234,
        /*▼====: #003*/
        [Description("PHIEUNHANTHUOC_SUMMARY")]
        PHARMACY_PHIEUNHANTHUOC_SUMMARY = 235,
        /*▲====: #003*/
        [Description("Mẫu 12 (Gộp 01+02)")]
        TEMP12 = 236,
        [Description("Phieu Mien Giam Ngoai Tru TV")]
        PHIEUMIENGIAM_NGOAITRU_TV = 237,
        [Description("Báo cáo trẻ em dưới 6 tuổi")]
        BAO_CAO_TRE_EM_DUOI_6_TUOI = 238,
        [Description("Báo cáo miễn giảm ngoại trú")]
        BAOCAO_MIENGIAM_NGOAITRU_TV = 239,
        [Description("Danh sách bệnh nhân tiếp nhận theo đối tượng")]
        DS_BNTiepNhanTheoDT = 240,
        [Description("Thống Kê Tiếp Nhận Theo Địa Bàn Cư Trú")]
        TKe_TNhanTheoDiaBanCuTru = 241,
        [Description("Phieu De Nghi Chi Dinh DichVu")]
        RptPatientServiceRequestDetailsByPatientServiceReqID = 242,
        [Description("Thống Kê Tiếp Nhận Theo Địa Bàn Cư Trú")]
        BAO_CAO_VIEN_PHI_BHYT = 243,
        [Description("Phieu De Nghi Chi Dinh DichVu")]
        BAO_CAO_VIEN_PHI_NGOAI_TRU = 244,
        [Description("Phiếu quyết toán ngoại trú")]
        RptOutPtTransactionFinalization = 245,
        [Description("Bảng kê thu hoàn ứng nội trú")]
        BangKeThuHoanUngNT = 246,
        [Description("Bảng kê thu tạm ứng nội trú")]
        BangKeThuTamUngNT = 247,
        [Description("Bảng kê thu tiền viện phí nội trú")]
        BangKeThuTienVienPhiNT = 248,
        [Description("Phiếu yêu cầu của kho BHYT - Nhà thuốc")]
        DrugDept_Request_HIStore = 249,
        [Description("Phiếu yêu cầu của kho BHYT - Nhà thuốc, đã duyệt")]
        DrugDept_Request_HIStore_Approved = 250,
        [Description("Chi tiết phiếu yêu cầu của kho BHYT - Nhà thuốc")]
        DrugDept_Request_HIStore_Details = 251,
        [Description("Chi tiết phiếu yêu cầu của kho BHYT - Nhà thuốc, đã duyệt")]
        DrugDept_Request_HIStore_Details_Approved = 252,
        [Description("Thẻ thủ thuật")]
        The_Thu_Thuat = 253,
        [Description("Báo cáo thu tiền Ngoại trú theo Biên lai")]
        BaoCaoThuTienNgoaiTruTheoBienLai = 254,
        [Description("DANH SÁCH BỆNH NHÂN CHỈ ĐỊNH THỰC HIỆN CLS")]
        DsachBNChiDinhThucHienXN = 255,
        [Description("TÌNH HÌNH HOẠT ĐỘNG CẬN LÂM SÀNG")]
        TinhHinhHoatDongCLS = 256,
        [Description("Bảng Kê Bác sĩ Thực hiện CLS")]
        BangKeBacSiThucHienCLS = 257,
        [Description("Phiếu công khai thuốc")]
        REPORT_PHIEU_CONG_KHAI_THUOC = 258,
        [Description("Báo cáo Giao Ban")]
        BAO_CAO_GIAO_BAN = 259,
        [Description("Thống kê Nhập xuất Theo mục đích")]
        TK_NX_THEOMUCDICH = 260,
        [Description("Kế Toán - Báo cáo doanh thu Ngoại trú")]
        KT_BaoCaoDoanhThuNgTru = 261,
        [Description("Kế Toán - Báo cáo doanh thu Nội trú")]
        KT_BaoCaoDoanhThuNoiTru = 262,
        [Description("TÌNH HÌNH HOẠT ĐỘNG CẬN LÂM SÀNG-Khoa XN")]
        TinhHinhHoatDongCLS_XN = 263,
        [Description("Thống kê Danh sách khám bệnh nội trú theo bác sĩ")]
        ThongKeDsachKBNoiTruTheoBacSi = 264,
        [Description("Nhập hàng VTYTTH từ nhà cung cấp ngoài nước")]
        DRUGDEPT_INWARD_VTYTTHSUPPLIER = 265,
        [Description("Nhập hàng VTYTTH từ nhà cung cấp trong ngoài")]
        DRUGDEPT_INWARD_VTYTTHSUPPLIER_TRONGNUOC = 266,
        [Description("Bảng kê Bác sĩ thực hiện PT-TT")]
        BangKeBacSiThucHienPT_TT = 267,
        [Description("Bảng kê bán lẻ hàng hóa, dịch vụ")]
        BangKeBanLeHangHoaDV = 268,
        [Description("Nhập hàng tiêm ngừa từ nhà cung cấp ngoài nước")]
        DRUGDEPT_INWARD_TIEM_NGUA_SUPPLIER = 269,
        [Description("Nhập hàng tiêm ngừa từ nhà cung cấp trong nước")]
        DRUGDEPT_INWARD_TIEM_NGUA_SUPPLIER_TRONGNUOC = 270,
        [Description("Nhập hàng hoá chất từ nhà cung cấp ngoài nước")]
        DRUGDEPT_INWARD_HOA_CHAT_SUPPLIER = 271,
        [Description("Nhập hàng hoá chất từ nhà cung cấp trong nước")]
        DRUGDEPT_INWARD_HOA_CHAT_SUPPLIER_TRONGNUOC = 272,
        [Description("Thống kê Danh sách khám bệnh ngoại trú theo bác sĩ")]
        ThongKeDsachKBNgTruTheoBacSi = 273,
        [Description("Nhập máu từ nhà cung cấp ngoài nước")]
        DRUGDEPT_INWARD_MAU_SUPPLIER = 274,
        [Description("Nhập máu từ nhà cung cấp trong nước")]
        DRUGDEPT_INWARD_MAU_SUPPLIER_TRONGNUOC = 275,
        [Description("Nhập hàng thanh trùng từ nhà cung cấp ngoài nước")]
        DRUGDEPT_INWARD_THANH_TRUNG_SUPPLIER = 276,
        [Description("Nhập hàng thanh trùng từ nhà cung cấp trong nước")]
        DRUGDEPT_INWARD_THANH_TRUNG_SUPPLIER_TRONGNUOC = 277,
        [Description("KTTH- Báo cáo tình hình dịch lây và các bệnh quan trọng")]
        KTTH_BCTinhHinhDichLay = 278,
        OutwardDrugsByStaffStatistic = 279,
        [Description("Nhập hàng thanh trùng từ nhà cung cấp ngoài nước")]
        DRUGDEPT_INWARD_VPP_SUPPLIER = 280,
        [Description("Nhập hàng thanh trùng từ nhà cung cấp trong nước")]
        DRUGDEPT_INWARD_VPP_SUPPLIER_TRONGNUOC = 281,
        [Description("Nhập vật tư tiêu hao từ nhà cung cấp ngoài nước")]
        DRUGDEPT_INWARD_VTTH_SUPPLIER = 282,
        [Description("Nhập vật tư tiêu hao từ nhà cung cấp trong nước")]
        DRUGDEPT_INWARD_VTTH_SUPPLIER_TRONGNUOC = 283,
        [Description("Biên Lai Thu Tiền theo máy in nhiệt")]
        REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4_THERMAL = 284,
        [Description("Báo cao thu tiền nhà thuốc tại quầy BHYT")]
        BC_THUTIEN_NT_TAI_QUAY_BHYT = 285,
        [Description("KTTH Báo cáo xuất viện")]
        KTTH_BC_XUAT_VIEN = 286,
        [Description("KTTH Báo cáo chỉ định nhập viện")]
        KTTH_BC_CHI_DINH_NHAP_VIEN = 287,
        [Description("Báo cáo thủ thuật cho Khoa/Phòng")]
        BC_NHAP_PTTT_KHOA_PHONG = 288,
        [Description("Báo cáo công nợ nội trú")]
        BC_CONGNO_NOITRU = 289,
        [Description("Báo cáo Nhập xuất tồn tổng hợp (thuốc) - Tất cả các kho (156) của khoa phòng")]
        BC_NXT_THUOC_TONGHOP = 290,
        [Description("Thống kê tình hình chỉ định thuốc chi tiết")]
        OutwardDrugsByStaffStatisticDetails = 291,
        [Description("Phiếu thu tiền (Thay thế cho hóa đơn)")]
        REGISTRATION_OUT_PATIENT_RECEIPT_XML_V5 = 292,
        [Description("Báo cáo BN xét nghiệm mỗi ngày")]
        BCBNXNMoiNgay = 293,
        [Description("Báo cáo Phiếu xuất Kho BHYT chưa thu tiền")]
        BCPXKhoBHYTChuaThuTien = 294,
        [Description("Báo cáo thời gian BN chờ xét nghiệm")]
        BCThoiGianBNChoXN = 295,
        [Description("Báo cáo PT-TT chưa thực hiện")]
        BaoCaoPT_TTChuaThucHien = 296,
        [Description("Phiếu nhận thuốc In Nhiệt")]
        PHIEUNHANTHUOC_BHYT_THERMAL = 297,
        [Description("Phiếu nhận thuốc BHYT In Nhiệt")]
        PHIEUNHANTHUOC_THERMAL = 298,
        [Description("Phiếu nhận thuốc summary In Nhiệt")]
        PHIEUNHANTHUOC_SUMMARY_THERMAL = 299,
        [Description("Báo cáo chi tiết dịch vụ & CLS")]
        BCChiTietDV_CLS = 300,
        [Description("Báo cáo doanh thu Thuốc BHYT Ngoại trú")]
        BCDoanhThuThuocBHYTNgTru = 301,
        [Description("Theo dõi thông tin chi tiết khách hàng của Nhà Thuốc")]
        TKTheoDoiTTChiTietKH_NT = 302,
        [Description("Theo dõi NXT thuốc khác của Nhà Thuốc")]
        TKTheoDoiNXTThuocKhac_NT = 303,
        [Description("ThongTu22_Mẫu Báo cáo về Kháng sinh")]
        TT22_BC_KhangSinh = 304,
        [Description("ThongTu22_Mẫu Báo cáo Sử dụng Thuốc")]
        TT22_BC_SuDungThuoc = 305,
        [Description("ThongTu22_Mẫu Báo cáo Sử dụng Hóa Chất")]
        TT22_BC_SuDungHoaChat = 306,
        [Description("Báo cáo thời gian chờ BN tại Bệnh viện")]
        BCThoiGianBNChoTaiBV = 307,
        [Description("Báo cáo Xuất dược nội bộ Nhà Thuốc")]
        BCXuatDuocNoiBo_NT = 308,
        [Description("Báo cáo BN hoàn trả")]
        BCBNHoanTraBienLai = 309,
        [Description("Báo cáo chi tiết nhập từ NCC")]
        BCChiTietNhapTuNCC = 310,
        [Description("KHTH Thống kê tình hình hoạt động Khám bệnh")]
        KHTH_TinhHinhKCB = 311,
        [Description("KT Tình hình hoạt động dịch vụ")]
        KT_TinhHinhHoatDongDV = 312,
        [Description("KT Báo cáo doanh thu theo Khoa")]
        RptTongHopDoanhThuTheoKhoa = 313,
        [Description("KT Báo cáo hàng tồn nhiều")]
        KT_BCHangTonNhieu = 314,
        [Description("KT Báo cáo hàng cận date")]
        KT_BCHangCanDate = 315,
        [Description("KT Báo cáo BN xuất viện mà còn nợ viện phí")]
        KT_BCBNXuatVienConNoVienPhi = 316,
        [Description("KT Báo cáo Trả sau")]
        KT_BCBNTraSau = 317,
        [Description("NT Báo cáo bán thuốc lẻ")]
        NT_BCBanThuocLe = 318,
        [Description("Sổ khám sức khỏe")]
        Kham_Suc_Khoe = 319,
        [Description("Chi tiết hợp đồng KSK")]
        CT_HD_KhamSucKhoe = 320,
        [Description("Y lệnh")]
        PATIENTINSTRUCTION_TH = 321,
        [Description("Bảng giá DV xem in")]
        RptBangGiaDV = 322,
        [Description("Bảng giá CLS xem in")]
        RptBangGiaCLS = 323,
        [Description("Bệnh án")]
        MedicalRecord = 324,
        [Description("BC Công tác khám chữa bệnh")]
        KHTH_BCCongTacKCB = 325,
        [Description("BC Nhóm CLS được chỉ định")]
        KB_BCCLSDuocChiDinh = 326,
        [Description("BC BSy chỉ định CLS")]
        KB_BCBsyChiDinhCLS = 327,
        [Description("BC tình hình sử dụng thuốc theo thầu")]
        DrugDeptInOutStockByBid = 328,
        [Description("Nhà Thuốc - Phiếu nhập hàng nội bộ")]
        Pharmacy_InwardFromInternal = 329,
        [Description("Báo cáo thuốc sắp hết hạn dùng")]
        BC_ThuocSapHetHanDung = 330,
        [Description("Phiếu chỉ định CLS - Y lệnh")]
        RptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML = 331,
        [Description("KT - Báo cáo hàng không xuất")]
        KT_BCHangKhongXuat = 332,
        [Description("Thống kê điều trị nhiễm khuẩn")]
        InfectionCaseStatistics = 333,
        [Description("Giấy cam đoan thực hiện thủ thuật")]
        GiayCamDoanThucHienTT = 334,
        [Description("Phiếu thu tiền khác cho Hóa đơn")]
        PHIEU_THU_KHAC_V5 = 335,
        [Description("Danh sách bệnh nhân khám có toa thuốc vượt trần")]
        DS_BN_CoToaThuocVuotTran = 336,
        [Description("Báo giá")]
        PatientQuotation = 337,
        [Description("Thống kê bệnh thường gặp")]
        TreatsStatisticsByDept = 338,
        [Description("Thống kê bệnh thường gặp")]
        TreatsStatisticsByDept_Detail = 339,
        [Description("TT22 - Biên bản kiểm kê")]
        TT22_BienBanKiemKe = 340,
        [Description("Mẫu bệnh án chung Ngoại trú")]
        GeneralOutPtMedicalFile = 341,
        [Description("Mẫu bệnh án sản khoa")]
        ObstetricsMedicalFile = 342,
        [Description("Mẫu bệnh án nhi khoa")]
        PediatricsMedicalFile = 343,
        [Description("Thẻ kho cho kế toán")]
        CardStorage_KT = 344,
        [Description("Phiếu khám sức khỏe")]
        HosClientContractPatientSummary = 345,
        [Description("Biên bản hội chẩn")]
        BienBanHoiChan = 346,
        [Description("Phiếu chỉ định BN Nội trú")]
        REGISTRATION_INPT_SPECIFY_VOTES_XML = 347,
        [Description("Báo cao thu tiền nhà thuốc tại quầy BHYT (Theo biên lai)")]
        BC_THUTIEN_NT_TAI_QUAY_BHYT_THEO_BIEN_LAI = 348,
        [Description("Báo cao miễn giảm nội trú")]
        BAOCAO_MIENGIAM_NOITRU_TV = 349,
        [Description("Mâu 12 nội bộ")]
        Temp12_NoiBo_TV = 350,
        [Description("Nhà Thuốc - Bảng kê xuất thuốc theo BN")]
        Pharmacy_BKXuatThuocTheoBN = 351,
        [Description("Dược lâm sàng - Báo cáo xuất thuốc cho BN từ khoa phòng")]
        DLS_BCXuatThuocBNKhoaPhong = 352,
        [Description("Bao cao tinh hinh hoat dong CLS - KK")]
        TinhHinhHoatDongCLS_KK = 353,
        [Description("Báo cáo danh sách bệnh nhân hẹn tái khám có bệnh mãn tính ")]
        BC_BNTaiKhamBenhManTinh = 354,
        [Description("Báo cáo bệnh nhân sử dụng máy thở")]
        BC_BNSuDungMayTho = 355,
        [Description("Báo cáo toa thuốc hàng ngày KD")]
        BC_ToaThuocHangNgay_KD = 356,
        [Description("Báo cáo toa thuốc hàng ngày DLS")]
        BC_ToaThuocHangNgay_DLS = 357,
        [Description("Báo cáo thuốc, y cụ dỡ dang cuối kỳ")]
        BC_ThuocYCuDoDangCuoiKy = 358,
        [Description("Biên bản giao nhận vaccine")]
        BBGiaoNhanVaccine = 359,
        [Description("Phiếu chỉ định CLS - Theo phòng")]
        XRptPatientPCLRequestDetailsByPatientPCLReqID_TV3 = 360,
        [Description("Thống kê tình hình chỉ định thuốc (TP) khoa phòng chi tiết")]
        OutwardDrugClinicDeptsByStaffStatisticDetails_TP = 361,
        [Description("Thông kê theo dõi thông tin chi tiết khách hàng theo thuốc")]
        TKTheoDoiTTChiTietKH_NTTheoThuoc = 362,
        [Description("In thẻ Khám chữa bệnh")]
        InTheKCB = 363,
        [Description("In phiếu cung cấp máu")]
        XRptPhieuCungCapMau = 364,
        [Description("Phiếu sao thuốc")]
        XRptPhieuSaoThuoc = 365,
        [Description("Toa thuốc gây nghiện - hướng thần")]
        CONSULTATION_TOATHUOC_GN_HT = 366,
        [Description("Báo cáo thủ thuật của Khoa/Phòng cho KHTH")]
        BC_NHAP_PTTT_KHOA_PHONG_KHTH = 367,
        [Description("KHTH Báo cáo xuất viện")]
        BC_XUAT_VIEN_KHTH = 368,
        [Description("Xét nghiệm - Phiếu truyền máu")]
        PHIEU_TRUYEN_MAU = 369,
        [Description("Phiếu treo")]
        XRptPhieuTreo = 370,
        [Description("Mẫu 12 (Gộp 01+02) Version 6556")]
        TEMP12_6556 = 371,
        [Description("Mẫu 12 Tổng hợp Temp12 + Version 6556")]
        TEMP12_TONGHOP = 372,
        [Description("Sổ BHYT - XQuang")]
        XRptSoXQuang = 373,
        [Description("Sổ BHYT - XetNghiem")]
        XRptSoXetNghiem = 374,
        [Description("Sổ BHYT - ThuThuat")]
        XRptSoThuThuat = 375,
        [Description("Sổ BHYT - SieuAm")]
        XRptSoSieuAm = 376,
        [Description("Sổ BHYT - KhamBenh")]
        XRptSoKhamBenh = 377,
        [Description("Sổ BHYT - DienTim")]
        XRptSoDienTim = 378,
        [Description("Báo cáo bệnh nhân tử vong (chi tiết)")]
        RptPatientDecease = 379,
        [Description("Báo cáo dịch vụ kỹ thuật đã thực hiện")]
        BCDichVuCLSDangThucHien_PhanTuyen = 380,
        [Description("Báo cáo chi tiết dịch vụ & CLS")]
        BCChiTietDV_CLS_KHTH = 381,
        [Description("Phiếu báo ăn dinh dưỡng")]
        XRptClinicDeptRequestFood = 382,
        [Description("Danh sách chi tiết BN chuyển đi các tuyến")]
        BCChiTietBNChuyenDi = 383,
        [Description("Danh sách chi tiết BN tuyến chuyển đến")]
        BCChiTietBNChuyenDen = 384,
        [Description("Báo cáo tổng hợp công tác chuyển tuyến")]
        BCCongTacChuyenTuyen = 385,
        [Description("Báo cáo NXT Thuốc toàn khoa Dược")]
        BC_NXT_THUOC_TONGHOP_V2 = 386,
        [Description("Tóm tắt quá trình điều trị")]
        TomTatQuaTrinhDieuTri = 387,
        [Description("Báo cáo thu tiền qua thẻ")]
        KT_BC_ThanhToanQuaThe = 388,
        [Description("Báo Cáo Danh Sách Dịch Vụ Kỹ Thuật Có Trên HIS")]
        BC_DS_DichVuKyThuatTrenHIS = 389,
        [Description("Báo Cáo Danh Mục Kỹ Thuật Mới")]
        BC_DM_KyThuatMoi = 390,
        [Description("Báo Cáo Bệnh Nhân Khám Bệnh")]
        BC_BenhNhanKhamBenh = 391,
        [Description("Báo Cáo Bệnh Nhân Hẹn Tái Khám Bệnh Đặc Trưng")]
        BC_BNHenTaiKhamBenhDacTrung = 392,
        [Description("Báo Cáo Thời Gian Chờ Trong Bệnh Viện")]
        BC_TGianChoTrongBV = 393,
        [Description("Xem/In phiếu tư vấn khu khám")]
        XRpt_AdvisoryVotes = 394,
        [Description("Xem/In phiếu đánh giá và kế hoạch dinh dưỡng")]
        XRpt_NutritionalRating = 395,
        [Description("Xem/In Tóm tắt hồ sơ bệnh án")]
        XRpt_TomTatHoSoBenhAn = 396,
        [Description("Xem/In Phiếu xác nhận điếu trị ngoại trú/ nội trú")]
        XRpt_XacNhanDieuTri_NgoaiTru_NoiTru = 397,
        [Description("Xem/In Phiếu chứng nhận thương tích ngoại trú/ nội trú")]
        XRpt_ChungNhanThuongTich_NgoaiTru_NoiTru = 398,
        [Description("Xem/In Giấy chứng sinh")]
        XRpt_GiayChungSinh = 399,
        [Description("Xem/In Giấy báo tử")]
        XRpt_GiayBaoTu = 400,
        [Description("Xem/In Giấy nghĩ việc không hưởng bảo hiểm")]
        XRpt_GiayNghiViecKhongHuongBaoHiem = 401,
        [Description("Báo cáo hủy/hoàn tiền nhà thuốc")]
        PHARMACY_BCHuyHoan = 402,
        [Description("Báo cáo Mẫu 12 theo khoa phòng")]
        TEMP12_KHOAPHONG = 403,
        [Description("Báo cáo chẩn đoán hình ảnh")]
        XRpt_PCLImagingResult_New = 404,
        [Description("Báo cáo chẩn đoán hình ảnh 4 hình")]
        XRpt_PCLImagingResult_New_4_Hinh = 405,
        [Description("Báo cáo chẩn đoán hình ảnh 3 hình")]
        XRpt_PCLImagingResult_New_3_Hinh = 406,
        [Description("Báo cáo chẩn đoán hình ảnh 0 hình")]
        XRpt_PCLImagingResult_New_0_Hinh = 407,
		[Description("Xuất excel 4210")]
        ExportExcel4210 = 408,
        [Description("Biên bản hội chẩn")]
        XRpt_BienBanHoiChan = 409,
        [Description("Phiếu khám vào viện")]
        XRpt_AdmissionExamination = 410,
        [Description("Phiếu chăm sóc")]
        XRpt_PhieuChamSoc = 411,
        //▼====: #008
        [Description("Báo cáo kho tổng hợp")]
        BC_KHO_TONGHOP = 412,
        //▲====: #008
        //▼====: #009
        [Description("Mẫu 80a cho doanh thu")]
        KTDoanhThu_TEMP80a = 413,
        //▲====: #009
        [Description("Báo cáo chẩn đoán hình ảnh 1 hình")]
        XRpt_PCLImagingResult_New_1_Hinh = 414,
        [Description("Mẫu xác nhận BN điều trị COVID")]
        XRpt_XacNhan_DieuTri_Covid = 415,
		//▼====: #010
        [Description("Xuất excel danh sách XN chờ kết quả")]
        ExportExcel_LabWaitResult = 416,
        //▲====: #010
        [Description("Báo cáo chẩn đoán hình ảnh 6 hình")]
        XRpt_PCLImagingResult_New_6_Hinh = 417,
        [Description("Báo cáo chẩn đoán hình ảnh Virus Real- time PCR  SARS-CoV-2")]
        XRpt_PCLImagingResult_New_Realtime_PCR_Cov = 418,
        [Description("Báo cáo chẩn đoán hình ảnh Virus test nhanh SARS - CoV - 2")]
        XRpt_PCLImagingResult_New_Test_Nhanh_Cov = 419,
        [Description("Báo cáo chẩn đoán hình ảnh Xét nghiệm")]
        XRpt_PCLImagingResult_New_Xet_Nghiem = 420,
        [Description("Báo cáo chẩn đoán hình ảnh Helicobacter Pylori")]
        XRpt_PCLImagingResult_New_Helicobacter_Pylori = 421,
        [Description("Báo cáo chẩn đoán hình ảnh Siêu âm tim")]
        XRpt_PCLImagingResult_New_Sieu_Am_Tim = 422,
        [Description("Báo cáo chẩn đoán hình ảnh Siêu âm sản 4D")]
        XRpt_PCLImagingResult_New_Sieu_Am_San_4D = 423,
        [Description("Phiếu thực hiện y lệnh thuốc")]
        XRpt_PhieuThucHienYLenhThuoc = 424,
        [Description("Thuốc theo bệnh nhân trong phiếu xuât")]
        XRptRequestDrugDeptDetailsGroupByPatient = 425,
        [Description("Báo cáo chẩn đoán hình ảnh điện tim")]
        XRpt_PCLImagingResult_New_Dien_Tim = 426,
        [Description("Báo cáo chẩn đoán hình ảnh điện não")]
        XRpt_PCLImagingResult_New_Dien_Nao = 427,
        [Description("Báo cáo chẩn đoán hình ảnh ABI")]
        XRpt_PCLImagingResult_New_ABI = 428,
        [Description("Xuất excel báo cáo 19 thuốc bn covid cho Khoa Dược")]
        ExportExcel_Temp19_THUOC_COVID_KD = 429,
        [Description("Xuất excel báo cáo 19 vật tư y tế bn covid cho Khoa Dược")]
        ExportExcel_Temp19_VTYT_COVID_KD = 430,
        [Description("Xuất excel báo cáo 21 bn covid cho Kế toán tổng hợp")]
        ExportExcel_Temp21_COVID_KT = 431,
        [Description("Báo cáo công suất giường bệnh")]
        BC_Cong_Suat_Giuong_Benh = 432, 
        [Description("Báo cáo chẩn đoán hình ảnh điện tim gắng sức")]
        XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc = 433,
        [Description("Báo cáo chi tiết khám bệnh")]
        BC_ChiTietKhamBenh = 433, 
        [Description("Báo cáo hủy dịch vụ ngoại trú")]
        BC_HuyDichVu_NgT = 434,
        [Description("Báo cáo NXT - Kho cơ số của khoa phòng (Tổng hợp")]
        BC_NXT_THUOC_TONGHOP_COSO = 435,
		[Description("Giấy giao nhận bệnh nhân đã điều trị covid")]
        XRpt_GiaoNhan_BenhNhan_Covid = 436,
        [Description("Báo cáo bệnh đặc trưng")]
        TEMP79a_BCBENHDACTRUNG = 437,
        [Description("KHTH Báo cáo giao ban theo khoa lâm sàng")]
        BC_GiaoBan_Theo_KhoaLS_KHTH = 438,
        [Description("Phiếu lĩnh khi chỉ định cls có thuốc kèm theo")]
        XRptRequestDrugDeptByPCLRequest = 439,
        //▼====: #011
        [Description("Khoa khám - Báo cáo đo DHST")]
        BC_DoDHST = 440,
        //▲====: #011
        [Description("Mẫu bệnh án RMH")]
        MaxillofacialOutPtMedicalFile = 441,
        [Description("Mẫu CLVT hai hàm")]
        XRpt_PCLImagingResult_New_CLVT_Hai_Ham = 442,
        [Description("BÁO CÁO SỬ DỤNG TOÀN BỆNH VIỆN")]
        BCSuDungToanBV = 443,
        //▼====: #012
        [Description("Báo cáo quản lý kiểm duyệt hồ sơ KHTH")]
        KTTH_BC_QLKiemDuyetHoSo = 444,
        //▲====: #012
        //▼====: #013
        [Description("Báo cáo kiểm tra lịch sử KCB DLS")]
        DLS_BCKiemTraLichSuKCB = 445,
        //▲====: #013
        //▼====: #014
        [Description("Báo cáo thông tin danh mục thuốc DLS")]
        DLS_BCThongTinDanhMucThuoc = 446,
        //▲====: #014
        [Description("KHTH Báo cáo giao ban phòng KHTH")]
        BC_GiaoBan_Phong_KHTH = 447,
        [Description("Sổ BHYT - NoiSoi")]
        XRptSoNoiSoi = 448,
        //▼====: #015
        [Description("Báo cáo tình hình thực hiện CLS - Đã thực hiện")]
        BCTinhHinhCLS_DaThucHien = 449,
        //▲====: #015
        //▼====: #016
        [Description("Báo cáo máu sắp hết hạn dùng")]
        BCMauSapHetHanDung = 450,
        //▲====: #016
        //▼====: #018
        [Description("Báo cáo danh sách dịch vụ kỹ thuật xét nghiệm")]
        BCDsachDichVuKyThuatXN = 451,
        //▲====: #018
        //▼====: #019
        [Description("Báo cáo lượt xét nghiệm")]
        BaoCaoLuotXetNghiem = 452,
        //▲====: #019
        //▼====: #020
        [Description("Báo cáo Doanh thu theo Khoa")]
        BaoCaoDoanhThuTheoKhoa = 453,
        //▲====: #020
        //▼====: #022
        [Description("Báo Cáo Hoãn/Miễn Tạm Ứng Nội Trú")]
        BaoCaoHoanMienTamUngNoiTru = 454,
        //▲====: #022
        //▼====: #023
        [Description("Giấy Miễn Tạm Ứng Nội Trú")]
        GiayMienTamUngNoiTru = 455,
        //▲====: #023
        //▼====: #024
        [Description("Giấy Hoãn Tạm Ứng Nội Trú")]
        GiayHoanTamUngNoiTru = 456,
        //▲====: #024
        //▼====: #025
        [Description("Báo cáo tổng hợp thu tiền viện phí ngoại trú")]
        BC_THU_TIEN_VIEN_PHI_NGOAI_TRU = 457,
        [Description("Báo cáo tổng hợp hủy, hoàn tiền viện phí ngoại trú")]
        BC_HOAN_TIEN_VIEN_PHI_NGOAI_TRU = 458,
        [Description("Báo cáo tổng hợp thu tiền tạm ứng nội trú")]
        BC_THU_TIEN_TAM_UNG_NOI_TRU = 459,
        [Description("Báo cáo tổng hợp hủy, hoàn tiền tạm ứng nội trú")]
        BC_HOAN_TIEN_TAM_UNG_NOI_TRU = 460,
        //▲====: #025
        //▼====: #026
        [Description("Huớng dẫn sử dụng thuốc")]
        Huong_Dan_Su_Dung_Thuoc = 461,
        //▲====: #026
        //▼====: #027
        [Description("Khoa khám - Báo cáo BN trễ hẹn điều trị ngoại trú")]
        BC_BNTreHenNgoaiTru = 462,
        //▲====: #027
        //▼====: #028
        [Description("Báo cáo thang điểm cảnh báo sớm")]
        BaoCaoThangDiemCanhBaoSom = 463,
        //▲====: #028
        //▼==== #029
        [Description("Báo cáo giao ban Khoa khám bệnh")]
        BCGiaoBanKhoaKhamBenh = 464,
        [Description("Báo cáo chỉ định cận lâm sàng – khoa khám")]
        BCChiDinhCLSKhoaKham = 465,
        //▲==== #029
        [Description("Sổ BHYT - Đo chức năng hô hấp")]
        XRptSoDoChucNangHoHap = 466,
        //▼==== #030
        [Description("Báo cáo thống kê số lượng hồ sơ điều trị ngoại trú")]
        BCThongKeSLHoSoDTNT = 467,
        //▲==== #030
        //▼==== #031
        [Description("Báo cáo danh sách BN điều trị ngoại trú")]
        BCDanhSachBNDTNT_KHTH = 468,
        //▲==== #031
        //▼==== #032
        [Description("Phiếu nhận thuốc")]
        PhieuSoanThuocPaging = 469,
        //▲==== #032
        //▼====: #033
        [Description("Báo cáo chẩn đoán hình ảnh Siêu âm sản 4D - mẫu 5 hình")]
        XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New = 470,
        [Description("Báo cáo chẩn đoán hình ảnh 6 hình (2 cột) - New")]
        XRpt_PCLImagingResult_New_6_Hinh_2_New = 471,
        [Description("Báo cáo chẩn đoán hình ảnh 6 hình (1 cột) - New")]
        XRpt_PCLImagingResult_New_6_Hinh_1_New = 472,
        [Description("Báo cáo chẩn đoán hình ảnh Siêu âm tim - New")]
        XRpt_PCLImagingResult_New_Sieu_Am_Tim_New = 473,
        [Description("Báo cáo chẩn đoán hình ảnh nội soi 9 hình")]
        XRpt_PCLImagingResult_New_Noi_Soi_9_Hinh = 474,
        //▲====: #033
        [Description("Báo cáo danh sách BN điều trị ngoại trú")]
        BaoCaoDSBenhNhanCapNhatKetQua = 475,
        //▼==== #034
        [Description("Phiếu công khai thuốc")]
        KK_PhieuCongKhaiThuoc = 476,
        [Description("Phiếu công khai dịch vụ khám, chữa bệnh nội trú")]
        PhieuCongKhaiDV = 477,
        [Description("Báo cáo danh sách bệnh nhân ĐTNT – khoa khám")]
        BCDanhSachBenhNhanDTNT = 478,
        //▲==== #034
        //▼==== #036
        [Description("Mẫu giấy đề nghị mở thẻ KCB")]
        XRpt_GiayDeNghiMoTheKCB = 479,
        [Description("Báo cáo Phát hành thẻ Khám chữa bệnh")]
        XRpt_PhatHanhTheKCB = 480,
        [Description("BÁO CÁO THỐNG KÊ SỐ LƯỢNG THẺ KHÁM CHỮA BỆNH")]
        XRpt_ThongKeSoLuongThe = 481,
        //▲==== #036
        //▼==== #039
        [Description("Báo cáo thao tác người dùng")]
        BCThaoTacNguoiDung = 482,
        //▲==== #039
        //▼==== #038
        [Description("Báo cáo lịch đăng ký khám ngoài giờ")]
        XRpt_LichDangKyKhamNgoaiGio = 483,
        [Description("Báo cáo giờ làm thêm bác sĩ")]
        XRpt_GioLamThemBacSi = 484,
        //▲==== #038
        //▼==== #040
        [Description("Thống kê hồ sơ ĐTNT")]
        BCThongKeHoSoDTNT = 485,
        //▲==== #040
        //▼==== #041
        [Description("Toa thuốc GN_HT nội trú")]
        CONSULTATION_TOATHUOC_INPT_GN_HT = 486,
        //▲==== #041
        [Description("Phiếu tự khai và cam kết điều trị")]
        XRptSelfDeclaration = 487,
        //▲==== #041
        //▼==== #042
        [Description("Mẫu bệnh án mãn tính")]
        ChronicOutPtMedicalFile = 488,
        //▲==== #042
        //▼==== #043
        [Description("Phiếu theo dõi dịch truyền")]
        PhieuTheoDoiDichTruyen = 489,
        //▲==== #043
        //▼==== #044
        [Description("Báo cáo vi khuẩn đa kháng/nhiễm khuẩn bệnh viện")]
        BaoCaoKSNK = 490,
        //▲==== #044
        //▼==== #045
        [Description("Báo cáo quản lý xuất sử dụng thuốc cản quang")]
        BCXuatSDThuocCanQuang = 491,
        //▲==== #045
        //▼==== #046
        [Description("Báo cáo huỷ đẩy cổng quốc gia")]
        BCHuyDQGReport = 492,
        //▲==== #046
        //▲==== #043 
        //▼==== #044
        [Description("Báo cáo hủy đơn thuốc điện tử")]
        BCHuyDTDT = 493,
        //▲==== #044
        //▼==== #046
        [Description("Báo cáo thời gian tư vấn cấp toa")]
        BCThGianTuVanCapToa = 494,
        [Description("Báo cáo thời gian tư vấn chỉ định")]
        BCThGianTuVanChiDinh = 495,
        //▲==== #046
        //▼==== #049
        [Description("Báo cáo xuất Khoa/Phòng - Xuất thuốc")]
        BC_XuatKP_Thuoc = 496,
        [Description("Báo cáo xuất Khoa/Phòng - Xuất y cụ")]
        BC_XuatKP_YCu = 497,
        //▲==== #049
        //▼==== #050
        [Description("Báo cáo khám bệnh BN ngoại trú và điều trị ngoại trú")]
        BC_DLS_KhamBenhNgoaiTru = 498,
        [Description("Báo cáo CLS BN ngoại trú và điều trị ngoại trú")]
        BC_DLS_CLSNgoaiTru = 499,
        [Description("Báo cáo Thuốc BN ngoại trú và điều trị ngoại trú")]
        BC_DLS_ThuocNgoaiTru = 500,
        //▲==== #050
        //▼==== #051
        [Description("Báo cáo chẩn đoán hình ảnh 0 hình")]
        XRpt_PCLImagingResult_New_0_Hinh_V2 = 501,
        [Description("Báo cáo chẩn đoán hình ảnh Xét nghiệm")]
        XRpt_PCLImagingResult_New_Xet_Nghiem_V2 = 502,
        //▲==== #051
        //▼==== #052
        [Description("Báo cáo chẩn đoán hình ảnh 0 hình")]
        XRpt_PCLImagingResult_New_0_Hinh_XN = 503,
        [Description("Báo cáo chẩn đoán hình ảnh 1 hình")]
        XRpt_PCLImagingResult_New_1_Hinh_XN = 504,
        //▲==== #052
        [Description("Báo cáo chẩn đoán hình ảnh 1 hình")]
        BaoCaoXNChuaNhapKetQua = 504,
        [Description("Xem/In Giấy ra viện")]
        XRpt_DisChargePapers = 505,
        //▼==== #053
        [Description("Kết quả tính tuổi động mạch")]
        XRpt_AgeOfTheArtery = 506,
        //▲==== #053
        [Description("Trích biên bản kiểm điểm tử vong")]
        XRpt_BienBanKiemDiemTuVong = 507,
        [Description("Sơ kết 15 ngày điều trị")]
        XRpt_TreatmentProcessSummary = 508,
        //▼==== #054
        [Description("Báo cáo thời gian tại bệnh viện")]
        XRpt_BCThGianCho_Khukham = 508,
        //▲==== #054      
        //▼==== #055
        [Description("Báo cáo thống kê đơn thuốc điện tử")]
        XRpt_BCTThongKeDTDT = 509,
        //▲==== #055
        [Description("Báo cáo phiếu lĩnh vật tư kèm DVKT")]
        DRUGDEPT_REQUEST_FOR_TECHNICALSERVICE = 510,
        //▼==== #055
        [Description("Báo cáo danh sách BN gửi SMS")]
        XRpt_AppointmentLab = 511,
        //▲==== #055
        //▼==== #057
        [Description("Báo cáo tổng hợp dự trù theo khoa/phòng")]
        DRUGDEPT_ESTIMATION_FOR_DEPT = 512
        //▲==== #057
    }

    public enum ReportType : int
    {
        [Description("Những báo cáo tổng hợp.")]
        BAOCAO_TONGHOP = 1,
        [Description("Bảng giá DV, CLS, thuốc.")]
        BANG_GIA = 2,
        [Description("Kiểm kê nhà thuốc, khoa dược, kho nội trú.")]
        KIEM_KE = 3,
        [Description("Báo cáo tồn kho nhà thuốc")]
        TON_KHO = 4,
        [Description("Danh mục Khoa Dược")]
        DANH_MUC_KHOA_DUOC = 5,
        [Description("Dự trù")]
        DU_TRU = 6,
        [Description("Những báo cáo của nhà thuốc")]
        BAOCAO_NHATHUOC = 7,
        [Description("Những báo cáo BHYT.")]
        BAOCAO_BHYT = 8,
        [Description("Sổ kiểm nhập")]
        SOKIEMNHAP = 9,
        //▼====: #004
        [Description("Những báo cáo tổng hợp cho kế toán")]
        BAOCAO_TONGHOP_KT = 10
        //▲====: #004
    }

    public enum ReceiptType : int
    {
        [Description("Quyết toán")]
        FINAL_SETTLEMENT = 1,
        [Description("Phiếu xuất bán lẻ của Khoa Dược")]
        OUTWARD_MEDDEPT = 2
    }

    public enum PriceListType : int
    {
        [Description("Bảng giá nhà thuốc")]
        BANG_GIA_NHA_THUOC = 1,
        [Description("Bảng giá dịch vụ khám chữa bệnh")]
        BANG_GIA_DV = 2,
        [Description("Bảng giá cận lâm sàng")]
        BANG_GIA_PCL = 3,
        [Description("Bảng giá khoa Dược")]
        BANG_GIA_KHOA_DUOC = 4,
    }

    public enum StockTakeType : int
    {
        [Description("Kiểm kê nhà thuốc")]
        KIEM_KE_NHA_THUOC = 1,
        [Description("Kiểm kê khoa Dược")]
        KIEM_KE_KHOA_DUOC = 2,
        [Description("Kiểm kê kho nội trú")]
        KIEM_KE_KHO_NOI_TRU = 3,
    }

    public enum DepartmentReport : int
    {
        [Description("Unknown Dept")]
        UNKNOWN_DEPT = 0,
        [Description("Tồn kho nhà thuốc")]
        NHA_THUOC = 1,
        [Description("Tồn kho Khoa Dược")]
        KHOA_DUOC = 2,
    }

    public class StockTake : NotifyChangedBase
    {
        private long _stockTakeID;
        [DataMemberAttribute()]
        public long StockTakeID
        {
            get { return _stockTakeID; }
            set
            {
                if (_stockTakeID != value)
                {
                    _stockTakeID = value;
                    RaisePropertyChanged("StockTakeID");
                }
            }
        }

        private StockTakeType _stockTakeType;
        [DataMemberAttribute()]
        public StockTakeType StockTakeType
        {
            get { return _stockTakeType; }
            set
            {
                if (_stockTakeType != value)
                {
                    _stockTakeType = value;
                    RaisePropertyChanged("StockTakeType");
                }
            }
        }

        private long _v_MedProductType;
        [DataMemberAttribute()]
        public long V_MedProductType
        {
            get
            {
                return _v_MedProductType;
            }
            set
            {
                if (_v_MedProductType != value)
                {
                    _v_MedProductType = value;
                    RaisePropertyChanged("V_MedProductType");
                }
            }
        }

        private DateTime _StockTakeDate;
        [DataMemberAttribute()]
        public DateTime StockTakeDate
        {
            get
            {
                return _StockTakeDate;
            }
            set
            {
                if (_StockTakeDate != value)
                {
                    _StockTakeDate = value;
                    RaisePropertyChanged("StockTakeDate");
                }
            }
        }
    }


    public class PriceList : NotifyChangedBase
    {
        private long _priceListID;
        [DataMemberAttribute()]
        public long PriceListID
        {
            get { return _priceListID; }
            set
            {
                if (_priceListID != value)
                {
                    _priceListID = value;
                    RaisePropertyChanged("PriceListID");
                }
            }
        }

        private PriceListType _priceListType;
        [DataMemberAttribute()]
        public PriceListType PriceListType
        {
            get { return _priceListType; }
            set
            {
                if (_priceListType != value)
                {
                    _priceListType = value;
                    RaisePropertyChanged("PriceListType");
                }
            }
        }
    }

    public partial class ReportParameters : NotifyChangedBase
    {
        private int _StaffID;
        [DataMemberAttribute()]
        public int StaffID
        {
            get { return _StaffID; }
            set
            {
                if (_StaffID != value)
                {
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }

        private string _BrandName;
        [DataMemberAttribute()]
        public string BrandName
        {
            get { return _BrandName; }
            set
            {
                if (_BrandName != value)
                {
                    _BrandName = value;
                    RaisePropertyChanged("BrandName");
                }
            }
        }

        private bool _IsDetail;
        [DataMemberAttribute()]
        public bool IsDetail
        {
            get { return _IsDetail; }
            set
            {
                if (_IsDetail != value)
                {
                    _IsDetail = value;
                    RaisePropertyChanged("IsDetail");
                }
            }
        }

        private bool _IsTongKho = false;
        [DataMemberAttribute()]
        public bool IsTongKho
        {
            get { return _IsTongKho; }
            set
            {
                if (_IsTongKho != value)
                {
                    _IsTongKho = value;
                    RaisePropertyChanged("IsTongKho");
                }
            }
        }
        /*▼====: #001*/
        private bool _Check3360 = false;
        [DataMemberAttribute()]
        public bool Check3360
        {
            get { return _Check3360; }
            set
            {
                if (_Check3360 != value)
                {
                    _Check3360 = value;
                    RaisePropertyChanged("Check3360");
                }
            }
        }
        /*▲====: #001*/

        private int _Quarter = 0;
        [DataMemberAttribute()]
        public int Quarter
        {
            get { return _Quarter; }
            set
            {
                if (_Quarter != value)
                {
                    _Quarter = value;
                    RaisePropertyChanged("Quarter");
                }
            }
        }

        private int _Month = 0;
        [DataMemberAttribute()]
        public int Month
        {
            get { return _Month; }
            set
            {
                if (_Month != value)
                {
                    _Month = value;
                    RaisePropertyChanged("Month");
                }
            }
        }

        private int _Year = 0;
        [DataMemberAttribute()]
        public int Year
        {
            get { return _Year; }
            set
            {
                if (_Year != value)
                {
                    _Year = value;
                    RaisePropertyChanged("Year");
                }
            }
        }

        private DateTime? _FromDate = DateTime.Now;
        [DataMemberAttribute()]
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        private DateTime? _ToDate = DateTime.Now;
        [DataMemberAttribute()]
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }

        //---17/10/2017 DPT bao cao 15 ngay

        private DateTime? _To15Date = DateTime.Now;
        [DataMemberAttribute()]
        public DateTime? To15Date
        {
            get { return _To15Date; }
            set
            {
                if (_To15Date != value)
                {
                    _To15Date = value;
                    RaisePropertyChanged("To15Date");
                }
            }
        }
        private long? _SearchID;
        [DataMemberAttribute()]
        public long? SearchID
        {
            get { return _SearchID; }
            set
            {
                if (_SearchID != value)
                {
                    _SearchID = value;
                    RaisePropertyChanged("SearchID");
                }
            }
        }

        //-------------------------
        private int _Flag = 0;
        [DataMemberAttribute()]
        public int Flag
        {
            get { return _Flag; }
            set
            {
                if (_Flag != value)
                {
                    _Flag = value;
                    RaisePropertyChanged("Flag");
                }
            }
        }

        private string _ShowFirst = "";
        [DataMemberAttribute()]
        public string ShowFirst
        {
            get { return _ShowFirst; }
            set
            {
                if (_ShowFirst != value)
                {
                    _ShowFirst = value;
                    RaisePropertyChanged("ShowFirst");
                }
            }
        }

        private string _Show = "";
        [DataMemberAttribute()]
        public string Show
        {
            get { return _Show; }
            set
            {
                if (_Show != value)
                {
                    _Show = value;
                    RaisePropertyChanged("Show");
                }
            }
        }

        private string _StoreName = "";
        [DataMemberAttribute()]
        public string StoreName
        {
            get { return _StoreName; }
            set
            {
                if (_StoreName != value)
                {
                    _StoreName = value;
                    RaisePropertyChanged("StoreName");
                }
            }
        }

        private long? _StoreID;
        [DataMemberAttribute()]
        public long? StoreID
        {
            get { return _StoreID; }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    RaisePropertyChanged("StoreID");
                }
            }
        }

        private int _InwardSource;
        [DataMemberAttribute()]
        public int InwardSource
        {
            get { return _InwardSource; }
            set
            {
                if (_InwardSource != value)
                {
                    _InwardSource = value;
                    RaisePropertyChanged("InwardSource");
                }
            }
        }

        private byte? _OutTo = 0;
        [DataMemberAttribute()]
        public byte? OutTo
        {
            get { return _OutTo; }
            set
            {
                if (_OutTo != value)
                {
                    _OutTo = value;
                    RaisePropertyChanged("OutTo");
                }
            }
        }

        private long? _TypID = 0;
        [DataMemberAttribute()]
        public long? TypID
        {
            get { return _TypID; }
            set
            {
                if (_TypID != value)
                {
                    _TypID = value;
                    RaisePropertyChanged("TypID");
                }
            }
        }

        private bool _HideXNB = false;
        [DataMemberAttribute()]
        public bool HideXNB
        {
            get { return _HideXNB; }
            set
            {
                if (_HideXNB != value)
                {
                    _HideXNB = value;
                    RaisePropertyChanged("HideXNB");
                }
            }
        }

        private bool _HideStore = true;
        [DataMemberAttribute()]
        public bool HideStore
        {
            get { return _HideStore; }
            set
            {
                if (_HideStore != value)
                {
                    _HideStore = value;
                    RaisePropertyChanged("HideStore");
                }
            }
        }


        private bool _HideInwardSource = false;
        [DataMemberAttribute()]
        public bool HideInwardSource
        {
            get { return _HideInwardSource; }
            set
            {
                if (_HideInwardSource != value)
                {
                    _HideInwardSource = value;
                    RaisePropertyChanged("HideInwardSource");
                }
            }
        }

        private long? _RefGenDrugCatID_1 = 0;
        [DataMemberAttribute()]
        public long? RefGenDrugCatID_1
        {
            get { return _RefGenDrugCatID_1; }
            set
            {
                if (_RefGenDrugCatID_1 != value)
                {
                    _RefGenDrugCatID_1 = value;
                    RaisePropertyChanged("RefGenDrugCatID_1");
                }
            }
        }


        private DrugClass _SelectedDrugClass;
        [DataMemberAttribute()]
        public DrugClass SelectedDrugClass
        {
            get { return _SelectedDrugClass; }
            set
            {
                if (_SelectedDrugClass != value)
                {
                    _SelectedDrugClass = value;
                    RaisePropertyChanged("SelectedDrugClass");
                }
            }
        }
        #region theo doi cong no
        private byte? _TypCongNo = 0;
        [DataMemberAttribute()]
        public byte? TypCongNo
        {
            get { return _TypCongNo; }
            set
            {
                if (_TypCongNo != value)
                {
                    _TypCongNo = value;
                    RaisePropertyChanged("TypCongNo");
                }
            }
        }

        private bool _HideTypCongNo = false;
        [DataMemberAttribute()]
        public bool HideTypCongNo
        {
            get { return _HideTypCongNo; }
            set
            {
                if (_HideTypCongNo != value)
                {
                    _HideTypCongNo = value;
                    RaisePropertyChanged("HideTypCongNo");
                }
            }
        }

        private string _ShowTitle = "";
        [DataMemberAttribute()]
        public string ShowTitle
        {
            get { return _ShowTitle; }
            set
            {
                if (_ShowTitle != value)
                {
                    _ShowTitle = value;
                    RaisePropertyChanged("ShowTitle");
                }
            }
        }
        #endregion


        private long _AppointmentID = 0;
        [DataMemberAttribute()]
        public long AppointmentID
        {
            get { return _AppointmentID; }
            set
            {
                if (_AppointmentID != value)
                {
                    _AppointmentID = value;
                    RaisePropertyChanged("AppointmentID");
                }
            }
        }

        private long _PaymentID = 0;
        [DataMemberAttribute()]
        public long PaymentID
        {
            get { return _PaymentID; }
            set
            {
                if (_PaymentID != value)
                {
                    _PaymentID = value;
                    RaisePropertyChanged("PaymentID");
                }
            }
        }


        private int _FindPatient = 0;
        [DataMemberAttribute()]
        public int FindPatient
        {
            get { return _FindPatient; }
            set
            {
                if (_FindPatient != value)
                {
                    _FindPatient = value;
                    RaisePropertyChanged("FindPatient");
                }
            }
        }

        private bool _HideFindPatient = true;
        [DataMemberAttribute()]
        public bool HideFindPatient
        {
            get { return _HideFindPatient; }
            set
            {
                if (_HideFindPatient != value)
                {
                    _HideFindPatient = value;
                    RaisePropertyChanged("HideFindPatient");
                }
            }
        }


        private long? _DeptLocID;
        [DataMemberAttribute()]
        public long? DeptLocID
        {
            get { return _DeptLocID; }
            set
            {
                if (_DeptLocID != value)
                {
                    _DeptLocID = value;
                    RaisePropertyChanged("DeptLocID");
                }
            }
        }

        private long? _DeptID;
        [DataMemberAttribute()]
        public long? DeptID
        {
            get { return _DeptID; }
            set
            {
                if (_DeptID != value)
                {
                    _DeptID = value;
                    RaisePropertyChanged("DeptID");
                }
            }
        }

        private string _DeptName;
        [DataMemberAttribute()]
        public string DeptName
        {
            get { return _DeptName; }
            set
            {
                if (_DeptName != value)
                {
                    _DeptName = value;
                    RaisePropertyChanged("DeptName");
                }
            }
        }

        private ReportName _reportName;
        [DataMemberAttribute()]
        public ReportName reportName
        {
            get { return _reportName; }
            set
            {
                if (_reportName != value)
                {
                    _reportName = value;
                    RaisePropertyChanged("reportName");
                }
            }
        }

        private ReportType _reportType;
        [DataMemberAttribute()]
        public ReportType ReportType
        {
            get { return _reportType; }
            set
            {
                if (_reportType != value)
                {
                    _reportType = value;
                    RaisePropertyChanged("ReportType");
                }
            }
        }

        private PriceList _priceList;
        [DataMemberAttribute()]
        public PriceList PriceList
        {
            get { return _priceList; }
            set
            {
                if (_priceList != value)
                {
                    _priceList = value;
                    RaisePropertyChanged("PriceList");
                }
            }
        }

        private StockTake _stockTake;
        [DataMemberAttribute()]
        public StockTake StockTake
        {
            get { return _stockTake; }
            set
            {
                if (_stockTake != value)
                {
                    _stockTake = value;
                    RaisePropertyChanged("StockTake");
                }
            }
        }


        private RefGenMedProductDetailsSearchCriteria _SearchRefGenMedProduct;
        [DataMemberAttribute()]
        public RefGenMedProductDetailsSearchCriteria SearchRefGenMedProduct
        {
            get { return _SearchRefGenMedProduct; }
            set
            {
                if (_SearchRefGenMedProduct != value)
                {
                    _SearchRefGenMedProduct = value;
                    RaisePropertyChanged("SearchRefGenMedProduct");
                }
            }
        }

        private DepartmentReport _FromDepartment = DepartmentReport.UNKNOWN_DEPT;
        [DataMemberAttribute()]
        public DepartmentReport FromDepartment
        {
            get { return _FromDepartment; }
            set
            {
                if (_FromDepartment != value)
                {
                    _FromDepartment = value;
                    RaisePropertyChanged("FromDepartment");
                }
            }
        }

        private long _V_MedProductType;
        [DataMemberAttribute()]
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    RaisePropertyChanged("V_MedProductType");
                }
            }
        }

        private bool _NotTreatedAsInPt;
        [DataMemberAttribute()]
        public bool NotTreatedAsInPt
        {
            get { return _NotTreatedAsInPt; }
            set
            {
                if (_NotTreatedAsInPt != value)
                {
                    _NotTreatedAsInPt = value;
                    RaisePropertyChanged("NotTreatedAsInPt");
                }
            }
        }

        private long _EstimatePoID = 0;
        [DataMemberAttribute()]
        public long EstimatePoID
        {
            get { return _EstimatePoID; }
            set
            {
                if (_EstimatePoID != value)
                {
                    _EstimatePoID = value;
                    RaisePropertyChanged("EstimatePoID");
                }
            }
        }

        private long _HIReportID = 0;
        [DataMemberAttribute()]
        public long HIReportID
        {
            get { return _HIReportID; }
            set
            {
                if (_HIReportID != value)
                {
                    _HIReportID = value;
                    RaisePropertyChanged("HIReportID");
                }
            }
        }

        private Lookup _V_PaymentMode;
        [DataMemberAttribute()]
        public Lookup V_PaymentMode
        {
            get { return _V_PaymentMode; }
            set
            {
                if (_V_PaymentMode != value)
                {
                    _V_PaymentMode = value;
                    RaisePropertyChanged("V_PaymentMode");
                }
            }
        }

        private ConsultingDiagnosysSearchCriteria _ConsultingDiagnosysSearchCriteria;
        [DataMemberAttribute()]
        public ConsultingDiagnosysSearchCriteria ConsultingDiagnosysSearchCriteria
        {
            get { return _ConsultingDiagnosysSearchCriteria; }
            set
            {
                if (_ConsultingDiagnosysSearchCriteria != value)
                {
                    _ConsultingDiagnosysSearchCriteria = value;
                    RaisePropertyChanged("ConsultingDiagnosysSearchCriteria");
                }
            }
        }
        private bool _IsFullDetails = false;
        [DataMemberAttribute]
        public bool IsFullDetails
        {
            get => _IsFullDetails; set
            {
                _IsFullDetails = value;
                RaisePropertyChanged("IsFullDetails");
            }
        }

        private long _DrugID = 0;
        [DataMemberAttribute()]
        public long DrugID
        {
            get { return _DrugID; }
            set
            {
                if (_DrugID != value)
                {
                    _DrugID = value;
                    RaisePropertyChanged("DrugID");
                }
            }
        }

        private int _TypeKCBBD = 1;
        [DataMemberAttribute()]
        public int TypeKCBBD
        {
            get { return _TypeKCBBD; }
            set
            {
                if (_TypeKCBBD != value)
                {
                    _TypeKCBBD = value;
                    RaisePropertyChanged("TypeKCBBD");
                }
            }
        }

        private long? _BidID;
        [DataMemberAttribute]
        public long? BidID
        {
            get
            {
                return _BidID;
            }
            set
            {
                _BidID = value;
            }
        }

        private bool _IsExportToCSV = false;
        [DataMemberAttribute]
        public bool IsExportToCSV
        {
            get
            {
                return _IsExportToCSV;
            }
            set
            {
                if (_IsExportToCSV == value)
                {
                    return;
                }
                _IsExportToCSV = value;
                RaisePropertyChanged("IsExportToCSV");
            }
        }

        private long _Status;
        [DataMemberAttribute]
        public long Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (_Status == value)
                {
                    return;
                }
                _Status = value;
                RaisePropertyChanged("Status");
            }
        }

        private bool _HasDischarge = true;
        [DataMemberAttribute]
        public bool HasDischarge
        {
            get
            {
                return _HasDischarge;
            }
            set
            {
                if (_HasDischarge == value)
                {
                    return;
                }
                _HasDischarge = value;
                RaisePropertyChanged("HasDischarge");
            }
        }

        private bool _IsNewForm = false;
        [DataMemberAttribute]
        public bool IsNewForm
        {
            get
            {
                return _IsNewForm;
            }
            set
            {
                if (_IsNewForm == value)
                {
                    return;
                }
                _IsNewForm = value;
                RaisePropertyChanged("IsNewForm");
            }
        }
        //▼===== #005
        private long _V_79AExportType = 84800;
        [DataMemberAttribute]
        public long V_79AExportType
        {
            get
            {
                return _V_79AExportType;
            }
            set
            {
                if (_V_79AExportType == value)
                {
                    return;
                }
                _V_79AExportType = value;
                RaisePropertyChanged("V_79AExportType");
            }
        }
        //▲===== #005
        private decimal _AverageTime = 0;
        [DataMemberAttribute]
        public decimal AverageTime
        {
            get
            {
                return _AverageTime;
            }
            set
            {
                if (_AverageTime == value)
                {
                    return;
                }
                _AverageTime = value;
                RaisePropertyChanged("AverageTime");
            }
        }

        private decimal _AverageAmount = 0;
        [DataMemberAttribute]
        public decimal AverageAmount
        {
            get
            {
                return _AverageAmount;
            }
            set
            {
                if (_AverageAmount == value)
                {
                    return;
                }
                _AverageAmount = value;
                RaisePropertyChanged("AverageAmount");
            }
        }

        private byte _RegistrationStatus = 0;
        [DataMemberAttribute]
        public byte RegistrationStatus
        {
            get
            {
                return _RegistrationStatus;
            }
            set
            {
                if (_RegistrationStatus == value)
                {
                    return;
                }
                _RegistrationStatus = value;
                RaisePropertyChanged("RegistrationStatus");
            }
        }
        //▼===== #005
        private long? _StoreType = 0;
        [DataMemberAttribute]
        public long? StoreType
        {
            get
            {
                return _StoreType;
            }
            set
            {
                if (_StoreType == value)
                {
                    return;
                }
                _StoreType = value;
                RaisePropertyChanged("StoreType");
            }
        }
        private long _SelectedDrugDeptProductGroupReportType = 0;
        public long SelectedDrugDeptProductGroupReportType
        {
            get
            {
                return _SelectedDrugDeptProductGroupReportType;
            }
            set
            {
                if (_SelectedDrugDeptProductGroupReportType != value)
                {
                    _SelectedDrugDeptProductGroupReportType = value;
                    RaisePropertyChanged("SelectedDrugDeptProductGroupReportType");
                }
            }
            //▲===== #005
        }
        private bool _Settlement ;
        [DataMemberAttribute]
        public bool Settlement
        {
            get
            {
                return _Settlement;
            }
            set
            {
                if (_Settlement == value)
                {
                    return;
                }
                _Settlement = value;
                RaisePropertyChanged("Settlement");
            }
        }

        //▼===== #017
        private bool _HidePCLStatus = true;
        [DataMemberAttribute()]
        public bool HidePCLStatus
        {
            get { return _HidePCLStatus; }
            set
            {
                if (_HidePCLStatus != value)
                {
                    _HidePCLStatus = value;
                    RaisePropertyChanged("HidePCLStatus");
                }
            }
        }
        //▲===== #017

        //▼===== #021
        private bool _ShowDischarge = false;
        [DataMemberAttribute()]
        public bool ShowDischarge
        {
            get { return _ShowDischarge; }
            set
            {
                if (_ShowDischarge != value)
                {
                    _ShowDischarge = value;
                    RaisePropertyChanged("ShowDischarge");
                }
            }
        }
        //▲===== #021


        //▼===== #031
        private Lookup _V_OutPtTreatmentStatus;
        [DataMemberAttribute()]
        public Lookup V_OutPtTreatmentStatus
        {
            get { return _V_OutPtTreatmentStatus; }
            set
            {
                if (_V_OutPtTreatmentStatus != value)
                {
                    _V_OutPtTreatmentStatus = value;
                    RaisePropertyChanged("V_OutPtTreatmentStatus");
                }
            }
        }

        private long _OutpatientTreatmentTypeID;
        public long OutpatientTreatmentTypeID
        {
            get
            {
                return _OutpatientTreatmentTypeID;
            }
            set
            {
                if (_OutpatientTreatmentTypeID != value)
                {
                    _OutpatientTreatmentTypeID = value;
                    RaisePropertyChanged("OutpatientTreatmentTypeID");
                }
            }
        }
        //▲===== #031

        //▼===== #037
        private Lookup _V_HealthRecordsType;
        [DataMemberAttribute()]
        public Lookup V_HealthRecordsType
        {
            get { return _V_HealthRecordsType; }
            set
            {
                if (_V_HealthRecordsType != value)
                {
                    _V_HealthRecordsType = value;
                    RaisePropertyChanged("V_HealthRecordsType");
                }
            }
        }
        //▲===== #037

        //▼==== #040
        private int _YNOutPtTreatmentCode;
        public int YNOutPtTreatmentCode
        {
            get
            {
                return _YNOutPtTreatmentCode;
            }
            set
            {
                if (_YNOutPtTreatmentCode != value)
                {
                    _YNOutPtTreatmentCode = value;
                    RaisePropertyChanged("YNOutPtTreatmentCode");
                }
            }
        }

        private int _YNOutPtTreatmentType;
        public int YNOutPtTreatmentType
        {
            get
            {
                return _YNOutPtTreatmentType;
            }
            set
            {
                if (_YNOutPtTreatmentType != value)
                {
                    _YNOutPtTreatmentType = value;
                    RaisePropertyChanged("YNOutPtTreatmentType");
                }
            }
        }

        private int _YNOutPtTreatmentProgram;
        public int YNOutPtTreatmentProgram
        {
            get
            {
                return _YNOutPtTreatmentProgram;
            }
            set
            {
                if (_YNOutPtTreatmentProgram != value)
                {
                    _YNOutPtTreatmentProgram = value;
                    RaisePropertyChanged("YNOutPtTreatmentProgram");
                }
            }
        }

        private int _YNOutPtTreatmentFinal;
        public int YNOutPtTreatmentFinal
        {
            get
            {
                return _YNOutPtTreatmentFinal;
            }
            set
            {
                if (_YNOutPtTreatmentFinal != value)
                {
                    _YNOutPtTreatmentFinal = value;
                    RaisePropertyChanged("YNOutPtTreatmentFinal");
                }
            }
        }
        //▲===== #040

        //▼==== #041
        private bool _ShowFindInfectionControl = false;
        public bool ShowFindInfectionControl
        {
            get
            {
                return _ShowFindInfectionControl;
            }
            set
            {
                if (_ShowFindInfectionControl != value)
                {
                    _ShowFindInfectionControl = value;
                    RaisePropertyChanged("ShowFindInfectionControl");
                }
            }
        }
        //▲==== #041

        //▲===== #040
        private int _PatientType = -1;
        [DataMemberAttribute()]
        public int PatientType
        {
            get { return _PatientType; }
            set
            {
                if (_PatientType != value)
                {
                    _PatientType = value;
                    RaisePropertyChanged("PatientType");
                }
            }
        }
        //▲===== #040

        //▼===== #054
        private Lookup _V_ExaminationProcess;
        [DataMemberAttribute()]
        public Lookup V_ExaminationProcess
        {
            get { return _V_ExaminationProcess; }
            set
            {
                if (_V_ExaminationProcess != value)
                {
                    _V_ExaminationProcess = value;
                    RaisePropertyChanged("V_ExaminationProcess");
                }
            }
        }
        //▲===== #054

        //▼===== #055
        private Lookup _V_HIReportStatus;
        [DataMemberAttribute()]
        public Lookup V_HIReportStatus
        {
            get { return _V_HIReportStatus; }
            set
            {
                if (_V_HIReportStatus != value)
                {
                    _V_HIReportStatus = value;
                    RaisePropertyChanged("V_HIReportStatus");
                }
            }
        }
        //▲===== #055
    }
}
