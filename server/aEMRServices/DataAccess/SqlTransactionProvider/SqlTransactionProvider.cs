using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Service.Core.Common;
using System.Diagnostics;
using eHCMS.Configurations;
/*
 * 20170917 #001 CMN: Add TotalInPtRevenue Report
 * 20171019 #002 CMN: Fixed Payment Percent On Create TechnicalServiceAndMedicalMaterial XML HI Report
 * 20180104 #004 CMN: Added new version for format XML of HI Reports
 * 20180906 #005 TBL: Xuat excel mau 79a-80a theo dinh dang 3360 
*/
namespace eHCMS.DAL
{
    public class SqlTransactionProvider:TransactionProvider
    {
        public SqlTransactionProvider()
            : base()
        {

        }
        #region 1. Transaction member
        protected override PatientTransaction GetPatientTransactionFromReader(IDataReader reader)
        {
            PatientTransaction p = base.GetPatientTransactionFromReader(reader);
            p.PatientRegistration=new PatientRegistration();
            if (reader.HasColumn("PtRegistrationID"))
            {
                p.PtRegistrationID = reader["PtRegistrationID"] as long?;
                if (reader["PtRegistrationID"] != DBNull.Value)
                {
                    p.PatientRegistration.PtRegistrationID = (long)reader["PtRegistrationID"];
                }
            }
            if (reader.HasColumn("PtRegistrationCode"))
            {
                p.PatientRegistration.PtRegistrationCode = reader["PtRegistrationCode"].ToString();
            }
            if (reader.HasColumn("FullName"))
            {
                p.PatientRegistration.FullName = reader["FullName"].ToString();
            }
            if (reader.HasColumn("PatientCode"))
            {
                p.PatientRegistration.PatientCode = reader["PatientCode"].ToString();
            }
            if (reader.HasColumn("PatientID") && reader["PatientID"] != DBNull.Value)
            {
                p.PatientRegistration.PatientID = (long)reader["PatientID"];
            }
            return p;
        }
        public override List<PatientTransaction> GetTransaction_ByPtID(SeachPtRegistrationCriteria SearchCriteria,int PageSize,int PageIndex,bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientRegistration_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter paramPatientName = new SqlParameter("@FullName", SqlDbType.NVarChar);
                paramPatientName.Value =ConvertNullObjectToDBNull(SearchCriteria.FullName);
                SqlParameter paramPatientCode = new SqlParameter("@PatientCode", SqlDbType.VarChar);
                paramPatientCode.Value = ConvertNullObjectToDBNull(SearchCriteria.PatientCode);
                SqlParameter paramFromDate = new SqlParameter("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = ConvertNullObjectToDBNull(SearchCriteria.FromDate);
                SqlParameter paramToDate = new SqlParameter("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = ConvertNullObjectToDBNull(SearchCriteria.ToDate);

                SqlParameter paramNgoaiTru = new SqlParameter("@IsNgoaiTru", SqlDbType.Bit);
                paramNgoaiTru.Value = ConvertNullObjectToDBNull(SearchCriteria.IsNgoaiTru);

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.Int);
                paramPageSize.Value = PageSize;
                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.Int);
                paramPageIndex.Value = PageIndex;
                SqlParameter paramCountTotal = new SqlParameter("@CountTotal", SqlDbType.Bit);
                paramCountTotal.Value = CountTotal;
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramPatientName);
                cmd.Parameters.Add(paramPatientCode);
                cmd.Parameters.Add(paramFromDate);
                cmd.Parameters.Add(paramToDate);
                cmd.Parameters.Add(paramNgoaiTru);

                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);
                cmd.Parameters.Add(paramCountTotal);
                cmd.Parameters.Add(paramTotal);

                cn.Open();
                List<PatientTransaction> lst = null;

                IDataReader reader = ExecuteReader(cmd);

                lst = GetPatientTransactionCollectionFromReader(reader);
                reader.Close();

                if (CountTotal && paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
                return lst;
            }
        }
        #endregion

        #region 2. InsertData Report
        public override bool InsertDataToReport(string begindate, string enddate)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHIPremiumPaymentDetails_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@begindate", SqlDbType.VarChar, begindate);
                cmd.AddParameter("@enddate", SqlDbType.VarChar, enddate);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    results = true;
                }
                reader.Close();
            }
            return results;
        }
        #endregion

        
        //export excel all
        #region Export excel from database
        private List<List<string>> getReportString(string storeName, ReportParameters criteria) 
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand(storeName, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                if (criteria.reportName == ReportName.RptTongHopDoanhThu
                    || criteria.reportName == ReportName.RptTongHopDoanhThuNoiTru)
                {
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                    if (criteria.reportName == ReportName.RptTongHopDoanhThu)
                    {
                        cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocID));
                    }
                }
                else if (criteria.reportName == ReportName.CONSULTINGDIAGNOSYSHISTORY)
                {
                    cmd.Parameters.Clear();
                    cmd.AddParameter("@IsApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsApproved));
                    cmd.AddParameter("@IsLated", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsLated));
                    cmd.AddParameter("@IsAllExamCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsAllExamCompleted));
                    cmd.AddParameter("@IsSurgeryCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsSurgeryCompleted));
                    cmd.AddParameter("@IsWaitSurgery", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsWaitSurgery));
                    cmd.AddParameter("@IsDuraGraft", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsDuraGraft));
                    cmd.AddParameter("@IsWaitingExamCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsWaitingExamCompleted));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.ToDate));
                }
                else if (criteria.reportName == ReportName.FollowICD)
                {
                    cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                }
                else if (criteria.reportName == ReportName.EmployeesReport)
                {
                    cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                    cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                }
                else if (criteria.reportName == ReportName.FINANCIAL_ACTIVITY_TEMP3)
                {
                }
                else
                {
                    cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                    cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                    cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                    cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));

                    if (criteria.reportName == ReportName.TEMP26a_CHITIET || criteria.reportName == ReportName.TEMP80a_CHITIET)
                    {
                        cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                        cmd.AddParameter("@NotTreatedAsInPt", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.NotTreatedAsInPt));
                    }

                    if (criteria.reportName == ReportName.REPORT_GENERAL_TEMP02 || criteria.reportName == ReportName.THEODOIHANGXUATKYGOI)
                    {
                        cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                    }
                    if (criteria.reportName == ReportName.RptDrugMedDept || criteria.reportName == ReportName.THEODOIHANGXUATKYGOI)
                    {
                        cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_MedProductType));
                    }
                }
                cmd.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                string newGuid = criteria.StoreName;

                if (dsExportToExcellAll.Tables.Count == 0)
                    return null;

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);

                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }

                return returnAllExcelData;
            }
        }
        private List<List<string>> GetOutwardDrugsByStaffStatisticReportString(string storeName, ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand(storeName, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                string newGuid = criteria.StoreName;

                if (dsExportToExcellAll.Tables.Count == 0)
                    return null;

                DataTable mTable = new DataTable();
                mTable.Columns.Add("DrugID", typeof(string));
                mTable.Columns.Add("BrandName", typeof(string));
                List<string[]> mDrugsContents = new List<string[]>();
                foreach (var item in dsExportToExcellAll.Tables[0].Rows.Cast<DataRow>().OrderBy(x => x["BrandName"]))
                {
                    if (mDrugsContents.Any(x => x[0] == item["DrugID"].ToString() && x[2] == item["InCost"].ToString() && x[3] == item["OutPrice"].ToString()))
                    {
                        continue;
                    }
                    mDrugsContents.Add(new string[] { item["DrugID"].ToString(), item["BrandName"].ToString()
                        , item["InCost"] == null || item["InCost"] == DBNull.Value ? "0" : item["InCost"].ToString()
                        , item["OutPrice"] == null || item["OutPrice"] == DBNull.Value ? "0" : item["OutPrice"].ToString()});
                }
                List<string[]> mStaffsContents = new List<string[]>();
                foreach (var item in dsExportToExcellAll.Tables[0].Rows.Cast<DataRow>().OrderBy(x => x["StaffName"]))
                {
                    if (mStaffsContents.Any(x => x[0] == item["StaffID"].ToString()))
                    {
                        continue;
                    }
                    mStaffsContents.Add(new string[] { item["StaffID"].ToString(), item["StaffName"].ToString() });
                    mTable.Columns.Add(item["StaffName"].ToString(), typeof(double));
                    mTable.Columns.Add("Thành tiền" + mStaffsContents.Count.ToString(), typeof(decimal));
                }
                mTable.Columns.Add("InCost", typeof(decimal));
                mTable.Columns.Add("OutPrice", typeof(decimal));
                foreach (var item in mDrugsContents)
                {
                    var mRow = mTable.NewRow();
                    mRow["DrugID"] = item[0];
                    mRow["BrandName"] = item[1];
                    mRow["InCost"] = Convert.ToDecimal(item[2]);
                    mRow["OutPrice"] = Convert.ToDecimal(item[3]);
                    mTable.Rows.Add(mRow);
                }
                int mSkipColumnNumb = 2;
                foreach (var item in dsExportToExcellAll.Tables[0].Rows.Cast<DataRow>().OrderBy(x => x["DrugID"]))
                {
                    var mColumnIndex = mStaffsContents.IndexOf(mStaffsContents.FirstOrDefault(x => x[0] == item["StaffID"].ToString())) * 2;
                    var mCurrentQty = mTable.Rows.Cast<DataRow>().FirstOrDefault(x => x["DrugID"].ToString() == item["DrugID"].ToString() && x["InCost"].ToString() == item["InCost"].ToString() && x["OutPrice"].ToString() == item["OutPrice"].ToString())[mColumnIndex + mSkipColumnNumb];
                    var mCurrentAmount = mTable.Rows.Cast<DataRow>().FirstOrDefault(x => x["DrugID"].ToString() == item["DrugID"].ToString() && x["InCost"].ToString() == item["InCost"].ToString() && x["OutPrice"].ToString() == item["OutPrice"].ToString())[mColumnIndex + mSkipColumnNumb + 1];
                    mTable.Rows.Cast<DataRow>().FirstOrDefault(x => x["DrugID"].ToString() == item["DrugID"].ToString() && x["InCost"].ToString() == item["InCost"].ToString() && x["OutPrice"].ToString() == item["OutPrice"].ToString())[mColumnIndex + mSkipColumnNumb] = Convert.ToDouble(item["OutQuantity"]) + Convert.ToDouble(mCurrentQty == null || mCurrentQty == DBNull.Value ? 0 : mCurrentQty);
                    mTable.Rows.Cast<DataRow>().FirstOrDefault(x => x["DrugID"].ToString() == item["DrugID"].ToString() && x["InCost"].ToString() == item["InCost"].ToString() && x["OutPrice"].ToString() == item["OutPrice"].ToString())[mColumnIndex + mSkipColumnNumb + 1] = Convert.ToDecimal(item["OutAmount"]) + Convert.ToDecimal(mCurrentAmount == null || mCurrentAmount == DBNull.Value ? 0 : mCurrentAmount);
                }
                mTable.Columns.RemoveAt(0);
                mTable.Columns["BrandName"].ColumnName = "Tên thuốc";
                mTable.Columns["InCost"].ColumnName = "Giá vốn";
                mTable.Columns["OutPrice"].ColumnName = "Giá bán";

                List<string> colname = new List<string>();
                for (int i = 0; i <= mTable.Columns.Count - 1; i++)
                {
                    colname.Add(mTable.Columns[i].ToString().Trim());
                }
                returnAllExcelData.Add(colname);
                for (int i = 0; i <= mTable.Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= mTable.Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(mTable.Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAllGeneric(ReportParameters criteria)
        {
            switch (criteria.reportName)
            {
                case ReportName.TEMP25a_CHITIET:
                    return getReportString("spRpt_CreateTemplate25a_Excel", criteria);
                case ReportName.TEMP25aTRATHUOC_CHITIET:
                    return getReportString("spRpt_CreateTemplate25aTraThuoc_Excel", criteria);
                case ReportName.TEMP26a_CHITIET:
                    return getReportString("spRpt_CreateTemplate26a_Excel", criteria);
                case ReportName.TEMP20_NGOAITRU:
                    return getReportString("spRpt_CreateTemp20NgoaiTru_Excel", criteria);
                case ReportName.TEMP20_NGOAITRU_TRATHUOC:
                    return getReportString("spRpt_CreateTemp20NgoaiTruTraThuoc_Excel", criteria);    
                case ReportName.TEMP20_NOITRU:
                    return getReportString("spRpt_CreateTemp20NoiTru_Excel", criteria);
                case ReportName.TEMP20_VTYTTH:
                    return getReportString("spRpt_CreateTemp20VTYTTH_Excel", criteria);       
                case ReportName.TEMP21_NGOAITRU:
                    return getReportString("spRpt_CreateTemp21NgoaiTru_Excel", criteria);                        
                case ReportName.TEMP21_NOITRU:
                    return getReportString("spRpt_CreateTemp21NoiTru_Excel", criteria);
                case ReportName.TEMP19:
                    return getReportString("spRpt_CreateTemp19VTYTTH_New_Excel", criteria);
                case ReportName.TEMP20_NOITRU_NEW:
                    return getReportString("spRpt_CreateTemp20NoiTru_New_Excel", criteria);
                case ReportName.TEMP21_NEW:
                    return getReportString("spRpt_CreateTemp21_New_Excel", criteria);
                /*▼====: #005*/
                //case ReportName.TEMP79a_CHITIET:
                //    return getReportString("spRpt_CreateTemplate79a_Excel", criteria);
                case ReportName.TEMP79a_CHITIET:
                case ReportName.TEMP79a_TONGHOP:
                    {
                        if (criteria.IsFullDetails && !criteria.IsDetail)
                        {
                            return getReportString("spRpt_CreateTemplate79a_Excel_BV", criteria);
                        }
                        else if (criteria.IsFullDetails && criteria.IsDetail)
                        {
                            return getReportString("spRpt_CreateTemplate79a_Excel_Details_BV", criteria);
                        }
                        else if (criteria.Check3360)
                        {
                            return getReportString("spRpt_CreateTemplate79a_3360_Excel", criteria);
                        }
                        else
                        {
                            return getReportString("spRpt_CreateTemplate79a_Excel", criteria);
                        }
                    }
                /*▲====: #005*/
                case ReportName.TEMP79aTRATHUOC_CHITIET:
                    return getReportString("spRpt_CreateTemplate79aTraThuoc_Excel", criteria);
                /*▼====: #005*/
                //case ReportName.TEMP80a_CHITIET:
                //    return getReportString("spRpt_CreateTemplate80a_Excel", criteria);
                case ReportName.TEMP80a_CHITIET:
                case ReportName.TEMP80a_TONGHOP:
                    {
                        if (criteria.IsFullDetails && !criteria.IsDetail)
                        {
                            return getReportString("spRpt_CreateTemplate80a_Excel_BV", criteria);
                        }
                        else if (criteria.IsFullDetails && criteria.IsDetail)
                        {
                            return getReportString("spRpt_CreateTemplate80a_Excel_Details_BV", criteria);
                        }
                        else if (criteria.Check3360)
                        {
                            return getReportString("spRpt_CreateTemplate80a_3360_Excel", criteria);
                        }
                        else
                        {
                            return getReportString("spRpt_CreateTemplate80a_Excel", criteria);
                        }
                    }
                /*▲====: #005*/
                case ReportName.TRANSACTION_VIENPHICHITIET_PK:
                    return getReportString("spRpt_BaoCaoChiTietVienPhi_PK_Excel", criteria);                        
                case ReportName.TRANSACTION_VIENPHICHITIET:
                    return getReportString("spRpt_BaoCaoChiTietVienPhi_Excel", criteria);
                        
                case ReportName.THONGKEDOANHTHU:
                    return getReportString("spRpt_ThongKeDoanhThu_Excel", criteria);                        

                case ReportName.RptTongHopDoanhThu:
                    return getReportString("spRptDoanhThuTongHop_Excel", criteria);                        
                //==== #001
                //case ReportName.RptTongHopDoanhThuNoiTru:
                //    return getReportString("spRptDoanhThu_NoiTru_TongHop_Excel", criteria);
                case ReportName.RptTongHopDoanhThuNoiTru:
                    return getReportString("spRptTotalInPtRevenue_Excel", criteria);
                //==== #001
                case ReportName.REPORT_INPATIENT:
                    return getReportString("sp_RptInPtCaredForAndDischarged_AdminOffice", criteria);
                case ReportName.REPORT_GENERAL_TEMP02:
                    return getReportString("sp_RptGeneralTemp02", criteria);

                case ReportName.HOSPITAL_FEES_REPORT:
                    return getReportString("sp_Report_HospitalFees", criteria);
                case ReportName.HIGH_TECH_FEES_REPORT:
                    return getReportString("sp_Report_HighTechFees", criteria);
                case ReportName.OUT_MEDICAL_MATERIAL_REPORT:
                    return getReportString("sp_Report_HighTechMat", criteria);
                case ReportName.HIGH_TECH_FEES_CHILD_REPORT:
                    return getReportString("sp_Report_HospitalFees_Child", criteria);
                case ReportName.CONSULTINGDIAGNOSYSHISTORY:
                    return getReportString("spRptGetConsultingDiagnosys_Excel", criteria);
                case ReportName.FollowICD:
                    return getReportString("spGetHISSummaryByICD_Excel", criteria);
                case ReportName.EmployeesReport:
                    return getReportString("spHISStaffInfos_Excel", criteria);
                /*TMA 06/10/2017 PHỤ LỤC 2B*/
                case ReportName.TransferFormType2Rpt:
                    return getReportString("spTransferFormType2Rpt", criteria);
                /*TMA 09/10/2017 PHỤ LỤC 2A*/
                case ReportName.TransferFormType2_1Rpt:
                    return getReportString("spTransferFormType2_1Rpt", criteria);
                /*TMA 12/10/2017 PHỤ LỤC 5*/
                case ReportName.TransferFormType5Rpt:
                    return getReportString("spTransferFormType5Rpt", criteria);
                /*TMA 24/10/2017 Sổ kiểm nhập - khoa dược*/
                case ReportName.RptDrugMedDept:
                    return getReportString("spSoKiemNhapThuoc_KhoaDuoc_ExporttoExcel", criteria);
                /*▼====: #003*/
                case ReportName.THEODOIHANGXUATKYGOI:
                    return getReportString("spRptDrugDeptTempInwardReport_ExporttoExcel", criteria);
                /*▲====: #003*/
                case ReportName.TRANSFERFORMDATA:
                    return getReportString("spExportTransferFormData", criteria);
                case ReportName.FINANCIAL_ACTIVITY_TEMP3:
                    return getReportString("spFinancialActivityTemp03_Excel", criteria);
                case ReportName.REPORT_IMPORT_EXPORT_DEPARTMENT:
                    return getReportString("spRpt_InPatientImportExportDepartment_Detail_Excel", criteria);
                case ReportName.OutwardDrugsByStaffStatistic:
                    return GetOutwardDrugsByStaffStatisticReportString("spOutwardDrugsByStaffStatistic", criteria);
                default: return null;
            }

        }

        private List<List<string>> getReportData(string storeName, ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand(storeName, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.HIReportID));

                cmd.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                string newGuid = criteria.StoreName;

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);

                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }

                return returnAllExcelData;
            }
        }


        public override List<List<string>> ExportToExcel_HIReport(ReportParameters criteria)
        {
            switch (criteria.reportName)
            {
                case ReportName.TEMP19_V2:
                    return getReportData("spRpt_CreateTemp19VTYTTH_New_Excel_V2", criteria);
                case ReportName.TEMP20_V2:
                    return getReportData("spRpt_CreateTemp20NoiTru_New_Excel_V2", criteria);
                case ReportName.TEMP21_V2:
                    return getReportData("spRpt_CreateTemp21_New_Excel_V2", criteria);
                case ReportName.TEMP79_V2:
                    return getReportData("spRpt_CreateTemplate79a_Excel_V2", criteria);
                case ReportName.TEMP79_TRATHUOC_V2:
                    return getReportData("spRpt_CreateTemplate79aTraThuoc_Excel_V2", criteria);
                case ReportName.TEMP80_V2:
                    return getReportData("spRpt_CreateTemplate80a_Excel_V2", criteria);
                case ReportName.TEMP9324_BANG_1:
                    return getReportData("spGetReport9324_General", criteria);
                case ReportName.TEMP9324_BANG_2:
                    return getReportData("spGetReport9324_Drug", criteria);
                case ReportName.TEMP9324_BANG_3:
                    return getReportData("spGetReport9324_TechnicalServiceAndMedicalMaterial", criteria);
                default: return null;
            }

        }



        public override List<List<string>> ExportToExcellAll_Temp25a(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRpt_CreateTemplate25a_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                cmd.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                string newGuid = criteria.StoreName;

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAll_Temp26a(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRpt_CreateTemplate26a_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }


        public override List<List<string>> ExportToExcellAll_Temp20NgoaiTru(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRpt_CreateTemp20NgoaiTru_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAll_Temp20NoiTru(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRpt_CreateTemp20NoiTru_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAll_Temp21NgoaiTru(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRpt_CreateTemp21NgoaiTru_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAll_Temp21NoiTru(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRpt_CreateTemp21NoiTru_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAll_ChiTietVienPhi_PK(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRpt_BaoCaoChiTietVienPhi_PK_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAll_ChiTietVienPhi(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRpt_BaoCaoChiTietVienPhi_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAll_ThongKeDoanhThu(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRpt_ThongKeDoanhThu_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAll_DoanhThuTongHop(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRptDoanhThuTongHop_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocID));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        public override List<List<string>> ExportToExcellAll_DoanhThuNoiTruTongHop(ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRptDoanhThu_NoiTru_TongHop_Excel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }
                return returnAllExcelData;
            }
        }

        #endregion

        public override bool FollowUserGetReport(long staffID, string reportName, string reportParams, long v_GetReportMethod)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spInsertFollowUserGetReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, staffID);
                    cmd.AddParameter("@ReportName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(reportName));
                    cmd.AddParameter("@Parameters", SqlDbType.NVarChar, ConvertNullObjectToDBNull(reportParams));
                    cmd.AddParameter("@V_GetReportMethod", SqlDbType.BigInt, v_GetReportMethod);

                    cn.Open();
                    int count = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override bool CreateHIReport(HealthInsuranceReport HIReport, out long HIReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCreateHIReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@Title", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HIReport.Title));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(HIReport.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(HIReport.ToDate));
                    cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(HIReport.Quarter));
                    cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(HIReport.Month));
                    cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(HIReport.Year));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, HIReport.Staff != null ? HIReport.Staff.StaffID : 0);
                    cmd.AddParameter("@IncludeBeforeFromDate", SqlDbType.Bit, ConvertNullObjectToDBNull(HIReport.IncludeBeforeFromDate));
                    cmd.AddParameter("@V_HIReportType", SqlDbType.BigInt, HIReport.V_HIReportType != null ? HIReport.V_HIReportType.LookupID : 0);
                    cmd.AddParameter("@RegistrationIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(HealthInsuranceReport.ConvertIDListToXml(HIReport.RegistrationIDList)));
                    cmd.AddParameter("@HIReportID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@V_ReportStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(HIReport.V_ReportStatus));
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();

                    if (cmd.Parameters["@HIReportID"].Value != null && cmd.Parameters["@HIReportID"].Value != DBNull.Value)
                        HIReportID = (long)cmd.Parameters["@HIReportID"].Value;
                    else HIReportID = 0;

                    cmd.Dispose();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override bool CreateFastReport(HealthInsuranceReport gReport, out long FastReportID)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spCreateFastReport", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandTimeout = int.MaxValue;
                    mCommand.AddParameter("@Title", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gReport.Title));
                    mCommand.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(gReport.FromDate));
                    mCommand.AddParameter("@StaffID", SqlDbType.BigInt, gReport.Staff != null ? gReport.Staff.StaffID : 0);
                    mCommand.AddParameter("@FastReportID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    mConnection.Open();
                    int ExcCount = mCommand.ExecuteNonQuery();
                    if (mCommand.Parameters["@FastReportID"].Value != null && mCommand.Parameters["@FastReportID"].Value != DBNull.Value)
                        FastReportID = (long)mCommand.Parameters["@FastReportID"].Value;
                    else FastReportID = 0;
                    mCommand.Dispose();
                    return ExcCount > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override bool DeleteRegistrationHIReport(string ma_lk)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteRegistrationHIReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@ma_lk", SqlDbType.VarChar, ConvertNullObjectToDBNull(ma_lk));
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override IList<PatientRegistration> SearchRegistrationsForHIReport(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSearchRegistrationsForHIReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ToDate));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptLocationID));
                    cmd.AddParameter("@IsReported", SqlDbType.Bit, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.IsReported));
                    cmd.AddParameter("@V_TreatmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.V_TreatmentType));

                    cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FullName));
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PatientCode));
                    cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PtRegistrationCode));

                    cn.Open();
                    var mReader = cmd.ExecuteReader();
                    List<PatientRegistration> mPatientRegistrationCollection = GetPatientRegistrationCollectionFromReader(mReader);
                    mReader.Close();
                    cmd.Dispose();
                    return mPatientRegistrationCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override IList<PatientRegistration> SearchRegistrationsForCreateOutPtTransactionFinalization(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSearchRegistrationsForCreateOutPtTransactionFinalization", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ToDate));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptLocationID));
                    cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FullName));
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PatientCode));
                    cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PtRegistrationCode));

                    cn.Open();
                    var mReader = cmd.ExecuteReader();
                    List<PatientRegistration> mPatientRegistrationCollection = GetPatientRegistrationCollectionFromReader(mReader);
                    mReader.Close();
                    cmd.Dispose();
                    return mPatientRegistrationCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override IList<OutPtTransactionFinalization> GetTransactionFinalizationSummaryInfos(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetTransactionFinalizationSummaryInfos", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ToDate));
                    cn.Open();
                    var mReader = cmd.ExecuteReader();
                    var retval = GetOutPtTransactionFinalizationCollectionFromReader(mReader);
                    mReader.Close();
                    if (retval == null)
                    {
                        return null;
                    }
                    return retval;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override bool UpdateHIReportStatus(HealthInsuranceReport aHealthInsuranceReport)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateHIReportStatus", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aHealthInsuranceReport.HIReportID));
                    cmd.AddParameter("@ReportAppliedCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aHealthInsuranceReport.ReportAppliedCode));
                    cmd.AddParameter("@V_ReportStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(aHealthInsuranceReport.V_ReportStatus));
                    cmd.AddParameter("@ReportAppliedResultCode", SqlDbType.Int, ConvertNullObjectToDBNull(aHealthInsuranceReport.ReportAppliedResultCode));
                    cn.Open();
                    var mResultValue = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    return mResultValue > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override List<HealthInsuranceReport> GetHIReport()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetHIReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    List<HealthInsuranceReport> HIReportList = null;
                    HIReportList = GetHealthInsuranceReportCollectionFromReader(reader);
                    reader.Close();
                    return HIReportList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override List<HealthInsuranceReport> GetFastReports()
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetFastReports", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandTimeout = int.MaxValue;
                    mConnection.Open();
                    IDataReader mReader = ExecuteReader(mCommand);
                    List<HealthInsuranceReport> mReportCollection = null;
                    mReportCollection = GetHealthInsuranceReportCollectionFromReader(mReader);
                    mReader.Close();
                    return mReportCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override DataSet GetFastReportDetails(long FastReportID)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetFastReportDetails", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandTimeout = int.MaxValue;
                    mCommand.AddParameter("@FastReportID", SqlDbType.BigInt, FastReportID);
                    mConnection.Open();
                    DataSet mDataSet = ExecuteDataSet(mCommand);
                    return mDataSet;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private XElement Get_XElement_TongHop_FromDS(DataSet dsTab1_General, int nTab1RowIdx, int version = 1)
        {
            if (version == 2)
            {
                return new XElement
                            (
                                "TONG_HOP",
                                new XElement("MA_LK", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_lk"])),
                                new XElement("STT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["stt"])),
                                new XElement("MA_BN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_bn"])),
                                new XElement("HO_TEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ho_ten"])),
                                new XElement("NGAY_SINH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ngay_sinh"])),
                                new XElement("GIOI_TINH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["gioi_tinh"])),
                                new XElement("DIA_CHI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["dia_chi"])),
                                new XElement("MA_THE", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_the"])),
                                new XElement("MA_DKBD", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_dkbd"])),
                                new XElement("GT_THE_TU", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["gt_the_tu"])),
                                new XElement("GT_THE_DEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["gt_the_den"])),
                                new XElement("MIEN_CUNG_CT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["mien_cung_ct"])),
                                new XElement("TEN_BENH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ten_benh"])),
                                new XElement("MA_BENH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_benh"])),
                                new XElement("MA_BENHKHAC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_benhkhac"])),
                                new XElement("MA_LYDO_VVIEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_lydo_vvien"])),
                                new XElement("MA_NOI_CHUYEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_noi_chuyen"])),
                                new XElement("MA_TAI_NAN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_tai_nan"])),
                                new XElement("NGAY_VAO", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ngay_vao"])),
                                new XElement("NGAY_RA", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ngay_ra"])),
                                new XElement("SO_NGAY_DTRI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["so_ngay_dtri"])),
                                new XElement("KET_QUA_DTRI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ket_qua_dtri"])),
                                new XElement("TINH_TRANG_RV", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["tinh_trang_rv"])),
                                new XElement("NGAY_TTOAN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ngay_ttoan"])),
                                new XElement("T_THUOC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["t_thuoc"])),
                                new XElement("T_VTYT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["t_vtyt"])),
                                new XElement("T_TONGCHI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["t_tongchi"])),
                                new XElement("T_BNTT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["t_bntt"])),
                                new XElement("T_BNCCT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["t_bncct"])),
                                new XElement("T_BHTT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["t_bhtt"])),
                                new XElement("T_NGUONKHAC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["t_nguonkhac"])),
                                new XElement("T_NGOAIDS", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["t_ngoaids"])),
                                new XElement("NAM_QT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["nam_qt"])),
                                new XElement("THANG_QT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["thang_qt"])),
                                new XElement("MA_LOAI_KCB", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_loai_kcb"])),
                                new XElement("MA_KHOA", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_khoa"])),
                                new XElement("MA_CSKCB", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_cskcb"])),
                                new XElement("MA_KHUVUC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_khuvuc"])),
                                new XElement("MA_PTTT_QT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_pttt_qt"])),
                                new XElement("CAN_NANG", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["can_nang"]))
                            );
            }
            return new XElement
                            (
                                "TONG_HOP",
                                new XElement("MA_LK", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][0])),
                                new XElement("STT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][1])),
                                new XElement("MA_BN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][2])),
                                new XElement("HO_TEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][3])),
                                new XElement("NGAY_SINH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][4])),
                                new XElement("GIOI_TINH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][5])),
                                new XElement("DIA_CHI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][6])),
                                new XElement("MA_THE", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][7])),
                                new XElement("MA_DKBD", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][8])),
                                new XElement("GT_THE_TU", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][9])),
                                new XElement("GT_THE_DEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][10])),
                                new XElement("TEN_BENH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][11])),
                                new XElement("MA_BENH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][12])),
                                new XElement("MA_BENHKHAC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][13])),
                                new XElement("MA_LYDO_VVIEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][14])),
                                new XElement("MA_NOI_CHUYEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][15])),
                                new XElement("MA_TAI_NAN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][16])),
                                new XElement("NGAY_VAO", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][17])),
                                new XElement("NGAY_RA", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][18])),
                                new XElement("SO_NGAY_DTRI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][19])),
                                new XElement("KET_QUA_DTRI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][20])),
                                new XElement("TINH_TRANG_RV", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][21])),
                                new XElement("NGAY_TTOAN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][22])),
                                new XElement("MUC_HUONG", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][23])),
                                new XElement("T_THUOC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][24])),
                                new XElement("T_VTYT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][25])),
                                new XElement("T_TONGCHI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][26])),
                                new XElement("T_BNTT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][27])),
                                new XElement("T_BHTT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][28])),
                                new XElement("T_NGUONKHAC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][29])),
                                new XElement("T_NGOAIDS", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][30])),
                                new XElement("NAM_QT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][31])),
                                new XElement("THANG_QT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][32])),
                                new XElement("MA_LOAI_KCB", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][33])),
                                new XElement("MA_KHOA", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][34])),
                                new XElement("MA_CSKCB", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][35])),
                                new XElement("MA_KHUVUC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][36])),
                                new XElement("MA_PTTT_QT", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][37])),
                                new XElement("CAN_NANG", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][38]))
                            );
        }

        private XElement Get_XElement_DrugDetails_FromDS(DataRow dsTab2_DrugRow, int version = 1)
        {
            if (version == 2)
            {
                return new XElement
                        (
                            "CHI_TIET_THUOC",
                            new XElement("MA_LK", Convert.ToString(dsTab2_DrugRow["ma_lk"])),
                            new XElement("STT", Convert.ToString(dsTab2_DrugRow["stt"])),
                            new XElement("MA_THUOC", Convert.ToString(dsTab2_DrugRow["ma_thuoc"])),
                            new XElement("MA_NHOM", Convert.ToString(dsTab2_DrugRow["ma_nhom"])),
                            new XElement("TEN_THUOC", Convert.ToString(dsTab2_DrugRow["ten_thuoc"])),
                            new XElement("DON_VI_TINH", Convert.ToString(dsTab2_DrugRow["don_vi_tinh"])),
                            new XElement("HAM_LUONG", Convert.ToString(dsTab2_DrugRow["ham_luong"])),
                            new XElement("DUONG_DUNG", Convert.ToString(dsTab2_DrugRow["duong_dung"])),
                            new XElement("LIEU_DUNG", Convert.ToString(dsTab2_DrugRow["lieu_dung"])),
                            new XElement("SO_DANG_KY", Convert.ToString(dsTab2_DrugRow["so_dang_ky"])),
                            new XElement("TT_THAU", Convert.ToString(dsTab2_DrugRow["tt_thau"])),
                            new XElement("PHAM_VI", Convert.ToString(dsTab2_DrugRow["pham_vi"])),
                            new XElement("TYLE_TT", Convert.ToString(dsTab2_DrugRow["tyle_tt"])),
                            new XElement("SO_LUONG", Convert.ToString(dsTab2_DrugRow["so_luong"])),
                            new XElement("DON_GIA", Convert.ToString(dsTab2_DrugRow["don_gia"])),
                            new XElement("THANH_TIEN", Convert.ToString(dsTab2_DrugRow["thanhtien"])),
                            new XElement("MUC_HUONG", Convert.ToString(dsTab2_DrugRow["muc_huong"])),
                            new XElement("T_NGUONKHAC", Convert.ToString(dsTab2_DrugRow["t_nguonkhac"])),
                            new XElement("T_BNTT", Convert.ToString(dsTab2_DrugRow["t_bntt"])),
                            new XElement("T_BHTT", Convert.ToString(dsTab2_DrugRow["t_bhtt"])),
                            new XElement("T_BNCCT", Convert.ToString(dsTab2_DrugRow["t_bncct"])),
                            new XElement("T_NGOAIDS", Convert.ToString(dsTab2_DrugRow["t_ngoaids"])),
                            new XElement("MA_KHOA", Convert.ToString(dsTab2_DrugRow["ma_khoa"])),
                            new XElement("MA_BAC_SI", Convert.ToString(dsTab2_DrugRow["ma_bac_si"])),
                            new XElement("MA_BENH", Convert.ToString(dsTab2_DrugRow["ma_benh"])),
                            new XElement("NGAY_YL", Convert.ToString(dsTab2_DrugRow["ngay_yl"])),
                            new XElement("MA_PTTT", Convert.ToString(dsTab2_DrugRow["ma_pttt"]))
                        );
            }
            return new XElement
                        (
                            "CHI_TIET_THUOC",
                            new XElement("MA_LK", Convert.ToString(dsTab2_DrugRow[0])),
                            new XElement("STT", Convert.ToString(dsTab2_DrugRow[1])),
                            new XElement("MA_THUOC", Convert.ToString(dsTab2_DrugRow[2])),
                            new XElement("MA_NHOM", Convert.ToString(dsTab2_DrugRow[3])),
                            new XElement("TEN_THUOC", Convert.ToString(dsTab2_DrugRow[4])),
                            new XElement("DON_VI_TINH", Convert.ToString(dsTab2_DrugRow[5])),
                            new XElement("HAM_LUONG", Convert.ToString(dsTab2_DrugRow[6])),
                            new XElement("DUONG_DUNG", Convert.ToString(dsTab2_DrugRow[7])),
                            new XElement("LIEU_DUNG", Convert.ToString(dsTab2_DrugRow[8])),
                            new XElement("SO_DANG_KY", Convert.ToString(dsTab2_DrugRow[9])),
                            new XElement("SO_LUONG", Convert.ToString(dsTab2_DrugRow[10])),
                            new XElement("DON_GIA", Convert.ToString(dsTab2_DrugRow[11])),
                            new XElement("TYLE_TT", "100"),
                            new XElement("THANH_TIEN", Convert.ToString(dsTab2_DrugRow[13])),
                            new XElement("MA_KHOA", Convert.ToString(dsTab2_DrugRow[14])),
                            new XElement("MA_BAC_SI", Convert.ToString(dsTab2_DrugRow[15])),
                            new XElement("MA_BENH", Convert.ToString(dsTab2_DrugRow[16])),
                            new XElement("NGAY_YL", Convert.ToString(dsTab2_DrugRow[17])),
                            new XElement("MA_PTTT", Convert.ToString(dsTab2_DrugRow[18]))
                        );
        }

        private XElement Get_XElement_MatAndService_FromDS(DataRow dsTab3_MatAndServices, int version = 1)
        {
            if (version == 2)
            {
                return new XElement
                        (
                            "CHI_TIET_DVKT",
                            new XElement("MA_LK", Convert.ToString(dsTab3_MatAndServices["ma_lk"])),
                            new XElement("STT", Convert.ToString(dsTab3_MatAndServices["stt"])),
                            new XElement("MA_DICH_VU", Convert.ToString(dsTab3_MatAndServices["ma_dich_vu"])),
                            new XElement("MA_VAT_TU", Convert.ToString(dsTab3_MatAndServices["ma_vat_tu"])),
                            new XElement("MA_NHOM", Convert.ToString(dsTab3_MatAndServices["ma_nhom"])),
                            new XElement("GOI_VTYT", Convert.ToString(dsTab3_MatAndServices["goi_vtyt"])),
                            new XElement("TEN_VAT_TU", Convert.ToString(dsTab3_MatAndServices["ten_vat_tu"])),
                            new XElement("TEN_DICH_VU", Convert.ToString(dsTab3_MatAndServices["ten_dich_vu"])),
                            new XElement("DON_VI_TINH", Convert.ToString(dsTab3_MatAndServices["don_vi_tinh"])),
                            new XElement("PHAM_VI", Convert.ToString(dsTab3_MatAndServices["pham_vi"])),
                            new XElement("SO_LUONG", Convert.ToString(dsTab3_MatAndServices["so_luong"])),
                            new XElement("DON_GIA", Convert.ToString(dsTab3_MatAndServices["don_gia"])),
                            new XElement("TT_THAU", Convert.ToString(dsTab3_MatAndServices["tt_thau"])),
                            new XElement("TYLE_TT", Convert.ToString(dsTab3_MatAndServices["tyle_tt"])),
                            new XElement("THANH_TIEN", Convert.ToString(dsTab3_MatAndServices["thanh_tien"])),
                            new XElement("T_TRANTT", Convert.ToString(dsTab3_MatAndServices["t_trantt"])),
                            new XElement("MUC_HUONG", Convert.ToString(dsTab3_MatAndServices["muc_huong"])),
                            new XElement("T_NGUONKHAC", Convert.ToString(dsTab3_MatAndServices["t_nguonkhac"])),
                            new XElement("T_BNTT", Convert.ToString(dsTab3_MatAndServices["t_bntt"])),
                            new XElement("T_BHTT", Convert.ToString(dsTab3_MatAndServices["t_bhtt"])),
                            new XElement("T_BNCCT", Convert.ToString(dsTab3_MatAndServices["t_bncct"])),
                            new XElement("T_NGOAIDS", Convert.ToString(dsTab3_MatAndServices["t_ngoaids"])),
                            new XElement("MA_KHOA", Convert.ToString(dsTab3_MatAndServices["ma_khoa"])),
                            new XElement("MA_GIUONG", Convert.ToString(dsTab3_MatAndServices["ma_giuong"])),
                            new XElement("MA_BAC_SI", Convert.ToString(dsTab3_MatAndServices["ma_bac_si"])),
                            new XElement("MA_BENH", Convert.ToString(dsTab3_MatAndServices["ma_benh"])),
                            new XElement("NGAY_YL", Convert.ToString(dsTab3_MatAndServices["ngay_yl"])),
                            new XElement("NGAY_KQ", Convert.ToString(dsTab3_MatAndServices["ngay_kq"])),
                            new XElement("MA_PTTT", Convert.ToString(dsTab3_MatAndServices["ma_pttt"]))
                        );
            }
            return new XElement
                        (
                            "CHI_TIET_DVKT",
                            new XElement("MA_LK",       Convert.ToString(dsTab3_MatAndServices[0])),
                            new XElement("STT",         Convert.ToString(dsTab3_MatAndServices[1])),
                            new XElement("MA_DICH_VU",  Convert.ToString(dsTab3_MatAndServices[2])),
                            new XElement("MA_VAT_TU",   Convert.ToString(dsTab3_MatAndServices[3])),
                            new XElement("MA_NHOM",     Convert.ToString(dsTab3_MatAndServices[4])),
                            new XElement("TEN_DICH_VU", Convert.ToString(dsTab3_MatAndServices[5])),
                            new XElement("DON_VI_TINH", Convert.ToString(dsTab3_MatAndServices[6])),
                            new XElement("SO_LUONG",    Convert.ToString(dsTab3_MatAndServices[7])),
                            new XElement("DON_GIA",     Convert.ToString(dsTab3_MatAndServices[8])),
                            /*▼====: #002*/
                            //new XElement("TYLE_TT",     "100"),
                            new XElement("TYLE_TT",     Convert.ToString(dsTab3_MatAndServices[9])),
                            /*▲====: #002*/
                            new XElement("THANH_TIEN",  Convert.ToString(dsTab3_MatAndServices[10])),
                            new XElement("MA_KHOA",     Convert.ToString(dsTab3_MatAndServices[11])),
                            new XElement("MA_BAC_SI",   Convert.ToString(dsTab3_MatAndServices[12])),
                            new XElement("MA_BENH",     Convert.ToString(dsTab3_MatAndServices[13])),
                            new XElement("NGAY_YL",     Convert.ToString(dsTab3_MatAndServices[14])),
                            new XElement("NGAY_KQ",     Convert.ToString(dsTab3_MatAndServices[15])),
                            new XElement("MA_PTTT",     Convert.ToString(dsTab3_MatAndServices[16]))
                        );
        }

        private XElement Get_XElement_PCLResults_FromDS(DataRow dsTab4_PCLResult)
        {
            return new XElement (
                "CHI_TIET_CLS",
                new XElement("MA_LK", Convert.ToString(dsTab4_PCLResult["ma_lk"])),
                new XElement("STT", Convert.ToString(dsTab4_PCLResult["stt"])),
                new XElement("MA_DICH_VU", Convert.ToString(dsTab4_PCLResult["ma_dich_vu"])),
                new XElement("MA_CHI_SO", Convert.ToString(dsTab4_PCLResult["ma_chi_so"])),
                new XElement("TEN_CHI_SO", Convert.ToString(dsTab4_PCLResult["ten_chi_so"])),
                new XElement("GIA_TRI", Convert.ToString(dsTab4_PCLResult["gia_tri"])),
                new XElement("MA_MAY", Convert.ToString(dsTab4_PCLResult["ma_may"])),
                new XElement("MO_TA", Convert.ToString(dsTab4_PCLResult["mo_ta"])),
                new XElement("KET_LUAN", Convert.ToString(dsTab4_PCLResult["ket_luan"])),
                new XElement("NGAY_KQ", Convert.ToString(dsTab4_PCLResult["ngay_kq"]))
            );
        }

        private int GetXmlElementsForAll3Tabs(long nHIReportID, XElement elemDANHSACH_AllHOSO_InReport)
        {
            DataSet dsTab1_General = new DataSet();
            DataTable dsTab2_Drug = new DataTable();
            DataTable dsTab3_MatAndServices = new DataTable();
            DataTable dsTab4_PCLResults = new DataTable();

            int nTotalNumRowInTab1 = 0;

            // Txd the following 3 totals only for debuging purpose
            int nTotXml1 = 0;
            int nTotXml2 = 0;
            int nTotXml3 = 0;
            int nTotXml4 = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmdTab1Gen = new SqlCommand("spGetReport9324_General", cn);
                cmdTab1Gen.CommandType = CommandType.StoredProcedure;
                cmdTab1Gen.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab1Gen.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab1Gen = new SqlDataAdapter(cmdTab1Gen);
                adapterTab1Gen.Fill(dsTab1_General);

                SqlCommand cmdTab2Drug = new SqlCommand("spGetReport9324_Drug", cn);
                cmdTab2Drug.CommandType = CommandType.StoredProcedure;
                cmdTab2Drug.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab2Drug.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab2Drug = new SqlDataAdapter(cmdTab2Drug);
                adapterTab2Drug.Fill(dsTab2_Drug);

                SqlCommand cmdTab3Svcs = new SqlCommand("spGetReport9324_TechnicalServiceAndMedicalMaterial", cn);
                cmdTab3Svcs.CommandType = CommandType.StoredProcedure;
                cmdTab3Svcs.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab3Svcs.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab3Svcs = new SqlDataAdapter(cmdTab3Svcs);
                adapterTab3Svcs.Fill(dsTab3_MatAndServices);

                SqlCommand cmdTab4PCLs = new SqlCommand("spGetReport9324_PCLResult", cn);
                cmdTab4PCLs.CommandType = CommandType.StoredProcedure;
                cmdTab4PCLs.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab4PCLs.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab4PCLs = new SqlDataAdapter(cmdTab4PCLs);
                adapterTab4PCLs.Fill(dsTab4_PCLResults);

                nTotalNumRowInTab1 = dsTab1_General.Tables[0].Rows.Count;

                for (int nTab1RowIdx = 0; nTab1RowIdx < nTotalNumRowInTab1; ++nTab1RowIdx)
                {
                    string tab1_MaLK = Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][0]);

                    XElement elemHoSo_ForEach_LanKham = new XElement("HOSO");

                    XElement elemFileHoSo_Tab1_Xml1 = new XElement("FILEHOSO", new XElement("LOAIHOSO","XML1"));

                    /*▼====: #004*/
                    //XElement elemTongHop = Get_XElement_TongHop_FromDS(dsTab1_General, nTab1RowIdx);
                    XElement elemTongHop = Get_XElement_TongHop_FromDS(dsTab1_General, nTab1RowIdx, Globals.AxServerSettings.CommonItems.CurrentHIReportVersion);
                    /*▲====: #004*/

                    // TxD: Uncomment the following line if you want to debug and see the content of XML1 before being encoded into Base64 string
                    //elemFileHoSo_Tab1_Xml1.Add(new XElement("NOIDUNGFILE", elemTongHop));

                    elemFileHoSo_Tab1_Xml1.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemTongHop.ToString()))));

                    elemHoSo_ForEach_LanKham.Add(elemFileHoSo_Tab1_Xml1);
                    nTotXml1++;

                    XElement elemFileHoSo_DrugTab2_Xml2 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML2"));
                    XElement elemDrugListDetails = new XElement("DSACH_CHI_TIET_THUOC");
                    
                    string strWhere = "ma_lk = '" + tab1_MaLK + "'";

                    DataRow[] drugRows = null;
                    if (dsTab2_Drug != null && dsTab2_Drug.Rows.Count > 0)
                    {
                        drugRows = dsTab2_Drug.Select(strWhere);
                    }

                    if (drugRows != null && drugRows.Count() > 0)
                    {
                        foreach (DataRow drugRow in drugRows)
                        {
                            /*▼====: #004*/
                            //XElement elemDrugDetails = Get_XElement_DrugDetails_FromDS(drugRow);
                            XElement elemDrugDetails = Get_XElement_DrugDetails_FromDS(drugRow, Globals.AxServerSettings.CommonItems.CurrentHIReportVersion);
                            /*▲====: #004*/
                            elemDrugListDetails.Add(elemDrugDetails);                            
                        }

                        // TxD: Uncomment the following line if you want to debug and see the content of XML2 before being encoded into Base64 string
                        //elemFileHoSo_DrugTab2_Xml2.Add( new XElement("NOIDUNGFILE", elemDrugListDetails));

                        elemFileHoSo_DrugTab2_Xml2.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemDrugListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_DrugTab2_Xml2);
                        nTotXml2++;

                    }

                    XElement elemFileHoSo_MatSvcTab3_Xml3     = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML3"));
                    XElement elemMatSvcListDetails  = new XElement("DSACH_CHI_TIET_DVKT");

                    DataRow[] matSvcRows = null;
                    if (dsTab3_MatAndServices != null && dsTab3_MatAndServices.Rows.Count > 0)
                    {
                        matSvcRows = dsTab3_MatAndServices.Select(strWhere);
                    }
                    if (matSvcRows != null && matSvcRows.Count() > 0)
                    {
                        foreach(DataRow matSvcRow in matSvcRows )
                        {
                            /*▼====: #004*/
                            //XElement elemMatSvcDetails = Get_XElement_MatAndService_FromDS(matSvcRow);
                            XElement elemMatSvcDetails = Get_XElement_MatAndService_FromDS(matSvcRow, Globals.AxServerSettings.CommonItems.CurrentHIReportVersion);
                            /*▲====: #004*/
                            elemMatSvcListDetails.Add(elemMatSvcDetails);
                        }

                        // TxD: Uncomment the following line if you want to debug and see the content of XML3 before being encoded into Base64 string
                        //elemFileHoSo_MatSvcTab3_Xml3.Add(new XElement("NOIDUNGFILE", elemMatSvcListDetails));

                        elemFileHoSo_MatSvcTab3_Xml3.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemMatSvcListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_MatSvcTab3_Xml3);
                        nTotXml3++;
                    }

                    XElement elemFileHoSo_PCLResultsTab4_Xml4 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML4"));
                    XElement elemPCLResultListDetails = new XElement("DSACH_CHI_TIET_CLS");
                    if (dsTab4_PCLResults != null && dsTab4_PCLResults.Rows.Count > 0)
                    {
                        foreach (DataRow aRow in dsTab4_PCLResults.Rows)
                        {
                            XElement elemPCLResultDetails = Get_XElement_PCLResults_FromDS(aRow);
                            elemPCLResultListDetails.Add(elemPCLResultDetails);
                        }

                        elemFileHoSo_PCLResultsTab4_Xml4.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemPCLResultListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_PCLResultsTab4_Xml4);
                        nTotXml4++;
                    }

                    elemDANHSACH_AllHOSO_InReport.Add(elemHoSo_ForEach_LanKham);
                }
            }

            Debug.WriteLine("==========>   Total number XML1 = {0} - XML2 = {1} - XML3 = {2} - XML4 = {3}", nTotXml1, nTotXml2, nTotXml3, nTotXml4);

            return nTotalNumRowInTab1;
        }

        public override Stream GetHIXmlReport9324_AllTab123_InOneRpt_Data(long nHIReportID)
        {
            XNamespace xmlns = XNamespace.Get("http://ns.hr-xml.org/2007-04-15");
            XNamespace xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");


            XDocument XD1 = new XDocument();
            XElement GIAMDINHHS = new XElement("GIAMDINHHS",
                                    new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                                    new XAttribute(XNamespace.Xmlns + "xsi", xsi));

            XD1.Add(GIAMDINHHS);

            XElement TTDV = new XElement("THONGTINDONVI", new XElement("MACSKCB", "95076"));

            XElement THONGTINHOSO = new XElement("THONGTINHOSO");
            XD1.Root.Add(TTDV);

            XD1.Root.Add(THONGTINHOSO);

            XElement DANHSACHHOSO = new XElement("DANHSACHHOSO");

            int nTotNumHoSoTrongBC = GetXmlElementsForAll3Tabs(nHIReportID, DANHSACHHOSO);

            DateTime dtToday = DateTime.Now;
            string strNgayLap = dtToday.Year.ToString() + dtToday.Month.ToString("D2") + dtToday.Day.ToString("D2");
            XElement NGAYLAP = new XElement("NGAYLAP", strNgayLap);
            XElement SOLUONGHOSO = new XElement("SOLUONGHOSO", nTotNumHoSoTrongBC.ToString());

            THONGTINHOSO.Add(NGAYLAP);
            THONGTINHOSO.Add(SOLUONGHOSO);
            THONGTINHOSO.Add(DANHSACHHOSO);

            MemoryStream memStream = new MemoryStream();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.OmitXmlDeclaration = false;
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.Encoding = Encoding.UTF8;

            using (XmlWriter xmlWriter = XmlWriter.Create(memStream, xmlWriterSettings))
            {            
                XD1.Save(xmlWriter);
            }

            memStream.Position = 0;

            //using (FileStream xmlFile = new FileStream("D:\\TxdTestXmlFile2.xml", FileMode.Create, FileAccess.Write))
            //{                
            //    memStream.WriteTo(xmlFile);
            //    xmlFile.Flush();
            //    xmlFile.Close();
            //}
            
            return memStream;

            
        }
        /*▼====: #002*/
        #region HOSPayments
        public override HOSPayment EditHOSPayment(HOSPayment aHOSPayment)
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spEditHOSPayment", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandTimeout = int.MaxValue;
                    if (aHOSPayment.HOSPaymentID > 0)
                        mCommand.AddParameter("@HOSPaymentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aHOSPayment.HOSPaymentID));
                    mCommand.AddParameter("@V_PayReson", SqlDbType.BigInt, ConvertNullObjectToDBNull(aHOSPayment.V_PayReson));
                    mCommand.AddParameter("@TransactionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aHOSPayment.TransactionDate));
                    mCommand.AddParameter("@PaymentDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aHOSPayment.PaymentDate));
                    mCommand.AddParameter("@PaymentReson", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aHOSPayment.PaymentReson));
                    mCommand.AddParameter("@PaymentNotice", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aHOSPayment.PaymentNotice));
                    mCommand.AddParameter("@PaymentAmount", SqlDbType.Money, ConvertNullObjectToDBNull(aHOSPayment.PaymentAmount));
                    mCommand.AddParameter("@V_CharityObjectType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aHOSPayment.V_CharityObjectType));
                    mCommand.AddParameter("@V_PatientSubject", SqlDbType.BigInt, ConvertNullObjectToDBNull(aHOSPayment.V_PatientSubject));
                    mCommand.AddParameter("@NumbOfPerson", SqlDbType.Int, ConvertNullObjectToDBNull(aHOSPayment.NumbOfPerson));
                    mCommand.AddParameter("@PatientName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aHOSPayment.PatientName));
                    mCommand.AddParameter("@DOB", SqlDbType.DateTime, ConvertNullObjectToDBNull(aHOSPayment.DOB));
                    SqlParameter mOutID = new SqlParameter("@OutID", SqlDbType.BigInt);
                    mOutID.Direction = ParameterDirection.Output;
                    mCommand.Parameters.Add(mOutID);
                    mConnect.Open();
                    int mCount = mCommand.ExecuteNonQuery();
                    if (mOutID.Value != null)
                    {
                        aHOSPayment.HOSPaymentID = (long)mOutID.Value;
                    }
                    mCommand.Dispose();
                    return aHOSPayment;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override bool DeleteHOSPayment(long aHOSPaymentID)
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spDeleteHOSPayment", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandTimeout = int.MaxValue;
                    mCommand.AddParameter("@HOSPaymentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aHOSPaymentID));
                    mConnect.Open();
                    return mCommand.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override IList<HOSPayment> GetHOSPayments(DateTime aStartDate, DateTime aEndDate)
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetHOSPayments", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandTimeout = int.MaxValue;
                    mCommand.AddParameter("@StartDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aStartDate));
                    mCommand.AddParameter("@EndDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aEndDate));
                    mConnect.Open();
                    var mResult = GetHOSPaymentCollectionFromReader(mCommand.ExecuteReader());
                    mCommand.Dispose();
                    return mResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        /*▲====: #002*/

        public override DataSet PreviewHIReport(long PtRegistrationID, long V_RegistrationType, out string ErrText)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPreviewHIReportDetail", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
                    cmd.AddParameter("@ErrText", SqlDbType.NVarChar, -1, ParameterDirection.Output);
                    cn.Open();
                    DataSet mDataSet = ExecuteDataSet(cmd);
                    ErrText = null;
                    var OutErrText = cmd.Parameters.Cast<SqlParameter>().FirstOrDefault(x => x.ParameterName == "@ErrText");
                    if (OutErrText != null && OutErrText.Value != null && OutErrText.Value != DBNull.Value)
                    {
                        ErrText = OutErrText.Value.ToString();
                    }
                    return mDataSet;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}