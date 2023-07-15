using System;
using DevExpress.XtraReports.UI;
using System.Linq;
/*
 * 20181029 #001 TTM: BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
 * 20181101 #002 TTM: BM 0004220: Thêm điều kiện chỉ FillData lúc cần thiết (khi bệnh nhân có toa hướng thần hoặc toa chứ TPCN/MP) và thêm toa TPCN/MP
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescription_SubLeft_V5 : XtraReport
    {
        public XRptEPrescription_SubLeft_V5()
        {
            InitializeComponent();
        }

        private void XRptEPrescription_SubLeft_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            if (Convert.ToInt64(parV_RegistrationType.Value) == 24001)
            {
                sp_GetListDoctorOnPrescriptionsTableAdapter.Fill(dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions, Convert.ToInt64(parPtRegDetailID.Value), Convert.ToInt64(parV_RegistrationType.Value));
                if (dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions != null && dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows.Count > 0)
                {
                    xrLabel2.Text = "";
                    xrLabel4.Text = "";
                    xrLabel5.Text = "";
                    for (int i = 0; i < dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows.Count; i++)
                    {
                        if (i == 0)
                        {
                            xrLabel5.Text +="DANH SÁCH BÁC SĨ" + "\n" + dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows[i]["SpecialistTypeNotes"] ;
                        }
                        if (dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows[i]["IsExtend"].ToString() == "True")
                        {
                            xrLabel4.Text += dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows[i]["FullName"] + "\n";
                            xrLabel3.Visible = true;
                            xrLabel4.Visible = true;
                        }
                        else
                        {
                            xrLabel2.Text += dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows[i]["FullName"] + "\n";
                        }
                    }
                }
            }
            else
            {
                sp_GetListDoctorOnPrescriptionsTableAdapter.Fill(dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions, Convert.ToInt64(parPtRegDetailID.Value), Convert.ToInt64(parV_RegistrationType.Value));
                if (dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions != null && dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows.Count > 0)
                {
                    xrLabel2.Text = "";
                    xrLabel4.Text = "";
                    xrLabel5.Text = "";
                    for (int i = 0; i < dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows.Count; i++)
                    {
                        if (i == 0)
                        {
                            xrLabel5.Text += "DANH SÁCH BÁC SĨ" + "\n" + dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows[i]["SpecialistTypeNotes"];
                        }
                        if (dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows[i]["IsExtend"].ToString() == "True")
                        {
                            xrLabel4.Text += dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows[i]["FullName"] + "\n";
                            xrLabel3.Visible = true;
                            xrLabel4.Visible = true;
                        }
                        else
                        {
                            xrLabel2.Text += dsEPrescription_SubLeft1.sp_GetListDoctorOnPrescriptions.Rows[i]["FullName"] + "\n";
                        }
                    }
                }
            }
        }
    }
}
