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
//using eHCMS.ViewModels.ResourcesManageVM;
using DataEntities;
using System.ComponentModel;
using aEMR.Controls;
using Orktane.Components;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResourcesEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ResourcesEditView : AxUserControl
    {
        public ResourcesEditView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ResourcesEditView_Loaded);
            this.Unloaded += new RoutedEventHandler(ResourcesEditView_Unloaded);
        }
        void ResourcesEditView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        void ResourcesEditView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        public void InitData()
        {
            
        }

        public void ResourcesManageVM_UpdateResourcesCompleted(object sender, EventArgs e)
        {
            
        }

    }
}

