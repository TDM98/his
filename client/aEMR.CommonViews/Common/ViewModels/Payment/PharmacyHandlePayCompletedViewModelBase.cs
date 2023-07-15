using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Common.BaseModel;
using System.Text;
using eHCMSLanguage;
using aEMR.Common.Printing;
using aEMR.Common.ConfigurationManager.Printer;
/*
 * 20190520 #001 TNHX:  [BM0006874] Create ViewModelBase for PrintReceipt At Pharmacy
 * 20220511 #002 QTD:   In biên lai màn hình xác nhận BHYT
 * 20220929 #003 DatTB: In biên lai bệnh nhân: Đổi cấu hình máy nhiệt thành máy in hóa đơn
 */
namespace aEMR.Common.ViewModels
{
    public class PharmacyHandlePayCompletedViewModelBase : ViewModelBase
    {
        #region EVENT HANDLERS
        public void ProcessPayCompletedEvent(PayForRegistrationCompletedAtConfirmHIView message)
        {
            switch (Globals.ServerConfigSection.CommonItems.ReceiptVersion)
            {
                case 1:
                    {
                        ReceiptVersion_1_2_4(message);
                        break;
                    }
                case 2:
                    {
                        ReceiptVersion_1_2_4(message);
                        break;
                    }
                case 3:
                    {
                        ReceiptVersion_3(message);
                        break;
                    }
                case 4:
                    {
                        ReceiptVersion_1_2_4(message);
                        break;
                    }
                default:
                    {
                        ReceiptVersion_1_2_4(message);
                        break;
                    }
            }
        }

        public void ReceiptVersion_1_2_4(PayForRegistrationCompletedAtConfirmHIView message)
        {
            if (message == null)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<Root>");
            if (message.PaymentList != null)
            {
                foreach (PaymentAndReceipt item in message.PaymentList)
                {
                    sb.Append("<IDList>");
                    sb.AppendFormat("<ID>{0}</ID>", item.MyID);
                    sb.AppendFormat("<OutPtCashAdvanceID>{0}</OutPtCashAdvanceID>", item.OutPtCashAdvanceID);
                    sb.Append("</IDList>");
                }
            }
            //▼====: #002
            else if (message.Registration != null && message.Registration.PatientTransaction != null && message.Registration.PatientTransaction.PatientCashAdvances != null)
            {
                long lastOutPtCashAdvanceID = message.Registration.PatientTransaction.PatientCashAdvances != null ?
                    message.Registration.PatientTransaction.PatientCashAdvances.Max(p => p.OutPtCashAdvanceID) : 0;
                if(lastOutPtCashAdvanceID > 0)
                {
                    sb.Append("<IDList>");
                    sb.AppendFormat("<ID>{0}</ID>", 0);
                    sb.AppendFormat("<OutPtCashAdvanceID>{0}</OutPtCashAdvanceID>", lastOutPtCashAdvanceID);
                    sb.Append("</IDList>");
                }
            }
            //▲====: #002
            sb.Append("</Root>");
            if (message.Registration != null && message.Registration.PtRegistrationID > 0)
            {
                var printingMode = Globals.ServerConfigSection.CommonItems.ReceiptPrintingMode;
                switch (printingMode)
                {
                    case 1:
                        ShowReceiptReport(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                        break;
                    case 2:
                        PrintReceipt(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                        break;
                    case 3:
                        {
                            ShowReceiptReport(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                            PrintReceipt(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                        }
                        break;
                }
            }
        }

        public void ReceiptVersion_3(PayForRegistrationCompletedAtConfirmHIView message)
        {
            if (message == null || message.PaymentList == null || !message.PaymentList.Any(x => x.PaymentID > 0))
            {
                return;
            }

            List<PaymentAndReceipt> paymentlist = message.PaymentList;
            StringBuilder sbPatient = new StringBuilder();
            StringBuilder sbHI = new StringBuilder();

            foreach (var details in paymentlist)
            {
                if (Globals.ServerConfigSection.CommonItems.IsPrintReceiptPatientNoPay || details.IsPrintReceiptForPT)
                {
                    sbPatient.Append("<IDList>");
                    sbPatient.AppendFormat("<ID>{0}</ID>", details.PaymentID);
                    sbPatient.Append("</IDList>");
                }
                if (Globals.ServerConfigSection.CommonItems.IsPrintReceiptHINoPay || details.IsPrintReceiptForHI)
                {
                    sbHI.Append("<IDList>");
                    sbHI.AppendFormat("<ID>{0}</ID>", details.PaymentID);
                    sbHI.Append("</IDList>");
                }
            }

            string patientPaymentXML = null;
            string hiPaymentXML = null;

            if (sbPatient.Length > 0)
            {
                patientPaymentXML = "<Root>" + sbPatient.ToString() + "</Root>";
            }
            if (sbHI.Length > 0)
            {
                hiPaymentXML = "<Root>" + sbHI.ToString() + "</Root>";
            }

            Receipt_V3(patientPaymentXML, hiPaymentXML);
        }

        #endregion
        public void ShowReceiptReport(string paymentxml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            void onInitDlg(ICommonPreviewView reportVm)
            {
                reportVm.Result = paymentxml;
                switch (Globals.ServerConfigSection.CommonItems.ReceiptVersion)
                {
                    case 1:
                        {
                            reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML;
                            break;
                        }
                    case 2:
                        {
                            reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V2;
                            break;
                        }
                    case 4:
                        {
                            reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4;
                            break;
                        }
                    default:
                        {
                            reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4;
                            break;
                        }
                }
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void PrintReceipt(string paymentxml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            switch (Globals.ServerConfigSection.CommonItems.ReceiptVersion)
            {
                case 1:
                    {
                        Receipt_V1(paymentxml, registrationDetails, patientPCLRequests);
                        break;
                    }
                case 2:
                    {
                        Receipt_V2(paymentxml, registrationDetails, patientPCLRequests);
                        break;
                    }
                case 4:
                    {
                        Receipt_V4(paymentxml, registrationDetails, patientPCLRequests);
                        break;
                    }
                default:
                    {
                        Receipt_V4(paymentxml, registrationDetails, patientPCLRequests);
                        break;
                    }
            }
        }

        public void Receipt_V1(string paymentxml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            var receiptForEachLocationPrintingMode = Globals.ServerConfigSection.CommonItems.ReceiptForEachLocationPrintingMode;
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetOutPatientReceiptReportXMLInPdfFormat(paymentxml,
                                Globals.DispatchCallback(asyncResult =>
                                {
                                    try
                                    {
                                        var data = contract.EndGetOutPatientReceiptReportXMLInPdfFormat(asyncResult);

                                        var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_HOA_DON, data, ActiveXPrintType.ByteArray);
                                        Globals.EventAggregator.Publish(printEvt);
                                    }
                                    catch (Exception ex)
                                    {
                                        ClientLoggerHelper.LogInfo(ex.ToString());
                                        MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
                                    }
                                    finally
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void Receipt_V2(string paymentxml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            var receiptForEachLocationPrintingMode = Globals.ServerConfigSection.CommonItems.ReceiptForEachLocationPrintingMode;
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutPatientReceiptReportXMLInPdfFormat_V2(paymentxml,
                                Globals.DispatchCallback(asyncResult =>
                                {
                                    try
                                    {
                                        var data = contract.EndGetOutPatientReceiptReportXMLInPdfFormat_V2(asyncResult);

                                        var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_HOA_DON, data, ActiveXPrintType.ByteArray);
                                        Globals.EventAggregator.Publish(printEvt);
                                    }
                                    catch (Exception ex)
                                    {
                                        ClientLoggerHelper.LogInfo(ex.ToString());
                                        MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
                                    }
                                    finally
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void Receipt_V3(string patientPaymentXML, string hiPaymentXML)
        {
            var receiptForEachLocationPrintingMode = Globals.ServerConfigSection.CommonItems.ReceiptForEachLocationPrintingMode;
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutPatientReceiptReportXMLInPdfFormat_V3(patientPaymentXML, hiPaymentXML,
                                Globals.DispatchCallback(asyncResult =>
                                {
                                    try
                                    {
                                        contract.EndGetOutPatientReceiptReportXMLInPdfFormat_V3(out byte[] ReportForPatient, out byte[] ReportForHI, asyncResult);

                                        if (ReportForPatient != null)
                                        {
                                            var printForPatient = new ActiveXPrintEvt(this, PrinterType.IN_HOA_DON, ReportForPatient, ActiveXPrintType.ByteArray);
                                            Globals.EventAggregator.Publish(printForPatient);

                                        }
                                        if (ReportForHI != null)
                                        {
                                            var printForHI = new ActiveXPrintEvt(this, PrinterType.IN_KHAC, ReportForHI, ActiveXPrintType.ByteArray);
                                            Globals.EventAggregator.Publish(printForHI);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ClientLoggerHelper.LogInfo(ex.ToString());
                                        MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
                                    }
                                    finally
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void Receipt_V4(string paymentxml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            string parHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string parDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            var receiptForEachLocationPrintingMode = Globals.ServerConfigSection.CommonItems.ReceiptForEachLocationPrintingMode;
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
            if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_NHIET) && allAssignedPrinterTypes[PrinterType.IN_NHIET] != "") //==== #003
            {
                this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ReportServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginGetOutPatientReceiptReportXMLInPdfFormat_V4_Thermal(paymentxml, parHospitalName, parDepartmentOfHealth,
                                    Globals.DispatchCallback(asyncResult =>
                                    {
                                        try
                                        {
                                            var data = contract.EndGetOutPatientReceiptReportXMLInPdfFormat_V4_Thermal(asyncResult);
                                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_NHIET, data, ActiveXPrintType.ByteArray, "A5"); //==== #003
                                            Globals.EventAggregator.Publish(printEvt);
                                        }
                                        catch (Exception ex)
                                        {
                                            ClientLoggerHelper.LogInfo(ex.ToString());
                                            MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
                                        }
                                        finally
                                        {
                                            this.HideBusyIndicator();
                                        }
                                    }), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        ClientLoggerHelper.LogInfo(ex.ToString());
                        this.HideBusyIndicator();
                    }
                });

                t.Start();
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ReportServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;

                            contract.BeginGetOutPatientReceiptReportXMLInPdfFormat_V4(paymentxml, parHospitalName, parDepartmentOfHealth
                                , reportLogoUrl, "", Globals.DispatchCallback(asyncResult =>
                                    {
                                        try
                                        {
                                            var data = contract.EndGetOutPatientReceiptReportXMLInPdfFormat_V4(asyncResult);
                                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_HOA_DON, data, ActiveXPrintType.ByteArray, "A5");
                                            Globals.EventAggregator.Publish(printEvt);
                                        }
                                        catch (Exception ex)
                                        {
                                            ClientLoggerHelper.LogInfo(ex.ToString());
                                            MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
                                        }
                                        finally
                                        {
                                            this.HideBusyIndicator();
                                        }
                                    }), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        ClientLoggerHelper.LogInfo(ex.ToString());
                        this.HideBusyIndicator();
                    }
                });

                t.Start();
            }
        }
    }
}
