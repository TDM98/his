using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.IO;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptAbdominalUltrasoundResult : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptAbdominalUltrasoundResult()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsAbdominalUltrasoundResult1.EnforceConstraints = false;
            spGetAbdominalUltrasoundResultTableAdapter.Fill(this.dsAbdominalUltrasoundResult1.spGetAbdominalUltrasoundResult, Convert.ToInt64(this.paramPatientPCLReqID.Value));
            spGetPatientInfoByPatientPCLReqIDTableAdapter.Fill(this.dsAbdominalUltrasoundResult1.spGetPatientInfoByPatientPCLReqID, Convert.ToInt64(this.paramPatientPCLReqID.Value));
        }

        private string FormatOrgParamString(string input)
        {
            string[] parts = input.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
            string temp = "";
            foreach (string item in parts)
            {
                temp += item.Replace("\n", "") + Environment.NewLine;
            }
            return temp;
        }

        private string FormatConclusionParamString(string input)
        {
            string[] parts = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
            string temp = "";
            foreach (string item in parts)
            {
                temp += item.Replace("\r", "") + Environment.NewLine;
            }
            return temp;
        }

        private void XRptAbdominalUltrasoundResult_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            if (this.dsAbdominalUltrasoundResult1.spGetAbdominalUltrasoundResult == null || this.dsAbdominalUltrasoundResult1.spGetAbdominalUltrasoundResult.Rows.Count == 0)
            {
                return;
            }

            if (this.dsAbdominalUltrasoundResult1.spGetPatientInfoByPatientPCLReqID == null || this.dsAbdominalUltrasoundResult1.spGetPatientInfoByPatientPCLReqID.Rows.Count == 0)
            {
                return;
            }

            var PatientInfo = this.dsAbdominalUltrasoundResult1.spGetPatientInfoByPatientPCLReqID.Rows[0];

            if (PatientInfo["Gender"] != DBNull.Value && PatientInfo["Gender"].ToString() == "M")
            {
                tbrProstate.Visible = true;
                tbrUterus.Visible = false;
                tbrRightOvary.Visible = false;
                tbrLeftOvary.Visible = false;
            }
            else
            {
                tbrProstate.Visible = false;
                tbrUterus.Visible = true;
                tbrRightOvary.Visible = true;
                tbrLeftOvary.Visible = true;
            }

            var dataRow = this.dsAbdominalUltrasoundResult1.spGetAbdominalUltrasoundResult.Rows[0];

            if (dataRow["Liver"] != DBNull.Value)
            {
                //lblLiver.Text = FormatConclusionParamString(dataRow["Liver"].ToString());
                //tbcLiver.Text = FormatConclusionParamString(dataRow["Liver"].ToString());
                tbcLiver.Text = dataRow["Liver"].ToString();
            }

            if (dataRow["Gallbladder"] != DBNull.Value)
            {
                //lblGallbladder.Text = FormatConclusionParamString(dataRow["Gallbladder"].ToString());
                //tbcGallbladder.Text = FormatConclusionParamString(dataRow["Gallbladder"].ToString());
                tbcGallbladder.Text = dataRow["Gallbladder"].ToString();
            }

            if (dataRow["Pancreas"] != DBNull.Value)
            {
                //lblPancreas.Text = FormatConclusionParamString(dataRow["Pancreas"].ToString());
                //tbcPancreas.Text = FormatConclusionParamString(dataRow["Pancreas"].ToString());
                tbcPancreas.Text = dataRow["Pancreas"].ToString();
            }

            if (dataRow["Spleen"] != DBNull.Value)
            {
                //lblSpleen.Text = FormatConclusionParamString(dataRow["Spleen"].ToString());
                //tbcSpleen.Text = FormatConclusionParamString(dataRow["Spleen"].ToString());
                tbcSpleen.Text = dataRow["Spleen"].ToString();
            }

            if (dataRow["RightKidney"] != DBNull.Value)
            {
                //lblRightKidney.Text = FormatConclusionParamString(dataRow["RightKidney"].ToString());
                //tbcRightKidney.Text = FormatConclusionParamString(dataRow["RightKidney"].ToString());
                tbcRightKidney.Text = dataRow["RightKidney"].ToString();
            }

            if (dataRow["LeftKidney"] != DBNull.Value)
            {
                //lblLeftKidney.Text = FormatConclusionParamString(dataRow["LeftKidney"].ToString());
                //tbcLeftKidney.Text = FormatConclusionParamString(dataRow["LeftKidney"].ToString());
                tbcLeftKidney.Text = dataRow["LeftKidney"].ToString();
            }

            if (dataRow["Bladder"] != DBNull.Value)
            {
                //lblBladder.Text = FormatConclusionParamString(dataRow["Bladder"].ToString());
                //tbcBladder.Text = FormatConclusionParamString(dataRow["Bladder"].ToString());
                tbcBladder.Text = dataRow["Bladder"].ToString();
            }

            if (dataRow["Prostate"] != DBNull.Value)
            {
                //lblProstate.Text = FormatConclusionParamString(dataRow["Prostate"].ToString());
                //tbcProstate.Text = FormatConclusionParamString(dataRow["Prostate"].ToString());
                tbcProstate.Text = dataRow["Prostate"].ToString();
            }

            if (dataRow["Uterus"] != DBNull.Value)
            {
                //lblUterus.Text = FormatConclusionParamString(dataRow["Uterus"].ToString());
                //tbcUterus.Text = FormatConclusionParamString(dataRow["Uterus"].ToString());
                tbcUterus.Text = dataRow["Uterus"].ToString();
            }

            if (dataRow["RightOvary"] != DBNull.Value)
            {
                //lblRightOvary.Text = FormatConclusionParamString(dataRow["RightOvary"].ToString());
                //tbcRightOvary.Text = FormatConclusionParamString(dataRow["RightOvary"].ToString());
                tbcRightOvary.Text = dataRow["RightOvary"].ToString();
            }

            if (dataRow["LeftOvary"] != DBNull.Value)
            {
                //lblLeftOvary.Text = FormatConclusionParamString(dataRow["LeftOvary"].ToString());
                //tbcLeftOvary.Text = FormatConclusionParamString(dataRow["LeftOvary"].ToString());
                tbcLeftOvary.Text = dataRow["LeftOvary"].ToString();
            }

            if (dataRow["PeritonealFluid"] != DBNull.Value)
            {
                //lblPeritonealFluid.Text = FormatConclusionParamString(dataRow["PeritonealFluid"].ToString());
                //tbcPeritonealFluid.Text = FormatConclusionParamString(dataRow["PeritonealFluid"].ToString());
                tbcPeritonealFluid.Text = dataRow["PeritonealFluid"].ToString();
            }

            if (dataRow["PleuralFluid"] != DBNull.Value)
            {
                //lblPleuralFluid.Text = FormatConclusionParamString(dataRow["PleuralFluid"].ToString());
                //tbcPleuralFluid.Text = FormatConclusionParamString(dataRow["PleuralFluid"].ToString());
                tbcPleuralFluid.Text = dataRow["PleuralFluid"].ToString();
            }

            if (dataRow["AbdominalAortic"] != DBNull.Value)
            {
                //lblAbdominalAortic.Text = FormatConclusionParamString(dataRow["AbdominalAortic"].ToString());
                //tbcAbdominalAortic.Text = FormatConclusionParamString(dataRow["AbdominalAortic"].ToString());
                tbcAbdominalAortic.Text = dataRow["AbdominalAortic"].ToString();
            }

            if (dataRow["Conclusion"] != DBNull.Value)
            {
                //lblConclusion.Text = FormatConclusionParamString(dataRow["Conclusion"].ToString());
                lblConclusion.Text = dataRow["Conclusion"].ToString();
            }

            ImgPic1.Visible = false;
            ImgPic2.Visible = false;

            if (Parameters["paramImage1"].Value != null && Parameters["paramImage1"].Value.ToString().Length > 0)
            {
                string strImageFilePath = Parameters["paramImage1"].Value.ToString();
                if (File.Exists(strImageFilePath))
                {
                    ImgPic1.Visible = true;
                    FileStream fileStream = new FileStream(strImageFilePath, FileMode.Open);
                    MemoryStream imgMemBuffer = new MemoryStream();
                    fileStream.Position = 0;
                    fileStream.CopyTo(imgMemBuffer);
                    fileStream.Close();
                    ImgPic1.Image = Image.FromStream(imgMemBuffer);
                }
            }

            if (Parameters["paramImage2"].Value != null && Parameters["paramImage2"].Value.ToString().Length > 0)
            {
                string strImageFilePath = Parameters["paramImage2"].Value.ToString();
                if (File.Exists(strImageFilePath))
                {
                    ImgPic2.Visible = true;
                    FileStream fileStream = new FileStream(strImageFilePath, FileMode.Open);
                    MemoryStream imgMemBuffer = new MemoryStream();
                    fileStream.Position = 0;
                    fileStream.CopyTo(imgMemBuffer);
                    fileStream.Close();
                    ImgPic2.Image = Image.FromStream(imgMemBuffer);
                }
            }

            if (Parameters["paramOrganizationName"].Value != null && Parameters["paramOrganizationName"].Value.ToString().Length > 0)
            {
                Parameters["paramOrganizationName"].Value = FormatOrgParamString(Parameters["paramOrganizationName"].Value.ToString());
            }
            if (Parameters["paramOrganizationAddress"].Value != null && Parameters["paramOrganizationAddress"].Value.ToString().Length > 0)
            {
                Parameters["paramOrganizationAddress"].Value = FormatOrgParamString(Parameters["paramOrganizationAddress"].Value.ToString());
            }
            if (Parameters["paramOrganizationNotes"].Value != null && Parameters["paramOrganizationNotes"].Value.ToString().Length > 0)
            {
                Parameters["paramOrganizationNotes"].Value = FormatOrgParamString(Parameters["paramOrganizationNotes"].Value.ToString());
            }
        }

    }
}
