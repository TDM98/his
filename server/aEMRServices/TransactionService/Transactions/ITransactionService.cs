using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using System.Xml.Linq;
using ErrorLibrary;
using System.IO;
using System.Data;
/*
 * 20190326 #001 TNHX: Add function to export excel in ConfirmHIRegistrationView
 * 20210323 #002 BLQ: #243 Lấy HIReport cho KHTH
 * 20221216 #003 TNHX: 994 Response cua API đơn thuốc điện tử
 * 20230108 #004 THNHX:  944 Thêm func xóa dữ liệu đẩy đơn thuốc điện tử không thành công
 * 20230220 #005 QTD:  Xoá đơn thuốc quốc gia nhà thuốc
 * 20230314 #006 BLQ: Thêm hàm tạo xml đẩy cổng BH theo 130
 */
namespace TransactionService.Transactions
{
    [ServiceContract]
    public interface ITransactionService
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientTransaction> GetTransaction_ByPtID(SeachPtRegistrationCriteria SearchCriteria, int PageSize, int PageIndex, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertDataToReport(string begindate, string enddate);

        //export excel all
        #region Export excel from database

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportFromExcel(ReportParameters criteria, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportToExcellAllGeneric(ReportParameters criteria, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportToExcellAllGeneric_New(ReportParameters criteria, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportToCSVAllGeneric(ReportParameters criteria, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportEInvoiceToExcel(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportHospitalClientContractDetails_Excel(long HosClientContractID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportHospitalClientContractResultDetails_Excel(long HosClientContractID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_Temp25a(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        string ExportToExcellAll_Temp25aNew(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_Temp26a(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_Temp20NgoaiTru(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_Temp20NoiTru(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_Temp21NgoaiTru(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_Temp21NoiTru(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_ChiTietVienPhi_PK(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_ChiTietVienPhi(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_ThongKeDoanhThu(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_DoanhThuTongHop(ReportParameters criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_DoanhThuNoiTruTongHop(ReportParameters criteria);

        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool FollowUserGetReport(long staffID, string reportName, string reportParams, long v_GetReportMethod);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateHIReport(HealthInsuranceReport HIReport);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateFastReport(HealthInsuranceReport gReport,long V_FastReportType, out long FastReportID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool TransferFastReport(long FastReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteFastReport(long FastReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DeleteFastLinkedReportDetail(int Case, long FastReportID, string so_ct, bool noi_tru, string ma_kh, string ma_bp, string ma_kho);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRegistrationHIReport(string ma_lk);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegistrationsForHIReport(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration_V2> SearchRegistrationsForCreateOutPtTransactionFinalization(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutPtTransactionFinalization> GetTransactionFinalizationSummaryInfos(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration_V2> SearchRegistrationsForCreateEInvoices(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateHIReportStatus(HealthInsuranceReport aHealthInsuranceReport);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateHIReport_V2(HealthInsuranceReport HIReport, out long HIReportID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateHIReportOutInPt(HealthInsuranceReport HIReport, out long HIReportID, out long HIReportOutPt);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<HealthInsuranceReport> GetHIReport();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<HealthInsuranceReport> GetFastReports(long V_FastReportType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DataSet GetFastReportDetails(long FastReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetHIXmlReport9324_AllTab123_InOneRpt(long nHIReportID);
        /*▼====: #001*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        HOSPayment EditHOSPayment(HOSPayment aHOSPayment);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<HOSPayment> GetHOSPayments(DateTime aStartDate, DateTime aEndDate);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteHOSPayment(long aHOSPaymentID);
        /*▲====: #001*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        DataSet PreviewHIReport(long PtRegistrationID, long V_RegistrationType, long? OutPtTreatmentProgramID, out string ErrText);
        //▼====: #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        DataSet PreviewHIReport_ForKHTH(long PtRegistrationID, long V_RegistrationType,out string ErrText);
        //▲====: #002
        //▼====: #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportExcelRegistrationsForHIReportWaitConfirm(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportExcelRegistrationsForHIReportOther(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);
        //▲====: #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RptOutPtTransactionFinalizationDetail> GetRptOutPtTransactionFinalizationDetails(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DQGReport CreateDQGReport(DQGReport aDQGReport);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateCodeDQGReport(DQGReport aDQGReport, byte aCase);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DQGReport> GetDQGReports();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DQGReport GetDQGReportWithDetails(long DQGReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DataSet GetDQGReportAllDetails(long DQGReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PositionInHospital> GetAllPositionInHospital();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXmlReportForCongDLQG(DateTime FromDate, DateTime ToDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool MarkHIReportByVAS(long HIReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiseasesReference> GetTreatmentStatisticsByDepartment(long DeptID, DateTime FromDate, DateTime ToDate, byte RegistrationStatus, out decimal OutTotalQuantity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> GetTreatmentStatisticsByDepartmentDetail(long DeptID, DateTime FromDate, DateTime ToDate, byte RegistrationStatus
            , decimal? FilerByTime
            , decimal? FilerByMoney);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportRegistrationsData(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportToExcelForHIReport(ReportParameters RptParameters, int PatientTypeIndex, long staffID);

        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateDTDTReportOutInPt(DTDTReport DTDTReport, out long DTDTReportID, out long DTDTReportOutPt);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DTDT_don_thuoc> GetDTDTData(long nDTDTReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DTDT_don_thuoc> GetDTDT_InPtData(long nDTDTReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDTDTReportStatus(DTDTReport aDTDTReport);
        //▲====: #003
        //▼====: #004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRegistrationDTDTReport(string ListPrescription);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteDTDTReport(long DTDTReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRegistrationDTDT_InPtReport(string ListPrescription);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteDTDT_InPtReport(long DTDTReportID);
        //▲====: #004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CancelConfirmDTDTReport(long PtRegistrationID, long V_RegistrationType, long StaffID, string CancelReason);

        //▼====: #005
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateDQGReportOutInpt(DQGReport aDQGReport, out long DQGReportID, out long DQGReportIDInpt);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteDQGReportOutInpt(long PtRegistrationID, long IssueID, long DQGReportID, long StaffID, string CancelReason, long V_RegistrationType);
        //▲====: #005
        //▼====: #006
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateHIReport_130_OutInPt(HealthInsuranceReport HIReport, out long HIReportID, out long HIReportOutPt);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetHIXmlReport_130_AllTab_InOneRpt(long nHIReportID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetHIXmlReport_130_CheckIn_OutPt(long PtRegistrationID);
        //▲====: #006
    }
}
