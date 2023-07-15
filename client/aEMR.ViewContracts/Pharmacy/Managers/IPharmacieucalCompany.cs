
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IPharmacieucalCompany
    {
        bool IsChildWindow { get; set; }
        string TitleForm { get; set; }
        eFirePharmacieucalCompanyEvent ePharmacieucalCompany { get; set; }
    }
}
