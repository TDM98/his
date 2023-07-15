/*
 * 20170803 #001 CMN: Add HI Store Service
 * 20170810 #002 CMN: Added Bid Service
 * 20180412 #003 TTM: BM 0005324: Tạo mới màn hình xuất thuốc từ khoa dược vào kho BHYT.
 * 20181219 #004 TTM: BM 0005443: Tạo mới màn hình lập phiếu lĩnh kho BHYT - Nhà thuốc.
 * 20190307 #005 TNHX: Bao cao thu tien cua nha thuoc tai quay BHYT
 * 20190503 #006 TNHX: [BM0006812] [BM0006813] Create XRpt_TKTheoDoiTTChiTietKH_NT, TKTheoDoiNXTThuocKhac_NT
 * 20190610 #007 TNHX: [BM0010768] Create XRpt_BCXuatDuocNoiBo_NT
 * 20190704 #008 TNHX: [BM0011926] Create NT_BanThuocLe
 * 20190927 #009 TNHX: [BM0016380] Create InwardFromInternalExport in Pharmacy
 * 20191004 #010 TNHX:  BM 	0017414 : Add report Z2847_G1_BCThuocSapHetHanDung
 * 20200109 #011 TNHX:  Add report BC_THUTIEN_NT_TAI_QUAY_BHYT_THEO_BIEN_LAI
 * 20200313 #012 TNHX:  Add report BKXUatThuocTheoBN
 * 20200831 #013 TNHX:  Add report XRpt_TKTheoDoiTTChiTietKH_NT theo thuốc
 * 20211116 #014 QTD:   Bổ sung phân quyền Lập phiếu lĩnh, 
 * 20230211 #015 QTD:   Thêm Menu Đơn thuốc điện tử
 */
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Windows;
using aEMR.Infrastructure.Events;
using eHCMSLanguage;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacyTopMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyTopMenuViewModel : Conductor<object>, IPharmacyTopMenu
       , IHandle<ItemSelected<IMessageBox, AxMessageBoxResult>>
    {
        public bool mBid { get; set; } = true;
        public bool mBidDetail { get; set; } = true;
        /*▲====: #002*/
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PharmacyTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.PageName = "";
            Globals.TitleForm = "";

            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            authorization();
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

            #region Management member
            bUnitCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyDonViTinh);
            bSupplierCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyNhaCungCap);
            bStorageCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyKho);
            bFormulaCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyCongThucTinhGia);

            bDrugClassCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyLopThuoc);

            bRefGenDrugBHYTCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyDanhMucBHYT);

            bRefGenDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyDanhMucThuoc);
            bGenDrugContraIndicateCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyChongChiDinh);
            bSupplierAndDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyNCCVaNSX);

            bSupplierGenericDrugPrice_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyNhaCungCap);

            bPharmacySellingItemPrices = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyGiaBanThuoc);

            bPharmacySellingPriceList = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mQuanLyBangGiaBan);

            #endregion

            #region sell member

            bSellVisitorDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBanThuocLe);

            bSellPrescriptionDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBanThuocTheoToa);

            bCollectionDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mNhanThuoc);

            #endregion

            #region return member

            bReturnDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mTraHang);
            #endregion

            #region Estimation member

            bEstimationCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mDuTruThuoc);

            #endregion

            #region Order member

            bOrderCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mDatHang);
            #endregion

            #region inward drug member
            bInwardDrugFromSupplierCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mNhapHangTuNhaCungCap);
            #endregion

            #region Thong Ke Member
            bTonKhoCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mThongKe_TonKho);
            bSellOnDateCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mThongKe_BanTheoNgay);
            #endregion

            #region Bao cao member

            bNhapXuatTonCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_NhapXuatTon);

            //bNhapXuatTonTheoTungThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
            //                , (int)ePharmacy.mBCKhac_NhapXuatTonTungKho);

            bThuocHetHanDungCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_ThuocHetHanDung);
            bDSXuatThuocNoiBoCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_DSXuatThuocNoiBoNguoiMua);
            bDSThuocXuatChoBHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_DSXuatThuocChoBH);
            bBaoCaoBanThuocTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_BanThuocTongHop);
            bTheoDoiSoLuongThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_TheoDoiSoLuongThuoc);
            bTheoDoiMuonThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_TheoDoiMuonThuoc);
            bTraThuocTHCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_TraThuocTongHop);

            bDanhSachXuatThuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCXuat_DanhSachXuat);


            bNopTienHangNgayCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBaoCaoNopTienHangNgay);
            bBangKeCTPhatThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBaoCaoBangKeChiTietPhatThuoc);
            bDoanhThuBanCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCThang_DoanhThuBan);
            bKiemKeHangThangCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCThang_KiemKeHangThang);

            bBangKeNhapHangThangTheoSoPhieuNhapKhoCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCNhap_BangKeNhapHangThangTheoSoPhieu);
            bBangKeNhapThuocHangThangCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCNhap_BangKeNhapThuocHangThang);
            bSoKiemNhapThuocCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCNhap_SoKiemNhapThuoc);
            //bFormNhapKhoCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
            //                , (int)ePharmacy.mBCNhap_FormNhapKho);
            //bPhieuDeNghiThanhToanCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
            //                , (int)ePharmacy.mBCNhap_PhieuDeNghiThanhToan);
            bBangKeChungTuThanhToanCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCNhap_BangKeChungTuThanhToan);
            bTheoDoiCongNoCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCNhap_TheoDoiCongNo);
            bTheKhoCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCNhap_TheKho);

            bMau20Cmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCBH_ThongKeTongHopThuocThang);
            bDMThuocThanhToanBHYTThangCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCBH_DanhMucThuocThanhToanBHYTThang);

            bBangGiaBanDuKienThangCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBGBanHangThang_BGBThangDuKien);
            bBangGiaBanThangCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBGBanHangThang_BGBThang);
            #endregion

            #region Kiem Ke Member
            bStockTakesCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mKiemKeKho);
            #endregion

            #region outward member
            bDemageExpiryDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mXuatHuyThuocHetHan);
            bDemageDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mXuatHuy);
            bOutDrugCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mXuatNoiBo);

            #endregion

            #region dutru
            bTheoCongTyCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mDuTru_TongHopTheoSoPhieu);
            bBangBaoSoDTHangNgayDuaVaoSoAnToanCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mDuTru_HangNgayTheoSoAnToan);
            mDuTru_ThuocCanLayThemDeBan = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mDuTru_ThuocCanLayThemDeBan);
            mDuTru_TongHopTheoSoPhieu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mDuTru_TongHopTheoSoPhieu);
            mDuTru_TongHopTheoThuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                        , (int)ePharmacy.mDuTru_TongHopTheoThuoc);
            #endregion

            bRequestPharmacyCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mPhieuYeuCau);

            //Operation
            //mBaoCaoNopTienHangNgay_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
            //                                   , (int)ePharmacy.mBaoCaoNopTienHangNgay,
            //                                   (int)oPharmacyEx.mBaoCaoNopTienHangNgay_In, (int)ePermission.mView);


            mBaoCaoBangKeChiTietPhatThuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBaoCaoBangKeChiTietPhatThuoc,
                                               (int)oPharmacyEx.mBaoCaoBangKeChiTietPhatThuoc_Xem, (int)ePermission.mView);
            mBaoCaoBangKeChiTietPhatThuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBaoCaoBangKeChiTietPhatThuoc,
                                               (int)oPharmacyEx.mBaoCaoBangKeChiTietPhatThuoc_In, (int)ePermission.mView);

            mBCThang_DoanhThuBan_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCThang_DoanhThuBan,
                                               (int)oPharmacyEx.mBCThang_DoanhThuBan_Xem, (int)ePermission.mView);
            mBCThang_DoanhThuBan_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCThang_DoanhThuBan,
                                               (int)oPharmacyEx.mBCThang_DoanhThuBan_In, (int)ePermission.mView);

            mBCThang_BCKiemKeHangThang_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCThang_BCKiemKeHangThang,
                                               (int)oPharmacyEx.mBCThang_BCKiemKeHangThang_In, (int)ePermission.mView);
            mBCThang_BCKiemKeHangThang_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCThang_BCKiemKeHangThang,
                                               (int)oPharmacyEx.mBCThang_BCKiemKeHangThang_Xem, (int)ePermission.mView);

            mBCNhap_BangKeNhapHangThangTheoSoPhieu_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCNhap_BangKeNhapHangThangTheoSoPhieu,
                                               (int)oPharmacyEx.mBCNhap_BangKeNhapHangThangTheoSoPhieu_Xem, (int)ePermission.mView);
            mBCNhap_BangKeNhapHangThangTheoSoPhieu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                   , (int)ePharmacy.mBCNhap_BangKeNhapHangThangTheoSoPhieu,
                                                   (int)oPharmacyEx.mBCNhap_BangKeNhapHangThangTheoSoPhieu_In, (int)ePermission.mView);

            mDuTru_TongHopTheoThuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                   , (int)ePharmacy.mDuTru_TongHopTheoThuoc,
                                                   (int)oPharmacyEx.mDuTru_TongHopTheoThuoc_In, (int)ePermission.mView);
            mDuTru_TongHopTheoThuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                   , (int)ePharmacy.mDuTru_TongHopTheoThuoc,
                                                   (int)oPharmacyEx.mDuTru_TongHopTheoThuoc_Xem, (int)ePermission.mView);

            mBCKhac_DSXuatThuocNoiBoNguoiMua_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                   , (int)ePharmacy.mBCKhac_DSXuatThuocNoiBoNguoiMua,
                                                   (int)oPharmacyEx.mBCKhac_DSXuatThuocNoiBoNguoiMua_Xem, (int)ePermission.mView);
            mBCKhac_DSXuatThuocNoiBoNguoiMua_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                       , (int)ePharmacy.mBCKhac_DSXuatThuocNoiBoNguoiMua,
                                                       (int)oPharmacyEx.mBCKhac_DSXuatThuocNoiBoNguoiMua_In, (int)ePermission.mView);

            mBCKhac_DSXuatThuocNoiBoTenThuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                       , (int)ePharmacy.mBCKhac_DSXuatThuocNoiBoTenThuoc,
                                                       (int)oPharmacyEx.mBCKhac_DSXuatThuocNoiBoTenThuoc_Xem, (int)ePermission.mView);
            mBCKhac_DSXuatThuocNoiBoTenThuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                       , (int)ePharmacy.mBCKhac_DSXuatThuocNoiBoTenThuoc,
                                                       (int)oPharmacyEx.mBCKhac_DSXuatThuocNoiBoTenThuoc_In, (int)ePermission.mView);

            mBCKhac_DSXuatThuocChoBH_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                       , (int)ePharmacy.mBCKhac_DSXuatThuocChoBH,
                                                       (int)oPharmacyEx.mBCKhac_DSXuatThuocChoBH_Xem, (int)ePermission.mView);
            mBCKhac_DSXuatThuocChoBH_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                           , (int)ePharmacy.mBCKhac_DSXuatThuocChoBH,
                                                           (int)oPharmacyEx.mBCKhac_DSXuatThuocChoBH_In, (int)ePermission.mView);

            mBCKhac_BanThuocTongHop_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                           , (int)ePharmacy.mBCKhac_BanThuocTongHop,
                                                           (int)oPharmacyEx.mBCKhac_BanThuocTongHop_Xem, (int)ePermission.mView);
            mBCKhac_BanThuocTongHop_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                               , (int)ePharmacy.mBCKhac_BanThuocTongHop,
                                                               (int)oPharmacyEx.mBCKhac_BanThuocTongHop_In, (int)ePermission.mView);
            mBCKhac_TraThuocTongHop_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                               , (int)ePharmacy.mBCKhac_TraThuocTongHop,
                                                               (int)oPharmacyEx.mBCKhac_TraThuocTongHop_Xem, (int)ePermission.mView);
            mBCKhac_TraThuocTongHop_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                                   , (int)ePharmacy.mBCKhac_TraThuocTongHop,
                                                                   (int)oPharmacyEx.mBCKhac_TraThuocTongHop_In, (int)ePermission.mView);

            mBCNhap_BangKeNhapThuocHangThang_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                                   , (int)ePharmacy.mBCNhap_BangKeNhapThuocHangThang,
                                                                   (int)oPharmacyEx.mBCNhap_BangKeNhapThuocHangThang_Xem, (int)ePermission.mView);
            mBCNhap_BangKeNhapThuocHangThang_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                               , (int)ePharmacy.mBCNhap_BangKeNhapThuocHangThang,
                                                               (int)oPharmacyEx.mBCNhap_BangKeNhapThuocHangThang_In, (int)ePermission.mView);

            mBCNhap_SoKiemNhapThuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                       , (int)ePharmacy.mBCNhap_SoKiemNhapThuoc,
                                                       (int)oPharmacyEx.mBCNhap_SoKiemNhapThuoc_Xem, (int)ePermission.mView);
            mBCNhap_SoKiemNhapThuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                                , (int)ePharmacy.mBCNhap_SoKiemNhapThuoc,
                                                                (int)oPharmacyEx.mBCNhap_SoKiemNhapThuoc_In, (int)ePermission.mView);
            mBCNhap_SoKiemNhapThuoc_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                               , (int)ePharmacy.mBCNhap_SoKiemNhapThuoc,
                                                               (int)oPharmacyEx.mBCNhap_SoKiemNhapThuoc_XuatExcel, (int)ePermission.mView);

            mBCNhap_TheoDoiCongNo_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                               , (int)ePharmacy.mBCNhap_TheoDoiCongNo,
                                                               (int)oPharmacyEx.mBCNhap_TheoDoiCongNo_Xem, (int)ePermission.mView);
            mBCNhap_TheoDoiCongNo_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                                   , (int)ePharmacy.mBCNhap_TheoDoiCongNo,
                                                                   (int)oPharmacyEx.mBCNhap_TheoDoiCongNo_In, (int)ePermission.mView);

            mBCBH_ThongKeTongHopThuocThang_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                                   , (int)ePharmacy.mBCBH_ThongKeTongHopThuocThang,
                                                                   (int)oPharmacyEx.mBCBH_ThongKeTongHopThuocThang_Xem, (int)ePermission.mView);
            mBCBH_ThongKeTongHopThuocThang_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                                       , (int)ePharmacy.mBCBH_ThongKeTongHopThuocThang,
                                                                       (int)oPharmacyEx.mBCBH_ThongKeTongHopThuocThang_In, (int)ePermission.mView);

            mDuTru_TongHopTheoSoPhieu_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                                       , (int)ePharmacy.mDuTru_TongHopTheoSoPhieu,
                                                                       (int)oPharmacyEx.mDuTru_TongHopTheoSoPhieu_Xem, (int)ePermission.mView);
            mDuTru_TongHopTheoSoPhieu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                                       , (int)ePharmacy.mDuTru_TongHopTheoSoPhieu,
                                                                       (int)oPharmacyEx.mDuTru_TongHopTheoSoPhieu_In, (int)ePermission.mView);

            #region leftMenu
            mBanHang = bSellVisitorDrugCmd || bSellPrescriptionDrugCmd || bCollectionDrugCmd || bReturnDrugCmd;
            mDuTru = bEstimationCmd;
            mDatHang = bOrderCmd;
            //mNhapHang = bInwardDrugFromSupplierCmd || bInwardDrugFromSupplierCmd;
            mYeuCau = bRequestPharmacyCmd;
            //mXuatHang = bDemageExpiryDrugCmd || bOutDrugCmd || bDemageDrugCmd;
            mQuanLy = bUnitCmd || bSupplierCmd || bSupplierCmd || bStorageCmd || bFormulaCmd || bDrugClassCmd || bRefGenDrugBHYTCmd
                || bRefGenDrugCmd || bGenDrugContraIndicateCmd || bSupplierAndDrugCmd || bSupplierGenericDrugPrice_Mgnt
                || bPharmacySellingItemPrices || bPharmacySellingPriceList;

            mBaoCaoHangNgay = bNopTienHangNgayCmd;
            mBaoCaoThang = bDoanhThuBanCmd || bKiemKeHangThangCmd || bKiemKeHangThangCmd;
            mBaoCaoNhap = bBangKeNhapHangThangTheoSoPhieuNhapKhoCmd || bBangKeNhapThuocHangThangCmd || bSoKiemNhapThuocCmd
                || bBangKeChungTuThanhToanCmd || bTheoDoiCongNoCmd || bTheKhoCmd;
            mBaoCaoXuat = bDanhSachXuatThuoc;
            mBaoCaoBaoHiem = bMau20Cmd || bDMThuocThanhToanBHYTThangCmd;
            mBaoCaoBangGiaBanHangThang = bBangGiaBanDuKienThangCmd || bBangGiaBanThangCmd;
            //mBaoCaoKhac = bNhapXuatTonCmd || bThuocHetHanDungCmd || bDSXuatThuocNoiBoCmd || bDSXuatThuocNoiBoCmd
            //    || bDSThuocXuatChoBHCmd || bBaoCaoBanThuocTHCmd || bTheoDoiSoLuongThuocCmd || bTheoDoiMuonThuocCmd
            //    || bTraThuocTHCmd;
            mBaoCaoDuTru = mDuTru_ThuocCanLayThemDeBan || bBangBaoSoDTHangNgayDuaVaoSoAnToanCmd
                || mDuTru_TongHopTheoSoPhieu || mDuTru_TongHopTheoThuoc;
            //mBaoCao = mBaoCaoHangNgay || mBaoCaoThang || mBaoCaoNhap || mBaoCaoXuat || mBaoCaoBaoHiem
            //    || mBaoCaoBangGiaBanHangThang || mBaoCaoKhac || mBaoCaoDuTru;
            mThongKe = bTonKhoCmd || bSellOnDateCmd;

            #endregion

            //mBid = bRefGenDrugCmd;
            mBidDetail = bRefGenDrugCmd;

            //▼====: #014
            mLapPhieuLinh = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mLapPhieuLinh);

            bEstimationBHYTCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.bEstimationBHYTCmd);

            mTangGiamKK = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mTangGiamKiemKe);

            mXuatHang = bDemageExpiryDrugCmd || bOutDrugCmd || bDemageDrugCmd || mTangGiamKK;

            bInwardDrugFromOtherCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mNhapHangTuNguonKhac);
            bInwardDrugFromDrugDeptCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mNhapTuKhoDuoc);
            bInwardDrugFromInternalCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mNhapNoiBo);

            mNhapHang = bInwardDrugFromSupplierCmd || bInwardDrugFromDrugDeptCmd || bInwardDrugFromInternalCmd;

            bTheoDoiThuocGioiHanSL = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_TheoDoiThuocGioiHanSL);
            bThuTienTaiQuay = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_ThuTienTaiQuay);
            bThuTienTaiQuayTheoBienLai = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_ThuTienTaiQuayTheoBienLai);
            bXuatDuocNoiBo = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_XuatDuocNoiBo);
            bBanThuocLe = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_BanThuocLe);
            bThuocSapHetHan = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_ThuocSapHetHanDung);
            bBangKeXuatThuocTheoBN = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_BangKeXuatThuocTheoBN);
            bHuyHoanTien = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCKhac_HuyHoanTien);
            mBaoCaoKhac = bNhapXuatTonCmd || bThuocHetHanDungCmd || bDSXuatThuocNoiBoCmd || bDSXuatThuocNoiBoCmd
                || bDSThuocXuatChoBHCmd || bBaoCaoBanThuocTHCmd || bTheoDoiSoLuongThuocCmd || bTheoDoiMuonThuocCmd
                || bTraThuocTHCmd || bTheoDoiThuocGioiHanSL || bThuTienTaiQuay || bThuTienTaiQuayTheoBienLai || bXuatDuocNoiBo
                || bBanThuocLe || bThuocSapHetHan || bBangKeXuatThuocTheoBN || bHuyHoanTien;

            mBaoCao = mBaoCaoHangNgay || mBaoCaoThang || mBaoCaoNhap || mBaoCaoXuat || mBaoCaoBaoHiem
                || mBaoCaoBangGiaBanHangThang || mBaoCaoKhac || mBaoCaoDuTru;

            mChiTietThau = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mChiTietThau);

            mBid = mChiTietThau;

            //▲====: #014
            mElectronicPrescription = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mElectronicPrescription);
            mBCHuyDQGReport = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPharmacy
                            , (int)ePharmacy.mBCHuyDQGReport);
            gElectronicPrescription = mElectronicPrescription || mBCHuyDQGReport;

            CheckValidSystemMenu();
        }
        private void CheckValidSystemMenu()
        {
            if (Globals.IseHMSSystem)
            {
                mDatHang = false;
                mNhapHang = false;
                mXuatHang = false;
                mQuanLy = false;
                mBaoCao = false;
                mThongKe = false;
                mBid = false;
                bSellPrescriptionDrugCmd = false;
                bCollectionDrugCmd = false;
                bCollectionDrugHICmd = false;
                bReturnDrugCmd = false;
                bReturnDrugHICmd = false;
            }
        }
        #region bool properties
        private bool _bUnitCmd = true;
        private bool _bSupplierCmd = true;
        private bool _bStorageCmd = true;
        private bool _bFormulaCmd = true;
        private bool _bDrugClassCmd = true;
        private bool _bRefGenDrugBHYTCmd = true;
        private bool _bRefGenDrugCmd = true;
        private bool _bGenDrugContraIndicateCmd = true;
        private bool _bSupplierAndDrugCmd = true;
        private bool _bSupplierGenericDrugPrice_Mgnt = true;
        private bool _bPharmacySellingItemPrices = true;
        private bool _bPharmacySellingPriceList = true;
        private bool _bCollectionDrugHICmd = true;
        private bool _bReturnDrugHICmd = true;
        public bool bCollectionDrugHICmd
        {
            get
            {
                return _bCollectionDrugHICmd && IsEnableHIStore;
            }
            set
            {
                _bCollectionDrugHICmd = value;
                NotifyOfPropertyChange(() => bCollectionDrugHICmd);
            }
        }
        public bool bReturnDrugHICmd
        {
            get
            {
                return _bReturnDrugHICmd && IsEnableHIStore;
            }
            set
            {
                _bReturnDrugHICmd = value;
                NotifyOfPropertyChange(() => bReturnDrugHICmd);
            }
        }
        public bool bUnitCmd
        {
            get
            {
                return _bUnitCmd;
            }
            set
            {
                if (_bUnitCmd == value)
                    return;
                _bUnitCmd = value;
            }
        }
        public bool bSupplierCmd
        {
            get
            {
                return _bSupplierCmd;
            }
            set
            {
                if (_bSupplierCmd == value)
                    return;
                _bSupplierCmd = value;
            }
        }
        public bool bStorageCmd
        {
            get
            {
                return _bStorageCmd;
            }
            set
            {
                if (_bStorageCmd == value)
                    return;
                _bStorageCmd = value;
            }
        }
        public bool bFormulaCmd
        {
            get
            {
                return _bFormulaCmd;
            }
            set
            {
                if (_bFormulaCmd == value)
                    return;
                _bFormulaCmd = value;
            }
        }
        public bool bDrugClassCmd
        {
            get
            {
                return _bDrugClassCmd;
            }
            set
            {
                if (_bDrugClassCmd == value)
                    return;
                _bDrugClassCmd = value;
            }
        }
        public bool bRefGenDrugBHYTCmd
        {
            get
            {
                return _bRefGenDrugBHYTCmd;
            }
            set
            {
                if (_bRefGenDrugBHYTCmd == value)
                    return;
                _bRefGenDrugBHYTCmd = value;
            }
        }
        public bool bRefGenDrugCmd
        {
            get
            {
                return _bRefGenDrugCmd;
            }
            set
            {
                if (_bRefGenDrugCmd == value)
                    return;
                _bRefGenDrugCmd = value;
            }
        }
        public bool bGenDrugContraIndicateCmd
        {
            get
            {
                return _bGenDrugContraIndicateCmd;
            }
            set
            {
                if (_bGenDrugContraIndicateCmd == value)
                    return;
                _bGenDrugContraIndicateCmd = value;
            }
        }
        public bool bSupplierAndDrugCmd
        {
            get
            {
                return _bSupplierAndDrugCmd;
            }
            set
            {
                if (_bSupplierAndDrugCmd == value)
                    return;
                _bSupplierAndDrugCmd = value;
            }
        }
        public bool bSupplierGenericDrugPrice_Mgnt
        {
            get
            {
                return _bSupplierGenericDrugPrice_Mgnt;
            }
            set
            {
                if (_bSupplierGenericDrugPrice_Mgnt == value)
                    return;
                _bSupplierGenericDrugPrice_Mgnt = value;
            }
        }
        public bool bPharmacySellingItemPrices
        {
            get
            {
                return _bPharmacySellingItemPrices;
            }
            set
            {
                if (_bPharmacySellingItemPrices
                 == value)
                    return;
                _bPharmacySellingItemPrices
                 = value;
                NotifyOfPropertyChange(() => bPharmacySellingItemPrices
                );
            }
        }
        public bool bPharmacySellingPriceList
        {
            get
            {
                return _bPharmacySellingPriceList;
            }
            set
            {
                if (_bPharmacySellingPriceList == value)
                    return;
                _bPharmacySellingPriceList = value;
                NotifyOfPropertyChange(() => bPharmacySellingPriceList);
            }
        }
        private bool _bSellVisitorDrugCmd = true;
        private bool _bSellPrescriptionDrugCmd = true;
        private bool _bCollectionDrugCmd = true;
        public bool bSellVisitorDrugCmd
        {
            get
            {
                return _bSellVisitorDrugCmd;
            }
            set
            {
                if (_bSellVisitorDrugCmd == value)
                    return;
                _bSellVisitorDrugCmd = value;
            }
        }
        public bool bSellPrescriptionDrugCmd
        {
            get
            {
                return _bSellPrescriptionDrugCmd;
            }
            set
            {
                if (_bSellPrescriptionDrugCmd == value)
                    return;
                _bSellPrescriptionDrugCmd = value;
            }
        }
        public bool bCollectionDrugCmd
        {
            get
            {
                return _bCollectionDrugCmd && IsEnableHIStore;
            }
            set
            {
                if (_bCollectionDrugCmd == value)
                    return;
                _bCollectionDrugCmd = value;
            }
        }
        private bool _bReturnDrugCmd = true;
        public bool bReturnDrugCmd
        {
            get
            {
                return _bReturnDrugCmd && IsEnableHIStore;
            }
            set
            {
                if (_bReturnDrugCmd == value)
                    return;
                _bReturnDrugCmd = value;
            }
        }
        private bool _bEstimationCmd = true;
        public bool bEstimationCmd
        {
            get
            {
                return _bEstimationCmd;
            }
            set
            {
                if (_bEstimationCmd == value)
                    return;
                _bEstimationCmd = value;
            }
        }
        private bool _bOrderCmd = true;
        public bool bOrderCmd
        {
            get
            {
                return _bOrderCmd;
            }
            set
            {
                if (_bOrderCmd == value)
                    return;
                _bOrderCmd = value;
            }
        }
        private bool _bInwardDrugFromSupplierCmd = true;
        public bool bInwardDrugFromSupplierCmd
        {
            get
            {
                return _bInwardDrugFromSupplierCmd;
            }
            set
            {
                if (_bInwardDrugFromSupplierCmd == value)
                    return;
                _bInwardDrugFromSupplierCmd = value;
            }
        }
        private bool _bTonKhoCmd = true;
        private bool _bSellOnDateCmd = true;
        private bool _bNhapXuatTonCmd = true;
        private bool _bTheKhoCmd = true;
        private bool _bNhapXuatTonTheoTungThuocCmd = true;
        private bool _bThuocHetHanDungCmd = true;
        private bool _bNopTienHangNgayCmd = true;
        private bool _bBangKeCTPhatThuocCmd = true;
        private bool _bStockTakesCmd = true;
        private bool _bDemageExpiryDrugCmd = true;
        private bool _bDemageDrugCmd = true;
        private bool _bOutDrugCmd = true;
        private bool _bRequestPharmacyCmd = true;
        //------
        private bool _bDSXuatThuocNoiBoCmd = true;
        private bool _bDSThuocXuatChoBHCmd = true;
        private bool _bBaoCaoBanThuocTHCmd = true;
        private bool _bTheoDoiSoLuongThuocCmd = true;
        private bool _bTheoDoiMuonThuocCmd = true;
        private bool _bTraThuocTHCmd = true;
        private bool _bDoanhThuBanCmd = true;
        private bool _bKiemKeHangThangCmd = true;
        private bool _bBangKeNhapHangThangTheoSoPhieuNhapKhoCmd = true;
        private bool _bBangKeNhapThuocHangThangCmd = true;
        private bool _bSoKiemNhapThuocCmd = true;
        private bool _bFormNhapKhoCmd = true;
        private bool _bPhieuDeNghiThanhToanCmd = true;
        private bool _bBangKeChungTuThanhToanCmd = true;
        private bool _bTheoDoiCongNoCmd = true;
        private bool _bMau20Cmd = true;
        private bool _bDMThuocThanhToanBHYTThangCmd = true;
        private bool _bBangGiaBanDuKienThangCmd = true;
        private bool _bBangGiaBanThangCmd = true;
        public bool bDSXuatThuocNoiBoCmd
        {
            get
            {
                return _bDSXuatThuocNoiBoCmd;
            }
            set
            {
                if (_bDSXuatThuocNoiBoCmd == value)
                    return;
                _bDSXuatThuocNoiBoCmd = value;
            }
        }
        public bool bDSThuocXuatChoBHCmd
        {
            get
            {
                return _bDSThuocXuatChoBHCmd;
            }
            set
            {
                if (_bDSThuocXuatChoBHCmd == value)
                    return;
                _bDSThuocXuatChoBHCmd = value;
            }
        }
        public bool bBaoCaoBanThuocTHCmd
        {
            get
            {
                return _bBaoCaoBanThuocTHCmd;
            }
            set
            {
                if (_bBaoCaoBanThuocTHCmd == value)
                    return;
                _bBaoCaoBanThuocTHCmd = value;
            }
        }
        public bool bTheoDoiSoLuongThuocCmd
        {
            get
            {
                return _bTheoDoiSoLuongThuocCmd;
            }
            set
            {
                if (_bTheoDoiSoLuongThuocCmd == value)
                    return;
                _bTheoDoiSoLuongThuocCmd = value;
            }
        }
        public bool bTheoDoiMuonThuocCmd
        {
            get
            {
                return _bTheoDoiMuonThuocCmd;
            }
            set
            {
                if (_bTheoDoiMuonThuocCmd == value)
                    return;
                _bTheoDoiMuonThuocCmd = value;
            }
        }
        public bool bTraThuocTHCmd
        {
            get
            {
                return _bTraThuocTHCmd;
            }
            set
            {
                if (_bTraThuocTHCmd == value)
                    return;
                _bTraThuocTHCmd = value;
            }
        }
        public bool bDoanhThuBanCmd
        {
            get
            {
                return _bDoanhThuBanCmd;
            }
            set
            {
                if (_bDoanhThuBanCmd == value)
                    return;
                _bDoanhThuBanCmd = value;
            }
        }
        public bool bKiemKeHangThangCmd
        {
            get
            {
                return _bKiemKeHangThangCmd;
            }
            set
            {
                if (_bKiemKeHangThangCmd == value)
                    return;
                _bKiemKeHangThangCmd = value;
            }
        }
        public bool bBangKeNhapHangThangTheoSoPhieuNhapKhoCmd
        {
            get
            {
                return _bBangKeNhapHangThangTheoSoPhieuNhapKhoCmd;
            }
            set
            {
                if (_bBangKeNhapHangThangTheoSoPhieuNhapKhoCmd == value)
                    return;
                _bBangKeNhapHangThangTheoSoPhieuNhapKhoCmd = value;
            }
        }
        public bool bBangKeNhapThuocHangThangCmd
        {
            get
            {
                return _bBangKeNhapThuocHangThangCmd;
            }
            set
            {
                if (_bBangKeNhapThuocHangThangCmd == value)
                    return;
                _bBangKeNhapThuocHangThangCmd = value;
            }
        }
        public bool bSoKiemNhapThuocCmd
        {
            get
            {
                return _bSoKiemNhapThuocCmd;
            }
            set
            {
                if (_bSoKiemNhapThuocCmd == value)
                    return;
                _bSoKiemNhapThuocCmd = value;
            }
        }
        public bool bFormNhapKhoCmd
        {
            get
            {
                return _bFormNhapKhoCmd;
            }
            set
            {
                if (_bFormNhapKhoCmd == value)
                    return;
                _bFormNhapKhoCmd = value;
            }
        }
        public bool bPhieuDeNghiThanhToanCmd
        {
            get
            {
                return _bPhieuDeNghiThanhToanCmd;
            }
            set
            {
                if (_bPhieuDeNghiThanhToanCmd == value)
                    return;
                _bPhieuDeNghiThanhToanCmd = value;
            }
        }
        public bool bBangKeChungTuThanhToanCmd
        {
            get
            {
                return _bBangKeChungTuThanhToanCmd;
            }
            set
            {
                if (_bBangKeChungTuThanhToanCmd == value)
                    return;
                _bBangKeChungTuThanhToanCmd = value;
            }
        }
        public bool bTheoDoiCongNoCmd
        {
            get
            {
                return _bTheoDoiCongNoCmd;
            }
            set
            {
                if (_bTheoDoiCongNoCmd == value)
                    return;
                _bTheoDoiCongNoCmd = value;
            }
        }
        public bool bMau20Cmd
        {
            get
            {
                return _bMau20Cmd;
            }
            set
            {
                if (_bMau20Cmd == value)
                    return;
                _bMau20Cmd = value;
            }
        }
        public bool bDMThuocThanhToanBHYTThangCmd
        {
            get
            {
                return _bDMThuocThanhToanBHYTThangCmd;
            }
            set
            {
                if (_bDMThuocThanhToanBHYTThangCmd == value)
                    return;
                _bDMThuocThanhToanBHYTThangCmd = value;
            }
        }
        public bool bBangGiaBanDuKienThangCmd
        {
            get
            {
                return _bBangGiaBanDuKienThangCmd;
            }
            set
            {
                if (_bBangGiaBanDuKienThangCmd == value)
                    return;
                _bBangGiaBanDuKienThangCmd = value;
            }
        }
        public bool bBangGiaBanThangCmd
        {
            get
            {
                return _bBangGiaBanThangCmd;
            }
            set
            {
                if (_bBangGiaBanThangCmd == value)
                    return;
                _bBangGiaBanThangCmd = value;
            }
        }
        //-------------
        public bool bTonKhoCmd
        {
            get
            {
                return _bTonKhoCmd;
            }
            set
            {
                if (_bTonKhoCmd == value)
                    return;
                _bTonKhoCmd = value;
            }
        }
        public bool bSellOnDateCmd
        {
            get
            {
                return _bSellOnDateCmd;
            }
            set
            {
                if (_bSellOnDateCmd == value)
                    return;
                _bSellOnDateCmd = value;
            }
        }
        public bool bNhapXuatTonCmd
        {
            get
            {
                return _bNhapXuatTonCmd;
            }
            set
            {
                if (_bNhapXuatTonCmd == value)
                    return;
                _bNhapXuatTonCmd = value;
            }
        }
        public bool bTheKhoCmd
        {
            get
            {
                return _bTheKhoCmd;
            }
            set
            {
                if (_bTheKhoCmd == value)
                    return;
                _bTheKhoCmd = value;
            }
        }
        public bool bNhapXuatTonTheoTungThuocCmd
        {
            get
            {
                return _bNhapXuatTonTheoTungThuocCmd;
            }
            set
            {
                if (_bNhapXuatTonTheoTungThuocCmd == value)
                    return;
                _bNhapXuatTonTheoTungThuocCmd = value;
            }
        }
        public bool bThuocHetHanDungCmd
        {
            get
            {
                return _bThuocHetHanDungCmd;
            }
            set
            {
                if (_bThuocHetHanDungCmd == value)
                    return;
                _bThuocHetHanDungCmd = value;
            }
        }
        public bool bNopTienHangNgayCmd
        {
            get
            {
                return _bNopTienHangNgayCmd;
            }
            set
            {
                if (_bNopTienHangNgayCmd == value)
                    return;
                _bNopTienHangNgayCmd = value;
            }
        }
        public bool bBangKeCTPhatThuocCmd
        {
            get
            {
                return _bBangKeCTPhatThuocCmd;
            }
            set
            {
                if (_bBangKeCTPhatThuocCmd == value)
                    return;
                _bBangKeCTPhatThuocCmd = value;
            }
        }
        public bool bStockTakesCmd
        {
            get
            {
                return _bStockTakesCmd;
            }
            set
            {
                if (_bStockTakesCmd == value)
                    return;
                _bStockTakesCmd = value;
            }
        }
        public bool bDemageExpiryDrugCmd
        {
            get
            {
                return _bDemageExpiryDrugCmd;
            }
            set
            {
                if (_bDemageExpiryDrugCmd == value)
                    return;
                _bDemageExpiryDrugCmd = value;
            }
        }
        public bool bDemageDrugCmd
        {
            get
            {
                return _bDemageDrugCmd;
            }
            set
            {
                if (_bDemageDrugCmd == value)
                    return;
                _bDemageDrugCmd = value;
            }
        }
        public bool bOutDrugCmd
        {
            get
            {
                return _bOutDrugCmd;
            }
            set
            {
                if (_bOutDrugCmd == value)
                    return;
                _bOutDrugCmd = value;
            }
        }
        #region dutru

        private bool _mDuTru_ThuocCanLayThemDeBan = true;
        private bool _bTheoCongTyCmd = true;
        private bool _bBangBaoSoDTHangNgayDuaVaoSoAnToanCmd = true;
        public bool bTheoCongTyCmd
        {
            get
            {
                return _bTheoCongTyCmd;
            }
            set
            {
                if (_bTheoCongTyCmd == value)
                    return;
                _bTheoCongTyCmd = value;
            }
        }
        public bool bBangBaoSoDTHangNgayDuaVaoSoAnToanCmd
        {
            get
            {
                return _bBangBaoSoDTHangNgayDuaVaoSoAnToanCmd;
            }
            set
            {
                if (_bBangBaoSoDTHangNgayDuaVaoSoAnToanCmd == value)
                    return;
                _bBangBaoSoDTHangNgayDuaVaoSoAnToanCmd = value;
            }
        }

        public bool mDuTru_ThuocCanLayThemDeBan
        {
            get
            {
                return _mDuTru_ThuocCanLayThemDeBan;
            }
            set
            {
                if (_mDuTru_ThuocCanLayThemDeBan == value)
                    return;
                _mDuTru_ThuocCanLayThemDeBan = value;
            }
        }

        private bool _mDuTru_TongHopTheoSoPhieu = true;
        public bool mDuTru_TongHopTheoSoPhieu
        {
            get
            {
                return _mDuTru_TongHopTheoSoPhieu;
            }
            set
            {
                if (_mDuTru_TongHopTheoSoPhieu == value)
                    return;
                _mDuTru_TongHopTheoSoPhieu = value;
            }
        }
        private bool _mDuTru_TongHopTheoThuoc = true;
        public bool mDuTru_TongHopTheoThuoc
        {
            get
            {
                return _mDuTru_TongHopTheoThuoc;
            }
            set
            {
                if (_mDuTru_TongHopTheoThuoc == value)
                    return;
                _mDuTru_TongHopTheoThuoc = value;
            }
        }
        #endregion

        private bool _bDanhSachXuatThuoc = true;
        public bool bDanhSachXuatThuoc
        {
            get
            {
                return _bDanhSachXuatThuoc;
            }
            set
            {
                if (_bDanhSachXuatThuoc == value)
                    return;
                _bDanhSachXuatThuoc = value;
                NotifyOfPropertyChange(() => bDanhSachXuatThuoc);
            }
        }

        public bool bRequestPharmacyCmd
        {
            get
            {
                return _bRequestPharmacyCmd;
            }
            set
            {
                if (_bRequestPharmacyCmd == value)
                    return;
                _bRequestPharmacyCmd = value;
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

        #region operation propeties

        private bool _mBaoCaoNopTienHangNgay_Xem = true;
        private bool _mBaoCaoNopTienHangNgay_In = true;

        private bool _mBaoCaoBangKeChiTietPhatThuoc_Xem = true;
        private bool _mBaoCaoBangKeChiTietPhatThuoc_In = true;

        private bool _mBCThang_DoanhThuBan_Xem = true;
        private bool _mBCThang_DoanhThuBan_In = true;

        private bool _mBCThang_KiemKeHangThang_Tim = true;
        private bool _mBCThang_KiemKeHangThang_ChinhSua = true;
        private bool _mBCThang_KiemKeHangThang_XuatExcel = true;
        private bool _mBCThang_KiemKeHangThang_In = true;

        private bool _mBCThang_BCKiemKeHangThang_Xem = true;
        private bool _mBCThang_BCKiemKeHangThang_In = true;


        private bool _mBCNhap_BangKeNhapHangThangTheoSoPhieu_Xem = true;
        private bool _mBCNhap_BangKeNhapHangThangTheoSoPhieu_In = true;

        private bool _mBCNhap_BangKeNhapThuocHangThang_Xem = true;
        private bool _mBCNhap_BangKeNhapThuocHangThang_In = true;

        private bool _mBCNhap_SoKiemNhapThuoc_Xem = true;
        private bool _mBCNhap_SoKiemNhapThuoc_In = true;
        private bool _mBCNhap_SoKiemNhapThuoc_XuatExcel = true;

        private bool _mBCNhap_BangKeChungTuThanhToan_Tim = true;
        private bool _mBCNhap_BangKeChungTuThanhToan_ChinhSua = true;
        private bool _mBCNhap_BangKeChungTuThanhToan_InBangKe = true;
        private bool _mBCNhap_BangKeChungTuThanhToan_InDNTT = true;

        private bool _mBCNhap_TheoDoiCongNo_Xem = true;
        private bool _mBCNhap_TheoDoiCongNo_In = true;

        private bool _mBCNhap_TheKho_Xem = true;
        private bool _mBCNhap_TheKho_In = true;

        private bool _mBCXuat_DanhSachXuat_Xem = true;
        private bool _mBCXuat_DanhSachXuat_In = true;


        private bool _mBCBH_ThongKeTongHopThuocThang_Xem = true;
        private bool _mBCBH_ThongKeTongHopThuocThang_In = true;

        private bool _mBCBH_DanhMucThuocThanhToanBHYTThang_Xem = true;
        private bool _mBCBH_DanhMucThuocThanhToanBHYTThang_In = true;

        private bool _mBGBanHangThang_BGBThangDuKien_Xem = true;
        private bool _mBGBanHangThang_BGBThangDuKien_In = true;

        private bool _mBGBanHangThang_BGBThang_Xem = true;
        private bool _mBGBanHangThang_BGBThang_In = true;

        private bool _mBCKhac_NhapXuatTon_Xem = true;
        private bool _mBCKhac_NhapXuatTon_In = true;
        private bool _mBCKhac_NhapXuatTon_KetChuyen = true;

        private bool _mBCKhac_ThuocHetHanDung_Xem = true;
        private bool _mBCKhac_ThuocHetHanDung_In = true;

        private bool _mBCKhac_DSXuatThuocNoiBoNguoiMua_Xem = true;
        private bool _mBCKhac_DSXuatThuocNoiBoNguoiMua_In = true;

        private bool _mBCKhac_DSXuatThuocNoiBoTenThuoc_Xem = true;
        private bool _mBCKhac_DSXuatThuocNoiBoTenThuoc_In = true;

        private bool _mBCKhac_DSXuatThuocChoBH_Xem = true;
        private bool _mBCKhac_DSXuatThuocChoBH_In = true;

        private bool _mBCKhac_BanThuocTongHop_Xem = true;
        private bool _mBCKhac_BanThuocTongHop_In = true;

        private bool _mBCKhac_TheoDoiSoLuongThuoc_Xem = true;
        private bool _mBCKhac_TheoDoiSoLuongThuoc_In = true;

        private bool _mBCKhac_TheoDoiMuonThuoc_Xem = true;
        private bool _mBCKhac_TheoDoiMuonThuoc_In = true;

        private bool _mBCKhac_TraThuocTongHop_Xem = true;
        private bool _mBCKhac_TraThuocTongHop_In = true;

        private bool _mDuTru_ThuocCanLayThemDeBan_Xem = true;
        private bool _mDuTru_ThuocCanLayThemDeBan_In = true;

        private bool _mDuTru_HangNgayTheoSoAnToan_Xem = true;
        private bool _mDuTru_HangNgayTheoSoAnToan_In = true;

        private bool _mDuTru_TongHopTheoSoPhieu_Xem = true;
        private bool _mDuTru_TongHopTheoSoPhieu_In = true;

        private bool _mDuTru_TongHopTheoThuoc_Xem = true;
        private bool _mDuTru_TongHopTheoThuoc_In = true;

        private bool _mThongKe_TonKho_Xem = true;
        private bool _mThongKe_TonKho_In = true;

        private bool _mThongKe_BanTheoNgay_Xem = true;
        private bool _mThongKe_BanTheoNgay_In = true;

        public bool mBaoCaoNopTienHangNgay_Xem
        {
            get
            {
                return _mBaoCaoNopTienHangNgay_Xem;
            }
            set
            {
                if (_mBaoCaoNopTienHangNgay_Xem == value)
                    return;
                _mBaoCaoNopTienHangNgay_Xem = value;
            }
        }

        public bool mBaoCaoNopTienHangNgay_In
        {
            get
            {
                return _mBaoCaoNopTienHangNgay_In;
            }
            set
            {
                if (_mBaoCaoNopTienHangNgay_In == value)
                    return;
                _mBaoCaoNopTienHangNgay_In = value;
            }
        }

        public bool mBaoCaoBangKeChiTietPhatThuoc_Xem
        {
            get
            {
                return _mBaoCaoBangKeChiTietPhatThuoc_Xem;
            }
            set
            {
                if (_mBaoCaoBangKeChiTietPhatThuoc_Xem == value)
                    return;
                _mBaoCaoBangKeChiTietPhatThuoc_Xem = value;
            }
        }

        public bool mBaoCaoBangKeChiTietPhatThuoc_In
        {
            get
            {
                return _mBaoCaoBangKeChiTietPhatThuoc_In;
            }
            set
            {
                if (_mBaoCaoBangKeChiTietPhatThuoc_In == value)
                    return;
                _mBaoCaoBangKeChiTietPhatThuoc_In = value;
            }
        }

        public bool mBCThang_DoanhThuBan_Xem
        {
            get
            {
                return _mBCThang_DoanhThuBan_Xem;
            }
            set
            {
                if (_mBCThang_DoanhThuBan_Xem == value)
                    return;
                _mBCThang_DoanhThuBan_Xem = value;
            }
        }

        public bool mBCThang_DoanhThuBan_In
        {
            get
            {
                return _mBCThang_DoanhThuBan_In;
            }
            set
            {
                if (_mBCThang_DoanhThuBan_In == value)
                    return;
                _mBCThang_DoanhThuBan_In = value;
            }
        }

        public bool mBCThang_KiemKeHangThang_Tim
        {
            get
            {
                return _mBCThang_KiemKeHangThang_Tim;
            }
            set
            {
                if (_mBCThang_KiemKeHangThang_Tim == value)
                    return;
                _mBCThang_KiemKeHangThang_Tim = value;
                NotifyOfPropertyChange(() => mBCThang_KiemKeHangThang_Tim);
            }
        }

        public bool mBCThang_KiemKeHangThang_ChinhSua
        {
            get
            {
                return _mBCThang_KiemKeHangThang_ChinhSua;
            }
            set
            {
                if (_mBCThang_KiemKeHangThang_ChinhSua == value)
                    return;
                _mBCThang_KiemKeHangThang_ChinhSua = value;
                NotifyOfPropertyChange(() => mBCThang_KiemKeHangThang_ChinhSua);
            }
        }

        public bool mBCThang_KiemKeHangThang_XuatExcel
        {
            get
            {
                return _mBCThang_KiemKeHangThang_XuatExcel;
            }
            set
            {
                if (_mBCThang_KiemKeHangThang_XuatExcel == value)
                    return;
                _mBCThang_KiemKeHangThang_XuatExcel = value;
                NotifyOfPropertyChange(() => mBCThang_KiemKeHangThang_XuatExcel);
            }
        }

        public bool mBCThang_KiemKeHangThang_In
        {
            get
            {
                return _mBCThang_KiemKeHangThang_In;
            }
            set
            {
                if (_mBCThang_KiemKeHangThang_In == value)
                    return;
                _mBCThang_KiemKeHangThang_In = value;
                NotifyOfPropertyChange(() => mBCThang_KiemKeHangThang_In);
            }
        }

        public bool mBCThang_BCKiemKeHangThang_Xem
        {
            get
            {
                return _mBCThang_BCKiemKeHangThang_Xem;
            }
            set
            {
                if (_mBCThang_BCKiemKeHangThang_Xem == value)
                    return;
                _mBCThang_BCKiemKeHangThang_Xem = value;
                NotifyOfPropertyChange(() => mBCThang_BCKiemKeHangThang_Xem);
            }
        }

        public bool mBCThang_BCKiemKeHangThang_In
        {
            get
            {
                return _mBCThang_BCKiemKeHangThang_In;
            }
            set
            {
                if (_mBCThang_BCKiemKeHangThang_In == value)
                    return;
                _mBCThang_BCKiemKeHangThang_In = value;
                NotifyOfPropertyChange(() => mBCThang_BCKiemKeHangThang_In);
            }
        }

        public bool mBCNhap_BangKeNhapHangThangTheoSoPhieu_Xem
        {
            get
            {
                return _mBCNhap_BangKeNhapHangThangTheoSoPhieu_Xem;
            }
            set
            {
                if (_mBCNhap_BangKeNhapHangThangTheoSoPhieu_Xem == value)
                    return;
                _mBCNhap_BangKeNhapHangThangTheoSoPhieu_Xem = value;
                NotifyOfPropertyChange(() => mBCNhap_BangKeNhapHangThangTheoSoPhieu_Xem);
            }
        }

        public bool mBCNhap_BangKeNhapHangThangTheoSoPhieu_In
        {
            get
            {
                return _mBCNhap_BangKeNhapHangThangTheoSoPhieu_In;
            }
            set
            {
                if (_mBCNhap_BangKeNhapHangThangTheoSoPhieu_In == value)
                    return;
                _mBCNhap_BangKeNhapHangThangTheoSoPhieu_In = value;
                NotifyOfPropertyChange(() => mBCNhap_BangKeNhapHangThangTheoSoPhieu_In);
            }
        }

        public bool mBCNhap_BangKeNhapThuocHangThang_Xem
        {
            get
            {
                return _mBCNhap_BangKeNhapThuocHangThang_Xem;
            }
            set
            {
                if (_mBCNhap_BangKeNhapThuocHangThang_Xem == value)
                    return;
                _mBCNhap_BangKeNhapThuocHangThang_Xem = value;
                NotifyOfPropertyChange(() => mBCNhap_BangKeNhapThuocHangThang_Xem);
            }
        }

        public bool mBCNhap_BangKeNhapThuocHangThang_In
        {
            get
            {
                return _mBCNhap_BangKeNhapThuocHangThang_In;
            }
            set
            {
                if (_mBCNhap_BangKeNhapThuocHangThang_In == value)
                    return;
                _mBCNhap_BangKeNhapThuocHangThang_In = value;
                NotifyOfPropertyChange(() => mBCNhap_BangKeNhapThuocHangThang_In);
            }
        }

        public bool mBCNhap_SoKiemNhapThuoc_Xem
        {
            get
            {
                return _mBCNhap_SoKiemNhapThuoc_Xem;
            }
            set
            {
                if (_mBCNhap_SoKiemNhapThuoc_Xem == value)
                    return;
                _mBCNhap_SoKiemNhapThuoc_Xem = value;
                NotifyOfPropertyChange(() => mBCNhap_SoKiemNhapThuoc_Xem);
            }
        }

        public bool mBCNhap_SoKiemNhapThuoc_In
        {
            get
            {
                return _mBCNhap_SoKiemNhapThuoc_In;
            }
            set
            {
                if (_mBCNhap_SoKiemNhapThuoc_In == value)
                    return;
                _mBCNhap_SoKiemNhapThuoc_In = value;
                NotifyOfPropertyChange(() => mBCNhap_SoKiemNhapThuoc_In);
            }
        }

        public bool mBCNhap_SoKiemNhapThuoc_XuatExcel
        {
            get
            {
                return _mBCNhap_SoKiemNhapThuoc_XuatExcel;
            }
            set
            {
                if (_mBCNhap_SoKiemNhapThuoc_XuatExcel == value)
                    return;
                _mBCNhap_SoKiemNhapThuoc_XuatExcel = value;
                NotifyOfPropertyChange(() => mBCNhap_SoKiemNhapThuoc_XuatExcel);
            }
        }

        public bool mBCNhap_BangKeChungTuThanhToan_Tim
        {
            get
            {
                return _mBCNhap_BangKeChungTuThanhToan_Tim;
            }
            set
            {
                if (_mBCNhap_BangKeChungTuThanhToan_Tim == value)
                    return;
                _mBCNhap_BangKeChungTuThanhToan_Tim = value;
                NotifyOfPropertyChange(() => mBCNhap_BangKeChungTuThanhToan_Tim);
            }
        }

        public bool mBCNhap_BangKeChungTuThanhToan_ChinhSua
        {
            get
            {
                return _mBCNhap_BangKeChungTuThanhToan_ChinhSua;
            }
            set
            {
                if (_mBCNhap_BangKeChungTuThanhToan_ChinhSua == value)
                    return;
                _mBCNhap_BangKeChungTuThanhToan_ChinhSua = value;
                NotifyOfPropertyChange(() => mBCNhap_BangKeChungTuThanhToan_ChinhSua);
            }
        }

        public bool mBCNhap_BangKeChungTuThanhToan_InBangKe
        {
            get
            {
                return _mBCNhap_BangKeChungTuThanhToan_InBangKe;
            }
            set
            {
                if (_mBCNhap_BangKeChungTuThanhToan_InBangKe == value)
                    return;
                _mBCNhap_BangKeChungTuThanhToan_InBangKe = value;
                NotifyOfPropertyChange(() => mBCNhap_BangKeChungTuThanhToan_InBangKe);
            }
        }

        public bool mBCNhap_BangKeChungTuThanhToan_InDNTT
        {
            get
            {
                return _mBCNhap_BangKeChungTuThanhToan_InDNTT;
            }
            set
            {
                if (_mBCNhap_BangKeChungTuThanhToan_InDNTT == value)
                    return;
                _mBCNhap_BangKeChungTuThanhToan_InDNTT = value;
                NotifyOfPropertyChange(() => mBCNhap_BangKeChungTuThanhToan_InDNTT);
            }
        }

        public bool mBCNhap_TheoDoiCongNo_Xem
        {
            get
            {
                return _mBCNhap_TheoDoiCongNo_Xem;
            }
            set
            {
                if (_mBCNhap_TheoDoiCongNo_Xem == value)
                    return;
                _mBCNhap_TheoDoiCongNo_Xem = value;
                NotifyOfPropertyChange(() => mBCNhap_TheoDoiCongNo_Xem);
            }
        }

        public bool mBCNhap_TheoDoiCongNo_In
        {
            get
            {
                return _mBCNhap_TheoDoiCongNo_In;
            }
            set
            {
                if (_mBCNhap_TheoDoiCongNo_In == value)
                    return;
                _mBCNhap_TheoDoiCongNo_In = value;
                NotifyOfPropertyChange(() => mBCNhap_TheoDoiCongNo_In);
            }
        }

        public bool mBCNhap_TheKho_Xem
        {
            get
            {
                return _mBCNhap_TheKho_Xem;
            }
            set
            {
                if (_mBCNhap_TheKho_Xem == value)
                    return;
                _mBCNhap_TheKho_Xem = value;
                NotifyOfPropertyChange(() => mBCNhap_TheKho_Xem);
            }
        }

        public bool mBCNhap_TheKho_In
        {
            get
            {
                return _mBCNhap_TheKho_In;
            }
            set
            {
                if (_mBCNhap_TheKho_In == value)
                    return;
                _mBCNhap_TheKho_In = value;
                NotifyOfPropertyChange(() => mBCNhap_TheKho_In);
            }
        }

        public bool mBCXuat_DanhSachXuat_Xem
        {
            get
            {
                return _mBCXuat_DanhSachXuat_Xem;
            }
            set
            {
                if (_mBCXuat_DanhSachXuat_Xem == value)
                    return;
                _mBCXuat_DanhSachXuat_Xem = value;
                NotifyOfPropertyChange(() => mBCXuat_DanhSachXuat_Xem);
            }
        }

        public bool mBCXuat_DanhSachXuat_In
        {
            get
            {
                return _mBCXuat_DanhSachXuat_In;
            }
            set
            {
                if (_mBCXuat_DanhSachXuat_In == value)
                    return;
                _mBCXuat_DanhSachXuat_In = value;
                NotifyOfPropertyChange(() => mBCXuat_DanhSachXuat_In);
            }
        }

        public bool mBCBH_ThongKeTongHopThuocThang_Xem
        {
            get
            {
                return _mBCBH_ThongKeTongHopThuocThang_Xem;
            }
            set
            {
                if (_mBCBH_ThongKeTongHopThuocThang_Xem == value)
                    return;
                _mBCBH_ThongKeTongHopThuocThang_Xem = value;
                NotifyOfPropertyChange(() => mBCBH_ThongKeTongHopThuocThang_Xem);
            }
        }

        public bool mBCBH_ThongKeTongHopThuocThang_In
        {
            get
            {
                return _mBCBH_ThongKeTongHopThuocThang_In;
            }
            set
            {
                if (_mBCBH_ThongKeTongHopThuocThang_In == value)
                    return;
                _mBCBH_ThongKeTongHopThuocThang_In = value;
                NotifyOfPropertyChange(() => mBCBH_ThongKeTongHopThuocThang_In);
            }
        }

        public bool mBCBH_DanhMucThuocThanhToanBHYTThang_Xem
        {
            get
            {
                return _mBCBH_DanhMucThuocThanhToanBHYTThang_Xem;
            }
            set
            {
                if (_mBCBH_DanhMucThuocThanhToanBHYTThang_Xem == value)
                    return;
                _mBCBH_DanhMucThuocThanhToanBHYTThang_Xem = value;
                NotifyOfPropertyChange(() => mBCBH_DanhMucThuocThanhToanBHYTThang_Xem);
            }
        }

        public bool mBCBH_DanhMucThuocThanhToanBHYTThang_In
        {
            get
            {
                return _mBCBH_DanhMucThuocThanhToanBHYTThang_In;
            }
            set
            {
                if (_mBCBH_DanhMucThuocThanhToanBHYTThang_In == value)
                    return;
                _mBCBH_DanhMucThuocThanhToanBHYTThang_In = value;
                NotifyOfPropertyChange(() => mBCBH_DanhMucThuocThanhToanBHYTThang_In);
            }
        }

        public bool mBGBanHangThang_BGBThangDuKien_Xem
        {
            get
            {
                return _mBGBanHangThang_BGBThangDuKien_Xem;
            }
            set
            {
                if (_mBGBanHangThang_BGBThangDuKien_Xem == value)
                    return;
                _mBGBanHangThang_BGBThangDuKien_Xem = value;
                NotifyOfPropertyChange(() => mBGBanHangThang_BGBThangDuKien_Xem);
            }
        }

        public bool mBGBanHangThang_BGBThangDuKien_In
        {
            get
            {
                return _mBGBanHangThang_BGBThangDuKien_In;
            }
            set
            {
                if (_mBGBanHangThang_BGBThangDuKien_In == value)
                    return;
                _mBGBanHangThang_BGBThangDuKien_In = value;
                NotifyOfPropertyChange(() => mBGBanHangThang_BGBThangDuKien_In);
            }
        }

        public bool mBGBanHangThang_BGBThang_Xem
        {
            get
            {
                return _mBGBanHangThang_BGBThang_Xem;
            }
            set
            {
                if (_mBGBanHangThang_BGBThang_Xem == value)
                    return;
                _mBGBanHangThang_BGBThang_Xem = value;
                NotifyOfPropertyChange(() => mBGBanHangThang_BGBThang_Xem);
            }
        }

        public bool mBGBanHangThang_BGBThang_In
        {
            get
            {
                return _mBGBanHangThang_BGBThang_In;
            }
            set
            {
                if (_mBGBanHangThang_BGBThang_In == value)
                    return;
                _mBGBanHangThang_BGBThang_In = value;
                NotifyOfPropertyChange(() => mBGBanHangThang_BGBThang_In);
            }
        }

        public bool mBCKhac_NhapXuatTon_Xem
        {
            get
            {
                return _mBCKhac_NhapXuatTon_Xem;
            }
            set
            {
                if (_mBCKhac_NhapXuatTon_Xem == value)
                    return;
                _mBCKhac_NhapXuatTon_Xem = value;
                NotifyOfPropertyChange(() => mBCKhac_NhapXuatTon_Xem);
            }
        }

        public bool mBCKhac_NhapXuatTon_In
        {
            get
            {
                return _mBCKhac_NhapXuatTon_In;
            }
            set
            {
                if (_mBCKhac_NhapXuatTon_In == value)
                    return;
                _mBCKhac_NhapXuatTon_In = value;
                NotifyOfPropertyChange(() => mBCKhac_NhapXuatTon_In);
            }
        }

        public bool mBCKhac_NhapXuatTon_KetChuyen
        {
            get
            {
                return _mBCKhac_NhapXuatTon_KetChuyen;
            }
            set
            {
                if (_mBCKhac_NhapXuatTon_KetChuyen == value)
                    return;
                _mBCKhac_NhapXuatTon_KetChuyen = value;
                NotifyOfPropertyChange(() => mBCKhac_NhapXuatTon_KetChuyen);
            }
        }

        public bool mBCKhac_ThuocHetHanDung_Xem
        {
            get
            {
                return _mBCKhac_ThuocHetHanDung_Xem;
            }
            set
            {
                if (_mBCKhac_ThuocHetHanDung_Xem == value)
                    return;
                _mBCKhac_ThuocHetHanDung_Xem = value;
                NotifyOfPropertyChange(() => mBCKhac_ThuocHetHanDung_Xem);
            }
        }

        public bool mBCKhac_ThuocHetHanDung_In
        {
            get
            {
                return _mBCKhac_ThuocHetHanDung_In;
            }
            set
            {
                if (_mBCKhac_ThuocHetHanDung_In == value)
                    return;
                _mBCKhac_ThuocHetHanDung_In = value;
                NotifyOfPropertyChange(() => mBCKhac_ThuocHetHanDung_In);
            }
        }

        public bool mBCKhac_DSXuatThuocNoiBoNguoiMua_Xem
        {
            get
            {
                return _mBCKhac_DSXuatThuocNoiBoNguoiMua_Xem;
            }
            set
            {
                if (_mBCKhac_DSXuatThuocNoiBoNguoiMua_Xem == value)
                    return;
                _mBCKhac_DSXuatThuocNoiBoNguoiMua_Xem = value;
                NotifyOfPropertyChange(() => mBCKhac_DSXuatThuocNoiBoNguoiMua_Xem);
            }
        }

        public bool mBCKhac_DSXuatThuocNoiBoNguoiMua_In
        {
            get
            {
                return _mBCKhac_DSXuatThuocNoiBoNguoiMua_In;
            }
            set
            {
                if (_mBCKhac_DSXuatThuocNoiBoNguoiMua_In == value)
                    return;
                _mBCKhac_DSXuatThuocNoiBoNguoiMua_In = value;
                NotifyOfPropertyChange(() => mBCKhac_DSXuatThuocNoiBoNguoiMua_In);
            }
        }

        public bool mBCKhac_DSXuatThuocNoiBoTenThuoc_Xem
        {
            get
            {
                return _mBCKhac_DSXuatThuocNoiBoTenThuoc_Xem;
            }
            set
            {
                if (_mBCKhac_DSXuatThuocNoiBoTenThuoc_Xem == value)
                    return;
                _mBCKhac_DSXuatThuocNoiBoTenThuoc_Xem = value;
                NotifyOfPropertyChange(() => mBCKhac_DSXuatThuocNoiBoTenThuoc_Xem);
            }
        }

        public bool mBCKhac_DSXuatThuocNoiBoTenThuoc_In
        {
            get
            {
                return _mBCKhac_DSXuatThuocNoiBoTenThuoc_In;
            }
            set
            {
                if (_mBCKhac_DSXuatThuocNoiBoTenThuoc_In == value)
                    return;
                _mBCKhac_DSXuatThuocNoiBoTenThuoc_In = value;
                NotifyOfPropertyChange(() => mBCKhac_DSXuatThuocNoiBoTenThuoc_In);
            }
        }

        public bool mBCKhac_DSXuatThuocChoBH_Xem
        {
            get
            {
                return _mBCKhac_DSXuatThuocChoBH_Xem;
            }
            set
            {
                if (_mBCKhac_DSXuatThuocChoBH_Xem == value)
                    return;
                _mBCKhac_DSXuatThuocChoBH_Xem = value;
                NotifyOfPropertyChange(() => mBCKhac_DSXuatThuocChoBH_Xem);
            }
        }

        public bool mBCKhac_DSXuatThuocChoBH_In
        {
            get
            {
                return _mBCKhac_DSXuatThuocChoBH_In;
            }
            set
            {
                if (_mBCKhac_DSXuatThuocChoBH_In == value)
                    return;
                _mBCKhac_DSXuatThuocChoBH_In = value;
                NotifyOfPropertyChange(() => mBCKhac_DSXuatThuocChoBH_In);
            }
        }

        public bool mBCKhac_BanThuocTongHop_Xem
        {
            get
            {
                return _mBCKhac_BanThuocTongHop_Xem;
            }
            set
            {
                if (_mBCKhac_BanThuocTongHop_Xem == value)
                    return;
                _mBCKhac_BanThuocTongHop_Xem = value;
                NotifyOfPropertyChange(() => mBCKhac_BanThuocTongHop_Xem);
            }
        }

        public bool mBCKhac_BanThuocTongHop_In
        {
            get
            {
                return _mBCKhac_BanThuocTongHop_In;
            }
            set
            {
                if (_mBCKhac_BanThuocTongHop_In == value)
                    return;
                _mBCKhac_BanThuocTongHop_In = value;
                NotifyOfPropertyChange(() => mBCKhac_BanThuocTongHop_In);
            }
        }

        public bool mBCKhac_TheoDoiSoLuongThuoc_Xem
        {
            get
            {
                return _mBCKhac_TheoDoiSoLuongThuoc_Xem;
            }
            set
            {
                if (_mBCKhac_TheoDoiSoLuongThuoc_Xem == value)
                    return;
                _mBCKhac_TheoDoiSoLuongThuoc_Xem = value;
                NotifyOfPropertyChange(() => mBCKhac_TheoDoiSoLuongThuoc_Xem);
            }
        }

        public bool mBCKhac_TheoDoiSoLuongThuoc_In
        {
            get
            {
                return _mBCKhac_TheoDoiSoLuongThuoc_In;
            }
            set
            {
                if (_mBCKhac_TheoDoiSoLuongThuoc_In == value)
                    return;
                _mBCKhac_TheoDoiSoLuongThuoc_In = value;
                NotifyOfPropertyChange(() => mBCKhac_TheoDoiSoLuongThuoc_In);
            }
        }

        public bool mBCKhac_TheoDoiMuonThuoc_Xem
        {
            get
            {
                return _mBCKhac_TheoDoiMuonThuoc_Xem;
            }
            set
            {
                if (_mBCKhac_TheoDoiMuonThuoc_Xem == value)
                    return;
                _mBCKhac_TheoDoiMuonThuoc_Xem = value;
                NotifyOfPropertyChange(() => mBCKhac_TheoDoiMuonThuoc_Xem);
            }
        }

        public bool mBCKhac_TheoDoiMuonThuoc_In
        {
            get
            {
                return _mBCKhac_TheoDoiMuonThuoc_In;
            }
            set
            {
                if (_mBCKhac_TheoDoiMuonThuoc_In == value)
                    return;
                _mBCKhac_TheoDoiMuonThuoc_In = value;
                NotifyOfPropertyChange(() => mBCKhac_TheoDoiMuonThuoc_In);
            }
        }

        public bool mBCKhac_TraThuocTongHop_Xem
        {
            get
            {
                return _mBCKhac_TraThuocTongHop_Xem;
            }
            set
            {
                if (_mBCKhac_TraThuocTongHop_Xem == value)
                    return;
                _mBCKhac_TraThuocTongHop_Xem = value;
                NotifyOfPropertyChange(() => mBCKhac_TraThuocTongHop_Xem);
            }
        }

        public bool mBCKhac_TraThuocTongHop_In
        {
            get
            {
                return _mBCKhac_TraThuocTongHop_In;
            }
            set
            {
                if (_mBCKhac_TraThuocTongHop_In == value)
                    return;
                _mBCKhac_TraThuocTongHop_In = value;
                NotifyOfPropertyChange(() => mBCKhac_TraThuocTongHop_In);
            }
        }

        public bool mDuTru_ThuocCanLayThemDeBan_Xem
        {
            get
            {
                return _mDuTru_ThuocCanLayThemDeBan_Xem;
            }
            set
            {
                if (_mDuTru_ThuocCanLayThemDeBan_Xem == value)
                    return;
                _mDuTru_ThuocCanLayThemDeBan_Xem = value;
                NotifyOfPropertyChange(() => mDuTru_ThuocCanLayThemDeBan_Xem);
            }
        }

        public bool mDuTru_ThuocCanLayThemDeBan_In
        {
            get
            {
                return _mDuTru_ThuocCanLayThemDeBan_In;
            }
            set
            {
                if (_mDuTru_ThuocCanLayThemDeBan_In == value)
                    return;
                _mDuTru_ThuocCanLayThemDeBan_In = value;
                NotifyOfPropertyChange(() => mDuTru_ThuocCanLayThemDeBan_In);
            }
        }

        public bool mDuTru_HangNgayTheoSoAnToan_Xem
        {
            get
            {
                return _mDuTru_HangNgayTheoSoAnToan_Xem;
            }
            set
            {
                if (_mDuTru_HangNgayTheoSoAnToan_Xem == value)
                    return;
                _mDuTru_HangNgayTheoSoAnToan_Xem = value;
                NotifyOfPropertyChange(() => mDuTru_HangNgayTheoSoAnToan_Xem);
            }
        }

        public bool mDuTru_HangNgayTheoSoAnToan_In
        {
            get
            {
                return _mDuTru_HangNgayTheoSoAnToan_In;
            }
            set
            {
                if (_mDuTru_HangNgayTheoSoAnToan_In == value)
                    return;
                _mDuTru_HangNgayTheoSoAnToan_In = value;
                NotifyOfPropertyChange(() => mDuTru_HangNgayTheoSoAnToan_In);
            }
        }

        public bool mDuTru_TongHopTheoSoPhieu_Xem
        {
            get
            {
                return _mDuTru_TongHopTheoSoPhieu_Xem;
            }
            set
            {
                if (_mDuTru_TongHopTheoSoPhieu_Xem == value)
                    return;
                _mDuTru_TongHopTheoSoPhieu_Xem = value;
                NotifyOfPropertyChange(() => mDuTru_TongHopTheoSoPhieu_Xem);
            }
        }

        public bool mDuTru_TongHopTheoSoPhieu_In
        {
            get
            {
                return _mDuTru_TongHopTheoSoPhieu_In;
            }
            set
            {
                if (_mDuTru_TongHopTheoSoPhieu_In == value)
                    return;
                _mDuTru_TongHopTheoSoPhieu_In = value;
                NotifyOfPropertyChange(() => mDuTru_TongHopTheoSoPhieu_In);
            }
        }

        public bool mDuTru_TongHopTheoThuoc_Xem
        {
            get
            {
                return _mDuTru_TongHopTheoThuoc_Xem;
            }
            set
            {
                if (_mDuTru_TongHopTheoThuoc_Xem == value)
                    return;
                _mDuTru_TongHopTheoThuoc_Xem = value;
                NotifyOfPropertyChange(() => mDuTru_TongHopTheoThuoc_Xem);
            }
        }

        public bool mDuTru_TongHopTheoThuoc_In
        {
            get
            {
                return _mDuTru_TongHopTheoThuoc_In;
            }
            set
            {
                if (_mDuTru_TongHopTheoThuoc_In == value)
                    return;
                _mDuTru_TongHopTheoThuoc_In = value;
                NotifyOfPropertyChange(() => mDuTru_TongHopTheoThuoc_In);
            }
        }

        public bool mThongKe_TonKho_Xem
        {
            get
            {
                return _mThongKe_TonKho_Xem;
            }
            set
            {
                if (_mThongKe_TonKho_Xem == value)
                    return;
                _mThongKe_TonKho_Xem = value;
                NotifyOfPropertyChange(() => mThongKe_TonKho_Xem);
            }
        }

        public bool mThongKe_TonKho_In
        {
            get
            {
                return _mThongKe_TonKho_In;
            }
            set
            {
                if (_mThongKe_TonKho_In == value)
                    return;
                _mThongKe_TonKho_In = value;
                NotifyOfPropertyChange(() => mThongKe_TonKho_In);
            }
        }

        public bool mThongKe_BanTheoNgay_Xem
        {
            get
            {
                return _mThongKe_BanTheoNgay_Xem;
            }
            set
            {
                if (_mThongKe_BanTheoNgay_Xem == value)
                    return;
                _mThongKe_BanTheoNgay_Xem = value;
                NotifyOfPropertyChange(() => mThongKe_BanTheoNgay_Xem);
            }
        }

        public bool mThongKe_BanTheoNgay_In
        {
            get
            {
                return _mThongKe_BanTheoNgay_In;
            }
            set
            {
                if (_mThongKe_BanTheoNgay_In == value)
                    return;
                _mThongKe_BanTheoNgay_In = value;
                NotifyOfPropertyChange(() => mThongKe_BanTheoNgay_In);
            }
        }

        #region leftMenu
        private bool _mBanHang = true;
        private bool _mDuTru = true;
        private bool _mDatHang = true;
        private bool _mNhapHang = true;
        private bool _mYeuCau = true;
        private bool _mXuatHang = true;
        private bool _mQuanLy = true;
        private bool _mBaoCao = true;
        private bool _mBaoCaoHangNgay = true;
        private bool _mBaoCaoThang = true;
        private bool _mBaoCaoNhap = true;
        private bool _mBaoCaoXuat = true;
        private bool _mBaoCaoBaoHiem = true;
        private bool _mBaoCaoBangGiaBanHangThang = true;
        private bool _mBaoCaoKhac = true;
        private bool _mBaoCaoDuTru = true;
        private bool _mThongKe = true;

        public bool mBanHang
        {
            get
            {
                return _mBanHang;
            }
            set
            {
                if (_mBanHang
                 == value)
                    return;
                _mBanHang
                 = value;
                NotifyOfPropertyChange(() => mBanHang
                );
            }
        }


        public bool mDuTru
        {
            get
            {
                return _mDuTru
                ;
            }
            set
            {
                if (_mDuTru
                 == value)
                    return;
                _mDuTru
                 = value;
                NotifyOfPropertyChange(() => mDuTru
                );
            }
        }


        public bool mDatHang
        {
            get
            {
                return _mDatHang;
            }
            set
            {
                if (_mDatHang
                 == value)
                    return;
                _mDatHang
                 = value;
                NotifyOfPropertyChange(() => mDatHang
                );
            }
        }


        public bool mNhapHang
        {
            get
            {
                return _mNhapHang;
            }
            set
            {
                if (_mNhapHang
                 == value)
                    return;
                _mNhapHang
                 = value;
                NotifyOfPropertyChange(() => mNhapHang
                );
            }
        }


        public bool mYeuCau
        {
            get
            {
                return _mYeuCau
                ;
            }
            set
            {
                if (_mYeuCau
                 == value)
                    return;
                _mYeuCau
                 = value;
                NotifyOfPropertyChange(() => mYeuCau
                );
            }
        }


        public bool mXuatHang
        {
            get
            {
                return _mXuatHang;
            }
            set
            {
                if (_mXuatHang
                 == value)
                    return;
                _mXuatHang
                 = value;
                NotifyOfPropertyChange(() => mXuatHang
                );
            }
        }


        public bool mQuanLy
        {
            get
            {
                return _mQuanLy;
            }
            set
            {
                if (_mQuanLy
                 == value)
                    return;
                _mQuanLy
                 = value;
                NotifyOfPropertyChange(() => mQuanLy
                );
            }
        }

        public bool mBaoCao
        {
            get
            {
                return _mBaoCao;
            }
            set
            {
                if (_mBaoCao == value)
                {
                    return;
                }
                _mBaoCao = value;
                NotifyOfPropertyChange(() => mBaoCao);
            }
        }

        public bool mBaoCaoHangNgay
        {
            get
            {
                return _mBaoCaoHangNgay;
            }
            set
            {
                if (_mBaoCaoHangNgay == value)
                    return;
                _mBaoCaoHangNgay = value;
                NotifyOfPropertyChange(() => mBaoCaoHangNgay);
            }
        }

        public bool mBaoCaoThang
        {
            get
            {
                return _mBaoCaoThang;
            }
            set
            {
                if (_mBaoCaoThang == value)
                    return;
                _mBaoCaoThang = value;
                NotifyOfPropertyChange(() => mBaoCaoThang);
            }
        }

        public bool mBaoCaoNhap
        {
            get
            {
                return _mBaoCaoNhap;
            }
            set
            {
                if (_mBaoCaoNhap == value)
                    return;
                _mBaoCaoNhap = value;
                NotifyOfPropertyChange(() => mBaoCaoNhap);
            }
        }

        public bool mBaoCaoXuat
        {
            get
            {
                return _mBaoCaoXuat;
            }
            set
            {
                if (_mBaoCaoXuat == value)
                    return;
                _mBaoCaoXuat = value;
                NotifyOfPropertyChange(() => mBaoCaoXuat);
            }
        }

        public bool mBaoCaoBaoHiem
        {
            get
            {
                return _mBaoCaoBaoHiem;
            }
            set
            {
                if (_mBaoCaoBaoHiem == value)
                    return;
                _mBaoCaoBaoHiem = value;
                NotifyOfPropertyChange(() => mBaoCaoBaoHiem);
            }
        }

        public bool mBaoCaoBangGiaBanHangThang
        {
            get
            {
                return _mBaoCaoBangGiaBanHangThang;
            }
            set
            {
                if (_mBaoCaoBangGiaBanHangThang == value)
                    return;
                _mBaoCaoBangGiaBanHangThang = value;
                NotifyOfPropertyChange(() => mBaoCaoBangGiaBanHangThang);
            }
        }

        public bool mBaoCaoKhac
        {
            get
            {
                return _mBaoCaoKhac;
            }
            set
            {
                if (_mBaoCaoKhac == value)
                    return;
                _mBaoCaoKhac = value;
                NotifyOfPropertyChange(() => mBaoCaoKhac);
            }
        }

        public bool mBaoCaoDuTru
        {
            get
            {
                return _mBaoCaoDuTru;
            }
            set
            {
                if (_mBaoCaoDuTru == value)
                    return;
                _mBaoCaoDuTru = value;
                NotifyOfPropertyChange(() => mBaoCaoDuTru);
            }
        }

        public bool mThongKe
        {
            get
            {
                return _mThongKe;
            }
            set
            {
                if (_mThongKe == value)
                    return;
                _mThongKe = value;
                NotifyOfPropertyChange(() => mThongKe);
            }
        }
        #endregion
        #endregion
        #region cmd
        #region Management member

        private void UnitCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IUnits>();
            Globals.PageName = Globals.TitleForm;
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void UnitCmd()
        {
            Globals.TitleForm = eHCMSResources.Z0795_G1_QLyDMucDVT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                UnitCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        UnitCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PharmacieucalCompanyCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var SupplierVM = Globals.GetViewModel<IPharmacieucalCompany>();
            Globals.PageName = Globals.TitleForm;
            SupplierVM.TitleForm = Globals.TitleForm;

            module.MainContent = SupplierVM;
            (module as Conductor<object>).ActivateItem(SupplierVM);
        }

        public void PharmacieucalCompanyCmd()
        {
            Globals.TitleForm = eHCMSResources.Q0456_G1_QuanLyDanhMucNSX;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PharmacieucalCompanyCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmacieucalCompanyCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SupplierCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var SupplierVM = Globals.GetViewModel<ISuppliers>();
            Globals.PageName = Globals.TitleForm;
            SupplierVM.TitleForm = Globals.TitleForm;

            module.MainContent = SupplierVM;
            (module as Conductor<object>).ActivateItem(SupplierVM);
        }

        public void SupplierCmd()
        {
            Globals.TitleForm = eHCMSResources.Q0455_G1_QuanLyDanhMucNCC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SupplierCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SupplierCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void StorageCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var StorageVM = Globals.GetViewModel<IStorage>();
            Globals.PageName = Globals.TitleForm;
            StorageVM.TitleForm = Globals.TitleForm;

            module.MainContent = StorageVM;
            (module as Conductor<object>).ActivateItem(StorageVM);
        }

        public void StorageCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1604_G1_QLyDMucKho.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StorageCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StorageCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void FormulaCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var FormulaeVM = Globals.GetViewModel<IPharmacySellPriceProfitScale>();
            Globals.PageName = Globals.TitleForm;
            FormulaeVM.TitleForm = Globals.TitleForm;

            module.MainContent = FormulaeVM;
            (module as Conductor<object>).ActivateItem(FormulaeVM);
        }

        public void FormulaCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1605_G1_QLyCThucGia.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FormulaCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FormulaCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DrugClassCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var DrugClassVM = Globals.GetViewModel<IDrugClass>();
            Globals.PageName = Globals.TitleForm;
            DrugClassVM.TitleForm = Globals.TitleForm;

            module.MainContent = DrugClassVM;
            (module as Conductor<object>).ActivateItem(DrugClassVM);
        }

        public void DrugClassCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1606_G1_QLyDMucLopThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DrugClassCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugClassCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenDrugBHYTCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugBHYTVM = Globals.GetViewModel<IRefGenDrugBHYT_Category>();
            Globals.PageName = Globals.TitleForm;

            module.MainContent = RefGenDrugBHYTVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugBHYTVM);
        }

        public void RefGenDrugBHYTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z0796_G1_QLyDMucBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenDrugBHYTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenDrugBHYTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenDrugCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            object RefGenDrugVM = null;
            //20191003 TBL: Sử dụng màn hình mới nhất
            //if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
            //{
            //    RefGenDrugVM = Globals.GetViewModel<IRefGenDrugListNew>();
            //    ((IRefGenDrugListNew)RefGenDrugVM).TitleForm = Globals.TitleForm;
            //}
            //else
            //{
            //    RefGenDrugVM = Globals.GetViewModel<IRefGenDrugList>();
            //    ((IRefGenDrugList)RefGenDrugVM).TitleForm = Globals.TitleForm;
            //}
            RefGenDrugVM = Globals.GetViewModel<IRefGenDrugListNew>();
            ((IRefGenDrugListNew)RefGenDrugVM).TitleForm = Globals.TitleForm;

            Globals.PageName = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void RefGenDrugCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1607_G1_QLyDMucThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenDrugCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenDrugCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void GenDrugContraIndicateCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IPharmacyModule>();
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

        private void SupplierAndDrugCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<ISupplierAndDrug>();

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void SupplierAndDrugCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1609_G1_QLyNCCVaThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SupplierAndDrugCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SupplierAndDrugCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PharmaciaucalSupplierCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IPharmacieucalAndSupplier>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void PharmaciaucalSupplierCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1610_G1_QLyNSXVaNCC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PharmaciaucalSupplierCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmaciaucalSupplierCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SupplierGenericDrugPrice_Mgnt_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var VM = Globals.GetViewModel<ISupplierGenericDrugPrice_ListSupplier>();
            Globals.PageName = Globals.TitleForm;
            VM.TitleForm = Globals.TitleForm;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void SupplierGenericDrugPrice_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.Z1088_G1_QLyGiaNCC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SupplierGenericDrugPrice_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SupplierGenericDrugPrice_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PharmacySellingItemPrices_Mgnt_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var VM = Globals.GetViewModel<IPharmacySellingItemPrices_ListDrug>();
            Globals.PageName = Globals.TitleForm;
            VM.TitleForm = Globals.TitleForm;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void PharmacySellingItemPrices_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.Z1611_G1_QLyGiaBanThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PharmacySellingItemPrices_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmacySellingItemPrices_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PharmacySellingPriceList_Mgnt_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var VM = Globals.GetViewModel<IPharmacySellingPriceList>();
            VM.IsReport = false;
            Globals.PageName = Globals.TitleForm;
            VM.TitleForm = Globals.TitleForm;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void PharmacySellingPriceList_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.Z1806_G1_QLyBGiaThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PharmacySellingPriceList_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmacySellingPriceList_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region sell member
        private void SellVisitorDrugCmd_In(bool IsPrescriptionCollect = false)
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IVisitor>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            RefGenDrugVM.IsPrescriptionCollect = IsPrescriptionCollect;
            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void SellVisitorDrugCmd()
        {
            Globals.TitleForm = eHCMSResources.K1013_G1_BanThuocLe;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SellVisitorDrugCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SellVisitorDrugCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void SellVisitorDrugFromPrescriptionCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2656_G1_BanThuocLeTheoToa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SellVisitorDrugCmd_In(true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SellVisitorDrugCmd_In(true);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SellPrescriptionDrugCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IPrescription>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void SellPrescriptionDrugCmd()
        {
            Globals.TitleForm = eHCMSResources.K1015_G1_BanThuocTheoToa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SellPrescriptionDrugCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SellPrescriptionDrugCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });

            }
        }

        //==== #001
        private bool _IsEnableHIStore = Globals.ServerConfigSection.CommonItems.EnableHIStore;
        public bool IsEnableHIStore
        {
            get
            {
                return _IsEnableHIStore;
            }
            set
            {
                if (_IsEnableHIStore == value)
                {
                    return;
                }
                _IsEnableHIStore = value;
                NotifyOfPropertyChange(() => IsEnableHIStore);
            }
        }
        public bool EnableHIStore
        {
            get { return Globals.ServerConfigSection.CommonItems.EnableHIStore && !Globals.ServerConfigSection.CommonItems.MixedHIPharmacyStores && !Globals.IseHMSSystem; }
        }
        private void SellPrescriptionDrugCmdHI_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IPrescription>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            RefGenDrugVM.IsHIOutPt = true;
            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }
        public void SellPrescriptionDrugCmdHI()
        {
            Globals.TitleForm = eHCMSResources.Z2038_G1_BanThuocTheoToaBH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SellPrescriptionDrugCmdHI_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SellPrescriptionDrugCmdHI_In();
                        GlobalsNAV.msgb = null;
                    }
                });

            }
        }
        //==== #001

        public void CollectionDrugCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<ICollectionDrug>();
            Globals.PageName = Globals.TitleForm;
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void CollectionDrugCmd()
        {
            Globals.TitleForm = eHCMSResources.N0191_G1_NhanThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CollectionDrugCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CollectionDrugCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void CollectionDrugDVCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<ICollectionDrug>();
            Globals.PageName = Globals.TitleForm;
            UnitVM.TitleForm = Globals.TitleForm;
            UnitVM.bFlagStoreHI = false;
            UnitVM.bFlagPaidTime = true;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void CollectionDrugDVCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2381_G1_NhanThuocDV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CollectionDrugDVCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CollectionDrugDVCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void CollectionDrugHICmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<ICollectionDrug>();
            Globals.PageName = Globals.TitleForm;
            UnitVM.TitleForm = Globals.TitleForm;
            UnitVM.bFlagStoreHI = true;
            UnitVM.bFlagPaidTime = true;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void CollectionDrugHICmd()
        {
            Globals.TitleForm = eHCMSResources.Z2382_G1_NhanThuocBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CollectionDrugHICmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CollectionDrugHICmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▼====== #004
        private void RequestThuocCmdNew_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IPharmacyHIStoreReqForm>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.DoseVisibility = true;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mPhieuYeuCau_Tim = true;
            UnitVM.mPhieuYeuCau_Them = true;
            UnitVM.mPhieuYeuCau_Xoa = true;
            UnitVM.mPhieuYeuCau_XemIn = true;
            UnitVM.mPhieuYeuCau_In = true;
            UnitVM.UsedForRequestingDrug = true;
            UnitVM.IsSearchByGenericName = true;
            UnitVM.vIsSearchByGenericName = true;
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void RequestThuocNewCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1113_G1_LapPhLinhThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestThuocCmdNew_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestThuocCmdNew_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #004
        #endregion

        #region return member
        private void ReturnDrugCmd_In()
        {


            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IReturnDrug>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void ReturnDrugCmd()
        {
            Globals.TitleForm = eHCMSResources.G1657_G1_TraHg;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReturnDrugCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReturnDrugCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });

            }


        }

        private void ReturnDrugDVCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IReturnDrug>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            RefGenDrugVM.bFlagStoreHI = false;
            RefGenDrugVM.bFlagPaidTime = true;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void ReturnDrugDVCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2383_G1_TraHangDV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReturnDrugDVCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReturnDrugDVCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReturnDrugHICmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IReturnDrug>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            RefGenDrugVM.bFlagStoreHI = true;
            RefGenDrugVM.bFlagPaidTime = true;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void ReturnDrugHICmd()
        {
            Globals.TitleForm = eHCMSResources.Z2384_G1_TraHangBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReturnDrugHICmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReturnDrugHICmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion

        #region Estimation member

        private void EstimationCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IEstimationPharmacy>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            RefGenDrugVM.IsHIStorage = false;
            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void EstimationCmd()
        {
            Globals.TitleForm = eHCMSResources.K3922_G1_DuTruThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EstimationCmdForBHYT_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IEstimationPharmacy>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            RefGenDrugVM.IsHIStorage = true;
            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void EstimationCmdForBHYT()
        {
            Globals.TitleForm = eHCMSResources.Z2558_G1_DuTruThuocChoKhoBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EstimationCmdForBHYT_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EstimationCmdForBHYT_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion

        #region Order member

        private void OrderCmd_In()
        {


            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IPurchaseOrder>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }
        public void OrderCmd()
        {
            Globals.TitleForm = eHCMSResources.K3119_G1_DaTh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OrderCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OrderCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion

        #region inward drug member

        private void InwardDrugFromSupplierCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IInwardDrugSupplier>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void InwardDrugFromSupplierCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1080_G1_NhapHgTuNCC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void InwardDrugOtherCmd_In()
        {


            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IInwardDrugOther>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void InwardDrugOtherCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1081_G1_NhapHgTuNguonKhac;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugOtherCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugOtherCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Thong Ke Member

        private void TonKhoCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<ITonKho>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void TonKhoCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1345_G1_TKeTonKho;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TonKhoCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TonKhoCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SellOnDateCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<ISellOnDate>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void SellOnDateCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1346_G1_TKeBanTheoNg;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SellOnDateCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SellOnDateCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Bao cao member

        private void NhapXuatTonCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<INhapXuatTon>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void NhapXuatTonCmd()
        {
            Globals.TitleForm = eHCMSResources.K1066_G1_BCNXT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonDoanhThuCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<INhapXuatTon>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            RefGenDrugVM.HasValue = true;
            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void NhapXuatTonDoanhThuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2104_G1_BCDoanhThuNXT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonDoanhThuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonDoanhThuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonDoanhThu_NTCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<INhapXuatTon>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;
            RefGenDrugVM.HasNormalPrice = true;
            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void NhapXuatTonDoanhThu_NTCmd()
        {
            Globals.TitleForm = eHCMSResources.K1066_G1_BCNXT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonDoanhThu_NTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonDoanhThu_NTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheKhoCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<ITheKho>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void TheKhoCmd()
        {
            Globals.TitleForm = eHCMSResources.G0152_G1_TheKho;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ThuocHetHanDungCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IThuocHetHanDung>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
        }

        public void ThuocHetHanDungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1612_G1_BCThuocHetHanDung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ThuocHetHanDungCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ThuocHetHanDungCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //----------------sua o day

        private void DSXuatThuocNoiBoCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();

            UnitVM.eItem = ReportName.PHARMACY_XUATNOIBOTHEONGUOIBAN;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideXNB = true;
            UnitVM.bXem = mBCKhac_DSXuatThuocNoiBoNguoiMua_Xem;
            UnitVM.bIn = mBCKhac_DSXuatThuocNoiBoNguoiMua_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DSXuatThuocNoiBoCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1613_G1_XuatNoiBoTheoNguoiMua.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DSXuatThuocNoiBoCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DSXuatThuocNoiBoCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //---------------moi them
        private void DSXuatThuocNoiBoTheoTenThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_XUATNOIBOTHEOTENTHUOC;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideXNB = true;
            UnitVM.bXem = mBCKhac_DSXuatThuocNoiBoTenThuoc_Xem;
            UnitVM.bIn = mBCKhac_DSXuatThuocNoiBoTenThuoc_In;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DSXuatThuocNoiBoTheoTenThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1614_G1_XuatNoiBoTheoTenThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DSXuatThuocNoiBoTheoTenThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DSXuatThuocNoiBoTheoTenThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DSThuocXuatChoBHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_XUATTHUOCCHOBH;
            UnitVM.pageTitle = Globals.TitleForm;

            UnitVM.bXem = mBCKhac_DSXuatThuocChoBH_Xem;
            UnitVM.bIn = mBCKhac_DSXuatThuocChoBH_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DSThuocXuatChoBHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1615_G1_DSThuocXuatBH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DSThuocXuatChoBHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DSThuocXuatChoBHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheoDoiThuocCoGioiHanSLCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_THEODOITHUOCCOGIOIHANSL;
            UnitVM.pageTitle = Globals.TitleForm;

            UnitVM.bXem = true;
            UnitVM.bIn = true;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TheoDoiThuocCoGioiHanSLCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1616_G1_TheoDoiThuocBHCoGioiHanSLg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheoDoiThuocCoGioiHanSLCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheoDoiThuocCoGioiHanSLCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BaoCaoBanThuocTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_BANTHUOCTH;
            UnitVM.IsShowPaymentMode = true; //20210321 QTD thêm điều kiện bật Combobox Hình thức TT
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.bIn = mBCKhac_BanThuocTongHop_In;
            UnitVM.bXem = mBCKhac_BanThuocTongHop_Xem;
            UnitVM.RptParameters.HideStore = true;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BaoCaoBanThuocTHCmd()
        {
            Globals.TitleForm = eHCMSResources.K1020_G1_BanThuocTgHop;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoBanThuocTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoBanThuocTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheoDoiSoLuongThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportTheoDoiSoLuongThuoc>();
            UnitVM.eItem = ReportName.PHARMACY_THEODOISOLUONGTHUOC;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.bIn = mBCKhac_BanThuocTongHop_In;
            UnitVM.bXem = mBCKhac_BanThuocTongHop_Xem;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TheoDoiSoLuongThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.G0361_G1_TheoDoiSLggThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheoDoiSoLuongThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheoDoiSoLuongThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheoDoiMuonThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
        }

        public void TheoDoiMuonThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.G0354_G1_TheoDoiMuonThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheoDoiMuonThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheoDoiMuonThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TraThuocTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_TRATHUOCTH;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.bXem = mBCKhac_TraThuocTongHop_Xem;
            UnitVM.bIn = mBCKhac_TraThuocTongHop_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TraThuocTHCmd()
        {
            Globals.TitleForm = eHCMSResources.G1672_G1_TraThuocTHop;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TraThuocTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TraThuocTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //-------------------------------------
        //public void NopTienHangNgayCmd()
        //{
        //    var module = Globals.GetViewModel<IPharmacyModule>();
        //    var UnitVM = Globals.GetViewModel<IBaoCaoHangNgay>();
        //    UnitVM.eItem = ReportName.PHARMACY_BCHANGNGAY_NOPTIEN;
        //    UnitVM.pageTitle = eHCMSResources.K1068_G1_BCNopTienHgNg;
        //    UnitVM.bXem = mBaoCaoNopTienHangNgay_Xem;
        //    UnitVM.bIn = mBaoCaoNopTienHangNgay_In;
        //    module.MainContent = UnitVM;
        //    (module as Conductor<object>).ActivateItem(UnitVM);
        //}
        //public void BangKeCTPhatThuocCmd()
        //{
        //    var module = Globals.GetViewModel<IPharmacyModule>();
        //    var UnitVM = Globals.GetViewModel<IBaoCaoHangNgay>();
        //    UnitVM.eItem = ReportName.PHARMACY_BCHANGNGAY_PHATTHUOC;
        //    UnitVM.pageTitle = eHCMSResources.K1058_G1_BCCTietPhatThuocHgNg;

        //    UnitVM.bXem = mBaoCaoBangKeChiTietPhatThuoc_In;
        //    UnitVM.bIn = mBaoCaoBangKeChiTietPhatThuoc_Xem;

        //    module.MainContent = UnitVM;
        //    (module as Conductor<object>).ActivateItem(UnitVM);
        //}

        private void BCNopTienHangNgayCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IPharmacyOutwardDrugReport>();
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BCNopTienHangNgayCmd()
        {
            Globals.TitleForm = eHCMSResources.K1068_G1_BCNopTienHgNg;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCNopTienHangNgayCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCNopTienHangNgayCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //----chinh sua tiep

        private void DoanhThuBanCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_BAOCAOTONGHOPDOANHTHU;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.bIn = mBCThang_DoanhThuBan_In;
            UnitVM.bXem = mBCThang_DoanhThuBan_Xem;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DoanhThuBanCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1619_G1_BCThDThuBan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DoanhThuBanCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DoanhThuBanCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KiemKeHangThangCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IStockTakes>();
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void KiemKeHangThangCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1621_G1_Kke.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KiemKeHangThangCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KiemKeHangThangCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //moi them
        private void ReportKiemKeHangThangCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByMMYYYY>();
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.eItem = ReportName.PHARMACY_BCKIEMKEVADUTRU;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.bIn = mBCThang_BCKiemKeHangThang_In;
            UnitVM.bXem = mBCThang_BCKiemKeHangThang_Xem;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ReportKiemKeHangThangCmd()
        {
            Globals.TitleForm = eHCMSResources.K1060_G1_BCKKHgTh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportKiemKeHangThangCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportKiemKeHangThangCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BangKeNhapHangThangTheoSoPhieuNhapKhoCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.eItem = ReportName.PHARMACY_NHAPHANGTHANGTHEOSOPHIEU;
            UnitVM.bIn = mBCNhap_BangKeNhapHangThangTheoSoPhieu_In;
            UnitVM.bXem = mBCNhap_BangKeNhapHangThangTheoSoPhieu_Xem;
            UnitVM.RptParameters.HideInwardSource = true;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangKeNhapHangThangTheoSoPhieuNhapKhoCmd()
        {
            Globals.TitleForm = eHCMSResources.K1041_G1_BKeNHgThTheoPhNKho;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapHangThangTheoSoPhieuNhapKhoCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapHangThangTheoSoPhieuNhapKhoCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BangKeNhapThuocHangThangCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_NHAPTHUOCHANGTHANG;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.bXem = mBCNhap_BangKeNhapThuocHangThang_Xem;
            UnitVM.bIn = mBCNhap_BangKeNhapThuocHangThang_In;
            UnitVM.RptParameters.HideInwardSource = true;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangKeNhapThuocHangThangCmd()
        {
            Globals.TitleForm = eHCMSResources.K1042_G1_BKeNhapThuocHgTh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeNhapThuocHangThangCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeNhapThuocHangThangCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SoKiemNhapThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_SOKIEMNHAPTHUOC;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.bXem = mBCNhap_SoKiemNhapThuoc_Xem;
            UnitVM.bIn = mBCNhap_SoKiemNhapThuoc_In;
            UnitVM.bXuatExcel = mBCNhap_SoKiemNhapThuoc_XuatExcel;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void SoKiemNhapThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.S0652_G1_SoKiemNhapThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SoKiemNhapThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SoKiemNhapThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BangKeChungTuThanhToanCmd_In()
        {
            Globals.TitleForm = eHCMSResources.K1040_G1_BKeCTuTToan;
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IBangKeChungTuThanhToan>();
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangKeChungTuThanhToanCmd()
        {
            Globals.TitleForm = eHCMSResources.K1040_G1_BKeCTuTToan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangKeChungTuThanhToanCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeChungTuThanhToanCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheoDoiCongNoCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_THEODOICONGNO;
            UnitVM.RptParameters.HideTypCongNo = true;
            UnitVM.RptParameters.ShowFirst = eHCMSResources.Z0095_G1_NhapKho.ToUpper();
            UnitVM.RptParameters.ShowTitle = eHCMSResources.G0345_G1_TheoDoiChuaTraTienoanHDon.ToUpper();
            UnitVM.pageTitle = eHCMSResources.K1064_G1_BCNhapTheoDoiCNo;

            UnitVM.bXem = mBCNhap_TheoDoiCongNo_Xem;
            UnitVM.bIn = mBCNhap_TheoDoiCongNo_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TheoDoiCongNoCmd()
        {
            Globals.TitleForm = eHCMSResources.K1064_G1_BCNhapTheoDoiCNo;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheoDoiCongNoCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheoDoiCongNoCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Mau20Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.TEMP20_NGOAITRU;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.bXem = mBCBH_ThongKeTongHopThuocThang_Xem;
            UnitVM.bIn = mBCBH_ThongKeTongHopThuocThang_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Mau20Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z0170_G1_TKeTgHopThuocMau20;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Mau20Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Mau20Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DMThuocThanhToanBHYTThangCmd_In()
        {
            // IPharmacySellingPriceList
            Globals.PageName = Globals.TitleForm;
            MessageBox.Show(eHCMSResources.Z0563_G1_ChuaLam);
        }

        public void DMThuocThanhToanBHYTThangCmd()
        {
            Globals.TitleForm = eHCMSResources.K2910_G1_DMucThuocTToanBHYTTh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DMThuocThanhToanBHYTThangCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DMThuocThanhToanBHYTThangCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BangGiaBanDuKienThangCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IPharmacySellingPriceList>();
            UnitVM.TitleForm = Globals.TitleForm;
            UnitVM.IsReport = true;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangGiaBanDuKienThangCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1477_G1_BGiaDuKien;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangGiaBanDuKienThangCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangGiaBanDuKienThangCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BangGiaBanThangCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IPharmacySellingPriceList>();
            UnitVM.IsReport = true;
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangGiaBanThangCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1478_G1_BCBGiaBan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangGiaBanThangCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangGiaBanThangCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void HeSoAnToanBanCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByStorage>();
            UnitVM.eItem = ReportName.PHARMACY_HESOANTOANBAN;
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void HeSoAnToanBanCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1349_G1_BCDSThuocLayThem;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HeSoAnToanBanCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HeSoAnToanBanCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BangBaoSoDTHangNgayDuaVaoSoAnToanCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportDocumentPreview>();
            UnitVM.eItem = ReportName.PHARMACY_DUTRUDUATRENHESOANTOAN;
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BangBaoSoDTHangNgayDuaVaoSoAnToanCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1479_G1_BCDuTruDuaTrenHSoAnToan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BangBaoSoDTHangNgayDuaVaoSoAnToanCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangBaoSoDTHangNgayDuaVaoSoAnToanCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DuTruTongHopTheoSoPhieuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.eItem = ReportName.PHARMACY_TONGHOPDUTRU_SOPHIEU;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.bXem = mDuTru_TongHopTheoSoPhieu_Xem;
            UnitVM.bIn = mDuTru_TongHopTheoSoPhieu_In;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DuTruTongHopTheoSoPhieuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1348_G1_DuTruTgHopSoPh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DuTruTongHopTheoSoPhieuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DuTruTongHopTheoSoPhieuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DuTruTongHopTheoThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_TONGHOPDUTRU_TENTHUOC;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.bIn = mDuTru_TongHopTheoThuoc_In;
            UnitVM.bXem = mDuTru_TongHopTheoThuoc_Xem;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DuTruTongHopTheoThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1347_G1_DuTruTgHopThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DuTruTongHopTheoThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DuTruTongHopTheoThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region outward member

        private void DemageExpiryDrugCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IPharmacyDamageExpiryDrug>();
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DemageExpiryDrugCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1623_G1_XuatHuyThuocHHDung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DemageExpiryDrugCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DemageExpiryDrugCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void OutwardInternalDamageCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IOutwardInternalDamage>();
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void OutwardInternalDamageCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1624_G1_XuatHuyThuocHu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutwardInternalDamageCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutwardInternalDamageCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void OutwardInternalCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IOutwardInternal>();
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void OutwardInternalCmd()
        {
            Globals.TitleForm = eHCMSResources.G2892_G1_XuatNBo;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutwardInternalCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutwardInternalCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Xuất cân bằng
        private void PharmacyBalanceOutCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IOutwardInternal>();
            UnitVM.ViewCase = 1;
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void PharmacyBalanceOutCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3079_G1_XuatCanBang;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PharmacyBalanceOutCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmacyBalanceOutCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void PharmacyBalanceInCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IPharmacyInwardBalance>();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void PharmacyBalanceInCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3082_G1_NhapCanBang;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PharmacyBalanceInCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmacyBalanceInCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        private void RequestPharmacyCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IRequestPharmacy>();
            UnitVM.strHienThi = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void RequestPharmacyCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1084_G1_LapPhYC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestPharmacyCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestPharmacyCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DanhSachXuatThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var VM = Globals.GetViewModel<IRptDanhSachXuatNhaThuoc>();
            VM.TieuDeRpt = Globals.TitleForm;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void DanhSachXuatThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.K3100_G1_DSXuatThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DanhSachXuatThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DanhSachXuatThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        /*▼====: #002*/
        private void BidDetailCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IBidDetail>();
            UnitVM.IsMedDept = false;
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BidDetailCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2112_G1_ChiTietThau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BidDetailCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BidDetailCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void Handle(ItemSelected<IMessageBox, AxMessageBoxResult> message)
        {
            if (message.Item == AxMessageBoxResult.Ok)
            {
                //load link moi o day la dc ne!
            }
        }

        //▼====== #003
        private void FromMedToPharmacyCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var VM = Globals.GetViewModel<IPharmacyInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.SearchCriteria.IsInputMedDept = true;
            VM.strHienThi = Globals.TitleForm;

            VM.mNhapHangTuKhoDuoc_Tim = true;
            VM.mNhapHangTuKhoDuoc_Them = true;
            VM.mNhapHangTuKhoDuoc_XemIn = true;
            VM.mNhapHangTuKhoDuoc_In = true;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void FromMedToPharmacyCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1816_G1_NhapThuocTuKhoDuocKhoPhg;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FromMedToPharmacyCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FromMedToPharmacyCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #003

        //▼====== #005
        private void BCThuTienNTTaiQuayBHYTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.BC_THUTIEN_NT_TAI_QUAY_BHYT;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.bXem = bThuTienTaiQuay;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BCThuTienNTTaiQuayBHYTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2594_G1_BCThuTienNTTaiQuayBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThuTienNTTaiQuayBHYTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThuTienNTTaiQuayBHYTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #005
        //▼====== #011
        private void BCThuTienNTTaiQuayBHYTTheoBienLaiCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.BC_THUTIEN_NT_TAI_QUAY_BHYT_THEO_BIEN_LAI;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;

            UnitVM.bXem = bThuTienTaiQuayTheoBienLai;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BCThuTienNTTaiQuayBHYTTheoBienLaiCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2964_G1_BCThuTienNTTaiQuayBHYTTheoBienLai;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThuTienNTTaiQuayBHYTTheoBienLaiCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThuTienNTTaiQuayBHYTTheoBienLaiCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #011
        //▼====== #006
        private void TKThongTinKhachHangNTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.TKTheoDoiTTChiTietKH_NT;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = true;
            UnitVM.bXem = mBCKhac_TraThuocTongHop_Xem;
            UnitVM.BXemChiTiet = false;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TKThongTinKhachHangNTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2658_G1_TKThongTinKhachHangNT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TKThongTinKhachHangNTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TKThongTinKhachHangNTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NXTThuocKhacNTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<ITheKho>();
            UnitVM.eItem = ReportName.TKTheoDoiNXTThuocKhac_NT;
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void NXTThuocKhacNTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2659_G1_NXTThuocKhacNT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NXTThuocKhacNTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NXTThuocKhacNTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #006

        //▼====: #007
        private void BCXuatDuocNoiBo_NTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.BCXuatDuocNoiBo_NT;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = true;
            UnitVM.bXem = bXuatDuocNoiBo;
            UnitVM.BXemChiTiet = false;
            UnitVM.ViewByDate();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BCXuatDuocNoiBo_NTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2721_G1_BCXuatDuocNoiBo_NT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCXuatDuocNoiBo_NTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCXuatDuocNoiBo_NTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #007
        //▼====: #008
        public void BCBanThuocLeCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2758_G1_BCBanThuocLe;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCBanThuocLeCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCBanThuocLeCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCBanThuocLeCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IPharmacyModule>();
            var reportVm = Globals.GetViewModel<IReportByDDMMYYYY>();
            reportVm.eItem = ReportName.NT_BCBanThuocLe;
            reportVm.pageTitle = Globals.TitleForm;
            reportVm.RptParameters.HideStore = true;
            reportVm.bXem = bBanThuocLe;
            reportVm.BXemChiTiet = false;
            reportVm.ViewByDate();
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #008
        //▼====: #009
        private void FromInternalExportToPharmacyCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var VM = Globals.GetViewModel<IInwardFromInternalExport>();
            VM.SearchCriteria.IsInputMedDept = true;
            VM.strHienThi = Globals.TitleForm;

            VM.mNhapHangTuKhoDuoc_Tim = true;
            VM.mNhapHangTuKhoDuoc_Them = true;
            VM.mNhapHangTuKhoDuoc_XemIn = true;
            VM.mNhapHangTuKhoDuoc_In = true;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void FromInternalExportToPharmacyCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2829_G1_NhapHangNoiBo;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FromInternalExportToPharmacyCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FromInternalExportToPharmacyCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #009
        //▼====: #010
        private void BCThuocSapHetHanSDCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IPharmacyModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BC_ThuocSapHetHanDung;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_EXTERNAL);
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
        //▲====: #010
        //▼====: #012
        private void BKXUatThuocTheoBNCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IPharmacyModule>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.Pharmacy_BKXuatThuocTheoBN;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_EXTERNAL);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BKXUatThuocTheoBNCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2997_G1_BKXuatThuocTheoBN;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BKXUatThuocTheoBNCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BKXUatThuocTheoBNCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #012
        //▼====: #013
        private void TKThongTinKhachHangNTTheoThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<ITheKho>();
            UnitVM.eItem = ReportName.TKTheoDoiTTChiTietKH_NTTheoThuoc;
            UnitVM.TitleForm = Globals.TitleForm;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TKThongTinKhachHangNTTheoThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3072_G1_TKThongTinKhachHangNTTheoThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TKThongTinKhachHangNTTheoThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TKThongTinKhachHangNTTheoThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #013
        private void BCHuyHoanCmd_In()
        {
            var module = Globals.GetViewModel<IPharmacyModule>();
            //var RefGenDrugVM = Globals.GetViewModel<INhapXuatTon>();
            //Globals.PageName = Globals.TitleForm;
            //RefGenDrugVM.TitleForm = Globals.TitleForm;

            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();

            UnitVM.eItem = ReportName.PHARMACY_BCHuyHoan;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.BXemChiTiet = false;
            UnitVM.bXem = true;
            UnitVM.ViewByDate();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BCHuyHoanCmd()
        {
            Globals.TitleForm = "Báo cáo hủy/ hoàn tiền";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCHuyHoanCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCHuyHoanCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //▼=====: #014
        private bool _mLapPhieuLinh = true;
        private bool _mTangGiamKK = true;
        private bool _bEstimationBHYTCmd = true;
        private bool _bInwardDrugFromOtherCmd = true;
        private bool _bInwardDrugFromDrugDeptCmd = true;
        private bool _bInwardDrugFromInternalCmd = true;

        private bool _bTheoDoiThuocGioiHanSL = true;
        private bool _bThuTienTaiQuay = true;
        private bool _bThuTienTaiQuayTheoBienLai = true;
        private bool _bXuatDuocNoiBo = true;
        private bool _bBanThuocLe = true;
        private bool _bThuocSapHetHan = true;
        private bool _bBangKeXuatThuocTheoBN = true;
        private bool _bHuyHoanTien = true;

        private bool _mChiTietThau = true;
        public bool mLapPhieuLinh
        {
            get
            {
                return _mLapPhieuLinh && IsNoteHMSSystem;
            }
            set
            {
                if (_mLapPhieuLinh == value)
                    return;
                _mLapPhieuLinh = value;
                NotifyOfPropertyChange(() => mLapPhieuLinh);
            }
        }

        public bool bEstimationBHYTCmd
        {
            get
            {
                return _bEstimationBHYTCmd;
            }
            set
            {
                if (_bEstimationBHYTCmd == value)
                    return;
                _bEstimationBHYTCmd = value;
                NotifyOfPropertyChange(() => bEstimationBHYTCmd);
            }
        }

        public bool bInwardDrugFromOtherCmd
        {
            get
            {
                return _bInwardDrugFromOtherCmd;
            }
            set
            {
                if (_bInwardDrugFromOtherCmd == value)
                    return;
                _bInwardDrugFromOtherCmd = value;
                NotifyOfPropertyChange(() => bInwardDrugFromOtherCmd);
            }
        }

        public bool bInwardDrugFromDrugDeptCmd
        {
            get
            {
                return _bInwardDrugFromDrugDeptCmd;
            }
            set
            {
                if (_bInwardDrugFromDrugDeptCmd == value)
                    return;
                _bInwardDrugFromDrugDeptCmd = value;
                NotifyOfPropertyChange(() => bInwardDrugFromDrugDeptCmd);
            }
        }

        public bool bInwardDrugFromInternalCmd
        {
            get
            {
                return _bInwardDrugFromInternalCmd;
            }
            set
            {
                if (_bInwardDrugFromInternalCmd == value)
                    return;
                _bInwardDrugFromInternalCmd = value;
                NotifyOfPropertyChange(() => bInwardDrugFromInternalCmd);
            }
        }

        public bool mTangGiamKK
        {
            get
            {
                return _mTangGiamKK;
            }
            set
            {
                if (_mTangGiamKK == value)
                    return;
                _mTangGiamKK = value;
                NotifyOfPropertyChange(() => mTangGiamKK);
            }
        }

        public bool bTheoDoiThuocGioiHanSL
        {
            get
            {
                return _bTheoDoiThuocGioiHanSL;
            }
            set
            {
                if (_bTheoDoiThuocGioiHanSL == value)
                    return;
                _bTheoDoiThuocGioiHanSL = value;
                NotifyOfPropertyChange(() => bTheoDoiThuocGioiHanSL);
            }
        }

        public bool bThuTienTaiQuay
        {
            get
            {
                return _bThuTienTaiQuay;
            }
            set
            {
                if (_bThuTienTaiQuay == value)
                    return;
                _bThuTienTaiQuay = value;
                NotifyOfPropertyChange(() => bThuTienTaiQuay);
            }
        }

        public bool bThuTienTaiQuayTheoBienLai
        {
            get
            {
                return _bThuTienTaiQuayTheoBienLai;
            }
            set
            {
                if (_bThuTienTaiQuayTheoBienLai == value)
                    return;
                _bThuTienTaiQuayTheoBienLai = value;
                NotifyOfPropertyChange(() => bThuTienTaiQuayTheoBienLai);
            }
        }

        public bool bXuatDuocNoiBo
        {
            get
            {
                return _bXuatDuocNoiBo;
            }
            set
            {
                if (_bXuatDuocNoiBo == value)
                    return;
                _bXuatDuocNoiBo = value;
                NotifyOfPropertyChange(() => bXuatDuocNoiBo);
            }
        }

        public bool bBanThuocLe
        {
            get
            {
                return _bBanThuocLe;
            }
            set
            {
                if (_bBanThuocLe == value)
                    return;
                _bBanThuocLe = value;
                NotifyOfPropertyChange(() => bBanThuocLe);
            }
        }

        public bool bThuocSapHetHan
        {
            get
            {
                return _bThuocSapHetHan;
            }
            set
            {
                if (_bThuocSapHetHan == value)
                    return;
                _bThuocSapHetHan = value;
                NotifyOfPropertyChange(() => bThuocSapHetHan);
            }
        }

        public bool bBangKeXuatThuocTheoBN
        {
            get
            {
                return _bBangKeXuatThuocTheoBN;
            }
            set
            {
                if (_bBangKeXuatThuocTheoBN == value)
                    return;
                _bBangKeXuatThuocTheoBN = value;
                NotifyOfPropertyChange(() => bBangKeXuatThuocTheoBN);
            }
        }

        public bool bHuyHoanTien
        {
            get
            {
                return _bHuyHoanTien;
            }
            set
            {
                if (_bHuyHoanTien == value)
                    return;
                _bHuyHoanTien = value;
                NotifyOfPropertyChange(() => bHuyHoanTien);
            }
        }

        public bool mChiTietThau
        {
            get
            {
                return _mChiTietThau;
            }
            set
            {
                if (_mChiTietThau == value)
                    return;
                _mChiTietThau = value;
                NotifyOfPropertyChange(() => mChiTietThau);
            }
        }
        //▲=====: #014
        //▼=====: #015
        private bool _gElectronicPrescription = true;
        public bool gElectronicPrescription
        {
            get
            {
                return _gElectronicPrescription;
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
                return _mElectronicPrescription;
            }
            set
            {
                if (_mElectronicPrescription == value)
                    return;
                _mElectronicPrescription = value;
                NotifyOfPropertyChange(() => mElectronicPrescription);
            }
        }
        private bool _mBCHuyDQGReport = true;
        public bool mBCHuyDQGReport
        {
            get
            {
                return _mBCHuyDQGReport;
            }
            set
            {
                if (_mBCHuyDQGReport == value)
                    return;
                _mBCHuyDQGReport = value;
                NotifyOfPropertyChange(() => mBCHuyDQGReport);
            }
        }
        private void ElectronicPrescriptionCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IElectronicPrescriptionPharmacy>();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ElectronicPrescriptionCmd()
        {
            Globals.TitleForm = "Quản lý đơn thuốc điện tử".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ElectronicPrescriptionCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ElectronicPrescriptionCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BCHuyDQGReport_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.BCHuyDQGReport;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.bXem = mBCHuyDQGReport;
            UnitVM.BXemChiTiet = false;
            UnitVM.bXuatExcel = true;
            UnitVM.ViewByDate();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void BCHuyDQGReport_Cmd()
        {
            Globals.TitleForm = "BÁO CÁO HỦY ĐẨY CỔNG ĐƠN THUỐC NHÀ THUỐC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCHuyDQGReport_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCHuyDQGReport_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲=====: #015
    }
}
#endregion