using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DataEntities;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Maintenance.Views
{
    [Export(typeof(ResourceMaintenanceLogView))]
    public partial class ResourceMaintenanceLogView :AxUserControl
    {
        public ResourceMaintenanceLogView()
        {
            InitializeComponent();
        }

        private void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            ResourceMaintenanceLog objRows = e.Row.DataContext as ResourceMaintenanceLog;
            if (objRows != null && objRows.ObjV_CurrentStatus != null)
            {
                if (objRows.ObjV_CurrentStatus.LookupID == 9999)
                    e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                
                else if (objRows.ObjV_CurrentStatus.LookupID == 9000)
                    e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                
                else if (objRows.ObjV_CurrentStatus.LookupID == 9998)//Đang chờ xác nhận
                    e.Row.Foreground = new SolidColorBrush(Colors.Orange);

                else
                    e.Row.Foreground = new SolidColorBrush(Colors.Green);
            }

        }
    }
}
