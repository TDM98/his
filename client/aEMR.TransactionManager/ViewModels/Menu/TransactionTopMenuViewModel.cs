using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts.Configuration;
using Castle.Windsor;
/*
* 20190426 #001 TNHX: [BM0006797] add report BCChiTietDVCLS
* 20190427 #002 TNHX: [BM0006799] add report BCDoanhThuThuocBHYTNgTru
* 20190603 #003 TNHX: [BM0011782] App new view for report NXTheoMucDich
* 20190611 #004 TNHX: [BM0011776] Create report BCChiTietNhapTuNCC
* 20190611 #005 TNHX: [BM0011776] Create report BCBNHoanTra
* 20190618 #006 TNHX: [BM0011845] Create report KHTH_TK_TinhHinhKCB
* 20190620 #007 TNHX: [BM0011868] Create report KT_TinhHinhHoatDongDV
* 20190625 #008 TNHX: [BM0011883] Create report BCHangTonNhieu + BCHangCanDate
* 20190704 #009 TNHX: [BM0011776] Create report BCTrasau
* 20190709 #010 TNHX: [BM0006694] Create report TEMP21_NEW + Add title
* 20190827 #011 TNHX: [BM0013276] Create report InOut of DrugDept + ClinicDept for Accountant
* 20191003 #012 TNHX: [BM 0013292] Update report BCChiTietNhapTuNCC
* 20191125 #013 TNHX: [BM 0013292] Create report BCTheKho for Accountant
* 20200115 #014 TNHX: [BM ] Create report BAOCAO_MIENGIAM_NOITRU_TV
* 20200218 #015 TNHX: [BM ] Create report CardStoreage for HIStore
* 20200324 #016 TNHX: [BM ] Create report DLS_BCXuatThuocBNKhoaPhong
* 20200610 #017 TNHX: [BM ] Create report BCToaThuocHangNgayDLS + BCThuocYCuDoDangCuoiKy
* 20200811 #018 TNHX: [BM ] Create report TkChiDinhThuocTPKhoaNoiTru
* 20201213 #019 TNHX: [BM ] Thêm báo cáo cho KHTH
* 20210315 #020 BAOLQ:   Task 237: tạo màn hình kiểm duyệt hồ sơ cho kê hoạch tổng hợp
* 20210908 #021 QTD:  Thêm BC NXT toàn khoa DƯỢC
* 20210910 #022 QTD: Thêm BC kho tổng hợp
* 20211224 #023 TNHX: 803 Thêm báo cao bn covid cho khoa dược/ kế toán tổng hợp
* 20220330 #024 DatTB:  Thêm Báo cáo quản lý kiểm duyệt hồ sơ KHTH
* 20220407 #025 DatTB:  Thêm Báo cáo kiểm tra lịch sử KCB DLS
* 20220409 #026 DatTB:  Thêm Báo cáo thông tin danh mục thuốc DLS
* 20220516 #027 DatTB: Báo cáo tình hình thực hiện CLS - Đã thực hiện
* 20220228 #028 QTD:  Bổ sung phân quyền Menu còn thiếu
* 20220527 #029 BLQ:  Thêm Báo cáo tổng hợp thu tiền viện phí ngoại trú
* 20220528 #030 BLQ:  Thêm Báo cáo tổng hợp hủy hoàn tiền viện phí ngoại trú
* 20220528 #031 BLQ:  Thêm Báo cáo tổng hợp thu tiền tạm ứng ngoại trú
* 20220528 #032 BLQ:  Thêm Báo cáo tổng hợp hủy hoàn tiền tạm ứng ngoại trú
* 20220523 #033 DatTB: Báo cáo Doanh thu theo Khoa
* 20220525 #034 DatTB: Thêm Báo Cáo Hoãn/Miễn Tạm Ứng Nội Trú
* 20220807 #035 DatTB: Báo cáo thống kê số lượng hồ sơ điều trị ngoại trú
* 20220808 #036 QTD: Thêm báo cáo thống kê danh sách BN ĐTNT
* 20220810 #037 QTD: Dời Menu Quản lý hồ sơ từ Module QL phòng khám sang
* 20220814 #038 QTD: Quản lý danh mục Dãy, Kệ, Ngăn - Quản lý mượn/trả hồ sơ
* 20221029 #039 QTD: Thêm quyền các nút màn hình Kiểm duyệt hồ sơ
* 20221128 #040 DatTB: Thêm báo cáo thao tác người dùng
* 20221201 #041 DatTB: Thêm báo cáo thống kê hồ sơ ĐTNT
* 20230207 #042 DatTB: Thêm báo cáo KSNK
* 20230111 #043 DatTB: Thêm ComboBox đối tượng khám bệnh
* 20230114 #044 DatTB: Clone báo cáo chi tiết DC&CLS
*/
namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ITransactionTopMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TransactionTopMenuViewModel : Conductor<object>, ITransactionTopMenu
    {
        [ImportingConstructor]
        public TransactionTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
            AuthorityOperation();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            authorization();
            AuthorityOperation();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                CheckValidSystemMenu();
                return;
            }
            else
            {
                #region Authorization of Báo cáo phân tích tài chính

                bTemp20NgoaiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp20NgoaiTru);
                bTemp20NoiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp20NoiTru);
                bTemp21NgoaiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp21NgoaiTru);
                bTemp21NoiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp21NoiTru);
                bTemp25aInsurance = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp25aInsurance);
                bTemp26a = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp26a);
                bTemp19 = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mTemp19);
                bTemp20NoiTruNew = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp20NoiTruNew);
                bTemp21New = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp21New);
                bTemp79a = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mTemp79a);
                bTemp79aTraThuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mTemp79aTraThuoc);
                bTemp80a = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp80a);

                bTemp01_BV_NgoaiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp01_BV_NgoaiTru);
                bTemp02_BV_NoiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp02_BV_NoiTru);
                bTemp02_ChiTietVienPhi_PK = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp02_ChiTietVienPhi_PK);
                bTemp04_ChiTietVienPhi = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp04_ChiTietVienPhi);
                bTemp02_HoatDongKB = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp02_HoatDongKB);
                bTemp06_CanLamSan = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp06_CanLamSan);
                bTemp07_DuocBV = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp07_DuocBV);
                bChiDaoTuyen = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mChiDaoTuyen);
                bBC15Day = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mBC15Day);
                bSXN = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mSXN);
                bNghienCuuKhoaHoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mNghienCuuKhoaHoc);
                bTempThongKeDoanhThu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTempThongKeDoanhThu);
                bTempTongHopDoanhThu_PK = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTempTongHopDoanhThu_PK);
                bTempTongHopDoanhThu_NTM = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTempTongHopDoanhThu_NTM);
                bBaoCaoBHYT = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mBaoCaoBHYT);
                bBaoCaoVienPhiTrai = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mBaoCaoVienPhiTrai);
                bBaoCaoVienPhiKTC = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mBaoCaoVienPhiKTC);
                bBaoCaoVTYT_KTC = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mBaoCaoVTYT_KTC);



                //▼====: #024
                bBaoCaoVPMTreEm = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mBaoCaoVPMTreEm);
                bInOutValueReport = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mInOutValueReport);
                bCreateFastReport = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mCreateFastReport);
                bCreateDQGReport = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mCreateDQGReport);
                bExportExcel4210 = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mExportExcel4210);
                bExportExcelPCLWaitResult = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mExportExcelPCLWaitResult);
                bOutwardDrugsByStaffStatistic = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mOutwardDrugsByStaffStatistic);
                bOutwardDrugsByStaffStatisticDetails = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mOutwardDrugsByStaffStatisticDetails);
                //KMx: Chỉ cần 1 Function của Group "Báo cáo" được cấp quyền sử dụng thì Group "Báo cáo" sẽ hiện ra.
                //mReport = bTemp20NgoaiTru || bTemp20NoiTru || bTemp19 || bTemp20NoiTruNew || bTemp21NgoaiTru || bTemp21NoiTru || bTemp25aInsurance || bTemp26a
                //            || bTemp79a || bTemp79aTraThuoc || bTemp80a || bTemp02_HoatDongKB || bTemp06_CanLamSan || bTemp07_DuocBV
                //          || bTemp01_BV_NgoaiTru || bTemp02_BV_NoiTru || bTemp02_ChiTietVienPhi_PK || bTemp04_ChiTietVienPhi
                //          || bTempThongKeDoanhThu || bTempTongHopDoanhThu_PK || bTempTongHopDoanhThu_NTM || bBaoCaoBHYT || bBaoCaoVienPhiTrai || bBaoCaoVienPhiKTC || bBaoCaoVTYT_KTC
                //          || bBaoCaoVPMTreEm || bInOutValueReport || bCreateFastReport || bCreateDQGReport || bExportExcel4210 || bExportExcelPCLWaitResult || bOutwardDrugsByStaffStatistic
                //          || bOutwardDrugsByStaffStatisticDetails;
                //▲====: #024

                #endregion

                #region Authorization of Price Management

                bMedServiceItemPriceList_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                                (int)eTransaction_Management.mBangGiaDichVu);
                bPCLExamTypePriceList_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                                (int)eTransaction_Management.mBangGiaCLS);
                bRefDepartmentReqCashAdv_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                                (int)eTransaction_Management.mGiaTamUng);

                mPriceManagement = bMedServiceItemPriceList_Mgnt || bPCLExamTypePriceList_Mgnt || bRefDepartmentReqCashAdv_Mgnt;
                #endregion

                #region Authorization of Y Vụ

                bThongKeNgoaiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mYVu_Management,
                                  (int)eYVu_Management.mThongKeNgoaiTru);
                bBaoCaoNhanhKhuKhamBenh = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mYVu_Management,
                                  (int)eYVu_Management.mBaoCaoNhanhKhuKhamBenh);
                bReportInPatient = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mYVu_Management,
                                  (int)eYVu_Management.mReportInPatient);
                bReportGeneralTemp02 = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mYVu_Management,
                                    (int)eYVu_Management.mReportGeneralTemp02);
                mYVuReport = bThongKeNgoaiTru || bBaoCaoNhanhKhuKhamBenh || bReportInPatient || bReportGeneralTemp02;
                #endregion
                //▼====: #028

                //Thống kê
                bInPtAdmDisStatistics = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mInPtAdmDisStatistics);
                bRptDrugMedDept = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mRptDrugMedDept);
                bSoXetNghiem = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mSoXetNghiem);
                gThongKeBV = bInPtAdmDisStatistics || bRptDrugMedDept || bBC15Day || bSoXetNghiem;

                //Kế Toán Tổng Hợp
                IsRevenueAccountant = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mIsRevenueAccountant);
                bIsWareHouseAccountant = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mIsWareHouseAccountant);
                bCreateEInvoice = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mCreateEInvoice);
                bCreateOutPtTransactionFinalization = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mCreateOutPtTransactionFinalization);
                IsAccountant = IsRevenueAccountant || bIsWareHouseAccountant || bCreateEInvoice || bCreateOutPtTransactionFinalization;

                //Kê Hoạch TH

                bThongTu372019 = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mThongTu372019);
                bKHTH_BCTinhHinhDichLay = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mKHTH_BCTinhHinhDichLay);
                bKHTH_TK_TinhHinhKCB = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mKHTH_TK_TinhHinhKCB);
                bBCCongTacKCB = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCCongTacKCB);
                bTinhHinhHoatDongCLS_KHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mTinhHinhHoatDongCLS_KHTH);
                bTinhHinhHoatDongDV_KHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mTinhHinhHoatDongDV_KHTH);
                bBCChiTietDVCLS_KHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCChiTietDVCLS_KHTH);
                bBaoCaoDVDaThucHien = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBaoCaoDVDaThucHien);
                bBC_DS_DVKyThuatTrenHIS_KHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBC_DS_DVKyThuatTrenHIS_KHTH);
                bRptPtAreTreated = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mRptPtAreTreated);
                bBCDsachBNXuatVien_KHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCDsachBNXuatVien_KHTH);
                bBCDsachBsiChiDinhNhapVien_KHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCDsachBsiChiDinhNhapVien_KHTH);
                bPatientDecease = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mPatientDecease);
                bBCBenhNhanPTT = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCBenhNhanPTT);
                bYVu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mYVu);
                bBCChuyenTuyen = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCChuyenTuyen);
                bCheckMedicalFileForKHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mCheckMedicalFileForKHTH);
                bThongTu56 = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mThongTu56);
                bBC_DMKyThuatMoi_KHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBC_DMKyThuatMoi_KHTH);
                bBCCongSuatGiuongBenh = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCCongSuatGiuongBenh);
                bBCGiaoBanTheoKhoaLS_KHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCGiaoBanTheoKhoaLS_KHTH);
                bBCSuDungToanBV = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCSuDungToanBV);
                bBCGiaoBanKHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCGiaoBanKHTH);
                bBCQuanLyKiemDuyetHoSo_KHTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCQuanLyKiemDuyetHoSo_KHTH);
                //▼====: #036
                bBCDanhSachBNDTNT = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCDanhSachBNDTNT);
                //▲====: #036
                gKeHoachTH = bThongTu372019 || bKHTH_BCTinhHinhDichLay || bKHTH_TK_TinhHinhKCB || bBCCongTacKCB || bTinhHinhHoatDongCLS_KHTH || bTinhHinhHoatDongDV_KHTH
                    || bBCChiTietDVCLS_KHTH || bBaoCaoDVDaThucHien || bBC_DS_DVKyThuatTrenHIS_KHTH || bRptPtAreTreated || bBCDsachBNXuatVien_KHTH || bBCDsachBsiChiDinhNhapVien_KHTH
                    || bPatientDecease || bBCBenhNhanPTT || bYVu || bBCChuyenTuyen || bCheckMedicalFileForKHTH || bThongTu56 || bBC_DMKyThuatMoi_KHTH || bBCCongSuatGiuongBenh || bBCGiaoBanTheoKhoaLS_KHTH
                    || bBCGiaoBanKHTH || bBCSuDungToanBV || bBCDanhSachBNDTNT;

                //Dược Lâm Sàng
                bDLS_BCQLXuatThuocKhoaPhong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mDLS_BCQLXuatThuocKhoaPhong);
                bDLS_BCToaThuocHangNgay = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mDLS_BCToaThuocHangNgay);
                bDLS_BCKiemTraLichSuKCB = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mDLS_BCKiemTraLichSuKCB);
                bDLS_BCThongTinDanhMucThuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mDLS_BCThongTinDanhMucThuoc);
                gDuocLS = bDLS_BCToaThuocHangNgay || bDLS_BCQLXuatThuocKhoaPhong || bDLS_BCKiemTraLichSuKCB || bDLS_BCThongTinDanhMucThuoc;

                //Kế Hoạch TH
                mReport = bTemp20NgoaiTru || bTemp20NoiTru || bTemp19 || bTemp20NoiTruNew || bTemp21NgoaiTru || bTemp21NoiTru || bTemp25aInsurance || bTemp26a
                            || bTemp79a || bTemp79aTraThuoc || bTemp80a || bTemp02_HoatDongKB || bTemp06_CanLamSan || bTemp07_DuocBV
                          || bTemp01_BV_NgoaiTru || bTemp02_BV_NoiTru || bTemp02_ChiTietVienPhi_PK || bTemp04_ChiTietVienPhi
                          || bTempThongKeDoanhThu || bTempTongHopDoanhThu_PK || bTempTongHopDoanhThu_NTM || bBaoCaoBHYT || bBaoCaoVienPhiTrai || bBaoCaoVienPhiKTC || bBaoCaoVTYT_KTC
                          || bBaoCaoVPMTreEm || bInOutValueReport || bCreateFastReport || bCreateDQGReport || bExportExcel4210 || bExportExcelPCLWaitResult || bOutwardDrugsByStaffStatistic
                          || bOutwardDrugsByStaffStatisticDetails;
                //▲====: #028

                //Quản lý hồ sơ
                //▼====: #037
                bFileManager = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mFileManagement);
                //▲====: #037

                //▼==== #040
                mUserManipulation = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mUserAccount,
                    (int)eUserAccount.mUserManipulation);

                mManagePersonnelReport = mUserManipulation;
                //▲==== #040

                //▼==== #041
                mBCThongKeHoSoDTNT = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBCThongKeHoSoDTNT);
                //▲==== #041

                //▼==== #042
                mBaoCaoKSNK = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                (int)eTransaction_Management.mBaoCaoKSNK);
                //▲==== #042

                CheckValidSystemMenu();
            }

        }

        private void CheckValidSystemMenu()
        {
            if (Globals.IseHMSSystem)
            {
                mPriceManagement = false;
                mYVuReport = false;
                bTemp20NgoaiTru = false;
                bTemp20NoiTru = false;
                bTemp25aInsurance = false;
                bTemp26a = false;
                bTemp19 = false;
                bTemp20NoiTruNew = false;
                bTemp21New = false;
                bTemp79a = false;
                bTemp79aTraThuoc = false;
                bTemp80a = false;
                bTemp01_BV_NgoaiTru = false;
                bTemp02_BV_NoiTru = false;
                bTemp02_ChiTietVienPhi_PK = false;
                bTemp04_ChiTietVienPhi = false;
                bTempThongKeDoanhThu = false;
                bTempTongHopDoanhThu_PK = false;
                bTempTongHopDoanhThu_NTM = false;
                bBaoCaoVienPhiTrai = false;
                bBaoCaoVienPhiKTC = false;
                bBaoCaoVTYT_KTC = false;
                bBaoCaoVTYT_KTC = false;
                bTemp21NgoaiTru = false;
                bTemp21NoiTru = false;
            }
        }
        #region Properties of Báo cáo phân tích tài chính

        private bool _mReport = true;
        private bool _bTemp20NgoaiTru = true;
        private bool _bTemp20NoiTru = true;
        private bool _bTemp21NgoaiTru = true;
        private bool _bTemp21NoiTru = true;
        private bool _bTemp25aInsurance = true;
        private bool _bTemp26a = true;
        private bool _bTemp19 = true;
        private bool _bTemp20NoiTruNew = true;
        private bool _bTemp21New = true;
        private bool _bTemp79a = true;
        private bool _bTemp79aTraThuoc = true;
        private bool _bTemp80a = true;
        private bool _bTemp01_BV_NgoaiTru = true;
        private bool _bTemp02_BV_NoiTru = true;
        private bool _bTemp02_ChiTietVienPhi_PK = true;
        private bool _bTemp04_ChiTietVienPhi = true;
        private bool _bTemp02_HoatDongKB = true;
        private bool _bTemp06_CanLamSan = true;
        private bool _bTemp07_DuocBV = true;
        private bool _bChiDaoTuyen = true;
        private bool _bBC15Day = true;
        private bool _bSXN = true;
        private bool _bNghienCuuKhoaHoc = true;
        private bool _bTempThongKeDoanhThu = true;
        private bool _bTempTongHopDoanhThu_PK = true;
        private bool _bTempTongHopDoanhThu_NTM = true;
        private bool _bBaoCaoBHYT = true;
        private bool _bBaoCaoVienPhiTrai = true;
        private bool _bBaoCaoVienPhiKTC = true;
        private bool _bBaoCaoVTYT_KTC = true;
        private bool _IsRevenueAccountant = true;
        private bool _IsAccountant = true;

        //▼====: #011
        public bool IsAccountant
        {
            get
            {
                return _IsAccountant;
            }
            set
            {
                if (_IsAccountant == value)
                    return;
                _IsAccountant = value;
                NotifyOfPropertyChange(() => IsAccountant);
            }
        }

        public bool IsRevenueAccountant
        {
            get
            {
                return _IsRevenueAccountant;
            }
            set
            {
                if (_IsRevenueAccountant == value)
                    return;
                _IsRevenueAccountant = value;
                NotifyOfPropertyChange(() => IsRevenueAccountant);
            }
        }
        //▲====: #011

        public bool bTemp02_HoatDongKB
        {
            get
            {
                return _bTemp02_HoatDongKB;
            }
            set
            {
                if (_bTemp02_HoatDongKB == value)
                    return;
                _bTemp02_HoatDongKB = value;
                NotifyOfPropertyChange(() => _bTemp02_HoatDongKB);
            }
        }
        public bool mReport
        {
            get
            {
                return _mReport;
            }
            set
            {
                if (_mReport == value)
                    return;
                _mReport = value;
                NotifyOfPropertyChange(() => mReport);
            }
        }

        public bool bTemp20NgoaiTru
        {
            get
            {
                return _bTemp20NgoaiTru;
            }
            set
            {
                if (_bTemp20NgoaiTru == value)
                    return;
                _bTemp20NgoaiTru = value;
            }
        }

        public bool bTemp20NoiTru
        {
            get
            {
                return _bTemp20NoiTru;
            }
            set
            {
                if (_bTemp20NoiTru == value)
                    return;
                _bTemp20NoiTru = value;
            }
        }

        public bool bTemp21NgoaiTru
        {
            get
            {
                return _bTemp21NgoaiTru;
            }
            set
            {
                if (_bTemp21NgoaiTru == value)
                    return;
                _bTemp21NgoaiTru = value;
            }
        }

        public bool bTemp21NoiTru
        {
            get
            {
                return _bTemp21NoiTru;
            }
            set
            {
                if (_bTemp21NoiTru == value)
                    return;
                _bTemp21NoiTru = value;
            }
        }

        public bool bTemp25aInsurance
        {
            get
            {
                return _bTemp25aInsurance;
            }
            set
            {
                if (_bTemp25aInsurance == value)
                    return;
                _bTemp25aInsurance = value;
            }
        }

        public bool bTemp26a
        {
            get
            {
                return _bTemp26a;
            }
            set
            {
                if (_bTemp26a == value)
                    return;
                _bTemp26a = value;
            }
        }

        public bool bTemp19
        {
            get
            {
                return _bTemp19;
            }
            set
            {
                if (_bTemp19 == value)
                    return;
                _bTemp19 = value;
            }
        }

        public bool bTemp20NoiTruNew
        {
            get
            {
                return _bTemp20NoiTruNew;
            }
            set
            {
                if (_bTemp20NoiTruNew == value)
                    return;
                _bTemp20NoiTruNew = value;
            }
        }

        public bool bTemp21New
        {
            get
            {
                return _bTemp21New;
            }
            set
            {
                if (_bTemp21New == value)
                    return;
                _bTemp21New = value;
            }
        }

        public bool bTemp79a
        {
            get
            {
                return _bTemp79a;
            }
            set
            {
                if (_bTemp79a == value)
                    return;
                _bTemp79a = value;
            }
        }

        public bool bTemp79aTraThuoc
        {
            get
            {
                return _bTemp79aTraThuoc;
            }
            set
            {
                if (_bTemp79aTraThuoc == value)
                    return;
                _bTemp79aTraThuoc = value;
            }
        }

        public bool bTemp80a
        {
            get
            {
                return _bTemp80a;
            }
            set
            {
                if (_bTemp80a == value)
                    return;
                _bTemp80a = value;
            }
        }

        public bool bTemp01_BV_NgoaiTru
        {
            get
            {
                return _bTemp01_BV_NgoaiTru;
            }
            set
            {
                if (_bTemp01_BV_NgoaiTru == value)
                    return;
                _bTemp01_BV_NgoaiTru = value;
            }
        }

        public bool bTemp02_BV_NoiTru
        {
            get
            {
                return _bTemp02_BV_NoiTru;
            }
            set
            {
                if (_bTemp02_BV_NoiTru == value)
                    return;
                _bTemp02_BV_NoiTru = value;
            }
        }

        public bool bTemp02_ChiTietVienPhi_PK
        {
            get
            {
                return _bTemp02_ChiTietVienPhi_PK;
            }
            set
            {
                if (_bTemp02_ChiTietVienPhi_PK == value)
                    return;
                _bTemp02_ChiTietVienPhi_PK = value;
            }
        }

        public bool bTemp04_ChiTietVienPhi
        {
            get
            {
                return _bTemp04_ChiTietVienPhi;
            }
            set
            {
                if (_bTemp04_ChiTietVienPhi == value)
                    return;
                _bTemp04_ChiTietVienPhi = value;
            }
        }
        public bool bTemp06_CanLamSan
        {
            get
            {
                return _bTemp06_CanLamSan;
            }
            set
            {
                if (_bTemp06_CanLamSan == value)
                    return;
                _bTemp06_CanLamSan = value;
            }
        }
        public bool bTemp07_DuocBV
        {
            get
            {
                return _bTemp07_DuocBV;
            }
            set
            {
                if (_bTemp07_DuocBV == value)
                    return;
                _bTemp07_DuocBV = value;
            }
        }
        public bool bBC15Day
        {
            get
            {
                return _bBC15Day;
            }
            set
            {
                if (_bBC15Day == value)
                    return;
                _bBC15Day = value;
            }
        }
        public bool bSXN
        {
            get
            {
                return _bSXN;
            }
            set
            {
                if (_bSXN == value)
                    return;
                _bSXN = value;
            }
        }
        public bool bChiDaoTuyen
        {
            get
            {
                return _bChiDaoTuyen;
            }
            set
            {
                if (_bChiDaoTuyen == value)
                    return;
                _bChiDaoTuyen = value;
            }
        }
        public bool bNghienCuuKhoaHoc
        {
            get
            {
                return _bNghienCuuKhoaHoc;
            }
            set
            {
                if (_bNghienCuuKhoaHoc == value)
                    return;
                _bNghienCuuKhoaHoc = value;
            }
        }
        public bool bTempThongKeDoanhThu
        {
            get
            {
                return _bTempThongKeDoanhThu;
            }
            set
            {
                if (_bTempThongKeDoanhThu == value)
                    return;
                _bTempThongKeDoanhThu = value;
            }
        }

        public bool bTempTongHopDoanhThu_PK
        {
            get
            {
                return _bTempTongHopDoanhThu_PK;
            }
            set
            {
                if (_bTempTongHopDoanhThu_PK == value)
                    return;
                _bTempTongHopDoanhThu_PK = value;
            }
        }

        public bool bTempTongHopDoanhThu_NTM
        {
            get
            {
                return _bTempTongHopDoanhThu_NTM;
            }
            set
            {
                if (_bTempTongHopDoanhThu_NTM == value)
                    return;
                _bTempTongHopDoanhThu_NTM = value;
            }
        }

        public bool bBaoCaoBHYT
        {
            get
            {
                return _bBaoCaoBHYT;
            }
            set
            {
                if (_bBaoCaoBHYT == value)
                    return;
                _bBaoCaoBHYT = value;
            }
        }

        public bool bBaoCaoVienPhiTrai
        {
            get
            {
                return _bBaoCaoVienPhiTrai;
            }
            set
            {
                if (_bBaoCaoVienPhiTrai == value)
                    return;
                _bBaoCaoVienPhiTrai = value;
            }
        }

        public bool bBaoCaoVienPhiKTC
        {
            get
            {
                return _bBaoCaoVienPhiKTC;
            }
            set
            {
                if (_bBaoCaoVienPhiKTC == value)
                    return;
                _bBaoCaoVienPhiKTC = value;
            }
        }

        public bool bBaoCaoVTYT_KTC
        {
            get
            {
                return _bBaoCaoVTYT_KTC;
            }
            set
            {
                if (_bBaoCaoVTYT_KTC == value)
                    return;
                _bBaoCaoVTYT_KTC = value;
            }
        }
        #endregion

        #region Properties of Price Management

        private bool _mPriceManagement = true;
        private bool _bMedServiceItemPriceList_Mgnt = true;
        private bool _bPCLExamTypePriceList_Mgnt = true;
        private bool _bRefDepartmentReqCashAdv_Mgnt = true;

        public bool mPriceManagement
        {
            get
            {
                return _mPriceManagement;
            }
            set
            {
                if (_mPriceManagement == value)
                    return;
                _mPriceManagement = value;
                NotifyOfPropertyChange(() => mPriceManagement);
            }
        }

        public bool bMedServiceItemPriceList_Mgnt
        {
            get
            {
                return _bMedServiceItemPriceList_Mgnt;
            }
            set
            {
                if (_bMedServiceItemPriceList_Mgnt == value)
                    return;
                _bMedServiceItemPriceList_Mgnt = value;
                NotifyOfPropertyChange(() => bMedServiceItemPriceList_Mgnt);
            }
        }

        public bool bPCLExamTypePriceList_Mgnt
        {
            get
            {
                return _bPCLExamTypePriceList_Mgnt;
            }
            set
            {
                if (_bPCLExamTypePriceList_Mgnt == value)
                    return;
                _bPCLExamTypePriceList_Mgnt = value;
                NotifyOfPropertyChange(() => bPCLExamTypePriceList_Mgnt);
            }
        }

        public bool bRefDepartmentReqCashAdv_Mgnt
        {
            get
            {
                return _bRefDepartmentReqCashAdv_Mgnt;
            }
            set
            {
                if (_bRefDepartmentReqCashAdv_Mgnt == value)
                    return;
                _bRefDepartmentReqCashAdv_Mgnt = value;
                NotifyOfPropertyChange(() => bRefDepartmentReqCashAdv_Mgnt);
            }
        }

        #endregion

        #region Properties of Y Vụ

        private bool _mYVuReport = true;
        private bool _bThongKeNgoaiTru = true;
        private bool _bBaoCaoNhanhKhuKhamBenh = true;
        private bool _bReportInPatient = true;
        private bool _bReportGeneralTemp02 = true;

        public bool mYVuReport
        {
            get
            {
                return _mYVuReport;
            }
            set
            {
                if (_mYVuReport == value)
                    return;
                _mYVuReport = value;
                NotifyOfPropertyChange(() => mYVuReport);
            }
        }

        public bool bThongKeNgoaiTru
        {
            get
            {
                return _bThongKeNgoaiTru;
            }
            set
            {
                if (_bThongKeNgoaiTru == value)
                    return;
                _bThongKeNgoaiTru = value;
                NotifyOfPropertyChange(() => bThongKeNgoaiTru);
            }
        }

        public bool bBaoCaoNhanhKhuKhamBenh
        {
            get
            {
                return _bBaoCaoNhanhKhuKhamBenh;
            }
            set
            {
                if (_bBaoCaoNhanhKhuKhamBenh == value)
                    return;
                _bBaoCaoNhanhKhuKhamBenh = value;
                NotifyOfPropertyChange(() => bBaoCaoNhanhKhuKhamBenh);
            }
        }

        public bool bReportInPatient
        {
            get
            {
                return _bReportInPatient;
            }
            set
            {
                if (_bReportInPatient == value)
                    return;
                _bReportInPatient = value;
                NotifyOfPropertyChange(() => bReportInPatient);
            }
        }

        public bool bReportGeneralTemp02
        {
            get
            {
                return _bReportGeneralTemp02;
            }
            set
            {
                if (_bReportGeneralTemp02 == value)
                    return;
                _bReportGeneralTemp02 = value;
                NotifyOfPropertyChange(() => bReportGeneralTemp02);
            }
        }

        private bool _IsNoteHMSSystem = !Globals.IseHMSSystem;
        public bool IsNoteHMSSystem
        {
            get
            {
                return _IsNoteHMSSystem;
            }
            set
            {
                if (_IsNoteHMSSystem == value)
                    return;
                _IsNoteHMSSystem = value;
                NotifyOfPropertyChange(() => IsNoteHMSSystem);
            }
        }
        #endregion

        private void Temp20NgoaiTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_NGOAITRU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NgoaiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20NgoaiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1217_G1_Mau20NgTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20NgoaiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20NgoaiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp20NgoaiTruTraThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_NGOAITRU_TRATHUOC;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            //KMX: Nếu có làm phân quyền cho mẫu 20 trả thuốc thì phải sửa lại Enum
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NgoaiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20NgoaiTruTraThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1218_G1_Mau20NgTruTraThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20NgoaiTruTraThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20NgoaiTruTraThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp20NoiTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_NOITRU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NoiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20NoiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1219_G1_Mau20NoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20NoiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20NoiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp20VTYTTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_VTYTTH;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NoiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20VTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1220_G1_Mau20VTYTTieuHao.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20VTYTTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20VTYTTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp21NgoaiTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP21_NGOAITRU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp21NgoaiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp21NgoaiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1221_G1_Mau21_NgTru.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp21NgoaiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp21NgoaiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void Temp21NoiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1222_G1_Mau21NoiTru;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp21NoiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp21NoiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void Temp21NoiTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP21_NOITRU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp21NoiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        private void Temp25aCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP25a_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp25aInsurance;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp25aCmd()
        {
            Globals.TitleForm = eHCMSResources.T3728_G1_Mau25A;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp25aCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp25aCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp25aTraThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP25aTRATHUOC_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            //KMX: Nếu có làm phân quyền cho mẫu 25 trả thuốc thì phải sửa lại Enum
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp25aInsurance;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp25aTraThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.T3729_G1_Mau25ATraThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp25aTraThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp25aTraThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp26aCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP26a_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp26a;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp26aCmd()
        {
            Globals.TitleForm = eHCMSResources.T3730_G1_Mau26A;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp26aCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp26aCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp19Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP19;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp19;
            UnitVM.Only79A = true;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp19Cmd()
        {
            Globals.TitleForm = eHCMSResources.G0023_G1_Mau19;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp19Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp19Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp20NoiTruNewCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_NOITRU_NEW;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NoiTruNew;
            UnitVM.Only79A = true;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20NoiTruNewCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1223_G1_Mau20NoiTruNew.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20NoiTruNewCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20NoiTruNewCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        public void Temp21NewCmd()
        {
            Globals.TitleForm = eHCMSResources.G0025_G1_Mau21;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp21NewCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp21NewCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void Temp21NewCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP21_NEW;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.ShowKCBBanDau = true;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp21New;
            UnitVM.Only79A = true;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }



        private void Temp79aCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP79a_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp79a;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp79aCmd()
        {
            Globals.TitleForm = eHCMSResources.T3733_G1_Mau79A;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp79aCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp79aCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void Temp79aTraThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP79aTRATHUOC_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp79aTraThuoc;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp79aTraThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.T3734_G1_Mau79ATraThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp79aTraThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp79aTraThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void Temp80aCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP80a_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp26a;
            UnitVM.Only79A = true;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp80aCmd()
        {
            Globals.TitleForm = eHCMSResources.T3736_G1_Mau80A;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp80aCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp80aCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp38aNgoaiTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp38NgoaiTru>();
            UnitVM.IsNgoaiTru = true;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp38aNgoaiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1224_G1_Mau01BVNgTru.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp38aNgoaiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp38aNgoaiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp38aNoiTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp38NoiTru>();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp38aNoiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1225_G1_Mau02BVNoiTru.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp38aNoiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp38aNoiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ChiTietVienPhiPKCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TRANSACTION_VIENPHICHITIET_PK;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp02_ChiTietVienPhi_PK;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ChiTietVienPhiPKCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1216_G1_CTietVPhiPK.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ChiTietVienPhiPKCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ChiTietVienPhiPKCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ChiTietVienPhiCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TRANSACTION_VIENPHICHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp04_ChiTietVienPhi;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ChiTietVienPhiCmd()
        {
            Globals.TitleForm = eHCMSResources.T3715_G1_Mau04CTietVPhi;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ChiTietVienPhiCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ChiTietVienPhiCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        // mẫu 02 - Hoạt động khám bệnh -TMA
        private void HoatDongKBCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.TRANSACTION_HOATDONGKB;
            UnitVM.RptParameters.ShowFirst = "";
            // UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp02_HoatDongKB;
            // UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void HoatDongKBCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1937_G1_HDongKB;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoatDongKBCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoatDongKBCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        // mẫu 06 - CLS -TMA
        private void CanLamSanCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.TRANSACTION_CANLAMSAN;
            UnitVM.RptParameters.ShowFirst = "";
            // UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp06_CanLamSan;
            // UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void CanLamSanCmd()
        {
            Globals.TitleForm = eHCMSResources.T3716_G1_Mau06CanLamSan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanLamSanCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanLamSanCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //
        // mẫu 07 - Duoc BV -TMA
        private void DuocBVCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.TRANSACTION_DUOCBV;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp07_DuocBV;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DuocBVCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1942_G1_DuocBV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DuocBVCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DuocBVCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        // PHỤ LỤC 2B - TransferFormType2Rpt -TMA : 06/10/2017
        private void TransferFormType2RptCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TransferFormType2Rpt;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp02b_TransferFormType2Rpt;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TransferFormType2RptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2131_G1_TransferFormType2Rpt;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferFormType2RptCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferFormType2RptCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        // PHỤ LỤC 2A - TransferFormType2_1Rpt -TMA : 09/10/2017
        private void TransferFormType2_1RptCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TransferFormType2_1Rpt;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp02a_TransferFormType2_1Rpt;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TransferFormType2_1RptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2132_G1_TransferFormType2_1Rpt;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferFormType2_1RptCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferFormType2_1RptCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //
        // PHỤ LỤC 5 - TransferFormType5Rpt -TMA : 12/10/2017
        private void TransferFormType5RptCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TransferFormType5Rpt;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp05_TransferFormType5Rpt;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TransferFormType5RptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2133_G1_TransferFormType5Rpt;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferFormType5RptCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferFormType5RptCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //

        //
        // Danh sách chi tiết BN chuyển đi các tuyến  DuyNH: 20/1/2020
        private void BCChiTietBNChuyenDiCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.BCChiTietBNChuyenDi;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            //UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp05_TransferFormType5Rpt;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BCChiTietBNChuyenDiCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2132_G1_TransferFormType2_1Rpt;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCChiTietBNChuyenDiCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCChiTietBNChuyenDiCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //

        // Danh sách chi tiết BN chuyển đến   DuyNH: 20/1/2020
        private void BCChiTietBNChuyenDenCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.BCChiTietBNChuyenDen;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            //UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp05_TransferFormType5Rpt;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BCChiTietBNChuyenDenCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2131_G1_TransferFormType2Rpt;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCChiTietBNChuyenDenCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCChiTietBNChuyenDenCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //


        // Báo cáo công tác chuyển tuyến  DuyNH: 20/1/2020
        private void BCcongtacchuyentuyenCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.BCCongTacChuyenTuyen;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            //UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp05_TransferFormType5Rpt;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BCcongtacchuyentuyenCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2133_G1_TransferFormType5Rpt;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCcongtacchuyentuyenCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCcongtacchuyentuyenCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //

        //TMA: 18/10/2017 MẪU SỔ KIỂM NHẬP THUỐC/HOÁ CHẤT/VẬT TƯ 
        private void RptDrugMedDeptCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.RptDrugMedDept;
            UnitVM.RptParameters.ShowFirst = "";
            // UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mRptDrugMedDept;
            // UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void RptDrugMedDeptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2135_G1_RptDrugMedDept;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RptDrugMedDeptCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RptDrugMedDeptCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ThongKeDoanhThuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.THONGKEDOANHTHU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTempThongKeDoanhThu;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ThongKeDoanhThuCmd()
        {
            Globals.TitleForm = eHCMSResources.G0454_G1_TKeDThu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ThongKeDoanhThuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ThongKeDoanhThuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TongHopDoanhThuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IRptDoanhThuTongHop>();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TongHopDoanhThuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1809_G1_TgHopDThu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TongHopDoanhThuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TongHopDoanhThuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void HoatDongQuayDKCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IRptHoatDongQuayDK>();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void HoatDongQuayDKCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1810_G1_BCHoatDongQDK;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoatDongQuayDKCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoatDongQuayDKCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TongHopDoanhThuNoiTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IRptDoanhThuTongHopNoiTru>();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TongHopDoanhThuNoiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2741_G1_BCTgHopDThuTheoKhoa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TongHopDoanhThuNoiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TongHopDoanhThuNoiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BaoCaoBHYTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<ICreateHIReport>();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void BaoCaoBHYTCmd()
        {
            Globals.TitleForm = eHCMSResources.K1053_G1_BCBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoBHYTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoBHYTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BaoCaoVienPhiTraiCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.HOSPITAL_FEES_REPORT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BaoCaoVienPhiTraiCmd()
        {
            Globals.TitleForm = eHCMSResources.K1085_G1_BCVPhiTrai;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoVienPhiTraiCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoVienPhiTraiCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BaoCaoVPMTreEmCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1811_G1_BCVPhiMoTreEm6Tuoi;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoVPMTreEmCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoVPMTreEmCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BaoCaoVPMTreEmCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.HIGH_TECH_FEES_CHILD_REPORT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        private void BaoCaoVienPhiKTCCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.HIGH_TECH_FEES_REPORT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BaoCaoVienPhiKTCCmd()
        {
            Globals.TitleForm = eHCMSResources.K1083_G1_BCVPhiKTC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoVienPhiKTCCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoVienPhiKTCCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BaoCaoVTYT_KTCCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.OUT_MEDICAL_MATERIAL_REPORT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BaoCaoVTYT_KTCCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1812_G1_BCVTYTKTC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoVTYT_KTCCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoVTYT_KTCCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ThongKeNgoaiTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<ITransactionModule>();
            var ThongKeNgoaiTruVM = Globals.GetViewModel<IThongKeNgoaiTru>();
            ThongKeNgoaiTruVM.LoadRefDept(V_DeptTypeOperation.KhoaNgoaiTru);
            Module.MainContent = ThongKeNgoaiTruVM;
            (Module as Conductor<object>).ActivateItem(ThongKeNgoaiTruVM);
        }

        public void ThongKeNgoaiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.K1035_G1_BKeCTietKB;
            ThongKeNgoaiTruCmd_In();
        }

        private void ReportQuickConsultationCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var innerVm = Globals.GetViewModel<IBaoCaoNhanhKhuKhamBenh>();
            innerVm.eItem = ReportName.BAOCAONHANH_KHUKHAMBENH;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportQuickConsultationCmd()
        {
            Globals.TitleForm = eHCMSResources.K1062_G1_BCNhanhKhuKB;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportQuickConsultationCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportQuickConsultationCmd_In();
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ReportInPatientCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.REPORT_INPATIENT;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.IsYVu = true;
            UnitVM.EnumOfFunction = (int)eYVu_Management.mReportInPatient;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ReportInPatientCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1813_G1_BCBNDTriNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportInPatientCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportInPatientCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportGeneralTemp02Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.REPORT_GENERAL_TEMP02;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.IsYVu = true;
            UnitVM.EnumOfFunction = (int)eYVu_Management.mReportGeneralTemp02;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ReportGeneralTemp02Cmd()
        {
            Globals.TitleForm = eHCMSResources.G1517_G1_THopCPhiDTriNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportGeneralTemp02Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportGeneralTemp02Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void MedServiceItemPriceList_Mgnt_In()
        {
            Globals.PageName = Globals.TitleForm;
            var TransactionModule = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IMedServiceItemPriceList>();

            TransactionModule.MainContent = VM;
            (TransactionModule as Conductor<object>).ActivateItem(VM);
        }

        public void MedServiceItemPriceList_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.K1028_G1_BGiaDV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                MedServiceItemPriceList_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        MedServiceItemPriceList_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PCLExamTypePriceList_Mgnt_In()
        {
            Globals.PageName = Globals.TitleForm;
            var TransactionModule = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IPCLExamTypePriceList>();

            TransactionModule.MainContent = VM;
            (TransactionModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTypePriceList_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.K1032_G1_BGiaPCLExamtype;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLExamTypePriceList_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamTypePriceList_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void RefDepartmentReqCashAdv_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.T1143_G1_GiaTUTungKhoa;
            Globals.PageName = Globals.TitleForm;
            var TransactionModule = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IRefDepartmentReqCashAdv>();

            TransactionModule.MainContent = VM;
            (TransactionModule as Conductor<object>).ActivateItem(VM);
        }

        private void FollowICDCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.FollowICD;
            UnitVM.ShowIsNewForm = true;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void FollowICDCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2041_G1_THBTTVTaiBV.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FollowICDCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FollowICDCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EmployeesCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.EmployeesReport;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EmployeesCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2044_G1_TinhHinhCBCCVC.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EmployeesCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EmployeesCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PaymentManagementCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IHISPaymentHistory>();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void PaymentManagementCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2046_G1_QuanLyChiTieu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PaymentManagementCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PaymentManagementCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void CreateFastReportCmd_In(long V_FastReportType)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<ICreateFastReport>();
            VM.V_FastReportType = V_FastReportType;
            VM.TitleForm = Globals.TitleForm;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void CreateFastReportCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2592_G1_BCKeToan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CreateFastReportCmd_In(0);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CreateFastReportCmd_In(0);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void CreateFastReport_BLCmd()
        {
            Globals.TitleForm = "Báo cáo kế toán - Biên lai thu tiền, hoàn ứng, hóa đơn, doanh thu";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CreateFastReportCmd_In((long)AllLookupValues.V_FastReportType.Bien_Lai_Hoa_Ung);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CreateFastReportCmd_In((long)AllLookupValues.V_FastReportType.Bien_Lai_Hoa_Ung);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void CreateFastReport_NTDCmd()
        {
            Globals.TitleForm = "Báo cáo kế toán - Nhập dược, trả dược";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CreateFastReportCmd_In((long)AllLookupValues.V_FastReportType.Nhap_Tra_Duoc);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CreateFastReportCmd_In((long)AllLookupValues.V_FastReportType.Nhap_Tra_Duoc);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void CreateFastReport_XDCmd()
        {
            Globals.TitleForm = "Báo cáo kế toán - Xuất dược";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CreateFastReportCmd_In((long)AllLookupValues.V_FastReportType.Xuat_Duoc);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CreateFastReportCmd_In((long)AllLookupValues.V_FastReportType.Xuat_Duoc);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void CreateDQGReportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<ICreateDQGReport>();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void CreateDQGReportCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2651_G1_BCDuocQuocGia;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CreateDQGReportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CreateDQGReportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #region Hospital Statistics
        private void HRStatisticsByDeptCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.HR_STATISTICS_BY_DEPT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void HRStatisticsByDeptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1936_G1_TinhHinhCanBo;
            HRStatisticsByDeptCmd_In();
        }

        private void MedExamActivityCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.MED_EXAM_ACTIVITY;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void MedExamActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1937_G1_HDongKB;
            MedExamActivityCmd_In();
        }

        private void TreatmentActivityCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.TREATMENT_ACTIVITY;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TreatmentActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1938_G1_HDongDTri;
            TreatmentActivityCmd_In();
        }

        private void SpecialistTreatmentActivityCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.SPECIALIST_TREATMENT_ACTIVITY;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void SpecialistTreatmentActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1939_G1_HDongDTriChuyenKhoa;
            SpecialistTreatmentActivityCmd_In();
        }

        private void SurgeryActivityCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.SURGERY_ACTIVITY;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void SurgeryActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1940_G1_HDongTThuatPThuat;
            SurgeryActivityCmd_In();
        }

        private void ReproductiveHealthActivityCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.REPRODUCTIVE_HEALTH_ACTIVITY;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void ReproductiveHealthActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1946_G1_HDongSKSS;
            ReproductiveHealthActivityCmd_In();
        }

        private void PCLActivityCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.PCL_ACTIVITY;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void PCLActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1941_G1_HDongCLS;
            PCLActivityCmd_In();
        }

        private void PharmacyDeptStatisticsCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.PHARMACY_DEPT_STATISTICS;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void PharmacyDeptStatisticsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1942_G1_DuocBV;
            PharmacyDeptStatisticsCmd_In();
        }

        //------ DPT 
        public void HoatdongchidaotuyenCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2118_G1_HDCDT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoatdongchidaotuyenCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoatdongchidaotuyenCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void HoatdongchidaotuyenCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mChiDaoTuyen;
            UnitVM.eItem = ReportName.HoatDongChiDaoTuyen;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BC15DayCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2134_G1_15Ngay;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BC15DayCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BC15DayCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BC15DayCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mBC15Day;
            UnitVM.eItem = ReportName.BaoCao15Ngay;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ChidaotuyenCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2118_G1_CDT;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ChidaotuyenCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ChidaotuyenCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ChidaotuyenCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITrainingForSubOrg>();
            UnitVM.TitleForm = "Quản lý hoạt động chỉ đạo tuyến";
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void SoXetNghiemCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2148_G1_SoXetNghiem;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SoXetNghiemCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SoXetNghiemCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SoXetNghiemCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mSXN;
            UnitVM.eItem = ReportName.SoXetNghiem;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        //---------
        private void MedicalEquipmentStatisticsCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.MEDICAL_EQUIPMENT_STATISTICS;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void MedicalEquipmentStatisticsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1947_G1_ThietBiYTe;
            MedicalEquipmentStatisticsCmd_In();
        }

        //-------DPT
        private void ScientificResearchActivityCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.SCIENTIFIC_RESEARCH_ACTIVITY;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mNghienCuuKhoaHoc;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        private void NghienCuuKhoaHocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IScientificResearchActivity>();
            UnitVM.TitleForm = Globals.TitleForm;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ScientificResearchActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2119_G1_HDNCKH;
            ScientificResearchActivityCmd_In();
        }

        public void NghienCuuKhoaHocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2119_G1_HDNCKH;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NghienCuuKhoaHocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NghienCuuKhoaHocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void FinancialActivityTemp1Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.FINANCIAL_ACTIVITY_TEMP1;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        //----------------------
        public void FinancialActivityTemp1Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z1944_G1_HDongTCMau1;
            FinancialActivityTemp1Cmd_In();
        }

        private void FinancialActivityTemp2Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.FINANCIAL_ACTIVITY_TEMP2;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void FinancialActivityTemp2Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z1944_G1_HDongTCMau2;
            FinancialActivityTemp2Cmd_In();
        }

        private void FinancialActivityTemp3Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.FINANCIAL_ACTIVITY_TEMP3;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void FinancialActivityTemp3Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z1944_G1_HDongTCMau3;
            FinancialActivityTemp3Cmd_In();
        }

        private void ICD10StatisticsCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.ICD10_STATISTICS;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void ICD10StatisticsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1945_G1_TinhHinhBenhTatTheoICD;
            ICD10StatisticsCmd_In();
        }

        //TransferFormListCmd
        public void TransferFormListCmd()
        {
            Globals.TitleForm = "Danh sách chuyển tuyến";
            TransferFormListCmd_In();
        }

        private void TransferFormListCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITransferFormList>();
            //UnitVM.eItem = ReportName.ICD10_STATISTICS;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        private void InPtAdmDisStatisticsCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.InPtAdmDisStatistics;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void InPtAdmDisStatisticsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2132_G1_SoVaoRaChuyenVien;
            InPtAdmDisStatisticsCmd_In();
        }
        #endregion

        private void InOutValueReportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.IsShowClinic = true;
            //VM.mXemIn = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            //VM.mKetChuyen = mBaoCaoXuatNhapTon_Thuoc_KetChuyen;
            VM.CanSelectedRefGenDrugCatID_1 = true;
            VM.IsGetValue = true;
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void InOutValueReportCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2104_G1_BCDoanhThuNXT.ToUpper();
            InOutValueReportCmd_In();
        }

        private void ConfirmBHYTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ICreateHIReport_V2>();
            UnitVM.ViewCase = 0;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void ConfirmBHYTCmd()
        {
            Globals.TitleForm = eHCMSResources.G2370_G1_XNhanBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ConfirmBHYTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ConfirmBHYTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void TransferFormDataExportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TRANSFERFORMDATA;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp05_TransferFormType5Rpt;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TransferFormDataExportCmd_Click()
        {
            Globals.TitleForm = eHCMSResources.Z2161_G1_DuLieuChuyenTuyen;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferFormDataExportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferFormDataExportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //--- DUY 07-04-2021

        private void BC_DS_DVKyThuatTrenHIS_KHTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.BC_DS_DichVuKyThuatTrenHIS;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp05_TransferFormType5Rpt;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BC_DS_DVKyThuatTrenHIS_KHTHCmd_Click()
        {
            Globals.TitleForm = eHCMSResources.Z3113_G1_BC_DSDichVuKyThuat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BC_DS_DVKyThuatTrenHIS_KHTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BC_DS_DVKyThuatTrenHIS_KHTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //--- DatTB 02-06-2021

        private void BC_DMKyThuatMoi_KHTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.BC_DM_KyThuatMoi;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp05_TransferFormType5Rpt;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BC_DMKyThuatMoi_KHTHCmd_Click()
        {
            Globals.TitleForm = eHCMSResources.Z3114_G1_BC_DMKyThuatMoi;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BC_DMKyThuatMoi_KHTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BC_DMKyThuatMoi_KHTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BangKeBacSiThucHienCLSCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2433_G1_BangKeBacSiThucHienCLS;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeBacSiThucHienCLSCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeBacSiThucHienCLSCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BangKeBacSiThucHienCLSCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BangKeBacSiThucHienCLS;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BaoCaoDoanhThuNgTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2438_G2_KeToanBaoCaoDoanhThuNgTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoDoanhThuNgTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoDoanhThuNgTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BaoCaoDoanhThuNgTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KT_BaoCaoDoanhThuNgTru;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;
            //▼==== #043
            reportVm.ShowPatientClassification = true;
            //▲==== #043
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void TinhHinhHoatDongCLS_KHTH_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2427_G1_TinhHinhHoatDongCLS;
            TinhHinhHoatDongCLS_Cmd_In(source);
        }
        public void TinhHinhHoatDongCLS_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2427_G1_TinhHinhHoatDongCLS;
            TinhHinhHoatDongCLS_Cmd_In(source);
        }

        private void TinhHinhHoatDongCLS_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.TinhHinhHoatDongCLS;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ThongKeDsachKhamBenhNgTruTheoBacSi_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2501_G1_ThongKeDsachKhamBenhNgTruTheoBsi;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ThongKeDsachKhamBenhNgTruTheoBacSi_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ThongKeDsachKhamBenhNgTruTheoBacSi_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void ThongKeDsachKhamBenhNgTruTheoBacSi_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.ThongKeDsachKBNgTruTheoBacSi;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ThongKeDsachKhamBenhNoiTruTheoBacSi_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2468_G1_ThongKeDsachKhamBenhNoiTruTheoBsi;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ThongKeDsachKhamBenhNoiTruTheoBacSi_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ThongKeDsachKhamBenhNoiTruTheoBacSi_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void ThongKeDsachKhamBenhNoiTruTheoBacSi_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.ThongKeDsachKBNoiTruTheoBacSi;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BangKeBacSiThucHienPT_TT_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2483_G1_BangKeBacSiThucHienPT_TT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeBacSiThucHienPT_TT_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeBacSiThucHienPT_TT_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BangKeBacSiThucHienPT_TT_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BangKeBacSiThucHienPT_TT;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BangKeBanLeHangHoaDV_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2483_G1_BangKeBanLeHangHoaDV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeBanLeHangHoaDV_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeBanLeHangHoaDV_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BangKeBanLeHangHoaDV_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.eItem = ReportName.BangKeBanLeHangHoaDV;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        private void CreateOutPtTransactionFinalizationCmd_In(bool IsExportEInvoiceView = false)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ICreateOutPtTransactionFinalization>();
            UnitVM.IsExportEInvoiceView = IsExportEInvoiceView;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void CreateOutPtTransactionFinalizationCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2563_G1_QuyetToanNgoaiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CreateOutPtTransactionFinalizationCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CreateOutPtTransactionFinalizationCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void CreateEInvoiceCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2438_G1_PhatHanhHDDT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CreateOutPtTransactionFinalizationCmd_In(true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CreateOutPtTransactionFinalizationCmd_In(true);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void KHTH_BCTinhHinhDichLayCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2505_G1_KHTH_BCTinhHinhDichLay_BenhQuanTrong;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KHTH_BCTinhHinhDichLayCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KHTH_BCTinhHinhDichLayCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void KHTH_BCTinhHinhDichLayCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KTTH_BCTinhHinhDichLay;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        private void OutwardDrugsByStaffStatisticCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.OutwardDrugsByStaffStatistic;
            UnitVM.StrHienThi = Globals.PageName;
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.ShowDepartment = Visibility.Collapsed;
            UnitVM.ViTreatedOrNot = Visibility.Collapsed;
            UnitVM.authorization();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void OutwardDrugsByStaffStatisticCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2495_G1_TKTinhHinhCDThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutwardDrugsByStaffStatisticCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutwardDrugsByStaffStatisticCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void OutwardDrugsByStaffStatisticDetailsCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.OutwardDrugsByStaffStatisticDetails;
            UnitVM.StrHienThi = Globals.PageName;
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.ShowDepartment = Visibility.Collapsed;
            UnitVM.ViTreatedOrNot = Visibility.Collapsed;
            UnitVM.mDrug = true;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.authorization();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void OutwardDrugsByStaffStatisticDetailsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2725_G1_ChiTietTinhHinhChiDinhThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutwardDrugsByStaffStatisticDetailsCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutwardDrugsByStaffStatisticDetailsCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void BCDsachBNXuatVien_KHTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2598_G1_BCDsachBNXuatVien;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCDsachBNXuatVien_KHTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCDsachBNXuatVien_KHTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCDsachBNXuatVienCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2598_G1_BCDsachBNXuatVien;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCDsachBNXuatVienCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCDsachBNXuatVienCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCDsachBNXuatVien_KHTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_XUAT_VIEN_KHTH;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.IsReportForKHTH = true;

            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void BCDsachBNXuatVienCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KTTH_BC_XUAT_VIEN;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCDsachBsiChiDinhNhapVienCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2599_G1_BCDsachBsiChiDinhNhapVien;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCDsachBsiChiDinhNhapVienCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCDsachBsiChiDinhNhapVienCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void BCDsachBsiChiDinhNhapVien_KHTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2599_G1_BCDsachBsiChiDinhNhapVien;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCDsachBsiChiDinhNhapVienCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCDsachBsiChiDinhNhapVienCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void BCDsachBsiChiDinhNhapVienCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KTTH_BC_CHI_DINH_NHAP_VIEN;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsReportForKHTH = true;

            regModule.MainContent = reportVm;

            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        //▼====: #003
        private void DeptInOutStatisticsCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.eItem = ReportName.TK_NX_THEOMUCDICH;
            reportVm.mXemChiTiet = true;
            reportVm.mXemChiTietTheoThuoc = true;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.ChonKho = true;
            reportVm.ViewBy = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void DeptInOutStatisticsCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2437_G2_NhapXuatTheoMucDich;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DeptInOutStatisticsCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DeptInOutStatisticsCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ClinicDeptInOutStatisticsCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.TK_NX_THEOMUCDICH;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mXemChiTietTheoThuoc = true;
            reportVm.mXemChiTiet = true;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.ChonKho = true;
            reportVm.ViewBy = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ClinicDeptInOutStatisticsCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2437_G4_NhapXuatTheoMucDichKhoPhong;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ClinicDeptInOutStatisticsCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ClinicDeptInOutStatisticsCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DrugInOutStatisticsCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.TK_NX_THEOMUCDICH;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mXemChiTietTheoThuoc = true;
            reportVm.mXemChiTiet = true;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.ChonKho = true;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_EXTERNAL);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DrugInOutStatisticsCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2437_G5_NhapXuatTheoMucDichKhoBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugInOutStatisticsCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugInOutStatisticsCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #003
        //▼====: #001
        private void BCChiTietDVCLSCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCChiTietDV_CLS;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mIn = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCChiTietDVCLSCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2656_G1_BCChiTietDichVuVaCLS;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCChiTietDVCLSCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCChiTietDVCLSCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BCChiTietDVCLS_KHTHCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCChiTietDV_CLS_KHTH;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mIn = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void BCChiTietDVCLS_KHTHCmd(object source)
        {
            Globals.TitleForm = "Báo cáo chi tiết dịch vụ và cận lâm sàng - KHTH";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCChiTietDVCLS_KHTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCChiTietDVCLS_KHTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //▲====: #001
        //▼==== #044
        private void BCChiTietDVCLSNewCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCChiTietDV_CLS;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mIn = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCChiTietDVCLSNewCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2656_G1_BCChiTietDichVuVaCLS;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCChiTietDVCLSNewCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCChiTietDVCLSNewCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲==== #044
        //▼====: #002
        private void BCDoanhThuThuocBHTNgTruCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCDoanhThuThuocBHYTNgTru;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mIn = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCDoanhThuThuocBHTNgTruCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2657_G1_BCDoanhThuThuocBHYTNgTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCDoanhThuThuocBHTNgTruCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCDoanhThuThuocBHTNgTruCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #002
        //▼====: #012
        //▼====: #004
        private void ChiTietNhapTuNCCCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BCChiTietNhapTuNCC;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.CanSelectedRefGenDrugCatID_1 = false;
            reportVm.IsShowGroupReportType = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            reportVm.ViewBy = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ChiTietNhapTuNCCCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2723_G1_BCChiTietNhapTuNCC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ChiTietNhapTuNCCCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ChiTietNhapTuNCCCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #004
        //▲====: #012
        //▼====: #005
        private void BCBNHoanTraCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCBNHoanTraBienLai;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mIn = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCBNHoanTraCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2722_G1_BCBNHoanTraBienLai;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCBNHoanTraCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCBNHoanTraCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #005

        //▼====: #006
        public void KHTH_TK_TinhHinhKCBCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2735_G1_KHTH_TK_TinhHinhKCB;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KHTH_TK_TinhHinhKCBCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KHTH_TK_TinhHinhKCBCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void KHTH_TK_TinhHinhKCBCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KHTH_TinhHinhKCB;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #006     
        //▼====: #007
        public void TinhHinhHoatDongDV_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2739_G1_KT_BC_TinhHinhHoatDongDV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TinhHinhHoatDongDV_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TinhHinhHoatDongDV_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void TinhHinhHoatDongDV_KHTH_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2739_G1_KT_BC_TinhHinhHoatDongDV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TinhHinhHoatDongDV_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TinhHinhHoatDongDV_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void TinhHinhHoatDongDV_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KT_TinhHinhHoatDongDV;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = true;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #007
        //▼====: #008
        public void BCHangTonNhieuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2751_G1_KT_BCHangTonNhieu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCHangTonNhieuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCHangTonNhieuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCHangTonNhieuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.CanSelectedWareHouse = true;
            VM.CanSelectedRefGenDrugCatID_1 = true;
            VM.IsGetValue = true;
            VM.eItem = ReportName.KT_BCHangTonNhieu;
            regModule.MainContent = VM;
            ((Conductor<object>)regModule).ActivateItem(VM);
        }

        public void BCHangCanDateCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2752_G1_KT_BCHangCanDate;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCHangCanDateCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCHangCanDateCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCHangCanDateCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KT_BCHangCanDate;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = true;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #008
        //▼====: #009
        public void BCTraSauCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2757_G1_BCTraSau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCTraSauCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCTraSauCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCTraSauCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KT_BCBNTraSau;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.isAllStaff = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #009

        //▼====: #011
        private void DrugDeptInOutValueReportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            //reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.mXemChiTiet = true;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DrugDeptInOutValueReportCmd()
        {
            Globals.TitleForm = eHCMSResources.T2257_G1_KhoaDuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptInOutValueReportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptInOutValueReportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ClinicDeptInOutValueReportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            //reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.mXemChiTiet = true;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ClinicDeptInOutValueReportCmd()
        {
            Globals.TitleForm = eHCMSResources.T2260_G1_KhoaNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ClinicDeptInOutValueReportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ClinicDeptInOutValueReportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PharmacyInOutValueReportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            //reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.mXemChiTiet = true;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_EXTERNAL);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void PharmacyInOutValueReportCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2802_G1_NXTNhaThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PharmacyInOutValueReportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmacyInOutValueReportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #011
        //▼====: #013
        private void DrugDeptCardStorageReportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ITheKhoKT>();
            //reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DrugDeptCardStorageReportCmd()
        {
            Globals.TitleForm = eHCMSResources.T2257_G1_KhoaDuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptCardStorageReportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptCardStorageReportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ClinicDeptCardStorageReportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ITheKhoKT>();
            //reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ClinicDeptCardStorageReportCmd()
        {
            Globals.TitleForm = eHCMSResources.T2260_G1_KhoaNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ClinicDeptCardStorageReportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ClinicDeptCardStorageReportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PharmacyCardStorageReportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ITheKhoKT>();
            //reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_EXTERNAL);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void PharmacyCardStorageReportCmd()
        {
            Globals.TitleForm = eHCMSResources.N0181_G1_NhaThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PharmacyCardStorageReportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmacyCardStorageReportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #013
        //▼====: #015
        private void HICardStorageReportCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ITheKhoKT>();
            //reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_EXTERNAL);
            reportVm.IsHIStore = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void HICardStorageReportCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2982_G1_KhoBHYTNgTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HICardStorageReportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HICardStorageReportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #015


        public void BCCongTacKCBCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2820_G1_BCCongTacKCB;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCCongTacKCBCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCCongTacKCBCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCCongTacKCBCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KHTH_BCCongTacKCB;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        //▼====: #011
        private void DrugDeptInOutValueReport_KTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.mXemChiTiet = true;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DrugDeptInOutValueReport_KTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2802_G1_NXTKhoaDuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptInOutValueReport_KTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptInOutValueReport_KTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ClinicDeptInOutValueReport_KTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.mXemChiTiet = true;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ClinicDeptInOutValueReport_KTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2802_G1_NXTKhoaNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ClinicDeptInOutValueReport_KTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ClinicDeptInOutValueReport_KTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PharmacyInOutValueReport_KTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            reportVm.mXemChiTiet = true;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_EXTERNAL);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void PharmacyInOutValueReport_KTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2802_G1_NXTNhaThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PharmacyInOutValueReport_KTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmacyInOutValueReport_KTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #011
        private void ReportMienGiamNoiTruCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BAOCAO_MIENGIAM_NOITRU_TV;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.LoadListStaff((byte)StaffRegistrationType.INSURANCE);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ReportMienGiamNoiTruCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2968_G1_BaoCaoMienGiamNoiTru;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportMienGiamNoiTruCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportMienGiamNoiTruCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        //▼====: #016
        private void DLS_BCQLXuatThuocKhoaPhongCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.DLS_BCXuatThuocBNKhoaPhong;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.ShowMoreThreeDays = true;
            reportVm.mDepartment = true;
            reportVm.IsShowPatientType = true;
            reportVm.IsShowFaName = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DLS_BCQLXuatThuocKhoaPhongCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3000_G1_BCQLXuatThuocKhoaPhong;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                DLS_BCQLXuatThuocKhoaPhongCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DLS_BCQLXuatThuocKhoaPhongCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }
        //▲====: #016
        //Báo cáo BN đang điều trị
        private void ReportPatientAreTreatedCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            //LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<ITransactionModule>();

            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();

            innerVm.eItem = ReportName.REPORT_PATIENT_ARE_TREATED;
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.mXemChiTiet = false;
            innerVm.mIn = false;
            innerVm.isAllStaff = false;
            innerVm.mDepartment = true;
            innerVm.IsReportForKHTH = true;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportPatientAreTreatedCmd(object source)
        {
            Globals.TitleForm = "BÁO CÁO BỆNH NHÂN ĐANG ĐIỀU TRỊ NỘI TRÚ";

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportPatientAreTreatedCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportPatientAreTreatedCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        public void BCBenhNhanPTTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3100_G1_BCBenhNhanPTTT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCBenhNhanPTTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCBenhNhanPTTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void BCBenhNhanPTTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_NHAP_PTTT_KHOA_PHONG_KHTH;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.mXemChiTiet = false;
            //reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.ShowLocation = true;
            reportVm.IsReportForKHTH = true;

            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void BaoCaoDVDaThucHienCmd()
        {
            Globals.TitleForm = "Báo cáo DS dịch vụ kỹ thuật đã thực hiện";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoDVDaThucHienCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoDVDaThucHienCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void BaoCaoDVDaThucHienCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCDichVuCLSDangThucHien_PhanTuyen;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.mXemChiTiet = false;
            //reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.ShowLocation = true;
            reportVm.IsReportForKHTH = true;

            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void PatientDeceaseCmd()
        {
            Globals.TitleForm = "Báo cáo danh sách bệnh nhân tử vong";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PatientDeceaseCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PatientDeceaseCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void PatientDeceaseCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.RptPatientDecease;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.isAllStaff = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▼====: #017
        private void DLS_BCToaThuocHangNgayCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_ToaThuocHangNgay_DLS;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.mXemIn = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DLS_BCToaThuocHangNgayCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3024_G1_BCToaThuocHangNgayDLS;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DLS_BCToaThuocHangNgayCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DLS_BCToaThuocHangNgayCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BCThuocYCuDoDangCuoiKyCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_ThuocYCuDoDangCuoiKy;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.mXemIn = false;
            reportVm.mDepartment = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCThuocYCuDoDangCuoiKyCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3025_G1_BCThuocYcuDoDangCuoiKy;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThuocYCuDoDangCuoiKyCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThuocYCuDoDangCuoiKyCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #017
        //▼====: #018
        private void TKChiDinhThuocTPNoiTruCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.OutwardDrugClinicDeptsByStaffStatisticDetails_TP;
            UnitVM.StrHienThi = Globals.PageName;
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.ViTreatedOrNot = Visibility.Collapsed;
            UnitVM.authorization();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TKChiDinhThuocTPNoiTruCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3059_G1_TKChiDinhThuocTPNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TKChiDinhThuocTPNoiTruCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TKChiDinhThuocTPNoiTruCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #018
        //▼====: #019
        public void BCDsachBsiChiDinhNhapVienKHTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2599_G1_BCDsachBsiChiDinhNhapVien;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCDsachBsiChiDinhNhapVienCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCDsachBsiChiDinhNhapVienCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #019

        //▼====: #20210312 QTD BC_ThanhToanThe
        public void BCThanhToanTheCmd()
        {
            Globals.TitleForm = "Báo cáo thu tiền qua thẻ";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThanhToanTheCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThanhToanTheCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCThanhToanTheCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KT_BC_ThanhToanQuaThe;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.isAllStaff = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #20210312

        //▼====== #020
        private void CheckMedicalFileForKHTH_In(object source)
        {
            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_RATOA;
            Globals.PageName = Globals.TitleForm;
            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;
            homeVM.IsExpandOST = false;
            var mModule = Globals.GetViewModel<ITransactionModule>();
            var mView = Globals.GetViewModel<ICheckMedicalFiles>();
            mView.mCheckMedicalFile_Tim = mCheckMedicalFile_Tim;
            mView.mCheckMedicalFile_DanhMucICD = mCheckMedicalFile_DanhMucICD;
            mView.mCheckMedicalFile_Luu = mCheckMedicalFile_Luu;
            mView.mCheckMedicalFile_TraHS = mCheckMedicalFile_TraHS;
            mView.mCheckMedicalFile_Duyet = mCheckMedicalFile_Duyet;
            mView.mCheckMedicalFile_MoKhoa = mCheckMedicalFile_MoKhoa;
            mView.mCheckMedicalFile_DLS_Save = mCheckMedicalFile_DLS_Save;
            mView.mCheckMedicalFile_DLS_Duyet = mCheckMedicalFile_DLS_Duyet;
            mView.mCheckMedicalFile_DLS_TraHS = mCheckMedicalFile_DLS_TraHS;
            mModule.MainContent = mView;
            ((Conductor<object>)mModule).ActivateItem(mView);
        }
        public void CheckMedicalFileForKHTH(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3110_G1_KiemDuyetHoSo;
            if (string.IsNullOrEmpty(Globals.TitleForm))
            {
                CheckMedicalFileForKHTH_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CheckMedicalFileForKHTH_In(source);
                    }
                });
            }
        }
        //▲====== #020
        private void ThongTu56Cmd_In(object source)
        {
            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_RATOA;
            Globals.PageName = Globals.TitleForm;
            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;
            homeVM.IsExpandOST = true;
            var mModule = Globals.GetViewModel<ITransactionModule>();
            var mView = Globals.GetViewModel<ICirculars56>();
            mModule.MainContent = mView;
            ((Conductor<object>)mModule).ActivateItem(mView);
        }
        public void ThongTu56Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3115_G1_ThongTu56;
            if (string.IsNullOrEmpty(Globals.TitleForm))
            {
                ThongTu56Cmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ThongTu56Cmd_In(source);
                    }
                });
            }
        }

        public void ExportExcel4210Cmd()
        {
            Globals.TitleForm = "Xuất dữ liệu theo 4210";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ExportExcel4210Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ExportExcel4210Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void ExportExcel4210Cmd_In()
        {
            //Globals.PageName = Globals.TitleForm;
            //var regModule = Globals.GetViewModel<ITransactionModule>();
            //var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            //reportVm.eItem = ReportName.KT_BC_ThanhToanQuaThe;
            //reportVm.strHienThi = Globals.TitleForm;
            //reportVm.IsEnabledToDatePicker = true;
            //reportVm.isAllStaff = false;
            //reportVm.mInPatientDeptStatus = false;
            //reportVm.RptParameters.HideFindPatient = false;
            //regModule.MainContent = reportVm;
            //((Conductor<object>)regModule).ActivateItem(reportVm);
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.ExportExcel4210;
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.IsExportExcel4210 = Visibility.Visible;
            UnitVM.mViewAndPrint = false;
            UnitVM.IsExportExcel4210 = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp79a;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        private void ExportExcel4210Cmd(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<ICreateDQGReport>();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);

            Globals.PageName = "Xuất dữ liệu theo 4210";
        }

        //▼====== #021
        private void BCNXTTHUOCTHCmd_In_V2(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var Module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.CanSelectedWareHouse = false;
            VM.IsCheck = false;
            VM.IsShowBHYT = true;
            VM.eItem = ReportName.BC_NXT_THUOC_TONGHOP_V2;
            VM.CanSelectedRefGenDrugCatID_1 = false;
            VM.ShowRangeOfHospital = true;
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void BCNXTTHUOCTHCmd_V2(object source)
        {
            Globals.TitleForm = "BC NXT TOÀN KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCNXTTHUOCTHCmd_In_V2(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCNXTTHUOCTHCmd_In_V2(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #021

        //▼====== #022
        private void BCNXTKHOTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BC_KHO_TONGHOP;
            reportVm.mXemChiTiet = false;
            reportVm.mXemChiTietTheoThuoc = false;
            reportVm.ChonKho = false;
            reportVm.IsPurpose = false;
            reportVm.ViewBy = false;
            reportVm.IsPurpose = false;
            reportVm.CanSelectedRefGenDrugCatID_1 = false;
            reportVm.IsShowGroupReportType = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCNXTKHOTHCmd()
        {
            Globals.TitleForm = eHCMSResources.T2257_G1_KhoaDuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCNXTKHOTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCNXTKHOTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #022
        public void BCCongSuatGiuongBenhCmd()
        {
            Globals.TitleForm = "Báo cáo công suất giường bệnh";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCCongSuatGiuongBenhCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCCongSuatGiuongBenhCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCCongSuatGiuongBenhCmd_In()
        {
            Globals.PageName = "Báo cáo công suất giường bệnh";
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_Cong_Suat_Giuong_Benh;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.IsReportForKHTH = true;

            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCGiaoBanTheoKhoaLS_KHTHCmd()
        {
            Globals.TitleForm = "Báo cáo giao ban theo khoa lâm sàng";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCGiaoBanTheoKhoaLS_KHTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCGiaoBanTheoKhoaLS_KHTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCGiaoBanTheoKhoaLS_KHTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_GiaoBan_Theo_KhoaLS_KHTH;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.IsReportForKHTH = true;

            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void BCSuDungToanBVCmd()
        {
            Globals.TitleForm = "BÁO CÁO SỬ DỤNG TOÀN BỆNH VIỆN";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCSuDungToanBVCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCSuDungToanBVCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCSuDungToanBVCmd_In()
        {
            Globals.PageName = "BÁO CÁO SỬ DỤNG TOÀN BỆNH VIỆN";
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.BCSuDungToanBV;
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.IsExportExcel4210 = Visibility.Visible;
            UnitVM.mViewAndPrint = false;
            regModule.MainContent = UnitVM;
            ((Conductor<object>)regModule).ActivateItem(UnitVM);
        }
        public void BCGiaoBanKHTHCmd()
        {
            Globals.TitleForm = "Báo cáo giao ban phòng KHTH";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCGiaoBanKHTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCGiaoBanKHTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCGiaoBanKHTHCmd_In()
        {
            Globals.PageName = "Báo cáo giao ban phòng KHTH";
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_GiaoBan_Phong_KHTH;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.IsReportForKHTH = true;
            reportVm.IsDateTime = true;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▼====== #024
        public void BCQuanLyKiemDuyetHoSo_KHTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3237_G1_BC_QLKiemDuyetHoSo_KTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                QuanLyKiemDuyetHoSo_KHTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        QuanLyKiemDuyetHoSo_KHTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void QuanLyKiemDuyetHoSo_KHTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KTTH_BC_QLKiemDuyetHoSo;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsReportForKHTH = true;

            regModule.MainContent = reportVm;

            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====== #024

        //▼====: #025
        private void DLS_BCKiemTraLichSuKCBCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.DLS_BCKiemTraLichSuKCB;
            reportVm.mDepartment = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.mXemIn = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DLS_BCKiemTraLichSuKCBCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3238_G1_DLS_BCKiemTraLichSuKCB;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DLS_BCKiemTraLichSuKCBCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DLS_BCKiemTraLichSuKCBCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #025

        //▼====: #026
        private void DLS_BCThongTinDanhMucThuocCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.DLS_BCThongTinDanhMucThuoc;
            reportVm.mDepartment = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.mXemIn = false;
            reportVm.IsEnabledFromDatePicker = false;
            reportVm.IsEnabledToDatePicker = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DLS_BCThongTinDanhMucThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3239_G1_DLS_BCThongTinDanhMucThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DLS_BCThongTinDanhMucThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DLS_BCThongTinDanhMucThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #026

        //▼====: #027
        private void BCTinhHinhCLS_DaThucHienCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCTinhHinhCLS_DaThucHien;
            reportVm.mDepartment = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.mXemIn = false;
            reportVm.RptParameters.HideFindPatient = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCTinhHinhCLS_DaThucHienCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3241_G1_BCTinhHinhCLS_DaThucHien;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCTinhHinhCLS_DaThucHienCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCTinhHinhCLS_DaThucHienCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #027
        //▼====: #033
        private void BaoCaoDoanhThuTheoKhoaCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BaoCaoDoanhThuTheoKhoa;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.RptParameters.ShowDischarge = true;
            reportVm.rdtPrint = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BaoCaoDoanhThuTheoKhoaCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3245_G1_BaoCaoDoanhThuTheoKhoa;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                BaoCaoDoanhThuTheoKhoaCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoDoanhThuTheoKhoaCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }
        //▲====: #033
        //▼====: #034
        private void BaoCaoHoanMienTamUngNoiTruCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BaoCaoHoanMienTamUngNoiTru;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BaoCaoHoanMienTamUngNoiTruCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3246_G1_BaoCaoHoanMienTamUngNoiTru;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                BaoCaoHoanMienTamUngNoiTruCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoHoanMienTamUngNoiTruCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }
        //▲====: #034
        //▼==== #035
        private void BCThongKeSLHoSoDTNTCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCThongKeSLHoSoDTNT;
            reportVm.mDepartment = true;
            reportVm.ShowLocationSelect = true;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.mXemIn = false;
            reportVm.IsShowOutPtTreatmentStatus = true;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.ShowOutPtTreatmentType = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCThongKeSLHoSoDTNTCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3256_G1_BCThongKeSLHoSoDTNT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThongKeSLHoSoDTNTCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThongKeSLHoSoDTNTCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲==== #035
        #region Kế toán doanh thu
        public void KTDoanhThu_Temp80aCmd()
        {
            Globals.TitleForm = eHCMSResources.T3736_G1_Mau80A;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KTDoanhThu_Temp80aCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KTDoanhThu_Temp80aCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KTDoanhThu_Temp80aCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.KTDoanhThu_TEMP80a;
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp26a;
            UnitVM.authorization();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        //▼====== #023
        public void Temp21CovidCmd()
        {
            Globals.TitleForm = "Mẫu 21 - Covid";

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp21CovidCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp21CovidCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void Temp21CovidCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.ExportExcel_Temp21_COVID_KT;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.ShowKCBBanDau = true;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp21New;
            UnitVM.Only79A = true;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        //▲====== #023
        //▼====: #029
        private void ReportBaoCaoThuTienVienPhiNgoaiTru_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            innerVm.eItem = ReportName.BC_THU_TIEN_VIEN_PHI_NGOAI_TRU;
            innerVm.LoadListStaff((byte)StaffRegistrationType.INSURANCE);
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.vBaoCaoVienPhiBHYT = false;
            innerVm.IsShowPaymentMode = true; //20210127 QTD thêm điều kiện bật Combobox Hình thức TT
            innerVm.Case = (int)AllLookupValues.HIType.All;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportBaoCaoThuTienVienPhiNgoaiTru(object source)
        {
            Globals.TitleForm = "Báo cáo tổng hợp thu tiền viện phí ngoại trú".ToUpper();
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportBaoCaoThuTienVienPhiNgoaiTru_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportBaoCaoThuTienVienPhiNgoaiTru_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }
        //▲====: #029
        //▼====: #030
        private void ReportBaoCaoHoanTienVienPhiNgoaiTru_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            innerVm.eItem = ReportName.BC_HOAN_TIEN_VIEN_PHI_NGOAI_TRU;
            innerVm.LoadListStaff((byte)StaffRegistrationType.INSURANCE);
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.vBaoCaoVienPhiBHYT = false;
            innerVm.IsShowPaymentMode = true; //20210127 QTD thêm điều kiện bật Combobox Hình thức TT
            innerVm.Case = (int)AllLookupValues.HIType.All;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportBaoCaoHoanTienVienPhiNgoaiTru(object source)
        {
            Globals.TitleForm = "Báo cáo tổng hợp hủy, hoàn tiền viện phí ngoại trú".ToUpper();
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportBaoCaoHoanTienVienPhiNgoaiTru_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportBaoCaoHoanTienVienPhiNgoaiTru_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }
        //▲====: #030
        //▼====: #031
        private void ReportBaoCaoThuTienTamUngNoiTru_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            innerVm.eItem = ReportName.BC_THU_TIEN_TAM_UNG_NOI_TRU;
            innerVm.LoadListStaff((byte)StaffRegistrationType.INSURANCE);
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.vBaoCaoVienPhiBHYT = false;
            innerVm.IsShowPaymentMode = true; //20210127 QTD thêm điều kiện bật Combobox Hình thức TT
            innerVm.Case = (int)AllLookupValues.HIType.All;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportBaoCaoThuTienTamUngNoiTru(object source)
        {
            Globals.TitleForm = "Báo cáo tổng hợp thu tiền tạm ứng nội trú".ToUpper();
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportBaoCaoThuTienTamUngNoiTru_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportBaoCaoThuTienTamUngNoiTru_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }
        //▲====: #031
        //▼====: #032
        private void ReportBaoCaoHoanTienTamUngNoiTru_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            innerVm.eItem = ReportName.BC_HOAN_TIEN_TAM_UNG_NOI_TRU;
            innerVm.LoadListStaff((byte)StaffRegistrationType.INSURANCE);
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.vBaoCaoVienPhiBHYT = false;
            innerVm.IsShowPaymentMode = true; //20210127 QTD thêm điều kiện bật Combobox Hình thức TT
            innerVm.Case = (int)AllLookupValues.HIType.All;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportBaoCaoHoanTienTamUngNoiTru(object source)
        {
            Globals.TitleForm = "Báo cáo tổng hợp hủy, hoàn tiền tạm ứng nội trú".ToUpper();
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportBaoCaoHoanTienTamUngNoiTru_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportBaoCaoHoanTienTamUngNoiTru_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }
        //▲====: #032
        #endregion

        #region Kế toán BHYT
        public void ExportExcelPCLWaitResultCmd()
        {
            Globals.TitleForm = "Báo cáo XN chờ kết quả";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ExportExcelPCLWaitResultCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ExportExcelPCLWaitResultCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ExportExcelPCLWaitResultCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.ExportExcel_LabWaitResult;
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.IsExportExcel4210 = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp26a;
            UnitVM.authorization();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        #endregion

        //▼====: #028
        private bool _gThongKeBV = true;
        private bool _bIsWareHouseAccountant = true;
        private bool _bCreateEInvoice = true;
        private bool _bCreateOutPtTransactionFinalization = true;
        private bool _gKeHoachTH = true;
        private bool _gDuocLS = true;
        private bool _bDLS_BCQLXuatThuocKhoaPhong = true;
        private bool _bDLS_BCToaThuocHangNgay = true;
        private bool _bRptDrugMedDept = true;
        private bool _bInPtAdmDisStatistics = true;
        private bool _bSoXetNghiem = true;

        private bool _bThongTu372019 = true;
        private bool _bKHTH_BCTinhHinhDichLay = true;
        private bool _bKHTH_TK_TinhHinhKCB = true;
        private bool _bBCCongTacKCB = true;
        private bool _bTinhHinhHoatDongCLS_KHTH = true;
        private bool _bTinhHinhHoatDongDV_KHTH = true;
        private bool _bBCChiTietDVCLS_KHTH = true;
        private bool _bBaoCaoDVDaThucHien = true;
        private bool _bBC_DS_DVKyThuatTrenHIS_KHTH = true;
        private bool _bRptPtAreTreated = true;
        private bool _bBCDsachBNXuatVien_KHTH = true;
        private bool _bBCDsachBsiChiDinhNhapVien_KHTH = true;
        private bool _bPatientDecease = true;
        private bool _bBCBenhNhanPTT = true;
        private bool _bYVu = true;
        private bool _bBCChuyenTuyen = true;
        private bool _bCheckMedicalFileForKHTH = true;
        private bool _bThongTu56 = true;
        private bool _bBC_DMKyThuatMoi_KHTH = true;
        private bool _bBCCongSuatGiuongBenh = true;
        private bool _bBCGiaoBanTheoKhoaLS_KHTH = true;

        private bool _bBaoCaoVPMTreEm = true;
        private bool _bInOutValueReport = true;
        private bool _bCreateFastReport = true;
        private bool _bCreateDQGReport = true;
        private bool _bExportExcel4210 = true;
        private bool _bExportExcelPCLWaitResult = true;
        private bool _bOutwardDrugsByStaffStatistic = true;
        private bool _bOutwardDrugsByStaffStatisticDetails = true;

        public bool gThongKeBV
        {
            get
            {
                return _gThongKeBV;
            }
            set
            {
                if (_gThongKeBV == value)
                    return;
                _gThongKeBV = value;
                NotifyOfPropertyChange(() => gThongKeBV);
            }
        }

        public bool bIsWareHouseAccountant
        {
            get
            {
                return _bIsWareHouseAccountant;
            }
            set
            {
                if (_bIsWareHouseAccountant == value)
                    return;
                _bIsWareHouseAccountant = value;
                NotifyOfPropertyChange(() => bIsWareHouseAccountant);
            }
        }

        public bool bCreateEInvoice
        {
            get
            {
                return _bCreateEInvoice;
            }
            set
            {
                if (_bCreateEInvoice == value)
                    return;
                _bCreateEInvoice = value;
                NotifyOfPropertyChange(() => bCreateEInvoice);
            }
        }

        public bool bCreateOutPtTransactionFinalization
        {
            get
            {
                return _bCreateOutPtTransactionFinalization;
            }
            set
            {
                if (_bCreateOutPtTransactionFinalization == value)
                    return;
                _bCreateOutPtTransactionFinalization = value;
                NotifyOfPropertyChange(() => bCreateOutPtTransactionFinalization);
            }
        }

        public bool gDuocLS
        {
            get
            {
                return _gDuocLS;
            }
            set
            {
                if (_gDuocLS == value)
                    return;
                _gDuocLS = value;
                NotifyOfPropertyChange(() => gDuocLS);
            }
        }

        public bool bDLS_BCQLXuatThuocKhoaPhong
        {
            get
            {
                return _bDLS_BCQLXuatThuocKhoaPhong;
            }
            set
            {
                if (_bDLS_BCQLXuatThuocKhoaPhong == value)
                    return;
                _bDLS_BCQLXuatThuocKhoaPhong = value;
                NotifyOfPropertyChange(() => bDLS_BCQLXuatThuocKhoaPhong);
            }
        }

        public bool bDLS_BCToaThuocHangNgay
        {
            get
            {
                return _bDLS_BCToaThuocHangNgay;
            }
            set
            {
                if (_bDLS_BCToaThuocHangNgay == value)
                    return;
                _bDLS_BCToaThuocHangNgay = value;
                NotifyOfPropertyChange(() => bDLS_BCToaThuocHangNgay);
            }
        }

        public bool bRptDrugMedDept
        {
            get
            {
                return _bRptDrugMedDept;
            }
            set
            {
                if (_bRptDrugMedDept == value)
                    return;
                _bRptDrugMedDept = value;
                NotifyOfPropertyChange(() => bRptDrugMedDept);
            }
        }

        public bool bInPtAdmDisStatistics
        {
            get
            {
                return _bInPtAdmDisStatistics;
            }
            set
            {
                if (_bInPtAdmDisStatistics == value)
                    return;
                _bInPtAdmDisStatistics = value;
                NotifyOfPropertyChange(() => bInPtAdmDisStatistics);
            }
        }

        public bool bSoXetNghiem
        {
            get
            {
                return _bSoXetNghiem;
            }
            set
            {
                if (_bSoXetNghiem == value)
                    return;
                _bSoXetNghiem = value;
                NotifyOfPropertyChange(() => bSoXetNghiem);
            }
        }

        public bool gKeHoachTH
        {
            get
            {
                return _gKeHoachTH;
            }
            set
            {
                if (_gKeHoachTH == value)
                    return;
                _gKeHoachTH = value;
                NotifyOfPropertyChange(() => gKeHoachTH);
            }
        }

        public bool bThongTu372019
        {
            get
            {
                return _bThongTu372019;
            }
            set
            {
                if (_bThongTu372019 == value)
                    return;
                _bThongTu372019 = value;
                NotifyOfPropertyChange(() => bThongTu372019);
            }
        }

        public bool bKHTH_BCTinhHinhDichLay
        {
            get
            {
                return _bKHTH_BCTinhHinhDichLay;
            }
            set
            {
                if (_bKHTH_BCTinhHinhDichLay == value)
                    return;
                _bKHTH_BCTinhHinhDichLay = value;
                NotifyOfPropertyChange(() => bKHTH_BCTinhHinhDichLay);
            }
        }

        public bool bKHTH_TK_TinhHinhKCB
        {
            get
            {
                return _bKHTH_TK_TinhHinhKCB;
            }
            set
            {
                if (_bKHTH_TK_TinhHinhKCB == value)
                    return;
                _bKHTH_TK_TinhHinhKCB = value;
                NotifyOfPropertyChange(() => bKHTH_TK_TinhHinhKCB);
            }
        }

        public bool bBCCongTacKCB
        {
            get
            {
                return _bBCCongTacKCB;
            }
            set
            {
                if (_bBCCongTacKCB == value)
                    return;
                _bBCCongTacKCB = value;
                NotifyOfPropertyChange(() => bBCCongTacKCB);
            }
        }

        public bool bTinhHinhHoatDongCLS_KHTH
        {
            get
            {
                return _bTinhHinhHoatDongCLS_KHTH;
            }
            set
            {
                if (_bTinhHinhHoatDongCLS_KHTH == value)
                    return;
                _bTinhHinhHoatDongCLS_KHTH = value;
                NotifyOfPropertyChange(() => bTinhHinhHoatDongCLS_KHTH);
            }
        }

        public bool bTinhHinhHoatDongDV_KHTH
        {
            get
            {
                return _bTinhHinhHoatDongDV_KHTH;
            }
            set
            {
                if (_bTinhHinhHoatDongDV_KHTH == value)
                    return;
                _bTinhHinhHoatDongDV_KHTH = value;
                NotifyOfPropertyChange(() => bTinhHinhHoatDongDV_KHTH);
            }
        }

        public bool bBCChiTietDVCLS_KHTH
        {
            get
            {
                return _bBCChiTietDVCLS_KHTH;
            }
            set
            {
                if (_bBCChiTietDVCLS_KHTH == value)
                    return;
                _bBCChiTietDVCLS_KHTH = value;
                NotifyOfPropertyChange(() => bBCChiTietDVCLS_KHTH);
            }
        }

        public bool bBaoCaoDVDaThucHien
        {
            get
            {
                return _bBaoCaoDVDaThucHien;
            }
            set
            {
                if (_bBaoCaoDVDaThucHien == value)
                    return;
                _bBaoCaoDVDaThucHien = value;
                NotifyOfPropertyChange(() => bBaoCaoDVDaThucHien);
            }
        }

        public bool bBC_DS_DVKyThuatTrenHIS_KHTH
        {
            get
            {
                return _bBC_DS_DVKyThuatTrenHIS_KHTH;
            }
            set
            {
                if (_bBC_DS_DVKyThuatTrenHIS_KHTH == value)
                    return;
                _bBC_DS_DVKyThuatTrenHIS_KHTH = value;
                NotifyOfPropertyChange(() => bBC_DS_DVKyThuatTrenHIS_KHTH);
            }
        }

        public bool bRptPtAreTreated
        {
            get
            {
                return _bRptPtAreTreated;
            }
            set
            {
                if (_bRptPtAreTreated == value)
                    return;
                _bRptPtAreTreated = value;
                NotifyOfPropertyChange(() => bRptPtAreTreated);
            }
        }

        public bool bBCDsachBNXuatVien_KHTH
        {
            get
            {
                return _bBCDsachBNXuatVien_KHTH;
            }
            set
            {
                if (_bBCDsachBNXuatVien_KHTH == value)
                    return;
                _bBCDsachBNXuatVien_KHTH = value;
                NotifyOfPropertyChange(() => bBCDsachBNXuatVien_KHTH);
            }
        }

        public bool bBCDsachBsiChiDinhNhapVien_KHTH
        {
            get
            {
                return _bBCDsachBsiChiDinhNhapVien_KHTH;
            }
            set
            {
                if (_bBCDsachBsiChiDinhNhapVien_KHTH == value)
                    return;
                _bBCDsachBsiChiDinhNhapVien_KHTH = value;
                NotifyOfPropertyChange(() => bBCDsachBsiChiDinhNhapVien_KHTH);
            }
        }

        public bool bPatientDecease
        {
            get
            {
                return _bPatientDecease;
            }
            set
            {
                if (_bPatientDecease == value)
                    return;
                _bPatientDecease = value;
                NotifyOfPropertyChange(() => bPatientDecease);
            }
        }

        public bool bBCBenhNhanPTT
        {
            get
            {
                return _bBCBenhNhanPTT;
            }
            set
            {
                if (_bBCBenhNhanPTT == value)
                    return;
                _bBCBenhNhanPTT = value;
                NotifyOfPropertyChange(() => bBCBenhNhanPTT);
            }
        }

        public bool bYVu
        {
            get
            {
                return _bYVu;
            }
            set
            {
                if (_bYVu == value)
                    return;
                _bYVu = value;
                NotifyOfPropertyChange(() => bYVu);
            }
        }

        public bool bBCChuyenTuyen
        {
            get
            {
                return _bBCChuyenTuyen;
            }
            set
            {
                if (_bBCChuyenTuyen == value)
                    return;
                _bBCChuyenTuyen = value;
                NotifyOfPropertyChange(() => bBCChuyenTuyen);
            }
        }

        public bool bCheckMedicalFileForKHTH
        {
            get
            {
                return _bCheckMedicalFileForKHTH;
            }
            set
            {
                if (_bCheckMedicalFileForKHTH == value)
                    return;
                _bCheckMedicalFileForKHTH = value;
                NotifyOfPropertyChange(() => bCheckMedicalFileForKHTH);
            }
        }

        public bool bThongTu56
        {
            get
            {
                return _bThongTu56;
            }
            set
            {
                if (_bThongTu56 == value)
                    return;
                _bThongTu56 = value;
                NotifyOfPropertyChange(() => bThongTu56);
            }
        }

        public bool bBC_DMKyThuatMoi_KHTH
        {
            get
            {
                return _bBC_DMKyThuatMoi_KHTH;
            }
            set
            {
                if (_bBC_DMKyThuatMoi_KHTH == value)
                    return;
                _bBC_DMKyThuatMoi_KHTH = value;
                NotifyOfPropertyChange(() => bBC_DMKyThuatMoi_KHTH);
            }
        }

        public bool bBCCongSuatGiuongBenh
        {
            get
            {
                return _bBCCongSuatGiuongBenh;
            }
            set
            {
                if (_bBCCongSuatGiuongBenh == value)
                    return;
                _bBCCongSuatGiuongBenh = value;
                NotifyOfPropertyChange(() => bBCCongSuatGiuongBenh);
            }
        }

        public bool bBCGiaoBanTheoKhoaLS_KHTH
        {
            get
            {
                return _bBCGiaoBanTheoKhoaLS_KHTH;
            }
            set
            {
                if (_bBCGiaoBanTheoKhoaLS_KHTH == value)
                    return;
                _bBCGiaoBanTheoKhoaLS_KHTH = value;
                NotifyOfPropertyChange(() => bBCGiaoBanTheoKhoaLS_KHTH);
            }
        }

        public bool bBaoCaoVPMTreEm
        {
            get
            {
                return _bBaoCaoVPMTreEm;
            }
            set
            {
                if (_bBaoCaoVPMTreEm == value)
                    return;
                _bBaoCaoVPMTreEm = value;
                NotifyOfPropertyChange(() => bBaoCaoVPMTreEm);
            }
        }

        public bool bInOutValueReport
        {
            get
            {
                return _bInOutValueReport;
            }
            set
            {
                if (_bInOutValueReport == value)
                    return;
                _bInOutValueReport = value;
                NotifyOfPropertyChange(() => bInOutValueReport);
            }
        }

        public bool bCreateFastReport
        {
            get
            {
                return _bCreateFastReport;
            }
            set
            {
                if (_bCreateFastReport == value)
                    return;
                _bCreateFastReport = value;
                NotifyOfPropertyChange(() => bCreateFastReport);
            }
        }

        public bool bCreateDQGReport
        {
            get
            {
                return _bCreateDQGReport;
            }
            set
            {
                if (_bCreateDQGReport == value)
                    return;
                _bCreateDQGReport = value;
                NotifyOfPropertyChange(() => bCreateDQGReport);
            }
        }

        public bool bExportExcel4210
        {
            get
            {
                return _bExportExcel4210;
            }
            set
            {
                if (_bExportExcel4210 == value)
                    return;
                _bExportExcel4210 = value;
                NotifyOfPropertyChange(() => bExportExcel4210);
            }
        }

        public bool bExportExcelPCLWaitResult
        {
            get
            {
                return _bExportExcelPCLWaitResult;
            }
            set
            {
                if (_bExportExcelPCLWaitResult == value)
                    return;
                _bExportExcelPCLWaitResult = value;
                NotifyOfPropertyChange(() => bExportExcelPCLWaitResult);
            }
        }

        public bool bOutwardDrugsByStaffStatistic
        {
            get
            {
                return _bOutwardDrugsByStaffStatistic;
            }
            set
            {
                if (_bOutwardDrugsByStaffStatistic == value)
                    return;
                _bOutwardDrugsByStaffStatistic = value;
                NotifyOfPropertyChange(() => bOutwardDrugsByStaffStatistic);
            }
        }

        public bool bOutwardDrugsByStaffStatisticDetails
        {
            get
            {
                return _bOutwardDrugsByStaffStatisticDetails;
            }
            set
            {
                if (_bOutwardDrugsByStaffStatisticDetails == value)
                    return;
                _bOutwardDrugsByStaffStatisticDetails = value;
                NotifyOfPropertyChange(() => bOutwardDrugsByStaffStatisticDetails);
            }
        }
        //▲====: #028

        private bool _bDLS_BCThongTinDanhMucThuoc = true;
        private bool _bDLS_BCKiemTraLichSuKCB = true;
        private bool _bBCSuDungToanBV = true;
        private bool _bBCGiaoBanKHTH = true;
        private bool _bBCQuanLyKiemDuyetHoSo_KHTH = true;

        public bool bDLS_BCThongTinDanhMucThuoc
        {
            get
            {
                return _bDLS_BCThongTinDanhMucThuoc;
            }
            set
            {
                if (_bDLS_BCThongTinDanhMucThuoc == value)
                    return;
                _bDLS_BCThongTinDanhMucThuoc = value;
                NotifyOfPropertyChange(() => bDLS_BCThongTinDanhMucThuoc);
            }
        }

        public bool bDLS_BCKiemTraLichSuKCB
        {
            get
            {
                return _bDLS_BCKiemTraLichSuKCB;
            }
            set
            {
                if (_bDLS_BCKiemTraLichSuKCB == value)
                    return;
                _bDLS_BCKiemTraLichSuKCB = value;
                NotifyOfPropertyChange(() => bDLS_BCKiemTraLichSuKCB);
            }
        }

        public bool bBCSuDungToanBV
        {
            get
            {
                return _bBCSuDungToanBV;
            }
            set
            {
                if (_bBCSuDungToanBV == value)
                    return;
                _bBCSuDungToanBV = value;
                NotifyOfPropertyChange(() => bBCSuDungToanBV);
            }
        }

        public bool bBCGiaoBanKHTH
        {
            get
            {
                return _bBCGiaoBanKHTH;
            }
            set
            {
                if (_bBCGiaoBanKHTH == value)
                    return;
                _bBCGiaoBanKHTH = value;
                NotifyOfPropertyChange(() => bBCGiaoBanKHTH);
            }
        }

        public bool bBCQuanLyKiemDuyetHoSo_KHTH
        {
            get
            {
                return _bBCQuanLyKiemDuyetHoSo_KHTH;
            }
            set
            {
                if (_bBCQuanLyKiemDuyetHoSo_KHTH == value)
                    return;
                _bBCQuanLyKiemDuyetHoSo_KHTH = value;
                NotifyOfPropertyChange(() => bBCQuanLyKiemDuyetHoSo_KHTH);
            }
        }

        //▼====: #036
        public bool _bBCDanhSachBNDTNT = true;
        public bool bBCDanhSachBNDTNT
        {
            get
            {
                return _bBCDanhSachBNDTNT;
            }
            set
            {
                if (_bBCDanhSachBNDTNT == value)
                    return;
                _bBCDanhSachBNDTNT = value;
                NotifyOfPropertyChange(() => bBCDanhSachBNDTNT);
            }
        }
        public void BCDanhSachBNDTNTCmd()
        {
            Globals.TitleForm = "Báo cáo danh sách Bệnh nhân điều trị ngoại trú";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCDanhSachBNDTNTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCDanhSachBNDTNTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCDanhSachBNDTNTCmd_In()
        {
            Globals.PageName = "Báo cáo danh sách Bệnh nhân điều trị ngoại trú";
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCDanhSachBNDTNT_KHTH;
            reportVm.strHienThi = Globals.TitleForm;

            reportVm.mDepartment = true;
            reportVm.ShowLocationSelect = true;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.IsShowOutPtTreatmentStatus = true;
            reportVm.ShowOutPtTreatmentType = true;
            reportVm.mXemChiTiet = false;
            reportVm.mXemIn = false;
            //reportVm.IsReportForKHTH = true;
            reportVm.IsDateTime = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #036
        //▲====: #037
        private void FileManCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IMedicalFileManagement>();
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void FileManCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1968_G1_QLyHS.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FileManCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FileManCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void ShelfManCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IRefShelf>();
            regModule.MainContent  = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void ShelfManCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1987_QLyKhoKe.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ShelfManCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ShelfManCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void FileInportCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IRefShelfImportFile>();
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void FileInportCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1999_G1_DatHSoVaoKe.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FileInportCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FileInportCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void FileCheckInCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IRefMedicalFileCheckIn>();
            regVm.IsCheckIn = true;
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void FileCheckInCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1994_G1_NhapHSo.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FileCheckInCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FileCheckInCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void FileCheckOutCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IRefMedicalFileCheckIn>();
            regVm.IsCheckIn = false;
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void FileCheckOutCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1995_G1_XuatHSo.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FileCheckOutCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FileCheckOutCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void FileCodePrintCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IRefMedicalFileCodePrint>();
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void FileCodePrintCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1988_InMaHS.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FileCodePrintCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FileCodePrintCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void FileCheckInFromRegCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IGetMedicalFileFromRegistration>();
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void FileCheckInFromRegCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1989_XuatHSTuDK.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FileCheckInFromRegCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FileCheckInFromRegCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void FileCodeHistoryCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IMedicalFileCheckInHistory>();
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void FileCodeHistoryCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2042_G1_LSGNHoSo.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FileCodeHistoryCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FileCodeHistoryCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void OutFileManCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IOutMedicalFileManagement>();
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void OutFileManCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3114_G1_OutFileManager.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutFileManCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutFileManCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private bool _bFileManager = true;
        public bool bFileManager
        {
            get
            {
                return _bFileManager;
            }
            set
            {
                if (_bFileManager == value)
                    return;
                _bFileManager = value;
                NotifyOfPropertyChange(() => bFileManager);
            }
        }
        //▲====: #037
        //▼====: #038
        private void RowShelfManCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IRefRow>();
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void RowShelfManCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3274_G1_QLDayKeNgan.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RowShelfManCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RowShelfManCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void FileCheckOut_V2Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IRefMedicalFileCheckOut>();
            regVm.IsCheckIn = false;
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void FileCheckOut_V2Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3287_G1_QLMuonHS.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FileCheckOut_V2Cmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FileCheckOut_V2Cmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void FileCheckIn_V2Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var regVm = Globals.GetViewModel<IRefMedicalFileCheckOut>();
            regVm.IsCheckIn = true;
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void FileCheckIn_V2Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3288_G1_QLTraHS.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FileCheckIn_V2Cmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FileCheckIn_V2Cmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #038
        //▼====: #039
        private bool _mCheckMedicalFile_Tim = true;
        private bool _mCheckMedicalFile_DanhMucICD = true;
        private bool _mCheckMedicalFile_Luu = true;
        private bool _mCheckMedicalFile_TraHS = true;
        private bool _mCheckMedicalFile_Duyet = true; 
        private bool _mCheckMedicalFile_MoKhoa = true;
        private bool _mCheckMedicalFile_DLS_Save = true;
        private bool _mCheckMedicalFile_DLS_Duyet = true;
        private bool _mCheckMedicalFile_DLS_TraHS = true;
        public bool mCheckMedicalFile_Tim
        {
            get
            {
                return _mCheckMedicalFile_Tim;
            }
            set
            {
                if (_mCheckMedicalFile_Tim == value)
                    return;
                _mCheckMedicalFile_Tim = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_Tim);
            }
        }
        public bool mCheckMedicalFile_DanhMucICD
        {
            get
            {
                return _mCheckMedicalFile_DanhMucICD;
            }
            set
            {
                if (_mCheckMedicalFile_DanhMucICD == value)
                    return;
                _mCheckMedicalFile_DanhMucICD = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_DanhMucICD);
            }
        }
        public bool mCheckMedicalFile_Luu
        {
            get
            {
                return _mCheckMedicalFile_Luu;
            }
            set
            {
                if (_mCheckMedicalFile_Luu == value)
                    return;
                _mCheckMedicalFile_Luu = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_Luu);
            }
        }
        public bool mCheckMedicalFile_TraHS
        {
            get
            {
                return _mCheckMedicalFile_TraHS;
            }
            set
            {
                if (_mCheckMedicalFile_TraHS == value)
                    return;
                _mCheckMedicalFile_TraHS = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_TraHS);
            }
        }
        public bool mCheckMedicalFile_Duyet
        {
            get
            {
                return _mCheckMedicalFile_Duyet;
            }
            set
            {
                if (_mCheckMedicalFile_Duyet == value)
                    return;
                _mCheckMedicalFile_Duyet = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_Duyet);
            }
        }
        public bool mCheckMedicalFile_MoKhoa
        {
            get
            {
                return _mCheckMedicalFile_MoKhoa;
            }
            set
            {
                if (_mCheckMedicalFile_MoKhoa == value)
                    return;
                _mCheckMedicalFile_MoKhoa = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_MoKhoa);
            }
        }
        public bool mCheckMedicalFile_DLS_Save
        {
            get
            {
                return _mCheckMedicalFile_DLS_Save;
            }
            set
            {
                if (_mCheckMedicalFile_DLS_Save == value)
                    return;
                _mCheckMedicalFile_DLS_Save = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_DLS_Save);
            }
        }
        public bool mCheckMedicalFile_DLS_Duyet
        {
            get
            {
                return _mCheckMedicalFile_DLS_Duyet;
            }
            set
            {
                if (_mCheckMedicalFile_DLS_Duyet == value)
                    return;
                _mCheckMedicalFile_DLS_Duyet = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_DLS_Duyet);
            }
        }
        public bool mCheckMedicalFile_DLS_TraHS
        {
            get
            {
                return _mCheckMedicalFile_DLS_TraHS;
            }
            set
            {
                if (_mCheckMedicalFile_DLS_TraHS == value)
                    return;
                _mCheckMedicalFile_DLS_TraHS = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_DLS_TraHS);
            }
        }
        public void AuthorityOperation()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mCheckMedicalFile_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mCheckMedicalFileForKHTH,
                                               (int)oTransaction_ManagementEx.mCheckMedicalFile_Tim, (int)ePermission.mView);
            mCheckMedicalFile_DanhMucICD = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mCheckMedicalFileForKHTH,
                                               (int)oTransaction_ManagementEx.mCheckMedicalFile_DanhMucICD, (int)ePermission.mView);
            mCheckMedicalFile_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mCheckMedicalFileForKHTH,
                                               (int)oTransaction_ManagementEx.mCheckMedicalFile_Luu, (int)ePermission.mView);
            mCheckMedicalFile_TraHS = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mCheckMedicalFileForKHTH,
                                               (int)oTransaction_ManagementEx.mCheckMedicalFile_TraHS, (int)ePermission.mView);
            mCheckMedicalFile_Duyet = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mCheckMedicalFileForKHTH,
                                               (int)oTransaction_ManagementEx.mCheckMedicalFile_Duyet, (int)ePermission.mView);
            mCheckMedicalFile_MoKhoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mCheckMedicalFileForKHTH,
                                               (int)oTransaction_ManagementEx.mCheckMedicalFile_MoKhoa, (int)ePermission.mView);
            mCheckMedicalFile_DLS_Save = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mCheckMedicalFileForKHTH,
                                               (int)oTransaction_ManagementEx.mCheckMedicalFile_DLS_Save, (int)ePermission.mView);
            mCheckMedicalFile_DLS_Duyet = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mCheckMedicalFileForKHTH,
                                               (int)oTransaction_ManagementEx.mCheckMedicalFile_DLS_Duyet, (int)ePermission.mView);
            mCheckMedicalFile_DLS_TraHS = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mCheckMedicalFileForKHTH,
                                               (int)oTransaction_ManagementEx.mCheckMedicalFile_DLS_TraHS, (int)ePermission.mView);
        }
        //▲====: #039
        
        //▼==== #040
        private bool _mManagePersonnelReport = true;
        public bool mManagePersonnelReport
        {
            get
            {
                return _mManagePersonnelReport;
            }
            set
            {
                if (_mManagePersonnelReport == value)
                    return;
                _mManagePersonnelReport = value;
                NotifyOfPropertyChange(() => mManagePersonnelReport);
            }
        }

        private bool _mUserManipulation = true;
        public bool mUserManipulation
        {
            get
            {
                return _mUserManipulation;
            }
            set
            {
                if (_mUserManipulation == value)
                    return;
                _mUserManipulation = value;
                NotifyOfPropertyChange(() => mUserManipulation);
            }
        }

        public void UserManipulationCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3296_G1_BCThaoTacNguoiDung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                UserManipulationCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        UserManipulationCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void UserManipulationCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCThaoTacNguoiDung;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.isAllStaff = false;
            reportVm.mXemIn = false;
            reportVm.RptParameters.HideFindPatient = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #040

        //▼==== #041
        private bool _mBCThongKeHoSoDTNT = true;
        public bool mBCThongKeHoSoDTNT
        {
            get
            {
                return _mBCThongKeHoSoDTNT;
            }
            set
            {
                if (_mBCThongKeHoSoDTNT == value)
                    return;
                _mBCThongKeHoSoDTNT = value;
                NotifyOfPropertyChange(() => mBCThongKeHoSoDTNT);
            }
        }
        
        public void BCThongKeHoSoDTNTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3297_G1_BCThongKeHoSoDTNT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThongKeHoSoDTNTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThongKeHoSoDTNTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCThongKeHoSoDTNTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCThongKeHoSoDTNT;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.isAllStaff = false;
            reportVm.mXemIn = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.ShowYNOutPtTreatmentCode= true;
            reportVm.ShowYNOutPtTreatmentType = true;
            reportVm.ShowYNOutPtTreatmentProgram = true;
            reportVm.ShowYNOutPtTreatmentFinal = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #041

        //▼==== #042
        private bool _mBaoCaoKSNK = true;
        public bool mBaoCaoKSNK
        {
            get
            {
                return _mBaoCaoKSNK;
            }
            set
            {
                if (_mBaoCaoKSNK == value)
                    return;
                _mBaoCaoKSNK = value;
                NotifyOfPropertyChange(() => mBaoCaoKSNK);
            }
        }

        public void BaoCaoKSNKCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3306_G1_BaoCaoKSNK;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoKSNKCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoKSNKCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BaoCaoKSNKCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BaoCaoKSNK;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.isAllStaff = false;
            reportVm.mDepartment = true;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.RptParameters.ShowFindInfectionControl = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #042
        private void DLS_BCKhamBenhNgoaiTruCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_DLS_KhamBenhNgoaiTru;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.isAllStaff = false;
            reportVm.mXemIn = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DLS_BCKhamBenhNgoaiTruCmd(object source)
        {
            Globals.TitleForm = "Báo cáo khám bệnh BN ngoại trú và điều trị ngoại trú";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DLS_BCKhamBenhNgoaiTruCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DLS_BCKhamBenhNgoaiTruCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void DLS_BCCLSNgoaiTruCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_DLS_CLSNgoaiTru;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.isAllStaff = false;
            reportVm.mXemIn = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DLS_BCCLSNgoaiTruCmd(object source)
        {
            Globals.TitleForm = "Báo cáo CLS BN ngoại trú và điều trị ngoại trú";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DLS_BCCLSNgoaiTruCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DLS_BCCLSNgoaiTruCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void DLS_BCThuocNgoaiTruCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_DLS_ThuocNgoaiTru;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.isAllStaff = false;
            reportVm.mXemIn = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void DLS_BCThuocNgoaiTruCmd(object source)
        {
            Globals.TitleForm = "Báo cáo Thuốc sử dụng trên BN ngoại trú và điều trị ngoại trú";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DLS_BCThuocNgoaiTruCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DLS_BCThuocNgoaiTruCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
    }
}
