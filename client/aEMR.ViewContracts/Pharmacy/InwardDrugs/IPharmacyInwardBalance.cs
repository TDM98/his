using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IPharmacyInwardBalance
    {
        string strHienThi { get; set; }
        InwardInvoiceSearchCriteria SearchCriteria { get; set; }

        bool mNhapHangTuKhoDuoc_Tim { get; set; }
        bool mNhapHangTuKhoDuoc_Them { get; set; }
        bool mNhapHangTuKhoDuoc_XemIn { get; set; }
        bool mNhapHangTuKhoDuoc_In { get; set; }
    }
}
