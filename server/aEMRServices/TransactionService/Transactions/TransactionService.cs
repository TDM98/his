using System;
using System.Collections.Generic;
using eHCMS.Caching;
using System.Text;
using DataEntities;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Data;
using ErrorLibrary;
using AxLogging;
using ErrorLibrary.Resources;
using eHCMS.Configurations;
using System.IO;
using Service.Core.Common;
using eHCMSLanguage;
using System.Data.SqlClient;
using OfficeOpenXml;
/*
* 20190326 #001 TNHX: Add function to export excel in ConfirmHIRegistrationView
* 20190608 #002 TNHX: [BM0006715] Add function to export excel for Accountant
* 20190628 #003 TTM:   BM 0011903: Thêm chức năng xoá báo cáo nếu như chưa chuyển đổi dữ liệu sang cho FAST.
* 20200319 #004 TTM:   BM 0027022: [79A] Bổ sung tích chọn xuất Excel toàn bộ dữ liệu, đã xác nhận, chưa xác nhận. 
* 20200404 #005 TTM:   BM 0029080: Xuất dữ liệu XML để báo cáo cho cổng dữ liệu Quốc Gia.
* 20200414 #006 TTM:   BM 0032119: Bổ sung mã giao dịch cho đăng ký báo cáo thủ công thông qua VAS.
* 20210323 #007 BLQ:    #243 Lấy HIReport cho KHTH
* 20221212 #008 TNHX: 994 Đẩy đơn thuốc điện tử - ngoại trú
* 20230108 #009 THNHX:  944 Thêm func xóa dữ liệu đẩy đơn thuốc điện tử không thành công
* 20230220 #010 QTD:   Thêm func cho DQG Nhà thuốc
* 20230314 #011 BLQ: Thêm hàm tạo xml đẩy cổng BH theo 130
*/
namespace TransactionService.Transactions
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class TransactionService : eHCMS.WCFServiceCustomHeader, ITransactionService
    {
        public TransactionService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        #region ITransactionService Members

        public IList<PatientTransaction> GetTransaction_ByPtID(SeachPtRegistrationCriteria SearchCriteria, int PageSize, int PageIndex, bool CountTotal, out int Total)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetTransaction_ByPtID.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetTransaction_ByPtID(SearchCriteria, PageSize, PageIndex, CountTotal, out Total);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.GetTransaction_ByPtID(SearchCriteria, PageSize, PageIndex, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetTransaction_ByPtID(SearchCriteria, PageSize, PageIndex, CountTotal, out Total);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InsertDataToReport(string begindate, string enddate)
        {

            try
            {
                AxLogger.Instance.LogInfo("Start InsertDataToReport.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.InsertDataToReport(begindate, enddate);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.InsertDataToReport(begindate, enddate);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.InsertDataToReport(begindate, enddate);
                //AxLogger.Instance.LogInfo("End of InsertDataToReport. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_INSERT, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        //export excel all
        #region Export excel from database
        #region b2d viet ham xuat excel chung o day
        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields, string strFormat)
        {
            switch (strFormat)
            {
                case "xls":
                    strBuilder.AppendLine("<Row>");
                    strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
                    strBuilder.AppendLine("</Row>");
                    break;
                case "CSV":
                    strBuilder.AppendLine(String.Join(",", lstFields.ToArray()));
                    break;
            }
        }
        private static string FormatField(string data, string format)
        {
            switch (format)
            {
                case "xls":
                    return String.Format("<Cell><Data ss:Type=\"String" +
                       "\">{0}</Data></Cell>", data);
                case "CSV":
                    return String.Format("\"{0}\"",
                      data.Replace("\"", "\"\"\"").Replace("\n",
                      "").Replace("\r", ""));
            }
            return data;
        }
        private string ExportAll(List<List<string>> listData, string sheetName, string fileName, string fileExt)
        {
            try
            {
                if (listData != null)
                {
                    string strFormat = fileExt.Substring(fileExt.IndexOf('.') + 1).ToLower();
                    StringBuilder strBuilder = new StringBuilder();
                    List<string> lstFields = new List<string>();
                    //BuildStringOfRow(strBuilder, columnNames);
                    foreach (List<string> data in listData)
                    {
                        lstFields.Clear();
                        foreach (var col in data)
                        {
                            lstFields.Add(FormatField(col, strFormat));
                        }
                        BuildStringOfRow(strBuilder, lstFields, strFormat);
                    }
                    fileName += fileExt;
                    StreamWriter sw = new StreamWriter(fileName);
                    if (strFormat == "xls")
                    {
                        //Let us write the headers for the Excel XML
                        sw.WriteLine("<?xml version=\"1.0\" " +
                                        "encoding=\"utf-8\"?>");
                        sw.WriteLine("<?mso-application progid" +
                                        "=\"Excel.Sheet\"?>");
                        sw.WriteLine("<Workbook xmlns=\"urn:" +
                                        "schemas-microsoft-com:office:spreadsheet\">");
                        sw.WriteLine("<DocumentProperties " +
                                        "xmlns=\"urn:schemas-microsoft-com:" +
                                        "office:office\">");
                        //sw.WriteLine("<Author>" + Globals.LoggedUserAccount.Staff.FullName.Trim() + "</Author>");
                        sw.WriteLine("<Created>" +
                                        DateTime.Now.ToLocalTime().ToLongDateString() +
                                        "</Created>");
                        sw.WriteLine("<LastSaved>" +
                                        DateTime.Now.ToLocalTime().ToLongDateString() +
                                        "</LastSaved>");
                        sw.WriteLine("<Company>Viện Tim</Company>");
                        sw.WriteLine("<Version>12.00</Version>");
                        sw.WriteLine("</DocumentProperties>");
                        sw.WriteLine("<Worksheet ss:Name=\"" + sheetName + "\" " +
                            "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                        sw.WriteLine("<Table>");
                    }
                    sw.Write(strBuilder.ToString());
                    if (strFormat == "xls")
                    {
                        sw.WriteLine("</Table>");
                        sw.WriteLine("</Worksheet>");
                        sw.WriteLine("</Workbook>");
                    }
                    sw.Close();
                }
                return fileName;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return "";
            }
            
        }
        private string SaveReportDataToFile(string Data, string fileName, string fileExt)
        {
            try
            {
                fileName += fileExt;
                StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8);
                sw.Write(Data);
                sw.Close();
                return fileName;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return "";
            }
        }
        private string GetReportParameterString(ReportParameters criteria)
        {
            string strParameter = "";
            string strComma = ", ";

            if (criteria.FromDate != null)
            {
                strParameter += "FromDate = " + criteria.FromDate.GetValueOrDefault().ToString() + strComma;
            }
            if (criteria.ToDate != null)
            {
                strParameter += "ToDate = " + criteria.ToDate.GetValueOrDefault().ToString() + strComma;
            }
            if (criteria.DeptID != null)
            {
                strParameter += "DeptID = " + criteria.DeptID.GetValueOrDefault().ToString() + strComma;
            }
            if (criteria.DeptLocID != null)
            {
                strParameter += "DeptLocID = " + criteria.DeptLocID.GetValueOrDefault().ToString() + strComma;
            }
            if (criteria.Quarter >= 0)
            {
                strParameter += "Quarter = " + criteria.Quarter.ToString() + strComma;
            }
            if (criteria.Month >= 0)
            {
                strParameter += "Month = " + criteria.Month.ToString() + strComma;
            }
            if (criteria.Year >= 0)
            {
                strParameter += "Year = " + criteria.Year.ToString() + strComma;
            }
            if (criteria.Flag >= 0)
            {
                strParameter += "Flag = " + criteria.Flag.ToString() + strComma;
            }

            if (criteria.reportName == ReportName.TEMP26a_CHITIET || criteria.reportName == ReportName.TEMP80a_CHITIET)
            {
                strParameter += "NotTreatedAsInPt = " + criteria.NotTreatedAsInPt.ToString() + strComma;
            }

            if (criteria.StoreID > 0)
            {
                strParameter += "StoreID = " + criteria.StoreID.ToString() + strComma;
            }

            if (criteria.EstimatePoID > 0)
            {
                strParameter += "EstimatePoID = " + criteria.EstimatePoID.ToString() + strComma;
            }

            if (criteria.V_MedProductType > 0)
            {
                strParameter += "V_MedProductType = " + criteria.V_MedProductType.ToString() + strComma;
            }

            if (criteria.PriceList != null)
            {
                strParameter += "PriceListID = " + criteria.PriceList.PriceListID.ToString() + strComma;
            }

            if (criteria.StockTake != null)
            {
                strParameter += "StockTakeID = " + criteria.StockTake.StockTakeID.ToString() + strComma;
                strParameter += "StockTake.V_MedProductType = " + criteria.StockTake.V_MedProductType.ToString() + strComma;
                strParameter += "StockTakeDate = " + criteria.StockTake.StockTakeDate.ToString() + strComma;
            }

            if (criteria.SearchRefGenMedProduct != null)
            {
                strParameter += "IsInsurance = " + criteria.SearchRefGenMedProduct.IsInsurance.ToString() + strComma;
                strParameter += "IsConsult = " + criteria.SearchRefGenMedProduct.IsConsult.ToString() + strComma;
                strParameter += "IsActive = " + criteria.SearchRefGenMedProduct.IsActive.ToString() + strComma;
                strParameter += "SearchRefGenMedProduct.V_MedProductType = " + criteria.SearchRefGenMedProduct.V_MedProductType.ToString() + strComma;
            }

            if (criteria.DrugID > 0)
            {
                strParameter += "DrugID = " + criteria.DrugID.ToString() + strComma;
            }
            //▼===== #004
            if (criteria.V_79AExportType > 0 && 
                (criteria.reportName == ReportName.TEMP79a_CHITIET 
                || criteria.reportName == ReportName.TEMP79a_TONGHOP
                || criteria.reportName == ReportName.TEMP80a_CHITIET
                || criteria.reportName == ReportName.TEMP80a_TONGHOP
                || criteria.reportName == ReportName.TEMP19
                || criteria.reportName == ReportName.TEMP20_NOITRU_NEW
                || criteria.reportName == ReportName.TEMP21_NEW))
            {
                strParameter += "V_79AExportType = " + criteria.V_79AExportType.ToString() + strComma;
            }
            //▲===== #004
            if (!string.IsNullOrEmpty(strParameter) && strParameter.Length >= 2)
            {
                strParameter = strParameter.Substring(0, strParameter.Length - 2);
            }


            return strParameter;
        }
        public Stream ExportToCSVAllGeneric(ReportParameters criteria, long staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Generic.", CurrentUser);
                string res;
                string reportName = "";
                string reportParameter = GetReportParameterString(criteria);
                switch (criteria.ReportType)
                {
                    case ReportType.BAOCAO_TONGHOP:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToCSVAllGeneric(criteria);
                        break;
                    default:
                        res = null;
                        break;
                }
                string folderPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\" + DateTime.Now.ToString("yyyyMMdd");
                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(folderPath);
                }
                //KMx: Nếu như không có Show (SheetName) thì phải set default, nếu không thì xuất excel xong rồi mở file lên không được (06/08/2014 16:24).
                if (string.IsNullOrEmpty(criteria.Show))
                {
                    criteria.Show = "Sheet1";
                }
                var filePath = SaveReportDataToFile(res, folderPath + "\\" + criteria.StoreName, criteria.ShowTitle);
                return CommonFunction.GetVideoAndImage(filePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream ExportToExcellAllGeneric(ReportParameters criteria, long staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Generic.", CurrentUser);
                List<List<string>> res;

                string reportName = "";
                string reportParameter = GetReportParameterString(criteria);

                switch (criteria.ReportType)
                {
                    //▼====: #002
                    case ReportType.BAOCAO_TONGHOP_KT:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportReportToExcelForAccountant(criteria);
                        break;
                    //▲====: #002
                    case ReportType.BAOCAO_TONGHOP:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAllGeneric(criteria);
                        //}
                        //else
                        //{
                        //    res = TransactionProvider.Instance.ExportToExcellAllGeneric(criteria);
                        //}
                        res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAllGeneric(criteria);
                        break;
                    case ReportType.BANG_GIA:
                        reportName = EnumTool.GetEnumDescription(criteria.PriceList.PriceListType);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcelAllItemsPriceList_New(criteria);
                        //}
                        //else
                        //{
                        //    res = RefDrugGenericDetailsProvider.Instance.ExportToExcelAllItemsPriceList_New(criteria);
                        //}
                        res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcelAllItemsPriceList_New(criteria);
                        break;
                    case ReportType.KIEM_KE:
                        reportName = EnumTool.GetEnumDescription(criteria.StockTake.StockTakeType);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportExcelStockTake(criteria);
                        //}
                        //else
                        //{
                        //    res = RefDrugGenericDetailsProvider.Instance.ExportExcelStockTake(criteria);
                        //}
                        res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportExcelStockTake(criteria);
                        break;
                    case ReportType.TON_KHO:
                        reportName = EnumTool.GetEnumDescription(criteria.FromDepartment);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportExcel_RemainInward(criteria);
                        //}
                        //else
                        //{
                        //    res = RefDrugGenericDetailsProvider.Instance.ExportExcel_RemainInward(criteria);
                        //}
                        res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportExcel_RemainInward(criteria);
                        break;
                    case ReportType.DANH_MUC_KHOA_DUOC:
                        reportName = "Danh mục Khoa Dược";
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcel_ListRefGenMedProductDetail(criteria);
                        //}
                        //else
                        //{
                        //    res = RefDrugGenericDetailsProvider.Instance.ExportToExcel_ListRefGenMedProductDetail(criteria);
                        //}
                        res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcel_ListRefGenMedProductDetail(criteria);
                        break;
                    case ReportType.DU_TRU:
                        reportName = "Dự trù Nhà Thuốc";
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcel_EstimationForPODetail(criteria);
                        //}
                        //else
                        //{
                        //    res = RefDrugGenericDetailsProvider.Instance.ExportToExcel_EstimationForPODetail(criteria);
                        //}
                        res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcel_EstimationForPODetail(criteria);
                        break;
                    case ReportType.BAOCAO_NHATHUOC:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcel_PharmacyReports(criteria);
                        //}
                        //else
                        //{
                        //    res = RefDrugGenericDetailsProvider.Instance.ExportToExcel_PharmacyReports(criteria);
                        //}
                        res = aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcel_PharmacyReports(criteria);
                        break;
                    case ReportType.BAOCAO_BHYT:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcel_HIReport(criteria);
                        //}
                        //else
                        //{
                        //    res = TransactionProvider.Instance.ExportToExcel_HIReport(criteria);
                        //}
                        res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcel_HIReport(criteria);
                        break;
                    case ReportType.SOKIEMNHAP:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAllGeneric(criteria);
                        //}
                        //else
                        //{
                        //    res = TransactionProvider.Instance.ExportToExcellAllGeneric(criteria);
                        //}
                        res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAllGeneric(criteria);
                        break;
                    default:
                        res = null;
                        break;
                }
                //var res = TransactionProvider.Instance.ExportToExcellAllGeneric(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);

                //string folderPath = Globals.ExcelStorePool + "\\"
                //    + DateTime.Now.Year.ToString()
                //    + DateTime.Now.Month.ToString()
                //    + DateTime.Now.Day.ToString()
                //    ;
                // Txd 25/05/2014 Replaced ConfigList
                string folderPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\"
                    + DateTime.Now.Year.ToString()
                    + DateTime.Now.Month.ToString()
                    + DateTime.Now.Day.ToString()
                    ;

                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(folderPath);
                }

                //KMx: Nếu như không có Show (SheetName) thì phải set default, nếu không thì xuất excel xong rồi mở file lên không được (06/08/2014 16:24).
                if (string.IsNullOrEmpty(criteria.Show))
                {
                    criteria.Show = "Sheet1";
                }
                var filePath= ExportAll(res, criteria.Show, folderPath + "\\" + criteria.StoreName, criteria.ShowTitle);
                return CommonFunction.GetVideoAndImage(filePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream ExportToExcellAllGeneric_New(ReportParameters criteria, long staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Generic.", CurrentUser);
                //List<List<string>> res;

                string folderPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\"
                    + DateTime.Now.ToString("yyyyMMdd");

                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(folderPath);
                }

                //KMx: Nếu như không có Show (SheetName) thì phải set default, nếu không thì xuất excel xong rồi mở file lên không được (06/08/2014 16:24).
                if (string.IsNullOrEmpty(criteria.Show))
                {
                    criteria.Show = "Sheet1";
                }
                string filePath = folderPath + "\\" + criteria.StoreName
                    + criteria.ShowTitle.Substring(criteria.ShowTitle.IndexOf('.') + 1).ToLower(); ;
                //filePath = ExportAll(res, criteria.Show, folderPath + "\\" + criteria.StoreName, criteria.ShowTitle);

                string reportName = "";
                string reportParameter = GetReportParameterString(criteria);

                switch (criteria.ReportType)
                {
                    //▼====: #002
                    case ReportType.BAOCAO_TONGHOP_KT:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        //res = 
                        aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportReportToExcelForAccountant_New(criteria, filePath);
                        break;
                    //▲====: #002
                    case ReportType.BAOCAO_TONGHOP:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAllGeneric_New(criteria, filePath);
                        break;
                    case ReportType.BANG_GIA:
                        reportName = EnumTool.GetEnumDescription(criteria.PriceList.PriceListType);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcelAllItemsPriceList_New_V2(criteria, filePath);
                        break;
                    case ReportType.KIEM_KE:
                        reportName = EnumTool.GetEnumDescription(criteria.StockTake.StockTakeType);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportExcelStockTake_New(criteria, filePath);
                        break;
                    case ReportType.TON_KHO:
                        reportName = EnumTool.GetEnumDescription(criteria.FromDepartment);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportExcel_RemainInward_New(criteria, filePath);
                        break;
                    case ReportType.DANH_MUC_KHOA_DUOC:
                        reportName = "Danh mục Khoa Dược";
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcel_ListRefGenMedProductDetail_New(criteria, filePath);
                        break;
                    case ReportType.DU_TRU:
                        reportName = "Dự trù Nhà Thuốc";
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcel_EstimationForPODetail_New(criteria, filePath);
                        break;
                    case ReportType.BAOCAO_NHATHUOC:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.ExportToExcel_PharmacyReports_New(criteria, filePath);
                        break;
                    case ReportType.BAOCAO_BHYT:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcel_HIReport_New(criteria, filePath);
                        break;
                    case ReportType.SOKIEMNHAP:
                        reportName = EnumTool.GetEnumDescription(criteria.reportName);
                        FollowUserGetReport(staffID, reportName, reportParameter, (long)AllLookupValues.V_GetReportMethod.EXPORT_EXCEL);
                        aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAllGeneric_New(criteria, filePath);
                        break;
                    default:
                        //res = null;
                        break;
                }
                //var res = TransactionProvider.Instance.ExportToExcellAllGeneric(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);

                //string folderPath = Globals.ExcelStorePool + "\\"
                //    + DateTime.Now.Year.ToString()
                //    + DateTime.Now.Month.ToString()
                //    + DateTime.Now.Day.ToString()
                //    ;
                // Txd 25/05/2014 Replaced ConfigList
                //string folderPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\"
                //    + DateTime.Now.Year.ToString()
                //    + DateTime.Now.Month.ToString()
                //    + DateTime.Now.Day.ToString()
                //    ;

                //if (!Directory.Exists(folderPath))
                //{
                //    DirectoryInfo dir = Directory.CreateDirectory(folderPath);
                //}

                //KMx: Nếu như không có Show (SheetName) thì phải set default, nếu không thì xuất excel xong rồi mở file lên không được (06/08/2014 16:24).
                //if (string.IsNullOrEmpty(criteria.Show))
                //{
                //    criteria.Show = "Sheet1";
                //}
                //var filePath = ExportAll(res, criteria.Show, folderPath + "\\" + criteria.StoreName, criteria.ShowTitle);

                return CommonFunction.GetVideoAndImage(filePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream ExportRegistrationsData(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Generic.", CurrentUser);

                string folderPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\"
                    + DateTime.Now.Year.ToString()
                    + DateTime.Now.Month.ToString()
                    + DateTime.Now.Day.ToString()
                    ;

                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(folderPath);
                }
              
                string filePath = folderPath + "\\" + aSeachPtRegistrationCriteria.SafeFileName  + aSeachPtRegistrationCriteria.SafeFileName.Substring(aSeachPtRegistrationCriteria.SafeFileName.IndexOf('.') + 1).ToLower();

                aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportReportToExcelForRegistrationsData(aSeachPtRegistrationCriteria, filePath);
                return CommonFunction.GetVideoAndImage(filePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream ExportFromExcel(ReportParameters criteria, long staffID)
        {
            try
            {
                string path = "";
                if (criteria.Quarter == 1)
                {
                    path = Globals.AxServerSettings.Servers.HospitalStatisticsQuarter1;
                }
                else if(criteria.Quarter == 2)
                {
                    path = Globals.AxServerSettings.Servers.HospitalStatisticsQuarter2;
                }
                else if(criteria.Quarter == 3)
                {
                    path = Globals.AxServerSettings.Servers.HospitalStatisticsQuarter3;
                }
                else if (criteria.Quarter == 4)
                {
                    path = Globals.AxServerSettings.Servers.HospitalStatisticsQuarter4;
                }

                switch (criteria.reportName)
                {
                    case ReportName.HR_STATISTICS_BY_DEPT:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_HRStatistics;
                        break;
                    case ReportName.MED_EXAM_ACTIVITY:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_MedExamActivity;
                        break;
                    case ReportName.TREATMENT_ACTIVITY:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_TreatmentActivity;
                        break;
                    case ReportName.SPECIALIST_TREATMENT_ACTIVITY:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_SpecialistTreatmentActivity;
                        break;
                    case ReportName.SURGERY_ACTIVITY:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_SurgeryActivity;
                        break;
                    case ReportName.REPRODUCTIVE_HEALTH_ACTIVITY:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_ReproductiveHealthActivity;
                        break;
                    case ReportName.PCL_ACTIVITY:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_PCLActivity;
                        break;
                    case ReportName.PHARMACY_DEPT_STATISTICS:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_PharmacyDeptStatistics;
                        break;
                    case ReportName.MEDICAL_EQUIPMENT_STATISTICS:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_MedicalEquipmentStatistics;
                        break;
                    case ReportName.SCIENTIFIC_RESEARCH_ACTIVITY:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_ScientificResearchActivity;
                        break;
                    case ReportName.FINANCIAL_ACTIVITY_TEMP1:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_FinancialActivityTemp1;
                        break;
                    case ReportName.FINANCIAL_ACTIVITY_TEMP2:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_FinancialActivityTemp2;
                        break;
                    case ReportName.FINANCIAL_ACTIVITY_TEMP3:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_FinancialActivityTemp3;
                        break;
                    case ReportName.ICD10_STATISTICS:
                        path += Globals.AxServerSettings.Servers.HospitalStatistics_ICD10Statistics;
                        break;
                }

                return CommonFunction.GetVideoAndImage(path);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ExportFromExcel. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_CANNOT_EXPORT_FROM_EXCEL, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream ExportEInvoiceToExcel(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                List<List<string>> mReportStringCollection = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportEInvoiceToExcel(aSeachPtRegistrationCriteria);
                string mDirPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                if (!Directory.Exists(mDirPath))
                {
                    Directory.CreateDirectory(mDirPath);
                }
                var mFilePath = ExportAll(mReportStringCollection, "EInvoiceData", mDirPath + "\\" + "EInvoiceData", aSeachPtRegistrationCriteria.SafeFileName);
                return CommonFunction.GetVideoAndImage(mFilePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Export To Excel. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream ExportHospitalClientContractDetails_Excel(long HosClientContractID)
        {
            try
            {
                List<List<string>> mReportStringCollection = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.HospitalClientContractDetails_Excel(HosClientContractID);
                string mDirPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                if (!Directory.Exists(mDirPath))
                {
                    Directory.CreateDirectory(mDirPath);
                }
                var mFilePath = ExportAll(mReportStringCollection, "HospitalClientData", mDirPath + "\\" + "HospitalClientData", new Guid().ToString() + ".xls");
                return CommonFunction.GetVideoAndImage(mFilePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Export To Excel. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream ExportHospitalClientContractResultDetails_Excel(long HosClientContractID)
        {
            try
            {
                List<List<string>> mReportStringCollection = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.HospitalClientContractResultDetails_Excel(HosClientContractID);
                string mDirPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                if (!Directory.Exists(mDirPath))
                {
                    Directory.CreateDirectory(mDirPath);
                }
                var mFilePath = ExportAll(mReportStringCollection, "HospitalClientData", mDirPath + "\\" + "HospitalClientData", new Guid().ToString() + ".xls");
                return CommonFunction.GetVideoAndImage(mFilePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Export To Excel. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        public List<List<string>> ExportToExcellAll_Temp25a(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Temp25a.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp25a(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_Temp25a(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp25a(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public string ExportToExcellAll_Temp25aNew(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Temp25a.", CurrentUser);
                List<List<string>> res;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp25a(criteria);
                //}
                //else
                //{
                //    res = TransactionProvider.Instance.ExportToExcellAll_Temp25a(criteria);
                //}
                res = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp25a(criteria);
                //ghi xuong file o ben server luon
                //xong roi copy ve client
                string folderPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\"
                    + DateTime.Now.Year.ToString()
                    + DateTime.Now.Month.ToString()
                    + DateTime.Now.Day.ToString()
                    ;
                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(folderPath);
                }
                return ExportAll(res, criteria.Show, folderPath + "\\" + criteria.StoreName, criteria.ShowTitle);
                
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_Temp26a(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Temp26a.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp26a(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_Temp26a(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp26a(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_Temp20NgoaiTru(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Temp20NgoaiTru.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp20NgoaiTru(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_Temp20NgoaiTru(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp20NgoaiTru(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_Temp20NoiTru(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Temp20NoiTru.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp20NoiTru(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_Temp20NoiTru(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp20NoiTru(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_Temp21NgoaiTru(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Temp21NgoaiTru.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp21NgoaiTru(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_Temp21NgoaiTru(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp21NgoaiTru(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_Temp21NoiTru(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_Temp21NoiTru.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp21NoiTru(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_Temp21NoiTru(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_Temp21NoiTru(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_ChiTietVienPhi_PK(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_ChiTietVienPhi_PK.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_ChiTietVienPhi_PK(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_ChiTietVienPhi_PK(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_ChiTietVienPhi_PK(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_ChiTietVienPhi_PK. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_ChiTietVienPhi(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_ChiTietVienPhi.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_ChiTietVienPhi(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_ChiTietVienPhi(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_ChiTietVienPhi(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_ChiTietVienPhi. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_ThongKeDoanhThu(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_ThongKeDoanhThu.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_ThongKeDoanhThu(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_ThongKeDoanhThu(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_ThongKeDoanhThu(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_ThongKeDoanhThu. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_DoanhThuTongHop(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_DoanhThuTongHop.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_DoanhThuTongHop(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_DoanhThuTongHop(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_DoanhThuTongHop(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_DoanhThuTongHop. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_DoanhThuNoiTruTongHop(ReportParameters criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcellAll_DoanhThuNoiTruTongHop.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_DoanhThuNoiTruTongHop(criteria);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.ExportToExcellAll_DoanhThuNoiTruTongHop(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellAll_DoanhThuNoiTruTongHop(criteria);
                //AxLogger.Instance.LogInfo("End of GetTransaction_ByPtID. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_DoanhThuNoiTruTongHop. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        public bool FollowUserGetReport(long staffID, string reportName, string reportParams, long v_GetReportMethod)
        {
            try
            {
                //AxLogger.Instance.LogInfo("Start loading FollowUserGetReport.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.FollowUserGetReport(staffID, reportName, reportParams, v_GetReportMethod);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.FollowUserGetReport(staffID, reportName, reportParams, v_GetReportMethod);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.FollowUserGetReport(staffID, reportName, reportParams, v_GetReportMethod);
                //AxLogger.Instance.LogInfo("End loading FollowUserGetReport.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading FollowUserGetReport. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.REPORT_CANNOT_INSERT, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool CreateHIReport(HealthInsuranceReport HIReport)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading CreateHIReport.", CurrentUser);
                long HIReportID;
                bool bRet;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateHIReport(HIReport, out HIReportID);
                //}
                //else
                //{
                //    bRet = TransactionProvider.Instance.CreateHIReport(HIReport, out HIReportID);
                //}
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateHIReport(HIReport, out HIReportID);
                AxLogger.Instance.LogInfo("End loading CreateHIReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CreateHIReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool CreateFastReport(HealthInsuranceReport gReport, long V_FastReportType, out long FastReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading CreateFastReport.", CurrentUser);
                bool bRet;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateFastReport(gReport, out FastReportID);
                //}
                //else
                //{
                //    bRet = TransactionProvider.Instance.CreateFastReport(gReport, out FastReportID);
                //}
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateFastReport(gReport, V_FastReportType, out FastReportID);
                AxLogger.Instance.LogInfo("End loading CreateFastReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CreateFastReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool TransferFastReport(long FastReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading TransferFastReport.", CurrentUser);
                bool bRet;
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.TransferFastReport(FastReportID);
                AxLogger.Instance.LogInfo("End loading TransferFastReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading TransferFastReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼===== #003
        public bool DeleteFastReport(long FastReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Delete Fast Report.", CurrentUser);
                bool bRet;
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteFastReport(FastReportID);
                AxLogger.Instance.LogInfo("End loading Delete Fast Report.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Delete Fast Report. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== #003
        public void DeleteFastLinkedReportDetail(int Case, long FastReportID, string so_ct, bool noi_tru, string ma_kh, string ma_bp, string ma_kho)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteFastLinkedReportDetail(Case, FastReportID, so_ct, noi_tru, ma_kh, ma_bp, ma_kho);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading DeleteFastLinkedReportDetail. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool DeleteRegistrationHIReport(string ma_lk)
        {
            try
            {
                bool bRet;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteRegistrationHIReport(ma_lk);
                //}
                //else
                //{
                //    bRet = TransactionProvider.Instance.DeleteRegistrationHIReport(ma_lk);
                //}
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteRegistrationHIReport(ma_lk);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading DeleteRegistrationHIReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PatientRegistration> SearchRegistrationsForHIReport(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading SearchRegistrationsForHIReport.", CurrentUser);
                IList<PatientRegistration> bRet;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.SearchRegistrationsForHIReport(aSeachPtRegistrationCriteria);
                //}
                //else
                //{
                //    bRet = TransactionProvider.Instance.SearchRegistrationsForHIReport(aSeachPtRegistrationCriteria);
                //}
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.SearchRegistrationsForHIReport(aSeachPtRegistrationCriteria, ViewCase);
                AxLogger.Instance.LogInfo("End loading SearchRegistrationsForHIReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading SearchRegistrationsForHIReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PatientRegistration_V2> SearchRegistrationsForCreateOutPtTransactionFinalization(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading SearchRegistrationsForCreateOutPtTransactionFinalization.", CurrentUser);
                IList<PatientRegistration_V2> bRet;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.SearchRegistrationsForCreateOutPtTransactionFinalization(aSeachPtRegistrationCriteria);
                //}
                //else
                //{
                //    bRet = TransactionProvider.Instance.SearchRegistrationsForCreateOutPtTransactionFinalization(aSeachPtRegistrationCriteria);
                //}
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.SearchRegistrationsForCreateOutPtTransactionFinalization(aSeachPtRegistrationCriteria);
                AxLogger.Instance.LogInfo("End loading SearchRegistrationsForCreateOutPtTransactionFinalization.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading SearchRegistrationsForCreateOutPtTransactionFinalization. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<OutPtTransactionFinalization> GetTransactionFinalizationSummaryInfos(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetTransactionFinalizationSummaryInfos.", CurrentUser);
                IList<OutPtTransactionFinalization> bRet;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetTransactionFinalizationSummaryInfos(aSeachPtRegistrationCriteria);
                //}
                //else
                //{
                //    bRet = TransactionProvider.Instance.GetTransactionFinalizationSummaryInfos(aSeachPtRegistrationCriteria);
                //}
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetTransactionFinalizationSummaryInfos(aSeachPtRegistrationCriteria);
                AxLogger.Instance.LogInfo("End loading GetTransactionFinalizationSummaryInfos.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetTransactionFinalizationSummaryInfos. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PatientRegistration_V2> SearchRegistrationsForCreateEInvoices(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.SearchRegistrationsForCreateEInvoices(aSeachPtRegistrationCriteria); ;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading SearchRegistrationsForCreateEInvoices. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateHIReportStatus(HealthInsuranceReport aHealthInsuranceReport)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading UpdateHIReportStatus.", CurrentUser);
                bool bRet;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.UpdateHIReportStatus(aHealthInsuranceReport);
                //}
                //else
                //{
                //    bRet = TransactionProvider.Instance.UpdateHIReportStatus(aHealthInsuranceReport);
                //}
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.UpdateHIReportStatus(aHealthInsuranceReport);
                AxLogger.Instance.LogInfo("End loading UpdateHIReportStatus.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading UpdateHIReportStatus. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool CreateHIReport_V2(HealthInsuranceReport HIReport, out long HIReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading CreateHIReport_V2.", CurrentUser);
                bool bRet;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateHIReport(HIReport, out HIReportID);
                //}
                //else
                //{
                //    bRet = TransactionProvider.Instance.CreateHIReport(HIReport, out HIReportID);
                //}
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateHIReport(HIReport, out HIReportID);
                AxLogger.Instance.LogInfo("End loading CreateHIReport_V2.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CreateHIReport_V2. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<HealthInsuranceReport> GetHIReport()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetHIReport.", CurrentUser);
                List<HealthInsuranceReport> listHIReps = new List<HealthInsuranceReport>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    listHIReps = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetHIReport();
                //}
                //else
                //{
                //    listHIReps = TransactionProvider.Instance.GetHIReport();
                //}
                listHIReps = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetHIReport();
                AxLogger.Instance.LogInfo("End loading GetHIReport.", CurrentUser);
                return listHIReps;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading FollowUserGetReport. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<HealthInsuranceReport> GetFastReports(long V_FastReportType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetFastReports.", CurrentUser);
                List<HealthInsuranceReport> listHIReps = new List<HealthInsuranceReport>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    listHIReps = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetFastReports();
                //}
                //else
                //{
                //    listHIReps = TransactionProvider.Instance.GetFastReports();
                //}
                listHIReps = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetFastReports(V_FastReportType);
                AxLogger.Instance.LogInfo("End loading GetFastReports.", CurrentUser);
                return listHIReps;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetFastReports. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public DataSet GetFastReportDetails(long FastReportID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetFastReportDetails(FastReportID);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.GetFastReportDetails(FastReportID);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetFastReportDetails(FastReportID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading GetFastReportDetails. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public Stream GetHIXmlReport9324_AllTab123_InOneRpt(long nHIReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Creating HIReport-9324-917 in XML format...", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetHIXmlReport9324_AllTab123_InOneRpt_Data(nHIReportID);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.GetHIXmlReport9324_AllTab123_InOneRpt_Data(nHIReportID);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetHIXmlReport9324_AllTab123_InOneRpt_Data(nHIReportID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of lCreating HIReport-9324-917 in XML format. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▼====: #001*/
        public HOSPayment EditHOSPayment(HOSPayment aHOSPayment)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading EditHOSPayment", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.EditHOSPayment(aHOSPayment);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.EditHOSPayment(aHOSPayment);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.EditHOSPayment(aHOSPayment);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading EditHOSPayment. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<HOSPayment> GetHOSPayments(DateTime aStartDate, DateTime aEndDate)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetHOSPayments", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetHOSPayments(aStartDate, aEndDate);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.GetHOSPayments(aStartDate, aEndDate);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetHOSPayments(aStartDate, aEndDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading GetHOSPayments. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool DeleteHOSPayment(long aHOSPaymentID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading DeleteHOSPayment", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteHOSPayment(aHOSPaymentID);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.DeleteHOSPayment(aHOSPaymentID);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteHOSPayment(aHOSPaymentID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading DeleteHOSPayment. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▲====: #001*/
        public DataSet PreviewHIReport(long PtRegistrationID, long V_RegistrationType, long? OutPtTreatmentProgramID, out string ErrText)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.PreviewHIReport(PtRegistrationID, V_RegistrationType, out ErrText);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.PreviewHIReport(PtRegistrationID, V_RegistrationType, out ErrText);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.PreviewHIReport(PtRegistrationID, V_RegistrationType, OutPtTreatmentProgramID, out ErrText);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading PreviewHIReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼====: #002
        public DataSet PreviewHIReport_ForKHTH(long PtRegistrationID, long V_RegistrationType, out string ErrText)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.PreviewHIReport(PtRegistrationID, V_RegistrationType, out ErrText);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.PreviewHIReport(PtRegistrationID, V_RegistrationType, out ErrText);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.PreviewHIReport_ForKHTH(PtRegistrationID, V_RegistrationType, out ErrText);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading PreviewHIReport_ForKHTH. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #002
        //▼====: #001
        public List<List<string>> ExportExcelRegistrationsForHIReportWaitConfirm(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ExportExcelRegistrationsForHIReportWaitConfirm.", CurrentUser);
                List<List<string>> bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportExcelRegistrationsForHIReportWaitConfirm(aSeachPtRegistrationCriteria);
                AxLogger.Instance.LogInfo("End loading ExportExcelRegistrationsForHIReportWaitConfirm.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportExcelRegistrationsForHIReportWaitConfirm. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<List<string>> ExportExcelRegistrationsForHIReportOther(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ExportExcelRegistrationsForHIReportOther.", CurrentUser);
                List<List<string>> bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportExcelRegistrationsForHIReportOther(aSeachPtRegistrationCriteria);
                AxLogger.Instance.LogInfo("End loading ExportExcelRegistrationsForHIReportOther.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportExcelRegistrationsForHIReportOther. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #001
        public List<RptOutPtTransactionFinalizationDetail> GetRptOutPtTransactionFinalizationDetails(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetRptOutPtTransactionFinalizationDetails(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading GetRptOutPtTransactionFinalizationDetails. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public DQGReport CreateDQGReport(DQGReport aDQGReport)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateDQGReport(aDQGReport);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading CreateDQGReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool UpdateCodeDQGReport(DQGReport aDQGReport, byte aCase)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.UpdateCodeDQGReport(aDQGReport, aCase);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading UpdateCodeDQGReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<DQGReport> GetDQGReports()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetDQGReports();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading GetDQGReports. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public DQGReport GetDQGReportWithDetails(long DQGReportID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetDQGReportWithDetails(DQGReportID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading GetDQGReportWithDetails. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public DataSet GetDQGReportAllDetails(long DQGReportID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetDQGReportAllDetails(DQGReportID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading GetDQGReportAllDetails. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<PositionInHospital> GetAllPositionInHospital()
        {
            try
            {
                string CurrentCacheKey = "AllPositionInHospital";
                if (ServerAppConfig.CachingEnabled && (List<PositionInHospital>)AxCache.Current[CurrentCacheKey] == null)
                {
                    var AllPositionInHospital = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetAllPositionInHospital();
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[CurrentCacheKey] = AllPositionInHospital;
                    }
                    else
                    {
                        AxCache.Current.Insert(CurrentCacheKey, AllPositionInHospital, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                    }
                }
                return (List<PositionInHospital>)AxCache.Current[CurrentCacheKey];
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading GetAllPositionInHospital. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼===== #005
        public Stream GetXmlReportForCongDLQG(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                AxLogger.Instance.LogInfo("Creating XML For Cong DLQG...", CurrentUser);
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetXmlReportForCongDLQG(FromDate, ToDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Creating XML For Cong DLQG. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== #005
        //▼===== #006
        public bool MarkHIReportByVAS(long HIReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Begin of mark hi report by VAS", CurrentUser);
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.MarkHIReportByVAS(HIReportID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of mark hi report by VAS. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== #006

        public IList<DiseasesReference> GetTreatmentStatisticsByDepartment(long DeptID, DateTime FromDate, DateTime ToDate, byte RegistrationStatus, out decimal OutTotalQuantity)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetTreatmentStatisticsByDepartment(DeptID, FromDate, ToDate, RegistrationStatus, out OutTotalQuantity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading GetTreatmentStatisticsByDepartment. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PatientRegistration> GetTreatmentStatisticsByDepartmentDetail(long DeptID, DateTime FromDate, DateTime ToDate, byte RegistrationStatus
            , decimal? FilerByTime
            , decimal? FilerByMoney)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetTreatmentStatisticsByDepartmentDetail(DeptID, FromDate, ToDate, RegistrationStatus
                    , FilerByTime
                    , FilerByMoney);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading GetTreatmentStatisticsByDepartmentDetail. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool CreateHIReportOutInPt(HealthInsuranceReport HIReport, out long HIReportID, out long HIReportOutPt)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading CreateHIReportOutInPt.", CurrentUser);
                bool bRet = true;
                bool bRetInPt = true;
                bool bOK;
                string ListOutPtRegistration = "";
                string ListInPtRegistration = "";
                HIReportOutPt = 0;
                HIReportID = 0;
                if (HIReport != null && !string.IsNullOrEmpty(HIReport.RegistrationIDList))
                {
                    string[] tmpListRegistration = HIReport.RegistrationIDList.Split(',');
                    foreach (var item in tmpListRegistration)
                    {
                        if (item.Substring(0, 2) == "1-")
                        {
                            if (string.IsNullOrEmpty(ListOutPtRegistration))
                            {
                                ListOutPtRegistration = string.Concat(ListOutPtRegistration, "", item);
                            }
                            else
                            {
                                ListOutPtRegistration = string.Concat(ListOutPtRegistration, ",", item);
                            }
                        }
                        if (item.Substring(0, 2) == "3-")
                        {
                            if (string.IsNullOrEmpty(ListInPtRegistration))
                            {
                                ListInPtRegistration = string.Concat(ListInPtRegistration, "", item);
                            }
                            else
                            {
                                ListInPtRegistration = string.Concat(ListInPtRegistration, ",", item);
                            }
                        }
                    }
                }
                HealthInsuranceReport HIReportForOutPt = HIReport.EntityDeepCopy();
                if (HIReportForOutPt != null && !string.IsNullOrEmpty(ListOutPtRegistration))
                {
                    HIReportForOutPt.Title = ListOutPtRegistration;
                    HIReportForOutPt.RegistrationIDList = ListOutPtRegistration;
                    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateHIReport_OutPt(HIReportForOutPt, out HIReportOutPt);
                }

                HealthInsuranceReport HIReportForInPt = HIReport.EntityDeepCopy();

                if (HIReportForInPt != null && !string.IsNullOrEmpty(ListInPtRegistration))
                {
                    HIReportForInPt.Title = ListInPtRegistration;
                    HIReportForInPt.RegistrationIDList = ListInPtRegistration;
                    bRetInPt = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateHIReport_InPt(HIReportForInPt, out HIReportID);
                }

                AxLogger.Instance.LogInfo("End loading CreateHIReportOutInPt.", CurrentUser);
                if (bRet && bRetInPt)
                {
                    bOK = true;
                }
                else
                {
                    bOK = false;
                }
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CreateHIReport_V2. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream ExportToExcelForHIReport(ReportParameters RptParameters, int PatientTypeIndex, long staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ExportToExcelForHIReport.", CurrentUser);
                string folderPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\"
                    + DateTime.Now.Year.ToString()
                    + DateTime.Now.Month.ToString()
                    + DateTime.Now.Day.ToString()
                    ;

                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(folderPath);
                }

                //KMx: Nếu như không có Show (SheetName) thì phải set default, nếu không thì xuất excel xong rồi mở file lên không được (06/08/2014 16:24).
                //if (string.IsNullOrEmpty(criteria.Show))
                //{
                //    criteria.Show = "Sheet1";
                //}
                string filePath = folderPath + "\\"+ DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                //filePath = ExportAll(res, criteria.Show, folderPath + "\\" + criteria.StoreName, criteria.ShowTitle);

                string reportName = "";
                //string reportParameter = GetReportParameterString(criteria);
                aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.ExportToExcellForHIReport(RptParameters, PatientTypeIndex, filePath, staffID);
             

                return CommonFunction.GetVideoAndImage(filePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ExportToExcellForHIReport. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.TRANSACTION_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼====: #008
        public bool CreateDTDTReportOutInPt(DTDTReport DTDTReport, out long DTDTReportID, out long DTDTReportOutPt)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading CreateDTDTReportOutInPt.", CurrentUser);
                bool bRet = true;
                bool bRetInPt = true;
                bool bOK;
                string ListOutPtRegistration = "";
                string ListInPtRegistration = "";
                DTDTReportOutPt = 0;
                DTDTReportID = 0;
                if (DTDTReport != null && !string.IsNullOrEmpty(DTDTReport.RegistrationIDList))
                {
                    string[] tmpListRegistration = DTDTReport.RegistrationIDList.Split(',');
                    foreach (var item in tmpListRegistration)
                    {
                        if (item.Substring(0, 2) == "1-")
                        {
                            if (string.IsNullOrEmpty(ListOutPtRegistration))
                            {
                                ListOutPtRegistration = string.Concat(ListOutPtRegistration, "", item);
                            }
                            else
                            {
                                ListOutPtRegistration = string.Concat(ListOutPtRegistration, ",", item);
                            }
                        }
                        if (item.Substring(0, 2) == "3-")
                        {
                            if (string.IsNullOrEmpty(ListInPtRegistration))
                            {
                                ListInPtRegistration = string.Concat(ListInPtRegistration, "", item);
                            }
                            else
                            {
                                ListInPtRegistration = string.Concat(ListInPtRegistration, ",", item);
                            }
                        }
                    }
                }
                DTDTReport DTDTReportForOutPt = DTDTReport.EntityDeepCopy();
                if (DTDTReportForOutPt != null && !string.IsNullOrEmpty(ListOutPtRegistration))
                {
                    DTDTReportForOutPt.RegistrationIDList = ListOutPtRegistration;
                    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateDTDTReport_OutPt(DTDTReportForOutPt, out DTDTReportOutPt);
                }

                DTDTReport DTDTReportForInPt = DTDTReport.EntityDeepCopy();

                if (DTDTReportForInPt != null && !string.IsNullOrEmpty(ListInPtRegistration))
                {
                    
                    DTDTReportForInPt.RegistrationIDList = ListInPtRegistration;
                    bRetInPt = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateDTDTReport_InPt(DTDTReportForInPt, out DTDTReportID);
                }

                AxLogger.Instance.LogInfo("End loading CreateDTDTReportOutInPt.", CurrentUser);
                if (bRet && bRetInPt)
                {
                    bOK = true;
                }
                else
                {
                    bOK = false;
                }
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CreateDTDTReportOutInPt. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<DTDT_don_thuoc> GetDTDTData(long nDTDTReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Creating DTDT JSON...", CurrentUser);
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetDTDTData(nDTDTReportID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of lCreating DTDT JSON format. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<DTDT_don_thuoc> GetDTDT_InPtData(long nDTDTReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Creating DTDT JSON...", CurrentUser);
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetDTDT_InPtData(nDTDTReportID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of lCreating DTDT JSON format. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateDTDTReportStatus(DTDTReport aDTDTReport)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading UpdateDTDTReportStatus.", CurrentUser);
                bool bRet;
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.UpdateDTDTReportStatus(aDTDTReport);
                AxLogger.Instance.LogInfo("End loading UpdateDTDTReportStatus.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading UpdateDTDTReportStatus. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #008
        //▼====: #009
        public bool DeleteRegistrationDTDTReport(string ListPrescription)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading DeleteRegistrationDTDTReport.", CurrentUser);
                bool bRet;
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteRegistrationDTDTReport(ListPrescription);
                AxLogger.Instance.LogInfo("End loading DeleteRegistrationDTDTReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading DeleteRegistrationDTDTReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteDTDTReport(long DTDTReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading DeleteDTDTReport.", CurrentUser);
                bool bRet;
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteDTDTReport(DTDTReportID);
                AxLogger.Instance.LogInfo("End loading DeleteDTDTReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading DeleteDTDTReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool DeleteRegistrationDTDT_InPtReport(string ListPrescription)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading DeleteRegistrationDTDTReport.", CurrentUser);
                bool bRet;
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteRegistrationDTDT_InPtReport(ListPrescription);
                AxLogger.Instance.LogInfo("End loading DeleteRegistrationDTDTReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading DeleteRegistrationDTDTReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteDTDT_InPtReport(long DTDTReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading DeleteDTDTReport.", CurrentUser);
                bool bRet;
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteDTDT_InPtReport(DTDTReportID);
                AxLogger.Instance.LogInfo("End loading DeleteDTDTReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading DeleteDTDTReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #009
        public bool CancelConfirmDTDTReport(long PtRegistrationID, long V_RegistrationType, long StaffID, string CancelReason)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading CancelConfirmDTDTReport.", CurrentUser);
                bool bRet;
                bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CancelConfirmDTDTReport(PtRegistrationID, V_RegistrationType, StaffID, CancelReason);
                AxLogger.Instance.LogInfo("End loading CancelConfirmDTDTReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CancelConfirmDTDTReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼====: #010
        public bool CreateDQGReportOutInpt(DQGReport aDQGReport, out long DQGReportID, out long DQGReportIDInpt)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading CreateDQGReportOutInpt.", CurrentUser);
                bool bOK;
                bool bRet = true;
                bool bRetInpt = true;
                string ListOutPtIssueID = "";
                string ListInPtIssueID = "";
                DQGReportID = 0;
                DQGReportIDInpt = 0;
                if (aDQGReport != null && !string.IsNullOrEmpty(aDQGReport.IssueIDList))
                {
                    string[] tmpListRegistration = aDQGReport.IssueIDList.Split(',');
                    foreach (var item in tmpListRegistration)
                    {
                        if (item.Substring(0, 2) == "1-")
                        {
                            if (string.IsNullOrEmpty(ListOutPtIssueID))
                            {
                                ListOutPtIssueID = string.Concat(ListOutPtIssueID, "", item);
                            }
                            else
                            {
                                ListOutPtIssueID = string.Concat(ListOutPtIssueID, ",", item);
                            }
                        }
                        if (item.Substring(0, 2) == "3-")
                        {
                            if (string.IsNullOrEmpty(ListInPtIssueID))
                            {
                                ListInPtIssueID = string.Concat(ListInPtIssueID, "", item);
                            }
                            else
                            {
                                ListInPtIssueID = string.Concat(ListInPtIssueID, ",", item);
                            }
                        }
                    }
                }

                DQGReport DQGReportOutpt = aDQGReport.EntityDeepCopy();
                if (DQGReportOutpt != null && !string.IsNullOrEmpty(ListOutPtIssueID))
                {
                    DQGReportOutpt.Title = ListOutPtIssueID;
                    DQGReportOutpt.IssueIDList = ListOutPtIssueID;
                    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateDQGReport_Outpt(DQGReportOutpt, out DQGReportID);
                }

                DQGReport DQGReportInpt = aDQGReport.EntityDeepCopy();
                if (DQGReportInpt != null && !string.IsNullOrEmpty(ListInPtIssueID))
                {
                    DQGReportInpt.Title = ListInPtIssueID;
                    DQGReportInpt.IssueIDList = ListInPtIssueID;
                    //bRetInpt = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateDQGReport_Outpt(DQGReportOutpt, out DQGReportID);
                    DQGReportIDInpt = 0;
                }
                if (bRet && bRetInpt)
                {
                    bOK = true;
                }
                else
                {
                    bOK = false;
                }
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading CreateDQGReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool DeleteDQGReportOutInpt(long PtRegistrationID, long IssueID, long DQGReportID, long StaffID, string CancelReason, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading DeleteDTQGReport.", CurrentUser);
                bool bRet = false;
                if(V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.DeleteDQGReportOutpt(PtRegistrationID, IssueID, DQGReportID, StaffID, CancelReason, V_RegistrationType);
                }
                AxLogger.Instance.LogInfo("End loading DeleteDTQGReport.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading DeleteDTQGReport. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #010
        //▼====: #011
        public bool CreateHIReport_130_OutInPt(HealthInsuranceReport HIReport, out long HIReportID, out long HIReportOutPt)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading CreateHIReportOutInPt.", CurrentUser);
                bool bRet = true;
                bool bRetInPt = true;
                bool bOK;
                string ListOutPtRegistration = "";
                string ListInPtRegistration = "";
                HIReportOutPt = 0;
                HIReportID = 0;
                if (HIReport != null && !string.IsNullOrEmpty(HIReport.RegistrationIDList))
                {
                    string[] tmpListRegistration = HIReport.RegistrationIDList.Split(',');
                    foreach (var item in tmpListRegistration)
                    {
                        if (item.Substring(0, 2) == "1-")
                        {
                            if (string.IsNullOrEmpty(ListOutPtRegistration))
                            {
                                ListOutPtRegistration = string.Concat(ListOutPtRegistration, "", item);
                            }
                            else
                            {
                                ListOutPtRegistration = string.Concat(ListOutPtRegistration, ",", item);
                            }
                        }
                        if (item.Substring(0, 2) == "3-")
                        {
                            if (string.IsNullOrEmpty(ListInPtRegistration))
                            {
                                ListInPtRegistration = string.Concat(ListInPtRegistration, "", item);
                            }
                            else
                            {
                                ListInPtRegistration = string.Concat(ListInPtRegistration, ",", item);
                            }
                        }
                    }
                }
                HealthInsuranceReport HIReportForOutPt = HIReport.EntityDeepCopy();
                if (HIReportForOutPt != null && !string.IsNullOrEmpty(ListOutPtRegistration))
                {
                    HIReportForOutPt.Title = ListOutPtRegistration;
                    HIReportForOutPt.RegistrationIDList = ListOutPtRegistration;
                    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateHIReport_130_OutPt(HIReportForOutPt, out HIReportOutPt);
                }

                HealthInsuranceReport HIReportForInPt = HIReport.EntityDeepCopy();

                if (HIReportForInPt != null && !string.IsNullOrEmpty(ListInPtRegistration))
                {
                    HIReportForInPt.Title = ListInPtRegistration;
                    HIReportForInPt.RegistrationIDList = ListInPtRegistration;
                    bRetInPt = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.CreateHIReport_130_InPt(HIReportForInPt, out HIReportID);
                }

                AxLogger.Instance.LogInfo("End loading CreateHIReport_130_OutInPt.", CurrentUser);
                if (bRet && bRetInPt)
                {
                    bOK = true;
                }
                else
                {
                    bOK = false;
                }
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CreateHIReport_130_OutInPt. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream GetHIXmlReport_130_AllTab_InOneRpt(long nHIReportID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Creating HIReport-130 in XML format...", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetHIXmlReport9324_AllTab123_InOneRpt_Data(nHIReportID);
                //}
                //else
                //{
                //    return TransactionProvider.Instance.GetHIXmlReport9324_AllTab123_InOneRpt_Data(nHIReportID);
                //}
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetHIXmlReport_130_AllTab_InOneRpt_Data(nHIReportID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of lCreating HIReport-130 in XML format. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public Stream GetHIXmlReport_130_CheckIn_OutPt(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Creating HIReport-130 in XML format...", CurrentUser);
                return aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.GetHIXmlReport_130_CheckIn_OutPt(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of lCreating HIReport-130 in XML format. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_LOAD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #011
    }
}