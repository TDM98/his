using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace eHCMS.ReportLib.InPatient.Reports
{
    public partial class XRptVitalSign_InPt_SubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptVitalSign_InPt_SubReport()
        {
            InitializeComponent();
        }

        private void XRptVitalSign_InPt_SubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsXRptVitalSign_InPt1.EnforceConstraints = false;
            spXRptVitalSign_InPtTableAdapter.Fill(dsXRptVitalSign_InPt1.spXRptVitalSign_InPt, Convert.ToInt64(PtRegistrationID.Value), Convert.ToInt64(V_RegistrationType.Value));
            int rowNum = 0;
            int pageCount = Convert.ToInt32(Page.Value);
            foreach (var item in dsXRptVitalSign_InPt1.Tables[0].Rows)
            {
                dsXRptVitalSign_InPt1.Tables[0].Rows[rowNum]["GroupChart"] = rowNum / 24;
                rowNum++;
            }
            if (dsXRptVitalSign_InPt1.Tables[0].Rows.Count % 24 != 0)
            {
                int count = 0;
                while (dsXRptVitalSign_InPt1.Tables[0].Rows.Count % 24 > 0)
                {
                    count++;
                    DataRow newRow = dsXRptVitalSign_InPt1.Tables[0].NewRow();
                    newRow["Temperature"] = DBNull.Value;
                    newRow["Pulse"] = DBNull.Value;
                    newRow["Hour"] = DBNull.Value;
                    newRow["Day"] = dsXRptVitalSign_InPt1.Tables[0].Rows[0]["Day"].ToString() + count;
                    newRow["GroupChart"] = dsXRptVitalSign_InPt1.Tables[0].Rows[dsXRptVitalSign_InPt1.Tables[0].Rows.Count - 1]["GroupChart"];
                    dsXRptVitalSign_InPt1.Tables[0].Rows.Add(newRow);
                }
            }

            DataTable outputTable = new DataTable();

            // Header row's first column is same as in inputTable
            outputTable.Columns.Add("Data");

            int col = 0;
            // Header row's second column onwards, 'inputTable's first column taken
            //foreach (DataRow inRow in dsXRptVitalSign_InPt1.Tables[0].Rows)
            //{
            //    col++;
            //    string newColName = "Column" + col;
            //    outputTable.Columns.Add(newColName);
            //}
            for (int i = 0; i < 24; i++)
            {
                string newColName = "Column" + i;
                outputTable.Columns.Add(newColName);
            }
            //outputTable.Columns.Add("GroupChart");

            // Add rows by looping columns        
            for (int rCount = 4; rCount <= dsXRptVitalSign_InPt1.Tables[0].Columns.Count - 2; rCount++)
            {

                DataRow newRow = outputTable.NewRow();
                newRow[0] = dsXRptVitalSign_InPt1.Tables[0].Columns[rCount].ColumnName.ToString();
                for (int cCount = 0; cCount < 24; cCount++)
                {
                    string colValue = dsXRptVitalSign_InPt1.Tables[0].Rows[pageCount * 24 + cCount][rCount].ToString();
                    newRow[cCount + 1] = colValue;
                }
                //newRow[25] = p;
                outputTable.Rows.Add(newRow);


                // First column is inputTable's Header row's second column

            }
            XRTable tableDetailHA = new XRTable();
            tableDetailHA.BeginInit();
            for (int j = 0; j < 2; j++)
            {
                XRTableRow detailRowSP = new XRTableRow();
                for (int i = 1; i < outputTable.Columns.Count; i++)
                {
                    XRTableCell detailCell = new XRTableCell();
                    detailCell.Text = outputTable.Rows[j][i].ToString();
                    detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                    detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom;
                    //detailCell.Summary.Running = SummaryRunning.Group;

                    detailRowSP.Cells.Add(detailCell);
                    detailRowSP.KeepTogether = true;
                }
                tableDetailHA.Rows.Add(detailRowSP);
            }
            tableDetailHA.SizeF = new SizeF(634.86F, 46);
            //foreach (XRTableRow item in tableDetailHA)
            //{
            //    item.HeightF = 23;
            //}
            tableDetailHA.AnchorVertical = VerticalAnchorStyles.Both;
            tableDetailHA.LocationF = new PointF(112.14F, 0);
            tableDetailHA.Rows.FirstRow.HeightF = 23;

            ReportHeader.Controls.Add(tableDetailHA);
            tableDetailHA.EndInit();

            XRTable tableDetail = new XRTable();
            tableDetail.BeginInit();
            for (int j = 2; j < outputTable.Rows.Count; j++)
            {
                XRTableRow detailRowSP = new XRTableRow();
                for (int i = 1; i < outputTable.Columns.Count; i++)
                {
                    XRTableCell detailCell = new XRTableCell();
                    detailCell.Name = outputTable.Columns[i].ColumnName;
                    detailCell.Text = outputTable.Rows[j][i].ToString();
                    detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                    detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom;
                    //detailRowSP.HeightF = 46;
                    //detailCell.Summary.Running = SummaryRunning.Group;
                    detailRowSP.Cells.Add(detailCell);
                    detailRowSP.KeepTogether = true;
                }
                tableDetail.Rows.Add(detailRowSP);
            }


            tableDetail.AnchorVertical = VerticalAnchorStyles.Both;
            tableDetail.SizeF = new SizeF(634.86F, 138);
            tableDetail.LocationF = new PointF(112.14F, tableDetailHA.BottomF);

            foreach (XRTableRow item in tableDetail)
            {
                item.HeightF = 46F;
            }
            ReportHeader.Controls.Add(tableDetail);

            tableDetail.EndInit();

            XRTable tableDetailOther = new XRTable();
            tableDetailOther.BeginInit();
            for (int j = 0; j < 4; j++)
            {
                XRTableRow detailRowSP = new XRTableRow();
                for (int i = 1; i < outputTable.Columns.Count; i++)
                {
                    XRTableCell detailCell = new XRTableCell();
                    detailCell.Text = "";
                    detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                    detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom;
                    //detailCell.Summary.Running = SummaryRunning.Group;
                    //detailRowSP.HeightF = 46;
                    detailRowSP.Cells.Add(detailCell);
                    detailRowSP.KeepTogether = true;

                }
                tableDetailOther.Rows.Add(detailRowSP);
            }


            tableDetailOther.AnchorVertical = VerticalAnchorStyles.Both;
            tableDetailOther.SizeF = new SizeF(634.86F, 184);
            tableDetailOther.LocationF = new PointF(112.14F, tableDetail.BottomF);

            foreach (XRTableRow item in tableDetailOther)
            {
                item.HeightF = 46F;
            }
            ReportHeader.Controls.Add(tableDetailOther);

            tableDetailOther.EndInit();


        }
    }
}
