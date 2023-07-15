using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using AxLogging;
using DataEntities;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptCashAdvanceBill : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptCashAdvanceBill()
        {
            InitializeComponent();
        }
        void XRptCashAdvanceBill_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            sp_Rpt_GetCashAdvanceBillTableAdapter.ClearBeforeFill = true;

            sp_Rpt_GetCashAdvanceBillTableAdapter.Fill(dsCashAdvanceBill1.sp_Rpt_GetCashAdvanceBill, Convert.ToInt64(this.parPtCashAdvanceID.Value));

            decimal total = 0;
            if (dsCashAdvanceBill1.sp_Rpt_GetCashAdvanceBill != null && dsCashAdvanceBill1.sp_Rpt_GetCashAdvanceBill.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsCashAdvanceBill1.sp_Rpt_GetCashAdvanceBill.Rows[0]["PaymentAmount"]);

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (total < 0)
                {
                    temp = 0 - Math.Round(total, 0);
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp = Math.Round(total, 0);
                    prefix = "";
                }
                this.Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}.",  eHCMSResources.Z0872_G1_Dong.ToLower());
            }
        }

    }
}
