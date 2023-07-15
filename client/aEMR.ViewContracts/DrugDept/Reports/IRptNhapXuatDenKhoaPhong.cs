namespace aEMR.ViewContracts
{
    public interface IRptNhapXuatDenKhoaPhong
    {
        string TitleForm { get; set; }
        string TieuDeRpt { get; set; }
        long V_MedProductType { get; set; }
        bool mIn { get; set; }
        bool mXuatExcel { get; set; }
    }
}
