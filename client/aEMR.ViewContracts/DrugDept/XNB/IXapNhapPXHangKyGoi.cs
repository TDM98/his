namespace aEMR.ViewContracts
{
    public interface IXapNhapPXHangKyGoi
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mSapNhapHangKyGui_Tim { get; set; }
        bool mSapNhapHangKyGui_PhieuMoi { get; set; }
        bool mSapNhapHangKyGui_CapNhat { get; set; }
        bool mSapNhapHangKyGui_Xoa { get; set; }
        bool mSapNhapHangKyGui_XemIn { get; set; }
        bool mSapNhapHangKyGui_In { get; set; }
    }
}
