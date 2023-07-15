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
* 20170810 #001 CMN: Added Bid Service
* 20170821 #002 CMN: Added AdjustClinicPrice Service
* 20171226 #003 CMN: Added Temp Inward Report
* 20171227 #004 CMN: Show DrugCategory Combobox on MatItems in InOutGeneralReport
* 20180806 #005 TBL: Added Generic
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugLeftMenu))]
    public class DrugLeftMenuViewModel : Conductor<object>, IDrugLeftMenu
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public DrugLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
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

         private void SetHyperlinkSelectedStyle(Button lnk)
         {
             if (_currentView != null)
             {
                 _currentView.ResetMenuColor();
             }
             if (lnk != null)
             {
                 lnk.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
             }
         }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            #region Group Function Duyệt Phiếu Lĩnh Hàng
            mDuyetPhieuLinhHang_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_Thuoc);
            mDuyetPhieuLinhHang_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_YCu);
            mDuyetPhieuLinhHang_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDuyetPhieuLinhHang_HoaChat);

            gfDuyetPhieuLinhHang = mDuyetPhieuLinhHang_Thuoc || mDuyetPhieuLinhHang_YCu || mDuyetPhieuLinhHang_HoaChat;
            #endregion

            #region Xuất Nội Bộ
            mXNBThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_Thuoc);
            mXNBYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_YCu);
            mXNBHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuat_HoaChat);

            gfXuatNoiBo = mXNBThuocCmd || mXNBYCuCmd || mXNBHoaChatCmd;
            #endregion

            #region Xuất Theo Toa

            mXuatTheoToa_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mXuatTheoToa_Thuoc);

            gfXuatTheoToa = mXuatTheoToa_Thuoc;
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

            gfDuTru = mEstimationDrugDeptCmd || mEstimationYCuCmd || mEstimationChemicalCmd;
            #endregion

            #region Đặt hàng
            mOrderThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_Thuoc);
            mOrderYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_YCu);
            mOrderHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mDatHang_HoaChat);
            gfDatHang = mOrderThuocCmd || mOrderYCuCmd || mOrderHoaChatCmd;
            #endregion

            #region Nhập hàng
            mInwardDrugFromSupplierDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_Thuoc);
            mInwardDrugFromSupplierMedicalDeviceCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_YCu);
            mInwardDrugFromSupplierChemicalCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapHangNCC_HoaChat);

            gf1NhapHangTuNCC = mInwardDrugFromSupplierDrugCmd || mInwardDrugFromSupplierMedicalDeviceCmd || mInwardDrugFromSupplierChemicalCmd;

            mNhapTraTuKhoPhong_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                             (int)eKhoaDuoc.mNhapTraTuKhoPhong_Thuoc);
            mNhapTraTuKhoPhong_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapTraTuKhoPhong_YCu);
            mNhapTraTuKhoPhong_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mNhapTraTuKhoPhong_HoaChat);

            gf1NhapTraTuKhoPhong = mNhapTraTuKhoPhong_Thuoc || mNhapTraTuKhoPhong_YCu || mNhapTraTuKhoPhong_HoaChat;

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

            gfKiemKe = mKKThuocCmd || mKKYCuCmd || mKKHoaChatCmd;
            #endregion

            #region Báo Cáo
            /*TMA 23/10/2017*/
            mNhapThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoNhap_Thuoc);
            mNhapYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoNhapXuat_YCu);
            mNhapHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoNhapXuat_HoaChat);
            gf1BaoCaoNhap = mNhapThuocCmd || mNhapYCuCmd || mNhapHoaChatCmd;
            /*TMA 23/10/2017*/
            mXuatThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_Thuoc);
            mXuatYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_YCu);
            mXuatHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoXuat_HoaChat);

            gf1BaoCaoXuat = mXuatThuocCmd || mXuatYCuCmd || mXuatHoaChatCmd;


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

            gf1NhapXuatTon = mNhapXuatTonThuocCmd || mNhapXuatTonYCuCmd || mNhapXuatTonHoaChatCmd;

            mTheKhoThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_Thuoc);
            mTheKhoYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_YCu);
            mTheKhoHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoTheKho_HoaChat);

            gf1TheKho = mTheKhoThuocCmd || mTheKhoYCuCmd || mTheKhoHoaChatCmd;

            mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_Thuoc);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_YCu);
            mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mBaoCaoDSPhieuNhapKho_HoaChat);

            gf1PhieuNhapKho = mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd || mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd;

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

            gfBaoCao = gf1BaoCaoXuat || gf1NhapXuatTon || gf1TheKho || gf1PhieuNhapKho || gf1BaoCaoNam || gf1XuatKhoaPhong || gf1TheoDoiCongNo;

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

            gfQuanLyNhomHang = mQLNhomHang_Thuoc || mQLNhomHang_YCu || mQLNhomHang_HoaChat;
            #endregion

            #region Quản lý danh mục
            mRefGenMedProductDetails_DrugMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_Thuoc);
            mRefGenMedProductDetails_MedicalDevicesMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_YCu);
            mRefGenMedProductDetails_ChemicalMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoaDuoc,
                                                         (int)eKhoaDuoc.mQLDanhMuc_HoaChat);

            gfQuanLyDanhMuc = mRefGenMedProductDetails_DrugMgnt || mRefGenMedProductDetails_MedicalDevicesMgnt || mRefGenMedProductDetails_ChemicalMgnt;
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
            gfThangGiaBan = mDrugDeptSellPriceProfitScale_Mgnt || mDrugDeptSellPriceProfitScale_Medical_Mgnt || mDrugDeptSellPriceProfitScale_Chemical_Mgnt;
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

            gfBangGiaBan = mDrugDeptSellingPriceList_Mgnt || mDrugDeptSellingPriceList_Medical_Mgnt || mDrugDeptSellingPriceList_Chemical_Mgnt;
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
        }

        #region bool checking


        #region Group Function

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
                return _gfXuatTheoToa;
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
                return _mNhapThuocCmd
                ;
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
                return _mNhapYCuCmd;
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
                return _mNhapHoaChatCmd;
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
                return _gf1BaoCaoNam;
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

        public bool mXNBThuocCmd
        {
            get
            {
                return _mXNBThuocCmd;
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
                return _mXNBYCuCmd;
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
                return _mXNBHoaChatCmd;
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
                return _mXuatTheoToa_Thuoc;
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
                return _mReturnThuocCmd;
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
                return _mReturnYCuCmd;
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
                return _mReturnHoaChatCmd
                ;
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
                return _mDemageThuocCmd
                ;
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
                return _mDemageYCuCmd
                ;
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
                return _mDemageHoaChatCmd
                ;
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
                return _mEstimationDrugDeptCmd
                ;
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
                return _mEstimationYCuCmd
                ;
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
                return _mEstimationChemicalCmd
                ;
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
                return _mOrderThuocCmd;
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
                return _mOrderYCuCmd;
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
                return _mOrderHoaChatCmd;
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
                return _mInwardDrugFromSupplierDrugCmd;
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
                return _mInwardDrugFromSupplierMedicalDeviceCmd;
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
                return _mInwardDrugFromSupplierChemicalCmd;
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
                return _mCostThuocCmd;
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
                return _mCostYCuCmd;
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
                return _mCostHoaChatCmd;
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
                return _mSuggestThuocCmd;
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
                return _mSuggestYCuCmd;
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
                return _mSuggestHoaChatCmd
                ;
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
                return _mKKThuocCmd
                ;
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
                return _mKKYCuCmd
                ;
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
                return _mKKHoaChatCmd
                ;
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
                return _mXuatThuocCmd
                ;
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
                return _mXuatYCuCmd;
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
                return _mXuatHoaChatCmd;
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
                return _mWatchMedCmd;
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
                return _mWatchMatCmd;
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
                return _mWatchLabCmd;
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
                return _mNhapXuatTonThuocCmd
                ;
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
                return _mNhapXuatTonYCuCmd;
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
                return _mNhapXuatTonHoaChatCmd;
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
                return _mTheKhoThuocCmd;
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
                return _mTheKhoYCuCmd;
            }
            set
            {
                if (_mTheKhoYCuCmd == value)
                    return;
                _mTheKhoYCuCmd = value;
                NotifyOfPropertyChange(() => mTheKhoYCuCmd);
            }
        }

        public bool mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd
        {
            get
            {
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd;
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
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd;
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
                return _mBangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd;
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
                return _mSuDungThuocCmd;
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
                return _mThuocXuatDenKhoaCmd;
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
                return _mYCuXuatDenKhoaCmd;
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
                return _mHoaChatXuatDenKhoaCmd;
            }
            set
            {
                if (_mHoaChatXuatDenKhoaCmd == value)
                    return;
                _mHoaChatXuatDenKhoaCmd = value;
                NotifyOfPropertyChange(() => mHoaChatXuatDenKhoaCmd);
            }
        }


        public bool mTheKhoHoaChatCmd
        {
            get
            {
                return _mTheKhoHoaChatCmd
                ;
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
                return _mQLNhomHang_Thuoc;
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
                return _mQLNhomHang_YCu;
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
                return _mQLNhomHang_HoaChat;
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
                return _mQLNhomHang_HoatChat;
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
                return _mRefGenMedProductDetails_DrugMgnt
                ;
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
                return _mRefGenMedProductDetails_MedicalDevicesMgnt
                ;
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
                return _mRefGenMedProductDetails_ChemicalMgnt
                ;
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




        public bool mSupplierGenMedProductsPrice_Mgnt
        {
            get
            {
                return _mSupplierGenMedProductsPrice_Mgnt
                ;
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
                return _mSupplierGenMedProductsPrice_Medical_Mgnt
                ;
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
                return _mSupplierGenMedProductsPrice_Chemical_Mgnt
                ;
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
                return _mDrugDeptSellPriceProfitScale_Mgnt
                ;
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
                return _mDrugDeptSellPriceProfitScale_Medical_Mgnt
                ;
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
                return _mDrugDeptSellPriceProfitScale_Chemical_Mgnt
                ;
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
                return _mDrugDeptSellingItemPrices_Mgnt
                ;
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
                return _mDrugDeptSellingItemPrices_Medical_Mgnt
                ;
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
                return _mDrugDeptSellingItemPrices_Chemical_Mgnt
                ;
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
                return _mDrugDeptSellingPriceList_Mgnt
                ;
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
                return _mDrugDeptSellingPriceList_Medical_Mgnt
                ;
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
                return _mDrugDeptSellingPriceList_Chemical_Mgnt;
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
                return _mBaoCaoTheoDoiCongNo_Thuoc;
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
                return _mBaoCaoTheoDoiCongNo_YCu;
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
                return _mBaoCaoTheoDoiCongNo_HoaChat;
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
                return _mDuyetPhieuLinhHang_Thuoc;
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
                return _mDuyetPhieuLinhHang_YCu;
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
                return _mDuyetPhieuLinhHang_HoaChat;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_HoaChat == value)
                    return;
                _mDuyetPhieuLinhHang_HoaChat = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_HoaChat);
            }
        }




        public bool mXuatHangKyGui_Thuoc
        {
            get
            {
                return _mXuatHangKyGui_Thuoc;
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
                return _mXuatHangKyGui_YCu;
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
                return _mXuatHangKyGui_HoaChat;
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
                return _mSapNhapHangKyGui_Thuoc;
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
                return _mSapNhapHangKyGui_YCu;
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
                return _mSapNhapHangKyGui_HoaChat;
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
                return _mNhapTraTuKhoPhong_Thuoc;
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
                return _mNhapTraTuKhoPhong_YCu;
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
                return _mNhapTraTuKhoPhong_HoaChat;
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
                return _mAdjustOutPrice_Med;
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
                return _mAdjustOutPrice_Mat;
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
                return _mAdjustOutPrice_Lab;
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
                _mBidDetail = value;
            }
        }
        /*▲====: #001*/
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
        }



        #endregion
        #endregion

        #region Approve Request member

        private void RequestNewThuocCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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



        #endregion

        #region Estimation member

        private void EstimationDrugDeptCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
          
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

        private void EstimationYCuCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
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

        private void EstimationChemicalCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
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
        #endregion

        #region Xuat Member

        private void XNBThuocCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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

        #endregion

        #region Xuất theo toa

        private void XuatThuocTheoToaCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
       
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
           SetHyperlinkSelectedStyle(source as Button);
          
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<ISupplierProduct>();
            UnitVM.mTim = mQLNCCNSX_NCC_Tim;
            UnitVM.mThemMoi = mQLNCCNSX_NCC_ThemMoi;
            UnitVM.mChinhSua = mQLNCCNSX_NCC_ChinhSua;
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
         
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
         
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

        #endregion

        #region "Giá Thuốc Khoa Dược"
        //Giá Thuốc Từ NCC
        private void SupplierGenMedProductsPrice_Mgnt_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);

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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
         
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
      
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
         
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
        #endregion

        #region Order member

        private void OrderThuocCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptPurchaseOrder>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;
            VM.LoadOrderWarning();

            VM.mDSCanDatHang = mDatHang_Thuoc_DSCanDatHang;
            VM.mTim = mDatHang_Thuoc_Tim;
            VM.mChinhSua = mDatHang_Thuoc_ChinhSua;
            VM.mThemMoi = mDatHang_Thuoc_ThemMoi;
            VM.mIn = mDatHang_Thuoc_In;


            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void OrderThuocCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0759_G1_DHgChoThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderThuocCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderThuocCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void OrderYCuCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptPurchaseOrder>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;
            VM.LoadOrderWarning();

            VM.mDSCanDatHang = mDatHang_YCu_DSCanDatHang;
            VM.mTim = mDatHang_YCu_Tim;
            VM.mChinhSua = mDatHang_YCu_ChinhSua;
            VM.mThemMoi = mDatHang_YCu_ThemMoi;
            VM.mIn = mDatHang_YCu_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void OrderYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0760_G1_DHgChoYCu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderYCuCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderYCuCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void OrderHoaChatCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
        
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptPurchaseOrder>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;
            VM.LoadOrderWarning();

            VM.mDSCanDatHang = mDatHang_HoaChat_DSCanDatHang;
            VM.mTim = mDatHang_HoaChat_Tim;
            VM.mChinhSua = mDatHang_HoaChat_ChinhSua;
            VM.mThemMoi = mDatHang_HoaChat_ThemMoi;
            VM.mIn = mDatHang_HoaChat_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void OrderHoaChatCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0761_G1_DHgChoHChat.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderHoaChatCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderHoaChatCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Nhap Hang Member

        private void InwardDrugFromSupplierDrugCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
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

        private void InwardDrugFromSupplierMedicalDeviceCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
        
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;
            VM.mNhapTraTuKhoPhong_Tim = mNhapTraTuKhoPhong_YCu_Tim;
            VM.mNhapTraTuKhoPhong_PhieuMoi = mNhapTraTuKhoPhong_YCu_Tim;
            VM.mNhapTraTuKhoPhong_XemIn = mNhapTraTuKhoPhong_YCu_Tim;
            VM.mNhapTraTuKhoPhong_In = mNhapTraTuKhoPhong_YCu_Tim;
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
           SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi =Globals.TitleForm;
            VM.mNhapTraTuKhoPhong_Tim = mNhapTraTuKhoPhong_HoaChat_Tim;
            VM.mNhapTraTuKhoPhong_PhieuMoi = mNhapTraTuKhoPhong_HoaChat_Tim;
            VM.mNhapTraTuKhoPhong_XemIn = mNhapTraTuKhoPhong_HoaChat_Tim;
            VM.mNhapTraTuKhoPhong_In = mNhapTraTuKhoPhong_HoaChat_Tim;

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
        #endregion

        #region Nhap & Phan Bo Chi Phi Member

        private void CostThuocCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
         
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
           SetHyperlinkSelectedStyle(source as Button);
           
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
            Globals.TitleForm = eHCMSResources.Z0769_G1_KKeThuocKhDuoc.ToUpper();
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
           SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi =Globals.TitleForm;

            VM.mTim = mKiemKe_YCu_Tim;
            VM.mThemMoi = mKiemKe_YCu_ThemMoi;
            VM.mXuatExcel = mKiemKe_YCu_XuatExcel;
            VM.mXemIn = mKiemKe_YCu_XemIn;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKYCuCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0770_G1_KKeYCuKhDuoc.ToUpper();
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
           SetHyperlinkSelectedStyle(source as Button);
          
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
            Globals.TitleForm = eHCMSResources.Z0771_G1_KKeHChatKhDuoc.ToUpper();
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
        #endregion

        #region Report Member

        #region Báo cáo hàng có giới hạn số lượng xuất

        private void WatchMedCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
        private void NhapXuatTonThuocCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
         
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.mXemIn = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            VM.mKetChuyen = mBaoCaoXuatNhapTon_Thuoc_KetChuyen;
            VM.CanSelectedRefGenDrugCatID_1 = true;
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

        private void NhapXuatTonYCuCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
       
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
           SetHyperlinkSelectedStyle(source as Button);
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
        #endregion

        #region Doanh Thu Nhap Xuat Ton
        private void DTNhapXuatTonThuocCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);
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
        #endregion

        #region The Kho Member
        private void TheKhoThuocCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
         
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
           SetHyperlinkSelectedStyle(source as Button);
         
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
           SetHyperlinkSelectedStyle(source as Button);
         
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
        #endregion

        #region Phieu DN TT Member

        private void SuggestThuocCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
        
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
          
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
           SetHyperlinkSelectedStyle(source as Button);
      
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
           SetHyperlinkSelectedStyle(source as Button);
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
        #endregion

        #region nhap hang hang thang theo invoice

        private void BangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd_In(object source)
        {
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
      
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
           SetHyperlinkSelectedStyle(source as Button);
         
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

        #endregion

        /*TMA 23/10/2017 - Báo cáo nhập*/
        #region Bao Cao Nhap Member

        private void NhapThuocCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);
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
        
        #endregion
        /*TMA 23/10/2017 - Báo cáo nhập*/

        /*▼====: #003*/
        #region Temp Inward Report

        private void TempInwardMedReportCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
        
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
           SetHyperlinkSelectedStyle(source as Button);
         
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
         
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
          
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatHangKyGoi>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mXuatHangKyGui_Tim = mXuatHangKyGui_Thuoc_Tim;
            UnitVM.mXuatHangKyGui_PhieuMoi = mXuatHangKyGui_Thuoc_PhieuMoi;
            UnitVM.mXuatHangKyGui_Save = mXuatHangKyGui_Thuoc_Save;
            UnitVM.mXuatHangKyGui_ThuTien = mXuatHangKyGui_Thuoc_ThuTien;
            UnitVM.mXuatHangKyGui_XemIn = mXuatHangKyGui_Thuoc_XemIn;
            UnitVM.mXuatHangKyGui_In = mXuatHangKyGui_Thuoc_In;
            UnitVM.mXuatHangKyGui_DeleteInvoice = mXuatHangKyGui_Thuoc_DeleteInvoice;
            UnitVM.mXuatHangKyGui_PrintReceipt = mXuatHangKyGui_Thuoc_PrintReceipt;
            UnitVM.mIsInputTemp = true;
            UnitVM.LoadRefOutputType();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBThuocHangKyGoiCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0789_G1_XuatHgKGThuocTuKhDuoc;
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
           SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IXuatHangKyGoi>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi =Globals.TitleForm;
            UnitVM.mXuatHangKyGui_Tim = mXuatHangKyGui_YCu_Tim;
            UnitVM.mXuatHangKyGui_PhieuMoi = mXuatHangKyGui_YCu_PhieuMoi;
            UnitVM.mXuatHangKyGui_Save = mXuatHangKyGui_YCu_Save;
            UnitVM.mXuatHangKyGui_ThuTien = mXuatHangKyGui_YCu_ThuTien;
            UnitVM.mXuatHangKyGui_XemIn = mXuatHangKyGui_YCu_XemIn;
            UnitVM.mXuatHangKyGui_In = mXuatHangKyGui_YCu_In;
            UnitVM.mXuatHangKyGui_DeleteInvoice = mXuatHangKyGui_YCu_DeleteInvoice;
            UnitVM.mXuatHangKyGui_PrintReceipt = mXuatHangKyGui_YCu_PrintReceipt;

            UnitVM.mIsInputTemp = true;
            UnitVM.LoadRefOutputType();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBYCuHangKyGoiCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0790_G1_XuatHgKGYCuTuKhDuoc;
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
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
           SetHyperlinkSelectedStyle(source as Button);
          
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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

            var module = Globals.GetViewModel<IDrugModule>();
            var DrugClassVM = Globals.GetViewModel<IDrugDeptClass>();
            Globals.PageName = Globals.TitleForm;
            DrugClassVM.TitleForm = Globals.TitleForm;
            DrugClassVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            DrugClassVM.LoadFamilyParent((long)AllLookupValues.MedProductType.THUOC);
            DrugClassVM.GetSearchTreeView((long)AllLookupValues.MedProductType.THUOC);

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
            SetHyperlinkSelectedStyle(source as Button);

            var module = Globals.GetViewModel<IDrugModule>();
            var DrugClassVM = Globals.GetViewModel<IDrugDeptClass>();
            Globals.PageName = Globals.TitleForm;
            DrugClassVM.TitleForm = Globals.TitleForm;
            DrugClassVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            DrugClassVM.LoadFamilyParent((long)AllLookupValues.MedProductType.Y_CU);
            DrugClassVM.GetSearchTreeView((long)AllLookupValues.MedProductType.Y_CU);

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
            SetHyperlinkSelectedStyle(source as Button);

            var module = Globals.GetViewModel<IDrugModule>();
            var DrugClassVM = Globals.GetViewModel<IDrugDeptClass>();
            Globals.PageName = Globals.TitleForm;
            DrugClassVM.TitleForm = Globals.TitleForm;
            DrugClassVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            DrugClassVM.LoadFamilyParent((long)AllLookupValues.MedProductType.HOA_CHAT);
            DrugClassVM.GetSearchTreeView((long)AllLookupValues.MedProductType.HOA_CHAT);

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
            SetHyperlinkSelectedStyle(source as Button);

            var module = Globals.GetViewModel<IDrugModule>();
            var DrugClassVM = Globals.GetViewModel<IGenericClass>();
            Globals.PageName = Globals.TitleForm;
            DrugClassVM.TitleForm = Globals.TitleForm;
            DrugClassVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOAT_CHAT;
            DrugClassVM.LoadFamilyParent((long)AllLookupValues.MedProductType.HOAT_CHAT);
            DrugClassVM.GetSearchTreeView((long)AllLookupValues.MedProductType.HOAT_CHAT);

            DrugClassVM.bTim = mQLNhomHang_HoatChat_Tim;
            DrugClassVM.bThem = mQLNhomHang_HoatChat_ThemMoi;
            DrugClassVM.bChinhSua = mQLNhomHang_HoatChat_ChinhSua;

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

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
            SetHyperlinkSelectedStyle(source as Button);

            Globals.PageName = Globals.TitleForm + ": " + SubName;

            var module = Globals.GetViewModel<IDrugModule>();
            var UnitVM = Globals.GetViewModel<IBidDetail>();
            UnitVM.IsMedDept = true;
            if ((source as Button).Name == "MedBidDetailCmd")
                UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            else if ((source as Button).Name == "MatBidDetailCmd")
                UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            else UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BidDetailCmd(object source)
        {
            string SubName = "";
            switch ((source as Button).Name)
            {
                case "MedBidDetailCmd":
                    SubName = eHCMSResources.G0787_G1_Thuoc;
                    break;
                case "MatBidDetailCmd":
                    SubName = eHCMSResources.G2907_G1_YCu;
                    break;
                default:
                    SubName = eHCMSResources.T1616_G1_HC;
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
        /*▼====: #002*/
        #region Đổi Giá Bán Theo Lô
        private void Clinic_AdjustOutPrice_In(object source, long MedType = (long)AllLookupValues.MedProductType.THUOC)
        {
            SetHyperlinkSelectedStyle(source as Button);

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
    }
}


//private void RequestThuocCmd_In(object source)
//{
//   SetHyperlinkSelectedStyle(source as Button);
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
//   SetHyperlinkSelectedStyle(source as Button);
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
//   SetHyperlinkSelectedStyle(source as Button);
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