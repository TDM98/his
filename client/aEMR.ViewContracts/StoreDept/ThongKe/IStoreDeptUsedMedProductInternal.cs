namespace aEMR.ViewContracts
{
    public interface IStoreDeptUsedMedProductInternal
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mThongKe_xem { get; set; }
        bool mThongKe_PhieuMoi { get; set; }
        bool mThongKe_XemIn { get; set; }
        bool mThongKe_In { get; set; }
    }
}
