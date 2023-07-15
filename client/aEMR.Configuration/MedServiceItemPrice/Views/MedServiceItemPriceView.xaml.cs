using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace aEMR.Configuration.MedServiceItemPrice.Views
{
    [Export(typeof(MedServiceItemPriceView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MedServiceItemPriceView : UserControl
    {
        public MedServiceItemPriceView()
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
                    case "PriceFuture-Active-0":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                            break;
                        }
                    case "PriceOld":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Orange);
                            break;
                        }
                }
            }
        }
    }
}
