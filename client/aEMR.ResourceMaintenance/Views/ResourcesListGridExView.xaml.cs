using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using aEMR.ViewContracts;
using DataEntities;
//using eHCMS.ViewModels.ResourcesManageVM;
using System.ComponentModel;
//using eHCMS.ViewModels.UserViewModels;
using Orktane.Components;
using aEMR.Controls;
using aEMR.ResourceMaintenance.ViewModels;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResourcesListGridExView))]
    public partial class ResourcesListGridExView : AxUserControl
    {
        public ResourcesListGridExView()
        {
            InitializeComponent();
            
            this.Loaded += new RoutedEventHandler(ResourcesListGridExView_Loaded);
            this.Unloaded += new RoutedEventHandler(ResourcesListGridExView_Unloaded);
        }

        void ResourcesListGridExView_Unloaded(object sender, RoutedEventArgs e)
        {
            //grdResources.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        void ResourcesListGridExView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        
        public int ResourceType
        {
            get { return (int)GetValue(ResourceTypeProperty); }
            set { SetValue(ResourceTypeProperty, value); }
        }

        public static readonly DependencyProperty ResourceTypeProperty = DependencyProperty.Register(
            "ResourceType",
            typeof(int),
            typeof(ResourcesListGridExView),
            new PropertyMetadata(0, new PropertyChangedCallback(OnResourceTypeChanged)));

        private static void OnResourceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ResourcesListGridExView)d).OnResourceTypeChanged(e);
        }

        protected virtual void OnResourceTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            //ViewModel.ResourceCategoryEnum = ResourceType;
        }

    }
}
