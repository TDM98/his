using eHCMS.Services.Core.Base;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Text;
/*
*20230218 #001 QTD: Thêm trường cho DTDT Nhà thuốc
*/

namespace DataEntities
{
    public class DQGReport : NotifyChangedBase
    {
        private long _DQGReportID;
        private string _Title;
        private DateTime _FromDate;
        private DateTime _ToDate;
        private Staff _CreatedStaff;
        private DateTime _RecCreatedDate;
        private IList<DQG_phieu_nhap> _phieu_nhap;
        private IList<DQG_don_thuoc> _don_thuoc;
        private IList<DQG_hoa_don> _hoa_don;
        private IList<DQG_phieu_xuat> _phieu_xuat;
        private bool _IsTransferCompleted = false;
        //▼====: #001
        private string _IssueIDList;
        private long _DQGReportIDInpt;
        //▲====: #001
        public long DQGReportID
        {
            get
            {
                return _DQGReportID;
            }
            set
            {
                _DQGReportID = value;
                RaisePropertyChanged("DQGReportID");
            }
        }
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        public IList<DQG_phieu_nhap> phieu_nhap
        {
            get
            {
                return _phieu_nhap;
            }
            set
            {
                _phieu_nhap = value;
                RaisePropertyChanged("phieu_nhap");
            }
        }
        public IList<DQG_don_thuoc> don_thuoc
        {
            get
            {
                return _don_thuoc;
            }
            set
            {
                _don_thuoc = value;
                RaisePropertyChanged("don_thuoc");
            }
        }
        public IList<DQG_hoa_don> hoa_don
        {
            get
            {
                return _hoa_don;
            }
            set
            {
                _hoa_don = value;
                RaisePropertyChanged("hoa_don");
            }
        }
        public IList<DQG_phieu_xuat> phieu_xuat
        {
            get
            {
                return _phieu_xuat;
            }
            set
            {
                _phieu_xuat = value;
                RaisePropertyChanged("phieu_xuat");
            }
        }
        public bool IsTransferCompleted
        {
            get
            {
                return _IsTransferCompleted;
            }
            set
            {
                _IsTransferCompleted = value;
                RaisePropertyChanged("IsTransferCompleted");
            }
        }
        //▼====: #001
        public string IssueIDList
        {
            get
            {
                return _IssueIDList;
            }
            set
            {
                if(_IssueIDList != value)
                {
                    _IssueIDList = value;
                    RaisePropertyChanged("IssueIDList");
                }    
            }
        }

        public long DQGReportIDInpt
        {
            get
            {
                return _DQGReportIDInpt;
            }
            set
            {
                _DQGReportIDInpt = value;
                RaisePropertyChanged("DQGReportIDInpt");
            }
        }

        public static string ConvertIDListToXml(string IssueIDList)
        {
            if (!string.IsNullOrEmpty(IssueIDList))
            {
                string[] IssueIDIDCollection = IssueIDList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();
                sb.Append("<IssueIDList>");
                foreach (string aIssueID in IssueIDIDCollection)
                {
                    if (string.IsNullOrEmpty(aIssueID)) continue;
                    string[] PrescriptionInf = aIssueID.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (PrescriptionInf.Length == 2)
                    {
                        sb.Append("<IssueID>");
                        sb.AppendFormat("<RegistrationType>{0}</RegistrationType>", PrescriptionInf[0]);
                        sb.AppendFormat("<IssueID>{0}</IssueID>", PrescriptionInf[1]);
                        sb.Append("</IssueID>");
                    }
                    else
                        throw new Exception(eHCMSResources.Z2200_G1_MaDotDieuTriKhongDungDD);
                }
                sb.Append("</IssueIDList>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        //▲====: #001
    }
    public class DQG_don_thuoc_chi_tiet : NotifyChangedBase
    {
        private long _id_don_thuoc_chi_tiet;
        private long _id_don_thuoc;
        private string _ma_thuoc;
        private string _ten_thuoc;
        private string _don_vi_tinh;
        private string _ham_luong;
        private string _duong_dung;
        private string _lieu_dung;
        private string _so_dang_ky;
        private int _so_luong;
        //▼====: #001
        private string _biet_duoc;
        private string _cach_dung;
        private string _ma_thuoc_da_ke_don;
        private int _so_luong_ban;
        //▲====: #001
        public long id_don_thuoc_chi_tiet
        {
            get
            {
                return _id_don_thuoc_chi_tiet;
            }
            set
            {
                _id_don_thuoc_chi_tiet = value;
                RaisePropertyChanged("id_don_thuoc_chi_tiet");
            }
        }
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
        public string ma_thuoc
        {
            get
            {
                return _ma_thuoc;
            }
            set
            {
                _ma_thuoc = value;
                RaisePropertyChanged("ma_thuoc");
            }
        }
        public string ten_thuoc
        {
            get
            {
                return _ten_thuoc;
            }
            set
            {
                _ten_thuoc = value;
                RaisePropertyChanged("ten_thuoc");
            }
        }
        public string don_vi_tinh
        {
            get
            {
                return _don_vi_tinh;
            }
            set
            {
                _don_vi_tinh = value;
                RaisePropertyChanged("don_vi_tinh");
            }
        }
        public string ham_luong
        {
            get
            {
                return _ham_luong;
            }
            set
            {
                _ham_luong = value;
                RaisePropertyChanged("ham_luong");
            }
        }
        public string duong_dung
        {
            get
            {
                return _duong_dung;
            }
            set
            {
                _duong_dung = value;
                RaisePropertyChanged("duong_dung");
            }
        }
        public string lieu_dung
        {
            get
            {
                return _lieu_dung;
            }
            set
            {
                _lieu_dung = value;
                RaisePropertyChanged("lieu_dung");
            }
        }
        public string so_dang_ky
        {
            get
            {
                return _so_dang_ky;
            }
            set
            {
                _so_dang_ky = value;
                RaisePropertyChanged("so_dang_ky");
            }
        }
        public int so_luong
        {
            get
            {
                return _so_luong;
            }
            set
            {
                _so_luong = value;
                RaisePropertyChanged("so_luong");
            }
        }

        //▼====: #001
        public int so_luong_ban
        {
            get
            {
                return _so_luong_ban;
            }
            set
            {
                _so_luong_ban = value;
                RaisePropertyChanged("so_luong_ban");
            }
        }       

        public string ma_thuoc_da_ke_don
        {
            get
            {
                return _ma_thuoc_da_ke_don;
            }
            set
            {
                _ma_thuoc_da_ke_don = value;
                RaisePropertyChanged("ma_thuoc_da_ke_don");
            }
        }
        public string biet_duoc
        {
            get
            {
                return _biet_duoc;
            }
            set
            {
                _biet_duoc = value;
                RaisePropertyChanged("biet_duoc");
            }
        }
        public string cach_dung
        {
            get
            {
                return _cach_dung;
            }
            set
            {
                _cach_dung = value;
                RaisePropertyChanged("cach_dung");
            }
        }
        //▲====: #001
    }
    public class DQG_don_thuoc : NotifyChangedBase
    {
        private long _DQGReportID;
        private long _id_don_thuoc;
        private string _ma_don_thuoc_co_so_kcb;
        private string _ma_co_so;
        private string _ten_co_so;
        private string _ma_benh_nhan;
        private string _ho_ten;
        private Int16 _tuoi;
        private Byte _gioi_tinh;
        private string _dia_chi;
        private string _ma_benh;
        private string _ten_benh;
        private string _ngay_ke_don;
        private string _ma_don_thuoc_quoc_gia;
        private long _IssueID;
        private IList<DQG_don_thuoc_chi_tiet> _chi_tiet;
        private string _nguoi_ke_don;
        //▼====: #001
        private string _ma_hoa_don;
        private string _dia_chi_co_so;
        private string _so_dien_thoai_co_so;
        //▲====: #001
        public long DQGReportID
        {
            get
            {
                return _DQGReportID;
            }
            set
            {
                _DQGReportID = value;
                RaisePropertyChanged("DQGReportID");
            }
        }
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
        public Int16 tuoi
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
        public Byte gioi_tinh
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
        public IList<DQG_don_thuoc_chi_tiet> chi_tiet
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
        //▼====: #001
        public string so_dien_thoai_co_so
        {
            get
            {
                return _so_dien_thoai_co_so;
            }
            set
            {
                _so_dien_thoai_co_so = value;
                RaisePropertyChanged("so_dien_thoai_co_so");
            }
        }
        public string dia_chi_co_so
        {
            get
            {
                return _dia_chi_co_so;
            }
            set
            {
                _dia_chi_co_so = value;
                RaisePropertyChanged("dia_chi_co_so");
            }
        }       
        public string ma_hoa_don
        {
            get
            {
                return _ma_hoa_don;
            }
            set
            {
                _ma_hoa_don = value;
                RaisePropertyChanged("ma_hoa_don");
            }
        }
        //▲====: #001
    }
    public class DQG_hoa_don_chi_tiet : NotifyChangedBase
    {
        private long _id_hoa_don_chi_tiet;
        private long _id_hoa_don;
        private string _ma_thuoc;
        private string _ten_thuoc;
        private string _so_lo;
        private string _ngay_san_xuat;
        private string _han_dung;
        private string _don_vi_tinh;
        private string _ham_luong;
        private string _duong_dung;
        private string _lieu_dung;
        private string _so_dang_ky;
        private int _so_luong;
        private decimal _don_gia;
        private decimal _thanh_tien;
        private int _ty_le_quy_doi = 1;
        public long id_hoa_don_chi_tiet
        {
            get
            {
                return _id_hoa_don_chi_tiet;
            }
            set
            {
                _id_hoa_don_chi_tiet = value;
                RaisePropertyChanged("id_hoa_don_chi_tiet");
            }
        }
        public long id_hoa_don
        {
            get
            {
                return _id_hoa_don;
            }
            set
            {
                _id_hoa_don = value;
                RaisePropertyChanged("id_hoa_don");
            }
        }
        public string ma_thuoc
        {
            get
            {
                return _ma_thuoc;
            }
            set
            {
                _ma_thuoc = value;
                RaisePropertyChanged("ma_thuoc");
            }
        }
        public string ten_thuoc
        {
            get
            {
                return _ten_thuoc;
            }
            set
            {
                _ten_thuoc = value;
                RaisePropertyChanged("ten_thuoc");
            }
        }
        public string so_lo
        {
            get
            {
                return _so_lo;
            }
            set
            {
                _so_lo = value;
                RaisePropertyChanged("so_lo");
            }
        }
        public string ngay_san_xuat
        {
            get
            {
                return _ngay_san_xuat;
            }
            set
            {
                _ngay_san_xuat = value;
                RaisePropertyChanged("ngay_san_xuat");
            }
        }
        public string han_dung
        {
            get
            {
                return _han_dung;
            }
            set
            {
                _han_dung = value;
                RaisePropertyChanged("han_dung");
            }
        }
        public string don_vi_tinh
        {
            get
            {
                return _don_vi_tinh;
            }
            set
            {
                _don_vi_tinh = value;
                RaisePropertyChanged("don_vi_tinh");
            }
        }
        public string ham_luong
        {
            get
            {
                return _ham_luong;
            }
            set
            {
                _ham_luong = value;
                RaisePropertyChanged("ham_luong");
            }
        }
        public string duong_dung
        {
            get
            {
                return _duong_dung;
            }
            set
            {
                _duong_dung = value;
                RaisePropertyChanged("duong_dung");
            }
        }
        public string lieu_dung
        {
            get
            {
                return _lieu_dung;
            }
            set
            {
                _lieu_dung = value;
                RaisePropertyChanged("lieu_dung");
            }
        }
        public string so_dang_ky
        {
            get
            {
                return _so_dang_ky;
            }
            set
            {
                _so_dang_ky = value;
                RaisePropertyChanged("so_dang_ky");
            }
        }
        public int so_luong
        {
            get
            {
                return _so_luong;
            }
            set
            {
                _so_luong = value;
                RaisePropertyChanged("so_luong");
            }
        }
        public decimal don_gia
        {
            get
            {
                return _don_gia;
            }
            set
            {
                _don_gia = value;
                RaisePropertyChanged("don_gia");
            }
        }
        public decimal thanh_tien
        {
            get
            {
                return _thanh_tien;
            }
            set
            {
                _thanh_tien = value;
                RaisePropertyChanged("thanh_tien");
            }
        }
        public int ty_le_quy_doi
        {
            get
            {
                return _ty_le_quy_doi;
            }
            set
            {
                _ty_le_quy_doi = value;
                RaisePropertyChanged("ty_le_quy_doi");
            }
        }
    }
    public class DQG_hoa_don : NotifyChangedBase
    {
        private long _DQGReportID;
        private long _id_hoa_don;
        private string _ma_hoa_don;
        private string _ma_co_so;
        private string _ma_don_thuoc_quoc_gia;
        private string _ngay_ban;
        private string _ho_ten_nguoi_ban;
        private string _ho_ten_khach_hang;
        private string _ma_hoa_don_quoc_gia;
        private long _outiID;
        private long _IssueID;
        private IList<DQG_hoa_don_chi_tiet> _chi_tiet;
        public long DQGReportID
        {
            get
            {
                return _DQGReportID;
            }
            set
            {
                _DQGReportID = value;
                RaisePropertyChanged("DQGReportID");
            }
        }
        public long id_hoa_don
        {
            get
            {
                return _id_hoa_don;
            }
            set
            {
                _id_hoa_don = value;
                RaisePropertyChanged("id_hoa_don");
            }
        }
        public string ma_hoa_don
        {
            get
            {
                return _ma_hoa_don;
            }
            set
            {
                _ma_hoa_don = value;
                RaisePropertyChanged("ma_hoa_don");
            }
        }
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
        public string ngay_ban
        {
            get
            {
                return _ngay_ban;
            }
            set
            {
                _ngay_ban = value;
                RaisePropertyChanged("ngay_ban");
            }
        }
        public string ho_ten_nguoi_ban
        {
            get
            {
                return _ho_ten_nguoi_ban;
            }
            set
            {
                _ho_ten_nguoi_ban = value;
                RaisePropertyChanged("ho_ten_nguoi_ban");
            }
        }
        public string ho_ten_khach_hang
        {
            get
            {
                return _ho_ten_khach_hang;
            }
            set
            {
                _ho_ten_khach_hang = value;
                RaisePropertyChanged("ho_ten_khach_hang");
            }
        }
        public string ma_hoa_don_quoc_gia
        {
            get
            {
                return _ma_hoa_don_quoc_gia;
            }
            set
            {
                _ma_hoa_don_quoc_gia = value;
                RaisePropertyChanged("ma_hoa_don_quoc_gia");
            }
        }
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
        public IList<DQG_hoa_don_chi_tiet> chi_tiet
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
    }
    public class DQG_phieu_nhap_chi_tiet : NotifyChangedBase
    {
        private long _id_phieu_nhap_chi_tiet;
        private long _id_phieu_nhap;
        private string _ma_thuoc;
        private string _ten_thuoc;
        private string _so_lo;
        private string _ngay_san_xuat;
        private string _han_dung;
        private string _so_dklh;
        private int _so_luong;
        private decimal _don_gia;
        private string _don_vi_tinh;
        public long id_phieu_nhap_chi_tiet
        {
            get
            {
                return _id_phieu_nhap_chi_tiet;
            }
            set
            {
                _id_phieu_nhap_chi_tiet = value;
                RaisePropertyChanged("id_phieu_nhap_chi_tiet");
            }
        }
        public long id_phieu_nhap
        {
            get
            {
                return _id_phieu_nhap;
            }
            set
            {
                _id_phieu_nhap = value;
                RaisePropertyChanged("id_phieu_nhap");
            }
        }
        public string ma_thuoc
        {
            get
            {
                return _ma_thuoc;
            }
            set
            {
                _ma_thuoc = value;
                RaisePropertyChanged("ma_thuoc");
            }
        }
        public string ten_thuoc
        {
            get
            {
                return _ten_thuoc;
            }
            set
            {
                _ten_thuoc = value;
                RaisePropertyChanged("ten_thuoc");
            }
        }
        public string so_lo
        {
            get
            {
                return _so_lo;
            }
            set
            {
                _so_lo = value;
                RaisePropertyChanged("so_lo");
            }
        }
        public string ngay_san_xuat
        {
            get
            {
                return _ngay_san_xuat;
            }
            set
            {
                _ngay_san_xuat = value;
                RaisePropertyChanged("ngay_san_xuat");
            }
        }
        public string han_dung
        {
            get
            {
                return _han_dung;
            }
            set
            {
                _han_dung = value;
                RaisePropertyChanged("han_dung");
            }
        }
        public string so_dklh
        {
            get
            {
                return _so_dklh;
            }
            set
            {
                _so_dklh = value;
                RaisePropertyChanged("so_dklh");
            }
        }
        public int so_luong
        {
            get
            {
                return _so_luong;
            }
            set
            {
                _so_luong = value;
                RaisePropertyChanged("so_luong");
            }
        }
        public decimal don_gia
        {
            get
            {
                return _don_gia;
            }
            set
            {
                _don_gia = value;
                RaisePropertyChanged("don_gia");
            }
        }
        public string don_vi_tinh
        {
            get
            {
                return _don_vi_tinh;
            }
            set
            {
                _don_vi_tinh = value;
                RaisePropertyChanged("don_vi_tinh");
            }
        }
    }
    public class DQG_phieu_nhap : NotifyChangedBase
    {
        private long _DQGReportID;
        private long _id_phieu_nhap;
        private string _ma_phieu_nhap;
        private string _ma_co_so;
        private string _ngay_nhap;
        private Byte _loai_phieu_nhap = 0;
        private string _ghi_chu;
        private string _ten_co_so_cung_cap;
        private string _ma_phieu_nhap_quoc_gia;
        private long _inviID;
        private long _outiID;
        private IList<DQG_phieu_nhap_chi_tiet> _chi_tiet;
        public long DQGReportID
        {
            get
            {
                return _DQGReportID;
            }
            set
            {
                _DQGReportID = value;
                RaisePropertyChanged("DQGReportID");
            }
        }
        public long id_phieu_nhap
        {
            get
            {
                return _id_phieu_nhap;
            }
            set
            {
                _id_phieu_nhap = value;
                RaisePropertyChanged("id_phieu_nhap");
            }
        }
        public string ma_phieu_nhap
        {
            get
            {
                return _ma_phieu_nhap;
            }
            set
            {
                _ma_phieu_nhap = value;
                RaisePropertyChanged("ma_phieu_nhap");
            }
        }
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
        public string ngay_nhap
        {
            get
            {
                return _ngay_nhap;
            }
            set
            {
                _ngay_nhap = value;
                RaisePropertyChanged("ngay_nhap");
            }
        }
        public Byte loai_phieu_nhap
        {
            get
            {
                return _loai_phieu_nhap;
            }
            set
            {
                _loai_phieu_nhap = value;
                RaisePropertyChanged("loai_phieu_nhap");
            }
        }
        public string ghi_chu
        {
            get
            {
                return _ghi_chu;
            }
            set
            {
                _ghi_chu = value;
                RaisePropertyChanged("ghi_chu");
            }
        }
        public string ten_co_so_cung_cap
        {
            get
            {
                return _ten_co_so_cung_cap;
            }
            set
            {
                _ten_co_so_cung_cap = value;
                RaisePropertyChanged("ten_co_so_cung_cap");
            }
        }
        public string ma_phieu_nhap_quoc_gia
        {
            get
            {
                return _ma_phieu_nhap_quoc_gia;
            }
            set
            {
                _ma_phieu_nhap_quoc_gia = value;
                RaisePropertyChanged("ma_phieu_nhap_quoc_gia");
            }
        }
        public long inviID
        {
            get
            {
                return _inviID;
            }
            set
            {
                _inviID = value;
                RaisePropertyChanged("inviID");
            }
        }
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
        public IList<DQG_phieu_nhap_chi_tiet> chi_tiet
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
    }
    public class DQG_phieu_xuat_chi_tiet : NotifyChangedBase
    {
        private long _id_phieu_xuat_chi_tiet;
        private long _id_phieu_xuat;
        private string _ma_thuoc;
        private string _ten_thuoc;
        private string _so_lo;
        private string _ngay_san_xuat;
        private string _han_dung;
        private string _so_dklh;
        private int _so_luong;
        private decimal _don_gia;
        private string _don_vi_tinh;
        public long id_phieu_xuat_chi_tiet
        {
            get
            {
                return _id_phieu_xuat_chi_tiet;
            }
            set
            {
                _id_phieu_xuat_chi_tiet = value;
                RaisePropertyChanged("id_phieu_xuat_chi_tiet");
            }
        }
        public long id_phieu_xuat
        {
            get
            {
                return _id_phieu_xuat;
            }
            set
            {
                _id_phieu_xuat = value;
                RaisePropertyChanged("id_phieu_xuat");
            }
        }
        public string ma_thuoc
        {
            get
            {
                return _ma_thuoc;
            }
            set
            {
                _ma_thuoc = value;
                RaisePropertyChanged("ma_thuoc");
            }
        }
        public string ten_thuoc
        {
            get
            {
                return _ten_thuoc;
            }
            set
            {
                _ten_thuoc = value;
                RaisePropertyChanged("ten_thuoc");
            }
        }
        public string so_lo
        {
            get
            {
                return _so_lo;
            }
            set
            {
                _so_lo = value;
                RaisePropertyChanged("so_lo");
            }
        }
        public string ngay_san_xuat
        {
            get
            {
                return _ngay_san_xuat;
            }
            set
            {
                _ngay_san_xuat = value;
                RaisePropertyChanged("ngay_san_xuat");
            }
        }
        public string han_dung
        {
            get
            {
                return _han_dung;
            }
            set
            {
                _han_dung = value;
                RaisePropertyChanged("han_dung");
            }
        }
        public string so_dklh
        {
            get
            {
                return _so_dklh;
            }
            set
            {
                _so_dklh = value;
                RaisePropertyChanged("so_dklh");
            }
        }
        public int so_luong
        {
            get
            {
                return _so_luong;
            }
            set
            {
                _so_luong = value;
                RaisePropertyChanged("so_luong");
            }
        }
        public decimal don_gia
        {
            get
            {
                return _don_gia;
            }
            set
            {
                _don_gia = value;
                RaisePropertyChanged("don_gia");
            }
        }
        public string don_vi_tinh
        {
            get
            {
                return _don_vi_tinh;
            }
            set
            {
                _don_vi_tinh = value;
                RaisePropertyChanged("don_vi_tinh");
            }
        }
    }
    public class DQG_phieu_xuat : NotifyChangedBase
    {
        private long _DQGReportID;
        private long _id_phieu_xuat;
        private string _ma_phieu_xuat;
        private string _ma_co_so;
        private string _ngay_xuat;
        private Byte _loai_phieu_xuat = 0;
        private string _ghi_chu;
        private string _ten_co_so_nhan;
        private string _ma_phieu_xuat_quoc_gia;
        private long _outiID;
        private IList<DQG_phieu_xuat_chi_tiet> _chi_tiet;
        public long DQGReportID
        {
            get
            {
                return _DQGReportID;
            }
            set
            {
                _DQGReportID = value;
                RaisePropertyChanged("DQGReportID");
            }
        }
        public long id_phieu_xuat
        {
            get
            {
                return _id_phieu_xuat;
            }
            set
            {
                _id_phieu_xuat = value;
                RaisePropertyChanged("id_phieu_xuat");
            }
        }
        public string ma_phieu_xuat
        {
            get
            {
                return _ma_phieu_xuat;
            }
            set
            {
                _ma_phieu_xuat = value;
                RaisePropertyChanged("ma_phieu_xuat");
            }
        }
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
        public string ngay_xuat
        {
            get
            {
                return _ngay_xuat;
            }
            set
            {
                _ngay_xuat = value;
                RaisePropertyChanged("ngay_xuat");
            }
        }
        public Byte loai_phieu_xuat
        {
            get
            {
                return _loai_phieu_xuat;
            }
            set
            {
                _loai_phieu_xuat = value;
                RaisePropertyChanged("loai_phieu_xuat");
            }
        }
        public string ghi_chu
        {
            get
            {
                return _ghi_chu;
            }
            set
            {
                _ghi_chu = value;
                RaisePropertyChanged("ghi_chu");
            }
        }
        public string ten_co_so_nhan
        {
            get
            {
                return _ten_co_so_nhan;
            }
            set
            {
                _ten_co_so_nhan = value;
                RaisePropertyChanged("ten_co_so_nhan");
            }
        }
        public string ma_phieu_xuat_quoc_gia
        {
            get
            {
                return _ma_phieu_xuat_quoc_gia;
            }
            set
            {
                _ma_phieu_xuat_quoc_gia = value;
                RaisePropertyChanged("ma_phieu_xuat_quoc_gia");
            }
        }
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
        public IList<DQG_phieu_xuat_chi_tiet> chi_tiet
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
    }
    public class GlobalDrugsSystemAPIResultCode
    {
        public string code { get; set; }
        public string mess { get; set; }
        public string ma_phieu_nhap_quoc_gia { get; set; }
        public string ma_don_thuoc_quoc_gia { get; set; }
        public string ma_hoa_don_quoc_gia { get; set; }
        public string ma_phieu_xuat_quoc_gia { get; set; }
    }
}