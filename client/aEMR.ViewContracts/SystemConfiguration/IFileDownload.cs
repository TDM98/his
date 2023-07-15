
namespace aEMR.ViewContracts
{
    public interface IFileDownload
    {
        void Download();
        int ProgressPercentage { get;}
    }
}
