using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IStoreDeptRequest
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mPhieuYeuCau_Tim { get; set; }
        bool mPhieuYeuCau_Them { get; set; }
        bool mPhieuYeuCau_Xoa { get; set; }
        bool mPhieuYeuCau_XemIn { get; set; }
        bool mPhieuYeuCau_In { get; set; }
    }
}
