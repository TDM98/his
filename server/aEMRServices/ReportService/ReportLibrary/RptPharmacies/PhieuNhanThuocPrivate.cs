using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using System.Data;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class PhieuNhanThuocPrivate : DevExpress.XtraReports.UI.XtraReport
    {
        public PhieuNhanThuocPrivate()
        {
            InitializeComponent();
        }
       
        public void FillData()
        {
            spRpt_PhieuNhanThuoc_InfoTableAdapter.Fill((this.DataSource as DataSchema.dsPhieuNhanThuoc).spRpt_PhieuNhanThuoc_Info, Convert.ToInt64(this.OutiID.Value));
            spRpt_PhieuNhanThuoc_DetailsTableAdapter.Fill((this.DataSource as DataSchema.dsPhieuNhanThuoc).spRpt_PhieuNhanThuoc_Details, Convert.ToInt64(this.OutiID.Value));
            spRptGetOrganizationInfoTableAdapter.Fill(this.dsPhieuNhanThuoc1.spRptGetOrganizationInfo);

            decimal total = 0;
            if (dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details != null && dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details.Rows.Count > 0)
            {
                for (int i = 0; i < dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details.Rows[i]["Amount"]);
                }
                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (total < 0)
                {
                    temp = 0 -Math.Round(total,0);
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp = Math.Round(total, 0);
                    prefix = "";
                }
                this.Parameters["ReadMoney"].Value = string.Format("{0} : ",  eHCMSResources.R0823_G1_Vietthanhchu) + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
            }
        }

        private void PhieuNhanThuocPrivate_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
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

    }
}
