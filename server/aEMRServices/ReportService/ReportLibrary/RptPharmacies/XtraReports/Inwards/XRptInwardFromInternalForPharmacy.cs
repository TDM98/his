using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.Inwards
{
    public partial class XRptInwardFromInternalForPharmacy : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardFromInternalForPharmacy()
        {
            InitializeComponent();
            FillDataInit();
        }

        public void FillDataInit()
        {
            spRpt_InwardDrugInternalDetails_ByIDTableAdapter.Fill(dsInwardDrugInternalInvoice1.spRpt_InwardDrugInternalDetails_ByID, 0);
            spRpt_InwardDrugInternalInvoice_ByIDTableAdapter.Fill(dsInwardDrugInternalInvoice1.spRpt_InwardDrugInternalInvoice_ByID, 0);
        }

        public void FillData()
        {
            spRpt_InwardDrugInternalDetails_ByIDTableAdapter.Fill(dsInwardDrugInternalInvoice1.spRpt_InwardDrugInternalDetails_ByID
                , Convert.ToInt64(InvID.Value));
            spRpt_InwardDrugInternalInvoice_ByIDTableAdapter.Fill(dsInwardDrugInternalInvoice1.spRpt_InwardDrugInternalInvoice_ByID
                , Convert.ToInt64(InvID.Value));
            decimal total = 0;

            if (dsInwardDrugInternalInvoice1.spRpt_InwardDrugInternalDetails_ByID != null && dsInwardDrugInternalInvoice1.spRpt_InwardDrugInternalDetails_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardDrugInternalInvoice1.spRpt_InwardDrugInternalDetails_ByID.Rows.Count; i ++)
                {
                    total += Convert.ToDecimal(dsInwardDrugInternalInvoice1.spRpt_InwardDrugInternalDetails_ByID.Rows[i]["TotalPriceNotVAT"]) ;
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
            Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
        }

        private void XRptInwardFromInternalForPharmacy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
