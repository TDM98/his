using System.Windows;
using System.Windows.Data;
using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IInPatientSummaryDetails : IPatientSummaryInfoCommon
    {
        //Patient CurrentPatient { get; set; }
        //bool IsCrossRegion { get; set; }
        //HealthInsurance ConfirmedHiItem { get; set; }
        //PaperReferal ConfirmedPaperReferal { get; set; }
        ///// <summary>
        ///// Sửa thông tin hành chánh bệnh nhân.
        ///// </summary>
        //void EditGeneralInfoCmd();

        ///// <summary>
        ///// Xác nhận thẻ bảo hiểm.
        ///// </summary>
        //void ConfirmHiCmd();
        ///// <summary>
        ///// Có được phép Confirm thẻ bảo hiểm hay không
        ///// </summary>
        //bool CanConfirmHi { get; set; }

        ////bool CanConfirmPaperReferal { get; set; }
        //double? HiBenefit { get; set; }

        //string HiComment { get; set; }

        //Visibility GeneralInfoVisibility { get; }
        //Visibility ConfirmHiVisibility { get; }
        //Visibility ConfirmPaperReferalVisibility { get; }

        //bool DisplayButtons { get; set; }
        //void ExpandCmd();
        //void CollapseCmd();

        //PatientClassification CurrentPatientClassification { get; set; }
        

        //bool mInfo_CapNhatThongTinBN { get; set; }
        //bool mInfo_XacNhan { get; set; }
        //bool mInfo_XoaThe { get; set; }
        //bool mInfo_XemPhongKham { get; set; }

        //// TxD 09/07/2014 : Added the following Property to enable ReConfirm HI Benefit for InPatient ONLY
        //bool Enable_ReConfirmHI_InPatientOnly { get; set; }

    }
}
