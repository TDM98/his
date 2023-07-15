using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;
using System.Data;
using System.Collections.Generic;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRptTotalInPtRevenue : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptTotalInPtRevenue()
        {
            InitializeComponent();
        }
        private void XRptTotalInPtRevenue_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string DeptName = "";
            this.spRptTotalInPtRevenueTableAdapter.Fill(this.dsTotalInPtRevenue1.spRptTotalInPtRevenue, Convert.ToDateTime(this.pStartDate.Value), Convert.ToDateTime(this.pEndDate.Value), Convert.ToInt32(this.pDeptID.Value), ref DeptName);
            if (Convert.ToInt32(this.pDeptID.Value) == 0)
                xrLabelTitle.Text = string.Format("CHI TIẾT DOANH THU NỘI TRÚ TỪ NGÀY {0} ĐẾN NGÀY {1}", Convert.ToDateTime(this.pStartDate.Value).ToString("dd/MM/yyyy"), Convert.ToDateTime(this.pEndDate.Value).ToString("dd/MM/yyyy"));
            else
                xrLabelTitle.Text = string.Format("CHI TIẾT DOANH THU\r\n{0} TỪ NGÀY {1} ĐẾN NGÀY {2}", DeptName.ToUpper(), Convert.ToDateTime(this.pStartDate.Value).ToString("dd/MM/yyyy"), Convert.ToDateTime(this.pEndDate.Value).ToString("dd/MM/yyyy"));
            foreach (DataRow row in this.dsTotalInPtRevenue1.spRptTotalInPtRevenue.Rows)
            {
                if ((row["MedicalServiceTypeName"] == null ? null : row["MedicalServiceTypeName"].ToString()) != "Xét nghiệm")
                    row["DeptName"] = null;
            }
            //List<string> DeptNameArray = new List<string>();
            //var LabRows = this.dsTotalInPtRevenue1.spRptTotalInPtRevenue.AsQueryable().Cast<DataRow>().Where(x => x["MedicalServiceTypeName"].ToString() == "Laboratory").ToList();
            //foreach (var aCurrentRow in LabRows)
            //{
            //    var DeptName = aCurrentRow["DeptName"].ToString();
            //    if (!DeptNameArray.Contains(DeptName))
            //    {
            //        var RowIndex = this.dsTotalInPtRevenue1.spRptTotalInPtRevenue.AsQueryable().Cast<DataRow>().ToList().IndexOf(aCurrentRow);
            //        DataRow mRow = this.dsTotalInPtRevenue1.spRptTotalInPtRevenue.NewRow();
            //        mRow["MedServiceName"] = DeptName;
            //        if (DeptNameArray.Count == 0)
            //        {
            //            this.dsTotalInPtRevenue1.spRptTotalInPtRevenue.Rows.InsertAt(mRow, RowIndex);
            //            DataRow mRowFirst = this.dsTotalInPtRevenue1.spRptTotalInPtRevenue.NewRow();
            //            mRowFirst["MedServiceName"] = "XÉT NGHIỆM";
            //            this.dsTotalInPtRevenue1.spRptTotalInPtRevenue.Rows.InsertAt(mRowFirst, RowIndex);
            //        }
            //        else
            //            this.dsTotalInPtRevenue1.spRptTotalInPtRevenue.Rows.InsertAt(mRow, RowIndex);
            //        DeptNameArray.Add(DeptName);
            //    }
            //}
        }
        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //var RowNumber = GetCurrentColumnValue("RowNumber");
            //DetailBand mCurrentRow = (sender as DetailBand);
            //if (string.IsNullOrEmpty(RowNumber.ToString()))
            //    mCurrentRow.Font = new Font(mCurrentRow.Font.FontFamily, mCurrentRow.Font.Size, FontStyle.Bold);
            //else
            //    mCurrentRow.Font = new Font(mCurrentRow.Font.FontFamily, mCurrentRow.Font.Size, FontStyle.Regular);
        }

        private void GroupDeptName_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var DeptName = GetCurrentColumnValue("DeptName") == null ? null : GetCurrentColumnValue("DeptName").ToString();
            GroupHeaderBand mHeader = sender as GroupHeaderBand;
            if (string.IsNullOrEmpty(DeptName))
                mHeader.Controls.Clear();
            else if (mHeader.Controls.Count == 0)
                mHeader.Controls.Add(xrTable4);
        }
    }
}
