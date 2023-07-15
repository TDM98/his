using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace aEMR.Configuration.PCLExamTypePrices.Views
{
    [Export(typeof(PCLExamTypePricesView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PCLExamTypePricesView : UserControl
    {
        public PCLExamTypePricesView()
        {
            InitializeComponent();
        }

        private void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.PCLExamTypePrice objRows = e.Row.DataContext as DataEntities.PCLExamTypePrice;
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
