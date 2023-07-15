
namespace aEMR.ViewContracts
{
    public interface IDrugDeptReturn
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mTim{ get; set; }
        bool mLuu{ get; set; }
        bool mTraTien{ get; set; }
        bool mIn { get; set; }
        
    }
}
