using System;
using System.Collections.Generic;

namespace DataEntities
{
    public class PharmacyAPILogin
    {
        public PharmacyAPILogin(string _usr, string _pwd)
        {
            usr = _usr;
            pwd = _pwd;
        }
        private string _usr;
        private string _pwd;
        public string usr
        {
            get
            {
                return _usr;
            }
            set
            {
                _usr = value;
            }
        }
        public string pwd
        {
            get
            {
                return _pwd;
            }
            set
            {
                _pwd = value;
            }
        }
        public string code { get; set; }
        public string message { get; set; }
        public PharmacyAPILoginData data { get; set; }
    }
    public class PharmacyAPILoginData
    {
        public string token { get; set; }
        public string token_type { get; set; }
    }
    public class PharmacyAPIInwardInvoice
    {
        public PharmacyAPIInwardInvoice(string _ma_co_so)
        {
            ma_co_so = _ma_co_so;
        }
        private string _ma_co_so;
        private PharmacyAPIInwardInvoiceType _InvoiceType = PharmacyAPIInwardInvoiceType.Supplier;
        public string ma_phieu { get; set; }
        public string ma_co_so
        {
            get
            {
                return _ma_co_so;
            }
            set
            {
                _ma_co_so = value;
            }
        }
        public string ngay_nhap { get; set; }
        public int loai_phieu_nhap { get => (int)_InvoiceType; }
        public string ghi_chu { get; set; }
        public string ten_co_so_cung_cap { get; set; }
        public List<PharmacyAPIInwardInvoiceDetail> chi_tiet { get; set; }
    }
    public class PharmacyAPIInwardInvoiceDetail
    {
        public string ma_thuoc { get; set; }
        public string ten_thuoc { get; set; }
        public string so_lo { get; set; }
        public string ngay_san_xuat { get; set; }
        public string han_dung { get; set; }
        public string so_dklh { get; set; }
        public int so_luong { get; set; }
        public int don_gia { get; set; }
        public string don_vi_tinh { get; set; }
    }
    public enum PharmacyAPIInwardInvoiceType : int
    {
        Supplier = 1,
        Refund = 2,
        StockTake = 3
    }
    public class PharmacyAPIPrescription
    {
        public string ma_don_thuoc_co_so_kcb { get; set; }
        public PharmacyAPIUnit thong_tin_don_vi { get; set; }
        public PharmacyAPIPatient thong_tin_benh_nhan { get; set; }
        public PharmacyAPIICD10 thong_tin_benh { get; set; }
        public string ngay_ke_don { get; set; }
        public List<PharmacyAPIPrescriptionDetail> thong_tin_don_thuoc { get; set; }
        public string nguoi_ke_don { get; set; }
    }
    public class PharmacyAPIPrescriptionDetail
    {
        public string ma_thuoc { get; set; }
        public string ten_thuoc { get; set; }
        public string don_vi_tinh { get; set; }
        public string ham_luong { get; set; }
        public string duong_dung { get; set; }
        public string lieu_dung { get; set; }
        public string so_dang_ky { get; set; }
        public int so_luong { get; set; }
    }
    public class PharmacyAPIUnit
    {
        public string ma_co_so_kcb { get; set; }
        public string ten_co_so_kcb { get; set; }
    }
    public class PharmacyAPIPatient
    {
        public string ma_benh_nhan { get; set; }
        public string ho_ten { get; set; }
        public int tuoi { get; set; }
        public int gioi_tinh { get; set; }
        public string dia_chi { get; set; }
    }
    public class PharmacyAPIICD10
    {
        public string ma_benh { get; set; }
        public string ten_benh { get; set; }
    }
    public class PharmacyAPIOutwardPrescription
    {
        public string ma_hoa_don { get; set; }
        public string ma_co_so { get; set; }
        public string ma_don_thuoc_quoc_gia { get; set; }
        public string ngay_ban { get; set; }
        public string ho_ten_nguoi_ban { get; set; }
        public string ho_ten_khach_hang { get; set; }
        public List<PharmacyAPIOutwardPrescriptionDetail> hoa_don_chi_tiet { get; set; }
    }
    public class PharmacyAPIOutwardPrescriptionDetail
    {
        public string ma_thuoc { get; set; }
        public string ten_thuoc { get; set; }
        public string so_lo { get; set; }
        public string ngay_san_xuat { get; set; }
        public string han_dung { get; set; }
        public string don_vi_tinh { get; set; }
        public string ham_luong { get; set; }
        public string duong_dung { get; set; }
        public string lieu_dung { get; set; }
        public string so_dang_ky { get; set; }
        public int so_luong { get; set; }
        public int don_gia { get; set; }
        public int thanh_tien { get; set; }
        public int ty_le_quy_doi { get; set; }
    }
    public class PharmacyAPIOutwardInvoice
    {
        public string ma_phieu { get; set; }
        public string ma_co_so { get; set; }
        public string ngay_xuat { get; set; }
        public int loai_phieu_xuat { get; set; }
        public string ghi_chu { get; set; }
        public string ten_co_so_nhan { get; set; }
        public List<PharmacyAPIOutwardInvoiceDetail> chi_tiet { get; set; }
    }
    public class PharmacyAPIOutwardInvoiceDetail
    {
        public string ma_thuoc { get; set; }
        public string ten_thuoc { get; set; }
        public string so_lo { get; set; }
        public string ngay_san_xuat { get; set; }
        public string han_dung { get; set; }
        public string so_dklh { get; set; }
        public int so_luong { get; set; }
        public int don_gia { get; set; }
        public string don_vi_tinh { get; set; }
    }
}