using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp06CLS : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp06CLS()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            baoCao_HoatDongCanLamSanTableAdapter.Fill(dsTemp06CLS1.BaoCao_HoatDongCanLamSan, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value));
        }
        private void ThongKeDoanhThu_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            this.xrLabel1.Text = "( Kỳ hạn báo cáo : " + Convert.ToDateTime(this.ToDate.Value).Month + " tháng )";
            this.xrLabel9.Text = "" + Convert.ToDateTime(this.ToDate.Value).Year + "";
            var dt = this.dsTemp06CLS1.BaoCao_HoatDongCanLamSan;
            if (dt.Rows.Count == 19)
            {
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    var dataRow = this.dsTemp06CLS1.BaoCao_HoatDongCanLamSan.Rows[i];
                    if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 1)
                    {
                        xrTableCell11.Text = dataRow["Unit"].ToString();
                        xrTableCell16.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell8.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                        //xrTableCell16.Text = isnull(dataRow["NumberOfTest"].ToString(), false); //("N", en-US)
                        //xrTableCell8.Text = isnull(dataRow["InPtNumberOfTest"].ToString(), false);
                    }
                    else if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 2)
                    {
                        xrTableCell10.Text = dataRow["Unit"].ToString();
                        //xrTableCell12.Text = isnull(dataRow["NumberOfTest"].ToString(), false);
                        //xrTableCell15.Text = isnull(dataRow["InPtNumberOfTest"].ToString(), false);
                        xrTableCell12.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell15.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                    }
                    else if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 5)
                    {
                        xrTableCell30.Text = dataRow["Unit"].ToString();
                        xrTableCell31.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell32.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                    }
                    else if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 6)
                    {
                        xrTableCell50.Text = dataRow["Unit"].ToString();
                        xrTableCell51.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell52.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                    }
                    else if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 7)
                    {
                        xrTableCell46.Text = dataRow["Unit"].ToString();
                        //xrTableCell47.Text = isnull(dataRow["NumberOfTest"].ToString(), false);
                        //xrTableCell48.Text = isnull(dataRow["InPtNumberOfTest"].ToString(), false);
                        xrTableCell47.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell48.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                    }
                    else if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 8)
                    {
                        xrTableCell42.Text = dataRow["Unit"].ToString();
                        //xrTableCell43.Text = isnull(dataRow["NumberOfTest"].ToString(), false);
                        //xrTableCell44.Text = isnull(dataRow["InPtNumberOfTest"].ToString(), false);
                        xrTableCell43.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell44.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                    }
                    else if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 9)
                    {
                        xrTableCell54.Text = dataRow["Unit"].ToString();
                        //xrTableCell55.Text = isnull(dataRow["NumberOfTest"].ToString(), false);
                        //xrTableCell56.Text = isnull(dataRow["InPtNumberOfTest"].ToString(), false);
                        xrTableCell55.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell56.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                    }
                    else if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 12)
                    {
                        //xrTableCell95.Text = isnull(dataRow["NumberOfTest"].ToString(), false);
                        //xrTableCell96.Text = isnull(dataRow["InPtNumberOfTest"].ToString(), false);
                        xrTableCell95.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell96.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                    }
                    else if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 15)
                    {
                        xrTableCell67.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell68.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                    }
                    else if (Convert.ToInt32(dataRow["ExamTypeDisplayID"]) == 16)
                    {
                        if (dataRow["Unit"].ToString() == "")
                        {
                            xrTableCell118.Text = "ml";
                        }
                        else
                        {
                            xrTableCell118.Text = dataRow["Unit"].ToString();
                        }
                        //xrTableCell119.Text = isnull(dataRow["NumberOfTest"].ToString(), false);
                        //xrTableCell120.Text = isnull(dataRow["InPtNumberOfTest"].ToString(), false);
                        xrTableCell119.Text = isnull(String.Format("{0:#,#}", dataRow["NumberOfTest"]), false);
                        xrTableCell120.Text = isnull(String.Format("{0:#,#}", dataRow["InPtNumberOfTest"]), false);
                    }
                }
                xrTableCell22.Text = "Lần";
                xrTableCell26.Text = "Lần";
                xrTableCell54.Text = "Lần";
                xrTableCell58.Text = "Lần";
                xrTableCell62.Text = "Lần";
            }
        }
        private string isnull(string str, Boolean i)
        {
            if (str == "0")
            {
                if (i == true)
                {
                    str = "0";
                }
                else
                {
                    str = "";
                }
            }
            return str;
        }

    }
}