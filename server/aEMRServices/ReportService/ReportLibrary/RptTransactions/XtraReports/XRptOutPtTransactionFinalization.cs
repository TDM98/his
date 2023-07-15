using DevExpress.XtraReports.UI;
using System;
using System.Data;
using System.Linq;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRptOutPtTransactionFinalization : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPtTransactionFinalization()
        {
            InitializeComponent();
        }

        private void XRptOutPtTransactionFinalization_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRptOutPtTransactionFinalizationTableAdapter.Fill(dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalization, Convert.ToInt64(pPtRegistrationID.Value), Convert.ToInt64(pV_RegistrationType.Value), Convert.ToInt64(pTranFinalizationID.Value), Convert.ToInt64(pTransactionFinalizationSummaryInfoID.Value));
            spRptOutPtTransactionFinalizationDetailsTableAdapter.Fill(dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalizationDetails, Convert.ToInt64(pPtRegistrationID.Value), Convert.ToInt64(pV_RegistrationType.Value), Convert.ToInt64(pTransactionFinalizationSummaryInfoID.Value));
            if (dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalization.Rows.Count > 0)
            {
                if (Convert.ToDecimal(dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalization.Rows[0]["Amount"]) == Convert.ToDecimal(dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalization.Rows[0]["TotalHasVATAmount"]))
                {
                    xrTableCellDV1.Visible = false;
                }
                if (Convert.ToDecimal(dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalization.Rows[0]["TotalHasVATAmount"]) == 0)
                {
                    xrTableCellDV8.Visible = false;
                    xrTableCellVATPercent1.Visible = false;
                }
                decimal mAmount = 0;
                decimal.TryParse(dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalization.Rows[0]["Amount"].ToString(), out mAmount);
                TotalAmountStr.Value = Globals.ReadMoneyToString(mAmount);
                XRTableCell[] mTitleCells = new XRTableCell[] { xrTableCellDV1, xrTableCellDV2, xrTableCellDV3, xrTableCellDV4, xrTableCellDV5, xrTableCellDV6, xrTableCellDV7, xrTableCell15 };
                XRTableCell[] mAmountCells = new XRTableCell[] { xrTableCellAmount1, xrTableCellAmount2, xrTableCellAmount3, xrTableCellAmount4, xrTableCellAmount5, xrTableCellAmount6, xrTableCellAmount7, xrTableCell29 };
                if (dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalizationDetails.Rows.Count > 0 && dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalizationDetails.Rows.Cast<DataRow>().Any(x => !Convert.ToBoolean(x["IsHasVAT"])))
                {
                    DataTable spRptOutPtTransactionFinalizationDetails = dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalizationDetails.Rows.Cast<DataRow>().Where(x => !Convert.ToBoolean(x["IsHasVAT"])).CopyToDataTable();
                    if (spRptOutPtTransactionFinalizationDetails.Rows.Count > 0 && spRptOutPtTransactionFinalizationDetails.Rows.Count <= 8)
                    {
                        xrTableCellAmount1.ExpressionBindings.Clear();
                        for (int i = 0; i < spRptOutPtTransactionFinalizationDetails.Rows.Count; i++)
                        {
                            var mRow = spRptOutPtTransactionFinalizationDetails.Rows[i];
                            mTitleCells[i].Text = mRow["HITypeDescription"].ToString();
                            mTitleCells[i].Visible = true;
                            mAmountCells[i].Text = Convert.ToDecimal(mRow["TotalPatientPayment"] == null || mRow["TotalPatientPayment"] == DBNull.Value ? 0 : mRow["TotalPatientPayment"]).ToString("#,#.##");
                            mAmountCells[i].Visible = true;
                        }
                    }
                }
                XRTableCell[] mTitleCellsVAT = new XRTableCell[] { xrTableCellDV8, xrTableCellDV9, xrTableCellDV10, xrTableCellDV11, xrTableCellDV12 };
                XRTableCell[] mNotVATCells = new XRTableCell[] { xrTableCellNotVAT1, xrTableCellNotVAT2, xrTableCellNotVAT3, xrTableCellNotVAT4, xrTableCellNotVAT5 };
                XRTableCell[] mVATPercentCells = new XRTableCell[] { xrTableCellVATPercent1, xrTableCellVATPercent2, xrTableCellVATPercent3, xrTableCellVATPercent4, xrTableCellVATPercent5 };
                XRTableCell[] mVATCells = new XRTableCell[] { xrTableCellVAT1, xrTableCellVAT2, xrTableCellVAT3, xrTableCellVAT4, xrTableCellVAT5 };
                XRTableCell[] mAmountCellsVAT = new XRTableCell[] { xrTableCellAmount8, xrTableCellAmount9, xrTableCellAmount10, xrTableCellAmount11, xrTableCellAmount12 };
                if (dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalizationDetails.Rows.Count > 0 && dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalizationDetails.Rows.Cast<DataRow>().Any(x => Convert.ToBoolean(x["IsHasVAT"])))
                {
                    DataTable spRptOutPtTransactionFinalizationDetails = dsOutPtTransactionFinalization1.spRptOutPtTransactionFinalizationDetails.Rows.Cast<DataRow>().Where(x => Convert.ToBoolean(x["IsHasVAT"])).CopyToDataTable();
                    if (spRptOutPtTransactionFinalizationDetails.Rows.Count > 0 && spRptOutPtTransactionFinalizationDetails.Rows.Count <= 7)
                    {
                        for (int i = 0; i < spRptOutPtTransactionFinalizationDetails.Rows.Count; i++)
                        {
                            decimal NotVAT = 0;
                            var mRow = spRptOutPtTransactionFinalizationDetails.Rows[i];
                            mTitleCellsVAT[i].Text = mRow["HITypeDescription"].ToString();
                            mTitleCellsVAT[i].Visible = true;
                            NotVAT = Math.Round(Convert.ToDecimal(mRow["TotalPatientPayment"]) / Convert.ToDecimal(mRow["VATPercent"]), 0);
                            mNotVATCells[i].Text = NotVAT.ToString("#,#.##");
                            mNotVATCells[i].Visible = true;
                            //▼===== 20200717 TTM: Nếu thuế 0 % thì hiện 0 %.
                            if ((Convert.ToDecimal(mRow["VATPercent"]) - 1) > 0)
                            {
                                mVATPercentCells[i].Text = ((Convert.ToDecimal(mRow["VATPercent"]) - 1) * 100).ToString("#,#.##") + " %";
                            }
                            else
                            {
                                mVATPercentCells[i].Text = "0 %";
                            }
                            //▲===== 
                            mVATPercentCells[i].Visible = true;
                            mVATCells[i].Text = (Convert.ToDecimal(mRow["TotalPatientPayment"]) - Math.Round(Convert.ToDecimal(mRow["TotalPatientPayment"]) / Convert.ToDecimal(mRow["VATPercent"]), 0)).ToString("#,#.##");
                            mVATCells[i].Visible = true;
                            mAmountCellsVAT[i].Text = Convert.ToDecimal(mRow["TotalPatientPayment"] == null || mRow["TotalPatientPayment"] == DBNull.Value ? 0 : mRow["TotalPatientPayment"]).ToString("#,#.##");
                            mAmountCellsVAT[i].Visible = true;
                        }
                    }
                }
            }
        }
    }
}
