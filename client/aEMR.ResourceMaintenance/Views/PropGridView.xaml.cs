using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using aEMR.Controls;

//using eHCMS.ViewModels.ResourcesManageVM;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(PropGridView))]
    public partial class PropGridView : AxUserControl
    {
        public PropGridView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PropGridView_Loaded);
            this.Unloaded += new RoutedEventHandler(PropGridView_Unloaded);
        }

        void PropGridView_Unloaded(object sender, RoutedEventArgs e)
        {
            //grdResources.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        void PropGridView_Loaded(object sender, RoutedEventArgs e)
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
