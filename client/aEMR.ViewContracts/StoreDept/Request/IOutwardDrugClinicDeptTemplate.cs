namespace aEMR.ViewContracts
{
    public interface IOutwardDrugClinicDeptTemplate
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        void InitSelDeptCombo();
        long V_OutwardTemplateType { get; set; }
    }
}