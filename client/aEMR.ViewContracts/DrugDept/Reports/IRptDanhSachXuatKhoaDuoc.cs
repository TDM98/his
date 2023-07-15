using DataEntities;
using System;

namespace aEMR.ViewContracts
{
    public interface IRptDanhSachXuatKhoaDuoc
    {
        string TieuDeRpt { get; set; }
        long V_MedProductType { get; set; }
        bool mXemIn { get; set; }
        bool mChonKho { get; set; }
        bool IsTempInwardReport { get; set; }
        bool VisibilityOutputType { get; set; }
        bool MedProductTypeVisible { get; set; }
        bool IsDrugDeptExportDetail { get; set; }
        void InitForIsDrugDeptExportDetail();
        bool IsViewByVisible { get; set; }
        Nullable<ReportName> gReportName { get; set; }
    }
}