using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IClinicInwardFromDrugDept
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        InwardInvoiceSearchCriteria SearchCriteria { get; set; }

        bool mNhapTraTuKhoPhong_Tim { get; set; }
        bool mNhapTraTuKhoPhong_PhieuMoi { get; set; }
        bool mNhapTraTuKhoPhong_XemIn { get; set; }
        bool mNhapTraTuKhoPhong_In { get; set; }
        bool vNhapTraKhoBHYT { get; set; }
    }
}
