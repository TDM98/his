using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ITheKhoKT
    {
        string StrHienThi { get; set; }

        //ReportName eItem { get; set; }

        void GetListStore(long? StoreType);

        bool IsHIStore { get; set; }
    }
}
