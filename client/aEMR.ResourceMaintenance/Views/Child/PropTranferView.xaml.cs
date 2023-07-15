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
//using eHCMS.ViewModels.Consultations;
using DataEntities;
using System.ComponentModel;
using System.Reflection;
using aEMR.Common;
//using eHCMS.ViewModels.eHCMSConfigurationManagerViewModel;
using aEMR.Controls;
using Orktane.Components;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(PropTranferView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PropTranferView : AxUserControl
    {
        
        public PropTranferView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PropTranferView_Loaded);
            this.Unloaded += new RoutedEventHandler(PropTranferView_Unloaded);
        }
        void PropTranferView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        void PropTranferView_Loaded(object sender, RoutedEventArgs e)
        {
            
           
        }
        
        public void InitData()
        {
           
        }

        private long GetLoggedUserIDSession()
        {
            return 1;
        }
    }
}

