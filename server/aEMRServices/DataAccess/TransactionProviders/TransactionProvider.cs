using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using eHCMS.Configurations;
using System.Xml.Linq;
using System.IO;

using eHCMS.Services.Core;

namespace eHCMS.DAL
{
    public abstract class TransactionProvider : DataProviderBase
    {
        static private TransactionProvider _instance = null;
        static public TransactionProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Transactions.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type[] types = assem.GetExportedTypes();
                    Type t = assem.GetType(Globals.Settings.Transactions.ProviderType);
                    _instance = (TransactionProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }
         public TransactionProvider()
        {
            this.ConnectionString = Globals.Settings.Transactions.ConnectionString;

        }

        #region 1. Transaction member
        public abstract List<PatientTransaction> GetTransaction_ByPtID(SeachPtRegistrationCriteria SearchCriteria,int PageSize,int PageIndex,bool CountTotal, out int Total);
   
        #endregion

        #region 2. InsertData Report
        public abstract bool InsertDataToReport(string begindate,string enddate);
        #endregion

        //export excel all
        #region Export excel from database

        public abstract List<List<string>> ExportToExcellAllGeneric(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcel_HIReport(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_Temp25a(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_Temp26a(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_Temp20NgoaiTru(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_Temp20NoiTru(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_Temp21NgoaiTru(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_Temp21NoiTru(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_ChiTietVienPhi_PK(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_ChiTietVienPhi(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_ThongKeDoanhThu(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_DoanhThuTongHop(ReportParameters criteria);

        public abstract List<List<string>> ExportToExcellAll_DoanhThuNoiTruTongHop(ReportParameters criteria);
        #endregion

        public abstract bool FollowUserGetReport(long staffID, string reportName, string reportParams, long v_GetReportMethod);

        public abstract bool CreateHIReport(HealthInsuranceReport HIReport, out long HIReportID);
        public abstract bool CreateFastReport(HealthInsuranceReport gReport, out long FastReportID);
        public abstract bool DeleteRegistrationHIReport(string ma_lk);
        public abstract IList<PatientRegistration> SearchRegistrationsForHIReport(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);
        public abstract IList<PatientRegistration> SearchRegistrationsForCreateOutPtTransactionFinalization(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);
        public abstract IList<OutPtTransactionFinalization> GetTransactionFinalizationSummaryInfos(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria);
        public abstract bool UpdateHIReportStatus(HealthInsuranceReport aHealthInsuranceReport);

        public abstract List<HealthInsuranceReport> GetHIReport();
        public abstract List<HealthInsuranceReport> GetFastReports();
        public abstract DataSet GetFastReportDetails(long FastReportID);

        public abstract Stream GetHIXmlReport9324_AllTab123_InOneRpt_Data(long nHIReportID);

        //
        /*▼====: #002*/
        #region HOSPayments
        public abstract HOSPayment EditHOSPayment(HOSPayment aHOSPayment);
        public abstract bool DeleteHOSPayment(long aHOSPaymentID);
        public abstract IList<HOSPayment> GetHOSPayments(DateTime aStartDate, DateTime aEndDate);
        #endregion
        /*▲====: #002*/
        public abstract DataSet PreviewHIReport(long PtRegistrationID, long V_RegistrationType, out string ErrText);
    }
}
