using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ITheKho
    {
        string TitleForm { get; set; }

        ReportName eItem { get; set; }
    }
}
