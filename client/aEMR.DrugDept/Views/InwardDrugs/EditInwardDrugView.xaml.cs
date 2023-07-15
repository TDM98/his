using aEMR.Common.Converters;
using aEMR.ViewContracts;

namespace aEMR.DrugDept.Views
{
    public partial class EditInwardDrugView :IResetConverter
    {
        public EditInwardDrugView()
        {
            InitializeComponent();
        }
        public void ResetConverter(bool IsVND)
        {
            DecimalConverter value = (DecimalConverter)this.Resources["DecimalConverter"];
            value.IsVND = IsVND;  
        }
      
    }
}
