using System;
using System.Windows.Controls;
using aEMR.Common.Converters;
using DataEntities;

namespace aEMR.DrugDept.Views
{
    public partial class XapNhapPXHangKyGoiView : UserControl
    {
        public XapNhapPXHangKyGoiView()
        {
            InitializeComponent();
        }

        private void cbxCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DecimalConverter value = (DecimalConverter)this.Resources["DecimalConverter"];
            if (Convert.ToInt64(cbxCurrency.SelectedValue) != (long)AllLookupValues.CurrencyTable.VND)
            {
                value.IsVND = false;
            }
            else
            {
                value.IsVND = true;
            }
        }
    }
}
