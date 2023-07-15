namespace aEMR.ViewContracts
{
    public interface IEstimationDrugDept
    {
        string strHienThi { get; set; }
        long V_MedProductType { get; set; }

        bool mTim{ get; set; }

        bool mThemMoi{ get; set; }
        
        bool mXoa{ get; set; }
        
        bool mXemIn{ get; set; }

    }
    public interface IEstimationDrugDeptByBid
    {
        string strHienThi { get; set; }
        long V_MedProductType { get; set; }
        bool mTim { get; set; }
        bool mThemMoi { get; set; }
        bool mXoa { get; set; }
        bool mXemIn { get; set; }
    }
    public interface IEstimationFromRequest
    {
        string strHienThi { get; set; }
        long V_MedProductType { get; set; }

        bool mTim { get; set; }

        bool mThemMoi { get; set; }

        bool mXoa { get; set; }

        bool mXemIn { get; set; }
    }
}
