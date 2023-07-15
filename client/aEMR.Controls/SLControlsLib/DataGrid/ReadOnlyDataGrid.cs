using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Collections;
//using System.Linq;
//using Service.Core.Common;
//using System.Collections.Generic;
//using aEMR.Common.Collections;
//using eHCMS.Services.Core.Base;
using aEMR.Common;
/*
 * 20181206 #001 TTM: BM 0005396
 */
namespace aEMR.Controls
{
    /// <summary>
    /// Dung de hien thi du lieu.
    /// 
    /// Override default EnterKey behavior.
    /// </summary>
    public class ReadOnlyDataGrid : DataGrid
    {
        private DataGridDoubleClickBehavior dblClickBehavior;
        public ReadOnlyDataGrid()
        {
            DefaultStyleKey = typeof(DataGrid);
            this.IsReadOnly = true;
            this.LayoutUpdated += new EventHandler(ReadOnlyDataGrid_LayoutUpdated);

            this.Unloaded += new RoutedEventHandler(ReadOnlyDataGrid_Unloaded);
            this.Loaded += new RoutedEventHandler(ReadOnlyDataGrid_Loaded);

            dblClickBehavior = new DataGridDoubleClickBehavior();
            System.Windows.Interactivity.Interaction.GetBehaviors(this).Add(dblClickBehavior);
            this.MouseDoubleClick += ReadOnlyDataGrid_MouseDoubleClick;
        }
        private void ReadOnlyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DblClick != null && this.SelectedItem != null)
            {
                DblClick(sender, new EventArgs<object>(this.SelectedItem));
            }
        }
        void ReadOnlyDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //dblClickBehavior.DoubleClick += new EventHandler<MouseButtonEventArgs>(dblClickBehavior_DoubleClick);
        }
        
        void ReadOnlyDataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            //dblClickBehavior.DoubleClick -= new EventHandler<MouseButtonEventArgs>(dblClickBehavior_DoubleClick);
        }

        public event EventHandler<EventArgs<object>>  DblClick;
        void dblClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(DblClick != null)
            {
                //DblClick(sender, new EventArgs<object>(((DataGridRow)sender).DataContext));
            }
        }
        
        void ReadOnlyDataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            //20190928 TBL: Anh Công nói khi thay đổi layout của grid thì sẽ chạy vào đây => làm cho khi thay đổi tab thì sẽ có những sự kiện được gọi 2 lần
            //if (this.SelectedIndex < 0)
            //{
            //    if (this.ItemsSource != null)
            //    {
            //        IEnumerator enumerator = this.ItemsSource.GetEnumerator();
            //        if (enumerator.MoveNext())
            //        {
            //            //▼====== #001: Tự động focus dòng đầu tiên khi có dữ liệu
            //            //              =>Khi lọc dữ liệu để tìm kiếm theo tên xét nghiệm màn hình đăng ký dịch vụ, không gõ đc kí tự thứ 2 do mất focus trong Text box tên xét nghiệm.
            //            //this.SelectedIndex = 0;   
            //            //this.Focus();
            //            //▲====== #001
                        
            //            //if (!Application.Current.IsRunningOutOfBrowser)
            //            //{
            //            //    System.Windows.Browser.HtmlPage.Plugin.Focus();
            //            //}
            //        }
            //    }
            //}
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }

        }
    }

    public class DoubleClickDataGrid : DataGrid
    {
        private DataGridDoubleClickBehavior dblClickBehavior;
        public DoubleClickDataGrid()
        {
            DefaultStyleKey = typeof(DataGrid);
            //this.IsReadOnly = true;
            this.LayoutUpdated += new EventHandler(ReadOnlyDataGrid_LayoutUpdated);

            this.Unloaded += new RoutedEventHandler(ReadOnlyDataGrid_Unloaded);
            this.Loaded += new RoutedEventHandler(ReadOnlyDataGrid_Loaded);

            dblClickBehavior = new DataGridDoubleClickBehavior();
            System.Windows.Interactivity.Interaction.GetBehaviors(this).Add(dblClickBehavior);
        }

        void ReadOnlyDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            dblClickBehavior.DoubleClick += new EventHandler<MouseButtonEventArgs>(dblClickBehavior_DoubleClick);
        }

        void ReadOnlyDataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            dblClickBehavior.DoubleClick -= new EventHandler<MouseButtonEventArgs>(dblClickBehavior_DoubleClick);
        }

        public event EventHandler<EventArgs<object>> DblClick;
        void dblClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DblClick != null)
            {
                DblClick(sender, new EventArgs<object>(((DataGridRow)sender).DataContext));
            }
        }

        void ReadOnlyDataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.SelectedIndex < 0)
            {
                if (this.ItemsSource != null)
                {
                    IEnumerator enumerator = this.ItemsSource.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        this.SelectedIndex = 0;

                        //if (!Application.Current.IsRunningOutOfBrowser)
                        //{
                        //    System.Windows.Browser.HtmlPage.Plugin.Focus();
                        //}

                        this.Focus();
                    }
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }

        }
    }
}
