using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IBaoCaoHangNgay
    {
        ReportName eItem { get; set; }
        string pageTitle { get; set; }
        bool bXem { get; set; }
        bool bIn { get; set; }
    }
}
