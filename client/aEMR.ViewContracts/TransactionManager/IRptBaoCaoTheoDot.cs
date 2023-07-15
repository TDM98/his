using DataEntities;
using DevExpress.Xpf.Printing;
using System.Windows;

namespace aEMR.ViewContracts
{
    public interface IRptBaoCaoTheoDot
    {
        ReportName eItem { get; set; }
        ReportParameters RptParameters { get; set; }
        Visibility ShowDepartment { get; set; }
        Visibility ViTreatedOrNot { get; set; }
        int EnumOfFunction { get; set; }
        void authorization();
        bool ShowIsNewForm { get; set; }
    }
}
