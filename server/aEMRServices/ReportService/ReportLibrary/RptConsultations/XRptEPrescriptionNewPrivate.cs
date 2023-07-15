using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptEPrescriptionNewPrivate : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionNewPrivate()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            this.dsPrescriptionNew1.EnforceConstraints = false;
            this.spPrescriptions_RptHeaderByIssueIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID, Convert.ToInt32(this.parIssueID.Value));
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID, Convert.ToInt32(this.parIssueID.Value), false, false);
            this.spRptGetOrganizationInfoTableAdapter.Fill(this.dsPrescriptionNew1.spRptGetOrganizationInfo);
        }

        private void XRptEPrescriptionNewPrivate_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void calOrganizationName_GetValue(object sender, GetValueEventArgs e)
        {
            object columnValue = ((DataRowView)e.Row).Row["OrganizationName"];
            e.Value = FormatString(columnValue.ToString());
        }

        private void calOrganizationAddress_GetValue(object sender, GetValueEventArgs e)
        {
            object columnValue = ((DataRowView)e.Row).Row["OrganizationAddress"];
            e.Value = FormatString(columnValue.ToString());
        }

        private void calOrganizationNotes_GetValue(object sender, GetValueEventArgs e)
        {
            
            object columnValue = ((DataRowView)e.Row).Row["OrganizationNotes"];
            e.Value = FormatString(columnValue.ToString());
        }

        private string FormatString(string input)
        {
            string[] parts = input.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
            string temp = "";
            foreach (string item in parts)
            {
                temp += item.Replace("\n", "") + Environment.NewLine;
            }
            return temp;
        }
    }
}
