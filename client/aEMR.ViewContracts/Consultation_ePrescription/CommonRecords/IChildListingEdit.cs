using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IChildListingEdit
    {
        BirthCertificates CurrentBirthCertificates { get; set; }
    }
}