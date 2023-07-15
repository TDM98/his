
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
using aEMR.Common;
using eHCMSLanguage;
using aEMR.Common.Printing;
using aEMR.Common.ConfigurationManager.Printer;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using System.IO;
/*
 * 20181005 #001 TNHX: [BM0000034] Add Report PhieuChiDinh, add message for busyindicator, refactor code
 * 20181030 #002 TNHX: [BM0002176] Update report base on new flow (XacNhanQLBH) and template for ThanhVuHospital
 * 20181113 #003 TNHX: [BM0003235] Print PhieuChiDinh base on list RegDetailsList , PCLRequestList
 * 20181129 #004 TNHX: [BM0005312] Add report PhieuMienGiam & silent print
 * 20181212 #005 TNHX: [BM0005404] Doesn't print PhieuChiDinh if Doctor already printed it 
 * 20181217 #006 TNHX: [BM0005436] Create button to Show report PhieuMienGiam
 * 20181219 #007 TNHX: [BM0005447] Fix print silent of PhieuChiDinh, PhieuMienGiam
 * 20181225 #008 TNHX: [BM0005462] Re-make report PhieuChiDinh
 * 20181226 #009 TNHX: [BM0005467] Add condition: doesn't print PhieuChiDinh with item is "KhamTruocTraSau"
 * 20181227 #010 TNHX: [BM0005470] Add condition print PhieuChiDinh (BN tra sau - khong in lai) && Khong in doi voi DV + CLS huy/hoan tien
 * 20210122 #011 TNHX: [] Change method PrintSlient from export PDF to using PrintDriect of DevExpress
 * 20220401 #012 QTD:  Add config QMS for DeptLocID
 */
namespace aEMR.Registration.ViewModels
{
    public class HandlePayCompletedViewModelBase : ViewModelBase
    {
        #region EVENT HANDLERS

        //public void ProcessPayCompletedEvent(PayForRegistrationCompleted message)
        //{
        //    if (message != null)
        //    {
        //        //Load lai dang ky:
        //        List<PaymentAndReceipt> paymentlist=null;
        //        if (message.PaymentList != null)
        //        {
        //            paymentlist = message.PaymentList.Where(x => x.PaymentID > 0).ToList();
        //        }
        //        var payment = message.Payment;

        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("<Root>");
        //        if (paymentlist != null && paymentlist.Count() > 0)
        //        {
        //            foreach (var details in paymentlist)
        //            {
        //                sb.Append("<IDList>");
        //                sb.AppendFormat("<ID>{0}</ID>", details.PaymentID);
        //                sb.Append("</IDList>");
        //            }
        //        }
        //        else if (payment != null && payment.TransactionID > 0)
        //        {
        //            sb.Append("<IDList>");
        //            sb.AppendFormat("<ID>{0}</ID>", payment.PtTranPaymtID);
        //            sb.Append("</IDList>");
        //        }
        //        else
        //        {
        //            return;
        //        }
        //        sb.Append("</Root>");

        //        if (payment != null || (paymentlist != null && paymentlist.Count() > 0) && message.Registration != null && message.Registration.PtRegistrationID > 0)
        //        {
        //            var printingMode = Globals.ServerConfigSection.CommonItems.ReceiptPrintingMode;
        //            switch (printingMode)
        //            {
        //                case 1:
        //                    ShowReceiptReport(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
        //                    break;
        //                case 2:
        //                    PrintReceipt(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
        //                    break;
        //                case 3:
        //                    {
        //                        ShowReceiptReport(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
        //                        PrintReceipt(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //}

        public void ProcessPayCompletedEvent(PayForRegistrationCompleted message)
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

        public void ReceiptVersion_1_2_4(PayForRegistrationCompleted message)
        {
            if (message == null)
            {
                return;
            }
            /*▼====: #002*/
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
            sb.Append("</Root>");
            /*▲====: #002*/
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

        public void ReceiptVersion_3(PayForRegistrationCompleted message)
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
        private Dictionary<long, List<long>> GroupRegDetailsByLocation(List<PatientRegistrationDetail> registrationDetails)
        {
            if (registrationDetails == null || registrationDetails.Count == 0)
            {
                return null;
            }
            var dictServicesByLocation = new Dictionary<long, List<long>>();
            foreach (var patientRegistrationDetail in registrationDetails)
            {
                if (patientRegistrationDetail.DeptLocID.GetValueOrDefault(-1) > 0)
                {
                    if (dictServicesByLocation.ContainsKey(patientRegistrationDetail.DeptLocID.Value))
                    {
                        dictServicesByLocation[patientRegistrationDetail.DeptLocID.Value].Add(patientRegistrationDetail.PtRegDetailID);
                    }
                    else
                    {
                        dictServicesByLocation.Add(patientRegistrationDetail.DeptLocID.Value, new List<long>() { patientRegistrationDetail.PtRegDetailID });
                    }
                }
            }
            return dictServicesByLocation;
        }
        private Dictionary<long, List<long>> GroupPCLRequestDetailsByLocation(List<PatientPCLRequest> pclRequests)
        {
            if (pclRequests == null || pclRequests.Count == 0)
            {
                return null;
            }
            var dict = new Dictionary<long, List<long>>();
            foreach (var item in pclRequests)
            {
                if (item.DeptLocID > 0)
                {
                    if (dict.ContainsKey(item.DeptLocID))
                    {
                        dict[item.DeptLocID].Add(item.PatientPCLReqID);
                    }
                    else
                    {
                        dict.Add(item.DeptLocID, new List<long>() { });
                        dict[item.DeptLocID].AddRange(item.PatientPCLRequestIndicators.Select(i => i.PCLReqItemID));
                    }
                }
            }
            return dict;
        }

        public void ShowReceiptReport(string paymentxml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
            {
                reportVm.Result = paymentxml;
                //reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML;
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
                    // 20181027 TNHX: [BM0002176] Add report to show
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
            };
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
                //▼====: #011
                case 4:
                    {

                        if (Globals.ServerConfigSection.OutRegisElements.PrintingWithoutExportPDF)
                        {
                            Receipt_V4WithoutExportPDF(paymentxml, registrationDetails, patientPCLRequests);
                        }
                        else
                        {
                            Receipt_V4(paymentxml, registrationDetails, patientPCLRequests);
                        }
                        break;
                    }
                default:
                    {
                        if (Globals.ServerConfigSection.OutRegisElements.PrintingWithoutExportPDF)
                        {
                            Receipt_V4WithoutExportPDF(paymentxml, registrationDetails, patientPCLRequests);
                        }
                        else
                        {
                            Receipt_V4(paymentxml, registrationDetails, patientPCLRequests);
                        }
                        break;
                    }
                    //▲====: #011
            }
        }

        public void Receipt_V1(string paymentxml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            //var receiptForEachLocationPrintingMode = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.ReceiptForEachLocationPrintingMode]);//Globals.ServerConfigSection.CommonItems.ReceiptForEachLocationPrintingMode;

            // Txd 25/05/2014 Replaced ConfigList
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
                                        byte[] ReportForPatient;
                                        byte[] ReportForHI;

                                        contract.EndGetOutPatientReceiptReportXMLInPdfFormat_V3(out ReportForPatient, out ReportForHI, asyncResult);

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
            //▼====: #002
            string parHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string parDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            // 20190228 TNHX If Thermal Printer was set -> using Thermal report else normal
            var receiptForEachLocationPrintingMode = Globals.ServerConfigSection.CommonItems.ReceiptForEachLocationPrintingMode;
            var reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
            if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_NHIET) && allAssignedPrinterTypes[PrinterType.IN_NHIET] != "")
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

                                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_NHIET, data, ActiveXPrintType.ByteArray, "A5");
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
                var parDeptLocIDQMS = Globals.ServerConfigSection.CommonItems.FloorDeptLocation_0 +
                                Globals.ServerConfigSection.CommonItems.FloorDeptLocation_1 +
                                Globals.ServerConfigSection.CommonItems.FloorDeptLocation_2;
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ReportServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;

                            contract.BeginGetOutPatientReceiptReportXMLInPdfFormat_V4(paymentxml, parHospitalName, parDepartmentOfHealth
                                , reportLogoUrl, parDeptLocIDQMS, Globals.DispatchCallback(asyncResult =>
                                    {
                                        try
                                        {
                                            var data = contract.EndGetOutPatientReceiptReportXMLInPdfFormat_V4(asyncResult);

                                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, data, ActiveXPrintType.ByteArray, "A5");
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
                            //▲====: #002
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

        private void PrintReceipt_RegistrationDetails(long paymentID, List<long> regDetailIDs)
        {
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutPatientReceiptReport_RegistrationDetails_InPdfFormat(paymentID, regDetailIDs,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var data = contract.EndGetOutPatientReceiptReport_RegistrationDetails_InPdfFormat(asyncResult);

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

        private void PrintReceipt_PclRequests(long paymentID, List<long> pclRequestIDs)
        {
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutPatientReceiptReport_PclRequests_InPdfFormat(paymentID, pclRequestIDs,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var data = contract.EndGetOutPatientReceiptReport_PclRequests_InPdfFormat(asyncResult);

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

        //▼====: #010
        //▼====: #009
        //▼====: #008
        //▼====: #001
        public void ProcessPrintPhieuChiDinhEvent(PhieuChiDinhForRegistrationCompleted message)
        {
            if (message == null || (message.RegDetailsList != null && message.RegDetailsList.Count == 0 && message.PCLRequestList != null && message.PCLRequestList.Count == 0))
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            bool hasItemToPrint = false;
            sb.Append("<Root>");
            sb.Append("<IDList>");
            if (message.RegDetailsList != null && message.RegDetailsList.Count > 0)
            {
                sb.Append("<ServiceIDList>");
                List<long?> listDeptLocID = message.RegDetailsList.Where(x => x.DoctorStaffID == 0).Select(x => x.DeptLocID).Distinct().ToList();
                foreach (long? deptLocID in listDeptLocID)
                {
                    sb.Append("<DeptData>");
                    sb.AppendFormat("<DeptLocID>{0}</DeptLocID>", deptLocID);
                    string listID = "";
                    foreach (PatientRegistrationDetail itemRegDetail in message.RegDetailsList)
                    {
                        if (itemRegDetail.DoctorStaffID == 0 && itemRegDetail.DeptLocID == deptLocID && itemRegDetail.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM)
                        {
                            hasItemToPrint = true;
                            if (listID == "")
                            {
                                listID += itemRegDetail.ID;
                            }
                            else listID = listID + "," + itemRegDetail.ID;
                        }
                    }
                    sb.AppendFormat("<IDList>{0}</IDList>", listID);
                    sb.Append("</DeptData>");
                    if (listID == "")
                    {
                        sb.Replace("<IDList></IDList>", "");
                        sb.Replace("<DeptData><DeptLocID>" + deptLocID + "</DeptLocID></DeptData>", "");
                    }
                }
                sb.Append("</ServiceIDList>");
                //sb.Replace("<IDList></IDList>", "");
                //sb.Replace("<DeptData></DeptData>", "");
                sb.Replace("<ServiceIDList></ServiceIDList>", "");
            }
            if (message.PCLRequestList != null && message.PCLRequestList.Count > 0)
            {
                sb.Append("<PCLIDList>");
                List<long> listDeptLocID = message.PCLRequestList.Select(x => x.DeptLocID).Distinct().ToList();
                foreach (long deptLocID in listDeptLocID)
                {
                    sb.Append("<DeptData>");
                    sb.AppendFormat("<DeptLocID>{0}</DeptLocID>", deptLocID);
                    string listID = "";
                    foreach (PatientPCLRequest itemPclRequest in message.PCLRequestList)
                    {
                        if (itemPclRequest.PatientPCLRequestIndicators.Count > 0)
                        {
                            foreach (PatientPCLRequestDetail itemPclRequestDetail in itemPclRequest.PatientPCLRequestIndicators)
                            {
                                if (itemPclRequestDetail.RequestedByDoctor == 0 && (itemPclRequestDetail.DeptLocation != null) && itemPclRequestDetail.DeptLocation.DeptLocationID == deptLocID && itemPclRequestDetail.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM)
                                {
                                    hasItemToPrint = true;
                                    if (listID == "")
                                    {
                                        listID += itemPclRequestDetail.PCLReqItemID;
                                    }
                                    else listID = listID + "," + itemPclRequestDetail.PCLReqItemID;
                                }
                            }
                        }
                        else
                        {
                            if (itemPclRequest.DoctorStaffID == 0)
                            {
                                if (listID == "")
                                {
                                    listID += itemPclRequest.PCLReqItemID;
                                }
                                else listID = listID + "," + itemPclRequest.PCLReqItemID;
                            }
                        }
                    }
                    sb.AppendFormat("<IDList>{0}</IDList>", listID);
                    sb.Append("</DeptData>");
                    if (listID == "")
                    {
                        sb.Replace("<IDList></IDList>", "");
                        sb.Replace("<DeptData><DeptLocID>" + deptLocID + "</DeptLocID></DeptData>", "");
                    }
                }
                sb.Append("</PCLIDList>");
                sb.Replace("<PCLIDList></PCLIDList>", "");
            }
            sb.Append("</IDList>");
            sb.Append("</Root>");
            if (message.Registration != null && message.Registration.PtRegistrationID > 0 && hasItemToPrint)
            {
                var printingMode = Globals.ServerConfigSection.CommonItems.PhieuChiDinhPrintingMode;
                switch (printingMode)
                {
                    case 1:
                        ShowPhieuChiDinhReport(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                        break;
                    //▼====: #011
                    case 2:
                        if (Globals.ServerConfigSection.OutRegisElements.PrintingWithoutExportPDF)
                        {
                            PrintPhieuChiDinhWithoutExportPDF(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                        }
                        else
                        {
                            PrintPhieuChiDinh(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                        }
                        break;
                    case 3:
                        ShowPhieuChiDinhReport(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                        if (Globals.ServerConfigSection.OutRegisElements.PrintingWithoutExportPDF)
                        {
                            PrintPhieuChiDinhWithoutExportPDF(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                        }
                        else
                        {
                            PrintPhieuChiDinh(sb.ToString(), message.RegDetailsList, message.PCLRequestList);
                        }
                        break;
                        //▲====: #011
                }
            }
        }
        //▲====: #009
        //▲====: #010

        public void ShowPhieuChiDinhReport(string listIdXml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            void onInitDlg(ICommonPreviewView reportVm)
            {
                reportVm.Result = listIdXml;
                reportVm.eItem = ReportName.REGISTRATION_SPECIFY_VOTES_XML;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        //▲====: #008

        public void PrintPhieuChiDinh(string listIdXml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            ClientLoggerHelper.LogInfo("Start PrintPhieuChiDinh");
            /*▼====: #002*/
            string parHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string parDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutPatientPhieuChiDinhReportXMLInPdfFormat(listIdXml, parHospitalName, parDepartmentOfHealth
                            , reportLogoUrl
                            , Globals.DispatchCallback(asyncResult =>
                        {
                            /*▲====: #002*/
                            try
                            {
                                var data = contract.EndGetOutPatientPhieuChiDinhReportXMLInPdfFormat(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, data, ActiveXPrintType.ByteArray, "A5");
                                Globals.EventAggregator.Publish(printEvt);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.Z2389_G1_Msg_InfoKhTheLayDataInPhieuChiDinh);
                            }
                            finally
                            {
                                ClientLoggerHelper.LogInfo("End PrintPhieuChiDinh");
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo("End has exception PrintPhieuChiDinh");
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        /*▲====: #001*/

        //▼====: #004
        public void ProcessPrintPhieuMienGiamEvent(PhieuMienGiamForRegistrationCompleted message)
        {
            if (message == null)
            {
                return;
            }
            decimal totalMienGiam = 0;
            if (message.PaymentList != null)
            {
                foreach (PaymentAndReceipt item in message.PaymentList)
                {
                    if (item.DiscountAmount > 0)
                    {
                        totalMienGiam += item.DiscountAmount;
                    }
                }
            }
            if (message.Registration != null && message.Registration.PtRegistrationID > 0 && totalMienGiam > 0)
            {
                var printingMode = Globals.ServerConfigSection.CommonItems.PhieuMienGiamPrintingMode;
                if (message.Registration.PromoDiscountProgramObj == null)
                {
                    return;
                }
                long PromoDiscProgramID = message.Registration.PromoDiscountProgramObj.PromoDiscProgID;
                //▼====: #007
                //▼====: #006
                switch (printingMode)
                {
                    case 1:
                        ShowPhieuMienGiamReport(message.Registration.PtRegistrationID, PromoDiscProgramID, totalMienGiam, message.RegDetailsList, message.PCLRequestList);
                        break;
                    case 2:
                        PrintPhieuMienGiam(message.Registration.PtRegistrationID, PromoDiscProgramID, totalMienGiam, message.RegDetailsList, message.PCLRequestList);
                        break;
                    case 3:
                        {
                            ShowPhieuMienGiamReport(message.Registration.PtRegistrationID, message.Registration.PromoDiscountProgramObj.PromoDiscProgID, totalMienGiam, message.RegDetailsList, message.PCLRequestList);
                            PrintPhieuMienGiam(message.Registration.PtRegistrationID, PromoDiscProgramID, totalMienGiam, message.RegDetailsList, message.PCLRequestList);
                        }
                        break;
                }
                //▲====: #006
                //▲====: #007
            }
        }

        //▼====: #006
        public void ShowPhieuMienGiamReport(long PtRegistrationID, long PromoDiscProgID, decimal totalMienGiam, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            void onInitDlg(ICommonPreviewView reportVm)
            {
                reportVm.PromoDiscProgID = PromoDiscProgID;
                reportVm.TotalMienGiam = (long)totalMienGiam;
                reportVm.RegistrationID = PtRegistrationID;
                reportVm.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                reportVm.eItem = ReportName.PHIEUMIENGIAM_NGOAITRU_TV;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        //▲====: #006
        //▼====: #007
        public void PrintPhieuMienGiam(long PtRegistrationID, long PromoDiscProgID, decimal totalMienGiam, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            string parHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string parHospitalAddress = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutPatientPhieuMienGiamNgoaiTruReportInPdfFormat(PtRegistrationID, PromoDiscProgID, (long)totalMienGiam, parHospitalName, parHospitalAddress, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var data = contract.EndGetOutPatientPhieuMienGiamNgoaiTruReportInPdfFormat(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, data, ActiveXPrintType.ByteArray, "A5");
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
        //▲====: #004
        //▲====: #007

        //▼====: #011
        private DocumentPreviewControl DocumentPreview = new DocumentPreviewControl();
        private Dictionary<PrinterType, string> allAssignedPrinterTypes = new PrinterConfigurationManager().GetAllAssignedPrinterType();

        public void Receipt_V4WithoutExportPDF(string paymentxml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            string parHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string parDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string parLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            var receiptForEachLocationPrintingMode = Globals.ServerConfigSection.CommonItems.ReceiptForEachLocationPrintingMode;
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
            if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_NHIET) && allAssignedPrinterTypes[PrinterType.IN_NHIET] != "")
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

                                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_NHIET, data, ActiveXPrintType.ByteArray, "A5");
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
                            contract.BeginGetOutPatientReceiptReportXML_V4(paymentxml, parHospitalName, parDepartmentOfHealth
                                    , parLogoUrl, Globals.DispatchCallback(asyncResult =>
                                    {
                                        try
                                        {
                                            var data = contract.EndGetOutPatientReceiptReportXML_V4(asyncResult);

                                            var printerName = "";
                                            if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_PHIEU) && allAssignedPrinterTypes[PrinterType.IN_PHIEU] != "")
                                            {
                                                printerName = allAssignedPrinterTypes[PrinterType.IN_PHIEU];
                                            }
                                            MemoryStream memoryStream = new MemoryStream(data);
                                            XtraReport XtraReportModel = new XtraReport();
                                            XtraReportModel.PrintingSystem.LoadDocument(memoryStream);

                                            DocumentPreview.DocumentSource = XtraReportModel;
                                            DocumentPreview.PrintDirect(printerName);
                                            GlobalsNAV.ShowMessagePopup(eHCMSResources.A0466_G1_Msg_InfoDaIn);
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

        public void PrintPhieuChiDinhWithoutExportPDF(string listIdXml, List<PatientRegistrationDetail> registrationDetails = null, List<PatientPCLRequest> patientPCLRequests = null)
        {
            /*▼====: #002*/
            string parHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string parDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string parLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutPatientPhieuChiDinhReportXML(listIdXml, parHospitalName, parDepartmentOfHealth
                            , parLogoUrl, Globals.DispatchCallback(asyncResult =>
                        {
                            /*▲====: #002*/
                            try
                            {
                                var data = contract.EndGetOutPatientPhieuChiDinhReportXML(asyncResult);
                                //var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, data, ActiveXPrintType.ByteArray, "A5");
                                //Globals.EventAggregator.Publish(printEvt);

                                var printerName = "";
                                if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_PHIEU) && allAssignedPrinterTypes[PrinterType.IN_PHIEU] != "")
                                {
                                    printerName = allAssignedPrinterTypes[PrinterType.IN_PHIEU];
                                }
                                MemoryStream memoryStream = new MemoryStream(data);
                                XtraReport XtraReportModel = new XtraReport();
                                XtraReportModel.PrintingSystem.LoadDocument(memoryStream);
                                DocumentPreview.DocumentSource = XtraReportModel;
                                DocumentPreview.PrintDirect(printerName);
                                GlobalsNAV.ShowMessagePopup(eHCMSResources.A0466_G1_Msg_InfoDaIn);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.Z2389_G1_Msg_InfoKhTheLayDataInPhieuChiDinh);
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
        //▲====: #011
    }
}
