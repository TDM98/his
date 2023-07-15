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

using System.Collections.ObjectModel;

using aEMR.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;

namespace aEMR.ConsultantEPrescription.Views
{
    //[Export(typeof(UCModulesTreeView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PatientTreeView : UserControl
    {
        public PatientTreeView()
        {
            InitializeComponent();
        }

    }
}
