using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace aEMR.Configuration.Exemptions.Views
{
    [Export(typeof(Exemptions_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Exemptions_AddEditView : UserControl
    {
        public Exemptions_AddEditView()
        {
            InitializeComponent();
        }
        private void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.MedServiceItemPrice objRows = e.Row.DataContext as DataEntities.MedServiceItemPrice;
            if (objRows != null)
            {
                switch (objRows.PriceType)
                {
                    case "PriceCurrent":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        }
                    case "PriceFuture-Active-1":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                            break;
                        }
                        //case "PriceOld":
                        //    {
                        //        e.Row.Foreground = new SolidColorBrush(Colors.Orange);
                        //        break;
                        //    }
                }
            }
        }
    }
}
