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
*/
namespace aEMR.StoreDept.Menu.ViewModels
{
    [Export(typeof(IStoreDeptLeftMenu))]
    public class StoreDeptLeftMenuViewModel : Conductor<object>, IStoreDeptLeftMenu
    {
        [ImportingConstructor]
        public StoreDeptLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
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
            resetMenuColor();
            OutwardDrugTemplate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            OutwardMedicalMaterialTemplate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            OutwardChemicalTemplate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
        #endregion


        #region Phieu Linh Thuoc Member

        private void RequestThuocCmdNew_In()
        {
            resetMenuColor();
            RequestThuocNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            RequestYCuNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            RequestHoaChatNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            RefGenMedProductDetails_Drug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            RefGenMedProductDetails_MedicalDevices.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            RefGenMedProductDetails_Chemical.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            StoreDeptSellingItemPrices.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            StoreDeptSellingItemPrices_Medical.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            StoreDeptSellingItemPrices_Chemical.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            FromMedToClinicThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            FromMedToClinicYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            FromMedToClinicHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            Globals.TitleForm = "NHẬP HÓA CHẤT TỪ KHO DƯỢC CHO KHO PHÒNG";
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

        #endregion

        #region Xuat Tra Member
        private void XuatTraThuocCmd_In()
        {
            resetMenuColor();
            XuatTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            XuatTraYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            XuatTraHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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

        #endregion

        #region Xuat Cho Benh Nhan Member
       
        private void XuatThuocChoBNCmd_In()
        {
            resetMenuColor();
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

        private void XuatYCuChoBNCmd_In()
        {
            resetMenuColor(); 
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
            resetMenuColor(); 
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
            resetMenuColor();
            KKThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.strHienThi = Globals.TitleForm;

            VM.mKiemKe_Tim = mKiemKe_Thuoc_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_Thuoc_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_Thuoc_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_Thuoc_XemIn;

            VM.InitData();
            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKThuocCmd()
        {
            Globals.TitleForm = "KIỂM KÊ THUỐC CỦA KHOA PHÒNG";
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
            resetMenuColor();
            KKYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            VM.strHienThi = Globals.TitleForm;
            VM.mKiemKe_Tim = mKiemKe_YCu_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_YCu_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_YCu_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_YCu_XemIn;

            VM.InitData();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKYCuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1823_G1_KKeYCuKhoaPhg;
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
            resetMenuColor();
            KKHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var VM = Globals.GetViewModel<IStoreDeptStockTakes>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
            VM.strHienThi = Globals.TitleForm;
            VM.mKiemKe_Tim = mKiemKe_HoaChat_Tim;
            VM.mKiemKe_ThemMoi = mKiemKe_HoaChat_ThemMoi;
            VM.mKiemKe_XuatExcel = mKiemKe_HoaChat_XuatExcel;
            VM.mKiemKe_XemIn = mKiemKe_HoaChat_XemIn;

            VM.InitData();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void KKHoaChatCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1824_G1_KKeHChatKhoaPhg;
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

        #endregion

        #region Thong Ke Member

        private void TKThuocCmd_In()
        {
            resetMenuColor();
            TKThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            TKYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            TKHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            NhapXuatTonThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            NhapXuatTonYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            NhapXuatTonHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
        #endregion

        #region The Kho Member
        private void TheKhoThuocCmd_In()
        {
            resetMenuColor();
            TheKhoThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            TheKhoYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            TheKhoHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();

            ReportOutwardMed.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();

            ReportOutwardMat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();

            ReportOutwardLab.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            BC15Day.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
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
            resetMenuColor();
            XuatThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            XuatYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
            resetMenuColor();
            XuatHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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

        #endregion

        #endregion

        #region load

        public Button OutwardDrugTemplate { get; set; }
        public Button OutwardMedicalMaterialTemplate { get; set; }
        public Button OutwardChemicalTemplate { get; set; }


        public Button RequestThuocNew { get; set; }
        public Button RequestYCuNew { get; set; }
        public Button RequestHoaChatNew { get; set; }
        public Button FollowThuoc { get; set; }

        public Button FromMedToClinicThuoc { get; set; }
        public Button FromMedToClinicYCu { get; set; }
        public Button FromMedToClinicHoaChat { get; set; }
        public Button XuatTraThuoc { get; set; }
        public Button XuatTraYCu { get; set; }
        public Button XuatTraHoaChat { get; set; }
        public Button XuatThuocChoBN { get; set; }
        public Button XuatYCuChoBN { get; set; }
        public Button XuatHoaChatChoBN { get; set; }
        public Button XuatThuoc_CasualOrPreOp { get; set; }
        public Button XuatYCu_CasualOrPreOp { get; set; }
        public Button XuatHoaChat_CasualOrPreOp { get; set; }

        public Button KKThuoc { get; set; }
        public Button KKYCu { get; set; }
        public Button KKHoaChat { get; set; }
        public Button TKThuoc { get; set; }
        public Button TKYCu { get; set; }
        public Button TKHoaChat { get; set; }
        public Button ReportOutwardMed { get; set; }
        public Button ReportOutwardMat { get; set; }
        public Button ReportOutwardLab { get; set; }
        public Button XuatThuoc { get; set; }
        public Button XuatYCu { get; set; }
        public Button XuatHoaChat { get; set; }
        public Button NhapXuatTonThuoc { get; set; }
        public Button NhapXuatTonYCu { get; set; }
        public Button NhapXuatTonHoaChat { get; set; }
        public Button TheKhoThuoc { get; set; }
        public Button TheKhoYCu { get; set; }
        public Button TheKhoHoaChat { get; set; }
        public Button RefGenMedProductDetails_Drug { get; set; }
        public Button RefGenMedProductDetails_MedicalDevices { get; set; }
        public Button RefGenMedProductDetails_Chemical { get; set; }
        public Button StoreDeptSellingItemPrices { get; set; }
        public Button StoreDeptSellingItemPrices_Medical { get; set; }
        public Button StoreDeptSellingItemPrices_Chemical { get; set; }
        public Button UpdateRequiredNumber { get; set; }
        public Button UpdatePresenceDaily { get; set; }
        public Button PatientManagement { get; set; }
        public Button BC15Day { get; set; }

        public void OutwardDrugTemplateCmd_Loaded(object sender)
        {
            OutwardDrugTemplate = sender as Button;
            OutwardDrugTemplate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void OutwardMedicalMaterialTemplateCmd_Loaded(object sender)
        {
            OutwardMedicalMaterialTemplate = sender as Button;
            OutwardMedicalMaterialTemplate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void OutwardChemicalTemplateCmd_Loaded(object sender)
        {
            OutwardChemicalTemplate = sender as Button;
            OutwardChemicalTemplate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void RequestThuocNewCmd_Loaded(object sender)
        {
            RequestThuocNew = sender as Button;
            RequestThuocNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void RequestYCuNewCmd_Loaded(object sender)
        {
            RequestYCuNew = sender as Button;
            RequestYCuNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void RequestHoaChatNewCmd_Loaded(object sender)
        {
            RequestHoaChatNew = sender as Button;
            RequestHoaChatNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //==== #001 ====
        public void FollowThuocCmd_Loaded(object sender)
        {
            FollowThuoc = sender as Button;
            FollowThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //==== #001 ====

        public void FromMedToClinicThuocCmd_Loaded(object sender)
        {
            FromMedToClinicThuoc = sender as Button;
            FromMedToClinicThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void FromMedToClinicYCuCmd_Loaded(object sender)
        {
            FromMedToClinicYCu = sender as Button;
            FromMedToClinicYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void FromMedToClinicHoaChatCmd_Loaded(object sender)
        {
            FromMedToClinicHoaChat = sender as Button;
            FromMedToClinicHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void XuatTraThuocCmd_Loaded(object sender)
        {
            XuatTraThuoc = sender as Button;
            XuatTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void XuatTraYCuCmd_Loaded(object sender)
        {
            XuatTraYCu = sender as Button;
            XuatTraYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void XuatTraHoaChatCmd_Loaded(object sender)
        {
            XuatTraHoaChat = sender as Button;
            XuatTraHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void XuatThuocChoBNCmd_Loaded(object sender)
        {
            XuatThuocChoBN = sender as Button;
            XuatThuocChoBN.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void XuatYCuChoBNCmd_Loaded(object sender)
        {
            XuatYCuChoBN = sender as Button;
            XuatYCuChoBN.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void XuatHoaChatChoBNCmd_Loaded(object sender)
        {
            XuatHoaChatChoBN = sender as Button;
            XuatHoaChatChoBN.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void XuatThuoc_CasualOrPreOpCmd_Loaded(object sender)
        {
            XuatThuoc_CasualOrPreOp = sender as Button;
            XuatThuoc_CasualOrPreOp.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void XuatYCu_CasualOrPreOpCmd_Loaded(object sender)
        {
            XuatYCu_CasualOrPreOp = sender as Button;
            XuatYCu_CasualOrPreOp.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void XuatHoaChat_CasualOrPreOpCmd_Loaded(object sender)
        {
            XuatHoaChat_CasualOrPreOp = sender as Button;
            XuatHoaChat_CasualOrPreOp.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void KKThuocCmd_Loaded(object sender)
        {
            KKThuoc = sender as Button;
            KKThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void KKYCuCmd_Loaded(object sender)
        {
            KKYCu = sender as Button;
            KKYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void KKHoaChatCmd_Loaded(object sender)
        {
            KKHoaChat = sender as Button;
            KKHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void TKThuocCmd_Loaded(object sender)
        {
            TKThuoc = sender as Button;
            TKThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TKYCuCmd_Loaded(object sender)
        {
            TKYCu = sender as Button;
            TKYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TKHoaChatCmd_Loaded(object sender)
        {
            TKHoaChat = sender as Button;
            TKHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ReportOutwardMedCmd_Loaded(object sender)
        {
            ReportOutwardMed = sender as Button;
            ReportOutwardMed.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ReportOutwardMatCmd_Loaded(object sender)
        {
            ReportOutwardMat = sender as Button;
            ReportOutwardMat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ReportOutwardLabCmd_Loaded(object sender)
        {
            ReportOutwardLab = sender as Button;
            ReportOutwardLab.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
       
        public void XuatThuocCmd_Loaded(object sender)
        {
            XuatThuoc = sender as Button;
            XuatThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void XuatYCuCmd_Loaded(object sender)
        {
            XuatYCu = sender as Button;
            XuatYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void XuatHoaChatCmd_Loaded(object sender)
        {
            XuatHoaChat = sender as Button;
            XuatHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void NhapXuatTonThuocCmd_Loaded(object sender)
        {
            NhapXuatTonThuoc = sender as Button;
            NhapXuatTonThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void NhapXuatTonYCuCmd_Loaded(object sender)
        {
            NhapXuatTonYCu = sender as Button;
            NhapXuatTonYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void NhapXuatTonHoaChatCmd_Loaded(object sender)
        {
            NhapXuatTonHoaChat = sender as Button;
            NhapXuatTonHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TheKhoThuocCmd_Loaded(object sender)
        {
            TheKhoThuoc = sender as Button;
            TheKhoThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TheKhoYCuCmd_Loaded(object sender)
        {
            TheKhoYCu = sender as Button;
            TheKhoYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TheKhoHoaChatCmd_Loaded(object sender)
        {
            TheKhoHoaChat = sender as Button;
            TheKhoHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void RefGenMedProductDetails_DrugMgnt_Loaded(object sender)
        {
            RefGenMedProductDetails_Drug = sender as Button;
            RefGenMedProductDetails_Drug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void RefGenMedProductDetails_MedicalDevicesMgnt_Loaded(object sender)
        {
            RefGenMedProductDetails_MedicalDevices = sender as Button;
            RefGenMedProductDetails_MedicalDevices.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void RefGenMedProductDetails_ChemicalMgnt_Loaded(object sender)
        {
            RefGenMedProductDetails_Chemical = sender as Button;
            RefGenMedProductDetails_Chemical.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void StoreDeptSellingItemPrices_Mgnt_Loaded(object sender)
        {
            StoreDeptSellingItemPrices = sender as Button;
            StoreDeptSellingItemPrices.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void StoreDeptSellingItemPrices_Medical_Mgnt_Loaded(object sender)
        {
            StoreDeptSellingItemPrices_Medical = sender as Button;
            StoreDeptSellingItemPrices_Medical.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void StoreDeptSellingItemPrices_Chemical_Mgnt_Loaded(object sender)
        {
            StoreDeptSellingItemPrices_Chemical = sender as Button;
            StoreDeptSellingItemPrices_Chemical.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void UpdateRequiredNumberLoad(object sender)
        {
            UpdateRequiredNumber = sender as Button;
            UpdateRequiredNumber.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void UpdatePresenseDailyLoad(object sender)
        {
            UpdatePresenceDaily = sender as Button;
            UpdatePresenceDaily.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void PatientManagementLoad(object sender)
        {
            PatientManagement = sender as Button;
            PatientManagement.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        //------- 28/11/2017 DPT Baocao 15 ngày
        public void BC15Day_Loaded(object sender)
        {
            BC15Day = sender as Button;
            BC15Day.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        #endregion

        public void resetMenuColor()
        {
            if (OutwardDrugTemplate != null)
            {
                OutwardDrugTemplate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (OutwardMedicalMaterialTemplate != null)
            {
                OutwardMedicalMaterialTemplate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (OutwardChemicalTemplate != null)
            {
                OutwardChemicalTemplate.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }

            if (RequestThuocNew != null)
            {
                RequestThuocNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (RequestYCuNew != null)
            {
                RequestYCuNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (RequestHoaChatNew != null)
            {
                RequestHoaChatNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (FromMedToClinicThuoc != null)
            {
                FromMedToClinicThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (FromMedToClinicYCu != null)
            {
                FromMedToClinicYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (FromMedToClinicHoaChat != null)
            {
                FromMedToClinicHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (XuatTraThuoc != null)
            {
                XuatTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (XuatTraYCu != null)
            {
                XuatTraYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (XuatTraHoaChat != null)
            {
                XuatTraHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (XuatThuocChoBN != null)
            {
                XuatThuocChoBN.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (XuatYCuChoBN != null)
            {
                XuatYCuChoBN.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (XuatHoaChatChoBN != null)
            {
                XuatHoaChatChoBN.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];           
            }
            if (KKThuoc != null)
            {
                KKThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (KKYCu != null)
            {
                KKYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (KKHoaChat != null)
            {
                KKHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TKThuoc != null)
            {
                TKThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TKYCu != null)
            {
                TKYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TKHoaChat != null)
            {
                TKHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ReportOutwardMed != null)
            {
                ReportOutwardMed.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ReportOutwardMat != null)
            {
                ReportOutwardMat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ReportOutwardLab != null)
            {
                ReportOutwardLab.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (XuatThuoc != null)
            {
                XuatThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (XuatYCu != null)
            {
                XuatYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (XuatHoaChat != null)
            {
                XuatHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (NhapXuatTonThuoc != null)
            {
                NhapXuatTonThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (NhapXuatTonYCu != null)
            {
                NhapXuatTonYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (NhapXuatTonHoaChat != null)
            {
                NhapXuatTonHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TheKhoThuoc != null)
            {
                TheKhoThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TheKhoYCu != null)
            {
                TheKhoYCu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TheKhoHoaChat != null)
            {
                TheKhoHoaChat.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (UpdatePresenceDaily != null)
            {
                UpdatePresenceDaily.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (UpdateRequiredNumber != null)
            {
                UpdateRequiredNumber.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (PatientManagement != null)
            {
                PatientManagement.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (BC15Day != null)
            {
                PatientManagement.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
          

            //==== #001 ====
            if (FollowThuoc != null)
            {
                FollowThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            //==== #001 ====
            //RefGenMedProductDetails_Drug.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            //RefGenMedProductDetails_MedicalDevices.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            //RefGenMedProductDetails_Chemical.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            //StoreDeptSellingItemPrices.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            //StoreDeptSellingItemPrices_Medical.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            //StoreDeptSellingItemPrices_Chemical.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

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
            resetMenuColor();
            if (IsUpdateRequiredNumber)
            {
                UpdateRequiredNumber.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            }
            else
            {
                UpdatePresenceDaily.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            }
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
            resetMenuColor();
            PatientManagement.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<IStoreDeptHome>();
            var PatientManagementVm = Globals.GetViewModel<IPatientManagement>();
            (module as Conductor<object>).ActivateItem(PatientManagementVm);
            module.MainContent = PatientManagementVm;
        }
        //==== #001 //====
        private void FollowThuocCmd_In()
        {
            resetMenuColor();
            FollowThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

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
    }
}

