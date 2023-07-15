using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using aEMR.Controls;

//using eHCMS.ViewModels.ResourcesManageVM;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(PropGridMoveHistoryView))]
    public partial class PropGridMoveHistoryView : AxUserControl
    {
        
        public PropGridMoveHistoryView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PropGridMoveHistoryView_Loaded);
            this.Unloaded += new RoutedEventHandler(PropGridMoveHistoryView_Unloaded);
        }

        void PropGridMoveHistoryView_Unloaded(object sender, RoutedEventArgs e)
        {
            //grdResources.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        void PropGridMoveHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        public void InitData()
        {
            
        }

        private void DataGridDoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
            {
                DoubleClick(this, null);
            }
        }
        public event EventHandler DoubleClick;
        
    }
}
