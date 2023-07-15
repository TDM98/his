
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptReportByDDMMYYYY
    {
        ReportName eItem { get; set; }
        ReportParameters RptParameters { get; set; }
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool mXemIn { get; set; }
        bool mIn { get; set; }    
        bool mXuatExcel { get; set; }    
    }
}
