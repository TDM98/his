using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardThanhTrungMedDeptFromClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardThanhTrungMedDeptFromClinicDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardThanhTrung_ByIDTableAdapter.Fill(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardThanhTrungInvoice_ByIDTableAdapter.Fill(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID, Convert.ToInt64(InvID.Value));

            decimal total = 0;
            if (dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID != null && dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows[i]["TotalPriceNotVAT"]);
                }
            }

            total = Math.Round(total, 0);

            NumberToLetterConverter converter = new NumberToLetterConverter();
            decimal temp = 0;
            string prefix = "";
            if (total < 0)
            {
                temp = 0 - total;
                prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp = total;
                prefix = "";
            }
            Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }

        private void XRptInwardThanhTrungMedDeptFromClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
