using eHCMSLanguage;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Windows.Controls;
using Castle.Windsor;
/*
* 20170410 #001 CMN: Add Popup follow the result of patient drug using
* 20190411 #002 TTM: BM 0006636: Cho phép khoa nội trú tìm thuốc bằng tên hoạt chất.
* 20191004 #003 TNHX:  BM 	0017414 : Add report Z2847_G1_BCThuocSapHetHanDung
* 20210109 #004 TNHX:  BM: Hoàn thiện chức năng quản lý suất ăn
* 20210824 #005 QTD Thêm loại dinh dưỡng
* 20210916 #006 QTD Thêm quyền mở khoa kho
* 20211117 #007 QTD Thêm phân quyền mới
* 20211218 #008 QTD Tạo mẫu lĩnh VTTH
* 20220517 #009 DatTB: Báo cáo máu sắp hết hạn dùng
* 20230106 #010 BLQ: Thêm menu Lập phiếu lĩnh ngoại trú
* 20230208 #011 QTD: Thêm báo cáo xuất sử dụng thuốc cản quang
*/
namespace aEMR.StoreDept.Menu.ViewModels
{
    [Export(typeof(IStoreDeptTopMenu))]
    public class StoreDeptTopMenuViewModel : Conductor<object>, IStoreDeptTopMenu
    {
        [ImportingConstructor]
        public StoreDeptTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
          
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

            #region function
            mTaoMauXuat_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mTaoMauXuat_Thuoc);
            mTaoMauXuat_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mTaoMauXuat_YCu);
            mTaoMauXuat_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mTaoMauXuat_HoaChat);

            mPhieuYeuCau_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mPhieuYeuCau_Thuoc);
            mPhieuYeuCau_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mPhieuYeuCau_YCu);
            mPhieuYeuCau_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mPhieuYeuCau_HoaChat);

            mNhapHangTuNCC_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuNCC_Thuoc);
            mNhapHangTuNCC_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuNCC_YCu);
            mNhapHangTuNCC_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuNCC_HoaChat);

            mNhapHangTuKhoDuoc_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_Thuoc);
            mNhapHangTuKhoDuoc_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_YCu);
            mNhapHangTuKhoDuoc_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_HoaChat);

            mKiemKe_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_Thuoc);
            mKiemKe_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_YCu);
            mKiemKe_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_HoaChat);

            mThongKe_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mThongKe_Thuoc);
            mThongKe_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mThongKe_YCu);
            mThongKe_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mThongKe_HoaChat);

            mBCXuat_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuat_Thuoc);
            mBCXuat_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuat_YCu);
            mBCXuat_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuat_HoaChat);

            mBCNhapXuatTon_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_Thuoc);
            mBCNhapXuatTon_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_YCu);
            mBCNhapXuatTon_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_HoaChat);

            mBCTheKho_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_Thuoc);
            mBCTheKho_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_YCu);
            mBCTheKho_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_HoaChat);

            mDanhMuc_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mDanhMuc_Thuoc);
            mDanhMuc_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mDanhMuc_YCu);
            mDanhMuc_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mDanhMuc_HoaChat);

            mGiaBan_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mGiaBan_Thuoc);
            mGiaBan_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mGiaBan_YCu);
            mGiaBan_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mGiaBan_HoaChat);


            mXuatTraHang_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_Thuoc);
            mXuatTraHang_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_YCu);
            mXuatTraHang_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_HoaChat);

            mXuatChoBenhNhan_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatChoBenhNhan_Thuoc);
            mXuatChoBenhNhan_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatChoBenhNhan_YCu);
            mXuatChoBenhNhan_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatChoBenhNhan_HoaChat);
            #endregion

            #region operation
            mPhieuYeuCau_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_Thuoc,
                                               (int)oKhoPhongEx.mPhieuYeuCau_Thuoc_Tim, (int)ePermission.mView);
            mPhieuYeuCau_Thuoc_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_Thuoc,
                                               (int)oKhoPhongEx.mPhieuYeuCau_Thuoc_Them, (int)ePermission.mView);
            mPhieuYeuCau_Thuoc_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_Thuoc,
                                               (int)oKhoPhongEx.mPhieuYeuCau_Thuoc_Xoa, (int)ePermission.mView);
            mPhieuYeuCau_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_Thuoc,
                                               (int)oKhoPhongEx.mPhieuYeuCau_Thuoc_XemIn, (int)ePermission.mView);
            mPhieuYeuCau_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_Thuoc,
                                               (int)oKhoPhongEx.mPhieuYeuCau_Thuoc_In, (int)ePermission.mView);

            mPhieuYeuCau_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_YCu,
                                               (int)oKhoPhongEx.mPhieuYeuCau_YCu_Tim, (int)ePermission.mView);
            mPhieuYeuCau_YCu_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_YCu,
                                               (int)oKhoPhongEx.mPhieuYeuCau_YCu_Them, (int)ePermission.mView);
            mPhieuYeuCau_YCu_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_YCu,
                                               (int)oKhoPhongEx.mPhieuYeuCau_YCu_Xoa, (int)ePermission.mView);
            mPhieuYeuCau_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_YCu,
                                               (int)oKhoPhongEx.mPhieuYeuCau_YCu_XemIn, (int)ePermission.mView);
            mPhieuYeuCau_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_YCu,
                                               (int)oKhoPhongEx.mPhieuYeuCau_YCu_In, (int)ePermission.mView);

            mPhieuYeuCau_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_HoaChat,
                                               (int)oKhoPhongEx.mPhieuYeuCau_HoaChat_Tim, (int)ePermission.mView);
            mPhieuYeuCau_HoaChat_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_HoaChat,
                                               (int)oKhoPhongEx.mPhieuYeuCau_HoaChat_Them, (int)ePermission.mView);
            mPhieuYeuCau_HoaChat_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_HoaChat,
                                               (int)oKhoPhongEx.mPhieuYeuCau_HoaChat_Xoa, (int)ePermission.mView);
            mPhieuYeuCau_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_HoaChat,
                                               (int)oKhoPhongEx.mPhieuYeuCau_HoaChat_XemIn, (int)ePermission.mView);
            mPhieuYeuCau_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_HoaChat,
                                               (int)oKhoPhongEx.mPhieuYeuCau_HoaChat_In, (int)ePermission.mView);

            mNhapHangTuNCC_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_Thuoc,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_Thuoc_Tim, (int)ePermission.mView);
            mNhapHangTuNCC_Thuoc_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_Thuoc,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_Thuoc_Them, (int)ePermission.mView);
            mNhapHangTuNCC_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_Thuoc,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_Thuoc_XemIn, (int)ePermission.mView);
            mNhapHangTuNCC_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_Thuoc,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_Thuoc_In, (int)ePermission.mView);

            mNhapHangTuNCC_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_YCu,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_YCu_Tim, (int)ePermission.mView);
            mNhapHangTuNCC_YCu_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_YCu,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_YCu_Them, (int)ePermission.mView);
            mNhapHangTuNCC_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_YCu,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_YCu_XemIn, (int)ePermission.mView);
            mNhapHangTuNCC_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_YCu,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_YCu_In, (int)ePermission.mView);

            mNhapHangTuNCC_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_HoaChat,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_HoaChat_Tim, (int)ePermission.mView);
            mNhapHangTuNCC_HoaChat_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_HoaChat,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_HoaChat_Them, (int)ePermission.mView);
            mNhapHangTuNCC_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_HoaChat,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_HoaChat_XemIn, (int)ePermission.mView);
            mNhapHangTuNCC_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuNCC_HoaChat,
                                               (int)oKhoPhongEx.mNhapHangTuNCC_HoaChat_In, (int)ePermission.mView);

            mNhapHangTuKhoDuoc_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_Thuoc,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_Thuoc_Tim, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_Thuoc_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_Thuoc,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_Thuoc_Them, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_Thuoc,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_Thuoc_XemIn, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_Thuoc,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_Thuoc_In, (int)ePermission.mView);

            mNhapHangTuKhoDuoc_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_YCu,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_YCu_Tim, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_YCu_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_YCu,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_YCu_Them, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_YCu,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_YCu_XemIn, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_YCu,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_YCu_In, (int)ePermission.mView);

            mNhapHangTuKhoDuoc_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_HoaChat,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_HoaChat_Tim, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_HoaChat_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_HoaChat,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_HoaChat_Them, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_HoaChat,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_HoaChat_XemIn, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_HoaChat,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_HoaChat_In, (int)ePermission.mView);

            mKiemKe_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Thuoc,
                                               (int)oKhoPhongEx.mKiemKe_Thuoc_Tim, (int)ePermission.mView);
            mKiemKe_Thuoc_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Thuoc,
                                               (int)oKhoPhongEx.mKiemKe_Thuoc_ThemMoi, (int)ePermission.mView);
            mKiemKe_Thuoc_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Thuoc,
                                               (int)oKhoPhongEx.mKiemKe_Thuoc_XuatExcel, (int)ePermission.mView);
            mKiemKe_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Thuoc,
                                               (int)oKhoPhongEx.mKiemKe_Thuoc_XemIn, (int)ePermission.mView);

            mKiemKe_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_YCu,
                                               (int)oKhoPhongEx.mKiemKe_YCu_Tim, (int)ePermission.mView);
            mKiemKe_YCu_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_YCu,
                                               (int)oKhoPhongEx.mKiemKe_YCu_ThemMoi, (int)ePermission.mView);
            mKiemKe_YCu_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_YCu,
                                               (int)oKhoPhongEx.mKiemKe_YCu_XuatExcel, (int)ePermission.mView);
            mKiemKe_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_YCu,
                                               (int)oKhoPhongEx.mKiemKe_YCu_XemIn, (int)ePermission.mView);

            mKiemKe_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_HoaChat,
                                               (int)oKhoPhongEx.mKiemKe_HoaChat_Tim, (int)ePermission.mView);
            mKiemKe_HoaChat_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_HoaChat,
                                               (int)oKhoPhongEx.mKiemKe_HoaChat_ThemMoi, (int)ePermission.mView);
            mKiemKe_HoaChat_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_HoaChat,
                                               (int)oKhoPhongEx.mKiemKe_HoaChat_XuatExcel, (int)ePermission.mView);
            mKiemKe_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_HoaChat,
                                               (int)oKhoPhongEx.mKiemKe_HoaChat_XemIn, (int)ePermission.mView);

            // BO SUNG KIEM KE KHOA PHONG
            mKiemKe_VTYTTH_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                              , (int)eKhoPhong.mKiemKe_VTYTTH,
                                              (int)oKhoPhongEx.mKiemKe_VTYTTH_Tim, (int)ePermission.mView);
            mKiemKe_VTYTTH_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_VTYTTH,
                                               (int)oKhoPhongEx.mKiemKe_VTYTTH_ThemMoi, (int)ePermission.mView);
            mKiemKe_VTYTTH_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_VTYTTH,
                                               (int)oKhoPhongEx.mKiemKe_VTYTTH_XuatExcel, (int)ePermission.mView);
            mKiemKe_VTYTTH_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_VTYTTH,
                                               (int)oKhoPhongEx.mKiemKe_VTYTTH_XemIn, (int)ePermission.mView);

            mKiemKe_Vaccine_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                              , (int)eKhoPhong.mKiemKe_Vaccine,
                                              (int)oKhoPhongEx.mKiemKe_Vaccine_Tim, (int)ePermission.mView);
            mKiemKe_Vaccine_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Vaccine,
                                               (int)oKhoPhongEx.mKiemKe_Vaccine_ThemMoi, (int)ePermission.mView);
            mKiemKe_Vaccine_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Vaccine,
                                               (int)oKhoPhongEx.mKiemKe_Vaccine_XuatExcel, (int)ePermission.mView);
            mKiemKe_Vaccine_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Vaccine,
                                               (int)oKhoPhongEx.mKiemKe_Vaccine_XemIn, (int)ePermission.mView);

            mKiemKe_Blood_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                              , (int)eKhoPhong.mKiemKe_Blood,
                                              (int)oKhoPhongEx.mKiemKe_Blood_Tim, (int)ePermission.mView);
            mKiemKe_Blood_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Blood,
                                               (int)oKhoPhongEx.mKiemKe_Blood_ThemMoi, (int)ePermission.mView);
            mKiemKe_Blood_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Blood,
                                               (int)oKhoPhongEx.mKiemKe_Blood_XuatExcel, (int)ePermission.mView);
            mKiemKe_Blood_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Blood,
                                               (int)oKhoPhongEx.mKiemKe_Blood_XemIn, (int)ePermission.mView);

            mKiemKe_VTTH_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                              , (int)eKhoPhong.mKiemKe_VTTH,
                                              (int)oKhoPhongEx.mKiemKe_VTTH_Tim, (int)ePermission.mView);
            mKiemKe_VTTH_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_VTTH,
                                               (int)oKhoPhongEx.mKiemKe_VTTH_ThemMoi, (int)ePermission.mView);
            mKiemKe_VTTH_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_VTTH,
                                               (int)oKhoPhongEx.mKiemKe_VTTH_XuatExcel, (int)ePermission.mView);
            mKiemKe_VTTH_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_VTTH,
                                               (int)oKhoPhongEx.mKiemKe_VTTH_XemIn, (int)ePermission.mView);

            mKiemKe_VPP_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                              , (int)eKhoPhong.mKiemKe_VPP,
                                              (int)oKhoPhongEx.mKiemKe_VPP_Tim, (int)ePermission.mView);
            mKiemKe_VPP_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_VPP,
                                               (int)oKhoPhongEx.mKiemKe_VPP_ThemMoi, (int)ePermission.mView);
            mKiemKe_VPP_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_VPP,
                                               (int)oKhoPhongEx.mKiemKe_VPP_XuatExcel, (int)ePermission.mView);
            mKiemKe_VPP_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_VPP,
                                               (int)oKhoPhongEx.mKiemKe_VPP_XemIn, (int)ePermission.mView);

            mKiemKe_ThanhTrung_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                              , (int)eKhoPhong.mKiemKe_ThanhTrung,
                                              (int)oKhoPhongEx.mKiemKe_ThanhTrung_Tim, (int)ePermission.mView);
            mKiemKe_ThanhTrung_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_ThanhTrung,
                                               (int)oKhoPhongEx.mKiemKe_ThanhTrung_ThemMoi, (int)ePermission.mView);
            mKiemKe_ThanhTrung_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_ThanhTrung,
                                               (int)oKhoPhongEx.mKiemKe_ThanhTrung_XuatExcel, (int)ePermission.mView);
            mKiemKe_ThanhTrung_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_ThanhTrung, 
                                               (int)oKhoPhongEx.mKiemKe_ThanhTrung_XemIn, (int)ePermission.mView);
            //END

            mThongKe_Thuoc_xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_Thuoc,
                                               (int)oKhoPhongEx.mThongKe_Thuoc_xem, (int)ePermission.mView);
            mThongKe_Thuoc_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_Thuoc,
                                               (int)oKhoPhongEx.mThongKe_Thuoc_PhieuMoi, (int)ePermission.mView);
            mThongKe_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_Thuoc,
                                               (int)oKhoPhongEx.mThongKe_Thuoc_XemIn, (int)ePermission.mView);
            mThongKe_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_Thuoc,
                                               (int)oKhoPhongEx.mThongKe_Thuoc_In, (int)ePermission.mView);

            mThongKe_YCu_xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_YCu,
                                               (int)oKhoPhongEx.mThongKe_YCu_xem, (int)ePermission.mView);
            mThongKe_YCu_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_YCu,
                                               (int)oKhoPhongEx.mThongKe_YCu_PhieuMoi, (int)ePermission.mView);
            mThongKe_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_YCu,
                                               (int)oKhoPhongEx.mThongKe_YCu_XemIn, (int)ePermission.mView);
            mThongKe_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_YCu,
                                               (int)oKhoPhongEx.mThongKe_YCu_In, (int)ePermission.mView);

            mThongKe_HoaChat_xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_HoaChat,
                                               (int)oKhoPhongEx.mThongKe_HoaChat_xem, (int)ePermission.mView);
            mThongKe_HoaChat_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_HoaChat,
                                               (int)oKhoPhongEx.mThongKe_HoaChat_PhieuMoi, (int)ePermission.mView);
            mThongKe_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_HoaChat,
                                               (int)oKhoPhongEx.mThongKe_HoaChat_XemIn, (int)ePermission.mView);
            mThongKe_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mThongKe_HoaChat,
                                               (int)oKhoPhongEx.mThongKe_HoaChat_In, (int)ePermission.mView);

            mBaoCaoXuat_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCXuat_Thuoc,
                                               (int)oKhoPhongEx.mBaoCaoXuat_Thuoc_XemIn, (int)ePermission.mView);
            mBaoCaoXuat_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCXuat_YCu,
                                               (int)oKhoPhongEx.mBaoCaoXuat_YCu_XemIn, (int)ePermission.mView);
            mBaoCaoXuat_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCXuat_HoaChat,
                                               (int)oKhoPhongEx.mBaoCaoXuat_HoaChat_XemIn, (int)ePermission.mView);

            mBaoCaoXuatNhapTon_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCNhapXuatTon_Thuoc,
                                               (int)oKhoPhongEx.mBaoCaoXuatNhapTon_Thuoc_XemIn, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_Thuoc_KetChuyen = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCNhapXuatTon_Thuoc,
                                               (int)oKhoPhongEx.mBaoCaoXuatNhapTon_Thuoc_KetChuyen, (int)ePermission.mView);

            mBaoCaoXuatNhapTon_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCNhapXuatTon_YCu,
                                               (int)oKhoPhongEx.mBaoCaoXuatNhapTon_YCu_XemIn, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_YCu_KetChuyen = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCNhapXuatTon_YCu,
                                               (int)oKhoPhongEx.mBaoCaoXuatNhapTon_YCu_KetChuyen, (int)ePermission.mView);
            
            mBaoCaoXuatNhapTon_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCNhapXuatTon_HoaChat,
                                               (int)oKhoPhongEx.mBaoCaoXuatNhapTon_HoaChat_XemIn, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_HoaChat_KetChuyen = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCNhapXuatTon_HoaChat,
                                               (int)oKhoPhongEx.mBaoCaoXuatNhapTon_HoaChat_KetChuyen, (int)ePermission.mView);

            mBaoCaoTheKho_Thuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCTheKho_Thuoc,
                                               (int)oKhoPhongEx.mBaoCaoTheKho_Thuoc_Xem, (int)ePermission.mView);
            mBaoCaoTheKho_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCTheKho_Thuoc,
                                               (int)oKhoPhongEx.mBaoCaoTheKho_Thuoc_In, (int)ePermission.mView);

            mBaoCaoTheKho_YCu_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCTheKho_YCu,
                                               (int)oKhoPhongEx.mBaoCaoTheKho_YCu_Xem, (int)ePermission.mView);
            mBaoCaoTheKho_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCTheKho_YCu,
                                               (int)oKhoPhongEx.mBaoCaoTheKho_YCu_In, (int)ePermission.mView);

            mBaoCaoTheKho_HoaChat_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCTheKho_HoaChat,
                                               (int)oKhoPhongEx.mBaoCaoTheKho_HoaChat_Xem, (int)ePermission.mView);
            mBaoCaoTheKho_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCTheKho_HoaChat,
                                               (int)oKhoPhongEx.mBaoCaoTheKho_HoaChat_In, (int)ePermission.mView);

            mQLDanhMuc_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mDanhMuc_Thuoc,
                                               (int)oKhoPhongEx.mQLDanhMuc_Thuoc_Tim, (int)ePermission.mView);
            mQLDanhMuc_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mDanhMuc_YCu,
                                               (int)oKhoPhongEx.mQLDanhMuc_YCu_Tim, (int)ePermission.mView);
            mQLDanhMuc_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mDanhMuc_HoaChat,
                                               (int)oKhoPhongEx.mQLDanhMuc_HoaChat_Tim, (int)ePermission.mView);

            mGiaBan_Thuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mGiaBan_Thuoc,
                                               (int)oKhoPhongEx.mGiaBan_Thuoc_Tim, (int)ePermission.mView);
            mGiaBan_Thuoc_XemDSGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mGiaBan_Thuoc,
                                               (int)oKhoPhongEx.mGiaBan_Thuoc_XemDSGia, (int)ePermission.mView);

            mGiaBan_YCu_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mGiaBan_YCu,
                                               (int)oKhoPhongEx.mGiaBan_YCu_Tim, (int)ePermission.mView);
            mGiaBan_YCu_XemDSGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mGiaBan_YCu,
                                               (int)oKhoPhongEx.mGiaBan_YCu_XemDSGia, (int)ePermission.mView);

            mGiaBan_HoaChat_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mGiaBan_HoaChat,
                                               (int)oKhoPhongEx.mGiaBan_HoaChat_Tim, (int)ePermission.mView);
            mGiaBan_HoaChat_XemDSGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mGiaBan_HoaChat,
                                               (int)oKhoPhongEx.mGiaBan_HoaChat_XemDSGia, (int)ePermission.mView);

            mXuatTraHang_Thuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_Thuoc,
                                               (int)oKhoPhongEx.mXuatTraHang_Thuoc_Xem, (int)ePermission.mView);
            mXuatTraHang_Thuoc_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_Thuoc,
                                               (int)oKhoPhongEx.mXuatTraHang_Thuoc_PhieuMoi, (int)ePermission.mView);
            mXuatTraHang_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_Thuoc,
                                               (int)oKhoPhongEx.mXuatTraHang_Thuoc_XemIn, (int)ePermission.mView);
            mXuatTraHang_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_Thuoc,
                                               (int)oKhoPhongEx.mXuatTraHang_Thuoc_In, (int)ePermission.mView);

            mXuatTraHang_YCu_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_YCu,
                                               (int)oKhoPhongEx.mXuatTraHang_YCu_Xem, (int)ePermission.mView);
            mXuatTraHang_YCu_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_YCu,
                                               (int)oKhoPhongEx.mXuatTraHang_YCu_PhieuMoi, (int)ePermission.mView);
            mXuatTraHang_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_YCu,
                                               (int)oKhoPhongEx.mXuatTraHang_YCu_XemIn, (int)ePermission.mView);
            mXuatTraHang_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_YCu,
                                               (int)oKhoPhongEx.mXuatTraHang_YCu_In, (int)ePermission.mView);

            mXuatTraHang_HoaChat_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_HoaChat,
                                               (int)oKhoPhongEx.mXuatTraHang_HoaChat_Xem, (int)ePermission.mView);
            mXuatTraHang_HoaChat_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_HoaChat,
                                               (int)oKhoPhongEx.mXuatTraHang_HoaChat_PhieuMoi, (int)ePermission.mView);
            mXuatTraHang_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_HoaChat,
                                               (int)oKhoPhongEx.mXuatTraHang_HoaChat_XemIn, (int)ePermission.mView);
            mXuatTraHang_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_HoaChat,
                                               (int)oKhoPhongEx.mXuatTraHang_HoaChat_In, (int)ePermission.mView);

            mXuatChoBenhNhan_Thuoc_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_Thuoc,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_Thuoc_Xem, (int)ePermission.mView);
            mXuatChoBenhNhan_Thuoc_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_Thuoc,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_Thuoc_PhieuMoi, (int)ePermission.mView);
            mXuatChoBenhNhan_Thuoc_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_Thuoc,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_Thuoc_XemIn, (int)ePermission.mView);
            mXuatChoBenhNhan_Thuoc_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_Thuoc,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_Thuoc_In, (int)ePermission.mView);

            mXuatChoBenhNhan_YCu_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_YCu,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_YCu_Xem, (int)ePermission.mView);
            mXuatChoBenhNhan_YCu_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_YCu,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_YCu_PhieuMoi, (int)ePermission.mView);
            mXuatChoBenhNhan_YCu_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_YCu,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_YCu_XemIn, (int)ePermission.mView);
            mXuatChoBenhNhan_YCu_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_YCu,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_YCu_In, (int)ePermission.mView);

            mXuatChoBenhNhan_HoaChat_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_HoaChat,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_HoaChat_Xem, (int)ePermission.mView);
            mXuatChoBenhNhan_HoaChat_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_HoaChat,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_HoaChat_PhieuMoi, (int)ePermission.mView);
            mXuatChoBenhNhan_HoaChat_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_HoaChat,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_HoaChat_XemIn, (int)ePermission.mView);
            mXuatChoBenhNhan_HoaChat_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_HoaChat,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_HoaChat_In, (int)ePermission.mView);


            #endregion

            mTaoMauXuat = mTaoMauXuat_Thuoc || mTaoMauXuat_YCu || mTaoMauXuat_HoaChat;

            //▼====== #005          
            mPhieuYeuCau_DinhDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                               (int)eKhoPhong.mPhieuYeuCau_DinhDuong);
            mPhieuYeuCau_DinhDuong_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_DinhDuong,
                                               (int)oKhoPhongEx.mPhieuYeuCau_DinhDuong_Tim, (int)ePermission.mView);
            mPhieuYeuCau_DinhDuong_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_DinhDuong,
                                               (int)oKhoPhongEx.mPhieuYeuCau_DinhDuong_Them, (int)ePermission.mView);
            mPhieuYeuCau_DinhDuong_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_DinhDuong,
                                               (int)oKhoPhongEx.mPhieuYeuCau_DinhDuong_Xoa, (int)ePermission.mView);
            mPhieuYeuCau_DinhDuong_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_DinhDuong,
                                               (int)oKhoPhongEx.mPhieuYeuCau_DinhDuong_XemIn, (int)ePermission.mView);
            mPhieuYeuCau_DinhDuong_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mPhieuYeuCau_DinhDuong,
                                               (int)oKhoPhongEx.mPhieuYeuCau_DinhDuong_In, (int)ePermission.mView);

            mNhapHangTuKhoDuoc_DinhDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_DinhDuong);
            mNhapHangTuKhoDuoc_DinhDuong_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_DinhDuong,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_DinhDuong_Tim, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_DinhDuong_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_DinhDuong,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_DinhDuong_Them, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_DinhDuong_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_DinhDuong,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_DinhDuong_XemIn, (int)ePermission.mView);
            mNhapHangTuKhoDuoc_DinhDuong_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mNhapHangTuKhoDuoc_DinhDuong,
                                               (int)oKhoPhongEx.mNhapHangTuKhoDuoc_DinhDuong_In, (int)ePermission.mView);

            mXuatTraHang_DDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_DDuong);
            mXuatTraHang_DDuong_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_DDuong,
                                               (int)oKhoPhongEx.mXuatTraHang_DDuong_Xem, (int)ePermission.mView);
            mXuatTraHang_DDuong_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_DDuong,
                                               (int)oKhoPhongEx.mXuatTraHang_DDuong_PhieuMoi, (int)ePermission.mView);
            mXuatTraHang_DDuong_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_DDuong,
                                               (int)oKhoPhongEx.mXuatTraHang_DDuong_XemIn, (int)ePermission.mView);
            mXuatTraHang_DDuong_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatTraHang_DDuong,
                                               (int)oKhoPhongEx.mXuatTraHang_DDuong_In, (int)ePermission.mView);

            //XUAT CHO BN
            mXuatChoBenhNhan_DDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatChoBenhNhan_DDuong);

            mXuatChoBenhNhan_DDuong_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_DDuong,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_DDuong_Xem, (int)ePermission.mView);
            mXuatChoBenhNhan_DDuong_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_DDuong,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_DDuong_PhieuMoi, (int)ePermission.mView);
            mXuatChoBenhNhan_DDuong_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_DDuong,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_DDuong_XemIn, (int)ePermission.mView);
            mXuatChoBenhNhan_DDuong_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mXuatChoBenhNhan_DDuong,
                                               (int)oKhoPhongEx.mXuatChoBenhNhan_DDuong_In, (int)ePermission.mView);

            mBCNhapXuatTon_DDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_DDuong);

            mBaoCaoXuatNhapTon_DDuong_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCNhapXuatTon_DDuong,
                                               (int)oKhoPhongEx.mBaoCaoXuatNhapTon_DDuong_XemIn, (int)ePermission.mView);
            mBaoCaoXuatNhapTon_DDuong_KetChuyen = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCNhapXuatTon_DDuong,
                                               (int)oKhoPhongEx.mBaoCaoXuatNhapTon_DDuong_KetChuyen, (int)ePermission.mView);

            mBCTheKho_DDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_DDuong);
            mBaoCaoTheKho_DDuong_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCTheKho_DDuong,
                                               (int)oKhoPhongEx.mBaoCaoTheKho_DDuong_Xem, (int)ePermission.mView);
            mBaoCaoTheKho_DDuong_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mBCTheKho_DDuong,
                                               (int)oKhoPhongEx.mBaoCaoTheKho_DDuong_In, (int)ePermission.mView);
            //▲====== #005

            //▼====== #006
            mKiemKe_MoKho = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Thuoc,
                                               (int)oKhoPhongEx.mKiemKe_MoKho, (int)ePermission.mView);
            mKiemKe_KhoaKho = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Thuoc,
                                               (int)oKhoPhongEx.mKiemKe_KhoaKho, (int)ePermission.mView);
            mKiemKe_KhoaTatCa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoPhong
                                               , (int)eKhoPhong.mKiemKe_Thuoc,
                                               (int)oKhoPhongEx.mKiemKe_KhoaTatCa, (int)ePermission.mView);
            //▲====== #006

            //▼====== #007
            mTaoMauLinh_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mTaoMauLinh_Thuoc);
            mTaoMauLinh_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mTaoMauLinh_YCu);
            mTaoMauLinh = mTaoMauLinh_Thuoc || mTaoMauLinh_YCu;

            mPhieuYeuCau_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mPhieuYeuCau_VTYTTH);
            mPhieuYeuCau_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mPhieuYeuCau_Vaccine);
            mPhieuYeuCau_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mPhieuYeuCau_Blood);
            mPhieuYeuCau_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mPhieuYeuCau_VPP);
            mPhieuYeuCau_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mPhieuYeuCau_VTTH);
            mPhieuYeuCau_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mPhieuYeuCau_ThanhTrung);
            mPhieuYeuCau_TongHop = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mPhieuYeuCau_TongHop);
            mPhieuYeuCau_SuatAn = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mPhieuYeuCau_SuatAn);
            mPhieuYeuCau = mPhieuYeuCau_Thuoc || mPhieuYeuCau_YCu || mPhieuYeuCau_HoaChat || mPhieuYeuCau_DinhDuong || mPhieuYeuCau_VTYTTH ||
                            mPhieuYeuCau_Vaccine || mPhieuYeuCau_Blood || mPhieuYeuCau_VPP || mPhieuYeuCau_VTTH || mPhieuYeuCau_ThanhTrung || mPhieuYeuCau_SuatAn || mPhieuYeuCau_TongHop;

            mNhapHangTuKhoDuoc_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_VTYTTH);
            mNhapHangTuKhoDuoc_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_Vaccine);
            mNhapHangTuKhoDuoc_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_Blood);
            mNhapHangTuKhoDuoc_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_VPP);
            mNhapHangTuKhoDuoc_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_VTTH);
            mNhapHangTuKhoDuoc_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mNhapHangTuKhoDuoc_ThanhTrung);

            mNhapHangTuKhoDuoc = mNhapHangTuKhoDuoc_Thuoc || mNhapHangTuKhoDuoc_YCu || mNhapHangTuKhoDuoc_HoaChat || mNhapHangTuKhoDuoc_DinhDuong || mNhapHangTuKhoDuoc_VTYTTH ||
                                mNhapHangTuKhoDuoc_Vaccine || mNhapHangTuKhoDuoc_Blood || mNhapHangTuKhoDuoc_VPP || mNhapHangTuKhoDuoc_VTTH || mNhapHangTuKhoDuoc_ThanhTrung;

            mXuatTraHang_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_VTYTTH);
            mXuatTraHang_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_Vaccine);
            mXuatTraHang_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_Blood);
            mXuatTraHang_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_VPP);
            mXuatTraHang_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_VTTH);
            mXuatTraHang_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatTraHang_ThanhTrung);

            mXuatTraHang = mXuatTraHang_Thuoc || mXuatTraHang_YCu || mXuatTraHang_HoaChat || mXuatTraHang_DDuong || mXuatTraHang_VTYTTH ||
                            mXuatTraHang_Vaccine || mXuatTraHang_Blood || mXuatTraHang_VPP || mXuatTraHang_VTTH || mXuatTraHang_ThanhTrung;

            mXuatChoBenhNhanNgoaiTru_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatChoBenhNhanNgoaiTru_Thuoc);
            mXuatChoBenhNhanNgoaiTru_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatChoBenhNhanNgoaiTru_YCu);
            mXuatChoBenhNhanNgoaiTru_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatChoBenhNhanNgoaiTru_HoaChat);
            mXuatChoBenhNhanNgoaiTru_DinhDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                         (int)eKhoPhong.mXuatChoBenhNhanNgoaiTru_DinhDuong);

            mKiemKe_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_VTYTTH);
            mKiemKe_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_Vaccine);
            mKiemKe_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_Blood);
            mKiemKe_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_VPP);
            mKiemKe_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_VTTH);
            mKiemKe_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_ThanhTrung);
            mKiemKe_DinhDuong = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mKiemKe_DinhDuong);

            mKiemKe = mKiemKe_Thuoc || mKiemKe_YCu || mKiemKe_HoaChat || mKiemKe_DinhDuong || mKiemKe_VTYTTH ||
                    mKiemKe_Vaccine || mKiemKe_Blood || mKiemKe_VPP || mKiemKe_VTTH || mKiemKe_ThanhTrung;

            mBCXuatChoBN_Thuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatChoBN_Thuoc);
            mBCXuatChoBN_YCu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatChoBN_YCu);
            mBCXuatChoBN_HoaChat = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatChoBN_HoaChat);
            mBCXuatChoBN_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatChoBN_VTYTTH);
            mBCXuatChoBN_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatChoBN_Vaccine);
            mBCXuatChoBN_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatChoBN_Blood);
            mBCXuatChoBN_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatChoBN_VPP);
            mBCXuatChoBN_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatChoBN_VTTH);
            mBCXuatChoBN_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatChoBN_ThanhTrung);
            mBC15NgaySuDungThuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBC15NgaySuDungThuoc);

            mBCXuatChoBN = mBCXuatChoBN_Thuoc || mBCXuatChoBN_YCu || mBCXuatChoBN_HoaChat || mBCXuatChoBN_VTYTTH || mBCXuatChoBN_Vaccine ||
                            mBCXuatChoBN_Blood || mBCXuatChoBN_VPP || mBCXuatChoBN_VTTH || mBCXuatChoBN_ThanhTrung;

            mBCXuat_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuat_VTYTTH);
            mBCXuat_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuat_Vaccine);
            mBCXuat_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuat_Blood);
            mBCXuat_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuat_VPP);
            mBCXuat_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuat_VTTH);
            mBCXuat_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuat_ThanhTrung);

            mBCXuat = mBCXuat_Thuoc || mBCXuat_YCu || mBCXuat_HoaChat || mBCXuat_VTYTTH || mBCXuat_Vaccine ||
                            mBCXuat_Blood || mBCXuat_VPP || mBCXuat_VTTH || mBCXuat_ThanhTrung;

            mBCNhapXuatTon_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_VTYTTH);
            mBCNhapXuatTon_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_Vaccine);
            mBCNhapXuatTon_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_Blood);
            mBCNhapXuatTon_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_VPP);
            mBCNhapXuatTon_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_VTTH);
            mBCNhapXuatTon_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCNhapXuatTon_ThanhTrung);

            mBCNhapXuatTon = mBCNhapXuatTon_Thuoc || mBCNhapXuatTon_YCu || mBCNhapXuatTon_HoaChat || mBCNhapXuatTon_VTYTTH || mBCNhapXuatTon_Vaccine ||
                            mBCNhapXuatTon_Blood || mBCNhapXuatTon_VPP || mBCNhapXuatTon_VTTH || mBCNhapXuatTon_ThanhTrung || mBCNhapXuatTon_DDuong;

            mBCTheKho_VTYTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_VTYTTH);
            mBCTheKho_Vaccine = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_Vaccine);
            mBCTheKho_Blood = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_Blood);
            mBCTheKho_VPP = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_VPP);
            mBCTheKho_VTTH = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_VTTH);
            mBCTheKho_ThanhTrung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCTheKho_ThanhTrung);

            mBCTheKho = mBCTheKho_Thuoc || mBCTheKho_YCu || mBCTheKho_HoaChat || mBCTheKho_VTYTTH || mBCTheKho_Vaccine ||
                            mBCTheKho_Blood || mBCTheKho_VPP || mBCTheKho_VTTH || mBCTheKho_ThanhTrung || mBCTheKho_DDuong;

            mBCThuocSapHetHan = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCThuocSapHetHan);

            mQLChiTieuNhanSu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mQLChiTieuNhanSu);
            mQLTinhHinhKhoa = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mQLTinhHinhKhoa);
            gQLTinhHinhKhoa = mQLChiTieuNhanSu || mQLTinhHinhKhoa;
            //▲====== #007

            mBCMauSapHetHanDung = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCMauSapHetHanDung);
            mTreatmentStatisticsByDept = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mTreatmentStatisticsByDept);
            //▼==== #011
            mBCXuatSDThuocCanQuang = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong,
                                                             (int)eKhoPhong.mBCXuatSDThuocCanQuang);
            //▲==== #011
        }

        #region bool checking

        #region function
        private bool _mTaoMauXuat = true;
        private bool _mTaoMauXuat_Thuoc = true;
        private bool _mTaoMauXuat_YCu = true;
        private bool _mTaoMauXuat_HoaChat = true;

        //25-07-2012
        private bool _mPhieuYeuCau_Thuoc = true;
        private bool _mPhieuYeuCau_YCu = true;
        private bool _mPhieuYeuCau_HoaChat = true;

        private bool _mNhapHangTuNCC_Thuoc = true;
        private bool _mNhapHangTuNCC_YCu = true;
        private bool _mNhapHangTuNCC_HoaChat = true;

        private bool _mNhapHangTuKhoDuoc_Thuoc = true;
        private bool _mNhapHangTuKhoDuoc_YCu = true;
        private bool _mNhapHangTuKhoDuoc_HoaChat = true;

        private bool _mKiemKe_Thuoc = true;
        private bool _mKiemKe_YCu = true;
        private bool _mKiemKe_HoaChat = true;

        private bool _mThongKe_Thuoc = true;
        private bool _mThongKe_YCu = true;
        private bool _mThongKe_HoaChat = true;

        private bool _mBCXuat_Thuoc = true;
        private bool _mBCXuat_YCu = true;
        private bool _mBCXuat_HoaChat = true;

        private bool _mBCNhapXuatTon_Thuoc = true;
        private bool _mBCNhapXuatTon_YCu = true;
        private bool _mBCNhapXuatTon_HoaChat = true;

        private bool _mBCTheKho_Thuoc = true;
        private bool _mBCTheKho_YCu = true;
        private bool _mBCTheKho_HoaChat = true;

        private bool _mDanhMuc_Thuoc = true;
        private bool _mDanhMuc_YCu = true;
        private bool _mDanhMuc_HoaChat = true;

        private bool _mGiaBan_Thuoc = true;
        private bool _mGiaBan_YCu = true;
        private bool _mGiaBan_HoaChat = true;

        private bool _mXuatTraHang_Thuoc = true;
        private bool _mXuatTraHang_YCu = true;
        private bool _mXuatTraHang_HoaChat = true;

        private bool _mXuatChoBenhNhan_Thuoc = true;
        private bool _mXuatChoBenhNhan_YCu = true;
        private bool _mXuatChoBenhNhan_HoaChat = true;

        public bool mTaoMauXuat
        {
            get
            {
                return _mTaoMauXuat;
            }
            set
            {
                if (_mTaoMauXuat == value)
                    return;
                _mTaoMauXuat = value;
                NotifyOfPropertyChange(() => mTaoMauXuat);
            }
        }

        public bool mTaoMauXuat_Thuoc
        {
            get
            {
                return _mTaoMauXuat_Thuoc;
            }
            set
            {
                if (_mTaoMauXuat_Thuoc == value)
                    return;
                _mTaoMauXuat_Thuoc = value;
                NotifyOfPropertyChange(() => mTaoMauXuat_Thuoc);
            }
        }

        public bool mTaoMauXuat_YCu
        {
            get
            {
                return _mTaoMauXuat_YCu;
            }
            set
            {
                if (_mTaoMauXuat_YCu == value)
                    return;
                _mTaoMauXuat_YCu = value;
                NotifyOfPropertyChange(() => mTaoMauXuat_YCu);
            }
        }

        public bool mTaoMauXuat_HoaChat
        {
            get
            {
                return _mTaoMauXuat_HoaChat;
            }
            set
            {
                if (_mTaoMauXuat_HoaChat == value)
                    return;
                _mTaoMauXuat_HoaChat = value;
                NotifyOfPropertyChange(() => mTaoMauXuat_HoaChat);
            }
        }


        //25-07-2012
        public bool mPhieuYeuCau_Thuoc
        {
            get
            {
                return _mPhieuYeuCau_Thuoc;
            }
            set
            {
                if (_mPhieuYeuCau_Thuoc == value)
                    return;
                _mPhieuYeuCau_Thuoc = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Thuoc);
            }
        }

        public bool mPhieuYeuCau_YCu
        {
            get
            {
                return _mPhieuYeuCau_YCu;
            }
            set
            {
                if (_mPhieuYeuCau_YCu == value)
                    return;
                _mPhieuYeuCau_YCu = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_YCu);
            }
        }

        public bool mPhieuYeuCau_HoaChat
        {
            get
            {
                return _mPhieuYeuCau_HoaChat;
            }
            set
            {
                if (_mPhieuYeuCau_HoaChat == value)
                    return;
                _mPhieuYeuCau_HoaChat = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_HoaChat);
            }
        }


        public bool mNhapHangTuNCC_Thuoc
        {
            get
            {
                return _mNhapHangTuNCC_Thuoc;
            }
            set
            {
                if (_mNhapHangTuNCC_Thuoc == value)
                    return;
                _mNhapHangTuNCC_Thuoc = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_Thuoc);
            }
        }

        public bool mNhapHangTuNCC_YCu
        {
            get
            {
                return _mNhapHangTuNCC_YCu;
            }
            set
            {
                if (_mNhapHangTuNCC_YCu == value)
                    return;
                _mNhapHangTuNCC_YCu = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_YCu);
            }
        }

        public bool mNhapHangTuNCC_HoaChat
        {
            get
            {
                return _mNhapHangTuNCC_HoaChat;
            }
            set
            {
                if (_mNhapHangTuNCC_HoaChat == value)
                    return;
                _mNhapHangTuNCC_HoaChat = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_HoaChat);
            }
        }


        public bool mNhapHangTuKhoDuoc_Thuoc
        {
            get
            {
                return _mNhapHangTuKhoDuoc_Thuoc;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_Thuoc == value)
                    return;
                _mNhapHangTuKhoDuoc_Thuoc = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_Thuoc);
            }
        }
        
        public bool mNhapHangTuKhoDuoc_YCu
        {
            get
            {
                return _mNhapHangTuKhoDuoc_YCu;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_YCu == value)
                    return;
                _mNhapHangTuKhoDuoc_YCu = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_YCu);
            }
        }

        public bool mNhapHangTuKhoDuoc_HoaChat
        {
            get
            {
                return _mNhapHangTuKhoDuoc_HoaChat;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_HoaChat == value)
                    return;
                _mNhapHangTuKhoDuoc_HoaChat = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_HoaChat);
            }
        }

        
        public bool mKiemKe_Thuoc
        {
            get
            {
                return _mKiemKe_Thuoc;
            }
            set
            {
                if (_mKiemKe_Thuoc == value)
                    return;
                _mKiemKe_Thuoc = value;
                NotifyOfPropertyChange(() => mKiemKe_Thuoc);
            }
        }

        public bool mKiemKe_YCu
        {
            get
            {
                return _mKiemKe_YCu;
            }
            set
            {
                if (_mKiemKe_YCu == value)
                    return;
                _mKiemKe_YCu = value;
                NotifyOfPropertyChange(() => mKiemKe_YCu);
            }
        }

        public bool mKiemKe_HoaChat
        {
            get
            {
                return _mKiemKe_HoaChat;
            }
            set
            {
                if (_mKiemKe_HoaChat == value)
                    return;
                _mKiemKe_HoaChat = value;
                NotifyOfPropertyChange(() => mKiemKe_HoaChat);
            }
        }
        

        public bool mThongKe_Thuoc
        {
            get
            {
                return _mThongKe_Thuoc;
            }
            set
            {
                if (_mThongKe_Thuoc == value)
                    return;
                _mThongKe_Thuoc = value;
                NotifyOfPropertyChange(() => mThongKe_Thuoc);
            }
        }

        public bool mThongKe_YCu
        {
            get
            {
                return _mThongKe_YCu;
            }
            set
            {
                if (_mThongKe_YCu == value)
                    return;
                _mThongKe_YCu = value;
                NotifyOfPropertyChange(() => mThongKe_YCu);
            }
        }
        
        public bool mThongKe_HoaChat
        {
            get
            {
                return _mThongKe_HoaChat;
            }
            set
            {
                if (_mThongKe_HoaChat == value)
                    return;
                _mThongKe_HoaChat = value;
                NotifyOfPropertyChange(() => mThongKe_HoaChat);
            }
        }


        public bool mBCXuat_Thuoc
        {
            get
            {
                return _mBCXuat_Thuoc;
            }
            set
            {
                if (_mBCXuat_Thuoc == value)
                    return;
                _mBCXuat_Thuoc = value;
                NotifyOfPropertyChange(() => mBCXuat_Thuoc);
            }
        }
        
        public bool mBCXuat_YCu
        {
            get
            {
                return _mBCXuat_YCu;
            }
            set
            {
                if (_mBCXuat_YCu == value)
                    return;
                _mBCXuat_YCu = value;
                NotifyOfPropertyChange(() => mBCXuat_YCu);
            }
        }
        
        public bool mBCXuat_HoaChat
        {
            get
            {
                return _mBCXuat_HoaChat;
            }
            set
            {
                if (_mBCXuat_HoaChat == value)
                    return;
                _mBCXuat_HoaChat = value;
                NotifyOfPropertyChange(() => mBCXuat_HoaChat);
            }
        }


        public bool mBCNhapXuatTon_Thuoc
        {
            get
            {
                return _mBCNhapXuatTon_Thuoc;
            }
            set
            {
                if (_mBCNhapXuatTon_Thuoc == value)
                    return;
                _mBCNhapXuatTon_Thuoc = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_Thuoc);
            }
        }

        public bool mBCNhapXuatTon_YCu
        {
            get
            {
                return _mBCNhapXuatTon_YCu;
            }
            set
            {
                if (_mBCNhapXuatTon_YCu == value)
                    return;
                _mBCNhapXuatTon_YCu = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_YCu);
            }
        }

        public bool mBCNhapXuatTon_HoaChat
        {
            get
            {
                return _mBCNhapXuatTon_HoaChat;
            }
            set
            {
                if (_mBCNhapXuatTon_HoaChat == value)
                    return;
                _mBCNhapXuatTon_HoaChat = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_HoaChat);
            }
        }

        
        public bool mBCTheKho_Thuoc
        {
            get
            {
                return _mBCTheKho_Thuoc;
            }
            set
            {
                if (_mBCTheKho_Thuoc == value)
                    return;
                _mBCTheKho_Thuoc = value;
                NotifyOfPropertyChange(() => mBCTheKho_Thuoc);
            }
        }
        
        public bool mBCTheKho_YCu
        {
            get
            {
                return _mBCTheKho_YCu;
            }
            set
            {
                if (_mBCTheKho_YCu == value)
                    return;
                _mBCTheKho_YCu = value;
                NotifyOfPropertyChange(() => mBCTheKho_YCu);
            }
        }

        public bool mBCTheKho_HoaChat
        {
            get
            {
                return _mBCTheKho_HoaChat;
            }
            set
            {
                if (_mBCTheKho_HoaChat == value)
                    return;
                _mBCTheKho_HoaChat = value;
                NotifyOfPropertyChange(() => mBCTheKho_HoaChat);
            }
        }

        
        public bool mDanhMuc_Thuoc
        {
            get
            {
                return _mDanhMuc_Thuoc;
            }
            set
            {
                if (_mDanhMuc_Thuoc == value)
                    return;
                _mDanhMuc_Thuoc = value;
                NotifyOfPropertyChange(() => mDanhMuc_Thuoc);
            }
        }
        
        public bool mDanhMuc_YCu
        {
            get
            {
                return _mDanhMuc_YCu;
            }
            set
            {
                if (_mDanhMuc_YCu == value)
                    return;
                _mDanhMuc_YCu = value;
                NotifyOfPropertyChange(() => mDanhMuc_YCu);
            }
        }
        
        public bool mDanhMuc_HoaChat
        {
            get
            {
                return _mDanhMuc_HoaChat;
            }
            set
            {
                if (_mDanhMuc_HoaChat == value)
                    return;
                _mDanhMuc_HoaChat = value;
                NotifyOfPropertyChange(() => mDanhMuc_HoaChat);
            }
        }

        
        public bool mGiaBan_Thuoc
        {
            get
            {
                return _mGiaBan_Thuoc;
            }
            set
            {
                if (_mGiaBan_Thuoc == value)
                    return;
                _mGiaBan_Thuoc = value;
                NotifyOfPropertyChange(() => mGiaBan_Thuoc);
            }
        }

        public bool mGiaBan_YCu
        {
            get
            {
                return _mGiaBan_YCu;
            }
            set
            {
                if (_mGiaBan_YCu == value)
                    return;
                _mGiaBan_YCu = value;
                NotifyOfPropertyChange(() => mGiaBan_YCu);
            }
        }

        public bool mGiaBan_HoaChat
        {
            get
            {
                return _mGiaBan_HoaChat;
            }
            set
            {
                if (_mGiaBan_HoaChat == value)
                    return;
                _mGiaBan_HoaChat = value;
                NotifyOfPropertyChange(() => mGiaBan_HoaChat);
            }
        }


        public bool mXuatTraHang_Thuoc
        {
            get
            {
                return _mXuatTraHang_Thuoc;
            }
            set
            {
                if (_mXuatTraHang_Thuoc == value)
                    return;
                _mXuatTraHang_Thuoc = value;
                NotifyOfPropertyChange(() => mXuatTraHang_Thuoc);
            }
        }
        
        public bool mXuatTraHang_YCu
        {
            get
            {
                return _mXuatTraHang_YCu;
            }
            set
            {
                if (_mXuatTraHang_YCu == value)
                    return;
                _mXuatTraHang_YCu = value;
                NotifyOfPropertyChange(() => mXuatTraHang_YCu);
            }
        }

        public bool mXuatTraHang_HoaChat
        {
            get
            {
                return _mXuatTraHang_HoaChat;
            }
            set
            {
                if (_mXuatTraHang_HoaChat == value)
                    return;
                _mXuatTraHang_HoaChat = value;
                NotifyOfPropertyChange(() => mXuatTraHang_HoaChat);
            }
        }
        
        public bool mXuatChoBenhNhan_Thuoc
        {
            get
            {
                return _mXuatChoBenhNhan_Thuoc;
            }
            set
            {
                if (_mXuatChoBenhNhan_Thuoc == value)
                    return;
                _mXuatChoBenhNhan_Thuoc = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_Thuoc);
            }
        }
        
        public bool mXuatChoBenhNhan_YCu
        {
            get
            {
                return _mXuatChoBenhNhan_YCu;
            }
            set
            {
                if (_mXuatChoBenhNhan_YCu == value)
                    return;
                _mXuatChoBenhNhan_YCu = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_YCu);
            }
        }

        public bool mXuatChoBenhNhan_HoaChat
        {
            get
            {
                return _mXuatChoBenhNhan_HoaChat;
            }
            set
            {
                if (_mXuatChoBenhNhan_HoaChat == value)
                    return;
                _mXuatChoBenhNhan_HoaChat = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_HoaChat);
            }
        }
        #endregion

        #region operation

        private bool _mPhieuYeuCau_Thuoc_Tim = true;
        private bool _mPhieuYeuCau_Thuoc_Them = true;
        private bool _mPhieuYeuCau_Thuoc_Xoa = true;
        private bool _mPhieuYeuCau_Thuoc_XemIn = true;
        private bool _mPhieuYeuCau_Thuoc_In = true;

        private bool _mPhieuYeuCau_YCu_Tim = true;
        private bool _mPhieuYeuCau_YCu_Them = true;
        private bool _mPhieuYeuCau_YCu_Xoa = true;
        private bool _mPhieuYeuCau_YCu_XemIn = true;
        private bool _mPhieuYeuCau_YCu_In = true;

        private bool _mPhieuYeuCau_HoaChat_Tim = true;
        private bool _mPhieuYeuCau_HoaChat_Them = true;
        private bool _mPhieuYeuCau_HoaChat_Xoa = true;
        private bool _mPhieuYeuCau_HoaChat_XemIn = true;
        private bool _mPhieuYeuCau_HoaChat_In = true;

        private bool _mNhapHangTuNCC_Thuoc_Tim = true;
        private bool _mNhapHangTuNCC_Thuoc_Them = true;
        private bool _mNhapHangTuNCC_Thuoc_XemIn = true;
        private bool _mNhapHangTuNCC_Thuoc_In = true;

        private bool _mNhapHangTuNCC_YCu_Tim = true;
        private bool _mNhapHangTuNCC_YCu_Them = true;
        private bool _mNhapHangTuNCC_YCu_XemIn = true;
        private bool _mNhapHangTuNCC_YCu_In = true;

        private bool _mNhapHangTuNCC_HoaChat_Tim = true;
        private bool _mNhapHangTuNCC_HoaChat_Them = true;
        private bool _mNhapHangTuNCC_HoaChat_XemIn = true;
        private bool _mNhapHangTuNCC_HoaChat_In = true;

        private bool _mNhapHangTuKhoDuoc_Thuoc_Tim = true;
        private bool _mNhapHangTuKhoDuoc_Thuoc_Them = true;
        private bool _mNhapHangTuKhoDuoc_Thuoc_XemIn = true;
        private bool _mNhapHangTuKhoDuoc_Thuoc_In = true;

        private bool _mNhapHangTuKhoDuoc_YCu_Tim = true;
        private bool _mNhapHangTuKhoDuoc_YCu_Them = true;
        private bool _mNhapHangTuKhoDuoc_YCu_XemIn = true;
        private bool _mNhapHangTuKhoDuoc_YCu_In = true;

        private bool _mNhapHangTuKhoDuoc_HoaChat_Tim = true;
        private bool _mNhapHangTuKhoDuoc_HoaChat_Them = true;
        private bool _mNhapHangTuKhoDuoc_HoaChat_XemIn = true;
        private bool _mNhapHangTuKhoDuoc_HoaChat_In = true;

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

        private bool _mThongKe_Thuoc_xem = true;
        private bool _mThongKe_Thuoc_PhieuMoi = true;
        private bool _mThongKe_Thuoc_XemIn = true;
        private bool _mThongKe_Thuoc_In = true;

        private bool _mThongKe_YCu_xem = true;
        private bool _mThongKe_YCu_PhieuMoi = true;
        private bool _mThongKe_YCu_XemIn = true;
        private bool _mThongKe_YCu_In = true;

        private bool _mThongKe_HoaChat_xem = true;
        private bool _mThongKe_HoaChat_PhieuMoi = true;
        private bool _mThongKe_HoaChat_XemIn = true;
        private bool _mThongKe_HoaChat_In = true;

        private bool _mBaoCaoXuat_Thuoc_XemIn = true;
        private bool _mBaoCaoXuat_YCu_XemIn = true;
        private bool _mBaoCaoXuat_HoaChat_XemIn = true;

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

        private bool _mQLDanhMuc_Thuoc_Tim = true;
        private bool _mQLDanhMuc_YCu_Tim = true;
        private bool _mQLDanhMuc_HoaChat_Tim = true;

        private bool _mGiaBan_Thuoc_Tim = true;
        private bool _mGiaBan_Thuoc_XemDSGia = true;

        private bool _mGiaBan_YCu_Tim = true;
        private bool _mGiaBan_YCu_XemDSGia = true;

        private bool _mGiaBan_HoaChat_Tim = true;
        private bool _mGiaBan_HoaChat_XemDSGia = true;

        private bool _mXuatTraHang_Thuoc_Xem = true;
        private bool _mXuatTraHang_Thuoc_PhieuMoi = true;
        private bool _mXuatTraHang_Thuoc_XemIn = true;
        private bool _mXuatTraHang_Thuoc_In = true;

        private bool _mXuatTraHang_YCu_Xem = true;
        private bool _mXuatTraHang_YCu_PhieuMoi = true;
        private bool _mXuatTraHang_YCu_XemIn = true;
        private bool _mXuatTraHang_YCu_In = true;

        private bool _mXuatTraHang_HoaChat_Xem = true;
        private bool _mXuatTraHang_HoaChat_PhieuMoi = true;
        private bool _mXuatTraHang_HoaChat_XemIn = true;
        private bool _mXuatTraHang_HoaChat_In = true;

        private bool _mXuatChoBenhNhan_Thuoc_Xem = true;
        private bool _mXuatChoBenhNhan_Thuoc_PhieuMoi = true;
        private bool _mXuatChoBenhNhan_Thuoc_XemIn = true;
        private bool _mXuatChoBenhNhan_Thuoc_In = true;

        private bool _mXuatChoBenhNhan_YCu_Xem = true;
        private bool _mXuatChoBenhNhan_YCu_PhieuMoi = true;
        private bool _mXuatChoBenhNhan_YCu_XemIn = true;
        private bool _mXuatChoBenhNhan_YCu_In = true;

        private bool _mXuatChoBenhNhan_HoaChat_Xem = true;
        private bool _mXuatChoBenhNhan_HoaChat_PhieuMoi = true;
        private bool _mXuatChoBenhNhan_HoaChat_XemIn = true;
        private bool _mXuatChoBenhNhan_HoaChat_In = true;

        private bool _mKiemKe_VTYTTH_Tim = true;
        private bool _mKiemKe_VTYTTH_ThemMoi = true;
        private bool _mKiemKe_VTYTTH_XuatExcel = true;
        private bool _mKiemKe_VTYTTH_XemIn = true;

        private bool _mKiemKe_Vaccine_Tim = true;
        private bool _mKiemKe_Vaccine_ThemMoi = true;
        private bool _mKiemKe_Vaccine_XuatExcel = true;
        private bool _mKiemKe_Vaccine_XemIn = true;

        private bool _mKiemKe_Blood_Tim = true;
        private bool _mKiemKe_Blood_ThemMoi = true;
        private bool _mKiemKe_Blood_XuatExcel = true;
        private bool _mKiemKe_Blood_XemIn = true;

        private bool _mKiemKe_VTTH_Tim = true;
        private bool _mKiemKe_VTTH_ThemMoi = true;
        private bool _mKiemKe_VTTH_XuatExcel = true;
        private bool _mKiemKe_VTTH_XemIn = true;

        private bool _mKiemKe_VPP_Tim = true;
        private bool _mKiemKe_VPP_ThemMoi = true;
        private bool _mKiemKe_VPP_XuatExcel = true;
        private bool _mKiemKe_VPP_XemIn = true;

        private bool _mKiemKe_ThanhTrung_Tim = true;
        private bool _mKiemKe_ThanhTrung_ThemMoi = true;
        private bool _mKiemKe_ThanhTrung_XuatExcel = true;
        private bool _mKiemKe_ThanhTrung_XemIn = true;

        public bool mPhieuYeuCau_Thuoc_Tim
        {
            get
            {
                return _mPhieuYeuCau_Thuoc_Tim;
            }
            set
            {
                if (_mPhieuYeuCau_Thuoc_Tim == value)
                    return;
                _mPhieuYeuCau_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Thuoc_Tim);
            }
        }
        
        public bool mPhieuYeuCau_Thuoc_Them
        {
            get
            {
                return _mPhieuYeuCau_Thuoc_Them;
            }
            set
            {
                if (_mPhieuYeuCau_Thuoc_Them == value)
                    return;
                _mPhieuYeuCau_Thuoc_Them = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Thuoc_Them);
            }
        }
        
        public bool mPhieuYeuCau_Thuoc_Xoa
        {
            get
            {
                return _mPhieuYeuCau_Thuoc_Xoa;
            }
            set
            {
                if (_mPhieuYeuCau_Thuoc_Xoa == value)
                    return;
                _mPhieuYeuCau_Thuoc_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Thuoc_Xoa);
            }
        }
        
        public bool mPhieuYeuCau_Thuoc_XemIn
        {
            get
            {
                return _mPhieuYeuCau_Thuoc_XemIn;
            }
            set
            {
                if (_mPhieuYeuCau_Thuoc_XemIn == value)
                    return;
                _mPhieuYeuCau_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Thuoc_XemIn);
            }
        }
        
        public bool mPhieuYeuCau_Thuoc_In
        {
            get
            {
                return _mPhieuYeuCau_Thuoc_In;
            }
            set
            {
                if (_mPhieuYeuCau_Thuoc_In == value)
                    return;
                _mPhieuYeuCau_Thuoc_In = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Thuoc_In);
            }
        }




        public bool mPhieuYeuCau_YCu_Tim
        {
            get
            {
                return _mPhieuYeuCau_YCu_Tim;
            }
            set
            {
                if (_mPhieuYeuCau_YCu_Tim == value)
                    return;
                _mPhieuYeuCau_YCu_Tim = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_YCu_Tim);
            }
        }


        public bool mPhieuYeuCau_YCu_Them
        {
            get
            {
                return _mPhieuYeuCau_YCu_Them;
            }
            set
            {
                if (_mPhieuYeuCau_YCu_Them == value)
                    return;
                _mPhieuYeuCau_YCu_Them = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_YCu_Them);
            }
        }


        public bool mPhieuYeuCau_YCu_Xoa
        {
            get
            {
                return _mPhieuYeuCau_YCu_Xoa;
            }
            set
            {
                if (_mPhieuYeuCau_YCu_Xoa == value)
                    return;
                _mPhieuYeuCau_YCu_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_YCu_Xoa);
            }
        }


        public bool mPhieuYeuCau_YCu_XemIn
        {
            get
            {
                return _mPhieuYeuCau_YCu_XemIn;
            }
            set
            {
                if (_mPhieuYeuCau_YCu_XemIn == value)
                    return;
                _mPhieuYeuCau_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_YCu_XemIn);
            }
        }


        public bool mPhieuYeuCau_YCu_In
        {
            get
            {
                return _mPhieuYeuCau_YCu_In;
            }
            set
            {
                if (_mPhieuYeuCau_YCu_In == value)
                    return;
                _mPhieuYeuCau_YCu_In = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_YCu_In);
            }
        }




        public bool mPhieuYeuCau_HoaChat_Tim
        {
            get
            {
                return _mPhieuYeuCau_HoaChat_Tim;
            }
            set
            {
                if (_mPhieuYeuCau_HoaChat_Tim == value)
                    return;
                _mPhieuYeuCau_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_HoaChat_Tim);
            }
        }


        public bool mPhieuYeuCau_HoaChat_Them
        {
            get
            {
                return _mPhieuYeuCau_HoaChat_Them;
            }
            set
            {
                if (_mPhieuYeuCau_HoaChat_Them == value)
                    return;
                _mPhieuYeuCau_HoaChat_Them = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_HoaChat_Them);
            }
        }


        public bool mPhieuYeuCau_HoaChat_Xoa
        {
            get
            {
                return _mPhieuYeuCau_HoaChat_Xoa;
            }
            set
            {
                if (_mPhieuYeuCau_HoaChat_Xoa == value)
                    return;
                _mPhieuYeuCau_HoaChat_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_HoaChat_Xoa);
            }
        }


        public bool mPhieuYeuCau_HoaChat_XemIn
        {
            get
            {
                return _mPhieuYeuCau_HoaChat_XemIn;
            }
            set
            {
                if (_mPhieuYeuCau_HoaChat_XemIn == value)
                    return;
                _mPhieuYeuCau_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_HoaChat_XemIn);
            }
        }


        public bool mPhieuYeuCau_HoaChat_In
        {
            get
            {
                return _mPhieuYeuCau_HoaChat_In;
            }
            set
            {
                if (_mPhieuYeuCau_HoaChat_In == value)
                    return;
                _mPhieuYeuCau_HoaChat_In = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_HoaChat_In);
            }
        }




        public bool mNhapHangTuNCC_Thuoc_Tim
        {
            get
            {
                return _mNhapHangTuNCC_Thuoc_Tim;
            }
            set
            {
                if (_mNhapHangTuNCC_Thuoc_Tim == value)
                    return;
                _mNhapHangTuNCC_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_Thuoc_Tim);
            }
        }


        public bool mNhapHangTuNCC_Thuoc_Them
        {
            get
            {
                return _mNhapHangTuNCC_Thuoc_Them;
            }
            set
            {
                if (_mNhapHangTuNCC_Thuoc_Them == value)
                    return;
                _mNhapHangTuNCC_Thuoc_Them = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_Thuoc_Them);
            }
        }


        public bool mNhapHangTuNCC_Thuoc_XemIn
        {
            get
            {
                return _mNhapHangTuNCC_Thuoc_XemIn;
            }
            set
            {
                if (_mNhapHangTuNCC_Thuoc_XemIn == value)
                    return;
                _mNhapHangTuNCC_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_Thuoc_XemIn);
            }
        }


        public bool mNhapHangTuNCC_Thuoc_In
        {
            get
            {
                return _mNhapHangTuNCC_Thuoc_In;
            }
            set
            {
                if (_mNhapHangTuNCC_Thuoc_In == value)
                    return;
                _mNhapHangTuNCC_Thuoc_In = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_Thuoc_In);
            }
        }




        public bool mNhapHangTuNCC_YCu_Tim
        {
            get
            {
                return _mNhapHangTuNCC_YCu_Tim;
            }
            set
            {
                if (_mNhapHangTuNCC_YCu_Tim == value)
                    return;
                _mNhapHangTuNCC_YCu_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_YCu_Tim);
            }
        }


        public bool mNhapHangTuNCC_YCu_Them
        {
            get
            {
                return _mNhapHangTuNCC_YCu_Them;
            }
            set
            {
                if (_mNhapHangTuNCC_YCu_Them == value)
                    return;
                _mNhapHangTuNCC_YCu_Them = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_YCu_Them);
            }
        }


        public bool mNhapHangTuNCC_YCu_XemIn
        {
            get
            {
                return _mNhapHangTuNCC_YCu_XemIn;
            }
            set
            {
                if (_mNhapHangTuNCC_YCu_XemIn == value)
                    return;
                _mNhapHangTuNCC_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_YCu_XemIn);
            }
        }


        public bool mNhapHangTuNCC_YCu_In
        {
            get
            {
                return _mNhapHangTuNCC_YCu_In;
            }
            set
            {
                if (_mNhapHangTuNCC_YCu_In == value)
                    return;
                _mNhapHangTuNCC_YCu_In = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_YCu_In);
            }
        }




        public bool mNhapHangTuNCC_HoaChat_Tim
        {
            get
            {
                return _mNhapHangTuNCC_HoaChat_Tim;
            }
            set
            {
                if (_mNhapHangTuNCC_HoaChat_Tim == value)
                    return;
                _mNhapHangTuNCC_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_HoaChat_Tim);
            }
        }


        public bool mNhapHangTuNCC_HoaChat_Them
        {
            get
            {
                return _mNhapHangTuNCC_HoaChat_Them;
            }
            set
            {
                if (_mNhapHangTuNCC_HoaChat_Them == value)
                    return;
                _mNhapHangTuNCC_HoaChat_Them = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_HoaChat_Them);
            }
        }


        public bool mNhapHangTuNCC_HoaChat_XemIn
        {
            get
            {
                return _mNhapHangTuNCC_HoaChat_XemIn;
            }
            set
            {
                if (_mNhapHangTuNCC_HoaChat_XemIn == value)
                    return;
                _mNhapHangTuNCC_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_HoaChat_XemIn);
            }
        }


        public bool mNhapHangTuNCC_HoaChat_In
        {
            get
            {
                return _mNhapHangTuNCC_HoaChat_In;
            }
            set
            {
                if (_mNhapHangTuNCC_HoaChat_In == value)
                    return;
                _mNhapHangTuNCC_HoaChat_In = value;
                NotifyOfPropertyChange(() => mNhapHangTuNCC_HoaChat_In);
            }
        }




        public bool mNhapHangTuKhoDuoc_Thuoc_Tim
        {
            get
            {
                return _mNhapHangTuKhoDuoc_Thuoc_Tim;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_Thuoc_Tim == value)
                    return;
                _mNhapHangTuKhoDuoc_Thuoc_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_Thuoc_Tim);
            }
        }

        public bool mNhapHangTuKhoDuoc_Thuoc_Them
        {
            get
            {
                return _mNhapHangTuKhoDuoc_Thuoc_Them;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_Thuoc_Them == value)
                    return;
                _mNhapHangTuKhoDuoc_Thuoc_Them = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_Thuoc_Them);
            }
        }

        public bool mNhapHangTuKhoDuoc_Thuoc_XemIn
        {
            get
            {
                return _mNhapHangTuKhoDuoc_Thuoc_XemIn;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_Thuoc_XemIn == value)
                    return;
                _mNhapHangTuKhoDuoc_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_Thuoc_XemIn);
            }
        }

        public bool mNhapHangTuKhoDuoc_Thuoc_In
        {
            get
            {
                return _mNhapHangTuKhoDuoc_Thuoc_In;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_Thuoc_In == value)
                    return;
                _mNhapHangTuKhoDuoc_Thuoc_In = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_Thuoc_In);
            }
        }




        public bool mNhapHangTuKhoDuoc_YCu_Tim
        {
            get
            {
                return _mNhapHangTuKhoDuoc_YCu_Tim;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_YCu_Tim == value)
                    return;
                _mNhapHangTuKhoDuoc_YCu_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_YCu_Tim);
            }
        }


        public bool mNhapHangTuKhoDuoc_YCu_Them
        {
            get
            {
                return _mNhapHangTuKhoDuoc_YCu_Them;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_YCu_Them == value)
                    return;
                _mNhapHangTuKhoDuoc_YCu_Them = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_YCu_Them);
            }
        }


        public bool mNhapHangTuKhoDuoc_YCu_XemIn
        {
            get
            {
                return _mNhapHangTuKhoDuoc_YCu_XemIn;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_YCu_XemIn == value)
                    return;
                _mNhapHangTuKhoDuoc_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_YCu_XemIn);
            }
        }


        public bool mNhapHangTuKhoDuoc_YCu_In
        {
            get
            {
                return _mNhapHangTuKhoDuoc_YCu_In;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_YCu_In == value)
                    return;
                _mNhapHangTuKhoDuoc_YCu_In = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_YCu_In);
            }
        }




        public bool mNhapHangTuKhoDuoc_HoaChat_Tim
        {
            get
            {
                return _mNhapHangTuKhoDuoc_HoaChat_Tim;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_HoaChat_Tim == value)
                    return;
                _mNhapHangTuKhoDuoc_HoaChat_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_HoaChat_Tim);
            }
        }


        public bool mNhapHangTuKhoDuoc_HoaChat_Them
        {
            get
            {
                return _mNhapHangTuKhoDuoc_HoaChat_Them;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_HoaChat_Them == value)
                    return;
                _mNhapHangTuKhoDuoc_HoaChat_Them = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_HoaChat_Them);
            }
        }


        public bool mNhapHangTuKhoDuoc_HoaChat_XemIn
        {
            get
            {
                return _mNhapHangTuKhoDuoc_HoaChat_XemIn;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_HoaChat_XemIn == value)
                    return;
                _mNhapHangTuKhoDuoc_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_HoaChat_XemIn);
            }
        }


        public bool mNhapHangTuKhoDuoc_HoaChat_In
        {
            get
            {
                return _mNhapHangTuKhoDuoc_HoaChat_In;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_HoaChat_In == value)
                    return;
                _mNhapHangTuKhoDuoc_HoaChat_In = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_HoaChat_In);
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




        public bool mThongKe_Thuoc_xem
        {
            get
            {
                return _mThongKe_Thuoc_xem;
            }
            set
            {
                if (_mThongKe_Thuoc_xem == value)
                    return;
                _mThongKe_Thuoc_xem = value;
                NotifyOfPropertyChange(() => mThongKe_Thuoc_xem);
            }
        }


        public bool mThongKe_Thuoc_PhieuMoi
        {
            get
            {
                return _mThongKe_Thuoc_PhieuMoi;
            }
            set
            {
                if (_mThongKe_Thuoc_PhieuMoi == value)
                    return;
                _mThongKe_Thuoc_PhieuMoi = value;
                NotifyOfPropertyChange(() => mThongKe_Thuoc_PhieuMoi);
            }
        }


        public bool mThongKe_Thuoc_XemIn
        {
            get
            {
                return _mThongKe_Thuoc_XemIn;
            }
            set
            {
                if (_mThongKe_Thuoc_XemIn == value)
                    return;
                _mThongKe_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mThongKe_Thuoc_XemIn);
            }
        }


        public bool mThongKe_Thuoc_In
        {
            get
            {
                return _mThongKe_Thuoc_In;
            }
            set
            {
                if (_mThongKe_Thuoc_In == value)
                    return;
                _mThongKe_Thuoc_In = value;
                NotifyOfPropertyChange(() => mThongKe_Thuoc_In);
            }
        }




        public bool mThongKe_YCu_xem
        {
            get
            {
                return _mThongKe_YCu_xem;
            }
            set
            {
                if (_mThongKe_YCu_xem == value)
                    return;
                _mThongKe_YCu_xem = value;
                NotifyOfPropertyChange(() => mThongKe_YCu_xem);
            }
        }


        public bool mThongKe_YCu_PhieuMoi
        {
            get
            {
                return _mThongKe_YCu_PhieuMoi;
            }
            set
            {
                if (_mThongKe_YCu_PhieuMoi == value)
                    return;
                _mThongKe_YCu_PhieuMoi = value;
                NotifyOfPropertyChange(() => mThongKe_YCu_PhieuMoi);
            }
        }


        public bool mThongKe_YCu_XemIn
        {
            get
            {
                return _mThongKe_YCu_XemIn;
            }
            set
            {
                if (_mThongKe_YCu_XemIn == value)
                    return;
                _mThongKe_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mThongKe_YCu_XemIn);
            }
        }


        public bool mThongKe_YCu_In
        {
            get
            {
                return _mThongKe_YCu_In;
            }
            set
            {
                if (_mThongKe_YCu_In == value)
                    return;
                _mThongKe_YCu_In = value;
                NotifyOfPropertyChange(() => mThongKe_YCu_In);
            }
        }




        public bool mThongKe_HoaChat_xem
        {
            get
            {
                return _mThongKe_HoaChat_xem;
            }
            set
            {
                if (_mThongKe_HoaChat_xem == value)
                    return;
                _mThongKe_HoaChat_xem = value;
                NotifyOfPropertyChange(() => mThongKe_HoaChat_xem);
            }
        }

        public bool mThongKe_HoaChat_PhieuMoi
        {
            get
            {
                return _mThongKe_HoaChat_PhieuMoi;
            }
            set
            {
                if (_mThongKe_HoaChat_PhieuMoi == value)
                    return;
                _mThongKe_HoaChat_PhieuMoi = value;
                NotifyOfPropertyChange(() => mThongKe_HoaChat_PhieuMoi);
            }
        }

        public bool mThongKe_HoaChat_XemIn
        {
            get
            {
                return _mThongKe_HoaChat_XemIn;
            }
            set
            {
                if (_mThongKe_HoaChat_XemIn == value)
                    return;
                _mThongKe_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mThongKe_HoaChat_XemIn);
            }
        }

        public bool mThongKe_HoaChat_In
        {
            get
            {
                return _mThongKe_HoaChat_In;
            }
            set
            {
                if (_mThongKe_HoaChat_In == value)
                    return;
                _mThongKe_HoaChat_In = value;
                NotifyOfPropertyChange(() => mThongKe_HoaChat_In);
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




        public bool mXuatTraHang_Thuoc_Xem
        {
            get
            {
                return _mXuatTraHang_Thuoc_Xem;
            }
            set
            {
                if (_mXuatTraHang_Thuoc_Xem == value)
                    return;
                _mXuatTraHang_Thuoc_Xem = value;
                NotifyOfPropertyChange(() => mXuatTraHang_Thuoc_Xem);
            }
        }

        public bool mXuatTraHang_Thuoc_PhieuMoi
        {
            get
            {
                return _mXuatTraHang_Thuoc_PhieuMoi;
            }
            set
            {
                if (_mXuatTraHang_Thuoc_PhieuMoi == value)
                    return;
                _mXuatTraHang_Thuoc_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatTraHang_Thuoc_PhieuMoi);
            }
        }

        public bool mXuatTraHang_Thuoc_XemIn
        {
            get
            {
                return _mXuatTraHang_Thuoc_XemIn;
            }
            set
            {
                if (_mXuatTraHang_Thuoc_XemIn == value)
                    return;
                _mXuatTraHang_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mXuatTraHang_Thuoc_XemIn);
            }
        }

        public bool mXuatTraHang_Thuoc_In
        {
            get
            {
                return _mXuatTraHang_Thuoc_In;
            }
            set
            {
                if (_mXuatTraHang_Thuoc_In == value)
                    return;
                _mXuatTraHang_Thuoc_In = value;
                NotifyOfPropertyChange(() => mXuatTraHang_Thuoc_In);
            }
        }


        public bool mXuatTraHang_YCu_Xem
        {
            get
            {
                return _mXuatTraHang_YCu_Xem;
            }
            set
            {
                if (_mXuatTraHang_YCu_Xem == value)
                    return;
                _mXuatTraHang_YCu_Xem = value;
                NotifyOfPropertyChange(() => mXuatTraHang_YCu_Xem);
            }
        }


        public bool mXuatTraHang_YCu_PhieuMoi
        {
            get
            {
                return _mXuatTraHang_YCu_PhieuMoi;
            }
            set
            {
                if (_mXuatTraHang_YCu_PhieuMoi == value)
                    return;
                _mXuatTraHang_YCu_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatTraHang_YCu_PhieuMoi);
            }
        }


        public bool mXuatTraHang_YCu_XemIn
        {
            get
            {
                return _mXuatTraHang_YCu_XemIn;
            }
            set
            {
                if (_mXuatTraHang_YCu_XemIn == value)
                    return;
                _mXuatTraHang_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mXuatTraHang_YCu_XemIn);
            }
        }


        public bool mXuatTraHang_YCu_In
        {
            get
            {
                return _mXuatTraHang_YCu_In;
            }
            set
            {
                if (_mXuatTraHang_YCu_In == value)
                    return;
                _mXuatTraHang_YCu_In = value;
                NotifyOfPropertyChange(() => mXuatTraHang_YCu_In);
            }
        }




        public bool mXuatTraHang_HoaChat_Xem
        {
            get
            {
                return _mXuatTraHang_HoaChat_Xem;
            }
            set
            {
                if (_mXuatTraHang_HoaChat_Xem == value)
                    return;
                _mXuatTraHang_HoaChat_Xem = value;
                NotifyOfPropertyChange(() => mXuatTraHang_HoaChat_Xem);
            }
        }


        public bool mXuatTraHang_HoaChat_PhieuMoi
        {
            get
            {
                return _mXuatTraHang_HoaChat_PhieuMoi;
            }
            set
            {
                if (_mXuatTraHang_HoaChat_PhieuMoi == value)
                    return;
                _mXuatTraHang_HoaChat_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatTraHang_HoaChat_PhieuMoi);
            }
        }


        public bool mXuatTraHang_HoaChat_XemIn
        {
            get
            {
                return _mXuatTraHang_HoaChat_XemIn;
            }
            set
            {
                if (_mXuatTraHang_HoaChat_XemIn == value)
                    return;
                _mXuatTraHang_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mXuatTraHang_HoaChat_XemIn);
            }
        }


        public bool mXuatTraHang_HoaChat_In
        {
            get
            {
                return _mXuatTraHang_HoaChat_In;
            }
            set
            {
                if (_mXuatTraHang_HoaChat_In == value)
                    return;
                _mXuatTraHang_HoaChat_In = value;
                NotifyOfPropertyChange(() => mXuatTraHang_HoaChat_In);
            }
        }




        public bool mXuatChoBenhNhan_Thuoc_Xem
        {
            get
            {
                return _mXuatChoBenhNhan_Thuoc_Xem;
            }
            set
            {
                if (_mXuatChoBenhNhan_Thuoc_Xem == value)
                    return;
                _mXuatChoBenhNhan_Thuoc_Xem = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_Thuoc_Xem);
            }
        }

        public bool mXuatChoBenhNhan_Thuoc_PhieuMoi
        {
            get
            {
                return _mXuatChoBenhNhan_Thuoc_PhieuMoi;
            }
            set
            {
                if (_mXuatChoBenhNhan_Thuoc_PhieuMoi == value)
                    return;
                _mXuatChoBenhNhan_Thuoc_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_Thuoc_PhieuMoi);
            }
        }

        public bool mXuatChoBenhNhan_Thuoc_XemIn
        {
            get
            {
                return _mXuatChoBenhNhan_Thuoc_XemIn;
            }
            set
            {
                if (_mXuatChoBenhNhan_Thuoc_XemIn == value)
                    return;
                _mXuatChoBenhNhan_Thuoc_XemIn = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_Thuoc_XemIn);
            }
        }

        public bool mXuatChoBenhNhan_Thuoc_In
        {
            get
            {
                return _mXuatChoBenhNhan_Thuoc_In;
            }
            set
            {
                if (_mXuatChoBenhNhan_Thuoc_In == value)
                    return;
                _mXuatChoBenhNhan_Thuoc_In = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_Thuoc_In);
            }
        }




        public bool mXuatChoBenhNhan_YCu_Xem
        {
            get
            {
                return _mXuatChoBenhNhan_YCu_Xem;
            }
            set
            {
                if (_mXuatChoBenhNhan_YCu_Xem == value)
                    return;
                _mXuatChoBenhNhan_YCu_Xem = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_YCu_Xem);
            }
        }


        public bool mXuatChoBenhNhan_YCu_PhieuMoi
        {
            get
            {
                return _mXuatChoBenhNhan_YCu_PhieuMoi;
            }
            set
            {
                if (_mXuatChoBenhNhan_YCu_PhieuMoi == value)
                    return;
                _mXuatChoBenhNhan_YCu_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_YCu_PhieuMoi);
            }
        }


        public bool mXuatChoBenhNhan_YCu_XemIn
        {
            get
            {
                return _mXuatChoBenhNhan_YCu_XemIn;
            }
            set
            {
                if (_mXuatChoBenhNhan_YCu_XemIn == value)
                    return;
                _mXuatChoBenhNhan_YCu_XemIn = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_YCu_XemIn);
            }
        }


        public bool mXuatChoBenhNhan_YCu_In
        {
            get
            {
                return _mXuatChoBenhNhan_YCu_In;
            }
            set
            {
                if (_mXuatChoBenhNhan_YCu_In == value)
                    return;
                _mXuatChoBenhNhan_YCu_In = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_YCu_In);
            }
        }




        public bool mXuatChoBenhNhan_HoaChat_Xem
        {
            get
            {
                return _mXuatChoBenhNhan_HoaChat_Xem;
            }
            set
            {
                if (_mXuatChoBenhNhan_HoaChat_Xem == value)
                    return;
                _mXuatChoBenhNhan_HoaChat_Xem = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_HoaChat_Xem);
            }
        }


        public bool mXuatChoBenhNhan_HoaChat_PhieuMoi
        {
            get
            {
                return _mXuatChoBenhNhan_HoaChat_PhieuMoi;
            }
            set
            {
                if (_mXuatChoBenhNhan_HoaChat_PhieuMoi == value)
                    return;
                _mXuatChoBenhNhan_HoaChat_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_HoaChat_PhieuMoi);
            }
        }


        public bool mXuatChoBenhNhan_HoaChat_XemIn
        {
            get
            {
                return _mXuatChoBenhNhan_HoaChat_XemIn;
            }
            set
            {
                if (_mXuatChoBenhNhan_HoaChat_XemIn == value)
                    return;
                _mXuatChoBenhNhan_HoaChat_XemIn = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_HoaChat_XemIn);
            }
        }


        public bool mXuatChoBenhNhan_HoaChat_In
        {
            get
            {
                return _mXuatChoBenhNhan_HoaChat_In;
            }
            set
            {
                if (_mXuatChoBenhNhan_HoaChat_In == value)
                    return;
                _mXuatChoBenhNhan_HoaChat_In = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_HoaChat_In);
            }
        }

        //VTYTTH
        public bool mKiemKe_VTYTTH_Tim
        {
            get
            {
                return _mKiemKe_VTYTTH_Tim;
            }
            set
            {
                if (_mKiemKe_VTYTTH_Tim == value)
                    return;
                _mKiemKe_VTYTTH_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_VTYTTH_Tim);
            }
        }


        public bool mKiemKe_VTYTTH_ThemMoi
        {
            get
            {
                return _mKiemKe_VTYTTH_ThemMoi;
            }
            set
            {
                if (_mKiemKe_VTYTTH_ThemMoi == value)
                    return;
                _mKiemKe_VTYTTH_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_VTYTTH_ThemMoi);
            }
        }


        public bool mKiemKe_VTYTTH_XuatExcel
        {
            get
            {
                return _mKiemKe_VTYTTH_XuatExcel;
            }
            set
            {
                if (_mKiemKe_VTYTTH_XuatExcel == value)
                    return;
                _mKiemKe_VTYTTH_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_VTYTTH_XuatExcel);
            }
        }


        public bool mKiemKe_VTYTTH_XemIn
        {
            get
            {
                return _mKiemKe_VTYTTH_XemIn;
            }
            set
            {
                if (_mKiemKe_VTYTTH_XemIn == value)
                    return;
                _mKiemKe_VTYTTH_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_VTYTTH_XemIn);
            }
        }

        //VACCINE
        public bool mKiemKe_Vaccine_Tim
        {
            get
            {
                return _mKiemKe_Vaccine_Tim;
            }
            set
            {
                if (_mKiemKe_Vaccine_Tim == value)
                    return;
                _mKiemKe_Vaccine_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_Vaccine_Tim);
            }
        }


        public bool mKiemKe_Vaccine_ThemMoi
        {
            get
            {
                return _mKiemKe_Vaccine_ThemMoi;
            }
            set
            {
                if (_mKiemKe_Vaccine_ThemMoi == value)
                    return;
                _mKiemKe_Vaccine_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_Vaccine_ThemMoi);
            }
        }


        public bool mKiemKe_Vaccine_XuatExcel
        {
            get
            {
                return _mKiemKe_Vaccine_XuatExcel;
            }
            set
            {
                if (_mKiemKe_Vaccine_XuatExcel == value)
                    return;
                _mKiemKe_Vaccine_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_Vaccine_XuatExcel);
            }
        }


        public bool mKiemKe_Vaccine_XemIn
        {
            get
            {
                return _mKiemKe_Vaccine_XemIn;
            }
            set
            {
                if (_mKiemKe_Vaccine_XemIn == value)
                    return;
                _mKiemKe_Vaccine_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_Vaccine_XemIn);
            }
        }

        //BLOOD
        public bool mKiemKe_Blood_Tim
        {
            get
            {
                return _mKiemKe_Blood_Tim;
            }
            set
            {
                if (_mKiemKe_Blood_Tim == value)
                    return;
                _mKiemKe_Blood_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_Blood_Tim);
            }
        }


        public bool mKiemKe_Blood_ThemMoi
        {
            get
            {
                return _mKiemKe_Blood_ThemMoi;
            }
            set
            {
                if (_mKiemKe_Blood_ThemMoi == value)
                    return;
                _mKiemKe_Blood_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_Blood_ThemMoi);
            }
        }


        public bool mKiemKe_Blood_XuatExcel
        {
            get
            {
                return _mKiemKe_Blood_XuatExcel;
            }
            set
            {
                if (_mKiemKe_Blood_XuatExcel == value)
                    return;
                _mKiemKe_Blood_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_Blood_XuatExcel);
            }
        }


        public bool mKiemKe_Blood_XemIn
        {
            get
            {
                return _mKiemKe_Blood_XemIn;
            }
            set
            {
                if (_mKiemKe_Blood_XemIn == value)
                    return;
                _mKiemKe_Blood_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_Blood_XemIn);
            }
        }

        //VTTH
        public bool mKiemKe_VTTH_Tim
        {
            get
            {
                return _mKiemKe_VTTH_Tim;
            }
            set
            {
                if (_mKiemKe_VTTH_Tim == value)
                    return;
                _mKiemKe_VTTH_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_VTTH_Tim);
            }
        }


        public bool mKiemKe_VTTH_ThemMoi
        {
            get
            {
                return _mKiemKe_VTTH_ThemMoi;
            }
            set
            {
                if (_mKiemKe_VTTH_ThemMoi == value)
                    return;
                _mKiemKe_VTTH_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_VTTH_ThemMoi);
            }
        }


        public bool mKiemKe_VTTH_XuatExcel
        {
            get
            {
                return _mKiemKe_VTTH_XuatExcel;
            }
            set
            {
                if (_mKiemKe_VTTH_XuatExcel == value)
                    return;
                _mKiemKe_VTTH_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_VTTH_XuatExcel);
            }
        }


        public bool mKiemKe_VTTH_XemIn
        {
            get
            {
                return _mKiemKe_VTTH_XemIn;
            }
            set
            {
                if (_mKiemKe_VTTH_XemIn == value)
                    return;
                _mKiemKe_VTTH_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_VTTH_XemIn);
            }
        }

        //VPP
        public bool mKiemKe_VPP_Tim
        {
            get
            {
                return _mKiemKe_VPP_Tim;
            }
            set
            {
                if (_mKiemKe_VPP_Tim == value)
                    return;
                _mKiemKe_VPP_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_VPP_Tim);
            }
        }


        public bool mKiemKe_VPP_ThemMoi
        {
            get
            {
                return _mKiemKe_VPP_ThemMoi;
            }
            set
            {
                if (_mKiemKe_VPP_ThemMoi == value)
                    return;
                _mKiemKe_VPP_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_VPP_ThemMoi);
            }
        }


        public bool mKiemKe_VPP_XuatExcel
        {
            get
            {
                return _mKiemKe_VPP_XuatExcel;
            }
            set
            {
                if (_mKiemKe_VPP_XuatExcel == value)
                    return;
                _mKiemKe_VPP_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_VPP_XuatExcel);
            }
        }


        public bool mKiemKe_VPP_XemIn
        {
            get
            {
                return _mKiemKe_VPP_XemIn;
            }
            set
            {
                if (_mKiemKe_VPP_XemIn == value)
                    return;
                _mKiemKe_VPP_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_VPP_XemIn);
            }
        }

        //THANH TRUNG
        public bool mKiemKe_ThanhTrung_Tim
        {
            get
            {
                return _mKiemKe_ThanhTrung_Tim;
            }
            set
            {
                if (_mKiemKe_ThanhTrung_Tim == value)
                    return;
                _mKiemKe_ThanhTrung_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_ThanhTrung_Tim);
            }
        }

        public bool mKiemKe_ThanhTrung_ThemMoi
        {
            get
            {
                return _mKiemKe_ThanhTrung_ThemMoi;
            }
            set
            {
                if (_mKiemKe_ThanhTrung_ThemMoi == value)
                    return;
                _mKiemKe_ThanhTrung_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_ThanhTrung_ThemMoi);
            }
        }


        public bool mKiemKe_ThanhTrung_XuatExcel
        {
            get
            {
                return _mKiemKe_ThanhTrung_XuatExcel;
            }
            set
            {
                if (_mKiemKe_ThanhTrung_XuatExcel == value)
                    return;
                _mKiemKe_ThanhTrung_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_ThanhTrung_XuatExcel);
            }
        }


        public bool mKiemKe_ThanhTrung_XemIn
        {
            get
            {
                return _mKiemKe_ThanhTrung_XemIn;
            }
            set
            {
                if (_mKiemKe_ThanhTrung_XemIn == value)
                    return;
                _mKiemKe_ThanhTrung_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_ThanhTrung_XemIn);
            }
        }

        #endregion

        #endregion

        #region Estimation - hien tai chua su dung member

        public void EstimationDrugDeptCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.strHienThi = eHCMSResources.K3922_G1_DuTruThuoc.ToUpper();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EstimationYCuCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = eHCMSResources.Z0737_G1_DuTruYCu;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void EstimationChemicalCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IEstimationDrugDept>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = eHCMSResources.Z0738_G1_DuTruHCHat;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        #endregion

        #region Mẫu phiếu xuất

        private void OutwardDrugTemplateCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IOutwardDrugClinicDeptTemplate>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.InitSelDeptCombo();
            UnitVM.strHienThi = Globals.TitleForm;

            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void OutwardDrugTemplateCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1110_G1_TaoMauXuatThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutwardDrugTemplateCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutwardDrugTemplateCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void OutwardMedicalMaterialTemplateCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IOutwardDrugClinicDeptTemplate>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.InitSelDeptCombo();
            UnitVM.strHienThi = Globals.TitleForm;

            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void OutwardMedicalMaterialTemplateCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1111_G1_TaoMauXuatYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutwardMedicalMaterialTemplateCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutwardMedicalMaterialTemplateCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void OutwardChemicalTemplateCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IOutwardDrugClinicDeptTemplate>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.InitSelDeptCombo();
            UnitVM.strHienThi = Globals.TitleForm;

            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void OutwardChemicalTemplateCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1112_G1_TaoMauXuatHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutwardChemicalTemplateCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutwardChemicalTemplateCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void OutwardGeneralTemplateCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IOutwardDrugClinicDeptTemplate>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Unknown;
            UnitVM.InitSelDeptCombo();
            UnitVM.strHienThi = Globals.TitleForm;
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }
        public void OutwardGeneralTemplateCmd()
        {
            Globals.TitleForm = eHCMSResources.T0794_G1_TaoMauXuat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutwardGeneralTemplateCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutwardGeneralTemplateCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RequestTemplateCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IOutwardDrugClinicDeptTemplate>();
            UnitVM.V_MedProductType = V_MedProductType;
            UnitVM.V_OutwardTemplateType = (long)AllLookupValues.V_OutwardTemplateType.RequestTemplate;
            UnitVM.InitSelDeptCombo();
            UnitVM.strHienThi = Globals.TitleForm;
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }
        public void RequestDrugTemplateCmd()
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2921_G1_TaoMauLinh0, eHCMSResources.G0787_G1_Thuoc);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestTemplateCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestTemplateCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void RequestMedicalMaterialTemplateCmd()
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2921_G1_TaoMauLinh0, eHCMSResources.G2907_G1_YCu);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestTemplateCmd_In((long)AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestTemplateCmd_In((long)AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Phieu Linh Thuoc Member
        private void RequestThuocCmdNew_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            //var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm>();
            var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm_V2>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.DoseVisibility = true;
            UnitVM.strHienThi = Globals.TitleForm;

            UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau_Thuoc_Tim;
            UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau_Thuoc_Them;
            UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau_Thuoc_Xoa;
            UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau_Thuoc_XemIn;
            UnitVM.mPhieuYeuCau_In = mPhieuYeuCau_Thuoc_In;

            UnitVM.UsedForRequestingDrug = true;

            //▼====== #002
            UnitVM.IsSearchByGenericName = true;
            UnitVM.vIsSearchByGenericName = true;
            //▲====== #002

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

        private void RequestYCuNewCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            //var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm>();
            var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm_V2>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau_YCu_Tim;
            UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau_YCu_Them;
            UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau_YCu_Xoa;
            UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau_YCu_XemIn;
            UnitVM.mPhieuYeuCau_In = mPhieuYeuCau_YCu_In;
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void RequestYCuNewCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1114_G1_LapPhLinhYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestYCuNewCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestYCuNewCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RequestHoaChatNewCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            //var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm>();
            var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm_V2>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau_HoaChat_Tim;
            UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau_HoaChat_Them;
            UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau_HoaChat_Xoa;
            UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau_HoaChat_XemIn;
            UnitVM.mPhieuYeuCau_In = mPhieuYeuCau_HoaChat_In;
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void RequestHoaChatNewCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1115_G1_LapPhLinhHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestHoaChatNewCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestHoaChatNewCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RequestCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm_V2>();
            UnitVM.V_MedProductType = V_MedProductType;
            UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau_HoaChat_Tim;
            //UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau_HoaChat_Them;
            //UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau_HoaChat_Xoa;
            //UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau_HoaChat_XemIn;
            //UnitVM.mPhieuYeuCau_In = mPhieuYeuCau_HoaChat_In;
            UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau;
            UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau;
            UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau;
            UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau;
            UnitVM.mPhieuYeuCau_In = mPhieuYeuCau;
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }
        public void RequestVTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2484_G1_LapPhLinhVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void RequestVaccinesCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2484_G1_LapPhLinhVacXin;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void RequestBloodsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2484_G1_LapPhLinhMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestCmd_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestCmd_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void RequestResourcesCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2547_G1_LapPhLinhVTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void RequestThanhTrungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2511_G1_LapPhLinhThanhTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void RequestVPPCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2536_G1_PhLinhVPP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void RequestTongHopCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm_V3>();
            UnitVM.strHienThi = Globals.TitleForm;
            UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau_HoaChat_Tim;
            UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau_HoaChat_Them;
            UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau_HoaChat_Xoa;
            UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau_HoaChat_XemIn;
            UnitVM.mPhieuYeuCau_In = mPhieuYeuCau_HoaChat_In;

            UnitVM.DoseVisibility = true;
            UnitVM.UsedForRequestingDrug = true;
            UnitVM.IsSearchByGenericName = true;
            UnitVM.vIsSearchByGenericName = true;

            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }
        public void RequestTongHopCmd()
        {
            Globals.TitleForm = "LẬP PHIẾU LĨNH TỔNG HỢP";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestTongHopCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestTongHopCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //▼====: #004
        private void RequestSuatAnCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm_Food>();
            UnitVM.strHienThi = Globals.TitleForm;
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void RequestSuatAnCmd()
        {
            Globals.TitleForm = "Lập phiếu báo ăn";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestSuatAnCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestSuatAnCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #004
        #endregion

        #region Phiếu lĩnh thuốc ngoại trú
        //▼====== #010
        private void OutRequestThuocCmdNew_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            //var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm>();
            var UnitVM = Globals.GetViewModel<IClinicDeptOutPtReqForm>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.DoseVisibility = true;
            UnitVM.strHienThi = Globals.TitleForm;

            UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau_Thuoc_Tim;
            UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau_Thuoc_Them;
            UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau_Thuoc_Xoa;
            UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau_Thuoc_XemIn;
            UnitVM.mPhieuYeuCau_In = mPhieuYeuCau_Thuoc_In;

            UnitVM.UsedForRequestingDrug = true;

            //▼====== #002
            UnitVM.IsSearchByGenericName = true;
            UnitVM.vIsSearchByGenericName = true;
            //▲====== #002

            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void OutRequestThuocNewCmd()
        {
            Globals.TitleForm = "Lập phiếu lĩnh hàng ngoại trú".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutRequestThuocCmdNew_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutRequestThuocCmdNew_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====== #010
        #endregion

        #region XNB - hien tai chua su dung
        public void XNBThuocCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            //UnitVM.InitializeInvoice();
            UnitVM.strHienThi = eHCMSResources.Z0739_G1_XuatThuocTuKhDuoc;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBYCuCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            //UnitVM.InitializeInvoice();
            UnitVM.strHienThi = eHCMSResources.Z0740_G1_XuatYCuTuKhDuoc;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void XNBHoaChatCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IXuatNoiBo>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            //UnitVM.InitializeInvoice();
            UnitVM.strHienThi = eHCMSResources.Z0741_G1_XuatHChatTuKhDuoc;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        #endregion

        #region Demage - hien tai chua su dung Member
        public void DemageThuocCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IDrugDeptDamageExpiryDrug>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            //UnitVM.InitializeInvoice();
            UnitVM.strHienThi = eHCMSResources.Z0745_G1_HuyThuocTaiKhDuoc;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DemageYCuCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IDrugDeptDamageExpiryDrug>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            //UnitVM.InitializeInvoice();
            UnitVM.strHienThi = eHCMSResources.Z0746_G1_HuyYCuTaiKhDuoc;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DemageHoaChatCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IDrugDeptDamageExpiryDrug>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            //UnitVM.InitializeInvoice();
            UnitVM.strHienThi = eHCMSResources.Z0747_G1_HuyHChatTaiKhDuoc;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        #endregion

        #region hien tai chua su dung

        public void SupplierProductCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<ISupplierProduct>();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void SupplierAndProductCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<ISupplierAndProduct>();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DrugDeptPharmaceulCompanyCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IDrugDeptPharmacieucalCompany>();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void DrugDeptPharmaceulCompanySupplierCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IDrugDeptPharmacieucalAndSupplier>();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        #endregion

        #region Quan ly danh muc member
        private void RefGenMedProductDetails_DrugMgnt_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.IsStore = true;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.G1230_G1_TimKiemThuoc;
            VM.TextButtonThemMoi = eHCMSResources.Z0459_G1_ThemMoiThuoc;
            VM.TextDanhSach = eHCMSResources.K3080_G1_DSThuoc;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Visible;
            VM.mTim = mQLDanhMuc_Thuoc_Tim;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_DrugMgnt()
        {
            Globals.TitleForm = eHCMSResources.K2906_G1_DMucThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_DrugMgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_DrugMgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenMedProductDetails_MedicalDevicesMgnt_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.IsStore = true;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z0678_G1_TimKiemYCu;
            VM.TextButtonThemMoi = eHCMSResources.Z0679_G1_ThemMoiYCu;
            VM.TextDanhSach = eHCMSResources.Z0657_G1_DSYCu;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;
            VM.mTim = mQLDanhMuc_YCu_Tim;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_MedicalDevicesMgnt()
        {
            Globals.TitleForm = eHCMSResources.K2917_G1_DMucYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_MedicalDevicesMgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_MedicalDevicesMgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefGenMedProductDetails_ChemicalMgnt_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<ICMDrugList>();
            VM.IsStore = true;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z0680_G1_TimKIemHChat;
            VM.TextButtonThemMoi = eHCMSResources.Z0681_G1_ThemMoiHChat;
            VM.TextDanhSach = eHCMSResources.Z0658_G1_DSHChat;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;
            VM.mTim = mQLDanhMuc_HoaChat_Tim;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void RefGenMedProductDetails_ChemicalMgnt()
        {
            Globals.TitleForm = eHCMSResources.K2895_G1_DMucHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefGenMedProductDetails_ChemicalMgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefGenMedProductDetails_ChemicalMgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region "Giá Bán Thuốc"
        //Giá Bán Thuốc
        private void StoreDeptSellingItemPrices_Mgnt_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<ICMDrugDeptSellingItemPrices_ListDrug>();
            VM.IsStore = true;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.G1230_G1_TimKiemThuoc;
            VM.TextDanhSach = eHCMSResources.K3080_G1_DSThuoc;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Visible;

            VM.mTim = mGiaBan_Thuoc_Tim;
            VM.mXemDSGia= mGiaBan_Thuoc_XemDSGia;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void StoreDeptSellingItemPrices_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.T0889_G1_GiaBanThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StoreDeptSellingItemPrices_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StoreDeptSellingItemPrices_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Giá Bán Y Cụ
        private void StoreDeptSellingItemPrices_Medical_Mgnt_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<ICMDrugDeptSellingItemPrices_ListDrug>();
            VM.IsStore = true;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z0678_G1_TimKiemYCu;
            VM.TextDanhSach = eHCMSResources.Z0657_G1_DSYCu;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;
            VM.mTim = mGiaBan_YCu_Tim;
            VM.mXemDSGia = mGiaBan_YCu_XemDSGia;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void StoreDeptSellingItemPrices_Medical_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.Z0754_G1_GiaBanYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StoreDeptSellingItemPrices_Medical_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StoreDeptSellingItemPrices_Medical_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //Giá Bán Hóa Chất
        private void StoreDeptSellingItemPrices_Chemical_Mgnt_In()
        {
            Globals.PageName=Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<ICMDrugDeptSellingItemPrices_ListDrug>();
            VM.IsStore = true;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.TitleForm = Globals.TitleForm;
            VM.TextGroupTimKiem = eHCMSResources.Z0680_G1_TimKIemHChat;
            VM.TextDanhSach = eHCMSResources.Z0658_G1_DSHChat;
            VM.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;

            VM.mTim = mGiaBan_HoaChat_Tim;
            VM.mXemDSGia = mGiaBan_HoaChat_XemDSGia;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void StoreDeptSellingItemPrices_Chemical_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.Z0755_G1_GiaBanHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StoreDeptSellingItemPrices_Chemical_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StoreDeptSellingItemPrices_Chemical_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Order - hien tai khong su dung member
        public void OrderThuocCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IDrugDeptPurchaseOrder>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = "ĐẶT HÀNG CHO THUỐC";
            VM.LoadOrderWarning();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void OrderYCuCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IDrugDeptPurchaseOrder>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = "ĐẶT HÀNG CHO Y CỤ";
            VM.LoadOrderWarning();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void OrderHoaChatCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IDrugDeptPurchaseOrder>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = "ĐẶT HÀNG CHO HÓA CHẤT";
            VM.LoadOrderWarning();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        #endregion

        #region Nhap Hang - hien tai khong su dung Member
        public void InwardListCodeCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IInwardListCost>();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierDrugCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IDrugDeptInwardDrugSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = "NHẬP THUỐC TỪ NCC CHO KHO DƯỢC";

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierMedicalDeviceCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IDrugDeptInwardDrugSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = "NHẬP Y CỤ TỪ NCC CHO KHO DƯỢC";

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierChemicalCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IDrugDeptInwardDrugSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = "NHẬP HÓA CHẤT TỪ NCC CHO KHO DƯỢC";

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        private void InwardDrugFromSupplierDrugClinicCmd_In()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IDrugDeptClinicInwardDrugSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierDrugClinicCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1815_G1_NhapThuocTuNCCKhoPhg;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                InwardDrugFromSupplierDrugClinicCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InwardDrugFromSupplierDrugClinicCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void InwardDrugFromSupplierMedicalDeviceClinicCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IDrugDeptClinicInwardDrugSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = "NHẬP Y CỤ TỪ NCC CHO KHO PHÒNG";

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void InwardDrugFromSupplierChemicalClinicCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IDrugDeptClinicInwardDrugSupplier>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = "NHẬP HÓA CHẤT TỪ NCC CHO KHO PHÒNG";

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        #endregion 

        #region Nhap Thuoc Tu Khoa Duoc Cho Khoa Phong
        private void FromMedToClinicThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.SearchCriteria.IsInputMedDept = true;
            VM.strHienThi = Globals.TitleForm;

            VM.mNhapHangTuKhoDuoc_Tim = mNhapHangTuKhoDuoc_Thuoc_Tim;
            VM.mNhapHangTuKhoDuoc_Them = mNhapHangTuKhoDuoc_Thuoc_Them;
            VM.mNhapHangTuKhoDuoc_XemIn = mNhapHangTuKhoDuoc_Thuoc_XemIn;
            VM.mNhapHangTuKhoDuoc_In = mNhapHangTuKhoDuoc_Thuoc_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void FromMedToClinicThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1816_G1_NhapThuocTuKhoDuocKhoPhg;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FromMedToClinicThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FromMedToClinicThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void FromMedToClinicYCuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.SearchCriteria.IsInputMedDept = true;
            VM.strHienThi = Globals.TitleForm;
            VM.mNhapHangTuKhoDuoc_Tim = mNhapHangTuKhoDuoc_YCu_Tim;
            VM.mNhapHangTuKhoDuoc_Them = mNhapHangTuKhoDuoc_YCu_Them;
            VM.mNhapHangTuKhoDuoc_XemIn = mNhapHangTuKhoDuoc_YCu_XemIn;
            VM.mNhapHangTuKhoDuoc_In = mNhapHangTuKhoDuoc_YCu_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void FromMedToClinicYCuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1817_G1_NhapYCuTuKhoDuocKhoPhg;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FromMedToClinicYCuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FromMedToClinicYCuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void FromMedToClinicHoaChatCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.SearchCriteria.IsInputMedDept = true;
            VM.strHienThi = Globals.TitleForm;
            VM.mNhapHangTuKhoDuoc_Tim = mNhapHangTuKhoDuoc_HoaChat_Tim;
            VM.mNhapHangTuKhoDuoc_Them = mNhapHangTuKhoDuoc_HoaChat_Them;
            VM.mNhapHangTuKhoDuoc_XemIn = mNhapHangTuKhoDuoc_HoaChat_XemIn;
            VM.mNhapHangTuKhoDuoc_In = mNhapHangTuKhoDuoc_HoaChat_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void FromMedToClinicHoaChatCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1849_G1_NhapHChatTuKhoDuocVaoPhg;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FromMedToClinicHoaChatCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FromMedToClinicHoaChatCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ImportedFromMedToClinicCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptClinicInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.SearchCriteria.IsInputMedDept = true;
            VM.strHienThi = Globals.TitleForm;
            //VM.mNhapHangTuKhoDuoc_Tim = mNhapHangTuKhoDuoc_HoaChat_Tim;
            //VM.mNhapHangTuKhoDuoc_Them = mNhapHangTuKhoDuoc_HoaChat_Them;
            //VM.mNhapHangTuKhoDuoc_XemIn = mNhapHangTuKhoDuoc_HoaChat_XemIn;
            //VM.mNhapHangTuKhoDuoc_In = mNhapHangTuKhoDuoc_HoaChat_In;
            VM.mNhapHangTuKhoDuoc_Tim = mNhapHangTuKhoDuoc;
            VM.mNhapHangTuKhoDuoc_Them = mNhapHangTuKhoDuoc;
            VM.mNhapHangTuKhoDuoc_XemIn = mNhapHangTuKhoDuoc;
            VM.mNhapHangTuKhoDuoc_In = mNhapHangTuKhoDuoc;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void FromMedToClinicVTYTTieuHaoCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2478_G1_NhapVTYTTHTuKDChoKP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromMedToClinicVaccinesCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2479_G1_NhapVacXinTuKDChoKP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromMedToClinicBloodsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2480_G1_NhapMauTuKDChoKP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromMedToClinicResourcesCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2552_G1_NhapVTTHtuKDchoKP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void FromMedToClinicThanhTrungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2514_G1_NhapThanhTrungTuKDChoKP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromMedToClinicVPPCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2535_G1_NhapVPPTuKDChoKP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromMedToClinicCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void ImportedFromClinicToClinicCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptClinicInwardFromDrugDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.SearchCriteria.IsInputMedDept = true;
            VM.strHienThi = Globals.TitleForm;
            VM.IsFromClinic = true;
            VM.mNhapHangTuKhoDuoc_Tim = mNhapHangTuKhoDuoc_HoaChat_Tim;
            VM.mNhapHangTuKhoDuoc_Them = mNhapHangTuKhoDuoc_HoaChat_Them;
            VM.mNhapHangTuKhoDuoc_XemIn = mNhapHangTuKhoDuoc_HoaChat_XemIn;
            VM.mNhapHangTuKhoDuoc_In = mNhapHangTuKhoDuoc_HoaChat_In;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void FromClinicToClinicThuocCmd()
        {
            Globals.TitleForm = "NHẬP THUỐC TỪ KHO KHÁC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.THUOC);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.THUOC);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromClinicToClinicYCuCmd()
        {
            Globals.TitleForm = "NHẬP Y CỤ TỪ KHO KHÁC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromClinicToClinicHoaChatCmd()
        {
            Globals.TitleForm = "NHẬP HÓA CHẤT TỪ KHO KHÁC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.HOA_CHAT);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.HOA_CHAT);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromClinicToClinicVTYTTieuHaoCmd()
        {
            Globals.TitleForm = "NHẬP VTYTTH TỪ KHO KHÁC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromClinicToClinicVaccinesCmd()
        {
            Globals.TitleForm = "NHẬP VACCINE TỪ KHO KHÁC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromClinicToClinicBloodsCmd()
        {
            Globals.TitleForm = "NHẬP MÁU TỪ KHO KHÁC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromClinicToClinicResourcesCmd()
        {
            Globals.TitleForm = "NHẬP VTTH TỪ KHO KHÁC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void FromClinicToClinicThanhTrungCmd()
        {
            Globals.TitleForm = "NHẬP THANH TRÙNG TỪ KHO KHÁC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void FromClinicToClinicVPPCmd()
        {
            Globals.TitleForm = "NHẬP VPP TỪ KHO KHÁC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ImportedFromClinicToClinicCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Xuat Tra Member
        private void XuatTraThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IXuatNoiBoStoreDept>();
            VM.V_MedProductType = AllLookupValues.MedProductType.THUOC;

            VM.mXuatTraHang_Xem = mXuatTraHang_Thuoc_Xem;
            VM.mXuatTraHang_PhieuMoi = mXuatTraHang_Thuoc_PhieuMoi;
            VM.mXuatTraHang_XemIn = mXuatTraHang_Thuoc_XemIn;
            VM.mXuatTraHang_In = mXuatTraHang_Thuoc_In;

            VM.InitData();

            VM.strHienThi = Globals.TitleForm;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatTraThuocCmd()
        {
            //Globals.TitleForm = "XUẤT TRẢ THUỐC CHO KHOA DƯỢC";
            Globals.TitleForm = eHCMSResources.Z1172_G1_XuatThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatTraYCuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IXuatNoiBoStoreDept>();
            VM.V_MedProductType = AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;

            VM.mXuatTraHang_Xem = mXuatTraHang_YCu_Xem;
            VM.mXuatTraHang_PhieuMoi = mXuatTraHang_YCu_PhieuMoi;
            VM.mXuatTraHang_XemIn = mXuatTraHang_YCu_XemIn;
            VM.mXuatTraHang_In = mXuatTraHang_YCu_In;

            VM.InitData();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatTraYCuCmd()
        {
            //Globals.TitleForm = "XUẤT TRẢ Y CỤ CHO KHOA DƯỢC";
            Globals.TitleForm = eHCMSResources.Z1818_G1_XuatYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraYCuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraYCuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatTraHoaChatCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IXuatNoiBoStoreDept>();
            VM.V_MedProductType = AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;
            VM.mXuatTraHang_Xem = mXuatTraHang_HoaChat_Xem;
            VM.mXuatTraHang_PhieuMoi = mXuatTraHang_HoaChat_PhieuMoi;
            VM.mXuatTraHang_XemIn = mXuatTraHang_HoaChat_XemIn;
            VM.mXuatTraHang_In = mXuatTraHang_HoaChat_In;

            VM.InitData();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatTraHoaChatCmd()
        {
            //Globals.TitleForm = "XUẤT TRẢ HÓA CHẤT CHO KHOA DƯỢC";
            Globals.TitleForm = eHCMSResources.Z1819_G1_XuatHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraHoaChatCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraHoaChatCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatTraCmd_In(AllLookupValues.MedProductType V_MedProductType = AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IXuatNoiBoStoreDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            //VM.mXuatTraHang_Xem = mXuatTraHang_HoaChat_Xem;
            //VM.mXuatTraHang_PhieuMoi = mXuatTraHang_HoaChat_PhieuMoi;
            //VM.mXuatTraHang_XemIn = mXuatTraHang_HoaChat_XemIn;
            //VM.mXuatTraHang_In = mXuatTraHang_HoaChat_In;
            VM.mXuatTraHang_Xem = mXuatTraHang;
            VM.mXuatTraHang_PhieuMoi = mXuatTraHang;
            VM.mXuatTraHang_XemIn = mXuatTraHang;
            VM.mXuatTraHang_In = mXuatTraHang;
            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void XuatTraVTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2494_G1_XuatVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraCmd_In(AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraCmd_In(AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XuatTraVaccineCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2494_G1_XuatVacXin;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraCmd_In(AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraCmd_In(AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XuatTraBloodsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2494_G1_XuatMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraCmd_In(AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraCmd_In(AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XuatTraVPPCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2494_G1_XuatVanPhongPham;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraCmd_In(AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraCmd_In(AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XuatTraThanhTrungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2513_G1_XuatThanhTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraCmd_In(AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraCmd_In(AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XuatTraResourceCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2552_G1_XuatVTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraCmd_In(AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraCmd_In(AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Xuat Cho Benh Nhan Member
        private void XuatThuocChoBNCmd_In(long aRegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IOutwardToPatient_V2>();
            VM.V_MedProductType = AllLookupValues.MedProductType.THUOC;
            VM.DoseVisibility = true;
            VM.RequireDoctorAndDate = Globals.ServerConfigSection.ClinicDeptElements.RequireDoctorAndDateForMed;
            VM.strHienThi = Globals.TitleForm;
            VM.mXuatChoBenhNhan_Xem = mXuatChoBenhNhan_Thuoc_Xem;
            VM.mXuatChoBenhNhan_PhieuMoi = mXuatChoBenhNhan_Thuoc_PhieuMoi;
            VM.mXuatChoBenhNhan_XemIn = mXuatChoBenhNhan_Thuoc_XemIn;
            VM.mXuatChoBenhNhan_In = mXuatChoBenhNhan_Thuoc_In;
            VM.V_RegistrationType = aRegistrationType;
            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void XuatThuocChoBNCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1820_G1_XuatThuocBN;           
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatThuocChoBNCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatThuocChoBNCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XuatThuocChoBNNTCmd()
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2722_G1_Xuat0ChoBNNgoaiTru, eHCMSResources.G0787_G1_Thuoc);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatThuocChoBNCmd_In((long)AllLookupValues.RegistrationType.NGOAI_TRU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatThuocChoBNCmd_In((long)AllLookupValues.RegistrationType.NGOAI_TRU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XuatYCuChoBNNTCmd()
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2722_G1_Xuat0ChoBNNgoaiTru, eHCMSResources.G2907_G1_YCu);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatYCuChoBNCmd_In((long)AllLookupValues.RegistrationType.NGOAI_TRU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatYCuChoBNCmd_In((long)AllLookupValues.RegistrationType.NGOAI_TRU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XuatHoaChatChoBNNTCmd() { }

        private void XuatYCuChoBNCmd_In(long aRegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IOutwardToPatient_V2>();
            VM.V_MedProductType = AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;
            VM.RequireDoctorAndDate = Globals.ServerConfigSection.ClinicDeptElements.RequireDoctorAndDateForMat;
            VM.mXuatChoBenhNhan_Xem = mXuatChoBenhNhan_YCu_Xem;
            VM.mXuatChoBenhNhan_PhieuMoi = mXuatChoBenhNhan_YCu_PhieuMoi;
            VM.mXuatChoBenhNhan_XemIn = mXuatChoBenhNhan_YCu_XemIn;
            VM.mXuatChoBenhNhan_In = mXuatChoBenhNhan_YCu_In;
            VM.V_RegistrationType = aRegistrationType;
            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatYCuChoBNCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1821_G1_XuatYCuBN;            
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatYCuChoBNCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatYCuChoBNCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatHoaChatChoBNCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IOutwardToPatient_V2>();
            VM.V_MedProductType = AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;
            VM.RequireDoctorAndDate = Globals.ServerConfigSection.ClinicDeptElements.RequireDoctorAndDateForLab;
            VM.mXuatChoBenhNhan_Xem = mXuatChoBenhNhan_HoaChat_Xem;
            VM.mXuatChoBenhNhan_PhieuMoi = mXuatChoBenhNhan_HoaChat_PhieuMoi;
            VM.mXuatChoBenhNhan_XemIn = mXuatChoBenhNhan_HoaChat_XemIn;
            VM.mXuatChoBenhNhan_In = mXuatChoBenhNhan_HoaChat_In;          
            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatHoaChatChoBNCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1822_G1_XuatHChatBN;           
            
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatHoaChatChoBNCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatHoaChatChoBNCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion

        #region Kiem Ke Member
        private void KKThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;

            VM.mKiemKe_Tim = mKiemKe_Thuoc_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_Thuoc_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_Thuoc_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_Thuoc_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKThuocCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ THUỐC CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKYCuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;
            VM.mKiemKe_Tim = mKiemKe_YCu_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_YCu_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_YCu_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_YCu_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKYCuCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ Y CỤ CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKYCuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKYCuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKHoaChatCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;
            VM.mKiemKe_Tim = mKiemKe_HoaChat_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_HoaChat_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_HoaChat_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_HoaChat_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKHoaChatCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ HÓA CHẤT CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKHoaChatCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKHoaChatCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKVTYTTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            VM.strHienThi = Globals.TitleForm;

            VM.mKiemKe_Tim = mKiemKe_VTYTTH_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_VTYTTH_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_VTYTTH_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_VTYTTH_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKVTYTTHCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ VTYTTH CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKVTYTTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKVTYTTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKVaccineCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            VM.strHienThi = Globals.TitleForm;

            VM.mKiemKe_Tim = mKiemKe_Vaccine_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_Vaccine_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_Vaccine_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_Vaccine_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKVaccineCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ VACCINE CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKVaccineCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKVaccineCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKBloodCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            VM.strHienThi = Globals.TitleForm;

            VM.mKiemKe_Tim = mKiemKe_Blood_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_Blood_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_Blood_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_Blood_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKBloodCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ MÁU CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKBloodCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKBloodCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKVTTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            VM.strHienThi = Globals.TitleForm;

            VM.mKiemKe_Tim = mKiemKe_VTTH_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_VTTH_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_VTTH_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_VTTH_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKVTTHCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ VẬT TƯ TIÊU HAO CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKVTTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKVTTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKVPPCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            VM.strHienThi = Globals.TitleForm;

            VM.mKiemKe_Tim = mKiemKe_VPP_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_VPP_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_VPP_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_VPP_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKVPPCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ VĂN PHÒNG PHẨM CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKVPPCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKVPPCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKThanhTrungCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            VM.strHienThi = Globals.TitleForm;

            VM.mKiemKe_Tim = mKiemKe_ThanhTrung_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_ThanhTrung_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_ThanhTrung_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_ThanhTrung_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKThanhTrungCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ THANH TRÙNG CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKThanhTrungCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKThanhTrungCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #endregion

        #region Thong Ke Member

        private void TKThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptUsedMedProductInternal>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;

            VM.mThongKe_xem = mThongKe_Thuoc_xem;
            VM.mThongKe_PhieuMoi = mThongKe_Thuoc_PhieuMoi;
            VM.mThongKe_XemIn = mThongKe_Thuoc_XemIn;
            VM.mThongKe_XemIn = mThongKe_Thuoc_XemIn;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TKThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1825_G1_TKeThuocKhoPhgXuatBN;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TKThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TKThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TKYCuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptUsedMedProductInternal>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;
            VM.mThongKe_xem = mThongKe_YCu_xem;
            VM.mThongKe_PhieuMoi = mThongKe_YCu_PhieuMoi;
            VM.mThongKe_XemIn = mThongKe_YCu_XemIn;
            VM.mThongKe_XemIn = mThongKe_YCu_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TKYCuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1826_G1_TKeYCuKhoPhgXuatBN;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TKYCuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TKYCuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TKHoaChatCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptUsedMedProductInternal>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;
            VM.mThongKe_xem = mThongKe_HoaChat_xem;
            VM.mThongKe_PhieuMoi = mThongKe_HoaChat_PhieuMoi;
            VM.mThongKe_XemIn = mThongKe_HoaChat_XemIn;
            VM.mThongKe_XemIn = mThongKe_HoaChat_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TKHoaChatCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1827_G1_TKeHChatKhoPhgXuatBN;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TKHoaChatCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TKHoaChatCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Report Member

        #region Nhap Xuat Ton Member
        private void NhapXuatTonThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            VM.CanSelectedRefGenDrugCatID_1 = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z0772_G1_BCNXTThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonYCuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_YCu_XemIn;
            VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_YCu_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonYCuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z0773_G1_BCNXTYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonYCuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonYCuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonHoaChatCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_HoaChat_XemIn;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonHoaChatCmd()
        {
            Globals.TitleForm = eHCMSResources.Z0774_G1_BCNXTHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonHoaChatCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonHoaChatCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NhapXuatTonCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = V_MedProductType;
            VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_HoaChat_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void NhapXuatTonVTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVTYTTH;
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
        public void NhapXuatTonVaccineCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVacXin;
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
        public void NhapXuatTonBloodsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTMau;
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
        public void NhapXuatTonResourcesCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2549_G1_BCNXT_VTTH;
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
        public void NhapXuatTonThanhTrungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2516_G1_BCNXT_ThanhTrung;
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
        public void NhapXuatTonVPPCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2485_G1_BCNXTVPP;
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

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_Thuoc_XemIn;
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

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
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

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
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
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = V_MedProductType;
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
        private void TheKhoThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptTheKho>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;
            VM.mBaoCaoTheKho_Xem = mBaoCaoTheKho_Thuoc_Xem;
            VM.mBaoCaoTheKho_In = mBaoCaoTheKho_Thuoc_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TheKhoThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z0775_G1_TheKhoThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheKhoYCuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptTheKho>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;
            VM.mBaoCaoTheKho_Xem = mBaoCaoTheKho_YCu_Xem;
            VM.mBaoCaoTheKho_In = mBaoCaoTheKho_YCu_In;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TheKhoYCuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z0776_G1_TheKhoYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoYCuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoYCuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheKhoHoaChatCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptTheKho>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;
            VM.mBaoCaoTheKho_Xem = mBaoCaoTheKho_HoaChat_Xem;
            VM.mBaoCaoTheKho_In = mBaoCaoTheKho_HoaChat_In;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TheKhoHoaChatCmd()
        {
            Globals.TitleForm = eHCMSResources.Z0777_G1_TheKhoHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoHoaChatCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoHoaChatCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TheKhoCmd_In(long V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptTheKho>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            //VM.mBaoCaoTheKho_Xem = mBaoCaoTheKho_HoaChat_Xem;
            //VM.mBaoCaoTheKho_In = mBaoCaoTheKho_HoaChat_In;
            VM.mBaoCaoTheKho_Xem = mBCTheKho;
            VM.mBaoCaoTheKho_In = mBCTheKho;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void TheKhoVTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2482_G1_TheKhoVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In((long)AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void TheKhoVacXinCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2482_G1_TheKhoVacXin;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In((long)AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void TheKhoMauCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2482_G1_TheKhoMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In((long)AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In((long)AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void TheKhoVPPCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2538_G1_TheKhoVPP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In((long)AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void TheKhoThanhTrungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2518_G1_TheKhoThanhTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In((long)AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void TheKhoVTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2553_G1_TheKhoVTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #region Phieu DN TT - hien tai khong su dung Member
        public void SuggestThuocCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptBangKeChungTuThanhToan>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = "BẢNG KÊ CHỨNG TỪ THANH TOÁN THUỐC";

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void SuggestYCuCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptBangKeChungTuThanhToan>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = "BẢNG KÊ CHỨNG TỪ THANH TOÁN Y CỤ";

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void SuggestHoaChatCmd()
        {
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptBangKeChungTuThanhToan>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = "BẢNG KÊ CHỨNG TỪ THANH TOÁN HÓA CHẤT";

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        #endregion

        #region Report Outward To Patient
        private void ReportOutwardMedCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.mDetail = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ReportOutwardMedCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1828_G1_BCXuatThuocBN.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOutwardMedCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardMedCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportOutwardMatCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.mDetail = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ReportOutwardMatCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1829_G1_BCXuatYCuBN.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOutwardMatCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardMatCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportOutwardLabCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.mDetail = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ReportOutwardLabCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1830_G1_BCXuatHChatBN.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOutwardLabCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardLabCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportOutwardVTYTTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            VM.mDetail = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ReportOutwardVTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2570_G1_BCXuatVTYTTHBN.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOutwardVTYTTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardVTYTTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportOutwardVaccineCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            VM.mDetail = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ReportOutwardVaccineCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2571_G1_BCXuatVaccineBN.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOutwardVaccineCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardVaccineCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportOutwardBloodCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            VM.mDetail = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ReportOutwardBloodCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2572_G1_BCXuatMauBN.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOutwardBloodCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardBloodCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportOutwardVPPCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            VM.mDetail = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ReportOutwardVPPCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2573_G1_BCXuatVPPBN.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOutwardVPPCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardVPPCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportOutwardVTTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            VM.mDetail = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ReportOutwardVTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2574_G1_BCXuatVTTHBN.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOutwardVTTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardVTTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportOutwardThanhTrungCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicCommonReport>();
            VM.eItem = ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT;
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            VM.mDetail = true;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void ReportOutwardThanhTrungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2575_G1_BCXuatThanhTrungBN.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOutwardThanhTrungCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardThanhTrungCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //------ DPT 
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
            var module = Globals.GetViewModel<IStoreDeptHome>();
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
        #endregion

        #region RptDanhSachXuatKhoPhong
        private void XuatThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mBaoCaoXuat_XemIn = mBaoCaoXuat_Thuoc_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1831_G1_BCDSXuatThuoc.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatYCuCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mBaoCaoXuat_XemIn = mBaoCaoXuat_YCu_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatYCuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1832_G1_BCDSXuatYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatYCuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatYCuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatHoaChatCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mBaoCaoXuat_XemIn = mBaoCaoXuat_HoaChat_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatHoaChatCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1833_G1_BCDSXuatHChat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatHoaChatCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatHoaChatCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatVTYTTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VTYT_TIEUHAO;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mBaoCaoXuat_XemIn = mBCXuat;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatVTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2564_G1_BCDSXuatVTYTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatVTYTTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatVTYTTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatVaccineCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.TIEM_NGUA;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mBaoCaoXuat_XemIn = mBCXuat;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatVaccineCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2565_G1_BCDSXuatVaccine;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatVaccineCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatVaccineCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatBloodCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.MAU;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mBaoCaoXuat_XemIn = mBCXuat;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatBloodCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2566_G1_BCDSXuatMau;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatBloodCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatBloodCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatVPPCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mBaoCaoXuat_XemIn = mBCXuat;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatVPPCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2567_G1_BCDSXuatVPP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatVPPCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatVPPCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatVTTHCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.VATTU_TIEUHAO;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mBaoCaoXuat_XemIn = mBCXuat;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatVTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2568_G1_BCDSXuatVTTH;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatVTTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatVTTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void XuatThanhTrungCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicRptDanhSachXuatKhoaDuoc>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THANHTRUNG;
            VM.TieuDeRpt = Globals.TitleForm;
            VM.mBaoCaoXuat_XemIn = mBCXuat;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatThanhTrungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2569_G1_BCDSXuatThanhTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatThanhTrungCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatThanhTrungCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #endregion

        #endregion

        //Dinh them phan kiem tra trach nhiem cho tung khoa phong cua nhan vien dang nhap
        public void CheckResponsibility()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        private void StaffDeptPresence_In(bool IsUpdateRequiredNumber)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var StaffDeptPresenceVm = Globals.GetViewModel<IStaffPresence>();
            StaffDeptPresenceVm.IsUpdateRequiredNumber = IsUpdateRequiredNumber;
            (module as Conductor<object>).ActivateItem(StaffDeptPresenceVm);
            module.MainContent = StaffDeptPresenceVm;
        }

        public void UpdateRequiredNumberCmd()
        {
            Globals.TitleForm = eHCMSLanguage.eHCMSResources.Z1924_G1_QLyChiTieuNhanSu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StaffDeptPresence_In(true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StaffDeptPresence_In(true);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void UpdatePresenceDailyCmd()
        {
            Globals.TitleForm = eHCMSLanguage.eHCMSResources.Z1925_G1_QLyTinhHinhKhoaNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StaffDeptPresence_In(false);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StaffDeptPresence_In(false);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void PatientManagementCmd()
        {
            Globals.TitleForm = "Danh sách bệnh nhân";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PatientManagement_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PatientManagement_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PatientManagement_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var PatientManagementVm = Globals.GetViewModel<IPatientManagement>();
            (module as Conductor<object>).ActivateItem(PatientManagementVm);
            module.MainContent = PatientManagementVm;
        }

        //==== #001 //====
        private void FollowThuocCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm_V2>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            UnitVM.DoseVisibility = true;
            UnitVM.strHienThi = Globals.TitleForm;

            UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau_Thuoc_Tim;
            UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau_Thuoc_Them;
            UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau_Thuoc_Xoa;
            UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau_Thuoc_XemIn;
            UnitVM.mPhieuYeuCau_In = mPhieuYeuCau_Thuoc_In;

            UnitVM.UsedForRequestingDrug = true;
            UnitVM.IsResultView = true;
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void FollowThuocCmd()
        {
            Globals.TitleForm = "THEO DÕI KẾT QUẢ DÙNG THUỐC";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FollowThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FollowThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //==== #001 //====

        //▼====: #003
        private void BCThuocSapHetHanSDCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IStoreDeptHome>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BC_ThuocSapHetHanDung;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
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
        //▲====: #003     
        //▼====: #008
        private void BCMauSapHetHanDungCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IStoreDeptHome>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BCMauSapHetHanDung;
            reportVm.ChonKho = true;
            reportVm.IsPurpose = false;
            reportVm.IsShowGroupReportType = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCMauSapHetHanDungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3242_G1_BCMauSapHetHanDung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCMauSapHetHanDungCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCMauSapHetHanDungCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #008
        private void TreatmentStatisticsByDeptCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<ITreatmentStatisticsByDepartment>();
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void TreatmentStatisticsByDeptCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2887_G1_TKBenhThuongGapTrongQuy;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TreatmentStatisticsByDeptCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TreatmentStatisticsByDeptCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //▼====: #007 
        private bool _mTaoMauLinh = true;
        private bool _mTaoMauLinh_Thuoc = true;
        private bool _mTaoMauLinh_YCu = true;
        public bool mTaoMauLinh
        {
            get

            {
                return _mTaoMauLinh;
            }
            set
            {
                if (_mTaoMauLinh == value)
                    return;
                _mTaoMauLinh = value;
                NotifyOfPropertyChange(() => mTaoMauLinh);




            }
        }

        public bool mTaoMauLinh_Thuoc
        {
            get
            {
                return _mTaoMauLinh_Thuoc;
            }
            set
            {
                if (_mTaoMauLinh_Thuoc == value)
                    return;
                _mTaoMauLinh_Thuoc = value;
                NotifyOfPropertyChange(() => mTaoMauLinh_Thuoc);
            }
        }

        public bool mTaoMauLinh_YCu
        {
            get
            {
                return _mTaoMauLinh_YCu;
            }
            set
            {
                if (_mTaoMauLinh_YCu == value)
                    return;
                _mTaoMauLinh_YCu = value;
                NotifyOfPropertyChange(() => mTaoMauLinh_YCu);
            }
        }

        private bool _mPhieuYeuCau = true;
        private bool _mPhieuYeuCau_VTYTTH = true;
        private bool _mPhieuYeuCau_Vaccine = true;
        private bool _mPhieuYeuCau_Blood = true;
        private bool _mPhieuYeuCau_VPP = true;
        private bool _mPhieuYeuCau_VTTH = true;
        private bool _mPhieuYeuCau_ThanhTrung = true;
        private bool _mPhieuYeuCau_TongHop = true;
        private bool _mPhieuYeuCau_SuatAn = true;

        public bool mPhieuYeuCau
        {
            get
            {
                return _mPhieuYeuCau;
            }
            set
            {
                if (_mPhieuYeuCau == value)
                    return;
                _mPhieuYeuCau = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau);
            }
        }

        public bool mPhieuYeuCau_VTYTTH
        {
            get
            {
                return _mPhieuYeuCau_VTYTTH;
            }
            set
            {
                if (_mPhieuYeuCau_VTYTTH == value)
                    return;
                _mPhieuYeuCau_VTYTTH = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_VTYTTH);
            }
        }

        public bool mPhieuYeuCau_Vaccine
        {
            get
            {
                return _mPhieuYeuCau_Vaccine;
            }
            set
            {
                if (_mPhieuYeuCau_Vaccine == value)
                    return;
                _mPhieuYeuCau_Vaccine = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Vaccine);
            }
        }

        public bool mPhieuYeuCau_Blood
        {
            get
            {
                return _mPhieuYeuCau_Blood;
            }
            set
            {
                if (_mPhieuYeuCau_Blood == value)
                    return;
                _mPhieuYeuCau_Blood = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Blood);
            }
        }

        public bool mPhieuYeuCau_VPP
        {
            get
            {
                return _mPhieuYeuCau_VPP;
            }
            set
            {
                if (_mPhieuYeuCau_VPP == value)
                    return;
                _mPhieuYeuCau_VPP = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_VPP);
            }
        }

        public bool mPhieuYeuCau_VTTH
        {
            get
            {
                return _mPhieuYeuCau_VTTH;
            }
            set
            {
                if (_mPhieuYeuCau_VTTH == value)
                    return;
                _mPhieuYeuCau_VTTH = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_VTTH);
            }
        }

        public bool mPhieuYeuCau_ThanhTrung
        {
            get
            {
                return _mPhieuYeuCau_ThanhTrung;
            }
            set
            {
                if (_mPhieuYeuCau_ThanhTrung == value)
                    return;
                _mPhieuYeuCau_ThanhTrung = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_ThanhTrung);
            }
        }

        public bool mPhieuYeuCau_TongHop
        {
            get
            {
                return _mPhieuYeuCau_TongHop;
            }
            set
            {
                if (_mPhieuYeuCau_TongHop == value)
                    return;
                _mPhieuYeuCau_TongHop = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_TongHop);
            }
        }

        public bool mPhieuYeuCau_SuatAn
        {
            get
            {
                return _mPhieuYeuCau_SuatAn;
            }
            set
            {
                if (_mPhieuYeuCau_SuatAn == value)
                    return;
                _mPhieuYeuCau_SuatAn = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_SuatAn);
            }
        }

        private bool _mNhapHangTuKhoDuoc = true;
        private bool _mNhapHangTuKhoDuoc_VTYTTH = true;
        private bool _mNhapHangTuKhoDuoc_Vaccine = true;
        private bool _mNhapHangTuKhoDuoc_Blood = true;
        private bool _mNhapHangTuKhoDuoc_VPP = true;
        private bool _mNhapHangTuKhoDuoc_VTTH = true;
        private bool _mNhapHangTuKhoDuoc_ThanhTrung = true;

        public bool mNhapHangTuKhoDuoc
        {
            get
            {
                return _mNhapHangTuKhoDuoc;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc == value)
                    return;
                _mNhapHangTuKhoDuoc = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc);
            }
        }

        public bool mNhapHangTuKhoDuoc_VTYTTH
        {
            get
            {
                return _mNhapHangTuKhoDuoc_VTYTTH;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_VTYTTH == value)
                    return;
                _mNhapHangTuKhoDuoc_VTYTTH = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_VTYTTH);
            }
        }

        public bool mNhapHangTuKhoDuoc_Vaccine
        {
            get
            {
                return _mNhapHangTuKhoDuoc_Vaccine;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_Vaccine == value)
                    return;
                _mNhapHangTuKhoDuoc_Vaccine = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_Vaccine);
            }
        }

        public bool mNhapHangTuKhoDuoc_Blood
        {
            get
            {
                return _mNhapHangTuKhoDuoc_Blood;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_Blood == value)
                    return;
                _mNhapHangTuKhoDuoc_Blood = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_Blood);
            }
        }

        public bool mNhapHangTuKhoDuoc_VPP
        {
            get
            {
                return _mNhapHangTuKhoDuoc_VPP;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_VPP == value)
                    return;
                _mNhapHangTuKhoDuoc_VPP = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_VPP);
            }
        }

        public bool mNhapHangTuKhoDuoc_VTTH
        {
            get
            {
                return _mNhapHangTuKhoDuoc_VTTH;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_VTTH == value)
                    return;
                _mNhapHangTuKhoDuoc_VTTH = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_VTTH);
            }
        }

        public bool mNhapHangTuKhoDuoc_ThanhTrung
        {
            get
            {
                return _mNhapHangTuKhoDuoc_ThanhTrung;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_ThanhTrung == value)
                    return;
                _mNhapHangTuKhoDuoc_ThanhTrung = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_ThanhTrung);
            }
        }

        private bool _mXuatTraHang = true;
        private bool _mXuatTraHang_VTYTTH = true;
        private bool _mXuatTraHang_Vaccine = true;
        private bool _mXuatTraHang_Blood = true;
        private bool _mXuatTraHang_VPP = true;
        private bool _mXuatTraHang_VTTH = true;
        private bool _mXuatTraHang_ThanhTrung = true;
        public bool mXuatTraHang
        {
            get
            {
                return _mXuatTraHang;
            }
            set
            {
                if (_mXuatTraHang == value)
                    return;
                _mXuatTraHang = value;
                NotifyOfPropertyChange(() => mXuatTraHang);
            }
        }

        public bool mXuatTraHang_VTYTTH
        {
            get
            {
                return _mXuatTraHang_VTYTTH;
            }
            set
            {
                if (_mXuatTraHang_VTYTTH == value)
                    return;
                _mXuatTraHang_VTYTTH = value;
                NotifyOfPropertyChange(() => mXuatTraHang_VTYTTH);
            }
        }

        public bool mXuatTraHang_Vaccine
        {
            get
            {
                return _mXuatTraHang_Vaccine;
            }
            set
            {
                if (_mXuatTraHang_Vaccine == value)
                    return;
                _mXuatTraHang_Vaccine = value;
                NotifyOfPropertyChange(() => mXuatTraHang_Vaccine);
            }
        }

        public bool mXuatTraHang_Blood
        {
            get
            {
                return _mXuatTraHang_Blood;
            }
            set
            {
                if (_mXuatTraHang_Blood == value)
                    return;
                _mXuatTraHang_Blood = value;
                NotifyOfPropertyChange(() => mXuatTraHang_Blood);
            }
        }

        public bool mXuatTraHang_VPP
        {
            get
            {
                return _mXuatTraHang_VPP;
            }
            set
            {
                if (_mXuatTraHang_VPP == value)
                    return;
                _mXuatTraHang_VPP = value;
                NotifyOfPropertyChange(() => mXuatTraHang_VPP);
            }
        }

        public bool mXuatTraHang_VTTH
        {
            get
            {
                return _mXuatTraHang_VTTH;
            }
            set
            {
                if (_mXuatTraHang_VTTH == value)
                    return;
                _mXuatTraHang_VTTH = value;
                NotifyOfPropertyChange(() => mXuatTraHang_VTTH);
            }
        }

        public bool mXuatTraHang_ThanhTrung
        {
            get
            {
                return _mXuatTraHang_ThanhTrung;
            }
            set
            {
                if (_mXuatTraHang_ThanhTrung == value)
                    return;
                _mXuatTraHang_ThanhTrung = value;
                NotifyOfPropertyChange(() => mXuatTraHang_ThanhTrung);
            }
        }

        private bool _mXuatChoBenhNhanNgoaiTru_Thuoc = true;
        private bool _mXuatChoBenhNhanNgoaiTru_YCu = true;
        private bool _mXuatChoBenhNhanNgoaiTru_HoaChat = true;
        private bool _mXuatChoBenhNhanNgoaiTru_DinhDuong = true;

        public bool mXuatChoBenhNhanNgoaiTru_Thuoc
        {
            get
            {
                return _mXuatChoBenhNhanNgoaiTru_Thuoc;
            }
            set
            {
                if (_mXuatChoBenhNhanNgoaiTru_Thuoc == value)
                    return;
                _mXuatChoBenhNhanNgoaiTru_Thuoc = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhanNgoaiTru_Thuoc);
            }
        }

        public bool mXuatChoBenhNhanNgoaiTru_YCu
        {
            get
            {
                return _mXuatChoBenhNhanNgoaiTru_YCu;
            }
            set
            {
                if (_mXuatChoBenhNhanNgoaiTru_YCu == value)
                    return;
                _mXuatChoBenhNhanNgoaiTru_YCu = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhanNgoaiTru_YCu);
            }
        }

        public bool mXuatChoBenhNhanNgoaiTru_HoaChat
        {
            get
            {
                return _mXuatChoBenhNhanNgoaiTru_HoaChat;
            }
            set
            {
                if (_mXuatChoBenhNhanNgoaiTru_HoaChat == value)
                    return;
                _mXuatChoBenhNhanNgoaiTru_HoaChat = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhanNgoaiTru_HoaChat);
            }
        }

        public bool mXuatChoBenhNhanNgoaiTru_DinhDuong
        {
            get
            {
                return _mXuatChoBenhNhanNgoaiTru_DinhDuong;
            }
            set
            {
                if (_mXuatChoBenhNhanNgoaiTru_DinhDuong == value)
                    return;
                _mXuatChoBenhNhanNgoaiTru_DinhDuong = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhanNgoaiTru_DinhDuong);
            }
        }

        private bool _mKiemKe = true;
        private bool _mKiemKe_VTYTTH = true;
        private bool _mKiemKe_Vaccine = true;
        private bool _mKiemKe_Blood = true;
        private bool _mKiemKe_VPP = true;
        private bool _mKiemKe_VTTH = true;
        private bool _mKiemKe_ThanhTrung = true;
        private bool _mKiemKe_DinhDuong = true;
        public bool mKiemKe
        {
            get
            {
                return _mKiemKe;
            }
            set
            {
                if (_mKiemKe == value)
                    return;
                _mKiemKe = value;
                NotifyOfPropertyChange(() => mKiemKe);
            }
        }

        public bool mKiemKe_VTYTTH
        {
            get
            {
                return _mKiemKe_VTYTTH;
            }
            set
            {
                if (_mKiemKe_VTYTTH == value)
                    return;
                _mKiemKe_VTYTTH = value;
                NotifyOfPropertyChange(() => mKiemKe_VTYTTH);
            }
        }

        public bool mKiemKe_Vaccine
        {
            get
            {
                return _mKiemKe_Vaccine;
            }
            set
            {
                if (_mKiemKe_Vaccine == value)
                    return;
                _mKiemKe_Vaccine = value;
                NotifyOfPropertyChange(() => mKiemKe_Vaccine);
            }
        }

        public bool mKiemKe_Blood
        {
            get
            {
                return _mKiemKe_Blood;
            }
            set
            {
                if (_mKiemKe_Blood == value)
                    return;
                _mKiemKe_Blood = value;
                NotifyOfPropertyChange(() => mKiemKe_Blood);
            }
        }

        public bool mKiemKe_VPP
        {
            get
            {
                return _mKiemKe_VPP;
            }
            set
            {
                if (_mKiemKe_VPP == value)
                    return;
                _mKiemKe_VPP = value;
                NotifyOfPropertyChange(() => mKiemKe_VPP);
            }
        }

        public bool mKiemKe_VTTH
        {
            get
            {
                return _mKiemKe_VTTH;
            }
            set
            {
                if (_mKiemKe_VTTH == value)
                    return;
                _mKiemKe_VTTH = value;
                NotifyOfPropertyChange(() => mKiemKe_VTTH);
            }
        }

        public bool mKiemKe_ThanhTrung
        {
            get
            {
                return _mKiemKe_ThanhTrung;
            }
            set
            {
                if (_mKiemKe_ThanhTrung == value)
                    return;
                _mKiemKe_ThanhTrung = value;
                NotifyOfPropertyChange(() => mKiemKe_ThanhTrung);
            }
        }

        public bool mKiemKe_DinhDuong
        {
            get
            {
                return _mKiemKe_DinhDuong;
            }
            set
            {
                if (_mKiemKe_DinhDuong == value)
                    return;
                _mKiemKe_DinhDuong = value;
                NotifyOfPropertyChange(() => mKiemKe_DinhDuong);
            }
        }

        private bool _mBCXuatChoBN = true;
        private bool _mBCXuatChoBN_Thuoc = true;
        private bool _mBCXuatChoBN_YCu = true;
        private bool _mBCXuatChoBN_HoaChat = true;
        private bool _mBCXuatChoBN_VTYTTH = true;
        private bool _mBCXuatChoBN_Vaccine = true;
        private bool _mBCXuatChoBN_Blood = true;
        private bool _mBCXuatChoBN_VPP = true;
        private bool _mBCXuatChoBN_VTTH = true;
        private bool _mBCXuatChoBN_ThanhTrung = true;
        private bool _mBC15NgaySuDungThuoc = true;
        public bool mBCXuatChoBN
        {
            get
            {
                return _mBCXuatChoBN;
            }
            set
            {
                if (_mBCXuatChoBN == value)
                    return;
                _mBCXuatChoBN = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN);
            }
        }

        public bool mBCXuatChoBN_Thuoc
        {
            get
            {
                return _mBCXuatChoBN_Thuoc;
            }
            set
            {
                if (_mBCXuatChoBN_Thuoc == value)
                    return;
                _mBCXuatChoBN_Thuoc = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN_Thuoc);
            }
        }

        public bool mBCXuatChoBN_YCu
        {
            get
            {
                return _mBCXuatChoBN_YCu;
            }
            set
            {
                if (_mBCXuatChoBN_YCu == value)
                    return;
                _mBCXuatChoBN_YCu = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN_YCu);
            }
        }

        public bool mBCXuatChoBN_HoaChat
        {
            get
            {
                return _mBCXuatChoBN_HoaChat;
            }
            set
            {
                if (_mBCXuatChoBN_HoaChat == value)
                    return;
                _mBCXuatChoBN_HoaChat = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN_HoaChat);
            }
        }

        public bool mBCXuatChoBN_VTYTTH
        {
            get
            {
                return _mBCXuatChoBN_VTYTTH;
            }
            set
            {
                if (_mBCXuatChoBN_VTYTTH == value)
                    return;
                _mBCXuatChoBN_VTYTTH = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN_VTYTTH);
            }
        }

        public bool mBCXuatChoBN_Vaccine
        {
            get
            {
                return _mBCXuatChoBN_Vaccine;
            }
            set
            {
                if (_mBCXuatChoBN_Vaccine == value)
                    return;
                _mBCXuatChoBN_Vaccine = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN_Vaccine);
            }
        }

        public bool mBCXuatChoBN_Blood
        {
            get
            {
                return _mBCXuatChoBN_Blood;
            }
            set
            {
                if (_mBCXuatChoBN_Blood == value)
                    return;
                _mBCXuatChoBN_Blood = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN_Blood);
            }
        }

        public bool mBCXuatChoBN_VPP
        {
            get
            {
                return _mBCXuatChoBN_VPP;
            }
            set
            {
                if (_mBCXuatChoBN_VPP == value)
                    return;
                _mBCXuatChoBN_VPP = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN_VPP);
            }
        }

        public bool mBCXuatChoBN_VTTH
        {
            get
            {
                return _mBCXuatChoBN_VTTH;
            }
            set
            {
                if (_mBCXuatChoBN_VTTH == value)
                    return;
                _mBCXuatChoBN_VTTH = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN_VTTH);
            }
        }

        public bool mBCXuatChoBN_ThanhTrung
        {
            get
            {
                return _mBCXuatChoBN_ThanhTrung;
            }
            set
            {
                if (_mBCXuatChoBN_ThanhTrung == value)
                    return;
                _mBCXuatChoBN_ThanhTrung = value;
                NotifyOfPropertyChange(() => mBCXuatChoBN_ThanhTrung);
            }
        }

        public bool mBC15NgaySuDungThuoc
        {
            get
            {
                return _mBC15NgaySuDungThuoc;
            }
            set
            {
                if (_mBC15NgaySuDungThuoc == value)
                    return;
                _mBC15NgaySuDungThuoc = value;
                NotifyOfPropertyChange(() => mBC15NgaySuDungThuoc);
            }
        }

        private bool _mBCXuat = true;
        private bool _mBCXuat_VTYTTH = true;
        private bool _mBCXuat_Vaccine = true;
        private bool _mBCXuat_Blood = true;
        private bool _mBCXuat_VPP = true;
        private bool _mBCXuat_VTTH = true;
        private bool _mBCXuat_ThanhTrung = true;
        public bool mBCXuat
        {
            get
            {
                return _mBCXuat;
            }
            set
            {
                if (_mBCXuat == value)
                    return;
                _mBCXuat = value;
                NotifyOfPropertyChange(() => mBCXuat);
            }
        }

        public bool mBCXuat_VTYTTH
        {
            get
            {
                return _mBCXuat_VTYTTH;
            }
            set
            {
                if (_mBCXuat_VTYTTH == value)
                    return;
                _mBCXuat_VTYTTH = value;
                NotifyOfPropertyChange(() => mBCXuat_VTYTTH);
            }
        }

        public bool mBCXuat_Vaccine
        {
            get
            {
                return _mBCXuat_Vaccine;
            }
            set
            {
                if (_mBCXuat_Vaccine == value)
                    return;
                _mBCXuat_Vaccine = value;
                NotifyOfPropertyChange(() => mBCXuat_Vaccine);
            }
        }

        public bool mBCXuat_Blood
        {
            get
            {
                return _mBCXuat_Blood;
            }
            set
            {
                if (_mBCXuat_Blood == value)
                    return;
                _mBCXuat_Blood = value;
                NotifyOfPropertyChange(() => mBCXuat_Blood);
            }
        }

        public bool mBCXuat_VPP
        {
            get
            {
                return _mBCXuat_VPP;
            }
            set
            {
                if (_mBCXuat_VPP == value)
                    return;
                _mBCXuat_VPP = value;
                NotifyOfPropertyChange(() => mBCXuat_VPP);
            }
        }

        public bool mBCXuat_VTTH
        {
            get
            {
                return _mBCXuat_VTTH;
            }
            set
            {
                if (_mBCXuat_VTTH == value)
                    return;
                _mBCXuat_VTTH = value;
                NotifyOfPropertyChange(() => mBCXuat_VTTH);
            }
        }

        public bool mBCXuat_ThanhTrung
        {
            get
            {
                return _mBCXuat_ThanhTrung;
            }
            set
            {
                if (_mBCXuat_ThanhTrung == value)
                    return;
                _mBCXuat_ThanhTrung = value;
                NotifyOfPropertyChange(() => mBCXuat_ThanhTrung);
            }
        }

        private bool _mBCNhapXuatTon = true;
        private bool _mBCNhapXuatTon_VTYTTH = true;
        private bool _mBCNhapXuatTon_Vaccine = true;
        private bool _mBCNhapXuatTon_Blood = true;
        private bool _mBCNhapXuatTon_VPP = true;
        private bool _mBCNhapXuatTon_VTTH = true;
        private bool _mBCNhapXuatTon_ThanhTrung = true;
        public bool mBCNhapXuatTon
        {
            get
            {
                return _mBCNhapXuatTon;
            }
            set
            {
                if (_mBCNhapXuatTon == value)
                    return;
                _mBCNhapXuatTon = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon);
            }
        }

        public bool mBCNhapXuatTon_VTYTTH
        {
            get
            {
                return _mBCNhapXuatTon_VTYTTH;
            }
            set
            {
                if (_mBCNhapXuatTon_VTYTTH == value)
                    return;
                _mBCNhapXuatTon_VTYTTH = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_VTYTTH);
            }
        }

        public bool mBCNhapXuatTon_Vaccine
        {
            get
            {
                return _mBCNhapXuatTon_Vaccine;
            }
            set
            {
                if (_mBCNhapXuatTon_Vaccine == value)
                    return;
                _mBCNhapXuatTon_Vaccine = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_Vaccine);
            }
        }

        public bool mBCNhapXuatTon_Blood
        {
            get
            {
                return _mBCNhapXuatTon_Blood;
            }
            set
            {
                if (_mBCNhapXuatTon_Blood == value)
                    return;
                _mBCNhapXuatTon_Blood = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_Blood);
            }
        }

        public bool mBCNhapXuatTon_VPP
        {
            get
            {
                return _mBCNhapXuatTon_VPP;
            }
            set
            {
                if (_mBCNhapXuatTon_VPP == value)
                    return;
                _mBCNhapXuatTon_VPP = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_VPP);
            }
        }

        public bool mBCNhapXuatTon_VTTH
        {
            get
            {
                return _mBCNhapXuatTon_VTTH;
            }
            set
            {
                if (_mBCNhapXuatTon_VTTH == value)
                    return;
                _mBCNhapXuatTon_VTTH = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_VTTH);
            }
        }

        public bool mBCNhapXuatTon_ThanhTrung
        {
            get
            {
                return _mBCNhapXuatTon_ThanhTrung;
            }
            set
            {
                if (_mBCNhapXuatTon_ThanhTrung == value)
                    return;
                _mBCNhapXuatTon_ThanhTrung = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_ThanhTrung);
            }
        }

        private bool _mBCTheKho = true;
        private bool _mBCTheKho_VTYTTH = true;
        private bool _mBCTheKho_Vaccine = true;
        private bool _mBCTheKho_Blood = true;
        private bool _mBCTheKho_VPP = true;
        private bool _mBCTheKho_VTTH = true;
        private bool _mBCTheKho_ThanhTrung = true;
        public bool mBCTheKho
        {
            get
            {
                return _mBCTheKho;
            }
            set
            {
                if (_mBCTheKho == value)
                    return;
                _mBCTheKho = value;
                NotifyOfPropertyChange(() => mBCTheKho);
            }
        }

        public bool mBCTheKho_VTYTTH
        {
            get
            {
                return _mBCTheKho_VTYTTH;
            }
            set
            {
                if (_mBCTheKho_VTYTTH == value)
                    return;
                _mBCTheKho_VTYTTH = value;
                NotifyOfPropertyChange(() => mBCTheKho_VTYTTH);
            }
        }

        public bool mBCTheKho_Vaccine
        {
            get
            {
                return _mBCTheKho_Vaccine;
            }
            set
            {
                if (_mBCTheKho_Vaccine == value)
                    return;
                _mBCTheKho_Vaccine = value;
                NotifyOfPropertyChange(() => mBCTheKho_Vaccine);
            }
        }

        public bool mBCTheKho_Blood
        {
            get
            {
                return _mBCTheKho_Blood;
            }
            set
            {
                if (_mBCTheKho_Blood == value)
                    return;
                _mBCTheKho_Blood = value;
                NotifyOfPropertyChange(() => mBCTheKho_Blood);
            }
        }

        public bool mBCTheKho_VPP
        {
            get
            {
                return _mBCTheKho_VPP;
            }
            set
            {
                if (_mBCTheKho_VPP == value)
                    return;
                _mBCTheKho_VPP = value;
                NotifyOfPropertyChange(() => mBCTheKho_VPP);
            }
        }

        public bool mBCTheKho_VTTH
        {
            get
            {
                return _mBCTheKho_VTTH;
            }
            set
            {
                if (_mBCTheKho_VTTH == value)
                    return;
                _mBCTheKho_VTTH = value;
                NotifyOfPropertyChange(() => mBCTheKho_VTTH);
            }
        }

        public bool mBCTheKho_ThanhTrung
        {
            get
            {
                return _mBCTheKho_ThanhTrung;
            }
            set
            {
                if (_mBCTheKho_ThanhTrung == value)
                    return;
                _mBCTheKho_ThanhTrung = value;
                NotifyOfPropertyChange(() => mBCTheKho_ThanhTrung);
            }
        }

        private bool _mBCThuocSapHetHan = true;
        public bool mBCThuocSapHetHan
        {
            get
            {
                return _mBCThuocSapHetHan;
            }
            set
            {
                if (_mBCThuocSapHetHan == value)
                    return;
                _mBCThuocSapHetHan = value;
                NotifyOfPropertyChange(() => mBCThuocSapHetHan);
            }
        }

        private bool _gQLTinhHinhKhoa = true;
        private bool _mQLChiTieuNhanSu = true;
        private bool _mQLTinhHinhKhoa = true;
        public bool gQLTinhHinhKhoa
        {
            get
            {
                return _gQLTinhHinhKhoa;
            }
            set
            {
                if (_gQLTinhHinhKhoa == value)
                    return;
                _gQLTinhHinhKhoa = value;
                NotifyOfPropertyChange(() => gQLTinhHinhKhoa);
            }
        }

        public bool mQLChiTieuNhanSu
        {
            get
            {
                return _mQLChiTieuNhanSu;
            }
            set
            {
                if (_mQLChiTieuNhanSu == value)
                    return;
                _mQLChiTieuNhanSu = value;
                NotifyOfPropertyChange(() => mQLChiTieuNhanSu);
            }
        }

        public bool mQLTinhHinhKhoa
        {
            get
            {
                return _mQLTinhHinhKhoa;
            }
            set
            {
                if (_mQLTinhHinhKhoa == value)
                    return;
                _mQLTinhHinhKhoa = value;
                NotifyOfPropertyChange(() => mQLTinhHinhKhoa);
            }
        }
        //▲====: #007

        #region
        private void CanBangCmd_In(AllLookupValues.MedProductType V_MedProductType = AllLookupValues.MedProductType.THUOC, int ViewCase = 1)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IXuatNoiBoStoreDept>();
            VM.V_MedProductType = V_MedProductType;
            VM.strHienThi = Globals.TitleForm;
            VM.mXuatTraHang_Xem = mXuatTraHang_HoaChat_Xem;
            VM.mXuatTraHang_PhieuMoi = mXuatTraHang_HoaChat_PhieuMoi;
            VM.mXuatTraHang_XemIn = mXuatTraHang_HoaChat_XemIn;
            VM.mXuatTraHang_In = mXuatTraHang_HoaChat_In;
            VM.InitData();
            VM.ViewCase = ViewCase;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }
        public void XuatCanBangThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.THUOC, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.THUOC, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XuatCanBangYCuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.Y_CU, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.Y_CU, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void XuatCanBangHoaChatCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.HOA_CHAT, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.HOA_CHAT, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //--▼ 02/01/2021 DatTB
        public void XuatCanBangVTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.VTYT_TIEUHAO, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.VTYT_TIEUHAO, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XuatCanBangVaccineCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.TIEM_NGUA, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.TIEM_NGUA, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XuatCanBangMauCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.MAU, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.MAU, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XuatCanBangVPPCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.VAN_PHONG_PHAM, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.VAN_PHONG_PHAM, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XuatCanBangThanhTrungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.THANHTRUNG, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.THANHTRUNG, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XuatCanBangVTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.VATTU_TIEUHAO, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.VATTU_TIEUHAO, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //--▲ 02/01/2021 DatTB

        public void NhapCanBangThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.THUOC);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.THUOC);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapCanBangYCuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.Y_CU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.Y_CU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void NhapCanBangHoaChatCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.HOA_CHAT);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.HOA_CHAT);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //--▼ 02/01/2021 DatTB
        public void NhapCanBangVTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.VTYT_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.VTYT_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NhapCanBangVaccineCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.TIEM_NGUA);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.TIEM_NGUA);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NhapCanBangMauCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.MAU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.MAU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NhapCanBangVPPCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.VAN_PHONG_PHAM);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.VAN_PHONG_PHAM);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NhapCanBangThanhTrungCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.THANHTRUNG);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.THANHTRUNG);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NhapCanBangVTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe; //--30/12/2020 DatTB
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //--▲ 02/01/2021 DatTB
        public void NhapCanBangCmd_In(AllLookupValues.MedProductType V_MedProductType = AllLookupValues.MedProductType.THUOC)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptInwardBalance>();
            VM.V_MedProductType = (long)V_MedProductType;
            VM.SearchCriteria.IsInputMedDept = true;
            VM.strHienThi = Globals.TitleForm;
            VM.IsFromClinic = true;
            VM.mNhapHangTuKhoDuoc_Tim = mNhapHangTuKhoDuoc_HoaChat_Tim;
            VM.mNhapHangTuKhoDuoc_Them = mNhapHangTuKhoDuoc_HoaChat_Them;
            VM.mNhapHangTuKhoDuoc_XemIn = mNhapHangTuKhoDuoc_HoaChat_XemIn;
            VM.mNhapHangTuKhoDuoc_In = mNhapHangTuKhoDuoc_HoaChat_In;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        //▼====== #005
        private bool _mPhieuYeuCau_DinhDuong = true;
        private bool _mPhieuYeuCau_DinhDuong_Tim = true;
        private bool _mPhieuYeuCau_DinhDuong_Them = true;
        private bool _mPhieuYeuCau_DinhDuong_Xoa = true;
        private bool _mPhieuYeuCau_DinhDuong_XemIn = true;
        private bool _mPhieuYeuCau_DinhDuong_In = true;

        public bool mPhieuYeuCau_DinhDuong
        {
            get
            {
                return _mPhieuYeuCau_DinhDuong;
            }
            set
            {
                if (_mPhieuYeuCau_DinhDuong == value)
                    return;
                _mPhieuYeuCau_DinhDuong = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_DinhDuong);
            }
        }

        public bool mPhieuYeuCau_DinhDuong_Tim
        {
            get
            {
                return _mPhieuYeuCau_DinhDuong_Tim;
            }
            set
            {
                if (_mPhieuYeuCau_DinhDuong_Tim == value)
                    return;
                _mPhieuYeuCau_DinhDuong_Tim = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_DinhDuong_Tim);
            }
        }


        public bool mPhieuYeuCau_DinhDuong_Them
        {
            get
            {
                return _mPhieuYeuCau_DinhDuong_Them;
            }
            set
            {
                if (_mPhieuYeuCau_DinhDuong_Them == value)
                    return;
                _mPhieuYeuCau_DinhDuong_Them = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_DinhDuong_Them);
            }
        }


        public bool mPhieuYeuCau_DinhDuong_Xoa
        {
            get
            {
                return _mPhieuYeuCau_DinhDuong_Xoa;
            }
            set
            {
                if (_mPhieuYeuCau_DinhDuong_Xoa == value)
                    return;
                _mPhieuYeuCau_DinhDuong_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_DinhDuong_Xoa);
            }
        }


        public bool mPhieuYeuCau_DinhDuong_XemIn
        {
            get
            {
                return _mPhieuYeuCau_DinhDuong_XemIn;
            }
            set
            {
                if (_mPhieuYeuCau_DinhDuong_XemIn == value)
                    return;
                _mPhieuYeuCau_DinhDuong_XemIn = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_DinhDuong_XemIn);
            }
        }


        public bool mPhieuYeuCau_DinhDuong_In
        {
            get
            {
                return _mPhieuYeuCau_DinhDuong_In;
            }
            set
            {
                if (_mPhieuYeuCau_DinhDuong_In == value)
                    return;
                _mPhieuYeuCau_DinhDuong_In = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_DinhDuong_In);
            }
        }

        private void RequestDinhDuongNewCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var UnitVM = Globals.GetViewModel<IClinicDeptInPtReqForm_V2>();
            UnitVM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            UnitVM.strHienThi = Globals.TitleForm;
            //UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau_YCu_Tim;
            //UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau_YCu_Them;
            //UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau_YCu_Xoa;
            //UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau_YCu_XemIn;
            //UnitVM.mPhieuYeuCau_In = mPhieuYeuCau_YCu_In;
            UnitVM.mPhieuYeuCau_Tim = mPhieuYeuCau_DinhDuong_Tim;
            UnitVM.mPhieuYeuCau_Them = mPhieuYeuCau_DinhDuong_Them;
            UnitVM.mPhieuYeuCau_Xoa = mPhieuYeuCau_DinhDuong_Xoa;
            UnitVM.mPhieuYeuCau_XemIn = mPhieuYeuCau_DinhDuong_XemIn;
            UnitVM.mPhieuYeuCau_In = mPhieuYeuCau_DinhDuong_In;
            (module as Conductor<object>).ActivateItem(UnitVM);
            module.MainContent = UnitVM;
        }

        public void RequestDinhDuongNewCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3205_G1_LapPhLinhDDuong;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestDinhDuongNewCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestDinhDuongNewCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private bool _mNhapHangTuKhoDuoc_DinhDuong_Tim = true;
        private bool _mNhapHangTuKhoDuoc_DinhDuong_Them = true;
        private bool _mNhapHangTuKhoDuoc_DinhDuong_XemIn = true;
        private bool _mNhapHangTuKhoDuoc_DinhDuong_In = true;
        private bool _mNhapHangTuKhoDuoc_DinhDuong = true;
        public bool mNhapHangTuKhoDuoc_DinhDuong
        {
            get
            {
                return _mNhapHangTuKhoDuoc_DinhDuong;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_DinhDuong == value)
                    return;
                _mNhapHangTuKhoDuoc_DinhDuong = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_DinhDuong);
            }
        }

        public bool mNhapHangTuKhoDuoc_DinhDuong_Tim
        {
            get
            {
                return _mNhapHangTuKhoDuoc_DinhDuong_Tim;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_DinhDuong_Tim == value)
                    return;
                _mNhapHangTuKhoDuoc_DinhDuong_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_DinhDuong_Tim);
            }
        }


        public bool mNhapHangTuKhoDuoc_DinhDuong_Them
        {
            get
            {
                return _mNhapHangTuKhoDuoc_DinhDuong_Them;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_DinhDuong_Them == value)
                    return;
                _mNhapHangTuKhoDuoc_DinhDuong_Them = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_DinhDuong_Them);
            }
        }


        public bool mNhapHangTuKhoDuoc_DinhDuong_XemIn
        {
            get
            {
                return _mNhapHangTuKhoDuoc_DinhDuong_XemIn;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_DinhDuong_XemIn == value)
                    return;
                _mNhapHangTuKhoDuoc_DinhDuong_XemIn = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_DinhDuong_XemIn);
            }
        }


        public bool mNhapHangTuKhoDuoc_DinhDuong_In
        {
            get
            {
                return _mNhapHangTuKhoDuoc_DinhDuong_In;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_DinhDuong_In == value)
                    return;
                _mNhapHangTuKhoDuoc_DinhDuong_In = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_DinhDuong_In);
            }
        }
        private void FromMedToClinicDDuongCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptClinicInwardFromDrugDept>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            VM.SearchCriteria.IsInputMedDept = true;
            VM.strHienThi = Globals.TitleForm;
            VM.mNhapHangTuKhoDuoc_Tim = mNhapHangTuKhoDuoc_DinhDuong_Tim;
            VM.mNhapHangTuKhoDuoc_Them = mNhapHangTuKhoDuoc_DinhDuong_Them;
            VM.mNhapHangTuKhoDuoc_XemIn = mNhapHangTuKhoDuoc_DinhDuong_XemIn;
            VM.mNhapHangTuKhoDuoc_In = mNhapHangTuKhoDuoc_DinhDuong_In;

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void FromMedToClinicDDuongCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3219_G1_NhapDDuongTuKhoDuocKhoPhg;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FromMedToClinicDDuongCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FromMedToClinicDDuongCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private bool _mXuatTraHang_DDuong_Xem = true;
        private bool _mXuatTraHang_DDuong_PhieuMoi = true;
        private bool _mXuatTraHang_DDuong_XemIn = true;
        private bool _mXuatTraHang_DDuong_In = true;
        private bool _mXuatTraHang_DDuong = true;

        public bool mXuatTraHang_DDuong
        {
            get
            {
                return _mXuatTraHang_DDuong;
            }
            set
            {
                if (_mXuatTraHang_DDuong == value)
                    return;
                _mXuatTraHang_DDuong = value;
                NotifyOfPropertyChange(() => mXuatTraHang_DDuong);
            }
        }
        public bool mXuatTraHang_DDuong_Xem
        {
            get
            {
                return _mXuatTraHang_DDuong_Xem;
            }
            set
            {
                if (_mXuatTraHang_DDuong_Xem == value)
                    return;
                _mXuatTraHang_DDuong_Xem = value;
                NotifyOfPropertyChange(() => mXuatTraHang_DDuong_Xem);
            }
        }


        public bool mXuatTraHang_DDuong_PhieuMoi
        {
            get
            {
                return _mXuatTraHang_DDuong_PhieuMoi;
            }
            set
            {
                if (_mXuatTraHang_DDuong_PhieuMoi == value)
                    return;
                _mXuatTraHang_DDuong_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatTraHang_DDuong_PhieuMoi);
            }
        }


        public bool mXuatTraHang_DDuong_XemIn
        {
            get
            {
                return _mXuatTraHang_DDuong_XemIn;
            }
            set
            {
                if (_mXuatTraHang_DDuong_XemIn == value)
                    return;
                _mXuatTraHang_DDuong_XemIn = value;
                NotifyOfPropertyChange(() => mXuatTraHang_DDuong_XemIn);
            }
        }


        public bool mXuatTraHang_DDuong_In
        {
            get
            {
                return _mXuatTraHang_DDuong_In;
            }
            set
            {
                if (_mXuatTraHang_DDuong_In == value)
                    return;
                _mXuatTraHang_DDuong_In = value;
                NotifyOfPropertyChange(() => mXuatTraHang_DDuong_In);
            }
        }

        private void XuatTraDDuongCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IXuatNoiBoStoreDept>();
            VM.V_MedProductType = AllLookupValues.MedProductType.NUTRITION;
            VM.strHienThi = Globals.TitleForm;

            VM.mXuatTraHang_Xem = mXuatTraHang_DDuong_Xem;
            VM.mXuatTraHang_PhieuMoi = mXuatTraHang_DDuong_PhieuMoi;
            VM.mXuatTraHang_XemIn = mXuatTraHang_DDuong_XemIn;
            VM.mXuatTraHang_In = mXuatTraHang_DDuong_In;

            VM.InitData();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatTraDDuongCmd()
        {
            Globals.TitleForm = "XUẤT DINH DƯỠNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatTraDDuongCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatTraDDuongCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //XUAT CHO BN
        private bool _mXuatChoBenhNhan_DDuong_Xem = true;
        private bool _mXuatChoBenhNhan_DDuong_PhieuMoi = true;
        private bool _mXuatChoBenhNhan_DDuong_XemIn = true;
        private bool _mXuatChoBenhNhan_DDuong_In = true;
        private bool _mXuatChoBenhNhan_DDuong = true;

        public bool mXuatChoBenhNhan_DDuong
        {
            get
            {
                return _mXuatChoBenhNhan_DDuong;
            }
            set
            {
                if (_mXuatChoBenhNhan_DDuong == value)
                    return;
                _mXuatChoBenhNhan_DDuong = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_DDuong);
            }
        }

        public bool mXuatChoBenhNhan_DDuong_Xem
        {
            get
            {
                return _mXuatChoBenhNhan_DDuong_Xem;
            }
            set
            {
                if (_mXuatChoBenhNhan_DDuong_Xem == value)
                    return;
                _mXuatChoBenhNhan_DDuong_Xem = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_DDuong_Xem);
            }
        }


        public bool mXuatChoBenhNhan_DDuong_PhieuMoi
        {
            get
            {
                return _mXuatChoBenhNhan_DDuong_PhieuMoi;
            }
            set
            {
                if (_mXuatChoBenhNhan_DDuong_PhieuMoi == value)
                    return;
                _mXuatChoBenhNhan_DDuong_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_DDuong_PhieuMoi);
            }
        }


        public bool mXuatChoBenhNhan_DDuong_XemIn
        {
            get
            {
                return _mXuatChoBenhNhan_DDuong_XemIn;
            }
            set
            {
                if (_mXuatChoBenhNhan_DDuong_XemIn == value)
                    return;
                _mXuatChoBenhNhan_DDuong_XemIn = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_DDuong_XemIn);
            }
        }


        public bool mXuatChoBenhNhan_DDuong_In
        {
            get
            {
                return _mXuatChoBenhNhan_DDuong_In;
            }
            set
            {
                if (_mXuatChoBenhNhan_DDuong_In == value)
                    return;
                _mXuatChoBenhNhan_DDuong_In = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_DDuong_In);
            }
        }

        private void XuatDDuongChoBNCmd_In(long aRegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IOutwardToPatient_V2>();
            VM.V_MedProductType = AllLookupValues.MedProductType.NUTRITION;
            VM.strHienThi = Globals.TitleForm;
            VM.RequireDoctorAndDate = Globals.ServerConfigSection.ClinicDeptElements.RequireDoctorAndDateForMat;
            VM.mXuatChoBenhNhan_Xem = mXuatChoBenhNhan_DDuong_Xem;
            VM.mXuatChoBenhNhan_PhieuMoi = mXuatChoBenhNhan_DDuong_PhieuMoi;
            VM.mXuatChoBenhNhan_XemIn = mXuatChoBenhNhan_DDuong_XemIn;
            VM.mXuatChoBenhNhan_In = mXuatChoBenhNhan_DDuong_In;
            VM.V_RegistrationType = aRegistrationType;
            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void XuatDDuongChoBNCmd()
        {
            Globals.TitleForm = "XUẤT DINH DƯỠNG CHO BỆNH NHÂN";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatDDuongChoBNCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatDDuongChoBNCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XuatDDuongChoBNNTCmd()
        {
            Globals.TitleForm = string.Format("XUẤT DINH DƯỠNG CHO BN NGOẠI TRÚ", eHCMSResources.Z3206_G1_DinhDuong);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                XuatDDuongChoBNCmd_In((long)AllLookupValues.RegistrationType.NGOAI_TRU);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        XuatDDuongChoBNCmd_In((long)AllLookupValues.RegistrationType.NGOAI_TRU);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DTNhapXuatTonDDuongCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
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

        private bool _mBCNhapXuatTon_DDuong = true;
        private bool _mBaoCaoXuatNhapTon_DDuong_XemIn = true;
        private bool _mBaoCaoXuatNhapTon_DDuong_KetChuyen = true;

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
        public bool mBCNhapXuatTon_DDuong
        {
            get
            {
                return _mBCNhapXuatTon_DDuong;
            }
            set
            {
                if (_mBCNhapXuatTon_DDuong == value)
                    return;
                _mBCNhapXuatTon_DDuong = value;
                NotifyOfPropertyChange(() => mBCNhapXuatTon_DDuong);
            }
        }
        private void NhapXuatTonDDuongCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            VM.mBaoCaoXuatNhapTon_XemIn = mBaoCaoXuatNhapTon_DDuong_XemIn;
            VM.mBaoCaoXuatNhapTon_KetChuyen = mBaoCaoXuatNhapTon_DDuong_XemIn;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void NhapXuatTonDDuongCmd()
        {
            Globals.TitleForm = eHCMSResources.Z0773_G1_BCNXTYCu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapXuatTonDDuongCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapXuatTonDDuongCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private bool _mBaoCaoTheKho_DDuong_Xem = true;
        private bool _mBaoCaoTheKho_DDuong_In = true;
        private bool _mBCTheKho_DDuong = true;

        public bool mBCTheKho_DDuong
        {
            get
            {
                return _mBCTheKho_DDuong;
            }
            set
            {
                if (_mBCTheKho_DDuong == value)
                    return;
                _mBCTheKho_DDuong = value;
                NotifyOfPropertyChange(() => mBCTheKho_DDuong);
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

        private void TheKhoDDuongCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IClinicDeptTheKho>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            VM.strHienThi = Globals.TitleForm;
            VM.mBaoCaoTheKho_Xem = mBaoCaoTheKho_DDuong_Xem;
            VM.mBaoCaoTheKho_In = mBaoCaoTheKho_DDuong_In;
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TheKhoDDuongCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3223_G1_TheKhoDDuong;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TheKhoDDuongCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TheKhoDDuongCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void KKDinhDuongCmd_In()
        {
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.NUTRITION;
            VM.strHienThi = Globals.TitleForm;
            VM.mKiemKe_Tim = mKiemKe_YCu_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_YCu_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_YCu_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_YCu_XemIn;
            VM.mKiemKe_KhoaKho = mKiemKe_KhoaKho;
            VM.mKiemKe_MoKho = mKiemKe_MoKho;
            VM.mKiemKe_KhoaTatCa = mKiemKe_KhoaTatCa;

            VM.InitData();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKDinhDuongCmd()
        {
            Globals.TitleForm = "TÍNH TỒN ĐẦU KỲ DINH DƯỠNG CỦA KHOA PHÒNG";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                KKDinhDuongCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KKDinhDuongCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void XuatCanBangDinhDuongCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3097_G1_GiamKiemKe;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanBangCmd_In(AllLookupValues.MedProductType.NUTRITION, 1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanBangCmd_In(AllLookupValues.MedProductType.NUTRITION, 1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void NhapCanBangDinhDuongCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3098_G1_TangKiemKe;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NhapCanBangCmd_In(AllLookupValues.MedProductType.NUTRITION);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NhapCanBangCmd_In(AllLookupValues.MedProductType.NUTRITION);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #005

        //▼====== #006
        private bool _mKiemKe_MoKho = true;
        private bool _mKiemKe_KhoaKho = true;
        private bool _mKiemKe_KhoaTatCa = true;

        public bool mKiemKe_MoKho
        {
            get
            {
                return _mKiemKe_MoKho;
            }
            set
            {
                if (_mKiemKe_MoKho == value)
                    return;
                _mKiemKe_MoKho = value;
                NotifyOfPropertyChange(() => mKiemKe_MoKho);
            }
        }


        public bool mKiemKe_KhoaKho
        {
            get
            {
                return _mKiemKe_KhoaKho;
            }
            set
            {
                if (_mKiemKe_KhoaKho == value)
                    return;
                _mKiemKe_KhoaKho = value;
                NotifyOfPropertyChange(() => mKiemKe_KhoaKho);
            }
        }


        public bool mKiemKe_KhoaTatCa
        {
            get
            {
                return _mKiemKe_KhoaTatCa;
            }
            set
            {
                if (_mKiemKe_KhoaTatCa == value)
                    return;
                _mKiemKe_KhoaTatCa = value;
                NotifyOfPropertyChange(() => mKiemKe_KhoaTatCa);
            }
        }
        //▲====: #006
        #endregion
        //▼====: #008
        public void RequestVTTHTemplateCmd()
        {
            Globals.TitleForm = string.Format(eHCMSResources.Z2921_G1_TaoMauLinh0, eHCMSResources.Z2521_G1_VTTH);
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RequestTemplateCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RequestTemplateCmd_In((long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #008

        private bool _mBCMauSapHetHanDung = true;
        private bool _mTreatmentStatisticsByDept = true;
        public bool mBCMauSapHetHanDung
        {
            get
            {
                return _mBCMauSapHetHanDung;
            }
            set
            {
                if (_mBCMauSapHetHanDung == value)
                    return;
                _mBCMauSapHetHanDung = value;
                NotifyOfPropertyChange(() => mBCMauSapHetHanDung);
            }
        }

        public bool mTreatmentStatisticsByDept
        {
            get
            {
                return _mTreatmentStatisticsByDept;
            }
            set
            {
                if (_mTreatmentStatisticsByDept == value)
                    return;
                _mTreatmentStatisticsByDept = value;
                NotifyOfPropertyChange(() => mTreatmentStatisticsByDept);
            }
        }

        //▼==== #011
        private bool _mBCXuatSDThuocCanQuang = true;
        public bool mBCXuatSDThuocCanQuang
        {
            get
            {
                return _mBCXuatSDThuocCanQuang;
            }
            set
            {
                if (_mBCXuatSDThuocCanQuang == value)
                    return;
                _mBCXuatSDThuocCanQuang = value;
                NotifyOfPropertyChange(() => mBCXuatSDThuocCanQuang);
            }
        }

        private void BCXuatSDThuocCanQuang_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IStoreDeptHome>();
            var reportVm = Globals.GetViewModel<ICommonReportForAccountant>();
            reportVm.eItem = ReportName.BCXuatSDThuocCanQuang;
            reportVm.mXemIn = false;
            reportVm.ChonKho = true;
            reportVm.ViewBy = false;
            reportVm.IsPurpose = false;
            reportVm.IsShowGroupReportType = false;
            reportVm.GetListStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCXuatSDThuocCanQuangCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3298_G1_BCXuatSDThuocCanQuang;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCXuatSDThuocCanQuang_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCXuatSDThuocCanQuang_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲==== #011
    }
}
