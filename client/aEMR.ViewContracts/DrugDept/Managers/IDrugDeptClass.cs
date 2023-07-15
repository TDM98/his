namespace aEMR.ViewContracts
{
    public interface IDrugDeptClass
    {
        string TitleForm { get; set; }
        long V_MedProductType { get; set; }

        void GetSearchTreeView(long V_MedProductType);
        void LoadFamilyParent(long V_MedProductType);

        bool bTim { get; set; }
        bool bThem { get; set; }
        bool bChinhSua { get; set; }
        bool IsDoubleClick { get; set; }

    }

    public interface IDrugDeptSelectClass
    {
        string TitleForm { get; set; }
        long V_MedProductType { get; set; }

        void GetSearchTreeView(long V_MedProductType);
        void LoadFamilyParent(long V_MedProductType);

        bool bTim { get; set; }
        bool bThem { get; set; }
        bool bChinhSua { get; set; }
        bool IsDoubleClick { get; set; }

    }

    public interface IGenericClass
    {
        string TitleForm { get; set; }
        long V_MedProductType { get; set; }

        void GetSearchTreeView(long V_MedProductType);
        void LoadFamilyParent(long V_MedProductType);

        bool bTim { get; set; }
        bool bThem { get; set; }
        bool bChinhSua { get; set; }

    }
}
