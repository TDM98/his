using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using aEMR.Controls;

namespace aEMR.Registration.Views
{
    public partial class ThongKeDoanhThuDangKyView : UserControl
    {
        public ThongKeDoanhThuDangKyView()
        {
            InitializeComponent();
            //gridRegistrations.UnloadingRowGroup += (s, e) => { Resize(); };
            //gridRegistrations.LoadingRowGroup += (s, e) => { refreshUI = true; };
        }

       
        //bool refreshUI = true;
        List<StackPanel> headers = null;
        DataGridColumnHeadersPresenter dghc = null;

        void Resize()
        {
            if (dghc != null)
            {
                headers = gridRegistrations.GetChildrenByType<StackPanel>().Where(x => x.Name == "ghsp").ToList();
                //headers.ForEach(x =>
                //{
                //    for (int i = 1; i < dghc.Children.Count - 1; i++)
                //    {
                //        (x.Children[i - 1] as DataGridCell).Width = dghc.Children[i].RenderSize.Width;
                //        (x.Children[i - 1] as DataGridCell).Visibility = dghc.Children[i].Visibility;
                //    }

                //    //(x.Children.Last() as DataGridCell).Width = dghc.Children.Last().RenderSize.Width;
                //    //(x.Children.Last() as DataGridCell).Visibility = dghc.Children.Last().Visibility;
                //});
            }
        }

        private void gridRegistrations_LayoutUpdated(object sender, EventArgs e)
        {
            //if (refreshUI && headers == null)
            //{
            //    dghc = gridRegistrations.GetChildrenByType<DataGridColumnHeadersPresenter>().FirstOrDefault();
            //    if (dghc != null)
            //    {
            //        foreach (DataGridColumnHeader dgch in dghc.Children)
            //        {
            //            dgch.SizeChanged += (s, args) => { Resize(); };
            //        }
            //    }
            //}
            //if (refreshUI)
            //    Resize();

            //refreshUI = false;
        }

        private void gridRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

     
      
      
    }
}
