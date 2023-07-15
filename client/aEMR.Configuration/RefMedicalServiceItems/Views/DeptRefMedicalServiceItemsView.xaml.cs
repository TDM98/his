using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Controls;

namespace aEMR.Configuration.RefMedicalServiceItems.Views
{
    public partial class DeptRefMedicalServiceItemsView : UserControl
    {
        public DeptRefMedicalServiceItemsView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(RefMedicalServiceItemsView_Loaded);
        }

        void RefMedicalServiceItemsView_Loaded(object sender, RoutedEventArgs e)
        {
            //AxDataGridTemplateColumn col1 = dtgList.Columns[0] as AxDataGridTemplateColumn;
            //if (col1 != null)
            //{
            //    Binding binding = new Binding();
            //    binding.Source = dtgList.DataContext;
            //    binding.Path = new PropertyPath("bBtnEdit");
            //    binding.Mode = BindingMode.OneWay;
            //    binding.Converter = new BooleanToVisibilityConverter();
            //    col1.VisibilityBinding = binding;
            //    //col1.CellTemplate.
            //}
            
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
