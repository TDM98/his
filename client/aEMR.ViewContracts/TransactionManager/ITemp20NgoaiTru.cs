using DataEntities;
using DevExpress.Xpf.Printing;
using System.Windows;
/*
 * 20190806 #001 TNHX: [BM0013097] Add filter KCBBD for Temp21_New
*/
namespace aEMR.ViewContracts
{
    public interface ITemp20NgoaiTru
    {
        ReportName eItem { get; set; }
        ReportParameters RptParameters { get; set; }
        //ReportPreviewModel ReportModel { get; set; }
        Visibility ViDetail { get; set; }
        Visibility ShowDepartment { get; set; }
        Visibility ViTreatedOrNot { get; set; }
        int EnumOfFunction { get; set; }
        void authorization();
        bool IsYVu { get; set; }
        void LockConditionCombobox();
        bool IsEnableViewBy { get; set; }
        bool mDrug { get; set; }
        long V_MedProductType { get; set; }
        string StrHienThi { get; set; }
        bool ShowKCBBanDau { get; set; }
        bool Only79A { get; set; }
        bool mViewAndPrint { get; set; }
        Visibility IsExportExcel4210 { get; set; }
    }
}
