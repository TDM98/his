using System.Windows;
using aEMR.ViewContracts;

namespace aEMR.Registration.Views
{
    public partial class RegistrationLeftMenuView : ILeftMenuView
    {
        public RegistrationLeftMenuView()
        {
            InitializeComponent();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           // mnuLeft.Height = LayoutRoot.ActualHeight - mnuLeft.Margin.Top - mnuLeft.Margin.Bottom;
        }

        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            
            RegisterCmd.Style = defaultStyle;
            ReceivePatientCmd.Style = defaultStyle;
            ProcessPaymentCmd.Style = defaultStyle;
            ReceiveInPatientCmd.Style = defaultStyle;
            InPtRegisterCmd.Style = defaultStyle;
            InPtRegister_HighTechServiceCmd.Style = defaultStyle;

            InPatientRegisterCmd_TV.Style = defaultStyle;

            ReceiveInPatientCmd.Style = defaultStyle;
            //HPT: Nếu không thêm các định dạng Style cho những dòng này trong hàm ResetMenuColor, sau khi chuyển focus màu không được set lại rất khó nhìn (08/09/2015)
            //1. Nhận bệnh Vãng lai           
            ReceiveInPatient_Casual_Cmd.Style = defaultStyle;
            //2. Nhận bệnh tiền giải phẫu
            ReceiveInPatient_PreOp_Cmd.Style = defaultStyle;

            ReceiveInPatient_WithHI_Cmd.Style = defaultStyle;            
            
            ReConfirmHI_ForInPtCmd.Style = defaultStyle;
            
            //ReceiveEmergencyPatientCmd.Style = defaultStyle;
            //ReceiveEmergencyPatient_WithHI_Cmd.Style = defaultStyle;
            InPatientAdmissionCmd.Style = defaultStyle;
            ManageInPatientAdmissionCmd.Style = defaultStyle;
            Temp02NoiTruCmd.Style = defaultStyle;
            Temp02NoiTruNewCmd.Style = defaultStyle;
            CreateRptForm02NoiTruCmd.Style = defaultStyle;
            InPtProcessPaymentCmd.Style = defaultStyle;

            InPatientSettlementCmd.Style = defaultStyle;
            InPatientSettlementCmd_TV.Style = defaultStyle;
            DischargeCmd.Style = defaultStyle;

            DischargeCmd_BS.Style = defaultStyle;

            SuggestAdvanceMoneyCmd_TV.Style = defaultStyle;
            SuggestPaymentCmd_TV.Style = defaultStyle;

            SuggestAdvanceMoneyCmd.Style = defaultStyle;
            SuggestPaymentCmd.Style = defaultStyle;

            AdvanceMoneyCmd.Style = defaultStyle;
            InPatientPaymentCmd.Style = defaultStyle;
            KiemToanCmd.Style = defaultStyle;
            ReportPaymentReceiptCmd.Style = defaultStyle;
            RegSummaryCmd.Style = defaultStyle;
            ReCalcHi_Cmd.Style = defaultStyle;
            DeptTranferCmd.Style = defaultStyle;
            DanhSachDKBHYTCmd.Style = defaultStyle;
            ReportQuickConsultationCmd.Style = defaultStyle;
            BangKeThuPhiXNTheoNgayCmd.Style = defaultStyle;
            BangKeThuPhiKB_CDHATheoNgayCmd.Style = defaultStyle;
            ReCalcHiCmd_NgTr.Style = defaultStyle;
            //RegisRoomManageCmd.Style = defaultStyle;
            //PhieuNopTienCmd.Style = defaultStyle;
            RegistrationSummaryCmd.Style = defaultStyle;
            TongHopDoanhThuPKCmd.Style = defaultStyle;
            PCLExamTargetCmd.Style = defaultStyle;

            ReportInPatientNotPayCashAdvanceCmd.Style = defaultStyle;
            ReportAdvanceMoneyCmd.Style = defaultStyle;
            ReportAdvanceMoneyForBillCmd.Style = defaultStyle;
            ReportRepayAdvanceMoneyCmd.Style = defaultStyle;
            ReportPatientSettlementCmd.Style = defaultStyle;
            ReportOutwardMedDeptInflowCmd.Style = defaultStyle;
            ReportInPatientImportExportDepartmentCmd.Style = defaultStyle;
            ReportPatientAreTreatedCmd.Style = defaultStyle;
            ReportGenericPaymentCmd.Style = defaultStyle;
            GenericPaymentCmd.Style = defaultStyle;
            ReportInPatientDischargeNotPayAllBillCmd.Style = defaultStyle;
            ConfirmHIBenefitCmd.Style = defaultStyle;
        }
    }
}