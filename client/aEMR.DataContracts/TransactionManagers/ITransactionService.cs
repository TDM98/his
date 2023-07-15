using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using System.IO;
using aEMR.DataContracts;
using System.Data;
/*
 * 20190326 #001 TNHX: Add function to export excel in ConfirmHIRegistrationView
 * 20210323 #002 BLQ: #243 Lấy HIReport cho KHTH
 * 20221212 #003 TNHX: 994 Đẩy dữ liệu đơn thuốc điện tử
 * 20230108 #004 TNHX: 994 Thêm func xóa dữ liệu đẩy đơn thuốc điện tử không thành công
 * 20230220 #005 QTD:  Xoá đơn thuốc quốc gia nhà thuốc
 * 20230314 #006 BLQ: Thêm hàm tạo xml cho 130
 */
namespace TransactionServiceProxy
{
    [ServiceContract]
    public interface ITransactionService
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTransaction_ByPtID(SeachPtRegistrationCriteria SearchCriteria, int PageSize, int PageIndex, bool CountTotal, AsyncCallback callback, object state);
        IList<PatientTransaction> EndGetTransaction_ByPtID(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertDataToReport(string begindate, string enddate, AsyncCallback callback, object state);
        bool EndInsertDataToReport(IAsyncResult asyncResult);

        //export excel all
        #region Export excel from database

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAllGeneric(ReportParameters criteria, long staffID, AsyncCallback callback, object state);
        byte[] EndExportToExcellAllGeneric(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAllGeneric_New(ReportParameters criteria, string filePath, long staffID, AsyncCallback callback, object state);
        byte[] EndExportToExcellAllGeneric_New(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToCSVAllGeneric(ReportParameters criteria, long staffID, AsyncCallback callback, object state);
        byte[] EndExportToCSVAllGeneric(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportEInvoiceToExcel(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, AsyncCallback callback, object state);
        byte[] EndExportEInvoiceToExcel(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportHospitalClientContractDetails_Excel(long HosClientContractID, AsyncCallback callback, object state);
        byte[] EndExportHospitalClientContractDetails_Excel(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportHospitalClientContractResultDetails_Excel(long HosClientContractID, AsyncCallback callback, object state);
        byte[] EndExportHospitalClientContractResultDetails_Excel(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportFromExcel(ReportParameters criteria, long staffID, AsyncCallback callback, object state);
        byte[] EndExportFromExcel(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_Temp25a(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_Temp25a(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_Temp25aNew(ReportParameters criteria, AsyncCallback callback, object state);
        string EndExportToExcellAll_Temp25aNew(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_Temp26a(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_Temp26a(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_Temp20NgoaiTru(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_Temp20NgoaiTru(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_Temp20NoiTru(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_Temp20NoiTru(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_Temp21NgoaiTru(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_Temp21NgoaiTru(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_Temp21NoiTru(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_Temp21NoiTru(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_ChiTietVienPhi_PK(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_ChiTietVienPhi_PK(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_ChiTietVienPhi(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_ChiTietVienPhi(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_ThongKeDoanhThu(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_ThongKeDoanhThu(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_DoanhThuTongHop(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_DoanhThuTongHop(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcellAll_DoanhThuNoiTruTongHop(ReportParameters criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_DoanhThuNoiTruTongHop(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginFollowUserGetReport(long staffID, string reportName, string reportParams, long v_GetReportMethod, AsyncCallback callback, object state);
        bool EndFollowUserGetReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCreateHIReport(HealthInsuranceReport HIReport, AsyncCallback callback, object state);
        bool EndCreateHIReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCreateFastReport(HealthInsuranceReport gReport, long V_FastReportType, AsyncCallback callback, object state);
        bool EndCreateFastReport(out long FastReportID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRegistrationHIReport(string ma_lk, AsyncCallback callback, object state);
        bool EndDeleteRegistrationHIReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationsForHIReport(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchRegistrationsForHIReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationsForCreateOutPtTransactionFinalization(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, AsyncCallback callback, object state);
        IList<PatientRegistration_V2> EndSearchRegistrationsForCreateOutPtTransactionFinalization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTransactionFinalizationSummaryInfos(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, AsyncCallback callback, object state);
        IList<OutPtTransactionFinalization> EndGetTransactionFinalizationSummaryInfos(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationsForCreateEInvoices(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, AsyncCallback callback, object state);
        IList<PatientRegistration_V2> EndSearchRegistrationsForCreateEInvoices(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateHIReportStatus(HealthInsuranceReport aHealthInsuranceReport, AsyncCallback callback, object state);
        bool EndUpdateHIReportStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCreateHIReport_V2(HealthInsuranceReport HIReport, AsyncCallback callback, object state);
        bool EndCreateHIReport_V2(out long HIReportID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCreateHIReportOutInPt(HealthInsuranceReport HIReport, AsyncCallback callback, object state);
        bool EndCreateHIReportOutInPt(out long HIReportID, out long HIReportOutPt, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetHIReport(AsyncCallback callback, object state);
        List<HealthInsuranceReport> EndGetHIReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetFastReports(long V_FastReportType, AsyncCallback callback, object state);
        List<HealthInsuranceReport> EndGetFastReports(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetFastReportDetails(long FastReportID, AsyncCallback callback, object state);
        DataSet EndGetFastReportDetails(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetHIXmlReport9324_AllTab123_InOneRpt(long nHIReportID, AsyncCallback callback, object state);
        byte[] EndGetHIXmlReport9324_AllTab123_InOneRpt(IAsyncResult asyncResult);
        /*▼====: #001*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditHOSPayment(HOSPayment aHOSPayment, AsyncCallback callback, object state);
        HOSPayment EndEditHOSPayment(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetHOSPayments(DateTime aStartDate, DateTime aEndDate, AsyncCallback callback, object state);
        IList<HOSPayment> EndGetHOSPayments(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteHOSPayment(long aHOSPaymentID, AsyncCallback callback, object state);
        bool EndDeleteHOSPayment(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        /*▲====: #001*/
        IAsyncResult BeginPreviewHIReport(long PtRegistrationID, long V_RegistrationType, long? OutPtTreatmentProgramID, AsyncCallback callback, object state);
        DataSet EndPreviewHIReport(out string ErrText, IAsyncResult asyncResult);
        //▼====: #002
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPreviewHIReport_ForKHTH(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        DataSet EndPreviewHIReport_ForKHTH(out string ErrText, IAsyncResult asyncResult);
        //▲====: #002
        //▼====: #001
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginExportExcelRegistrationsForHIReportWaitConfirm(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, AsyncCallback callback, object state);
        List<List<string>> EndExportExcelRegistrationsForHIReportWaitConfirm(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginExportExcelRegistrationsForHIReportOther(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, AsyncCallback callback, object state);
        List<List<string>> EndExportExcelRegistrationsForHIReportOther(IAsyncResult asyncResult);
        //▲====: #001
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        /*▲====: #001*/
        IAsyncResult BeginGetRptOutPtTransactionFinalizationDetails(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        List<RptOutPtTransactionFinalizationDetail> EndGetRptOutPtTransactionFinalizationDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreateDQGReport(DQGReport aDQGReport, AsyncCallback callback, object state);
        DQGReport EndCreateDQGReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateCodeDQGReport(DQGReport aDQGReport, byte aCase, AsyncCallback callback, object state);
        bool EndUpdateCodeDQGReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDQGReports(AsyncCallback callback, object state);
        List<DQGReport> EndGetDQGReports(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDQGReportWithDetails(long DQGReportID, AsyncCallback callback, object state);
        DQGReport EndGetDQGReportWithDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDQGReportAllDetails(long DQGReportID, AsyncCallback callback, object state);
        DataSet EndGetDQGReportAllDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginTransferFastReport(long FastReportID, AsyncCallback callback, object state);
        bool EndTransferFastReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteFastReport(long FastReportID, AsyncCallback callback, object state);
        bool EndDeleteFastReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteFastLinkedReportDetail(int Case, long FastReportID, string so_ct, bool noi_tru, string ma_kh, string ma_bp, string ma_kho, AsyncCallback callback, object state);
        void EndDeleteFastLinkedReportDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllPositionInHospital(AsyncCallback callback, object state);
        IList<PositionInHospital> EndGetAllPositionInHospital(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXmlReportForCongDLQG(DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        byte[] EndGetXmlReportForCongDLQG(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMarkHIReportByVAS(long HIReportID, AsyncCallback callback, object state);
        bool EndMarkHIReportByVAS(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTreatmentStatisticsByDepartment(long DeptID, DateTime FromDate, DateTime ToDate, byte RegistrationStatus, AsyncCallback callback, object state);
        IList<DiseasesReference> EndGetTreatmentStatisticsByDepartment(out decimal OutTotalQuantity, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTreatmentStatisticsByDepartmentDetail(long DeptID, DateTime FromDate, DateTime ToDate, byte RegistrationStatus
            , decimal? FilerByTime
            , decimal? FilerByMoney
            , AsyncCallback callback, object state);
        IList<PatientRegistration> EndGetTreatmentStatisticsByDepartmentDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportRegistrationsData(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, AsyncCallback callback, object state);
        byte[] EndExportRegistrationsData(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcelForHIReport(ReportParameters RptParameters, int PatientTypeIndex, long staffID, AsyncCallback callback, object state);
        byte[] EndExportToExcelForHIReport(IAsyncResult asyncResult);
        //▼====: #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCreateDTDTReportOutInPt(DTDTReport DTDTReport, AsyncCallback callback, object state);
        bool EndCreateDTDTReportOutInPt(out long DTDTReportID, out long DTDTReportOutPt, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDTDTData(long nDTDTReportID, AsyncCallback callback, object state);
        List<DTDT_don_thuoc> EndGetDTDTData(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDTDT_InPtData(long nDTDTReportID, AsyncCallback callback, object state);
        List<DTDT_don_thuoc> EndGetDTDT_InPtData(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateDTDTReportStatus(DTDTReport aDTDTReport, AsyncCallback callback, object state);
        bool EndUpdateDTDTReportStatus(IAsyncResult asyncResult);

        //▲====: #003
        //▼====: #004
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteRegistrationDTDTReport(string ListPrescription, AsyncCallback callback, object state);
        bool EndDeleteRegistrationDTDTReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteDTDTReport(long DTDTReportID, AsyncCallback callback, object state);
        bool EndDeleteDTDTReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteRegistrationDTDT_InPtReport(string ListPrescription, AsyncCallback callback, object state);
        bool EndDeleteRegistrationDTDT_InPtReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteDTDT_InPtReport(long DTDTReportID, AsyncCallback callback, object state);
        bool EndDeleteDTDT_InPtReport(IAsyncResult asyncResult);
        //▲====: #004
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCancelConfirmDTDTReport(long PtRegistrationID, long V_RegistrationType, long StaffID, string CancelReason, AsyncCallback callback, object state);
        bool EndCancelConfirmDTDTReport(IAsyncResult asyncResult);
        //▼====: #005
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreateDQGReportOutInpt(DQGReport aDQGReport, AsyncCallback callback, object state);
        bool EndCreateDQGReportOutInpt(out long DQGReportID, out long DQGReportIDInpt, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteDQGReportOutInpt(long PtRegistrationID, long IssueID, long DQGReportID, long StaffID, string CancelReason, long V_RegistrationType, AsyncCallback callback, object state);
        bool EndDeleteDQGReportOutInpt(IAsyncResult asyncResult);
        //▲====: #005
        //▼====: #006
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCreateHIReport_130_OutInPt(HealthInsuranceReport HIReport, AsyncCallback callback, object state);
        bool EndCreateHIReport_130_OutInPt(out long HIReportID, out long HIReportOutPt, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetHIXmlReport_130_AllTab_InOneRpt(long nHIReportID, AsyncCallback callback, object state);
        byte[] EndGetHIXmlReport_130_AllTab_InOneRpt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetHIXmlReport_130_CheckIn_OutPt(long PtRegistrationID, AsyncCallback callback, object state);
        byte[] EndGetHIXmlReport_130_CheckIn_OutPt(IAsyncResult asyncResult);
        //▲====: #006
    }
}
