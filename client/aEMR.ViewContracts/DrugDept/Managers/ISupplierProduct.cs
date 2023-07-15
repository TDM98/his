using aEMR.Common;
namespace aEMR.ViewContracts
{
    public interface ISupplierProduct
    {
        bool IsChildWindow { get; set; }
        bool mTim { get; set; }
        bool mThemMoi { get; set; }
        bool mChinhSua { get; set; }

        LeftModuleActive LeftModule { get; set; }
        long ExportFor { get; set; }
    }
}
