using System;
using DevExpress.XtraReports.UI;
using System.Linq;
/*
 * 20181029 #001 TTM: BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
 * 20181101 #002 TTM: BM 0004220: Thêm điều kiện chỉ FillData lúc cần thiết (khi bệnh nhân có toa hướng thần hoặc toa chứ TPCN/MP) và thêm toa TPCN/MP
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescription_GN_HT_SubLeft_TV2 : XtraReport
    {
        public XRptEPrescription_GN_HT_SubLeft_TV2()
        {
            InitializeComponent();
        }

        private void XRptEPrescription_SubLeft_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }
    }
}
