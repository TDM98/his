using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
/*
20170111 #001 CMN: Fix unable set default width for expand menu
*/
namespace aEMR.ConsultantEPrescription.Views
{
    public partial class ConsultationLeftMenuView : ILeftMenuView
    {
        public ConsultationLeftMenuView()
        {
            InitializeComponent();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //==== #001
            //mnuLeft.Height = LayoutRoot.ActualHeight - mnuLeft.Margin.Top - mnuLeft.Margin.Bottom;
            //==== #001
        }
        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];

            SummaryCmd.Style = defaultStyle;
            CommonRecs.Style = defaultStyle;
            ConsultationCmd.Style = defaultStyle;
            PrescriptionCmd.Style = defaultStyle;
            PatientSummaryCmd.Style = defaultStyle;
            //ManagePatientDetailsCmd.Style = defaultStyle;

            Consultation_InPt_OutDept_Cmd.Style = defaultStyle;
            Consultation_InPt_OutHos_Cmd.Style = defaultStyle;
            InPrescriptionCmd.Style = defaultStyle;
            InPatientDischargeCmd.Style = defaultStyle;
            PatientInstructionCmd.Style = defaultStyle;
            
            PCLRequestCmd.Style = defaultStyle;
            PCLLaboratoryRequestCmd.Style = defaultStyle;
            PCLRequestHenCLSCmd.Style = defaultStyle;
            ConsultRoomDetailCmd.Style = defaultStyle;
            PCLImagingExtResultsCmd.Style = defaultStyle;

            DiagnosisTreatmentByDoctorStaffIDCmd.Style = defaultStyle;
            AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd.Style = defaultStyle;

            PCLLaboratoryResultsCmd.Style = defaultStyle;
            PCLImagingResultsCmd.Style = defaultStyle;
            PCLExamTargetCmd.Style = defaultStyle;

            PCLLaboratoryRequestCmd_InPt.Style = defaultStyle;
            PCLImageRequestCmd_InPt.Style = defaultStyle;

            UpdateRequiredNumberCmd.Style = defaultStyle;
            UpdatePresenceDailyCmd.Style = defaultStyle;
            TransferToCmd.Style = defaultStyle;
            TransferFromCmd.Style = defaultStyle;
            TransferPCLCmd.Style = defaultStyle;

            HoiChanCmd.Style = defaultStyle;
            SurgeryCmd.Style = defaultStyle;
            WaitForSurgeryCmd.Style = defaultStyle;

            SurgeryBookingCmd.Style = defaultStyle;
            SurgicalReportCmd.Style = defaultStyle;
            ReportSurgeryCmd.Style = defaultStyle;

            Consultation_InPt_Daily_Cmd.Style = defaultStyle;
        }
    }
}
