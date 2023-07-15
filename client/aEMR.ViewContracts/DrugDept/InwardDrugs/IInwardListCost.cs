
namespace aEMR.ViewContracts
{
    public interface IInwardListCost
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool mTim { get; set; }
        bool mIn { get; set; }
        bool mChinhSua_Them { get; set; }
    }
}
