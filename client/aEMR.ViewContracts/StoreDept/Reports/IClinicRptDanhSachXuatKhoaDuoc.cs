namespace aEMR.ViewContracts
{
    public interface IClinicRptDanhSachXuatKhoaDuoc
    {
        string TieuDeRpt { get; set; }
        long V_MedProductType { get; set; }
        bool mBaoCaoXuat_XemIn { get; set; }
    }
}
