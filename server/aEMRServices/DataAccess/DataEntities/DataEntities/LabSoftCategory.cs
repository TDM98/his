using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;
namespace DataEntities
{
    public partial class LIS_Staff : NotifyChangedBase
    {
        private string _MaBacSi;
        [DataMemberAttribute]
        public string MaBacSi
        {
            get
            {
                return _MaBacSi;
            }
            set
            {
                _MaBacSi = value;
                RaisePropertyChanged("MaBacSi");
            }
        }
        private string _TenBacSi;
        [DataMemberAttribute]
        public string TenBacSi
        {
            get
            {
                return _TenBacSi;
            }
            set
            {
                _TenBacSi = value;
                RaisePropertyChanged("TenBacSi");
            }
        }
    }
    public partial class LIS_Department : NotifyChangedBase
    {
        private string _MaPhongBan;
        [DataMemberAttribute]
        public string MaPhongBan
        {
            get
            {
                return _MaPhongBan;
            }
            set
            {
                _MaPhongBan = value;
                RaisePropertyChanged("MaPhongBan");
            }
        }
        private string _TenPhongBan;
        [DataMemberAttribute]
        public string TenPhongBan
        {
            get
            {
                return _TenPhongBan;
            }
            set
            {
                _TenPhongBan = value;
                RaisePropertyChanged("TenPhongBan");
            }
        }
    }
    public partial class LIS_Object : NotifyChangedBase
    {
        private string _MaDoiTuong;
        [DataMemberAttribute]
        public string MaDoiTuong
        {
            get
            {
                return _MaDoiTuong;
            }
            set
            {
                _MaDoiTuong = value;
                RaisePropertyChanged("MaDoiTuong");
            }
        }
        private string _TenDoiTuong;
        [DataMemberAttribute]
        public string TenDoiTuong
        {
            get
            {
                return _TenDoiTuong;
            }
            set
            {
                _TenDoiTuong = value;
                RaisePropertyChanged("TenDoiTuong");
            }
        }
    }
    public partial class LIS_PCLItem : NotifyChangedBase
    {
        private string _Madichvu;
        [DataMemberAttribute]
        public string Madichvu
        {
            get
            {
                return _Madichvu;
            }
            set
            {
                _Madichvu = value;
                RaisePropertyChanged("Madichvu");
            }
        }
        private string _Dichvucaptren;
        [DataMemberAttribute]
        public string Dichvucaptren
        {
            get
            {
                return _Dichvucaptren;
            }
            set
            {
                _Dichvucaptren = value;
                RaisePropertyChanged("Dichvucaptren");
            }
        }
        private string _Tendichvu;
        [DataMemberAttribute]
        public string Tendichvu
        {
            get
            {
                return _Tendichvu;
            }
            set
            {
                _Tendichvu = value;
                RaisePropertyChanged("Tendichvu");
            }
        }
        private string _Donvitinh;
        [DataMemberAttribute]
        public string Donvitinh
        {
            get
            {
                return _Donvitinh;
            }
            set
            {
                _Donvitinh = value;
                RaisePropertyChanged("Donvitinh");
            }
        }

        private Int16 _Type;
        [DataMemberAttribute]
        public Int16 Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
                RaisePropertyChanged("Type");
            }
        }
    }
    public partial class LIS_Device : NotifyChangedBase
    {
        private string _MaThietBi;
        [DataMemberAttribute]
        public string MaThietBi
        {
            get
            {
                return _MaThietBi;
            }
            set
            {
                _MaThietBi = value;
                RaisePropertyChanged("MaThietBi");
            }
        }
        private string _TenThietBi;
        [DataMemberAttribute]
        public string TenThietBi
        {
            get
            {
                return _TenThietBi;
            }
            set
            {
                _TenThietBi = value;
                RaisePropertyChanged("TenThietBi");
            }
        }
    }
    public partial class LIS_User : NotifyChangedBase
    {
        private string _MaNguoiDung;
        [DataMemberAttribute]
        public string MaNguoiDung
        {
            get
            {
                return _MaNguoiDung;
            }
            set
            {
                _MaNguoiDung = value;
                RaisePropertyChanged("MaNguoiDung");
            }
        }
        private string _TenNguoiDung;
        [DataMemberAttribute]
        public string TenNguoiDung
        {
            get
            {
                return _TenNguoiDung;
            }
            set
            {
                _TenNguoiDung = value;
                RaisePropertyChanged("TenNguoiDung");
            }
        }
    }
    public partial class LIS_PCLRequest : NotifyChangedBase
    {
        private string _SoPhieuChiDinh;
        [DataMemberAttribute]
        public string SoPhieuChiDinh
        {
            get
            {
                return _SoPhieuChiDinh;
            }
            set
            {
                _SoPhieuChiDinh = value;
                RaisePropertyChanged("SoPhieuChiDinh");
            }
        }
        private string _MaDichVu;
        [DataMemberAttribute]
        public string MaDichVu
        {
            get
            {
                return _MaDichVu;
            }
            set
            {
                _MaDichVu = value;
                RaisePropertyChanged("MaDichVu");
            }
        }
        private string _TenDichVu;
        [DataMemberAttribute]
        public string TenDichVu
        {
            get
            {
                return _TenDichVu;
            }
            set
            {
                _TenDichVu = value;
                RaisePropertyChanged("TenDichVu");
            }
        }
        private DateTime _NgayChiDinh;
        [DataMemberAttribute]
        public DateTime NgayChiDinh
        {
            get
            {
                return _NgayChiDinh;
            }
            set
            {
                _NgayChiDinh = value;
                RaisePropertyChanged("NgayChiDinh");
            }
        }
        [DataMemberAttribute]
        public DateTime ThoiGianChiDinh
        {
            get
            {
                return _NgayChiDinh;
            }
            set
            {
                _NgayChiDinh = value;
                RaisePropertyChanged("ThoiGianChiDinh");
            }
        }
        private string _MaBenhNhan;
        [DataMemberAttribute]
        private string _KetQua;
        [DataMemberAttribute]
        public string KetQua
        {
            get
            {
                return _KetQua;
            }
            set
            {
                _KetQua = value;
                RaisePropertyChanged("KetQua");
            }
        }
        public string MaBenhNhan
        {
            get
            {
                return _MaBenhNhan;
            }
            set
            {
                _MaBenhNhan = value;
                RaisePropertyChanged("MaBenhNhan");
            }
        }
        private string _TenBenhNhan;
        [DataMemberAttribute]
        public string TenBenhNhan
        {
            get
            {
                return _TenBenhNhan;
            }
            set
            {
                _TenBenhNhan = value;
                RaisePropertyChanged("TenBenhNhan");
            }
        }
        private string _DiaChi;
        [DataMemberAttribute]
        public string DiaChi
        {
            get
            {
                return _DiaChi;
            }
            set
            {
                _DiaChi = value;
                RaisePropertyChanged("DiaChi");
            }
        }
        private int _NamSinh;
        [DataMemberAttribute]
        public int NamSinh
        {
            get
            {
                return _NamSinh;
            }
            set
            {
                _NamSinh = value;
                RaisePropertyChanged("NamSinh");
            }
        }
        private string _GioiTinh;
        [DataMemberAttribute]
        public string GioiTinh
        {
            get
            {
                return _GioiTinh;
            }
            set
            {
                _GioiTinh = value;
                RaisePropertyChanged("GioiTinh");
            }
        }
        private string _ChanDoan;
        [DataMemberAttribute]
        public string ChanDoan
        {
            get
            {
                return _ChanDoan;
            }
            set
            {
                _ChanDoan = value;
                RaisePropertyChanged("ChanDoan");
            }
        }
        private string _MaDoiTuong;
        [DataMemberAttribute]
        public string MaDoiTuong
        {
            get
            {
                return _MaDoiTuong;
            }
            set
            {
                _MaDoiTuong = value;
                RaisePropertyChanged("MaDoiTuong");
            }
        }
        private string _TenDoiTuong;
        [DataMemberAttribute]
        public string TenDoiTuong
        {
            get
            {
                return _TenDoiTuong;
            }
            set
            {
                _TenDoiTuong = value;
                RaisePropertyChanged("TenDoiTuong");
            }
        }
        private string _MaKhoaPhong;
        [DataMemberAttribute]
        public string MaKhoaPhong
        {
            get
            {
                return _MaKhoaPhong;
            }
            set
            {
                _MaKhoaPhong = value;
                RaisePropertyChanged("MaKhoaPhong");
            }
        }
        private string _TenKhoaPhong;
        [DataMemberAttribute]
        public string TenKhoaPhong
        {
            get
            {
                return _TenKhoaPhong;
            }
            set
            {
                _TenKhoaPhong = value;
                RaisePropertyChanged("TenKhoaPhong");
            }
        }
        private string _MaBacSi;
        [DataMemberAttribute]
        public string MaBacSi
        {
            get
            {
                return _MaBacSi;
            }
            set
            {
                _MaBacSi = value;
                RaisePropertyChanged("MaBacSi");
            }
        }
        private string _TenBacSi;
        [DataMemberAttribute]
        public string TenBacSi
        {
            get
            {
                return _TenBacSi;
            }
            set
            {
                _TenBacSi = value;
                RaisePropertyChanged("TenBacSi");
            }
        }
        private int _TrangThai;
        [DataMemberAttribute]
        public int TrangThai
        {
            get
            {
                return _TrangThai;
            }
            set
            {
                _TrangThai = value;
                RaisePropertyChanged("TrangThai");
            }
        }
        private int _CanNang;
        [DataMemberAttribute]
        public int CanNang
        {
            get
            {
                return _CanNang;
            }
            set
            {
                _CanNang = value;
                RaisePropertyChanged("CanNang");
            }
        }
        private int _ChieuCao;
        [DataMemberAttribute]
        public int ChieuCao
        {
            get
            {
                return _ChieuCao;
            }
            set
            {
                _ChieuCao = value;
                RaisePropertyChanged("ChieuCao");
            }
        }
        private int _Tuoi;
        [DataMemberAttribute]
        public int Tuoi
        {
            get
            {
                return _Tuoi;
            }
            set
            {
                _Tuoi = value;
                RaisePropertyChanged("Tuoi");
            }
        }
        private bool _TreEm;
        [DataMemberAttribute]
        public bool TreEm
        {
            get
            {
                return _TreEm;
            }
            set
            {
                _TreEm = value;
                RaisePropertyChanged("TreEm");
            }
        }
    }
    public enum PCLRequestStatus : int
    {
        Pending = 0,
        Received = 1
    }
}