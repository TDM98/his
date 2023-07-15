using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IRequestPharmacy
    {
        void RefeshRequest();
        string strHienThi { get; set; }

    }
}
