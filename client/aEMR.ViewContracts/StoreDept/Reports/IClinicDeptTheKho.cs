namespace aEMR.ViewContracts
{
    public interface IClinicDeptTheKho
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mBaoCaoTheKho_Xem { get; set; }
        bool mBaoCaoTheKho_In { get; set; }
    }
}
