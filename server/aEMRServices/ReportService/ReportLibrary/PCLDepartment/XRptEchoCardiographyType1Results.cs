using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.ReportLib.Enterprise.HeartInstitute;
using eHCMS.ReportLib.Enterprise.DrHuan;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptEchoCardiographyType1Results : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEchoCardiographyType1Results()
        {
            InitializeComponent();
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private void FillData()
        {
            dsEchoCardiographyType1Results1.EnforceConstraints = false;
            sp_Rpt_UltraResParams_EchoCardiographyGetAllTableAdapter.Fill(this.dsEchoCardiographyType1Results1.sp_Rpt_UltraResParams_EchoCardiographyGetAll, Convert.ToInt32(this.paramUltraResParams_EchoCardiographyID.Value), Convert.ToInt32(this.paramPatientPCLReqID.Value));            
            spRpt_PatientPCLRequest_GetHeaderDetailsTableAdapter.Fill(this.dsEchoCardiographyType1Results1.spRpt_PatientPCLRequest_GetHeaderDetails, Convert.ToInt32(this.paramPatientPCLReqID.Value), (bool)this.PatientType.Value);
        }

        private void XRptEchoCardiographyType1Results_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            if (this.dsEchoCardiographyType1Results1.sp_Rpt_UltraResParams_EchoCardiographyGetAll == null || this.dsEchoCardiographyType1Results1.sp_Rpt_UltraResParams_EchoCardiographyGetAll.Rows.Count == 0)
                return;
            ImgPic1.Visible = false;
            ImgPic2.Visible = false;
            xrLine5.Visible = false;
            if (Parameters["paramImage1"].Value != null && Parameters["paramImage1"].Value.ToString().Length > 0)
            {
                //Stream streamImg = new MemoryStream(GetBytes(Parameters["paramImage1"].Value.ToString()));
                //ImgPic1.Image = Image.FromStream(streamImg);
                //ImgPic1.Image = Image.FromFile("D:\\Tulips.jpg");
                string strImageFilePath = Parameters["paramImage1"].Value.ToString();
                if (File.Exists(strImageFilePath))
                {
                    ImgPic1.Visible = true;
                    xrLine5.Visible = true;
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
                    xrLine5.Visible = true;
                    FileStream fileStream = new FileStream(strImageFilePath, FileMode.Open);
                    MemoryStream imgMemBuffer = new MemoryStream();
                    fileStream.Position = 0;
                    fileStream.CopyTo(imgMemBuffer);
                    fileStream.Close();
                    ImgPic2.Image = Image.FromStream(imgMemBuffer);
                }
            }

            //Int16 paramHosOrPC = Convert.ToInt16(Parameters["paramHospitalOrPC"].Value);
            //if (paramHosOrPC == 1) // Hospital
            //{
            //    //xrPicture_HosLogo.Visible = true;
            //    //xrLabel_HosHealthDept.Visible = true;
            //    //xrLabel_HosName.Visible = true;
            //    //xrLabel_HosAddress.Visible = true;
            //}
            //else if (paramHosOrPC == 2) // PC ie. Private Consultation
            //{
            //    if (Parameters["paramOrganizationName"].Value != null && Parameters["paramOrganizationName"].Value.ToString().Length > 0)
            //    {
            //        Parameters["paramOrganizationName"].Value = FormatOrgParamString(Parameters["paramOrganizationName"].Value.ToString());
            //    }
            //    if (Parameters["paramOrganizationAddress"].Value != null && Parameters["paramOrganizationAddress"].Value.ToString().Length > 0)
            //    {
            //        Parameters["paramOrganizationAddress"].Value = FormatOrgParamString(Parameters["paramOrganizationAddress"].Value.ToString());
            //    }
            //    if (Parameters["paramOrganizationNotes"].Value != null && Parameters["paramOrganizationNotes"].Value.ToString().Length > 0)
            //    {
            //        Parameters["paramOrganizationNotes"].Value = FormatOrgParamString(Parameters["paramOrganizationNotes"].Value.ToString());
            //    }

            //    xrLabel_PC_OrgName.Visible = true;           // Bound to paramOrganizationName
            //    xrLabel_PC_OrgAddress.Visible = true;        // Bound to paramOrganizationAddress
            //    xrLabel_PC_OrgNotes.Visible = true;        // Bound to paramOrganizationNotes
                
            //}

            //Parameters["paramOrganizationName"].Value = FormatOrgParamString(Parameters["paramOrganizationName"].Value.ToString());

            var dataHeaderRow = this.dsEchoCardiographyType1Results1.spRpt_PatientPCLRequest_GetHeaderDetails.Rows[0];
            if (dataHeaderRow["PhysicalExamInfo"] != DBNull.Value)
            {
                int commaIdx =  dataHeaderRow["PhysicalExamInfo"].ToString().IndexOf(',');
                if (commaIdx > 0)
                {
                    this.xrLabel_Patient_Height.Text = dataHeaderRow["PhysicalExamInfo"].ToString().Substring(0, commaIdx);

                    if (dataHeaderRow["PhysicalExamInfo"].ToString().Length - commaIdx > 1)
                    {
                        this.xrLabel_Patient_Weight.Text = dataHeaderRow["PhysicalExamInfo"].ToString().Substring(commaIdx + 1);
                    }
                }
            }


            var dataRow = this.dsEchoCardiographyType1Results1.sp_Rpt_UltraResParams_EchoCardiographyGetAll.Rows[0];
            if (dataRow["LDOPPLER_Mitral_Grade"].ToString() == "")
            {
                this.xrLabel209.Visible = false;
                this.xrLabel129.Visible = false;
            }
            else
            {
                this.xrLabel209.Visible = true;
                this.xrLabel129.Visible = true;
            }

            if (dataRow["LDOPPLER_Aortic_Grade"].ToString() == "")
            {
                this.xrLabel208.Visible = false;
                this.xrLabel146.Visible = false;
            }
            else
            {
                this.xrLabel208.Visible = true;
                this.xrLabel146.Visible = true;
            }

            if (dataRow["LDOPPLER_Tricuspid_Grade"].ToString() == "")
            {
                this.xrLabel210.Visible = false;
                this.xrLabel211.Visible = false;
            }
            else
            {
                this.xrLabel210.Visible = true;
                this.xrLabel211.Visible = true;
            }

            if (dataRow["LDOPPLER_Pulmonary_Grade"].ToString() == "")
            {
                this.xrLabel212.Visible = false;
                this.xrLabel213.Visible = false;
            }
            else
            {
                this.xrLabel212.Visible = true;
                this.xrLabel213.Visible = true;
            }

            if (dataRow["V_2D_LSVC"] != DBNull.Value)
            {
                if (Convert.ToInt16(dataRow["V_2D_LSVC"].ToString()) > 0)
                {
                    this.xrLabel_TwoD_LSVC_Plus.Visible = true;
                }
                else
                {
                    this.xrLabel_TwoD_LSVC_Minus.Visible = true;
                }
            }
            if (dataRow["V_2D_Azygos"] != DBNull.Value)
            {
                if (Convert.ToInt16(dataRow["V_2D_Azygos"].ToString()) > 0)
                {
                    this.xrLabel_TwoD_Azygos_Plus.Visible = true;
                }
                else
                {
                    this.xrLabel_TwoD_Azygos_Minus.Visible = true;
                }
            }
            if (dataRow["V_DOPPLER_Mitral_Mr"] != DBNull.Value)
            {
                if (dataRow["V_DOPPLER_Mitral_Mr"].Equals(true) )
                {
                    this.xrLabel_Doppler_MR_Plus.Visible = true;
                }
                else
                {
                    this.xrLabel_Doppler_MR_Minus.Visible = true;
                }
            }
            if (dataRow["V_DOPPLER_Aortic_Ar"] != DBNull.Value)
            {
                if (dataRow["V_DOPPLER_Aortic_Ar"].Equals(true))
                {
                    this.xrLabel_Doppler_AR_Plus.Visible = true;
                }
                else
                {
                    this.xrLabel_Doppler_AR_Minus.Visible = true;
                }
            }
            if (dataRow["V_DOPPLER_Tricuspid_Tr"] != DBNull.Value)
            {
                if (dataRow["V_DOPPLER_Tricuspid_Tr"].Equals(true))
                {
                    this.xrLabel_Doppler_TR_Plus.Visible = true;
                }
                else
                {
                    this.xrLabel_Doppler_TR_Minus.Visible = true;
                }
            }
            if (dataRow["V_DOPPLER_Pulmonary_Pr"] != DBNull.Value)
            {
                if (dataRow["V_DOPPLER_Pulmonary_Pr"].Equals(true))
                {
                    this.xrLabel_Doppler_PR_Plus.Visible = true;
                }
                else
                {
                    this.xrLabel_Doppler_PR_Minus.Visible = true;
                }
            }
            if (dataRow["Conclusion"] != DBNull.Value)
            {
                //Parameters["paramConclusion"].Value = FormatConclusionParamString(dataRow["Conclusion"].ToString());
                Parameters["paramConclusion"].Value = dataRow["Conclusion"].ToString();
            }

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

        private void xrSubLogo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Int16 paramHosOrPC = Convert.ToInt16(Parameters["paramHospitalOrPC"].Value);

            if (paramHosOrPC == 1) // Hospital
            {
                xrSubLogo.ReportSource = new XRptSub_HeartInstituteLogo();
            }
            else if (paramHosOrPC == 2) // PC ie. Private Consultation
            {
                xrSubLogo.ReportSource = new XRptSub_DrHuanLogo();
                if (Parameters["paramOrganizationName"].Value != null && Parameters["paramOrganizationName"].Value.ToString().Length > 0)
                {
                    ((XRptSub_DrHuanLogo)xrSubLogo.ReportSource).parOrganizationName.Value = FormatOrgParamString(Parameters["paramOrganizationName"].Value.ToString());
                }
                if (Parameters["paramOrganizationAddress"].Value != null && Parameters["paramOrganizationAddress"].Value.ToString().Length > 0)
                {
                    ((XRptSub_DrHuanLogo)xrSubLogo.ReportSource).parOrganizationAddress.Value = FormatOrgParamString(Parameters["paramOrganizationAddress"].Value.ToString());
                }
                if (Parameters["paramOrganizationNotes"].Value != null && Parameters["paramOrganizationNotes"].Value.ToString().Length > 0)
                {
                    ((XRptSub_DrHuanLogo)xrSubLogo.ReportSource).parOrganizationNotes.Value = FormatOrgParamString(Parameters["paramOrganizationNotes"].Value.ToString());
                }
            }
        }
        
    }
}
