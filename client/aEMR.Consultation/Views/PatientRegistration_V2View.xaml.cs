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
using aEMR.ViewContracts;

namespace aEMR.ConsultantEPrescription.Views
{
    public partial class PatientRegistration_V2View : UserControl, IPatientRegistration_V2View
    {
        public PatientRegistration_V2View()
        {
            InitializeComponent();
        }
    }
}
