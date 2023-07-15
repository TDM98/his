using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ITemp02NoiTru
    {
        bool IsTemp02NoiTruNew { get; set; }
        ReportName ReportNameObj { get; set; }
        bool IsPhieuCongKhaiDV { get; set; }
    }
}