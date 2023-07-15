using DataEntities;
using System.Windows;

namespace aEMR.ViewContracts
{
    public interface ISupplier_Edit
    {
        Supplier SelectedSupplier { get; set; }
        void LoadDsHangCC();

        Visibility bOKButton { get; set; }
    }
}
