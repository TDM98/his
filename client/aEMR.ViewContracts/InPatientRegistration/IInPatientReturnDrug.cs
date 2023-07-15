using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientReturnDrug
    {
        IInPatientUsedMedProductListing MedProductListingContent { get; set; }
        AllLookupValues.MedProductType MedProductType { get; set; }
        PatientRegistration Registration { get; set; }
        void InitData(long? DeptID);
    }
}
