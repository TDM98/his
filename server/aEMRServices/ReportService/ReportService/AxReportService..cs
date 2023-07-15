using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Text;
using System;
using System.IO;
using DevExpress.XtraReports.UI;
using eHCMS.ReportLib.RptPatientRegistration;
using eHCMS.ReportLib.RptPharmacies;
using eHCMS.ReportLib.RptTransactions;
using eHCMS.ReportLib.RptConsultations;
using ErrorLibrary;
using System.Runtime.Serialization;
using AxLogging;
using ErrorLibrary.Resources;
using eHCMS.ReportLib.RptPharmacies.SupplierPharmacyPaymentReqs.XtraReports;
using eHCMS.ReportLib.RptPharmacies.XtraReports.OutInternal;
using eHCMS.ReportLib.RptPharmacies.XtraReports.OutInsurance;
using eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral;
using eHCMS.ReportLib.RptPharmacies.XtraReports;
using eHCMS.ReportLib.RptPharmacies.Temp;
using eHCMS.ReportLib.RptDrugDepts.XtraReports.Request;
using SupplierPharmacyPaymentReqs = eHCMS.ReportLib.RptPharmacies.SupplierPharmacyPaymentReqs.XtraReports.SupplierPharmacyPaymentReqs;
using System.Linq;
using DataEntities;
using eHCMS.ReportLib.RptAppointment;
using eHCMSLanguage;
using eHCMS.ReportLib.RptConsultations.XtraReports;
using eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards;

using aEMR.DataAccessLayer.Providers;
using eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports;
using eHCMS.ReportLib.RptTransactions.XtraReports.Accountant;
using eHCMS.ReportLib.PCLDepartment;
using System.Xml;
using System.Xml.Serialization;
using DevExpress.Pdf;
using System.Data.SqlClient;
using System.Net;
/*
* 20181023 #001 CMN: Added firsttime load for import libraries to auto print in first time
* 20181030 #002 TNHX: [BM0002176] Add params HospitalName, DepartmentOfHealth for PhieuChiDinh and Receipt
* 20181201 #003 TNHX: [BM0005312] Add slient print report PhieuMienGiam
* 20181219 #004 TNHX: [BM0005447] Fix print silent of PhieuChiDinh, PhieuMienGiam
* 20190424 #005 TNHX: [BM0006716] [BM0006777] Create PhieuNhanThuoc, PhieuNhanThuocBHYT, PhieuNhanThuocSummary for Thermal, Apply InNhiet When Save Prescription at Screen "BanThuocLe"
* 20190917 #006 TNHX: [BM0013247] Apply search for XRptInOutStockValueDrugDept_TV base on config "AllowSearchInReport"
* 20191029 #007 TNHX: [0013247] Apply search for XRptInOutStock base on config "AllowSearchInReport" + XRptInOutStockValue
* 20200128 #008 TNHX: [] Apply search for XRptInOutStockGeneral base on config "AllowSearchInReport"
* 20200906 #009 TNHX: [] Change method PrintSilent from export PDF to using PrintDriect of DevExpress
* 20200915 #010 TNHX: [] Add parameter for Prescriptions
* 20210122 #011 TNHX: [] Add new method when create report
* 20210911 #012 QTD: Add new report
* 20220211 #013 QTD: Add new report
* 20220215 #014 QTD: Add new report to PDF
* 20220401 #015 QTD: Add config DeptLocID for QMS
* 20220613 #016 QTD: Add search in BC Nhap NCC
* 20220823 #017 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
* 20221007 #018 DatTB: Thêm nút in trực tiếp phiếu soạn thuốc
* 20221128 #019 TNHX: Thêm fuc xuất kết quả PDF
* 20230603 #020 DatTB: Upgrade Devexpress: Thêm biến cho phép gọi report bằng tên
*/
namespace ReportService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class AxReportService : DevExpress.XtraReports.Service.ReportService, IAxReportService
    {
        //▼====: #011
        private static readonly XRptOutPatientReceiptXML_V4 Instance = new XRptOutPatientReceiptXML_V4();
        public XRptOutPatientReceiptXML_V4 GetInstanceOutPatientReceipt()
        {
            return (XRptOutPatientReceiptXML_V4)XtraReport.FromStream(InvoiceReportSingleton.GetInstance().ReportStreamBienLai, true);
        }

        public XRptOutPatientPhieuChiDinhXML GetInstanceOutPatientPhieuChiDinh()
        {
            return (XRptOutPatientPhieuChiDinhXML)XtraReport.FromStream(InvoiceReportSingleton.GetInstance().ReportStreamPhieuChiDinh, true);
        }

        public class InvoiceReportSingleton
        {
            private InvoiceReportSingleton() { }

            private static InvoiceReportSingleton _instance;
            private static readonly object _lock = new object();
            public MemoryStream ReportStreamBienLai { get; set; }
            public MemoryStream ReportStreamPhieuChiDinh { get; set; }

            public static InvoiceReportSingleton GetInstance()
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new InvoiceReportSingleton();
                            _instance.ReportStreamBienLai = new MemoryStream();
                            _instance.ReportStreamPhieuChiDinh = new MemoryStream();

                            XRptOutPatientReceiptXML_V4 xRptOutPatientReceiptXML_V4 = new XRptOutPatientReceiptXML_V4();
                            xRptOutPatientReceiptXML_V4.SaveLayout(_instance.ReportStreamBienLai);

                            XRptOutPatientPhieuChiDinhXML xXRptOutPatientPhieuChiDinhXML = new XRptOutPatientPhieuChiDinhXML();
                            xXRptOutPatientPhieuChiDinhXML.SaveLayout(_instance.ReportStreamPhieuChiDinh);
                        }
                    }
                }
                return _instance;
            }
        }
        //▲====: #011

        static AxReportService()
        {
            //▼==== #020
            ReportService.AxReportService.CanCreateReportFromTypeName = true;
            //▲==== #020
            //Load dll lan dau tien. (Khong can dung ham Assembly.LoadFile)
            var type = typeof(eHCMS.ReportLib.RptPatientRegistration.BHYT_NgoaiTru);
        }
        public AxReportService()
        {
            var currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        //▼====: #001
        public AxReportService(bool LoadDynamicLibs)
        {
            var mReport = new XRptEPrescriptionNewForPrintSilently_V2();
        }
        //▲====: #001

        public string WhoAmI()
        {
            return "I am Tuyen";
        }
      
#region IAxReportService - extra functions
        public Stream GetReportDataInPdfFormat_Test()
        {
            //XtraReport report = new ReportLibrary.Patients.AllPatientReport();
            //report.Parameters["paramName"].Value = "Tuyen ne";

            var report = new XRptPatientPayment();
            report.Parameters["param_PaymentID"].Value = (decimal)135;

            return ExportReportToPdf(report);
        }

        //public Stream GetOutPatientReceiptReportInPdfFormat(Patient patient, PatientPayment payment,
        //    List<PatientRegistrationDetail> regDetailsList,List<PatientPCLRequest> pclRequestList , string cashierName)
        public Stream GetOutPatientReceiptReportInPdfFormat(long paymentId)
        {
            try
            {
                //AxLogger.Instance.LogInfo("Start loading Out-Patient Receipt Report");
                var report = new XRptOutPatientReceiptAuto();
                report.Parameters["param_PaymentID"].Value = (int)paymentId;
                report.PaperKind = PaperKind.A5Rotated;
                //AxLogger.Instance.LogInfo("End of loading Out-Patient Receipt Report. Status: Success.");

                //AxLogger.Instance.LogInfo("Start exporting Out-Patient Receipt Report to PDF format");
                var ms = ExportReportToPdf(report);
                //AxLogger.Instance.LogInfo("End of exporting Out-Patient Receipt Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetOutPatientReceiptReportXMLInPdfFormat(string paymentIdxml)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient Receipt Report");
                var report = new XRptOutPatientReceiptXML_Auto();
                report.Parameters["param_PaymentID"].Value = paymentIdxml;
                report.PaperKind = PaperKind.A5Rotated;
                AxLogger.Instance.LogInfo("End of loading Out-Patient Receipt Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient Receipt Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient Receipt Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetOutPatientReceiptReportXMLInPdfFormat_V2(string paymentIdxml)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient Receipt Report");
                var report = new XRptOutPatientReceiptXML_Auto_V2();
                report.Parameters["param_PaymentID"].Value = paymentIdxml;
                AxLogger.Instance.LogInfo("End of loading Out-Patient Receipt Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient Receipt Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient Receipt Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void GetOutPatientReceiptReportXMLInPdfFormat_V3(string patientPaymentIDXML, string hiPaymentIDXML, out byte[] ReceiptForPatient, out byte[] ReceiptForHI)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of processing Out-Patient Receipt Report.");

                ReceiptForPatient = null;
                ReceiptForHI = null;

                if (patientPaymentIDXML != null)
                {
                    var reportPatient = new XRptOutPatientReceiptXML_Auto_V3_Patient();
                    reportPatient.Parameters["param_PaymentID"].Value = patientPaymentIDXML;
                    ReceiptForPatient = ExportReportToPdf(reportPatient).ToArray();

                }
                if (hiPaymentIDXML != null)
                {
                    var reportHI = new XRptOutPatientReceiptXML_Auto_V3_HI();
                    reportHI.Parameters["param_PaymentID"].Value = hiPaymentIDXML;
                    ReceiptForHI = ExportReportToPdf(reportHI).ToArray();
                }
                AxLogger.Instance.LogInfo("Start of processing Out-Patient Receipt Report. Status: Success.");
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        /*▼====: #005*/
        public Stream GetOutPatientReceiptReportXMLInPdfFormat_V4_Thermal(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth, bool ApplyNewMethod = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient Receipt Thermal Report");
                var report = new XRptOutPatientReceiptXML_V4_InNhiet();
                report.Parameters["param_PaymentID"].Value = paymentIdxml;
                report.Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                AxLogger.Instance.LogInfo("End of loading Out-Patient Receipt Thermal Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient Receipt Thermal Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient Receipt Thermal Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Thermal Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        /*▲====: #005*/

        public Stream GetOutPatientReceiptReportXMLInPdfFormat_V4(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth
            , string parLogoUrl, string parDeptLocIDQMS, bool ApplyNewMethod = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient Receipt Report");
                var report = new XtraReport();
                if (ApplyNewMethod)
                {
                    report = GetInstanceOutPatientReceipt();
                }
                else
                {
                    report = new XRptOutPatientReceiptXML_V4();
                }
                report.Parameters["param_PaymentID"].Value = paymentIdxml;
                /*▼====: #002*/
                report.Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;
                /*▲====: #004*/
                /*▼====: #015*/
                report.Parameters["parDeptLocIDQMS"].Value = parDeptLocIDQMS;
                /*▲====: #015*/

                AxLogger.Instance.LogInfo("End of loading Out-Patient Receipt Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient Receipt Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient Receipt Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetOutPatientReceiptTestPrintInPdfFormat()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient Receipt Test Print");
                var report = new XRptOutPatientReceiptTestPrint();
                report.PaperKind = PaperKind.A5Rotated;
                AxLogger.Instance.LogInfo("End of loading Out-Patient Receipt Test Print. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient Receipt Test Print to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient Receipt Test Print to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Test Print. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_TESTPRINT_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public Stream GetConfirmHIReportInPdfFormat(long ptRegistrationID)
        {
            try
            {
                var report = new BHYT_NgoaiTru();
                report.Parameters["param_RegistrationID"].Value = ptRegistrationID;
                //report.ReportUnit = ReportUnit.TenthsOfAMillimeter;
                //report.PageHeight = 210;
                //report.PageWidth = 148;
                report.PaperKind = PaperKind.A4;
                var ms = ExportReportToPdf(report);
                return ms;
            }
            catch(Exception ex)
            {
                //AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_CONFIRM_HI_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetConfirmHITestPrintInPdfFormat()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading BHYT_NgoaiTru Test Print");
                var report = new BHYT_NgoaiTruTestPrint();
                report.PaperKind = PaperKind.A4;
                AxLogger.Instance.LogInfo("End of loading BHYT_NgoaiTru Test Print. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting BHYT_NgoaiTru Test Print to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting BHYT_NgoaiTru Test Print to PDF format. Status: Success.");

                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing BHYT_NgoaiTru Test Print. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_CONFIRM_HI_TESTPRINT_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetOutPatientReceiptReport_RegistrationDetails_InPdfFormat(long paymentId, List<long> regDetailIDs)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient Receipt Report");
                var report = new XRptOutPatientReceipt();
                report.Parameters["param_PaymentID"].Value = (int)paymentId;
                report.PaperKind = PaperKind.A5Rotated;

                report.Parameters["param_ReceiptForEachLocationPrintingMode"].Value = 1;
                if (regDetailIDs != null && regDetailIDs.Count > 0)
                {
                    report.Parameters["param_ServiceItemIDString"].Value = string.Join(",", regDetailIDs);
                }
                else
                {
                    report.Parameters["param_ServiceItemIDString"].Value = null;
                }

                AxLogger.Instance.LogInfo("End of loading Out-Patient Receipt Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient Receipt Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient Receipt Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_ADDICTIVE_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetOutPatientReceiptReport_PclRequests_InPdfFormat(long paymentId, List<long> pclRequestIDs)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient Receipt Report");
                var report = new XRptOutPatientReceipt();
                report.Parameters["param_PaymentID"].Value = (int)paymentId;
                report.PaperKind = PaperKind.A5Rotated;

                report.Parameters["param_ReceiptForEachLocationPrintingMode"].Value = 1;
                if (pclRequestIDs != null && pclRequestIDs.Count > 0)
                {
                    report.Parameters["param_PclItemIDString"].Value = string.Join(",", pclRequestIDs);
                }
                else
                {
                    report.Parameters["param_PclItemIDString"].Value = null;
                }

                AxLogger.Instance.LogInfo("End of loading Out-Patient Receipt Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient Receipt Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient Receipt Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_ADDICTIVE_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetPaymentReportInPdfFormat(decimal paymentID, int FindPatient, long CashAdvanceID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptPatientPayment();
                report.Parameters["param_PaymentID"].Value = paymentID;
                report.Parameters["FindPatient"].Value = FindPatient;
                report.Parameters["param_CashAdvanceID"].Value = CashAdvanceID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");

                //AxException axErr = new AxException(ex, "PR.0_0000002");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_ADDICTIVE_CANNOT_LOAD);
                //AxLogger.Instance.LogDebug(axErr);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetPatientCashAdvanceReportInPdfFormat(decimal paymentID,int FindPatient)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptPatientCashAdvance();
                report.Parameters["param_PaymentID"].Value = paymentID;
                report.Parameters["FindPatient"].Value = FindPatient;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");

                //AxException axErr = new AxException(ex, "PR.0_0000002");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_ADDICTIVE_CANNOT_LOAD);
                //AxLogger.Instance.LogDebug(axErr);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetRegisteredServiceReportInPdfFormat(string patientName, string address, string gender, string deptLocation, string registrationCode, string service, string sequenceNumber)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                var report = new XRptRegisteredService();
                report.Parameters["param_Address"].Value = address;
                report.Parameters["param_PatientName"].Value = patientName;
                report.Parameters["param_Gender"].Value = gender;
                report.Parameters["param_DeptLocation"].Value = deptLocation;
                report.Parameters["param_RegistrationCode"].Value = registrationCode;
                report.Parameters["param_Service"].Value = service;
                report.Parameters["param_SequenceNumber"].Value = sequenceNumber;

                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");

                //AxException axErr = new AxException(ex, "PR.0_0000002");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_ADDICTIVE_CANNOT_LOAD);
                //AxLogger.Instance.LogDebug(axErr);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetPclItemsReportInPdfFormat(string patientName, string address, string gender, string deptLocation, string diagnosis, long pclRequestId, List<long> requestDetailsIdList)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptPclItems();
                report.Parameters["param_Address"].Value = address;
                report.Parameters["param_PatientName"].Value = patientName;
                report.Parameters["param_Gender"].Value = gender;
                report.Parameters["param_DeptLocation"].Value = deptLocation;
                report.Parameters["param_Diagnosis"].Value = diagnosis;

                var sb = new StringBuilder();
                sb.Append("<IDList>");
                if (requestDetailsIdList != null && requestDetailsIdList.Count > 0)
                {
                    foreach (var id in requestDetailsIdList)
                    {
                        sb.AppendFormat("<RecInfo><PCLReqItemID>{0}</PCLReqItemID></RecInfo>", id);
                    }
                }
                sb.Append("</IDList>");
                report.Parameters["param_PCLReqItemIDList"].Value = sb.ToString();
                report.Parameters["param_PCLRequestID"].Value = pclRequestId.ToString();

                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");

                //AxException axErr = new AxException(ex, "PR.0_0000002");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_ADDICTIVE_CANNOT_LOAD);
                //AxLogger.Instance.LogDebug(axErr);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region Pharmacies
        //▼====: #005
        public Stream GetCollectionDrugInPdfFormat(long outID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new PhieuNhanThuoc();
                report.Parameters["OutiID"].Value = outID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr =AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_SELLVISITOR_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetCollectionDrugForThermalInPdfFormat(long outID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new PhieuNhanThuocInNhiet();
                report.Parameters["OutiID"].Value = outID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_SELLVISITOR_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetCollectionDrugBHYTForThermalInPdfFormat(long outID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new PhieuNhanThuocBHYTInNhiet();
                report.Parameters["OutiID"].Value = outID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_SELLVISITOR_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetCollectionDrugForThermalSummaryInPdfFormat(string outID, string parHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new PhieuNhanThuocSummary_XML_InNhiet();
                report.Parameters["OutiID"].Value = outID;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_SELLVISITOR_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #005

        public Stream GetOutwardInternalInPdfFormat(long outID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_XuatNoiBo();
                report.Parameters["OutiID"].Value = outID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_SELLVISITOR_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetPharmacyDemageDrugInPdfFormat(long outID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_PharmacyDemageDrug();
                report.Parameters["OutiID"].Value = outID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_SELLVISITOR_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetReturnDrugInPdfFormat(long outiID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XptReturnDrug();
                report.Parameters["OutiID"].Value = outiID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_SELLVISITOR_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetCardStorageInPdfFormat(long DrugID, string DrugName, long StoreID, string StorageName, string UnitName, DateTime Fromdate, DateTime ToDate, string dateshow)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptCardStorage();
                report.Parameters["DrugID"].Value = DrugID;
                report.Parameters["DrugName"].Value = DrugName;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["UnitName"].Value = UnitName;
                report.Parameters["FromDate"].Value = Fromdate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["Show"].Value = dateshow;

                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_CARDSTORAGE_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetGroupStorageInPdfFormat(long DrugID, string DrugName, string UnitName, DateTime Fromdate, DateTime ToDate,string dateshow)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptGroupStorages();
                report.Parameters["DrugID"].Value = DrugID;
                report.Parameters["DrugName"].Value = DrugName;
                report.Parameters["UnitName"].Value = UnitName;
                report.Parameters["FromDate"].Value = Fromdate;
                report.Parameters["ToDate"].Value = ToDate;
                 report.Parameters["DateShow"].Value = dateshow;

                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_GROUPSTORAGE_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetInOutStocksInPdfFormat(DateTime FromDate, DateTime ToDate, string StorageName, long StoreID,string dateshow)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptInOutStocks();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = dateshow;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INOUTSTOCKS_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetInWardDrugSupplierInPdfFormat(long InvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptInwardDrugSupplier();
                report.Parameters["InvID"].Value = InvID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetInWardVTYTTHInPdfFormat(long InvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptInwardVTYTTHSupplier();
                report.Parameters["InvID"].Value = InvID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public Stream GetInWardVaccineInPdfFormat(long InvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptInwardTiemNguaSupplier();
                report.Parameters["InvID"].Value = InvID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public Stream GetInWardChemicalInPdfFormat(long InvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptInwardChemicalSupplier();
                report.Parameters["InvID"].Value = InvID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetInWardBloodInPdfFormat(long InvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptInwardBloodSupplier();
                report.Parameters["InvID"].Value = InvID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetInWardThanhTrungInPdfFormat(long InvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptInwardThanhTrungSupplier();
                report.Parameters["InvID"].Value = InvID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetInWardVPPInPdfFormat(long InvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptInwardVPPSupplier();
                report.Parameters["InvID"].Value = InvID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetInWardVTTHInPdfFormat(long InvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptInwardVTTHSupplier();
                report.Parameters["InvID"].Value = InvID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public Stream GetRequestPharmacyInPdfFormat(long RequestID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XptRequestDrugPharmacy();
                report.Parameters["RequestID"].Value = RequestID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetRequestInPdfFormat(long RequestID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptRequestDrugDept();
                report.Parameters["RequestID"].Value = RequestID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetDrugExpiryInPdfFormat(long? StoreID, int Type, string Message, string showdate)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptRequestDrugDept();
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["Type"].Value = Type;
                report.Parameters["ShowMessage"].Value = Message;
                report.Parameters["DateReport"].Value = showdate;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetEstimationPharmacyInPdfFormat(long PharmacyEstimatePoID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptEstimatePharmacy();
                report.Parameters["PharmacyEstimatePoID"].Value = PharmacyEstimatePoID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetBaoCaoNopTienHangNgayInPdfFormat(DateTime Date)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_BaoCaoNopTienHangNgay();
                report.Parameters["Date"].Value = Date;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetBaoCaoPhatThuocHangNgayInPdfFormat(DateTime Date)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_BangKeChiTietPhatThuoc();
                report.Parameters["Date"].Value = Date;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public Stream GetPhieuKiemKeInPdfFormat(long ID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_PhieuKiemKe();
                report.Parameters["ID"].Value = ID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetSupplierTemplateInPdfFormat()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_SupplierTemplate();
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_INWARDSUPPLIER_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream BaocaoTraThuocTongHopPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_ReturnDrugGeneral();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public Stream BaocaoBanThuocTongHopPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_SellDrugGeneral();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream BaocaoXuatThuocChoBHPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_XuatBH();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                report.Parameters["StoreName"].Value = StoreName;
                report.Parameters["StoreID"].Value = StoreID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream BaocaoNhapThuocHangThangPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new BaoCaoNhapThuocHangThang();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["Show"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                report.Parameters["StoreName"].Value = StoreName;
                report.Parameters["StoreID"].Value = StoreID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public Stream BaocaoNhapThuocHangThangInvoicePdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new BaoCaoNhapThuocHangThangInvoice();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["Show"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                report.Parameters["StoreName"].Value = StoreName;
                report.Parameters["StoreID"].Value = StoreID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream BaocaoXuatNoiBoTheoNguoiMuaPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID, int OutTo, long TypID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_XuatNoiBoTheoNguoiMua();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                report.Parameters["StoreName"].Value = StoreName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["OutTo"].Value = OutTo;
                report.Parameters["TypID"].Value = TypID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public Stream BaocaoXuatNoiBoTheoTenThuocPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID, int OutTo, long TypID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_XuatNoiBoTheoTenThuoc();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                report.Parameters["StoreName"].Value = StoreName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["OutTo"].Value = OutTo;
                report.Parameters["TypID"].Value = TypID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public Stream BangKeChungTuThanhToanPdfFormat(long? ID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new SupplierPharmacyPaymentReqs();
                report.Parameters["ID"].Value = ID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream PhieuDeNghiThanhToanPdfFormat(long? ID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new PhieuDeNghiThanhToan();
                report.Parameters["ID"].Value = ID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetDuTruDuaVaoHeSoAnToanPdfFormat()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_DuTruDuaTrenHeSoAnToan();
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Transactions

        public Stream GetPaymentVisistorReportInPdfFormat(long paymentID,long OutiID,string LyDo)
        {
            try
            {
                //AxLogger.Instance.LogInfo("Start loading report");
                //var report = new XRptPatientPaymentVisitor();
                //report.Parameters["param_PaymentID"].Value = paymentID;
                //report.Parameters["OutiID"].Value = OutiID;
                //report.Parameters["parameter_LyDo"].Value = LyDo;
                //AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                //AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                //var ms = ExportReportToPdf(report);
                //AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return null;// ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");

                //AxException axErr = new AxException(ex, "PR.0_0000002");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_ADDICTIVE_CANNOT_LOAD);
                //AxLogger.Instance.LogDebug(axErr);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetTemplate25aInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                var report = new XRpt_Temp25a();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetTemplate25aTHInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                var report = new XRpt_Temp25aTH();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetTemplate26aInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_Temp26aCT();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetTemplate26aTHInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_Temp26aTH();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetTemplate20NgoaiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_Temp20NgoaiTru();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetTemplate20NoiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_Temp20NoiTru();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetTemplate21NgoaiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_Temp21NgoaiiTru();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetTemplate21NoiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_Temp21NoiTru();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = ShowDate;
                report.Parameters["Quarter"].Value = Quarter;
                report.Parameters["Month"].Value = Month;
                report.Parameters["Year"].Value = Year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP25A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetTemplate38aInPdfFormat(long transactionID, long ptRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRpt_Temp38NgoaiTru();
                report.Parameters["TransactionID"].Value = transactionID;
                report.Parameters["PtRegistrationID"].Value = 0;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP38A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream ThongKeDoanhThu(DateTime fromDate, DateTime toDate, string showDate,int quarter, int month, int year, int flag)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new ThongKeDoanhThu();
                report.Parameters["FromDate"].Value = fromDate;
                report.Parameters["ToDate"].Value = toDate;
                report.Parameters["DateShow"].Value = showDate;
                report.Parameters["Quarter"].Value = quarter;
                report.Parameters["Month"].Value = month;
                report.Parameters["Year"].Value = year;
                report.Parameters["Flag"].Value = flag;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_TEMP38A_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Consultation Members

        //▼====: #010
        public Stream GetXRptEPrescriptionInPdfFormat(long parIssueID, bool IsPsychotropicDrugs, bool IsFuncfoodsOrCosmetics
            , int parTypeOfForm, int parOrganizationUseSoftware, int nNumCopies, string parHospitalCode
            , int PrescriptionOutPtVersion, string PrescriptionMainRightHeader, string PrescriptionSubRightHeader
            , string DepartmentOfHealth, string HospitalName, string HospitalAddress, string KBYTLink, bool HasPharmacyDrug, bool IsSeparatePrescription, string ReportHospitalPhone
            )
        {
            try
            {
                //AxLogger.Instance.LogInfo("Start loading report");

                if (nNumCopies < 1)
                {
                    nNumCopies = 1;
                }
                else if (nNumCopies > 5)
                {
                    nNumCopies = 5;
                }

                //XtraReport report;                
                XtraReport[] repObjs = new XtraReport[nNumCopies];

                for (int nCnt = 0; nCnt < nNumCopies; ++nCnt)
                {
                    if (parOrganizationUseSoftware == 0)
                    {
                        if (Convert.ToInt64(parHospitalCode) == (long)AllLookupValues.HospitalCode.VIEN_TIM)
                        {
                            if (parTypeOfForm == 1)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionNewForPrintSilently_V2();
                            }
                            else if (parTypeOfForm == 0)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionNewPrivateForPrintSilently();
                            }
                        }
                        else
                        {
                            if (parTypeOfForm == 1)
                            {
                                if (PrescriptionOutPtVersion == 6)
                                {
                                    //if(IsSeparatePrescription)
                                    //{
                                    //    repObjs[nCnt] = new XRptEPrescription_V6_SubReport_TV3();
                                    //    repObjs[nCnt].Parameters["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                    //}
                                    //else
                                    //{
                                        repObjs[nCnt] = new XRptEPrescription_V6_SubReport();
                                    //}
                                    repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    //Duynh: 22-2-2021
                                    repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                    repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                    repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;
                                    repObjs[nCnt].Parameters["parHospitalPhone"].Value = ReportHospitalPhone;
                                }
                                else if (PrescriptionOutPtVersion == 5)
                                {
                                    if(IsSeparatePrescription)
                                    {
                                        repObjs[nCnt] = new XRptEPrescription_V5_SubReport_TV3();
                                        repObjs[nCnt].Parameters["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                    }
                                    else
                                    {
                                        repObjs[nCnt] = new XRptEPrescription_V5_SubReport();
                                    }
                                    repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    //Duynh: 22-2-2021
                                    repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                    repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                    repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;

                                }
                                else if (PrescriptionOutPtVersion == 4)
                                {
                                    repObjs[nCnt] = new XRptEPrescription_V2_SubReport_TV4();
                                    repObjs[nCnt].Parameters["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                    repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    //Duynh: 22-2-2021
                                    repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                    repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                    repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;

                                }
                                else if (PrescriptionOutPtVersion == 3)
                                {
                                    repObjs[nCnt] = new XRptEPrescription_V2_SubReport_TV3();
                                    repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    //Duynh: 22-2-2021
                                    repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                    repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                    repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;

                                }
                                else if (PrescriptionOutPtVersion == 2)
                                {
                                    repObjs[nCnt] = new XRptEPrescription_V2_SubReport_TV();
                                }
                                else
                                {
                                    repObjs[nCnt] = new XRptEPrescription_V2_SubReport();
                                }
                            }
                            else if (parTypeOfForm == 0)
                            {
                                if (PrescriptionOutPtVersion == 6)
                                {
                                    //if (IsSeparatePrescription)
                                    //{
                                    //    repObjs[nCnt] = new XRptEPrescription_V6_SubReport_TV3();
                                    //    repObjs[nCnt].Parameters["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                    //}
                                    //else
                                    //{
                                        repObjs[nCnt] = new XRptEPrescription_V6_SubReport();
                                    //}
                                    repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    //Duynh: 22-2-2021
                                    repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                    repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                    repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;
                                    repObjs[nCnt].Parameters["parHospitalPhone"].Value = ReportHospitalPhone;
                                }
                                else if (PrescriptionOutPtVersion == 5)
                                {
                                    if (IsSeparatePrescription)
                                    {
                                        repObjs[nCnt] = new XRptEPrescription_V5_SubReport_TV3();
                                        repObjs[nCnt].Parameters["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                    }
                                    else
                                    {
                                        repObjs[nCnt] = new XRptEPrescription_V5_SubReport();
                                    }
                                    repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    //Duynh: 22-2-2021
                                    repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                    repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                    repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;

                                }
                                else if (PrescriptionOutPtVersion == 4)
                                {
                                    repObjs[nCnt] = new XRptEPrescription_V2_SubReport_TV4();
                                    repObjs[nCnt].Parameters["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                    repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    //Duynh: 22-2-2021
                                    repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                    repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                    repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;

                                }
                                else if(PrescriptionOutPtVersion == 3)
                                {
                                    repObjs[nCnt] = new XRptEPrescription_V2_SubReport_TV3();
                                    repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    //Duynh: 22-2-2021
                                    repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                    repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                    repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;
                                }
                                else if (PrescriptionOutPtVersion == 2)
                                {
                                    repObjs[nCnt] = new XRptEPrescription_V2_SubReport_TV();
                                }
                                else
                                {
                                    repObjs[nCnt] = new XRptEPrescription_V2_SubReport();
                                }
                            }
                            repObjs[nCnt].Parameters["parIsPsychotropicDrugs"].Value = IsPsychotropicDrugs;
                            repObjs[nCnt].Parameters["parIsFuncfoodsOrCosmetics"].Value = IsFuncfoodsOrCosmetics;
                            if (PrescriptionOutPtVersion != 1)
                            {
                                repObjs[nCnt].Parameters["parHospitalCode"].Value = parHospitalCode;
                            }
                        }
                    }
                    else
                    {
                        repObjs[nCnt] = new XRptEPrescriptionNewPrivateForPrintSilently();
                    }
                    repObjs[nCnt].Parameters["parIssueID"].Value = parIssueID;
                    repObjs[nCnt].Parameters["parHospitalCode"].Value = parHospitalCode;
                    repObjs[nCnt].Parameters["parKBYTLink"].Value = KBYTLink;
                    repObjs[nCnt].CreateDocument();
                }

                if (nNumCopies > 1)
                {
                    repObjs[0].PrintingSystem.ContinuousPageNumbering = false;
                }

                for (int nIdx = 1; nIdx < nNumCopies; ++nIdx)
                {
                    repObjs[0].Pages.AddRange(repObjs[nIdx].Pages);
                }

                //AxLogger.Instance.LogInfo("End loading report");

                //AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(repObjs[0]);
                //AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_PRESCRIPTION_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public Stream GetXRptEPrescriptionInPdfFormat_InPt(long parIssueID, int parTypeOfForm, int nNumCopies, string parHospitalCode
            , int PrescriptionInPtVersion, string PrescriptionMainRightHeader, string PrescriptionSubRightHeader
            , string DepartmentOfHealth, string HospitalName, string HospitalAddress, string KBYTLink, string ReportHospitalPhone
            )
        {
            try
            {
                if (nNumCopies < 1)
                {
                    nNumCopies = 1;
                }
                else if (nNumCopies > 5)
                {
                    nNumCopies = 5;
                }
           
                XtraReport[] repObjs = new XtraReport[nNumCopies];

                for (int nCnt = 0; nCnt < nNumCopies; ++nCnt)
                {
                    if (Convert.ToInt64(parHospitalCode) == (long)AllLookupValues.HospitalCode.VIEN_TIM)
                    {
                        repObjs[nCnt] = new XRptEPrescriptionNewForPrintSilently_InPt_V2();
                    }
                    else
                    {
                        if (parTypeOfForm == 1)
                        {

                            if (PrescriptionInPtVersion == 5)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V5_SubReport();
                                repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //Duynh: 22-2-2021
                                repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;
                                repObjs[nCnt].Parameters["parHospitalPhone"].Value = ReportHospitalPhone;
                            }
                            else if (PrescriptionInPtVersion == 4)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V4_SubReport();
                                repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //Duynh: 22-2-2021
                                repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;

                            }
                            else if (PrescriptionInPtVersion == 3)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V2_SubReport_TV3();
                                repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //Duynh: 22-2-2021
                                repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;

                            }
                            else if (PrescriptionInPtVersion == 2)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V2_SubReport_TV();
                            }
                            else
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V2_SubReport();
                            }
                        }
                        else if (parTypeOfForm == 0)
                        {
                            if (PrescriptionInPtVersion == 5)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V5_SubReport();
                                repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //Duynh: 22-2-2021
                                repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;
                                repObjs[nCnt].Parameters["parHospitalPhone"].Value = ReportHospitalPhone;
                            }
                            else  if (PrescriptionInPtVersion == 4)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V4_SubReport();
                                repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //Duynh: 22-2-2021
                                repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;

                            }
                            else if(PrescriptionInPtVersion == 3)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V2_SubReport_TV3();
                                repObjs[nCnt].Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                repObjs[nCnt].Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //Duynh: 22-2-2021
                                repObjs[nCnt].Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                                repObjs[nCnt].Parameters["parHospitalName"].Value = HospitalName;
                                repObjs[nCnt].Parameters["parHospitalAddress"].Value = HospitalAddress;
                            }
                            else if (PrescriptionInPtVersion == 2)
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V2_SubReport_TV();
                            }
                            else
                            {
                                repObjs[nCnt] = new XRptEPrescriptionInpt_V2_SubReport();
                            }
                        }
                        if(PrescriptionInPtVersion != 1)
                        {
                            repObjs[nCnt].Parameters["parHospitalCode"].Value = parHospitalCode;
                        }
                    }
                    repObjs[nCnt].Parameters["parIssueID"].Value = parIssueID;
                    repObjs[nCnt].Parameters["parKBYTLink"].Value = KBYTLink;
                    repObjs[nCnt].CreateDocument();
                }

                if (nNumCopies > 1)
                {
                    repObjs[0].PrintingSystem.ContinuousPageNumbering = false;
                }

                for (int nIdx = 1; nIdx < nNumCopies; ++nIdx)
                {
                    repObjs[0].Pages.AddRange(repObjs[nIdx].Pages);
                }
                MemoryStream ms = ExportReportToPdf(repObjs[0]);
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_PRESCRIPTION_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #010

        public Stream GetAllXRptAppointment(PatientAppointment parPatientAppointment)
        {
            try
            {
                XtraReport tempReport = new XtraReport();
                List<XtraReport> reportList = new List<XtraReport>();

                switch (parPatientAppointment.ServiceDetailPrintType)
                {
                    case AllLookupValues.AppServiceDetailPrintType.NormalApp:
                        {
                            tempReport = new XRptPatientApptServiceDetails();
                            tempReport.Parameters["parAppointmentID"].Value = (int)parPatientAppointment.AppointmentID;
                            tempReport.CreateDocument();
                            reportList.Add(tempReport);
                            break;
                        }
                    case AllLookupValues.AppServiceDetailPrintType.HIApp:
                        {
                            tempReport = new GiayHenTaiKhamBHYT();
                            tempReport.Parameters["param_RegistrationID"].Value = (int)parPatientAppointment.PtRegistrationID;
                            tempReport.Parameters["param_AppointmentID"].Value = (int)parPatientAppointment.AppointmentID;
                            tempReport.CreateDocument();
                            reportList.Add(tempReport);
                            break;
                        }
                    case AllLookupValues.AppServiceDetailPrintType.HIApp_InPt:
                        {
                            tempReport = new GiayHenTaiKhamBHYT_InPt();
                            tempReport.Parameters["param_RegistrationID"].Value = (int)parPatientAppointment.PtRegistrationID;
                            tempReport.Parameters["param_AppointmentID"].Value = (int)parPatientAppointment.AppointmentID;
                            tempReport.CreateDocument();
                            reportList.Add(tempReport);
                            break;
                        }
                    case AllLookupValues.AppServiceDetailPrintType.HIApp_New:
                        {
                            tempReport = new XRptHIAppointment();
                            tempReport.Parameters["param_AppointmentID"].Value = (int)parPatientAppointment.AppointmentID;
                            tempReport.CreateDocument();
                            reportList.Add(tempReport);
                            break;
                        }
                }

                if (parPatientAppointment.IsPrintLaboratoryPCLApp)
                {
                    tempReport = new XRptPatientApptPCLRequestsCombo();
                    tempReport.Parameters["parAppointmentID"].Value = (int)parPatientAppointment.AppointmentID;
                    tempReport.Parameters["parPtPCLReqID_List"].Value = (string)parPatientAppointment.LaboratoryPCLRequestIDListXml;
                    tempReport.CreateDocument();
                    reportList.Add(tempReport);
                }
                if (parPatientAppointment.IsPrintImagingPCLApp)
                {
                    tempReport = new XRptPatientApptPCLRequestsCombo();
                    tempReport.Parameters["parAppointmentID"].Value = (int)parPatientAppointment.AppointmentID;
                    tempReport.Parameters["parPtPCLReqID_List"].Value = (string)parPatientAppointment.ImagingPCLRequestIDListXml;
                    tempReport.CreateDocument();
                    reportList.Add(tempReport);
                }

                if (reportList == null || reportList.Count <= 0)
                {
                    return null;
                }

                XtraReport[] reportArray = reportList.ToArray();

                for (int nIdx = 1; nIdx < reportArray.Count(); ++nIdx)
                {
                    reportArray[0].Pages.AddRange(reportArray[nIdx].Pages);
                }

                if (reportArray.Count() > 1)
                {
                    reportArray[0].PrintingSystem.ContinuousPageNumbering = false;
                }

                MemoryStream ms = ExportReportToPdf(reportArray[0]);
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing GetAllXRptAppointment. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_APPOINTMENT_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }





        public Stream GetXRptPCLFormInPdfFormat(string parDoctorName, string parDoctorPhone, long parPatientID, long parPCLFormID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptPCLForm();
                report.Parameters["parDoctorName"].Value = parDoctorName;
                report.Parameters["parDoctorPhone"].Value = parDoctorPhone;
                report.Parameters["parPatientID"].Value = parPatientID;
                report.Parameters["parPCLFormID"].Value = parPCLFormID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_PCLF0RM_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetXRptPCLFormRequestInPdfFormat(long parPCLFormID, long parPtPCLAppID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptPCLFormRequest();
                report.Parameters["parPCLFormID"].Value = parPCLFormID;
                report.Parameters["parPtPCLAppID"].Value = parPtPCLAppID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_PCLFORMREQUEST_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetXRptPCLLabResultInPdfFormat(bool parAllHistories, long parPtPCLLabReqID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptPCLLabResult();
                report.Parameters["parAllHistories"].Value = parAllHistories;
                report.Parameters["parPtPCLLabReqID"].Value = parPtPCLLabReqID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_PCLLABRESULT_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Stream GetXRptPMRInPdfFormat(long parPatientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report");
                XtraReport report = new XRptPMR();
                report.Parameters["parPatientID"].Value = parPatientID;
                AxLogger.Instance.LogInfo("End of loading report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting report to PDF format");
                MemoryStream ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing report. Status: Failed.");
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_PMR_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion
        #endregion
        private MemoryStream ExportReportToPdf(XtraReport report)
        {
            var ms = new MemoryStream();
            //report.ExportToPdf("D:\\temp\\tuyenreport.pdf");
            report.ExportToPdf(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }


        #region RptNhapXuatDenKhoaPhong

        //public String RptNhapXuatDenKhoaPhong(long vMedProductType, DateTime fromDate, DateTime toDate, long storeID, long? StoreClinicID, bool? IsShowHave, bool? IsShowHaveMedProduct)
        //{
        //    var sb=new StringBuilder();
        //    sb.Append("<data>");
        //    sb.Append("<columns>");
        //        //sb.Append("<column name=\"Name\"></column>");
        //        //sb.Append("<column name=\"Age\"></column>");
        //        sb.Append("[ValueCols]");
        //    sb.Append("</columns>");
            
        //    sb.Append("<rows>");
        //        //sb.Append("<row>");
        //            //sb.Append("<cell>Bob</cell>");
        //            //sb.Append("<cell>30</cell>");
        //            sb.Append("[ValueCells]");    
        //        //sb.Append("</row>");
        //    sb.Append("</rows>");

        //    sb.Append("</data>");



        //    //Các cột động xuất đến khoa
        //    var cacCotKhoaPhong = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoices_ListOutToKhoKhoaPhong(vMedProductType, fromDate, toDate, storeID, StoreClinicID, IsShowHave);
        //    var cacCotXuatDenCacKhoaPhong = cacCotKhoaPhong.Rows.Count;

        //    var AllStorageWarehouseLocation = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoices_ListOutToKhoKhoaPhong(vMedProductType, fromDate, toDate, storeID, StoreClinicID, IsShowHave);

        //    List<long> outputToIDList = new List<long>();
        //    foreach (DataRow row in cacCotKhoaPhong.Rows)
        //    {
        //        outputToIDList.Add(Convert.ToInt64(row["StoreID"]));
        //    }

        //    //Lấy danh sách số lượng xuất đến các kho.
        //    var dtOutQtyToClinicDept = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDept_OutToClinicDept(vMedProductType, fromDate, toDate, storeID, outputToIDList);

        //    //DataTable Column
        //    var sbValueCols = new StringBuilder();
        //    var dtResult = new DataTable();

        //    //Add Columns động
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "STT", DataType = typeof(Int32),AllowDBNull = false,AutoIncrement  = true,AutoIncrementSeed = 1,AutoIncrementStep = 1});
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "CODE", DataType = typeof(string) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Tên Hàng", DataType = typeof(string) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Phân Loại", DataType = typeof(string) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Tồn Đầu " + fromDate.ToString("dd/MM/yyyy"), DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Nhập", DataType = typeof(double) });

        //    const int sumCacCotCoDinhBefore = 6;

        //    if (cacCotXuatDenCacKhoaPhong > 0)
        //    {
        //        foreach (DataRow dr in cacCotKhoaPhong.Rows)
        //        {
        //            dtResult.Columns.Add(new DataColumn { ColumnName = dr["swhlName"].ToString().Trim(), DataType = typeof(double) });
        //        }
        //    }

        //    dtResult.Columns.Add(new DataColumn { ColumnName = "I.C", DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "C.M.I", DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Bán, XV Kg BHYT", DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "XV BHYT", DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Hủy", DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Trả Hàng", DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Nhà Thuốc VT", DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Khác", DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Tồn Cuối " + toDate.ToString("dd/MM/yyyy"), DataType = typeof(double) });
        //    dtResult.Columns.Add(new DataColumn { ColumnName = "Giá + VAT trung bình kho mới VND", DataType = typeof(double) });

        //    //Đánh chỉ số các cột.
        //    var indexSTT = 0;
        //    var indexCode = 1;
        //    var indexBrandName = 2;
        //    var indexCategory = 3;
        //    var indexStockFirst = 4;
        //    var indexInward = 5;

        //    var indexIC = sumCacCotCoDinhBefore + cacCotXuatDenCacKhoaPhong;
        //    var indexCMI = sumCacCotCoDinhBefore + cacCotXuatDenCacKhoaPhong + 1;
        //    var indexOutNotHI = sumCacCotCoDinhBefore + cacCotXuatDenCacKhoaPhong + 2;
        //    var indexOutHI = sumCacCotCoDinhBefore + cacCotXuatDenCacKhoaPhong + 3;
        //    var indexOutDelete = sumCacCotCoDinhBefore + cacCotXuatDenCacKhoaPhong + 4;
        //    var indexOutReturn = sumCacCotCoDinhBefore + cacCotXuatDenCacKhoaPhong + 5;
        //    var indexOutPharmacy = sumCacCotCoDinhBefore + cacCotXuatDenCacKhoaPhong + 6;
        //    var indexOutOther = sumCacCotCoDinhBefore + cacCotXuatDenCacKhoaPhong + 7;
        //    var indexStockEnd = sumCacCotCoDinhBefore + cacCotXuatDenCacKhoaPhong + 8;


        //    //Lấy nhập xuất tồn.
        //    var dtListGenMedProductID = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoices_ListGenMedProductIDNhap_Ton(vMedProductType, fromDate, toDate, storeID, IsShowHaveMedProduct);

        //    foreach (DataRow dr in dtListGenMedProductID.Rows)
        //    {
        //        long genMedProductID = Convert.ToInt64(dr["GenMedProductID"]);

        //        var row = dtResult.NewRow();

        //        row[indexCode] = dr["Code"].ToString().Trim();
        //        row[indexBrandName] = dr["BrandName"].ToString().Trim().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">","&gt;");
        //        row[indexStockFirst] = Convert.ToDouble(dr["QtyAtFirst"]);//ton dau ky phai la first -khong phai final
        //        row[indexInward] = Convert.ToDouble(dr["InQty"]);

        //        var indexStartCotDiDongTrenRpt = 6;

        //        double sumIc = 0;

        //        if (cacCotXuatDenCacKhoaPhong > 0)
        //        {
        //            foreach (DataRow store in cacCotKhoaPhong.Rows)
        //            {
        //                //KMx: Với mỗi loại hàng, lấy số lượng xuất đến kho phòng;
        //                double qty = 0;
        //                DataRow OutQtyToClinic = dtOutQtyToClinicDept.Rows.Cast<DataRow>().Where(x => Convert.ToInt64(x["GenMedProductID"]) == genMedProductID && Convert.ToInt64(x["OutputToID"]) == Convert.ToInt64(store["StoreID"])).FirstOrDefault();
        //                if (OutQtyToClinic != null)
        //                 {
        //                     qty = Convert.ToDouble(OutQtyToClinic["Qty"]);
        //                 }
        //                sumIc = sumIc + qty;

        //                row[indexStartCotDiDongTrenRpt] = qty;
        //                indexStartCotDiDongTrenRpt++;
        //            }
        //        }

        //        row[indexIC] = sumIc;

        //        row[indexCMI] = Convert.ToDouble(dr["OutQtyToCI"]);
        //        row[indexOutNotHI] = Convert.ToDouble(dr["OutQtyNotHI"]);
        //        row[indexOutHI] = Convert.ToDouble(dr["OutQtyHI"]);
        //        row[indexOutDelete] = Convert.ToDouble(dr["OutQtyDelete"]);
        //        row[indexOutPharmacy] = Convert.ToDouble(dr["OutQtyToPharmacy"]);

        //        row[indexStockEnd] = Convert.ToDouble(dr["QtyFinal"]);

        //        double TotalOutQty = Convert.ToDouble(row[indexIC]) + Convert.ToDouble(row[indexCMI]) + Convert.ToDouble(row[indexOutNotHI]) + Convert.ToDouble(row[indexOutHI]) + Convert.ToDouble(row[indexOutDelete]) + Convert.ToDouble(row[indexOutPharmacy]);
                
        //        row[indexOutOther] = (Convert.ToDouble(row[indexStockFirst]) + Convert.ToDouble(row[indexInward]) - TotalOutQty) - Convert.ToDouble(row[indexStockEnd]);
        //        //var stockEnd = Convert.ToDouble(dr["QtyAtFirst"]) + Convert.ToDouble(dr["InQty"]) - sumIc - Convert.ToDouble(row[indexCMI]);
        //        //row[indexStockEnd] = stockEnd;

        //        dtResult.Rows.Add(row);
        //    }

        //    //=================================================
        //    //Tính toán phun về Client nhận chuỗi
        //    foreach (DataColumn dc in dtResult.Columns)
        //    {
        //        sbValueCols.Append("<column name=\"" + dc.ColumnName.Trim() + "\"></column>");
        //    }

        //    //DataTable Row
        //    var sbValueCells = new StringBuilder();

        //    for (int i = 0; i < dtResult.Rows.Count; i++)
        //    {
        //        sbValueCells.Append("<row>");
        //        for (int j = 0; j < dtResult.Columns.Count; j++)
        //        {
        //            sbValueCells.Append("<cell>" + dtResult.Rows[i][j] + "</cell>");
        //        }
        //        sbValueCells.Append("</row>");
        //    }

        //    sb = sb.Replace("[ValueCols]", sbValueCols.ToString());
        //    sb = sb.Replace("[ValueCells]", sbValueCells.ToString());

        //    return sb.ToString();
        //}


        public String RptNhapXuatDenKhoaPhong(long vMedProductType, DateTime fromDate, DateTime toDate, long storeID, long? StoreClinicID, bool? IsShowHave, bool? IsShowHaveMedProduct)
        {
            if (StoreClinicID == null || StoreClinicID <= 0)
            {
                return RptAllStorage(vMedProductType, fromDate, toDate, storeID, StoreClinicID, IsShowHave, IsShowHaveMedProduct);
            }
            else
            {
                return RptOneStorage(vMedProductType, fromDate, toDate, storeID, StoreClinicID, IsShowHave, IsShowHaveMedProduct);
            }
        }


        public String RptAllStorage(long vMedProductType, DateTime fromDate, DateTime toDate, long storeID, long? StoreClinicID, bool? IsShowHave, bool? IsShowHaveMedProduct)
        {
            var sb = new StringBuilder();
            sb.Append("<data>");
            sb.Append("<columns>");
            sb.Append("[ValueCols]");
            sb.Append("</columns>");

            sb.Append("<rows>");
            sb.Append("[ValueCells]");
            sb.Append("</rows>");

            sb.Append("</data>");



            //Các cột động xuất đến khoa
            var AllStorageWarehouseLocation = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoices_ListOutToKhoKhoaPhong(vMedProductType, fromDate, toDate, storeID, StoreClinicID, IsShowHave);

            List<DataRow> StorageClinic = new List<DataRow>();
            List<DataRow> StorageClinicOther = new List<DataRow>();

            List<long> outputToIDList = new List<long>();

            if (AllStorageWarehouseLocation != null)
            {
                //KMx: Kho nội trú.
                StorageClinic = AllStorageWarehouseLocation.Rows.Cast<DataRow>().Where(x => Convert.ToInt64(x["StoreTypeID"]) == (long)AllLookupValues.StoreType.STORAGE_CLINIC).ToList();
                //KMx: Kho nội trú khác.
                StorageClinicOther = AllStorageWarehouseLocation.Rows.Cast<DataRow>().Where(x => Convert.ToInt64(x["StoreTypeID"]) == (long)AllLookupValues.StoreType.STORAGE_CLINIC_OTHER).ToList();

                foreach (DataRow row in AllStorageWarehouseLocation.Rows)
                {
                    outputToIDList.Add(Convert.ToInt64(row["StoreID"]));
                }
            }

            int StorageClinicCount = StorageClinic != null ? StorageClinic.Count : 0;
 
            int StorageClinicOtherCount = StorageClinicOther != null ? StorageClinicOther.Count : 0;

            int StorageClinicOtherColumn = 0;

            if (StorageClinicOtherCount > 0)
            {
                StorageClinicOtherColumn = 1;
            }

            //Lấy danh sách số lượng xuất đến các kho.
            var dtOutQtyToClinicDept = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDept_OutToClinicDept(vMedProductType, fromDate, toDate, storeID, outputToIDList);

            //DataTable Column
            var sbValueCols = new StringBuilder();
            var dtResult = new DataTable();

            //Add Columns động
            dtResult.Columns.Add(new DataColumn { ColumnName = "STT", DataType = typeof(Int32), AllowDBNull = false, AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 });
            dtResult.Columns.Add(new DataColumn { ColumnName = "CODE", DataType = typeof(string) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Tên Hàng", DataType = typeof(string) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Phân Loại", DataType = typeof(string) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Tồn Đầu " + fromDate.ToString("dd/MM/yyyy"), DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Nhập", DataType = typeof(double) });

            const int sumCacCotCoDinhBefore = 6;

            if (StorageClinic != null && StorageClinicCount > 0)
            {
                foreach (DataRow dr in StorageClinic)
                {
                    dtResult.Columns.Add(new DataColumn { ColumnName = dr["swhlName"].ToString().Trim(), DataType = typeof(double) });
                }
            }

            if (StorageClinicOther != null && StorageClinicOther.Count > 0)
            {
                dtResult.Columns.Add(new DataColumn { ColumnName = "Autres", DataType = typeof(double) });
            }

            dtResult.Columns.Add(new DataColumn { ColumnName = "I.C", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "C.M.I", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Bán, XV Kg BHYT", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "XV BHYT", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Hủy", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Trả Hàng", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Nhà Thuốc VT", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Khác", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Trả", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "SLCB", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Tồn Cuối " + toDate.ToString("dd/MM/yyyy"), DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Giá + VAT trung bình kho mới VND", DataType = typeof(double) });

            //Đánh chỉ số các cột.
            //var indexSTT = 0;
            var indexCode = 1;
            var indexBrandName = 2;
            var indexCategory = 3;
            var indexStockFirst = 4;
            var indexInward = 5;

            var indexIC = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn;
            var indexCMI = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 1;
            var indexOutNotHI = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 2;
            var indexOutHI = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 3;
            var indexOutDelete = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 4;
            var indexOutReturnSupplier = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 5;
            var indexOutPharmacy = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 6;
            var indexOutOther = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 7;
            var indexOutReturn = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 8;
            var indexAdjustQty = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 9;
            var indexStockEnd = sumCacCotCoDinhBefore + StorageClinicCount + StorageClinicOtherColumn + 10;


            //Lấy nhập xuất tồn.
            var dtListGenMedProductID = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoices_ListGenMedProductIDNhap_Ton(vMedProductType, fromDate, toDate, storeID, IsShowHaveMedProduct);

            if (dtListGenMedProductID != null)
            {
                foreach (DataRow dr in dtListGenMedProductID.Rows)
                {
                    long genMedProductID = Convert.ToInt64(dr["GenMedProductID"]);

                    var row = dtResult.NewRow();

                    row[indexCode] = dr["Code"].ToString().Trim();
                    row[indexBrandName] = dr["BrandName"].ToString().Trim().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

                    if (vMedProductType == (long)AllLookupValues.MedProductType.THUOC)
                    {
                        row[indexCategory] = dr["RefGenDrugCat_2"].ToString().Trim();
                    }
                    else
                    {
                        row[indexCategory] = dr["RefGenDrugCat_1"].ToString().Trim();
                    }

                    row[indexStockFirst] = Convert.ToDouble(dr["QtyAtFirst"]);//ton dau ky phai la first -khong phai final

                    //KMx: Lấy số lượng nhập trừ số lượng nhập trả ra. Số lượng nhập trả sẽ được cộng vào cột trả riêng (03/02/2015 17:02).
                    row[indexInward] = Convert.ToDouble(dr["InQty"]) - Convert.ToDouble(dr["InReturn"]);

                    var indexStartCotDiDongTrenRpt = 6;

                    double sumIc = 0;

                    if (dtOutQtyToClinicDept != null)
                    {
                        if (StorageClinicCount > 0)
                        {
                            foreach (DataRow store in StorageClinic)
                            {
                                //KMx: Với mỗi loại hàng, lấy số lượng xuất đến kho phòng.
                                double qty = 0;
                                //RecordType = 1: Xuất cho Kho Phòng, 2: Kho phòng trả cho Kho Dược.
                                DataRow OutQtyToClinic = dtOutQtyToClinicDept.Rows.Cast<DataRow>().Where(x => Convert.ToInt64(x["GenMedProductID"]) == genMedProductID && Convert.ToInt64(x["StoreClinicID"]) == Convert.ToInt64(store["StoreID"]) && Convert.ToByte(x["RecordType"]) == 1).FirstOrDefault();
                                if (OutQtyToClinic != null)
                                {
                                    qty = Convert.ToDouble(OutQtyToClinic["Qty"]);
                                }
                                sumIc = sumIc + qty;

                                row[indexStartCotDiDongTrenRpt] = qty;
                                indexStartCotDiDongTrenRpt++;
                            }
                        }

                        var indexStoreOther = 6 + StorageClinicCount;

                        if (StorageClinicOtherCount > 0)
                        {
                            double qty = 0;
                            foreach (DataRow store in StorageClinicOther)
                            {
                                //KMx: Với mỗi loại hàng, lấy số lượng xuất đến kho nội trú khác.
                                //RecordType = 1: Xuất cho Kho Phòng, 2: Kho phòng trả cho Kho Dược.
                                DataRow OutQtyToClinic = dtOutQtyToClinicDept.Rows.Cast<DataRow>().Where(x => Convert.ToInt64(x["GenMedProductID"]) == genMedProductID && Convert.ToInt64(x["StoreClinicID"]) == Convert.ToInt64(store["StoreID"]) && Convert.ToByte(x["RecordType"]) == 1).FirstOrDefault();
                                if (OutQtyToClinic != null)
                                {
                                    qty += Convert.ToDouble(OutQtyToClinic["Qty"]);
                                }

                            }

                            sumIc = sumIc + qty;

                            row[indexStoreOther] = qty;
                        }
                    }


                    row[indexIC] = sumIc;

                    row[indexCMI] = Convert.ToDouble(dr["OutQtyToCI"]);
                    row[indexOutNotHI] = Convert.ToDouble(dr["OutQtyNotHI"]);
                    row[indexOutHI] = Convert.ToDouble(dr["OutQtyHI"]);
                    row[indexOutDelete] = Convert.ToDouble(dr["OutQtyDelete"]);
                    row[indexOutPharmacy] = Convert.ToDouble(dr["OutQtyToPharmacy"]);

                    double TotalOutQty = sumIc + Convert.ToDouble(dr["OutQtyToCI"]) + Convert.ToDouble(dr["OutQtyNotHI"]) + Convert.ToDouble(dr["OutQtyHI"]) + Convert.ToDouble(dr["OutQtyDelete"]) + Convert.ToDouble(dr["OutQtyToPharmacy"]);

                    row[indexOutOther] = Convert.ToDouble(dr["QtyAtFirst"]) + Convert.ToDouble(dr["InQty"]) - TotalOutQty + Convert.ToDouble(dr["OutReturn"]) + Convert.ToDouble(dr["AdjustQty"]) - Convert.ToDouble(dr["QtyFinal"]);

                    //KMx: Cộng số lượng nhập trả từ kho phòng vào cột trả, không tính nhập trả chung với nhập từ NCC (03/02/2015 17:04).
                    row[indexOutReturn] = Convert.ToDouble(dr["OutReturn"]) + Convert.ToDouble(dr["InReturn"]);
                    row[indexAdjustQty] = Convert.ToDouble(dr["AdjustQty"]);

                    row[indexStockEnd] = Convert.ToDouble(dr["QtyFinal"]);

                    //var stockEnd = Convert.ToDouble(dr["QtyAtFirst"]) + Convert.ToDouble(dr["InQty"]) - sumIc - Convert.ToDouble(row[indexCMI]);
                    //row[indexStockEnd] = stockEnd;

                    dtResult.Rows.Add(row);
                }

            }



            //=================================================
            //Tính toán phun về Client nhận chuỗi
            foreach (DataColumn dc in dtResult.Columns)
            {
                sbValueCols.Append("<column name=\"" + dc.ColumnName.Trim() + "\"></column>");
            }

            //DataTable Row
            var sbValueCells = new StringBuilder();

            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                sbValueCells.Append("<row>");
                for (int j = 0; j < dtResult.Columns.Count; j++)
                {
                    sbValueCells.Append("<cell>" + dtResult.Rows[i][j] + "</cell>");
                }
                sbValueCells.Append("</row>");
            }

            sb = sb.Replace("[ValueCols]", sbValueCols.ToString());
            sb = sb.Replace("[ValueCells]", sbValueCells.ToString());

            return sb.ToString();
        }



        public String RptOneStorage(long vMedProductType, DateTime fromDate, DateTime toDate, long storeID, long? StoreClinicID, bool? IsShowHave, bool? IsShowHaveMedProduct)
        {
            var sb = new StringBuilder();
            sb.Append("<data>");
            sb.Append("<columns>");
            sb.Append("[ValueCols]");
            sb.Append("</columns>");

            sb.Append("<rows>");
            sb.Append("[ValueCells]");
            sb.Append("</rows>");

            sb.Append("</data>");


            //Các cột động xuất đến khoa
            var AllStorageWarehouseLocation = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoices_ListOutToKhoKhoaPhong(vMedProductType, fromDate, toDate, storeID, StoreClinicID, IsShowHave);

            //DataRow test = new DataRow();

            List<long> outputToIDList = new List<long>();

           
            //if (AllStorageWarehouseLocation != null)
            //{
            DataRow StorageClinic = AllStorageWarehouseLocation.Rows.Cast<DataRow>().FirstOrDefault();

            if (StorageClinic != null && Convert.ToInt64(StorageClinic["StoreID"]) > 0)
                {
                    outputToIDList.Add(Convert.ToInt64(StorageClinic["StoreID"]));
                }
            //}

            //Lấy danh sách số lượng xuất đến các kho.
            var dtOutQtyToClinicDept = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDept_OutToClinicDept(vMedProductType, fromDate, toDate, storeID, outputToIDList);

            //DataTable Column
            var sbValueCols = new StringBuilder();
            var dtResult = new DataTable();

            //Add Columns động
            dtResult.Columns.Add(new DataColumn { ColumnName = "STT", DataType = typeof(Int32), AllowDBNull = false, AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 });
            dtResult.Columns.Add(new DataColumn { ColumnName = "CODE", DataType = typeof(string) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Tên Hàng", DataType = typeof(string) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Phân Loại", DataType = typeof(string) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Tồn Đầu " + fromDate.ToString("dd/MM/yyyy"), DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Nhập", DataType = typeof(double) });

            const int sumCacCotCoDinhBefore = 6;

            int StorageClinicCount = 0;

            if (StorageClinic != null && Convert.ToInt64(StorageClinic["StoreID"]) > 0)
            {
                dtResult.Columns.Add(new DataColumn { ColumnName = StorageClinic["swhlName"].ToString().Trim(), DataType = typeof(double) });
                dtResult.Columns.Add(new DataColumn { ColumnName = StorageClinic["swhlName"].ToString().Trim() + " Trả", DataType = typeof(double) });

                StorageClinicCount = 2; //1 cột xuất, 1 cột trả.
            }

            dtResult.Columns.Add(new DataColumn { ColumnName = "I.C", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "C.M.I", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Bán, XV Kg BHYT", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "XV BHYT", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Hủy", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Trả Hàng", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Nhà Thuốc VT", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Khác", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Trả", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "SLCB", DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Tồn Cuối " + toDate.ToString("dd/MM/yyyy"), DataType = typeof(double) });
            dtResult.Columns.Add(new DataColumn { ColumnName = "Giá + VAT trung bình kho mới VND", DataType = typeof(double) });

            //Đánh chỉ số các cột.
            //var indexSTT = 0;
            var indexCode = 1;
            var indexBrandName = 2;
            var indexCategory = 3;
            var indexStockFirst = 4;
            var indexInward = 5;

            var indexIC = sumCacCotCoDinhBefore + StorageClinicCount;
            var indexCMI = sumCacCotCoDinhBefore + StorageClinicCount + 1;
            var indexOutNotHI = sumCacCotCoDinhBefore + StorageClinicCount + 2;
            var indexOutHI = sumCacCotCoDinhBefore + StorageClinicCount + 3;
            var indexOutDelete = sumCacCotCoDinhBefore + StorageClinicCount + 4;
            var indexOutReturnSupplier = sumCacCotCoDinhBefore + StorageClinicCount + 5;
            var indexOutPharmacy = sumCacCotCoDinhBefore + StorageClinicCount + 6;
            var indexOutOther = sumCacCotCoDinhBefore + StorageClinicCount + 7;
            var indexOutReturn = sumCacCotCoDinhBefore + StorageClinicCount + 8;
            var indexAdjustQty = sumCacCotCoDinhBefore + StorageClinicCount + 9;
            var indexStockEnd = sumCacCotCoDinhBefore + StorageClinicCount + 10;


            //Lấy nhập xuất tồn.
            var dtListGenMedProductID = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoices_ListGenMedProductIDNhap_Ton(vMedProductType, fromDate, toDate, storeID, IsShowHaveMedProduct);

            if (dtListGenMedProductID != null)
            {
                foreach (DataRow dr in dtListGenMedProductID.Rows)
                {
                    long genMedProductID = Convert.ToInt64(dr["GenMedProductID"]);

                    var row = dtResult.NewRow();

                    row[indexCode] = dr["Code"].ToString().Trim();
                    row[indexBrandName] = dr["BrandName"].ToString().Trim().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

                    if (vMedProductType == (long)AllLookupValues.MedProductType.THUOC)
                    {
                        row[indexCategory] = dr["RefGenDrugCat_2"].ToString().Trim();
                    }
                    else
                    {
                        row[indexCategory] = dr["RefGenDrugCat_1"].ToString().Trim();
                    }

                    row[indexStockFirst] = Convert.ToDouble(dr["QtyAtFirst"]);//ton dau ky phai la first -khong phai final

                    //KMx: Lấy số lượng nhập trừ số lượng nhập trả ra. Số lượng nhập trả sẽ được cộng vào cột trả riêng (03/02/2015 17:02).
                    row[indexInward] = Convert.ToDouble(dr["InQty"]) - Convert.ToDouble(dr["InReturn"]);

                    var indexStartCotDiDongTrenRpt = 6;

                    double Qty = 0;

                    double ReturnQty= 0;

                    if (dtOutQtyToClinicDept != null)
                    {
                        if (StorageClinic != null && Convert.ToInt64(StorageClinic["StoreID"]) > 0)
                        {
                            //KMx: Với mỗi loại hàng, lấy số lượng xuất đến kho phòng.
                            //RecordType = 1: Xuất cho Kho Phòng, 2: Kho phòng trả cho Kho Dược.
                            DataRow OutQtyToClinic = dtOutQtyToClinicDept.Rows.Cast<DataRow>().Where(x => Convert.ToInt64(x["GenMedProductID"]) == genMedProductID && Convert.ToInt64(x["StoreClinicID"]) == Convert.ToInt64(StorageClinic["StoreID"]) && Convert.ToByte(x["RecordType"]) == 1).FirstOrDefault();
                            if (OutQtyToClinic != null)
                            {
                                Qty = Convert.ToDouble(OutQtyToClinic["Qty"]);
                            }

                            DataRow ReturnQtyToMedDept = dtOutQtyToClinicDept.Rows.Cast<DataRow>().Where(x => Convert.ToInt64(x["GenMedProductID"]) == genMedProductID && Convert.ToInt64(x["StoreClinicID"]) == Convert.ToInt64(StorageClinic["StoreID"]) && Convert.ToByte(x["RecordType"]) == 2).FirstOrDefault();
                            if (ReturnQtyToMedDept != null)
                            {
                                ReturnQty = Convert.ToDouble(ReturnQtyToMedDept["Qty"]);
                            }

                            row[indexStartCotDiDongTrenRpt] = Qty;
                            row[indexStartCotDiDongTrenRpt + 1] = ReturnQty;
                        }

                    }

                    row[indexIC] = Qty;

                    row[indexCMI] = Convert.ToDouble(dr["OutQtyToCI"]);
                    row[indexOutNotHI] = Convert.ToDouble(dr["OutQtyNotHI"]);
                    row[indexOutHI] = Convert.ToDouble(dr["OutQtyHI"]);
                    row[indexOutDelete] = Convert.ToDouble(dr["OutQtyDelete"]);
                    row[indexOutPharmacy] = Convert.ToDouble(dr["OutQtyToPharmacy"]);

                    double TotalOutQty = Qty + Convert.ToDouble(dr["OutQtyToCI"]) + Convert.ToDouble(dr["OutQtyNotHI"]) + Convert.ToDouble(dr["OutQtyHI"]) + Convert.ToDouble(dr["OutQtyDelete"]) + Convert.ToDouble(dr["OutQtyToPharmacy"]);

                    row[indexOutOther] = Convert.ToDouble(dr["QtyAtFirst"]) + Convert.ToDouble(dr["InQty"]) - TotalOutQty + Convert.ToDouble(dr["OutReturn"]) + Convert.ToDouble(dr["AdjustQty"]) - Convert.ToDouble(dr["QtyFinal"]);

                    //KMx: Cộng số lượng nhập trả từ kho phòng vào cột trả, không tính nhập trả chung với nhập từ NCC.
                    //Nếu xem báo cáo 1 kho thì trừ số lượng trả của kho đang xem vì kho đang xem có 1 cột trả riêng. (03/02/2015 17:04).
                    row[indexOutReturn] = Convert.ToDouble(dr["OutReturn"]) + Convert.ToDouble(dr["InReturn"]) - ReturnQty;
                    row[indexAdjustQty] = Convert.ToDouble(dr["AdjustQty"]);

                    row[indexStockEnd] = Convert.ToDouble(dr["QtyFinal"]);

                    dtResult.Rows.Add(row);
                }

            }



            //=================================================
            //Tính toán phun về Client nhận chuỗi
            foreach (DataColumn dc in dtResult.Columns)
            {
                sbValueCols.Append("<column name=\"" + dc.ColumnName.Trim() + "\"></column>");
            }

            //DataTable Row
            var sbValueCells = new StringBuilder();

            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                sbValueCells.Append("<row>");
                for (int j = 0; j < dtResult.Columns.Count; j++)
                {
                    sbValueCells.Append("<cell>" + dtResult.Rows[i][j] + "</cell>");
                }
                sbValueCells.Append("</row>");
            }

            sb = sb.Replace("[ValueCols]", sbValueCols.ToString());
            sb = sb.Replace("[ValueCells]", sbValueCells.ToString());

            return sb.ToString();
        }

        #endregion

        // 20181005 TNHX: [BM0000034] Phieu Chi Dinh
        public Stream GetOutPatientPhieuChiDinhReportXMLInPdfFormat(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth,string parLogoUrl
            ,bool ApplyNewMethod = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient PhieuChiDinh Report");
                var report = new XtraReport();
                if (ApplyNewMethod)
                {
                    report = GetInstanceOutPatientPhieuChiDinh();
                }
                else
                {
                    report = new XRptOutPatientPhieuChiDinhXML();
                }
                report.Parameters["param_ListID"].Value = paymentIdxml;
                /*▼====: #002*/
                report.Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;
                /*▲====: #002*/
                AxLogger.Instance.LogInfo("End of loading Out-Patient PhieuChiDinh Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient PhieuChiDinh Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient PhieuChiDinh Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient PhieuChiDinh Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #004
        // 20181201 TNHX: [BM0005312] Phieu Mien Giam Ngoai Tru
        public Stream GetOutPatientPhieuMienGiamNgoaiTruReportInPdfFormat(long PtRegistrationID, long PromoDiscProgID, long totalMienGiam, string parHospitalName, string parHospitalAddress)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient PhieuMienGiamNgoaiTru Report");
                var report = new XRptPhieuMienGiamNgoaiTru_TV();
                report.Parameters["parPromoDiscProgID"].Value = (int)PromoDiscProgID;
                report.Parameters["parPtRegistrationID"].Value = PtRegistrationID;
                report.Parameters["parTotalMienGiam"].Value = (int)totalMienGiam;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                report.Parameters["parHospitalAddress"].Value = parHospitalAddress;

                AxLogger.Instance.LogInfo("End of loading Out-Patient PhieuMienGiamNgoaiTru Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient PhieuMienGiamNgoaiTru Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient PhieuMienGiamNgoaiTru Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient PhieuMienGiamNgoaiTru Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #004
        //▼====: #006
        public byte[] GetXRptInOutStockValueDrugDept_TV(string ReportTitle, DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, long SelectedDrugDeptProductGroupReportType
            , string ReportHospitalName, string ReportLogoUrl, long BidID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockValueDrugDept_TV Report");
                var report = new XRptInOutStockValueDrugDept_TV();
                report.Parameters["ReportTitle"].Value = ReportTitle;
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = Convert.ToInt32(StoreID);
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = Convert.ToInt32(V_MedProductType);
                report.Parameters["RefGenDrugCatID_1"].Value = Convert.ToInt32(RefGenDrugCatID_1);
                report.Parameters["DrugDeptProductGroupReportTypeID"].Value = Convert.ToInt32(SelectedDrugDeptProductGroupReportType);
                report.Parameters["parLogoUrl"].Value = ReportLogoUrl;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                if (BidID > 0)
                {
                    report.Parameters["BidID"].Value = BidID;
                }

                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRptInOutStockValueDrugDept_TV Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockValueDrugDept_TV Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #006
        //▼====: #008
        public byte[] GetXRptInOutStockGeneral(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockGeneral Report");
                var report = new XRptInOutStockGeneral();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRptInOutStockGeneral Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockGeneral Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #008

        //▼====: QTD NXT Thuốc toàn Khoa DƯỢC
        public byte[] GetXRptInOutStockGeneral_BHYT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, bool IsBHYT, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockGeneral_BHYT Report");
                var report = new XRptInOutStockGeneral_BHYT();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["IsBHYT"].Value = IsBHYT;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRptInOutStockGeneral_BHYT Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockGeneral Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====

        //▼====: NXT KT
        public byte[] GetXRptInOutStockValueDrugDept_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockValueDrugDept_KT Report");
                var report = new XRptInOutStockValueDrugDept_KT();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRptInOutStockValueDrugDept_KT Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockValueDrugDept_KT Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====

        //▼====: NXT KT - KHOAPHONG
        public byte[] GetXRptInOutStockValueClinicDept_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockValueClinicDept_KT Report");
                var report = new XRptInOutStockValueClinicDept_KT();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRptInOutStockValueClinicDept_KT Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockValueClinicDept_KT Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====

        //▼====: NXT KT - NHATHUOC
        public byte[] GetXRptInOutStockValue_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockValue_KT Report");
                var report = new XRptInOutStockValue_KT();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRptInOutStockValue_KT Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockValue_KT Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====

        //▼====: NXT KT CT
        public byte[] GetXRptInOutStockValueDrugDeptDetails_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockValueDrugDeptDetails_KT Report");
                var report = new XRptInOutStockValueDrugDeptDetails_KT();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRptInOutStockValueDrugDeptDetails_KT Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockValueDrugDeptDetails_KT Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====

        //▼====: NXT CT KT - KHOAPHONG
        public byte[] GetXRptInOutStockClinicDeptDetails_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockClinicDeptDetails_KT Report");
                var report = new XRptInOutStockClinicDeptDetails_KT();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRptInOutStockClinicDeptDetails_KT Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockClinicDeptDetails_KT Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====

        //▼====: NX Theo muc dich - DUOC
        public byte[] GetXRpt_MedDeptInOutStatisticsDetail(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string DepartmentOfHealth)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRpt_MedDeptInOutStatisticsDetail Report");
                var report = new XRpt_MedDeptInOutStatisticsDetail();
                report.Parameters["parFromDate"].Value = FromDate;
                report.Parameters["parToDate"].Value = ToDate;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["StoreName"].Value = StorageName;
                report.Parameters["parStoreIn"].Value = StoreIDIn;
                report.Parameters["parStoreOut"].Value = StoreIDOut;
                report.Parameters["parPurposeIn"].Value = PurposeInID;
                report.Parameters["parPurposeOut"].Value = PurposeOutID;
                report.Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                report.Parameters["parV_MedProductType"].Value = V_MedProductType;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRpt_MedDeptInOutStatisticsDetail Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRpt_MedDeptInOutStatisticsDetail Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetXRpt_MedDeptInOutStatisticsDetail_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string DepartmentOfHealth)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRpt_MedDeptInOutStatisticsDetail_V2 Report");
                var report = new XRpt_MedDeptInOutStatisticsDetail_V2();
                report.Parameters["parFromDate"].Value = FromDate;
                report.Parameters["parToDate"].Value = ToDate;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["StoreName"].Value = StorageName;
                report.Parameters["parStoreIn"].Value = StoreIDIn;
                report.Parameters["parStoreOut"].Value = StoreIDOut;
                report.Parameters["parPurposeIn"].Value = PurposeInID;
                report.Parameters["parPurposeOut"].Value = PurposeOutID;
                report.Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                report.Parameters["parV_MedProductType"].Value = V_MedProductType;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRpt_MedDeptInOutStatisticsDetail_V2 Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRpt_MedDeptInOutStatisticsDetail_V2 Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====

        //▼====: NX Theo muc dich - KHOAPHONG
        public byte[] GetXRpt_ClinicDeptInOutStatisticsDetail(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string DepartmentOfHealth)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRpt_ClinicDeptInOutStatisticsDetail Report");
                var report = new XRpt_ClinicDeptInOutStatisticsDetail();
                report.Parameters["parFromDate"].Value = FromDate;
                report.Parameters["parToDate"].Value = ToDate;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["StoreName"].Value = StorageName;
                report.Parameters["parStoreIn"].Value = StoreIDIn;
                report.Parameters["parStoreOut"].Value = StoreIDOut;
                report.Parameters["parPurposeIn"].Value = PurposeInID;
                report.Parameters["parPurposeOut"].Value = PurposeOutID;
                report.Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                report.Parameters["parV_MedProductType"].Value = V_MedProductType;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRpt_ClinicDeptInOutStatisticsDetail Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRpt_ClinicDeptInOutStatisticsDetail Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetXRpt_ClinicDeptInOutStatisticsDetail_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string DepartmentOfHealth)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRpt_ClinicDeptInOutStatisticsDetail_V2 Report");
                var report = new XRpt_ClinicDeptInOutStatisticsDetail_V2();
                report.Parameters["parFromDate"].Value = FromDate;
                report.Parameters["parToDate"].Value = ToDate;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["StoreName"].Value = StorageName;
                report.Parameters["parStoreIn"].Value = StoreIDIn;
                report.Parameters["parStoreOut"].Value = StoreIDOut;
                report.Parameters["parPurposeIn"].Value = PurposeInID;
                report.Parameters["parPurposeOut"].Value = PurposeOutID;
                report.Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                report.Parameters["parV_MedProductType"].Value = V_MedProductType;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRpt_ClinicDeptInOutStatisticsDetail_V2 Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRpt_ClinicDeptInOutStatisticsDetail_V2 Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====   

        //▼====: NX Theo muc dich - NHATHUOC
        public byte[] GetXRpt_DrugInOutStatisticsDetail(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, string ReportHospitalName, string DepartmentOfHealth)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRpt_DrugInOutStatisticsDetail Report");
                var report = new XRpt_DrugInOutStatisticsDetail();
                report.Parameters["parFromDate"].Value = FromDate;
                report.Parameters["parToDate"].Value = ToDate;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["StoreName"].Value = StorageName;
                report.Parameters["parStoreIn"].Value = StoreIDIn;
                report.Parameters["parStoreOut"].Value = StoreIDOut;
                report.Parameters["parPurposeIn"].Value = PurposeInID;
                report.Parameters["parPurposeOut"].Value = PurposeOutID;
                report.Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRpt_DrugInOutStatisticsDetail Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRpt_ClinicDeptInOutStatisticsDetail Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetXRpt_DrugInOutStatisticsDetail_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, string ReportHospitalName, string DepartmentOfHealth)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRpt_DrugInOutStatisticsDetail_V2 Report");
                var report = new XRpt_DrugInOutStatisticsDetail_V2();
                report.Parameters["parFromDate"].Value = FromDate;
                report.Parameters["parToDate"].Value = ToDate;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["StoreName"].Value = StorageName;
                report.Parameters["parStoreIn"].Value = StoreIDIn;
                report.Parameters["parStoreOut"].Value = StoreIDOut;
                report.Parameters["parPurposeIn"].Value = PurposeInID;
                report.Parameters["parPurposeOut"].Value = PurposeOutID;
                report.Parameters["parDepartmentOfHealth"].Value = DepartmentOfHealth;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRpt_DrugInOutStatisticsDetail_V2 Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRpt_ClinicDeptInOutStatisticsDetail_V2 Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====  

        //▼====: #007
        public byte[] GetPharmacyXRptInOutStocks_TV(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, string ReportLogoUrl)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStocks_TV Report");
                var report = new XRptInOutStocks();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = (int)StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["parLogoUrl"].Value = ReportLogoUrl;

                //report.ShowPreviewDialog();

                AxLogger.Instance.LogInfo("End of loading XRptInOutStocks_TV Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStocks_TV Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetPharmacyXRptInOutStockValue_TV(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, string ReportLogoUrl, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockValue_TV Report");
                var report = new XRptInOutStockValue_TV();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                //report.Parameters["parLogoUrl"].Value = ReportLogoUrl;
                //report.Parameters["parHospitalName"].Value = ReportHospitalName;

                //report.ShowPreviewDialog();

                AxLogger.Instance.LogInfo("End of loading XRptInOutStockValue_TV Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockValue_TV Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetXRptInOutStockValueClinicDept_TV(string ReportTitle, DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, long SelectedDrugDeptProductGroupReportType
            , string ReportHospitalName, string ReportLogoUrl, long BidID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockValueClinicDept_TV Report");
                var report = new XRptInOutStockValueClinicDept_TV();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                report.Parameters["DrugDeptProductGroupReportTypeID"].Value = SelectedDrugDeptProductGroupReportType;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;

                AxLogger.Instance.LogInfo("End of loading XRptInOutStockValueClinicDept_TV Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockValueClinicDept_TV Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetXRptInOutStockClinicDept_TV(string ReportTitle, DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, bool ViewBefore20150331
            , string ReportLogoUrl)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockClinicDept Report");
                var report = new XRptInOutStockClinicDept();
                report.Parameters["ReportTitle"].Value = ReportTitle;
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                report.Parameters["ViewBefore20150331"].Value = ViewBefore20150331;
                report.Parameters["parLogoUrl"].Value = ReportLogoUrl;

                //report.ShowPreviewDialog();

                AxLogger.Instance.LogInfo("End of loading XRptInOutStockClinicDept Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockClinicDept Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #007
        //▼====: #009
        public byte[] GetOutPatientReceiptReportXML_V4(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth
            , string parLogoUrl, bool ApplyNewMethod = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient Receipt Report");
                var report = new XRptOutPatientReceiptXML_V4();
                report.Parameters["param_PaymentID"].Value = paymentIdxml;
                report.Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;

                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                AxLogger.Instance.LogInfo("End of loading Out-Patient Receipt Report. Status: Success.");
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient Receipt Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetOutPatientPhieuChiDinhReportXML(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth
            , string parLogoUrl, bool ApplyNewMethod = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient PhieuChiDinh Report");
                var report = new XRptOutPatientPhieuChiDinhXML();
                report.Parameters["param_ListID"].Value = paymentIdxml;
                report.Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;

                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                AxLogger.Instance.LogInfo("End of loading Out-Patient PhieuChiDinh Report. Status: Success.");
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient PhieuChiDinh Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #009

        //20210806 QTD New method for GetCardStorage
        //Accountant
        public byte[] GetXRptDrugDeptCardStorage(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report GetXRptDrugDeptCardStorage");
                XtraReport report = new XRptDrugDeptCardStorage();
                report.Parameters["DrugName"].Value = BrandName;
                report.Parameters["GenMedProductID"].Value = Convert.ToInt32(GenMedProductID);
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["UnitName"].Value = UnitName;
                report.Parameters["V_MedProductType"].Value = Convert.ToInt32(V_MedProductType);
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["ShowDate"].Value = dateshow;
                report.Parameters["Code"].Value = Code;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;

                AxLogger.Instance.LogInfo("End of loading GetXRptDrugDeptCardStorage Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing GetXRptDrugDeptCardStorage Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetXRptClinicDeptCardStorage(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report GetXRptClinicDeptCardStorage");
                XtraReport report = new XRptClinicDeptCardStorage();
                report.Parameters["DrugName"].Value = BrandName;
                report.Parameters["GenMedProductID"].Value = Convert.ToInt32(GenMedProductID);
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["UnitName"].Value = UnitName;
                report.Parameters["V_MedProductType"].Value = Convert.ToInt32(V_MedProductType);
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["ShowDate"].Value = dateshow;
                report.Parameters["Code"].Value = Code;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;

                AxLogger.Instance.LogInfo("End of loading GetXRptClinicDeptCardStorage Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing GetXRptClinicDeptCardStorage Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetXRptPharmacyCardStorage(long StoreID, long GenMedProductID, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report GetXRptPharmacyCardStorage");
                XtraReport report = new XRptPharmacyCardStorage();
                report.Parameters["DrugName"].Value = BrandName;
                report.Parameters["GenMedProductID"].Value = Convert.ToInt32(GenMedProductID);
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["UnitName"].Value = UnitName;
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["ShowDate"].Value = dateshow;
                report.Parameters["Code"].Value = Code;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;

                AxLogger.Instance.LogInfo("End of loading GetXRptPharmacyCardStorage Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing GetXRptPharmacyCardStorage Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //End Accountant

        public byte[] GetXRptCardStorageDrugDept(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate, string dateshow, string Code, string parLogoUrl)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report GetXRptDrugDeptCardStorage");
                XtraReport report = new XRptDrugDeptCardStorage();
                report.Parameters["DrugName"].Value = BrandName;
                report.Parameters["GenMedProductID"].Value = Convert.ToInt32(GenMedProductID);
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["UnitName"].Value = UnitName;
                report.Parameters["V_MedProductType"].Value = Convert.ToInt32(V_MedProductType);
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["ShowDate"].Value = dateshow;
                report.Parameters["Code"].Value = Code;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;

                AxLogger.Instance.LogInfo("End of loading GetXRptCardStorageDrugDept Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing GetXRptDrugDeptCardStorage Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetXRptCardStorage(long StoreID, long GenMedProductID, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate, string dateshow, string Code, string parLogoUrl)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report GetXRptCardStorage");
                XtraReport report = new XRptCardStorage();
                report.Parameters["DrugName"].Value = BrandName;
                report.Parameters["GenMedProductID"].Value = Convert.ToInt32(GenMedProductID);
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["UnitName"].Value = UnitName;
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["ShowDate"].Value = dateshow;
                report.Parameters["Code"].Value = Code;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;

                AxLogger.Instance.LogInfo("End of loading GetXRptCardStorage Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing GetXRptCardStorage Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public byte[] GetXRptCardStorageClinicDept(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate, string dateshow, string Code, string parLogoUrl)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading report GetXRptCardStorageClinicDept");
                XtraReport report = new XRptCardStorageClinicDept();
                report.Parameters["DrugName"].Value = BrandName;
                report.Parameters["GenMedProductID"].Value = Convert.ToInt32(GenMedProductID);
                report.Parameters["V_MedProductType"].Value = Convert.ToInt32(V_MedProductType);
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["UnitName"].Value = UnitName;
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["ShowDate"].Value = dateshow;
                report.Parameters["Code"].Value = Code;
                report.Parameters["parLogoUrl"].Value = parLogoUrl;

                AxLogger.Instance.LogInfo("End of loading GetXRptCardStorageClinicDept Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing GetXRptCardStorageClinicDept Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #011
        public byte[] GetXRptGeneralInOutStatistics_V3(DateTime? FromDate, DateTime? ToDate, string DateShow, string ReportHospitalName, string ReportHospitalAddress)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptGeneralInOutStatistics_V3 Report");
                var report = new XRptGeneralInOutStatistics_V3();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                report.Parameters["parHospitalAddress"].Value = ReportHospitalAddress;
                //report.ShowPreviewDialog();
                AxLogger.Instance.LogInfo("End of loading XRptGeneralInOutStatistics_V3 Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptGeneralInOutStatistics_V3 Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #011

        public Stream GetPCLImagingResultInPdfFormat(long V_ReportForm, long PCLImgResultID, long V_PCLRequestType, string reportHospitalName, 
            string hospitalCode, string reportDepartmentOfHealth, string reportHospitalAddress, bool isApplyPCRDual )
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PCLImagingResult Report");
                var report = new XtraReport();
                switch (V_ReportForm)
                {
                    case (long)AllLookupValues.V_ReportForm.Mau_0_Hinh:
                        report = new XRpt_PCLImagingResult_New_0_Hinh();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_1_Hinh:
                        report = new XRpt_PCLImagingResult_New_1_Hinh();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_2_Hinh:
                        report = new XRpt_PCLImagingResult_New();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_3_Hinh:
                        report = new XRpt_PCLImagingResult_New_3_Hinh();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_4_Hinh:
                        report = new XRpt_PCLImagingResult_New_4_Hinh();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh:
                        report = new XRpt_PCLImagingResult_New_6_Hinh();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Realtime_PCR:
                        if (isApplyPCRDual)
                        {
                            report = new XRpt_PCLImagingResult_New_Realtime_PCR_Cov_Dual();
                        }
                        else
                        {
                            report = new XRpt_PCLImagingResult_New_Realtime_PCR_Cov();
                        }
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Test_Nhanh:
                        report = new XRpt_PCLImagingResult_New_Test_Nhanh_Cov();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Xet_Nghiem:
                        report = new XRpt_PCLImagingResult_New_Xet_Nghiem();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Helicobacter_Pylori:
                        report = new XRpt_PCLImagingResult_New_Helicobacter_Pylori();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim:
                        report = new XRpt_PCLImagingResult_New_Sieu_Am_Tim();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D:
                        report = new XRpt_PCLImagingResult_New_Sieu_Am_San_4D();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim:
                        report = new XRpt_PCLImagingResult_New_Dien_Tim();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Dien_Nao:
                        report = new XRpt_PCLImagingResult_New_Dien_Nao();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_ABI:
                        report = new XRpt_PCLImagingResult_New_ABI();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim_Gang_Suc:
                        report = new XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_CLVT_Hai_Ham:
                        report = new XRpt_PCLImagingResult_New_CLVT_Hai_Ham();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D_New:
                        report = new XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_2_New:
                        report = new XRpt_PCLImagingResult_New_6_Hinh_2_New();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_1_New:
                        report = new XRpt_PCLImagingResult_New_6_Hinh_1_New();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim_New:
                        report = new XRpt_PCLImagingResult_New_Sieu_Am_Tim_New();
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Noi_Soi_9_Hinh:
                        report = new XRpt_PCLImagingResult_New_Noi_Soi_9_Hinh();
                        break;
                    default:
                        report = new XRpt_PCLImagingResult_New();
                        break;
                }
                report.Parameters["PCLImgResultID"].Value = PCLImgResultID;
                report.Parameters["V_PCLRequestType"].Value = V_PCLRequestType;
                report.Parameters["parHospitalName"].Value = reportHospitalName;
                report.Parameters["parHospitalCode"].Value = hospitalCode;
                report.Parameters["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                report.Parameters["parHospitalAddress"].Value = reportHospitalAddress;
                AxLogger.Instance.LogInfo("End of loading PCLImagingResult Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting PCLImagingResult Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting PCLImagingResult Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing PCLImagingResult Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #013
        public byte[] GetXRptInOutStockValueClinicDept_KT_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockValueClinicDept_KT_V2 Report");
                var report = new XRptInOutStockValueClinicDept_KT_V2();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                AxLogger.Instance.LogInfo("End of loading XRptInOutStockValueClinicDept_KT_V2 Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRptInOutStockValueClinicDept_KT_V2 Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #013
        //▼====: #014
        public Stream GetXRptPCLDepartmentLaboratoryResultReportModel_TV3PdfFormat(string parPrescriptionMainRightHeader, string parPrescriptionSubRightHeader, string parHeadOfLaboratoryFullName
            , string parHeadOfLaboratoryPositionName, string parDepartmentOfHealth, string parHospitalAddress, string parHospitalName, string parHospitalCode, int parPatientID, int parPatientPCLReqID
            , int parV_PCLRequestType, int parPatientFindBy, string parStaffName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PCLDepartmentLaboratoryResultReportModel_TV3 Report");
                var report = new XRptPatientPCLLaboratoryResults_TV3();
                report.Parameters["parPrescriptionMainRightHeader"].Value = parPrescriptionMainRightHeader;
                report.Parameters["parPrescriptionSubRightHeader"].Value = parPrescriptionSubRightHeader;
                report.Parameters["parHeadOfLaboratoryFullName"].Value = parHeadOfLaboratoryFullName;
                report.Parameters["parHeadOfLaboratoryPositionName"].Value = parHeadOfLaboratoryPositionName;
                report.Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth;
                report.Parameters["parHospitalAddress"].Value = parHospitalAddress;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                report.Parameters["parHospitalCode"].Value = parHospitalCode;
                report.Parameters["parPatientID"].Value = parPatientID;
                report.Parameters["parPatientPCLReqID"].Value = parPatientPCLReqID;
                report.Parameters["parV_PCLRequestType"].Value = parV_PCLRequestType;
                report.Parameters["parPatientFindBy"].Value = parPatientFindBy;
                report.Parameters["parStaffName"].Value = parStaffName;

                AxLogger.Instance.LogInfo("End of loading PCLDepartmentLaboratoryResultReportModel_TV3 Report. Status: Success.");
                MemoryStream ms = ExportReportToPdf(report);
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing PCLDepartmentLaboratoryResultReportModel_TV3 Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #014

        //▼====: #015
        public byte[] GetXRptBCNhapTuNCC(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, string ReportHospitalName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading XRptInOutStockValueClinicDept_KT_V2 Report");
                var report = new XRpt_BCNhapTuNCC();
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["StorageName"].Value = StorageName;
                report.Parameters["StoreID"].Value = StoreID;
                report.Parameters["DateShow"].Value = DateShow;
                report.Parameters["V_MedProductType"].Value = V_MedProductType;
                report.Parameters["parHospitalName"].Value = ReportHospitalName;
                AxLogger.Instance.LogInfo("End of loading XRpt_BCNhapTuNCC Report. Status: Success.");
                MemoryStream stream = new MemoryStream();
                report.CreateDocument();
                report.PrintingSystem.SaveDocument(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing XRpt_BCNhapTuNCC Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #015

        //▼==== #017
        public Stream InHuongDanSuDungThuocWithoutView(long PtRegistrationID, bool IsInsurance)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient InHuongDanSuDungThuocWithoutView Report");
                var report = new XtraReport();

                report = new RptHuongDanDungThuocPaging();
                report.Parameters["PtRegistrationID"].Value = PtRegistrationID;
                report.Parameters["BeOfHIMedicineList"].Value = IsInsurance;

                AxLogger.Instance.LogInfo("End of loading Out-Patient InHuongDanSuDungThuocWithoutView Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient InHuongDanSuDungThuocWithoutView Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient InHuongDanSuDungThuocWithoutView Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient InHuongDanSuDungThuocWithoutView Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #017
        public Stream GetXRpt_Temp12InPdfFormat(long TransactionID, long PtRegistrationID, DateTime? FromDate, DateTime? ToDate, bool ViewByDate, string StaffName
            , long DeptID, string DeptName, long RegistrationType, string parHospitalAdress, string parHospitalName, string parDepartmentOfHealth, bool IsPatientCOVID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetXRpt_Temp12_6556 Report");
                var report = new XtraReport();

                report = new XRpt_Temp12_TongHop();
                report.Parameters["TransactionID"].Value = TransactionID;
                report.Parameters["PtRegistrationID"].Value = PtRegistrationID;
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["ViewByDate"].Value = ViewByDate;
                report.Parameters["StaffName"].Value = StaffName;
                report.Parameters["DeptID"].Value = DeptID;
                report.Parameters["DeptName"].Value = DeptName;
                report.Parameters["RegistrationType"].Value = RegistrationType;
                report.Parameters["parHospitalAdress"].Value = parHospitalAdress;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                report.Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth;
                report.Parameters["IsPatientCOVID"].Value = IsPatientCOVID;

                AxLogger.Instance.LogInfo("End of loading GetXRpt_Temp12_6556 Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting GetXRpt_Temp12_6556 Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting GetXRpt_Temp12_6556 Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient InHuongDanSuDungThuocWithoutView Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public Stream GetXRpt_Temp12_6556InPdfFormat(long TransactionID, long PtRegistrationID, DateTime? FromDate, DateTime? ToDate, bool ViewByDate, string StaffName, long DeptID,
            string DeptName, long RegistrationType, string parHospitalAdress, string parHospitalName, string parDepartmentOfHealth)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetXRpt_Temp12_6556 Report");
                var report = new XtraReport();

                report = new XRpt_Temp12_6556();
                report.Parameters["TransactionID"].Value = TransactionID;
                report.Parameters["PtRegistrationID"].Value = PtRegistrationID;
                report.Parameters["FromDate"].Value = FromDate;
                report.Parameters["ToDate"].Value = ToDate;
                report.Parameters["ViewByDate"].Value = ViewByDate;
                report.Parameters["StaffName"].Value = StaffName;
                report.Parameters["DeptID"].Value = DeptID;
                report.Parameters["DeptName"].Value = DeptName;
                report.Parameters["RegistrationType"].Value = RegistrationType;
                report.Parameters["parHospitalAdress"].Value = parHospitalAdress;
                report.Parameters["parHospitalName"].Value = parHospitalName;
                report.Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth;

                AxLogger.Instance.LogInfo("End of loading GetXRpt_Temp12_6556 Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting GetXRpt_Temp12_6556 Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting GetXRpt_Temp12_6556 Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient InHuongDanSuDungThuocWithoutView Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼==== #018
        public Stream InPhieuChoSoanThuocWithoutView(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Out-Patient InPhieuChoSoanThuocWithoutView Report");
                var report = new XtraReport();

                report = new PhieuSoanThuocPaging();
                report.Parameters["PtRegistrationID"].Value = PtRegistrationID;

                AxLogger.Instance.LogInfo("End of loading Out-Patient InPhieuChoSoanThuocWithoutView Report. Status: Success.");

                AxLogger.Instance.LogInfo("Start exporting Out-Patient InPhieuChoSoanThuocWithoutView Report to PDF format");
                var ms = ExportReportToPdf(report);
                AxLogger.Instance.LogInfo("End of exporting Out-Patient InPhieuChoSoanThuocWithoutView Report to PDF format. Status: Success.");
                return ms;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing Out-Patient InPhieuChoSoanThuocWithoutView Report. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_OUTPATIENT_RECEIPT_XML_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #018
        //▼==== #019
        public string GetPDFResulstToSign(string ListSendTransaction, int LaboratoryResultVersion, string StaffName
            , string PrescriptionMainRightHeader, string PrescriptionSubRightHeader, string parHeadOfLaboratoryFullName, string parHeadOfLaboratoryPositionName
            , string reportDepartmentOfHealth, string reportHospitalAddress, string reportHospitalName, string HospitalCode
            , string ServicePool, string PDFFileResultToSignPath, string PDFFileResultSignedPath
            , string FtpUsername, string FtpPassword, string FtpUrl)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start calling GetPDFResulstToSign from list result Report into pdf file and upload into server");
                XmlTextReader txtReader = new XmlTextReader(new StringReader(ListSendTransaction));
                XmlSerializer serializer = new XmlSerializer(typeof(Root));
                Root RootPatientPCLRequest = (Root)serializer.Deserialize(txtReader);

                if (RootPatientPCLRequest != null && RootPatientPCLRequest.Item  != null && RootPatientPCLRequest.Item.Count() > 0)
                {
                    foreach (var item in RootPatientPCLRequest.Item)
                    {
                        var report = new XtraReport();
                        if (item.V_PCLMainCategory == 28201)
                        {
                            switch (LaboratoryResultVersion)
                            {
                                case 3:
                                    report = new XRptPatientPCLLaboratoryResults_TV3();
                                    report.Parameters["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    report.Parameters["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    report.Parameters["parHeadOfLaboratoryFullName"].Value = parHeadOfLaboratoryFullName;
                                    report.Parameters["parHeadOfLaboratoryPositionName"].Value = parHeadOfLaboratoryPositionName;
                                    report.Parameters["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                    report.Parameters["parHospitalAddress"].Value = reportHospitalAddress;
                                    report.Parameters["parHospitalName"].Value = reportHospitalName;
                                    report.Parameters["parHospitalCode"].Value = HospitalCode;
                                    break;
                                default:
                                    break;
                            }
                            report.Parameters["parPatientID"].Value = (int)item.PatientID;
                            report.Parameters["parPatientPCLReqID"].Value = (int)item.patientPCLReqID;
                            report.Parameters["parV_PCLRequestType"].Value = (int)item.V_PCLRequestType;
                            report.Parameters["parPatientFindBy"].Value = (int)item.FindPatient;
                            report.Parameters["parStaffName"].Value = StaffName;
                            report.CreateDocument();
                            MemoryStream filePDF = new MemoryStream();
                            report.ExportToPdf(filePDF);
                            item.filePDF = filePDF.ToArray();
                            item.fileName = report.ExportOptions.PrintPreview.DefaultFileName;
                            item.Pages = report.Pages.Count();
                        }
                        else
                        {
                            string error = "Chưa thực hiện kết quả ngoài xét nghiệm!";
                            var axErr = AxException.CatchExceptionAndLogMessage(new Exception(error), ErrorNames.COMMON_SERVICE_GetForm02);
                            throw new FaultException<AxException>(axErr, error);
                        }
                    }
                }
                // đổi thành upload ftp qua service của server ký số                
                foreach (var item in RootPatientPCLRequest.Item)
                {
                    // tạo file tại server trước
                    var strFolderPath = "";
                    strFolderPath = Path.GetFullPath(Path.Combine(ServicePool, PDFFileResultToSignPath, item.folderName));
                    if (!Directory.Exists(strFolderPath))
                    {
                        Directory.CreateDirectory(strFolderPath);
                    }

                    string strFolderPathAndFileName = Path.Combine(strFolderPath, item.fileName);
                    item.fullPathPDFFileToSign = strFolderPathAndFileName;
                    item.fullPathPDFFileSigned = Path.Combine(ServicePool, PDFFileResultSignedPath);
                    //string fileName = item.fileName
                    item.fileNameSigned = item.fileName.Replace(".pdf", "") + "_signed.pdf";
                    if (!File.Exists(strFolderPathAndFileName) && item.filePDF != null)
                    {
                        using (FileStream fs = new FileStream(strFolderPathAndFileName, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(item.filePDF, 0, item.filePDF.Length);
                            fs.Close();
                        }
                    }
                    // upload file qua ftp
                    string ftpFolder = FtpUrl + "//" + PDFFileResultToSignPath.Replace("\\", "/") + "/" + item.folderName;
                    if (DirectoryExists(ftpFolder, FtpUsername, FtpPassword))
                    {
                        if (File.Exists(strFolderPathAndFileName))
                        {
                            using (WebClient client = new WebClient())
                            {
                                client.Credentials = new NetworkCredential(FtpUsername, FtpPassword);
                                client.UploadFile(ftpFolder + "/" + item.fileName, "STOR", strFolderPathAndFileName);
                            }
                        }
                    }
                }
                // trả danh sách path đã upload về client
                StringBuilder sb = new StringBuilder();
                sb.Append("<Root>");
                foreach (var item in RootPatientPCLRequest.Item)
                {
                    sb.Append("<Item>");
                    sb.AppendFormat("<patientPCLReqID>{0}</patientPCLReqID>", item.patientPCLReqID);
                    sb.AppendFormat("<fullPathPDFFileToSign>{0}</fullPathPDFFileToSign>", item.fullPathPDFFileToSign);
                    sb.AppendFormat("<fullPathPDFFileSigned>{0}</fullPathPDFFileSigned>", item.fullPathPDFFileSigned);
                    sb.AppendFormat("<folderName>{0}</folderName>", item.folderName);
                    sb.AppendFormat("<fileNameSigned>{0}</fileNameSigned>", item.fileNameSigned);
                    sb.AppendFormat("<fileName>{0}</fileName>", item.fileName);
                    sb.AppendFormat("<pageIndex>{0}</pageIndex>", item.Pages);
                    sb.AppendFormat("<findPatient>{0}</findPatient>", item.FindPatient);
                    sb.AppendFormat("<resultID>{0}</resultID>", item.resultID);
                    sb.Append("</Item>");
                }
                sb.Append("</Root>");
                AxLogger.Instance.LogInfo("End of processing GetPDFResulstToSign. Status: Success.");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing GetPDFResulstToSign. Status: Failed.");
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_PCLLABRESULT_NOT_PRINT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DirectoryExists(string directory, string ftpUName, string ftpPWord)
        {
            bool directoryExists;

            var request = (FtpWebRequest)WebRequest.Create(directory);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(ftpUName, ftpPWord);

            try
            {
                using (request.GetResponse())
                {
                    directoryExists = true;
                }
            }
            catch (WebException)
            {
                directoryExists = false;
                WebRequest ftpRequest = WebRequest.Create(directory);
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpRequest.Credentials = new NetworkCredential(ftpUName, ftpPWord);
                try
                {
                    using (ftpRequest.GetResponse())
                    {
                        directoryExists = true;
                    }
                }
                catch (WebException webEx)
                {
                    throw new Exception(webEx.Message);
                }
            }

            return directoryExists;
        }

        [XmlRoot(ElementName = "Item")]
        public class Item
        {
            [XmlElement(ElementName = "patientPCLReqID")]
            public long patientPCLReqID { get; set; }
            [XmlElement(ElementName = "PatientID")]
            public long PatientID { get; set; }
            [XmlElement(ElementName = "V_PCLRequestType")]
            public long V_PCLRequestType { get; set; }
            [XmlElement(ElementName = "V_PCLMainCategory")]
            public long V_PCLMainCategory { get; set; }
            [XmlElement(ElementName = "FindPatient")]
            public int FindPatient { get; set; }
            [XmlElement(ElementName = "filePDF")]
            public byte[] filePDF { get; set; }
            [XmlElement(ElementName = "fileName")]
            public string fileName { get; set; }
            [XmlElement(ElementName = "folderName")]
            public string folderName { get; set; }
            [XmlElement(ElementName = "fullPathPDFFileToSign")]
            public string fullPathPDFFileToSign { get; set; }
            [XmlElement(ElementName = "fullPathPDFFileSigned")]
            public string fullPathPDFFileSigned { get; set; }
            [XmlElement(ElementName = "Pages")]
            public int Pages { get; set; }
            [XmlElement(ElementName = "fileNameSigned")]
            public string fileNameSigned { get; set; }
            [XmlElement(ElementName = "pageIndex")]
            public string pageIndex { get; set; }
            [XmlElement(ElementName = "resultID")]
            public long resultID { get; set; }
        }

        [Serializable()]
        [XmlRootAttribute("Root", Namespace = "", IsNullable = false)]
        public class Root
        {
            [XmlElement(ElementName = "Item")]
            public List<Item> Item { get; set; }
        }
        //▲==== #019
    }
}
