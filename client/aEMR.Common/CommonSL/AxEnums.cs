using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
/*
 * 20181121 #001 TTM: Tạo mới enum để phục vụ việc chia trường hợp Out standing task nội trú.
 * 20220319 #002 QTD: Thêm enum Outstandingtask cho màn hình Thu tiền tạm ứng/ Đề nghi tạm ứng
 * 20221020 #003 QTD: Thêm enum Outstandingtask cho màn hìn Tính lại bill viện phí
 */
namespace aEMR.Common
{
    public enum FormOperation
    {
        AddNew = 1,
        Edit = 2,
        ReadOnly = 3,
        None = 4
    }
    public enum FormState
    {
        NONE = 0,
        NEW = 1,
        EDIT = 2,
        READONLY = 3,
        SUBMITTED = 4,
        REJECTED = 5,
        ACCEPTED = 6
    }
    public enum HomeModuleActive
    {
        NONE=0,
        DANGKY = 1,
        KHAMBENH = 2,
        XEMTTKHAMBENH = 3,
        HENBENH = 4,
        XETNGHIEM = 5,
        SIEUAM = 6,
        NHATHUOC = 7,
        KHOADUOC = 8,
        KHONOITRU = 9,
        TRANSACTION = 10,
        CAUHINH = 11,
        VATTU = 12,
        QUANLYPHONGKHAM = 13,
        QUANLYNGUOIDUNG = 14,
        CAUHINHHETHONG = 15,
        YEUCAUCHUNG = 16,
    }
    public enum LeftModuleActive
    {
        NONE = 0,
        KHAMBENH_NONE = 1,
        KHAMBENH_GENERAL = 2,
        KHAMBENH_THONGTINCHUNG = 3,
        KHAMBENH_TONGQUAT = 4,
        KHAMBENH_CHANDOAN = 5,
        KHAMBENH_RATOA = 6,
        KHAMBENH_LSBENHAN = 7,
        KHAMBENH_BKCTKHAMBENH = 8,
        KHAMBENH_CLS_PHIEUYEUCAU = 9,
        KHAMBENH_HENCLS_HENCLS = 10,

        KHAMBENH_QUANLY_THONGTIN_BN = 11,       

        KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM = 30,
        KHAMBENH_CLS_PHIEUYEUCAUHINHANH = 31,
        KHAMBENH_CLS_KETQUAXETNGHIEM = 32,
        KHAMBENH_CLS_KETQUAHINHANH = 33,
        KHAMBENH_CLS_NGOAIVIEN_XETNGHIEM = 34,
        KHAMBENH_CLS_NGOAIVIEN_HINHANH = 35,
        KHAMBENH_CHANDOAN_NOITRU = 36,
        KHAMBENH_RATOA_XUATVIEN = 37,

        KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM_NT = 40,
        KHAMBENH_CLS_PHIEUYEUCAUHINHANH_NT = 41,
        KHAMBENH_CLS_KETQUAXETNGHIEM_NT = 42,
        KHAMBENH_CLS_KETQUAHINHANH_NT = 43,

        DANGKY_NONE = 11,
        DANGKY_GENERAL = 12,
        DANGKY_NGOAITRU_DKDICHVU = 13,
        DANGKY_NGOAITRU_NBBAOHIEM = 14,
        DANGKY_NGOAITRU_TINHTIEN = 15,
        DANGKY_NGOAITRU_THONGKEDANGKY = 16,

        DANGKY_NOITRU_NHANBENHNOITRU = 17,
        DANGKY_NOITRU_NHANBENHCAPCUU = 18,
        DANGKY_NOITRU_NHAPVIEN = 19,
        DANGKY_NOITRU_XACNHANLAIBH = 20,
        DANGKY_NOITRU_TOABILL = 21,
        DANGKY_NOITRU_TINHTIENNOITRU = 22,
        DANGKY_NOITRU_CHUYENKHOA = 23,
        DANGKY_NOITRU_XUATVIEN = 24,
        DANGKY_NOITRU_DENGHITAMUNG = 25,
        DANGKY_NOITRU_TAMUNG = 26,
        DANGKY_NOITRU_MAU02 = 27,

        KHOADUOC_NHAPHANG_TIMPHIEUNHAP = 38,
        KHOADUOC_PHANBOPHI_TIMPHIEUPHANBO = 39,
        KHAMBENH_CHANDOAN_RATOA = 44,

        //▼====== #001:
        KHAMBENH_THONGTINCHUNG_NOITRU = 45,
        XUATVIEN = 46,
        //▲====== #001:
    }

    public enum eLeftMenuByPTType
    {
        NONE = 0,
        OUT_PT = 1,
        IN_PT = 2,
        BOTH = 3,
    }

    //▼====== #001:
    public enum SetOutStandingTask
    {
        KHAMBENH = 1,
        QUANLY_BENHNHAN_NOITRU = 2,
        TAO_BILL_VP = 3, 
        XUATTHUOC = 4,
        THANHTOAN = 5,
        RATOA = 6,
        PHIEU_YEU_CAU_CLS = 7,
        PHIEU_YEU_CAU_XET_NGHIEM = 8,

        XUATVIEN = 10,
        DIEU_TRI_NHIEM_KHUAN = 11,
        XACNHAN_BHYT = 12,
        //▼====== #002
        THUTIEN_TAMUNG = 13,
        DENGHI_TAMUNG = 14,
        //▲====== #002
        //▼====== #003
        TINH_LAI_BILL_VP = 15
        //▲====== #003
    }
    //▲====== #001:

    public enum eFireBookingBedEvent
    {
        NONE = 0,
        AcceptChangeDeptView = 1 //Bắn event về view chấp nhận chuyển khoa.
    }
}
