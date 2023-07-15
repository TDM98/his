using DataEntities;
using System;
using System.Windows;
namespace aEMR.ViewContracts
{
    public interface ICriteriaA
    {
        Visibility VisibilityStore { get; set; }
        Visibility VisibilityExcel { get; set; }
        void GetListStore(long StoreType);
        bool isStoreDept { get; set; }
        bool VisibilityOutputType { get; set; }
        bool MedProductTypeVisible { get; set; }
        bool IsDrugDeptExportDetail { get; set; }
        void InitForIsDrugDeptExportDetail();
        DateTime? FromDate { get; }
        DateTime? ToDate { get; }
        RefStorageWarehouseLocation CurStore { get; }
        RefStorageWarehouseLocation OutStore { get; }
        bool IsViewByVisible { get; set; }
        Nullable<ReportName> gReportName { get; set; }
        long V_MedProductType { get; set; }
        bool GetStoreFollowV_MedProductType { get; set; }
    }
}