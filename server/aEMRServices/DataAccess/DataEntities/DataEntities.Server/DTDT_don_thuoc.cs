using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
/*
* 20221203 #001 BLQ: Create entity
*/

namespace DataEntities
{
    public partial class DTDT_don_thuoc : EntityBase
    {
        #region Primitive Properties
        public long id_don_thuoc
        {
            get
            {
                return _id_don_thuoc;
            }
            set
            {
                _id_don_thuoc = value;
                RaisePropertyChanged("id_don_thuoc");
            }
        }
        private long _id_don_thuoc;

        [DataMemberAttribute()]
        public string ma_don_thuoc
        {
            get
            {
                return _ma_don_thuoc;
            }
            set
            {
                _ma_don_thuoc = value;
                RaisePropertyChanged("ma_don_thuoc");
            }
        }
        private string _ma_don_thuoc;

        public string ma_don_thuoc_co_so_kcb
        {
            get
            {
                return _ma_don_thuoc_co_so_kcb;
            }
            set
            {
                _ma_don_thuoc_co_so_kcb = value;
                RaisePropertyChanged("ma_don_thuoc_co_so_kcb");
            }
        }
        private string _ma_don_thuoc_co_so_kcb;

        public string ma_co_so
        {
            get
            {
                return _ma_co_so;
            }
            set
            {
                _ma_co_so = value;
                RaisePropertyChanged("ma_co_so");
            }
        }
        private string _ma_co_so;

        public string ten_co_so
        {
            get
            {
                return _ten_co_so;
            }
            set
            {
                _ten_co_so = value;
                RaisePropertyChanged("ten_co_so");
            }
        }
        private string _ten_co_so;

        [DataMemberAttribute()]
        public string ma_so_bao_hiem_y_te
        {
            get
            {
                return _ma_so_bao_hiem_y_te;
            }
            set
            {
                _ma_so_bao_hiem_y_te = value;
                RaisePropertyChanged("ma_so_bao_hiem_y_te");
            }
        }
        private string _ma_so_bao_hiem_y_te;

        [DataMemberAttribute()]
        public string ma_benh_nhan
        {
            get
            {
                return _ma_benh_nhan;
            }
            set
            {
                _ma_benh_nhan = value;
                RaisePropertyChanged("ma_benh_nhan");
            }
        }
        private string _ma_benh_nhan;

        public string ho_ten
        {
            get
            {
                return _ho_ten;
            }
            set
            {
                _ho_ten = value;
                RaisePropertyChanged("ho_ten");
            }
        }
        private string _ho_ten;

        [DataMemberAttribute()]
        public string ho_ten_benh_nhan
        {
            get
            {
                return _ho_ten_benh_nhan;
            }
            set
            {
                _ho_ten_benh_nhan = value;
                RaisePropertyChanged("ho_ten_benh_nhan");
            }
        }
        private string _ho_ten_benh_nhan;

        [DataMemberAttribute()]
        public string ngay_sinh_benh_nhan
        {
            get
            {
                return _ngay_sinh_benh_nhan;
            }
            set
            {
                _ngay_sinh_benh_nhan = value;
                RaisePropertyChanged("ngay_sinh_benh_nhan");
            }
        }
        private string _ngay_sinh_benh_nhan;

        [DataMemberAttribute()]
        public string ma_dinh_danh_y_te
        {
            get
            {
                return _ma_dinh_danh_y_te;
            }
            set
            {
                _ma_dinh_danh_y_te = value;
                RaisePropertyChanged("ma_dinh_danh_y_te");
            }
        }
        private string _ma_dinh_danh_y_te;

        [DataMemberAttribute()]
        public int tuoi
        {
            get
            {
                return _tuoi;
            }
            set
            {
                _tuoi = value;
                RaisePropertyChanged("tuoi");
            }
        }
        private int _tuoi;

        [DataMemberAttribute()]
        public int gioi_tinh
        {
            get
            {
                return _gioi_tinh;
            }
            set
            {
                _gioi_tinh = value;
                RaisePropertyChanged("gioi_tinh");
            }
        }
        private int _gioi_tinh;

        [DataMemberAttribute()]
        public string dia_chi
        {
            get
            {
                return _dia_chi;
            }
            set
            {
                _dia_chi = value;
                RaisePropertyChanged("dia_chi");
            }
        }
        private string _dia_chi;

        public string ma_benh
        {
            get
            {
                return _ma_benh;
            }
            set
            {
                _ma_benh = value;
                RaisePropertyChanged("ma_benh");
            }
        }
        private string _ma_benh;

        public string ten_benh
        {
            get
            {
                return _ten_benh;
            }
            set
            {
                _ten_benh = value;
                RaisePropertyChanged("ten_benh");
            }
        }
        private string _ten_benh;
        [DataMemberAttribute()]
        public List<DTDT_chan_doan> chan_doan
        {
            get
            {
                return _chan_doan;
            }
            set
            {
                _chan_doan = value;
                RaisePropertyChanged("chan_doan");
            }
        }
        private List<DTDT_chan_doan> _chan_doan;

        public string ngay_ke_don
        {
            get
            {
                return _ngay_ke_don;
            }
            set
            {
                _ngay_ke_don = value;
                RaisePropertyChanged("ngay_ke_don");
            }
        }
        private string _ngay_ke_don;
        [DataMemberAttribute()]
        public string ngay_gio_ke_don
        {
            get
            {
                return _ngay_gio_ke_don;
            }
            set
            {
                _ngay_gio_ke_don = value;
                RaisePropertyChanged("ngay_gio_ke_don");
            }
        }
        private string _ngay_gio_ke_don;

        public DateTime ngay_vao_vien
        {
            get
            {
                return _ngay_vao_vien;
            }
            set
            {
                _ngay_vao_vien = value;
                RaisePropertyChanged("ngay_vao_vien");
            }
        }
        private DateTime _ngay_vao_vien;

        public DateTime? ngay_ra_vien
        {
            get
            {
                return _ngay_ra_vien;
            }
            set
            {
                _ngay_ra_vien = value;
                RaisePropertyChanged("ngay_ra_vien");
            }
        }
        private DateTime? _ngay_ra_vien;

        public string khoa_xuat_vien
        {
            get
            {
                return _khoa_xuat_vien;
            }
            set
            {
                _khoa_xuat_vien = value;
                RaisePropertyChanged("khoa_xuat_vien");
            }
        }
        private string _khoa_xuat_vien;

        [DataMemberAttribute()]
        public string ma_don_thuoc_quoc_gia
        {
            get
            {
                return _ma_don_thuoc_quoc_gia;
            }
            set
            {
                _ma_don_thuoc_quoc_gia = value;
                RaisePropertyChanged("ma_don_thuoc_quoc_gia");
            }
        }
        private string _ma_don_thuoc_quoc_gia;

        public long IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                _IssueID = value;
                RaisePropertyChanged("IssueID");
            }
        }
        private long _IssueID;

        public IList<DTDT_don_thuoc_chi_tiet> chi_tiet
        {
            get
            {
                return _chi_tiet;
            }
            set
            {
                _chi_tiet = value;
                RaisePropertyChanged("chi_tiet");
            }
        }
        private IList<DTDT_don_thuoc_chi_tiet> _chi_tiet;

        public string nguoi_ke_don
        {
            get
            {
                return _nguoi_ke_don;
            }
            set
            {
                _nguoi_ke_don = value;
                RaisePropertyChanged("nguoi_ke_don");
            }
        }
        private string _nguoi_ke_don;

        public long outiID
        {
            get
            {
                return _outiID;
            }
            set
            {
                _outiID = value;
                RaisePropertyChanged("outiID");
            }
        }
        private long _outiID;

        [DataMemberAttribute()]
        public string loai_don_thuoc
        {
            get
            {
                return _loai_don_thuoc;
            }
            set
            {
                _loai_don_thuoc = value;
                RaisePropertyChanged("loai_don_thuoc");
            }
        }
        private string _loai_don_thuoc;

        [DataMemberAttribute()]
        public string can_nang
        {
            get
            {
                return _can_nang;
            }
            set
            {
                _can_nang = value;
                RaisePropertyChanged("can_nang");
            }
        }
        private string _can_nang;

        public string gioi_tinh_str
        {
            get
            {
                return _gioi_tinh_str;
            }
            set
            {
                _gioi_tinh_str = value;
                RaisePropertyChanged("gioi_tinh_str");
            }
        }
        private string _gioi_tinh_str;

        [DataMemberAttribute()]
        public string thong_tin_nguoi_giam_ho
        {
            get
            {
                return _thong_tin_nguoi_giam_ho;
            }
            set
            {
                _thong_tin_nguoi_giam_ho = value;
                RaisePropertyChanged("thong_tin_nguoi_giam_ho");
            }
        }
        private string _thong_tin_nguoi_giam_ho;

        [DataMemberAttribute()]
        public string luu_y
        {
            get
            {
                return _luu_y;
            }
            set
            {
                _luu_y = value;
                RaisePropertyChanged("luu_y");
            }
        }
        private string _luu_y;

        [DataMemberAttribute()]
        public string hinh_thuc_dieu_tri
        {
            get
            {
                return _hinh_thuc_dieu_tri;
            }
            set
            {
                _hinh_thuc_dieu_tri = value;
                RaisePropertyChanged("hinh_thuc_dieu_tri");
            }
        }
        private string _hinh_thuc_dieu_tri;

        [DataMemberAttribute()]
        public string loi_dan
        {
            get
            {
                return _loi_dan;
            }
            set
            {
                _loi_dan = value;
                RaisePropertyChanged("loi_dan");
            }
        }
        private string _loi_dan;

        [DataMemberAttribute()]
        public string so_dien_thoai_nguoi_kham_benh
        {
            get
            {
                return _so_dien_thoai_nguoi_kham_benh;
            }
            set
            {
                _so_dien_thoai_nguoi_kham_benh = value;
                RaisePropertyChanged("so_dien_thoai_nguoi_kham_benh");
            }
        }
        private string _so_dien_thoai_nguoi_kham_benh;

        [DataMemberAttribute()]
        public string ngay_tai_kham
        {
            get
            {
                return _ngay_tai_kham;
            }
            set
            {
                _ngay_tai_kham = value;
                RaisePropertyChanged("ngay_tai_kham");
            }
        }
        private string _ngay_tai_kham;

        public string bac_si_ke_don
        {
            get
            {
                return _bac_si_ke_don;
            }
            set
            {
                _bac_si_ke_don = value;
                RaisePropertyChanged("bac_si_ke_don");
            }
        }
        private string _bac_si_ke_don;

        public string tinh_trang_day_cong
        {
            get
            {
                return _tinh_trang_day_cong;
            }
            set
            {
                _tinh_trang_day_cong = value;
                RaisePropertyChanged("tinh_trang_day_cong");
            }
        }
        private string _tinh_trang_day_cong;

        public DateTime ngay_day_cong
        {
            get
            {
                return _ngay_day_cong;
            }
            set
            {
                _ngay_day_cong = value;
                RaisePropertyChanged("ngay_day_cong");
            }
        }
        private DateTime _ngay_day_cong;

        [DataMemberAttribute()]
        public List<DTDT_don_thuoc_chi_tiet> thong_tin_don_thuoc
        {
            get
            {
                return _thong_tin_don_thuoc;
            }
            set
            {
                _thong_tin_don_thuoc = value;
                RaisePropertyChanged("thong_tin_don_thuoc");
            }
        }
        private List<DTDT_don_thuoc_chi_tiet> _thong_tin_don_thuoc;

        public DTDT_don_thuoc_chi_tiet thong_tin_don_thuoc_item
        {
            get
            {
                return _thong_tin_don_thuoc_item;
            }
            set
            {
                _thong_tin_don_thuoc_item = value;
                RaisePropertyChanged("thong_tin_don_thuoc_item");
            }
        }
        private DTDT_don_thuoc_chi_tiet _thong_tin_don_thuoc_item;

        public Staff Doctor
        {
            get
            {
                return _Doctor;
            }
            set
            {
                _Doctor = value;
                RaisePropertyChanged("Doctor");
            }
        }
        private Staff _Doctor;

        [DataMemberAttribute()]
        public DTDT_dot_dung_thuoc dot_dung_thuoc
        {
            get
            {
                return _dot_dung_thuoc;
            }
            set
            {
                _dot_dung_thuoc = value;
                RaisePropertyChanged("dot_dung_thuoc");
            }
        }
        private DTDT_dot_dung_thuoc _dot_dung_thuoc = new DTDT_dot_dung_thuoc();
        #endregion
    }
}
