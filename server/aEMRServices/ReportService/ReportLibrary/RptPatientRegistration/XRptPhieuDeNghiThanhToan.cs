using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DataEntities;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public sealed partial class XRptPhieuDeNghiThanhToan : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuDeNghiThanhToan()
        {
            InitializeComponent();
        }


        void XRptPhieuDeNghiThanhToan_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            spRptPatientCashAdvReminder_ByIDTableAdapter.ClearBeforeFill = true;

            spRptPatientCashAdvReminder_ByIDTableAdapter.Fill(dsPhieuDeNghiTamUng1.spRptPatientCashAdvReminder_ByID, Convert.ToInt64(param_RptPtCashAdvRemID.Value));

            decimal total = 0;
            if (dsPhieuDeNghiTamUng1.spRptPatientCashAdvReminder_ByID != null && dsPhieuDeNghiTamUng1.spRptPatientCashAdvReminder_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsPhieuDeNghiTamUng1.spRptPatientCashAdvReminder_ByID.Rows[0]["RemAmount"]);

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (total < 0)
                {
                    temp = 0 - Math.Round(total, 0);
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am);
                }
                else
                {
                    temp = Math.Round(total, 0);
                    prefix = "";
                }
                this.Parameters["param_AmountInWords"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }

        }
    }
}
