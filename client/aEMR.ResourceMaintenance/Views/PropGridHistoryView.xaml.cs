using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using aEMR.Controls;

//using eHCMS.ViewModels.ResourcesManageVM;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(PropGridHistoryView))]
    public partial class PropGridHistoryView : AxUserControl
    {
        
        public PropGridHistoryView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PropGridHistoryView_Loaded);
            this.Unloaded += new RoutedEventHandler(PropGridHistoryView_Unloaded);
        }

        void PropGridHistoryView_Unloaded(object sender, RoutedEventArgs e)
        {
            //grdResources.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        void PropGridHistoryView_Loaded(object sender, RoutedEventArgs e)
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
