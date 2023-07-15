using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp12_TongHop : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp12_TongHop()
        {
            InitializeComponent();
        }

        private void XRpt_Temp12_TongHop_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }

        private void xrSubreport3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (Convert.ToBoolean(IsPatientCOVID.Value))
            {
                xrPageBreak2.Visible = true;
                xrSubreport3.ReportSource = new XRpt_Temp12_COVID();
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).TransactionID.Value = Convert.ToInt32(TransactionID.Value);
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).PtRegistrationID.Value = Convert.ToInt32(PtRegistrationID.Value);
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).FromDate.Value = FromDate.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).ToDate.Value = ToDate.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).ViewByDate.Value = ViewByDate.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).StaffName.Value = StaffName.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).DeptID.Value = Convert.ToInt32(DeptID.Value);
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).DeptName.Value = DeptName.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).RegistrationType.Value = Convert.ToInt64(RegistrationType.Value);
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).parHospitalAdress.Value = parHospitalAdress.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).IsCOVID.Value = true;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).IsKHTHView.Value = IsKHTHView.Value;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if(Convert.ToBoolean(IsPatientCOVID.Value))
            {
                xrSubreport2.ReportSource = new XRpt_Temp12_COVID();
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).TransactionID.Value = Convert.ToInt32(TransactionID.Value);
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).PtRegistrationID.Value = Convert.ToInt32(PtRegistrationID.Value);
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).FromDate.Value = FromDate.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).ToDate.Value = ToDate.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).ViewByDate.Value = ViewByDate.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).StaffName.Value = StaffName.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).DeptID.Value = Convert.ToInt32(DeptID.Value);
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).DeptName.Value = DeptName.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).RegistrationType.Value = Convert.ToInt64(RegistrationType.Value);
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).parHospitalAdress.Value = parHospitalAdress.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).PrintNoChargeItem.Value = PrintNoChargeItem.Value;
                ((XRpt_Temp12_COVID)((XRSubreport)sender).ReportSource).IsKHTHView.Value = IsKHTHView.Value;
            }
            else
            {
                xrSubreport2.ReportSource = new XRpt_Temp12();
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).TransactionID.Value = Convert.ToInt32(TransactionID.Value);
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).PtRegistrationID.Value = Convert.ToInt32(PtRegistrationID.Value);
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).FromDate.Value = FromDate.Value;
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).ToDate.Value = ToDate.Value;
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).ViewByDate.Value = ViewByDate.Value;
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).StaffName.Value = StaffName.Value;
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).DeptID.Value = Convert.ToInt32(DeptID.Value);
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).DeptName.Value = DeptName.Value;
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).RegistrationType.Value = Convert.ToInt64(RegistrationType.Value);
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).parHospitalAdress.Value = parHospitalAdress.Value;
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value;
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).PrintNoChargeItem.Value = PrintNoChargeItem.Value;
                ((XRpt_Temp12)((XRSubreport)sender).ReportSource).IsKHTHView.Value = IsKHTHView.Value;
            }
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).TransactionID.Value = Convert.ToInt32(TransactionID.Value);
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).PtRegistrationID.Value = Convert.ToInt32(PtRegistrationID.Value);
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).FromDate.Value = FromDate.Value;
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).ToDate.Value = ToDate.Value;
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).ViewByDate.Value = ViewByDate.Value;
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).StaffName.Value = StaffName.Value;
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).DeptID.Value = Convert.ToInt32(DeptID.Value);
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).DeptName.Value = DeptName.Value;
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).RegistrationType.Value = Convert.ToInt64(RegistrationType.Value);
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).parHospitalAdress.Value = parHospitalAdress.Value;
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value;
            ((XRpt_Temp12_6556)((XRSubreport)sender).ReportSource).IsKHTHView.Value = IsKHTHView.Value;
        }
    }
}
