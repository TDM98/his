/*
 * 20170803 #001 CMN: Add HI Store Service
 * 20170810 #002 CMN: Added Bid Service
*/
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Common;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using eHCMSLanguage;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacyLeftMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyLeftMenuViewModel : Conductor<object>, IPharmacyLeftMenu
       , IHandle<ItemSelected<IMessageBox, AxMessageBoxResult>>
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }
        /*▼====: #002*/
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
        /*▲====: #002*/
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PharmacyLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
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
            mNhapHang = bInwardDrugFromSupplierCmd || bInwardDrugFromSupplierCmd;
            mYeuCau = bRequestPharmacyCmd;
            mXuatHang = bDemageExpiryDrugCmd || bOutDrugCmd || bDemageDrugCmd;
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
            mBaoCaoKhac = bNhapXuatTonCmd || bThuocHetHanDungCmd || bDSXuatThuocNoiBoCmd || bDSXuatThuocNoiBoCmd
                || bDSThuocXuatChoBHCmd || bBaoCaoBanThuocTHCmd || bTheoDoiSoLuongThuocCmd || bTheoDoiMuonThuocCmd
                || bTraThuocTHCmd;
            mBaoCaoDuTru = mDuTru_ThuocCanLayThemDeBan || bBangBaoSoDTHangNgayDuaVaoSoAnToanCmd
                || mDuTru_TongHopTheoSoPhieu || mDuTru_TongHopTheoThuoc;
            mBaoCao = mBaoCaoHangNgay || mBaoCaoThang || mBaoCaoNhap || mBaoCaoXuat || mBaoCaoBaoHiem
                || mBaoCaoBangGiaBanHangThang || mBaoCaoKhac || mBaoCaoDuTru;

            mThongKe = bTonKhoCmd || bSellOnDateCmd;



            #endregion

            mBid = bRefGenDrugCmd;
            mBidDetail = bRefGenDrugCmd;
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
                return _bPharmacySellingItemPrices
                ;
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
                return _bCollectionDrugCmd;
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
                return _bReturnDrugCmd;
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
                return _mBanHang
                ;
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
                return _mDatHang
                ;
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
                return _mNhapHang
                ;
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
                return _mXuatHang
                ;
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
                return _mQuanLy
                ;
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
                return _mBaoCao
                ;
            }
            set
            {
                if (_mBaoCao
                 == value)
                    return;
                _mBaoCao
                 = value;
                NotifyOfPropertyChange(() => mBaoCao
                );
            }
        }


        public bool mBaoCaoHangNgay
        {
            get
            {
                return _mBaoCaoHangNgay
                ;
            }
            set
            {
                if (_mBaoCaoHangNgay
                 == value)
                    return;
                _mBaoCaoHangNgay
                 = value;
                NotifyOfPropertyChange(() => mBaoCaoHangNgay
                );
            }
        }


        public bool mBaoCaoThang
        {
            get
            {
                return _mBaoCaoThang
                ;
            }
            set
            {
                if (_mBaoCaoThang
                 == value)
                    return;
                _mBaoCaoThang
                 = value;
                NotifyOfPropertyChange(() => mBaoCaoThang
                );
            }
        }


        public bool mBaoCaoNhap
        {
            get
            {
                return _mBaoCaoNhap
                ;
            }
            set
            {
                if (_mBaoCaoNhap
                 == value)
                    return;
                _mBaoCaoNhap
                 = value;
                NotifyOfPropertyChange(() => mBaoCaoNhap
                );
            }
        }


        public bool mBaoCaoXuat
        {
            get
            {
                return _mBaoCaoXuat
                ;
            }
            set
            {
                if (_mBaoCaoXuat
                 == value)
                    return;
                _mBaoCaoXuat
                 = value;
                NotifyOfPropertyChange(() => mBaoCaoXuat
                );
            }
        }


        public bool mBaoCaoBaoHiem
        {
            get
            {
                return _mBaoCaoBaoHiem
                ;
            }
            set
            {
                if (_mBaoCaoBaoHiem
                 == value)
                    return;
                _mBaoCaoBaoHiem
                 = value;
                NotifyOfPropertyChange(() => mBaoCaoBaoHiem
                );
            }
        }


        public bool mBaoCaoBangGiaBanHangThang
        {
            get
            {
                return _mBaoCaoBangGiaBanHangThang
                ;
            }
            set
            {
                if (_mBaoCaoBangGiaBanHangThang
                 == value)
                    return;
                _mBaoCaoBangGiaBanHangThang
                 = value;
                NotifyOfPropertyChange(() => mBaoCaoBangGiaBanHangThang
                );
            }
        }


        public bool mBaoCaoKhac
        {
            get
            {
                return _mBaoCaoKhac
                ;
            }
            set
            {
                if (_mBaoCaoKhac
                 == value)
                    return;
                _mBaoCaoKhac
                 = value;
                NotifyOfPropertyChange(() => mBaoCaoKhac
                );
            }
        }


        public bool mBaoCaoDuTru
        {
            get
            {
                return _mBaoCaoDuTru
                ;
            }
            set
            {
                if (_mBaoCaoDuTru
                 == value)
                    return;
                _mBaoCaoDuTru
                 = value;
                NotifyOfPropertyChange(() => mBaoCaoDuTru
                );
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
            resetMenuColor();
            Unit.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            PharmacieucalCompany.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            Supplier.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            Storage.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            Formula.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
                resetMenuColor();
                DrugClass.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            RefGenDrugBHYT.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            RefGenDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            var module = Globals.GetViewModel<IPharmacyModule>();
            object RefGenDrugVM = null;

            //31072018 TTM
            //Bật CH cho quản lý danh mục mới 
            if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
            {
                RefGenDrugVM = Globals.GetViewModel<IRefGenDrugListNew>();
                ((IRefGenDrugListNew)RefGenDrugVM).TitleForm = Globals.TitleForm;
            }
            else
            {
                RefGenDrugVM = Globals.GetViewModel<IRefGenDrugList>();
                ((IRefGenDrugList)RefGenDrugVM).TitleForm = Globals.TitleForm;
            }
            
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
                resetMenuColor();
                GenDrugContraIndicate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

                var module = Globals.GetViewModel<IPharmacyModule>();
                var ContraindicatorDrug = Globals.GetViewModel<IContraindicatorDrug>();
                Globals.PageName = Globals.TitleForm;
                ContraindicatorDrug.TitleForm = Globals.TitleForm;

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
                resetMenuColor();
                SupplierAndDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
                resetMenuColor();
                PharmaciaucalSupplier.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
                resetMenuColor();
                SupplierGenericDrugPrice.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
                resetMenuColor();
                PharmacySellingItemPrices.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
                resetMenuColor();
                PharmacySellingPriceList.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
        private void SellVisitorDrugCmd_In()
        {
            resetMenuColor();
            SellVisitorDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            IsLoading = true;
            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IVisitor>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

            module.MainContent = RefGenDrugVM;
            (module as Conductor<object>).ActivateItem(RefGenDrugVM);
            IsLoading = false;
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

        private void SellPrescriptionDrugCmd_In()
        {
            resetMenuColor();
            SellPrescriptionDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
                    if (GlobalsNAV.msgb==null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok )
                    {
                        SellPrescriptionDrugCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });

            }
        }

        //==== #001
        public bool EnableHIStore
        {
            get { return Globals.ServerConfigSection.CommonItems.EnableHIStore; }
        }
        private void SellPrescriptionDrugCmdHI_In()
        {
            resetMenuColor();
            SellPrescriptionDrugHI.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            CollectionDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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

        #endregion

        #region return member

        private void ReturnDrugCmd_In()
        {
            resetMenuColor();
            ReturnDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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

        #endregion

        #region Estimation member

        private void EstimationCmd_In()
        {
            resetMenuColor();
            Estimation.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            var module = Globals.GetViewModel<IPharmacyModule>();
            var RefGenDrugVM = Globals.GetViewModel<IEstimationPharmacy>();
            Globals.PageName = Globals.TitleForm;
            RefGenDrugVM.TitleForm = Globals.TitleForm;

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

        #endregion

        #region Order member

        private void OrderCmd_In()
        {
            resetMenuColor();
            Order.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            InwardDrugFromSupplier.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            InwardDrugOther.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            TonKho.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            SellOnDate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            NhapXuatTon.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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

        private void TheKhoCmd_In()
        {
            resetMenuColor();
            TheKho.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            ThuocHetHanDung.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            DSXuatThuocNoiBo.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            DSXuatThuocNoiBoTheoTenThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            DSThuocXuatChoBH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            TheoDoiThuocCoGioiHanSL.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            BaoCaoBanThuocTH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IReportByDDMMYYYY>();
            UnitVM.eItem = ReportName.PHARMACY_BANTHUOCTH;
            UnitVM.pageTitle = Globals.TitleForm;
            UnitVM.RptParameters.HideStore = false;
            UnitVM.bIn = mBCKhac_BanThuocTongHop_In;
            UnitVM.bXem = mBCKhac_BanThuocTongHop_Xem;

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
            resetMenuColor();
            TheoDoiSoLuongThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            TheoDoiMuonThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
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
            resetMenuColor();
            TraThuocTH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            BCNopTienHangNgay.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            DoanhThuBan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            KiemKeHangThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            ReportKiemKeHangThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            BangKeNhapHangThangTheoSoPhieuNhapKho.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            BangKeNhapThuocHangThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            SoKiemNhapThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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

            resetMenuColor();
            BangKeChungTuThanhToan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            TheoDoiCongNo.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            Mau20.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            DMThuocThanhToanBHYTThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
           
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
            resetMenuColor();
            BangGiaBanDuKienThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            BangGiaBanThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            HeSoAnToanBan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            BangBaoSoDTHangNgayDuaVaoSoAnToan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            DuTruTongHopTheoSoPhieu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            DuTruTongHopTheoThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            DemageExpiryDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            OutwardInternalDamage.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            OutwardInternal.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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

        private void RequestPharmacyCmd_In()
        {
            resetMenuColor();
            RequestPharmacy.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            DanhSachXuatThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
        #region Bid Detail
        private void BidDetailCmd_In()
        {
            resetMenuColor();
            BidDetail.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IPharmacyModule>();
            var UnitVM = Globals.GetViewModel<IBidDetail>();
            UnitVM.IsMedDept = false;
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
        #endregion
        /*▲====: #002*/
        #endregion

        #region load
        public Button SellVisitorDrug { get; set; }
        public Button SellPrescriptionDrug { get; set; }
        //==== #001
        public Button SellPrescriptionDrugHI { get; set; }
        //==== #001
        public Button CollectionDrug { get; set; }
        public Button ReturnDrug { get; set; }
        public Button Estimation { get; set; }
        public Button Order { get; set; }
        public Button InwardDrugFromSupplier { get; set; }
        public Button InwardDrugOther { get; set; }
        public Button RequestPharmacy { get; set; }
        public Button DemageExpiryDrug { get; set; }
        public Button OutwardInternalDamage { get; set; }
        public Button OutwardInternal { get; set; }
        public Button Unit { get; set; }
        public Button PharmacieucalCompany { get; set; }
        public Button Supplier { get; set; }
        public Button Storage { get; set; }
        public Button Formula { get; set; }
        public Button DrugClass { get; set; }
        public Button RefGenDrugBHYT { get; set; }
        public Button RefGenDrug { get; set; }
        public Button GenDrugContraIndicate { get; set; }
        public Button SupplierAndDrug { get; set; }
        public Button PharmaciaucalSupplier { get; set; }
        public Button SupplierGenericDrugPrice { get; set; }
        public Button PharmacySellingItemPrices { get; set; }
        public Button PharmacySellingPriceList { get; set; }
        public Button BCNopTienHangNgay { get; set; }
        public Button DoanhThuBan { get; set; }
        public Button KiemKeHangThang { get; set; }
        public Button ReportKiemKeHangThang { get; set; }
        public Button BangKeNhapHangThangTheoSoPhieuNhapKho { get; set; }
        public Button BangKeNhapThuocHangThang { get; set; }
        public Button SoKiemNhapThuoc { get; set; }
        public Button BangKeChungTuThanhToan { get; set; }
        public Button TheoDoiCongNo { get; set; }
        public Button TheKho { get; set; }
        public Button DanhSachXuatThuoc { get; set; }
        public Button Mau20 { get; set; }
        public Button DMThuocThanhToanBHYTThang { get; set; }
        public Button BangGiaBanDuKienThang { get; set; }
        public Button BangGiaBanThang { get; set; }
        public Button NhapXuatTon { get; set; }
        public Button ThuocHetHanDung { get; set; }
        public Button DSXuatThuocNoiBo { get; set; }
        public Button DSXuatThuocNoiBoTheoTenThuoc { get; set; }
        public Button DSThuocXuatChoBH { get; set; }
        public Button TheoDoiThuocCoGioiHanSL { get; set; }
        public Button BaoCaoBanThuocTH { get; set; }
        public Button TheoDoiSoLuongThuoc { get; set; }
        public Button TheoDoiMuonThuoc { get; set; }
        public Button TraThuocTH { get; set; }
        public Button HeSoAnToanBan { get; set; }
        public Button BangBaoSoDTHangNgayDuaVaoSoAnToan { get; set; }
        public Button DuTruTongHopTheoSoPhieu { get; set; }
        public Button DuTruTongHopTheoThuoc { get; set; }
        public Button TonKho { get; set; }
        public Button SellOnDate { get; set; }
        public Button BidDetail { get; set; }
        /*▼====: #002*/
        public void BidDetailCmd_Loaded(object sender)
        {
            BidDetail = sender as Button;
            BidDetail.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        /*▲====: #002*/
        public void SellVisitorDrugCmd_Loaded(object sender)
        {
            SellVisitorDrug = sender as Button;
            SellVisitorDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void SellPrescriptionDrugCmd_Loaded(object sender)
        {
            SellPrescriptionDrug = sender as Button;
            SellPrescriptionDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //==== #001
        public void SellPrescriptionDrugCmdHI_Loaded(object sender)
        {
            SellPrescriptionDrugHI = sender as Button;
            SellPrescriptionDrugHI.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //==== #001
        public void CollectionDrugCmd_Loaded(object sender)
        {
            CollectionDrug = sender as Button;
            CollectionDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ReturnDrugCmd_Loaded(object sender)
        {
            ReturnDrug = sender as Button;
            ReturnDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void EstimationCmd_Loaded(object sender)
        {
            Estimation = sender as Button;
            Estimation.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void OrderCmd_Loaded(object sender)
        {
            Order = sender as Button;
            Order.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void InwardDrugFromSupplierCmd_Loaded(object sender)
        {
            InwardDrugFromSupplier = sender as Button;
            InwardDrugFromSupplier.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void InwardDrugOtherCmd_Loaded(object sender)
        {
            InwardDrugOther = sender as Button;
            InwardDrugOther.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void RequestPharmacyCmd_Loaded(object sender)
        {
            RequestPharmacy = sender as Button;
            RequestPharmacy.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DemageExpiryDrugCmd_Loaded(object sender)
        {
            DemageExpiryDrug = sender as Button;
            DemageExpiryDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void OutwardInternalDamageCmd_Loaded(object sender)
        {
            OutwardInternalDamage = sender as Button;
            OutwardInternalDamage.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void OutwardInternalCmd_Loaded(object sender)
        {
            OutwardInternal = sender as Button;
            OutwardInternal.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void UnitCmd_Loaded(object sender)
        {
            Unit = sender as Button;
            Unit.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void PharmacieucalCompanyCmd_Loaded(object sender)
        {
            PharmacieucalCompany = sender as Button;
            PharmacieucalCompany.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void SupplierCmd_Loaded(object sender)
        {
            Supplier = sender as Button;
            Supplier.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void StorageCmd_Loaded(object sender)
        {
            Storage = sender as Button;
            Storage.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void FormulaCmd_Loaded(object sender)
        {
            Formula = sender as Button;
            Formula.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DrugClassCmd_Loaded(object sender)
        {
            DrugClass = sender as Button;
            DrugClass.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void RefGenDrugBHYTCmd_Loaded(object sender)
        {
            RefGenDrugBHYT = sender as Button;
            RefGenDrugBHYT.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void RefGenDrugCmd_Loaded(object sender)
        {
            RefGenDrug = sender as Button;
            RefGenDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void GenDrugContraIndicateCmd_Loaded(object sender)
        {
            GenDrugContraIndicate = sender as Button;
            GenDrugContraIndicate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void SupplierAndDrugCmd_Loaded(object sender)
        {
            SupplierAndDrug = sender as Button;
            SupplierAndDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void PharmaciaucalSupplierCmd_Loaded(object sender)
        {
            PharmaciaucalSupplier = sender as Button;
            PharmaciaucalSupplier.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void SupplierGenericDrugPrice_Mgnt_Loaded(object sender)
        {
            SupplierGenericDrugPrice = sender as Button;
            SupplierGenericDrugPrice.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void PharmacySellingItemPrices_Mgnt_Loaded(object sender)
        {
            PharmacySellingItemPrices = sender as Button;
            PharmacySellingItemPrices.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void PharmacySellingPriceList_Mgnt_Loaded(object sender)
        {
            PharmacySellingPriceList = sender as Button;
            PharmacySellingPriceList.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void BCNopTienHangNgayCmd_Loaded(object sender)
        {
            BCNopTienHangNgay = sender as Button;
            BCNopTienHangNgay.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DoanhThuBanCmd_Loaded(object sender)
        {
            DoanhThuBan = sender as Button;
            DoanhThuBan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void KiemKeHangThangCmd_Loaded(object sender)
        {
            KiemKeHangThang = sender as Button;
            KiemKeHangThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ReportKiemKeHangThangCmd_Loaded(object sender)
        {
            ReportKiemKeHangThang = sender as Button;
            ReportKiemKeHangThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void BangKeNhapHangThangTheoSoPhieuNhapKhoCmd_Loaded(object sender)
        {
            BangKeNhapHangThangTheoSoPhieuNhapKho = sender as Button;
            BangKeNhapHangThangTheoSoPhieuNhapKho.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void BangKeNhapThuocHangThangCmd_Loaded(object sender)
        {
            BangKeNhapThuocHangThang = sender as Button;
            BangKeNhapThuocHangThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void SoKiemNhapThuocCmd_Loaded(object sender)
        {
            SoKiemNhapThuoc = sender as Button;
            SoKiemNhapThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void BangKeChungTuThanhToanCmd_Loaded(object sender)
        {
            BangKeChungTuThanhToan = sender as Button;
            BangKeChungTuThanhToan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TheoDoiCongNoCmd_Loaded(object sender)
        {
            TheoDoiCongNo = sender as Button;
            TheoDoiCongNo.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TheKhoCmd_Loaded(object sender)
        {
            TheKho = sender as Button;
            TheKho.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DanhSachXuatThuocCmd_Loaded(object sender)
        {
            DanhSachXuatThuoc = sender as Button;
            DanhSachXuatThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Mau20Cmd_Loaded(object sender)
        {
            Mau20 = sender as Button;
            Mau20.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DMThuocThanhToanBHYTThangCmd_Loaded(object sender)
        {
            DMThuocThanhToanBHYTThang = sender as Button;
            DMThuocThanhToanBHYTThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void BangGiaBanDuKienThangCmd_Loaded(object sender)
        {
            BangGiaBanDuKienThang = sender as Button;
            BangGiaBanDuKienThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void BangGiaBanThangCmd_Loaded(object sender)
        {
            BangGiaBanThang = sender as Button;
            BangGiaBanThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void NhapXuatTonCmd_Loaded(object sender)
        {
            NhapXuatTon = sender as Button;
            NhapXuatTon.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ThuocHetHanDungCmd_Loaded(object sender)
        {
            ThuocHetHanDung = sender as Button;
            ThuocHetHanDung.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DSXuatThuocNoiBoCmd_Loaded(object sender)
        {
            DSXuatThuocNoiBo = sender as Button;
            DSXuatThuocNoiBo.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DSXuatThuocNoiBoTheoTenThuocCmd_Loaded(object sender)
        {
            DSXuatThuocNoiBoTheoTenThuoc = sender as Button;
            DSXuatThuocNoiBoTheoTenThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DSThuocXuatChoBHCmd_Loaded(object sender)
        {
            DSThuocXuatChoBH = sender as Button;
            DSThuocXuatChoBH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TheoDoiThuocCoGioiHanSLCmd_Loaded(object sender)
        {
            TheoDoiThuocCoGioiHanSL = sender as Button;
            TheoDoiThuocCoGioiHanSL.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void BaoCaoBanThuocTHCmd_Loaded(object sender)
        {
            BaoCaoBanThuocTH = sender as Button;
            BaoCaoBanThuocTH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TheoDoiSoLuongThuocCmd_Loaded(object sender)
        {
            TheoDoiSoLuongThuoc = sender as Button;
            TheoDoiSoLuongThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TheoDoiMuonThuocCmd_Loaded(object sender)
        {
            TheoDoiMuonThuoc = sender as Button;
            TheoDoiMuonThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TraThuocTHCmd_Loaded(object sender)
        {
            TraThuocTH = sender as Button;
            TraThuocTH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void HeSoAnToanBanCmd_Loaded(object sender)
        {
            HeSoAnToanBan = sender as Button;
            HeSoAnToanBan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void BangBaoSoDTHangNgayDuaVaoSoAnToanCmd_Loaded(object sender)
        {
            BangBaoSoDTHangNgayDuaVaoSoAnToan = sender as Button;
            BangBaoSoDTHangNgayDuaVaoSoAnToan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DuTruTongHopTheoSoPhieuCmd_Loaded(object sender)
        {
            DuTruTongHopTheoSoPhieu = sender as Button;
            DuTruTongHopTheoSoPhieu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void DuTruTongHopTheoThuocCmd_Loaded(object sender)
        {
            DuTruTongHopTheoThuoc = sender as Button;
            DuTruTongHopTheoThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TonKhoCmd_Loaded(object sender)
        {
            TonKho = sender as Button;
            TonKho.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void SellOnDateCmd_Loaded(object sender)
        {
            SellOnDate = sender as Button;
            SellOnDate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        #endregion

        public void resetMenuColor()
        {
            if (SellVisitorDrug != null)
                SellVisitorDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (SellPrescriptionDrug != null)
                SellPrescriptionDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (CollectionDrug != null)
                CollectionDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (ReturnDrug != null)
                ReturnDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (Estimation != null)
                Estimation.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (Order != null)
                Order.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (InwardDrugFromSupplier != null)
                InwardDrugFromSupplier.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (InwardDrugOther != null)
                InwardDrugOther.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (RequestPharmacy != null)
                RequestPharmacy.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DemageExpiryDrug != null)
                DemageExpiryDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (OutwardInternalDamage != null)
                OutwardInternalDamage.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (OutwardInternal != null)
                OutwardInternal.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (Unit != null)
                Unit.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (PharmacieucalCompany != null)
                PharmacieucalCompany.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (Supplier != null)
                Supplier.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (Storage != null)
                Storage.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (Formula != null)
                Formula.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DrugClass != null)
                DrugClass.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (RefGenDrugBHYT != null)
                RefGenDrugBHYT.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (RefGenDrug != null)
                RefGenDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];

            if (GenDrugContraIndicate != null)
                GenDrugContraIndicate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (SupplierAndDrug != null)
                SupplierAndDrug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (PharmaciaucalSupplier != null)
                PharmaciaucalSupplier.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (SupplierGenericDrugPrice != null)
                SupplierGenericDrugPrice.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (PharmacySellingItemPrices != null)
                PharmacySellingItemPrices.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (PharmacySellingPriceList != null)
                PharmacySellingPriceList.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (BCNopTienHangNgay != null)
                BCNopTienHangNgay.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DoanhThuBan != null)
                DoanhThuBan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (KiemKeHangThang != null)
                KiemKeHangThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (ReportKiemKeHangThang != null)
                ReportKiemKeHangThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (BangKeNhapHangThangTheoSoPhieuNhapKho != null)
                BangKeNhapHangThangTheoSoPhieuNhapKho.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (BangKeNhapThuocHangThang != null)
                BangKeNhapThuocHangThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (SoKiemNhapThuoc != null)
                SoKiemNhapThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (BangKeChungTuThanhToan != null)
                BangKeChungTuThanhToan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (TheoDoiCongNo != null)
                TheoDoiCongNo.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (TheKho != null)
                TheKho.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DanhSachXuatThuoc != null)
                DanhSachXuatThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (Mau20 != null)
                Mau20.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DMThuocThanhToanBHYTThang != null)
                DMThuocThanhToanBHYTThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (BangGiaBanDuKienThang != null)
                BangGiaBanDuKienThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (BangGiaBanThang != null)
                BangGiaBanThang.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (NhapXuatTon != null)
                NhapXuatTon.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (ThuocHetHanDung != null)
                ThuocHetHanDung.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DSXuatThuocNoiBo != null)
                DSXuatThuocNoiBo.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DSXuatThuocNoiBoTheoTenThuoc != null)
                DSXuatThuocNoiBoTheoTenThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DSThuocXuatChoBH != null)
                DSThuocXuatChoBH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (TheoDoiThuocCoGioiHanSL != null)
                TheoDoiThuocCoGioiHanSL.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (BaoCaoBanThuocTH != null)
                BaoCaoBanThuocTH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (TheoDoiSoLuongThuoc != null)
                TheoDoiSoLuongThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (TheoDoiMuonThuoc != null)
                TheoDoiMuonThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (TraThuocTH != null)
                TraThuocTH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (HeSoAnToanBan != null)
                HeSoAnToanBan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (BangBaoSoDTHangNgayDuaVaoSoAnToan != null)
                BangBaoSoDTHangNgayDuaVaoSoAnToan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DuTruTongHopTheoSoPhieu != null)
                DuTruTongHopTheoSoPhieu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (DuTruTongHopTheoThuoc != null)
                DuTruTongHopTheoThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (TonKho != null)
                TonKho.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            if (SellOnDate != null)
                SellOnDate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            //==== #001
            if (SellPrescriptionDrugHI != null)
                SellPrescriptionDrugHI.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            //==== #001
            /*▼====: #002*/
            if (BidDetail != null)
                BidDetail.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            /*▲====: #002*/
        }

        public void Handle(ItemSelected<IMessageBox, AxMessageBoxResult> message)
        {
            if (message.Item == AxMessageBoxResult.Ok)
            {
                //load link moi o day la dc ne!
            }
        }
    }
}
