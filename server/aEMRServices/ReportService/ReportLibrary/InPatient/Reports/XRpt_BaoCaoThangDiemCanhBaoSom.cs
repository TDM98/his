using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.InPatient.Reports
{
    public partial class XRpt_BaoCaoThangDiemCanhBaoSom : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoThangDiemCanhBaoSom()
        {
            InitializeComponent();
        }

        private void XRpt_BaoCaoThangDiemCanhBaoSom_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_BaoCaoThangDiemCanhBaoSom1.EnforceConstraints = false;
            spXRpt_BaoCaoThangDiemCanhBaoSomTableAdapter.Fill(dsXRpt_BaoCaoThangDiemCanhBaoSom1.spXRpt_BaoCaoThangDiemCanhBaoSom
                , Convert.ToInt64(PhyExamID.Value));
            if (dsXRpt_BaoCaoThangDiemCanhBaoSom1.spXRpt_BaoCaoThangDiemCanhBaoSom != null && dsXRpt_BaoCaoThangDiemCanhBaoSom1.spXRpt_BaoCaoThangDiemCanhBaoSom.Rows.Count > 0)
            {
                if (dsXRpt_BaoCaoThangDiemCanhBaoSom1.spXRpt_BaoCaoThangDiemCanhBaoSom.Columns["MucDoDau"] != null && dsXRpt_BaoCaoThangDiemCanhBaoSom1.spXRpt_BaoCaoThangDiemCanhBaoSom.Rows[0]["MucDoDau"] == DBNull.Value)
                {
                    xrLabel88.Text = null;
                    xrLabel89.Text = null;
                    xrLabel90.Text = null;
                    xrLabel91.Text = null;
                    xrLabel92.Text = null;
                    xrPictureBox1.Image = null;
                }
            }
        }
    }
}
