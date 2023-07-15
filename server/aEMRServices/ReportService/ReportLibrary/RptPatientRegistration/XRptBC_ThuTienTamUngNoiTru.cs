using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBC_ThuTienTamUngNoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBC_ThuTienTamUngNoiTru()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptBaoCaoThuVienPhiNgoaiTru_BeforePhint);
        }

        public void FillData()
        {
            dsXRptBC_ThuTienTamUngNoiTru1.EnforceConstraints = false;
            spXRptBC_ThuTienTamUngNoiTruTableAdapter.Fill(dsXRptBC_ThuTienTamUngNoiTru1.spXRptBC_ThuTienTamUngNoiTru
                                                                , Convert.ToInt32(this.RepPaymentRecvID.Value)
                                                                , Convert.ToInt32(this.StaffID.Value)
                                                                , Convert.ToDateTime(this.FromDate.Value)
                                                                , Convert.ToDateTime(this.ToDate.Value)
                                                                , Convert.ToInt32(this.Case.Value)
                                                                , Convert.ToInt32(this.V_PaymentMode.Value)); //--26/01/2021 DatTB
        }

        private void XRptBaoCaoThuVienPhiNgoaiTru_BeforePhint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            //decimal TotalAmount = 0;

            //if (dsXRptBC_ThuTienVienPhiNgoaiTru1.spXRptBC_ThuTienVienPhiNgoaiTru != null && dsXRptBC_ThuTienVienPhiNgoaiTru1.spXRptBC_ThuTienVienPhiNgoaiTru.Rows.Count > 0)
            //{

            //    for (int i = 0; i < dsXRptBC_ThuTienVienPhiNgoaiTru1.spXRptBC_ThuTienVienPhiNgoaiTru.Rows.Count; i++)
            //    {
            //        TotalAmount += ((Convert.ToDecimal(dsXRptBC_ThuTienVienPhiNgoaiTru1.spXRptBC_ThuTienVienPhiNgoaiTru.Rows[i]["PayAmount"]))
            //                         - (Convert.ToDecimal(dsXRptBC_ThuTienVienPhiNgoaiTru1.spXRptBC_ThuTienVienPhiNgoaiTru.Rows[i]["CancelAmount"]))
            //                         - (Convert.ToDecimal(dsXRptBC_ThuTienVienPhiNgoaiTru1.spXRptBC_ThuTienVienPhiNgoaiTru.Rows[i]["RefundAmount"])));
            //    }
            //    this.Parameters["TotalAmount"].Value = TotalAmount;
            //    this.Parameters["parReadMoneyTotalAmount"].Value = Globals.ReadMoneyToString(TotalAmount);
            //}
        }
    }
}

