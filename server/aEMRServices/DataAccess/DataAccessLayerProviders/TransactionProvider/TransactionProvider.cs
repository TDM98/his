using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using eHCMS.Configurations;
using System.Xml.Linq;
using System.IO;
using eHCMS.DAL;
using System.Xml;
using eHCMSLanguage;
using OfficeOpenXml;
/*
* 20190326 #001 TNHX: Add function to export excel in ConfirmHIRegistrationView
* 20190608 #002 TNHX: [BM0006715] Add function to export excel for Accountant
* 20190622 #003 TNHX: [BM0011874] Add function to export excel for report RptTongHopDoanhThuTheoKhoa
* 20190628 #004 TTM:   BM 0011903: Thêm chức năng xoá báo cáo nếu như chưa chuyển đổi dữ liệu sang cho FAST.
* 20200101 #005 TTM:   Bổ sung biến để lấy thông tin CSKCB theo cấu hình.
* 20200319 #006 TTM:   BM 0027022: [79A] Bổ sung tích chọn xuất Excel toàn bộ dữ liệu, đã xác nhận, chưa xác nhận. 
* 20200401 #007 TTM:   BM 0029062: Thêm nút mẫu nội bộ cho xuất excel của mẫu 21. Mẫu nội bộ sẽ gom các dịch vụ có cùng mã bảo hiểm, tên bảo hiểm và giá bảo hiểm thành 1 dòng.
* 20200404 #008 TTM:   BM 0029080: Xuất dữ liệu XML để báo cáo cho cổng dữ liệu Quốc Gia
* 20200408 #009 TNHX:   Thêm xuất excel cho báo cáo BC_BNTaiKhamBenhManTinh
* 20200414 #010 TTM:   BM 0032119: Bổ sung mã giao dịch cho đăng ký báo cáo thủ công thông qua VAS.
* 20200519 #011 TTM:   BM 0038182: Chức năng bắt cặp đăng ký ngoại trú và nội trú (Nội trú nhập viện từ đề nghi nhập viện).
* 20200610 #012 TNHX: Thêm báo cáo toa thuoc hang ngay DLS + KD, bc thuoc - y cu dỡ dang cuoi ky
* 20200624 #013 TNHX: [BM ] Create export excel for BC_NHAP_PTTT_KHOA_PHONG
* 20200811 #014 TNHX: [BM ] Create export excel for TKChiDinhThuocTPKhoaNoiTru
* 20200811 #014 TNHX: [BM ] Create export excel for XRptInOutStockValueDrugDept_KT + XRptInOutStockValueClinicDept_KT
* 20210323 #015 BLQ: #243 Lấy HIReport cho KHTH
* 20210615 #016 DatTB:   Thêm xuất excel cho báo cáo BC_BenhNhanKhamBenh
* 20210615 #017 DatTB:   Thêm xuất excel cho báo cáo BC_BNHenTaiKhamBenhDacTrung
* 20211224 #018 TNHX: 803 Thêm báo cao bn covid cho khoa dược/ kế toán tổng hợp
* 20220214 #019 QTD:  Thêm xuất excel cho báo cáo Bệnh đặc trưng
* 20220215 #020 QTD:  Thêm xuất excel cho báo cáo quyết toán
* 20220330 #021 DatTB:  Thêm xuất excel cho Báo cáo quản lý kiểm duyệt hồ sơ KHTH
* 20220409 #022 DatTB:  Thêm Báo cáo thông tin danh mục thuốc DLS
* 20220407 #023 DatTB:  Thêm Báo cáo kiểm tra lịch sử KCB DLS
* 20220516 #024 DatTB: Báo cáo tình hình thực hiện CLS - Đã thực hiện
* 20220523 #025 DatTB: Báo cáo Doanh thu theo Khoa
* 20220807 #026 DatTB: Báo cáo thống kê số lượng hồ sơ điều trị ngoại trú
* + Tạo màn hình, thêm các trường lọc dữ liệu.
* + Thêm trường phòng khám sau khi chọn khoa.
* + Validate các trường lọc.
* + Thêm điều kiện để lấy khoa theo list DeptID.
* 20220808 #027 QTD:   Báo cáo danh sách BN điều trị ngoại trú
* 20220927 #028 DatTB: Báo cáo danh sách bệnh nhân ĐTNT
* 20221019 #029 BLQ: Thêm báo cáo Phát hành thẻ Khám chữa bệnh
* 20221125 #030 BLQ: Thêm báo cáo giờ làm thêm bác sĩ
* 20221128 #031 DatTB: Thêm báo cáo thao tác người dùng
* 20221201 #032 DatTB: Thêm báo cáo thống kê hồ sơ ĐTNT
* 20221213 #033 TnHX: 994 Thêm func tạo dữ liệu toa thuốc điện tử
* 20221213 #034 DatTB: Lấy thêm trường tên bác sĩ xuất dữ liệu KSK
* 20221227 #035 QTD   Thêm điều kiện lọc trạng thái khi xuất DL Đăng ký
* 20230108 #036 THNHX:  944 Thêm func xóa dữ liệu đẩy đơn thuốc điện tử không thành công
* 20230111 #037 DatTB: Fix lỗi đơn giá bị chồng lên cột bác sĩ
* 20230210 #038 QTD:  Xuất Excel BC Sử dụng thuốc cản quang
* 20230220 #039 QTD:  Huỷ đẩy cổng DTQG Nhà thuốc 
* 20230218 #040 DatTB: Thêm Báo cáo thời gian tư vấn cấp toa/chỉ định
* 20230309 #041 BLQ: Thêm báo cáo DLS
* 20230314 #042 BLQ: Thêm hàm tạo xml đẩy cổng BH theo 130
* 20230621 #043 DatTB: Thay đổi stored cho export excel riêng
*/
namespace aEMR.DataAccessLayer.Providers
{
    public class TransactionProvider : DataProviderBase
    {
        static private TransactionProvider _instance = null;
        static public TransactionProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TransactionProvider();
                }
                return _instance;
            }
        }
         public TransactionProvider()
        {
            this.ConnectionString = Globals.Settings.Transactions.ConnectionString;

        }


        protected override PatientTransaction GetPatientTransactionFromReader(IDataReader reader)
        {
            PatientTransaction p = base.GetPatientTransactionFromReader(reader);
            p.PatientRegistration = new PatientRegistration();
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
        public  List<PatientTransaction> GetTransaction_ByPtID(SeachPtRegistrationCriteria SearchCriteria, int PageSize, int PageIndex, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientRegistration_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter paramPatientName = new SqlParameter("@FullName", SqlDbType.NVarChar);
                paramPatientName.Value = ConvertNullObjectToDBNull(SearchCriteria.FullName);
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        #region 2. InsertData Report
        public  bool InsertDataToReport(string begindate, string enddate)
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
                CleanUpConnectionAndCommand(cn, cmd);
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
                    cmd.AddParameter("@ConsultDiagRepType", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.ConsultDiagRepType));
                    cmd.AddParameter("@IsConsultingHistoryView", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsConsultingHistoryView));
                    cmd.AddParameter("@IsApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsApproved));
                    cmd.AddParameter("@IsLated", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsLated));
                    cmd.AddParameter("@IsAllExamCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsAllExamCompleted));
                    cmd.AddParameter("@IsSurgeryCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsSurgeryCompleted));
                    cmd.AddParameter("@IsWaitSurgery", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsWaitSurgery));
                    cmd.AddParameter("@IsDuraGraft", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsDuraGraft));
                    cmd.AddParameter("@IsWaitingExamCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsWaitingExamCompleted));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.ToDate));
                    cmd.AddParameter("@IsCancelSurgery", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsCancelSurgery));
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
                    if (criteria.reportName == ReportName.TEMP19
                        || criteria.reportName == ReportName.TEMP20_NOITRU_NEW
                        || criteria.reportName == ReportName.TEMP21_NEW
                        || criteria.reportName == ReportName.TEMP80a_CHITIET
                        || criteria.reportName == ReportName.TEMP80a_TONGHOP)
                    {
                        cmd.AddParameter("@V_79AExportType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_79AExportType));
                    }
                    if (criteria.reportName == ReportName.BCChiTietBNChuyenDi)
                    {
                        cmd.AddParameter("@TransferType", SqlDbType.BigInt, 1);
                    }
                    if (criteria.reportName == ReportName.BCChiTietBNChuyenDen)
                    {
                        cmd.AddParameter("@TransferType", SqlDbType.BigInt, 2);
                    }
                }
                cmd.CommandTimeout = int.MaxValue;
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.Fill(dsExportToExcellAll);
                dsExportToExcellAll = ExecuteDataSet(cmd);
                List<List<string>> returnAllExcelData = GetReportStringFromData(dsExportToExcellAll);
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }
        private List<List<string>> GetReportStringFromData(DataTable aDataTable)
        {
            List<List<string>> mReportStringCollection = new List<List<string>>();
            if (aDataTable == null || aDataTable.Rows.Count == 0)
            {
                return null;
            }
            List<string> mColumnCollection = new List<string>();
            for (int i = 0; i <= aDataTable.Columns.Count - 1; i++)
            {
                mColumnCollection.Add(aDataTable.Columns[i].ToString().Trim());
            }
            mReportStringCollection.Add(mColumnCollection);
            for (int i = 0; i <= aDataTable.Rows.Count - 1; i++)
            {
                List<string> mRowData = new List<string>();
                for (int j = 0; j <= aDataTable.Columns.Count - 1; j++)
                {
                    mRowData.Add(Convert.ToString(aDataTable.Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                }
                mReportStringCollection.Add(mRowData);
            }
            return mReportStringCollection;
        }
        private List<List<string>> GetReportStringFromData(DataSet aDataSet)
        {
            List<List<string>> mReportStringCollection = new List<List<string>>();
            if (aDataSet == null || aDataSet.Tables == null || aDataSet.Tables.Count == 0)
            {
                return null;
            }
            return GetReportStringFromData(aDataSet.Tables[0]);
        }
        private string getCSVReportString(string storeName, ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand(storeName, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (criteria.reportName != ReportName.PatientQuotation)
                {
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                }
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
                else if (criteria.reportName == ReportName.InfectionCaseStatistics)
                {
                    cmd.AddParameter("@V_InfectionCaseStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.Status));
                    cmd.AddParameter("@DrugName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.BrandName));
                }
                else if (criteria.reportName == ReportName.TreatsStatisticsByDept)
                {
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                    cmd.AddParameter("@IsExportExcel", SqlDbType.Bit, true);
                    cmd.AddParameter("@RegistrationStatus", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.RegistrationStatus));
                }
                else if (criteria.reportName == ReportName.TreatsStatisticsByDept_Detail)
                {
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                    cmd.AddParameter("@IsExportExcel", SqlDbType.Bit, true);
                    cmd.AddParameter("@RegistrationStatus", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.RegistrationStatus));
                    if (criteria.AverageTime + criteria.AverageAmount > 0)
                    {
                        cmd.AddParameter("@FilerByTime", SqlDbType.Decimal, ConvertNullObjectToDBNull(criteria.AverageTime));
                        cmd.AddParameter("@FilerByMoney", SqlDbType.Decimal, ConvertNullObjectToDBNull(criteria.AverageAmount));
                    }
                }
                else if (criteria.reportName == ReportName.PatientQuotation)
                {
                    cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.SearchID));
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
                    //▼===== #006
                    if (criteria.reportName == ReportName.TEMP79a_CHITIET || criteria.reportName == ReportName.TEMP79a_TONGHOP)
                    {
                        cmd.AddParameter("@V_79AExportType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_79AExportType));
                    }
                    //▲===== #006
                }
                cmd.CommandTimeout = int.MaxValue;
                dsExportToExcellAll = ExecuteDataSet(cmd);
                if (dsExportToExcellAll == null || dsExportToExcellAll.Tables == null || dsExportToExcellAll.Tables.Count == 0)
                {
                    return null;
                }
                string returnAllExcelData = GetCSVReportStringFromData(dsExportToExcellAll.Tables[0]);
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }
        private string GetCellStringValue(object aCellValue)
        {
            if (aCellValue == null || aCellValue == DBNull.Value)
            {
                return "";
            }
            if (aCellValue.GetType().Equals(typeof(DateTime)) || aCellValue.GetType().Equals(typeof(DateTime?)))
            {
                var SystemDateTimeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
                return (aCellValue as DateTime?).Value.ToString(SystemDateTimeFormat);
            }
            decimal mDecimalValue = 0;
            if (decimal.TryParse(aCellValue.ToString(), out mDecimalValue))
            {
                if (mDecimalValue > 1000000000)
                {
                    return string.Format("=\"{0}\"", mDecimalValue);
                }
            }
            if (aCellValue.ToString().Contains(",") || aCellValue.ToString().Contains("\n") || aCellValue.ToString().Contains("\r"))
            {
                return string.Format("\"{0}\"", aCellValue.ToString().TrimEnd(new char[] { '\n', '\r' }));
            }
            return aCellValue.ToString().TrimEnd(new char[] { '\n', '\r' });
        }
        private string GetCSVReportStringFromData(DataTable aDataTable)
        {
            var CSVBuilder = new StringBuilder();
            CSVBuilder.AppendLine(string.Join(",", aDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName)));
            CSVBuilder.AppendLine(string.Join(Environment.NewLine, aDataTable.Rows.Cast<DataRow>().Select(r => string.Join(",", r.ItemArray.Select(x => GetCellStringValue(x)).ToArray()))));
            return CSVBuilder.ToString();
        }

        // 20200722 TNHX: Thêm cột mã thuốc + dời cột đơn giá + giá vốn lên trước
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
                if (criteria.DrugID > 0)
                {
                    cmd.AddParameter("@DrugID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DrugID));
                }
                cmd.CommandTimeout = int.MaxValue;
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.Fill(dsExportToExcellAll);
                dsExportToExcellAll = ExecuteDataSet(cmd);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                string newGuid = criteria.StoreName;

                if (dsExportToExcellAll.Tables.Count == 0)
                    return null;

                DataTable mTable = new DataTable();
                mTable.Columns.Add("DrugID", typeof(string));
                mTable.Columns.Add("BrandName", typeof(string));
                mTable.Columns.Add("Code", typeof(string));
                mTable.Columns.Add("InCost", typeof(decimal));
                mTable.Columns.Add("OutPrice", typeof(decimal));
                List<string[]> mDrugsContents = new List<string[]>();
                foreach (var item in dsExportToExcellAll.Tables[0].Rows.Cast<DataRow>().OrderBy(x => x["BrandName"]))
                {
                    if (mDrugsContents.Any(x => x[0] == item["DrugID"].ToString() && x[2] == item["InCost"].ToString() && x[3] == item["OutPrice"].ToString()))
                    {
                        continue;
                    }
                    mDrugsContents.Add(new string[] { item["DrugID"].ToString(), item["BrandName"].ToString()
                        , item["InCost"] == null || item["InCost"] == DBNull.Value ? "0" : item["InCost"].ToString()
                        , item["OutPrice"] == null || item["OutPrice"] == DBNull.Value ? "0" : item["OutPrice"].ToString()
                        , item["Code"].ToString()});
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
                foreach (var item in mDrugsContents)
                {
                    var mRow = mTable.NewRow();
                    mRow["DrugID"] = item[0];
                    mRow["BrandName"] = item[1];
                    mRow["InCost"] = Convert.ToDecimal(item[2]);
                    mRow["OutPrice"] = Convert.ToDecimal(item[3]);
                    mRow["Code"] = item[4];
                    mTable.Rows.Add(mRow);
                }
                int mSkipColumnNumb = 5;
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

                //adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        //▼====: #014
        private List<List<string>> OutwardDrugClinicDeptsByStaffStatisticDetails_TPReportString(string storeName, ReportParameters criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand(storeName, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                cmd.CommandTimeout = int.MaxValue;
                dsExportToExcellAll = ExecuteDataSet(cmd);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                string newGuid = criteria.StoreName;

                if (dsExportToExcellAll.Tables.Count == 0)
                    return null;

                DataTable mTable = new DataTable();
                mTable.Columns.Add("DrugID", typeof(string));
                mTable.Columns.Add("BrandName", typeof(string));
                mTable.Columns.Add("Code", typeof(string));
                mTable.Columns.Add("InCost", typeof(decimal));
                List<string[]> mDrugsContents = new List<string[]>();
                foreach (var item in dsExportToExcellAll.Tables[0].Rows.Cast<DataRow>().OrderBy(x => x["BrandName"]))
                {
                    if (mDrugsContents.Any(x => x[0] == item["DrugID"].ToString() && x[2] == item["InCost"].ToString()))
                    {
                        continue;
                    }
                    mDrugsContents.Add(new string[] { item["DrugID"].ToString(), item["BrandName"].ToString()
                        , item["InCost"] == null || item["InCost"] == DBNull.Value ? "0" : item["InCost"].ToString()
                        , item["Code"].ToString()});
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
                foreach (var item in mDrugsContents)
                {
                    var mRow = mTable.NewRow();
                    mRow["DrugID"] = item[0];
                    mRow["BrandName"] = item[1];
                    mRow["InCost"] = Convert.ToDecimal(item[2]);
                    mRow["Code"] = item[3];
                    mTable.Rows.Add(mRow);
                }
                int mSkipColumnNumb = 4;
                foreach (var item in dsExportToExcellAll.Tables[0].Rows.Cast<DataRow>().OrderBy(x => x["DrugID"]))
                {
                    var mColumnIndex = mStaffsContents.IndexOf(mStaffsContents.FirstOrDefault(x => x[0] == item["StaffID"].ToString())) * 2;
                    var mCurrentQty = mTable.Rows.Cast<DataRow>().FirstOrDefault(x => x["DrugID"].ToString() == item["DrugID"].ToString() && x["InCost"].ToString() == item["InCost"].ToString())[mColumnIndex + mSkipColumnNumb];
                    var mCurrentAmount = mTable.Rows.Cast<DataRow>().FirstOrDefault(x => x["DrugID"].ToString() == item["DrugID"].ToString() && x["InCost"].ToString() == item["InCost"].ToString())[mColumnIndex + mSkipColumnNumb + 1];
                    mTable.Rows.Cast<DataRow>().FirstOrDefault(x => x["DrugID"].ToString() == item["DrugID"].ToString() && x["InCost"].ToString() == item["InCost"].ToString())[mColumnIndex + mSkipColumnNumb] = Convert.ToDouble(item["OutQuantity"]) + Convert.ToDouble(mCurrentQty == null || mCurrentQty == DBNull.Value ? 0 : mCurrentQty);
                    mTable.Rows.Cast<DataRow>().FirstOrDefault(x => x["DrugID"].ToString() == item["DrugID"].ToString() && x["InCost"].ToString() == item["InCost"].ToString())[mColumnIndex + mSkipColumnNumb + 1] = Convert.ToDecimal(item["OutAmount"]) + Convert.ToDecimal(mCurrentAmount == null || mCurrentAmount == DBNull.Value ? 0 : mCurrentAmount);
                }
                mTable.Columns.RemoveAt(0);
                mTable.Columns["BrandName"].ColumnName = "Tên thuốc";
                mTable.Columns["InCost"].ColumnName = "Giá vốn";

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

                //adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }
        //▲====: #014

        
        public List<List<string>> ExportToExcellAllGeneric(ReportParameters criteria)
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
                    //▼===== #007
                    if (!criteria.IsFullDetails)
                    {
                        return getReportString("spRpt_CreateTemp21_New_Excel", criteria);
                    }
                    else
                    {
                        return getReportString("spRpt_CreateTemp21_New_Excel_BV", criteria);
                    }
                    //▲===== #007
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
                    //return getReportString("spRptGetConsultingDiagnosys_Excel", criteria);
                    return getReportString("spRptGetConsultingDiagnosys_Excel_New", criteria);
                case ReportName.FollowICD:
                    if (criteria.IsNewForm)
                    {
                        return getReportString("spRptHISSummaryFollowByICD_Bieu14_Excel", criteria);
                    }
                    else
                    {
                        return getReportString("spGetHISSummaryByICD_New_Excel", criteria);
                    }
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

                /*▼==== DuyNH 20/01/2021 Tạo 3 báo cáo chuyển tuyến KHTH*/
                
                case ReportName.BCChiTietBNChuyenDi:
                    return getReportString("spTransferFormData", criteria);
                
                case ReportName.BCChiTietBNChuyenDen:
                    return getReportString("spTransferFormData", criteria);
                
                case ReportName.BCCongTacChuyenTuyen:
                    return getReportString("spTransferFormType5Rpt", criteria);
                /*▲====*/

                /*▼==== DuyNH 07/04/2021 Tạo báo cáo danh sách dịch vụ có trên HIS*/

                case ReportName.BC_DS_DichVuKyThuatTrenHIS:
                    return getReportString("spBC_DS_DichVuKyThuatTrenHIS", criteria);
                /*▲====*/

                /*▼==== DatTB 02/06/2021 Tạo báo cáo danh mục kỹ thuật mới*/

                case ReportName.BC_DM_KyThuatMoi:
                    return getReportString("spBC_DM_KyThuatMoi", criteria);
                /*▲====*/

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
                case ReportName.OutwardDrugsByStaffStatisticDetails:
                    return GetOutwardDrugsByStaffStatisticReportString("spBonusByOutwardDrugsStatistic", criteria);
                //▼====: #014
                case ReportName.OutwardDrugClinicDeptsByStaffStatisticDetails_TP:
                    return OutwardDrugClinicDeptsByStaffStatisticDetails_TPReportString("spOutwardDrugClinicDeptsByStaffStatistic_TP", criteria);
                //▲====: #014
                case ReportName.KTDoanhThu_TEMP80a:
                     return getReportString("spRpt_ktdoanhthu_CreateTemplate80a_Excel_BV", criteria);
                case ReportName.BCHuyDTDT:
                     return getReportString("spXRpt_BaoCaoHuyDTDT", criteria);                
                default: return null;
            }
        }

        public string ExportToCSVAllGeneric(ReportParameters criteria)
        {
            switch (criteria.reportName)
            {
                case ReportName.TEMP79a_CHITIET:
                case ReportName.TEMP79a_TONGHOP:
                    {
                        if (criteria.IsFullDetails && !criteria.IsDetail)
                        {
                            return getCSVReportString("spRpt_CreateTemplate79a_Excel_BV", criteria);
                        }
                        else if (criteria.IsFullDetails && criteria.IsDetail)
                        {
                            return getCSVReportString("spRpt_CreateTemplate79a_Excel_Details_BV", criteria);
                        }
                        else if (criteria.Check3360)
                        {
                            return getCSVReportString("spRpt_CreateTemplate79a_3360_Excel", criteria);
                        }
                        else
                        {
                            return getCSVReportString("spRpt_CreateTemplate79a_Excel", criteria);
                        }
                    }
                case ReportName.InfectionCaseStatistics:
                    return getCSVReportString("spGetInfectionCaseAllContentInfo_Excel", criteria);
                case ReportName.TreatsStatisticsByDept:
                    return getCSVReportString("spTreatmentStatisticsByDeptID", criteria);
                case ReportName.TreatsStatisticsByDept_Detail:
                    return getCSVReportString("spTreatmentStatisticsByDeptID_Data", criteria);
                case ReportName.PatientQuotation:
                    return getCSVReportString("spPatientQuotation_Excel", criteria);
                default: return null;
            }
        }

        //▼====: #002
        public List<List<string>> ExportReportToExcelForAccountant(ReportParameters criteria)
        {
            DataSet mData = GetDataForCreateKTTHReport(criteria);
            return GetReportStringFromData(mData);
        }

        public DataSet GetDataForCreateKTTHReport(ReportParameters criteria)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = null;
                    switch (criteria.reportName)
                    {
                        case ReportName.BangKeBacSiThucHienCLS:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BangKeBacSiThucHienCLS_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            cmd.AddParameter("@Settlement", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.Settlement));
                            break;
                        case ReportName.BCChiTietDV_CLS:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BCChiTietDV_CLS_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.KT_BaoCaoDoanhThuNgTru:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_KT_BaoCaoDoanhThuNgoai_NoiTru_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.ThongKeDsachKBNoiTruTheoBacSi:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_ThongKeDsachBacSiKhamBenh_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.ThongKeDsachKBNgTruTheoBacSi:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_ThongKeDsachBacSiKhamBenhNgTru_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.BangKeBacSiThucHienPT_TT:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BangKeBacSiThucHienPTTT_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.BangKeBanLeHangHoaDV:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BangKeBanLeHangHoaDV_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.KTTH_BC_XUAT_VIEN:
                            cmd = null;
                            cmd = new SqlCommand("sp_ThongKeDsachBNXuatVien_Export", cn);
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.KTTH_BC_CHI_DINH_NHAP_VIEN:
                            cmd = null;
                            cmd = new SqlCommand("sp_ThongKeDsachBsiChiDinhNhapVien_Export", cn);
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.BangKeThuTamUngNT:
                            cmd = null;
                            cmd = new SqlCommand("spRptInPtCashAdvanceStatistics_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                            break;
                        case ReportName.BangKeThuHoanUngNT:
                            cmd = null;
                            cmd = new SqlCommand("spRptInPtPaidCashAdvanceStatistics_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                            break;    
                        //▼====: #003
                        case ReportName.RptTongHopDoanhThuTheoKhoa:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_KT_TongHopDoanhThuTheoKhoa_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                            cmd.AddParameter("@HasDischarge", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.HasDischarge));
                            break;
                        //▲====: #003
                        //▼====: #009
                        case ReportName.BC_BNTaiKhamBenhManTinh:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_BNTaiKhamBenhManTinh_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@PatientAges", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.SearchID));
                            break;
                        //▲====: #009
                        //▼====: #016
                        case ReportName.BC_BenhNhanKhamBenh:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_BenhNhanKhamBenh_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲====: #016
                        //▼====: #017
                        case ReportName.BC_BNHenTaiKhamBenhDacTrung:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_BNHenTaiKhamBenhDacTrung_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲====: #017
                        //▼====: #012
                        case ReportName.BC_ToaThuocHangNgay_DLS:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_ToaThuocHangNgay_DLS_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.BC_ToaThuocHangNgay_KD:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_ToaThuocHangNgay_KD_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.BC_ThuocYCuDoDangCuoiKy:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_ThuocYCuDoDangCuoiKy_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            break;
                        //▲====: #012
                        //▼====: #013
                        case ReportName.BC_NHAP_PTTT_KHOA_PHONG:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BaoCaoNhapPTTTKhoaPhong_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocID));
                            break;
                        //▲====: #013
                        case ReportName.BC_NHAP_PTTT_KHOA_PHONG_KHTH:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BaoCaoNhapPTTTKhoaPhong_KHTH", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocID));
                            break;
                        //▼====: #014
                        case ReportName.BC_NXT_THUOC_TONGHOP:
                            cmd = null;
                            switch (criteria.StoreType)
                            {
                                case (long)AllLookupValues.StoreType.STORAGE_CLINIC:
                                    cmd = new SqlCommand("spRpt_ClinicDept_InOutStocks_ByStoreID_KT_Export", cn);
                                    cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_MedProductType));
                                    cmd.AddParameter("@RefGenDrugCatID_1", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RefGenDrugCatID_1));
                                    cmd.AddParameter("@DrugDeptProductGroupReportTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.SelectedDrugDeptProductGroupReportType));
                                    break;
                                case (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT:
                                    cmd = new SqlCommand("spRpt_DrugDept_InOutStocks_ByStoreID_KT_Export", cn);
                                    cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_MedProductType));
                                    cmd.AddParameter("@RefGenDrugCatID_1", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RefGenDrugCatID_1));
                                    cmd.AddParameter("@DrugDeptProductGroupReportTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.SelectedDrugDeptProductGroupReportType));
                                    break;
                                case (long)AllLookupValues.StoreType.STORAGE_EXTERNAL:
                                    cmd = new SqlCommand("spRpt_InOutStockValue_KT_Export", cn);
                                    break;
                            }
                            
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StoreID));
                            
                            break;
                        case ReportName.XRptSoDienTim:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoDienTim", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoKhamBenh:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoKhamBenh", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoThuThuat:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoThuThuat", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoSieuAm:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoSieuAm", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoXetNghiem:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoXetNghiem", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoXQuang:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoXQuang", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoNoiSoi:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoNoiSoi", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoDoChucNangHoHap:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoDoChucNangHoHap", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲====: #014
                        //▼==== #030
                        case ReportName.XRpt_GioLamThemBacSi:
                            cmd = null;
                            cmd = new SqlCommand("spXRpt_GioLamThemBacSi", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲==== #030
                        default:
                            break;
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cn.Open();
                    var mData = ExecuteDataSet(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mData;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲====: #002
        public List<List<string>> HospitalClientContractDetails_Excel(long HosClientContractID)
        {
            using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spHospitalClientContractDetails_Excel", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@HosClientContractID", SqlDbType.BigInt, ConvertNullObjectToDBNull(HosClientContractID));
                mCommand.CommandTimeout = int.MaxValue;
                mConnection.Open();
                var mTable = ExecuteDataTable(mCommand);
                CleanUpConnectionAndCommand(mConnection, mCommand);
                if (mTable == null || mTable.Rows.Count == 0)
                {
                    return null;
                }
                DataTable ExportTable = new DataTable();
                ExportTable.Columns.AddRange(new DataColumn[] {
                    new DataColumn("STT"),
                    new DataColumn("Mã BN"),
                    new DataColumn("HỌ VÀ TÊN"),
                    new DataColumn("Giới tính"),
                    new DataColumn("Năm sinh"),
                });
                int FreezeColumnCount = 5;
                var AllServiceColumns = mTable.Rows.Cast<DataRow>().Select(x => new
                {
                    MedServiceID = x["MedServiceID"],
                    PCLExamTypeID = x["PCLExamTypeID"],
                    MedServiceName = x["MedServiceName"],
                    UnitPrice = x["UnitPrice"],
                    //▼==== #033
                    DoctorPerform = "Bác sĩ thực hiện " + x["MedServiceName"],
                    //▲==== #033
                }).Distinct().ToList();
                foreach (var aItem in AllServiceColumns)
                {
                    ExportTable.Columns.Add(Convert.ToString(aItem.MedServiceName));
                    //▼==== #033
                    ExportTable.Columns.Add(Convert.ToString(aItem.DoctorPerform));
                    //▲==== #033
                }
                ExportTable.Columns.Add(" ");
                var PriceRow = ExportTable.NewRow();
                for (int i = 0; i < AllServiceColumns.Count; i++)
                {
                    PriceRow[FreezeColumnCount + i] = Convert.ToDecimal(AllServiceColumns[i].UnitPrice).ToString("#,#");
                    //▼==== #037
                    FreezeColumnCount++;
                    //▲==== #037
                }
                ExportTable.Rows.Add(PriceRow);
                var AllPatientRow = mTable.Rows.Cast<DataRow>().Select(x => new
                {
                    PatientCode = x["PatientCode"],
                    FullName = x["FullName"],
                    Gender = x["Gender"],
                    YOB = x["YOB"]
                }).Distinct().ToList();
                for (int p = 0; p < AllPatientRow.Count; p++)
                {
                    //▼==== #033
                    FreezeColumnCount = 5;
                    //▲==== #033
                    var PatientRow = ExportTable.NewRow();
                    PatientRow["STT"] = p + 1;
                    PatientRow["Mã BN"] = AllPatientRow[p].PatientCode;
                    PatientRow["HỌ VÀ TÊN"] = AllPatientRow[p].FullName;
                    PatientRow["Giới tính"] = AllPatientRow[p].Gender;
                    PatientRow["Năm sinh"] = AllPatientRow[p].YOB;
                    decimal TotalAmount = 0;
                    for (int s = 0; s < AllServiceColumns.Count; s++)
                    {
                        DataRow FindRow = null;
                        foreach (var aItem in mTable.Rows.Cast<DataRow>())
                        {
                            if (aItem["PatientCode"].ToString() != AllPatientRow[p].PatientCode.ToString())
                            {
                                continue;
                            }
                            if (aItem["MedServiceID"] != null && aItem["MedServiceID"] != DBNull.Value)
                            {
                                if (AllServiceColumns[s].MedServiceID == null || AllServiceColumns[s].MedServiceID == DBNull.Value)
                                {
                                    continue;
                                }
                                if (Convert.ToInt64(aItem["MedServiceID"]) != Convert.ToInt64(AllServiceColumns[s].MedServiceID))
                                {
                                    continue;
                                }
                                FindRow = aItem;
                                break;
                            }
                            else if (aItem["PCLExamTypeID"] != null && aItem["PCLExamTypeID"] != DBNull.Value)
                            {
                                if (AllServiceColumns[s].PCLExamTypeID == null || AllServiceColumns[s].PCLExamTypeID == DBNull.Value)
                                {
                                    continue;
                                }
                                if (Convert.ToInt64(aItem["PCLExamTypeID"]) != Convert.ToInt64(AllServiceColumns[s].PCLExamTypeID))
                                {
                                    continue;
                                }
                                FindRow = aItem;
                                break;
                            }
                        }
                        if (FindRow != null)
                        {
                            if (Convert.ToBoolean(FindRow["IsUsed"]))
                            {
                                PatientRow[FreezeColumnCount + s] = "1";
                                TotalAmount += Convert.ToDecimal(AllServiceColumns[s].UnitPrice);
                                //▼==== #033
                                FreezeColumnCount++;

                                PatientRow[FreezeColumnCount + s] = Convert.ToString(FindRow["DoctorFullName"]);
                                //▲==== #033 
                            }
                            else
                            {
                                PatientRow[FreezeColumnCount + s] = "";
                                //▼==== #033
                                FreezeColumnCount++;

                                PatientRow[FreezeColumnCount + s] = "";
                                //▲==== #033 
                            }
                        }
                        else
                        {
                            PatientRow[FreezeColumnCount + s] = "-";

                            //▼==== #033
                            FreezeColumnCount++;

                            PatientRow[FreezeColumnCount + s] = "-";
                            //▲==== #033 
                        }
                    }
                    PatientRow[" "] = TotalAmount.ToString("#,#");
                    ExportTable.Rows.Add(PatientRow);
                }
                return GetReportStringFromData(ExportTable);
            }
        }
        public List<List<string>> HospitalClientContractResultDetails_Excel(long HosClientContractID)
        {
            using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spHospitalClientContractResultDetails_Excel", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@HosClientContractID", SqlDbType.BigInt, ConvertNullObjectToDBNull(HosClientContractID));
                mConnection.Open();
                var mTable = ExecuteDataTable(mCommand);
                CleanUpConnectionAndCommand(mConnection, mCommand);
                if (mTable == null || mTable.Rows.Count == 0)
                {
                    return null;
                }
                DataTable ExportTable = new DataTable();
                ExportTable.Columns.AddRange(new DataColumn[] {
                    new DataColumn("STT"),
                    new DataColumn("Mã BN"),
                    new DataColumn("HỌ VÀ TÊN"),
                    new DataColumn("Giới tính"),
                    new DataColumn("Năm sinh"),
                    new DataColumn("Chiều cao"),
                    new DataColumn("Cân nặng"),
                    new DataColumn("BMI"),
                    new DataColumn("Mạch"),
                    new DataColumn("Huyết áp"),
                });
                //int FreezeColumnCount = 10;
                var AllServiceColumns = mTable.Rows.Cast<DataRow>().Select(x => new
                {
                    MedServiceID = x["MedServiceID"],
                    PCLExamTypeID = x["PCLExamTypeID"],
                    //PCLExamTestItemID = x["PCLExamTestItemID"],
                    MedServiceName = x["MedServiceName"],
                    //KetQua = x["KetQua"]
                    //▼==== #033
                    DoctorPerform = "Bác sĩ thực hiện " + x["MedServiceName"],
                    //▲==== #033
                }).Distinct().ToList();
                foreach (var aItem in AllServiceColumns)
                {
                    //if (aItem.PCLExamTestItemID != DBNull.Value)
                    //{
                    //    ExportTable.Columns.Add(Convert.ToString(aItem.PCLExamTestItemID));
                    //}
                    //else
                    //{
                        ExportTable.Columns.Add(Convert.ToString(aItem.MedServiceName));
                    //}
                    //▼==== #033
                    ExportTable.Columns.Add(Convert.ToString(aItem.DoctorPerform));
                    //▲==== #033

                }
                //ExportTable.Columns.Add(" ");
                //var ItemRow = ExportTable.NewRow();
                //for (int i = 0; i < AllServiceColumns.Count; i++)
                //{
                //    if(AllServiceColumns[i].PCLExamTestItemID!= null || AllServiceColumns[i].PCLExamTestItemID != DBNull.Value)
                //    {
                //        ItemRow[FreezeColumnCount + i] = AllServiceColumns[i].MedServiceName.ToString();
                //    }
                //}
                //ExportTable.Rows.Add(ItemRow);
                var AllPatientRow = mTable.Rows.Cast<DataRow>().Select(x => new
                {
                    PatientCode = x["PatientCode"],
                    FullName = x["FullName"],
                    Gender = x["Gender"],
                    YOB = x["YOB"],
                    ChieuCao = x["ChieuCao"],
                    CanNang = x["CanNang"],
                    BMI = x["BMI"],
                    Mach = x["Mach"],
                    HuyetAp = x["HuyetAp"]
                }).Distinct().ToList();
                for (int p = 0; p < AllPatientRow.Count; p++)
                {
                    //▼==== #033
                    int FreezeColumnCount = 10;
                    //▲==== #033
                    var PatientRow = ExportTable.NewRow();
                    PatientRow["STT"] = p + 1;
                    PatientRow["Mã BN"] = AllPatientRow[p].PatientCode;
                    PatientRow["HỌ VÀ TÊN"] = AllPatientRow[p].FullName;
                    PatientRow["Giới tính"] = AllPatientRow[p].Gender;
                    PatientRow["Năm sinh"] = AllPatientRow[p].YOB;
                    PatientRow["Chiều cao"] = AllPatientRow[p].ChieuCao;
                    PatientRow["Cân nặng"] = AllPatientRow[p].CanNang;
                    PatientRow["BMI"] = AllPatientRow[p].BMI;
                    PatientRow["Mạch"] = AllPatientRow[p].Mach;
                    PatientRow["Huyết áp"] = AllPatientRow[p].HuyetAp;
                    //decimal TotalAmount = 0;
                    for (int s = 0; s < AllServiceColumns.Count; s++)
                    {
                        DataRow FindRow = null;
                        foreach (var aItem in mTable.Rows.Cast<DataRow>())
                        {
                            if (aItem["PatientCode"].ToString() != AllPatientRow[p].PatientCode.ToString())
                            {
                                continue;
                            }
                            if (aItem["MedServiceID"] != null && aItem["MedServiceID"] != DBNull.Value)
                            {
                                if (AllServiceColumns[s].MedServiceID == null || AllServiceColumns[s].MedServiceID == DBNull.Value)
                                {
                                    continue;
                                }
                                if (Convert.ToInt64(aItem["MedServiceID"]) != Convert.ToInt64(AllServiceColumns[s].MedServiceID))
                                {
                                    continue;
                                }
                                FindRow = aItem;
                                break;
                            }
                            else if (aItem["PCLExamTypeID"] != null && aItem["PCLExamTypeID"] != DBNull.Value)
                            {
                                if (AllServiceColumns[s].PCLExamTypeID == null || AllServiceColumns[s].PCLExamTypeID == DBNull.Value)
                                {
                                    continue;
                                }
                                if (Convert.ToInt64(aItem["PCLExamTypeID"]) != Convert.ToInt64(AllServiceColumns[s].PCLExamTypeID))
                                {
                                    continue;
                                }
                                FindRow = aItem;
                                break;
                            }
                            //else if (aItem["PCLExamTestItemID"] != null && aItem["PCLExamTestItemID"] != DBNull.Value)
                            //{
                            //    if (AllServiceColumns[s].PCLExamTestItemID == null || AllServiceColumns[s].PCLExamTestItemID == DBNull.Value)
                            //    {
                            //        continue;
                            //    }
                            //    if (Convert.ToInt64(aItem["PCLExamTestItemID"]) != Convert.ToInt64(AllServiceColumns[s].PCLExamTestItemID))
                            //    {
                            //        continue;
                            //    }
                            //    FindRow = aItem;
                            //    break;
                            //}                    
                        }
                        if (FindRow != null)
                        {
                            //if (Convert.ToBoolean(FindRow["KetQua"]))
                            //{
                            PatientRow[FreezeColumnCount + s] = Convert.ToString(FindRow["KetQua"]);

                            //▼==== #033
                            FreezeColumnCount++;

                            PatientRow[FreezeColumnCount + s] = Convert.ToString(FindRow["DoctorFullName"]);
                            //▲==== #033 
                            //TotalAmount += Convert.ToDecimal(AllServiceColumns[s].UnitPrice);
                            //}
                            //else
                            //{
                            // PatientRow[FreezeColumnCount + s] = "";
                            //}
                        }
                        else
                        {
                            PatientRow[FreezeColumnCount + s] = "-";

                            //▼==== #033
                            FreezeColumnCount++;

                            PatientRow[FreezeColumnCount + s] = "-";
                            //▲==== #033 
                        }
                    }
                    //PatientRow[" "] = TotalAmount.ToString("#,#");
                    ExportTable.Rows.Add(PatientRow);
                }
                return GetReportStringFromData(ExportTable);
            }
        }
        public List<List<string>> ExportEInvoiceToExcel(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            DataSet mData = GetRegistrationsForCreateEInvoices(aSeachPtRegistrationCriteria);
            return GetReportStringFromData(mData);
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }


        public  List<List<string>> ExportToExcel_HIReport(ReportParameters criteria)
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
        public void ExportToExcel_HIReport_New(ReportParameters criteria,string filePath)
        {
            string storeName = null;
            switch (criteria.reportName)
            {
                case ReportName.TEMP19_V2:
                    storeName= "spRpt_CreateTemp19VTYTTH_New_Excel_V2";
                    break;
                case ReportName.TEMP20_V2:
                    storeName = "spRpt_CreateTemp20NoiTru_New_Excel_V2";
                    break;
                case ReportName.TEMP21_V2:
                    storeName = "spRpt_CreateTemp21_New_Excel_V2";
                    break;
                case ReportName.TEMP79_V2:
                    storeName = "spRpt_CreateTemplate79a_Excel_V2";
                    break;
                case ReportName.TEMP79_TRATHUOC_V2:
                    storeName = "spRpt_CreateTemplate79aTraThuoc_Excel_V2";
                    break;
                case ReportName.TEMP80_V2:
                    storeName = "spRpt_CreateTemplate80a_Excel_V2";
                    break;
                case ReportName.TEMP9324_BANG_1:
                    storeName = "spGetReport9324_General";
                    break;
                case ReportName.TEMP9324_BANG_2:
                    storeName = "spGetReport9324_Drug";
                    break;
                case ReportName.TEMP9324_BANG_3:
                    storeName = "spGetReport9324_TechnicalServiceAndMedicalMaterial";
                    break;
                default: storeName = "";break;
            }
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand(storeName, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.HIReportID));

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;

                cn.Open();
                var _DataReader = cmd.ExecuteReader();
                WriteExcelFile(_DataReader, "Sheet1", filePath);
                CleanUpConnectionAndCommand(cn, cmd);
                _DataReader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }


        public  List<List<string>> ExportToExcellAll_Temp25a(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        public  List<List<string>> ExportToExcellAll_Temp26a(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }


        public  List<List<string>> ExportToExcellAll_Temp20NgoaiTru(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        public  List<List<string>> ExportToExcellAll_Temp20NoiTru(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        public  List<List<string>> ExportToExcellAll_Temp21NgoaiTru(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        public  List<List<string>> ExportToExcellAll_Temp21NoiTru(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        public  List<List<string>> ExportToExcellAll_ChiTietVienPhi_PK(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        public  List<List<string>> ExportToExcellAll_ChiTietVienPhi(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        public  List<List<string>> ExportToExcellAll_ThongKeDoanhThu(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        public  List<List<string>> ExportToExcellAll_DoanhThuTongHop(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        public  List<List<string>> ExportToExcellAll_DoanhThuNoiTruTongHop(ReportParameters criteria)
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

                adapter.Dispose();
                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

        #endregion

        public  bool FollowUserGetReport(long staffID, string reportName, string reportParams, long v_GetReportMethod)
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
                    
                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool CreateHIReport(HealthInsuranceReport HIReport, out long HIReportID)
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
                    
                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  bool CreateFastReport(HealthInsuranceReport gReport, long V_FastReportType, out long FastReportID)
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
                    mCommand.AddParameter("@V_FastReportType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_FastReportType));
                    mConnection.Open();
                    int ExcCount = mCommand.ExecuteNonQuery();
                    if (mCommand.Parameters["@FastReportID"].Value != null && mCommand.Parameters["@FastReportID"].Value != DBNull.Value)
                        FastReportID = (long)mCommand.Parameters["@FastReportID"].Value;
                    else FastReportID = 0;

                    
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return ExcCount > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool TransferFastReport(long FastReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spTransferFastDataToFastDB", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@FastReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FastReportID));
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (count > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▼===== #004
        public bool DeleteFastReport(long FastReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteFastReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@FastReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FastReportID));
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (count > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲===== #004
        public void DeleteFastLinkedReportDetail(int Case, long FastReportID, string so_ct, bool noi_tru, string ma_kh, string ma_bp, string ma_kho)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spDeleteFastLinkedReportDetail", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@Case", SqlDbType.TinyInt, ConvertNullObjectToDBNull(Case));
                    mCommand.AddParameter("@so_ct", SqlDbType.Char, 12, ConvertNullObjectToDBNull(so_ct), ParameterDirection.Input);
                    mCommand.AddParameter("@noi_tru", SqlDbType.BigInt, noi_tru);
                    mCommand.AddParameter("@FastReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FastReportID));
                    mCommand.AddParameter("@ma_kh", SqlDbType.Char, 8, ConvertNullObjectToDBNull(ma_kh), ParameterDirection.Input);
                    mCommand.AddParameter("@ma_bp", SqlDbType.Char, 8, ConvertNullObjectToDBNull(ma_bp), ParameterDirection.Input);
                    mCommand.AddParameter("@ma_kho", SqlDbType.Char, 16, ConvertNullObjectToDBNull(ma_kho), ParameterDirection.Input);
                    mConnection.Open();
                    mCommand.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  bool DeleteRegistrationHIReport(string ma_lk)
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
                    
                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  IList<PatientRegistration> SearchRegistrationsForHIReport(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase)
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
                    cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PatientFindBy));
                    //▼===== #011
                    cmd.AddParameter("@ViewCase", SqlDbType.Int, ConvertNullObjectToDBNull(ViewCase));
                    //▲===== #011

                    cn.Open();
                    var mReader = cmd.ExecuteReader();
                    List<PatientRegistration> mPatientRegistrationCollection = GetPatientRegistrationCollectionFromReader(mReader);
                    mReader.Close();
                    
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mPatientRegistrationCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  IList<PatientRegistration_V2> SearchRegistrationsForCreateOutPtTransactionFinalization(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
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
                    cmd.AddParameter("@IsExportEInvoiceView", SqlDbType.Bit, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.IsExportEInvoiceView));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.V_RegistrationType));
                    cmd.AddParameter("@ViewCase", SqlDbType.TinyInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ViewCase));
                    cn.Open();
                    var mReader = cmd.ExecuteReader();
                    List<PatientRegistration_V2> mPatientRegistrationCollection = GetPatientRegistration_V2CollectionFromReader(mReader);
                    mReader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mPatientRegistrationCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  IList<OutPtTransactionFinalization> GetTransactionFinalizationSummaryInfos(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
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
                    CleanUpConnectionAndCommand(cn, cmd);
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
        public IList<PatientRegistration_V2> SearchRegistrationsForCreateEInvoices(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSearchRegistrationsForCreateEInvoices", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ToDate));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptLocationID));
                    cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FullName));
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PatientCode));
                    cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PtRegistrationCode));
                    cmd.AddParameter("@IsHasInvoice", SqlDbType.Bit, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.IsHasInvoice));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.V_RegistrationType));
                    cmd.AddParameter("@IsExportData", SqlDbType.Bit, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.IsExportData));
                    //▼====: #035
                    cmd.AddParameter("@ViewCase", SqlDbType.TinyInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ViewCase));
                    //▲====: #035
                    cn.Open();
                    var mReader = cmd.ExecuteReader();
                    List<PatientRegistration_V2> mPatientRegistrationCollection = GetPatientRegistration_V2CollectionFromReader(mReader);
                    mReader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mPatientRegistrationCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet GetRegistrationsForCreateEInvoices(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSearchRegistrationsForCreateEInvoices", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ToDate));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptLocationID));
                    cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FullName));
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PatientCode));
                    cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PtRegistrationCode));
                    cmd.AddParameter("@IsHasInvoice", SqlDbType.Bit, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.IsHasInvoice));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.V_RegistrationType));
                    cmd.AddParameter("@IsExportData", SqlDbType.Bit, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.IsExportData));
                    //▼====: #035
                    cmd.AddParameter("@ViewCase", SqlDbType.TinyInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ViewCase));
                    //▲====: #035
                    cn.Open();
                    var mData = ExecuteDataSet(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mData;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  bool UpdateHIReportStatus(HealthInsuranceReport aHealthInsuranceReport)
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

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mResultValue > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  List<HealthInsuranceReport> GetHIReport()
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return HIReportList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  List<HealthInsuranceReport> GetFastReports(long V_FastReportType)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetFastReports", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@V_FastReportType", SqlDbType.BigInt, V_FastReportType);
                    mCommand.CommandTimeout = int.MaxValue;
                    mConnection.Open();
                    IDataReader mReader = ExecuteReader(mCommand);
                    List<HealthInsuranceReport> mReportCollection = null;
                    mReportCollection = GetHealthInsuranceReportCollectionFromReader(mReader);

                    mReader.Close();
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return mReportCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  DataSet GetFastReportDetails(long FastReportID)
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

                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return mDataSet;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string ConverToStringOnlyForChanDoanXML1(object ten_benh, object LoaiKCB)
        {
            string ReturnValue = "";
            if (ten_benh != null && !string.IsNullOrEmpty((Convert.ToString(ten_benh))))
            {
                if (LoaiKCB != null && !string.IsNullOrEmpty((Convert.ToString(LoaiKCB))))
                {
                    if (Convert.ToString(LoaiKCB) != "2")
                    {
                        ReturnValue = Convert.ToString(ten_benh);
                        return ReturnValue;
                    }
                }
                string[] tmp = Convert.ToString(ten_benh).Split(';');
                var tmpStr = tmp.Distinct();
                if (tmpStr != null && tmpStr.Count() > 0)
                {
                    foreach (var item in tmpStr)
                    {
                        ReturnValue = string.Concat(ReturnValue, ";", item);
                    }
                    if (!string.IsNullOrEmpty(ReturnValue) && ReturnValue.Substring(0, 1) == ";")
                    {
                        ReturnValue = ReturnValue.Substring(1, ReturnValue.Length - 1);
                    }
                }
            }
            return ReturnValue;
        }
        private XElement Get_XElement_TongHop_FromDS(DataSet dsTab1_General, int nTab1RowIdx, int version = 1)
        {
            if (version == 3)
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
                                new XElement("TEN_BENH", ConverToStringOnlyForChanDoanXML1(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ten_benh"], dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_loai_kcb"])),
                                new XElement("MA_BENH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_benh"])),
                                new XElement("MA_BENHKHAC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_benhkhac"])),
                                new XElement("MA_LYDO_VVIEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_lydo_vvien"])),
                                new XElement("MA_NOI_DI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_noi_di"])),
                                new XElement("MA_NOI_DEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_noi_den"])),
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
                                new XElement("TEN_BENH", ConverToStringOnlyForChanDoanXML1(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ten_benh"], dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_loai_kcb"])),
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
            if (version == 2 || version == 3)
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
            if (version == 2 || version == 3)
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
                            new XElement("MA_LK", Convert.ToString(dsTab3_MatAndServices[0])),
                            new XElement("STT", Convert.ToString(dsTab3_MatAndServices[1])),
                            new XElement("MA_DICH_VU", Convert.ToString(dsTab3_MatAndServices[2])),
                            new XElement("MA_VAT_TU", Convert.ToString(dsTab3_MatAndServices[3])),
                            new XElement("MA_NHOM", Convert.ToString(dsTab3_MatAndServices[4])),
                            new XElement("TEN_DICH_VU", Convert.ToString(dsTab3_MatAndServices[5])),
                            new XElement("DON_VI_TINH", Convert.ToString(dsTab3_MatAndServices[6])),
                            new XElement("SO_LUONG", Convert.ToString(dsTab3_MatAndServices[7])),
                            new XElement("DON_GIA", Convert.ToString(dsTab3_MatAndServices[8])),
                            /*▼====: #002*/
                            //new XElement("TYLE_TT",     "100"),
                            new XElement("TYLE_TT", Convert.ToString(dsTab3_MatAndServices[9])),
                            /*▲====: #002*/
                            new XElement("THANH_TIEN", Convert.ToString(dsTab3_MatAndServices[10])),
                            new XElement("MA_KHOA", Convert.ToString(dsTab3_MatAndServices[11])),
                            new XElement("MA_BAC_SI", Convert.ToString(dsTab3_MatAndServices[12])),
                            new XElement("MA_BENH", Convert.ToString(dsTab3_MatAndServices[13])),
                            new XElement("NGAY_YL", Convert.ToString(dsTab3_MatAndServices[14])),
                            new XElement("NGAY_KQ", Convert.ToString(dsTab3_MatAndServices[15])),
                            new XElement("MA_PTTT", Convert.ToString(dsTab3_MatAndServices[16]))
                        );
        }

        private XElement Get_XElement_PCLResults_FromDS(DataRow dsTab4_PCLResult)
        {
            return new XElement(
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

        private XElement Get_XElement_PCLHappenings_FromDS(DataRow dsTab5_PCLHappening)
        {
            return new XElement(
                "CHI_TIET_DIEN_BIEN_BENH",
                new XElement("MA_LK", Convert.ToString(dsTab5_PCLHappening["ma_lk"])),
                new XElement("STT", Convert.ToString(dsTab5_PCLHappening["stt"])),
                new XElement("DIEN_BIEN", Convert.ToString(dsTab5_PCLHappening["dien_bien"])),
                new XElement("HOI_CHAN", Convert.ToString(dsTab5_PCLHappening["hoi_chan"])),
                new XElement("PHAU_THUAT", Convert.ToString(dsTab5_PCLHappening["phau_thuat"])),
                new XElement("NGAY_YL", Convert.ToString(dsTab5_PCLHappening["ngay_yl"]))
            );
        }

        private int GetXmlElementsForAll3Tabs(long nHIReportID, XElement elemDANHSACH_AllHOSO_InReport)
        {
            DataSet dsTab1_General = new DataSet();
            DataTable dsTab2_Drug = new DataTable();
            DataTable dsTab3_MatAndServices = new DataTable();
            DataTable dsTab4_PCLResults = new DataTable();
            DataTable dsTab5_PCLHappening = new DataTable();

            int nTotalNumRowInTab1 = 0;

            // Txd the following 3 totals only for debuging purpose
            int nTotXml1 = 0;
            int nTotXml2 = 0;
            int nTotXml3 = 0;
            int nTotXml4 = 0;
            int nTotXml5 = 0;
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

                SqlCommand cmdTab5PCLHs = new SqlCommand("spGetReport9324_PCLHappening", cn);
                cmdTab5PCLHs.CommandType = CommandType.StoredProcedure;
                cmdTab5PCLHs.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab5PCLHs.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab5PCLHs = new SqlDataAdapter(cmdTab5PCLHs);
                adapterTab5PCLHs.Fill(dsTab5_PCLHappening);

                nTotalNumRowInTab1 = dsTab1_General.Tables[0].Rows.Count;

                for (int nTab1RowIdx = 0; nTab1RowIdx < nTotalNumRowInTab1; ++nTab1RowIdx)
                {
                    string tab1_MaLK = Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][0]);

                    XElement elemHoSo_ForEach_LanKham = new XElement("HOSO");

                    XElement elemFileHoSo_Tab1_Xml1 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML1"));

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

                    XElement elemFileHoSo_MatSvcTab3_Xml3 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML3"));
                    XElement elemMatSvcListDetails = new XElement("DSACH_CHI_TIET_DVKT");

                    DataRow[] matSvcRows = null;
                    if (dsTab3_MatAndServices != null && dsTab3_MatAndServices.Rows.Count > 0)
                    {
                        matSvcRows = dsTab3_MatAndServices.Select(strWhere);
                    }
                    if (matSvcRows != null && matSvcRows.Count() > 0)
                    {
                        foreach (DataRow matSvcRow in matSvcRows)
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

                    DataRow[] PCLRows = null;
                    if (dsTab4_PCLResults != null && dsTab4_PCLResults.Rows.Count > 0)
                    {
                        PCLRows = dsTab4_PCLResults.Select(strWhere);
                    }
                    if (PCLRows != null && PCLRows.Length > 0)
                    {
                        XElement elemFileHoSo_PCLResultsTab4_Xml4 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML4"));
                        XElement elemPCLResultListDetails = new XElement("DSACH_CHI_TIET_CLS");

                        foreach (DataRow aRow in PCLRows)
                        {
                            XElement elemPCLResultDetails = Get_XElement_PCLResults_FromDS(aRow);
                            elemPCLResultListDetails.Add(elemPCLResultDetails);
                        }
                        elemFileHoSo_PCLResultsTab4_Xml4.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemPCLResultListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_PCLResultsTab4_Xml4);
                        nTotXml4++;
                    }

                    DataRow[] PCLHappeningRows = null;
                    if (dsTab5_PCLHappening != null && dsTab5_PCLHappening.Rows.Count > 0)
                    {
                        PCLHappeningRows = dsTab5_PCLHappening.Select(strWhere);
                    }
                    if (PCLHappeningRows != null && PCLHappeningRows.Length > 0)
                    {
                        XElement elemFileHoSo_PCLResultsTab5_Xml5 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML5"));
                        XElement elemPCLHappeningListDetails = new XElement("DSACH_CHI_TIET_DIEN_BIEN_BENH");

                        foreach (DataRow aRow in PCLHappeningRows)
                        {
                            XElement elemPCLHappeningDetails = Get_XElement_PCLHappenings_FromDS(aRow);
                            elemPCLHappeningListDetails.Add(elemPCLHappeningDetails);
                        }
                        elemFileHoSo_PCLResultsTab5_Xml5.Add(new XElement("NOIDUNGFILE", Convert.ToBase64String(Encoding.UTF8.GetBytes(elemPCLHappeningListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_PCLResultsTab5_Xml5);
                        nTotXml5++;
                    }
                    elemDANHSACH_AllHOSO_InReport.Add(elemHoSo_ForEach_LanKham);
                }

                adapterTab1Gen.Dispose();
                adapterTab2Drug.Dispose();
                adapterTab3Svcs.Dispose();
                adapterTab4PCLs.Dispose();
                adapterTab5PCLHs.Dispose();
                CleanUpConnectionAndCommand(cn, null);
            }

            Debug.WriteLine("==========>   Total number XML1 = {0} - XML2 = {1} - XML3 = {2} - XML4 = {3} - XML5 = {4}", nTotXml1, nTotXml2, nTotXml3, nTotXml4, nTotXml5);

            return nTotalNumRowInTab1;
        }

        public  Stream GetHIXmlReport9324_AllTab123_InOneRpt_Data(long nHIReportID)
        {
            XNamespace xmlns = XNamespace.Get("http://ns.hr-xml.org/2007-04-15");
            XNamespace xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");


            XDocument XD1 = new XDocument();
            XElement GIAMDINHHS = new XElement("GIAMDINHHS",
                                    new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                                    new XAttribute(XNamespace.Xmlns + "xsi", xsi));

            XD1.Add(GIAMDINHHS);
            //▼===== #005
            string HospitalCode = Globals.AxServerSettings.Hospitals.HospitalCode;
            XElement TTDV = new XElement("THONGTINDONVI", new XElement("MACSKCB", HospitalCode));
            //XElement TTDV = new XElement("THONGTINDONVI", new XElement("MACSKCB", "95076"));
            //▲===== #005

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
        public  HOSPayment EditHOSPayment(HOSPayment aHOSPayment)
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

                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return aHOSPayment;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  bool DeleteHOSPayment(long aHOSPaymentID)
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
                    bool bRes = mCommand.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return bRes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  IList<HOSPayment> GetHOSPayments(DateTime aStartDate, DateTime aEndDate)
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

                    IDataReader reader = mCommand.ExecuteReader();
                    var mResult = GetHOSPaymentCollectionFromReader(reader);

                    reader.Close();
                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    
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
        //▼==== #015
        public DataSet PreviewHIReport_ForKHTH(long PtRegistrationID, long V_RegistrationType, out string ErrText)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPreviewHIReportDetail_ForKHTH", cn);
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

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mDataSet;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲==== #015
        public DataSet PreviewHIReport(long PtRegistrationID, long V_RegistrationType, long? OutPtTreatmentProgramID, out string ErrText)
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
                    cmd.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, OutPtTreatmentProgramID);
                    cmd.AddParameter("@ErrText", SqlDbType.NVarChar, -1, ParameterDirection.Output);
                    cn.Open();
                    DataSet mDataSet = ExecuteDataSet(cmd);
                    ErrText = null;
                    var OutErrText = cmd.Parameters.Cast<SqlParameter>().FirstOrDefault(x => x.ParameterName == "@ErrText");
                    if (OutErrText != null && OutErrText.Value != null && OutErrText.Value != DBNull.Value)
                    {
                        ErrText = OutErrText.Value.ToString();
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mDataSet;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //▼====: #001
        public List<List<string>> ExportExcelRegistrationsForHIReportWaitConfirm(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                DataSet dsExportToExcellAll = new DataSet();
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spExportExcelRegistrationsForHIReportWaitConfirm", cn);
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

                    CleanUpConnectionAndCommand(cn, cmd);
                    return returnAllExcelData;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<List<string>> ExportExcelRegistrationsForHIReportOther(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                DataSet dsExportToExcellAll = new DataSet();
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spExportExcelRegistrationsForHIReportOther", cn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = int.MaxValue
                    };
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ToDate));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptLocationID));
                    cmd.AddParameter("@IsReported", SqlDbType.Bit, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.IsReported));
                    cmd.AddParameter("@V_TreatmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.V_TreatmentType));

                    cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FullName));
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PatientCode));
                    cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PtRegistrationCode));

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

                    CleanUpConnectionAndCommand(cn, cmd);
                    return returnAllExcelData;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲====: #001
        public List<RptOutPtTransactionFinalizationDetail> GetRptOutPtTransactionFinalizationDetails(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spRptOutPtTransactionFinalizationDetails", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandTimeout = int.MaxValue;
                    mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                    mCommand.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
                    mConnect.Open();
                    IDataReader reader = mCommand.ExecuteReader();
                    var mResult = GetRptOutPtTransactionFinalizationDetailCollectionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return mResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DQGReport CreateDQGReport(DQGReport aDQGReport)
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spCreateDQGReport", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandTimeout = int.MaxValue;
                    mCommand.AddParameter("@FromDate", SqlDbType.DateTime, aDQGReport.FromDate);
                    mCommand.AddParameter("@ToDate", SqlDbType.DateTime, aDQGReport.ToDate);
                    mCommand.AddParameter("@StaffID", SqlDbType.BigInt, aDQGReport.CreatedStaff == null ? (long?)null : aDQGReport.CreatedStaff.StaffID);
                    mCommand.AddParameter("@Title", SqlDbType.NVarChar, aDQGReport.Title);
                    mConnect.Open();
                    IDataReader reader = mCommand.ExecuteReader();
                    DQGReport mResult = GetDQGReportWithDetailFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return mResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật lại các mã code liên thông với dữ liệu Dược quốc gia vào hệ thống
        /// </summary>
        /// <param name="aDQGReport">Chi tiết cập nhật</param>
        /// <param name="aCase">Trường hợp cập nhật (0: Cập nhật phiếu nhập)</param>
        /// <returns>Trả về giá trị true or false = cập nhật thành công hay thất bại</returns>
        public bool UpdateCodeDQGReport(DQGReport aDQGReport, byte aCase)
        {
            if (aDQGReport == null)
            {
                throw new Exception(eHCMSResources.T0074_G1_I);
            }
            if (aCase == 0 && (aDQGReport.phieu_nhap == null || aDQGReport.phieu_nhap.Count != 1))
            {
                throw new Exception(eHCMSResources.T0074_G1_I);
            }
            if (aCase == 1 && (aDQGReport.don_thuoc == null || aDQGReport.don_thuoc.Count != 1))
            {
                throw new Exception(eHCMSResources.T0074_G1_I);
            }
            if (aCase == 2 && (aDQGReport.hoa_don == null || aDQGReport.hoa_don.Count != 1))
            {
                throw new Exception(eHCMSResources.T0074_G1_I);
            }
            if (aCase == 3 && (aDQGReport.phieu_xuat == null || aDQGReport.phieu_xuat.Count != 1))
            {
                throw new Exception(eHCMSResources.T0074_G1_I);
            }
            if (aCase == 4 && aDQGReport.DQGReportID == 0)
            {
                throw new Exception(eHCMSResources.T0074_G1_I);
            }
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spUpdateCodeDQGReport", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@aCase", SqlDbType.TinyInt, aCase);
                    if (aCase == 0)
                    {
                        mCommand.AddParameter("@id_phieu_nhap", SqlDbType.BigInt, aDQGReport.phieu_nhap.First().id_phieu_nhap);
                        mCommand.AddParameter("@ma_phieu_nhap_quoc_gia", SqlDbType.VarChar, aDQGReport.phieu_nhap.First().ma_phieu_nhap_quoc_gia);
                    }
                    else if (aCase == 1)
                    {
                        mCommand.AddParameter("@id_don_thuoc", SqlDbType.BigInt, aDQGReport.don_thuoc.First().id_don_thuoc);
                        mCommand.AddParameter("@ma_don_thuoc_quoc_gia", SqlDbType.VarChar, aDQGReport.don_thuoc.First().ma_don_thuoc_quoc_gia);
                    }
                    else if (aCase == 2)
                    {
                        mCommand.AddParameter("@id_hoa_don", SqlDbType.BigInt, aDQGReport.hoa_don.First().id_hoa_don);
                        mCommand.AddParameter("@ma_hoa_don_quoc_gia", SqlDbType.VarChar, aDQGReport.hoa_don.First().ma_hoa_don_quoc_gia);
                    }
                    else if (aCase == 3)
                    {
                        mCommand.AddParameter("@id_phieu_xuat", SqlDbType.BigInt, aDQGReport.phieu_xuat.First().id_phieu_xuat);
                        mCommand.AddParameter("@ma_phieu_xuat_quoc_gia", SqlDbType.VarChar, aDQGReport.phieu_xuat.First().ma_phieu_xuat_quoc_gia);
                    }
                    else if (aCase == 4)
                    {
                        mCommand.AddParameter("@DQGReportID", SqlDbType.BigInt, aDQGReport.DQGReportID);
                    }
                    mConnect.Open();
                    var mResult = mCommand.ExecuteNonQuery() > 0;
                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return mResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<DQGReport> GetDQGReports()
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetDQGReports", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mConnect.Open();
                    IDataReader reader = mCommand.ExecuteReader();
                    List<DQGReport> mResult = GetDQGReportCollectionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return mResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DQGReport GetDQGReportWithDetails(long DQGReportID)
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetDQGReportWithDetails", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@DQGReportID", SqlDbType.BigInt, DQGReportID);
                    mConnect.Open();
                    IDataReader reader = mCommand.ExecuteReader();
                    DQGReport mResult = GetDQGReportWithDetailFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return mResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet GetDQGReportAllDetails(long DQGReportID)
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetDQGReportAllDetails", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@DQGReportID", SqlDbType.BigInt, DQGReportID);
                    mConnect.Open();
                    DataSet mResult = ExecuteDataSet(mCommand);
                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return mResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IList<PositionInHospital> GetAllPositionInHospital()
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetAllPositionInHospital", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mConnect.Open();
                    IDataReader reader = mCommand.ExecuteReader();
                    List<PositionInHospital> mResult = GetPositionInHospitalCollectionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return mResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▼===== #008
        public Stream GetXmlReportForCongDLQG(DateTime FromDate, DateTime ToDate)
        {
            XNamespace xmlns = XNamespace.Get("http://ns.hr-xml.org/2007-04-15");
            XNamespace xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");


            XDocument XD1 = new XDocument();
            XElement GIAMDINHHS = new XElement("GIAMDINHHS",
                                    new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                                    new XAttribute(XNamespace.Xmlns + "xsi", xsi));

            XD1.Add(GIAMDINHHS);
            //▼===== #005
            string HospitalCode = Globals.AxServerSettings.Hospitals.HospitalCode;
            XElement TTDV = new XElement("THONGTINDONVI", new XElement("MACSKCB", HospitalCode));
            //XElement TTDV = new XElement("THONGTINDONVI", new XElement("MACSKCB", "95076"));
            //▲===== #005

            XElement THONGTINHOSO = new XElement("THONGTINHOSO");
            XD1.Root.Add(TTDV);

            XD1.Root.Add(THONGTINHOSO);

            XElement DANHSACHHOSO = new XElement("DANHSACHHOSO");

            int nTotNumHoSoTrongBC = GetXMLForCongDLQG(FromDate, ToDate, DANHSACHHOSO);

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


            return memStream;
        }

        private int GetXMLForCongDLQG(DateTime FromDate, DateTime ToDate, XElement elemDANHSACH_AllHOSO_InReport)
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
                SqlCommand cmdTab1Gen = new SqlCommand("spGetXMLForCongDLQG_General", cn);
                cmdTab1Gen.CommandType = CommandType.StoredProcedure;
                cmdTab1Gen.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmdTab1Gen.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cmdTab1Gen.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab1Gen = new SqlDataAdapter(cmdTab1Gen);
                adapterTab1Gen.Fill(dsTab1_General);

                SqlCommand cmdTab2Drug = new SqlCommand("spGetXMLForCongDLQG_Drug", cn);
                cmdTab2Drug.CommandType = CommandType.StoredProcedure;
                cmdTab2Drug.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmdTab2Drug.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cmdTab2Drug.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab2Drug = new SqlDataAdapter(cmdTab2Drug);
                adapterTab2Drug.Fill(dsTab2_Drug);

                SqlCommand cmdTab3Svcs = new SqlCommand("spGetXMLForCongDLQG_TechnicalServiceAndMedicalMaterial", cn);
                cmdTab3Svcs.CommandType = CommandType.StoredProcedure;
                cmdTab3Svcs.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmdTab3Svcs.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cmdTab3Svcs.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab3Svcs = new SqlDataAdapter(cmdTab3Svcs);
                adapterTab3Svcs.Fill(dsTab3_MatAndServices);

                SqlCommand cmdTab4PCLs = new SqlCommand("spGetXMLForCongDLQG_PCLResult", cn);
                cmdTab4PCLs.CommandType = CommandType.StoredProcedure;
                cmdTab4PCLs.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmdTab4PCLs.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cmdTab4PCLs.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab4PCLs = new SqlDataAdapter(cmdTab4PCLs);
                adapterTab4PCLs.Fill(dsTab4_PCLResults);

                nTotalNumRowInTab1 = dsTab1_General.Tables[0].Rows.Count;

                for (int nTab1RowIdx = 0; nTab1RowIdx < nTotalNumRowInTab1; ++nTab1RowIdx)
                {
                    string tab1_MaLK = Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][0]);

                    XElement elemHoSo_ForEach_LanKham = new XElement("HOSO");

                    XElement elemFileHoSo_Tab1_Xml1 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML1"));

                    XElement elemTongHop = Get_XElement_TongHop_FromDS(dsTab1_General, nTab1RowIdx, Globals.AxServerSettings.CommonItems.CurrentHIReportVersion);

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
                            XElement elemDrugDetails = Get_XElement_DrugDetails_FromDS(drugRow, Globals.AxServerSettings.CommonItems.CurrentHIReportVersion);
                            elemDrugListDetails.Add(elemDrugDetails);
                        }

                        elemFileHoSo_DrugTab2_Xml2.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemDrugListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_DrugTab2_Xml2);
                        nTotXml2++;

                    }

                    XElement elemFileHoSo_MatSvcTab3_Xml3 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML3"));
                    XElement elemMatSvcListDetails = new XElement("DSACH_CHI_TIET_DVKT");

                    DataRow[] matSvcRows = null;
                    if (dsTab3_MatAndServices != null && dsTab3_MatAndServices.Rows.Count > 0)
                    {
                        matSvcRows = dsTab3_MatAndServices.Select(strWhere);
                    }
                    if (matSvcRows != null && matSvcRows.Count() > 0)
                    {
                        foreach (DataRow matSvcRow in matSvcRows)
                        {
                            XElement elemMatSvcDetails = Get_XElement_MatAndService_FromDS(matSvcRow, Globals.AxServerSettings.CommonItems.CurrentHIReportVersion);
                            elemMatSvcListDetails.Add(elemMatSvcDetails);
                        }

                        elemFileHoSo_MatSvcTab3_Xml3.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemMatSvcListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_MatSvcTab3_Xml3);
                        nTotXml3++;
                    }
                    DataRow[] PCLRows = null;
                    if (dsTab4_PCLResults != null && dsTab4_PCLResults.Rows.Count > 0)
                    {
                        PCLRows = dsTab4_PCLResults.Select(strWhere);
                    }
                    if (PCLRows != null && PCLRows.Length > 0)
                    {
                        XElement elemFileHoSo_PCLResultsTab4_Xml4 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML4"));
                        XElement elemPCLResultListDetails = new XElement("DSACH_CHI_TIET_CLS");

                        foreach (DataRow aRow in PCLRows)
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

                adapterTab1Gen.Dispose();
                adapterTab2Drug.Dispose();
                adapterTab3Svcs.Dispose();
                adapterTab4PCLs.Dispose();
                CleanUpConnectionAndCommand(cn, null);

            }
            return nTotalNumRowInTab1;
        }
        //▲===== #008
        //▼===== #010
        public bool MarkHIReportByVAS(long HIReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spMarkHIReportByVAS", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@HIReportID", SqlDbType.BigInt, HIReportID);

                    cn.Open();
                    int count = cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲===== #010
        public IList<DiseasesReference> GetTreatmentStatisticsByDepartment(long DeptID, DateTime FromDate, DateTime ToDate, byte RegistrationStatus, out decimal OutTotalQuantity)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spTreatmentStatisticsByDeptID", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                    CurrentCommand.AddParameter("@FromDate", SqlDbType.DateTime, FromDate);
                    CurrentCommand.AddParameter("@ToDate", SqlDbType.DateTime, ToDate);
                    CurrentCommand.AddParameter("@OutTotalQuantity", SqlDbType.Decimal, DBNull.Value, ParameterDirection.Output);
                    CurrentCommand.AddParameter("@RegistrationStatus", SqlDbType.TinyInt, RegistrationStatus);
                    CurrentConnection.Open();
                    IDataReader CurrentReader = ExecuteReader(CurrentCommand);
                    var CurrentCollection = GetRefDiseasesCollectionFromReader(CurrentReader);
                    CurrentReader.Close();
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    OutTotalQuantity = Convert.ToDecimal(CurrentCommand.Parameters["@OutTotalQuantity"].Value);
                    return CurrentCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IList<PatientRegistration> GetTreatmentStatisticsByDepartmentDetail(long DeptID, DateTime FromDate, DateTime ToDate, byte RegistrationStatus
            , decimal? FilerByTime
            , decimal? FilerByMoney)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spTreatmentStatisticsByDeptID_Data", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.CommandTimeout = int.MaxValue;
                    CurrentCommand.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                    CurrentCommand.AddParameter("@FromDate", SqlDbType.DateTime, FromDate);
                    CurrentCommand.AddParameter("@ToDate", SqlDbType.DateTime, ToDate);
                    CurrentCommand.AddParameter("@FilerByTime", SqlDbType.Decimal, ConvertNullObjectToDBNull(FilerByTime));
                    CurrentCommand.AddParameter("@FilerByMoney", SqlDbType.Decimal, ConvertNullObjectToDBNull(FilerByMoney));
                    CurrentCommand.AddParameter("@RegistrationStatus", SqlDbType.TinyInt, RegistrationStatus);
                    CurrentConnection.Open();
                    IDataReader CurrentReader = ExecuteReader(CurrentCommand);
                    var CurrentCollection = GetPatientRegistrationCollectionFromReader(CurrentReader);
                    CurrentReader.Close();
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    return CurrentCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CreateHIReport_OutPt(HealthInsuranceReport HIReport, out long HIReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCreateHIReport_OutPt_ByListPtRegID", cn);
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
                    cmd.AddParameter("@IsAutoCreateHIReport", SqlDbType.Bit, ConvertNullObjectToDBNull(HIReport.IsAutoCreateHIReport));
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();

                    if (cmd.Parameters["@HIReportID"].Value != null && cmd.Parameters["@HIReportID"].Value != DBNull.Value)
                        HIReportID = (long)cmd.Parameters["@HIReportID"].Value;
                    else HIReportID = 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CreateHIReport_InPt(HealthInsuranceReport HIReport, out long HIReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCreateHIReport_InPt_ByListPtRegID", cn);
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

                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void ExportToExcellAllGeneric_New(ReportParameters criteria,string filePath)
        {
            switch (criteria.reportName)
            {
                case ReportName.TEMP25a_CHITIET:
                    ExportToExcel("spRpt_CreateTemplate25a_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP25aTRATHUOC_CHITIET:
                    ExportToExcel("spRpt_CreateTemplate25aTraThuoc_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP26a_CHITIET:
                    ExportToExcel("spRpt_CreateTemplate26a_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP20_NGOAITRU:
                    ExportToExcel("spRpt_CreateTemp20NgoaiTru_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP20_NGOAITRU_TRATHUOC:
                    ExportToExcel("spRpt_CreateTemp20NgoaiTruTraThuoc_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP20_NOITRU:
                    ExportToExcel("spRpt_CreateTemp20NoiTru_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP20_VTYTTH:
                    ExportToExcel("spRpt_CreateTemp20VTYTTH_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP21_NGOAITRU:
                    ExportToExcel("spRpt_CreateTemp21NgoaiTru_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP21_NOITRU:
                    ExportToExcel("spRpt_CreateTemp21NoiTru_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP19:
                    ExportToExcel("spRpt_CreateTemp19VTYTTH_New_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP20_NOITRU_NEW:
                    ExportToExcel("spRpt_CreateTemp20NoiTru_New_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TEMP21_NEW:
                    if (!criteria.IsFullDetails)
                    { 
                        ExportToExcel("spRpt_CreateTemp21_New_Excel", criteria, "sheet1", filePath);
                        return;
                    }
                    else
                    {
                        ExportToExcel("spRpt_CreateTemp21_New_Excel_BV", criteria, "sheet1", filePath);
                        return;
                    }
                //case ReportName.TEMP79a_CHITIET:
                //    return getReportString("spRpt_CreateTemplate79a_Excel", criteria);
                case ReportName.TEMP79a_TONGHOP:
                    {
                        if (criteria.IsFullDetails && !criteria.IsDetail)
                        {
                            ExportToExcel("spRpt_CreateTemplate79a_Excel_BV", criteria, "sheet1", filePath);
                            return;
                        }
                        else if (criteria.IsFullDetails && criteria.IsDetail)
                        {
                            ExportToExcel("spRpt_CreateTemplate79a_Excel_Details_BV", criteria, "sheet1", filePath);
                            return;
                        }
                        else if (criteria.Check3360)
                        {
                            ExportToExcel("spRpt_CreateTemplate79a_3360_Excel", criteria, "sheet1", filePath);
                            return;
                        }
                        else
                        {
                            ExportToExcel("spRpt_CreateTemplate79a_Excel", criteria, "sheet1", filePath);
                            return;
                        }
                    }
                case ReportName.TEMP79aTRATHUOC_CHITIET:
                    ExportToExcel("spRpt_CreateTemplate79aTraThuoc_Excel", criteria, "sheet1", filePath);
                    return;
                //case ReportName.TEMP80a_CHITIET:
                //    return getReportString("spRpt_CreateTemplate80a_Excel", criteria);
                case ReportName.TEMP80a_CHITIET:
                case ReportName.TEMP80a_TONGHOP:
                    {
                        if (criteria.IsFullDetails && !criteria.IsDetail)
                        {
                            ExportToExcel("spRpt_CreateTemplate80a_Excel_BV", criteria, "sheet1", filePath);
                            return;
                        }
                        else if (criteria.IsFullDetails && criteria.IsDetail)
                        {
                            ExportToExcel("spRpt_CreateTemplate80a_Excel_Details_BV", criteria, "sheet1", filePath);
                            return;
                        }
                        else if (criteria.Check3360)
                        {
                            ExportToExcel("spRpt_CreateTemplate80a_3360_Excel", criteria, "sheet1", filePath);
                            return;
                        }
                        else
                        {
                            ExportToExcel("spRpt_CreateTemplate80a_Excel", criteria, "sheet1", filePath);
                            return;
                        }
                    }
                case ReportName.TRANSACTION_VIENPHICHITIET_PK:
                    ExportToExcel("spRpt_BaoCaoChiTietVienPhi_PK_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TRANSACTION_VIENPHICHITIET:
                    ExportToExcel("spRpt_BaoCaoChiTietVienPhi_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.THONGKEDOANHTHU:
                    ExportToExcel("spRpt_ThongKeDoanhThu_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.RptTongHopDoanhThu:
                    ExportToExcel("spRptDoanhThuTongHop_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.RptTongHopDoanhThuNoiTru:
                    ExportToExcel("spRptTotalInPtRevenue_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.REPORT_INPATIENT:
                    ExportToExcel("sp_RptInPtCaredForAndDischarged_AdminOffice", criteria, "sheet1", filePath);
                    return;
                case ReportName.REPORT_GENERAL_TEMP02:
                    ExportToExcel("sp_RptGeneralTemp02", criteria, "sheet1", filePath);
                    return;
                case ReportName.HOSPITAL_FEES_REPORT:
                    ExportToExcel("sp_Report_HospitalFees", criteria, "sheet1", filePath);
                    return;
                case ReportName.HIGH_TECH_FEES_REPORT:
                    ExportToExcel("sp_Report_HighTechFees", criteria, "sheet1", filePath);
                    return;
                case ReportName.OUT_MEDICAL_MATERIAL_REPORT:
                    ExportToExcel("sp_Report_HighTechMat", criteria, "sheet1", filePath);
                    return;
                case ReportName.HIGH_TECH_FEES_CHILD_REPORT:
                    ExportToExcel("sp_Report_HospitalFees_Child", criteria, "sheet1", filePath);
                    return;
                case ReportName.CONSULTINGDIAGNOSYSHISTORY:
                    ExportToExcel("spRptGetConsultingDiagnosys_Excel_New", criteria, "sheet1", filePath);
                    return;
                case ReportName.FollowICD:
                    if (criteria.IsNewForm)
                    {
                        ExportToExcel("spRptHISSummaryFollowByICD_Bieu14_Excel", criteria, "sheet1", filePath);
                        return;
                    }
                    else
                    {
                        ExportToExcel("spGetHISSummaryByICD_New_Excel", criteria, "sheet1", filePath);
                        return;
                    }
                case ReportName.EmployeesReport:
                    ExportToExcel("spHISStaffInfos_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TransferFormType2Rpt:
                    ExportToExcel("spTransferFormType2Rpt", criteria, "sheet1", filePath);
                    return;
                case ReportName.TransferFormType2_1Rpt:
                    ExportToExcel("spTransferFormType2_1Rpt", criteria, "sheet1", filePath);
                    return;
                case ReportName.TransferFormType5Rpt:
                    ExportToExcel("spTransferFormType5Rpt", criteria, "sheet1", filePath);
                    return;
                    //-22-1-2021 duynh 
                case ReportName.BCChiTietBNChuyenDi:
                    ExportToExcel("spTransferFormData", criteria, "sheet1", filePath);
                    return;
                case ReportName.BCChiTietBNChuyenDen:
                    ExportToExcel("spTransferFormData", criteria, "sheet1", filePath);
                    return;
                case ReportName.BCCongTacChuyenTuyen:
                    ExportToExcel("spTransferFormType5Rpt", criteria, "sheet1", filePath);
                    return;

                /*▼==== DuyNH 07/04/2021 Tạo báo cáo danh sách dịch vụ có trên HIS*/

                case ReportName.BC_DS_DichVuKyThuatTrenHIS:
                    ExportToExcel("spBC_DS_DichVuKyThuatTrenHIS", criteria, "sheet1", filePath);
                    return;
                /*▲====*/
                /*▼==== DatTB 02/06/2021 Tạo báo cáo danh mục kỹ thuật mới*/
                case ReportName.BC_DM_KyThuatMoi:
                    ExportToExcel("spBC_DM_KyThuatMoi", criteria, "sheet1", filePath);
                    return;
                /*▲====*/

                case ReportName.RptDrugMedDept:
                    ExportToExcel("spSoKiemNhapThuoc_KhoaDuoc_ExporttoExcel", criteria, "sheet1", filePath);
                    return;
                case ReportName.THEODOIHANGXUATKYGOI:
                    ExportToExcel("spRptDrugDeptTempInwardReport_ExporttoExcel", criteria, "sheet1", filePath);
                    return;
                case ReportName.TRANSFERFORMDATA:
                    ExportToExcel("spExportTransferFormData", criteria, "sheet1", filePath);
                    return;
                case ReportName.FINANCIAL_ACTIVITY_TEMP3:
                    ExportToExcel("spFinancialActivityTemp03_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.REPORT_IMPORT_EXPORT_DEPARTMENT:
                    ExportToExcel("spRpt_InPatientImportExportDepartment_Detail_Excel", criteria, "sheet1", filePath);
                    return;
                case ReportName.OutwardDrugsByStaffStatistic:
                    //    return GetOutwardDrugsByStaffStatisticReportString("spOutwardDrugsByStaffStatistic", criteria);
                    ExportToExcel("spOutwardDrugsByStaffStatistic", criteria, "sheet1", filePath);
                    return;
                case ReportName.OutwardDrugsByStaffStatisticDetails:
                    //    return GetOutwardDrugsByStaffStatisticReportString("spBonusByOutwardDrugsStatistic", criteria);
                    ExportToExcel("spBonusByOutwardDrugsStatistic", criteria, "sheet1", filePath);
                    return;
                //▼====: #014
                case ReportName.OutwardDrugClinicDeptsByStaffStatisticDetails_TP:
                    //    return OutwardDrugClinicDeptsByStaffStatisticDetails_TPReportString("spOutwardDrugClinicDeptsByStaffStatistic_TP", criteria);
                    ExportToExcel("spOutwardDrugClinicDeptsByStaffStatistic_TP", criteria, "sheet1", filePath);
                    return;
                //▲====: #014
                case ReportName.KTDoanhThu_TEMP80a:
                    ExportToExcel("spRpt_ktdoanhthu_CreateTemplate80a_Excel_BV", criteria, "ketoan_80a", filePath);
                    return;
                case ReportName.ExportExcel_LabWaitResult:
                    ExportToExcel("spRpt_ktbhyt_LabWaitResult", criteria, "LabWaitResult", filePath);
                    return;
                //▼====: #018
                case ReportName.ExportExcel_Temp19_THUOC_COVID_KD:
                    ExportToExcel("spRpt_CreateTemp19_Thuoc_Covid_Excel", criteria, "19Covid", filePath);
                    return;
                case ReportName.ExportExcel_Temp19_VTYT_COVID_KD:
                    ExportToExcel("spRpt_CreateTemp19_VTYT_Covid_Excel", criteria, "19Covid", filePath);
                    return;
                case ReportName.ExportExcel_Temp21_COVID_KT:
                    ExportToExcel("spRpt_CreateTemp21_Covid_Excel", criteria, "21Covid", filePath);
                    return;
                case ReportName.BC_ChiTietKhamBenh:
                    ExportToExcel("spRpt_BaoCaoChiTietKhamBenh_NgoaiTru_Export", criteria, "khambenh", filePath);
                    return;
                case ReportName.BC_HuyDichVu_NgT:
                    ExportToExcel("spBC_HuyDichVu_NgT_Export", criteria, "dichvuhuy", filePath);
                    return;
                //▲====: #018

                //▼====: #019
                case ReportName.TEMP79a_BCBENHDACTRUNG:
                    ExportToExcel("spRpt_BC_BNKhamBenhDacTrung_Export", criteria, "Mau79aBenhDacTrung", filePath);
                    return;
                //▲====: #019

                case ReportName.BC_DoDHST:
                    ExportToExcel("spRptPhysicalExamination_ByDeptID_Export", criteria, "BC_DoDHST", filePath);
                    return;
                case ReportName.BCSuDungToanBV:
                    {
                        if (criteria.IsDetail)
                        {
                            ExportToExcel("spBCSuDungToanBV_Export_Details", criteria, "sheet1", filePath);
                            return;
                        }
                        else
                        {
                            ExportToExcel("spBCSuDungToanBV_Export", criteria, "sheet1", filePath);
                            return;
                        }
                    }
                case ReportName.BCHuyDTDT:
                    ExportToExcel("spXRpt_BaoCaoHuyDTDT", criteria, "BC_DoDHST", filePath);
                    return;
                //▼====: #038
                case ReportName.BCXuatSDThuocCanQuang:
                    ExportToExcel("spXRpt_XuatSDThuocCanQuang", criteria, "XuatSDCanQuang", filePath);
                    return;
                //▲====: #038
                default:  return;
            }
        }
        public void ExportToExcel(string storeName, ReportParameters criteria, string sheetName, string filePath)
        {
            if (null == criteria
                || String.IsNullOrEmpty(filePath))
            {
                return;
            }
            try
            {
                SqlDataReader _DataReader = null;
                using (SqlConnection _Connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(storeName, _Connection);

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
                        cmd.AddParameter("@ConsultDiagRepType", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.ConsultDiagRepType));
                        cmd.AddParameter("@IsConsultingHistoryView", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsConsultingHistoryView));
                        cmd.AddParameter("@IsApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsApproved));
                        cmd.AddParameter("@IsLated", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsLated));
                        cmd.AddParameter("@IsAllExamCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsAllExamCompleted));
                        cmd.AddParameter("@IsSurgeryCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsSurgeryCompleted));
                        cmd.AddParameter("@IsWaitSurgery", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsWaitSurgery));
                        cmd.AddParameter("@IsDuraGraft", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsDuraGraft));
                        cmd.AddParameter("@IsWaitingExamCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsWaitingExamCompleted));
                        cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.FromDate));
                        cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.ToDate));
                        cmd.AddParameter("@IsCancelSurgery", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ConsultingDiagnosysSearchCriteria.IsCancelSurgery));
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
                    else if (criteria.reportName == ReportName.FINANCIAL_ACTIVITY_TEMP3 || criteria.reportName == ReportName.BC_ChiTietKhamBenh || criteria.reportName == ReportName.BC_HuyDichVu_NgT
                        || criteria.reportName == ReportName.TEMP79a_BCBENHDACTRUNG || criteria.reportName == ReportName.REPORT_PATIENT_SETTLEMENT)
                    {
                    }
                    else if (criteria.reportName == ReportName.BC_DoDHST)
                    {
                        cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                    }
                    //▼====: #038
                    else if (criteria.reportName == ReportName.BCXuatSDThuocCanQuang)
                    {
                        cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StoreID));
                    }
                    //▲====: #038
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
                        if (criteria.reportName == ReportName.TEMP19
                            || criteria.reportName == ReportName.TEMP20_NOITRU_NEW
                            || criteria.reportName == ReportName.TEMP21_NEW
                            || criteria.reportName == ReportName.TEMP80a_CHITIET
                            || criteria.reportName == ReportName.TEMP80a_TONGHOP
                            || criteria.reportName == ReportName.TEMP79a_TONGHOP
                            || criteria.reportName == ReportName.TEMP79a_CHITIET)
                        {
                            cmd.AddParameter("@V_79AExportType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_79AExportType));
                        }

                        if (criteria.reportName == ReportName.BCChiTietBNChuyenDi)
                        {
                            cmd.AddParameter("@TransferType", SqlDbType.BigInt, 1);
                        }
                        if (criteria.reportName == ReportName.BCChiTietBNChuyenDen)
                        {
                            cmd.AddParameter("@TransferType", SqlDbType.BigInt, 2);
                        }
                        if (criteria.reportName == ReportName.ExportExcel_LabWaitResult
                            || criteria.reportName == ReportName.BCSuDungToanBV)
                        {
                            cmd.AddParameter("@FindPatient", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.FindPatient));
                        }
                        else
                        {

                        }
                    }
                    cmd.CommandTimeout = int.MaxValue;


                    cmd.CommandType = CommandType.StoredProcedure;

                    _Connection.Open();
                    _DataReader = cmd.ExecuteReader();

                    WriteExcelFile(_DataReader, sheetName, filePath);

                    _DataReader.Close();
                    CleanUpConnectionAndCommand(_Connection, cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        

        public void ExportReportToExcelForAccountant_New(ReportParameters criteria,string filePath)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = null;
                    switch (criteria.reportName)
                    {
                        case ReportName.BangKeBacSiThucHienCLS:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BangKeBacSiThucHienCLS_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            cmd.AddParameter("@Settlement", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.Settlement));
                            break;
                        case ReportName.BCChiTietDV_CLS:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BCChiTietDV_CLS_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.KT_BaoCaoDoanhThuNgTru:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_KT_BaoCaoDoanhThuNgoai_NoiTru_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.ThongKeDsachKBNoiTruTheoBacSi:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_ThongKeDsachBacSiKhamBenh_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.ThongKeDsachKBNgTruTheoBacSi:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_ThongKeDsachBacSiKhamBenhNgTru_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.BangKeBacSiThucHienPT_TT:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BangKeBacSiThucHienPTTT_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.BangKeBanLeHangHoaDV:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BangKeBanLeHangHoaDV_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        case ReportName.KTTH_BC_XUAT_VIEN:
                            cmd = null;
                            cmd = new SqlCommand("sp_ThongKeDsachBNXuatVien_Export", cn);
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.KTTH_BC_CHI_DINH_NHAP_VIEN:
                            cmd = null;
                            cmd = new SqlCommand("sp_ThongKeDsachBsiChiDinhNhapVien_Export", cn);
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.BangKeThuTamUngNT:
                            cmd = null;
                            cmd = new SqlCommand("spRptInPtCashAdvanceStatistics_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                            //20210127 Thêm điều kiện lọc theo phương thức thanh toán khi xuất excel - Báo cáo Tạm ứng Nội trú
                            cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_PaymentMode.LookupID));
                            break;
                        case ReportName.BangKeThuHoanUngNT:
                            cmd = null;
                            cmd = new SqlCommand("spRptInPtPaidCashAdvanceStatistics_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                            break;
                        //▼====: #003
                        case ReportName.RptTongHopDoanhThuTheoKhoa:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_KT_TongHopDoanhThuTheoKhoa_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@HasDischarge", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.HasDischarge));
                            break;
                        //▲====: #003
                        //▼====: #009
                        case ReportName.BC_BNTaiKhamBenhManTinh:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_BNTaiKhamBenhManTinh_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@PatientAges", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.SearchID));
                            break;
                        //▲====: #009
                        //▼====: #016
                        case ReportName.BC_BenhNhanKhamBenh:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_BenhNhanKhamBenh_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲====: #016
                        //▼====: #017
                        case ReportName.BC_BNHenTaiKhamBenhDacTrung:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_BNHenTaiKhamBenhDacTrung_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲====: #017
                        //▼====: #012
                        case ReportName.BC_ToaThuocHangNgay_DLS:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_ToaThuocHangNgay_DLS_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.BC_ToaThuocHangNgay_KD:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_ToaThuocHangNgay_KD_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.BC_ThuocYCuDoDangCuoiKy:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BC_ThuocYCuDoDangCuoiKy_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            break;
                        //▲====: #012
                        //▼====: #013
                        case ReportName.BC_NHAP_PTTT_KHOA_PHONG:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BaoCaoNhapPTTTKhoaPhong_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocID));
                            break;
                        //▲====: #013
                        case ReportName.BC_NHAP_PTTT_KHOA_PHONG_KHTH:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_BaoCaoNhapPTTTKhoaPhong_KHTH", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocID));
                            break;
                        //▼====: #014
                        case ReportName.BC_NXT_THUOC_TONGHOP:
                            cmd = null;
                            switch (criteria.StoreType)
                            {
                                case (long)AllLookupValues.StoreType.STORAGE_CLINIC:
                                    cmd = new SqlCommand("spRpt_ClinicDept_InOutStocks_ByStoreID_KT_Export", cn);
                                    cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_MedProductType));
                                    cmd.AddParameter("@RefGenDrugCatID_1", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RefGenDrugCatID_1));
                                    cmd.AddParameter("@DrugDeptProductGroupReportTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.SelectedDrugDeptProductGroupReportType));
                                    break;
                                case (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT:
                                    cmd = new SqlCommand("spRpt_DrugDept_InOutStocks_ByStoreID_KT_Export", cn);
                                    cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_MedProductType));
                                    cmd.AddParameter("@RefGenDrugCatID_1", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RefGenDrugCatID_1));
                                    cmd.AddParameter("@DrugDeptProductGroupReportTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.SelectedDrugDeptProductGroupReportType));
                                    break;
                                case (long)AllLookupValues.StoreType.STORAGE_EXTERNAL:
                                    cmd = new SqlCommand("spRpt_InOutStockValue_KT_Export", cn);
                                    break;
                            }

                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StoreID));

                            break;
                        //▼==== #043
                        case ReportName.XRptSoDienTim:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoDienTim_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoKhamBenh:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoKhamBenh_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoThuThuat:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoThuThuat_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoSieuAm:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoSieuAm_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoXetNghiem:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoXetNghiem_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoXQuang:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoXQuang_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoNoiSoi:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoNoiSoi_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.XRptSoDoChucNangHoHap:
                            cmd = null;
                            cmd = new SqlCommand("spRptSoDoChucNangHoHap_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲==== #043
                        case ReportName.BC_CONGNO_NOITRU:
                            cmd = null;
                            cmd = new SqlCommand("spBCCongNoNoiTru_Export", cn);
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲====: #014
                        
                        //▼====: #020
                        case ReportName.REPORT_PATIENT_SETTLEMENT:
                            cmd = null;
                            cmd = new SqlCommand("spRpt_PatientSettlement_Detail_ExportExcel_V2", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_PaymentMode.LookupID));
                            break;
                        //▲====: #020
                        //▼====: #021
                        case ReportName.KTTH_BC_QLKiemDuyetHoSo:
                            cmd = null;
                            cmd = new SqlCommand("spRptCheckMedicalFiles_InPt_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲====: #021
                        //▼====: #022
                        case ReportName.DLS_BCThongTinDanhMucThuoc:
                            cmd = null;
                            cmd = new SqlCommand("spRefGenMedDrugInfoDetails_Export", cn);
                            break;
                        //▲====: #022
                        //▼====: #023
                        case ReportName.DLS_BCKiemTraLichSuKCB:
                            cmd = null;
                            cmd = new SqlCommand("spRptCheckDoctorDiagAndPrescription_Export", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲====: #023
                        //▼====: #024
                        case ReportName.BCTinhHinhCLS_DaThucHien:
                            cmd = null;
                            cmd = new SqlCommand("sp_Rpt_KT_TinhHinhHoatDongCLS_DaThucHien_Excel", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        //▲====: #024
                        //▼====: #025
                        case ReportName.BaoCaoDoanhThuTheoKhoa:
                            cmd = null;
                            if (criteria.IsDetail)
                                cmd = new SqlCommand("sp_Rpt_KT_TinhHinhHoatDongCLS_DaThucHien_Excel", cn);
                            else
                                cmd = new SqlCommand("sp_Rpt_KT_TinhHinhHoatDongCLS_DaThucHien_Excel", cn);

                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        //▲====: #025
                        //▼====: #026
                        case ReportName.BCThongKeSLHoSoDTNT:
                            cmd = null;
                            cmd = new SqlCommand("spBCThongKeSLHoSoDTNT_Excel", cn);
                            cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                            cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                            cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                            cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@LocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocID));
                            cmd.AddParameter("@V_OutPtTreatmentStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_OutPtTreatmentStatus.LookupID));
                            cmd.AddParameter("@OutpatientTreatmentTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.OutpatientTreatmentTypeID));
                            break;
                        //▲====: #026
                        //▼====: #027
                        case ReportName.BCDanhSachBNDTNT_KHTH:
                            cmd = null;
                            cmd = new SqlCommand("spRptBCDanhSachBNDieuTriNgoaiTru_ExportExcel", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                            cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocID));
                            cmd.AddParameter("@V_OutPtTreatmentStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_OutPtTreatmentStatus.LookupID));
                            cmd.AddParameter("@OutpatientTreatmentTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.OutpatientTreatmentTypeID));
                            break;
                        //▲====: #027
                        //▼==== #028
                        case ReportName.BCDanhSachBenhNhanDTNT:
                            cmd = null;
                            cmd = new SqlCommand("sp_BCDsachBNDTNT_Excel", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲==== #028
                        //▼==== #029
                        case ReportName.XRpt_PhatHanhTheKCB:
                            cmd = null;
                            cmd = new SqlCommand("spXRpt_PhatHanhTheKCB", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲==== #029
                        //▼==== #031
                        case ReportName.BCThaoTacNguoiDung:
                            cmd = null;
                            cmd = new SqlCommand("spRptUserManipulation", cn);
                            cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                            cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                            cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                            cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        //▲==== #031
                        //▼==== #030
                        case ReportName.XRpt_GioLamThemBacSi:
                            cmd = null;
                            cmd = new SqlCommand("spXRpt_GioLamThemBacSi", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲==== #030
                        //▼==== #032
                        case ReportName.BCThongKeHoSoDTNT:
                            cmd = null;
                            cmd = new SqlCommand("spBCThongKeHoSoDTNT", cn);
                            cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Quarter));
                            cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Month));
                            cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Year));
                            cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.Flag));
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@YNOutPtTreatmentCode", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.YNOutPtTreatmentCode));
                            cmd.AddParameter("@YNOutPtTreatmentType", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.YNOutPtTreatmentType));
                            cmd.AddParameter("@YNOutPtTreatmentProgram", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.YNOutPtTreatmentProgram));
                            cmd.AddParameter("@YNOutPtTreatmentFinal", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.YNOutPtTreatmentFinal));
                            break;
                        //▲==== #032
                        //▼==== #039
                        case ReportName.BCThGianTuVanCapToa:
                            cmd = null;
                            cmd = new SqlCommand("spBCThGianTuVanCapToa", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@PatientType", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.PatientType));
                            break;
                        case ReportName.BCThGianTuVanChiDinh:
                            cmd = null;
                            cmd = new SqlCommand("spBCThGianTuVanChiDinh", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@PatientType", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.PatientType));
                            break;
                        //▲==== #039
                        //▼==== #041
                        case ReportName.BC_DLS_KhamBenhNgoaiTru:
                            cmd = null;
                            cmd = new SqlCommand("spBaoCao_DLS_KhamBenhNgoaiTru", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.BC_DLS_CLSNgoaiTru:
                            cmd = null;
                            cmd = new SqlCommand("spBaoCao_DLS_CLSNgoaiTru", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        case ReportName.BC_DLS_ThuocNgoaiTru:
                            cmd = null;
                            cmd = new SqlCommand("spBaoCao_DLS_ThuocNgoaiTru", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            break;
                        //▲==== #041
                        case ReportName.BaoCaoXNChuaNhapKetQua:
                            cmd = null;
                            cmd = new SqlCommand("spBaoCaoXNChuaNhapKetQua", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@FindPatient", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.FindPatient));
                            break;
                        //▼==== #043
                        case ReportName.XRpt_BCThGianCho_Khukham:
                            cmd = null;
                            cmd = new SqlCommand("spBCThGianCho_KhuKham", cn);
                            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                            cmd.AddParameter("@PatientType", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.PatientType));
                            cmd.AddParameter("@V_ExaminationProcess", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.V_ExaminationProcess.LookupID));
                            break;
                        //▲==== #043
                        default:
                            break;
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cn.Open();
                    var _DataReader = cmd.ExecuteReader();
                    WriteExcelFile(_DataReader, "Sheet1", filePath);
                    CleanUpConnectionAndCommand(cn, cmd);
                    _DataReader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void WriteExcelFile(SqlDataReader dataReader, string sheetName, string filePath)
        {
            if (null == dataReader
                || String.IsNullOrEmpty(filePath))
            {
                return;
            }
            FileInfo _FileInfo = new FileInfo(filePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage _ExcelPackage = new ExcelPackage(_FileInfo))
            {
                ExcelWorksheet _ExcelSheet = _ExcelPackage.Workbook.Worksheets.Add(sheetName);
                WriteRow(_ExcelSheet, (IDataRecord)dataReader, 0);
                int _Row = 1;
                while (dataReader.Read())
                {
                    //IDataRecord _Record = (IDataRecord)dataReader;
                    WriteRow(_ExcelSheet, (IDataRecord)dataReader, _Row++);
                }
                _ExcelPackage.Save();
            }
        }
        private void WriteExcelFile(DataTable dataTable, string sheetName, string filePath)
        {
            if (null == dataTable
                || String.IsNullOrEmpty(filePath))
            {
                return;
            }
            FileInfo _FileInfo = new FileInfo(filePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage _ExcelPackage = new ExcelPackage(_FileInfo))
            {
                ExcelWorksheet _ExcelSheet = _ExcelPackage.Workbook.Worksheets.Add(sheetName);
                int _Row = 0;
                foreach(DataRow row in dataTable.Rows)
                {
                    WriteRow(_ExcelSheet,row, _Row++);
                }
                _ExcelPackage.Save();
            }
        }

        private void WriteRow(ExcelWorksheet excelSheet, IDataRecord record, int curRow)
        {
            if (null == excelSheet
                || null == record
                || record.FieldCount <= 0)
            {
                return;
            }
            if(curRow == 0)
            {
                for (int _Idx = 0; _Idx < record.FieldCount; _Idx++)
                {
                    excelSheet.Cells[curRow+1, _Idx + 1].Value = record.GetName(_Idx);
                }
            }
            else
            {
                for (int _Idx = 0; _Idx < record.FieldCount; _Idx++)
                {
                    excelSheet.Cells[curRow+1, _Idx + 1].Value = record[_Idx] == null
                        ? string.Empty : record[_Idx].ToString();
                }
            }
            
        }
        private void WriteRow(ExcelWorksheet excelSheet, DataRow record, int curRow)
        {
            if (null == excelSheet
                || null == record
                || record.Table.Columns.Count <= 0
                )
            {
                return;
            }
            if (curRow == 0)
            {
                for (int _Idx = 0; _Idx < record.Table.Columns.Count; _Idx++)
                {
                    excelSheet.Cells[curRow + 1, _Idx + 1].Value = record.Table.Columns[_Idx].ColumnName;
                    excelSheet.Cells[curRow + 2, _Idx + 1].Value = record[_Idx] == null
                       ? string.Empty : record[_Idx].ToString();
                }
            }
            else
            {
                for (int _Idx = 0; _Idx < record.Table.Columns.Count; _Idx++)
                {
                    excelSheet.Cells[curRow + 2, _Idx + 1].Value = record[_Idx] == null
                        ? string.Empty : record[_Idx].ToString();
                }
            }

        }
        public void ExportReportToExcelForRegistrationsData(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, string filePath)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = null;
                    cmd = null;
                    cmd = new SqlCommand("spSearchRegistrationsForCreateOutPtTransactionFinalization_Excel", cn);
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ToDate));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptLocationID));
                    cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FullName));
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PatientCode));
                    cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PtRegistrationCode));
                    //cmd.AddParameter("@IsExportEInvoiceView", SqlDbType.Bit, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.IsExportEInvoiceView));
                    //cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.V_RegistrationType));
                    //▼====: #035
                    cmd.AddParameter("@ViewCase", SqlDbType.TinyInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ViewCase));
                    //▲====: #035
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cn.Open();
                    var _DataReader = cmd.ExecuteReader();
                    WriteExcelFile(_DataReader, "Sheet1", filePath);
                    CleanUpConnectionAndCommand(cn, cmd);
                    _DataReader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ExportToExcellForHIReport(ReportParameters RptParameters, int PatientTypeIndex, string filePath, long staffID)
        {
            try
            {
                using (SqlConnection _Connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spExport4210", _Connection);
                    SqlDataAdapter da = new SqlDataAdapter();
                    cmd.AddParameter("@Quarter", SqlDbType.Int, ConvertNullObjectToDBNull(RptParameters.Quarter));
                    cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(RptParameters.Month));
                    cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(RptParameters.Year));
                    cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(RptParameters.Flag));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(RptParameters.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(RptParameters.ToDate));
                    cmd.AddParameter("@PatientTypeIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PatientTypeIndex));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(staffID));
                    //cmd.AddParameter("@ListRegistrationID", SqlDbType.Xml, ConvertNullObjectToDBNull(Get_XElement_MaLienKet(listPatientRegistration).ToString()));
                    cmd.CommandTimeout = int.MaxValue;

                    cmd.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                   
                    _Connection.Open();
                    da.Fill(ds);
                    int i = 1;
                    foreach (DataTable table in ds.Tables)
                    {
                        WriteExcelFile(table, i ==6?"Mẫu kiểm soát giường":"XML" + i++, filePath);
                    }
                    CleanUpConnectionAndCommand(_Connection, cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private XDocument Get_XElement_MaLienKet(List<PatientRegistration> listPatientRegistration)
        {
            var xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                       new XElement("Root",
                       from Registration in listPatientRegistration
                       select new XElement("IDList",
                        new XElement("ma_lk", Registration.RegistrationType.RegTypeID+"-"+Registration.PtRegistrationID)
                        )));
            return xmlDocument;
        }

        //▼====: #033
        public bool CreateDTDTReport_OutPt(DTDTReport DTDTReport, out long DTDTReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCreateDTDTReport_OutPt_ByListPtRegID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, DTDTReport.Staff != null ? DTDTReport.Staff.StaffID : 0);
                    cmd.AddParameter("@RegistrationIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(HealthInsuranceReport.ConvertIDListToXml(DTDTReport.RegistrationIDList)));
                    cmd.AddParameter("@DTDTReportID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@V_ReportStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTDTReport.V_ReportStatus));
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();

                    if (cmd.Parameters["@DTDTReportID"].Value != null && cmd.Parameters["@DTDTReportID"].Value != DBNull.Value)
                        DTDTReportID = (long)cmd.Parameters["@DTDTReportID"].Value;
                    else DTDTReportID = 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CreateDTDTReport_InPt(DTDTReport DTDTReport, out long DTDTReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCreateDTDTReport_InPt_ByListPtRegID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, DTDTReport.Staff != null ? DTDTReport.Staff.StaffID : 0);
                    cmd.AddParameter("@RegistrationIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(HealthInsuranceReport.ConvertIDListToXml(DTDTReport.RegistrationIDList)));
                    cmd.AddParameter("@DTDTReportID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@V_ReportStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTDTReport.V_ReportStatus));
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();

                    if (cmd.Parameters["@DTDTReportID"].Value != null && cmd.Parameters["@DTDTReportID"].Value != DBNull.Value)
                        DTDTReportID = (long)cmd.Parameters["@DTDTReportID"].Value;
                    else DTDTReportID = 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<DTDT_don_thuoc> GetDTDTData(long nDTDTReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetDTDTDataForDTDTReportID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@DTDTReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nDTDTReportID));

                    cn.Open();
                    List<DTDT_don_thuoc> lst = null;

                    IDataReader reader = ExecuteReader(cmd);

                    lst = GetDTDTDonThuocCollectionFromReader(reader);
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return lst;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<DTDT_don_thuoc> GetDTDT_InPtData(long nDTDTReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetDTDTDataForDTDTReportID_InPt", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@DTDTReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nDTDTReportID));

                    cn.Open();
                    List<DTDT_don_thuoc> lst = null;

                    IDataReader reader = ExecuteReader(cmd);

                    lst = GetDTDTDonThuocCollectionFromReader(reader);
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return lst;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateDTDTReportStatus(DTDTReport aDTDTReport)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateDTDTReportStatus", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@DTDTReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aDTDTReport.DTDTReportID));
                    cmd.AddParameter("@CheckSum", SqlDbType.VarChar, ConvertNullObjectToDBNull(aDTDTReport.CheckSum));
                    cmd.AddParameter("@V_ReportStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(aDTDTReport.V_ReportStatus));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aDTDTReport.V_RegistrationType));
                    cn.Open();
                    var mResultValue = cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mResultValue > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲====: #033
        //▼====: #034
        public bool DeleteRegistrationDTDTReport(string ListPrescription)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteRegistrationDTDTReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@ListPrescription", SqlDbType.Xml, ConvertNullObjectToDBNull(ListPrescription));
                    cn.Open();
                    var mResultValue = cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mResultValue > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteDTDTReport(long DTDTReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteDTDTReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@DTDTReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTDTReportID));
                    cn.Open();
                    var mResultValue = cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mResultValue > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool DeleteRegistrationDTDT_InPtReport(string ListPrescription)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteRegistrationDTDT_InPtReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@ListPrescription", SqlDbType.Xml, ConvertNullObjectToDBNull(ListPrescription));
                    cn.Open();
                    var mResultValue = cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mResultValue > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteDTDT_InPtReport(long DTDTReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteDTDT_InPtReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@DTDTReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTDTReportID));
                    cn.Open();
                    var mResultValue = cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mResultValue > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲====: #034
        public bool CancelConfirmDTDTReport(long PtRegistrationID, long V_RegistrationType, long StaffID, string CancelReason)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCancelConfirmDTDTReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CancelReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CancelReason));
                    cn.Open();
                    var mResultValue = cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mResultValue > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▼====: #039
        public bool CreateDQGReport_Outpt(DQGReport aDQGReport, out long DQGReportID)
        {
            try
            {
                using (SqlConnection mConnect = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spCreateDQGReport_Outpt_ByListIssueID", mConnect);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandTimeout = int.MaxValue;
                    mCommand.AddParameter("@StaffID", SqlDbType.BigInt, aDQGReport.CreatedStaff == null ? (long?)null : aDQGReport.CreatedStaff.StaffID);
                    mCommand.AddParameter("@Title", SqlDbType.NVarChar, aDQGReport.Title);
                    mCommand.AddParameter("@IssueIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(DQGReport.ConvertIDListToXml(aDQGReport.IssueIDList)));
                    mCommand.AddParameter("@DQGReportID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    mConnect.Open();
                    int count = mCommand.ExecuteNonQuery();
                    if (mCommand.Parameters["@DQGReportID"].Value != null && mCommand.Parameters["@DQGReportID"].Value != DBNull.Value)
                    {
                       DQGReportID = (long)mCommand.Parameters["@DQGReportID"].Value;
                    }
                    else DQGReportID = 0;

                    CleanUpConnectionAndCommand(mConnect, mCommand);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool DeleteDQGReportOutpt(long PtRegistrationID, long IssueID, long DQGReportID, long StaffID, string CancelReason, long V_RegistrationType)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteDQGReportOutpt", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@DQGReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DQGReportID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CancelReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CancelReason));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                    cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IssueID));
                    cn.Open();
                    var mResultValue = cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mResultValue > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲====: #039
        //▼====: #042
        public bool CreateHIReport_130_OutPt(HealthInsuranceReport HIReport, out long HIReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCreateHIReport_130_OutPt_ByListPtRegID", cn);
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
                    cmd.AddParameter("@IsAutoCreateHIReport", SqlDbType.Bit, ConvertNullObjectToDBNull(HIReport.IsAutoCreateHIReport));
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();

                    if (cmd.Parameters["@HIReportID"].Value != null && cmd.Parameters["@HIReportID"].Value != DBNull.Value)
                        HIReportID = (long)cmd.Parameters["@HIReportID"].Value;
                    else HIReportID = 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CreateHIReport_130_InPt(HealthInsuranceReport HIReport, out long HIReportID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCreateHIReport_130_InPt_ByListPtRegID", cn);
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

                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Stream GetHIXmlReport_130_AllTab_InOneRpt_Data(long nHIReportID)
        {
            XNamespace xmlns = XNamespace.Get("http://ns.hr-xml.org/2007-04-15");
            XNamespace xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");


            XDocument XD1 = new XDocument();
            XElement GIAMDINHHS = new XElement("GIAMDINHHS",
                                    new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                                    new XAttribute(XNamespace.Xmlns + "xsi", xsi));

            XD1.Add(GIAMDINHHS);
            //▼===== #005
            string HospitalCode = Globals.AxServerSettings.Hospitals.HospitalCode;
            XElement TTDV = new XElement("THONGTINDONVI", new XElement("MACSKCB", HospitalCode));
            //XElement TTDV = new XElement("THONGTINDONVI", new XElement("MACSKCB", "95076"));
            //▲===== #005

            XElement THONGTINHOSO = new XElement("THONGTINHOSO");
            XD1.Root.Add(TTDV);

            XD1.Root.Add(THONGTINHOSO);

            XElement DANHSACHHOSO = new XElement("DANHSACHHOSO");

            int nTotNumHoSoTrongBC = GetXmlElementsForAll13Tabs(nHIReportID, DANHSACHHOSO);

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

        private int GetXmlElementsForAll13Tabs(long nHIReportID, XElement elemDANHSACH_AllHOSO_InReport)
        {
            DataSet dsTab1_General = new DataSet();
            DataTable dsTab2_Drug = new DataTable();
            DataTable dsTab3_MatAndServices = new DataTable();
            DataTable dsTab4_PCLResults = new DataTable();
            DataTable dsTab5_PCLHappening = new DataTable();
            DataTable dsTab7_DisChargePapers = new DataTable();
            DataTable dsTab8_SummaryMedicalRecord = new DataTable();
            DataTable dsTab9_BirthCertificate = new DataTable();
            DataTable dsTab10_PrenatalCertificates = new DataTable();
            DataTable dsTab11_VacationInsuranceCertificates = new DataTable();


            int nTotalNumRowInTab1 = 0;

            // Txd the following 3 totals only for debuging purpose
            int nTotXml1 = 0;
            int nTotXml2 = 0;
            int nTotXml3 = 0;
            int nTotXml4 = 0;
            int nTotXml5 = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmdTab1Gen = new SqlCommand("spGetReport130_General", cn);
                cmdTab1Gen.CommandType = CommandType.StoredProcedure;
                cmdTab1Gen.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab1Gen.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab1Gen = new SqlDataAdapter(cmdTab1Gen);
                adapterTab1Gen.Fill(dsTab1_General);

                SqlCommand cmdTab2Drug = new SqlCommand("spGetReport130_Drug", cn);
                cmdTab2Drug.CommandType = CommandType.StoredProcedure;
                cmdTab2Drug.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab2Drug.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab2Drug = new SqlDataAdapter(cmdTab2Drug);
                adapterTab2Drug.Fill(dsTab2_Drug);

                SqlCommand cmdTab3Svcs = new SqlCommand("spGetReport130_TechnicalServiceAndMedicalMaterial", cn);
                cmdTab3Svcs.CommandType = CommandType.StoredProcedure;
                cmdTab3Svcs.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab3Svcs.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab3Svcs = new SqlDataAdapter(cmdTab3Svcs);
                adapterTab3Svcs.Fill(dsTab3_MatAndServices);

                SqlCommand cmdTab4PCLs = new SqlCommand("spGetReport130_PCLResult", cn);
                cmdTab4PCLs.CommandType = CommandType.StoredProcedure;
                cmdTab4PCLs.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab4PCLs.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab4PCLs = new SqlDataAdapter(cmdTab4PCLs);
                adapterTab4PCLs.Fill(dsTab4_PCLResults);

                SqlCommand cmdTab5PCLHs = new SqlCommand("spGetReport130_PCLHappening", cn);
                cmdTab5PCLHs.CommandType = CommandType.StoredProcedure;
                cmdTab5PCLHs.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab5PCLHs.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab5PCLHs = new SqlDataAdapter(cmdTab5PCLHs);
                adapterTab5PCLHs.Fill(dsTab5_PCLHappening);

                SqlCommand cmdTab7DischargePapers = new SqlCommand("spGetReport130_DischargePapers", cn);
                cmdTab7DischargePapers.CommandType = CommandType.StoredProcedure;
                cmdTab7DischargePapers.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab7DischargePapers.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab7DischargePapers = new SqlDataAdapter(cmdTab7DischargePapers);
                adapterTab7DischargePapers.Fill(dsTab7_DisChargePapers);

                SqlCommand cmdTab8SMR= new SqlCommand("spGetReport130_SummaryMedicalRecord", cn);
                cmdTab8SMR.CommandType = CommandType.StoredProcedure;
                cmdTab8SMR.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab8SMR.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab8SMR = new SqlDataAdapter(cmdTab8SMR);
                adapterTab5PCLHs.Fill(dsTab8_SummaryMedicalRecord);

                SqlCommand cmdTab9BirthCertificate = new SqlCommand("spGetReport130_BirthCertificate", cn);
                cmdTab9BirthCertificate.CommandType = CommandType.StoredProcedure;
                cmdTab9BirthCertificate.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab9BirthCertificate.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab9BirthCertificate = new SqlDataAdapter(cmdTab9BirthCertificate);
                adapterTab9BirthCertificate.Fill(dsTab9_BirthCertificate);

                SqlCommand cmdTab10PrenatalCertificates = new SqlCommand("spGetReport130_PrenatalCertificates", cn);
                cmdTab10PrenatalCertificates.CommandType = CommandType.StoredProcedure;
                cmdTab10PrenatalCertificates.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab10PrenatalCertificates.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab10PrenatalCertificates = new SqlDataAdapter(cmdTab10PrenatalCertificates);
                adapterTab10PrenatalCertificates.Fill(dsTab10_PrenatalCertificates);

                SqlCommand cmdTab11VacationInsuranceCertificates = new SqlCommand("spGetReport130_VacationInsuranceCertificates", cn);
                cmdTab11VacationInsuranceCertificates.CommandType = CommandType.StoredProcedure;
                cmdTab11VacationInsuranceCertificates.AddParameter("@HIReportID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nHIReportID));
                cmdTab11VacationInsuranceCertificates.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab11VacationInsuranceCertificates = new SqlDataAdapter(cmdTab11VacationInsuranceCertificates);
                adapterTab11VacationInsuranceCertificates.Fill(dsTab11_VacationInsuranceCertificates);


                nTotalNumRowInTab1 = dsTab1_General.Tables[0].Rows.Count;

                for (int nTab1RowIdx = 0; nTab1RowIdx < nTotalNumRowInTab1; ++nTab1RowIdx)
                {
                    string tab1_MaLK = Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx][0]);

                    XElement elemHoSo_ForEach_LanKham = new XElement("HOSO");

                    XElement elemFileHoSo_Tab1_Xml1 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML1"));

                    /*▼====: #004*/
                    //XElement elemTongHop = Get_XElement_TongHop_FromDS(dsTab1_General, nTab1RowIdx);
                    XElement elemTongHop = Get_XElement_TongHop_130_FromDS(dsTab1_General, nTab1RowIdx);
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
                            XElement elemDrugDetails = Get_XElement_DrugDetails_130_FromDS(drugRow);
                            /*▲====: #004*/
                            elemDrugListDetails.Add(elemDrugDetails);
                        }

                        // TxD: Uncomment the following line if you want to debug and see the content of XML2 before being encoded into Base64 string
                        //elemFileHoSo_DrugTab2_Xml2.Add( new XElement("NOIDUNGFILE", elemDrugListDetails));

                        elemFileHoSo_DrugTab2_Xml2.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemDrugListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_DrugTab2_Xml2);
                        nTotXml2++;

                    }

                    XElement elemFileHoSo_MatSvcTab3_Xml3 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML3"));
                    XElement elemMatSvcListDetails = new XElement("DSACH_CHI_TIET_DVKT");

                    DataRow[] matSvcRows = null;
                    if (dsTab3_MatAndServices != null && dsTab3_MatAndServices.Rows.Count > 0)
                    {
                        matSvcRows = dsTab3_MatAndServices.Select(strWhere);
                    }
                    if (matSvcRows != null && matSvcRows.Count() > 0)
                    {
                        foreach (DataRow matSvcRow in matSvcRows)
                        {
                            /*▼====: #004*/
                            //XElement elemMatSvcDetails = Get_XElement_MatAndService_FromDS(matSvcRow);
                            XElement elemMatSvcDetails = Get_XElement_MatAndService_130_FromDS(matSvcRow);
                            /*▲====: #004*/
                            elemMatSvcListDetails.Add(elemMatSvcDetails);
                        }

                        // TxD: Uncomment the following line if you want to debug and see the content of XML3 before being encoded into Base64 string
                        //elemFileHoSo_MatSvcTab3_Xml3.Add(new XElement("NOIDUNGFILE", elemMatSvcListDetails));

                        elemFileHoSo_MatSvcTab3_Xml3.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemMatSvcListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_MatSvcTab3_Xml3);
                        nTotXml3++;
                    }

                    DataRow[] PCLRows = null;
                    if (dsTab4_PCLResults != null && dsTab4_PCLResults.Rows.Count > 0)
                    {
                        PCLRows = dsTab4_PCLResults.Select(strWhere);
                    }
                    if (PCLRows != null && PCLRows.Length > 0)
                    {
                        XElement elemFileHoSo_PCLResultsTab4_Xml4 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML4"));
                        XElement elemPCLResultListDetails = new XElement("DSACH_CHI_TIET_CLS");

                        foreach (DataRow aRow in PCLRows)
                        {
                            XElement elemPCLResultDetails = Get_XElement_PCLResults_130_FromDS(aRow);
                            elemPCLResultListDetails.Add(elemPCLResultDetails);
                        }
                        elemFileHoSo_PCLResultsTab4_Xml4.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemPCLResultListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_PCLResultsTab4_Xml4);
                        nTotXml4++;
                    }

                    DataRow[] PCLHappeningRows = null;
                    if (dsTab5_PCLHappening != null && dsTab5_PCLHappening.Rows.Count > 0)
                    {
                        PCLHappeningRows = dsTab5_PCLHappening.Select(strWhere);
                    }
                    if (PCLHappeningRows != null && PCLHappeningRows.Length > 0)
                    {
                        XElement elemFileHoSo_PCLResultsTab5_Xml5 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML5"));
                        XElement elemPCLHappeningListDetails = new XElement("DSACH_CHI_TIET_DIEN_BIEN_BENH");

                        foreach (DataRow aRow in PCLHappeningRows)
                        {
                            XElement elemPCLHappeningDetails = Get_XElement_PCLHappenings_130_FromDS(aRow);
                            elemPCLHappeningListDetails.Add(elemPCLHappeningDetails);
                        }
                        elemFileHoSo_PCLResultsTab5_Xml5.Add(new XElement("NOIDUNGFILE", Convert.ToBase64String(Encoding.UTF8.GetBytes(elemPCLHappeningListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_PCLResultsTab5_Xml5);
                        nTotXml5++;
                    }

                    DataRow[] DisChargePapers = null;
                    if (dsTab7_DisChargePapers != null && dsTab7_DisChargePapers.Rows.Count > 0)
                    {
                        DisChargePapers = dsTab7_DisChargePapers.Select(strWhere);
                    }
                    if (DisChargePapers != null && DisChargePapers.Length > 0)
                    {
                        XElement elemFileHoSo_DisChargePapers_Xml7 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML7"));
                        XElement elemDisChargePapersList = new XElement("GIAY_RA_VIEN");

                        foreach (DataRow aRow in DisChargePapers)
                        {
                            XElement elemDisChargePapers = Get_XElement_DisChargePapers_130_FromDS(aRow);
                            elemDisChargePapersList.Add(elemDisChargePapersList);
                        }
                        elemFileHoSo_DisChargePapers_Xml7.Add(new XElement("NOIDUNGFILE", Convert.ToBase64String(Encoding.UTF8.GetBytes(elemDisChargePapersList.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_DisChargePapers_Xml7);
                    }

                    DataRow[] SummaryMedicalRecord = null;
                    if (dsTab8_SummaryMedicalRecord != null && dsTab8_SummaryMedicalRecord.Rows.Count > 0)
                    {
                        SummaryMedicalRecord = dsTab8_SummaryMedicalRecord.Select(strWhere);
                    }
                    if (SummaryMedicalRecord != null && SummaryMedicalRecord.Length > 0)
                    {
                        XElement elemFileHoSo_PCLResultsTab8_Xml8 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML8"));
                        XElement elemSummaryMedicalRecordListDetails = new XElement("DSACH_CHI_TIET_HO_SO_BENH_AN");

                        foreach (DataRow aRow in SummaryMedicalRecord)
                        {
                            XElement elemSummaryMedicalRecordDetails = Get_XElement_SummaryMedicalRecord_130_FromDS(aRow);
                            elemSummaryMedicalRecordListDetails.Add(elemSummaryMedicalRecordDetails);
                        }
                        elemFileHoSo_PCLResultsTab8_Xml8.Add(new XElement("NOIDUNGFILE", Convert.ToBase64String(Encoding.UTF8.GetBytes(elemSummaryMedicalRecordListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_PCLResultsTab8_Xml8);
                    }

                    DataRow[] BirthCertificateRows = null;
                    if (dsTab9_BirthCertificate != null && dsTab9_BirthCertificate.Rows.Count > 0)
                    {
                        BirthCertificateRows = dsTab9_BirthCertificate.Select(strWhere);
                    }
                    if (BirthCertificateRows != null && BirthCertificateRows.Length > 0)
                    {
                        XElement elemFileHoSo_BirthCertificateTab9_Xml9 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML9"));
                        XElement elemBirthCertificateListDetails = new XElement("DSACH_CHI_TIET_GIAY_CHUNG_SINH");

                        foreach (DataRow aRow in BirthCertificateRows)
                        {
                            XElement elemBirthCertificateDetails = Get_XElement_BirthCertificate_130_FromDS(aRow);
                            elemBirthCertificateListDetails.Add(elemBirthCertificateDetails);
                        }
                        elemFileHoSo_BirthCertificateTab9_Xml9.Add(new XElement("NOIDUNGFILE", Convert.ToBase64String(Encoding.UTF8.GetBytes(elemBirthCertificateListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_BirthCertificateTab9_Xml9);
                        //nTotXml9++;
                    }

                    DataRow[] PrenatalCertificatesRows = null;
                    if (dsTab10_PrenatalCertificates != null && dsTab10_PrenatalCertificates.Rows.Count > 0)
                    {
                        PrenatalCertificatesRows = dsTab10_PrenatalCertificates.Select(strWhere);
                    }
                    if (PrenatalCertificatesRows != null && PrenatalCertificatesRows.Length > 0)
                    {
                        XElement elemFileHoSo_PrenatalCertificatesTab10_Xml10 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML10"));
                        XElement elemPrenatalCertificatesListDetails = new XElement("DSACH_CHI_TIET_GIAY_NGHI_DUONG_THAI");

                        foreach (DataRow aRow in PrenatalCertificatesRows)
                        {
                            XElement elemPrenatalCertificatesDetails = Get_XElement_PrenatalCertificates_130_FromDS(aRow);
                            elemPrenatalCertificatesListDetails.Add(elemPrenatalCertificatesDetails);
                        }
                        elemFileHoSo_PrenatalCertificatesTab10_Xml10.Add(new XElement("NOIDUNGFILE", Convert.ToBase64String(Encoding.UTF8.GetBytes(elemPrenatalCertificatesListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_PrenatalCertificatesTab10_Xml10);
                        //nTotXml9++;
                    }

                    DataRow[] VacationInsuranceCertificatesRows = null;
                    if (dsTab11_VacationInsuranceCertificates != null && dsTab11_VacationInsuranceCertificates.Rows.Count > 0)
                    {
                        VacationInsuranceCertificatesRows = dsTab11_VacationInsuranceCertificates.Select(strWhere);
                    }
                    if (VacationInsuranceCertificatesRows != null && VacationInsuranceCertificatesRows.Length > 0)
                    {
                        XElement elemFileHoSo_VacationInsuranceCertificatesTab11_Xml11 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML11"));
                        XElement elemVacationInsuranceCertificatesListDetails = new XElement("DSACH_CHI_TIET_GIAY_NGHI_DUONG_THAI");

                        foreach (DataRow aRow in VacationInsuranceCertificatesRows)
                        {
                            XElement elemVacationInsuranceCertificatesDetails = Get_XElement_VacationInsuranceCertificates_130_FromDS(aRow);
                            elemVacationInsuranceCertificatesListDetails.Add(elemVacationInsuranceCertificatesDetails);
                        }
                        elemFileHoSo_VacationInsuranceCertificatesTab11_Xml11.Add(new XElement("NOIDUNGFILE", Convert.ToBase64String(Encoding.UTF8.GetBytes(elemVacationInsuranceCertificatesListDetails.ToString()))));
                        elemHoSo_ForEach_LanKham.Add(elemFileHoSo_VacationInsuranceCertificatesTab11_Xml11);
                        //nTotXml9++;
                    }

                    elemDANHSACH_AllHOSO_InReport.Add(elemHoSo_ForEach_LanKham);
                }

                adapterTab1Gen.Dispose();
                adapterTab2Drug.Dispose();
                adapterTab3Svcs.Dispose();
                adapterTab4PCLs.Dispose();
                adapterTab5PCLHs.Dispose();
                adapterTab8SMR.Dispose();
                adapterTab9BirthCertificate.Dispose();
                CleanUpConnectionAndCommand(cn, null);
            }

            Debug.WriteLine("==========>   Total number XML1 = {0} - XML2 = {1} - XML3 = {2} - XML4 = {3} - XML5 = {4}", nTotXml1, nTotXml2, nTotXml3, nTotXml4, nTotXml5);

            return nTotalNumRowInTab1;
        }
        private XElement Get_XElement_TongHop_130_FromDS(DataSet dsTab1_General, int nTab1RowIdx)
        {
            return new XElement(
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
                new XElement("TEN_BENH", ConverToStringOnlyForChanDoanXML1(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ten_benh"], dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_loai_kcb"])),
                new XElement("MA_BENH", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_benh"])),
                new XElement("MA_BENHKHAC", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_benhkhac"])),
                new XElement("MA_LYDO_VVIEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_lydo_vvien"])),
                new XElement("MA_NOI_DI", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_noi_di"])),
                new XElement("MA_NOI_DEN", Convert.ToString(dsTab1_General.Tables[0].Rows[nTab1RowIdx]["ma_noi_den"])),
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

        private XElement Get_XElement_DrugDetails_130_FromDS(DataRow dsTab2_DrugRow, int version = 1)
        {
            return new XElement(
                "CHI_TIET_THUOC",               
                new XElement("MA_LK", Convert.ToString(dsTab2_DrugRow["ma_lk"])),
                new XElement("STT", Convert.ToString(dsTab2_DrugRow["stt"])),
                new XElement("MA_THUOC", Convert.ToString(dsTab2_DrugRow["ma_thuoc"])),
                new XElement("MA_PP_CHEBIEN", Convert.ToString(dsTab2_DrugRow["ma_pp_chebien"])),
                new XElement("MA_CSKCB_THUOC", Convert.ToString(dsTab2_DrugRow["ma_cskcb_thuoc"])),
                new XElement("MA_NHOM", Convert.ToString(dsTab2_DrugRow["ma_nhom"])),
                new XElement("TEN_THUOC", Convert.ToString(dsTab2_DrugRow["ten_thuoc"])),
                new XElement("DON_VI_TINH", Convert.ToString(dsTab2_DrugRow["don_vi_tinh"])),
                new XElement("HAM_LUONG", Convert.ToString(dsTab2_DrugRow["ham_luong"])),
                new XElement("DUONG_DUNG", Convert.ToString(dsTab2_DrugRow["duong_dung"])),
                new XElement("DANG_BAO_CHE", Convert.ToString(dsTab2_DrugRow["dang_bao_che"])),
                new XElement("LIEU_DUNG", Convert.ToString(dsTab2_DrugRow["lieu_dung"])),
                new XElement("CACH_DUNG", Convert.ToString(dsTab2_DrugRow["cach_dung"])),
                new XElement("SO_DANG_KY", Convert.ToString(dsTab2_DrugRow["so_dang_ky"])),
                new XElement("TT_THAU", Convert.ToString(dsTab2_DrugRow["tt_thau"])),
                new XElement("PHAM_VI", Convert.ToString(dsTab2_DrugRow["pham_vi"])),
                new XElement("TYLE_TT_BH", Convert.ToString(dsTab2_DrugRow["tyle_tt_bh"])),
                new XElement("SO_LUONG", Convert.ToString(dsTab2_DrugRow["so_luong"])),
                new XElement("DON_GIA", Convert.ToString(dsTab2_DrugRow["don_gia"])),
                new XElement("THANH_TIEN_BV", Convert.ToString(dsTab2_DrugRow["thanh_tien_bv"])),
                new XElement("THANH_TIEN_BH", Convert.ToString(dsTab2_DrugRow["thanh_tien_bh"])),
                new XElement("T_NGUONKHAC_NSNN", Convert.ToString(dsTab2_DrugRow["t_nguonkhac_nsnn"])),
                new XElement("T_NGUONKHAC_VTNN", Convert.ToString(dsTab2_DrugRow["t_nguonkhac_vtnn"])),
                new XElement("T_NGUONKHAC_VTTN", Convert.ToString(dsTab2_DrugRow["t_nguonkhac_vttn"])),
                new XElement("T_NGUONKHAC_CL", Convert.ToString(dsTab2_DrugRow["t_nguonkhac_cl"])),
                new XElement("T_NGUONKHAC", Convert.ToString(dsTab2_DrugRow["t_nguonkhac"])),
                new XElement("MUC_HUONG", Convert.ToString(dsTab2_DrugRow["muc_huong"])),
                new XElement("T_BNTT", Convert.ToString(dsTab2_DrugRow["t_bntt"])),
                new XElement("T_BNCCT", Convert.ToString(dsTab2_DrugRow["t_bncct"])),
                new XElement("T_BHTT", Convert.ToString(dsTab2_DrugRow["t_bhtt"])),
                new XElement("MA_KHOA", Convert.ToString(dsTab2_DrugRow["ma_khoa"])),
                new XElement("MA_BAC_SI", Convert.ToString(dsTab2_DrugRow["ma_bac_si"])),
                new XElement("NGAY_YL", Convert.ToString(dsTab2_DrugRow["ngay_yl"])),
                new XElement("MA_PTTT", Convert.ToString(dsTab2_DrugRow["ma_pttt"])),
                new XElement("NGUON_CTRA", Convert.ToString(dsTab2_DrugRow["nguon_ctra"])),
                new XElement("MA_BENH", Convert.ToString(dsTab2_DrugRow["ma_benh"]))
            );          
        }

        private XElement Get_XElement_MatAndService_130_FromDS(DataRow dsTab3_MatAndServices)
        {
            return new XElement(
                "CHI_TIET_DVKT",
                new XElement("MA_LK", Convert.ToString(dsTab3_MatAndServices["ma_lk"])),
                new XElement("STT", Convert.ToString(dsTab3_MatAndServices["stt"])),
                new XElement("MA_DICH_VU", Convert.ToString(dsTab3_MatAndServices["ma_dich_vu"])),
                new XElement("MA_PTTT_QT", Convert.ToString(dsTab3_MatAndServices["ma_pttt_qt"])),
                new XElement("MA_VAT_TU", Convert.ToString(dsTab3_MatAndServices["ma_vat_tu"])),
                new XElement("MA_NHOM", Convert.ToString(dsTab3_MatAndServices["ma_nhom"])),
                new XElement("GOI_VTYT", Convert.ToString(dsTab3_MatAndServices["goi_vtyt"])),
                new XElement("TEN_VAT_TU", Convert.ToString(dsTab3_MatAndServices["ten_vat_tu"])),
                new XElement("TEN_DICH_VU", Convert.ToString(dsTab3_MatAndServices["ten_dich_vu"])),
                new XElement("MA_XANG_DAU", Convert.ToString(dsTab3_MatAndServices["ma_xang_dau"])),
                new XElement("DON_VI_TINH", Convert.ToString(dsTab3_MatAndServices["don_vi_tinh"])),
                new XElement("PHAM_VI", Convert.ToString(dsTab3_MatAndServices["pham_vi"])),
                new XElement("SO_LUONG", Convert.ToString(dsTab3_MatAndServices["so_luong"])),
                new XElement("DON_GIA_BV", Convert.ToString(dsTab3_MatAndServices["don_gia_bv"])),
                new XElement("DON_GIA_BH", Convert.ToString(dsTab3_MatAndServices["don_gia_bh"])),
                new XElement("TT_THAU", Convert.ToString(dsTab3_MatAndServices["tt_thau"])),
                new XElement("TYLE_TT_DV", Convert.ToString(dsTab3_MatAndServices["tyle_tt_dv"])),
                new XElement("TYLE_TT_BH", Convert.ToString(dsTab3_MatAndServices["tyle_tt_bh"])),
                new XElement("THANH_TIEN_BV", Convert.ToString(dsTab3_MatAndServices["thanh_tien_bv"])),
                new XElement("THANH_TIEN_BH", Convert.ToString(dsTab3_MatAndServices["thanh_tien_bh"])),
                new XElement("T_TRANTT", Convert.ToString(dsTab3_MatAndServices["t_trantt"])),
                new XElement("MUC_HUONG", Convert.ToString(dsTab3_MatAndServices["muc_huong"])),
                new XElement("T_NGUONKHAC_NSNN", Convert.ToString(dsTab3_MatAndServices["t_nguonkhac_nsnn"])),
                new XElement("T_NGUONKHAC_VTNN", Convert.ToString(dsTab3_MatAndServices["t_nguonkhac_vtnn"])),
                new XElement("T_NGUONKHAC_VTTN", Convert.ToString(dsTab3_MatAndServices["t_nguonkhac_vttn"])),
                new XElement("T_NGUONKHAC_CL", Convert.ToString(dsTab3_MatAndServices["t_nguonkhac_cl"])),
                new XElement("T_NGUONKHAC", Convert.ToString(dsTab3_MatAndServices["t_nguonkhac"])),
                new XElement("T_BNTT", Convert.ToString(dsTab3_MatAndServices["t_bntt"])),
                new XElement("T_BNCCT", Convert.ToString(dsTab3_MatAndServices["t_bncct"])),
                new XElement("T_BHTT", Convert.ToString(dsTab3_MatAndServices["t_bhtt"])),
                new XElement("MA_KHOA", Convert.ToString(dsTab3_MatAndServices["ma_khoa"])),
                new XElement("MA_GIUONG", Convert.ToString(dsTab3_MatAndServices["ma_giuong"])),
                new XElement("MA_BAC_SI", Convert.ToString(dsTab3_MatAndServices["ma_bac_si"])),
                new XElement("NGUOI_THUC_HIEN", Convert.ToString(dsTab3_MatAndServices["nguoi_thuc_hien"])),
                new XElement("MA_BENH", Convert.ToString(dsTab3_MatAndServices["ma_benh"])),
                new XElement("MA_BENH_YHCT", Convert.ToString(dsTab3_MatAndServices["ma_benh_yhct"])),
                new XElement("NGAY_YL", Convert.ToString(dsTab3_MatAndServices["ngay_yl"])),
                new XElement("NGAY_TH_YL", Convert.ToString(dsTab3_MatAndServices["ngay_th_yl"])),
                new XElement("NGAY_KQ", Convert.ToString(dsTab3_MatAndServices["ngay_kq"])),
                new XElement("MA_PTTT", Convert.ToString(dsTab3_MatAndServices["ma_pttt"])),
                new XElement("VET_THUONG_TP", Convert.ToString(dsTab3_MatAndServices["vet_thuong_tp"])),
                new XElement("PP_VO_CAM", Convert.ToString(dsTab3_MatAndServices["pp_vo_cam"])),
                new XElement("VI_TRI_TH_DVKT", Convert.ToString(dsTab3_MatAndServices["vi_tri_th_dvkt"])),
                new XElement("MA_MAY", Convert.ToString(dsTab3_MatAndServices["ma_may"])),
                new XElement("MA_HIEU_SP", Convert.ToString(dsTab3_MatAndServices["ma_hieu_sp"])),
                new XElement("TAI_SU_DUNG", Convert.ToString(dsTab3_MatAndServices["tai_su_dung"])),
                new XElement("DU_PHONG", Convert.ToString(dsTab3_MatAndServices["du_phong"]))
            );          
        }

        private XElement Get_XElement_PCLResults_130_FromDS(DataRow dsTab4_PCLResult)
        {
            return new XElement(
                "CHI_TIET_CLS",
                new XElement("MA_LK", Convert.ToString(dsTab4_PCLResult["ma_lk"])),
                new XElement("STT", Convert.ToString(dsTab4_PCLResult["stt"])),
                new XElement("MA_DICH_VU", Convert.ToString(dsTab4_PCLResult["ma_dich_vu"])),
                new XElement("MA_CHI_SO", Convert.ToString(dsTab4_PCLResult["ma_chi_so"])),
                new XElement("TEN_CHI_SO", Convert.ToString(dsTab4_PCLResult["ten_chi_so"])),
                new XElement("GIA_TRI", Convert.ToString(dsTab4_PCLResult["gia_tri"])),
                new XElement("DON_VI_DO", Convert.ToString(dsTab4_PCLResult["don_vi_do"])),
                //new XElement("MA_MAY", Convert.ToString(dsTab4_PCLResult["ma_may"])),
                new XElement("MO_TA", Convert.ToString(dsTab4_PCLResult["mo_ta"])),
                new XElement("KET_LUAN", Convert.ToString(dsTab4_PCLResult["ket_luan"])),
                new XElement("NGAY_KQ", Convert.ToString(dsTab4_PCLResult["ngay_kq"])),
                new XElement("MA_BC_DOC_KQ", Convert.ToString(dsTab4_PCLResult["ma_bs_doc_kq"]))
            );
        }

        private XElement Get_XElement_PCLHappenings_130_FromDS(DataRow dsTab5_PCLHappening)
        {
            return new XElement(
                "CHI_TIET_DIEN_BIEN_BENH",
                new XElement("MA_LK", Convert.ToString(dsTab5_PCLHappening["ma_lk"])),
                new XElement("STT", Convert.ToString(dsTab5_PCLHappening["stt"])),
                new XElement("DIEN_BIEN_LS", Convert.ToString(dsTab5_PCLHappening["dien_bien_ls"])),
                new XElement("GIAI_DOAN_BENH", Convert.ToString(dsTab5_PCLHappening["giai_doan_benh"])),
                new XElement("HOI_CHAN", Convert.ToString(dsTab5_PCLHappening["hoi_chan"])),
                new XElement("PHAU_THUAT", Convert.ToString(dsTab5_PCLHappening["phau_thuat"])),
                new XElement("THOI_DIEM_DBLS", Convert.ToString(dsTab5_PCLHappening["thoi_diem_dbls"])),
                new XElement("NGUOI_THUC_HIEN", Convert.ToString(dsTab5_PCLHappening["nguoi_thuc_hien"])),
                new XElement("DU_PHONG", Convert.ToString(dsTab5_PCLHappening["du_phong"]))
            );
        }
        private XElement Get_XElement_DisChargePapers_130_FromDS(DataRow dsTab7_DisChargePapers)
        {
            return new XElement(
                "CHI_TIET_DIEN_BIEN_BENH",
                new XElement("MA_LK", Convert.ToString(dsTab7_DisChargePapers["ma_lk"])),
                new XElement("SO_LUU_TRU", Convert.ToString(dsTab7_DisChargePapers["so_luu_tru"])),
                new XElement("MA_YTE", Convert.ToString(dsTab7_DisChargePapers["ma_yte"])),
                new XElement("NGAY_VAO", Convert.ToString(dsTab7_DisChargePapers["ngay_vao"])),
                new XElement("NGAY_RA", Convert.ToString(dsTab7_DisChargePapers["ngay_ra"])),
                new XElement("MA_DINH_CHI_THAI", Convert.ToString(dsTab7_DisChargePapers["ma_dinh_chi_thai"])),
                new XElement("NGUYENNHAN_DINHCHI", Convert.ToString(dsTab7_DisChargePapers["nguyennhan_dinhchi"])),
                new XElement("THOIGIAN_DINHCHI", Convert.ToString(dsTab7_DisChargePapers["thoigian_dinhchi"])),
                new XElement("TUOI_THAI", Convert.ToString(dsTab7_DisChargePapers["tuoi_thai"])),
                new XElement("CHAN_DOAN_RV", Convert.ToString(dsTab7_DisChargePapers["chan_doan_rv"])),
                new XElement("PP_DIEUTRI", Convert.ToString(dsTab7_DisChargePapers["pp_dieutri"])),
                new XElement("GHI_CHU", Convert.ToString(dsTab7_DisChargePapers["ghi_chu"])),
                new XElement("MA_TTDV", Convert.ToString(dsTab7_DisChargePapers["ma_ttdv"])),
                new XElement("MA_BS", Convert.ToString(dsTab7_DisChargePapers["ma_bs"])),
                new XElement("TEN_BS", Convert.ToString(dsTab7_DisChargePapers["ten_bs"])),
                new XElement("NGAY_CT", Convert.ToString(dsTab7_DisChargePapers["ngay_ct"])),
                new XElement("MA_CHA", Convert.ToString(dsTab7_DisChargePapers["ma_cha"])),
                new XElement("MA_ME", Convert.ToString(dsTab7_DisChargePapers["ma_me"])),
                new XElement("HO_TEN_CHA", Convert.ToString(dsTab7_DisChargePapers["ho_ten_cha"])),
                new XElement("HO_TEN_ME", Convert.ToString(dsTab7_DisChargePapers["ho_ten_me"])),
                new XElement("SO_NGAY_NGHI", Convert.ToString(dsTab7_DisChargePapers["so_ngay_nghi"])),
                new XElement("NGOAITRU_TUNGAY", Convert.ToString(dsTab7_DisChargePapers["ngoaitru_tungay"])),
                new XElement("NGOAITRU_DENNGAY", Convert.ToString(dsTab7_DisChargePapers["ngoaitru_denngay"])),
                new XElement("MA_THE_TAM", Convert.ToString(dsTab7_DisChargePapers["ma_the_tam"])),
                new XElement("MA_KHOA_RV", Convert.ToString(dsTab7_DisChargePapers["ma_khoa_rv"]))
            );
        }
        private XElement Get_XElement_SummaryMedicalRecord_130_FromDS(DataRow dsTab8_SummaryMedicalRecords)
        {
            return new XElement(
                "CHI_TIET_DIEN_BIEN_BENH",
                new XElement("MA_LK", Convert.ToString(dsTab8_SummaryMedicalRecords["ma_lk"])),
                new XElement("MA_LOAI_KCB", Convert.ToString(dsTab8_SummaryMedicalRecords["ma_loai_kcb"])),
                new XElement("HO_TEN_CHA", Convert.ToString(dsTab8_SummaryMedicalRecords["ho_ten_cha"])),
                new XElement("HO_TEN_ME", Convert.ToString(dsTab8_SummaryMedicalRecords["ho_ten_me"])),
                new XElement("NGUOI_GIAM_HO", Convert.ToString(dsTab8_SummaryMedicalRecords["nguoi_giam_ho"])),
                new XElement("DON_VI", Convert.ToString(dsTab8_SummaryMedicalRecords["don_vi"])),
                new XElement("NGAY_VAO", Convert.ToString(dsTab8_SummaryMedicalRecords["ngay_vao"])),
                new XElement("NGAY_RA", Convert.ToString(dsTab8_SummaryMedicalRecords["ngay_ra"])),
                new XElement("CHAN_DOAN_VAO", Convert.ToString(dsTab8_SummaryMedicalRecords["chan_doan_vao"])),
                new XElement("CHAN_DOAN_RV", Convert.ToString(dsTab8_SummaryMedicalRecords["chan_doan_rv"])),
                new XElement("QT_BENHLY", Convert.ToString(dsTab8_SummaryMedicalRecords["qt_benhly"])),
                new XElement("TOMTAT_KQ", Convert.ToString(dsTab8_SummaryMedicalRecords["tomtat_kq"])),
                new XElement("PP_DIEUTRI", Convert.ToString(dsTab8_SummaryMedicalRecords["pp_dieutri"])),
                new XElement("NGAY_SINHCON", Convert.ToString(dsTab8_SummaryMedicalRecords["ngay_sinhcon"])),
                new XElement("NGAY_CONCHET", Convert.ToString(dsTab8_SummaryMedicalRecords["ngay_conchet"])),
                new XElement("SO_CONCHET", Convert.ToString(dsTab8_SummaryMedicalRecords["so_conchet"])),
                new XElement("KQ_DIEUTRI", Convert.ToString(dsTab8_SummaryMedicalRecords["kq_dieutri"])),
                new XElement("GHI_CHU", Convert.ToString(dsTab8_SummaryMedicalRecords["ghi_chu"])),
                new XElement("MA_TTDV", Convert.ToString(dsTab8_SummaryMedicalRecords["ma_ttdv"])),
                new XElement("NGAY_CT", Convert.ToString(dsTab8_SummaryMedicalRecords["ngay_ct"])),
                new XElement("MA_THE_TAM", Convert.ToString(dsTab8_SummaryMedicalRecords["ma_the_tam"])),
                new XElement("DU_PHONG", Convert.ToString(dsTab8_SummaryMedicalRecords["du_phong"]))
            );
        }
        private XElement Get_XElement_BirthCertificate_130_FromDS(DataRow dsTab9_BirthCertificate)
        {
            return new XElement(
                "CHI_TIET_GIAY_CHUNG_SINH",
                new XElement("MA_LK", Convert.ToString(dsTab9_BirthCertificate["ma_lk"])),
                new XElement("MA_BHXH_NND", Convert.ToString(dsTab9_BirthCertificate["ma_bhxh_nnd"])),
                new XElement("MA_THE_NND", Convert.ToString(dsTab9_BirthCertificate["ma_the_nnd"])),
                new XElement("HO_TEN_NND", Convert.ToString(dsTab9_BirthCertificate["ho_ten_nnd"])),
                new XElement("NGAYSINH_NND", Convert.ToString(dsTab9_BirthCertificate["ngaysinh_nnd"])),
                new XElement("MA_DANTOC_NND", Convert.ToString(dsTab9_BirthCertificate["ma_dantoc_nnd"])),
                new XElement("SO_CCCD_NND", Convert.ToString(dsTab9_BirthCertificate["so_cccd_nnd"])),
                new XElement("NGAYCAP_CCCD_NND", Convert.ToString(dsTab9_BirthCertificate["ngaycap_cccd_nnd"])),
                new XElement("NOICAP_CCCD_NND", Convert.ToString(dsTab9_BirthCertificate["noicap_cccd_nnd"])),
                new XElement("NOI_CU_TRU_NND", Convert.ToString(dsTab9_BirthCertificate["noi_cu_tru_nnd"])),
                new XElement("MA_QUOCTICH", Convert.ToString(dsTab9_BirthCertificate["ma_quoctich"])),
                new XElement("MATINH_CU_TRU", Convert.ToString(dsTab9_BirthCertificate["matinh_cu_tru"])),
                new XElement("MAHUYEN_CU_TRU", Convert.ToString(dsTab9_BirthCertificate["mahuyen_cu_tru"])),
                new XElement("MAXA_CU_TRU", Convert.ToString(dsTab9_BirthCertificate["maxa_cu_tru"])),
                new XElement("HO_TEN_CHA", Convert.ToString(dsTab9_BirthCertificate["ho_ten_cha"])),
                new XElement("MA_THE_TAM", Convert.ToString(dsTab9_BirthCertificate["ma_the_tam"])),
                new XElement("HO_TEN_CON", Convert.ToString(dsTab9_BirthCertificate["ho_ten_con"])),
                new XElement("GIOI_TINH_CON", Convert.ToString(dsTab9_BirthCertificate["gioi_tinh_con"])),
                new XElement("SO_CON", Convert.ToString(dsTab9_BirthCertificate["so_con"])),
                new XElement("LAN_SINH", Convert.ToString(dsTab9_BirthCertificate["lan_sinh"])),
                new XElement("SO_CON_SONG", Convert.ToString(dsTab9_BirthCertificate["so_con_song"])),
                new XElement("CAN_NANG_CON", Convert.ToString(dsTab9_BirthCertificate["can_nang_con"])),
                new XElement("NGAY_SINH_CON", Convert.ToString(dsTab9_BirthCertificate["ngay_sinh_con"])),
                new XElement("NOI_SINH_CON", Convert.ToString(dsTab9_BirthCertificate["noi_sinh_con"])),
                new XElement("TINH_TRANG_CON", Convert.ToString(dsTab9_BirthCertificate["tinh_trang_con"])),
                new XElement("SINHCON_PHAUTHUAT", Convert.ToString(dsTab9_BirthCertificate["sinhcon_phauthuat"])),
                new XElement("SINHCON_DUOI32TUAN", Convert.ToString(dsTab9_BirthCertificate["sinhcon_duoi32tuan"])),
                new XElement("GHI_CHU", Convert.ToString(dsTab9_BirthCertificate["ghi_chu"])),
                new XElement("NGUOI_DO_DE", Convert.ToString(dsTab9_BirthCertificate["nguoi_do_de"])),
                new XElement("NGUOI_GHI_PHIEU", Convert.ToString(dsTab9_BirthCertificate["nguoi_ghi_phieu"])),
                new XElement("NGAY_CT", Convert.ToString(dsTab9_BirthCertificate["ngay_ct"])),
                new XElement("SO", Convert.ToString(dsTab9_BirthCertificate["so"])),
                new XElement("QUYEN_SO", Convert.ToString(dsTab9_BirthCertificate["quyen_so"])),
                new XElement("MA_TTDV", Convert.ToString(dsTab9_BirthCertificate["ma_ttdv"]))
            );
        }
        private XElement Get_XElement_PrenatalCertificates_130_FromDS(DataRow dsTab10_PrenatalCertificates)
        {
            return new XElement(
                "CHI_TIET_GIAY_NGHI_DUONG_THAI",
                new XElement("MA_LK", Convert.ToString(dsTab10_PrenatalCertificates["ma_lk"])),
                new XElement("SO_SERI", Convert.ToString(dsTab10_PrenatalCertificates["so_seri"])),
                new XElement("SO_CT", Convert.ToString(dsTab10_PrenatalCertificates["so_ct"])), 
                new XElement("SO_NGAY", Convert.ToString(dsTab10_PrenatalCertificates["so_ngay"])),
                new XElement("DON_VI", Convert.ToString(dsTab10_PrenatalCertificates["don_vi"])),
                new XElement("CHAN_DOAN_RV", Convert.ToString(dsTab10_PrenatalCertificates["chan_doan_rv"])),
                new XElement("TU_NGAY", Convert.ToString(dsTab10_PrenatalCertificates["tu_ngay"])),
                new XElement("DEN_NGAY", Convert.ToString(dsTab10_PrenatalCertificates["den_ngay"])),
                new XElement("MA_TTDV", Convert.ToString(dsTab10_PrenatalCertificates["ma_ttdv"])),
                new XElement("TEN_BS", Convert.ToString(dsTab10_PrenatalCertificates["ten_bs"])),
                new XElement("MA_BS", Convert.ToString(dsTab10_PrenatalCertificates["ma_bs"])),
                new XElement("NGAY_CT", Convert.ToString(dsTab10_PrenatalCertificates["ngay_ct"]))
            );
        }
        private XElement Get_XElement_VacationInsuranceCertificates_130_FromDS(DataRow dsTab11_VacationInsuranceCertificates)
        {
            return new XElement(
                "CHI_TIET_GIAY_NGHI_HUONG_BHXH",
                new XElement("MA_LK", Convert.ToString(dsTab11_VacationInsuranceCertificates["ma_lk"])),
                new XElement("SO_SERI", Convert.ToString(dsTab11_VacationInsuranceCertificates["so_seri"])),
                new XElement("SO_KCB", Convert.ToString(dsTab11_VacationInsuranceCertificates["so_kcb"])),
                new XElement("DON_VI", Convert.ToString(dsTab11_VacationInsuranceCertificates["don_vi"])),
                new XElement("MA_BHXH", Convert.ToString(dsTab11_VacationInsuranceCertificates["ma_bhxh"])),
                new XElement("MA_THE_BHYT", Convert.ToString(dsTab11_VacationInsuranceCertificates["ma_the_bhyt"])),
                new XElement("CHAN_DOAN_RV", Convert.ToString(dsTab11_VacationInsuranceCertificates["chan_doan_rv"])),
                new XElement("PP_DIEUTRI", Convert.ToString(dsTab11_VacationInsuranceCertificates["pp_dieutri"])),
                new XElement("MA_DINH_CHI_THAI", Convert.ToString(dsTab11_VacationInsuranceCertificates["ma_dinh_chi_thai"])),
                new XElement("NGUYENNHAN_DINHCHI", Convert.ToString(dsTab11_VacationInsuranceCertificates["nguyennhan_dinhchi"])),
                new XElement("TUOI_THAI", Convert.ToString(dsTab11_VacationInsuranceCertificates["tuoi_thai"])),
                new XElement("SO_NGAY_NGHI", Convert.ToString(dsTab11_VacationInsuranceCertificates["so_ngay_nghi"])),
                new XElement("TU_NGAY", Convert.ToString(dsTab11_VacationInsuranceCertificates["tu_ngay"])),
                new XElement("DEN_NGAY", Convert.ToString(dsTab11_VacationInsuranceCertificates["den_ngay"])),
                new XElement("HO_TEN_CHA", Convert.ToString(dsTab11_VacationInsuranceCertificates["ho_ten_cha"])),
                new XElement("HO_TEN_ME", Convert.ToString(dsTab11_VacationInsuranceCertificates["ho_ten_me"])),
                new XElement("MA_TTDV", Convert.ToString(dsTab11_VacationInsuranceCertificates["ma_ttdv"])),
                new XElement("MA_BS", Convert.ToString(dsTab11_VacationInsuranceCertificates["ma_bs"])),
                new XElement("NGAY_CT", Convert.ToString(dsTab11_VacationInsuranceCertificates["ngay_ct"])),
                new XElement("MA_THE_TAM", Convert.ToString(dsTab11_VacationInsuranceCertificates["ma_the_tam"])),
                new XElement("MAU_SO", Convert.ToString(dsTab11_VacationInsuranceCertificates["mau_so"]))
            );
        }
        public Stream GetHIXmlReport_130_CheckIn_OutPt(long PtRegistrationID)
        {
            XNamespace xmlns = XNamespace.Get("http://ns.hr-xml.org/2007-04-15");
            XNamespace xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");


            XDocument XD1 = new XDocument();
            XElement GIAMDINHHS = new XElement("GIAMDINHHS",
                                    new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                                    new XAttribute(XNamespace.Xmlns + "xsi", xsi));

            XD1.Add(GIAMDINHHS);
            //▼===== #005
            string HospitalCode = Globals.AxServerSettings.Hospitals.HospitalCode;
            XElement TTDV = new XElement("THONGTINDONVI", new XElement("MACSKCB", HospitalCode));
            //XElement TTDV = new XElement("THONGTINDONVI", new XElement("MACSKCB", "95076"));
            //▲===== #005

            XElement THONGTINHOSO = new XElement("THONGTINHOSO");
            XD1.Root.Add(TTDV);

            XD1.Root.Add(THONGTINHOSO);

            XElement DANHSACHHOSO = new XElement("DANHSACHHOSO");

            int nTotNumHoSoTrongBC = GetXmlElementsForCheckIn(PtRegistrationID, DANHSACHHOSO);

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
            //using (FileStream xmlFile = new FileStream("D:\\TxdTestXmlCheckIn.xml", FileMode.Create, FileAccess.Write))
            //{                
            //    memStream.WriteTo(xmlFile);
            //    xmlFile.Flush();
            //    xmlFile.Close();
            //}

            return memStream;
        }
        private int GetXmlElementsForCheckIn(long PtRegistrationID, XElement elemDANHSACH_AllHOSO_InReport)
        {
            DataSet dsCheckIn = new DataSet();

            int nTotalNumRowInTab1 = 0;

            // Txd the following 3 totals only for debuging purpose
            int nTotXml1 = 0;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmdTab1Gen = new SqlCommand("spGetReport130_CheckIn_OutPt", cn);
                cmdTab1Gen.CommandType = CommandType.StoredProcedure;
                cmdTab1Gen.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmdTab1Gen.CommandTimeout = int.MaxValue;
                SqlDataAdapter adapterTab1Gen = new SqlDataAdapter(cmdTab1Gen);
                adapterTab1Gen.Fill(dsCheckIn);

                nTotalNumRowInTab1 = dsCheckIn.Tables[0].Rows.Count;

                for (int nTab1RowIdx = 0; nTab1RowIdx < nTotalNumRowInTab1; ++nTab1RowIdx)
                {
                    string tab1_MaLK = Convert.ToString(dsCheckIn.Tables[0].Rows[nTab1RowIdx][0]);

                    XElement elemHoSo_ForEach_LanKham = new XElement("HOSO");
                    XElement elemFileHoSo_Tab1_Xml1 = new XElement("FILEHOSO", new XElement("LOAIHOSO", "XML_CHECKIN"));
                    XElement elemCheckIn = Get_XElement_CheckIn_130_FromDS(dsCheckIn.Tables[0].Rows[0]);

                    // TxD: Uncomment the following line if you want to debug and see the content of XML1 before being encoded into Base64 string
                    //elemFileHoSo_Tab1_Xml1.Add(new XElement("NOIDUNGFILE", elemTongHop));

                    elemFileHoSo_Tab1_Xml1.Add(new XElement("NOIDUNGFILE", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(elemCheckIn.ToString()))));
                    //20230410 BLQ: Thêm raise xml check in để test
                    throw new Exception(elemCheckIn.ToString());
                    elemHoSo_ForEach_LanKham.Add(elemFileHoSo_Tab1_Xml1);
                    nTotXml1++;
                    elemDANHSACH_AllHOSO_InReport.Add(elemHoSo_ForEach_LanKham);
                }
                adapterTab1Gen.Dispose();
                CleanUpConnectionAndCommand(cn, null);
            }
            return nTotalNumRowInTab1;
        }
        private XElement Get_XElement_CheckIn_130_FromDS(DataRow dsCheckIn)
        {
            return new XElement(
                "CHECK_IN",
                new XElement("MA_LK", Convert.ToString(dsCheckIn["ma_lk"])),
                new XElement("STT", Convert.ToString(dsCheckIn["stt"])),
                new XElement("MA_BN", Convert.ToString(dsCheckIn["ma_bn"])),
                new XElement("HO_TEN", Convert.ToString(dsCheckIn["ho_ten"])),
                new XElement("SO_CCCD", Convert.ToString(dsCheckIn["so_cccd"])),
                new XElement("NGAY_SINH", Convert.ToString(dsCheckIn["ngay_sinh"])),
                new XElement("GIOI_TINH", Convert.ToString(dsCheckIn["gioi_tinh"])),
                new XElement("MA_THE_BHYT", Convert.ToString(dsCheckIn["ma_the_bhyt"])),
                new XElement("MA_DKBD", Convert.ToString(dsCheckIn["ma_dkbd"])),
                new XElement("GT_THE_TU", Convert.ToString(dsCheckIn["gt_the_tu"])),
                new XElement("GT_THE_DEN", Convert.ToString(dsCheckIn["gt_the_den"])),
                new XElement("MA_DOITUONG_KCB", Convert.ToString(dsCheckIn["ma_doituong_kcb"])),
                new XElement("NGAY_VAO", Convert.ToString(dsCheckIn["ngay_vao"])),
                new XElement("MA_LOAI_KCB", Convert.ToString(dsCheckIn["ma_loai_kcb"])),
                new XElement("MA_CSKCB", Convert.ToString(dsCheckIn["ma_cskcb"])),
                new XElement("MA_DICH_VU", Convert.ToString(dsCheckIn["ma_dich_vu"])),
                new XElement("TEN_DICH_VU", Convert.ToString(dsCheckIn["ten_dich_vu"])),
                new XElement("NGAY_YL", Convert.ToString(dsCheckIn["ngay_yl"]))

            );

        }
        //▲====: #042
    }
}
