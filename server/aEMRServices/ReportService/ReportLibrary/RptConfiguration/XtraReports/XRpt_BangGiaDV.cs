using System;

namespace eHCMS.ReportLib.RptConfiguration.XtraReports
{
    public partial class XRpt_BangGiaDV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BangGiaDV()
        {
            InitializeComponent();
        }

        private void XRpt_BangGiaDV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsMedServicePriceList_Detail1.EnforceConstraints = false;
            spRpt_MedServicePriceList_DetailTableAdapter.Fill(dsMedServicePriceList_Detail1.spRpt_MedServicePriceList_Detail
                , Convert.ToInt64(parMedServiceItemPriceListID.Value)
            );

            //if (parFindPatient.Value.ToString() == "1")
            //{
            //    xrLabel15.Text = "BẢNG KÊ BÁC SĨ THỰC HIỆN PHẨU THUẬT - THỦ THUẬT CỦA NỘI TRÚ";
            //}
            //else if (parFindPatient.Value.ToString() == "0")
            //{
            //    xrLabel15.Text = "BẢNG KÊ BÁC SĨ THỰC HIỆN PHẨU THUẬT - THỦ THUẬT CỦA NGOẠI TRÚ";
            //}
            //else
            //{
            //    xrLabel15.Text = "BẢNG KÊ BÁC SĨ THỰC HIỆN PHẨU THUẬT - THỦ THUẬT CỦA NỘI-NGOẠI TRÚ";
            //}
        }
    }
}
