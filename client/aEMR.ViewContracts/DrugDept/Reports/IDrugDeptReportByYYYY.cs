﻿
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptReportByYYYY
    {
        ReportName eItem { get; set; }
        ReportParameters RptParameters { get; set; }
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool mXemIn { get; set; }
        bool mIn { get; set; }    
    }
}
