using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public sealed partial class XRptPhieuMienGiamNgoaiTru_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuMienGiamNgoaiTru_TV()
        {
            InitializeComponent();
        }

        void XRptPhieuMienGiamNgoaiTru_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            spPhieuMienGiamNgoaiTruTableAdapter.ClearBeforeFill = true;
            if (pV_RegistrationType.Value == null || pV_RegistrationType.Value == DBNull.Value)
            {
                pV_RegistrationType.Value = 24001;
            }
            switch (pV_RegistrationType.Value.ToString())
            {
                case "24003":
                    xrLabel24.Text = "PHIẾU MIỄN GIẢM NỘI TRÚ";
                    break;
                case "24009":
                    xrLabel24.Text = "PHIẾU MIỄN GIẢM BỆNH NHÂN ĐIỀU TRỊ NGOẠI TRÚ";
                    break;
                default:
                    xrLabel24.Text = "PHIẾU MIỄN GIẢM NGOẠI TRÚ";
                    break;
            }
            spPhieuMienGiamNgoaiTruTableAdapter.Fill(dsPhieuMienGiamNgoaiTru1.spPhieuMienGiamNgoaiTru, Convert.ToInt32(parPromoDiscProgID.Value), Convert.ToInt32(parPtRegistrationID.Value), Convert.ToInt32(parPatientPCLReqID.Value), Convert.ToInt64(pV_RegistrationType.Value));
            var cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            var converter = new NumberToLetterConverter();
            if (Parameters["parTotalMienGiam"].Value.ToString() == "0")
            {
                if (dsPhieuMienGiamNgoaiTru1.spPhieuMienGiamNgoaiTru.Count > 0)
                {
                    var total = dsPhieuMienGiamNgoaiTru1.spPhieuMienGiamNgoaiTru.Rows[0]["TotalPromo"] as decimal?;
                    Parameters["parAmountInWords"].Value = converter.Convert(total.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
                    Parameters["parTotalMienGiam"].Value = Convert.ToInt32(total);
                }
                else
                {
                    Parameters["parAmountInWords"].Value = "";
                }
            }
            else
            {
                Parameters["parAmountInWords"].Value = converter.Convert(Parameters["parTotalMienGiam"].Value.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }
    }
}
