using System;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class RptHospitalClientContractDetails : DevExpress.XtraReports.UI.XtraReport
    {
        public RptHospitalClientContractDetails()
        {
            InitializeComponent();
        }
        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spHospitalClientContractDetails_ExcelTableAdapter.Fill(dsHospitalClientContractDetails1.spHospitalClientContractDetails_Excel, this.pHosClientContractID.Value as long?, false);
            if (dsHospitalClientContractDetails1.spHospitalClientContractDetails_Excel != null && dsHospitalClientContractDetails1.spHospitalClientContractDetails_Excel.Rows != null
                && dsHospitalClientContractDetails1.spHospitalClientContractDetails_Excel.Rows.Count > 0)
            {
                pCompanyName.Value = dsHospitalClientContractDetails1.spHospitalClientContractDetails_Excel.Rows[0]["CompanyName"];
                if (dsHospitalClientContractDetails1.spHospitalClientContractDetails_Excel.Rows[0]["ContractDate"] != null && dsHospitalClientContractDetails1.spHospitalClientContractDetails_Excel.Rows[0]["ContractDate"] != DBNull.Value)
                {
                    pTitleName.Value = string.Format("Theo hợp đồng số: {0} ngày {1:dd} tháng {1:MM} năm {1:yyyy}", dsHospitalClientContractDetails1.spHospitalClientContractDetails_Excel.Rows[0]["ContractNo"], Convert.ToDateTime(dsHospitalClientContractDetails1.spHospitalClientContractDetails_Excel.Rows[0]["ContractDate"]));
                }
            }
        }
    }
}