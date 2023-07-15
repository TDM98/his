
namespace aEMR.ViewContracts
{
    public interface IDrugDeptPurchaseOrder
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        void LoadOrderWarning();

        bool mDSCanDatHang{ get; set; }

        bool mTim{ get; set; }
        
        bool mChinhSua{ get; set; }
        
        bool mThemMoi{ get; set; }
        
        bool mIn{ get; set; }

        bool IsEstimateFromRequest { get; set; }
    }
}
