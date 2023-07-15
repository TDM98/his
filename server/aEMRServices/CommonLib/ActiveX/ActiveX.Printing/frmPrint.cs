using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eHCMS.ActiveX.Printing
{
    public partial class frmPrint : Form
    {
        public frmPrint()
        {
            InitializeComponent();
        }
        public void PrintPdf(string fileName)
        {
            axAcroPDF1.LoadFile(fileName);

            axAcroPDF1.printAll();
            Wait(500);
        }
        private void Wait(int miliseconds)
        {
            DateTime start = DateTime.Now;
            while (start.AddMilliseconds(miliseconds) >= DateTime.Now)
            {
                Application.DoEvents();
            }
        }
    }
}
