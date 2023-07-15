using System;
using System.Collections.Generic;
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

namespace aEMR.Pharmacy.Views
{
    public partial class PrescriptionView : IPrescriptionView
    {
        public PrescriptionView()
        {
            InitializeComponent();
        }
        public string GetValuePreID()
        {
            return tbx_PreID.Text;
        }
        public string GetValuePreHICode()
        {
            return tbx_PreHICode.Text;
        }
        public string GetValueHICode()
        {
            return tbx_HICode.Text;
        }
        public string GetValueInvoiceID()
        {
            return tbx_Invoice.Text;
        }
    
    }
}
