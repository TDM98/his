using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward
{
    public partial class XRptFromClinicDeptToDrugDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptFromClinicDeptToDrugDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spOutwardDrugClinicDept_ByOutIDTableAdapter.Fill(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value));
            spOutwardDrugClinicDeptInvoices_GetTableAdapter.Fill(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDeptInvoices_Get, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value));

            decimal total = 0;
            if (dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID != null && dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows.Count > 0)
            {
                for (int i = 0; i < dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows[i]["OutAmount"]);
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

        private void XRptFromClinicDeptToDrugDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
