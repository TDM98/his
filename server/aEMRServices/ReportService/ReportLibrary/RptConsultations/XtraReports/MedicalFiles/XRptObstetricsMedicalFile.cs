namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptObstetricsMedicalFile : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptObstetricsMedicalFile()
        {
            InitializeComponent();
        }
        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            this.spRptGeneralOutPtMedicalFileTableAdapter.Fill(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFile, PtRegDetailID.Value as long?, PtRegistrationID.Value as long?, V_RegistrationType.Value as long?,null);
            this.spRptGeneralOutPtMedicalFileDeptDetailInfosTableAdapter.Fill(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos, PtRegDetailID.Value as long?, PtRegistrationID.Value as long?);
            try
            {
                if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos != null && this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows.Count > 0)
                {
                    txtDeptDetailCode0.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[0]["DeptCode"].ToString();
                    txtDeptDetailFromDate0.Text = string.Format("{0:HH} giờ {0:mm} phút, {0:dd/MM/yyyy}", this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[0]["FromDate"]);
                    txtDeptDetailDate01.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[0]["ConsultingDate"].ToString().PadLeft(3, '0')[0].ToString();
                    txtDeptDetailDate02.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[0]["ConsultingDate"].ToString().PadLeft(3, '0')[1].ToString();
                    txtDeptDetailDate03.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[0]["ConsultingDate"].ToString().PadLeft(3, '0')[2].ToString();
                }
                if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos != null && this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows.Count > 1)
                {
                    txtDeptDetailCode1.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[1]["DeptCode"].ToString();
                    txtDeptDetailFromDate1.Text = string.Format("{0:HH} giờ {0:mm} phút, {0:dd/MM/yyyy}", this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[1]["FromDate"]);
                    txtDeptDetailDate11.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[1]["ConsultingDate"].ToString().PadLeft(3, '0')[0].ToString();
                    txtDeptDetailDate12.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[1]["ConsultingDate"].ToString().PadLeft(3, '0')[1].ToString();
                    txtDeptDetailDate13.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[1]["ConsultingDate"].ToString().PadLeft(3, '0')[2].ToString();
                }
                if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos != null && this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows.Count > 2)
                {
                    txtDeptDetailCode2.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[2]["DeptCode"].ToString();
                    txtDeptDetailFromDate2.Text = string.Format("{0:HH} giờ {0:mm} phút, {0:dd/MM/yyyy}", this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[2]["FromDate"]);
                    txtDeptDetailDate21.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[2]["ConsultingDate"].ToString().PadLeft(3, '0')[0].ToString();
                    txtDeptDetailDate22.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[2]["ConsultingDate"].ToString().PadLeft(3, '0')[1].ToString();
                    txtDeptDetailDate23.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[2]["ConsultingDate"].ToString().PadLeft(3, '0')[2].ToString();
                }
                if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos != null && this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows.Count > 3)
                {
                    txtDeptDetailCode3.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[3]["DeptCode"].ToString();
                    txtDeptDetailFromDate3.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[3]["FromDate"].ToString();
                    txtDeptDetailFromDate3.Text = string.Format("{0:HH} giờ {0:mm} phút, {0:dd/MM/yyyy}", this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[3]["FromDate"]);
                    txtDeptDetailDate31.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[3]["ConsultingDate"].ToString().PadLeft(3, '0')[0].ToString();
                    txtDeptDetailDate32.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[3]["ConsultingDate"].ToString().PadLeft(3, '0')[1].ToString();
                    txtDeptDetailDate33.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileDeptDetailInfos.Rows[3]["ConsultingDate"].ToString().PadLeft(3, '0')[2].ToString();
                }
            }
            catch { }
            //this.spRptGeneralOutPtMedicalFileFilmsRecvTableAdapter.Fill(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv, PtRegDetailID.Value as long?);
            //if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv != null && this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 0)
            //{
            //    int TotalFilmsRecv = 0;
            //    if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 0)
            //    {
            //        txtFilmsRecvName1.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["PCLSubCategoryDescription"].ToString();
            //        if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"] != null &&
            //            this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"] != DBNull.Value &&
            //            !string.IsNullOrEmpty(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"].ToString()))
            //        {
            //            txtFilmsRecvValue1.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"].ToString();
            //            TotalFilmsRecv += Convert.ToInt32(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"]);
            //        }
            //    }
            //    if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 1)
            //    {
            //        txtFilmsRecvName2.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["PCLSubCategoryDescription"].ToString();
            //        if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"] != null &&
            //            this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"] != DBNull.Value &&
            //            !string.IsNullOrEmpty(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"].ToString()))
            //        {
            //            txtFilmsRecvValue2.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"].ToString();
            //            TotalFilmsRecv += Convert.ToInt32(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"]);
            //        }
            //    }
            //    if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 2)
            //    {
            //        txtFilmsRecvName3.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["PCLSubCategoryDescription"].ToString();
            //        if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"] != null &&
            //            this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"] != DBNull.Value &&
            //            !string.IsNullOrEmpty(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"].ToString()))
            //        {
            //            txtFilmsRecvValue3.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"].ToString();
            //            TotalFilmsRecv += Convert.ToInt32(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"]);
            //        }
            //    }
            //    if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 3)
            //    {
            //        txtFilmsRecvName4.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["PCLSubCategoryDescription"].ToString();
            //        if (this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"] != null &&
            //            this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"] != DBNull.Value &&
            //            !string.IsNullOrEmpty(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"].ToString()))
            //        {
            //            txtFilmsRecvValue4.Text = this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"].ToString();
            //            TotalFilmsRecv += Convert.ToInt32(this.dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"]);
            //        }
            //    }
            //    txtFilmsRecvTotalValue.Text = TotalFilmsRecv.ToString();
            //}
        }
    }
}