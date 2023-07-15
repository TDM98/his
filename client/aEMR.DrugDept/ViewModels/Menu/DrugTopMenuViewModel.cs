using System.ComponentModel.Composition;
using System.Windows;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Windows.Controls;
using eHCMSLanguage;
using Castle.Windsor;
/*
 * 20170810 #001 CMN:   Added Bid Service
 * 20170821 #002 CMN:   Added AdjustClinicPrice Service
 * 20171226 #003 CMN:   Added Temp Inward Report
 * 20171227 #004 CMN:   Show DrugCategory Combobox on MatItems in InOutGeneralReport
 * 20180806 #005 TBL:   Added Generic
 * 20180921 #006 TTM: 
 * 20181002 #007 TBL:   BM 0000070. Fix khi tu QL Hoat chat qua cac QL khac thi hien thong bao loi
 * 20181013 #008 TTM:   Thêm biến điều khiển Visible cho TopMenu
 * 20181124 #009 TTM:   BM 0005309: Tạo mới màn hình nhập trả hàng từ kho BHYT dựa trên màn hình Nhập trả từ khoa Phòng. 
 *                      Bổ sung trong các hàm gọi màn hình Nhập trả từ khoa Phòng (thuốc, y cụ, hóa chất) biến nhận biết đang vào màn hình nào để visible control cho chính xác (vNhapTraKhoBHYT)
 * 20181219 #010 TTM:   BM 0005443: Tạo mới màn hình duyệt hàng từ phiếu lĩnh (phiếu yêu cầu kho BHYT - nhà thuốc).
 * 20190514 #011 TNHX:  BM 0006872: Create BC_KhangSinh report
 * 20190515 #012 TNHX:  BM 0006872: Create BC_SuDungThuoc, BC_SuDungHoaChat report
 * 20190604 #013 TTM:   BM 0011781: Thêm dự trù cho các kho mới
 * 20190623 #014 TTM:   BM 0011881: Bổ sung đặt hàng cho các kho mới
 * 20191004 #015 TNHX:  BM 0017414 : Add report Z2847_G1_BCThuocSapHetHanDung
 * 20191005 #016 TNHX:  BM 0017414 : Add report KT_BCHangKhongXuat
 * 20191029 #017 TNHX:  BM 0006884 : Add report TT22_BienBanKiemKe + Group by report by TT22
 * 20191104 #018 TNHX:  Move Z1608_G1_QLyCCDThuoc from Pharmacy to DrugDept
 * 20210821 #019 QTD:   Bổ sung loại dinh dưỡng
 * 20211105 #020 QTD:   Bổ sung thêm quyền Duyệt phiếu cho các loại Máu, Vaccine, VTTH, VTYTTH, Thanh trùng, VPP
 * 20211112 #021 QTD:   Bổ sung thêm quyền Nhập NCC, Nhập trả từ kho phòng, Nhập nội bộ cho các loại Máu, Vaccine, VTTH, VTYTTH, Thanh Trùng, VPP
 * 20211113 #022 QTD:   Bổ sung thêm quyền màn hình Xuất, Dự trù, Đặt hàng,Bảng giá bán, BC XNT, BC xuất, Thẻ kho, BC nhập, Bảng giá bán cho các loại Máu, Vaccine, VTTH, VTYTTH, Thanh Trùng, VPP
 *                      BC hàng không xuất, BC thuốc hết hạn dùng, BC toa treo
 * 20211223 #023 TNHX:  803 Thêm báo cao bn covid cho khoa dược/ kế toán tổng hợp
 * 20220211 #024 QTD:   Thêm BC NXT kho cơ số
 * 20220513 #025 QTD:   Bổ sung quyền QL thầu
 * 20221111 #026 QTD:   Thêm Màn hình xuất trả hàng ký gửi
 * 20221129 #027 BLQ:   Thêm menu quản lý đơn thuốc điện tử
 * 20230304 #028 DatTB:   Báo cáo xuất thuốc Khoa/Phòng
 * 20230526 #029 DatTB: Thêm Báo cáo thống kê đơn thuốc điện tử
 */

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugTopMenu))]
    public class DrugTopMenuViewModel : Conductor<object>, IDrugTopMenu
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public DrugTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
            AuthorityOperation();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            Globals.IsAdmission = true;
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        ILeftMenuView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as ILeftMenuView;
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
        }
        //▼====== #006: TopMenu không cần hàm này, nên comment hàm này ra và delete tất cả những chỗ nào set hàm này xài.
        //private void SetHyperlinkSelectedStyle(Button lnk)
        //{
        //    if (_currentView != null)
        //    {
        //        _currentView.ResetMenuColor();
        //    }
        //    if (lnk != null)
        //    {
        //        lnk.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
        //    }
        //}
        //▲====== #006
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            //▼====== #008
            #region VisibleNewAllGroupFunc
            gfNhapHangAll = gf1NhapHangTuNCC || gf1NhapTraTuKhoPhong || gf1NhapTraTuKhoPhong || gfPhanBoPhi || gfSapNhapHangKyGoi || gfDeNghiThanhToan;
            gfXuatHangAll = gfXuatNoiBo || gfXuatHangKyGoi || gfXuatTheoToa || gfTraHang;
            gfQuanLyGiaAll = gfThangGiaBan || gfGiaNCC || gfBangGiaBan || gfAdjustOutPrice;
            //gfBaoCaoThongKeAll = gf1PhieuNhapKho || gf1NhapXuatTon || gf1NhapXuatTon || gf1TheKho || gf1TheoDoiCongNo || gf1BaoCaoXuat || gf1BaoCaoNhap || gf1WatchOutQty || gf1BaoCaoNam || gf1XuatKhoaPhong || gTempInwardReport;
            gfQuanLyDanhMucAll = gfQuanLyNhomHang || gfQuanLy || gfQuanLyDanhMuc;
            #endregion
            //▲====== #008
            #region Group Function Duyệt Phiếu Lĩnh Hàng
            mDuyetPhieuLinhHang_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_Thuoc);
            mDuyetPhieuLinhHang_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_YCu);
            mDuyetPhieuLinhHang_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_HoaChat);

            //gfDuyetPhieuLinhHang = mDuyetPhieuLinhHang_Thuoc || mDuyetPhieuLinhHang_YCu || mDuyetPhieuLinhHang_HoaChat;
            #endregion

            #region Xuất Nội Bộ
            mXNBThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_Thuoc);
            mXNBYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_YCu);
            mXNBHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_HoaChat);

            //gfXuatNoiBo = mXNBThuocCmd || mXNBYCuCmd || mXNBHoaChatCmd;
            #endregion

            #region Xuất Theo Toa

            mXuatTheoToa_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTheoToa_Thuoc);

            gfXuatTheoToa = mXuatTheoToa_Thuoc;
            #endregion
            #region Nhập hàng ký gửi
            mNhapHangKyGui_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                        (int)eKhoaDuoc.mNhapHangKyGui_Thuoc);
            mNhapHangKyGui_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangKyGui_YCu);
            mNhapHangKyGui = mNhapHangKyGui_Thuoc || mNhapHangKyGui_YCu; 
            #endregion
            #region Xuất hàng ký gởi
            mXuatHangKyGui_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatHangKyGui_Thuoc);
            mXuatHangKyGui_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatHangKyGui_YCu);
            mXuatHangKyGui_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatHangKyGui_HoaChat);

            gfXuatHangKyGoi = mXuatHangKyGui_Thuoc || mXuatHangKyGui_YCu || mXuatHangKyGui_HoaChat;
            #endregion

            #region Sáp nhập hàng ký gởi
            mSapNhapHangKyGui_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mSapNhapHangKyGui_Thuoc);
            mSapNhapHangKyGui_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mSapNhapHangKyGui_YCu);
            mSapNhapHangKyGui_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mSapNhapHangKyGui_HoaChat);

            gfSapNhapHangKyGoi = mSapNhapHangKyGui_Thuoc || mSapNhapHangKyGui_YCu || mSapNhapHangKyGui_HoaChat;

            #endregion


            #region Trả thuốc
            mReturnThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTraHang_Thuoc);
            mReturnYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTraHang_YCu);
            mReturnHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTraHang_HoaChat);

            gfTraHang = mReturnThuocCmd || mReturnYCuCmd || mReturnHoaChatCmd;
            #endregion

            mDemageThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatHuy_Thuoc);
            mDemageYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatHuy_YCu);
            mDemageHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatHuy_HoaChat);

            #region Dự Trù
            mEstimationDrugDeptCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_Thuoc);
            mEstimationYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_YCu);
            mEstimationChemicalCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_HoaChat);

            //gfDuTru = mEstimationDrugDeptCmd || mEstimationYCuCmd || mEstimationChemicalCmd || mEstimationDDuongCmd;
            #endregion

            #region Đặt hàng
            mOrderThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_Thuoc);
            mOrderYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_YCu);
            mOrderHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_HoaChat);
            //gfDatHang = mOrderThuocCmd || mOrderYCuCmd || mOrderHoaChatCmd;
            #endregion

            #region Nhập hàng
            mInwardDrugFromSupplierDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_Thuoc);
            mInwardDrugFromSupplierMedicalDeviceCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_YCu);
            mInwardDrugFromSupplierChemicalCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_HoaChat);
            /*▼====: #019*/
            mInwardDrugFromSupplierNutritionCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_DinhDuong);

            gf1NhapHangTuNCC = mInwardDrugFromSupplierDrugCmd || mInwardDrugFromSupplierMedicalDeviceCmd || mInwardDrugFromSupplierChemicalCmd || mInwardDrugFromSupplierNutritionCmd;
            /*▲====: #019*/
            mNhapTraTuKhoPhong_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapTraTuKhoPhong_Thuoc);
            mNhapTraTuKhoPhong_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapTraTuKhoPhong_YCu);
            mNhapTraTuKhoPhong_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapTraTuKhoPhong_HoaChat);

            //gf1NhapTraTuKhoPhong = mNhapTraTuKhoPhong_Thuoc || mNhapTraTuKhoPhong_YCu || mNhapTraTuKhoPhong_HoaChat;

            gfNhapHang = gf1NhapHangTuNCC || gf1NhapTraTuKhoPhong;

            #endregion

            #region Phân bổ phí
            mCostThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapPhanBoPhi_Thuoc);
            mCostYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapPhanBoPhi_YCu);
            mCostHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapPhanBoPhi_HoaChat);

            gfPhanBoPhi = mCostThuocCmd || mCostYCuCmd || mCostHoaChatCmd;
            #endregion

            #region Đề nghị thanh toán
            mSuggestThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc);
            mSuggestYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mPhieuDNThanhToan_YCu);
            mSuggestHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat);

            gfDeNghiThanhToan = mSuggestThuocCmd || mSuggestYCuCmd || mSuggestHoaChatCmd;
            #endregion

            #region Kiểm kê
            mKKThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_Thuoc);
            mKKYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_YCu);
            mKKHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_HoaChat);

            //gfKiemKe = mKKThuocCmd || mKKYCuCmd || mKKHoaChatCmd;
            #endregion

            #region Báo Cáo
            /*TMA 23/10/2017*/
            mNhapThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhap_Thuoc);
            mNhapYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoNhapXuat_YCu);
            mNhapHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoNhapXuat_HoaChat);
            //gf1BaoCaoNhap = mNhapThuocCmd || mNhapYCuCmd || mNhapHoaChatCmd;
            /*TMA 23/10/2017*/
            mXuatThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_Thuoc);
            mXuatYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_YCu);
            mXuatHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_HoaChat);

            //gf1BaoCaoXuat = mXuatThuocCmd || mXuatYCuCmd || mXuatHoaChatCmd;


            mWatchMedCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mWatchMedCmd);
            mWatchMatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mWatchMatCmd);
            mWatchLabCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mWatchLabCmd);

            gf1WatchOutQty = mWatchMedCmd || mWatchMatCmd || mWatchLabCmd;


            mNhapXuatTonThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_Thuoc);
            mNhapXuatTonYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_YCu);
            mNhapXuatTonHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_HoaChat);

            gf1NhapXuatTon = mNhapXuatTonThuocCmd || mNhapXuatTonYCuCmd || mNhapXuatTonHoaChatCmd || mNhapXuatTonDDuongCmd;

            mTheKhoThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_Thuoc);
            mTheKhoYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_YCu);
            mTheKhoHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_HoaChat);

            //gf1TheKho = mTheKhoThuocCmd || mTheKhoYCuCmd || mTheKhoHoaChatCmd || mTheKhoDDuongCmd;

            mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_Thuoc);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_YCu);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_HoaChat);

            //gf1PhieuNhapKho = mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd;

            mSuDungThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoSuDung_Thuoc);
            gf1BaoCaoNam = mSuDungThuocCmd;


            mThuocXuatDenKhoaCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhapXuatDenKhoaPhong_Thuoc);
            mYCuXuatDenKhoaCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhapXuatDenKhoaPhong_YCu);
            mHoaChatXuatDenKhoaCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhapXuatDenKhoaPhong_HoaChat);

            gf1XuatKhoaPhong = mThuocXuatDenKhoaCmd || mYCuXuatDenKhoaCmd || mHoaChatXuatDenKhoaCmd;


            //them moi ngay 28-07-2012
            mBaoCaoTheoDoiCongNo_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheoDoiCongNo_Thuoc);
            mBaoCaoTheoDoiCongNo_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheoDoiCongNo_YCu);
            mBaoCaoTheoDoiCongNo_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheoDoiCongNo_HoaChat);

            gf1TheoDoiCongNo = mBaoCaoTheoDoiCongNo_Thuoc || mBaoCaoTheoDoiCongNo_YCu || mBaoCaoTheoDoiCongNo_HoaChat;

            #endregion

            #region Quản lý

            mSupplierProductCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLNCCNSX_NCC);
            mDrugDeptPharmaceulCompanyCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLNCCNSX_NSX);
            mDrugDeptPharmaceulCompanySupplierCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLNCCNSX_NCCNSX);
            //mDrugDeptPharmaceulCompanyCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
            //                                             (int)eKhoaDuoc.mQLNCCNSX_NCCNSX);
            mDrugDeptUnitCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQuanLyDonViTinh);

            mRefGenDrugBHYTCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQuanLyDanhMucBHYT);

            gfQuanLy = mSupplierProductCmd || mDrugDeptPharmaceulCompanyCmd || mDrugDeptPharmaceulCompanySupplierCmd || mDrugDeptUnitCmd;
            #endregion

            #region Quản lý nhóm hàng
            mQLNhomHang_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLNhomHang_Thuoc);

            mQLNhomHang_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLNhomHang_YCu);

            mQLNhomHang_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLNhomHang_HoaChat);

            /*▼====: #005*/
            mQLNhomHang_HoatChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLNhomHang_HoatChat);
            /*▲====: #005*/

            gfQuanLyNhomHang = mQLNhomHang_Thuoc || mQLNhomHang_YCu || mQLNhomHang_HoaChat || mQLNhomHang_HoatChat;
            #endregion

            #region Quản lý danh mục
            mRefGenMedProductDetails_DrugMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_Thuoc);
            mRefGenMedProductDetails_MedicalDevicesMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_YCu);
            mRefGenMedProductDetails_ChemicalMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_HoaChat);
            mRefGenMedProductDetails_VTYTTHMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_VTYTTH);
            mRefGenMedProductDetails_TiemNguaMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_TiemNgua);
            mRefGenMedProductDetails_MauMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_Mau);
            mRefGenMedProductDetails_ThanhTrungMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_ThanhTrung);
            mRefGenMedProductDetails_VPPMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_VPP);
            mRefGenMedProductDetails_VTTHMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_VTTH);

            gfQuanLyDanhMuc = mRefGenMedProductDetails_DrugMgnt || mRefGenMedProductDetails_MedicalDevicesMgnt || mRefGenMedProductDetails_ChemicalMgnt
                || mRefGenMedProductDetails_VTYTTHMgnt || mRefGenMedProductDetails_TiemNguaMgnt || mRefGenMedProductDetails_MauMgnt
                || mRefGenMedProductDetails_ThanhTrungMgnt || mRefGenMedProductDetails_VPPMgnt || mRefGenMedProductDetails_VTTHMgnt;
            #endregion

            #region Giá từ NCC
            mSupplierGenMedProductsPrice_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mGiaTuNCC_Thuoc);
            mSupplierGenMedProductsPrice_Medical_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mGiaTuNCC_YCu);
            mSupplierGenMedProductsPrice_Chemical_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mGiaTuNCC_HoaChat);

            gfGiaNCC = mSupplierGenMedProductsPrice_Mgnt || mSupplierGenMedProductsPrice_Medical_Mgnt || mSupplierGenMedProductsPrice_Chemical_Mgnt;
            #endregion

            #region Thang giá bán
            mDrugDeptSellPriceProfitScale_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mThangGiaBan_Thuoc);
            mDrugDeptSellPriceProfitScale_Medical_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mThangGiaBan_YCu);
            mDrugDeptSellPriceProfitScale_Chemical_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mThangGiaBan_HoaChat);
            //gfThangGiaBan = mDrugDeptSellPriceProfitScale_Mgnt || mDrugDeptSellPriceProfitScale_Medical_Mgnt || mDrugDeptSellPriceProfitScale_Chemical_Mgnt;
            #endregion

            mDrugDeptSellingItemPrices_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mGiaBan_Thuoc);
            mDrugDeptSellingItemPrices_Medical_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mGiaBan_YCu);
            mDrugDeptSellingItemPrices_Chemical_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mGiaBan_HoaChat);

            #region Bảng giá bán
            mDrugDeptSellingPriceList_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_Thuoc);
            mDrugDeptSellingPriceList_Medical_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_YCu);
            mDrugDeptSellingPriceList_Chemical_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_HoaChat);

            //gfBangGiaBan = mDrugDeptSellingPriceList_Mgnt || mDrugDeptSellingPriceList_Medical_Mgnt || mDrugDeptSellingPriceList_Chemical_Mgnt;
            #endregion

            #region Đổi Giá Bán Theo Lô
            mAdjustOutPrice_Med = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mAdjustOutPrice_Med);
            mAdjustOutPrice_Mat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mAdjustOutPrice_Mat);
            mAdjustOutPrice_Lab = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mAdjustOutPrice_Lab);

            gfAdjustOutPrice = mAdjustOutPrice_Med || mAdjustOutPrice_Mat || mAdjustOutPrice_Lab;

            #endregion

            mBid = gfQuanLyDanhMuc;

            /*▼====: #003*/
            gTempInwardMedReport = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc, (int)eKhoaDuoc.TheoDoiHangKyGoi_Thuoc);
            gTempInwardMatReport = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc, (int)eKhoaDuoc.TheoDoiHangKyGoi_YCu);
            gTempInwardLabReport = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc, (int)eKhoaDuoc.TheoDoiHangKyGoi_HoaChat);
            gTempInwardReport = gTempInwardMedReport || gTempInwardMatReport || gTempInwardLabReport;
            /*▲====: #003*/

            //▼====: #019
            mXNBDinhDuongCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_DinhDuong);

            mDuyetPhieuLinhHang_DinhDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_DinhDuong);

            mNhapTraTuKhoPhong_DDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapTraTuKhoPhong_DDuong);

            mNhapXuatTonDDuongCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_DDuong);

            mTheKhoDDuongCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_DDuong);

            mKKDDuongCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_DDuong);

            mEstimationDDuongCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_DDuong);

            mDrugDeptSellPriceProfitScale_Nutrition_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mThangGiaBan_DDuong);
            mQuanLyThuocKemTheoCLS_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQuanLyThuocKemTheoCLS);

            gfThangGiaBan = mDrugDeptSellPriceProfitScale_Mgnt || mDrugDeptSellPriceProfitScale_Medical_Mgnt || mDrugDeptSellPriceProfitScale_Chemical_Mgnt || mDrugDeptSellPriceProfitScale_Nutrition_Mgnt;
            //▲====: #019

            //▼====: #020
            mDuyetPhieuLinhHang_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_VTYTTH);

            mDuyetPhieuLinhHang_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_Vaccine);

            mDuyetPhieuLinhHang_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_ThanhTrung);

            mDuyetPhieuLinhHang_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_Blood);

            mDuyetPhieuLinhHang_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_VTTH);

            mDuyetPhieuLinhHang_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_VPP);

            gfDuyetPhieuLinhHang = mDuyetPhieuLinhHang_Thuoc || mDuyetPhieuLinhHang_YCu || mDuyetPhieuLinhHang_HoaChat || mDuyetPhieuLinhHang_Vaccine || mDuyetPhieuLinhHang_Blood
                                   || mDuyetPhieuLinhHang_VPP || mDuyetPhieuLinhHang_VTTH || mDuyetPhieuLinhHang_ThanhTrung || mDuyetPhieuLinhHang_VTYTTH;
            //▲====: #020

            //▼====: #021
            mInwardDrugFromSupplierVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_VTYTTH);
            mInwardDrugFromSupplierVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_Vaccine);
            mInwardDrugFromSupplierBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_Blood);
            mInwardDrugFromSupplierVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_VPP);
            mInwardDrugFromSupplierVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_VTTH);
            mInwardDrugFromSupplierThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_ThanhTrung);

            gf2NhapHangTuNCC = mInwardDrugFromSupplierVTYTTHCmd || mInwardDrugFromSupplierVaccineCmd || mInwardDrugFromSupplierBloodCmd
                || mInwardDrugFromSupplierVPPCmd || mInwardDrugFromSupplierVTTHCmd || mInwardDrugFromSupplierThanhTrungCmd;

            mNhapTraTuKhoPhong_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapTraTuKhoPhong_VTYTTH);
            mNhapTraTuKhoPhong_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapTraTuKhoPhong_Vaccine);
            mNhapTraTuKhoPhong_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapTraTuKhoPhong_Blood);
            mNhapTraTuKhoPhong_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapTraTuKhoPhong_VPP);
            mNhapTraTuKhoPhong_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapTraTuKhoPhong_VTTH);
            mNhapTraTuKhoPhong_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapTraTuKhoPhong_ThanhTrung);

            gf1NhapTraTuKhoPhong = mNhapTraTuKhoPhong_Thuoc || mNhapTraTuKhoPhong_YCu || mNhapTraTuKhoPhong_HoaChat || mNhapTraTuKhoPhong_DDuong || mNhapTraTuKhoPhong_VTYTTH
                                || mNhapTraTuKhoPhong_Vaccine || mNhapTraTuKhoPhong_Blood || mNhapTraTuKhoPhong_VPP || mNhapTraTuKhoPhong_VTTH || mNhapTraTuKhoPhong_ThanhTrung;

            mNhapNoiBo = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapNoiBo);
            mNhapTraTuKhoLe = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapTraTuKhoLe);

            gNhapNoiBo = mNhapTraTuKhoLe || mNhapNoiBo;
            //▲====: #021

            //▼====: #022
            //Xuất
            mXNBVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_VTYTTH);
            mXNBVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_Vaccine);
            mXNBBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_Blood);
            mXNBVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_VPP);
            mXNBVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_VTTH);
            mXNBThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_ThanhTrung);

            gfXuatNoiBo = mXNBThuocCmd || mXNBYCuCmd || mXNBHoaChatCmd || mXNBDinhDuongCmd || mXNBVTYTTHCmd ||
                            mXNBVaccineCmd || mXNBBloodCmd || mXNBVPPCmd || mXNBVTTHCmd || mXNBThanhTrungCmd;

            //Xuất trả NCC
            mReturnToSupplier_Drug = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_Thuoc);
            mReturnToSupplier_Mat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_Ycu);
            mReturnToSupplier_Chemical = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_HoaChat);
            mReturnToSupplier_Nutri = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_DinhDuong);
            mReturnToSupplier_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_VTYTTH);
            mReturnToSupplier_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_Vaccine);
            mReturnToSupplier_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_Blood);
            mReturnToSupplier_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_VPP);
            mReturnToSupplier_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_VTTH);
            mReturnToSupplier_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTraNCC_ThanhTrung);

            mReturnToSupplier = mReturnToSupplier_Drug || mReturnToSupplier_Mat || mReturnToSupplier_Chemical || mReturnToSupplier_Nutri || mReturnToSupplier_VTYTTH ||
                                mReturnToSupplier_Vaccine || mReturnToSupplier_Blood || mReturnToSupplier_VPP || mReturnToSupplier_VTTH || mReturnToSupplier_ThanhTrung;

            //Dự trù
            mEstimationVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_VTYTTH);
            mEstimationVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_Vaccine);
            mEstimationBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_Blood);
            mEstimationVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_VPP);
            mEstimationVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_VTTH);
            mEstimationThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuTru_ThanhTrung);

            gfDuTru = mEstimationDrugDeptCmd || mEstimationYCuCmd || mEstimationChemicalCmd || mEstimationDDuongCmd || mEstimationVTYTTHCmd ||
                    mEstimationVaccineCmd || mEstimationBloodCmd || mEstimationVPPCmd || mEstimationVTTHCmd || mEstimationThanhTrungCmd;

            //Đặt hàng
            mOrderVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_VTYTTH);
            mOrderVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_Vaccine);
            mOrderBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_Blood);
            mOrderVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_VPP);
            mOrderVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_VTTH);
            mOrderThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_ThanhTrung);
            mOrderDinhDuongCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_DinhDuong);

            gfDatHang = mOrderThuocCmd || mOrderYCuCmd || mOrderHoaChatCmd || mOrderBloodCmd || mOrderVTYTTHCmd ||
                        mOrderVaccineCmd || mOrderVPPCmd || mOrderThanhTrungCmd || mOrderVTTHCmd || mOrderDinhDuongCmd;

            //Bảng giá bán
            mDrugDeptSellingPriceList_VTYTTH_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_VTYTTH);
            mDrugDeptSellingPriceList_Vaccine_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_Vaccine);
            mDrugDeptSellingPriceList_Blood_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_Blood);
            mDrugDeptSellingPriceList_VPP_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_VPP);
            mDrugDeptSellingPriceList_VTTH_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_VTTH);
            mDrugDeptSellingPriceList_ThanhTrung_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_ThanhTrung);
            mDrugDeptSellingPriceList_DinhDuong_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBangGiaBan_DinhDuong);

            gfBangGiaBan = mDrugDeptSellingPriceList_Mgnt || mDrugDeptSellingPriceList_Medical_Mgnt || mDrugDeptSellingPriceList_Chemical_Mgnt || mDrugDeptSellingPriceList_VTYTTH_Mgnt || mDrugDeptSellingPriceList_Vaccine_Mgnt ||
                        mDrugDeptSellingPriceList_Blood_Mgnt || mDrugDeptSellingPriceList_VPP_Mgnt || mDrugDeptSellingPriceList_VTTH_Mgnt || mDrugDeptSellingPriceList_ThanhTrung_Mgnt || mDrugDeptSellingPriceList_DinhDuong_Mgnt;

            //DS phiếu nhập kho
            mBangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_VTYTTH);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_Vaccine);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_Blood);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_VPP);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_VTTH);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_ThanhTrung);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoDinhDuongCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_DinhDuong);

            gf1PhieuNhapKho = mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoVaccineCmd ||
                            mBangKeNhapHangThangTheoSoPhieuNhapKhoBloodCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoVTTHCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoDinhDuongCmd;

            //BC Nhập xuất tồn
            mNhapXuatTonVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_VTYTTH);
            mNhapXuatTonVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_Vaccine);
            mNhapXuatTonBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_Blood);
            mNhapXuatTonVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_VPP);
            mNhapXuatTonVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_VTTH);
            mNhapXuatTonThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuatNhapTon_ThanhTrung);

            gf1NhapXuatTon = mNhapXuatTonThuocCmd || mNhapXuatTonYCuCmd || mNhapXuatTonHoaChatCmd || mNhapXuatTonDDuongCmd || mNhapXuatTonVTYTTHCmd ||
                            mNhapXuatTonVaccineCmd || mNhapXuatTonBloodCmd || mNhapXuatTonVPPCmd || mNhapXuatTonVTTHCmd || mNhapXuatTonThanhTrungCmd;

            mTheKhoVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_VTYTTH);
            mTheKhoVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_Vaccine);
            mTheKhoBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_Blood);
            mTheKhoVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_VPP);
            mTheKhoVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_VTTH);
            mTheKhoThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_ThanhTrung);

            gf1TheKho = mTheKhoThuocCmd || mTheKhoYCuCmd || mTheKhoHoaChatCmd || mTheKhoDDuongCmd || mTheKhoVTYTTHCmd ||
                        mTheKhoVaccineCmd || mTheKhoBloodCmd || mTheKhoVPPCmd || mTheKhoVTTHCmd || mTheKhoThanhTrungCmd;

            mXuatVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_VTYTTH);
            mXuatVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_Vaccine);
            mXuatBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_Blood);
            mXuatVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_VPP);
            mXuatVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_VTTH);
            mXuatThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_ThanhTrung);

            gf1BaoCaoXuat = mXuatThuocCmd || mXuatYCuCmd || mXuatHoaChatCmd || mXuatVTTHCmd || mXuatVaccineCmd ||
                            mXuatBloodCmd || mXuatVTYTTHCmd || mXuatVPPCmd || mXuatThanhTrungCmd;

            mNhapVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhap_VTYTTH);
            mNhapVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhap_Vaccine);
            mNhapBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhap_Blood);
            mNhapVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhap_VPP);
            mNhapVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhap_VTTH);
            mNhapThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhap_ThanhTrung);

            gf1BaoCaoNhap = mNhapThuocCmd || mNhapYCuCmd || mNhapHoaChatCmd || mNhapVTYTTHCmd || mNhapVaccineCmd ||
                            mNhapBloodCmd || mNhapVPPCmd || mNhapVTTHCmd || mNhapThanhTrungCmd;

            mBaoCaoThuocHetHan = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mBaoCaoThuocHetHanDung);
            mBaoCaoHangKhongXuat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,

                                             (int)eKhoaDuoc.mBaoCaoHangKhongXuat);
            mBaoCaoToaTreo_Duoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mBaoCaoToaTreo_Duoc);
            mBaoCaoToaTreo_NhaThuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mBaoCaoToaTreo_NhaThuoc);
            mBaoCaoToaTreo_KhoaPhong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mBaoCaoToaTreo_KhoaPhong);

            gBaoCaoToaTreo = mBaoCaoToaTreo_Duoc || mBaoCaoToaTreo_NhaThuoc || mBaoCaoToaTreo_KhoaPhong;

            mBaoCaoToaThuocHangNgay = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mBaoCaoToaThuocHangNgay);
            gBaoCaoKhac = mBaoCaoToaThuocHangNgay;

            gfBaoCaoThongKeAll = gf1PhieuNhapKho || gf1NhapXuatTon || gf1NhapXuatTon || gf1TheKho || gf1TheoDoiCongNo || gf1BaoCaoXuat || gf1BaoCaoNhap || gf1WatchOutQty ||
                                gf1BaoCaoNam || gf1XuatKhoaPhong || gTempInwardReport || gBaoCaoToaTreo || gBaoCaoKhac
                                //▼==== #028
                                || gBaoCaoXuatKhoaPhong;
                                //▲==== #028

            mKKVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_VTYTTH);
            mKKVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_Vaccine);
            mKKBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_Blood);
            mKKVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_VPP);
            mKKVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_VTTH);
            mKKThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mKiemKe_ThanhTrung);

            gfKiemKe = mKKThuocCmd || mKKYCuCmd || mKKHoaChatCmd || mKKDDuongCmd || mKKVTTHCmd || mKKVaccineCmd || mKKBloodCmd || mKKVPPCmd || mKKVTTHCmd || mKKThanhTrungCmd;

            mTangGiamKKThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_Thuoc);
            mTangGiamKKYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_Ycu);
            mTangGiamKKHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_HoaChat);
            mTangGiamKKDinhDuongCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_DinhDuong);
            mTangGiamKKVTYTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_VTYTTH);
            mTangGiamKKVaccineCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_Vaccine);
            mTangGiamKKBloodCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_Blood);
            mTangGiamKKVPPCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_VPP);
            mTangGiamKKVTTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_VTTH);
            mTangGiamKKThanhTrungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mTangGiamKiemKe_ThanhTrung);
            gTangGiamKK = mTangGiamKKThuocCmd || mTangGiamKKYCuCmd || mTangGiamKKHoaChatCmd || mTangGiamKKDinhDuongCmd || mTangGiamKKVTYTTHCmd ||
                        mTangGiamKKVaccineCmd || mTangGiamKKBloodCmd || mTangGiamKKVPPCmd || mTangGiamKKVTTHCmd || mTangGiamKKThanhTrungCmd;

            gQuanLy = mQuanLyKho;
            mQuanLyKho = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQuanLyKho);
            //▲====: #022

            //▼====: #025
            mBidDetail_Med = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLThau_Thuoc);
            mBidDetail_Mat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLThau_Ycu);
            mBidDetail_Chemical = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLThau_HoaChat);
            mBidDetail = mBidDetail_Med || mBidDetail_Mat || mBidDetail_Chemical;
            //▲====: #025

            gfBaoCao = gf1BaoCaoXuat || gf1NhapXuatTon || gf1TheKho || gf1PhieuNhapKho || gf1BaoCaoNam || gf1XuatKhoaPhong || gf1TheoDoiCongNo;
            //▼====: #027
            gElectronicPrescription = mElectronicPrescription;
            mElectronicPrescription = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc, (int)eKhoaDuoc.mDonThuocDienTu);
            //▲====: #027
            //▼==== #028
            gBaoCaoXuatKhoaPhong = mBC_XuatKP_Thuoc || mBC_XuatKP_YCu;

            mBC_XuatKP_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mBC_XuatKP_Thuoc);

            mBC_XuatKP_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mBC_XuatKP_YCu);
            //▲==== #028
            //▼==== #029
            mBCTThongKeDTDT = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc, (int)eKhoaDuoc.mBCTThongKeDTDT); ;
            //▲==== #029
        }

        #region bool checking


        #region Group Function
        //▼====== #008
        private bool _gfNhapHangAll = true;
        public bool gfNhapHangAll
        {
            get
            {
                return _gfNhapHangAll;
            }
            set
            {
                if (_gfNhapHangAll == value)
                    return;
                _gfNhapHangAll = value;
                NotifyOfPropertyChange(() => gfNhapHangAll);
            }
        }

        private bool _gfXuatHangAll = true;
        public bool gfXuatHangAll
        {
            get
            {
                return _gfXuatHangAll;
            }
            set
            {
                if (_gfXuatHangAll == value)
                    return;
                _gfXuatHangAll = value;
                NotifyOfPropertyChange(() => gfXuatHangAll);
            }
        }
        private bool _gfQuanLyGiaAll = true;
        public bool gfQuanLyGiaAll
        {
            get
            {
                return _gfQuanLyGiaAll;
            }
            set
            {
                if (_gfQuanLyGiaAll == value)
                    return;
                _gfQuanLyGiaAll = value;
                NotifyOfPropertyChange(() => gfQuanLyGiaAll);
            }
        }

        private bool _gfBaoCaoThongKeAll = true;
        public bool gfBaoCaoThongKeAll
        {
            get
            {
                return _gfBaoCaoThongKeAll;
            }
            set
            {
                if (_gfBaoCaoThongKeAll == value)
                    return;
                _gfBaoCaoThongKeAll = value;
                NotifyOfPropertyChange(() => gfBaoCaoThongKeAll);
            }
        }

        private bool _gfQuanLyDanhMucAll = true;
        public bool gfQuanLyDanhMucAll
        {
            get
            {
                return _gfQuanLyDanhMucAll;
            }
            set
            {
                if (_gfQuanLyDanhMucAll == value)
                    return;
                _gfQuanLyDanhMucAll = value;
                NotifyOfPropertyChange(() => gfQuanLyDanhMucAll);
            }
        }
        //▲====== #008
        private bool _gfDuyetPhieuLinhHang = true;
        public bool gfDuyetPhieuLinhHang
        {
            get
            {
                return _gfDuyetPhieuLinhHang;
            }
            set
            {
                if (_gfDuyetPhieuLinhHang == value)
                    return;
                _gfDuyetPhieuLinhHang = value;
                NotifyOfPropertyChange(() => gfDuyetPhieuLinhHang);
            }
        }

        private bool _gfXuatNoiBo = true;
        public bool gfXuatNoiBo
        {
            get
            {
                return _gfXuatNoiBo;
            }
            set
            {
                if (_gfXuatNoiBo == value)
                    return;
                _gfXuatNoiBo = value;
                NotifyOfPropertyChange(() => gfXuatNoiBo);
            }
        }

        private bool _gfXuatTheoToa = true;
        public bool gfXuatTheoToa
        {
            get
            {
                return _gfXuatTheoToa && MenuVisibleCollection[0];
            }
            set
            {
                if (_gfXuatTheoToa == value)
                    return;
                _gfXuatTheoToa = value;
                NotifyOfPropertyChange(() => gfXuatTheoToa);
            }
        }
        private bool _gfXuatHangKyGoi = true;
        public bool gfXuatHangKyGoi
        {
            get
            {
                return _gfXuatHangKyGoi;
            }
            set
            {
                if (_gfXuatHangKyGoi == value)
                    return;
                _gfXuatHangKyGoi = value;
                NotifyOfPropertyChange(() => gfXuatHangKyGoi);
            }
        }

        private bool _gfSapNhapHangKyGoi = true;
        public bool gfSapNhapHangKyGoi
        {
            get
            {
                return _gfSapNhapHangKyGoi;
            }
            set
            {
                if (_gfSapNhapHangKyGoi == value)
                    return;
                _gfSapNhapHangKyGoi = value;
                NotifyOfPropertyChange(() => gfSapNhapHangKyGoi);
            }
        }


        private bool _gfTraHang = true;
        public bool gfTraHang
        {
            get
            {
                return _gfTraHang;
            }
            set
            {
                if (_gfTraHang == value)
                    return;
                _gfTraHang = value;
                NotifyOfPropertyChange(() => gfTraHang);
            }
        }

        private bool _gfDuTru = true;
        public bool gfDuTru
        {
            get
            {
                return _gfDuTru;
            }
            set
            {
                if (_gfDuTru == value)
                    return;
                _gfDuTru = value;
                NotifyOfPropertyChange(() => gfDuTru);
            }
        }


        private bool _gfDatHang = true;
        public bool gfDatHang
        {
            get
            {
                return _gfDatHang;
            }
            set
            {
                if (_gfDatHang == value)
                    return;
                _gfDatHang = value;
                NotifyOfPropertyChange(() => gfDatHang);
            }
        }

        private bool _gf1NhapHangTuNCC = true;
        public bool gf1NhapHangTuNCC
        {
            get
            {
                return _gf1NhapHangTuNCC;
            }
            set
            {
                if (_gf1NhapHangTuNCC == value)
                    return;
                _gf1NhapHangTuNCC = value;
                NotifyOfPropertyChange(() => gf1NhapHangTuNCC);
            }
        }


        private bool _gf1NhapTraTuKhoPhong = true;
        public bool gf1NhapTraTuKhoPhong
        {
            get
            {
                return _gf1NhapTraTuKhoPhong;
            }
            set
            {
                if (_gf1NhapTraTuKhoPhong == value)
                    return;
                _gf1NhapTraTuKhoPhong = value;
                NotifyOfPropertyChange(() => gf1NhapTraTuKhoPhong);
            }
        }


        private bool _gfNhapHang = true;
        public bool gfNhapHang
        {
            get
            {
                return _gfNhapHang;
            }
            set
            {
                if (_gfNhapHang == value)
                    return;
                _gfNhapHang = value;
                NotifyOfPropertyChange(() => gfNhapHang);
            }
        }

        private bool _gfPhanBoPhi = true;
        public bool gfPhanBoPhi
        {
            get
            {
                return _gfPhanBoPhi;
            }
            set
            {
                if (_gfPhanBoPhi == value)
                    return;
                _gfPhanBoPhi = value;
                NotifyOfPropertyChange(() => gfPhanBoPhi);
            }
        }

        private bool _gfDeNghiThanhToan = true;
        public bool gfDeNghiThanhToan
        {
            get
            {
                return _gfDeNghiThanhToan;
            }
            set
            {
                if (_gfDeNghiThanhToan == value)
                    return;
                _gfDeNghiThanhToan = value;
                NotifyOfPropertyChange(() => gfDeNghiThanhToan);
            }
        }


        private bool _gfKiemKe = true;
        public bool gfKiemKe
        {
            get
            {
                return _gfKiemKe;
            }
            set
            {
                if (_gfKiemKe == value)
                    return;
                _gfKiemKe = value;
                NotifyOfPropertyChange(() => gfKiemKe);
            }
        }


        private bool _gf1BaoCaoXuat = true;
        public bool gf1BaoCaoXuat
        {
            get
            {
                return _gf1BaoCaoXuat;
            }
            set
            {
                if (_gf1BaoCaoXuat == value)
                    return;
                _gf1BaoCaoXuat = value;
                NotifyOfPropertyChange(() => gf1BaoCaoXuat);
            }
        }

        /*TMA 23/10/2017*/
        private bool _gf1BaoCaoNhap = true;
        public bool gf1BaoCaoNhap
        {
            get
            {
                return _gf1BaoCaoNhap;
            }
            set
            {
                if (_gf1BaoCaoNhap == value)
                    return;
                _gf1BaoCaoNhap = value;
                NotifyOfPropertyChange(() => gf1BaoCaoNhap);
            }
        }
        public bool mNhapThuocCmd
        {
            get
            {
                return _mNhapThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mNhapThuocCmd == value)
                    return;
                _mNhapThuocCmd = value;
                NotifyOfPropertyChange(() => mNhapThuocCmd);
            }
        }
        public bool mNhapYCuCmd
        {
            get
            {
                return _mNhapYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapYCuCmd == value)
                    return;
                _mNhapYCuCmd = value;
                NotifyOfPropertyChange(() => mNhapYCuCmd);
            }
        }
        public bool mNhapHoaChatCmd
        {
            get
            {
                return _mNhapHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapHoaChatCmd == value)
                    return;
                _mNhapHoaChatCmd = value;
                NotifyOfPropertyChange(() => mNhapHoaChatCmd);
            }
        }
        public bool mBaoCaoNhap_Thuoc_XemIn
        {
            get
            {
                return _mBaoCaoNhap_Thuoc_XemIn;
            }
            set
            {
                if (_mBaoCaoNhap_Thuoc_XemIn == value)
                    return;
                _mBaoCaoNhap_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoNhap_Thuoc_XemIn);
            }
        }
        public bool mBaoCaoNhap_YCu_XemIn
        {
            get
            {
                return _mBaoCaoNhap_YCu_XemIn;
            }
            set
            {
                if (_mBaoCaoNhap_YCu_XemIn == value)
                    return;
                _mBaoCaoNhap_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoNhap_YCu_XemIn);
            }
        }
        public bool mBaoCaoNhap_HoaChat_XemIn
        {
            get
            {
                return _mBaoCaoNhap_HoaChat_XemIn;
            }
            set
            {
                if (_mBaoCaoNhap_HoaChat_XemIn == value)
                    return;
                _mBaoCaoNhap_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoNhap_HoaChat_XemIn);
            }
        }
        /**/
        /*▼====: #003*/
        private bool _gTempInwardReport = true;
        public bool gTempInwardReport
        {
            get
            {
                return _gTempInwardReport;
            }
            set
            {
                _gTempInwardReport = value;
                NotifyOfPropertyChange(() => gTempInwardReport);
                NotifyOfPropertyChange(() => gTempInwardReport_Med);
                NotifyOfPropertyChange(() => gTempInwardReport_Mat);
            }
        }
        public bool gTempInwardReport_Med
        {
            get
            {
                return gTempInwardReport && MenuVisibleCollection[0];
            }
        }
        public bool gTempInwardReport_Mat
        {
            get
            {
                return gTempInwardReport && MenuVisibleCollection[1];
            }
        }
        private bool _gTempInwardMedReport = true;
        public bool gTempInwardMedReport
        {
            get
            {
                return _gTempInwardMedReport;
            }
            set
            {
                _gTempInwardMedReport = value;
                NotifyOfPropertyChange(() => gTempInwardMedReport);
            }
        }
        private bool _gTempInwardMatReport = true;
        public bool gTempInwardMatReport
        {
            get
            {
                return _gTempInwardMatReport;
            }
            set
            {
                _gTempInwardMatReport = value;
                NotifyOfPropertyChange(() => gTempInwardMatReport);
            }
        }
        private bool _gTempInwardLabReport = true;
        public bool gTempInwardLabReport
        {
            get
            {
                return _gTempInwardLabReport;
            }
            set
            {
                _gTempInwardLabReport = value;
                NotifyOfPropertyChange(() => gTempInwardLabReport);
            }
        }
        /*▲====: #003*/
        private bool _gf1WatchOutQty = true;
        public bool gf1WatchOutQty
        {
            get
            {
                return _gf1WatchOutQty;
            }
            set
            {
                if (_gf1WatchOutQty == value)
                    return;
                _gf1WatchOutQty = value;
                NotifyOfPropertyChange(() => gf1WatchOutQty);
            }
        }

        private bool _gf1NhapXuatTon = true;
        public bool gf1NhapXuatTon
        {
            get
            {
                return _gf1NhapXuatTon;
            }
            set
            {
                if (_gf1NhapXuatTon == value)
                    return;
                _gf1NhapXuatTon = value;
                NotifyOfPropertyChange(() => gf1NhapXuatTon);
            }
        }


        private bool _gf1TheKho = true;
        public bool gf1TheKho
        {
            get
            {
                return _gf1TheKho;
            }
            set
            {
                if (_gf1TheKho == value)
                    return;
                _gf1TheKho = value;
                NotifyOfPropertyChange(() => gf1TheKho);
            }
        }


        private bool _gf1PhieuNhapKho = true;
        public bool gf1PhieuNhapKho
        {
            get
            {
                return _gf1PhieuNhapKho;
            }
            set
            {
                if (_gf1PhieuNhapKho == value)
                    return;
                _gf1PhieuNhapKho = value;
                NotifyOfPropertyChange(() => gf1PhieuNhapKho);
            }
        }


        private bool _gf1BaoCaoNam = true;
        public bool gf1BaoCaoNam
        {
            get
            {
                return _gf1BaoCaoNam && MenuVisibleCollection[0];
            }
            set
            {
                if (_gf1BaoCaoNam == value)
                    return;
                _gf1BaoCaoNam = value;
                NotifyOfPropertyChange(() => gf1BaoCaoNam);
            }
        }

        private bool _gf1XuatKhoaPhong = true;
        public bool gf1XuatKhoaPhong
        {
            get
            {
                return _gf1XuatKhoaPhong;
            }
            set
            {
                if (_gf1XuatKhoaPhong == value)
                    return;
                _gf1XuatKhoaPhong = value;
                NotifyOfPropertyChange(() => gf1XuatKhoaPhong);
            }
        }

        private bool _gf1TheoDoiCongNo = true;
        public bool gf1TheoDoiCongNo
        {
            get
            {
                return _gf1TheoDoiCongNo;
            }
            set
            {
                if (_gf1TheoDoiCongNo == value)
                    return;
                _gf1TheoDoiCongNo = value;
                NotifyOfPropertyChange(() => gf1TheoDoiCongNo);
            }
        }

        private bool _gfBaoCao = true;
        public bool gfBaoCao
        {
            get
            {
                return _gfBaoCao;
            }
            set
            {
                if (_gfBaoCao == value)
                    return;
                _gfBaoCao = value;
                NotifyOfPropertyChange(() => gfBaoCao);
            }
        }

        private bool _gfQuanLy = true;
        public bool gfQuanLy
        {
            get
            {
                return _gfQuanLy;
            }
            set
            {
                if (_gfQuanLy == value)
                    return;
                _gfQuanLy = value;
                NotifyOfPropertyChange(() => gfQuanLy);
            }
        }

        private bool _gfQuanLyNhomHang = true;
        public bool gfQuanLyNhomHang
        {
            get
            {
                return _gfQuanLyNhomHang;
            }
            set
            {
                if (_gfQuanLyNhomHang == value)
                    return;
                _gfQuanLyNhomHang = value;
                NotifyOfPropertyChange(() => gfQuanLyNhomHang);
            }
        }

        private bool _gfQuanLyDanhMuc = true;
        public bool gfQuanLyDanhMuc
        {
            get
            {
                return _gfQuanLyDanhMuc;
            }
            set
            {
                if (_gfQuanLyDanhMuc == value)
                    return;
                _gfQuanLyDanhMuc = value;
                NotifyOfPropertyChange(() => gfQuanLyDanhMuc);
            }
        }

        private bool _gfGiaNCC = true;
        public bool gfGiaNCC
        {
            get
            {
                return _gfGiaNCC;
            }
            set
            {
                if (_gfGiaNCC == value)
                    return;
                _gfGiaNCC = value;
                NotifyOfPropertyChange(() => gfGiaNCC);
            }
        }

        private bool _gfThangGiaBan = true;
        public bool gfThangGiaBan
        {
            get
            {
                return _gfThangGiaBan;
            }
            set
            {
                if (_gfThangGiaBan == value)
                    return;
                _gfThangGiaBan = value;
                NotifyOfPropertyChange(() => gfThangGiaBan);
            }
        }


        private bool _gfBangGiaBan = true;
        public bool gfBangGiaBan
        {
            get
            {
                return _gfBangGiaBan;
            }
            set
            {
                if (_gfBangGiaBan == value)
                    return;
                _gfBangGiaBan = value;
                NotifyOfPropertyChange(() => gfBangGiaBan);
            }
        }

        private bool _gfAdjustOutPrice = true;
        public bool gfAdjustOutPrice
        {
            get
            {
                return _gfAdjustOutPrice;
            }
            set
            {
                if (_gfAdjustOutPrice == value)
                    return;
                _gfAdjustOutPrice = value;
                NotifyOfPropertyChange(() => gfAdjustOutPrice);
            }
        }
        #endregion

        #region new
        /*TMA - 23/10/2017*/
        private bool _mNhapThuocCmd = true;
        private bool _mNhapYCuCmd = true;
        private bool _mNhapHoaChatCmd = true;
        /*TMA - 23/10/2017*/

        private bool _mXNBThuocCmd = true;
        private bool _mXNBYCuCmd = true;
        private bool _mXNBHoaChatCmd = true;

        private bool _mXuatTheoToa_Thuoc = true;

        private bool _mReturnThuocCmd = true;
        private bool _mReturnYCuCmd = true;
        private bool _mReturnHoaChatCmd = true;

        private bool _mDemageThuocCmd = true;
        private bool _mDemageYCuCmd = true;
        private bool _mDemageHoaChatCmd = true;

        private bool _mEstimationDrugDeptCmd = true;
        private bool _mEstimationYCuCmd = true;
        private bool _mEstimationChemicalCmd = true;

        private bool _mOrderThuocCmd = true;
        private bool _mOrderYCuCmd = true;
        private bool _mOrderHoaChatCmd = true;

        private bool _mInwardDrugFromSupplierDrugCmd = true;
        private bool _mInwardDrugFromSupplierMedicalDeviceCmd = true;
        private bool _mInwardDrugFromSupplierChemicalCmd = true;

        private bool _mCostThuocCmd = true;
        private bool _mCostYCuCmd = true;
        private bool _mCostHoaChatCmd = true;

        private bool _mSuggestThuocCmd = true;
        private bool _mSuggestYCuCmd = true;
        private bool _mSuggestHoaChatCmd = true;

        private bool _mKKThuocCmd = true;
        private bool _mKKYCuCmd = true;
        private bool _mKKHoaChatCmd = true;

        private bool _mXuatThuocCmd = true;
        private bool _mXuatYCuCmd = true;
        private bool _mXuatHoaChatCmd = true;

        private bool _mWatchMedCmd = true;
        private bool _mWatchMatCmd = true;
        private bool _mWatchLabCmd = true;

        private bool _mNhapXuatTonThuocCmd = true;
        private bool _mNhapXuatTonYCuCmd = true;
        private bool _mNhapXuatTonHoaChatCmd = true;

        private bool _mTheKhoThuocCmd = true;
        private bool _mTheKhoYCuCmd = true;
        private bool _mTheKhoHoaChatCmd = true;



        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd = true;
        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd = true;
        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd = true;

        private bool _mSuDungThuocCmd = true;

        private bool _mThuocXuatDenKhoaCmd = true;
        private bool _mYCuXuatDenKhoaCmd = true;
        private bool _mHoaChatXuatDenKhoaCmd = true;

        private bool _mSupplierProductCmd = true;
        //private bool _mSupplierAndProductCmd= true;
        private bool _mDrugDeptPharmaceulCompanyCmd = true;
        private bool _mDrugDeptPharmaceulCompanySupplierCmd = true;
        private bool _mDrugDeptUnitCmd = true;
        private bool _mRefGenDrugBHYTCmd = true;

        private bool _mQLNhomHang_Thuoc = true;
        private bool _mQLNhomHang_YCu = true;
        private bool _mQLNhomHang_HoaChat = true;
        /*▼====: #005*/
        private bool _mQLNhomHang_HoatChat = true;
        /*▲====: #005*/


        private bool _mRefGenMedProductDetails_DrugMgnt = true;
        private bool _mRefGenMedProductDetails_MedicalDevicesMgnt = true;
        private bool _mRefGenMedProductDetails_ChemicalMgnt = true;
        private bool _mRefGenMedProductDetails_VTYTTHMgnt = true;
        private bool _mRefGenMedProductDetails_TiemNguaMgnt = true;
        private bool _mRefGenMedProductDetails_MauMgnt = true;
        private bool _mRefGenMedProductDetails_ThanhTrungMgnt = true;
        private bool _mRefGenMedProductDetails_VPPMgnt = true;
        private bool _mRefGenMedProductDetails_VTTHMgnt = true;

        private bool _mSupplierGenMedProductsPrice_Mgnt = true;
        private bool _mSupplierGenMedProductsPrice_Medical_Mgnt = true;
        private bool _mSupplierGenMedProductsPrice_Chemical_Mgnt = true;

        private bool _mDrugDeptSellPriceProfitScale_Mgnt = true;
        private bool _mDrugDeptSellPriceProfitScale_Medical_Mgnt = true;
        private bool _mDrugDeptSellPriceProfitScale_Chemical_Mgnt = true;

        private bool _mDrugDeptSellingItemPrices_Mgnt = true;
        private bool _mDrugDeptSellingItemPrices_Medical_Mgnt = true;
        private bool _mDrugDeptSellingItemPrices_Chemical_Mgnt = true;

        private bool _mDrugDeptSellingPriceList_Mgnt = true;
        private bool _mDrugDeptSellingPriceList_Medical_Mgnt = true;
        private bool _mDrugDeptSellingPriceList_Chemical_Mgnt = true;

        //them moi ngay 28-07-2012
        private bool _mBaoCaoTheoDoiCongNo_Thuoc = true;
        private bool _mBaoCaoTheoDoiCongNo_YCu = true;
        private bool _mBaoCaoTheoDoiCongNo_HoaChat = true;

        private bool _mDuyetPhieuLinhHang_Thuoc = true;
        private bool _mDuyetPhieuLinhHang_YCu = true;
        private bool _mDuyetPhieuLinhHang_HoaChat = true;

        private bool _mNhapHangKyGui = true;
        private bool _mNhapHangKyGui_Thuoc = true;
        private bool _mNhapHangKyGui_YCu = true;

        private bool _mXuatHangKyGui_Thuoc = true;
        private bool _mXuatHangKyGui_YCu = true;
        private bool _mXuatHangKyGui_HoaChat = true;

        private bool _mSapNhapHangKyGui_Thuoc = true;
        private bool _mSapNhapHangKyGui_YCu = true;
        private bool _mSapNhapHangKyGui_HoaChat = true;

        private bool _mNhapTraTuKhoPhong_Thuoc = true;
        private bool _mNhapTraTuKhoPhong_YCu = true;
        private bool _mNhapTraTuKhoPhong_HoaChat = true;
        private bool _mAdjustOutPrice_Med = true;
        private bool _mAdjustOutPrice_Mat = true;
        private bool _mAdjustOutPrice_Lab = true;
        private bool _mReturnToSupplier = true;
        private bool _mAdjustOutPrice = true;
        private bool _mInventory = true;
        private bool _mUseFollowBid = true;
        public bool mXNBThuocCmd
        {
            get
            {
                return _mXNBThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mXNBThuocCmd == value)
                    return;
                _mXNBThuocCmd = value;
                NotifyOfPropertyChange(() => mXNBThuocCmd);
            }
        }
        public bool mXNBYCuCmd
        {
            get
            {
                return _mXNBYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXNBYCuCmd == value)
                    return;
                _mXNBYCuCmd = value;
                NotifyOfPropertyChange(() => mXNBYCuCmd);
            }
        }
        public bool mXNBHoaChatCmd
        {
            get
            {
                return _mXNBHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXNBHoaChatCmd == value)
                    return;
                _mXNBHoaChatCmd = value;
                NotifyOfPropertyChange(() => mXNBHoaChatCmd);
            }
        }
        public bool mXuatTheoToa_Thuoc
        {
            get
            {
                return _mXuatTheoToa_Thuoc && MenuVisibleCollection[0];
            }
            set
            {
                if (_mXuatTheoToa_Thuoc == value)
                    return;
                _mXuatTheoToa_Thuoc = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc);
            }
        }
        public bool mReturnThuocCmd
        {
            get
            {
                return _mReturnThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mReturnThuocCmd == value)
                    return;
                _mReturnThuocCmd = value;
                NotifyOfPropertyChange(() => mReturnThuocCmd);
            }
        }
        public bool mReturnYCuCmd
        {
            get
            {
                return _mReturnYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnYCuCmd == value)
                    return;
                _mReturnYCuCmd = value;
                NotifyOfPropertyChange(() => mReturnYCuCmd);
            }
        }
        public bool mReturnHoaChatCmd
        {
            get
            {
                return _mReturnHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnHoaChatCmd
                 == value)
                    return;
                _mReturnHoaChatCmd
                 = value;
                NotifyOfPropertyChange(() => mReturnHoaChatCmd
                );
            }
        }
        public bool mDemageThuocCmd
        {
            get
            {
                return _mDemageThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mDemageThuocCmd
                 == value)
                    return;
                _mDemageThuocCmd
                 = value;
                NotifyOfPropertyChange(() => mDemageThuocCmd
                );
            }
        }
        public bool mDemageYCuCmd
        {
            get
            {
                return _mDemageYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDemageYCuCmd
                 == value)
                    return;
                _mDemageYCuCmd
                 = value;
                NotifyOfPropertyChange(() => mDemageYCuCmd
                );
            }
        }
        public bool mDemageHoaChatCmd
        {
            get
            {
                return _mDemageHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDemageHoaChatCmd
                 == value)
                    return;
                _mDemageHoaChatCmd
                 = value;
                NotifyOfPropertyChange(() => mDemageHoaChatCmd
                );
            }
        }
        public bool mEstimationDrugDeptCmd
        {
            get
            {
                return _mEstimationDrugDeptCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mEstimationDrugDeptCmd
                 == value)
                    return;
                _mEstimationDrugDeptCmd
                 = value;
                NotifyOfPropertyChange(() => mEstimationDrugDeptCmd
                );
            }
        }
        public bool mEstimationYCuCmd
        {
            get
            {
                return _mEstimationYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mEstimationYCuCmd
                 == value)
                    return;
                _mEstimationYCuCmd
                 = value;
                NotifyOfPropertyChange(() => mEstimationYCuCmd
                );
            }
        }
        public bool mEstimationChemicalCmd
        {
            get
            {
                return _mEstimationChemicalCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mEstimationChemicalCmd
                 == value)
                    return;
                _mEstimationChemicalCmd
                 = value;
                NotifyOfPropertyChange(() => mEstimationChemicalCmd
                );
            }
        }
        public bool mOrderThuocCmd
        {
            get
            {
                return _mOrderThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mOrderThuocCmd == value)
                    return;
                _mOrderThuocCmd = value;
                NotifyOfPropertyChange(() => mOrderThuocCmd);
            }
        }
        public bool mOrderYCuCmd
        {
            get
            {
                return _mOrderYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mOrderYCuCmd == value)
                    return;
                _mOrderYCuCmd = value;
                NotifyOfPropertyChange(() => mOrderYCuCmd);
            }
        }
        public bool mOrderHoaChatCmd
        {
            get
            {
                return _mOrderHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mOrderHoaChatCmd == value)
                    return;
                _mOrderHoaChatCmd = value;
                NotifyOfPropertyChange(() => mOrderHoaChatCmd);
            }
        }
        public bool mInwardDrugFromSupplierDrugCmd
        {
            get
            {
                return _mInwardDrugFromSupplierDrugCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mInwardDrugFromSupplierDrugCmd == value)
                    return;
                _mInwardDrugFromSupplierDrugCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierDrugCmd);
            }
        }
        public bool mInwardDrugFromSupplierMedicalDeviceCmd
        {
            get
            {
                return _mInwardDrugFromSupplierMedicalDeviceCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mInwardDrugFromSupplierMedicalDeviceCmd == value)
                    return;
                _mInwardDrugFromSupplierMedicalDeviceCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierMedicalDeviceCmd);
            }
        }
        public bool mInwardDrugFromSupplierChemicalCmd
        {
            get
            {
                return _mInwardDrugFromSupplierChemicalCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mInwardDrugFromSupplierChemicalCmd == value)
                    return;
                _mInwardDrugFromSupplierChemicalCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierChemicalCmd);
            }
        }
        public bool mCostThuocCmd
        {
            get
            {
                return _mCostThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mCostThuocCmd == value)
                    return;
                _mCostThuocCmd = value;
                NotifyOfPropertyChange(() => mCostThuocCmd);
            }
        }
        public bool mCostYCuCmd
        {
            get
            {
                return _mCostYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mCostYCuCmd == value)
                    return;
                _mCostYCuCmd = value;
                NotifyOfPropertyChange(() => mCostYCuCmd);
            }
        }
        public bool mCostHoaChatCmd
        {
            get
            {
                return _mCostHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mCostHoaChatCmd == value)
                    return;
                _mCostHoaChatCmd = value;
                NotifyOfPropertyChange(() => mCostHoaChatCmd);
            }
        }
        public bool mSuggestThuocCmd
        {
            get
            {
                return _mSuggestThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mSuggestThuocCmd == value)
                    return;
                _mSuggestThuocCmd = value;
                NotifyOfPropertyChange(() => mSuggestThuocCmd);
            }
        }
        public bool mSuggestYCuCmd
        {
            get
            {
                return _mSuggestYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mSuggestYCuCmd == value)
                    return;
                _mSuggestYCuCmd = value;
                NotifyOfPropertyChange(() => mSuggestYCuCmd);
            }
        }
        public bool mSuggestHoaChatCmd
        {
            get
            {
                return _mSuggestHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mSuggestHoaChatCmd
                 == value)
                    return;
                _mSuggestHoaChatCmd
                 = value;
                NotifyOfPropertyChange(() => mSuggestHoaChatCmd
                );
            }
        }
        public bool mKKThuocCmd
        {
            get
            {
                return _mKKThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mKKThuocCmd
                 == value)
                    return;
                _mKKThuocCmd
                 = value;
                NotifyOfPropertyChange(() => mKKThuocCmd
                );
            }
        }
        public bool mKKYCuCmd
        {
            get
            {
                return _mKKYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mKKYCuCmd
                 == value)
                    return;
                _mKKYCuCmd
                 = value;
                NotifyOfPropertyChange(() => mKKYCuCmd
                );
            }
        }
        public bool mKKHoaChatCmd
        {
            get
            {
                return _mKKHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mKKHoaChatCmd
                 == value)
                    return;
                _mKKHoaChatCmd
                 = value;
                NotifyOfPropertyChange(() => mKKHoaChatCmd
                );
            }
        }
        public bool mXuatThuocCmd
        {
            get
            {
                return _mXuatThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mXuatThuocCmd == value)
                    return;
                _mXuatThuocCmd = value;
                NotifyOfPropertyChange(() => mXuatThuocCmd);
            }
        }
        public bool mXuatYCuCmd
        {
            get
            {
                return _mXuatYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatYCuCmd == value)
                    return;
                _mXuatYCuCmd = value;
                NotifyOfPropertyChange(() => mXuatYCuCmd);
            }
        }
        public bool mXuatHoaChatCmd
        {
            get
            {
                return _mXuatHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatHoaChatCmd == value)
                    return;
                _mXuatHoaChatCmd = value;
                NotifyOfPropertyChange(() => mXuatHoaChatCmd);
            }
        }
        public bool mWatchMedCmd
        {
            get
            {
                return _mWatchMedCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mWatchMedCmd == value)
                    return;
                _mWatchMedCmd = value;
                NotifyOfPropertyChange(() => mWatchMedCmd);
            }
        }
        public bool mWatchMatCmd
        {
            get
            {
                return _mWatchMatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mWatchMatCmd == value)
                    return;
                _mWatchMatCmd = value;
                NotifyOfPropertyChange(() => mWatchMatCmd);
            }
        }
        public bool mWatchLabCmd
        {
            get
            {
                return _mWatchLabCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mWatchLabCmd == value)
                    return;
                _mWatchLabCmd = value;
                NotifyOfPropertyChange(() => mWatchLabCmd);
            }
        }
        public bool mNhapXuatTonThuocCmd
        {
            get
            {
                return _mNhapXuatTonThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mNhapXuatTonThuocCmd == value)
                    return;
                _mNhapXuatTonThuocCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonThuocCmd);
            }
        }
        public bool mNhapXuatTonYCuCmd
        {
            get
            {
                return _mNhapXuatTonYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapXuatTonYCuCmd == value)
                    return;
                _mNhapXuatTonYCuCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonYCuCmd);
            }
        }
        public bool mNhapXuatTonHoaChatCmd
        {
            get
            {
                return _mNhapXuatTonHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapXuatTonHoaChatCmd == value)
                    return;
                _mNhapXuatTonHoaChatCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonHoaChatCmd);
            }
        }
        public bool mTheKhoThuocCmd
        {
            get
            {
                return _mTheKhoThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mTheKhoThuocCmd == value)
                    return;
                _mTheKhoThuocCmd = value;
                NotifyOfPropertyChange(() => mTheKhoThuocCmd);
            }
        }
        public bool mTheKhoYCuCmd
        {
            get
            {
                return _mTheKhoYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTheKhoYCuCmd == value)
                    return;
                _mTheKhoYCuCmd = value;
                NotifyOfPropertyChange(() => mTheKhoYCuCmd);
            }
        }
        public bool mTheKhoHoaChatCmd
        {
            get
            {
                return _mTheKhoHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTheKhoHoaChatCmd
                 == value)
                    return;
                _mTheKhoHoaChatCmd
                 = value;
                NotifyOfPropertyChange(() => mTheKhoHoaChatCmd
                );
            }
        }
        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd);
            }
        }
        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd);
            }
        }
        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd);
            }
        }
        public bool mSuDungThuocCmd
        {
            get
            {
                return _mSuDungThuocCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mSuDungThuocCmd == value)
                    return;
                _mSuDungThuocCmd = value;
                NotifyOfPropertyChange(() => mSuDungThuocCmd);
            }
        }
        public bool mThuocXuatDenKhoaCmd
        {
            get
            {
                return _mThuocXuatDenKhoaCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mThuocXuatDenKhoaCmd == value)
                    return;
                _mThuocXuatDenKhoaCmd = value;
                NotifyOfPropertyChange(() => mThuocXuatDenKhoaCmd);
            }
        }
        public bool mYCuXuatDenKhoaCmd
        {
            get
            {
                return _mYCuXuatDenKhoaCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mYCuXuatDenKhoaCmd == value)
                    return;
                _mYCuXuatDenKhoaCmd = value;
                NotifyOfPropertyChange(() => mYCuXuatDenKhoaCmd);
            }
        }
        public bool mHoaChatXuatDenKhoaCmd
        {
            get
            {
                return _mHoaChatXuatDenKhoaCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mHoaChatXuatDenKhoaCmd == value)
                    return;
                _mHoaChatXuatDenKhoaCmd = value;
                NotifyOfPropertyChange(() => mHoaChatXuatDenKhoaCmd);
            }
        }
        public bool mSupplierProductCmd
        {
            get
            {
                return _mSupplierProductCmd
                ;
            }
            set
            {
                if (_mSupplierProductCmd
                 == value)
                    return;
                _mSupplierProductCmd
                 = value;
                NotifyOfPropertyChange(() => mSupplierProductCmd
                );
            }
        }
        public bool mDrugDeptPharmaceulCompanyCmd
        {
            get
            {
                return _mDrugDeptPharmaceulCompanyCmd
                ;
            }
            set
            {
                if (_mDrugDeptPharmaceulCompanyCmd
                 == value)
                    return;
                _mDrugDeptPharmaceulCompanyCmd
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptPharmaceulCompanyCmd
                );
            }
        }
        public bool mDrugDeptPharmaceulCompanySupplierCmd
        {
            get
            {
                return _mDrugDeptPharmaceulCompanySupplierCmd
                ;
            }
            set
            {
                if (_mDrugDeptPharmaceulCompanySupplierCmd
                 == value)
                    return;
                _mDrugDeptPharmaceulCompanySupplierCmd
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptPharmaceulCompanySupplierCmd
                );
            }
        }
        public bool mDrugDeptUnitCmd
        {
            get
            {
                return _mDrugDeptUnitCmd;
            }
            set
            {
                if (_mDrugDeptUnitCmd == value)
                    return;
                _mDrugDeptUnitCmd = value;
                NotifyOfPropertyChange(() => mDrugDeptUnitCmd);
            }
        }
        public bool mRefGenDrugBHYTCmd
        {
            get
            {
                return _mRefGenDrugBHYTCmd;
            }
            set
            {
                if (_mRefGenDrugBHYTCmd == value)
                    return;
                _mRefGenDrugBHYTCmd = value;
                NotifyOfPropertyChange(() => mRefGenDrugBHYTCmd);
            }
        }
        public bool mQLNhomHang_Thuoc
        {
            get
            {
                return _mQLNhomHang_Thuoc && MenuVisibleCollection[0];
            }
            set
            {
                if (_mQLNhomHang_Thuoc == value)
                    return;
                _mQLNhomHang_Thuoc = value;
                NotifyOfPropertyChange(() => mQLNhomHang_Thuoc);
            }
        }
        public bool mQLNhomHang_YCu
        {
            get
            {
                return _mQLNhomHang_YCu && MenuVisibleCollection[1];
            }
            set
            {
                if (_mQLNhomHang_YCu == value)
                    return;
                _mQLNhomHang_YCu = value;
                NotifyOfPropertyChange(() => mQLNhomHang_YCu);
            }
        }
        public bool mQLNhomHang_HoaChat
        {
            get
            {
                return _mQLNhomHang_HoaChat && MenuVisibleCollection[1];
            }
            set
            {
                if (_mQLNhomHang_HoaChat == value)
                    return;
                _mQLNhomHang_HoaChat = value;
                NotifyOfPropertyChange(() => mQLNhomHang_HoaChat);
            }
        }
        /*▼====: #005*/
        public bool mQLNhomHang_HoatChat
        {
            get
            {
                return _mQLNhomHang_HoatChat && MenuVisibleCollection[0];
            }
            set
            {
                if (_mQLNhomHang_HoatChat == value)
                    return;
                _mQLNhomHang_HoatChat = value;
                NotifyOfPropertyChange(() => mQLNhomHang_HoatChat);
            }
        }
        /*▲====: #005*/
        public bool mRefGenMedProductDetails_DrugMgnt
        {
            get
            {
                return _mRefGenMedProductDetails_DrugMgnt && MenuVisibleCollection[0];
            }
            set
            {
                if (_mRefGenMedProductDetails_DrugMgnt
                 == value)
                    return;
                _mRefGenMedProductDetails_DrugMgnt
                 = value;
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_DrugMgnt
                );
            }
        }
        public bool mRefGenMedProductDetails_MedicalDevicesMgnt
        {
            get
            {
                return _mRefGenMedProductDetails_MedicalDevicesMgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mRefGenMedProductDetails_MedicalDevicesMgnt
                 == value)
                    return;
                _mRefGenMedProductDetails_MedicalDevicesMgnt
                 = value;
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_MedicalDevicesMgnt
                );
            }
        }
        public bool mRefGenMedProductDetails_ChemicalMgnt
        {
            get
            {
                return _mRefGenMedProductDetails_ChemicalMgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mRefGenMedProductDetails_ChemicalMgnt
                 == value)
                    return;
                _mRefGenMedProductDetails_ChemicalMgnt
                 = value;
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_ChemicalMgnt
                );
            }
        }
        public bool mRefGenMedProductDetails_VTYTTHMgnt
        {
            get
            {
                return _mRefGenMedProductDetails_VTYTTHMgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mRefGenMedProductDetails_VTYTTHMgnt
                 == value)
                    return;
                _mRefGenMedProductDetails_VTYTTHMgnt
                 = value;
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_VTYTTHMgnt
                );
            }
        }
        public bool mRefGenMedProductDetails_TiemNguaMgnt
        {
            get
            {
                return _mRefGenMedProductDetails_TiemNguaMgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mRefGenMedProductDetails_TiemNguaMgnt
                 == value)
                    return;
                _mRefGenMedProductDetails_TiemNguaMgnt
                 = value;
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_TiemNguaMgnt
                );
            }
        }
        public bool mRefGenMedProductDetails_MauMgnt
        {
            get
            {
                return _mRefGenMedProductDetails_MauMgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mRefGenMedProductDetails_MauMgnt
                 == value)
                    return;
                _mRefGenMedProductDetails_MauMgnt
                 = value;
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_MauMgnt
                );
            }
        }
        public bool mRefGenMedProductDetails_ThanhTrungMgnt
        {
            get
            {
                return _mRefGenMedProductDetails_ThanhTrungMgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mRefGenMedProductDetails_ThanhTrungMgnt
                 == value)
                    return;
                _mRefGenMedProductDetails_ThanhTrungMgnt
                 = value;
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_ThanhTrungMgnt
                );
            }
        }
        public bool mRefGenMedProductDetails_VPPMgnt
        {
            get
            {
                return _mRefGenMedProductDetails_VPPMgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mRefGenMedProductDetails_VPPMgnt
                 == value)
                    return;
                _mRefGenMedProductDetails_VPPMgnt
                 = value;
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_VPPMgnt
                );
            }
        }
        public bool mRefGenMedProductDetails_VTTHMgnt
        {
            get
            {
                return _mRefGenMedProductDetails_VTTHMgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mRefGenMedProductDetails_VTTHMgnt
                 == value)
                    return;
                _mRefGenMedProductDetails_VTTHMgnt
                 = value;
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_VTTHMgnt
                );
            }
        }
        public bool mSupplierGenMedProductsPrice_Mgnt
        {
            get
            {
                return _mSupplierGenMedProductsPrice_Mgnt && MenuVisibleCollection[0];
            }
            set
            {
                if (_mSupplierGenMedProductsPrice_Mgnt
                 == value)
                    return;
                _mSupplierGenMedProductsPrice_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mSupplierGenMedProductsPrice_Mgnt
                );
            }
        }
        public bool mSupplierGenMedProductsPrice_Medical_Mgnt
        {
            get
            {
                return _mSupplierGenMedProductsPrice_Medical_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mSupplierGenMedProductsPrice_Medical_Mgnt
                 == value)
                    return;
                _mSupplierGenMedProductsPrice_Medical_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mSupplierGenMedProductsPrice_Medical_Mgnt
                );
            }
        }
        public bool mSupplierGenMedProductsPrice_Chemical_Mgnt
        {
            get
            {
                return _mSupplierGenMedProductsPrice_Chemical_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mSupplierGenMedProductsPrice_Chemical_Mgnt
                 == value)
                    return;
                _mSupplierGenMedProductsPrice_Chemical_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mSupplierGenMedProductsPrice_Chemical_Mgnt
                );
            }
        }
        public bool mDrugDeptSellPriceProfitScale_Mgnt
        {
            get
            {
                return _mDrugDeptSellPriceProfitScale_Mgnt && MenuVisibleCollection[0];
            }
            set
            {
                if (_mDrugDeptSellPriceProfitScale_Mgnt
                 == value)
                    return;
                _mDrugDeptSellPriceProfitScale_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptSellPriceProfitScale_Mgnt
                );
            }
        }
        public bool mDrugDeptSellPriceProfitScale_Medical_Mgnt
        {
            get
            {
                return _mDrugDeptSellPriceProfitScale_Medical_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellPriceProfitScale_Medical_Mgnt
                 == value)
                    return;
                _mDrugDeptSellPriceProfitScale_Medical_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptSellPriceProfitScale_Medical_Mgnt
                );
            }
        }
        public bool mDrugDeptSellPriceProfitScale_Chemical_Mgnt
        {
            get
            {
                return _mDrugDeptSellPriceProfitScale_Chemical_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellPriceProfitScale_Chemical_Mgnt
                 == value)
                    return;
                _mDrugDeptSellPriceProfitScale_Chemical_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptSellPriceProfitScale_Chemical_Mgnt
                );
            }
        }
        public bool mDrugDeptSellingItemPrices_Mgnt
        {
            get
            {
                return _mDrugDeptSellingItemPrices_Mgnt && MenuVisibleCollection[0];
            }
            set
            {
                if (_mDrugDeptSellingItemPrices_Mgnt
                 == value)
                    return;
                _mDrugDeptSellingItemPrices_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingItemPrices_Mgnt
                );
            }
        }
        public bool mDrugDeptSellingItemPrices_Medical_Mgnt
        {
            get
            {
                return _mDrugDeptSellingItemPrices_Medical_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingItemPrices_Medical_Mgnt
                 == value)
                    return;
                _mDrugDeptSellingItemPrices_Medical_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingItemPrices_Medical_Mgnt
                );
            }
        }
        public bool mDrugDeptSellingItemPrices_Chemical_Mgnt
        {
            get
            {
                return _mDrugDeptSellingItemPrices_Chemical_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingItemPrices_Chemical_Mgnt
                 == value)
                    return;
                _mDrugDeptSellingItemPrices_Chemical_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingItemPrices_Chemical_Mgnt
                );
            }
        }
        public bool mDrugDeptSellingPriceList_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_Mgnt && MenuVisibleCollection[0];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_Mgnt
                 == value)
                    return;
                _mDrugDeptSellingPriceList_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Mgnt
                );
            }
        }
        public bool mDrugDeptSellingPriceList_Medical_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_Medical_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_Medical_Mgnt
                 == value)
                    return;
                _mDrugDeptSellingPriceList_Medical_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Medical_Mgnt
                );
            }
        }
        public bool mDrugDeptSellingPriceList_Chemical_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_Chemical_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_Chemical_Mgnt == value)
                    return;
                _mDrugDeptSellingPriceList_Chemical_Mgnt = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Chemical_Mgnt);
            }
        }
        //them moi ngay 28-07-2012
        public bool mBaoCaoTheoDoiCongNo_Thuoc
        {
            get
            {
                return _mBaoCaoTheoDoiCongNo_Thuoc && MenuVisibleCollection[0];
            }
            set
            {
                if (_mBaoCaoTheoDoiCongNo_Thuoc == value)
                    return;
                _mBaoCaoTheoDoiCongNo_Thuoc = value;
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_Thuoc);
            }
        }
        public bool mBaoCaoTheoDoiCongNo_YCu
        {
            get
            {
                return _mBaoCaoTheoDoiCongNo_YCu && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBaoCaoTheoDoiCongNo_YCu == value)
                    return;
                _mBaoCaoTheoDoiCongNo_YCu = value;
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_YCu);
            }
        }
        public bool mBaoCaoTheoDoiCongNo_HoaChat
        {
            get
            {
                return _mBaoCaoTheoDoiCongNo_HoaChat && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBaoCaoTheoDoiCongNo_HoaChat == value)
                    return;
                _mBaoCaoTheoDoiCongNo_HoaChat = value;
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_HoaChat);
            }
        }
        public bool mDuyetPhieuLinhHang_Thuoc
        {
            get
            {
                return _mDuyetPhieuLinhHang_Thuoc && MenuVisibleCollection[0];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_Thuoc == value)
                    return;
                _mDuyetPhieuLinhHang_Thuoc = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Thuoc);
            }
        }
        public bool mDuyetPhieuLinhHang_YCu
        {
            get
            {
                return _mDuyetPhieuLinhHang_YCu && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_YCu == value)
                    return;
                _mDuyetPhieuLinhHang_YCu = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_YCu);
            }
        }
        public bool mDuyetPhieuLinhHang_HoaChat
        {
            get
            {
                return _mDuyetPhieuLinhHang_HoaChat && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_HoaChat == value)
                    return;
                _mDuyetPhieuLinhHang_HoaChat = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_HoaChat);
            }
        }
        public bool mNhapHangKyGui
        {
            get
            {
                return _mNhapHangKyGui;
            }
            set
            {
                if (_mNhapHangKyGui == value)
                    return;
                _mNhapHangKyGui = value;
                NotifyOfPropertyChange(() => mNhapHangKyGui);
            }
        }
        public bool mNhapHangKyGui_Thuoc
        {
            get
            {
                return _mNhapHangKyGui_Thuoc;
            }
            set
            {
                if (_mNhapHangKyGui_Thuoc == value)
                    return;
                _mNhapHangKyGui_Thuoc = value;
                NotifyOfPropertyChange(() => mNhapHangKyGui_Thuoc);
            }
        }
        public bool mNhapHangKyGui_YCu
        {
            get
            {
                return _mNhapHangKyGui_YCu;
            }
            set
            {
                if (_mNhapHangKyGui_YCu == value)
                    return;
                _mNhapHangKyGui_YCu = value;
                NotifyOfPropertyChange(() => mNhapHangKyGui_YCu);
            }
        }
        public bool mXuatHangKyGui_Thuoc
        {
            get
            {
                return _mXuatHangKyGui_Thuoc && MenuVisibleCollection[0];
            }
            set
            {
                if (_mXuatHangKyGui_Thuoc == value)
                    return;
                _mXuatHangKyGui_Thuoc = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc);
            }
        }
        public bool mXuatHangKyGui_YCu
        {
            get
            {
                return _mXuatHangKyGui_YCu && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatHangKyGui_YCu == value)
                    return;
                _mXuatHangKyGui_YCu = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu);
            }
        }
        public bool mXuatHangKyGui_HoaChat
        {
            get
            {
                return _mXuatHangKyGui_HoaChat && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatHangKyGui_HoaChat == value)
                    return;
                _mXuatHangKyGui_HoaChat = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat);
            }
        }
        public bool mSapNhapHangKyGui_Thuoc
        {
            get
            {
                return _mSapNhapHangKyGui_Thuoc && MenuVisibleCollection[0];
            }
            set
            {
                if (_mSapNhapHangKyGui_Thuoc == value)
                    return;
                _mSapNhapHangKyGui_Thuoc = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Thuoc);
            }
        }
        public bool mSapNhapHangKyGui_YCu
        {
            get
            {
                return _mSapNhapHangKyGui_YCu && MenuVisibleCollection[1];
            }
            set
            {
                if (_mSapNhapHangKyGui_YCu == value)
                    return;
                _mSapNhapHangKyGui_YCu = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_YCu);
            }
        }
        public bool mSapNhapHangKyGui_HoaChat
        {
            get
            {
                return _mSapNhapHangKyGui_HoaChat && MenuVisibleCollection[1];
            }
            set
            {
                if (_mSapNhapHangKyGui_HoaChat == value)
                    return;
                _mSapNhapHangKyGui_HoaChat = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_HoaChat);
            }
        }
        public bool mNhapTraTuKhoPhong_Thuoc
        {
            get
            {
                return _mNhapTraTuKhoPhong_Thuoc && MenuVisibleCollection[0];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_Thuoc == value)
                    return;
                _mNhapTraTuKhoPhong_Thuoc = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Thuoc);
            }
        }
        public bool mNhapTraTuKhoPhong_YCu
        {
            get
            {
                return _mNhapTraTuKhoPhong_YCu && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_YCu == value)
                    return;
                _mNhapTraTuKhoPhong_YCu = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_YCu);
            }
        }
        public bool mNhapTraTuKhoPhong_HoaChat
        {
            get
            {
                return _mNhapTraTuKhoPhong_HoaChat && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_HoaChat == value)
                    return;
                _mNhapTraTuKhoPhong_HoaChat = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_HoaChat);
            }
        }
        public bool mAdjustOutPrice_Med
        {
            get
            {
                return _mAdjustOutPrice_Med && MenuVisibleCollection[0];
            }
            set
            {
                if (_mAdjustOutPrice_Med == value)
                {
                    return;
                }
                _mAdjustOutPrice_Med = value;
                NotifyOfPropertyChange(() => mAdjustOutPrice_Med);
            }
        }
        public bool mAdjustOutPrice_Mat
        {
            get
            {
                return _mAdjustOutPrice_Mat && MenuVisibleCollection[1];
            }
            set
            {
                if (_mAdjustOutPrice_Mat == value)
                {
                    return;
                }
                _mAdjustOutPrice_Mat = value;
                NotifyOfPropertyChange(() => mAdjustOutPrice_Mat);
            }
        }
        public bool mAdjustOutPrice_Lab
        {
            get
            {
                return _mAdjustOutPrice_Lab && MenuVisibleCollection[1];
            }
            set
            {
                if (_mAdjustOutPrice_Lab == value)
                {
                    return;
                }
                _mAdjustOutPrice_Lab = value;
                NotifyOfPropertyChange(() => mAdjustOutPrice_Lab);
            }
        }
        public bool mReturnToSupplier
        {
            get
            {
                return _mReturnToSupplier;
            }
            set
            {
                if (_mReturnToSupplier == value)
                {
                    return;
                }
                _mReturnToSupplier = value;
                NotifyOfPropertyChange(() => mReturnToSupplier);
                //NotifyOfPropertyChange(() => mReturnToSupplier_Drug);
                //NotifyOfPropertyChange(() => mReturnToSupplier_Mat);
            }
        }
        //public bool mReturnToSupplier_Drug
        //{
        //    get
        //    {
        //        return mReturnToSupplier && MenuVisibleCollection[0];
        //    }
        //}
        //public bool mReturnToSupplier_Mat
        //{
        //    get
        //    {
        //        return mReturnToSupplier && MenuVisibleCollection[1];
        //    }
        //}
        public bool mAdjustOutPrice
        {
            get
            {
                return _mAdjustOutPrice;
            }
            set
            {
                if (_mAdjustOutPrice == value)
                {
                    return;
                }
                _mAdjustOutPrice = value;
                NotifyOfPropertyChange(() => mAdjustOutPrice);
                NotifyOfPropertyChange(() => mAdjustOutPrice_MedCmd);
                NotifyOfPropertyChange(() => mAdjustOutPrice_MatCmd);
                NotifyOfPropertyChange(() => mAdjustOutPrice_LabCmd);
            }
        }
        public bool mAdjustOutPrice_MedCmd
        {
            get
            {
                return mAdjustOutPrice && MenuVisibleCollection[0];
            }
        }
        public bool mAdjustOutPrice_MatCmd
        {
            get
            {
                return mAdjustOutPrice && MenuVisibleCollection[1];
            }
        }
        public bool mAdjustOutPrice_LabCmd
        {
            get
            {
                return mAdjustOutPrice && MenuVisibleCollection[1];
            }
        }
        public bool mInventory
        {
            get
            {
                return _mInventory;
            }
            set
            {
                if (_mInventory == value)
                {
                    return;
                }
                _mInventory = value;
                NotifyOfPropertyChange(() => mInventory);
                NotifyOfPropertyChange(() => mInventory_MedCmd);
                NotifyOfPropertyChange(() => mInventory_MatCmd);
                NotifyOfPropertyChange(() => mInventory_LabCmd);
            }
        }
        public bool mInventory_MedCmd
        {
            get
            {
                return mInventory && MenuVisibleCollection[0];
            }
        }
        public bool mInventory_MatCmd
        {
            get
            {
                return mInventory && MenuVisibleCollection[1];
            }
        }
        public bool mInventory_LabCmd
        {
            get
            {
                return mInventory && MenuVisibleCollection[1];
            }
        }
        public bool mUseFollowBid
        {
            get
            {
                return _mUseFollowBid;
            }
            set
            {
                if (_mUseFollowBid == value)
                {
                    return;
                }
                _mUseFollowBid = value;
                NotifyOfPropertyChange(() => mUseFollowBid);
                NotifyOfPropertyChange(() => mUseFollowBid_MedCmd);
                NotifyOfPropertyChange(() => mUseFollowBid_MatCmd);
            }
        }
        public bool mUseFollowBid_MedCmd
        {
            get
            {
                return mUseFollowBid && MenuVisibleCollection[0];
            }
        }
        public bool mUseFollowBid_MatCmd
        {
            get
            {
                return mUseFollowBid && MenuVisibleCollection[1];
            }
        }
        #endregion
        #region Operation
        private bool _mXuat_Thuoc_Tim = true;
        private bool _mXuat_Thuoc_PhieuMoi = true;
        private bool _mXuat_Thuoc_ThucHien = true;
        private bool _mXuat_Thuoc_ThuTien = true;
        private bool _mXuat_Thuoc_In = true;
        private bool _mXuat_Thuoc_DeleteInvoice = true;
        private bool _mXuat_Thuoc_PrintReceipt = true;
        private bool _mXuat_YCu_Tim = true;
        private bool _mXuat_YCu_PhieuMoi = true;
        private bool _mXuat_YCu_ThucHien = true;
        private bool _mXuat_YCu_ThuTien = true;
        private bool _mXuat_YCu_In = true;
        private bool _mXuat_YCu_DeleteInvoice = true;
        private bool _mXuat_YCu_PrintReceipt = true;
        private bool _mXuat_HoaChat_Tim = true;
        private bool _mXuat_HoaChat_PhieuMoi = true;
        private bool _mXuat_HoaChat_ThucHien = true;
        private bool _mXuat_HoaChat_ThuTien = true;
        private bool _mXuat_HoaChat_In = true;
        private bool _mXuat_HoaChat_DeleteInvoice = true;
        private bool _mXuat_HoaChat_PrintReceipt = true;
        private bool _mTraHang_Thuoc_Tim = true;
        private bool _mTraHang_Thuoc_Luu = true;
        private bool _mTraHang_Thuoc_TraTien = true;
        private bool _mTraHang_Thuoc_In = true;
        private bool _mTraHang_YCu_Tim = true;
        private bool _mTraHang_YCu_Luu = true;
        private bool _mTraHang_YCu_TraTien = true;
        private bool _mTraHang_YCu_In = true;
        private bool _mTraHang_HoaChat_Tim = true;
        private bool _mTraHang_HoaChat_Luu = true;
        private bool _mTraHang_HoaChat_TraTien = true;
        private bool _mTraHang_HoaChat_In = true;
        private bool _mXuatHuy_Thuoc_Tim = true;
        private bool _mXuatHuy_Thuoc_ThemMoi = true;
        private bool _mXuatHuy_Thuoc_XuatExcel = true;
        private bool _mXuatHuy_Thuoc_XemIn = true;
        private bool _mXuatHuy_YCu_Tim = true;
        private bool _mXuatHuy_YCu_ThemMoi = true;
        private bool _mXuatHuy_YCu_XuatExcel = true;
        private bool _mXuatHuy_YCu_XemIn = true;
        private bool _mXuatHuy_HoaChat_Tim = true;
        private bool _mXuatHuy_HoaChat_ThemMoi = true;
        private bool _mXuatHuy_HoaChat_XuatExcel = true;
        private bool _mXuatHuy_HoaChat_XemIn = true;
        private bool _mDuTru_Thuoc_Tim = true;
        private bool _mDuTru_Thuoc_ThemMoi = true;
        private bool _mDuTru_Thuoc_Xoa = true;
        private bool _mDuTru_Thuoc_XemIn = true;
        private bool _mDuTru_YCu_Tim = true;
        private bool _mDuTru_YCu_ThemMoi = true;
        private bool _mDuTru_YCu_Xoa = true;
        private bool _mDuTru_YCu_XemIn = true;
        private bool _mDuTru_HoaChat_Tim = true;
        private bool _mDuTru_HoaChat_ThemMoi = true;
        private bool _mDuTru_HoaChat_Xoa = true;
        private bool _mDuTru_HoaChat_XemIn = true;
        private bool _mDatHang_Thuoc_DSCanDatHang = true;
        private bool _mDatHang_Thuoc_Tim = true;
        private bool _mDatHang_Thuoc_ChinhSua = true;
        private bool _mDatHang_Thuoc_ThemMoi = true;
        private bool _mDatHang_Thuoc_In = true;
        private bool _mDatHang_YCu_DSCanDatHang = true;
        private bool _mDatHang_YCu_Tim = true;
        private bool _mDatHang_YCu_ChinhSua = true;
        private bool _mDatHang_YCu_ThemMoi = true;
        private bool _mDatHang_YCu_In = true;
        private bool _mDatHang_HoaChat_DSCanDatHang = true;
        private bool _mDatHang_HoaChat_Tim = true;
        private bool _mDatHang_HoaChat_ChinhSua = true;
        private bool _mDatHang_HoaChat_ThemMoi = true;
        private bool _mDatHang_HoaChat_In = true;
        private bool _mNhapHangNCC_Thuoc_Tim = true;
        private bool _mNhapHangNCC_Thuoc_ThemMoi = true;
        private bool _mNhapHangNCC_Thuoc_CapNhat = true;
        private bool _mNhapHangNCC_Thuoc_In = true;
        private bool _mNhapHangNCC_YCu_Tim = true;
        private bool _mNhapHangNCC_YCu_ThemMoi = true;
        private bool _mNhapHangNCC_YCu_CapNhat = true;
        private bool _mNhapHangNCC_YCu_In = true;
        private bool _mNhapHangNCC_HoaChat_Tim = true;
        private bool _mNhapHangNCC_HoaChat_ThemMoi = true;
        private bool _mNhapHangNCC_HoaChat_CapNhat = true;
        private bool _mNhapHangNCC_HoaChat_In = true;
        private bool _mNhapPhanBoPhi_Thuoc_Tim = true;
        private bool _mNhapPhanBoPhi_Thuoc_ChinhSua_Them = true;
        private bool _mNhapPhanBoPhi_Thuoc_In = true;
        private bool _mNhapPhanBoPhi_YCu_Tim = true;
        private bool _mNhapPhanBoPhi_YCu_ChinhSua_Them = true;
        private bool _mNhapPhanBoPhi_YCu_In = true;
        private bool _mNhapPhanBoPhi_HoaChat_Tim = true;
        private bool _mNhapPhanBoPhi_HoaChat_ChinhSua_Them = true;
        private bool _mNhapPhanBoPhi_HoaChat_In = true;
        private bool _mPhieuDNThanhToan_Thuoc_Tim = true;
        private bool _mPhieuDNThanhToan_Thuoc_PhieuMoi = true;
        private bool _mPhieuDNThanhToan_Thuoc_Xoa = true;
        private bool _mPhieuDNThanhToan_Thuoc_XemInBK = true;
        private bool _mPhieuDNThanhToan_Thuoc_XemInPDNTT = true;
        private bool _mPhieuDNThanhToan_YCu_Tim = true;
        private bool _mPhieuDNThanhToan_YCu_PhieuMoi = true;
        private bool _mPhieuDNThanhToan_YCu_Xoa = true;
        private bool _mPhieuDNThanhToan_YCu_XemInBK = true;
        private bool _mPhieuDNThanhToan_YCu_XemInPDNTT = true;
        private bool _mPhieuDNThanhToan_HoaChat_Tim = true;
        private bool _mPhieuDNThanhToan_HoaChat_PhieuMoi = true;
        private bool _mPhieuDNThanhToan_HoaChat_Xoa = true;
        private bool _mPhieuDNThanhToan_HoaChat_XemInBK = true;
        private bool _mPhieuDNThanhToan_HoaChat_XemInPDNTT = true;
        private bool _mKiemKe_Thuoc_Tim = true;
        private bool _mKiemKe_Thuoc_ThemMoi = true;
        private bool _mKiemKe_Thuoc_XuatExcel = true;
        private bool _mKiemKe_Thuoc_XemIn = true;
        private bool _mKiemKe_YCu_Tim = true;
        private bool _mKiemKe_YCu_ThemMoi = true;
        private bool _mKiemKe_YCu_XuatExcel = true;
        private bool _mKiemKe_YCu_XemIn = true;
        private bool _mKiemKe_HoaChat_Tim = true;
        private bool _mKiemKe_HoaChat_ThemMoi = true;
        private bool _mKiemKe_HoaChat_XuatExcel = true;
        private bool _mKiemKe_HoaChat_XemIn = true;
        private bool _mBaoCaoXuat_Thuoc_XemIn = true;
        private bool _mBaoCaoXuat_YCu_XemIn = true;
        private bool _mBaoCaoXuat_HoaChat_XemIn = true;
        /*TMA 23/10/2017*/
        private bool _mBaoCaoNhap_Thuoc_XemIn = true;
        private bool _mBaoCaoNhap_YCu_XemIn = true;
        private bool _mBaoCaoNhap_HoaChat_XemIn = true;
        /*TMA 23/10/2017*/
        private bool _mBaoCaoXuatNhapTon_Thuoc_XemIn = true;
        private bool _mBaoCaoXuatNhapTon_Thuoc_KetChuyen = true;
        private bool _mBaoCaoXuatNhapTon_YCu_XemIn = true;
        private bool _mBaoCaoXuatNhapTon_YCu_KetChuyen = true;
        private bool _mBaoCaoXuatNhapTon_HoaChat_XemIn = true;
        private bool _mBaoCaoXuatNhapTon_HoaChat_KetChuyen = true;
        private bool _mBaoCaoTheKho_Thuoc_Xem = true;
        private bool _mBaoCaoTheKho_Thuoc_In = true;
        private bool _mBaoCaoTheKho_YCu_Xem = true;
        private bool _mBaoCaoTheKho_YCu_In = true;
        private bool _mBaoCaoTheKho_HoaChat_Xem = true;
        private bool _mBaoCaoTheKho_HoaChat_In = true;
        private bool _mQLNCCNSX_NCC_Tim = true;
        private bool _mQLNCCNSX_NCC_ThemMoi = true;
        private bool _mQLNCCNSX_NCC_ChinhSua = true;
        private bool _mQLNCCNSX_NSX_Tim = true;
        private bool _mQLNCCNSX_NSX_ThemMoi = true;
        private bool _mQLNCCNSX_NSX_ChinhSua = true;
        private bool _mQLNCCNSX_NCCNSX_Tim = true;
        private bool _mQLNCCNSX_NCCNSX_ThemMoi = true;
        private bool _mQLNCCNSX_NCCNSX_ChinhSua = true;
        private bool _mQLDanhMuc_Thuoc_Tim = true;
        private bool _mQLDanhMuc_Thuoc_ThemMoi = true;
        private bool _mQLDanhMuc_Thuoc_ChinhSua = true;
        private bool _mQLDanhMuc_YCu_Tim = true;
        private bool _mQLDanhMuc_YCu_ThemMoi = true;
        private bool _mQLDanhMuc_YCu_ChinhSua = true;
        private bool _mQLDanhMuc_HoaChat_Tim = true;
        private bool _mQLDanhMuc_HoaChat_ThemMoi = true;
        private bool _mQLDanhMuc_HoaChat_ChinhSua = true;
        private bool _mQLNhomHang_Thuoc_Tim = true;
        private bool _mQLNhomHang_Thuoc_ThemMoi = true;
        private bool _mQLNhomHang_Thuoc_ChinhSua = true;
        private bool _mQLNhomHang_YCu_Tim = true;
        private bool _mQLNhomHang_YCu_ThemMoi = true;
        private bool _mQLNhomHang_YCu_ChinhSua = true;
        private bool _mQLNhomHang_HoaChat_Tim = true;
        private bool _mQLNhomHang_HoaChat_ThemMoi = true;
        private bool _mQLNhomHang_HoaChat_ChinhSua = true;


        /*▼====: #005*/
        private bool _mQLNhomHang_HoatChat_Tim = true;
        private bool _mQLNhomHang_HoatChat_ThemMoi = true;
        private bool _mQLNhomHang_HoatChat_ChinhSua = true;
        /*▲====: #005*/
        private bool _mGiaTuNCC_Thuoc_Tim = true;
        private bool _mGiaTuNCC_Thuoc_QuanLy = true;
        private bool _mGiaTuNCC_Thuoc_TaoGiaMoi = true;
        private bool _mGiaTuNCC_Thuoc_SuaGia = true;
        private bool _mGiaTuNCC_YCu_Tim = true;
        private bool _mGiaTuNCC_YCu_QuanLy = true;
        private bool _mGiaTuNCC_YCu_TaoGiaMoi = true;
        private bool _mGiaTuNCC_YCu_SuaGia = true;
        private bool _mGiaTuNCC_HoaChat_Tim = true;
        private bool _mGiaTuNCC_HoaChat_QuanLy = true;
        private bool _mGiaTuNCC_HoaChat_TaoGiaMoi = true;
        private bool _mGiaTuNCC_HoaChat_SuaGia = true;
        private bool _mThangGiaBan_Thuoc_Tim = true;
        private bool _mThangGiaBan_Thuoc_TaoMoiCTGia = true;
        private bool _mThangGiaBan_Thuoc_ChinhSua = true;
        private bool _mThangGiaBan_YCu_Tim = true;
        private bool _mThangGiaBan_YCu_TaoMoiCTGia = true;
        private bool _mThangGiaBan_YCu_ChinhSua = true;
        private bool _mThangGiaBan_HoaChat_Tim = true;
        private bool _mThangGiaBan_HoaChat_TaoMoiCTGia = true;
        private bool _mThangGiaBan_HoaChat_ChinhSua = true;
        private bool _mGiaBan_Thuoc_Tim = true;
        private bool _mGiaBan_Thuoc_ChinhSua = true;
        private bool _mGiaBan_Thuoc_XemDSGia = true;
        private bool _mGiaBan_Thuoc_TaoGiaMoi = true;
        private bool _mGiaBan_Thuoc_ChinhSuaGia = true;
        private bool _mGiaBan_YCu_Tim = true;
        private bool _mGiaBan_YCu_ChinhSua = true;
        private bool _mGiaBan_YCu_XemDSGia = true;
        private bool _mGiaBan_YCu_TaoGiaMoi = true;
        private bool _mGiaBan_YCu_ChinhSuaGia = true;
        private bool _mGiaBan_HoaChat_Tim = true;
        private bool _mGiaBan_HoaChat_ChinhSua = true;
        private bool _mGiaBan_HoaChat_XemDSGia = true;
        private bool _mGiaBan_HoaChat_TaoGiaMoi = true;
        private bool _mGiaBan_HoaChat_ChinhSuaGia = true;
        private bool _mBangGiaBan_Thuoc_Xem = true;
        private bool _mBangGiaBan_Thuoc_ChinhSua = true;
        private bool _mBangGiaBan_Thuoc_TaoBangGia = true;
        private bool _mBangGiaBan_Thuoc_PreView = true;
        private bool _mBangGiaBan_Thuoc_In = true;
        private bool _mBangGiaBan_YCu_Xem = true;
        private bool _mBangGiaBan_YCu_ChinhSua = true;
        private bool _mBangGiaBan_YCu_TaoBangGia = true;
        private bool _mBangGiaBan_YCu_PreView = true;
        private bool _mBangGiaBan_YCu_In = true;
        private bool _mBangGiaBan_HoaChat_Xem = true;
        private bool _mBangGiaBan_HoaChat_ChinhSua = true;
        private bool _mBangGiaBan_HoaChat_TaoBangGia = true;
        private bool _mBangGiaBan_HoaChat_PreView = true;
        private bool _mBangGiaBan_HoaChat_In = true;
        //--moi them phan moi cua Ny
        private bool _mPhieuDeNghiThanhToan_Thuoc_Tim = true;
        private bool _mPhieuDeNghiThanhToan_Thuoc_ThemMoi = true;
        private bool _mPhieuDeNghiThanhToan_Thuoc_ChinhSua = true;
        private bool _mPhieuDeNghiThanhToan_Thuoc_XemInBK = true;
        private bool _mPhieuDeNghiThanhToan_Thuoc_XemInPDNTT = true;
        private bool _mPhieuDeNghiThanhToan_YCu_Tim = true;
        private bool _mPhieuDeNghiThanhToan_YCu_ThemMoi = true;
        private bool _mPhieuDeNghiThanhToan_YCu_ChinhSua = true;
        private bool _mPhieuDeNghiThanhToan_YCu_XemInBK = true;
        private bool _mPhieuDeNghiThanhToan_YCu_XemInPDNTT = true;
        private bool _mPhieuDeNghiThanhToan_HoaChat_Tim = true;
        private bool _mPhieuDeNghiThanhToan_HoaChat_ThemMoi = true;
        private bool _mPhieuDeNghiThanhToan_HoaChat_ChinhSua = true;
        private bool _mPhieuDeNghiThanhToan_HoaChat_XemInBK = true;
        private bool _mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT = true;
        private bool _mBaoCaoDSPhieuNhapKho_Thuoc_XemIn = true;
        private bool _mBaoCaoDSPhieuNhapKho_Thuoc_In = true;
        private bool _mBaoCaoDSPhieuNhapKho_YCu_XemIn = true;
        private bool _mBaoCaoDSPhieuNhapKho_YCu_In = true;
        private bool _mBaoCaoDSPhieuNhapKho_HoaChat_XemIn = true;
        private bool _mBaoCaoDSPhieuNhapKho_HoaChat_In = true;
        private bool _mBaoCaoSuDung_Thuoc_XemIn = true;
        private bool _mBaoCaoSuDung_Thuoc_In = true;
        private bool _mBaoCaoNhapXuatDenKhoaPhong_Thuoc_In = true;
        private bool _mBaoCaoNhapXuatDenKhoaPhong_Thuoc_XuatExcel = true;
        private bool _mBaoCaoNhapXuatDenKhoaPhong_YCu_In = true;
        private bool _mBaoCaoNhapXuatDenKhoaPhong_YCu_XuatExcel = true;
        private bool _mBaoCaoNhapXuatDenKhoaPhong_HoaChat_In = true;
        private bool _mBaoCaoNhapXuatDenKhoaPhong_HoaChat_XuatExcel = true;
        //them ngay 28-07-2012
        private bool _mBaoCaoTheoDoiCongNo_Thuoc_Xem = true;
        private bool _mBaoCaoTheoDoiCongNo_Thuoc_In = true;
        private bool _mBaoCaoTheoDoiCongNo_YCu_Xem = true;
        private bool _mBaoCaoTheoDoiCongNo_YCu_In = true;
        private bool _mBaoCaoTheoDoiCongNo_HoaChat_Xem = true;
        private bool _mBaoCaoTheoDoiCongNo_HoaChat_In = true;
        private bool _mDuyetPhieuLinhHang_Thuoc_Tim = true;
        private bool _mDuyetPhieuLinhHang_Thuoc_PhieuMoi = true;
        private bool _mDuyetPhieuLinhHang_Thuoc_XuatHang = true;
        private bool _mDuyetPhieuLinhHang_Thuoc_XemInTH = true;
        private bool _mDuyetPhieuLinhHang_Thuoc_XemInCT = true;
        private bool _mDuyetPhieuLinhHang_YCu_Tim = true;
        private bool _mDuyetPhieuLinhHang_YCu_PhieuMoi = true;
        private bool _mDuyetPhieuLinhHang_YCu_XuatHang = true;
        private bool _mDuyetPhieuLinhHang_YCu_XemInTH = true;
        private bool _mDuyetPhieuLinhHang_YCu_XemInCT = true;
        private bool _mDuyetPhieuLinhHang_HoaChat_Tim = true;
        private bool _mDuyetPhieuLinhHang_HoaChat_PhieuMoi = true;
        private bool _mDuyetPhieuLinhHang_HoaChat_XuatHang = true;
        private bool _mDuyetPhieuLinhHang_HoaChat_XemInTH = true;
        private bool _mDuyetPhieuLinhHang_HoaChat_XemInCT = true;
        private bool _mXuatHangKyGui_Thuoc_Tim = true;
        private bool _mXuatHangKyGui_Thuoc_PhieuMoi = true;
        private bool _mXuatHangKyGui_Thuoc_Save = true;
        private bool _mXuatHangKyGui_Thuoc_ThuTien = true;
        private bool _mXuatHangKyGui_Thuoc_XemIn = true;
        private bool _mXuatHangKyGui_Thuoc_In = true;
        private bool _mXuatHangKyGui_Thuoc_DeleteInvoice = true;
        private bool _mXuatHangKyGui_Thuoc_PrintReceipt = true;
        private bool _mXuatHangKyGui_YCu_Tim = true;
        private bool _mXuatHangKyGui_YCu_PhieuMoi = true;
        private bool _mXuatHangKyGui_YCu_Save = true;
        private bool _mXuatHangKyGui_YCu_ThuTien = true;
        private bool _mXuatHangKyGui_YCu_XemIn = true;
        private bool _mXuatHangKyGui_YCu_In = true;
        private bool _mXuatHangKyGui_YCu_DeleteInvoice = true;
        private bool _mXuatHangKyGui_YCu_PrintReceipt = true;
        private bool _mXuatHangKyGui_HoaChat_Tim = true;
        private bool _mXuatHangKyGui_HoaChat_PhieuMoi = true;
        private bool _mXuatHangKyGui_HoaChat_Save = true;
        private bool _mXuatHangKyGui_HoaChat_ThuTien = true;
        private bool _mXuatHangKyGui_HoaChat_XemIn = true;
        private bool _mXuatHangKyGui_HoaChat_In = true;
        private bool _mXuatHangKyGui_HoaChat_DeleteInvoice = true;
        private bool _mXuatHangKyGui_HoaChat_PrintReceipt = true;
        private bool _mSapNhapHangKyGui_Thuoc_Tim = true;
        private bool _mSapNhapHangKyGui_Thuoc_PhieuMoi = true;
        private bool _mSapNhapHangKyGui_Thuoc_CapNhat = true;
        private bool _mSapNhapHangKyGui_Thuoc_Xoa = true;
        private bool _mSapNhapHangKyGui_Thuoc_XemIn = true;
        private bool _mSapNhapHangKyGui_Thuoc_In = true;
        private bool _mSapNhapHangKyGui_YCu_Tim = true;
        private bool _mSapNhapHangKyGui_YCu_PhieuMoi = true;
        private bool _mSapNhapHangKyGui_YCu_CapNhat = true;
        private bool _mSapNhapHangKyGui_YCu_Xoa = true;
        private bool _mSapNhapHangKyGui_YCu_XemIn = true;
        private bool _mSapNhapHangKyGui_YCu_In = true;
        private bool _mSapNhapHangKyGui_HoaChat_Tim = true;
        private bool _mSapNhapHangKyGui_HoaChat_PhieuMoi = true;
        private bool _mSapNhapHangKyGui_HoaChat_CapNhat = true;
        private bool _mSapNhapHangKyGui_HoaChat_Xoa = true;
        private bool _mSapNhapHangKyGui_HoaChat_XemIn = true;
        private bool _mSapNhapHangKyGui_HoaChat_In = true;
        private bool _mNhapTraTuKhoPhong_Thuoc_Tim = true;
        private bool _mNhapTraTuKhoPhong_Thuoc_PhieuMoi = true;
        private bool _mNhapTraTuKhoPhong_Thuoc_XemIn = true;
        private bool _mNhapTraTuKhoPhong_Thuoc_In = true;
        private bool _mNhapTraTuKhoPhong_YCu_Tim = true;
        private bool _mNhapTraTuKhoPhong_YCu_PhieuMoi = true;
        private bool _mNhapTraTuKhoPhong_YCu_XemIn = true;
        private bool _mNhapTraTuKhoPhong_YCu_In = true;
        private bool _mNhapTraTuKhoPhong_HoaChat_Tim = true;
        private bool _mNhapTraTuKhoPhong_HoaChat_PhieuMoi = true;
        private bool _mNhapTraTuKhoPhong_HoaChat_XemIn = true;
        private bool _mNhapTraTuKhoPhong_HoaChat_In = true;
        private bool _mWatchMedOutQty_Preview = true;
        private bool _mWatchMedOutQty_Print = true;
        private bool _mWatchMatOutQty_Preview = true;
        private bool _mWatchMatOutQty_Print = true;
        private bool _mWatchLabOutQty_Preview = true;
        private bool _mWatchLabOutQty_Print = true;

        private bool _mQLDanhMuc_Nutrition_Tim = true;
        private bool _mQLDanhMuc_Nutrition_ThemMoi = true;
        private bool _mQLDanhMuc_Nutrition_ChinhSua = true;

        public bool mXuat_Thuoc_Tim
        {
            get
            {
                return _mXuat_Thuoc_Tim;
            }
            set
            {
                if (_mXuat_Thuoc_Tim == value)
                    return;
                _mXuat_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mXuat_Thuoc_Tim);
            }
        }
        public bool mXuat_Thuoc_PhieuMoi
        {
            get
            {
                return _mXuat_Thuoc_PhieuMoi;
            }
            set
            {
                if (_mXuat_Thuoc_PhieuMoi == value)
                    return;
                _mXuat_Thuoc_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuat_Thuoc_PhieuMoi);
            }
        }
        public bool mXuat_Thuoc_ThucHien
        {
            get
            {
                return _mXuat_Thuoc_ThucHien;
            }
            set
            {
                if (_mXuat_Thuoc_ThucHien == value)
                    return;
                _mXuat_Thuoc_ThucHien = value;
                NotifyOfPropertyChange(() => mXuat_Thuoc_ThucHien);
            }
        }
        public bool mXuat_Thuoc_ThuTien
        {
            get
            {
                return _mXuat_Thuoc_ThuTien;
            }
            set
            {
                if (_mXuat_Thuoc_ThuTien == value)
                    return;
                _mXuat_Thuoc_ThuTien = value;
                NotifyOfPropertyChange(() => mXuat_Thuoc_ThuTien);
            }
        }
        public bool mXuat_Thuoc_In
        {
            get
            {
                return _mXuat_Thuoc_In;
            }
            set
            {
                if (_mXuat_Thuoc_In == value)
                    return;
                _mXuat_Thuoc_In = value;
                NotifyOfPropertyChange(() => mXuat_Thuoc_In);
            }
        }
        public bool mXuat_Thuoc_DeleteInvoice
        {
            get
            {
                return _mXuat_Thuoc_DeleteInvoice;
            }
            set
            {
                if (_mXuat_Thuoc_DeleteInvoice == value)
                    return;
                _mXuat_Thuoc_DeleteInvoice = value;
                NotifyOfPropertyChange(() => mXuat_Thuoc_DeleteInvoice);
            }
        }
        public bool mXuat_Thuoc_PrintReceipt
        {
            get
            {
                return _mXuat_Thuoc_PrintReceipt;
            }
            set
            {
                if (_mXuat_Thuoc_PrintReceipt == value)
                    return;
                _mXuat_Thuoc_PrintReceipt = value;
                NotifyOfPropertyChange(() => mXuat_Thuoc_PrintReceipt);
            }
        }
        public bool mXuat_YCu_Tim
        {
            get
            {
                return _mXuat_YCu_Tim;
            }
            set
            {
                if (_mXuat_YCu_Tim == value)
                    return;
                _mXuat_YCu_Tim = value;
                NotifyOfPropertyChange(() => mXuat_YCu_Tim);
            }
        }
        public bool mXuat_YCu_PhieuMoi
        {
            get
            {
                return _mXuat_YCu_PhieuMoi;
            }
            set
            {
                if (_mXuat_YCu_PhieuMoi == value)
                    return;
                _mXuat_YCu_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuat_YCu_PhieuMoi);
            }
        }
        public bool mXuat_YCu_ThucHien
        {
            get
            {
                return _mXuat_YCu_ThucHien;
            }
            set
            {
                if (_mXuat_YCu_ThucHien == value)
                    return;
                _mXuat_YCu_ThucHien = value;
                NotifyOfPropertyChange(() => mXuat_YCu_ThucHien);
            }
        }
        public bool mXuat_YCu_ThuTien
        {
            get
            {
                return _mXuat_YCu_ThuTien;
            }
            set
            {
                if (_mXuat_YCu_ThuTien == value)
                    return;
                _mXuat_YCu_ThuTien = value;
                NotifyOfPropertyChange(() => mXuat_YCu_ThuTien);
            }
        }
        public bool mXuat_YCu_In
        {
            get
            {
                return _mXuat_YCu_In;
            }
            set
            {
                if (_mXuat_YCu_In == value)
                    return;
                _mXuat_YCu_In = value;
                NotifyOfPropertyChange(() => mXuat_YCu_In);
            }
        }
        public bool mXuat_YCu_DeleteInvoice
        {
            get
            {
                return _mXuat_YCu_DeleteInvoice;
            }
            set
            {
                if (_mXuat_YCu_DeleteInvoice == value)
                    return;
                _mXuat_YCu_DeleteInvoice = value;
                NotifyOfPropertyChange(() => mXuat_YCu_DeleteInvoice);
            }
        }
        public bool mXuat_YCu_PrintReceipt
        {
            get
            {
                return _mXuat_YCu_PrintReceipt;
            }
            set
            {
                if (_mXuat_YCu_PrintReceipt == value)
                    return;
                _mXuat_YCu_PrintReceipt = value;
                NotifyOfPropertyChange(() => mXuat_YCu_PrintReceipt);
            }
        }
        public bool mXuat_HoaChat_Tim
        {
            get
            {
                return _mXuat_HoaChat_Tim;
            }
            set
            {
                if (_mXuat_HoaChat_Tim == value)
                    return;
                _mXuat_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mXuat_HoaChat_Tim);
            }
        }
        public bool mXuat_HoaChat_PhieuMoi
        {
            get
            {
                return _mXuat_HoaChat_PhieuMoi;
            }
            set
            {
                if (_mXuat_HoaChat_PhieuMoi == value)
                    return;
                _mXuat_HoaChat_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuat_HoaChat_PhieuMoi);
            }
        }
        public bool mXuat_HoaChat_ThucHien
        {
            get
            {
                return _mXuat_HoaChat_ThucHien;
            }
            set
            {
                if (_mXuat_HoaChat_ThucHien == value)
                    return;
                _mXuat_HoaChat_ThucHien = value;
                NotifyOfPropertyChange(() => mXuat_HoaChat_ThucHien);
            }
        }

        public bool mXuat_HoaChat_ThuTien
        {
            get
            {
                return _mXuat_HoaChat_ThuTien;
            }
            set
            {
                if (_mXuat_HoaChat_ThuTien == value)
                    return;
                _mXuat_HoaChat_ThuTien = value;
                NotifyOfPropertyChange(() => mXuat_HoaChat_ThuTien);
            }
        }

        public bool mXuat_HoaChat_In
        {
            get
            {
                return _mXuat_HoaChat_In;
            }
            set
            {
                if (_mXuat_HoaChat_In == value)
                    return;
                _mXuat_HoaChat_In = value;
                NotifyOfPropertyChange(() => mXuat_HoaChat_In);
            }
        }


        public bool mXuat_HoaChat_DeleteInvoice
        {
            get
            {
                return _mXuat_HoaChat_DeleteInvoice;
            }
            set
            {
                if (_mXuat_HoaChat_DeleteInvoice == value)
                    return;
                _mXuat_HoaChat_DeleteInvoice = value;
                NotifyOfPropertyChange(() => mXuat_HoaChat_DeleteInvoice);
            }
        }

        public bool mXuat_HoaChat_PrintReceipt
        {
            get
            {
                return _mXuat_HoaChat_PrintReceipt;
            }
            set
            {
                if (_mXuat_HoaChat_PrintReceipt == value)
                    return;
                _mXuat_HoaChat_PrintReceipt = value;
                NotifyOfPropertyChange(() => mXuat_HoaChat_PrintReceipt);
            }
        }

        public bool mTraHang_Thuoc_Tim
        {
            get
            {
                return _mTraHang_Thuoc_Tim;
            }
            set
            {
                if (_mTraHang_Thuoc_Tim == value)
                    return;
                _mTraHang_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mTraHang_Thuoc_Tim);
            }
        }

        public bool mTraHang_Thuoc_Luu
        {
            get
            {
                return _mTraHang_Thuoc_Luu;
            }
            set
            {
                if (_mTraHang_Thuoc_Luu == value)
                    return;
                _mTraHang_Thuoc_Luu = value;
                NotifyOfPropertyChange(() => mTraHang_Thuoc_Luu);
            }
        }

        public bool mTraHang_Thuoc_TraTien
        {
            get
            {
                return _mTraHang_Thuoc_TraTien;
            }
            set
            {
                if (_mTraHang_Thuoc_TraTien == value)
                    return;
                _mTraHang_Thuoc_TraTien = value;
                NotifyOfPropertyChange(() => mTraHang_Thuoc_TraTien);
            }
        }

        public bool mTraHang_Thuoc_In
        {
            get
            {
                return _mTraHang_Thuoc_In;
            }
            set
            {
                if (_mTraHang_Thuoc_In == value)
                    return;
                _mTraHang_Thuoc_In = value;
                NotifyOfPropertyChange(() => mTraHang_Thuoc_In);
            }
        }



        public bool mTraHang_YCu_Tim
        {
            get
            {
                return _mTraHang_YCu_Tim;
            }
            set
            {
                if (_mTraHang_YCu_Tim == value)
                    return;
                _mTraHang_YCu_Tim = value;
                NotifyOfPropertyChange(() => mTraHang_YCu_Tim);
            }
        }

        public bool mTraHang_YCu_Luu
        {
            get
            {
                return _mTraHang_YCu_Luu;
            }
            set
            {
                if (_mTraHang_YCu_Luu == value)
                    return;
                _mTraHang_YCu_Luu = value;
                NotifyOfPropertyChange(() => mTraHang_YCu_Luu);
            }
        }

        public bool mTraHang_YCu_TraTien
        {
            get
            {
                return _mTraHang_YCu_TraTien;
            }
            set
            {
                if (_mTraHang_YCu_TraTien == value)
                    return;
                _mTraHang_YCu_TraTien = value;
                NotifyOfPropertyChange(() => mTraHang_YCu_TraTien);
            }
        }

        public bool mTraHang_YCu_In
        {
            get
            {
                return _mTraHang_YCu_In;
            }
            set
            {
                if (_mTraHang_YCu_In == value)
                    return;
                _mTraHang_YCu_In = value;
                NotifyOfPropertyChange(() => mTraHang_YCu_In);
            }
        }


        public bool mTraHang_HoaChat_Tim
        {
            get
            {
                return _mTraHang_HoaChat_Tim;
            }
            set
            {
                if (_mTraHang_HoaChat_Tim == value)
                    return;
                _mTraHang_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mTraHang_HoaChat_Tim);
            }
        }

        public bool mTraHang_HoaChat_Luu
        {
            get
            {
                return _mTraHang_HoaChat_Luu;
            }
            set
            {
                if (_mTraHang_HoaChat_Luu == value)
                    return;
                _mTraHang_HoaChat_Luu = value;
                NotifyOfPropertyChange(() => mTraHang_HoaChat_Luu);
            }
        }

        public bool mTraHang_HoaChat_TraTien
        {
            get
            {
                return _mTraHang_HoaChat_TraTien;
            }
            set
            {
                if (_mTraHang_HoaChat_TraTien == value)
                    return;
                _mTraHang_HoaChat_TraTien = value;
                NotifyOfPropertyChange(() => mTraHang_HoaChat_TraTien);
            }
        }

        public bool mTraHang_HoaChat_In
        {
            get
            {
                return _mTraHang_HoaChat_In;
            }
            set
            {
                if (_mTraHang_HoaChat_In == value)
                    return;
                _mTraHang_HoaChat_In = value;
                NotifyOfPropertyChange(() => mTraHang_HoaChat_In);
            }
        }


        public bool mXuatHuy_Thuoc_Tim
        {
            get
            {
                return _mXuatHuy_Thuoc_Tim;
            }
            set
            {
                if (_mXuatHuy_Thuoc_Tim == value)
                    return;
                _mXuatHuy_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mXuatHuy_Thuoc_Tim);
            }
        }

        public bool mXuatHuy_Thuoc_ThemMoi
        {
            get
            {
                return _mXuatHuy_Thuoc_ThemMoi;
            }
            set
            {
                if (_mXuatHuy_Thuoc_ThemMoi == value)
                    return;
                _mXuatHuy_Thuoc_ThemMoi = value;
                NotifyOfPropertyChange(() => mXuatHuy_Thuoc_ThemMoi);
            }
        }

        public bool mXuatHuy_Thuoc_XuatExcel
        {
            get
            {
                return _mXuatHuy_Thuoc_XuatExcel;
            }
            set
            {
                if (_mXuatHuy_Thuoc_XuatExcel == value)
                    return;
                _mXuatHuy_Thuoc_XuatExcel = value;
                NotifyOfPropertyChange(() => mXuatHuy_Thuoc_XuatExcel);
            }
        }

        public bool mXuatHuy_Thuoc_XemIn
        {
            get
            {
                return _mXuatHuy_Thuoc_XemIn;
            }
            set
            {
                if (_mXuatHuy_Thuoc_XemIn == value)
                    return;
                _mXuatHuy_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mXuatHuy_Thuoc_XemIn);
            }
        }


        public bool mXuatHuy_YCu_Tim
        {
            get
            {
                return _mXuatHuy_YCu_Tim;
            }
            set
            {
                if (_mXuatHuy_YCu_Tim == value)
                    return;
                _mXuatHuy_YCu_Tim = value;
                NotifyOfPropertyChange(() => mXuatHuy_YCu_Tim);
            }
        }


        public bool mXuatHuy_YCu_ThemMoi
        {
            get
            {
                return _mXuatHuy_YCu_ThemMoi;
            }
            set
            {
                if (_mXuatHuy_YCu_ThemMoi == value)
                    return;
                _mXuatHuy_YCu_ThemMoi = value;
                NotifyOfPropertyChange(() => mXuatHuy_YCu_ThemMoi);
            }
        }


        public bool mXuatHuy_YCu_XuatExcel
        {
            get
            {
                return _mXuatHuy_YCu_XuatExcel;
            }
            set
            {
                if (_mXuatHuy_YCu_XuatExcel == value)
                    return;
                _mXuatHuy_YCu_XuatExcel = value;
                NotifyOfPropertyChange(() => mXuatHuy_YCu_XuatExcel);
            }
        }


        public bool mXuatHuy_YCu_XemIn
        {
            get
            {
                return _mXuatHuy_YCu_XemIn;
            }
            set
            {
                if (_mXuatHuy_YCu_XemIn == value)
                    return;
                _mXuatHuy_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mXuatHuy_YCu_XemIn);
            }
        }


        public bool mXuatHuy_HoaChat_Tim
        {
            get
            {
                return _mXuatHuy_HoaChat_Tim;
            }
            set
            {
                if (_mXuatHuy_HoaChat_Tim == value)
                    return;
                _mXuatHuy_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mXuatHuy_HoaChat_Tim);
            }
        }


        public bool mXuatHuy_HoaChat_ThemMoi
        {
            get
            {
                return _mXuatHuy_HoaChat_ThemMoi;
            }
            set
            {
                if (_mXuatHuy_HoaChat_ThemMoi == value)
                    return;
                _mXuatHuy_HoaChat_ThemMoi = value;
                NotifyOfPropertyChange(() => mXuatHuy_HoaChat_ThemMoi);
            }
        }


        public bool mXuatHuy_HoaChat_XuatExcel
        {
            get
            {
                return _mXuatHuy_HoaChat_XuatExcel;
            }
            set
            {
                if (_mXuatHuy_HoaChat_XuatExcel == value)
                    return;
                _mXuatHuy_HoaChat_XuatExcel = value;
                NotifyOfPropertyChange(() => mXuatHuy_HoaChat_XuatExcel);
            }
        }


        public bool mXuatHuy_HoaChat_XemIn
        {
            get
            {
                return _mXuatHuy_HoaChat_XemIn;
            }
            set
            {
                if (_mXuatHuy_HoaChat_XemIn == value)
                    return;
                _mXuatHuy_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mXuatHuy_HoaChat_XemIn);
            }
        }


        public bool mDuTru_Thuoc_Tim
        {
            get
            {
                return _mDuTru_Thuoc_Tim;
            }
            set
            {
                if (_mDuTru_Thuoc_Tim == value)
                    return;
                _mDuTru_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mDuTru_Thuoc_Tim);
            }
        }


        public bool mDuTru_Thuoc_ThemMoi
        {
            get
            {
                return _mDuTru_Thuoc_ThemMoi;
            }
            set
            {
                if (_mDuTru_Thuoc_ThemMoi == value)
                    return;
                _mDuTru_Thuoc_ThemMoi = value;
                NotifyOfPropertyChange(() => mDuTru_Thuoc_ThemMoi);
            }
        }


        public bool mDuTru_Thuoc_Xoa
        {
            get
            {
                return _mDuTru_Thuoc_Xoa;
            }
            set
            {
                if (_mDuTru_Thuoc_Xoa == value)
                    return;
                _mDuTru_Thuoc_Xoa = value;
                NotifyOfPropertyChange(() => mDuTru_Thuoc_Xoa);
            }
        }


        public bool mDuTru_Thuoc_XemIn
        {
            get
            {
                return _mDuTru_Thuoc_XemIn;
            }
            set
            {
                if (_mDuTru_Thuoc_XemIn == value)
                    return;
                _mDuTru_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mDuTru_Thuoc_XemIn);
            }
        }






        public bool mDuTru_YCu_Tim
        {
            get
            {
                return _mDuTru_YCu_Tim;
            }
            set
            {
                if (_mDuTru_YCu_Tim == value)
                    return;
                _mDuTru_YCu_Tim = value;
                NotifyOfPropertyChange(() => mDuTru_YCu_Tim);
            }
        }


        public bool mDuTru_YCu_ThemMoi
        {
            get
            {
                return _mDuTru_YCu_ThemMoi;
            }
            set
            {
                if (_mDuTru_YCu_ThemMoi == value)
                    return;
                _mDuTru_YCu_ThemMoi = value;
                NotifyOfPropertyChange(() => mDuTru_YCu_ThemMoi);
            }
        }


        public bool mDuTru_YCu_Xoa
        {
            get
            {
                return _mDuTru_YCu_Xoa;
            }
            set
            {
                if (_mDuTru_YCu_Xoa == value)
                    return;
                _mDuTru_YCu_Xoa = value;
                NotifyOfPropertyChange(() => mDuTru_YCu_Xoa);
            }
        }


        public bool mDuTru_YCu_XemIn
        {
            get
            {
                return _mDuTru_YCu_XemIn;
            }
            set
            {
                if (_mDuTru_YCu_XemIn == value)
                    return;
                _mDuTru_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mDuTru_YCu_XemIn);
            }
        }




        public bool mDuTru_HoaChat_Tim
        {
            get
            {
                return _mDuTru_HoaChat_Tim;
            }
            set
            {
                if (_mDuTru_HoaChat_Tim == value)
                    return;
                _mDuTru_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mDuTru_HoaChat_Tim);
            }
        }


        public bool mDuTru_HoaChat_ThemMoi
        {
            get
            {
                return _mDuTru_HoaChat_ThemMoi;
            }
            set
            {
                if (_mDuTru_HoaChat_ThemMoi == value)
                    return;
                _mDuTru_HoaChat_ThemMoi = value;
                NotifyOfPropertyChange(() => mDuTru_HoaChat_ThemMoi);
            }
        }


        public bool mDuTru_HoaChat_Xoa
        {
            get
            {
                return _mDuTru_HoaChat_Xoa;
            }
            set
            {
                if (_mDuTru_HoaChat_Xoa == value)
                    return;
                _mDuTru_HoaChat_Xoa = value;
                NotifyOfPropertyChange(() => mDuTru_HoaChat_Xoa);
            }
        }


        public bool mDuTru_HoaChat_XemIn
        {
            get
            {
                return _mDuTru_HoaChat_XemIn;
            }
            set
            {
                if (_mDuTru_HoaChat_XemIn == value)
                    return;
                _mDuTru_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mDuTru_HoaChat_XemIn);
            }
        }




        public bool mDatHang_Thuoc_DSCanDatHang
        {
            get
            {
                return _mDatHang_Thuoc_DSCanDatHang;
            }
            set
            {
                if (_mDatHang_Thuoc_DSCanDatHang == value)
                    return;
                _mDatHang_Thuoc_DSCanDatHang = value;
                NotifyOfPropertyChange(() => mDatHang_Thuoc_DSCanDatHang);
            }
        }


        public bool mDatHang_Thuoc_Tim
        {
            get
            {
                return _mDatHang_Thuoc_Tim;
            }
            set
            {
                if (_mDatHang_Thuoc_Tim == value)
                    return;
                _mDatHang_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mDatHang_Thuoc_Tim);
            }
        }


        public bool mDatHang_Thuoc_ChinhSua
        {
            get
            {
                return _mDatHang_Thuoc_ChinhSua;
            }
            set
            {
                if (_mDatHang_Thuoc_ChinhSua == value)
                    return;
                _mDatHang_Thuoc_ChinhSua = value;
                NotifyOfPropertyChange(() => mDatHang_Thuoc_ChinhSua);
            }
        }


        public bool mDatHang_Thuoc_ThemMoi
        {
            get
            {
                return _mDatHang_Thuoc_ThemMoi;
            }
            set
            {
                if (_mDatHang_Thuoc_ThemMoi == value)
                    return;
                _mDatHang_Thuoc_ThemMoi = value;
                NotifyOfPropertyChange(() => mDatHang_Thuoc_ThemMoi);
            }
        }


        public bool mDatHang_Thuoc_In
        {
            get
            {
                return _mDatHang_Thuoc_In;
            }
            set
            {
                if (_mDatHang_Thuoc_In == value)
                    return;
                _mDatHang_Thuoc_In = value;
                NotifyOfPropertyChange(() => mDatHang_Thuoc_In);
            }
        }




        public bool mDatHang_YCu_DSCanDatHang
        {
            get
            {
                return _mDatHang_YCu_DSCanDatHang;
            }
            set
            {
                if (_mDatHang_YCu_DSCanDatHang == value)
                    return;
                _mDatHang_YCu_DSCanDatHang = value;
                NotifyOfPropertyChange(() => mDatHang_YCu_DSCanDatHang);
            }
        }


        public bool mDatHang_YCu_Tim
        {
            get
            {
                return _mDatHang_YCu_Tim;
            }
            set
            {
                if (_mDatHang_YCu_Tim == value)
                    return;
                _mDatHang_YCu_Tim = value;
                NotifyOfPropertyChange(() => mDatHang_YCu_Tim);
            }
        }


        public bool mDatHang_YCu_ChinhSua
        {
            get
            {
                return _mDatHang_YCu_ChinhSua;
            }
            set
            {
                if (_mDatHang_YCu_ChinhSua == value)
                    return;
                _mDatHang_YCu_ChinhSua = value;
                NotifyOfPropertyChange(() => mDatHang_YCu_ChinhSua);
            }
        }


        public bool mDatHang_YCu_ThemMoi
        {
            get
            {
                return _mDatHang_YCu_ThemMoi;
            }
            set
            {
                if (_mDatHang_YCu_ThemMoi == value)
                    return;
                _mDatHang_YCu_ThemMoi = value;
                NotifyOfPropertyChange(() => mDatHang_YCu_ThemMoi);
            }
        }


        public bool mDatHang_YCu_In
        {
            get
            {
                return _mDatHang_YCu_In;
            }
            set
            {
                if (_mDatHang_YCu_In == value)
                    return;
                _mDatHang_YCu_In = value;
                NotifyOfPropertyChange(() => mDatHang_YCu_In);
            }
        }




        public bool mDatHang_HoaChat_DSCanDatHang
        {
            get
            {
                return _mDatHang_HoaChat_DSCanDatHang;
            }
            set
            {
                if (_mDatHang_HoaChat_DSCanDatHang == value)
                    return;
                _mDatHang_HoaChat_DSCanDatHang = value;
                NotifyOfPropertyChange(() => mDatHang_HoaChat_DSCanDatHang);
            }
        }


        public bool mDatHang_HoaChat_Tim
        {
            get
            {
                return _mDatHang_HoaChat_Tim;
            }
            set
            {
                if (_mDatHang_HoaChat_Tim == value)
                    return;
                _mDatHang_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mDatHang_HoaChat_Tim);
            }
        }


        public bool mDatHang_HoaChat_ChinhSua
        {
            get
            {
                return _mDatHang_HoaChat_ChinhSua;
            }
            set
            {
                if (_mDatHang_HoaChat_ChinhSua == value)
                    return;
                _mDatHang_HoaChat_ChinhSua = value;
                NotifyOfPropertyChange(() => mDatHang_HoaChat_ChinhSua);
            }
        }


        public bool mDatHang_HoaChat_ThemMoi
        {
            get
            {
                return _mDatHang_HoaChat_ThemMoi;
            }
            set
            {
                if (_mDatHang_HoaChat_ThemMoi == value)
                    return;
                _mDatHang_HoaChat_ThemMoi = value;
                NotifyOfPropertyChange(() => mDatHang_HoaChat_ThemMoi);
            }
        }


        public bool mDatHang_HoaChat_In
        {
            get
            {
                return _mDatHang_HoaChat_In;
            }
            set
            {
                if (_mDatHang_HoaChat_In == value)
                    return;
                _mDatHang_HoaChat_In = value;
                NotifyOfPropertyChange(() => mDatHang_HoaChat_In);
            }
        }




        public bool mNhapHangNCC_Thuoc_Tim
        {
            get
            {
                return _mNhapHangNCC_Thuoc_Tim;
            }
            set
            {
                if (_mNhapHangNCC_Thuoc_Tim == value)
                    return;
                _mNhapHangNCC_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_Thuoc_Tim);
            }
        }


        public bool mNhapHangNCC_Thuoc_ThemMoi
        {
            get
            {
                return _mNhapHangNCC_Thuoc_ThemMoi;
            }
            set
            {
                if (_mNhapHangNCC_Thuoc_ThemMoi == value)
                    return;
                _mNhapHangNCC_Thuoc_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_Thuoc_ThemMoi);
            }
        }


        public bool mNhapHangNCC_Thuoc_CapNhat
        {
            get
            {
                return _mNhapHangNCC_Thuoc_CapNhat;
            }
            set
            {
                if (_mNhapHangNCC_Thuoc_CapNhat == value)
                    return;
                _mNhapHangNCC_Thuoc_CapNhat = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_Thuoc_CapNhat);
            }
        }


        public bool mNhapHangNCC_Thuoc_In
        {
            get
            {
                return _mNhapHangNCC_Thuoc_In;
            }
            set
            {
                if (_mNhapHangNCC_Thuoc_In == value)
                    return;
                _mNhapHangNCC_Thuoc_In = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_Thuoc_In);
            }
        }




        public bool mNhapHangNCC_YCu_Tim
        {
            get
            {
                return _mNhapHangNCC_YCu_Tim;
            }
            set
            {
                if (_mNhapHangNCC_YCu_Tim == value)
                    return;
                _mNhapHangNCC_YCu_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_YCu_Tim);
            }
        }


        public bool mNhapHangNCC_YCu_ThemMoi
        {
            get
            {
                return _mNhapHangNCC_YCu_ThemMoi;
            }
            set
            {
                if (_mNhapHangNCC_YCu_ThemMoi == value)
                    return;
                _mNhapHangNCC_YCu_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_YCu_ThemMoi);
            }
        }


        public bool mNhapHangNCC_YCu_CapNhat
        {
            get
            {
                return _mNhapHangNCC_YCu_CapNhat;
            }
            set
            {
                if (_mNhapHangNCC_YCu_CapNhat == value)
                    return;
                _mNhapHangNCC_YCu_CapNhat = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_YCu_CapNhat);
            }
        }


        public bool mNhapHangNCC_YCu_In
        {
            get
            {
                return _mNhapHangNCC_YCu_In;
            }
            set
            {
                if (_mNhapHangNCC_YCu_In == value)
                    return;
                _mNhapHangNCC_YCu_In = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_YCu_In);
            }
        }


        public bool mNhapHangNCC_HoaChat_Tim
        {
            get
            {
                return _mNhapHangNCC_HoaChat_Tim;
            }
            set
            {
                if (_mNhapHangNCC_HoaChat_Tim == value)
                    return;
                _mNhapHangNCC_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_HoaChat_Tim);
            }
        }


        public bool mNhapHangNCC_HoaChat_ThemMoi
        {
            get
            {
                return _mNhapHangNCC_HoaChat_ThemMoi;
            }
            set
            {
                if (_mNhapHangNCC_HoaChat_ThemMoi == value)
                    return;
                _mNhapHangNCC_HoaChat_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_HoaChat_ThemMoi);
            }
        }


        public bool mNhapHangNCC_HoaChat_CapNhat
        {
            get
            {
                return _mNhapHangNCC_HoaChat_CapNhat;
            }
            set
            {
                if (_mNhapHangNCC_HoaChat_CapNhat == value)
                    return;
                _mNhapHangNCC_HoaChat_CapNhat = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_HoaChat_CapNhat);
            }
        }


        public bool mNhapHangNCC_HoaChat_In
        {
            get
            {
                return _mNhapHangNCC_HoaChat_In;
            }
            set
            {
                if (_mNhapHangNCC_HoaChat_In == value)
                    return;
                _mNhapHangNCC_HoaChat_In = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_HoaChat_In);
            }
        }




        public bool mNhapPhanBoPhi_Thuoc_Tim
        {
            get
            {
                return _mNhapPhanBoPhi_Thuoc_Tim;
            }
            set
            {
                if (_mNhapPhanBoPhi_Thuoc_Tim == value)
                    return;
                _mNhapPhanBoPhi_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mNhapPhanBoPhi_Thuoc_Tim);
            }
        }


        public bool mNhapPhanBoPhi_Thuoc_ChinhSua_Them
        {
            get
            {
                return _mNhapPhanBoPhi_Thuoc_ChinhSua_Them;
            }
            set
            {
                if (_mNhapPhanBoPhi_Thuoc_ChinhSua_Them == value)
                    return;
                _mNhapPhanBoPhi_Thuoc_ChinhSua_Them = value;
                NotifyOfPropertyChange(() => mNhapPhanBoPhi_Thuoc_ChinhSua_Them);
            }
        }


        public bool mNhapPhanBoPhi_Thuoc_In
        {
            get
            {
                return _mNhapPhanBoPhi_Thuoc_In;
            }
            set
            {
                if (_mNhapPhanBoPhi_Thuoc_In == value)
                    return;
                _mNhapPhanBoPhi_Thuoc_In = value;
                NotifyOfPropertyChange(() => mNhapPhanBoPhi_Thuoc_In);
            }
        }




        public bool mNhapPhanBoPhi_YCu_Tim
        {
            get
            {
                return _mNhapPhanBoPhi_YCu_Tim;
            }
            set
            {
                if (_mNhapPhanBoPhi_YCu_Tim == value)
                    return;
                _mNhapPhanBoPhi_YCu_Tim = value;
                NotifyOfPropertyChange(() => mNhapPhanBoPhi_YCu_Tim);
            }
        }


        public bool mNhapPhanBoPhi_YCu_ChinhSua_Them
        {
            get
            {
                return _mNhapPhanBoPhi_YCu_ChinhSua_Them;
            }
            set
            {
                if (_mNhapPhanBoPhi_YCu_ChinhSua_Them == value)
                    return;
                _mNhapPhanBoPhi_YCu_ChinhSua_Them = value;
                NotifyOfPropertyChange(() => mNhapPhanBoPhi_YCu_ChinhSua_Them);
            }
        }


        public bool mNhapPhanBoPhi_YCu_In
        {
            get
            {
                return _mNhapPhanBoPhi_YCu_In;
            }
            set
            {
                if (_mNhapPhanBoPhi_YCu_In == value)
                    return;
                _mNhapPhanBoPhi_YCu_In = value;
                NotifyOfPropertyChange(() => mNhapPhanBoPhi_YCu_In);
            }
        }






        public bool mNhapPhanBoPhi_HoaChat_Tim
        {
            get
            {
                return _mNhapPhanBoPhi_HoaChat_Tim;
            }
            set
            {
                if (_mNhapPhanBoPhi_HoaChat_Tim == value)
                    return;
                _mNhapPhanBoPhi_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mNhapPhanBoPhi_HoaChat_Tim);
            }
        }


        public bool mNhapPhanBoPhi_HoaChat_ChinhSua_Them
        {
            get
            {
                return _mNhapPhanBoPhi_HoaChat_ChinhSua_Them;
            }
            set
            {
                if (_mNhapPhanBoPhi_HoaChat_ChinhSua_Them == value)
                    return;
                _mNhapPhanBoPhi_HoaChat_ChinhSua_Them = value;
                NotifyOfPropertyChange(() => mNhapPhanBoPhi_HoaChat_ChinhSua_Them);
            }
        }


        public bool mNhapPhanBoPhi_HoaChat_In
        {
            get
            {
                return _mNhapPhanBoPhi_HoaChat_In;
            }
            set
            {
                if (_mNhapPhanBoPhi_HoaChat_In == value)
                    return;
                _mNhapPhanBoPhi_HoaChat_In = value;
                NotifyOfPropertyChange(() => mNhapPhanBoPhi_HoaChat_In);
            }
        }






        public bool mPhieuDNThanhToan_Thuoc_Tim
        {
            get
            {
                return _mPhieuDNThanhToan_Thuoc_Tim;
            }
            set
            {
                if (_mPhieuDNThanhToan_Thuoc_Tim == value)
                    return;
                _mPhieuDNThanhToan_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_Thuoc_Tim);
            }
        }


        public bool mPhieuDNThanhToan_Thuoc_PhieuMoi
        {
            get
            {
                return _mPhieuDNThanhToan_Thuoc_PhieuMoi;
            }
            set
            {
                if (_mPhieuDNThanhToan_Thuoc_PhieuMoi == value)
                    return;
                _mPhieuDNThanhToan_Thuoc_PhieuMoi = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_Thuoc_PhieuMoi);
            }
        }


        public bool mPhieuDNThanhToan_Thuoc_Xoa
        {
            get
            {
                return _mPhieuDNThanhToan_Thuoc_Xoa;
            }
            set
            {
                if (_mPhieuDNThanhToan_Thuoc_Xoa == value)
                    return;
                _mPhieuDNThanhToan_Thuoc_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_Thuoc_Xoa);
            }
        }

        public bool mPhieuDNThanhToan_Thuoc_XemInBK
        {
            get
            {
                return _mPhieuDNThanhToan_Thuoc_XemInBK;
            }
            set
            {
                if (_mPhieuDNThanhToan_Thuoc_XemInBK == value)
                    return;
                _mPhieuDNThanhToan_Thuoc_XemInBK = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_Thuoc_XemInBK);
            }
        }
        public bool mPhieuDNThanhToan_Thuoc_XemInPDNTT
        {
            get
            {
                return _mPhieuDNThanhToan_Thuoc_XemInPDNTT;
            }
            set
            {
                if (_mPhieuDNThanhToan_Thuoc_XemInPDNTT == value)
                    return;
                _mPhieuDNThanhToan_Thuoc_XemInPDNTT = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_Thuoc_XemInPDNTT);
            }
        }



        public bool mPhieuDNThanhToan_YCu_Tim
        {
            get
            {
                return _mPhieuDNThanhToan_YCu_Tim;
            }
            set
            {
                if (_mPhieuDNThanhToan_YCu_Tim == value)
                    return;
                _mPhieuDNThanhToan_YCu_Tim = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_YCu_Tim);
            }
        }


        public bool mPhieuDNThanhToan_YCu_PhieuMoi
        {
            get
            {
                return _mPhieuDNThanhToan_YCu_PhieuMoi;
            }
            set
            {
                if (_mPhieuDNThanhToan_YCu_PhieuMoi == value)
                    return;
                _mPhieuDNThanhToan_YCu_PhieuMoi = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_YCu_PhieuMoi);
            }
        }


        public bool mPhieuDNThanhToan_YCu_Xoa
        {
            get
            {
                return _mPhieuDNThanhToan_YCu_Xoa;
            }
            set
            {
                if (_mPhieuDNThanhToan_YCu_Xoa == value)
                    return;
                _mPhieuDNThanhToan_YCu_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_YCu_Xoa);
            }
        }


        public bool mPhieuDNThanhToan_YCu_XemInBK
        {
            get
            {
                return _mPhieuDNThanhToan_YCu_XemInBK;
            }
            set
            {
                if (_mPhieuDNThanhToan_YCu_XemInBK == value)
                    return;
                _mPhieuDNThanhToan_YCu_XemInBK = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_YCu_XemInBK);
            }
        }
        public bool mPhieuDNThanhToan_YCu_XemInPDNTT
        {
            get
            {
                return _mPhieuDNThanhToan_YCu_XemInPDNTT;
            }
            set
            {
                if (_mPhieuDNThanhToan_YCu_XemInPDNTT == value)
                    return;
                _mPhieuDNThanhToan_YCu_XemInPDNTT = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_YCu_XemInPDNTT);
            }
        }




        public bool mPhieuDNThanhToan_HoaChat_Tim
        {
            get
            {
                return _mPhieuDNThanhToan_HoaChat_Tim;
            }
            set
            {
                if (_mPhieuDNThanhToan_HoaChat_Tim == value)
                    return;
                _mPhieuDNThanhToan_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_HoaChat_Tim);
            }
        }


        public bool mPhieuDNThanhToan_HoaChat_PhieuMoi
        {
            get
            {
                return _mPhieuDNThanhToan_HoaChat_PhieuMoi;
            }
            set
            {
                if (_mPhieuDNThanhToan_HoaChat_PhieuMoi == value)
                    return;
                _mPhieuDNThanhToan_HoaChat_PhieuMoi = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_HoaChat_PhieuMoi);
            }
        }


        public bool mPhieuDNThanhToan_HoaChat_Xoa
        {
            get
            {
                return _mPhieuDNThanhToan_HoaChat_Xoa;
            }
            set
            {
                if (_mPhieuDNThanhToan_HoaChat_Xoa == value)
                    return;
                _mPhieuDNThanhToan_HoaChat_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_HoaChat_Xoa);
            }
        }

        public bool mPhieuDNThanhToan_HoaChat_XemInBK
        {
            get
            {
                return _mPhieuDNThanhToan_HoaChat_XemInBK;
            }
            set
            {
                if (_mPhieuDNThanhToan_HoaChat_XemInBK == value)
                    return;
                _mPhieuDNThanhToan_HoaChat_XemInBK = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_HoaChat_XemInBK);
            }
        }
        public bool mPhieuDNThanhToan_HoaChat_XemInPDNTT
        {
            get
            {
                return _mPhieuDNThanhToan_HoaChat_XemInPDNTT;
            }
            set
            {
                if (_mPhieuDNThanhToan_HoaChat_XemInPDNTT == value)
                    return;
                _mPhieuDNThanhToan_HoaChat_XemInPDNTT = value;
                NotifyOfPropertyChange(() => mPhieuDNThanhToan_HoaChat_XemInPDNTT);
            }
        }



        public bool mKiemKe_Thuoc_Tim
        {
            get
            {
                return _mKiemKe_Thuoc_Tim;
            }
            set
            {
                if (_mKiemKe_Thuoc_Tim == value)
                    return;
                _mKiemKe_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_Thuoc_Tim);
            }
        }


        public bool mKiemKe_Thuoc_ThemMoi
        {
            get
            {
                return _mKiemKe_Thuoc_ThemMoi;
            }
            set
            {
                if (_mKiemKe_Thuoc_ThemMoi == value)
                    return;
                _mKiemKe_Thuoc_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_Thuoc_ThemMoi);
            }
        }


        public bool mKiemKe_Thuoc_XuatExcel
        {
            get
            {
                return _mKiemKe_Thuoc_XuatExcel;
            }
            set
            {
                if (_mKiemKe_Thuoc_XuatExcel == value)
                    return;
                _mKiemKe_Thuoc_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_Thuoc_XuatExcel);
            }
        }


        public bool mKiemKe_Thuoc_XemIn
        {
            get
            {
                return _mKiemKe_Thuoc_XemIn;
            }
            set
            {
                if (_mKiemKe_Thuoc_XemIn == value)
                    return;
                _mKiemKe_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_Thuoc_XemIn);
            }
        }




        public bool mKiemKe_YCu_Tim
        {
            get
            {
                return _mKiemKe_YCu_Tim;
            }
            set
            {
                if (_mKiemKe_YCu_Tim == value)
                    return;
                _mKiemKe_YCu_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_YCu_Tim);
            }
        }


        public bool mKiemKe_YCu_ThemMoi
        {
            get
            {
                return _mKiemKe_YCu_ThemMoi;
            }
            set
            {
                if (_mKiemKe_YCu_ThemMoi == value)
                    return;
                _mKiemKe_YCu_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_YCu_ThemMoi);
            }
        }


        public bool mKiemKe_YCu_XuatExcel
        {
            get
            {
                return _mKiemKe_YCu_XuatExcel;
            }
            set
            {
                if (_mKiemKe_YCu_XuatExcel == value)
                    return;
                _mKiemKe_YCu_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_YCu_XuatExcel);
            }
        }


        public bool mKiemKe_YCu_XemIn
        {
            get
            {
                return _mKiemKe_YCu_XemIn;
            }
            set
            {
                if (_mKiemKe_YCu_XemIn == value)
                    return;
                _mKiemKe_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_YCu_XemIn);
            }
        }




        public bool mKiemKe_HoaChat_Tim
        {
            get
            {
                return _mKiemKe_HoaChat_Tim;
            }
            set
            {
                if (_mKiemKe_HoaChat_Tim == value)
                    return;
                _mKiemKe_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_HoaChat_Tim);
            }
        }


        public bool mKiemKe_HoaChat_ThemMoi
        {
            get
            {
                return _mKiemKe_HoaChat_ThemMoi;
            }
            set
            {
                if (_mKiemKe_HoaChat_ThemMoi == value)
                    return;
                _mKiemKe_HoaChat_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_HoaChat_ThemMoi);
            }
        }


        public bool mKiemKe_HoaChat_XuatExcel
        {
            get
            {
                return _mKiemKe_HoaChat_XuatExcel;
            }
            set
            {
                if (_mKiemKe_HoaChat_XuatExcel == value)
                    return;
                _mKiemKe_HoaChat_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_HoaChat_XuatExcel);
            }
        }


        public bool mKiemKe_HoaChat_XemIn
        {
            get
            {
                return _mKiemKe_HoaChat_XemIn;
            }
            set
            {
                if (_mKiemKe_HoaChat_XemIn == value)
                    return;
                _mKiemKe_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_HoaChat_XemIn);
            }
        }




        public bool mBaoCaoXuat_Thuoc_XemIn
        {
            get
            {
                return _mBaoCaoXuat_Thuoc_XemIn;
            }
            set
            {
                if (_mBaoCaoXuat_Thuoc_XemIn == value)
                    return;
                _mBaoCaoXuat_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoXuat_Thuoc_XemIn);
            }
        }


        public bool mBaoCaoXuat_YCu_XemIn
        {
            get
            {
                return _mBaoCaoXuat_YCu_XemIn;
            }
            set
            {
                if (_mBaoCaoXuat_YCu_XemIn == value)
                    return;
                _mBaoCaoXuat_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoXuat_YCu_XemIn);
            }
        }


        public bool mBaoCaoXuat_HoaChat_XemIn
        {
            get
            {
                return _mBaoCaoXuat_HoaChat_XemIn;
            }
            set
            {
                if (_mBaoCaoXuat_HoaChat_XemIn == value)
                    return;
                _mBaoCaoXuat_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoXuat_HoaChat_XemIn);
            }
        }




        public bool mBaoCaoXuatNhapTon_Thuoc_XemIn
        {
            get
            {
                return _mBaoCaoXuatNhapTon_Thuoc_XemIn;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_Thuoc_XemIn == value)
                    return;
                _mBaoCaoXuatNhapTon_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_Thuoc_XemIn);
            }
        }


        public bool mBaoCaoXuatNhapTon_Thuoc_KetChuyen
        {
            get
            {
                return _mBaoCaoXuatNhapTon_Thuoc_KetChuyen;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_Thuoc_KetChuyen == value)
                    return;
                _mBaoCaoXuatNhapTon_Thuoc_KetChuyen = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_Thuoc_KetChuyen);
            }
        }


        public bool mBaoCaoXuatNhapTon_YCu_XemIn
        {
            get
            {
                return _mBaoCaoXuatNhapTon_YCu_XemIn;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_YCu_XemIn == value)
                    return;
                _mBaoCaoXuatNhapTon_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_YCu_XemIn);
            }
        }


        public bool mBaoCaoXuatNhapTon_YCu_KetChuyen
        {
            get
            {
                return _mBaoCaoXuatNhapTon_YCu_KetChuyen;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_YCu_KetChuyen == value)
                    return;
                _mBaoCaoXuatNhapTon_YCu_KetChuyen = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_YCu_KetChuyen);
            }
        }


        public bool mBaoCaoXuatNhapTon_HoaChat_XemIn
        {
            get
            {
                return _mBaoCaoXuatNhapTon_HoaChat_XemIn;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_HoaChat_XemIn == value)
                    return;
                _mBaoCaoXuatNhapTon_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_HoaChat_XemIn);
            }
        }


        public bool mBaoCaoXuatNhapTon_HoaChat_KetChuyen
        {
            get
            {
                return _mBaoCaoXuatNhapTon_HoaChat_KetChuyen;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_HoaChat_KetChuyen == value)
                    return;
                _mBaoCaoXuatNhapTon_HoaChat_KetChuyen = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_HoaChat_KetChuyen);
            }
        }




        public bool mBaoCaoTheKho_Thuoc_Xem
        {
            get
            {
                return _mBaoCaoTheKho_Thuoc_Xem;
            }
            set
            {
                if (_mBaoCaoTheKho_Thuoc_Xem == value)
                    return;
                _mBaoCaoTheKho_Thuoc_Xem = value;
                NotifyOfPropertyChange(() => mBaoCaoTheKho_Thuoc_Xem);
            }
        }


        public bool mBaoCaoTheKho_Thuoc_In
        {
            get
            {
                return _mBaoCaoTheKho_Thuoc_In;
            }
            set
            {
                if (_mBaoCaoTheKho_Thuoc_In == value)
                    return;
                _mBaoCaoTheKho_Thuoc_In = value;
                NotifyOfPropertyChange(() => mBaoCaoTheKho_Thuoc_In);
            }
        }




        public bool mBaoCaoTheKho_YCu_Xem
        {
            get
            {
                return _mBaoCaoTheKho_YCu_Xem;
            }
            set
            {
                if (_mBaoCaoTheKho_YCu_Xem == value)
                    return;
                _mBaoCaoTheKho_YCu_Xem = value;
                NotifyOfPropertyChange(() => mBaoCaoTheKho_YCu_Xem);
            }
        }


        public bool mBaoCaoTheKho_YCu_In
        {
            get
            {
                return _mBaoCaoTheKho_YCu_In;
            }
            set
            {
                if (_mBaoCaoTheKho_YCu_In == value)
                    return;
                _mBaoCaoTheKho_YCu_In = value;
                NotifyOfPropertyChange(() => mBaoCaoTheKho_YCu_In);
            }
        }




        public bool mBaoCaoTheKho_HoaChat_Xem
        {
            get
            {
                return _mBaoCaoTheKho_HoaChat_Xem;
            }
            set
            {
                if (_mBaoCaoTheKho_HoaChat_Xem == value)
                    return;
                _mBaoCaoTheKho_HoaChat_Xem = value;
                NotifyOfPropertyChange(() => mBaoCaoTheKho_HoaChat_Xem);
            }
        }


        public bool mBaoCaoTheKho_HoaChat_In
        {
            get
            {
                return _mBaoCaoTheKho_HoaChat_In;
            }
            set
            {
                if (_mBaoCaoTheKho_HoaChat_In == value)
                    return;
                _mBaoCaoTheKho_HoaChat_In = value;
                NotifyOfPropertyChange(() => mBaoCaoTheKho_HoaChat_In);
            }
        }






        public bool mQLNCCNSX_NCC_Tim
        {
            get
            {
                return _mQLNCCNSX_NCC_Tim;
            }
            set
            {
                if (_mQLNCCNSX_NCC_Tim == value)
                    return;
                _mQLNCCNSX_NCC_Tim = value;
                NotifyOfPropertyChange(() => mQLNCCNSX_NCC_Tim);
            }
        }


        public bool mQLNCCNSX_NCC_ThemMoi
        {
            get
            {
                return _mQLNCCNSX_NCC_ThemMoi;
            }
            set
            {
                if (_mQLNCCNSX_NCC_ThemMoi == value)
                    return;
                _mQLNCCNSX_NCC_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLNCCNSX_NCC_ThemMoi);
            }
        }
        public bool mQLNCCNSX_NCC_ChinhSua
        {
            get
            {
                return _mQLNCCNSX_NCC_ChinhSua;
            }
            set
            {
                if (_mQLNCCNSX_NCC_ChinhSua == value)
                    return;
                _mQLNCCNSX_NCC_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLNCCNSX_NCC_ChinhSua);
            }
        }

        public bool mQLNCCNSX_NSX_Tim
        {
            get
            {
                return _mQLNCCNSX_NSX_Tim;
            }
            set
            {
                if (_mQLNCCNSX_NSX_Tim == value)
                    return;
                _mQLNCCNSX_NSX_Tim = value;
                NotifyOfPropertyChange(() => mQLNCCNSX_NSX_Tim);
            }
        }


        public bool mQLNCCNSX_NSX_ThemMoi
        {
            get
            {
                return _mQLNCCNSX_NSX_ThemMoi;
            }
            set
            {
                if (_mQLNCCNSX_NSX_ThemMoi == value)
                    return;
                _mQLNCCNSX_NSX_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLNCCNSX_NSX_ThemMoi);
            }
        }
        public bool mQLNCCNSX_NSX_ChinhSua
        {
            get
            {
                return _mQLNCCNSX_NSX_ChinhSua;
            }
            set
            {
                if (_mQLNCCNSX_NSX_ChinhSua == value)
                    return;
                _mQLNCCNSX_NSX_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLNCCNSX_NSX_ChinhSua);
            }
        }

        public bool mQLNCCNSX_NCCNSX_Tim
        {
            get
            {
                return _mQLNCCNSX_NCCNSX_Tim;
            }
            set
            {
                if (_mQLNCCNSX_NCCNSX_Tim == value)
                    return;
                _mQLNCCNSX_NCCNSX_Tim = value;
                NotifyOfPropertyChange(() => mQLNCCNSX_NCCNSX_Tim);
            }
        }


        public bool mQLNCCNSX_NCCNSX_ThemMoi
        {
            get
            {
                return _mQLNCCNSX_NCCNSX_ThemMoi;
            }
            set
            {
                if (_mQLNCCNSX_NCCNSX_ThemMoi == value)
                    return;
                _mQLNCCNSX_NCCNSX_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLNCCNSX_NCCNSX_ThemMoi);
            }
        }

        public bool mQLNCCNSX_NCCNSX_ChinhSua
        {
            get
            {
                return _mQLNCCNSX_NCCNSX_ChinhSua;
            }
            set
            {
                if (_mQLNCCNSX_NCCNSX_ChinhSua == value)
                    return;
                _mQLNCCNSX_NCCNSX_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLNCCNSX_NCCNSX_ChinhSua);
            }
        }


        public bool mQLDanhMuc_Thuoc_Tim
        {
            get
            {
                return _mQLDanhMuc_Thuoc_Tim;
            }
            set
            {
                if (_mQLDanhMuc_Thuoc_Tim == value)
                    return;
                _mQLDanhMuc_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_Thuoc_Tim);
            }
        }


        public bool mQLDanhMuc_Thuoc_ThemMoi
        {
            get
            {
                return _mQLDanhMuc_Thuoc_ThemMoi;
            }
            set
            {
                if (_mQLDanhMuc_Thuoc_ThemMoi == value)
                    return;
                _mQLDanhMuc_Thuoc_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_Thuoc_ThemMoi);
            }
        }

        public bool mQLDanhMuc_Thuoc_ChinhSua
        {
            get
            {
                return _mQLDanhMuc_Thuoc_ChinhSua;
            }
            set
            {
                if (_mQLDanhMuc_Thuoc_ChinhSua == value)
                    return;
                _mQLDanhMuc_Thuoc_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_Thuoc_ChinhSua);
            }
        }

        public bool mQLDanhMuc_YCu_Tim
        {
            get
            {
                return _mQLDanhMuc_YCu_Tim;
            }
            set
            {
                if (_mQLDanhMuc_YCu_Tim == value)
                    return;
                _mQLDanhMuc_YCu_Tim = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_YCu_Tim);
            }
        }

        public bool mQLDanhMuc_Nutrition_Tim
        {
            get
            {
                return _mQLDanhMuc_Nutrition_Tim;
            }
            set
            {
                if (_mQLDanhMuc_Nutrition_Tim == value)
                    return;
                _mQLDanhMuc_Nutrition_Tim = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_Nutrition_Tim);
            }
        }

        public bool mQLDanhMuc_Nutrition_ThemMoi
        {
            get
            {
                return _mQLDanhMuc_Nutrition_ThemMoi;
            }
            set
            {
                if (_mQLDanhMuc_Nutrition_ThemMoi == value)
                    return;
                _mQLDanhMuc_Nutrition_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_Nutrition_ThemMoi);
            }
        }

        public bool mQLDanhMuc_Nutrition_ChinhSua
        {
            get
            {
                return _mQLDanhMuc_Nutrition_ChinhSua;
            }
            set
            {
                if (_mQLDanhMuc_Nutrition_ChinhSua == value)
                    return;
                _mQLDanhMuc_Nutrition_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_Nutrition_ChinhSua);
            }
        }

        public bool mQLDanhMuc_YCu_ThemMoi
        {
            get
            {
                return _mQLDanhMuc_YCu_ThemMoi;
            }
            set
            {
                if (_mQLDanhMuc_YCu_ThemMoi == value)
                    return;
                _mQLDanhMuc_YCu_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_YCu_ThemMoi);
            }
        }

        public bool mQLDanhMuc_YCu_ChinhSua
        {
            get
            {
                return _mQLDanhMuc_YCu_ChinhSua;
            }
            set
            {
                if (_mQLDanhMuc_YCu_ChinhSua == value)
                    return;
                _mQLDanhMuc_YCu_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_YCu_ChinhSua);
            }
        }

        public bool mQLDanhMuc_HoaChat_Tim
        {
            get
            {
                return _mQLDanhMuc_HoaChat_Tim;
            }
            set
            {
                if (_mQLDanhMuc_HoaChat_Tim == value)
                    return;
                _mQLDanhMuc_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_HoaChat_Tim);
            }
        }


        public bool mQLDanhMuc_HoaChat_ThemMoi
        {
            get
            {
                return _mQLDanhMuc_HoaChat_ThemMoi;
            }
            set
            {
                if (_mQLDanhMuc_HoaChat_ThemMoi == value)
                    return;
                _mQLDanhMuc_HoaChat_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_HoaChat_ThemMoi);
            }
        }

        public bool mQLDanhMuc_HoaChat_ChinhSua
        {
            get
            {
                return _mQLDanhMuc_HoaChat_ChinhSua;
            }
            set
            {
                if (_mQLDanhMuc_HoaChat_ChinhSua == value)
                    return;
                _mQLDanhMuc_HoaChat_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLDanhMuc_HoaChat_ChinhSua);
            }
        }

        public bool mQLNhomHang_Thuoc_Tim
        {
            get
            {
                return _mQLNhomHang_Thuoc_Tim;
            }
            set
            {
                if (_mQLNhomHang_Thuoc_Tim == value)
                    return;
                _mQLNhomHang_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mQLNhomHang_Thuoc_Tim);
            }
        }

        public bool mQLNhomHang_Thuoc_ThemMoi
        {
            get
            {
                return _mQLNhomHang_Thuoc_ThemMoi;
            }
            set
            {
                if (_mQLNhomHang_Thuoc_ThemMoi == value)
                    return;
                _mQLNhomHang_Thuoc_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLNhomHang_Thuoc_ThemMoi);
            }
        }

        public bool mQLNhomHang_Thuoc_ChinhSua
        {
            get
            {
                return _mQLNhomHang_Thuoc_ChinhSua;
            }
            set
            {
                if (_mQLNhomHang_Thuoc_ChinhSua == value)
                    return;
                _mQLNhomHang_Thuoc_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLNhomHang_Thuoc_ChinhSua);
            }
        }

        public bool mQLNhomHang_YCu_Tim
        {
            get
            {
                return _mQLNhomHang_YCu_Tim;
            }
            set
            {
                if (_mQLNhomHang_YCu_Tim == value)
                    return;
                _mQLNhomHang_YCu_Tim = value;
                NotifyOfPropertyChange(() => mQLNhomHang_YCu_Tim);
            }
        }

        public bool mQLNhomHang_YCu_ThemMoi
        {
            get
            {
                return _mQLNhomHang_YCu_ThemMoi;
            }
            set
            {
                if (_mQLNhomHang_YCu_ThemMoi == value)
                    return;
                _mQLNhomHang_YCu_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLNhomHang_YCu_ThemMoi);
            }
        }

        public bool mQLNhomHang_YCu_ChinhSua
        {
            get
            {
                return _mQLNhomHang_YCu_ChinhSua;
            }
            set
            {
                if (_mQLNhomHang_YCu_ChinhSua == value)
                    return;
                _mQLNhomHang_YCu_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLNhomHang_YCu_ChinhSua);
            }
        }

        public bool mQLNhomHang_HoaChat_Tim
        {
            get
            {
                return _mQLNhomHang_HoaChat_Tim;
            }
            set
            {
                if (_mQLNhomHang_HoaChat_Tim == value)
                    return;
                _mQLNhomHang_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mQLNhomHang_HoaChat_Tim);
            }
        }

        public bool mQLNhomHang_HoaChat_ThemMoi
        {
            get
            {
                return _mQLNhomHang_HoaChat_ThemMoi;
            }
            set
            {
                if (_mQLNhomHang_HoaChat_ThemMoi == value)
                    return;
                _mQLNhomHang_HoaChat_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLNhomHang_HoaChat_ThemMoi);
            }
        }

        public bool mQLNhomHang_HoaChat_ChinhSua
        {
            get
            {
                return _mQLNhomHang_HoaChat_ChinhSua;
            }
            set
            {
                if (_mQLNhomHang_HoaChat_ChinhSua == value)
                    return;
                _mQLNhomHang_HoaChat_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLNhomHang_HoaChat_ChinhSua);
            }
        }

        /*▼====: #005*/
        public bool mQLNhomHang_HoatChat_Tim
        {
            get
            {
                return _mQLNhomHang_HoatChat_Tim;
            }
            set
            {
                if (_mQLNhomHang_HoatChat_Tim == value)
                    return;
                _mQLNhomHang_HoatChat_Tim = value;
                NotifyOfPropertyChange(() => mQLNhomHang_HoatChat_Tim);
            }
        }

        public bool mQLNhomHang_HoatChat_ThemMoi
        {
            get
            {
                return _mQLNhomHang_HoatChat_ThemMoi;
            }
            set
            {
                if (_mQLNhomHang_HoatChat_ThemMoi == value)
                    return;
                _mQLNhomHang_HoatChat_ThemMoi = value;
                NotifyOfPropertyChange(() => mQLNhomHang_HoatChat_ThemMoi);
            }
        }

        public bool mQLNhomHang_HoatChat_ChinhSua
        {
            get
            {
                return _mQLNhomHang_HoatChat_ChinhSua;
            }
            set
            {
                if (_mQLNhomHang_HoatChat_ChinhSua == value)
                    return;
                _mQLNhomHang_HoatChat_ChinhSua = value;
                NotifyOfPropertyChange(() => mQLNhomHang_HoatChat_ChinhSua);
            }
        }
        /*▲====: #005*/


        public bool mGiaTuNCC_Thuoc_Tim
        {
            get
            {
                return _mGiaTuNCC_Thuoc_Tim;
            }
            set
            {
                if (_mGiaTuNCC_Thuoc_Tim == value)
                    return;
                _mGiaTuNCC_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_Thuoc_Tim);
            }
        }


        public bool mGiaTuNCC_Thuoc_QuanLy
        {
            get
            {
                return _mGiaTuNCC_Thuoc_QuanLy;
            }
            set
            {
                if (_mGiaTuNCC_Thuoc_QuanLy == value)
                    return;
                _mGiaTuNCC_Thuoc_QuanLy = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_Thuoc_QuanLy);
            }
        }


        public bool mGiaTuNCC_Thuoc_TaoGiaMoi
        {
            get
            {
                return _mGiaTuNCC_Thuoc_TaoGiaMoi;
            }
            set
            {
                if (_mGiaTuNCC_Thuoc_TaoGiaMoi == value)
                    return;
                _mGiaTuNCC_Thuoc_TaoGiaMoi = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_Thuoc_TaoGiaMoi);
            }
        }


        public bool mGiaTuNCC_Thuoc_SuaGia
        {
            get
            {
                return _mGiaTuNCC_Thuoc_SuaGia;
            }
            set
            {
                if (_mGiaTuNCC_Thuoc_SuaGia == value)
                    return;
                _mGiaTuNCC_Thuoc_SuaGia = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_Thuoc_SuaGia);
            }
        }




        public bool mGiaTuNCC_YCu_Tim
        {
            get
            {
                return _mGiaTuNCC_YCu_Tim;
            }
            set
            {
                if (_mGiaTuNCC_YCu_Tim == value)
                    return;
                _mGiaTuNCC_YCu_Tim = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_YCu_Tim);
            }
        }


        public bool mGiaTuNCC_YCu_QuanLy
        {
            get
            {
                return _mGiaTuNCC_YCu_QuanLy;
            }
            set
            {
                if (_mGiaTuNCC_YCu_QuanLy == value)
                    return;
                _mGiaTuNCC_YCu_QuanLy = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_YCu_QuanLy);
            }
        }


        public bool mGiaTuNCC_YCu_TaoGiaMoi
        {
            get
            {
                return _mGiaTuNCC_YCu_TaoGiaMoi;
            }
            set
            {
                if (_mGiaTuNCC_YCu_TaoGiaMoi == value)
                    return;
                _mGiaTuNCC_YCu_TaoGiaMoi = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_YCu_TaoGiaMoi);
            }
        }


        public bool mGiaTuNCC_YCu_SuaGia
        {
            get
            {
                return _mGiaTuNCC_YCu_SuaGia;
            }
            set
            {
                if (_mGiaTuNCC_YCu_SuaGia == value)
                    return;
                _mGiaTuNCC_YCu_SuaGia = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_YCu_SuaGia);
            }
        }




        public bool mGiaTuNCC_HoaChat_Tim
        {
            get
            {
                return _mGiaTuNCC_HoaChat_Tim;
            }
            set
            {
                if (_mGiaTuNCC_HoaChat_Tim == value)
                    return;
                _mGiaTuNCC_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_HoaChat_Tim);
            }
        }


        public bool mGiaTuNCC_HoaChat_QuanLy
        {
            get
            {
                return _mGiaTuNCC_HoaChat_QuanLy;
            }
            set
            {
                if (_mGiaTuNCC_HoaChat_QuanLy == value)
                    return;
                _mGiaTuNCC_HoaChat_QuanLy = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_HoaChat_QuanLy);
            }
        }


        public bool mGiaTuNCC_HoaChat_TaoGiaMoi
        {
            get
            {
                return _mGiaTuNCC_HoaChat_TaoGiaMoi;
            }
            set
            {
                if (_mGiaTuNCC_HoaChat_TaoGiaMoi == value)
                    return;
                _mGiaTuNCC_HoaChat_TaoGiaMoi = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_HoaChat_TaoGiaMoi);
            }
        }


        public bool mGiaTuNCC_HoaChat_SuaGia
        {
            get
            {
                return _mGiaTuNCC_HoaChat_SuaGia;
            }
            set
            {
                if (_mGiaTuNCC_HoaChat_SuaGia == value)
                    return;
                _mGiaTuNCC_HoaChat_SuaGia = value;
                NotifyOfPropertyChange(() => mGiaTuNCC_HoaChat_SuaGia);
            }
        }







        public bool mThangGiaBan_Thuoc_Tim
        {
            get
            {
                return _mThangGiaBan_Thuoc_Tim;
            }
            set
            {
                if (_mThangGiaBan_Thuoc_Tim == value)
                    return;
                _mThangGiaBan_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mThangGiaBan_Thuoc_Tim);
            }
        }


        public bool mThangGiaBan_Thuoc_TaoMoiCTGia
        {
            get
            {
                return _mThangGiaBan_Thuoc_TaoMoiCTGia;
            }
            set
            {
                if (_mThangGiaBan_Thuoc_TaoMoiCTGia == value)
                    return;
                _mThangGiaBan_Thuoc_TaoMoiCTGia = value;
                NotifyOfPropertyChange(() => mThangGiaBan_Thuoc_TaoMoiCTGia);
            }
        }


        public bool mThangGiaBan_Thuoc_ChinhSua
        {
            get
            {
                return _mThangGiaBan_Thuoc_ChinhSua;
            }
            set
            {
                if (_mThangGiaBan_Thuoc_ChinhSua == value)
                    return;
                _mThangGiaBan_Thuoc_ChinhSua = value;
                NotifyOfPropertyChange(() => mThangGiaBan_Thuoc_ChinhSua);
            }
        }




        public bool mThangGiaBan_YCu_Tim
        {
            get
            {
                return _mThangGiaBan_YCu_Tim;
            }
            set
            {
                if (_mThangGiaBan_YCu_Tim == value)
                    return;
                _mThangGiaBan_YCu_Tim = value;
                NotifyOfPropertyChange(() => mThangGiaBan_YCu_Tim);
            }
        }


        public bool mThangGiaBan_YCu_TaoMoiCTGia
        {
            get
            {
                return _mThangGiaBan_YCu_TaoMoiCTGia;
            }
            set
            {
                if (_mThangGiaBan_YCu_TaoMoiCTGia == value)
                    return;
                _mThangGiaBan_YCu_TaoMoiCTGia = value;
                NotifyOfPropertyChange(() => mThangGiaBan_YCu_TaoMoiCTGia);
            }
        }


        public bool mThangGiaBan_YCu_ChinhSua
        {
            get
            {
                return _mThangGiaBan_YCu_ChinhSua;
            }
            set
            {
                if (_mThangGiaBan_YCu_ChinhSua == value)
                    return;
                _mThangGiaBan_YCu_ChinhSua = value;
                NotifyOfPropertyChange(() => mThangGiaBan_YCu_ChinhSua);
            }
        }




        public bool mThangGiaBan_HoaChat_Tim
        {
            get
            {
                return _mThangGiaBan_HoaChat_Tim;
            }
            set
            {
                if (_mThangGiaBan_HoaChat_Tim == value)
                    return;
                _mThangGiaBan_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mThangGiaBan_HoaChat_Tim);
            }
        }


        public bool mThangGiaBan_HoaChat_TaoMoiCTGia
        {
            get
            {
                return _mThangGiaBan_HoaChat_TaoMoiCTGia;
            }
            set
            {
                if (_mThangGiaBan_HoaChat_TaoMoiCTGia == value)
                    return;
                _mThangGiaBan_HoaChat_TaoMoiCTGia = value;
                NotifyOfPropertyChange(() => mThangGiaBan_HoaChat_TaoMoiCTGia);
            }
        }


        public bool mThangGiaBan_HoaChat_ChinhSua
        {
            get
            {
                return _mThangGiaBan_HoaChat_ChinhSua;
            }
            set
            {
                if (_mThangGiaBan_HoaChat_ChinhSua == value)
                    return;
                _mThangGiaBan_HoaChat_ChinhSua = value;
                NotifyOfPropertyChange(() => mThangGiaBan_HoaChat_ChinhSua);
            }
        }




        public bool mGiaBan_Thuoc_Tim
        {
            get
            {
                return _mGiaBan_Thuoc_Tim;
            }
            set
            {
                if (_mGiaBan_Thuoc_Tim == value)
                    return;
                _mGiaBan_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mGiaBan_Thuoc_Tim);
            }
        }


        public bool mGiaBan_Thuoc_ChinhSua
        {
            get
            {
                return _mGiaBan_Thuoc_ChinhSua;
            }
            set
            {
                if (_mGiaBan_Thuoc_ChinhSua == value)
                    return;
                _mGiaBan_Thuoc_ChinhSua = value;
                NotifyOfPropertyChange(() => mGiaBan_Thuoc_ChinhSua);
            }
        }


        public bool mGiaBan_Thuoc_XemDSGia
        {
            get
            {
                return _mGiaBan_Thuoc_XemDSGia;
            }
            set
            {
                if (_mGiaBan_Thuoc_XemDSGia == value)
                    return;
                _mGiaBan_Thuoc_XemDSGia = value;
                NotifyOfPropertyChange(() => mGiaBan_Thuoc_XemDSGia);
            }
        }


        public bool mGiaBan_Thuoc_TaoGiaMoi
        {
            get
            {
                return _mGiaBan_Thuoc_TaoGiaMoi;
            }
            set
            {
                if (_mGiaBan_Thuoc_TaoGiaMoi == value)
                    return;
                _mGiaBan_Thuoc_TaoGiaMoi = value;
                NotifyOfPropertyChange(() => mGiaBan_Thuoc_TaoGiaMoi);
            }
        }

        public bool mGiaBan_Thuoc_ChinhSuaGia
        {
            get
            {
                return _mGiaBan_Thuoc_ChinhSuaGia;
            }
            set
            {
                if (_mGiaBan_Thuoc_ChinhSuaGia == value)
                    return;
                _mGiaBan_Thuoc_ChinhSuaGia = value;
                NotifyOfPropertyChange(() => mGiaBan_Thuoc_ChinhSuaGia);
            }
        }


        public bool mGiaBan_YCu_Tim
        {
            get
            {
                return _mGiaBan_YCu_Tim;
            }
            set
            {
                if (_mGiaBan_YCu_Tim == value)
                    return;
                _mGiaBan_YCu_Tim = value;
                NotifyOfPropertyChange(() => mGiaBan_YCu_Tim);
            }
        }


        public bool mGiaBan_YCu_ChinhSua
        {
            get
            {
                return _mGiaBan_YCu_ChinhSua;
            }
            set
            {
                if (_mGiaBan_YCu_ChinhSua == value)
                    return;
                _mGiaBan_YCu_ChinhSua = value;
                NotifyOfPropertyChange(() => mGiaBan_YCu_ChinhSua);
            }
        }


        public bool mGiaBan_YCu_XemDSGia
        {
            get
            {
                return _mGiaBan_YCu_XemDSGia;
            }
            set
            {
                if (_mGiaBan_YCu_XemDSGia == value)
                    return;
                _mGiaBan_YCu_XemDSGia = value;
                NotifyOfPropertyChange(() => mGiaBan_YCu_XemDSGia);
            }
        }


        public bool mGiaBan_YCu_TaoGiaMoi
        {
            get
            {
                return _mGiaBan_YCu_TaoGiaMoi;
            }
            set
            {
                if (_mGiaBan_YCu_TaoGiaMoi == value)
                    return;
                _mGiaBan_YCu_TaoGiaMoi = value;
                NotifyOfPropertyChange(() => mGiaBan_YCu_TaoGiaMoi);
            }
        }

        public bool mGiaBan_YCu_ChinhSuaGia
        {
            get
            {
                return _mGiaBan_YCu_ChinhSuaGia;
            }
            set
            {
                if (_mGiaBan_YCu_ChinhSuaGia == value)
                    return;
                _mGiaBan_YCu_ChinhSuaGia = value;
                NotifyOfPropertyChange(() => mGiaBan_YCu_ChinhSuaGia);
            }
        }


        public bool mGiaBan_HoaChat_Tim
        {
            get
            {
                return _mGiaBan_HoaChat_Tim;
            }
            set
            {
                if (_mGiaBan_HoaChat_Tim == value)
                    return;
                _mGiaBan_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mGiaBan_HoaChat_Tim);
            }
        }


        public bool mGiaBan_HoaChat_ChinhSua
        {
            get
            {
                return _mGiaBan_HoaChat_ChinhSua;
            }
            set
            {
                if (_mGiaBan_HoaChat_ChinhSua == value)
                    return;
                _mGiaBan_HoaChat_ChinhSua = value;
                NotifyOfPropertyChange(() => mGiaBan_HoaChat_ChinhSua);
            }
        }


        public bool mGiaBan_HoaChat_XemDSGia
        {
            get
            {
                return _mGiaBan_HoaChat_XemDSGia;
            }
            set
            {
                if (_mGiaBan_HoaChat_XemDSGia == value)
                    return;
                _mGiaBan_HoaChat_XemDSGia = value;
                NotifyOfPropertyChange(() => mGiaBan_HoaChat_XemDSGia);
            }
        }


        public bool mGiaBan_HoaChat_TaoGiaMoi
        {
            get
            {
                return _mGiaBan_HoaChat_TaoGiaMoi;
            }
            set
            {
                if (_mGiaBan_HoaChat_TaoGiaMoi == value)
                    return;
                _mGiaBan_HoaChat_TaoGiaMoi = value;
                NotifyOfPropertyChange(() => mGiaBan_HoaChat_TaoGiaMoi);
            }
        }

        public bool mGiaBan_HoaChat_ChinhSuaGia
        {
            get
            {
                return _mGiaBan_HoaChat_ChinhSuaGia;
            }
            set
            {
                if (_mGiaBan_HoaChat_ChinhSuaGia == value)
                    return;
                _mGiaBan_HoaChat_ChinhSuaGia = value;
                NotifyOfPropertyChange(() => mGiaBan_HoaChat_ChinhSuaGia);
            }
        }



        public bool mBangGiaBan_Thuoc_Xem
        {
            get
            {
                return _mBangGiaBan_Thuoc_Xem;
            }
            set
            {
                if (_mBangGiaBan_Thuoc_Xem == value)
                    return;
                _mBangGiaBan_Thuoc_Xem = value;
                NotifyOfPropertyChange(() => mBangGiaBan_Thuoc_Xem);
            }
        }


        public bool mBangGiaBan_Thuoc_ChinhSua
        {
            get
            {
                return _mBangGiaBan_Thuoc_ChinhSua;
            }
            set
            {
                if (_mBangGiaBan_Thuoc_ChinhSua == value)
                    return;
                _mBangGiaBan_Thuoc_ChinhSua = value;
                NotifyOfPropertyChange(() => mBangGiaBan_Thuoc_ChinhSua);
            }
        }


        public bool mBangGiaBan_Thuoc_TaoBangGia
        {
            get
            {
                return _mBangGiaBan_Thuoc_TaoBangGia;
            }
            set
            {
                if (_mBangGiaBan_Thuoc_TaoBangGia == value)
                    return;
                _mBangGiaBan_Thuoc_TaoBangGia = value;
                NotifyOfPropertyChange(() => mBangGiaBan_Thuoc_TaoBangGia);
            }
        }


        public bool mBangGiaBan_Thuoc_PreView
        {
            get
            {
                return _mBangGiaBan_Thuoc_PreView;
            }
            set
            {
                if (_mBangGiaBan_Thuoc_PreView == value)
                    return;
                _mBangGiaBan_Thuoc_PreView = value;
                NotifyOfPropertyChange(() => mBangGiaBan_Thuoc_PreView);
            }
        }


        public bool mBangGiaBan_Thuoc_In
        {
            get
            {
                return _mBangGiaBan_Thuoc_In;
            }
            set
            {
                if (_mBangGiaBan_Thuoc_In == value)
                    return;
                _mBangGiaBan_Thuoc_In = value;
                NotifyOfPropertyChange(() => mBangGiaBan_Thuoc_In);
            }
        }




        public bool mBangGiaBan_YCu_Xem
        {
            get
            {
                return _mBangGiaBan_YCu_Xem;
            }
            set
            {
                if (_mBangGiaBan_YCu_Xem == value)
                    return;
                _mBangGiaBan_YCu_Xem = value;
                NotifyOfPropertyChange(() => mBangGiaBan_YCu_Xem);
            }
        }


        public bool mBangGiaBan_YCu_ChinhSua
        {
            get
            {
                return _mBangGiaBan_YCu_ChinhSua;
            }
            set
            {
                if (_mBangGiaBan_YCu_ChinhSua == value)
                    return;
                _mBangGiaBan_YCu_ChinhSua = value;
                NotifyOfPropertyChange(() => mBangGiaBan_YCu_ChinhSua);
            }
        }


        public bool mBangGiaBan_YCu_TaoBangGia
        {
            get
            {
                return _mBangGiaBan_YCu_TaoBangGia;
            }
            set
            {
                if (_mBangGiaBan_YCu_TaoBangGia == value)
                    return;
                _mBangGiaBan_YCu_TaoBangGia = value;
                NotifyOfPropertyChange(() => mBangGiaBan_YCu_TaoBangGia);
            }
        }


        public bool mBangGiaBan_YCu_PreView
        {
            get
            {
                return _mBangGiaBan_YCu_PreView;
            }
            set
            {
                if (_mBangGiaBan_YCu_PreView == value)
                    return;
                _mBangGiaBan_YCu_PreView = value;
                NotifyOfPropertyChange(() => mBangGiaBan_YCu_PreView);
            }
        }


        public bool mBangGiaBan_YCu_In
        {
            get
            {
                return _mBangGiaBan_YCu_In;
            }
            set
            {
                if (_mBangGiaBan_YCu_In == value)
                    return;
                _mBangGiaBan_YCu_In = value;
                NotifyOfPropertyChange(() => mBangGiaBan_YCu_In);
            }
        }




        public bool mBangGiaBan_HoaChat_Xem
        {
            get
            {
                return _mBangGiaBan_HoaChat_Xem;
            }
            set
            {
                if (_mBangGiaBan_HoaChat_Xem == value)
                    return;
                _mBangGiaBan_HoaChat_Xem = value;
                NotifyOfPropertyChange(() => mBangGiaBan_HoaChat_Xem);
            }
        }


        public bool mBangGiaBan_HoaChat_ChinhSua
        {
            get
            {
                return _mBangGiaBan_HoaChat_ChinhSua;
            }
            set
            {
                if (_mBangGiaBan_HoaChat_ChinhSua == value)
                    return;
                _mBangGiaBan_HoaChat_ChinhSua = value;
                NotifyOfPropertyChange(() => mBangGiaBan_HoaChat_ChinhSua);
            }
        }


        public bool mBangGiaBan_HoaChat_TaoBangGia
        {
            get
            {
                return _mBangGiaBan_HoaChat_TaoBangGia;
            }
            set
            {
                if (_mBangGiaBan_HoaChat_TaoBangGia == value)
                    return;
                _mBangGiaBan_HoaChat_TaoBangGia = value;
                NotifyOfPropertyChange(() => mBangGiaBan_HoaChat_TaoBangGia);
            }
        }


        public bool mBangGiaBan_HoaChat_PreView
        {
            get
            {
                return _mBangGiaBan_HoaChat_PreView;
            }
            set
            {
                if (_mBangGiaBan_HoaChat_PreView == value)
                    return;
                _mBangGiaBan_HoaChat_PreView = value;
                NotifyOfPropertyChange(() => mBangGiaBan_HoaChat_PreView);
            }
        }


        public bool mBangGiaBan_HoaChat_In
        {
            get
            {
                return _mBangGiaBan_HoaChat_In;
            }
            set
            {
                if (_mBangGiaBan_HoaChat_In == value)
                    return;
                _mBangGiaBan_HoaChat_In = value;
                NotifyOfPropertyChange(() => mBangGiaBan_HoaChat_In);
            }
        }


        //---Them phan moi cua NY
        public bool mPhieuDeNghiThanhToan_Thuoc_Tim
        {
            get
            {
                return _mPhieuDeNghiThanhToan_Thuoc_Tim;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_Thuoc_Tim == value)
                    return;
                _mPhieuDeNghiThanhToan_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_Thuoc_Tim);
            }
        }

        public bool mPhieuDeNghiThanhToan_Thuoc_ThemMoi
        {
            get
            {
                return _mPhieuDeNghiThanhToan_Thuoc_ThemMoi;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_Thuoc_ThemMoi == value)
                    return;
                _mPhieuDeNghiThanhToan_Thuoc_ThemMoi = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_Thuoc_ThemMoi);
            }
        }

        public bool mPhieuDeNghiThanhToan_Thuoc_ChinhSua
        {
            get
            {
                return _mPhieuDeNghiThanhToan_Thuoc_ChinhSua;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_Thuoc_ChinhSua == value)
                    return;
                _mPhieuDeNghiThanhToan_Thuoc_ChinhSua = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_Thuoc_ChinhSua);
            }
        }

        public bool mPhieuDeNghiThanhToan_Thuoc_XemInBK
        {
            get
            {
                return _mPhieuDeNghiThanhToan_Thuoc_XemInBK;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_Thuoc_XemInBK == value)
                    return;
                _mPhieuDeNghiThanhToan_Thuoc_XemInBK = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_Thuoc_XemInBK);
            }
        }

        public bool mPhieuDeNghiThanhToan_Thuoc_XemInPDNTT
        {
            get
            {
                return _mPhieuDeNghiThanhToan_Thuoc_XemInPDNTT;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_Thuoc_XemInPDNTT == value)
                    return;
                _mPhieuDeNghiThanhToan_Thuoc_XemInPDNTT = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_Thuoc_XemInPDNTT);
            }
        }


        public bool mPhieuDeNghiThanhToan_YCu_Tim
        {
            get
            {
                return _mPhieuDeNghiThanhToan_YCu_Tim;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_YCu_Tim == value)
                    return;
                _mPhieuDeNghiThanhToan_YCu_Tim = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_YCu_Tim);
            }
        }

        public bool mPhieuDeNghiThanhToan_YCu_ThemMoi
        {
            get
            {
                return _mPhieuDeNghiThanhToan_YCu_ThemMoi;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_YCu_ThemMoi == value)
                    return;
                _mPhieuDeNghiThanhToan_YCu_ThemMoi = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_YCu_ThemMoi);
            }
        }

        public bool mPhieuDeNghiThanhToan_YCu_ChinhSua
        {
            get
            {
                return _mPhieuDeNghiThanhToan_YCu_ChinhSua;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_YCu_ChinhSua == value)
                    return;
                _mPhieuDeNghiThanhToan_YCu_ChinhSua = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_YCu_ChinhSua);
            }
        }

        public bool mPhieuDeNghiThanhToan_YCu_XemInBK
        {
            get
            {
                return _mPhieuDeNghiThanhToan_YCu_XemInBK;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_YCu_XemInBK == value)
                    return;
                _mPhieuDeNghiThanhToan_YCu_XemInBK = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_YCu_XemInBK);
            }
        }

        public bool mPhieuDeNghiThanhToan_YCu_XemInPDNTT
        {
            get
            {
                return _mPhieuDeNghiThanhToan_YCu_XemInPDNTT;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_YCu_XemInPDNTT == value)
                    return;
                _mPhieuDeNghiThanhToan_YCu_XemInPDNTT = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_YCu_XemInPDNTT);
            }
        }


        public bool mPhieuDeNghiThanhToan_HoaChat_Tim
        {
            get
            {
                return _mPhieuDeNghiThanhToan_HoaChat_Tim;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_HoaChat_Tim == value)
                    return;
                _mPhieuDeNghiThanhToan_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_HoaChat_Tim);
            }
        }

        public bool mPhieuDeNghiThanhToan_HoaChat_ThemMoi
        {
            get
            {
                return _mPhieuDeNghiThanhToan_HoaChat_ThemMoi;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_HoaChat_ThemMoi == value)
                    return;
                _mPhieuDeNghiThanhToan_HoaChat_ThemMoi = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_HoaChat_ThemMoi);
            }
        }

        public bool mPhieuDeNghiThanhToan_HoaChat_ChinhSua
        {
            get
            {
                return _mPhieuDeNghiThanhToan_HoaChat_ChinhSua;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_HoaChat_ChinhSua == value)
                    return;
                _mPhieuDeNghiThanhToan_HoaChat_ChinhSua = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_HoaChat_ChinhSua);
            }
        }

        public bool mPhieuDeNghiThanhToan_HoaChat_XemInBK
        {
            get
            {
                return _mPhieuDeNghiThanhToan_HoaChat_XemInBK;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_HoaChat_XemInBK == value)
                    return;
                _mPhieuDeNghiThanhToan_HoaChat_XemInBK = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_HoaChat_XemInBK);
            }
        }

        public bool mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT
        {
            get
            {
                return _mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT;
            }
            set
            {
                if (_mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT == value)
                    return;
                _mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT = value;
                NotifyOfPropertyChange(() => mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT);
            }
        }


        public bool mBaoCaoDSPhieuNhapKho_Thuoc_XemIn
        {
            get
            {
                return _mBaoCaoDSPhieuNhapKho_Thuoc_XemIn;
            }
            set
            {
                if (_mBaoCaoDSPhieuNhapKho_Thuoc_XemIn == value)
                    return;
                _mBaoCaoDSPhieuNhapKho_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoDSPhieuNhapKho_Thuoc_XemIn);
            }
        }

        public bool mBaoCaoDSPhieuNhapKho_Thuoc_In
        {
            get
            {
                return _mBaoCaoDSPhieuNhapKho_Thuoc_In;
            }
            set
            {
                if (_mBaoCaoDSPhieuNhapKho_Thuoc_In == value)
                    return;
                _mBaoCaoDSPhieuNhapKho_Thuoc_In = value;
                NotifyOfPropertyChange(() => mBaoCaoDSPhieuNhapKho_Thuoc_In);
            }
        }



        public bool mBaoCaoDSPhieuNhapKho_YCu_XemIn
        {
            get
            {
                return _mBaoCaoDSPhieuNhapKho_YCu_XemIn;
            }
            set
            {
                if (_mBaoCaoDSPhieuNhapKho_YCu_XemIn == value)
                    return;
                _mBaoCaoDSPhieuNhapKho_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoDSPhieuNhapKho_YCu_XemIn);
            }
        }

        public bool mBaoCaoDSPhieuNhapKho_YCu_In
        {
            get
            {
                return _mBaoCaoDSPhieuNhapKho_YCu_In;
            }
            set
            {
                if (_mBaoCaoDSPhieuNhapKho_YCu_In == value)
                    return;
                _mBaoCaoDSPhieuNhapKho_YCu_In = value;
                NotifyOfPropertyChange(() => mBaoCaoDSPhieuNhapKho_YCu_In);
            }
        }




        public bool mBaoCaoDSPhieuNhapKho_HoaChat_XemIn
        {
            get
            {
                return _mBaoCaoDSPhieuNhapKho_HoaChat_XemIn;
            }
            set
            {
                if (_mBaoCaoDSPhieuNhapKho_HoaChat_XemIn == value)
                    return;
                _mBaoCaoDSPhieuNhapKho_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoDSPhieuNhapKho_HoaChat_XemIn);
            }
        }


        public bool mBaoCaoDSPhieuNhapKho_HoaChat_In
        {
            get
            {
                return _mBaoCaoDSPhieuNhapKho_HoaChat_In;
            }
            set
            {
                if (_mBaoCaoDSPhieuNhapKho_HoaChat_In == value)
                    return;
                _mBaoCaoDSPhieuNhapKho_HoaChat_In = value;
                NotifyOfPropertyChange(() => mBaoCaoDSPhieuNhapKho_HoaChat_In);
            }
        }


        public bool mBaoCaoSuDung_Thuoc_XemIn
        {
            get
            {
                return _mBaoCaoSuDung_Thuoc_XemIn;
            }
            set
            {
                if (_mBaoCaoSuDung_Thuoc_XemIn == value)
                    return;
                _mBaoCaoSuDung_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoSuDung_Thuoc_XemIn);
            }
        }


        public bool mBaoCaoSuDung_Thuoc_In
        {
            get
            {
                return _mBaoCaoSuDung_Thuoc_In;
            }
            set
            {
                if (_mBaoCaoSuDung_Thuoc_In == value)
                    return;
                _mBaoCaoSuDung_Thuoc_In = value;
                NotifyOfPropertyChange(() => mBaoCaoSuDung_Thuoc_In);
            }
        }




        public bool mBaoCaoNhapXuatDenKhoaPhong_Thuoc_In
        {
            get
            {
                return _mBaoCaoNhapXuatDenKhoaPhong_Thuoc_In;
            }
            set
            {
                if (_mBaoCaoNhapXuatDenKhoaPhong_Thuoc_In == value)
                    return;
                _mBaoCaoNhapXuatDenKhoaPhong_Thuoc_In = value;
                NotifyOfPropertyChange(() => mBaoCaoNhapXuatDenKhoaPhong_Thuoc_In);
            }
        }


        public bool mBaoCaoNhapXuatDenKhoaPhong_Thuoc_XuatExcel
        {
            get
            {
                return _mBaoCaoNhapXuatDenKhoaPhong_Thuoc_XuatExcel;
            }
            set
            {
                if (_mBaoCaoNhapXuatDenKhoaPhong_Thuoc_XuatExcel == value)
                    return;
                _mBaoCaoNhapXuatDenKhoaPhong_Thuoc_XuatExcel = value;
                NotifyOfPropertyChange(() => mBaoCaoNhapXuatDenKhoaPhong_Thuoc_XuatExcel);
            }
        }




        public bool mBaoCaoNhapXuatDenKhoaPhong_YCu_In
        {
            get
            {
                return _mBaoCaoNhapXuatDenKhoaPhong_YCu_In;
            }
            set
            {
                if (_mBaoCaoNhapXuatDenKhoaPhong_YCu_In == value)
                    return;
                _mBaoCaoNhapXuatDenKhoaPhong_YCu_In = value;
                NotifyOfPropertyChange(() => mBaoCaoNhapXuatDenKhoaPhong_YCu_In);
            }
        }


        public bool mBaoCaoNhapXuatDenKhoaPhong_YCu_XuatExcel
        {
            get
            {
                return _mBaoCaoNhapXuatDenKhoaPhong_YCu_XuatExcel;
            }
            set
            {
                if (_mBaoCaoNhapXuatDenKhoaPhong_YCu_XuatExcel == value)
                    return;
                _mBaoCaoNhapXuatDenKhoaPhong_YCu_XuatExcel = value;
                NotifyOfPropertyChange(() => mBaoCaoNhapXuatDenKhoaPhong_YCu_XuatExcel);
            }
        }




        public bool mBaoCaoNhapXuatDenKhoaPhong_HoaChat_In
        {
            get
            {
                return _mBaoCaoNhapXuatDenKhoaPhong_HoaChat_In;
            }
            set
            {
                if (_mBaoCaoNhapXuatDenKhoaPhong_HoaChat_In == value)
                    return;
                _mBaoCaoNhapXuatDenKhoaPhong_HoaChat_In = value;
                NotifyOfPropertyChange(() => mBaoCaoNhapXuatDenKhoaPhong_HoaChat_In);
            }
        }


        public bool mBaoCaoNhapXuatDenKhoaPhong_HoaChat_XuatExcel
        {
            get
            {
                return _mBaoCaoNhapXuatDenKhoaPhong_HoaChat_XuatExcel;
            }
            set
            {
                if (_mBaoCaoNhapXuatDenKhoaPhong_HoaChat_XuatExcel == value)
                    return;
                _mBaoCaoNhapXuatDenKhoaPhong_HoaChat_XuatExcel = value;
                NotifyOfPropertyChange(() => mBaoCaoNhapXuatDenKhoaPhong_HoaChat_XuatExcel);
            }
        }


        //them ngay 28-07-1012
        public bool mBaoCaoTheoDoiCongNo_Thuoc_Xem
        {
            get
            {
                return _mBaoCaoTheoDoiCongNo_Thuoc_Xem;
            }
            set
            {
                if (_mBaoCaoTheoDoiCongNo_Thuoc_Xem == value)
                    return;
                _mBaoCaoTheoDoiCongNo_Thuoc_Xem = value;
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_Thuoc_Xem);
            }
        }


        public bool mBaoCaoTheoDoiCongNo_Thuoc_In
        {
            get
            {
                return _mBaoCaoTheoDoiCongNo_Thuoc_In;
            }
            set
            {
                if (_mBaoCaoTheoDoiCongNo_Thuoc_In == value)
                    return;
                _mBaoCaoTheoDoiCongNo_Thuoc_In = value;
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_Thuoc_In);
            }
        }




        public bool mBaoCaoTheoDoiCongNo_YCu_Xem
        {
            get
            {
                return _mBaoCaoTheoDoiCongNo_YCu_Xem;
            }
            set
            {
                if (_mBaoCaoTheoDoiCongNo_YCu_Xem == value)
                    return;
                _mBaoCaoTheoDoiCongNo_YCu_Xem = value;
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_YCu_Xem);
            }
        }


        public bool mBaoCaoTheoDoiCongNo_YCu_In
        {
            get
            {
                return _mBaoCaoTheoDoiCongNo_YCu_In;
            }
            set
            {
                if (_mBaoCaoTheoDoiCongNo_YCu_In == value)
                    return;
                _mBaoCaoTheoDoiCongNo_YCu_In = value;
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_YCu_In);
            }
        }




        public bool mBaoCaoTheoDoiCongNo_HoaChat_Xem
        {
            get
            {
                return _mBaoCaoTheoDoiCongNo_HoaChat_Xem;
            }
            set
            {
                if (_mBaoCaoTheoDoiCongNo_HoaChat_Xem == value)
                    return;
                _mBaoCaoTheoDoiCongNo_HoaChat_Xem = value;
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_HoaChat_Xem);
            }
        }


        public bool mBaoCaoTheoDoiCongNo_HoaChat_In
        {
            get
            {
                return _mBaoCaoTheoDoiCongNo_HoaChat_In;
            }
            set
            {
                if (_mBaoCaoTheoDoiCongNo_HoaChat_In == value)
                    return;
                _mBaoCaoTheoDoiCongNo_HoaChat_In = value;
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_HoaChat_In);
            }
        }






        public bool mDuyetPhieuLinhHang_Thuoc_Tim
        {
            get
            {
                return _mDuyetPhieuLinhHang_Thuoc_Tim;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_Thuoc_Tim == value)
                    return;
                _mDuyetPhieuLinhHang_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Thuoc_Tim);
            }
        }

        public bool mDuyetPhieuLinhHang_Thuoc_PhieuMoi
        {
            get
            {
                return _mDuyetPhieuLinhHang_Thuoc_PhieuMoi;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_Thuoc_PhieuMoi == value)
                    return;
                _mDuyetPhieuLinhHang_Thuoc_PhieuMoi = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Thuoc_PhieuMoi);
            }
        }

        public bool mDuyetPhieuLinhHang_Thuoc_XuatHang
        {
            get
            {
                return _mDuyetPhieuLinhHang_Thuoc_XuatHang;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_Thuoc_XuatHang == value)
                    return;
                _mDuyetPhieuLinhHang_Thuoc_XuatHang = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Thuoc_XuatHang);
            }
        }

        public bool mDuyetPhieuLinhHang_Thuoc_XemInTH
        {
            get
            {
                return _mDuyetPhieuLinhHang_Thuoc_XemInTH;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_Thuoc_XemInTH == value)
                    return;
                _mDuyetPhieuLinhHang_Thuoc_XemInTH = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Thuoc_XemInTH);
            }
        }

        public bool mDuyetPhieuLinhHang_Thuoc_XemInCT
        {
            get
            {
                return _mDuyetPhieuLinhHang_Thuoc_XemInCT;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_Thuoc_XemInCT == value)
                    return;
                _mDuyetPhieuLinhHang_Thuoc_XemInCT = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Thuoc_XemInCT);
            }
        }




        public bool mDuyetPhieuLinhHang_YCu_Tim
        {
            get
            {
                return _mDuyetPhieuLinhHang_YCu_Tim;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_YCu_Tim == value)
                    return;
                _mDuyetPhieuLinhHang_YCu_Tim = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_YCu_Tim);
            }
        }

        public bool mDuyetPhieuLinhHang_YCu_PhieuMoi
        {
            get
            {
                return _mDuyetPhieuLinhHang_YCu_PhieuMoi;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_YCu_PhieuMoi == value)
                    return;
                _mDuyetPhieuLinhHang_YCu_PhieuMoi = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_YCu_PhieuMoi);
            }
        }

        public bool mDuyetPhieuLinhHang_YCu_XuatHang
        {
            get
            {
                return _mDuyetPhieuLinhHang_YCu_XuatHang;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_YCu_XuatHang == value)
                    return;
                _mDuyetPhieuLinhHang_YCu_XuatHang = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_YCu_XuatHang);
            }
        }

        public bool mDuyetPhieuLinhHang_YCu_XemInTH
        {
            get
            {
                return _mDuyetPhieuLinhHang_YCu_XemInTH;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_YCu_XemInTH == value)
                    return;
                _mDuyetPhieuLinhHang_YCu_XemInTH = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_YCu_XemInTH);
            }
        }

        public bool mDuyetPhieuLinhHang_YCu_XemInCT
        {
            get
            {
                return _mDuyetPhieuLinhHang_YCu_XemInCT;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_YCu_XemInCT == value)
                    return;
                _mDuyetPhieuLinhHang_YCu_XemInCT = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_YCu_XemInCT);
            }
        }


        public bool mDuyetPhieuLinhHang_HoaChat_Tim
        {
            get
            {
                return _mDuyetPhieuLinhHang_HoaChat_Tim;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_HoaChat_Tim == value)
                    return;
                _mDuyetPhieuLinhHang_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_HoaChat_Tim);
            }
        }

        public bool mDuyetPhieuLinhHang_HoaChat_PhieuMoi
        {
            get
            {
                return _mDuyetPhieuLinhHang_HoaChat_PhieuMoi;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_HoaChat_PhieuMoi == value)
                    return;
                _mDuyetPhieuLinhHang_HoaChat_PhieuMoi = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_HoaChat_PhieuMoi);
            }
        }

        public bool mDuyetPhieuLinhHang_HoaChat_XuatHang
        {
            get
            {
                return _mDuyetPhieuLinhHang_HoaChat_XuatHang;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_HoaChat_XuatHang == value)
                    return;
                _mDuyetPhieuLinhHang_HoaChat_XuatHang = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_HoaChat_XuatHang);
            }
        }

        public bool mDuyetPhieuLinhHang_HoaChat_XemInTH
        {
            get
            {
                return _mDuyetPhieuLinhHang_HoaChat_XemInTH;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_HoaChat_XemInTH == value)
                    return;
                _mDuyetPhieuLinhHang_HoaChat_XemInTH = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_HoaChat_XemInTH);
            }
        }

        public bool mDuyetPhieuLinhHang_HoaChat_XemInCT
        {
            get
            {
                return _mDuyetPhieuLinhHang_HoaChat_XemInCT;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_HoaChat_XemInCT == value)
                    return;
                _mDuyetPhieuLinhHang_HoaChat_XemInCT = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_HoaChat_XemInCT);
            }
        }


        public bool mXuatHangKyGui_Thuoc_Tim
        {
            get
            {
                return _mXuatHangKyGui_Thuoc_Tim;
            }
            set
            {
                if (_mXuatHangKyGui_Thuoc_Tim == value)
                    return;
                _mXuatHangKyGui_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc_Tim);
            }
        }

        public bool mXuatHangKyGui_Thuoc_PhieuMoi
        {
            get
            {
                return _mXuatHangKyGui_Thuoc_PhieuMoi;
            }
            set
            {
                if (_mXuatHangKyGui_Thuoc_PhieuMoi == value)
                    return;
                _mXuatHangKyGui_Thuoc_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc_PhieuMoi);
            }
        }

        public bool mXuatHangKyGui_Thuoc_Save
        {
            get
            {
                return _mXuatHangKyGui_Thuoc_Save;
            }
            set
            {
                if (_mXuatHangKyGui_Thuoc_Save == value)
                    return;
                _mXuatHangKyGui_Thuoc_Save = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc_Save);
            }
        }

        public bool mXuatHangKyGui_Thuoc_ThuTien
        {
            get
            {
                return _mXuatHangKyGui_Thuoc_ThuTien;
            }
            set
            {
                if (_mXuatHangKyGui_Thuoc_ThuTien == value)
                    return;
                _mXuatHangKyGui_Thuoc_ThuTien = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc_ThuTien);
            }
        }

        public bool mXuatHangKyGui_Thuoc_XemIn
        {
            get
            {
                return _mXuatHangKyGui_Thuoc_XemIn;
            }
            set
            {
                if (_mXuatHangKyGui_Thuoc_XemIn == value)
                    return;
                _mXuatHangKyGui_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc_XemIn);
            }
        }

        public bool mXuatHangKyGui_Thuoc_In
        {
            get
            {
                return _mXuatHangKyGui_Thuoc_In;
            }
            set
            {
                if (_mXuatHangKyGui_Thuoc_In == value)
                    return;
                _mXuatHangKyGui_Thuoc_In = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc_In);
            }
        }

        public bool mXuatHangKyGui_Thuoc_DeleteInvoice
        {
            get
            {
                return _mXuatHangKyGui_Thuoc_DeleteInvoice;
            }
            set
            {
                if (_mXuatHangKyGui_Thuoc_DeleteInvoice == value)
                    return;
                _mXuatHangKyGui_Thuoc_DeleteInvoice = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc_DeleteInvoice);
            }
        }

        public bool mXuatHangKyGui_Thuoc_PrintReceipt
        {
            get
            {
                return _mXuatHangKyGui_Thuoc_PrintReceipt;
            }
            set
            {
                if (_mXuatHangKyGui_Thuoc_PrintReceipt == value)
                    return;
                _mXuatHangKyGui_Thuoc_PrintReceipt = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc_PrintReceipt);
            }
        }



        public bool mXuatHangKyGui_YCu_Tim
        {
            get
            {
                return _mXuatHangKyGui_YCu_Tim;
            }
            set
            {
                if (_mXuatHangKyGui_YCu_Tim == value)
                    return;
                _mXuatHangKyGui_YCu_Tim = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu_Tim);
            }
        }

        public bool mXuatHangKyGui_YCu_PhieuMoi
        {
            get
            {
                return _mXuatHangKyGui_YCu_PhieuMoi;
            }
            set
            {
                if (_mXuatHangKyGui_YCu_PhieuMoi == value)
                    return;
                _mXuatHangKyGui_YCu_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu_PhieuMoi);
            }
        }

        public bool mXuatHangKyGui_YCu_Save
        {
            get
            {
                return _mXuatHangKyGui_YCu_Save;
            }
            set
            {
                if (_mXuatHangKyGui_YCu_Save == value)
                    return;
                _mXuatHangKyGui_YCu_Save = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu_Save);
            }
        }

        public bool mXuatHangKyGui_YCu_ThuTien
        {
            get
            {
                return _mXuatHangKyGui_YCu_ThuTien;
            }
            set
            {
                if (_mXuatHangKyGui_YCu_ThuTien == value)
                    return;
                _mXuatHangKyGui_YCu_ThuTien = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu_ThuTien);
            }
        }

        public bool mXuatHangKyGui_YCu_XemIn
        {
            get
            {
                return _mXuatHangKyGui_YCu_XemIn;
            }
            set
            {
                if (_mXuatHangKyGui_YCu_XemIn == value)
                    return;
                _mXuatHangKyGui_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu_XemIn);
            }
        }

        public bool mXuatHangKyGui_YCu_In
        {
            get
            {
                return _mXuatHangKyGui_YCu_In;
            }
            set
            {
                if (_mXuatHangKyGui_YCu_In == value)
                    return;
                _mXuatHangKyGui_YCu_In = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu_In);
            }
        }

        public bool mXuatHangKyGui_YCu_DeleteInvoice
        {
            get
            {
                return _mXuatHangKyGui_YCu_DeleteInvoice;
            }
            set
            {
                if (_mXuatHangKyGui_YCu_DeleteInvoice == value)
                    return;
                _mXuatHangKyGui_YCu_DeleteInvoice = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu_DeleteInvoice);
            }
        }

        public bool mXuatHangKyGui_YCu_PrintReceipt
        {
            get
            {
                return _mXuatHangKyGui_YCu_PrintReceipt;
            }
            set
            {
                if (_mXuatHangKyGui_YCu_PrintReceipt == value)
                    return;
                _mXuatHangKyGui_YCu_PrintReceipt = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu_PrintReceipt);
            }
        }


        public bool mXuatHangKyGui_HoaChat_Tim
        {
            get
            {
                return _mXuatHangKyGui_HoaChat_Tim;
            }
            set
            {
                if (_mXuatHangKyGui_HoaChat_Tim == value)
                    return;
                _mXuatHangKyGui_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat_Tim);
            }
        }


        public bool mXuatHangKyGui_HoaChat_PhieuMoi
        {
            get
            {
                return _mXuatHangKyGui_HoaChat_PhieuMoi;
            }
            set
            {
                if (_mXuatHangKyGui_HoaChat_PhieuMoi == value)
                    return;
                _mXuatHangKyGui_HoaChat_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat_PhieuMoi);
            }
        }

        public bool mXuatHangKyGui_HoaChat_Save
        {
            get
            {
                return _mXuatHangKyGui_HoaChat_Save;
            }
            set
            {
                if (_mXuatHangKyGui_HoaChat_Save == value)
                    return;
                _mXuatHangKyGui_HoaChat_Save = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat_Save);
            }
        }

        public bool mXuatHangKyGui_HoaChat_ThuTien
        {
            get
            {
                return _mXuatHangKyGui_HoaChat_ThuTien;
            }
            set
            {
                if (_mXuatHangKyGui_HoaChat_ThuTien == value)
                    return;
                _mXuatHangKyGui_HoaChat_ThuTien = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat_ThuTien);
            }
        }


        public bool mXuatHangKyGui_HoaChat_XemIn
        {
            get
            {
                return _mXuatHangKyGui_HoaChat_XemIn;
            }
            set
            {
                if (_mXuatHangKyGui_HoaChat_XemIn == value)
                    return;
                _mXuatHangKyGui_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat_XemIn);
            }
        }


        public bool mXuatHangKyGui_HoaChat_In
        {
            get
            {
                return _mXuatHangKyGui_HoaChat_In;
            }
            set
            {
                if (_mXuatHangKyGui_HoaChat_In == value)
                    return;
                _mXuatHangKyGui_HoaChat_In = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat_In);
            }
        }

        public bool mXuatHangKyGui_HoaChat_DeleteInvoice
        {
            get
            {
                return _mXuatHangKyGui_HoaChat_DeleteInvoice;
            }
            set
            {
                if (_mXuatHangKyGui_HoaChat_DeleteInvoice == value)
                    return;
                _mXuatHangKyGui_HoaChat_DeleteInvoice = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat_DeleteInvoice);
            }
        }

        public bool mXuatHangKyGui_HoaChat_PrintReceipt
        {
            get
            {
                return _mXuatHangKyGui_HoaChat_PrintReceipt;
            }
            set
            {
                if (_mXuatHangKyGui_HoaChat_PrintReceipt == value)
                    return;
                _mXuatHangKyGui_HoaChat_PrintReceipt = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat_PrintReceipt);
            }
        }




        public bool mSapNhapHangKyGui_Thuoc_Tim
        {
            get
            {
                return _mSapNhapHangKyGui_Thuoc_Tim;
            }
            set
            {
                if (_mSapNhapHangKyGui_Thuoc_Tim == value)
                    return;
                _mSapNhapHangKyGui_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Thuoc_Tim);
            }
        }

        public bool mSapNhapHangKyGui_Thuoc_PhieuMoi
        {
            get
            {
                return _mSapNhapHangKyGui_Thuoc_PhieuMoi;
            }
            set
            {
                if (_mSapNhapHangKyGui_Thuoc_PhieuMoi == value)
                    return;
                _mSapNhapHangKyGui_Thuoc_PhieuMoi = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Thuoc_PhieuMoi);
            }
        }

        public bool mSapNhapHangKyGui_Thuoc_CapNhat
        {
            get
            {
                return _mSapNhapHangKyGui_Thuoc_CapNhat;
            }
            set
            {
                if (_mSapNhapHangKyGui_Thuoc_CapNhat == value)
                    return;
                _mSapNhapHangKyGui_Thuoc_CapNhat = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Thuoc_CapNhat);
            }
        }

        public bool mSapNhapHangKyGui_Thuoc_Xoa
        {
            get
            {
                return _mSapNhapHangKyGui_Thuoc_Xoa;
            }
            set
            {
                if (_mSapNhapHangKyGui_Thuoc_Xoa == value)
                    return;
                _mSapNhapHangKyGui_Thuoc_Xoa = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Thuoc_Xoa);
            }
        }

        public bool mSapNhapHangKyGui_Thuoc_XemIn
        {
            get
            {
                return _mSapNhapHangKyGui_Thuoc_XemIn;
            }
            set
            {
                if (_mSapNhapHangKyGui_Thuoc_XemIn == value)
                    return;
                _mSapNhapHangKyGui_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Thuoc_XemIn);
            }
        }

        public bool mSapNhapHangKyGui_Thuoc_In
        {
            get
            {
                return _mSapNhapHangKyGui_Thuoc_In;
            }
            set
            {
                if (_mSapNhapHangKyGui_Thuoc_In == value)
                    return;
                _mSapNhapHangKyGui_Thuoc_In = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Thuoc_In);
            }
        }




        public bool mSapNhapHangKyGui_YCu_Tim
        {
            get
            {
                return _mSapNhapHangKyGui_YCu_Tim;
            }
            set
            {
                if (_mSapNhapHangKyGui_YCu_Tim == value)
                    return;
                _mSapNhapHangKyGui_YCu_Tim = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_YCu_Tim);
            }
        }


        public bool mSapNhapHangKyGui_YCu_PhieuMoi
        {
            get
            {
                return _mSapNhapHangKyGui_YCu_PhieuMoi;
            }
            set
            {
                if (_mSapNhapHangKyGui_YCu_PhieuMoi == value)
                    return;
                _mSapNhapHangKyGui_YCu_PhieuMoi = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_YCu_PhieuMoi);
            }
        }


        public bool mSapNhapHangKyGui_YCu_CapNhat
        {
            get
            {
                return _mSapNhapHangKyGui_YCu_CapNhat;
            }
            set
            {
                if (_mSapNhapHangKyGui_YCu_CapNhat == value)
                    return;
                _mSapNhapHangKyGui_YCu_CapNhat = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_YCu_CapNhat);
            }
        }


        public bool mSapNhapHangKyGui_YCu_Xoa
        {
            get
            {
                return _mSapNhapHangKyGui_YCu_Xoa;
            }
            set
            {
                if (_mSapNhapHangKyGui_YCu_Xoa == value)
                    return;
                _mSapNhapHangKyGui_YCu_Xoa = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_YCu_Xoa);
            }
        }


        public bool mSapNhapHangKyGui_YCu_XemIn
        {
            get
            {
                return _mSapNhapHangKyGui_YCu_XemIn;
            }
            set
            {
                if (_mSapNhapHangKyGui_YCu_XemIn == value)
                    return;
                _mSapNhapHangKyGui_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_YCu_XemIn);
            }
        }


        public bool mSapNhapHangKyGui_YCu_In
        {
            get
            {
                return _mSapNhapHangKyGui_YCu_In;
            }
            set
            {
                if (_mSapNhapHangKyGui_YCu_In == value)
                    return;
                _mSapNhapHangKyGui_YCu_In = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_YCu_In);
            }
        }




        public bool mSapNhapHangKyGui_HoaChat_Tim
        {
            get
            {
                return _mSapNhapHangKyGui_HoaChat_Tim;
            }
            set
            {
                if (_mSapNhapHangKyGui_HoaChat_Tim == value)
                    return;
                _mSapNhapHangKyGui_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_HoaChat_Tim);
            }
        }


        public bool mSapNhapHangKyGui_HoaChat_PhieuMoi
        {
            get
            {
                return _mSapNhapHangKyGui_HoaChat_PhieuMoi;
            }
            set
            {
                if (_mSapNhapHangKyGui_HoaChat_PhieuMoi == value)
                    return;
                _mSapNhapHangKyGui_HoaChat_PhieuMoi = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_HoaChat_PhieuMoi);
            }
        }


        public bool mSapNhapHangKyGui_HoaChat_CapNhat
        {
            get
            {
                return _mSapNhapHangKyGui_HoaChat_CapNhat;
            }
            set
            {
                if (_mSapNhapHangKyGui_HoaChat_CapNhat == value)
                    return;
                _mSapNhapHangKyGui_HoaChat_CapNhat = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_HoaChat_CapNhat);
            }
        }


        public bool mSapNhapHangKyGui_HoaChat_Xoa
        {
            get
            {
                return _mSapNhapHangKyGui_HoaChat_Xoa;
            }
            set
            {
                if (_mSapNhapHangKyGui_HoaChat_Xoa == value)
                    return;
                _mSapNhapHangKyGui_HoaChat_Xoa = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_HoaChat_Xoa);
            }
        }


        public bool mSapNhapHangKyGui_HoaChat_XemIn
        {
            get
            {
                return _mSapNhapHangKyGui_HoaChat_XemIn;
            }
            set
            {
                if (_mSapNhapHangKyGui_HoaChat_XemIn == value)
                    return;
                _mSapNhapHangKyGui_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_HoaChat_XemIn);
            }
        }


        public bool mSapNhapHangKyGui_HoaChat_In
        {
            get
            {
                return _mSapNhapHangKyGui_HoaChat_In;
            }
            set
            {
                if (_mSapNhapHangKyGui_HoaChat_In == value)
                    return;
                _mSapNhapHangKyGui_HoaChat_In = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_HoaChat_In);
            }
        }






        public bool mNhapTraTuKhoPhong_Thuoc_Tim
        {
            get
            {
                return _mNhapTraTuKhoPhong_Thuoc_Tim;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_Thuoc_Tim == value)
                    return;
                _mNhapTraTuKhoPhong_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Thuoc_Tim);
            }
        }

        public bool mNhapTraTuKhoPhong_Thuoc_PhieuMoi
        {
            get
            {
                return _mNhapTraTuKhoPhong_Thuoc_PhieuMoi;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_Thuoc_PhieuMoi == value)
                    return;
                _mNhapTraTuKhoPhong_Thuoc_PhieuMoi = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Thuoc_PhieuMoi);
            }
        }

        public bool mNhapTraTuKhoPhong_Thuoc_XemIn
        {
            get
            {
                return _mNhapTraTuKhoPhong_Thuoc_XemIn;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_Thuoc_XemIn == value)
                    return;
                _mNhapTraTuKhoPhong_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Thuoc_XemIn);
            }
        }

        public bool mNhapTraTuKhoPhong_Thuoc_In
        {
            get
            {
                return _mNhapTraTuKhoPhong_Thuoc_In;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_Thuoc_In == value)
                    return;
                _mNhapTraTuKhoPhong_Thuoc_In = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Thuoc_In);
            }
        }




        public bool mNhapTraTuKhoPhong_YCu_Tim
        {
            get
            {
                return _mNhapTraTuKhoPhong_YCu_Tim;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_YCu_Tim == value)
                    return;
                _mNhapTraTuKhoPhong_YCu_Tim = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_YCu_Tim);
            }
        }


        public bool mNhapTraTuKhoPhong_YCu_PhieuMoi
        {
            get
            {
                return _mNhapTraTuKhoPhong_YCu_PhieuMoi;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_YCu_PhieuMoi == value)
                    return;
                _mNhapTraTuKhoPhong_YCu_PhieuMoi = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_YCu_PhieuMoi);
            }
        }


        public bool mNhapTraTuKhoPhong_YCu_XemIn
        {
            get
            {
                return _mNhapTraTuKhoPhong_YCu_XemIn;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_YCu_XemIn == value)
                    return;
                _mNhapTraTuKhoPhong_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_YCu_XemIn);
            }
        }


        public bool mNhapTraTuKhoPhong_YCu_In
        {
            get
            {
                return _mNhapTraTuKhoPhong_YCu_In;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_YCu_In == value)
                    return;
                _mNhapTraTuKhoPhong_YCu_In = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_YCu_In);
            }
        }




        public bool mNhapTraTuKhoPhong_HoaChat_Tim
        {
            get
            {
                return _mNhapTraTuKhoPhong_HoaChat_Tim;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_HoaChat_Tim == value)
                    return;
                _mNhapTraTuKhoPhong_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_HoaChat_Tim);
            }
        }


        public bool mNhapTraTuKhoPhong_HoaChat_PhieuMoi
        {
            get
            {
                return _mNhapTraTuKhoPhong_HoaChat_PhieuMoi;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_HoaChat_PhieuMoi == value)
                    return;
                _mNhapTraTuKhoPhong_HoaChat_PhieuMoi = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_HoaChat_PhieuMoi);
            }
        }


        public bool mNhapTraTuKhoPhong_HoaChat_XemIn
        {
            get
            {
                return _mNhapTraTuKhoPhong_HoaChat_XemIn;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_HoaChat_XemIn == value)
                    return;
                _mNhapTraTuKhoPhong_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_HoaChat_XemIn);
            }
        }


        public bool mNhapTraTuKhoPhong_HoaChat_In
        {
            get
            {
                return _mNhapTraTuKhoPhong_HoaChat_In;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_HoaChat_In == value)
                    return;
                _mNhapTraTuKhoPhong_HoaChat_In = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_HoaChat_In);
            }
        }


        public bool mWatchMedOutQty_Preview
        {
            get
            {
                return _mWatchMedOutQty_Preview;
            }
            set
            {
                if (_mWatchMedOutQty_Preview == value)
                    return;
                _mWatchMedOutQty_Preview = value;
                NotifyOfPropertyChange(() => mWatchMedOutQty_Preview);
            }
        }

        public bool mWatchMedOutQty_Print
        {
            get
            {
                return _mWatchMedOutQty_Print;
            }
            set
            {
                if (_mWatchMedOutQty_Print == value)
                    return;
                _mWatchMedOutQty_Print = value;
                NotifyOfPropertyChange(() => mWatchMedOutQty_Print);
            }
        }

        public bool mWatchMatOutQty_Preview
        {
            get
            {
                return _mWatchMatOutQty_Preview;
            }
            set
            {
                if (_mWatchMatOutQty_Preview == value)
                    return;
                _mWatchMatOutQty_Preview = value;
                NotifyOfPropertyChange(() => mWatchMatOutQty_Preview);
            }
        }

        public bool mWatchMatOutQty_Print
        {
            get
            {
                return _mWatchMatOutQty_Print;
            }
            set
            {
                if (_mWatchMatOutQty_Print == value)
                    return;
                _mWatchMatOutQty_Print = value;
                NotifyOfPropertyChange(() => mWatchMatOutQty_Print);
            }
        }

        public bool mWatchLabOutQty_Preview
        {
            get
            {
                return _mWatchLabOutQty_Preview;
            }
            set
            {
                if (_mWatchLabOutQty_Preview == value)
                    return;
                _mWatchLabOutQty_Preview = value;
                NotifyOfPropertyChange(() => mWatchLabOutQty_Preview);
            }
        }

        public bool mWatchLabOutQty_Print
        {
            get
            {
                return _mWatchLabOutQty_Print;
            }
            set
            {
                if (_mWatchLabOutQty_Print == value)
                    return;
                _mWatchLabOutQty_Print = value;
                NotifyOfPropertyChange(() => mWatchLabOutQty_Print);
            }
        }

        /*▼====: #001*/
        private bool _mBid = true;
        public bool mBid
        {
            get
            {
                return _mBid;
            }
            set
            {
                _mBid = value;
            }
        }
        private bool _mBidDetail = true;
        public bool mBidDetail
        {
            get
            {
                return _mBidDetail;
            }
            set
            {
                if (_mBidDetail == value)
                {
                    return;
                }
                _mBidDetail = value;
                NotifyOfPropertyChange(() => mBidDetail);
                NotifyOfPropertyChange(() => mBidDetail_Med);
                NotifyOfPropertyChange(() => mBidDetail_Mat);
            }
        }
        private bool _mBidDetail_Med = true;
        private bool _mBidDetail_Mat = true;
        public bool mBidDetail_Med
        {
            //get
            //{
            //    return mBidDetail && MenuVisibleCollection[0];
            //}
            get
            {
                return _mBidDetail_Med;
            }
            set
            {
                if (_mBidDetail_Med == value)
                {
                    return;
                }
                _mBidDetail_Med = value;
                NotifyOfPropertyChange(() => mBidDetail_Med);
            }
        }
        public bool mBidDetail_Mat
        {
            //get
            //{
            //    return mBidDetail && MenuVisibleCollection[1];
            //}
            get
            {
                return _mBidDetail_Mat;
            }
            set
            {
                if (_mBidDetail_Mat == value)
                {
                    return;
                }
                _mBidDetail_Mat = value;
                NotifyOfPropertyChange(() => mBidDetail_Mat);
            }
        }
        /*▲====: #001*/

        //▼====: #019
        private bool _mNhapHangNCC_DinhDuong_Tim = true;
        private bool _mNhapHangNCC_DinhDuong_ThemMoi = true;
        private bool _mNhapHangNCC_DinhDuong_CapNhat = true;
        private bool _mNhapHangNCC_DinhDuong_In = true;
        private bool _mInwardDrugFromSupplierNutritionCmd = true;

        public bool mInwardDrugFromSupplierNutritionCmd
        {
            get
            {
                return _mInwardDrugFromSupplierNutritionCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mInwardDrugFromSupplierNutritionCmd == value)
                    return;
                _mInwardDrugFromSupplierNutritionCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierNutritionCmd);
            }
        }
        public bool mNhapHangNCC_DinhDuong_Tim
        {
            get
            {
                return _mNhapHangNCC_DinhDuong_Tim;
            }
            set
            {
                if (_mNhapHangNCC_DinhDuong_Tim == value)
                    return;
                _mNhapHangNCC_DinhDuong_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_DinhDuong_Tim);
            }
        }


        public bool mNhapHangNCC_DinhDuong_ThemMoi
        {
            get
            {
                return _mNhapHangNCC_DinhDuong_ThemMoi;
            }
            set
            {
                if (_mNhapHangNCC_DinhDuong_ThemMoi == value)
                    return;
                _mNhapHangNCC_DinhDuong_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_DinhDuong_ThemMoi);
            }
        }


        public bool mNhapHangNCC_DinhDuong_CapNhat
        {
            get
            {
                return _mNhapHangNCC_DinhDuong_CapNhat;
            }
            set
            {
                if (_mNhapHangNCC_DinhDuong_CapNhat == value)
                    return;
                _mNhapHangNCC_DinhDuong_CapNhat = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_DinhDuong_CapNhat);
            }
        }


        public bool mNhapHangNCC_DinhDuong_In
        {
            get
            {
                return _mNhapHangNCC_DinhDuong_In;
            }
            set
            {
                if (_mNhapHangNCC_DinhDuong_In == value)
                    return;
                _mNhapHangNCC_DinhDuong_In = value;
                NotifyOfPropertyChange(() => mNhapHangNCC_DinhDuong_In);
            }
        }
        //▲====: #019
        #endregion

        #region Authority Operation
        public void AuthorityOperation()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mXuat_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_Thuoc,
                                               (int)oKhoaDuocEx.mXuat_Thuoc_Tim, (int)ePermission.mView);
            mXuat_Thuoc_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_Thuoc,
                                               (int)oKhoaDuocEx.mXuat_Thuoc_PhieuMoi, (int)ePermission.mView);
            mXuat_Thuoc_ThucHien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_Thuoc,
                                               (int)oKhoaDuocEx.mXuat_Thuoc_ThucHien, (int)ePermission.mView);
            mXuat_Thuoc_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_Thuoc,
                                               (int)oKhoaDuocEx.mXuat_Thuoc_ThuTien, (int)ePermission.mView);
            mXuat_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_Thuoc,
                                               (int)oKhoaDuocEx.mXuat_Thuoc_In, (int)ePermission.mView);
            mXuat_Thuoc_DeleteInvoice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_Thuoc,
                                               (int)oKhoaDuocEx.mXuat_Thuoc_DeleteInvoice, (int)ePermission.mView);
            mXuat_Thuoc_PrintReceipt = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_Thuoc,
                                               (int)oKhoaDuocEx.mXuat_Thuoc_PrintReceipt, (int)ePermission.mView);


            mXuat_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_YCu,
                                               (int)oKhoaDuocEx.mXuat_YCu_Tim, (int)ePermission.mView);
            mXuat_YCu_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_YCu,
                                               (int)oKhoaDuocEx.mXuat_YCu_PhieuMoi, (int)ePermission.mView);
            mXuat_YCu_ThucHien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_YCu,
                                               (int)oKhoaDuocEx.mXuat_YCu_ThucHien, (int)ePermission.mView);
            mXuat_YCu_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_YCu,
                                               (int)oKhoaDuocEx.mXuat_YCu_ThuTien, (int)ePermission.mView);
            mXuat_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_YCu,
                                               (int)oKhoaDuocEx.mXuat_YCu_In, (int)ePermission.mView);
            mXuat_YCu_DeleteInvoice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_YCu,
                                               (int)oKhoaDuocEx.mXuat_YCu_DeleteInvoice, (int)ePermission.mView);
            mXuat_YCu_PrintReceipt = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_YCu,
                                               (int)oKhoaDuocEx.mXuat_YCu_PrintReceipt, (int)ePermission.mView);

            mXuat_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_HoaChat,
                                               (int)oKhoaDuocEx.mXuat_HoaChat_Tim, (int)ePermission.mView);
            mXuat_HoaChat_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_HoaChat,
                                               (int)oKhoaDuocEx.mXuat_HoaChat_PhieuMoi, (int)ePermission.mView);
            mXuat_HoaChat_ThucHien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_HoaChat,
                                               (int)oKhoaDuocEx.mXuat_HoaChat_ThucHien, (int)ePermission.mView);
            mXuat_HoaChat_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_HoaChat,
                                               (int)oKhoaDuocEx.mXuat_HoaChat_ThuTien, (int)ePermission.mView);
            mXuat_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_HoaChat,
                                               (int)oKhoaDuocEx.mXuat_HoaChat_In, (int)ePermission.mView);
            mXuat_HoaChat_DeleteInvoice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_HoaChat,
                                               (int)oKhoaDuocEx.mXuat_HoaChat_DeleteInvoice, (int)ePermission.mView);
            mXuat_HoaChat_PrintReceipt = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_HoaChat,
                                               (int)oKhoaDuocEx.mXuat_HoaChat_PrintReceipt, (int)ePermission.mView);


            mTraHang_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_Thuoc,
                                               (int)oKhoaDuocEx.mTraHang_Thuoc_Tim, (int)ePermission.mView);
            mTraHang_Thuoc_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_Thuoc,
                                               (int)oKhoaDuocEx.mTraHang_Thuoc_Luu, (int)ePermission.mView);
            mTraHang_Thuoc_TraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_Thuoc,
                                               (int)oKhoaDuocEx.mTraHang_Thuoc_TraTien, (int)ePermission.mView);
            mTraHang_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_Thuoc,
                                               (int)oKhoaDuocEx.mTraHang_Thuoc_In, (int)ePermission.mView);

            mTraHang_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_YCu,
                                               (int)oKhoaDuocEx.mTraHang_YCu_Tim, (int)ePermission.mView);
            mTraHang_YCu_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_YCu,
                                               (int)oKhoaDuocEx.mTraHang_YCu_Luu, (int)ePermission.mView);
            mTraHang_YCu_TraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_YCu,
                                               (int)oKhoaDuocEx.mTraHang_YCu_TraTien, (int)ePermission.mView);
            mTraHang_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_YCu,
                                               (int)oKhoaDuocEx.mTraHang_YCu_In, (int)ePermission.mView);

            mTraHang_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_HoaChat,
                                               (int)oKhoaDuocEx.mTraHang_HoaChat_Tim, (int)ePermission.mView);
            mTraHang_HoaChat_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_HoaChat,
                                               (int)oKhoaDuocEx.mTraHang_HoaChat_Luu, (int)ePermission.mView);
            mTraHang_HoaChat_TraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_HoaChat,
                                               (int)oKhoaDuocEx.mTraHang_HoaChat_TraTien, (int)ePermission.mView);
            mTraHang_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mTraHang_HoaChat,
                                               (int)oKhoaDuocEx.mTraHang_HoaChat_In, (int)ePermission.mView);


            mXuatHuy_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_Thuoc,
                                               (int)oKhoaDuocEx.mXuatHuy_Thuoc_Tim, (int)ePermission.mView);
            mXuatHuy_Thuoc_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_Thuoc,
                                               (int)oKhoaDuocEx.mXuatHuy_Thuoc_ThemMoi, (int)ePermission.mView);
            mXuatHuy_Thuoc_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_Thuoc,
                                               (int)oKhoaDuocEx.mXuatHuy_Thuoc_XuatExcel, (int)ePermission.mView);
            mXuatHuy_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_Thuoc,
                                               (int)oKhoaDuocEx.mXuatHuy_Thuoc_XemIn, (int)ePermission.mView);


            mXuatHuy_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_YCu,
                                               (int)oKhoaDuocEx.mXuatHuy_YCu_Tim, (int)ePermission.mView);
            mXuatHuy_YCu_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_YCu,
                                               (int)oKhoaDuocEx.mXuatHuy_YCu_ThemMoi, (int)ePermission.mView);
            mXuatHuy_YCu_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_YCu,
                                               (int)oKhoaDuocEx.mXuatHuy_YCu_XuatExcel, (int)ePermission.mView);
            mXuatHuy_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_YCu,
                                               (int)oKhoaDuocEx.mXuatHuy_YCu_XemIn, (int)ePermission.mView);

            mXuatHuy_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_HoaChat,
                                               (int)oKhoaDuocEx.mXuatHuy_HoaChat_Tim, (int)ePermission.mView);
            mXuatHuy_HoaChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_HoaChat,
                                               (int)oKhoaDuocEx.mXuatHuy_HoaChat_ThemMoi, (int)ePermission.mView);
            mXuatHuy_HoaChat_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_HoaChat,
                                               (int)oKhoaDuocEx.mXuatHuy_HoaChat_XuatExcel, (int)ePermission.mView);
            mXuatHuy_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuatHuy_HoaChat,
                                               (int)oKhoaDuocEx.mXuatHuy_HoaChat_XemIn, (int)ePermission.mView);

            mDuTru_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_Thuoc,
                                               (int)oKhoaDuocEx.mDuTru_Thuoc_Tim, (int)ePermission.mView);
            mDuTru_Thuoc_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_Thuoc,
                                               (int)oKhoaDuocEx.mDuTru_Thuoc_ThemMoi, (int)ePermission.mView);
            mDuTru_Thuoc_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_Thuoc,
                                               (int)oKhoaDuocEx.mDuTru_Thuoc_Xoa, (int)ePermission.mView);
            mDuTru_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_Thuoc,
                                               (int)oKhoaDuocEx.mDuTru_Thuoc_XemIn, (int)ePermission.mView);


            mDuTru_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_YCu,
                                               (int)oKhoaDuocEx.mDuTru_YCu_Tim, (int)ePermission.mView);
            mDuTru_YCu_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_YCu,
                                               (int)oKhoaDuocEx.mDuTru_YCu_ThemMoi, (int)ePermission.mView);
            mDuTru_YCu_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_YCu,
                                               (int)oKhoaDuocEx.mDuTru_YCu_Xoa, (int)ePermission.mView);
            mDuTru_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_YCu,
                                               (int)oKhoaDuocEx.mDuTru_YCu_XemIn, (int)ePermission.mView);

            mDuTru_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_HoaChat,
                                               (int)oKhoaDuocEx.mDuTru_HoaChat_Tim, (int)ePermission.mView);
            mDuTru_HoaChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_HoaChat,
                                               (int)oKhoaDuocEx.mDuTru_HoaChat_ThemMoi, (int)ePermission.mView);
            mDuTru_HoaChat_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_HoaChat,
                                               (int)oKhoaDuocEx.mDuTru_HoaChat_Xoa, (int)ePermission.mView);
            mDuTru_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_HoaChat,
                                               (int)oKhoaDuocEx.mDuTru_HoaChat_XemIn, (int)ePermission.mView);

            mDatHang_Thuoc_DSCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_Thuoc,
                                               (int)oKhoaDuocEx.mDatHang_Thuoc_DSCanDatHang, (int)ePermission.mView);
            mDatHang_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_Thuoc,
                                               (int)oKhoaDuocEx.mDatHang_Thuoc_Tim, (int)ePermission.mView);
            mDatHang_Thuoc_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_Thuoc,
                                               (int)oKhoaDuocEx.mDatHang_Thuoc_ChinhSua, (int)ePermission.mView);
            mDatHang_Thuoc_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_Thuoc,
                                               (int)oKhoaDuocEx.mDatHang_Thuoc_ThemMoi, (int)ePermission.mView);
            mDatHang_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_Thuoc,
                                               (int)oKhoaDuocEx.mDatHang_Thuoc_In, (int)ePermission.mView);

            mDatHang_YCu_DSCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_YCu,
                                               (int)oKhoaDuocEx.mDatHang_YCu_DSCanDatHang, (int)ePermission.mView);
            mDatHang_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_YCu,
                                               (int)oKhoaDuocEx.mDatHang_YCu_Tim, (int)ePermission.mView);
            mDatHang_YCu_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_YCu,
                                               (int)oKhoaDuocEx.mDatHang_YCu_ChinhSua, (int)ePermission.mView);
            mDatHang_YCu_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_YCu,
                                               (int)oKhoaDuocEx.mDatHang_YCu_ThemMoi, (int)ePermission.mView);
            mDatHang_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_YCu,
                                               (int)oKhoaDuocEx.mDatHang_YCu_In, (int)ePermission.mView);

            mDatHang_HoaChat_DSCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_HoaChat,
                                               (int)oKhoaDuocEx.mDatHang_HoaChat_DSCanDatHang, (int)ePermission.mView);
            mDatHang_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_HoaChat,
                                               (int)oKhoaDuocEx.mDatHang_HoaChat_Tim, (int)ePermission.mView);
            mDatHang_HoaChat_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_HoaChat,
                                               (int)oKhoaDuocEx.mDatHang_HoaChat_ChinhSua, (int)ePermission.mView);
            mDatHang_HoaChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_HoaChat,
                                               (int)oKhoaDuocEx.mDatHang_HoaChat_ThemMoi, (int)ePermission.mView);
            mDatHang_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDatHang_HoaChat,
                                               (int)oKhoaDuocEx.mDatHang_HoaChat_In, (int)ePermission.mView);

            mNhapHangNCC_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_Thuoc,
                                               (int)oKhoaDuocEx.mNhapHangNCC_Thuoc_Tim, (int)ePermission.mView);
            mNhapHangNCC_Thuoc_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_Thuoc,
                                               (int)oKhoaDuocEx.mNhapHangNCC_Thuoc_ThemMoi, (int)ePermission.mView);
            mNhapHangNCC_Thuoc_CapNhat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_Thuoc,
                                               (int)oKhoaDuocEx.mNhapHangNCC_Thuoc_CapNhat, (int)ePermission.mView);
            mNhapHangNCC_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_Thuoc,
                                               (int)oKhoaDuocEx.mNhapHangNCC_Thuoc_In, (int)ePermission.mView);

            mNhapHangNCC_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_YCu,
                                               (int)oKhoaDuocEx.mNhapHangNCC_YCu_Tim, (int)ePermission.mView);
            mNhapHangNCC_YCu_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_YCu,
                                               (int)oKhoaDuocEx.mNhapHangNCC_YCu_ThemMoi, (int)ePermission.mView);
            mNhapHangNCC_YCu_CapNhat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_YCu,
                                               (int)oKhoaDuocEx.mNhapHangNCC_YCu_CapNhat, (int)ePermission.mView);
            mNhapHangNCC_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_YCu,
                                               (int)oKhoaDuocEx.mNhapHangNCC_YCu_In, (int)ePermission.mView);


            mNhapHangNCC_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_HoaChat,
                                               (int)oKhoaDuocEx.mNhapHangNCC_HoaChat_Tim, (int)ePermission.mView);
            mNhapHangNCC_HoaChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_HoaChat,
                                               (int)oKhoaDuocEx.mNhapHangNCC_HoaChat_ThemMoi, (int)ePermission.mView);
            mNhapHangNCC_HoaChat_CapNhat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_HoaChat,
                                               (int)oKhoaDuocEx.mNhapHangNCC_HoaChat_CapNhat, (int)ePermission.mView);
            mNhapHangNCC_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_HoaChat,
                                               (int)oKhoaDuocEx.mNhapHangNCC_HoaChat_In, (int)ePermission.mView);

            mNhapPhanBoPhi_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapPhanBoPhi_Thuoc,
                                               (int)oKhoaDuocEx.mNhapPhanBoPhi_Thuoc_Tim, (int)ePermission.mView);
            mNhapPhanBoPhi_Thuoc_ChinhSua_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapPhanBoPhi_Thuoc,
                                               (int)oKhoaDuocEx.mNhapPhanBoPhi_Thuoc_ChinhSua_Them, (int)ePermission.mView);
            mNhapPhanBoPhi_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapPhanBoPhi_Thuoc,
                                               (int)oKhoaDuocEx.mNhapPhanBoPhi_Thuoc_In, (int)ePermission.mView);

            mNhapPhanBoPhi_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapPhanBoPhi_YCu,
                                               (int)oKhoaDuocEx.mNhapPhanBoPhi_YCu_Tim, (int)ePermission.mView);
            mNhapPhanBoPhi_YCu_ChinhSua_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapPhanBoPhi_YCu,
                                               (int)oKhoaDuocEx.mNhapPhanBoPhi_YCu_ChinhSua_Them, (int)ePermission.mView);
            mNhapPhanBoPhi_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapPhanBoPhi_YCu,
                                               (int)oKhoaDuocEx.mNhapPhanBoPhi_YCu_In, (int)ePermission.mView);


            mNhapPhanBoPhi_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapPhanBoPhi_HoaChat,
                                               (int)oKhoaDuocEx.mNhapPhanBoPhi_HoaChat_Tim, (int)ePermission.mView);
            mNhapPhanBoPhi_HoaChat_ChinhSua_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapPhanBoPhi_HoaChat,
                                               (int)oKhoaDuocEx.mNhapPhanBoPhi_HoaChat_ChinhSua_Them, (int)ePermission.mView);
            mNhapPhanBoPhi_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapPhanBoPhi_HoaChat,
                                               (int)oKhoaDuocEx.mNhapPhanBoPhi_HoaChat_In, (int)ePermission.mView);


            mPhieuDNThanhToan_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_Thuoc_Tim, (int)ePermission.mView);
            mPhieuDNThanhToan_Thuoc_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_Thuoc_PhieuMoi, (int)ePermission.mView);
            mPhieuDNThanhToan_Thuoc_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_Thuoc_Xoa, (int)ePermission.mView);
            mPhieuDNThanhToan_Thuoc_XemInBK = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_Thuoc_XemInBK, (int)ePermission.mView);
            mPhieuDNThanhToan_Thuoc_XemInPDNTT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_Thuoc_XemInPDNTT, (int)ePermission.mView);


            mPhieuDNThanhToan_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_YCu_Tim, (int)ePermission.mView);
            mPhieuDNThanhToan_YCu_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_YCu_PhieuMoi, (int)ePermission.mView);
            mPhieuDNThanhToan_YCu_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_YCu_Xoa, (int)ePermission.mView);
            mPhieuDNThanhToan_YCu_XemInBK = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_YCu_XemInBK, (int)ePermission.mView);
            mPhieuDNThanhToan_YCu_XemInPDNTT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_YCu_XemInPDNTT, (int)ePermission.mView);


            mPhieuDNThanhToan_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_HoaChat_Tim, (int)ePermission.mView);
            mPhieuDNThanhToan_HoaChat_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_HoaChat_PhieuMoi, (int)ePermission.mView);
            mPhieuDNThanhToan_HoaChat_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_HoaChat_Xoa, (int)ePermission.mView);
            mPhieuDNThanhToan_HoaChat_XemInBK = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_HoaChat_XemInBK, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT, (int)ePermission.mView);


            mKiemKe_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_Thuoc,
                                               (int)oKhoaDuocEx.mKiemKe_Thuoc_Tim, (int)ePermission.mView);
            mKiemKe_Thuoc_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_Thuoc,
                                               (int)oKhoaDuocEx.mKiemKe_Thuoc_ThemMoi, (int)ePermission.mView);
            mKiemKe_Thuoc_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_Thuoc,
                                               (int)oKhoaDuocEx.mKiemKe_Thuoc_XuatExcel, (int)ePermission.mView);
            mKiemKe_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_Thuoc,
                                               (int)oKhoaDuocEx.mKiemKe_Thuoc_XemIn, (int)ePermission.mView);

            mKiemKe_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_YCu,
                                               (int)oKhoaDuocEx.mKiemKe_YCu_Tim, (int)ePermission.mView);
            mKiemKe_YCu_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_YCu,
                                               (int)oKhoaDuocEx.mKiemKe_YCu_ThemMoi, (int)ePermission.mView);
            mKiemKe_YCu_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_YCu,
                                               (int)oKhoaDuocEx.mKiemKe_YCu_XuatExcel, (int)ePermission.mView);
            mKiemKe_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_YCu,
                                               (int)oKhoaDuocEx.mKiemKe_YCu_XemIn, (int)ePermission.mView);

            mKiemKe_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_HoaChat,
                                               (int)oKhoaDuocEx.mKiemKe_HoaChat_Tim, (int)ePermission.mView);
            mKiemKe_HoaChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_HoaChat,
                                               (int)oKhoaDuocEx.mKiemKe_HoaChat_ThemMoi, (int)ePermission.mView);
            mKiemKe_HoaChat_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_HoaChat,
                                               (int)oKhoaDuocEx.mKiemKe_HoaChat_XuatExcel, (int)ePermission.mView);
            mKiemKe_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_HoaChat,
                                               (int)oKhoaDuocEx.mKiemKe_HoaChat_XemIn, (int)ePermission.mView);

            mBaoCaoXuat_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuat_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoXuat_Thuoc_XemIn, (int)ePermission.mView);
            mBaoCaoXuat_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuat_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoXuat_Thuoc_XemIn, (int)ePermission.mView);
            mBaoCaoXuat_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuat_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoXuat_Thuoc_XemIn, (int)ePermission.mView);

            mBaoCaoXuatNhapTon_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuatNhapTon_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoXuatNhapTon_Thuoc_XemIn, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_Thuoc_KetChuyen = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuatNhapTon_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoXuatNhapTon_Thuoc_KetChuyen, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuatNhapTon_YCu,
                                               (int)oKhoaDuocEx.mBaoCaoXuatNhapTon_YCu_XemIn, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_YCu_KetChuyen = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuatNhapTon_YCu,
                                               (int)oKhoaDuocEx.mBaoCaoXuatNhapTon_YCu_KetChuyen, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuatNhapTon_HoaChat,
                                               (int)oKhoaDuocEx.mBaoCaoXuatNhapTon_HoaChat_XemIn, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_HoaChat_KetChuyen = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuatNhapTon_HoaChat,
                                               (int)oKhoaDuocEx.mBaoCaoXuatNhapTon_HoaChat_KetChuyen, (int)ePermission.mView);

            mBaoCaoTheKho_Thuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoTheKho_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoTheKho_Thuoc_Xem, (int)ePermission.mView);
            mBaoCaoTheKho_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoTheKho_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoTheKho_Thuoc_In, (int)ePermission.mView);

            mBaoCaoTheKho_YCu_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoTheKho_YCu,
                                               (int)oKhoaDuocEx.mBaoCaoTheKho_YCu_Xem, (int)ePermission.mView);
            mBaoCaoTheKho_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoTheKho_YCu,
                                               (int)oKhoaDuocEx.mBaoCaoTheKho_YCu_In, (int)ePermission.mView);

            mBaoCaoTheKho_HoaChat_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoTheKho_HoaChat,
                                               (int)oKhoaDuocEx.mBaoCaoTheKho_HoaChat_Xem, (int)ePermission.mView);
            mBaoCaoTheKho_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoTheKho_HoaChat,
                                               (int)oKhoaDuocEx.mBaoCaoTheKho_HoaChat_In, (int)ePermission.mView);


            mQLNCCNSX_NCC_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNCCNSX_NCC,
                                               (int)oKhoaDuocEx.mQLNCCNSX_NCC_Tim, (int)ePermission.mView);
            mQLNCCNSX_NCC_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNCCNSX_NCC,
                                               (int)oKhoaDuocEx.mQLNCCNSX_NCC_ThemMoi, (int)ePermission.mView);
            mQLNCCNSX_NCC_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNCCNSX_NCC,
                                               (int)oKhoaDuocEx.mQLNCCNSX_NCC_ChinhSua, (int)ePermission.mView);

            mQLNCCNSX_NSX_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNCCNSX_NSX,
                                               (int)oKhoaDuocEx.mQLNCCNSX_NSX_Tim, (int)ePermission.mView);
            mQLNCCNSX_NSX_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNCCNSX_NSX,
                                               (int)oKhoaDuocEx.mQLNCCNSX_NSX_ThemMoi, (int)ePermission.mView);
            mQLNCCNSX_NSX_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNCCNSX_NSX,
                                               (int)oKhoaDuocEx.mQLNCCNSX_NSX_ChinhSua, (int)ePermission.mView);


            mQLNCCNSX_NCCNSX_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNCCNSX_NCCNSX,
                                               (int)oKhoaDuocEx.mQLNCCNSX_NCCNSX_Tim, (int)ePermission.mView);
            mQLNCCNSX_NCCNSX_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNCCNSX_NCCNSX,
                                               (int)oKhoaDuocEx.mQLNCCNSX_NCCNSX_ThemMoi, (int)ePermission.mView);
            mQLNCCNSX_NCCNSX_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNCCNSX_NCCNSX,
                                               (int)oKhoaDuocEx.mQLNCCNSX_NCCNSX_ChinhSua, (int)ePermission.mView);



            mQLDanhMuc_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLDanhMuc_Thuoc,
                                               (int)oKhoaDuocEx.mQLDanhMuc_Thuoc_Tim, (int)ePermission.mView);
            mQLDanhMuc_Thuoc_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLDanhMuc_Thuoc,
                                               (int)oKhoaDuocEx.mQLDanhMuc_Thuoc_ThemMoi, (int)ePermission.mView);
            mQLDanhMuc_Thuoc_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLDanhMuc_Thuoc,
                                               (int)oKhoaDuocEx.mQLDanhMuc_Thuoc_ChinhSua, (int)ePermission.mView);


            mQLDanhMuc_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLDanhMuc_YCu,
                                               (int)oKhoaDuocEx.mQLDanhMuc_YCu_Tim, (int)ePermission.mView);
            mQLDanhMuc_YCu_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLDanhMuc_YCu,
                                               (int)oKhoaDuocEx.mQLDanhMuc_YCu_ThemMoi, (int)ePermission.mView);
            mQLDanhMuc_YCu_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLDanhMuc_YCu,
                                               (int)oKhoaDuocEx.mQLDanhMuc_YCu_ChinhSua, (int)ePermission.mView);

            mQLDanhMuc_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLDanhMuc_HoaChat,
                                               (int)oKhoaDuocEx.mQLDanhMuc_HoaChat_Tim, (int)ePermission.mView);
            mQLDanhMuc_HoaChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLDanhMuc_HoaChat,
                                               (int)oKhoaDuocEx.mQLDanhMuc_HoaChat_ThemMoi, (int)ePermission.mView);
            mQLDanhMuc_HoaChat_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLDanhMuc_HoaChat,
                                               (int)oKhoaDuocEx.mQLDanhMuc_HoaChat_ChinhSua, (int)ePermission.mView);



            mQLNhomHang_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                   , (int)eKhoaDuoc.mQLNhomHang_Thuoc,
                                   (int)oKhoaDuocEx.mQLNhomHang_Thuoc_Tim, (int)ePermission.mView);
            mQLNhomHang_Thuoc_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_Thuoc,
                                               (int)oKhoaDuocEx.mQLNhomHang_Thuoc_ThemMoi, (int)ePermission.mView);
            mQLNhomHang_Thuoc_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_Thuoc,
                                               (int)oKhoaDuocEx.mQLNhomHang_Thuoc_ChinhSua, (int)ePermission.mView);

            mQLNhomHang_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_YCu,
                                               (int)oKhoaDuocEx.mQLNhomHang_YCu_Tim, (int)ePermission.mView);
            mQLNhomHang_YCu_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_YCu,
                                               (int)oKhoaDuocEx.mQLNhomHang_YCu_ThemMoi, (int)ePermission.mView);
            mQLNhomHang_YCu_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_YCu,
                                               (int)oKhoaDuocEx.mQLNhomHang_YCu_ChinhSua, (int)ePermission.mView);

            mQLNhomHang_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_HoaChat,
                                               (int)oKhoaDuocEx.mQLNhomHang_HoaChat_Tim, (int)ePermission.mView);
            mQLNhomHang_HoaChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_HoaChat,
                                               (int)oKhoaDuocEx.mQLNhomHang_HoaChat_ThemMoi, (int)ePermission.mView);
            mQLNhomHang_HoaChat_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_HoaChat,
                                               (int)oKhoaDuocEx.mQLNhomHang_HoaChat_ChinhSua, (int)ePermission.mView);
            /*▼====: #005*/
            mQLNhomHang_HoatChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                              , (int)eKhoaDuoc.mQLNhomHang_HoatChat,
                                              (int)oKhoaDuocEx.mQLNhomHang_HoatChat_Tim, (int)ePermission.mView);
            mQLNhomHang_HoatChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_HoatChat,
                                               (int)oKhoaDuocEx.mQLNhomHang_HoatChat_ThemMoi, (int)ePermission.mView);
            mQLNhomHang_HoatChat_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mQLNhomHang_HoatChat,
                                               (int)oKhoaDuocEx.mQLNhomHang_HoatChat_ChinhSua, (int)ePermission.mView);
            /*▲====: #005*/



            mGiaTuNCC_Thuoc_SuaGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_Thuoc,
                                               (int)oKhoaDuocEx.mGiaTuNCC_Thuoc_SuaGia, (int)ePermission.mView);
            mGiaTuNCC_Thuoc_TaoGiaMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_Thuoc,
                                               (int)oKhoaDuocEx.mGiaTuNCC_Thuoc_TaoGiaMoi, (int)ePermission.mView);
            mGiaTuNCC_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_Thuoc,
                                               (int)oKhoaDuocEx.mGiaTuNCC_Thuoc_Tim, (int)ePermission.mView)
                                               ;
            mGiaTuNCC_Thuoc_QuanLy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_Thuoc,
                                               (int)oKhoaDuocEx.mGiaTuNCC_Thuoc_QuanLy, (int)ePermission.mView)
                                               || mGiaTuNCC_Thuoc_SuaGia || mGiaTuNCC_Thuoc_TaoGiaMoi;


            mGiaTuNCC_YCu_SuaGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_YCu,
                                               (int)oKhoaDuocEx.mGiaTuNCC_YCu_SuaGia, (int)ePermission.mView);
            mGiaTuNCC_YCu_TaoGiaMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_YCu,
                                               (int)oKhoaDuocEx.mGiaTuNCC_YCu_TaoGiaMoi, (int)ePermission.mView);
            mGiaTuNCC_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_YCu,
                                               (int)oKhoaDuocEx.mGiaTuNCC_YCu_Tim, (int)ePermission.mView);
            mGiaTuNCC_YCu_QuanLy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_YCu,
                                               (int)oKhoaDuocEx.mGiaTuNCC_YCu_QuanLy, (int)ePermission.mView)
                                               || mGiaTuNCC_YCu_SuaGia || mGiaTuNCC_YCu_TaoGiaMoi;


            mGiaTuNCC_HoaChat_SuaGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_HoaChat,
                                               (int)oKhoaDuocEx.mGiaTuNCC_HoaChat_SuaGia, (int)ePermission.mView);
            mGiaTuNCC_HoaChat_TaoGiaMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_HoaChat,
                                               (int)oKhoaDuocEx.mGiaTuNCC_HoaChat_TaoGiaMoi, (int)ePermission.mView);
            mGiaTuNCC_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_HoaChat,
                                               (int)oKhoaDuocEx.mGiaTuNCC_HoaChat_Tim, (int)ePermission.mView);
            mGiaTuNCC_HoaChat_QuanLy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaTuNCC_HoaChat,
                                               (int)oKhoaDuocEx.mGiaTuNCC_HoaChat_QuanLy, (int)ePermission.mView)
                                               || mGiaTuNCC_HoaChat_SuaGia || mGiaTuNCC_HoaChat_TaoGiaMoi;


            mThangGiaBan_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mThangGiaBan_Thuoc_Tim, (int)ePermission.mView);
            mThangGiaBan_Thuoc_TaoMoiCTGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mThangGiaBan_Thuoc_TaoMoiCTGia, (int)ePermission.mView);
            mThangGiaBan_Thuoc_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mThangGiaBan_Thuoc_ChinhSua, (int)ePermission.mView);

            mThangGiaBan_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_YCu,
                                               (int)oKhoaDuocEx.mThangGiaBan_YCu_Tim, (int)ePermission.mView);
            mThangGiaBan_YCu_TaoMoiCTGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_YCu,
                                               (int)oKhoaDuocEx.mThangGiaBan_YCu_TaoMoiCTGia, (int)ePermission.mView);
            mThangGiaBan_YCu_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_YCu,
                                               (int)oKhoaDuocEx.mThangGiaBan_YCu_ChinhSua, (int)ePermission.mView);

            mThangGiaBan_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mThangGiaBan_HoaChat_Tim, (int)ePermission.mView);
            mThangGiaBan_HoaChat_TaoMoiCTGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mThangGiaBan_HoaChat_TaoMoiCTGia, (int)ePermission.mView);
            mThangGiaBan_HoaChat_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mThangGiaBan_HoaChat_ChinhSua, (int)ePermission.mView);

            mGiaBan_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mGiaBan_Thuoc_Tim, (int)ePermission.mView);
            mGiaBan_Thuoc_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mGiaBan_Thuoc_ChinhSua, (int)ePermission.mView);
            mGiaBan_Thuoc_XemDSGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mGiaBan_Thuoc_XemDSGia, (int)ePermission.mView);
            mGiaBan_Thuoc_TaoGiaMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mGiaBan_Thuoc_TaoGiaMoi, (int)ePermission.mView);
            mGiaBan_Thuoc_TaoGiaMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mGiaBan_Thuoc_ChinhSuaGia, (int)ePermission.mView);


            mGiaBan_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_YCu,
                                               (int)oKhoaDuocEx.mGiaBan_YCu_Tim, (int)ePermission.mView);
            mGiaBan_YCu_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_YCu,
                                               (int)oKhoaDuocEx.mGiaBan_YCu_ChinhSua, (int)ePermission.mView);
            mGiaBan_YCu_XemDSGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_YCu,
                                               (int)oKhoaDuocEx.mGiaBan_YCu_XemDSGia, (int)ePermission.mView);
            mGiaBan_YCu_TaoGiaMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_YCu,
                                               (int)oKhoaDuocEx.mGiaBan_YCu_TaoGiaMoi, (int)ePermission.mView);
            mGiaBan_YCu_ChinhSuaGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_YCu,
                                               (int)oKhoaDuocEx.mGiaBan_YCu_ChinhSuaGia, (int)ePermission.mView);


            mGiaBan_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mGiaBan_HoaChat_Tim, (int)ePermission.mView);
            mGiaBan_HoaChat_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mGiaBan_HoaChat_ChinhSua, (int)ePermission.mView);
            mGiaBan_HoaChat_XemDSGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mGiaBan_HoaChat_XemDSGia, (int)ePermission.mView);
            mGiaBan_HoaChat_TaoGiaMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mGiaBan_HoaChat_TaoGiaMoi, (int)ePermission.mView);
            mGiaBan_HoaChat_ChinhSuaGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mGiaBan_HoaChat_ChinhSuaGia, (int)ePermission.mView);


            mBangGiaBan_Thuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mBangGiaBan_Thuoc_Xem, (int)ePermission.mView);
            mBangGiaBan_Thuoc_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mBangGiaBan_Thuoc_ChinhSua, (int)ePermission.mView);
            mBangGiaBan_Thuoc_TaoBangGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mBangGiaBan_Thuoc_TaoBangGia, (int)ePermission.mView);
            mBangGiaBan_Thuoc_PreView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mBangGiaBan_Thuoc_PreView, (int)ePermission.mView);
            mBangGiaBan_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_Thuoc,
                                               (int)oKhoaDuocEx.mBangGiaBan_Thuoc_In, (int)ePermission.mView);

            mBangGiaBan_YCu_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_YCu,
                                               (int)oKhoaDuocEx.mBangGiaBan_YCu_Xem, (int)ePermission.mView);
            mBangGiaBan_YCu_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_YCu,
                                               (int)oKhoaDuocEx.mBangGiaBan_YCu_ChinhSua, (int)ePermission.mView);
            mBangGiaBan_YCu_TaoBangGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_YCu,
                                               (int)oKhoaDuocEx.mBangGiaBan_YCu_TaoBangGia, (int)ePermission.mView);
            mBangGiaBan_YCu_PreView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_YCu,
                                               (int)oKhoaDuocEx.mBangGiaBan_YCu_PreView, (int)ePermission.mView);
            mBangGiaBan_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_YCu,
                                               (int)oKhoaDuocEx.mBangGiaBan_YCu_In, (int)ePermission.mView);

            mBangGiaBan_HoaChat_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mBangGiaBan_HoaChat_Xem, (int)ePermission.mView);
            mBangGiaBan_HoaChat_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mBangGiaBan_HoaChat_ChinhSua, (int)ePermission.mView);
            mBangGiaBan_HoaChat_TaoBangGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mBangGiaBan_HoaChat_TaoBangGia, (int)ePermission.mView);
            mBangGiaBan_HoaChat_PreView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mBangGiaBan_HoaChat_PreView, (int)ePermission.mView);
            mBangGiaBan_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBangGiaBan_HoaChat,
                                               (int)oKhoaDuocEx.mBangGiaBan_HoaChat_In, (int)ePermission.mView);

            //them nua ne
            mPhieuDeNghiThanhToan_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_Thuoc_Tim, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_Thuoc_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_Thuoc_ThemMoi, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_Thuoc_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_Thuoc_ChinhSua, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_Thuoc_XemInBK = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_Thuoc_XemInBK, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_Thuoc_XemInPDNTT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_Thuoc,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_Thuoc_XemInPDNTT, (int)ePermission.mView);

            mPhieuDeNghiThanhToan_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDNThanhToan_YCu_Tim, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_YCu_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_YCu_ThemMoi, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_YCu_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_YCu_ChinhSua, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_YCu_XemInBK = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_YCu_XemInBK, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_YCu_XemInPDNTT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_YCu,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_YCu_XemInPDNTT, (int)ePermission.mView);

            mPhieuDeNghiThanhToan_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_HoaChat_Tim, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_HoaChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_HoaChat_ThemMoi, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_HoaChat_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_HoaChat_ChinhSua, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_HoaChat_XemInBK = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_HoaChat_XemInBK, (int)ePermission.mView);
            mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mPhieuDNThanhToan_HoaChat,
                                               (int)oKhoaDuocEx.mPhieuDeNghiThanhToan_HoaChat_XemInPDNTT, (int)ePermission.mView);


            mBaoCaoDSPhieuNhapKho_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoDSPhieuNhapKho_Thuoc_XemIn, (int)ePermission.mView);
            mBaoCaoDSPhieuNhapKho_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoDSPhieuNhapKho_Thuoc_In, (int)ePermission.mView);

            mBaoCaoDSPhieuNhapKho_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_YCu,
                                               (int)oKhoaDuocEx.mBaoCaoDSPhieuNhapKho_YCu_XemIn, (int)ePermission.mView);
            mBaoCaoDSPhieuNhapKho_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_YCu,
                                               (int)oKhoaDuocEx.mBaoCaoDSPhieuNhapKho_YCu_In, (int)ePermission.mView);

            mBaoCaoDSPhieuNhapKho_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_HoaChat,
                                               (int)oKhoaDuocEx.mBaoCaoDSPhieuNhapKho_HoaChat_XemIn, (int)ePermission.mView);
            mBaoCaoDSPhieuNhapKho_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_HoaChat,
                                               (int)oKhoaDuocEx.mBaoCaoDSPhieuNhapKho_HoaChat_In, (int)ePermission.mView);


            mBaoCaoSuDung_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoSuDung_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoSuDung_Thuoc_XemIn, (int)ePermission.mView);
            mBaoCaoSuDung_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoSuDung_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoSuDung_Thuoc_In, (int)ePermission.mView);


            mBaoCaoNhapXuatDenKhoaPhong_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoNhapXuatDenKhoaPhong_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoNhapXuatDenKhoaPhong_Thuoc_In, (int)ePermission.mView);
            mBaoCaoNhapXuatDenKhoaPhong_Thuoc_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoNhapXuatDenKhoaPhong_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoNhapXuatDenKhoaPhong_Thuoc_XuatExcel, (int)ePermission.mView);

            mBaoCaoNhapXuatDenKhoaPhong_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoNhapXuatDenKhoaPhong_YCu,
                                               (int)oKhoaDuocEx.mBaoCaoNhapXuatDenKhoaPhong_YCu_In, (int)ePermission.mView);
            mBaoCaoNhapXuatDenKhoaPhong_YCu_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoNhapXuatDenKhoaPhong_YCu,
                                               (int)oKhoaDuocEx.mBaoCaoNhapXuatDenKhoaPhong_YCu_XuatExcel, (int)ePermission.mView);

            mBaoCaoNhapXuatDenKhoaPhong_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoNhapXuatDenKhoaPhong_HoaChat,
                                               (int)oKhoaDuocEx.mBaoCaoNhapXuatDenKhoaPhong_HoaChat_In, (int)ePermission.mView);
            mBaoCaoNhapXuatDenKhoaPhong_HoaChat_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoNhapXuatDenKhoaPhong_HoaChat,
                                               (int)oKhoaDuocEx.mBaoCaoNhapXuatDenKhoaPhong_HoaChat_XuatExcel, (int)ePermission.mView);


            //them moi ngay 28-07-2012
            mBaoCaoTheoDoiCongNo_Thuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoTheoDoiCongNo_Thuoc,
                                               (int)oKhoaDuocEx.mBaoCaoTheoDoiCongNo_Thuoc_Xem, (int)ePermission.mView);
            mBaoCaoTheoDoiCongNo_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mBaoCaoTheoDoiCongNo_Thuoc,
                                                   (int)oKhoaDuocEx.mBaoCaoTheoDoiCongNo_Thuoc_In, (int)ePermission.mView);

            mBaoCaoTheoDoiCongNo_YCu_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mBaoCaoTheoDoiCongNo_YCu,
                                                   (int)oKhoaDuocEx.mBaoCaoTheoDoiCongNo_YCu_Xem, (int)ePermission.mView);
            mBaoCaoTheoDoiCongNo_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mBaoCaoTheoDoiCongNo_YCu,
                                                   (int)oKhoaDuocEx.mBaoCaoTheoDoiCongNo_YCu_In, (int)ePermission.mView);

            mBaoCaoTheoDoiCongNo_HoaChat_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mBaoCaoTheoDoiCongNo_HoaChat,
                                                   (int)oKhoaDuocEx.mBaoCaoTheoDoiCongNo_HoaChat_Xem, (int)ePermission.mView);
            mBaoCaoTheoDoiCongNo_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mBaoCaoTheoDoiCongNo_HoaChat,
                                                   (int)oKhoaDuocEx.mBaoCaoTheoDoiCongNo_HoaChat_In, (int)ePermission.mView);


            #region Group Function Duyệt Phiếu Lãnh Hàng

            mDuyetPhieuLinhHang_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_Thuoc,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_Thuoc_Tim, (int)ePermission.mView);
            mDuyetPhieuLinhHang_Thuoc_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_Thuoc,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_Thuoc_PhieuMoi, (int)ePermission.mView);
            mDuyetPhieuLinhHang_Thuoc_XuatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_Thuoc,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_Thuoc_XuatHang, (int)ePermission.mView);
            mDuyetPhieuLinhHang_Thuoc_XemInTH = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_Thuoc,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_Thuoc_XemInTH, (int)ePermission.mView);
            mDuyetPhieuLinhHang_Thuoc_XemInCT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_Thuoc,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_Thuoc_XemInCT, (int)ePermission.mView);

            mDuyetPhieuLinhHang_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_YCu,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_YCu_Tim, (int)ePermission.mView);
            mDuyetPhieuLinhHang_YCu_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_YCu,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_YCu_PhieuMoi, (int)ePermission.mView);
            mDuyetPhieuLinhHang_YCu_XuatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_YCu,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_YCu_XuatHang, (int)ePermission.mView);
            mDuyetPhieuLinhHang_YCu_XemInTH = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_YCu,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_YCu_XemInTH, (int)ePermission.mView);
            mDuyetPhieuLinhHang_YCu_XemInCT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_YCu,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_YCu_XemInCT, (int)ePermission.mView);

            mDuyetPhieuLinhHang_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_HoaChat,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_HoaChat_Tim, (int)ePermission.mView);
            mDuyetPhieuLinhHang_HoaChat_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_HoaChat,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_HoaChat_PhieuMoi, (int)ePermission.mView);
            mDuyetPhieuLinhHang_HoaChat_XuatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_HoaChat,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_HoaChat_XuatHang, (int)ePermission.mView);
            mDuyetPhieuLinhHang_HoaChat_XemInTH = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_HoaChat,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_HoaChat_XemInTH, (int)ePermission.mView);
            mDuyetPhieuLinhHang_HoaChat_XemInCT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_HoaChat,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_HoaChat_XemInCT, (int)ePermission.mView);
            #endregion

            mXuatHangKyGui_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_Thuoc_Tim, (int)ePermission.mView);
            mXuatHangKyGui_Thuoc_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_Thuoc_PhieuMoi, (int)ePermission.mView);
            mXuatHangKyGui_Thuoc_Save = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_Thuoc_Save, (int)ePermission.mView);
            mXuatHangKyGui_Thuoc_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_Thuoc_ThuTien, (int)ePermission.mView);
            mXuatHangKyGui_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_Thuoc_XemIn, (int)ePermission.mView);
            mXuatHangKyGui_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_Thuoc_In, (int)ePermission.mView);
            mXuatHangKyGui_Thuoc_DeleteInvoice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_Thuoc_DeleteInvoice, (int)ePermission.mView);
            mXuatHangKyGui_Thuoc_PrintReceipt = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_Thuoc_PrintReceipt, (int)ePermission.mView);

            mXuatHangKyGui_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_YCu_Tim, (int)ePermission.mView);
            mXuatHangKyGui_YCu_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_YCu_PhieuMoi, (int)ePermission.mView);
            mXuatHangKyGui_YCu_Save = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_YCu_Save, (int)ePermission.mView);
            mXuatHangKyGui_YCu_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_YCu_ThuTien, (int)ePermission.mView);
            mXuatHangKyGui_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_YCu_XemIn, (int)ePermission.mView);
            mXuatHangKyGui_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_YCu_In, (int)ePermission.mView);
            mXuatHangKyGui_YCu_DeleteInvoice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_YCu_DeleteInvoice, (int)ePermission.mView);
            mXuatHangKyGui_YCu_PrintReceipt = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_YCu_PrintReceipt, (int)ePermission.mView);

            mXuatHangKyGui_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_HoaChat_Tim, (int)ePermission.mView);
            mXuatHangKyGui_HoaChat_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_HoaChat_PhieuMoi, (int)ePermission.mView);
            mXuatHangKyGui_HoaChat_Save = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_HoaChat_Save, (int)ePermission.mView);
            mXuatHangKyGui_HoaChat_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_HoaChat_ThuTien, (int)ePermission.mView);
            mXuatHangKyGui_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_HoaChat_XemIn, (int)ePermission.mView);
            mXuatHangKyGui_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_HoaChat_In, (int)ePermission.mView);
            mXuatHangKyGui_HoaChat_DeleteInvoice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_HoaChat_DeleteInvoice, (int)ePermission.mView);
            mXuatHangKyGui_HoaChat_PrintReceipt = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mXuatHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mXuatHangKyGui_HoaChat_PrintReceipt, (int)ePermission.mView);

            mSapNhapHangKyGui_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_Thuoc_Tim, (int)ePermission.mView);
            mSapNhapHangKyGui_Thuoc_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_Thuoc_PhieuMoi, (int)ePermission.mView);
            mSapNhapHangKyGui_Thuoc_CapNhat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_Thuoc_CapNhat, (int)ePermission.mView);
            mSapNhapHangKyGui_Thuoc_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_Thuoc_Xoa, (int)ePermission.mView);
            mSapNhapHangKyGui_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_Thuoc_XemIn, (int)ePermission.mView);
            mSapNhapHangKyGui_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_Thuoc,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_Thuoc_In, (int)ePermission.mView);

            mSapNhapHangKyGui_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_YCu_Tim, (int)ePermission.mView);
            mSapNhapHangKyGui_YCu_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_YCu_PhieuMoi, (int)ePermission.mView);
            mSapNhapHangKyGui_YCu_CapNhat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_YCu_CapNhat, (int)ePermission.mView);
            mSapNhapHangKyGui_YCu_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_YCu_Xoa, (int)ePermission.mView);
            mSapNhapHangKyGui_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_YCu_XemIn, (int)ePermission.mView);
            mSapNhapHangKyGui_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_YCu,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_YCu_In, (int)ePermission.mView);

            mSapNhapHangKyGui_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_HoaChat_Tim, (int)ePermission.mView);
            mSapNhapHangKyGui_HoaChat_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_HoaChat_PhieuMoi, (int)ePermission.mView);
            mSapNhapHangKyGui_HoaChat_CapNhat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_HoaChat_CapNhat, (int)ePermission.mView);
            mSapNhapHangKyGui_HoaChat_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_HoaChat_Xoa, (int)ePermission.mView);
            mSapNhapHangKyGui_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_HoaChat_XemIn, (int)ePermission.mView);
            mSapNhapHangKyGui_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mSapNhapHangKyGui_HoaChat,
                                                   (int)oKhoaDuocEx.mSapNhapHangKyGui_HoaChat_In, (int)ePermission.mView);


            mNhapTraTuKhoPhong_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_Thuoc,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_Thuoc_Tim, (int)ePermission.mView);
            mNhapTraTuKhoPhong_Thuoc_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_Thuoc,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_Thuoc_PhieuMoi, (int)ePermission.mView);
            mNhapTraTuKhoPhong_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_Thuoc,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_Thuoc_XemIn, (int)ePermission.mView);
            mNhapTraTuKhoPhong_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_Thuoc,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_Thuoc_In, (int)ePermission.mView);

            mNhapTraTuKhoPhong_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_YCu,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_YCu_Tim, (int)ePermission.mView);
            mNhapTraTuKhoPhong_YCu_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_YCu,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_YCu_PhieuMoi, (int)ePermission.mView);
            mNhapTraTuKhoPhong_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_YCu,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_YCu_XemIn, (int)ePermission.mView);
            mNhapTraTuKhoPhong_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_YCu,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_YCu_In, (int)ePermission.mView);

            mNhapTraTuKhoPhong_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_HoaChat,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_HoaChat_Tim, (int)ePermission.mView);
            mNhapTraTuKhoPhong_HoaChat_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_HoaChat,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_HoaChat_PhieuMoi, (int)ePermission.mView);
            mNhapTraTuKhoPhong_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_HoaChat,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_HoaChat_XemIn, (int)ePermission.mView);
            mNhapTraTuKhoPhong_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_HoaChat,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_HoaChat_In, (int)ePermission.mView);

            mWatchMedOutQty_Preview = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mWatchMedCmd,
                                                   (int)oKhoaDuocEx.mWatchMedOutQty_Preview, (int)ePermission.mView);
            mWatchMedOutQty_Print = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mWatchMedCmd,
                                                   (int)oKhoaDuocEx.mWatchMedOutQty_Print, (int)ePermission.mView);

            mWatchMatOutQty_Preview = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mWatchMatCmd,
                                                   (int)oKhoaDuocEx.mWatchMatOutQty_Preview, (int)ePermission.mView);
            mWatchMatOutQty_Print = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mWatchMatCmd,
                                                   (int)oKhoaDuocEx.mWatchMatOutQty_Print, (int)ePermission.mView);

            mWatchLabOutQty_Preview = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                       , (int)eKhoaDuoc.mWatchLabCmd,
                                       (int)oKhoaDuocEx.mWatchLabOutQty_Preview, (int)ePermission.mView);
            mWatchLabOutQty_Print = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mWatchLabCmd,
                                                   (int)oKhoaDuocEx.mWatchLabOutQty_Print, (int)ePermission.mView);
            //▼====== #019            
            mNhapHangNCC_DinhDuong_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_DinhDuong,
                                               (int)oKhoaDuocEx.mNhapHangNCC_DinhDuong_Tim, (int)ePermission.mView);
            mNhapHangNCC_DinhDuong_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_DinhDuong,
                                               (int)oKhoaDuocEx.mNhapHangNCC_DinhDuong_ThemMoi, (int)ePermission.mView);
            mNhapHangNCC_DinhDuong_CapNhat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_DinhDuong,
                                               (int)oKhoaDuocEx.mNhapHangNCC_DinhDuong_CapNhat, (int)ePermission.mView);
            mNhapHangNCC_DinhDuong_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mNhapHangNCC_DinhDuong,
                                               (int)oKhoaDuocEx.mNhapHangNCC_DinhDuong_In, (int)ePermission.mView);

            mXuat_DinhDuong_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_DinhDuong,
                                               (int)oKhoaDuocEx.mXuat_DinhDuong_Tim, (int)ePermission.mView);
            mXuat_DinhDuong_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_DinhDuong,
                                               (int)oKhoaDuocEx.mXuat_DinhDuong_PhieuMoi, (int)ePermission.mView);
            mXuat_DinhDuong_ThucHien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_DinhDuong,
                                               (int)oKhoaDuocEx.mXuat_DinhDuong_ThucHien, (int)ePermission.mView);
            mXuat_DinhDuong_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_DinhDuong,
                                               (int)oKhoaDuocEx.mXuat_DinhDuong_ThuTien, (int)ePermission.mView);
            mXuat_DinhDuong_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_DinhDuong,
                                               (int)oKhoaDuocEx.mXuat_DinhDuong_In, (int)ePermission.mView);
            mXuat_DinhDuong_DeleteInvoice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_DinhDuong,
                                               (int)oKhoaDuocEx.mXuat_DinhDuong_DeleteInvoice, (int)ePermission.mView);
            mXuat_DinhDuong_PrintReceipt = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mXuat_DinhDuong,
                                               (int)oKhoaDuocEx.mXuat_DinhDuong_PrintReceipt, (int)ePermission.mView);

            mDuyetPhieuLinhHang_DDuong_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_DinhDuong,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_DinhDuong_Tim, (int)ePermission.mView);
            mDuyetPhieuLinhHang_DDuong_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_DinhDuong,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_DinhDuong_PhieuMoi, (int)ePermission.mView);
            mDuyetPhieuLinhHang_DDuong_XuatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_DinhDuong,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_DinhDuong_XuatHang, (int)ePermission.mView);
            mDuyetPhieuLinhHang_DDuong_XemInTH = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_DinhDuong,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_DinhDuong_XemInTH, (int)ePermission.mView);
            mDuyetPhieuLinhHang_DDuong_XemInCT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mDuyetPhieuLinhHang_DinhDuong,
                                                   (int)oKhoaDuocEx.mDuyetPhieuLinhHang_DinhDuong_XemInCT, (int)ePermission.mView);

            mNhapTraTuKhoPhong_DDuong_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_DDuong,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_DDuong_Tim, (int)ePermission.mView);
            mNhapTraTuKhoPhong_DDuong_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_DDuong,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_DDuong_PhieuMoi, (int)ePermission.mView);
            mNhapTraTuKhoPhong_DDuong_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_DDuong,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_DDuong_XemIn, (int)ePermission.mView);
            mNhapTraTuKhoPhong_DDuong_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                                   , (int)eKhoaDuoc.mNhapTraTuKhoPhong_DDuong,
                                                   (int)oKhoaDuocEx.mNhapTraTuKhoPhong_DDuong_In, (int)ePermission.mView);

            mBaoCaoXuatNhapTon_DDuong_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuatNhapTon_DDuong,
                                               (int)oKhoaDuocEx.mBaoCaoXuatNhapTon_DDuong_XemIn, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_DDuong_KetChuyen = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoXuatNhapTon_DDuong,
                                               (int)oKhoaDuocEx.mBaoCaoXuatNhapTon_DDuong_KetChuyen, (int)ePermission.mView);

            mBaoCaoTheKho_DDuong_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoTheKho_DDuong,
                                               (int)oKhoaDuocEx.mBaoCaoTheKho_DDuong_Xem, (int)ePermission.mView);
            mBaoCaoTheKho_DDuong_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mBaoCaoTheKho_DDuong,
                                               (int)oKhoaDuocEx.mBaoCaoTheKho_DDuong_In, (int)ePermission.mView);

            mKiemKe_DDuong_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_DDuong,
                                               (int)oKhoaDuocEx.mKiemKe_DDuong_Tim, (int)ePermission.mView);
            mKiemKe_DDuong_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_DDuong,
                                               (int)oKhoaDuocEx.mKiemKe_DDuong_ThemMoi, (int)ePermission.mView);
            mKiemKe_DDuong_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_DDuong,
                                               (int)oKhoaDuocEx.mKiemKe_DDuong_XuatExcel, (int)ePermission.mView);
            mKiemKe_DDuong_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mKiemKe_DDuong,
                                               (int)oKhoaDuocEx.mKiemKe_DDuong_XemIn, (int)ePermission.mView);

            mDuTru_DDuong_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_DDuong,
                                               (int)oKhoaDuocEx.mDuTru_DDuong_Tim, (int)ePermission.mView);
            mDuTru_DDuong_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_DDuong,
                                               (int)oKhoaDuocEx.mDuTru_DDuong_ThemMoi, (int)ePermission.mView);
            mDuTru_DDuong_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_DDuong,
                                               (int)oKhoaDuocEx.mDuTru_DDuong_Xoa, (int)ePermission.mView);
            mDuTru_DDuong_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mDuTru_DDuong,
                                               (int)oKhoaDuocEx.mDuTru_DDuong_XemIn, (int)ePermission.mView);

            mThangGiaBan_DinhDuong_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_DDuong,
                                               (int)oKhoaDuocEx.mThangGiaBan_DDuong_Tim, (int)ePermission.mView);
            mThangGiaBan_DinhDuong_TaoMoiCTGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_DDuong,
                                               (int)oKhoaDuocEx.mThangGiaBan_DDuong_TaoMoiCTGia, (int)ePermission.mView);
            mThangGiaBan_DinhDuong_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                               , (int)eKhoaDuoc.mThangGiaBan_DDuong,
                                               (int)oKhoaDuocEx.mThangGiaBan_DDuong_ChinhSua, (int)ePermission.mView);
            //▲====== #019
        }



        #endregion
        #endregion

        #region Approve Request member

        private void RequestNewThuocCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IRequestNew>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mDuyetPhieuLinhHang_Tim = mDuyetPhieuLinhHang_Thuoc_Tim;
            UnitVM.mDuyetPhieuLinhHang_PhieuMoi = mDuyetPhieuLinhHang_Thuoc_PhieuMoi;
            UnitVM.mDuyetPhieuLinhHang_XuatHang = mDuyetPhieuLinhHang_Thuoc_XuatHang;
            UnitVM.mDuyetPhieuLinhHang_XemInTH = mDuyetPhieuLinhHang_Thuoc_XemInTH;
            UnitVM.mDuyetPhieuLinhHang_XemInCT = mDuyetPhieuLinhHang_Thuoc_XemInCT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void RequestNewThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0732_G1_DuyetPhLinhThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestNewThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestNewThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RequestNewYCuCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IRequestNew>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mDuyetPhieuLinhHang_Tim = mDuyetPhieuLinhHang_YCu_Tim;
            UnitVM.mDuyetPhieuLinhHang_PhieuMoi = mDuyetPhieuLinhHang_YCu_PhieuMoi;
            UnitVM.mDuyetPhieuLinhHang_XuatHang = mDuyetPhieuLinhHang_YCu_XuatHang;
            UnitVM.mDuyetPhieuLinhHang_XemInTH = mDuyetPhieuLinhHang_YCu_XemInTH;
            UnitVM.mDuyetPhieuLinhHang_XemInCT = mDuyetPhieuLinhHang_YCu_XemInCT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void RequestNewYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0735_G1_DuyetPhLinhYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestNewYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestNewYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RequestNewHoaChatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IRequestNew>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mDuyetPhieuLinhHang_Tim = mDuyetPhieuLinhHang_HoaChat_Tim;
            UnitVM.mDuyetPhieuLinhHang_PhieuMoi = mDuyetPhieuLinhHang_HoaChat_PhieuMoi;
            UnitVM.mDuyetPhieuLinhHang_XuatHang = mDuyetPhieuLinhHang_HoaChat_XuatHang;
            UnitVM.mDuyetPhieuLinhHang_XemInTH = mDuyetPhieuLinhHang_HoaChat_XemInTH;
            UnitVM.mDuyetPhieuLinhHang_XemInCT = mDuyetPhieuLinhHang_HoaChat_XemInCT;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void RequestNewHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0736_G1_DuyetPhLinhHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestNewHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestNewHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ApprovedRequestCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IRequestNew>();
            UnitVM.V_MedProductType = V_MedProductType;
            UnitVM.strHienThi = Globals.TitleForm;
            //Không set quyền button của các phân mới duyệt phiếu mới
            //UnitVM.mDuyetPhieuLinhHang_Tim = mDuyetPhieuLinhHang_HoaChat_Tim;
            //UnitVM.mDuyetPhieuLinhHang_PhieuMoi = mDuyetPhieuLinhHang_HoaChat_PhieuMoi;
            //UnitVM.mDuyetPhieuLinhHang_XuatHang = mDuyetPhieuLinhHang_HoaChat_XuatHang;
            //UnitVM.mDuyetPhieuLinhHang_XemInTH = mDuyetPhieuLinhHang_HoaChat_XemInTH;
            //UnitVM.mDuyetPhieuLinhHang_XemInCT = mDuyetPhieuLinhHang_HoaChat_XemInCT;
            bool roleButton = mDuyetPhieuLinhHang_Blood || mDuyetPhieuLinhHang_HoaChat || mDuyetPhieuLinhHang_ThanhTrung
                || mDuyetPhieuLinhHang_Vaccine || mDuyetPhieuLinhHang_VPP || mDuyetPhieuLinhHang_VTTH || mDuyetPhieuLinhHang_VTYTTH;
            UnitVM.mDuyetPhieuLinhHang_Tim = roleButton;
            UnitVM.mDuyetPhieuLinhHang_PhieuMoi = roleButton;
            UnitVM.mDuyetPhieuLinhHang_XuatHang = roleButton;
            UnitVM.mDuyetPhieuLinhHang_XemInTH = roleButton;
            UnitVM.mDuyetPhieuLinhHang_XemInCT = roleButton;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void ApprovedVTYTTH(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2483_G1_DuyetPhLinhVTYTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void ApprovedVaccine(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2483_G1_DuyetPhLinhVacXin.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void ApprovedBloods(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2483_G1_DuyetPhLinhMau.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void ApprovedResouces(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2548_G1_DuyetPhLinhVTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void ApprovedThanhTrung(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2537_G1_DuyetLinhThanhTrung.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void ApprovedVPP(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2483_G1_DuyetPhLinhVPP.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ApprovedRequestCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Estimation member
        private void EstimationDrugDeptCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mDuTru_Thuoc_Tim;
            UnitVM.mThemMoi = mDuTru_Thuoc_ThemMoi;
            UnitVM.mXoa = mDuTru_Thuoc_Xoa;
            UnitVM.mXemIn = mDuTru_Thuoc_XemIn;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void EstimationDrugDeptCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K3922_G1_DuTruThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationDrugDeptCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationDrugDeptCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EstimationDrugDeptByBidCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDeptByBid>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mDuTru_Thuoc_Tim;
            UnitVM.mThemMoi = mDuTru_Thuoc_ThemMoi;
            UnitVM.mXoa = mDuTru_Thuoc_Xoa;
            UnitVM.mXemIn = mDuTru_Thuoc_XemIn;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void EstimationDrugDeptByBidCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K3922_G1_DuTruThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationDrugDeptByBidCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationDrugDeptByBidCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EstimationYCuCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mDuTru_YCu_Tim;
            UnitVM.mThemMoi = mDuTru_YCu_ThemMoi;
            UnitVM.mXoa = mDuTru_YCu_Xoa;
            UnitVM.mXemIn = mDuTru_YCu_XemIn;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void EstimationYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0737_G1_DuTruYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EstimationYCuByBidCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDeptByBid>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mDuTru_YCu_Tim;
            UnitVM.mThemMoi = mDuTru_YCu_ThemMoi;
            UnitVM.mXoa = mDuTru_YCu_Xoa;
            UnitVM.mXemIn = mDuTru_YCu_XemIn;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void EstimationYCuByBidCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0737_G1_DuTruYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationYCuByBidCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationYCuByBidCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EstimationChemicalCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = Globals.TitleForm;

            UnitVM.mTim = mDuTru_HoaChat_Tim;
            UnitVM.mThemMoi = mDuTru_HoaChat_ThemMoi;
            UnitVM.mXoa = mDuTru_HoaChat_Xoa;
            UnitVM.mXemIn = mDuTru_HoaChat_XemIn;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EstimationChemicalCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0738_G1_DuTruHCHat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationChemicalCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationChemicalCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▼====== #013
        private void EstimationVTYTTHCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            UnitVM.strHienThi = Globals.TitleForm;

            //UnitVM.mTim = mDuTru_HoaChat_Tim;
            //UnitVM.mThemMoi = mDuTru_HoaChat_ThemMoi;
            //UnitVM.mXoa = mDuTru_HoaChat_Xoa;
            //UnitVM.mXemIn = mDuTru_HoaChat_XemIn;
            UnitVM.mTim = gfDuTru;
            UnitVM.mThemMoi = gfDuTru;
            UnitVM.mXoa = gfDuTru;
            UnitVM.mXemIn = gfDuTru;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EstimationVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2695_G1_DuTruVTYTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationVTYTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationVTYTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EstimationVaccineCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            UnitVM.strHienThi = Globals.TitleForm;

            //UnitVM.mTim = mDuTru_HoaChat_Tim;
            //UnitVM.mThemMoi = mDuTru_HoaChat_ThemMoi;
            //UnitVM.mXoa = mDuTru_HoaChat_Xoa;
            //UnitVM.mXemIn = mDuTru_HoaChat_XemIn;
            UnitVM.mTim = gfDuTru;
            UnitVM.mThemMoi = gfDuTru;
            UnitVM.mXoa = gfDuTru;
            UnitVM.mXemIn = gfDuTru;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EstimationVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2696_G1_DuTruVaccine.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationVaccineCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationVaccineCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EstimationBloodCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            UnitVM.strHienThi = Globals.TitleForm;

            //UnitVM.mTim = mDuTru_HoaChat_Tim;
            //UnitVM.mThemMoi = mDuTru_HoaChat_ThemMoi;
            //UnitVM.mXoa = mDuTru_HoaChat_Xoa;
            //UnitVM.mXemIn = mDuTru_HoaChat_XemIn;
            UnitVM.mTim = gfDuTru;
            UnitVM.mThemMoi = gfDuTru;
            UnitVM.mXoa = gfDuTru;
            UnitVM.mXemIn = gfDuTru;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EstimationBloodCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2697_G1_DuTruBlood.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationBloodCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationBloodCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EstimationVPPCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationFromRequest>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            UnitVM.strHienThi = Globals.TitleForm;

            //UnitVM.mTim = mDuTru_HoaChat_Tim;
            //UnitVM.mThemMoi = mDuTru_HoaChat_ThemMoi;
            //UnitVM.mXoa = mDuTru_HoaChat_Xoa;
            //UnitVM.mXemIn = mDuTru_HoaChat_XemIn;
            UnitVM.mTim = gfDuTru;
            UnitVM.mThemMoi = gfDuTru;
            UnitVM.mXoa = gfDuTru;
            UnitVM.mXemIn = gfDuTru;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EstimationVPPCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2698_G1_DuTruVPP.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationVPPCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationVPPCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void EstimationVTTHCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationFromRequest>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            UnitVM.strHienThi = Globals.TitleForm;

            //UnitVM.mTim = mDuTru_HoaChat_Tim;
            //UnitVM.mThemMoi = mDuTru_HoaChat_ThemMoi;
            //UnitVM.mXoa = mDuTru_HoaChat_Xoa;
            //UnitVM.mXemIn = mDuTru_HoaChat_XemIn;
            UnitVM.mTim = gfDuTru;
            UnitVM.mThemMoi = gfDuTru;
            UnitVM.mXoa = gfDuTru;
            UnitVM.mXemIn = gfDuTru;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EstimationVTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2699_G1_DuTruVTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationVTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationVTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void EstimationThanhTrungCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            UnitVM.strHienThi = Globals.TitleForm;

            //UnitVM.mTim = mDuTru_HoaChat_Tim;
            //UnitVM.mThemMoi = mDuTru_HoaChat_ThemMoi;
            //UnitVM.mXoa = mDuTru_HoaChat_Xoa;
            //UnitVM.mXemIn = mDuTru_HoaChat_XemIn;
            UnitVM.mTim = gfDuTru;
            UnitVM.mThemMoi = gfDuTru;
            UnitVM.mXoa = gfDuTru;
            UnitVM.mXemIn = gfDuTru;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EstimationThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2700_G1_DuTruThanhTrung.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationThanhTrungCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationThanhTrungCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #013
        #endregion

        #region Xuat Member

        private void XNBThuocCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mXuat_Thuoc_Tim;
            UnitVM.mPhieuMoi = mXuat_Thuoc_PhieuMoi;
            UnitVM.mThucHien = mXuat_Thuoc_ThucHien;
            UnitVM.mThuTien = mXuat_Thuoc_ThuTien;
            UnitVM.mIn = mXuat_Thuoc_In;
            UnitVM.mDeleteInvoice = mXuat_Thuoc_DeleteInvoice;
            UnitVM.mPrintReceipt = mXuat_Thuoc_PrintReceipt;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0739_G1_XuatThuocTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBYCuCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mXuat_YCu_Tim;
            UnitVM.mPhieuMoi = mXuat_YCu_PhieuMoi;
            UnitVM.mThucHien = mXuat_YCu_ThucHien;
            UnitVM.mThuTien = mXuat_YCu_ThuTien;
            UnitVM.mIn = mXuat_YCu_In;
            UnitVM.mDeleteInvoice = mXuat_YCu_DeleteInvoice;
            UnitVM.mPrintReceipt = mXuat_YCu_PrintReceipt;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0740_G1_XuatYCuTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBHoaChatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mXuat_HoaChat_Tim;
            UnitVM.mPhieuMoi = mXuat_HoaChat_PhieuMoi;
            UnitVM.mThucHien = mXuat_HoaChat_ThucHien;
            UnitVM.mThuTien = mXuat_HoaChat_ThuTien;
            UnitVM.mIn = mXuat_HoaChat_In;
            UnitVM.mDeleteInvoice = mXuat_HoaChat_DeleteInvoice;
            UnitVM.mPrintReceipt = mXuat_HoaChat_PrintReceipt;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0741_G1_XuatHChatTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBVTYTTHCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mTim = mXuat_YCu_Tim;
            //UnitVM.mPhieuMoi = mXuat_YCu_PhieuMoi;
            //UnitVM.mThucHien = mXuat_YCu_ThucHien;
            //UnitVM.mThuTien = mXuat_YCu_ThuTien;
            //UnitVM.mIn = mXuat_YCu_In;
            //UnitVM.mDeleteInvoice = mXuat_YCu_DeleteInvoice;
            //UnitVM.mPrintReceipt = mXuat_YCu_PrintReceipt;
            UnitVM.mTim = gfXuatNoiBo;
            UnitVM.mPhieuMoi = gfXuatNoiBo;
            UnitVM.mThucHien = gfXuatNoiBo;
            UnitVM.mThuTien = gfXuatNoiBo;
            UnitVM.mIn = gfXuatNoiBo;
            UnitVM.mDeleteInvoice = gfXuatNoiBo;
            UnitVM.mPrintReceipt = gfXuatNoiBo;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2469_G1_XuatVTYTTHTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBVTYTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBVTYTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBVaccineCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mTim = mXuat_HoaChat_Tim;
            //UnitVM.mPhieuMoi = mXuat_HoaChat_PhieuMoi;
            //UnitVM.mThucHien = mXuat_HoaChat_ThucHien;
            //UnitVM.mThuTien = mXuat_HoaChat_ThuTien;
            //UnitVM.mIn = mXuat_HoaChat_In;
            //UnitVM.mDeleteInvoice = mXuat_HoaChat_DeleteInvoice;
            //UnitVM.mPrintReceipt = mXuat_HoaChat_PrintReceipt;
            UnitVM.mTim = gfXuatNoiBo;
            UnitVM.mPhieuMoi = gfXuatNoiBo;
            UnitVM.mThucHien = gfXuatNoiBo;
            UnitVM.mThuTien = gfXuatNoiBo;
            UnitVM.mIn = gfXuatNoiBo;
            UnitVM.mDeleteInvoice = gfXuatNoiBo;
            UnitVM.mPrintReceipt = gfXuatNoiBo;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2476_G1_XuatVaccineTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBVaccineCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBVaccineCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBBloodCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mTim = mXuat_HoaChat_Tim;
            //UnitVM.mPhieuMoi = mXuat_HoaChat_PhieuMoi;
            //UnitVM.mThucHien = mXuat_HoaChat_ThucHien;
            //UnitVM.mThuTien = mXuat_HoaChat_ThuTien;
            //UnitVM.mIn = mXuat_HoaChat_In;
            //UnitVM.mDeleteInvoice = mXuat_HoaChat_DeleteInvoice;
            //UnitVM.mPrintReceipt = mXuat_HoaChat_PrintReceipt;
            UnitVM.mTim = gfXuatNoiBo;
            UnitVM.mPhieuMoi = gfXuatNoiBo;
            UnitVM.mThucHien = gfXuatNoiBo;
            UnitVM.mThuTien = gfXuatNoiBo;
            UnitVM.mIn = gfXuatNoiBo;
            UnitVM.mDeleteInvoice = gfXuatNoiBo;
            UnitVM.mPrintReceipt = gfXuatNoiBo;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBBloodCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2488_G1_XuatMauTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBBloodCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBBloodCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBThanhTrungCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mTim = mXuat_YCu_Tim;
            //UnitVM.mPhieuMoi = mXuat_YCu_PhieuMoi;
            //UnitVM.mThucHien = mXuat_YCu_ThucHien;
            //UnitVM.mThuTien = mXuat_YCu_ThuTien;
            //UnitVM.mIn = mXuat_YCu_In;
            //UnitVM.mDeleteInvoice = mXuat_YCu_DeleteInvoice;
            //UnitVM.mPrintReceipt = mXuat_YCu_PrintReceipt;
            UnitVM.mTim = gfXuatNoiBo;
            UnitVM.mPhieuMoi = gfXuatNoiBo;
            UnitVM.mThucHien = gfXuatNoiBo;
            UnitVM.mThuTien = gfXuatNoiBo;
            UnitVM.mIn = gfXuatNoiBo;
            UnitVM.mDeleteInvoice = gfXuatNoiBo;
            UnitVM.mPrintReceipt = gfXuatNoiBo;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2494_G1_XuatThanhTrungTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBThanhTrungCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBThanhTrungCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBVPPCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mTim = mXuat_YCu_Tim;
            //UnitVM.mPhieuMoi = mXuat_YCu_PhieuMoi;
            //UnitVM.mThucHien = mXuat_YCu_ThucHien;
            //UnitVM.mThuTien = mXuat_YCu_ThuTien;
            //UnitVM.mIn = mXuat_YCu_In;
            //UnitVM.mDeleteInvoice = mXuat_YCu_DeleteInvoice;
            //UnitVM.mPrintReceipt = mXuat_YCu_PrintReceipt;
            UnitVM.mTim = gfXuatNoiBo;
            UnitVM.mPhieuMoi = gfXuatNoiBo;
            UnitVM.mThucHien = gfXuatNoiBo;
            UnitVM.mThuTien = gfXuatNoiBo;
            UnitVM.mIn = gfXuatNoiBo;
            UnitVM.mDeleteInvoice = gfXuatNoiBo;
            UnitVM.mPrintReceipt = gfXuatNoiBo;
            UnitVM.IsEstimationFromRequest = true;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBVPPCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2522_G1_XuatVPPTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBVPPCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBVPPCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBVTTHCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mTim = mXuat_YCu_Tim;
            //UnitVM.mPhieuMoi = mXuat_YCu_PhieuMoi;
            //UnitVM.mThucHien = mXuat_YCu_ThucHien;
            //UnitVM.mThuTien = mXuat_YCu_ThuTien;
            //UnitVM.mIn = mXuat_YCu_In;
            //UnitVM.mDeleteInvoice = mXuat_YCu_DeleteInvoice;
            //UnitVM.mPrintReceipt = mXuat_YCu_PrintReceipt;
            UnitVM.mTim = gfXuatNoiBo;
            UnitVM.mPhieuMoi = gfXuatNoiBo;
            UnitVM.mThucHien = gfXuatNoiBo;
            UnitVM.mThuTien = gfXuatNoiBo;
            UnitVM.mIn = gfXuatNoiBo;
            UnitVM.mDeleteInvoice = gfXuatNoiBo;
            UnitVM.mPrintReceipt = gfXuatNoiBo;
            UnitVM.IsEstimationFromRequest = true;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBVTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2523_G1_XuatVTTHTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBVTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBVTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReturnFromInwardSupplierCmd_In(long V_MedProductType)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptInwardDrugSupplier>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.mTim = mNhapHangNCC_Thuoc_Tim;
            VM.mThemMoi = mNhapHangNCC_Thuoc_ThemMoi;
            VM.mCapNhat = mNhapHangNCC_Thuoc_CapNhat;
            VM.mIn = mNhapHangNCC_Thuoc_In;
            VM.IsRetundView = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void ReturnDrugFromInwardSupplierCmd(object source)
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2602_G1_XuatTraChoNCC.ToUpper(), eHCMSResources.G0787_G1_Thuoc.ToUpper());
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReturnFromInwardSupplierCmd_In((long)AllLookupValues.MedProductType.THUOC);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReturnFromInwardSupplierCmd_In((long)AllLookupValues.MedProductType.THUOC);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void ReturnLabFromInwardSupplierCmd(object source)
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2602_G1_XuatTraChoNCC.ToUpper(), eHCMSResources.G2907_G1_YCu.ToUpper());
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReturnFromInwardSupplierCmd_In((long)AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReturnFromInwardSupplierCmd_In((long)AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Xuất theo toa

        private void XuatThuocTheoToaCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IOutwardFromPrescription>();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XuatThuocTheoToaCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0742_G1_XuatThuocTheoToa.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatThuocTheoToaCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatThuocTheoToaCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SellPrescriptionDrugCmdHI_In(object source)
        {

            var module = Globals.GetViewModel<IDrugModule>();
            var RefGenDrugVM = Globals.GetViewModel<IPrescription>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            RefGenDrugVM.IsHIOutPt = true;
            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }
        public void SellPrescriptionDrugCmdHI(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2038_G1_BanThuocTheoToaBH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SellPrescriptionDrugCmdHI_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SellPrescriptionDrugCmdHI_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Tra Hang member

        private void ReturnThuocCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReturn>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mTraHang_Thuoc_Tim;
            UnitVM.mLuu = mTraHang_Thuoc_Luu;
            UnitVM.mTraTien = mTraHang_Thuoc_TraTien;
            UnitVM.mIn = mTraHang_Thuoc_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ReturnThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G1666_G1_TraThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReturnThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReturnThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReturnYCuCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReturn>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mTraHang_YCu_Tim;
            UnitVM.mLuu = mTraHang_YCu_Luu;
            UnitVM.mTraTien = mTraHang_YCu_TraTien;
            UnitVM.mIn = mTraHang_YCu_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ReturnYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0743_G1_TraYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReturnYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReturnYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReturnHoaChatCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReturn>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = Globals.TitleForm;

            UnitVM.mTim = mTraHang_HoaChat_Tim;
            UnitVM.mLuu = mTraHang_HoaChat_Luu;
            UnitVM.mTraTien = mTraHang_HoaChat_TraTien;
            UnitVM.mIn = mTraHang_HoaChat_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ReturnHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0744_G1_TraHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReturnHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReturnHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion

        #region Demage Member
        private void DemageThuocCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptDamageExpiryDrug>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;

            UnitVM.mTim = mXuatHuy_Thuoc_Tim;
            UnitVM.mThemMoi = mXuatHuy_Thuoc_ThemMoi;
            UnitVM.mXuatExcel = mXuatHuy_Thuoc_XuatExcel;
            UnitVM.mXemIn = mXuatHuy_Thuoc_XemIn;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DemageThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0745_G1_HuyThuocTaiKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DemageThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DemageThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DemageYCuCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptDamageExpiryDrug>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            //UnitVM.InitializeInvoice();
            UnitVM.strHienThi = Globals.TitleForm;

            UnitVM.mTim = mXuatHuy_YCu_Tim;
            UnitVM.mThemMoi = mXuatHuy_YCu_ThemMoi;
            UnitVM.mXuatExcel = mXuatHuy_YCu_XuatExcel;
            UnitVM.mXemIn = mXuatHuy_YCu_XemIn;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DemageYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0746_G1_HuyYCuTaiKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DemageYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DemageYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DemageHoaChatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptDamageExpiryDrug>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            //UnitVM.InitializeInvoice();
            UnitVM.strHienThi = Globals.TitleForm;

            UnitVM.mTim = mXuatHuy_HoaChat_Tim;
            UnitVM.mThemMoi = mXuatHuy_HoaChat_ThemMoi;
            UnitVM.mXuatExcel = mXuatHuy_HoaChat_XuatExcel;
            UnitVM.mXemIn = mXuatHuy_HoaChat_XemIn;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DemageHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0747_G1_HuyHChatTaiKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DemageHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DemageHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Danh Muc Member

        private void SupplierProductCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<ISupplierProduct>();
            UnitVM.mTim = mQLNCCNSX_NCC_Tim;
            UnitVM.mThemMoi = mQLNCCNSX_NCC_ThemMoi;
            UnitVM.mChinhSua = mQLNCCNSX_NCC_ChinhSua;
            UnitVM.ExportFor = (long)AllLookupValues.SupplierDrugDeptPharmOthers.DRUGDEPT;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void SupplierProductCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0748_G1_KhDuocQLyNCC.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SupplierProductCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SupplierProductCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DrugDeptPharmaceulCompanyCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptPharmacieucalCompany>();

            UnitVM.mTim = mQLNCCNSX_NSX_Tim;
            UnitVM.mThemMoi = mQLNCCNSX_NSX_ThemMoi;
            UnitVM.mChinhSua = mQLNCCNSX_NSX_ChinhSua;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DrugDeptPharmaceulCompanyCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0749_G1_KhDuocQLyNSX.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptPharmaceulCompanyCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptPharmaceulCompanyCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DrugDeptPharmaceulCompanySupplierCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptPharmacieucalAndSupplier>();

            UnitVM.mTim = mQLNCCNSX_NCCNSX_Tim;
            UnitVM.mThemMoi = mQLNCCNSX_NCCNSX_ThemMoi;
            UnitVM.mChinhSua = mQLNCCNSX_NCCNSX_ChinhSua;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DrugDeptPharmaceulCompanySupplierCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0750_G1_KhDuocQLyNCCNSX.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptPharmaceulCompanySupplierCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptPharmaceulCompanySupplierCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenMedProductDetails_DrugMgnt_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.G1230_G1_TimKiemThuoc;
            VM.TextButtonThemMoi = eHCMSResources.Z0459_G1_ThemMoiThuoc;
            VM.TextDanhSach = eHCMSResources.K3080_G1_DSThuoc;

            VM.mTim = mQLDanhMuc_Thuoc_Tim;
            VM.mThemMoi = mQLDanhMuc_Thuoc_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_Thuoc_ChinhSua;

            VM.dgColumnExtOfThuoc_Visible = Visibility.Visible;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_DrugMgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K2906_G1_DMucThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_DrugMgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_DrugMgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenMedProductDetails_MedicalDevicesMgnt_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z0678_G1_TimKiemYCu;
            VM.TextButtonThemMoi = eHCMSResources.Z0679_G1_ThemMoiYCu;
            VM.TextDanhSach = eHCMSResources.Z0657_G1_DSYCu;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mQLDanhMuc_YCu_Tim;
            VM.mThemMoi = mQLDanhMuc_YCu_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_YCu_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_MedicalDevicesMgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K2917_G1_DMucYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_MedicalDevicesMgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_MedicalDevicesMgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenMedProductDetails_ChemicalMgnt_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z0680_G1_TimKIemHChat;
            VM.TextButtonThemMoi = eHCMSResources.Z0681_G1_ThemMoiHChat;
            VM.TextDanhSach = eHCMSResources.Z0658_G1_DSHChat;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mQLDanhMuc_HoaChat_Tim;
            VM.mThemMoi = mQLDanhMuc_HoaChat_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_HoaChat_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_ChemicalMgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K2895_G1_DMucHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_ChemicalMgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_ChemicalMgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenMedProductDetails_VTYTTH_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z2455_G1_TKiemVTYTTH;
            VM.TextButtonThemMoi = eHCMSResources.Z2456_G1_ThemMoiVTYTTH;
            VM.TextDanhSach = eHCMSResources.Z2457_G1_DSachVTYTTH;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mQLDanhMuc_YCu_Tim;
            VM.mThemMoi = mQLDanhMuc_YCu_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_YCu_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_VTYTTH(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2454_G1_DMucVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_VTYTTH_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_VTYTTH_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenMedProductDetails_TiemNgua_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z2461_G1_TKiemTiemNgua;
            VM.TextButtonThemMoi = eHCMSResources.Z2462_G1_ThemMoiTiemNgua;
            VM.TextDanhSach = eHCMSResources.Z2463_G1_DSachTiemNgua;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mQLDanhMuc_HoaChat_Tim;
            VM.mThemMoi = mQLDanhMuc_HoaChat_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_HoaChat_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_TiemNgua(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2460_G1_DMucTiemNgua;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_TiemNgua_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_TiemNgua_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenMedProductDetails_Mau_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z2490_G1_TKiemMau;
            VM.TextButtonThemMoi = eHCMSResources.Z2491_G1_ThemMoiMau;
            VM.TextDanhSach = eHCMSResources.Z2492_G1_DSachMau;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mQLDanhMuc_HoaChat_Tim;
            VM.mThemMoi = mQLDanhMuc_HoaChat_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_HoaChat_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_Mau(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2489_G1_DMucMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_Mau_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_Mau_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void RefGenMedProductDetails_ThanhTrung_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z2496_G1_TKiemThanhTrung;
            VM.TextButtonThemMoi = eHCMSResources.Z2497_G1_ThemMoiThanhTrung;
            VM.TextDanhSach = eHCMSResources.Z2498_G1_DSachThanhTrung;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mQLDanhMuc_YCu_Tim;
            VM.mThemMoi = mQLDanhMuc_YCu_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_YCu_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_ThanhTrung(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2495_G1_DMucThanhTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_ThanhTrung_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_ThanhTrung_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void RefGenMedProductDetails_VPP_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z2528_G1_TKiemVPP;
            VM.TextButtonThemMoi = eHCMSResources.Z2529_G1_ThemMoiVPP;
            VM.TextDanhSach = eHCMSResources.Z2524_G1_DSachVPP;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mQLDanhMuc_YCu_Tim;
            VM.mThemMoi = mQLDanhMuc_YCu_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_YCu_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_VPP(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2526_G1_DMucVPP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_VPP_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_VPP_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void RefGenMedProductDetails_VTTH_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z2530_G1_TKiemVTTH;
            VM.TextButtonThemMoi = eHCMSResources.Z2531_G1_ThemMoiVTTH;
            VM.TextDanhSach = eHCMSResources.Z2525_G1_DSachVTTH;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mQLDanhMuc_YCu_Tim;
            VM.mThemMoi = mQLDanhMuc_YCu_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_YCu_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_VTTH(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2527_G1_DMucVTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_VTTH_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_VTTH_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        /// <summary>
        /// Author: VuTTM
        /// </summary>
        /// <param name="source"></param>
        public void RefGenMedProductDetails_Nutrition(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3201_G1_DMucNutrition;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_Nutrition_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_Nutrition_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        /// <summary>
        /// Author: VuTTM
        /// </summary>
        /// <param name="source"></param>
        private void RefGenMedProductDetails_Nutrition_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z3203_G1_SearchNutrition;
            VM.TextButtonThemMoi = eHCMSResources.Z3204_G1_AddNutrition;
            VM.TextDanhSach = eHCMSResources.Z3202_G1_NutritionList;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mQLDanhMuc_Nutrition_Tim;
            VM.mThemMoi = mQLDanhMuc_Nutrition_ThemMoi;
            VM.mChinhSua = mQLDanhMuc_Nutrition_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        #endregion

        #region "Giá Thuốc Khoa Dược"
        //Giá Thuốc Từ NCC
        private void SupplierGenMedProductsPrice_Mgnt_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ISupplierGenMedProductsPrice_ListSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.TitleForm = Globals.TitleForm;

            VM.mTim = mGiaTuNCC_Thuoc_Tim;
            VM.mQuanLy = mGiaTuNCC_Thuoc_QuanLy;
            VM.mTaoGiaMoi = mGiaTuNCC_Thuoc_TaoGiaMoi;
            VM.mSuaGia = mGiaTuNCC_Thuoc_SuaGia;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void SupplierGenMedProductsPrice_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0751_G1_GiaThuocTuNCC;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SupplierGenMedProductsPrice_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SupplierGenMedProductsPrice_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Giá Y Cụ Từ NCC
        private void SupplierGenMedProductsPrice_Medical_Mgnt_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ISupplierGenMedProductsPrice_ListSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.TitleForm = Globals.TitleForm;

            VM.mTim = mGiaTuNCC_YCu_Tim;
            VM.mQuanLy = mGiaTuNCC_YCu_QuanLy;
            VM.mTaoGiaMoi = mGiaTuNCC_YCu_TaoGiaMoi;
            VM.mSuaGia = mGiaTuNCC_YCu_SuaGia;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void SupplierGenMedProductsPrice_Medical_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0752_G1_GiaYCuTuNCC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SupplierGenMedProductsPrice_Medical_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SupplierGenMedProductsPrice_Medical_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Giá Hóa Chất Từ NCC
        private void SupplierGenMedProductsPrice_Chemical_Mgnt_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ISupplierGenMedProductsPrice_ListSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.TitleForm = Globals.TitleForm;

            VM.mTim = mGiaTuNCC_HoaChat_Tim;
            VM.mQuanLy = mGiaTuNCC_HoaChat_QuanLy;
            VM.mTaoGiaMoi = mGiaTuNCC_HoaChat_TaoGiaMoi;
            VM.mSuaGia = mGiaTuNCC_HoaChat_SuaGia;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void SupplierGenMedProductsPrice_Chemical_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0753_G1_GiaHChatTuNCC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SupplierGenMedProductsPrice_Chemical_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SupplierGenMedProductsPrice_Chemical_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Giá Bán Thuốc
        private void DrugDeptSellingItemPrices_Mgnt_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugDeptSellingItemPrices_ListDrug>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.G1230_G1_TimKiemThuoc;
            VM.TextDanhSach = eHCMSResources.K3080_G1_DSThuoc;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Visible;

            VM.mTim = mGiaBan_Thuoc_Tim;
            VM.mChinhSua = mGiaBan_Thuoc_ChinhSua;
            VM.mXemDSGia = mGiaBan_Thuoc_XemDSGia;
            VM.mTaoGiaMoi = mGiaBan_Thuoc_TaoGiaMoi;
            VM.mChinhSuaGia = mGiaBan_Thuoc_ChinhSuaGia;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void DrugDeptSellingItemPrices_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.T0889_G1_GiaBanThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingItemPrices_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingItemPrices_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Giá Bán Y Cụ
        private void DrugDeptSellingItemPrices_Medical_Mgnt_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugDeptSellingItemPrices_ListDrug>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z0678_G1_TimKiemYCu;
            VM.TextDanhSach = eHCMSResources.Z0657_G1_DSYCu;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mGiaBan_YCu_Tim;
            VM.mChinhSua = mGiaBan_YCu_ChinhSua;
            VM.mXemDSGia = mGiaBan_YCu_XemDSGia;
            VM.mTaoGiaMoi = mGiaBan_YCu_TaoGiaMoi;
            VM.mChinhSuaGia = mGiaBan_YCu_ChinhSuaGia;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void DrugDeptSellingItemPrices_Medical_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0754_G1_GiaBanYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingItemPrices_Medical_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingItemPrices_Medical_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Giá Bán Hóa Chất
        private void DrugDeptSellingItemPrices_Chemical_Mgnt_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICMDrugDeptSellingItemPrices_ListDrug>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z0680_G1_TimKIemHChat;
            VM.TextDanhSach = eHCMSResources.Z0658_G1_DSHChat;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mGiaBan_HoaChat_Tim;
            VM.mChinhSua = mGiaBan_HoaChat_ChinhSua;
            VM.mXemDSGia = mGiaBan_HoaChat_XemDSGia;
            VM.mTaoGiaMoi = mGiaBan_HoaChat_TaoGiaMoi;
            VM.mChinhSuaGia = mGiaBan_HoaChat_ChinhSuaGia;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void DrugDeptSellingItemPrices_Chemical_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0755_G1_GiaBanHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingItemPrices_Chemical_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingItemPrices_Chemical_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Thang Giá Thuốc
        private void DrugDeptSellPriceProfitScale_Mgnt_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellPriceProfitScale>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.TitleForm = Globals.TitleForm;

            VM.mTim = mThangGiaBan_Thuoc_Tim;
            VM.mTaoMoiCTGia = mThangGiaBan_Thuoc_TaoMoiCTGia;
            VM.mChinhSua = mThangGiaBan_Thuoc_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellPriceProfitScale_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0756_G1_ThangGiaThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellPriceProfitScale_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellPriceProfitScale_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Thang Giá Y Cụ
        private void DrugDeptSellPriceProfitScale_Medical_Mgnt_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellPriceProfitScale>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.TitleForm = Globals.TitleForm;

            VM.mTim = mThangGiaBan_YCu_Tim;
            VM.mTaoMoiCTGia = mThangGiaBan_YCu_TaoMoiCTGia;
            VM.mChinhSua = mThangGiaBan_YCu_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellPriceProfitScale_Medical_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0757_G1_ThangGiaYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellPriceProfitScale_Medical_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellPriceProfitScale_Medical_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Thang Giá Hóa Chất
        private void DrugDeptSellPriceProfitScale_Chemical_Mgnt_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellPriceProfitScale>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.TitleForm = Globals.TitleForm;

            VM.mTim = mThangGiaBan_HoaChat_Tim;
            VM.mTaoMoiCTGia = mThangGiaBan_HoaChat_TaoMoiCTGia;
            VM.mChinhSua = mThangGiaBan_HoaChat_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellPriceProfitScale_Chemical_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0758_G1_ThangGiaHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellPriceProfitScale_Chemical_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellPriceProfitScale_Chemical_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Bảng Giá Thuốc
        private void DrugDeptSellingPriceList_Mgnt_In(object source)
        {

            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellingPriceList>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.TitleForm = Globals.TitleForm;

            VM.mXem = mBangGiaBan_Thuoc_Xem;
            VM.mChinhSua = mBangGiaBan_Thuoc_ChinhSua;
            VM.mTaoBangGia = mBangGiaBan_Thuoc_TaoBangGia;
            VM.mPreView = mBangGiaBan_Thuoc_PreView;
            VM.mIn = mBangGiaBan_Thuoc_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellingPriceList_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0700_G1_BGiaThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingPriceList_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingPriceList_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Bảng Giá Y Cụ
        private void DrugDeptSellingPriceList_Medical_Mgnt_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellingPriceList>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.TitleForm = Globals.TitleForm;

            VM.mXem = mBangGiaBan_YCu_Xem;
            VM.mChinhSua = mBangGiaBan_YCu_ChinhSua;
            VM.mTaoBangGia = mBangGiaBan_YCu_TaoBangGia;
            VM.mPreView = mBangGiaBan_YCu_PreView;
            VM.mIn = mBangGiaBan_YCu_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellingPriceList_Medical_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0701_G1_BGiaYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingPriceList_Medical_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingPriceList_Medical_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Bảng Giá Hóa Chất
        public void DrugDeptSellingPriceList_Chemical_Mgnt_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellingPriceList>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.TitleForm = Globals.TitleForm;

            VM.mXem = mBangGiaBan_HoaChat_Xem;
            VM.mChinhSua = mBangGiaBan_HoaChat_ChinhSua;
            VM.mTaoBangGia = mBangGiaBan_HoaChat_TaoBangGia;
            VM.mPreView = mBangGiaBan_HoaChat_PreView;
            VM.mIn = mBangGiaBan_HoaChat_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellingPriceList_Chemical_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0704_G1_BGiaHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingPriceList_Chemical_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingPriceList_Chemical_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //Bảng giá VTYTTH
        private void DrugDeptSellingPriceList_VTYTTH_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellingPriceList>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            VM.TitleForm = Globals.TitleForm;

            //VM.mXem = mBangGiaBan_YCu_Xem;
            //VM.mChinhSua = mBangGiaBan_YCu_ChinhSua;
            //VM.mTaoBangGia = mBangGiaBan_YCu_TaoBangGia;
            //VM.mPreView = mBangGiaBan_YCu_PreView;
            //VM.mIn = mBangGiaBan_YCu_In;
            VM.mXem = gfBangGiaBan;
            VM.mChinhSua = gfBangGiaBan;
            VM.mTaoBangGia = gfBangGiaBan;
            VM.mPreView = gfBangGiaBan;
            VM.mIn = gfBangGiaBan;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellingPriceList_VTYTTH(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2459_G1_BGiaVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingPriceList_VTYTTH_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingPriceList_VTYTTH_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //Bảng giá Tiêm ngừa
        private void DrugDeptSellingPriceList_TiemNgua_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellingPriceList>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            VM.TitleForm = Globals.TitleForm;

            //VM.mXem = mBangGiaBan_HoaChat_Xem;
            //VM.mChinhSua = mBangGiaBan_HoaChat_ChinhSua;
            //VM.mTaoBangGia = mBangGiaBan_HoaChat_TaoBangGia;
            //VM.mPreView = mBangGiaBan_HoaChat_PreView;
            //VM.mIn = mBangGiaBan_HoaChat_In;
            VM.mXem = gfBangGiaBan;
            VM.mChinhSua = gfBangGiaBan;
            VM.mTaoBangGia = gfBangGiaBan;
            VM.mPreView = gfBangGiaBan;
            VM.mIn = gfBangGiaBan;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellingPriceList_TiemNgua(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2465_G1_BGiaTiemNgua;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingPriceList_TiemNgua_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingPriceList_TiemNgua_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //Bảng giá Máu
        private void DrugDeptSellingPriceList_Mau_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellingPriceList>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            VM.TitleForm = Globals.TitleForm;

            //VM.mXem = mBangGiaBan_HoaChat_Xem;
            //VM.mChinhSua = mBangGiaBan_HoaChat_ChinhSua;
            //VM.mTaoBangGia = mBangGiaBan_HoaChat_TaoBangGia;
            //VM.mPreView = mBangGiaBan_HoaChat_PreView;
            //VM.mIn = mBangGiaBan_HoaChat_In;
            VM.mXem = gfBangGiaBan;
            VM.mChinhSua = gfBangGiaBan;
            VM.mTaoBangGia = gfBangGiaBan;
            VM.mPreView = gfBangGiaBan;
            VM.mIn = gfBangGiaBan;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellingPriceList_Mau(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2493_G1_BGiaMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingPriceList_Mau_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingPriceList_Mau_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //Bảng giá Thanh trùng
        private void DrugDeptSellingPriceList_ThanhTrung_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellingPriceList>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            VM.TitleForm = Globals.TitleForm;

            //VM.mXem = mBangGiaBan_YCu_Xem;
            //VM.mChinhSua = mBangGiaBan_YCu_ChinhSua;
            //VM.mTaoBangGia = mBangGiaBan_YCu_TaoBangGia;
            //VM.mPreView = mBangGiaBan_YCu_PreView;
            //VM.mIn = mBangGiaBan_YCu_In;
            VM.mXem = gfBangGiaBan;
            VM.mChinhSua = gfBangGiaBan;
            VM.mTaoBangGia = gfBangGiaBan;
            VM.mPreView = gfBangGiaBan;
            VM.mIn = gfBangGiaBan;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellingPriceList_ThanhTrung(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2493_G1_BGiaMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingPriceList_ThanhTrung_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingPriceList_ThanhTrung_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //Bảng giá VPP
        private void DrugDeptSellingPriceList_VPP_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellingPriceList>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            VM.TitleForm = Globals.TitleForm;

            //VM.mXem = mBangGiaBan_YCu_Xem;
            //VM.mChinhSua = mBangGiaBan_YCu_ChinhSua;
            //VM.mTaoBangGia = mBangGiaBan_YCu_TaoBangGia;
            //VM.mPreView = mBangGiaBan_YCu_PreView;
            //VM.mIn = mBangGiaBan_YCu_In;
            VM.mXem = gfBangGiaBan;
            VM.mChinhSua = gfBangGiaBan;
            VM.mTaoBangGia = gfBangGiaBan;
            VM.mPreView = gfBangGiaBan;
            VM.mIn = gfBangGiaBan;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellingPriceList_VPP(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2532_G1_BGiaVPP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingPriceList_VPP_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingPriceList_VPP_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //Bảng giá VTTH
        private void DrugDeptSellingPriceList_VTTH_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellingPriceList>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            VM.TitleForm = Globals.TitleForm;

            //VM.mXem = mBangGiaBan_YCu_Xem;
            //VM.mChinhSua = mBangGiaBan_YCu_ChinhSua;
            //VM.mTaoBangGia = mBangGiaBan_YCu_TaoBangGia;
            //VM.mPreView = mBangGiaBan_YCu_PreView;
            //VM.mIn = mBangGiaBan_YCu_In;
            VM.mXem = gfBangGiaBan;
            VM.mChinhSua = gfBangGiaBan;
            VM.mTaoBangGia = gfBangGiaBan;
            VM.mPreView = gfBangGiaBan;
            VM.mIn = gfBangGiaBan;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellingPriceList_VTTH(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2533_G1_BGiaVTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellingPriceList_VTTH_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellingPriceList_VTTH_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Order member
        //▼===== #014
        private void OrderProductCmd_In(object source, long V_MedProductType)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptPurchaseOrder>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.LoadOrderWarning();
            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                VM.mDSCanDatHang = mDatHang_Thuoc_DSCanDatHang;
                VM.mTim = mDatHang_Thuoc_Tim;
                VM.mChinhSua = mDatHang_Thuoc_ChinhSua;
                VM.mThemMoi = mDatHang_Thuoc_ThemMoi;
                VM.mIn = mDatHang_Thuoc_In;

            }
            else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            {
                VM.mDSCanDatHang = mDatHang_YCu_DSCanDatHang;
                VM.mTim = mDatHang_YCu_Tim;
                VM.mChinhSua = mDatHang_YCu_ChinhSua;
                VM.mThemMoi = mDatHang_YCu_ThemMoi;
                VM.mIn = mDatHang_YCu_In;
            }
            else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
            {
                VM.mDSCanDatHang = mDatHang_HoaChat_DSCanDatHang;
                VM.mTim = mDatHang_HoaChat_Tim;
                VM.mChinhSua = mDatHang_HoaChat_ChinhSua;
                VM.mThemMoi = mDatHang_HoaChat_ThemMoi;
                VM.mIn = mDatHang_HoaChat_In;
            }
            else if(V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM|| V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO)
            {
                VM.mDSCanDatHang = gfDatHang;
                VM.mTim = gfDatHang;
                VM.mChinhSua = gfDatHang;
                VM.mThemMoi = gfDatHang;
                VM.mIn = gfDatHang;
                VM.IsEstimateFromRequest = true;
            }
            else
            {
                VM.mDSCanDatHang = gfDatHang;
                VM.mTim = gfDatHang;
                VM.mChinhSua = gfDatHang;
                VM.mThemMoi = gfDatHang;
                VM.mIn = gfDatHang;
            }

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void OrderThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0759_G1_DHgChoThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.THUOC);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.THUOC);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void OrderYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0760_G1_DHgChoYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void OrderHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0761_G1_DHgChoHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.HOA_CHAT);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.HOA_CHAT);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        public void OrderVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2745_G1_DHgChoVTYTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void OrderVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2746_G1_DHgChoVaccine.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void OrderBloodCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2747_G1_DHgChoBlood.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void OrderVPPCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2748_G1_DHgChoVPP.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void OrderVTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2749_G1_DHgChoVTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void OrderThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2750_G1_DHgChoThanhTrung.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderProductCmd_In(source, (long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲===== #014
        #endregion

        #region Nhap Hang Member

        private void InwardDrugFromSupplierDrugCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptInwardDrugSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mNhapHangNCC_Thuoc_Tim;
            VM.mThemMoi = mNhapHangNCC_Thuoc_ThemMoi;
            VM.mCapNhat = mNhapHangNCC_Thuoc_CapNhat;
            VM.mIn = mNhapHangNCC_Thuoc_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierDrugCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0762_G1_NhapThuocTuNCC.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierDrugCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierDrugCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardDrugFromSupplierToVTYTTHCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IVTYTTHMedDeptInwardSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            VM.strHienThi = Globals.TitleForm;

            //VM.mTim = mNhapHangNCC_Thuoc_Tim;
            //VM.mThemMoi = mNhapHangNCC_Thuoc_ThemMoi;
            //VM.mCapNhat = mNhapHangNCC_Thuoc_CapNhat;
            //VM.mIn = mNhapHangNCC_Thuoc_In;
            VM.mTim = gf2NhapHangTuNCC;
            VM.mThemMoi = gf2NhapHangTuNCC;
            VM.mCapNhat = gf2NhapHangTuNCC;
            VM.mIn = gf2NhapHangTuNCC;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierToVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2453_G1_NhapThuocTuNCCVaoKhoVYTYTieuHao.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierToVTYTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierToVTYTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardFromSupplierToKhoTiemNguaCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ITiemNguaMedDeptInwardSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            VM.strHienThi = Globals.TitleForm;

            //VM.mTim = mNhapHangNCC_Thuoc_Tim;
            //VM.mThemMoi = mNhapHangNCC_Thuoc_ThemMoi;
            //VM.mCapNhat = mNhapHangNCC_Thuoc_CapNhat;
            //VM.mIn = mNhapHangNCC_Thuoc_In;
            VM.mTim = gf2NhapHangTuNCC;
            VM.mThemMoi = gf2NhapHangTuNCC;
            VM.mCapNhat = gf2NhapHangTuNCC;
            VM.mIn = gf2NhapHangTuNCC;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardFromSupplierToKhoTiemNguaCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2466_G1_NhapTuNCCVaoKhoTiemNgua.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromSupplierToKhoTiemNguaCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromSupplierToKhoTiemNguaCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardFromSupplierToChemicalDeptCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IChemicalMedDeptInwardSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;

            //VM.mTim = mNhapHangNCC_Thuoc_Tim;
            //VM.mThemMoi = mNhapHangNCC_Thuoc_ThemMoi;
            //VM.mCapNhat = mNhapHangNCC_Thuoc_CapNhat;
            //VM.mIn = mNhapHangNCC_Thuoc_In;
            VM.mTim = gf2NhapHangTuNCC;
            VM.mThemMoi = gf2NhapHangTuNCC;
            VM.mCapNhat = gf2NhapHangTuNCC;
            VM.mIn = gf2NhapHangTuNCC;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardFromSupplierToChemicalDeptCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2467_G1_NhapTuNCCVaoKhoHoaChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromSupplierToChemicalDeptCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromSupplierToChemicalDeptCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardDrugFromSupplierMedicalDeviceCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptInwardDrugSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mNhapHangNCC_YCu_Tim;
            VM.mThemMoi = mNhapHangNCC_YCu_ThemMoi;
            VM.mCapNhat = mNhapHangNCC_YCu_CapNhat;
            VM.mIn = mNhapHangNCC_YCu_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierMedicalDeviceCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0763_G1_NhapYCuTuNCC.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierMedicalDeviceCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierMedicalDeviceCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardDrugFromSupplierChemicalCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptInwardDrugSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mNhapHangNCC_HoaChat_Tim;
            VM.mThemMoi = mNhapHangNCC_HoaChat_ThemMoi;
            VM.mCapNhat = mNhapHangNCC_HoaChat_CapNhat;
            VM.mIn = mNhapHangNCC_HoaChat_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierChemicalCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0842_G1_NhapHChatTuNCC.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierChemicalCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierChemicalCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardDrugFromSupplierDrugClinicCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;
            VM.mNhapTraTuKhoPhong_Tim = mNhapTraTuKhoPhong_Thuoc_Tim;
            VM.mNhapTraTuKhoPhong_PhieuMoi = mNhapTraTuKhoPhong_Thuoc_Tim;
            VM.mNhapTraTuKhoPhong_XemIn = mNhapTraTuKhoPhong_Thuoc_Tim;
            VM.mNhapTraTuKhoPhong_In = mNhapTraTuKhoPhong_Thuoc_Tim;
            //▼====== #009
            VM.vNhapTraKhoBHYT = false;
            //▲====== #009
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierDrugClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0843_G1_NhapTraThuocTuKhPhg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierDrugClinicCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierDrugClinicCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardDrugFromSupplierMedicalDeviceClinicCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;
            VM.mNhapTraTuKhoPhong_Tim = mNhapTraTuKhoPhong_YCu_Tim;
            VM.mNhapTraTuKhoPhong_PhieuMoi = mNhapTraTuKhoPhong_YCu_Tim;
            VM.mNhapTraTuKhoPhong_XemIn = mNhapTraTuKhoPhong_YCu_Tim;
            VM.mNhapTraTuKhoPhong_In = mNhapTraTuKhoPhong_YCu_Tim;
            //▼====== #009
            VM.vNhapTraKhoBHYT = false;
            //▲====== #009
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierMedicalDeviceClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0764_G1_NhapTraYCuTuKhoPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierMedicalDeviceClinicCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierMedicalDeviceClinicCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardDrugFromSupplierChemicalClinicCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;
            VM.mNhapTraTuKhoPhong_Tim = mNhapTraTuKhoPhong_HoaChat_Tim;
            VM.mNhapTraTuKhoPhong_PhieuMoi = mNhapTraTuKhoPhong_HoaChat_Tim;
            VM.mNhapTraTuKhoPhong_XemIn = mNhapTraTuKhoPhong_HoaChat_Tim;
            VM.mNhapTraTuKhoPhong_In = mNhapTraTuKhoPhong_HoaChat_Tim;
            //▼====== #009
            VM.vNhapTraKhoBHYT = false;
            //▲====== #009
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierChemicalClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0765_G1_NhapTraHChatTuKhoPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierChemicalClinicCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierChemicalClinicCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardFromClinicDeptCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            //VM.mNhapTraTuKhoPhong_Tim = mNhapTraTuKhoPhong_HoaChat_Tim;
            //VM.mNhapTraTuKhoPhong_PhieuMoi = mNhapTraTuKhoPhong_HoaChat_Tim;
            //VM.mNhapTraTuKhoPhong_XemIn = mNhapTraTuKhoPhong_HoaChat_Tim;
            //VM.mNhapTraTuKhoPhong_In = mNhapTraTuKhoPhong_HoaChat_Tim;
            VM.mNhapTraTuKhoPhong_Tim = gf1NhapTraTuKhoPhong;
            VM.mNhapTraTuKhoPhong_PhieuMoi = gf1NhapTraTuKhoPhong;
            VM.mNhapTraTuKhoPhong_XemIn = gf1NhapTraTuKhoPhong;
            VM.mNhapTraTuKhoPhong_In = gf1NhapTraTuKhoPhong;
            //▼====== #009
            VM.vNhapTraKhoBHYT = false;
            //▲====== #009
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void InwardDrugFromSupplierVTYTTHClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2545_G1_NhapTraVTYTTHTuKhoPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void InwardDrugFromSupplierVaccinesClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2545_G1_NhapTraVacXinTuKhoPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void InwardDrugFromSupplierBloodsClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2545_G1_NhapTraMauTuKhoPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void InwardDrugFromSupplierVPPClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2545_G1_NhapTraVPPTuKhoPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void InwardDrugFromSupplierResourcesClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2556_G1_NhapTraVTTHTuKhoPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void InwardDrugFromSupplierThanhTrungClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2545_G1_NhapTraThanhTrungTuKhoPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromClinicDeptCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Nhap & Phan Bo Chi Phi Member

        private void CostThuocCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IInwardListCost>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mNhapPhanBoPhi_Thuoc_Tim;
            VM.mChinhSua_Them = mNhapPhanBoPhi_Thuoc_ChinhSua_Them;
            VM.mIn = mNhapPhanBoPhi_Thuoc_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void CostThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0766_G1_NhapVaPBoCPhiHDThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CostThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CostThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void CostYCuCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IInwardListCost>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mNhapPhanBoPhi_YCu_Tim;
            VM.mChinhSua_Them = mNhapPhanBoPhi_YCu_ChinhSua_Them;
            VM.mIn = mNhapPhanBoPhi_YCu_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void CostYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0767_G1_NhapVaPBoCPhiHDYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CostYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CostYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void CostHoaChatCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IInwardListCost>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mNhapPhanBoPhi_HoaChat_Tim;
            VM.mChinhSua_Them = mNhapPhanBoPhi_HoaChat_ChinhSua_Them;
            VM.mIn = mNhapPhanBoPhi_HoaChat_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void CostHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0768_G1_NhapVaPBoCPhiHDHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CostHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CostHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Kiem Ke Member
        private void KKThuocCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mKiemKe_Thuoc_Tim;
            VM.mThemMoi = mKiemKe_Thuoc_ThemMoi;
            VM.mXuatExcel = mKiemKe_Thuoc_XuatExcel;
            VM.mXemIn = mKiemKe_Thuoc_XemIn;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKThuocCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ THUỐC KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKYCuCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mKiemKe_YCu_Tim;
            VM.mThemMoi = mKiemKe_YCu_ThemMoi;
            VM.mXuatExcel = mKiemKe_YCu_XuatExcel;
            VM.mXemIn = mKiemKe_YCu_XemIn;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKYCuCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ Y CỤ KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKHoaChatCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mKiemKe_HoaChat_Tim;
            VM.mThemMoi = mKiemKe_HoaChat_ThemMoi;
            VM.mXuatExcel = mKiemKe_HoaChat_XuatExcel;
            VM.mXemIn = mKiemKe_HoaChat_XemIn;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKHoaChatCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ HÓA CHẤT KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKeKetChuyenCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptStockTakes>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            //VM.mTim = mKiemKe_HoaChat_Tim;
            //VM.mThemMoi = mKiemKe_HoaChat_ThemMoi;
            //VM.mXuatExcel = mKiemKe_HoaChat_XuatExcel;
            //VM.mXemIn = mKiemKe_HoaChat_XemIn;
            VM.mTim = gfKiemKe;
            VM.mThemMoi = gfKiemKe;
            VM.mXuatExcel = gfKiemKe;
            VM.mXemIn = gfKiemKe;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void KKVTYTTHCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ VTYTTH KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void KKVaccineCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ VACCINE KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void KKMauCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ MÁU KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void KKVPPCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ VPP KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void KKThanhTrungCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ THANH TRÙNG KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void KKVTTHCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ VTTH KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKeKetChuyenCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Report Member

        #region Báo cáo hàng có giới hạn số lượng xuất

        private void WatchMedCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUG_DEPT_WATCH_OUT_QTY;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.RptParameters.ShowTitle = eHCMSResources.G0364_G1_TheoDoiThuocBHCoGHanSLggXuat.ToUpper();

            UnitVM.mXemIn = mWatchMedOutQty_Preview;
            UnitVM.mIn = mWatchMedOutQty_Print;


            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void WatchMedCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G0364_G1_TheoDoiThuocBHCoGHanSLggXuat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                WatchMedCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        WatchMedCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void WatchMatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUG_DEPT_WATCH_OUT_QTY;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.RptParameters.ShowTitle = eHCMSResources.G0366_G1_TheoDoiYCuBHCoGHanSLggXuat.ToUpper();

            UnitVM.mXemIn = mWatchMatOutQty_Preview;
            UnitVM.mIn = mWatchMatOutQty_Print;


            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void WatchMatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G0366_G1_TheoDoiYCuBHCoGHanSLggXuat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                WatchMatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        WatchMatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void WatchLabCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUG_DEPT_WATCH_OUT_QTY;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.RptParameters.ShowTitle = eHCMSResources.G0353_G1_TheoDoiHChatBHCoGHanSLggXuat.ToUpper();

            UnitVM.mXemIn = mWatchLabOutQty_Preview;
            UnitVM.mIn = mWatchLabOutQty_Print;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void WatchLabCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G0353_G1_TheoDoiHChatBHCoGHanSLggXuat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                WatchLabCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        WatchLabCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion

        #region Nhap Xuat Ton Member
        private void NhapXuatTonThuocCmd_In(object source, long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC, ReportName aReportName = ReportName.None, bool ShowBid = false)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = V_MedProductType;
            VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_Thuoc_KetChuyen;
            VM.CanSelectedRefGenDrugCatID_1 = true;
            VM.ShowBid = ShowBid;
            if (aReportName != ReportName.None)
            {
                VM.eItem = aReportName;
            }
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void NhapXuatTonThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0772_G1_BCNXTThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void DrugDeptInOutDrugStockByBidCmd(object source)
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2824_G1_BCSudung0TheoThau, eHCMSResources.G0787_G1_Thuoc);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonThuocCmd_In(source, (long)AllLookupValues.MedProductType.THUOC, ReportName.DrugDeptInOutStockByBid, true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonThuocCmd_In(source, (long)AllLookupValues.MedProductType.THUOC, ReportName.DrugDeptInOutStockByBid);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void DrugDeptInOutLabStockByBidCmd(object source)
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2824_G1_BCSudung0TheoThau, eHCMSResources.G2907_G1_YCu);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonThuocCmd_In(source, (long)AllLookupValues.MedProductType.Y_CU, ReportName.DrugDeptInOutStockByBid);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonThuocCmd_In(source, (long)AllLookupValues.MedProductType.Y_CU, ReportName.DrugDeptInOutStockByBid);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonYCuCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            /*▼====: #004*/
            VM.LoadDrugDeptProductGroupReportTypes();
            /*▲====: #004*/
            VM.strHienThi = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuatNhapTon_YCu_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_YCu_KetChuyen;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0773_G1_BCNXTYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonHoaChatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;

            VM.mXemIn = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_HoaChat_KetChuyen;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0774_G1_BCNXTHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_HoaChat_KetChuyen;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void NhapXuatTonVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVTYTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapXuatTonVaccinesCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVacXin.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapXuatTonBloodsCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTMau.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapXuatTonResourcesCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2549_G1_BCNXT_VTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NhapXuatTonThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2516_G1_BCNXT_ThanhTrung.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapXuatTonVPPCmd(object source)
        {
            Globals.TitleForm = "BÁO CÁO NHẬP XUẤT TỒN VĂN PHÒNG PHẨM".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Doanh Thu Nhap Xuat Ton
        private void DTNhapXuatTonThuocCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_Thuoc_KetChuyen;
            VM.CanSelectedRefGenDrugCatID_1 = true;
            VM.IsGetValue = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void DTNhapXuatTonThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2106_G1_DoanhThuNXTThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DTNhapXuatTonYCuCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            /*▼====: #004*/
            VM.LoadDrugDeptProductGroupReportTypes();
            /*▲====: #004*/
            VM.strHienThi = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuatNhapTon_YCu_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_YCu_KetChuyen;
            VM.IsGetValue = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void DTNhapXuatTonYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2107_G1_DoanhThuNXTYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DTNhapXuatTonHoaChatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;

            VM.mXemIn = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_HoaChat_KetChuyen;
            VM.IsGetValue = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void DTNhapXuatTonHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2108_G1_DoanhThuNXTHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DTNhapXuatTonCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            //VM.mKetChuyen = mBaoCaoXuatNhapTon_HoaChat_KetChuyen;
            VM.mXemIn = gf1NhapXuatTon;
            VM.mKetChuyen = gf1NhapXuatTon;
            VM.IsGetValue = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void DTNhapXuatTonVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVTYTTHTongHop.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void DTNhapXuatTonVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVacXinTongHop.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void DTNhapXuatTonBloodsCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTMauTongHop.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void DTNhapXuatTonResourcesCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2550_G1_BCNXTTH_VTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void DTNhapXuatTonThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2517_G1_BCNXTTH_ThanhTrung.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void DTNhapXuatTonVPPCmd(object source)
        {
            Globals.TitleForm = "BÁO CÁO NHẬP XUẤT TỒN VĂN PHÒNG PHẨM (TỔNG HỢP)".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region The Kho Member
        private void TheKhoThuocCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptTheKho>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;
            VM.mXem = mBaoCaoTheKho_Thuoc_Xem;
            VM.mIn = mBaoCaoTheKho_Thuoc_In;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TheKhoThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0775_G1_TheKhoThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheKhoYCuCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptTheKho>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;
            VM.mXem = mBaoCaoTheKho_YCu_Xem;
            VM.mIn = mBaoCaoTheKho_YCu_In;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TheKhoYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0776_G1_TheKhoYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheKhoHoaChatCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptTheKho>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;
            VM.mXem = mBaoCaoTheKho_HoaChat_Xem;
            VM.mIn = mBaoCaoTheKho_HoaChat_In;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TheKhoHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0777_G1_TheKhoHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheKhoCmd_In(object source, long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptTheKho>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            //VM.mXem = mBaoCaoTheKho_HoaChat_Xem;
            //VM.mIn = mBaoCaoTheKho_HoaChat_In;
            VM.mXem = gf1TheKho;
            VM.mIn = gf1TheKho;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void TheKhoVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2559_G1_BCTKVTYTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void TheKhoVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2561_G1_BCTKVaccine.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void TheKhoBloodCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2560_G1_BCTKMau.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void TheKhoResourcesCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2553_G1_TheKhoVTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void TheKhoThanhTrungCmd(object source)
        {
            Globals.TitleForm = "THẺ KHO THANH TRÙNG".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void TheKhoVPPCmd(object source)
        {
            Globals.TitleForm = "THẺ KHO VĂN PHÒNG PHẨM".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In(source, (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        // 20191224 TNHX: Add new TheKhoCoGia
        private void TheKhoHasValueCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IDrugModule>();
            var reportVm = Globals.GetViewModel<ITheKhoKT>();
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void TheKhoHasValueCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2948_G1_TheKhoCoGia;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoHasValueCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoHasValueCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Phieu DN TT Member

        private void SuggestThuocCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptBangKeChungTuThanhToan>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mPhieuDNThanhToan_Thuoc_Tim;
            VM.mThemMoi = mPhieuDNThanhToan_Thuoc_PhieuMoi;
            //VM.mChinhSua = mPhieuDNThanhToan_HoaChat_ChinhSua;
            VM.mXemInBK = mPhieuDNThanhToan_Thuoc_XemInBK;
            VM.mXemInPDNTT = mPhieuDNThanhToan_Thuoc_XemInPDNTT;


            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void SuggestThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0778_G1_BKeCTuTToanThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SuggestThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SuggestThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SuggestYCuCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptBangKeChungTuThanhToan>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mPhieuDNThanhToan_YCu_Tim;
            VM.mThemMoi = mPhieuDNThanhToan_YCu_PhieuMoi;
            //VM.mChinhSua = mPhieuDNThanhToan_HoaChat_ChinhSua;
            VM.mXemInBK = mPhieuDNThanhToan_YCu_XemInBK;
            VM.mXemInPDNTT = mPhieuDNThanhToan_YCu_XemInPDNTT;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void SuggestYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0779_G1_BKeCTuTToanYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SuggestYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SuggestYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SuggestHoaChatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptBangKeChungTuThanhToan>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mPhieuDNThanhToan_HoaChat_Tim;
            VM.mThemMoi = mPhieuDNThanhToan_HoaChat_PhieuMoi;
            //VM.mChinhSua = mPhieuDNThanhToan_HoaChat_ChinhSua;
            VM.mXemInBK = mPhieuDNThanhToan_HoaChat_XemInBK;
            VM.mXemInPDNTT = mPhieuDNThanhToan_HoaChat_XemInPDNTT;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void SuggestHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0780_G1_BKeCTuTToanHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SuggestHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SuggestHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion

        #region RptDanhSachXuatKhoaDuoc
        private void XuatThuocCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.TieuDeRpt = Globals.TitleForm.ToUpper();
            VM.mXemIn = mBaoCaoXuat_Thuoc_XemIn;
            VM.VisibilityOutputType = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K3100_G1_DSXuatThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatYCuCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuat_YCu_XemIn;
            VM.VisibilityOutputType = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0844_G1_DSXuatYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatHoaChatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuat_HoaChat_XemIn;
            VM.VisibilityOutputType = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0845_G1_DSXuatHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatVTYTTHCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoXuat_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoXuat;
            VM.VisibilityOutputType = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2577_G1_DSXuatVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatVTYTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatVTYTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatVaccineCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoXuat_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoXuat;
            VM.VisibilityOutputType = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2578_G1_DSXuatVaccine;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatVaccineCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatVaccineCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatMauCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoXuat_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoXuat;
            VM.VisibilityOutputType = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatMauCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2579_G1_DSXuatMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatMauCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatMauCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatVPPCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoXuat_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoXuat;
            VM.VisibilityOutputType = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatVPPCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2580_G1_DSXuatVPP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatVPPCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatVPPCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatVTTHCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoXuat_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoXuat;
            VM.VisibilityOutputType = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatVTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2581_G1_DSXuatVTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatVTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatVTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatThanhTrungCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoXuat_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoXuat;
            VM.VisibilityOutputType = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2582_G1_DSXuatThanhTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatThanhTrungCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatThanhTrungCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatNoiBoCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.TieuDeRpt = Globals.TitleForm;
            VM.VisibilityOutputType = false;
            VM.IsDrugDeptExportDetail = true;
            VM.InitForIsDrugDeptExportDetail();
            VM.IsViewByVisible = false;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void XuatNoiBoCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G2892_G1_XuatNBo;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatNoiBoCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatNoiBoCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region nhap hang hang thang theo invoice

        private void BangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_MEDDEPTINVOICE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.mXemIn = mBaoCaoDSPhieuNhapKho_Thuoc_XemIn;
            UnitVM.mIn = mBaoCaoDSPhieuNhapKho_Thuoc_In;


            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0781_G1_BKePhNhapKhoThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_MEDDEPTINVOICE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.mXemIn = mBaoCaoDSPhieuNhapKho_YCu_XemIn;
            UnitVM.mIn = mBaoCaoDSPhieuNhapKho_YCu_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0782_G1_BKePhNhapKhoYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_MEDDEPTINVOICE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.mXemIn = mBaoCaoDSPhieuNhapKho_HoaChat_XemIn;
            UnitVM.mIn = mBaoCaoDSPhieuNhapKho_HoaChat_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0783_G1_BKePhNhapKhoHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2539_G1_BKePhNhapKhoVTYTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_MEDDEPTINVOICE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            //UnitVM.mXemIn = mBaoCaoDSPhieuNhapKho_HoaChat_XemIn;
            //UnitVM.mIn = mBaoCaoDSPhieuNhapKho_HoaChat_In;
            UnitVM.mXemIn = gf1PhieuNhapKho;
            UnitVM.mIn = gf1PhieuNhapKho;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangKeNhapHangThangTheoSoPhieuNhapKhoMauCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2541_G1_BKePhNhapKhoMau.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoMauCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoMauCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BangKeNhapHangThangTheoSoPhieuNhapKhoMauCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_MEDDEPTINVOICE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            //UnitVM.mXemIn = mBaoCaoDSPhieuNhapKho_HoaChat_XemIn;
            //UnitVM.mIn = mBaoCaoDSPhieuNhapKho_HoaChat_In;
            UnitVM.mXemIn = gf1PhieuNhapKho;
            UnitVM.mIn = gf1PhieuNhapKho;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangKeNhapHangThangTheoSoPhieuNhapKhoTiemNguaCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2540_G1_BKePhNhapKhoVaccine.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoTiemNguaCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoTiemNguaCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BangKeNhapHangThangTheoSoPhieuNhapKhoTiemNguaCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_MEDDEPTINVOICE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            //UnitVM.mXemIn = mBaoCaoDSPhieuNhapKho_HoaChat_XemIn;
            //UnitVM.mIn = mBaoCaoDSPhieuNhapKho_HoaChat_In;
            UnitVM.mXemIn = gf1PhieuNhapKho;
            UnitVM.mIn = gf1PhieuNhapKho;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void BangKeNhapHangThangTheoSoPhieuNhapKhoVatTuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2542_G1_BKePhNhapKhoVatTu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoVatTuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoVatTuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BangKeNhapHangThangTheoSoPhieuNhapKhoVatTuCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_MEDDEPTINVOICE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            //UnitVM.mXemIn = mBaoCaoDSPhieuNhapKho_HoaChat_XemIn;
            //UnitVM.mIn = mBaoCaoDSPhieuNhapKho_HoaChat_In;
            UnitVM.mXemIn = gf1PhieuNhapKho;
            UnitVM.mIn = gf1PhieuNhapKho;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void BangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2543_G1_BKePhNhapKhoThanhTrung.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_MEDDEPTINVOICE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            //UnitVM.mXemIn = mBaoCaoDSPhieuNhapKho_HoaChat_XemIn;
            //UnitVM.mIn = mBaoCaoDSPhieuNhapKho_HoaChat_In;
            UnitVM.mXemIn = gf1PhieuNhapKho;
            UnitVM.mIn = gf1PhieuNhapKho;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void BangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2544_G1_BKePhNhapKhoVPP.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_MEDDEPTINVOICE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            //UnitVM.mXemIn = mBaoCaoDSPhieuNhapKho_HoaChat_XemIn;
            //UnitVM.mIn = mBaoCaoDSPhieuNhapKho_HoaChat_In;
            UnitVM.mXemIn = gf1PhieuNhapKho;
            UnitVM.mIn = gf1PhieuNhapKho;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        #endregion

        /*TMA 23/10/2017 - Báo cáo nhập*/
        #region Bao Cao Nhap Member

        private void NhapThuocCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mXemIn = mBaoCaoNhap_Thuoc_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.S0652_G1_SoKiemNhapThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapYCuCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mXemIn = mBaoCaoNhap_YCu_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2139_G1_SoKiemNhapYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapHoaChatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mXemIn = mBaoCaoNhap_HoaChat_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2138_G1_SoKiemNhapHoaChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapVTYTTHCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoNhap_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoNhap;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2583_G1_SoKiemNhapVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapVTYTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapVTYTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapVaccnieCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoNhap_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoNhap;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2584_G1_SoKiemNhapVaccine;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapVaccnieCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapVaccnieCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapMauCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoNhap_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoNhap;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapMauCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2585_G1_SoKiemNhapMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapMauCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapMauCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapVPPCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoNhap_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoNhap;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapVPPCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2586_G1_SoKiemNhapVPP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapVPPCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapVPPCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapVTTHCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoNhap_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoNhap;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapVTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2587_G1_SoKiemNhapVTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapVTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapVTTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapThanhTrungCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            //VM.mXemIn = mBaoCaoNhap_HoaChat_XemIn;
            VM.mXemIn = gf1BaoCaoNhap;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2588_G1_SoKiemNhapThanhTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapThanhTrungCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapThanhTrungCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion
        /*TMA 23/10/2017 - Báo cáo nhập*/

        /*▼====: #003*/
        #region Temp Inward Report

        private void TempInwardMedReportCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mXemIn = mBaoCaoNhap_Thuoc_XemIn;
            VM.IsTempInwardReport = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void TempInwardMedReportCmd(object source)
        {
            Globals.TitleForm = string.Format("{0} ({1})", eHCMSResources.Z2160_G1_TheoDoiHangKyGoi, eHCMSResources.G0787_G1_Thuoc);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TempInwardMedReportCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TempInwardMedReportCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TempInwardMatReportCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mXemIn = mBaoCaoNhap_YCu_XemIn;
            VM.IsTempInwardReport = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void TempInwardMatReportCmd(object source)
        {
            Globals.TitleForm = string.Format("{0} ({1})", eHCMSResources.Z2160_G1_TheoDoiHangKyGoi, eHCMSResources.G2907_G1_YCu);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TempInwardMatReportCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TempInwardMatReportCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TempInwardLabReportCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.mChonKho = false;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mXemIn = mBaoCaoNhap_HoaChat_XemIn;
            VM.IsTempInwardReport = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void TempInwardLabReportCmd(object source)
        {
            Globals.TitleForm = string.Format("{0} ({1})", eHCMSResources.Z2160_G1_TheoDoiHangKyGoi, eHCMSResources.T1616_G1_HC);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TempInwardLabReportCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TempInwardLabReportCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion
        /*▲====: #003*/

        #region Bao Cao Nam Member

        private void SuDungThuocCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_INOUTSTOCK_ADDICTIVE;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mXemIn = mBaoCaoSuDung_Thuoc_XemIn;
            UnitVM.mIn = mBaoCaoSuDung_Thuoc_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void SuDungThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0784_G1_BCSDThuocTheoNam.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SuDungThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SuDungThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Theo Doi Cong No Member

        private void TheoDoiCongNoThuocCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_THEODOICONGNO;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.RptParameters.HideTypCongNo = true;
            UnitVM.RptParameters.ShowFirst = string.Format("{0} ", eHCMSResources.Z0095_G1_NhapKho.ToUpper());
            UnitVM.RptParameters.ShowTitle = eHCMSResources.G0348_G1_TheoDoiChuaTraTienoanHDonThuoc.ToUpper();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TheoDoiCongNoThuocCmd(object source)
        {
            Globals.TitleForm = string.Format("{0} {1}", eHCMSResources.G0350_G1_TheoDoiCNo, eHCMSResources.G0787_G1_Thuoc).ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheoDoiCongNoThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheoDoiCongNoThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheoDoiCongNoYCuCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_THEODOICONGNO;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.RptParameters.HideTypCongNo = true;
            UnitVM.RptParameters.ShowFirst = string.Format("{0} ", eHCMSResources.Z0095_G1_NhapKho.ToUpper());
            UnitVM.RptParameters.ShowTitle = eHCMSResources.G0349_G1_TheoDoiChuaTraTienoanHDonYCu;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TheoDoiCongNoYCuCmd(object source)
        {
            Globals.TitleForm = string.Format("{0} {1}", eHCMSResources.G0350_G1_TheoDoiCNo, eHCMSResources.G2907_G1_YCu).ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheoDoiCongNoYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheoDoiCongNoYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheoDoiCongNoHoaChatCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.DRUGDEPT_REPORT_THEODOICONGNO;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.RptParameters.HideTypCongNo = true;
            UnitVM.RptParameters.ShowFirst = string.Format("{0} ", eHCMSResources.Z0095_G1_NhapKho.ToUpper());
            UnitVM.RptParameters.ShowTitle = eHCMSResources.G0347_G1_TheoDoiChuaTraTienoanHDonHChat;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TheoDoiCongNoHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0785_G1_TheoDoiCNoHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheoDoiCongNoHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheoDoiCongNoHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Nhập Xuất Đến Khoa Phòng
        private void ThuocXuatDenKhoaCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptNhapXuatDenKhoaPhong>();
            VM.TitleForm = Globals.TitleForm;
            VM.TieuDeRpt = eHCMSResources.K3100_G1_DSXuatThuoc.ToUpper();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;


            VM.mIn = mBaoCaoNhapXuatDenKhoaPhong_Thuoc_In;
            VM.mXuatExcel = mBaoCaoNhapXuatDenKhoaPhong_Thuoc_XuatExcel;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ThuocXuatDenKhoaCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0786_G1_SLgThuocNXDenKhoa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ThuocXuatDenKhoaCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ThuocXuatDenKhoaCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void YCuXuatDenKhoaCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptNhapXuatDenKhoaPhong>();
            VM.TitleForm = Globals.TitleForm;
            VM.TieuDeRpt = eHCMSResources.Z0844_G1_DSXuatYCu;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;


            VM.mIn = mBaoCaoNhapXuatDenKhoaPhong_YCu_In;
            VM.mXuatExcel = mBaoCaoNhapXuatDenKhoaPhong_YCu_XuatExcel;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void YCuXuatDenKhoaCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0787_G1_SLgYCuNXDenKhoa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                YCuXuatDenKhoaCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        YCuXuatDenKhoaCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void HoaChatXuatDenKhoaCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IRptNhapXuatDenKhoaPhong>();
            VM.TitleForm = Globals.TitleForm;
            VM.TieuDeRpt = eHCMSResources.Z0845_G1_DSXuatHChat;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;


            VM.mIn = mBaoCaoNhapXuatDenKhoaPhong_HoaChat_In;
            VM.mXuatExcel = mBaoCaoNhapXuatDenKhoaPhong_HoaChat_XuatExcel;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void HoaChatXuatDenKhoaCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0788_G1_SLgHChatNXDenKhoa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoaChatXuatDenKhoaCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoaChatXuatDenKhoaCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #endregion

        #region Xuất Hàng Ký Gởi

        private void XNBThuocHangKyGoiCmd_In(object source)
        {
            //Globals.PageName = Globals.TitleForm;

            //var module = Globals.GetViewModel<IDrugModule>();
            //var UnitVM = Globals.GetViewModel<IXuatHangKyGoi>();
            //UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            //UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mXuatHangKyGui_Tim = mXuatHangKyGui_Thuoc_Tim;
            //UnitVM.mXuatHangKyGui_PhieuMoi = mXuatHangKyGui_Thuoc_PhieuMoi;
            //UnitVM.mXuatHangKyGui_Save = mXuatHangKyGui_Thuoc_Save;
            //UnitVM.mXuatHangKyGui_ThuTien = mXuatHangKyGui_Thuoc_ThuTien;
            //UnitVM.mXuatHangKyGui_XemIn = mXuatHangKyGui_Thuoc_XemIn;
            //UnitVM.mXuatHangKyGui_In = mXuatHangKyGui_Thuoc_In;
            //UnitVM.mXuatHangKyGui_DeleteInvoice = mXuatHangKyGui_Thuoc_DeleteInvoice;
            //UnitVM.mXuatHangKyGui_PrintReceipt = mXuatHangKyGui_Thuoc_PrintReceipt;
            //UnitVM.mIsInputTemp = true;
            //UnitVM.LoadRefOutputType();
            //module.MainContent = UnitVM;
            //(module as Conductor<object>).ActivateItem(UnitVM);

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBoHangKyGui>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mXuatHangKyGui_Thuoc_Tim;
            UnitVM.mPhieuMoi = mXuatHangKyGui_Thuoc_PhieuMoi;
            UnitVM.mThucHien = mXuatHangKyGui_Thuoc_Save;
            UnitVM.mThuTien = mXuatHangKyGui_Thuoc_ThuTien;
            UnitVM.mIn = mXuatHangKyGui_Thuoc_In;
            UnitVM.mDeleteInvoice = mXuatHangKyGui_Thuoc_DeleteInvoice;
            UnitVM.mPrintReceipt = mXuatHangKyGui_Thuoc_PrintReceipt;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBThuocHangKyGoiCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0789_G1_XuatHgKGThuocTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBThuocHangKyGoiCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBThuocHangKyGoiCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBYCuHangKyGoiCmd_In(object source)
        {

            //Globals.PageName = Globals.TitleForm;

            //var module = Globals.GetViewModel<IDrugModule>();
            //var UnitVM = Globals.GetViewModel<IXuatHangKyGoi>();
            //UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            //UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mXuatHangKyGui_Tim = mXuatHangKyGui_YCu_Tim;
            //UnitVM.mXuatHangKyGui_PhieuMoi = mXuatHangKyGui_YCu_PhieuMoi;
            //UnitVM.mXuatHangKyGui_Save = mXuatHangKyGui_YCu_Save;
            //UnitVM.mXuatHangKyGui_ThuTien = mXuatHangKyGui_YCu_ThuTien;
            //UnitVM.mXuatHangKyGui_XemIn = mXuatHangKyGui_YCu_XemIn;
            //UnitVM.mXuatHangKyGui_In = mXuatHangKyGui_YCu_In;
            //UnitVM.mXuatHangKyGui_DeleteInvoice = mXuatHangKyGui_YCu_DeleteInvoice;
            //UnitVM.mXuatHangKyGui_PrintReceipt = mXuatHangKyGui_YCu_PrintReceipt;

            //UnitVM.mIsInputTemp = true;
            //UnitVM.LoadRefOutputType();
            //module.MainContent = UnitVM;
            //(module as Conductor<object>).ActivateItem(UnitVM);
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBoHangKyGui>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mXuatHangKyGui_Thuoc_Tim;
            UnitVM.mPhieuMoi = mXuatHangKyGui_Thuoc_PhieuMoi;
            UnitVM.mThucHien = mXuatHangKyGui_Thuoc_Save;
            UnitVM.mThuTien = mXuatHangKyGui_Thuoc_ThuTien;
            UnitVM.mIn = mXuatHangKyGui_Thuoc_In;
            UnitVM.mDeleteInvoice = mXuatHangKyGui_Thuoc_DeleteInvoice;
            UnitVM.mPrintReceipt = mXuatHangKyGui_Thuoc_PrintReceipt;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBYCuHangKyGoiCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0790_G1_XuatHgKGYCuTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBYCuHangKyGoiCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBYCuHangKyGoiCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XNBHoaChatHangKyGoiCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatHangKyGoi>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mXuatHangKyGui_Tim = mXuatHangKyGui_HoaChat_Tim;
            UnitVM.mXuatHangKyGui_PhieuMoi = mXuatHangKyGui_HoaChat_PhieuMoi;
            UnitVM.mXuatHangKyGui_Save = mXuatHangKyGui_HoaChat_Save;
            UnitVM.mXuatHangKyGui_ThuTien = mXuatHangKyGui_HoaChat_ThuTien;
            UnitVM.mXuatHangKyGui_XemIn = mXuatHangKyGui_HoaChat_XemIn;
            UnitVM.mXuatHangKyGui_In = mXuatHangKyGui_HoaChat_In;
            UnitVM.mXuatHangKyGui_DeleteInvoice = mXuatHangKyGui_HoaChat_DeleteInvoice;
            UnitVM.mXuatHangKyGui_PrintReceipt = mXuatHangKyGui_HoaChat_PrintReceipt;

            UnitVM.mIsInputTemp = true;
            UnitVM.LoadRefOutputType();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBHoaChatHangKyGoiCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0791_G1_XuatHgKGHChatTuKhDuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBHoaChatHangKyGoiCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBHoaChatHangKyGoiCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Xáp Nhập Hàng Ký Gởi

        private void JoinThuocHangKyGoiCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IXapNhapPXHangKyGoi>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;

            VM.mSapNhapHangKyGui_Tim = mSapNhapHangKyGui_Thuoc_Tim;
            VM.mSapNhapHangKyGui_PhieuMoi = mSapNhapHangKyGui_Thuoc_PhieuMoi;
            VM.mSapNhapHangKyGui_CapNhat = mSapNhapHangKyGui_Thuoc_CapNhat;
            VM.mSapNhapHangKyGui_Xoa = mSapNhapHangKyGui_Thuoc_Xoa;
            VM.mSapNhapHangKyGui_XemIn = mSapNhapHangKyGui_Thuoc_XemIn;
            VM.mSapNhapHangKyGui_In = mSapNhapHangKyGui_Thuoc_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void JoinThuocHangKyGoiCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0792_G1_XapNhapPXHgKGThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                JoinThuocHangKyGoiCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        JoinThuocHangKyGoiCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void JoinYCuHangKyGoiCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IXapNhapPXHangKyGoi>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;

            VM.mSapNhapHangKyGui_Tim = mSapNhapHangKyGui_YCu_Tim;
            VM.mSapNhapHangKyGui_PhieuMoi = mSapNhapHangKyGui_YCu_PhieuMoi;
            VM.mSapNhapHangKyGui_CapNhat = mSapNhapHangKyGui_YCu_CapNhat;
            VM.mSapNhapHangKyGui_Xoa = mSapNhapHangKyGui_YCu_Xoa;
            VM.mSapNhapHangKyGui_XemIn = mSapNhapHangKyGui_YCu_XemIn;
            VM.mSapNhapHangKyGui_In = mSapNhapHangKyGui_YCu_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void JoinYCuHangKyGoiCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0793_G1_XapNhapPXHgKGYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                JoinYCuHangKyGoiCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        JoinYCuHangKyGoiCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void JoinHoaChatHangKyGoiCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IXapNhapPXHangKyGoi>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;

            VM.mSapNhapHangKyGui_Tim = mSapNhapHangKyGui_HoaChat_Tim;
            VM.mSapNhapHangKyGui_PhieuMoi = mSapNhapHangKyGui_HoaChat_PhieuMoi;
            VM.mSapNhapHangKyGui_CapNhat = mSapNhapHangKyGui_HoaChat_CapNhat;
            VM.mSapNhapHangKyGui_Xoa = mSapNhapHangKyGui_HoaChat_Xoa;
            VM.mSapNhapHangKyGui_XemIn = mSapNhapHangKyGui_HoaChat_XemIn;
            VM.mSapNhapHangKyGui_In = mSapNhapHangKyGui_HoaChat_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void JoinHoaChatHangKyGoiCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0794_G1_XapNhapPXHgKGHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                JoinHoaChatHangKyGoiCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        JoinHoaChatHangKyGoiCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion


        private void UnitCmd_In(object source)
        {


            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptUnits>();
            Globals.PageName = Globals.TitleForm;
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void UnitCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0795_G1_QLyDMucDVT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                UnitCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        UnitCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenDrugBHYTCmd_In(object source)
        {
            var module = Globals.GetViewModel<IDrugModule>();
            var RefGenDrugBHYTVM = Globals.GetViewModel<IRefGenDrugBHYT_Category>();
            Globals.PageName = Globals.TitleForm;

            module.MainContent = RefGenDrugBHYTVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugBHYTVM);
        }

        public void RefGenDrugBHYTCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0796_G1_QLyDMucBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenDrugBHYTCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenDrugBHYTCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DrugClass_DeptCmd_In(object source)
        {
            var module = Globals.GetViewModel<IDrugModule>();
            var DrugClassVM = Globals.GetViewModel<IDrugDeptClass>();
            Globals.PageName = Globals.TitleForm;
            DrugClassVM.TitleForm = Globals.TitleForm;
            DrugClassVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            /*▼====: #007*/
            //DrugClassVM.LoadFamilyParent((long)AllLookupValues.MedProductType.THUOC);
            /*▲====: #007*/
            DrugClassVM.GetSearchTreeView((long)AllLookupValues.MedProductType.THUOC);
            DrugClassVM.IsDoubleClick = false;
            DrugClassVM.bTim = mQLNhomHang_Thuoc_Tim;
            DrugClassVM.bThem = mQLNhomHang_Thuoc_ThemMoi;
            DrugClassVM.bChinhSua = mQLNhomHang_Thuoc_ChinhSua;

            module.MainContent = DrugClassVM;
            (module as Conductor<object>).ActivateItem(DrugClassVM);
        }

        public void DrugClass_DeptCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0797_G1_QLyDMucNhomThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugClass_DeptCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugClass_DeptCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void DrugClass_MedicalCmd_In(object source)
        {
            var module = Globals.GetViewModel<IDrugModule>();
            var DrugClassVM = Globals.GetViewModel<IDrugDeptClass>();
            Globals.PageName = Globals.TitleForm;
            DrugClassVM.TitleForm = Globals.TitleForm;
            DrugClassVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            /*▼====: #007*/
            //DrugClassVM.LoadFamilyParent((long)AllLookupValues.MedProductType.Y_CU);
            /*▲====: #007*/
            DrugClassVM.GetSearchTreeView((long)AllLookupValues.MedProductType.Y_CU);
            DrugClassVM.IsDoubleClick = false;
            DrugClassVM.bTim = mQLNhomHang_YCu_Tim;
            DrugClassVM.bThem = mQLNhomHang_YCu_ThemMoi;
            DrugClassVM.bChinhSua = mQLNhomHang_YCu_ChinhSua;

            module.MainContent = DrugClassVM;
            (module as Conductor<object>).ActivateItem(DrugClassVM);
        }

        public void DrugClass_MedicalCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0798_G1_QLyDMucNhomYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugClass_MedicalCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugClass_MedicalCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DrugClass_ChemicalCmd_In(object source)
        {
            var module = Globals.GetViewModel<IDrugModule>();
            var DrugClassVM = Globals.GetViewModel<IDrugDeptClass>();
            Globals.PageName = Globals.TitleForm;
            DrugClassVM.TitleForm = Globals.TitleForm;
            DrugClassVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            /*▼====: #007*/
            //DrugClassVM.LoadFamilyParent((long)AllLookupValues.MedProductType.HOA_CHAT);
            /*▲====: #007*/
            DrugClassVM.GetSearchTreeView((long)AllLookupValues.MedProductType.HOA_CHAT);
            DrugClassVM.IsDoubleClick = false;
            DrugClassVM.bTim = mQLNhomHang_HoaChat_Tim;
            DrugClassVM.bThem = mQLNhomHang_HoaChat_ThemMoi;
            DrugClassVM.bChinhSua = mQLNhomHang_HoaChat_ChinhSua;

            module.MainContent = DrugClassVM;
            (module as Conductor<object>).ActivateItem(DrugClassVM);
        }

        public void DrugClass_ChemicalCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0799_G1_QLyDMucNhomHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugClass_ChemicalCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugClass_ChemicalCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        /*▼====: #005*/
        private void DrugClass_RefGeneric_In(object source)
        {
            var module = Globals.GetViewModel<IDrugModule>();
            var DrugClassVM = Globals.GetViewModel<IGenericClass>();
            Globals.PageName = Globals.TitleForm;
            DrugClassVM.TitleForm = Globals.TitleForm;
            DrugClassVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOAT_CHAT;
            /*▼====: #007*/
            //DrugClassVM.LoadFamilyParent((long)AllLookupValues.MedProductType.HOAT_CHAT);
            /*▲====: #007*/
            DrugClassVM.GetSearchTreeView((long)AllLookupValues.MedProductType.HOAT_CHAT);

            DrugClassVM.bTim = mQLNhomHang_HoatChat_Tim;
            DrugClassVM.bThem = mQLNhomHang_HoatChat_ThemMoi;
            DrugClassVM.bChinhSua = mQLNhomHang_HoatChat_ChinhSua;

            //DrugClassVM.bTim = mQLNhomHang_Thuoc_Tim;
            //DrugClassVM.bThem = mQLNhomHang_Thuoc_ThemMoi;
            //DrugClassVM.bChinhSua = mQLNhomHang_Thuoc_ChinhSua;

            module.MainContent = DrugClassVM;
            (module as Conductor<object>).ActivateItem(DrugClassVM);
        }

        public void DrugClass_RefGeneric(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2254_G1_QLDMNhomHoatChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugClass_RefGeneric_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugClass_RefGeneric_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        /*▲====: #005*/

        #region Tồn Kho
        private void InStock_MedCmd_In(object source)
        {


            var module = Globals.GetViewModel<IDrugModule>();
            var vm = Globals.GetViewModel<IInStock>();
            Globals.PageName = Globals.TitleForm;
            vm.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;

            module.MainContent = vm;
            (module as Conductor<object>).ActivateItem(vm);
        }

        public void InStock_MedCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0800_G1_TonKhoThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InStock_MedCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InStock_MedCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void InStock_MatCmd_In(object source)
        {


            var module = Globals.GetViewModel<IDrugModule>();
            var vm = Globals.GetViewModel<IInStock>();
            Globals.PageName = Globals.TitleForm;
            vm.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;

            module.MainContent = vm;
            (module as Conductor<object>).ActivateItem(vm);
        }

        public void InStock_MatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0801_G1_TonKhoYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InStock_MatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InStock_MatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void InStock_LabCmd_In(object source)
        {


            var module = Globals.GetViewModel<IDrugModule>();
            var vm = Globals.GetViewModel<IInStock>();
            Globals.PageName = Globals.TitleForm;
            vm.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;

            module.MainContent = vm;
            (module as Conductor<object>).ActivateItem(vm);
        }

        public void InStock_LabCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0802_G1_TonKhoHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InStock_LabCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InStock_LabCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion

        #region Đổi Giá Bán Theo Lô
        private void AdjustOutPrice_MedCmd_In(object source)
        {


            var module = Globals.GetViewModel<IDrugModule>();
            var vm = Globals.GetViewModel<IAdjustOutPrice>();
            Globals.PageName = Globals.TitleForm;
            vm.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            vm.TitleForm = eHCMSResources.Z0803_G1_ThayDoiGiaBanThuocTheoLo;
            module.MainContent = vm;
            (module as Conductor<object>).ActivateItem(vm);
        }

        public void AdjustOutPrice_MedCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0803_G1_ThayDoiGiaBanThuocTheoLo;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AdjustOutPrice_MedCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AdjustOutPrice_MedCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void AdjustOutPrice_MatCmd_In(object source)
        {


            var module = Globals.GetViewModel<IDrugModule>();
            var vm = Globals.GetViewModel<IAdjustOutPrice>();
            Globals.PageName = Globals.TitleForm;
            vm.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            vm.TitleForm = eHCMSResources.Z0804_G1_ThayDoiGiaBanYCuTheoLo;

            module.MainContent = vm;
            (module as Conductor<object>).ActivateItem(vm);
        }

        public void AdjustOutPrice_MatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0804_G1_ThayDoiGiaBanYCuTheoLo;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AdjustOutPrice_MatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AdjustOutPrice_MatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void AdjustOutPrice_LabCmd_In(object source)
        {


            var module = Globals.GetViewModel<IDrugModule>();
            var vm = Globals.GetViewModel<IAdjustOutPrice>();
            Globals.PageName = Globals.TitleForm;
            vm.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            vm.TitleForm = eHCMSResources.Z0805_G1_ThayDoiGiaBanHChatTheoLo;

            module.MainContent = vm;
            (module as Conductor<object>).ActivateItem(vm);
        }

        public void AdjustOutPrice_LabCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0805_G1_ThayDoiGiaBanHChatTheoLo;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AdjustOutPrice_LabCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AdjustOutPrice_LabCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion
        /*▼====: #001*/
        #region BidDetail
        private void BidDetailCmd_In(object source, string SubName)
        {
            Globals.PageName = Globals.TitleForm + ": " + SubName;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IBidDetail>();
            UnitVM.IsMedDept = true;
            if ((source as MenuItem).Name == "MedBidDetailCmd")
                UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            else if ((source as MenuItem).Name == "MatBidDetailCmd")
                UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            else if ((source as MenuItem).Name == "ChemicalBidDetailCmd")
            {
                UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            }
            else UnitVM.V_MedProductType = 0;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BidDetailCmd(object source)
        {
            string SubName = "";
            switch ((source as MenuItem).Name)
            {
                case "MedBidDetailCmd":
                    SubName = eHCMSResources.G0787_G1_Thuoc;
                    break;
                case "MatBidDetailCmd":
                    SubName = eHCMSResources.G2907_G1_YCu;
                    break;
                case "ChemicalBidDetailCmd":
                    SubName = eHCMSResources.T1616_G1_HC;
                    break;
                default:
                    SubName = "Chưa định nghĩa";
                    break;
            }
            Globals.TitleForm = eHCMSResources.Z2112_G1_ChiTietThau + ": " + SubName;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BidDetailCmd_In(source, SubName);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BidDetailCmd_In(source, SubName);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion
        /*▲====: #001*/
        private void StorageCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptStorage>();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void StorageCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1604_G1_QLyDMucKho.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StorageCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StorageCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        /*▼====: #002*/
        #region Đổi Giá Bán Theo Lô
        private void Clinic_AdjustOutPrice_In(object source, long MedType = (long)AllLookupValues.MedProductType.THUOC)
        {


            var module = Globals.GetViewModel<IDrugModule>();
            var vm = Globals.GetViewModel<IAdjustClinicPrice>();
            Globals.PageName = Globals.TitleForm;
            vm.V_MedProductType = MedType;
            vm.TitleForm = Globals.TitleForm;
            module.MainContent = vm;
            (module as Conductor<object>).ActivateItem(vm);
        }
        public void Clinic_AdjustOutPrice_MedCmd(object source)
        {
            Globals.TitleForm = string.Format("{0}: {1}", eHCMSResources.G2115_G1_DoiGiaBanTheoLoKP.ToUpper(), eHCMSResources.G0787_G1_Thuoc.ToUpper());
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Clinic_AdjustOutPrice_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Clinic_AdjustOutPrice_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void Clinic_AdjustOutPrice_MatCmd(object source)
        {
            Globals.TitleForm = string.Format("{0}: {1}", eHCMSResources.G2115_G1_DoiGiaBanTheoLoKP.ToUpper(), eHCMSResources.G2907_G1_YCu.ToUpper());
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Clinic_AdjustOutPrice_In(source, (long)AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Clinic_AdjustOutPrice_In(source, (long)AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void Clinic_AdjustOutPrice_LabCmd(object source)
        {
            Globals.TitleForm = string.Format("{0}: {1}", eHCMSResources.G2115_G1_DoiGiaBanTheoLoKP.ToUpper(), eHCMSResources.T1616_G1_HC.ToUpper());
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Clinic_AdjustOutPrice_In(source, (long)AllLookupValues.MedProductType.HOA_CHAT);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Clinic_AdjustOutPrice_In(source, (long)AllLookupValues.MedProductType.HOA_CHAT);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion
        /*▲====: #002*/

        //▼====== #009
        private void InwardDrugFromPharmacyCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;
            VM.mNhapTraTuKhoPhong_Tim = mNhapTraTuKhoPhong_Thuoc_Tim;
            VM.mNhapTraTuKhoPhong_PhieuMoi = mNhapTraTuKhoPhong_Thuoc_Tim;
            VM.mNhapTraTuKhoPhong_XemIn = mNhapTraTuKhoPhong_Thuoc_Tim;
            VM.mNhapTraTuKhoPhong_In = mNhapTraTuKhoPhong_Thuoc_Tim;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromPharmacyCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2357_G1_NhapTraThuocTuKhBHYTNhaThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromPharmacyCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromPharmacyCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #009
        //▼====== #010
        private void ApprovedDrug_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IRequestForHIStore>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mDuyetPhieuLinhHang_Tim = mDuyetPhieuLinhHang_Thuoc_Tim;
            UnitVM.mDuyetPhieuLinhHang_PhieuMoi = mDuyetPhieuLinhHang_Thuoc_PhieuMoi;
            UnitVM.mDuyetPhieuLinhHang_XuatHang = mDuyetPhieuLinhHang_Thuoc_XuatHang;
            UnitVM.mDuyetPhieuLinhHang_XemInTH = mDuyetPhieuLinhHang_Thuoc_XemInTH;
            UnitVM.mDuyetPhieuLinhHang_XemInCT = mDuyetPhieuLinhHang_Thuoc_XemInCT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ApprovedDrug(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0732_G1_DuyetPhLinhThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ApprovedDrug_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ApprovedDrug_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #010

        private void InwardDrugFromDeptDrugCmd_In(object source, bool IsImportFromSubStorage = false, long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ISubStorageInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.SearchCriteria = new InwardInvoiceSearchCriteria { IsInputMedDept = true };
            VM.IsImportFromSubStorage = IsImportFromSubStorage;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void InwardDrugFromDeptDrugCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2443_G1_NhapNoiBo;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void MainStorageInwardDrugFromDeptDrugCmd_In(object source, bool IsImportFromSubStorage = false, long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IMainStorageInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.SearchCriteria = new InwardInvoiceSearchCriteria { IsInputMedDept = true };
            //VM.IsImportFromSubStorage = IsImportFromSubStorage;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void MainStorageInwardDrugFromDeptDrugCmd(object source)
        {
            Globals.TitleForm = "NHẬP NỘI BỘ HÀNG KÝ GỬI THUỐC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                MainStorageInwardDrugFromDeptDrugCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        MainStorageInwardDrugFromDeptDrugCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void MainStorageInwardDrugFromDeptDrugYCCmd(object source)
        {
            Globals.TitleForm = "NHẬP NỘI BỘ HÀNG KÝ GỬI Y CỤ";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                MainStorageInwardDrugFromDeptDrugCmd_In(source, false, (long)AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        MainStorageInwardDrugFromDeptDrugCmd_In(source, false, (long)AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void InwardDrugFromDeptDrugVTYTTHCmd_In(object source, bool IsImportFromSubStorage = false, long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ISubStorageInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.SearchCriteria = new InwardInvoiceSearchCriteria { IsInputMedDept = true };
            VM.IsImportFromSubStorage = IsImportFromSubStorage;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void InwardDrugFromDeptDrugVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2506_G1_NhapNBVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugVTYTTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugVTYTTHCmd_In(source, false, (long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void InwardDrugFromDeptDrugVaccineCmd_In(object source, bool IsImportFromSubStorage = false, long V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ISubStorageInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.SearchCriteria = new InwardInvoiceSearchCriteria { IsInputMedDept = true };
            VM.IsImportFromSubStorage = IsImportFromSubStorage;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void InwardDrugFromDeptDrugVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2507_G1_NhapNBVaccine;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugVaccineCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugVaccineCmd_In(source, false, (long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void InwardDrugFromDeptDrugMauCmd_In(object source, bool IsImportFromSubStorage = false, long V_MedProductType = (long)AllLookupValues.MedProductType.MAU)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ISubStorageInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.SearchCriteria = new InwardInvoiceSearchCriteria { IsInputMedDept = true };
            VM.IsImportFromSubStorage = IsImportFromSubStorage;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void InwardDrugFromDeptDrugMauCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2508_G1_NhapNBMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugMauCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugMauCmd_In(source, false, (long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void InwardDrugFromDeptDrugThanhTrungCmd_In(object source, bool IsImportFromSubStorage = false, long V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ISubStorageInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.SearchCriteria = new InwardInvoiceSearchCriteria { IsInputMedDept = true };
            VM.IsImportFromSubStorage = IsImportFromSubStorage;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void InwardDrugFromDeptDrugThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2509_G1_NhapNBThanhTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugThanhTrungCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugThanhTrungCmd_In(source, false, (long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void InwardDrugFromDeptDrugHoaChatCmd_In(object source, bool IsImportFromSubStorage = false, long V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ISubStorageInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.SearchCriteria = new InwardInvoiceSearchCriteria { IsInputMedDept = true };
            VM.IsImportFromSubStorage = IsImportFromSubStorage;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void InwardDrugFromDeptDrugHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2510_G1_NhapNBHoaChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugHoaChatCmd_In(source, false, (long)AllLookupValues.MedProductType.HOA_CHAT);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void InwardDrugFromSubStoreCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2445_G1_NhapTraTuKhoLe;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugCmd_In(source, true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugCmd_In(source, true);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void InwardDrugFromDeptDrugYCCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2451_G1_NhapNoiBoYcu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugCmd_In(source, false, (long)AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugCmd_In(source, false, (long)AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void InwardDrugFromSubStoreYCCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2452_G1_NhapTraTuKhoLeYcu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugCmd_In(source, true, (long)AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugCmd_In(source, true, (long)AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void InwardFromSupplierToBloodDeptCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IBloodMedDeptInwardSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            VM.strHienThi = Globals.TitleForm;

            //VM.mTim = mNhapHangNCC_Thuoc_Tim;
            //VM.mThemMoi = mNhapHangNCC_Thuoc_ThemMoi;
            //VM.mCapNhat = mNhapHangNCC_Thuoc_CapNhat;
            //VM.mIn = mNhapHangNCC_Thuoc_In;
            VM.mTim = gf2NhapHangTuNCC;
            VM.mThemMoi = gf2NhapHangTuNCC;
            VM.mCapNhat = gf2NhapHangTuNCC;
            VM.mIn = gf2NhapHangTuNCC;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardFromSupplierToBloodDeptCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2487_G1_NhapTuNCCVaoKhoMau.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromSupplierToBloodDeptCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromSupplierToBloodDeptCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardFromSupplierToThanhTrungDeptCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IThanhTrungMedDeptInwardSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            VM.strHienThi = Globals.TitleForm;

            //VM.mTim = mNhapHangNCC_Thuoc_Tim;
            //VM.mThemMoi = mNhapHangNCC_Thuoc_ThemMoi;
            //VM.mCapNhat = mNhapHangNCC_Thuoc_CapNhat;
            //VM.mIn = mNhapHangNCC_Thuoc_In;
            VM.mTim = gf2NhapHangTuNCC;
            VM.mThemMoi = gf2NhapHangTuNCC;
            VM.mCapNhat = gf2NhapHangTuNCC;
            VM.mIn = gf2NhapHangTuNCC;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardFromSupplierToThanhTrungDeptCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2502_G1_NhapNCCThanhTrung.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromSupplierToThanhTrungDeptCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromSupplierToThanhTrungDeptCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardFromSupplierToVPPDeptCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IVPPMedDeptInwardSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            VM.strHienThi = Globals.TitleForm;

            //VM.mTim = mNhapHangNCC_Thuoc_Tim;
            //VM.mThemMoi = mNhapHangNCC_Thuoc_ThemMoi;
            //VM.mCapNhat = mNhapHangNCC_Thuoc_CapNhat;
            //VM.mIn = mNhapHangNCC_Thuoc_In;
            VM.mTim = gf2NhapHangTuNCC;
            VM.mThemMoi = gf2NhapHangTuNCC;
            VM.mCapNhat = gf2NhapHangTuNCC;
            VM.mIn = gf2NhapHangTuNCC;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardFromSupplierToVPPDeptCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2519_G1_NhapNCCVPP.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromSupplierToVPPDeptCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromSupplierToVPPDeptCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void InwardFromSupplierToVTTHDeptCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2546_G1_NhapNCCKhoVTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardFromSupplierToVTTHDeptCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardFromSupplierToVTTHDeptCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void InwardFromSupplierToVTTHDeptCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IVTTHMedDeptInwardSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            VM.strHienThi = Globals.TitleForm;

            //VM.mTim = mNhapHangNCC_Thuoc_Tim;
            //VM.mThemMoi = mNhapHangNCC_Thuoc_ThemMoi;
            //VM.mCapNhat = mNhapHangNCC_Thuoc_CapNhat;
            //VM.mIn = mNhapHangNCC_Thuoc_In;
            VM.mTim = gf2NhapHangTuNCC;
            VM.mThemMoi = gf2NhapHangTuNCC;
            VM.mCapNhat = gf2NhapHangTuNCC;
            VM.mIn = gf2NhapHangTuNCC;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        private void BCNXTTHUOCTHCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.CanSelectedWareHouse = false;
            VM.IsCheck = false;
            VM.eItem = ReportName.BC_NXT_THUOC_TONGHOP;
            VM.CanSelectedRefGenDrugCatID_1 = false;
            VM.ShowRangeOfHospital = true;
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void BCNXTTHUOCTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2619_G1_BCNXTTH_THUOC.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCNXTTHUOCTHCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCNXTTHUOCTHCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //BC NXT TOÀN KHOA DƯỢC
        private void BCNXTTHUOCTHCmd_In_V2(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<IDrugModule>();
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

        //▼====: #015
        private void BCThuocSapHetHanSDCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IDrugModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BC_ThuocSapHetHanDung;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCThuocSapHetHanSDCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2847_G1_BCThuocSapHetHanDung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThuocSapHetHanSDCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThuocSapHetHanSDCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #015
        //▼====: #016
        private void BCHangKhongXuatCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IDrugModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.KT_BCHangKhongXuat;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCHangKhongXuatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2854_G1_BCHangKhongXuat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCHangKhongXuatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCHangKhongXuatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #016
        //▼====: #017
        private void BCToaThuocHangNgayKDCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IDrugModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_ToaThuocHangNgay_KD;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.mXemIn = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCToaThuocHangNgayKDCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3023_G1_BCToaThuocHangNgayKD;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCToaThuocHangNgayKDCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCToaThuocHangNgayKDCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //▲====: #017
        #region Nhap Xuat Ton Member
        private void NhapXuatTonThuocCmd_KNT_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            VM.CanSelectedRefGenDrugCatID_1 = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonThuocCmd_KNT()
        {
            Globals.TitleForm = eHCMSResources.Z0772_G1_BCNXTThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonThuocCmd_KNT_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonThuocCmd_KNT_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonYCuCmd_KNT_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_YCu_XemIn;
            VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_YCu_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonYCuCmd_KNT()
        {
            Globals.TitleForm = eHCMSResources.Z0773_G1_BCNXTYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonYCuCmd_KNT_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonYCuCmd_KNT_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonHoaChatCmd_KNT_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_HoaChat_XemIn;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonHoaChatCmd_KNT()
        {
            Globals.TitleForm = eHCMSResources.Z0774_G1_BCNXTHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonHoaChatCmd_KNT_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonHoaChatCmd_KNT_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonCmd_KNT_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = V_MedProductType;
            //VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            //VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            VM.mBaoCaoXuatNhapTon_XemIn = gf1NhapXuatTon;
            VM.mBaoCaoXuatNhapTon_KetChuyen = gf1NhapXuatTon;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void NhapXuatTonVTYTTHCmd_KNT()
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapXuatTonVaccineCmd_KNT()
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVacXin;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapXuatTonBloodsCmd_KNT()
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapXuatTonResourcesCmd_KNT()
        {
            Globals.TitleForm = eHCMSResources.Z2549_G1_BCNXT_VTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapXuatTonThanhTrungCmd_KNT()
        {
            Globals.TitleForm = eHCMSResources.Z2516_G1_BCNXT_ThanhTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapXuatTonVPPCmd_KNT()
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVPP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_KNT_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region ThongTu22
        //▼====: #017
        //▼====: #011
        private void TT22BCKhangSinhReportCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            //VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.CanSelectedWareHouse = false;
            VM.IsCheck = false;
            VM.eItem = ReportName.TT22_BC_KhangSinh;
            VM.CanSelectedRefGenDrugCatID_1 = false;
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void TT22BCKhangSinhReportCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2673_G1_BC_KhangSinh.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TT22BCKhangSinhReportCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TT22BCKhangSinhReportCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #011
        //▼====: #012
        private void TT22BCSuDungThuocReportCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            //VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.CanSelectedWareHouse = false;
            VM.IsCheck = false;
            VM.eItem = ReportName.TT22_BC_SuDungThuoc;
            VM.CanSelectedRefGenDrugCatID_1 = false;
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void TT22BCSuDungThuocReportCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0946_G1_BCSDThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TT22BCSuDungThuocReportCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TT22BCSuDungThuocReportCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TT22BCSuDungHoaChatReportCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            //VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.CanSelectedWareHouse = false;
            VM.IsCheck = false;
            VM.eItem = ReportName.TT22_BC_SuDungHoaChat;
            VM.CanSelectedRefGenDrugCatID_1 = false;
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void TT22BCSuDungHoaChatReportCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2677_G1_BCSuDungHoaChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TT22BCSuDungHoaChatReportCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TT22BCSuDungHoaChatReportCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TT22BCSuDungVTYTTHReportCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            //VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.CanSelectedWareHouse = false;
            VM.IsCheck = false;
            VM.eItem = ReportName.TT22_BC_SuDungHoaChat;
            VM.CanSelectedRefGenDrugCatID_1 = false;
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void TT22BCSuDungVTYTTHReportCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2684_G1_BCSuDungVTYTTH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TT22BCSuDungVTYTTHReportCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TT22BCSuDungVTYTTHReportCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #012

        private void TT22BienBanKiemKeCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<IDrugModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.eItem = ReportName.TT22_BienBanKiemKe;
            reportVm.mXemChiTiet = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.ViewBy = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            Module.MainContent = reportVm;
            (Module as Conductor<object>).ActivateItem(reportVm);
        }

        public void TT22BienBanKiemKeCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2898_G1_BienBanKiemKe.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TT22BienBanKiemKeCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TT22BienBanKiemKeCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #017
        //▼====: #018
        private void GenDrugContraIndicateCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var ContraindicatorDrug = Globals.GetViewModel<IMedicalControl>();

            module.MainContent = ContraindicatorDrug;
            (module as Conductor<object>).ActivateItem(ContraindicatorDrug);
        }

        public void GenDrugContraIndicateCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1608_G1_QLyCCDThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                GenDrugContraIndicateCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        GenDrugContraIndicateCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #018
        #endregion

        //0: Thuoc, 1: Khac
        private bool[] _MenuVisibleCollection = new bool[] { true, true };
        public bool[] MenuVisibleCollection
        {
            get
            {
                return _MenuVisibleCollection;
            }
            set
            {
                if (_MenuVisibleCollection == value)
                {
                    return;
                }
                _MenuVisibleCollection = value;
                NotifyOfPropertyChange(() => MenuVisibleCollection);
                NotifyOfPropertyChange(() => gfXuatTheoToa);
                NotifyOfPropertyChange(() => mNhapThuocCmd);
                NotifyOfPropertyChange(() => mNhapYCuCmd);
                NotifyOfPropertyChange(() => mNhapHoaChatCmd);
                NotifyOfPropertyChange(() => gTempInwardReport_Med);
                NotifyOfPropertyChange(() => gTempInwardReport_Mat);
                NotifyOfPropertyChange(() => gf1BaoCaoNam);
                NotifyOfPropertyChange(() => mXNBThuocCmd);
                NotifyOfPropertyChange(() => mXNBYCuCmd);
                NotifyOfPropertyChange(() => mXNBHoaChatCmd);
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc);
                NotifyOfPropertyChange(() => mReturnThuocCmd);
                NotifyOfPropertyChange(() => mReturnYCuCmd);
                NotifyOfPropertyChange(() => mReturnHoaChatCmd);
                NotifyOfPropertyChange(() => mDemageThuocCmd);
                NotifyOfPropertyChange(() => mDemageYCuCmd);
                NotifyOfPropertyChange(() => mDemageHoaChatCmd);
                NotifyOfPropertyChange(() => mEstimationDrugDeptCmd);
                NotifyOfPropertyChange(() => mEstimationYCuCmd);
                NotifyOfPropertyChange(() => mEstimationChemicalCmd);
                NotifyOfPropertyChange(() => mOrderThuocCmd);
                NotifyOfPropertyChange(() => mOrderYCuCmd);
                NotifyOfPropertyChange(() => mOrderHoaChatCmd);
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierDrugCmd);
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierMedicalDeviceCmd);
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierChemicalCmd);
                NotifyOfPropertyChange(() => mCostThuocCmd);
                NotifyOfPropertyChange(() => mCostYCuCmd);
                NotifyOfPropertyChange(() => mCostHoaChatCmd);
                NotifyOfPropertyChange(() => mSuggestThuocCmd);
                NotifyOfPropertyChange(() => mSuggestYCuCmd);
                NotifyOfPropertyChange(() => mSuggestHoaChatCmd);
                NotifyOfPropertyChange(() => mKKThuocCmd);
                NotifyOfPropertyChange(() => mKKYCuCmd);
                NotifyOfPropertyChange(() => mKKHoaChatCmd);
                NotifyOfPropertyChange(() => mXuatThuocCmd);
                NotifyOfPropertyChange(() => mXuatYCuCmd);
                NotifyOfPropertyChange(() => mXuatHoaChatCmd);
                NotifyOfPropertyChange(() => mWatchMedCmd);
                NotifyOfPropertyChange(() => mWatchMatCmd);
                NotifyOfPropertyChange(() => mWatchLabCmd);
                NotifyOfPropertyChange(() => mNhapXuatTonThuocCmd);
                NotifyOfPropertyChange(() => mNhapXuatTonYCuCmd);
                NotifyOfPropertyChange(() => mNhapXuatTonHoaChatCmd);
                NotifyOfPropertyChange(() => mTheKhoThuocCmd);
                NotifyOfPropertyChange(() => mTheKhoYCuCmd);
                NotifyOfPropertyChange(() => mTheKhoHoaChatCmd);
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd);
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd);
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd);
                NotifyOfPropertyChange(() => mSuDungThuocCmd);
                NotifyOfPropertyChange(() => mThuocXuatDenKhoaCmd);
                NotifyOfPropertyChange(() => mYCuXuatDenKhoaCmd);
                NotifyOfPropertyChange(() => mHoaChatXuatDenKhoaCmd);
                NotifyOfPropertyChange(() => mQLNhomHang_Thuoc);
                NotifyOfPropertyChange(() => mQLNhomHang_YCu);
                NotifyOfPropertyChange(() => mQLNhomHang_HoaChat);
                NotifyOfPropertyChange(() => mQLNhomHang_HoatChat);
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_DrugMgnt);
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_MedicalDevicesMgnt);
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_ChemicalMgnt);
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_VTYTTHMgnt);
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_TiemNguaMgnt);
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_MauMgnt);
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_ThanhTrungMgnt);
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_VPPMgnt);
                NotifyOfPropertyChange(() => mRefGenMedProductDetails_VTTHMgnt);
                NotifyOfPropertyChange(() => mSupplierGenMedProductsPrice_Mgnt);
                NotifyOfPropertyChange(() => mSupplierGenMedProductsPrice_Medical_Mgnt);
                NotifyOfPropertyChange(() => mSupplierGenMedProductsPrice_Chemical_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellPriceProfitScale_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellPriceProfitScale_Medical_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellPriceProfitScale_Chemical_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingItemPrices_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingItemPrices_Medical_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingItemPrices_Chemical_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Medical_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Chemical_Mgnt);
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_Thuoc);
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_YCu);
                NotifyOfPropertyChange(() => mBaoCaoTheoDoiCongNo_HoaChat); 
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Thuoc);
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_YCu);
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_HoaChat);
                NotifyOfPropertyChange(() => mXuatHangKyGui_Thuoc);
                NotifyOfPropertyChange(() => mXuatHangKyGui_YCu);
                NotifyOfPropertyChange(() => mXuatHangKyGui_HoaChat);
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Thuoc);
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_YCu);
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_HoaChat);
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Thuoc);
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_YCu);
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_HoaChat);
                NotifyOfPropertyChange(() => mAdjustOutPrice_Med);
                NotifyOfPropertyChange(() => mAdjustOutPrice_Mat);
                NotifyOfPropertyChange(() => mAdjustOutPrice_Lab);
                NotifyOfPropertyChange(() => mReturnToSupplier_Drug);
                NotifyOfPropertyChange(() => mReturnToSupplier_Mat);
                NotifyOfPropertyChange(() => mAdjustOutPrice_MedCmd);
                NotifyOfPropertyChange(() => mAdjustOutPrice_MatCmd);
                NotifyOfPropertyChange(() => mAdjustOutPrice_LabCmd);
                NotifyOfPropertyChange(() => mInventory_MedCmd);
                NotifyOfPropertyChange(() => mInventory_MatCmd);
                NotifyOfPropertyChange(() => mInventory_LabCmd);
                NotifyOfPropertyChange(() => mUseFollowBid_MedCmd);
                NotifyOfPropertyChange(() => mUseFollowBid_MatCmd);
                NotifyOfPropertyChange(() => mBidDetail_Med);
                NotifyOfPropertyChange(() => mBidDetail_Mat);
                NotifyOfPropertyChange(() => mBidDetail_Chemical);
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierNutritionCmd);
                NotifyOfPropertyChange(() => mXNBDinhDuongCmd);
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_DinhDuong);
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_DDuong);
                NotifyOfPropertyChange(() => mNhapXuatTonDDuongCmd);
                NotifyOfPropertyChange(() => mTheKhoDDuongCmd);
                NotifyOfPropertyChange(() => mKKDDuongCmd);
                NotifyOfPropertyChange(() => mEstimationDDuongCmd);

                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_VTYTTH);
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Vaccine);
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Blood);
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_VPP);
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_VTTH);
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_ThanhTrung);

                NotifyOfPropertyChange(() => mInwardDrugFromSupplierVTYTTHCmd);
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierVaccineCmd);
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierBloodCmd);
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierVPPCmd);
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierVTTHCmd);
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierThanhTrungCmd);

                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_VTYTTH);
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Vaccine);
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Blood);
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_VPP);
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_VTTH);
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_ThanhTrung);

                NotifyOfPropertyChange(() => mNhapNoiBo);
                NotifyOfPropertyChange(() => mNhapTraTuKhoLe);

                NotifyOfPropertyChange(() => mXNBVTYTTHCmd);
                NotifyOfPropertyChange(() => mXNBVaccineCmd);
                NotifyOfPropertyChange(() => mXNBBloodCmd);
                NotifyOfPropertyChange(() => mXNBVPPCmd);
                NotifyOfPropertyChange(() => mXNBVTTHCmd);
                NotifyOfPropertyChange(() => mXNBThanhTrungCmd);

                NotifyOfPropertyChange(() => mReturnToSupplier_Chemical);
                NotifyOfPropertyChange(() => mReturnToSupplier_Nutri);
                NotifyOfPropertyChange(() => mReturnToSupplier_VTYTTH);
                NotifyOfPropertyChange(() => mReturnToSupplier_Vaccine);
                NotifyOfPropertyChange(() => mReturnToSupplier_Blood);
                NotifyOfPropertyChange(() => mReturnToSupplier_VPP);
                NotifyOfPropertyChange(() => mReturnToSupplier_VTTH);
                NotifyOfPropertyChange(() => mReturnToSupplier_ThanhTrung);

                NotifyOfPropertyChange(() => mReturnToSupplier);

                NotifyOfPropertyChange(() => mEstimationVTYTTHCmd);
                NotifyOfPropertyChange(() => mEstimationVaccineCmd);
                NotifyOfPropertyChange(() => mEstimationBloodCmd);
                NotifyOfPropertyChange(() => mEstimationVPPCmd);
                NotifyOfPropertyChange(() => mEstimationVTTHCmd);
                NotifyOfPropertyChange(() => mEstimationThanhTrungCmd);

                NotifyOfPropertyChange(() => mOrderVTYTTHCmd);
                NotifyOfPropertyChange(() => mOrderVaccineCmd);
                NotifyOfPropertyChange(() => mOrderBloodCmd);
                NotifyOfPropertyChange(() => mOrderVPPCmd);
                NotifyOfPropertyChange(() => mOrderVTTHCmd);
                NotifyOfPropertyChange(() => mOrderThanhTrungCmd);
                NotifyOfPropertyChange(() => mOrderDinhDuongCmd);

                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_VTYTTH_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Vaccine_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Blood_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_VPP_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_VTTH_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_ThanhTrung_Mgnt);
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_DinhDuong_Mgnt);

                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd);
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoVaccineCmd);
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoBloodCmd);
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd);
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoVTTHCmd);
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd);
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoDinhDuongCmd);

                NotifyOfPropertyChange(() => mNhapXuatTonVTYTTHCmd);
                NotifyOfPropertyChange(() => mNhapXuatTonVaccineCmd);
                NotifyOfPropertyChange(() => mNhapXuatTonBloodCmd);
                NotifyOfPropertyChange(() => mNhapXuatTonVPPCmd);
                NotifyOfPropertyChange(() => mNhapXuatTonVTTHCmd);
                NotifyOfPropertyChange(() => mNhapXuatTonThanhTrungCmd);

                NotifyOfPropertyChange(() => mTheKhoVTYTTHCmd);
                NotifyOfPropertyChange(() => mTheKhoVaccineCmd);
                NotifyOfPropertyChange(() => mTheKhoBloodCmd);
                NotifyOfPropertyChange(() => mTheKhoVPPCmd);
                NotifyOfPropertyChange(() => mTheKhoVTTHCmd);
                NotifyOfPropertyChange(() => mTheKhoThanhTrungCmd);

                NotifyOfPropertyChange(() => mXuatVTYTTHCmd);
                NotifyOfPropertyChange(() => mXuatVaccineCmd);
                NotifyOfPropertyChange(() => mXuatBloodCmd);
                NotifyOfPropertyChange(() => mXuatVPPCmd);
                NotifyOfPropertyChange(() => mXuatVTTHCmd);
                NotifyOfPropertyChange(() => mXuatThanhTrungCmd);

                NotifyOfPropertyChange(() => mNhapVTYTTHCmd);
                NotifyOfPropertyChange(() => mNhapVaccineCmd);
                NotifyOfPropertyChange(() => mNhapBloodCmd);
                NotifyOfPropertyChange(() => mNhapVPPCmd);
                NotifyOfPropertyChange(() => mNhapVTTHCmd);
                NotifyOfPropertyChange(() => mNhapThanhTrungCmd);

                NotifyOfPropertyChange(() => mBaoCaoHangKhongXuat);
                NotifyOfPropertyChange(() => mBaoCaoThuocHetHan);

                NotifyOfPropertyChange(() => gBaoCaoToaTreo);
                NotifyOfPropertyChange(() => mBaoCaoToaTreo_Duoc);
                NotifyOfPropertyChange(() => mBaoCaoToaTreo_NhaThuoc);
                NotifyOfPropertyChange(() => mBaoCaoToaTreo_KhoaPhong);

                NotifyOfPropertyChange(() => gBaoCaoKhac);
                NotifyOfPropertyChange(() => mBaoCaoToaThuocHangNgay);

                NotifyOfPropertyChange(() => mKKVTYTTHCmd);
                NotifyOfPropertyChange(() => mKKVaccineCmd);
                NotifyOfPropertyChange(() => mKKBloodCmd);
                NotifyOfPropertyChange(() => mKKVPPCmd);
                NotifyOfPropertyChange(() => mKKVTTHCmd);
                NotifyOfPropertyChange(() => mKKThanhTrungCmd);

                NotifyOfPropertyChange(() => gTangGiamKK);
                NotifyOfPropertyChange(() => mTangGiamKKThuocCmd);
                NotifyOfPropertyChange(() => mTangGiamKKYCuCmd);
                NotifyOfPropertyChange(() => mTangGiamKKHoaChatCmd);
                NotifyOfPropertyChange(() => mTangGiamKKDinhDuongCmd);
                NotifyOfPropertyChange(() => mTangGiamKKVTYTTHCmd);
                NotifyOfPropertyChange(() => mTangGiamKKVaccineCmd);
                NotifyOfPropertyChange(() => mTangGiamKKBloodCmd);
                NotifyOfPropertyChange(() => mTangGiamKKVPPCmd);
                NotifyOfPropertyChange(() => mTangGiamKKVTTHCmd);
                NotifyOfPropertyChange(() => mTangGiamKKThanhTrungCmd);

                NotifyOfPropertyChange(() => gQuanLy);
                NotifyOfPropertyChange(() => mQuanLyKho);
                NotifyOfPropertyChange(() => mDrugDeptSellPriceProfitScale_Nutrition_Mgnt);
                NotifyOfPropertyChange(() => gElectronicPrescription);
                NotifyOfPropertyChange(() => mElectronicPrescription);
                //▼==== #028
                NotifyOfPropertyChange(() => gBaoCaoXuatKhoaPhong);
                NotifyOfPropertyChange(() => mBC_XuatKP_Thuoc);
                NotifyOfPropertyChange(() => mBC_XuatKP_YCu);
                //▲==== #028
                //▼==== #029
                NotifyOfPropertyChange(() => mBCTThongKeDTDT);
                //▲==== #029
            }
        }
        #region Xuất cân bằng
        private void XCBMedProductTypeCmd_In(object source, long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC, int ViewCase = 1)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = V_MedProductType;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mXuat_Thuoc_Tim;
            UnitVM.mPhieuMoi = mXuat_Thuoc_PhieuMoi;
            UnitVM.mThucHien = mXuat_Thuoc_ThucHien;
            UnitVM.mThuTien = mXuat_Thuoc_ThuTien;
            UnitVM.mIn = mXuat_Thuoc_In;
            UnitVM.mDeleteInvoice = mXuat_Thuoc_DeleteInvoice;
            UnitVM.mPrintReceipt = mXuat_Thuoc_PrintReceipt;
            UnitVM.ViewCase = ViewCase;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XCBThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.THUOC, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.THUOC, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XCBYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.Y_CU, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.Y_CU, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //--▼--31/12/2020 DatTB
        public void XCBHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.HOA_CHAT, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.HOA_CHAT, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XCBVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VTYT_TIEUHAO, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VTYT_TIEUHAO, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XCBVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.TIEM_NGUA, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.TIEM_NGUA, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XCBMauCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.MAU, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.MAU, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XCBVPPCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XCBThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.THANHTRUNG, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.THANHTRUNG, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XCBVTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VATTU_TIEUHAO, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VATTU_TIEUHAO, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //--▲--31/12/2020 DatTB

        public void NCBThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.THUOC);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.THUOC);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NCBYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //--▼--31/12/2020 DatTB
        public void NCBHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.HOA_CHAT);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.HOA_CHAT);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NCBVTYTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NCBVaccineCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NCBMauCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NCBVPPCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NCBThanhTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NCBVTTHCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //--▲--31/12/2020 DatTB

        private void NCBMedProductTypeCmd_In(object source, long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptInwardBalance>();
            UnitVM.V_MedProductType = V_MedProductType;
            UnitVM.strHienThi = Globals.TitleForm;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        #endregion
        private void BCToaTreoCmd_KD_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            VM.eItem = ReportName.XRptPhieuTreo;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void BCToaTreoCmd_KD(object source)
        {
            Globals.TitleForm = "BÁO CÁO TOA TREO - KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCToaTreoCmd_KD_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCToaTreoCmd_KD_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BCToaTreoCmd_KNT_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.XRptPhieuTreo;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void BCToaTreoCmd_KNT(object source)
        {
            Globals.TitleForm = "BÁO CÁO TOA TREO - KHOA NỘI TRÚ";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCToaTreoCmd_KNT_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCToaTreoCmd_KNT_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BCToaTreoCmd_NT_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IReportByDDMMYYYY>();
            VM.eItem = ReportName.XRptPhieuTreo;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void BCToaTreoCmd_NT(object source)
        {
            Globals.TitleForm = "BÁO CÁO TOA TREO - NHÀ THUỐC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCToaTreoCmd_NT_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCToaTreoCmd_NT_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        
        //▼==== #028
        private void BC_XuatKP_ThuocCmd_KD_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            VM.eItem = ReportName.BC_XuatKP_Thuoc;
            VM.isAllStaff = false;
            VM.RptParameters.HideFindPatient = false;
            VM.ChonKho = true;
            VM.ChonKhoNhan = true;
            VM.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            VM.GetListInStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void BC_XuatKP_ThuocCmd_KD(object source)
        {
            Globals.TitleForm = "BÁO CÁO XUẤT THUỐC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BC_XuatKP_ThuocCmd_KD_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BC_XuatKP_ThuocCmd_KD_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BC_XuatKP_YCuCmd_KD_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            VM.eItem = ReportName.BC_XuatKP_YCu;
            VM.isAllStaff = false;
            VM.RptParameters.HideFindPatient = false;
            VM.ChonKho = true;
            VM.ChonKhoNhan = true;
            VM.GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            VM.GetListInStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void BC_XuatKP_YCuCmd_KD(object source)
        {
            Globals.TitleForm = "BÁO CÁO XUẤT Y CỤ";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BC_XuatKP_YCuCmd_KD_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BC_XuatKP_YCuCmd_KD_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲==== #028

        /*▼====: #019*/
        private void InwardDrugFromSupplierNutritionCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptInwardDrugSupplier>();
            //VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.V_MedProductType = 11014;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mNhapHangNCC_DinhDuong_Tim;
            VM.mThemMoi = mNhapHangNCC_DinhDuong_ThemMoi;
            VM.mCapNhat = mNhapHangNCC_DinhDuong_CapNhat;
            VM.mIn = mNhapHangNCC_DinhDuong_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierNutritionCmd(object source)
        {
            //Globals.TitleForm = eHCMSResources.Z0842_G1_NhapHChatTuNCC.ToUpper();
            Globals.TitleForm = "NHẬP DINH DƯỠNG TỪ NCC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierNutritionCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierNutritionCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private bool _mXNBDinhDuongCmd = true;
        private bool _mXuat_DinhDuong_Tim = true;
        private bool _mXuat_DinhDuong_PhieuMoi = true;
        private bool _mXuat_DinhDuong_ThucHien = true;
        private bool _mXuat_DinhDuong_ThuTien = true;
        private bool _mXuat_DinhDuong_In = true;
        private bool _mXuat_DinhDuong_DeleteInvoice = true;
        private bool _mXuat_DinhDuong_PrintReceipt = true;

        public bool mXNBDinhDuongCmd
        {
            get
            {
                return _mXNBDinhDuongCmd && MenuVisibleCollection[0];
            }
            set
            {
                if (_mXNBDinhDuongCmd == value)
                    return;
                _mXNBDinhDuongCmd = value;
                NotifyOfPropertyChange(() => mXNBDinhDuongCmd);
            }
        }

        public bool mXuat_DinhDuong_Tim
        {
            get
            {
                return _mXuat_DinhDuong_Tim;
            }
            set
            {
                if (_mXuat_DinhDuong_Tim == value)
                    return;
                _mXuat_DinhDuong_Tim = value;
                NotifyOfPropertyChange(() => mXuat_DinhDuong_Tim);
            }
        }
        public bool mXuat_DinhDuong_PhieuMoi
        {
            get
            {
                return _mXuat_DinhDuong_PhieuMoi;
            }
            set
            {
                if (_mXuat_DinhDuong_PhieuMoi == value)
                    return;
                _mXuat_DinhDuong_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuat_DinhDuong_PhieuMoi);
            }
        }
        public bool mXuat_DinhDuong_ThuTien
        {
            get
            {
                return _mXuat_DinhDuong_ThuTien;
            }
            set
            {
                if (_mXuat_DinhDuong_ThuTien == value)
                    return;
                _mXuat_DinhDuong_ThuTien = value;
                NotifyOfPropertyChange(() => mXuat_DinhDuong_ThuTien);
            }
        }
        public bool mXuat_DinhDuong_ThucHien
        {
            get
            {
                return _mXuat_DinhDuong_ThucHien;
            }
            set
            {
                if (_mXuat_DinhDuong_ThucHien == value)
                    return;
                _mXuat_DinhDuong_ThucHien = value;
                NotifyOfPropertyChange(() => mXuat_DinhDuong_ThucHien);
            }
        }
        public bool mXuat_DinhDuong_In
        {
            get
            {
                return _mXuat_DinhDuong_In;
            }
            set
            {
                if (_mXuat_DinhDuong_In == value)
                    return;
                _mXuat_DinhDuong_In = value;
                NotifyOfPropertyChange(() => mXuat_DinhDuong_In);
            }
        }
        public bool mXuat_DinhDuong_DeleteInvoice
        {
            get
            {
                return _mXuat_DinhDuong_DeleteInvoice;
            }
            set
            {
                if (_mXuat_DinhDuong_DeleteInvoice == value)
                    return;
                _mXuat_DinhDuong_DeleteInvoice = value;
                NotifyOfPropertyChange(() => mXuat_DinhDuong_DeleteInvoice);
            }
        }
        public bool mXuat_DinhDuong_PrintReceipt
        {
            get
            {
                return _mXuat_DinhDuong_PrintReceipt;
            }
            set
            {
                if (_mXuat_DinhDuong_PrintReceipt == value)
                    return;
                _mXuat_DinhDuong_PrintReceipt = value;
                NotifyOfPropertyChange(() => mXuat_DinhDuong_PrintReceipt);
            }
        }

        private void XNBDinhDuongCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mXuat_DinhDuong_Tim;
            UnitVM.mPhieuMoi = mXuat_DinhDuong_PhieuMoi;
            UnitVM.mThucHien = mXuat_DinhDuong_ThucHien;
            UnitVM.mThuTien = mXuat_DinhDuong_ThuTien;
            UnitVM.mIn = mXuat_DinhDuong_In;
            UnitVM.mDeleteInvoice = mXuat_DinhDuong_DeleteInvoice;
            UnitVM.mPrintReceipt = mXuat_DinhDuong_PrintReceipt;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBDinhDuongCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3207_G1_XuatDinhDuongTuKhDuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XNBDinhDuongCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XNBDinhDuongCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void InwardDrugFromDeptDrugDDCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3214_G1_NhapNoiBoDDuong;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugCmd_In(source, false, (long)AllLookupValues.MedProductType.NUTRITION);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugCmd_In(source, false, (long)AllLookupValues.MedProductType.NUTRITION);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void InwardDrugFromSubStoreDDCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3215_G1_NhapTraTuKhoLeDDuong;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromDeptDrugCmd_In(source, true, (long)AllLookupValues.MedProductType.NUTRITION);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromDeptDrugCmd_In(source, true, (long)AllLookupValues.MedProductType.NUTRITION);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private bool _mDuyetPhieuLinhHang_DinhDuong = true;
        private bool _mDuyetPhieuLinhHang_DDuong_Tim = true;
        private bool _mDuyetPhieuLinhHang_DDuong_PhieuMoi = true;
        private bool _mDuyetPhieuLinhHang_DDuong_XuatHang = true;
        private bool _mDuyetPhieuLinhHang_DDuong_XemInTH = true;
        private bool _mDuyetPhieuLinhHang_DDuong_XemInCT = true;
        public bool mDuyetPhieuLinhHang_DinhDuong
        {
            get
            {
                return _mDuyetPhieuLinhHang_DinhDuong && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_DinhDuong == value)
                    return;
                _mDuyetPhieuLinhHang_DinhDuong = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_DinhDuong);
            }
        }

        public bool mDuyetPhieuLinhHang_DDuong_Tim
        {
            get
            {
                return _mDuyetPhieuLinhHang_DDuong_Tim;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_DDuong_Tim == value)
                    return;
                _mDuyetPhieuLinhHang_DDuong_Tim = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_DDuong_Tim);
            }
        }

        public bool mDuyetPhieuLinhHang_DDuong_PhieuMoi
        {
            get
            {
                return _mDuyetPhieuLinhHang_DDuong_PhieuMoi;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_DDuong_PhieuMoi == value)
                    return;
                _mDuyetPhieuLinhHang_DDuong_PhieuMoi = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_DDuong_PhieuMoi);
            }
        }

        public bool mDuyetPhieuLinhHang_DDuong_XuatHang
        {
            get
            {
                return _mDuyetPhieuLinhHang_DDuong_XuatHang;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_DDuong_XuatHang == value)
                    return;
                _mDuyetPhieuLinhHang_DDuong_XuatHang = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_DDuong_XuatHang);
            }
        }

        public bool mDuyetPhieuLinhHang_DDuong_XemInTH
        {
            get
            {
                return _mDuyetPhieuLinhHang_DDuong_XemInTH;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_DDuong_XemInTH == value)
                    return;
                _mDuyetPhieuLinhHang_DDuong_XemInTH = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_DDuong_XemInTH);
            }
        }

        public bool mDuyetPhieuLinhHang_DDuong_XemInCT
        {
            get
            {
                return _mDuyetPhieuLinhHang_DDuong_XemInCT;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_DDuong_XemInCT == value)
                    return;
                _mDuyetPhieuLinhHang_DDuong_XemInCT = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_DDuong_XemInCT);
            }
        }

        private void RequestNewDDuongCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IRequestNew>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mDuyetPhieuLinhHang_Tim = mDuyetPhieuLinhHang_DDuong_Tim;
            UnitVM.mDuyetPhieuLinhHang_PhieuMoi = mDuyetPhieuLinhHang_DDuong_PhieuMoi;
            UnitVM.mDuyetPhieuLinhHang_XuatHang = mDuyetPhieuLinhHang_DDuong_XuatHang;
            UnitVM.mDuyetPhieuLinhHang_XemInTH = mDuyetPhieuLinhHang_DDuong_XemInTH;
            UnitVM.mDuyetPhieuLinhHang_XemInCT = mDuyetPhieuLinhHang_DDuong_XemInCT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void RequestNewDDuongCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3216_G1_DuyetPhLinhDDuong.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestNewDDuongCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestNewDDuongCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private bool _mNhapTraTuKhoPhong_DDuong_Tim = true;
        private bool _mNhapTraTuKhoPhong_DDuong_PhieuMoi = true;
        private bool _mNhapTraTuKhoPhong_DDuong_XemIn = true;
        private bool _mNhapTraTuKhoPhong_DDuong_In = true;
        private bool _mNhapTraTuKhoPhong_DDuong = true;
        public bool mNhapTraTuKhoPhong_DDuong
        {
            get
            {
                return _mNhapTraTuKhoPhong_DDuong && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_DDuong == value)
                    return;
                _mNhapTraTuKhoPhong_DDuong = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_DDuong);
            }
        }
        public bool mNhapTraTuKhoPhong_DDuong_Tim
        {
            get
            {
                return _mNhapTraTuKhoPhong_DDuong_Tim;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_DDuong_Tim == value)
                    return;
                _mNhapTraTuKhoPhong_DDuong_Tim = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_DDuong_Tim);
            }
        }


        public bool mNhapTraTuKhoPhong_DDuong_PhieuMoi
        {
            get
            {
                return _mNhapTraTuKhoPhong_DDuong_PhieuMoi;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_DDuong_PhieuMoi == value)
                    return;
                _mNhapTraTuKhoPhong_DDuong_PhieuMoi = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_DDuong_PhieuMoi);
            }
        }


        public bool mNhapTraTuKhoPhong_DDuong_XemIn
        {
            get
            {
                return _mNhapTraTuKhoPhong_DDuong_XemIn;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_DDuong_XemIn == value)
                    return;
                _mNhapTraTuKhoPhong_DDuong_XemIn = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_DDuong_XemIn);
            }
        }


        public bool mNhapTraTuKhoPhong_DDuong_In
        {
            get
            {
                return _mNhapTraTuKhoPhong_DDuong_In;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_DDuong_In == value)
                    return;
                _mNhapTraTuKhoPhong_DDuong_In = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_DDuong_In);
            }
        }

        private void InwardDrugFromSupplierNutritionClinicCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            VM.strHienThi = Globals.TitleForm;
            VM.mNhapTraTuKhoPhong_Tim = mNhapTraTuKhoPhong_DDuong_Tim;
            VM.mNhapTraTuKhoPhong_PhieuMoi = mNhapTraTuKhoPhong_DDuong_PhieuMoi;
            VM.mNhapTraTuKhoPhong_XemIn = mNhapTraTuKhoPhong_DDuong_XemIn;
            VM.mNhapTraTuKhoPhong_In = mNhapTraTuKhoPhong_DDuong_In;
            //▼====== #009
            VM.vNhapTraKhoBHYT = false;
            //▲====== #009
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierNutritionClinicCmd(object source)
        {
            Globals.TitleForm = "NHẬP TRẢ DINH DƯỠNG TỪ KHO KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierNutritionClinicCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierNutritionClinicCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private bool _mNhapXuatTonDDuongCmd = true;
        private bool _mBaoCaoXuatNhapTon_DDuong_XemIn = true;
        private bool _mBaoCaoXuatNhapTon_DDuong_KetChuyen = true;

        public bool mNhapXuatTonDDuongCmd
        {
            get
            {
                return _mNhapXuatTonDDuongCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapXuatTonDDuongCmd == value)
                    return;
                _mNhapXuatTonDDuongCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonDDuongCmd);
            }
        }

        public bool mBaoCaoXuatNhapTon_DDuong_XemIn
        {
            get
            {
                return _mBaoCaoXuatNhapTon_DDuong_XemIn;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_DDuong_XemIn == value)
                    return;
                _mBaoCaoXuatNhapTon_DDuong_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_DDuong_XemIn);
            }
        }


        public bool mBaoCaoXuatNhapTon_DDuong_KetChuyen
        {
            get
            {
                return _mBaoCaoXuatNhapTon_DDuong_KetChuyen;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_DDuong_KetChuyen == value)
                    return;
                _mBaoCaoXuatNhapTon_DDuong_KetChuyen = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_DDuong_KetChuyen);
            }
        }

        private void NhapXuatTonDDuongCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            /*▼====: #004*/
            VM.LoadDrugDeptProductGroupReportTypes();
            /*▲====: #004*/
            VM.strHienThi = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuatNhapTon_DDuong_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_DDuong_KetChuyen;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonDDuongCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3220_G1_BCNXTDDuong.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonDDuongCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonDDuongCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DTNhapXuatTonDDuongCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            /*▼====: #004*/
            VM.LoadDrugDeptProductGroupReportTypes();
            /*▲====: #004*/
            VM.strHienThi = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuatNhapTon_DDuong_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_DDuong_KetChuyen;
            VM.IsGetValue = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void DTNhapXuatTonDDuongCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3222_G1_DoanhThuNXTDDuong.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DTNhapXuatTonDDuongCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DTNhapXuatTonDDuongCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private bool _mTheKhoDDuongCmd = true;
        private bool _mBaoCaoTheKho_DDuong_Xem = true;
        private bool _mBaoCaoTheKho_DDuong_In = true;
        public bool mTheKhoDDuongCmd
        {
            get
            {
                return _mTheKhoDDuongCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTheKhoDDuongCmd == value)
                    return;
                _mTheKhoDDuongCmd = value;
                NotifyOfPropertyChange(() => mTheKhoDDuongCmd);
            }
        }

        public bool mBaoCaoTheKho_DDuong_Xem
        {
            get
            {
                return _mBaoCaoTheKho_DDuong_Xem;
            }
            set
            {
                if (_mBaoCaoTheKho_DDuong_Xem == value)
                    return;
                _mBaoCaoTheKho_DDuong_Xem = value;
                NotifyOfPropertyChange(() => mBaoCaoTheKho_DDuong_Xem);
            }
        }


        public bool mBaoCaoTheKho_DDuong_In
        {
            get
            {
                return _mBaoCaoTheKho_DDuong_In;
            }
            set
            {
                if (_mBaoCaoTheKho_DDuong_In == value)
                    return;
                _mBaoCaoTheKho_DDuong_In = value;
                NotifyOfPropertyChange(() => mBaoCaoTheKho_DDuong_In);
            }
        }

        private void TheKhoDDuongCmd_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptTheKho>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            VM.strHienThi = Globals.TitleForm;
            VM.mXem = mBaoCaoTheKho_DDuong_Xem;
            VM.mIn = mBaoCaoTheKho_DDuong_In;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TheKhoDDuongCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3223_G1_TheKhoDDuong.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoDDuongCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoDDuongCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void ReturnNutritionFromInwardSupplierCmd(object source)
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2602_G1_XuatTraChoNCC.ToUpper(), eHCMSResources.Z3206_G1_DinhDuong.ToUpper());
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReturnFromInwardSupplierCmd_In((long)AllLookupValues.MedProductType.NUTRITION);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReturnFromInwardSupplierCmd_In((long)AllLookupValues.MedProductType.NUTRITION);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Tăng/Giảm KK Dinh dưỡng
        public void XCBDDuongCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.NUTRITION, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.NUTRITION, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NCBDDuongCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.NUTRITION);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NCBMedProductTypeCmd_In(source, (long)AllLookupValues.MedProductType.NUTRITION);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //KK Dinh Dưỡng
        private bool _mKKDDuongCmd = true;
        private bool _mKiemKe_DDuong_Tim = true;
        private bool _mKiemKe_DDuong_ThemMoi = true;
        private bool _mKiemKe_DDuong_XuatExcel = true;
        private bool _mKiemKe_DDuong_XemIn = true;

        public bool mKKDDuongCmd
        {
            get
            {
                return _mKKDDuongCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mKKDDuongCmd == value)
                    return;
                _mKKDDuongCmd = value;
                NotifyOfPropertyChange(() => mKKDDuongCmd
                );
            }
        }

        public bool mKiemKe_DDuong_Tim
        {
            get
            {
                return _mKiemKe_DDuong_Tim;
            }
            set
            {
                if (_mKiemKe_DDuong_Tim == value)
                    return;
                _mKiemKe_DDuong_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_DDuong_Tim);
            }
        }


        public bool mKiemKe_DDuong_ThemMoi
        {
            get
            {
                return _mKiemKe_DDuong_ThemMoi;
            }
            set
            {
                if (_mKiemKe_DDuong_ThemMoi == value)
                    return;
                _mKiemKe_DDuong_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_DDuong_ThemMoi);
            }
        }


        public bool mKiemKe_DDuong_XuatExcel
        {
            get
            {
                return _mKiemKe_DDuong_XuatExcel;
            }
            set
            {
                if (_mKiemKe_DDuong_XuatExcel == value)
                    return;
                _mKiemKe_DDuong_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_DDuong_XuatExcel);
            }
        }


        public bool mKiemKe_DDuong_XemIn
        {
            get
            {
                return _mKiemKe_DDuong_XemIn;
            }
            set
            {
                if (_mKiemKe_DDuong_XemIn == value)
                    return;
                _mKiemKe_DDuong_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_DDuong_XemIn);
            }
        }

        private void KKDDuongCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            VM.strHienThi = Globals.TitleForm;

            VM.mTim = mKiemKe_DDuong_Tim;
            VM.mThemMoi = mKiemKe_DDuong_ThemMoi;
            VM.mXuatExcel = mKiemKe_DDuong_XuatExcel;
            VM.mXemIn = mKiemKe_DDuong_XemIn;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKDDuongCmd(object source)
        {
            Globals.TitleForm = "TÍNH LẠI TỒN ĐẦU KỲ DINH DƯỠNG KHOA DƯỢC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKDDuongCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKDDuongCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Dự trù
        private bool _mEstimationDDuongCmd = true;
        private bool _mDuTru_DDuong_Tim = true;
        private bool _mDuTru_DDuong_ThemMoi = true;
        private bool _mDuTru_DDuong_Xoa = true;
        private bool _mDuTru_DDuong_XemIn = true;

        public bool mEstimationDDuongCmd
        {
            get
            {
                return _mEstimationDDuongCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mEstimationDDuongCmd == value)
                    return;
                _mEstimationDDuongCmd = value;
                NotifyOfPropertyChange(() => mEstimationDDuongCmd
                );
            }
        }

        public bool mDuTru_DDuong_Tim
        {
            get
            {
                return _mDuTru_DDuong_Tim;
            }
            set
            {
                if (_mDuTru_DDuong_Tim == value)
                    return;
                _mDuTru_DDuong_Tim = value;
                NotifyOfPropertyChange(() => mDuTru_DDuong_Tim);
            }
        }


        public bool mDuTru_DDuong_ThemMoi
        {
            get
            {
                return _mDuTru_DDuong_ThemMoi;
            }
            set
            {
                if (_mDuTru_DDuong_ThemMoi == value)
                    return;
                _mDuTru_DDuong_ThemMoi = value;
                NotifyOfPropertyChange(() => mDuTru_DDuong_ThemMoi);
            }
        }


        public bool mDuTru_DDuong_Xoa
        {
            get
            {
                return _mDuTru_DDuong_Xoa;
            }
            set
            {
                if (_mDuTru_DDuong_Xoa == value)
                    return;
                _mDuTru_YCu_Xoa = value;
                NotifyOfPropertyChange(() => mDuTru_DDuong_Xoa);
            }
        }


        public bool mDuTru_DDuong_XemIn
        {
            get
            {
                return _mDuTru_DDuong_XemIn;
            }
            set
            {
                if (_mDuTru_DDuong_XemIn == value)
                    return;
                _mDuTru_DDuong_XemIn = value;
                NotifyOfPropertyChange(() => mDuTru_DDuong_XemIn);
            }
        }

        private void EstimationDDuongCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mDuTru_DDuong_Tim;
            UnitVM.mThemMoi = mDuTru_DDuong_ThemMoi;
            UnitVM.mXoa = mDuTru_DDuong_Xoa;
            UnitVM.mXemIn = mDuTru_DDuong_XemIn;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void EstimationDDuongCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3225_G1_DuTruDDuong.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationDDuongCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationDDuongCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private bool _mThangGiaBan_DinhDuong_Tim = true;
        private bool _mThangGiaBan_DinhDuong_TaoMoiCTGia = true;
        private bool _mThangGiaBan_DinhDuong_ChinhSua = true;

        public bool mThangGiaBan_DinhDuong_Tim
        {
            get
            {
                return _mThangGiaBan_DinhDuong_Tim;
            }
            set
            {
                if (_mThangGiaBan_DinhDuong_Tim == value)
                    return;
                _mThangGiaBan_DinhDuong_Tim = value;
                NotifyOfPropertyChange(() => mThangGiaBan_DinhDuong_Tim);
            }
        }


        public bool mThangGiaBan_DinhDuong_TaoMoiCTGia
        {
            get
            {
                return _mThangGiaBan_DinhDuong_TaoMoiCTGia;
            }
            set
            {
                if (_mThangGiaBan_DinhDuong_TaoMoiCTGia == value)
                    return;
                _mThangGiaBan_DinhDuong_TaoMoiCTGia = value;
                NotifyOfPropertyChange(() => mThangGiaBan_DinhDuong_TaoMoiCTGia);
            }
        }


        public bool mThangGiaBan_DinhDuong_ChinhSua
        {
            get
            {
                return _mThangGiaBan_DinhDuong_ChinhSua;
            }
            set
            {
                if (_mThangGiaBan_DinhDuong_ChinhSua == value)
                    return;
                _mThangGiaBan_DinhDuong_ChinhSua = value;
                NotifyOfPropertyChange(() => mThangGiaBan_DinhDuong_ChinhSua);
            }
        }

        private bool _mDrugDeptSellPriceProfitScale_Nutrition_Mgnt = true;
        public bool mDrugDeptSellPriceProfitScale_Nutrition_Mgnt
        {
            get
            {
                return _mDrugDeptSellPriceProfitScale_Nutrition_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellPriceProfitScale_Nutrition_Mgnt
                 == value)
                    return;
                _mDrugDeptSellPriceProfitScale_Nutrition_Mgnt
                 = value;
                NotifyOfPropertyChange(() => mDrugDeptSellPriceProfitScale_Nutrition_Mgnt
                );
            }
        }
        private bool _mQuanLyThuocKemTheoCLS_Mgnt = true;
        public bool mQuanLyThuocKemTheoCLS_Mgnt
        {
            get
            {
                return _mQuanLyThuocKemTheoCLS_Mgnt;
            }
            set
            {
                if (_mQuanLyThuocKemTheoCLS_Mgnt == value)
                    return;
                _mQuanLyThuocKemTheoCLS_Mgnt = value;
                NotifyOfPropertyChange(() => mQuanLyThuocKemTheoCLS_Mgnt);
            }
        }

        private void DrugDeptSellPriceProfitScale_Nutrition_Mgnt_In(object source)
        {


            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptSellPriceProfitScale>();

            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            VM.TitleForm = Globals.TitleForm;

            VM.mTim = mThangGiaBan_DinhDuong_Tim;
            VM.mTaoMoiCTGia = mThangGiaBan_DinhDuong_TaoMoiCTGia;
            VM.mChinhSua = mThangGiaBan_DinhDuong_ChinhSua;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
            VM.Init();
        }

        public void DrugDeptSellPriceProfitScale_Nutrition_Mgnt(object source)
        {
            Globals.TitleForm = "Thang giá Dinh dưỡng";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugDeptSellPriceProfitScale_Nutrition_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugDeptSellPriceProfitScale_Nutrition_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #019

        //▼======#020
        private bool _mDuyetPhieuLinhHang_VTYTTH = true;
        private bool _mDuyetPhieuLinhHang_Vaccine = true;
        private bool _mDuyetPhieuLinhHang_Blood = true;
        private bool _mDuyetPhieuLinhHang_VPP = true;
        private bool _mDuyetPhieuLinhHang_VTTH = true;
        private bool _mDuyetPhieuLinhHang_ThanhTrung = true;

        public bool mDuyetPhieuLinhHang_VTYTTH
        {
            get
            {
                return _mDuyetPhieuLinhHang_VTYTTH && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_VTYTTH == value)
                    return;
                _mDuyetPhieuLinhHang_VTYTTH = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_VTYTTH);
            }
        }

        public bool mDuyetPhieuLinhHang_Vaccine
        {
            get
            {
                return _mDuyetPhieuLinhHang_Vaccine && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_Vaccine == value)
                    return;
                _mDuyetPhieuLinhHang_Vaccine = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Vaccine);
            }
        }

        public bool mDuyetPhieuLinhHang_Blood
        {
            get
            {
                return _mDuyetPhieuLinhHang_Blood && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_Blood == value)
                    return;
                _mDuyetPhieuLinhHang_Blood = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Blood);
            }
        }

        public bool mDuyetPhieuLinhHang_VPP
        {
            get
            {
                return _mDuyetPhieuLinhHang_VPP && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_VPP == value)
                    return;
                _mDuyetPhieuLinhHang_VPP = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_VPP);
            }
        }

        public bool mDuyetPhieuLinhHang_VTTH
        {
            get
            {
                return _mDuyetPhieuLinhHang_VTTH && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_VTTH == value)
                    return;
                _mDuyetPhieuLinhHang_VTTH = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_VTTH);
            }
        }

        public bool mDuyetPhieuLinhHang_ThanhTrung
        {
            get
            {
                return _mDuyetPhieuLinhHang_ThanhTrung && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDuyetPhieuLinhHang_ThanhTrung == value)
                    return;
                _mDuyetPhieuLinhHang_ThanhTrung = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_ThanhTrung);
            }
        }
        //▲======#020

        //▼======#021
        private bool _mInwardDrugFromSupplierVTYTTHCmd = true;
        private bool _mInwardDrugFromSupplierVaccineCmd = true;
        private bool _mInwardDrugFromSupplierBloodCmd = true;
        private bool _mInwardDrugFromSupplierVPPCmd = true;
        private bool _mInwardDrugFromSupplierVTTHCmd = true;
        private bool _mInwardDrugFromSupplierThanhTrungCmd = true;

        public bool mInwardDrugFromSupplierVTYTTHCmd
        {
            get
            {
                return _mInwardDrugFromSupplierVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mInwardDrugFromSupplierVTYTTHCmd == value)
                    return;
                _mInwardDrugFromSupplierVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierVTYTTHCmd);
            }
        }

        public bool mInwardDrugFromSupplierVaccineCmd
        {
            get
            {
                return _mInwardDrugFromSupplierVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mInwardDrugFromSupplierVaccineCmd == value)
                    return;
                _mInwardDrugFromSupplierVaccineCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierVaccineCmd);
            }
        }

        public bool mInwardDrugFromSupplierBloodCmd
        {
            get
            {
                return _mInwardDrugFromSupplierBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mInwardDrugFromSupplierBloodCmd == value)
                    return;
                _mInwardDrugFromSupplierBloodCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierBloodCmd);
            }
        }

        public bool mInwardDrugFromSupplierVPPCmd
        {
            get
            {
                return _mInwardDrugFromSupplierVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mInwardDrugFromSupplierVPPCmd == value)
                    return;
                _mInwardDrugFromSupplierVPPCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierVPPCmd);
            }
        }


        public bool mInwardDrugFromSupplierVTTHCmd
        {
            get
            {
                return _mInwardDrugFromSupplierVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mInwardDrugFromSupplierVTTHCmd == value)
                    return;
                _mInwardDrugFromSupplierVTTHCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierVTTHCmd);
            }
        }

        public bool mInwardDrugFromSupplierThanhTrungCmd
        {
            get
            {
                return _mInwardDrugFromSupplierThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mInwardDrugFromSupplierThanhTrungCmd == value)
                    return;
                _mInwardDrugFromSupplierThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mInwardDrugFromSupplierThanhTrungCmd);
            }
        }

        private bool _mNhapTraTuKhoPhong_VTYTTH = true;
        private bool _mNhapTraTuKhoPhong_Vaccine = true;
        private bool _mNhapTraTuKhoPhong_Blood = true;
        private bool _mNhapTraTuKhoPhong_VPP = true;
        private bool _mNhapTraTuKhoPhong_VTTH = true;
        private bool _mNhapTraTuKhoPhong_ThanhTrung = true;
        public bool mNhapTraTuKhoPhong_VTYTTH
        {
            get
            {
                return _mNhapTraTuKhoPhong_VTYTTH && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_VTYTTH == value)
                    return;
                _mNhapTraTuKhoPhong_VTYTTH = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_VTYTTH);
            }
        }

        public bool mNhapTraTuKhoPhong_Vaccine
        {
            get
            {
                return _mNhapTraTuKhoPhong_Vaccine && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_Vaccine == value)
                    return;
                _mNhapTraTuKhoPhong_Vaccine = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Vaccine);
            }
        }

        public bool mNhapTraTuKhoPhong_Blood
        {
            get
            {
                return _mNhapTraTuKhoPhong_Blood && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_Blood == value)
                    return;
                _mNhapTraTuKhoPhong_Blood = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Blood);
            }
        }

        public bool mNhapTraTuKhoPhong_VPP
        {
            get
            {
                return _mNhapTraTuKhoPhong_VPP && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_VPP == value)
                    return;
                _mNhapTraTuKhoPhong_VPP = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_VPP);
            }
        }

        public bool mNhapTraTuKhoPhong_VTTH
        {
            get
            {
                return _mNhapTraTuKhoPhong_VTTH && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_VTTH == value)
                    return;
                _mNhapTraTuKhoPhong_VTTH = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_VTTH);
            }
        }

        public bool mNhapTraTuKhoPhong_ThanhTrung
        {
            get
            {
                return _mNhapTraTuKhoPhong_ThanhTrung && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoPhong_ThanhTrung == value)
                    return;
                _mNhapTraTuKhoPhong_ThanhTrung = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_ThanhTrung);
            }
        }

        private bool _mNhapNoiBo = true;
        private bool _mNhapTraTuKhoLe = true;
        public bool mNhapNoiBo
        {
            get
            {
                return _mNhapNoiBo && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapNoiBo == value)
                    return;
                _mNhapNoiBo = value;
                NotifyOfPropertyChange(() => mNhapNoiBo);
            }
        }

        public bool mNhapTraTuKhoLe
        {
            get
            {
                return _mNhapTraTuKhoLe && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapTraTuKhoLe == value)
                    return;
                _mNhapTraTuKhoLe = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoLe);
            }
        }

        private bool _gNhapNoiBo = true;
        public bool gNhapNoiBo
        {
            get
            {
                return _gNhapNoiBo;
            }
            set
            {
                _gNhapNoiBo = value;
                NotifyOfPropertyChange(() => gNhapNoiBo);
            }
        }
        //▲======#021

        //▼======#022
        private bool _mXNBVTYTTHCmd = true;
        private bool _mXNBVaccineCmd = true;
        private bool _mXNBBloodCmd = true;
        private bool _mXNBVPPCmd = true;
        private bool _mXNBVTTHCmd = true;
        private bool _mXNBThanhTrungCmd = true;

        public bool mXNBVTYTTHCmd
        {
            get
            {
                return _mXNBVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXNBVTYTTHCmd == value)
                    return;
                _mXNBVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mXNBVTYTTHCmd);
            }
        }

        public bool mXNBVaccineCmd
        {
            get
            {
                return _mXNBVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXNBVaccineCmd == value)
                    return;
                _mXNBVaccineCmd = value;
                NotifyOfPropertyChange(() => mXNBVaccineCmd);
            }
        }

        public bool mXNBBloodCmd
        {
            get
            {
                return _mXNBBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXNBBloodCmd == value)
                    return;
                _mXNBBloodCmd = value;
                NotifyOfPropertyChange(() => mXNBBloodCmd);
            }
        }

        public bool mXNBVPPCmd
        {
            get
            {
                return _mXNBVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXNBVPPCmd == value)
                    return;
                _mXNBVPPCmd = value;
                NotifyOfPropertyChange(() => mXNBVPPCmd);
            }
        }

        public bool mXNBVTTHCmd
        {
            get
            {
                return _mXNBVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXNBVTTHCmd == value)
                    return;
                _mXNBVTTHCmd = value;
                NotifyOfPropertyChange(() => mXNBVTTHCmd);
            }
        }

        public bool mXNBThanhTrungCmd
        {
            get
            {
                return _mXNBThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXNBThanhTrungCmd == value)
                    return;
                _mXNBThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mXNBThanhTrungCmd);
            }
        }

        private bool _mReturnToSupplier_Drug = true;
        private bool _mReturnToSupplier_Mat = true;
        private bool _mReturnToSupplier_Chemical = true;
        private bool _mReturnToSupplier_Nutri = true;
        private bool _mReturnToSupplier_VTYTTH = true;
        private bool _mReturnToSupplier_Vaccine = true;
        private bool _mReturnToSupplier_Blood = true;
        private bool _mReturnToSupplier_VPP = true;
        private bool _mReturnToSupplier_VTTH = true;
        private bool _mReturnToSupplier_ThanhTrung = true;

        public bool mReturnToSupplier_Drug
        {
            get
            {
                return _mReturnToSupplier_Drug && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_Drug == value)
                    return;
                _mReturnToSupplier_Drug = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_Drug);
            }
        }

        public bool mReturnToSupplier_Mat
        {
            get
            {
                return _mReturnToSupplier_Mat && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_Mat == value)
                    return;
                _mReturnToSupplier_Mat = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_Mat);
            }
        }

        public bool mReturnToSupplier_Chemical
        {
            get
            {
                return _mReturnToSupplier_Chemical && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_Chemical == value)
                    return;
                _mReturnToSupplier_Chemical = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_Chemical);
            }
        }

        public bool mReturnToSupplier_Nutri
        {
            get
            {
                return _mReturnToSupplier_Nutri && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_Nutri == value)
                    return;
                _mReturnToSupplier_Nutri = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_Nutri);
            }
        }

        public bool mReturnToSupplier_VTYTTH
        {
            get
            {
                return _mReturnToSupplier_VTYTTH && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_VTYTTH == value)
                    return;
                _mReturnToSupplier_VTYTTH = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_VTYTTH);
            }
        }

        public bool mReturnToSupplier_Vaccine
        {
            get
            {
                return _mReturnToSupplier_Vaccine && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_Vaccine == value)
                    return;
                _mReturnToSupplier_Vaccine = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_Vaccine);
            }
        }

        public bool mReturnToSupplier_Blood
        {
            get
            {
                return _mReturnToSupplier_Blood && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_Blood == value)
                    return;
                _mReturnToSupplier_Blood = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_Blood);
            }
        }

        public bool mReturnToSupplier_VPP
        {
            get
            {
                return _mReturnToSupplier_VPP && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_VPP == value)
                    return;
                _mReturnToSupplier_VPP = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_VPP);
            }
        }

        public bool mReturnToSupplier_VTTH
        {
            get
            {
                return _mReturnToSupplier_VTTH && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_VTTH == value)
                    return;
                _mReturnToSupplier_VTTH = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_VTTH);
            }
        }

        public bool mReturnToSupplier_ThanhTrung
        {
            get
            {
                return _mReturnToSupplier_ThanhTrung && MenuVisibleCollection[1];
            }
            set
            {
                if (_mReturnToSupplier_ThanhTrung == value)
                    return;
                _mReturnToSupplier_ThanhTrung = value;
                NotifyOfPropertyChange(() => mReturnToSupplier_ThanhTrung);
            }
        }

        private bool _mEstimationVTYTTHCmd = true;
        private bool _mEstimationVaccineCmd = true;
        private bool _mEstimationBloodCmd = true;
        private bool _mEstimationVPPCmd = true;
        private bool _mEstimationVTTHCmd = true;
        private bool _mEstimationThanhTrungCmd = true;

        public bool mEstimationVTYTTHCmd
        {
            get
            {
                return _mEstimationVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mEstimationVTYTTHCmd == value)
                    return;
                _mEstimationVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mEstimationVTYTTHCmd);
            }
        }

        public bool mEstimationVaccineCmd
        {
            get
            {
                return _mEstimationVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mEstimationVaccineCmd == value)
                    return;
                _mEstimationVaccineCmd = value;
                NotifyOfPropertyChange(() => mEstimationVaccineCmd);
            }
        }

        public bool mEstimationBloodCmd
        {
            get
            {
                return _mEstimationBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mEstimationBloodCmd == value)
                    return;
                _mEstimationBloodCmd = value;
                NotifyOfPropertyChange(() => mEstimationBloodCmd);
            }
        }

        public bool mEstimationVPPCmd
        {
            get
            {
                return _mEstimationVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mEstimationVPPCmd == value)
                    return;
                _mEstimationVPPCmd = value;
                NotifyOfPropertyChange(() => mEstimationVPPCmd);
            }
        }

        public bool mEstimationVTTHCmd
        {
            get
            {
                return _mEstimationVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mEstimationVTTHCmd == value)
                    return;
                _mEstimationVTTHCmd = value;
                NotifyOfPropertyChange(() => mEstimationVTTHCmd);
            }
        }

        public bool mEstimationThanhTrungCmd
        {
            get
            {
                return _mEstimationThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mEstimationThanhTrungCmd == value)
                    return;
                _mEstimationThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mEstimationThanhTrungCmd);
            }
        }


        private bool _mOrderVTYTTHCmd = true;
        private bool _mOrderVaccineCmd = true;
        private bool _mOrderBloodCmd = true;
        private bool _mOrderVPPCmd = true;
        private bool _mOrderVTTHCmd = true;
        private bool _mOrderThanhTrungCmd = true;
        private bool _mOrderDinhDuongCmd = true;

        public bool mOrderVTYTTHCmd
        {
            get
            {
                return _mOrderVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mOrderVTYTTHCmd == value)
                    return;
                _mOrderVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mOrderVTYTTHCmd);
            }
        }

        public bool mOrderVaccineCmd
        {
            get
            {
                return _mOrderVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mOrderVaccineCmd == value)
                    return;
                _mOrderVaccineCmd = value;
                NotifyOfPropertyChange(() => mOrderVaccineCmd);
            }
        }

        public bool mOrderBloodCmd
        {
            get
            {
                return _mOrderBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mOrderBloodCmd == value)
                    return;
                _mOrderBloodCmd = value;
                NotifyOfPropertyChange(() => mOrderBloodCmd);
            }
        }

        public bool mOrderVPPCmd
        {
            get
            {
                return _mOrderVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mOrderVPPCmd == value)
                    return;
                _mOrderVPPCmd = value;
                NotifyOfPropertyChange(() => mOrderVPPCmd);
            }
        }

        public bool mOrderVTTHCmd
        {
            get
            {
                return _mOrderVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mOrderVTTHCmd == value)
                    return;
                _mOrderVTTHCmd = value;
                NotifyOfPropertyChange(() => mOrderVTTHCmd);
            }
        }

        public bool mOrderThanhTrungCmd
        {
            get
            {
                return _mOrderThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mOrderThanhTrungCmd == value)
                    return;
                _mOrderThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mOrderThanhTrungCmd);
            }
        }

        public bool mOrderDinhDuongCmd
        {
            get
            {
                return _mOrderDinhDuongCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mOrderDinhDuongCmd == value)
                    return;
                _mOrderDinhDuongCmd = value;
                NotifyOfPropertyChange(() => mOrderDinhDuongCmd);
            }
        }


        private bool _mDrugDeptSellingPriceList_VTYTTH_Mgnt = true;
        private bool _mDrugDeptSellingPriceList_Vaccine_Mgnt = true;
        private bool _mDrugDeptSellingPriceList_Blood_Mgnt = true;
        private bool _mDrugDeptSellingPriceList_VPP_Mgnt = true;
        private bool _mDrugDeptSellingPriceList_VTTH_Mgnt = true;
        private bool _mDrugDeptSellingPriceList_ThanhTrung_Mgnt = true;
        private bool _mDrugDeptSellingPriceList_DinhDuong_Mgnt = true;

        public bool mDrugDeptSellingPriceList_VTYTTH_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_VTYTTH_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_VTYTTH_Mgnt == value)
                    return;
                _mDrugDeptSellingPriceList_VTYTTH_Mgnt = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_VTYTTH_Mgnt);
            }
        }

        public bool mDrugDeptSellingPriceList_Vaccine_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_Vaccine_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_Vaccine_Mgnt == value)
                    return;
                _mDrugDeptSellingPriceList_Vaccine_Mgnt = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Vaccine_Mgnt);
            }
        }

        public bool mDrugDeptSellingPriceList_Blood_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_Blood_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_Blood_Mgnt == value)
                    return;
                _mDrugDeptSellingPriceList_Blood_Mgnt = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_Blood_Mgnt);
            }
        }

        public bool mDrugDeptSellingPriceList_VPP_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_VPP_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_VPP_Mgnt == value)
                    return;
                _mDrugDeptSellingPriceList_VPP_Mgnt = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_VPP_Mgnt);
            }
        }

        public bool mDrugDeptSellingPriceList_VTTH_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_VTTH_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_VTTH_Mgnt == value)
                    return;
                _mDrugDeptSellingPriceList_VTTH_Mgnt = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_VTTH_Mgnt);
            }
        }

        public bool mDrugDeptSellingPriceList_ThanhTrung_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_ThanhTrung_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_ThanhTrung_Mgnt == value)
                    return;
                _mDrugDeptSellingPriceList_ThanhTrung_Mgnt = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_ThanhTrung_Mgnt);
            }
        }

        public bool mDrugDeptSellingPriceList_DinhDuong_Mgnt
        {
            get
            {
                return _mDrugDeptSellingPriceList_DinhDuong_Mgnt && MenuVisibleCollection[1];
            }
            set
            {
                if (_mDrugDeptSellingPriceList_DinhDuong_Mgnt == value)
                    return;
                _mDrugDeptSellingPriceList_DinhDuong_Mgnt = value;
                NotifyOfPropertyChange(() => mDrugDeptSellingPriceList_DinhDuong_Mgnt);
            }
        }


        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd = true;
        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoVaccineCmd = true;
        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoBloodCmd = true;
        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd = true;
        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoVTTHCmd = true;
        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd = true;
        private bool _mBangKeNhapHangThangTheoSoPhieuNhapKhoDinhDuongCmd = true;

        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoVTYTTHCmd);
            }
        }

        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoVaccineCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoVaccineCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoVaccineCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoVaccineCmd);
            }
        }

        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoBloodCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoBloodCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoBloodCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoBloodCmd);
            }
        }

        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoVPPCmd);
            }
        }

        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoVTTHCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoVTTHCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoVTTHCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoVTTHCmd);
            }
        }

        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoThanhTrungCmd);
            }
        }

        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoDinhDuongCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoDinhDuongCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBangKeNhapHangThangTheoSoPhieuNhapKhoDinhDuongCmd == value)
                    return;
                _mBangKeNhapHangThangTheoSoPhieuNhapKhoDinhDuongCmd = value;
                NotifyOfPropertyChange(() => mBangKeNhapHangThangTheoSoPhieuNhapKhoDinhDuongCmd);
            }
        }


        private bool _mNhapXuatTonVTYTTHCmd = true;
        private bool _mNhapXuatTonVaccineCmd = true;
        private bool _mNhapXuatTonBloodCmd = true;
        private bool _mNhapXuatTonVPPCmd = true;
        private bool _mNhapXuatTonVTTHCmd = true;
        private bool _mNhapXuatTonThanhTrungCmd = true;

        public bool mNhapXuatTonVTYTTHCmd
        {
            get
            {
                return _mNhapXuatTonVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapXuatTonVTYTTHCmd == value)
                    return;
                _mNhapXuatTonVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonVTYTTHCmd);
            }
        }

        public bool mNhapXuatTonVaccineCmd
        {
            get
            {
                return _mNhapXuatTonVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapXuatTonVaccineCmd == value)
                    return;
                _mNhapXuatTonVaccineCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonVaccineCmd);
            }
        }

        public bool mNhapXuatTonBloodCmd
        {
            get
            {
                return _mNhapXuatTonBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapXuatTonBloodCmd == value)
                    return;
                _mNhapXuatTonBloodCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonBloodCmd);
            }
        }

        public bool mNhapXuatTonVPPCmd
        {
            get
            {
                return _mNhapXuatTonVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapXuatTonVPPCmd == value)
                    return;
                _mNhapXuatTonVPPCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonVPPCmd);
            }
        }

        public bool mNhapXuatTonVTTHCmd
        {
            get
            {
                return _mNhapXuatTonVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapXuatTonVTTHCmd == value)
                    return;
                _mNhapXuatTonVTTHCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonVTTHCmd);
            }
        }

        public bool mNhapXuatTonThanhTrungCmd
        {
            get
            {
                return _mNhapXuatTonThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapXuatTonThanhTrungCmd == value)
                    return;
                _mNhapXuatTonThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mNhapXuatTonThanhTrungCmd);
            }
        }

        private bool _mTheKhoVTYTTHCmd = true;
        private bool _mTheKhoVaccineCmd = true;
        private bool _mTheKhoBloodCmd = true;
        private bool _mTheKhoVPPCmd = true;
        private bool _mTheKhoVTTHCmd = true;
        private bool _mTheKhoThanhTrungCmd = true;

        public bool mTheKhoVTYTTHCmd
        {
            get
            {
                return _mTheKhoVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTheKhoVTYTTHCmd == value)
                    return;
                _mTheKhoVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mTheKhoVTYTTHCmd);
            }
        }

        public bool mTheKhoVaccineCmd
        {
            get
            {
                return _mTheKhoVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTheKhoVaccineCmd == value)
                    return;
                _mTheKhoVaccineCmd = value;
                NotifyOfPropertyChange(() => mTheKhoVaccineCmd);
            }
        }

        public bool mTheKhoBloodCmd
        {
            get
            {
                return _mTheKhoBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTheKhoBloodCmd == value)
                    return;
                _mTheKhoBloodCmd = value;
                NotifyOfPropertyChange(() => mTheKhoBloodCmd);
            }
        }

        public bool mTheKhoVPPCmd
        {
            get
            {
                return _mTheKhoVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTheKhoVPPCmd == value)
                    return;
                _mTheKhoVPPCmd = value;
                NotifyOfPropertyChange(() => mTheKhoVPPCmd);
            }
        }

        public bool mTheKhoVTTHCmd
        {
            get
            {
                return _mTheKhoVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTheKhoVTTHCmd == value)
                    return;
                _mTheKhoVTTHCmd = value;
                NotifyOfPropertyChange(() => mTheKhoVTTHCmd);
            }
        }

        public bool mTheKhoThanhTrungCmd
        {
            get
            {
                return _mTheKhoThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTheKhoThanhTrungCmd == value)
                    return;
                _mTheKhoThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mTheKhoThanhTrungCmd);
            }
        }


        private bool _mXuatVTYTTHCmd = true;
        private bool _mXuatVaccineCmd = true;
        private bool _mXuatBloodCmd = true;
        private bool _mXuatVPPCmd = true;
        private bool _mXuatVTTHCmd = true;
        private bool _mXuatThanhTrungCmd = true;

        public bool mXuatVTYTTHCmd
        {
            get
            {
                return _mXuatVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatVTYTTHCmd == value)
                    return;
                _mXuatVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mXuatVTYTTHCmd);
            }
        }

        public bool mXuatVaccineCmd
        {
            get
            {
                return _mXuatVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatVaccineCmd == value)
                    return;
                _mXuatVaccineCmd = value;
                NotifyOfPropertyChange(() => mXuatVaccineCmd);
            }
        }

        public bool mXuatBloodCmd
        {
            get
            {
                return _mXuatBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatBloodCmd == value)
                    return;
                _mXuatBloodCmd = value;
                NotifyOfPropertyChange(() => mXuatBloodCmd);
            }
        }

        public bool mXuatVPPCmd
        {
            get
            {
                return _mXuatVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatVPPCmd == value)
                    return;
                _mXuatVPPCmd = value;
                NotifyOfPropertyChange(() => mXuatVPPCmd);
            }
        }

        public bool mXuatVTTHCmd
        {
            get
            {
                return _mXuatVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatVTTHCmd == value)
                    return;
                _mXuatVTTHCmd = value;
                NotifyOfPropertyChange(() => mXuatVTTHCmd);
            }
        }

        public bool mXuatThanhTrungCmd
        {
            get
            {
                return _mXuatThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mXuatThanhTrungCmd == value)
                    return;
                _mXuatThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mXuatThanhTrungCmd);
            }
        }


        private bool _mNhapVTYTTHCmd = true;
        private bool _mNhapVaccineCmd = true;
        private bool _mNhapBloodCmd = true;
        private bool _mNhapVPPCmd = true;
        private bool _mNhapVTTHCmd = true;
        private bool _mNhapThanhTrungCmd = true;

        public bool mNhapVTYTTHCmd
        {
            get
            {
                return _mNhapVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapVTYTTHCmd == value)
                    return;
                _mNhapVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mNhapVTYTTHCmd);
            }
        }

        public bool mNhapVaccineCmd
        {
            get
            {
                return _mNhapVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapVaccineCmd == value)
                    return;
                _mNhapVaccineCmd = value;
                NotifyOfPropertyChange(() => mNhapVaccineCmd);
            }
        }

        public bool mNhapBloodCmd
        {
            get
            {
                return _mNhapBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapBloodCmd == value)
                    return;
                _mNhapBloodCmd = value;
                NotifyOfPropertyChange(() => mNhapBloodCmd);
            }
        }

        public bool mNhapVPPCmd
        {
            get
            {
                return _mNhapVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapVPPCmd == value)
                    return;
                _mNhapVPPCmd = value;
                NotifyOfPropertyChange(() => mNhapVPPCmd);
            }
        }

        public bool mNhapVTTHCmd
        {
            get
            {
                return _mNhapVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapVTTHCmd == value)
                    return;
                _mNhapVTTHCmd = value;
                NotifyOfPropertyChange(() => mNhapVTTHCmd);
            }
        }

        public bool mNhapThanhTrungCmd
        {
            get
            {
                return _mNhapThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mNhapThanhTrungCmd == value)
                    return;
                _mNhapThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mNhapThanhTrungCmd);
            }
        }


        private bool _mBaoCaoThuocHetHan = true;
        private bool _mBaoCaoHangKhongXuat = true;
        public bool mBaoCaoThuocHetHan
        {
            get
            {
                return _mBaoCaoThuocHetHan && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBaoCaoThuocHetHan == value)
                    return;
                _mBaoCaoThuocHetHan = value;
                NotifyOfPropertyChange(() => mBaoCaoThuocHetHan);
            }
        }

        public bool mBaoCaoHangKhongXuat
        {
            get
            {
                return _mBaoCaoHangKhongXuat && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBaoCaoHangKhongXuat == value)
                    return;
                _mBaoCaoHangKhongXuat = value;
                NotifyOfPropertyChange(() => mBaoCaoHangKhongXuat);
            }
        }


        private bool _gBaoCaoToaTreo = true;
        private bool _gBaoCaoKhac = true;
        private bool _mBaoCaoToaTreo_Duoc = true;
        private bool _mBaoCaoToaTreo_NhaThuoc = true;
        private bool _mBaoCaoToaTreo_KhoaPhong = true;
        private bool _mBaoCaoToaThuocHangNgay = true;
        public bool gBaoCaoToaTreo
        {
            get
            {
                return _gBaoCaoToaTreo && MenuVisibleCollection[1];
            }
            set
            {
                if (_gBaoCaoToaTreo == value)
                    return;
                _gBaoCaoToaTreo = value;
                NotifyOfPropertyChange(() => gBaoCaoToaTreo);
            }
        }

        public bool mBaoCaoToaTreo_Duoc
        {
            get
            {
                return _mBaoCaoToaTreo_Duoc && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBaoCaoToaTreo_Duoc == value)
                    return;
                _mBaoCaoToaTreo_Duoc = value;
                NotifyOfPropertyChange(() => mBaoCaoToaTreo_Duoc);
            }
        }

        public bool mBaoCaoToaTreo_NhaThuoc
        {
            get
            {
                return _mBaoCaoToaTreo_NhaThuoc && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBaoCaoToaTreo_NhaThuoc == value)
                    return;
                _mBaoCaoToaTreo_NhaThuoc = value;
                NotifyOfPropertyChange(() => mBaoCaoToaTreo_NhaThuoc);
            }
        }

        public bool mBaoCaoToaTreo_KhoaPhong
        {
            get
            {
                return _mBaoCaoToaTreo_KhoaPhong && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBaoCaoToaTreo_KhoaPhong == value)
                    return;
                _mBaoCaoToaTreo_KhoaPhong = value;
                NotifyOfPropertyChange(() => mBaoCaoToaTreo_KhoaPhong);
            }
        }

        public bool gBaoCaoKhac
        {
            get
            {
                return _gBaoCaoKhac && MenuVisibleCollection[1];
            }
            set
            {
                if (_gBaoCaoKhac == value)
                    return;
                _gBaoCaoKhac = value;
                NotifyOfPropertyChange(() => gBaoCaoKhac);
            }
        }

        public bool mBaoCaoToaThuocHangNgay
        {
            get
            {
                return _mBaoCaoToaThuocHangNgay && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBaoCaoToaThuocHangNgay == value)
                    return;
                _mBaoCaoToaThuocHangNgay = value;
                NotifyOfPropertyChange(() => mBaoCaoToaThuocHangNgay);
            }
        }


        private bool _mKKVTYTTHCmd = true;
        private bool _mKKVaccineCmd = true;
        private bool _mKKBloodCmd = true;
        private bool _mKKVPPCmd = true;
        private bool _mKKVTTHCmd = true;
        private bool _mKKThanhTrungCmd = true;

        public bool mKKVTYTTHCmd
        {
            get
            {
                return _mKKVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mKKVTYTTHCmd == value)
                    return;
                _mKKVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mKKVTYTTHCmd);
            }
        }

        public bool mKKVaccineCmd
        {
            get
            {
                return _mKKVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mKKVaccineCmd == value)
                    return;
                _mKKVaccineCmd = value;
                NotifyOfPropertyChange(() => mKKVaccineCmd);
            }
        }

        public bool mKKBloodCmd
        {
            get
            {
                return _mKKBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mKKBloodCmd == value)
                    return;
                _mKKBloodCmd = value;
                NotifyOfPropertyChange(() => mKKBloodCmd);
            }
        }

        public bool mKKVPPCmd
        {
            get
            {
                return _mKKVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mKKVPPCmd == value)
                    return;
                _mKKVPPCmd = value;
                NotifyOfPropertyChange(() => mKKVPPCmd);
            }
        }

        public bool mKKVTTHCmd
        {
            get
            {
                return _mKKVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mKKVTTHCmd == value)
                    return;
                _mKKVTTHCmd = value;
                NotifyOfPropertyChange(() => mKKVTTHCmd);
            }
        }

        public bool mKKThanhTrungCmd
        {
            get
            {
                return _mKKThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mKKThanhTrungCmd == value)
                    return;
                _mKKThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mKKThanhTrungCmd);
            }
        }


        private bool _gTangGiamKK = true;
        private bool _mTangGiamKKThuocCmd = true;
        private bool _mTangGiamKKYCuCmd = true;
        private bool _mTangGiamKKHoaChatCmd = true;
        private bool _mTangGiamKKDinhDuongCmd = true;
        private bool _mTangGiamKKVTYTTHCmd = true;
        private bool _mTangGiamKKVaccineCmd = true;
        private bool _mTangGiamKKBloodCmd = true;
        private bool _mTangGiamKKVPPCmd = true;
        private bool _mTangGiamKKVTTHCmd = true;
        private bool _mTangGiamKKThanhTrungCmd = true;

        public bool gTangGiamKK
        {
            get
            {
                return _gTangGiamKK && MenuVisibleCollection[1];
            }
            set
            {
                if (_gTangGiamKK == value)
                    return;
                _gTangGiamKK = value;
                NotifyOfPropertyChange(() => gTangGiamKK);
            }
        }

        public bool mTangGiamKKThuocCmd
        {
            get
            {
                return _mTangGiamKKThuocCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKThuocCmd == value)
                    return;
                _mTangGiamKKThuocCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKThuocCmd);
            }
        }

        public bool mTangGiamKKYCuCmd
        {
            get
            {
                return _mTangGiamKKYCuCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKYCuCmd == value)
                    return;
                _mTangGiamKKYCuCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKYCuCmd);
            }
        }

        public bool mTangGiamKKHoaChatCmd
        {
            get
            {
                return _mTangGiamKKHoaChatCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKHoaChatCmd == value)
                    return;
                _mTangGiamKKHoaChatCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKHoaChatCmd);
            }
        }

        public bool mTangGiamKKDinhDuongCmd
        {
            get
            {
                return _mTangGiamKKDinhDuongCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKDinhDuongCmd == value)
                    return;
                _mTangGiamKKDinhDuongCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKDinhDuongCmd);
            }
        }

        public bool mTangGiamKKVTYTTHCmd
        {
            get
            {
                return _mTangGiamKKVTYTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKVTYTTHCmd == value)
                    return;
                _mTangGiamKKVTYTTHCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKVTYTTHCmd);
            }
        }

        public bool mTangGiamKKVaccineCmd
        {
            get
            {
                return _mTangGiamKKVaccineCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKVaccineCmd == value)
                    return;
                _mTangGiamKKVaccineCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKVaccineCmd);
            }
        }

        public bool mTangGiamKKBloodCmd
        {
            get
            {
                return _mTangGiamKKBloodCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKBloodCmd == value)
                    return;
                _mTangGiamKKBloodCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKBloodCmd);
            }
        }

        public bool mTangGiamKKVPPCmd
        {
            get
            {
                return _mTangGiamKKVPPCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKVPPCmd == value)
                    return;
                _mTangGiamKKVPPCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKVPPCmd);
            }
        }

        public bool mTangGiamKKVTTHCmd
        {
            get
            {
                return _mTangGiamKKVTTHCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKVTTHCmd == value)
                    return;
                _mTangGiamKKVTTHCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKVTTHCmd);
            }
        }

        public bool mTangGiamKKThanhTrungCmd
        {
            get
            {
                return _mTangGiamKKThanhTrungCmd && MenuVisibleCollection[1];
            }
            set
            {
                if (_mTangGiamKKThanhTrungCmd == value)
                    return;
                _mTangGiamKKThanhTrungCmd = value;
                NotifyOfPropertyChange(() => mTangGiamKKThanhTrungCmd);
            }
        }


        private bool _gQuanLy = true;
        private bool _mQuanLyKho = true;
        public bool gQuanLy
        {
            get
            {
                return _gQuanLy && MenuVisibleCollection[1];
            }
            set
            {
                if (_gQuanLy == value)
                    return;
                _gQuanLy = value;
                NotifyOfPropertyChange(() => gQuanLy);
            }
        }

        public bool mQuanLyKho
        {
            get
            {
                return _mQuanLyKho && MenuVisibleCollection[1];
            }
            set
            {
                if (_mQuanLyKho == value)
                    return;
                _mQuanLyKho = value;
                NotifyOfPropertyChange(() => mQuanLyKho);
            }
        }
        //▲======#022
        //▼====: #027
        private bool _gElectronicPrescription = true;
        public bool gElectronicPrescription
        {
            get
            {
                return _gElectronicPrescription && MenuVisibleCollection[1];
            }
            set
            {
                if (_gElectronicPrescription == value)
                    return;
                _gElectronicPrescription = value;
                NotifyOfPropertyChange(() => gElectronicPrescription);
            }
        }
        private bool _mElectronicPrescription = true;
        public bool mElectronicPrescription
        {
            get
            {
                return _mElectronicPrescription && MenuVisibleCollection[1];
            }
            set
            {
                if (_mElectronicPrescription == value)
                    return;
                _mElectronicPrescription = value;
                NotifyOfPropertyChange(() => mElectronicPrescription);
            }
        }
        //▲======#027
        //▼==== #028
        private bool _gBaoCaoXuatKhoaPhong = true;
        public bool gBaoCaoXuatKhoaPhong
        {
            get
            {
                return _gBaoCaoXuatKhoaPhong && MenuVisibleCollection[1];
            }
            set
            {
                if (_gBaoCaoXuatKhoaPhong == value)
                    return;
                _gBaoCaoXuatKhoaPhong = value;
                NotifyOfPropertyChange(() => gBaoCaoXuatKhoaPhong);
            }
        }

        private bool _mBC_XuatKP_Thuoc = true;
        public bool mBC_XuatKP_Thuoc
        {
            get
            {
                return _mBC_XuatKP_Thuoc && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBC_XuatKP_Thuoc == value)
                    return;
                _mBC_XuatKP_Thuoc = value;
                NotifyOfPropertyChange(() => mBC_XuatKP_Thuoc);
            }
        }

        private bool _mBC_XuatKP_YCu = true;
        public bool mBC_XuatKP_YCu
        {
            get
            {
                return _mBC_XuatKP_YCu && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBC_XuatKP_YCu == value)
                    return;
                _mBC_XuatKP_YCu = value;
                NotifyOfPropertyChange(() => mBC_XuatKP_YCu);
            }
        }
        //▲==== #028
        //▼====: #023
        private void Temp19Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.ExportExcel_Temp19_THUOC_COVID_KD;
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp19;
            UnitVM.Only79A = true;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp19Cmd()
        {
            Globals.TitleForm = "Mẫu 19 Thuốc - Covid";
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

        private void Temp19VTYTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.ExportExcel_Temp19_VTYT_COVID_KD;
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp19;
            UnitVM.Only79A = true;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp19VTYTCmd()
        {
            Globals.TitleForm = "Mẫu 19 VTYT - Covid";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp19VTYTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp19VTYTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #023

        //▼====: #024
        private void ClinicDeptInOutValueReport_KTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IDrugModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BC_NXT_THUOC_TONGHOP_COSO;
            reportVm.mXemChiTiet = false;
            reportVm.ChonKho = false;
            reportVm.IsPurpose = false;
            reportVm.IsShowGroupReportType = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ClinicDeptInOutValueReport_KTCmd()
        {
            Globals.TitleForm = "Báo cáo NXT Kho cơ số (Tổng hợp)";
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
        //▲====: #024
        private void PCLContactDrugCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IDrugModule>();
            var reportVm = Globals.GetViewModel<IPCLExamTypeContactDrug>();
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void PCLContactDrugCmd()
        {
            Globals.TitleForm = "Quản lý gói thuốc/ vật tư đi kèm DVKT";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLContactDrugCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLContactDrugCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //▼====: #025
        private bool _mBidDetail_Chemical = true;
        public bool mBidDetail_Chemical
        {
            get
            {
                return _mBidDetail_Chemical && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBidDetail_Chemical == value)
                    return;
                _mBidDetail_Chemical = value;
                NotifyOfPropertyChange(() => mBidDetail_Chemical);
            }
        }
        //▲====: #025

        private bool _gf2NhapHangTuNCC = true;
        public bool gf2NhapHangTuNCC
        {
            get
            {
                return _gf2NhapHangTuNCC;
            }
            set
            {
                if (_gf2NhapHangTuNCC == value)
                    return;
                _gf2NhapHangTuNCC = value;
                NotifyOfPropertyChange(() => gf2NhapHangTuNCC);
            }
        }
        //▼====: #026
        private void XTraThuocHangKyGoiCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBoHangKyGui>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mXuatHangKyGui_Thuoc_Tim;
            UnitVM.mPhieuMoi = mXuatHangKyGui_Thuoc_PhieuMoi;
            UnitVM.mThucHien = mXuatHangKyGui_Thuoc_Save;
            UnitVM.mThuTien = mXuatHangKyGui_Thuoc_ThuTien;
            UnitVM.mIn = mXuatHangKyGui_Thuoc_In;
            UnitVM.mDeleteInvoice = mXuatHangKyGui_Thuoc_DeleteInvoice;
            UnitVM.mPrintReceipt = mXuatHangKyGui_Thuoc_PrintReceipt;
            UnitVM.IsReturn = true;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XTraThuocHangKyGoiCmd(object source)
        {
            Globals.TitleForm = ("Xuất trả hàng ký gửi - Thuốc").ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XTraThuocHangKyGoiCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XTraThuocHangKyGoiCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XTraYCuHangKyGoiCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBoHangKyGui>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mTim = mXuatHangKyGui_Thuoc_Tim;
            UnitVM.mPhieuMoi = mXuatHangKyGui_Thuoc_PhieuMoi;
            UnitVM.mThucHien = mXuatHangKyGui_Thuoc_Save;
            UnitVM.mThuTien = mXuatHangKyGui_Thuoc_ThuTien;
            UnitVM.mIn = mXuatHangKyGui_Thuoc_In;
            UnitVM.mDeleteInvoice = mXuatHangKyGui_Thuoc_DeleteInvoice;
            UnitVM.mPrintReceipt = mXuatHangKyGui_Thuoc_PrintReceipt;
            UnitVM.IsReturn = true;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XTraYCuHangKyGoiCmd(object source)
        {
            Globals.TitleForm = ("Xuất trả hàng ký gửi - Y cụ").ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XTraYCuHangKyGoiCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XTraYCuHangKyGoiCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #026
        //▼====: #027
        private void ElectronicPrescriptionCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IElectronicPrescription>();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ElectronicPrescriptionCmd(object source)
        {
            Globals.TitleForm = "Quản lý đơn thuốc điện tử".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ElectronicPrescriptionCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ElectronicPrescriptionCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #027
        //▼====: #027
        private void RptCancelElectronicPrescriptionCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IDrugDeptReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.BCHuyDTDT;

            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.mXuatExcel = true;
            UnitVM.mIn = false;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void RptCancelElectronicPrescriptionCmd(object source)
        {
            Globals.TitleForm = "Báo cáo hủy đẩy cổng đơn thuốc".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RptCancelElectronicPrescriptionCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RptCancelElectronicPrescriptionCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #027
        //▼==== #029
        private bool _mBCTThongKeDTDT = true;
        public bool mBCTThongKeDTDT
        {
            get
            {
                return _mBCTThongKeDTDT && MenuVisibleCollection[1];
            }
            set
            {
                if (_mBCTThongKeDTDT == value)
                    return;
                _mBCTThongKeDTDT = value;
                NotifyOfPropertyChange(() => mBCTThongKeDTDT);
            }
        }

        private void BCTThongKeDTDTCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            VM.eItem = ReportName.XRpt_BCTThongKeDTDT;
            VM.isAllStaff = false;
            VM.RptParameters.HideFindPatient = false;
            VM.ShowHIReportStatus = true;
            VM.ShowHIReportFindPatient = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void BCTThongKeDTDTCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3323_G1_BCTThongKeDTDT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCTThongKeDTDTCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCTThongKeDTDTCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲==== #029
    }
}


//private void RequestThuocCmd_In(object source)
//{
//   
//    Globals.PageName = Globals.TitleForm;
//    var module = Globals.GetViewModel<IDrugModule>();
//    var UnitVM = Globals.GetViewModel<IRequest>();
//    UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
//    UnitVM.strHienThi = Globals.TitleForm;
//    UnitVM.mDuyetPhieuLinhHang_Tim = mDuyetPhieuLinhHang_Thuoc_Tim;
//    UnitVM.mDuyetPhieuLinhHang_PhieuMoi = mDuyetPhieuLinhHang_Thuoc_PhieuMoi;
//    UnitVM.mDuyetPhieuLinhHang_XuatHang = mDuyetPhieuLinhHang_Thuoc_XuatHang;
//    UnitVM.mDuyetPhieuLinhHang_XemInTH = mDuyetPhieuLinhHang_Thuoc_XemInTH;
//    UnitVM.mDuyetPhieuLinhHang_XemInCT = mDuyetPhieuLinhHang_Thuoc_XemInCT;

//    module.MainContent = UnitVM;
//    (module as Conductor<object>).ActivateItem(UnitVM);
//}

//public void RequestThuocCmd(object source)
//{
//    Globals.TitleForm = eHCMSResources.Z0732_G1_DuyetPhLinhThuoc.ToUpper();
//    if (string.IsNullOrEmpty(Globals.PageName))
//    {
//        RequestThuocCmd_In(source);
//    }
//    else if (Globals.PageName != Globals.TitleForm)
//    {
//        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
//        {
//            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
//            {
//                RequestThuocCmd_In(source);
//                GlobalsNAV.msgb = null;
//            }
//        });
//    }
//}

//private void RequestYCuCmd_In(object source)
//{
//   
//    Globals.PageName = Globals.TitleForm;

//    var module = Globals.GetViewModel<IDrugModule>();
//    var UnitVM = Globals.GetViewModel<IRequest>();
//    UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
//    UnitVM.strHienThi = Globals.TitleForm;
//    UnitVM.mDuyetPhieuLinhHang_Tim = mDuyetPhieuLinhHang_YCu_Tim;
//    UnitVM.mDuyetPhieuLinhHang_PhieuMoi = mDuyetPhieuLinhHang_YCu_PhieuMoi;
//    UnitVM.mDuyetPhieuLinhHang_XuatHang = mDuyetPhieuLinhHang_YCu_XuatHang;
//    UnitVM.mDuyetPhieuLinhHang_XemInTH = mDuyetPhieuLinhHang_YCu_XemInTH;
//    UnitVM.mDuyetPhieuLinhHang_XemInCT = mDuyetPhieuLinhHang_YCu_XemInCT;

//    module.MainContent = UnitVM;
//    (module as Conductor<object>).ActivateItem(UnitVM);
//}

//public void RequestYCuCmd(object source)
//{
//    Globals.TitleForm = eHCMSResources.Z0735_G1_DuyetPhLinhYCu.ToUpper();
//    if (string.IsNullOrEmpty(Globals.PageName))
//    {
//        RequestYCuCmd_In(source);
//    }
//    else if (Globals.PageName != Globals.TitleForm)
//    {
//        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
//        {
//            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
//            {
//                RequestYCuCmd_In(source);
//                GlobalsNAV.msgb = null;
//            }
//        });
//    }
//}

//private void RequestHoaChatCmd_In(object source)
//{
//   
//    Globals.PageName = Globals.TitleForm;

//    var module = Globals.GetViewModel<IDrugModule>();
//    var UnitVM = Globals.GetViewModel<IRequest>();
//    UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
//    UnitVM.strHienThi = Globals.TitleForm;
//    UnitVM.mDuyetPhieuLinhHang_Tim = mDuyetPhieuLinhHang_HoaChat_Tim;
//    UnitVM.mDuyetPhieuLinhHang_PhieuMoi = mDuyetPhieuLinhHang_HoaChat_PhieuMoi;
//    UnitVM.mDuyetPhieuLinhHang_XuatHang = mDuyetPhieuLinhHang_HoaChat_XuatHang;
//    UnitVM.mDuyetPhieuLinhHang_XemInTH = mDuyetPhieuLinhHang_HoaChat_XemInTH;
//    UnitVM.mDuyetPhieuLinhHang_XemInCT = mDuyetPhieuLinhHang_HoaChat_XemInCT;
//    module.MainContent = UnitVM;
//    (module as Conductor<object>).ActivateItem(UnitVM);
//}

//public void RequestHoaChatCmd(object source)
//{
//    Globals.TitleForm = eHCMSResources.Z0736_G1_DuyetPhLinhHChat.ToUpper();
//    if (string.IsNullOrEmpty(Globals.PageName))
//    {
//        RequestHoaChatCmd_In(source);
//    }
//    else if (Globals.PageName != Globals.TitleForm)
//    {
//        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
//        {
//            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
//            {
//                RequestHoaChatCmd_In(source);
//                GlobalsNAV.msgb = null;
//            }
//        });
//    }
//}