using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp07DuocBV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp07DuocBV()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            baoCao_DuocBVTableAdapter.Fill(dsTemp07DuocBV1.BaoCao_DuocBV, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value));
        }
        private void XRpt_Temp07DuocBV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            this.xrLabel1.Text = "( Kỳ hạn báo cáo : " + Convert.ToDateTime(this.ToDate.Value).Month + " tháng )";
            this.xrLabel9.Text = "" + Convert.ToDateTime(this.ToDate.Value).Year + "";

            var dt = this.dsTemp07DuocBV1.BaoCao_DuocBV;
            if (dt.Rows.Count >0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var dataRow = this.dsTemp07DuocBV1.BaoCao_DuocBV.Rows[i];
                    if (Convert.ToInt32(dataRow["line"]) == 1 )
                    {
                        //xrSL1.Text = dataRow["soluong"].ToString();
                        xrSL1.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                    }
                    else
                        if (Convert.ToInt32(dataRow["line"]) == 3 )
                        {
                            xrSL3.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                        }
                        else
                            if (Convert.ToInt32(dataRow["line"]) == 4)
                            {
                                xrSL4.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                            }
                            else
                                if (Convert.ToInt32(dataRow["line"]) == 5)
                                {
                                    xrSL5.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                }
                                else
                                    if (Convert.ToInt32(dataRow["line"]) == 6)
                                    {
                                        xrSL6.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                    }
                                    else
                                        if (Convert.ToInt32(dataRow["line"]) == 7)
                                        {
                                            xrSL7.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                        }
                                        else
                                            if (Convert.ToInt32(dataRow["line"]) == 9)
                                            {
                                                xrSL9.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                            }
                                            else
                                                if (Convert.ToInt32(dataRow["line"]) == 10)
                                                {
                                                    xrSL10.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                                }
                                                else
                                                    if (Convert.ToInt32(dataRow["line"]) == 13)
                                                    {
                                                        xrSL13.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                                    }
                                                    else
                                                        if (Convert.ToInt32(dataRow["line"]) == 14)
                                                        {
                                                            xrSL14.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                                        }
                                                        else
                                                            if (Convert.ToInt32(dataRow["line"]) == 22)
                                                            {
                                                                xrSL22.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                                            }
                                                            else
                                                                if (Convert.ToInt32(dataRow["line"]) == 24)
                                                                {
                                                                    xrSL24.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                                                }
                                                                else
                                                                    if (Convert.ToInt32(dataRow["line"]) == 25)
                                                                    {
                                                                        xrSL25.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                                                    }
                                                                    else
                                                                        if (Convert.ToInt32(dataRow["line"]) == 27)
                                                                        {
                                                                            xrSL27.Text = String.Format("{0:#,#}", dataRow["soluong"]);
                                                                        }               

                }
            }
        }
    }
}
