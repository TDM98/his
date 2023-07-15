using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IBidCollection
    {
        Bid SelectedBid { get; set; }

        long V_MedProductType { get; set; }
    }
}
