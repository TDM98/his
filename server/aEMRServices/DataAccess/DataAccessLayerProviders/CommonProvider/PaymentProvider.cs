/*
 * 20170609 #001 CMN: Fix SupportFund About TT04 with TT04
 * 20170619 #002 CMN: Service for payment report OutPt with large data
 * 20200423 #003 TTM: BM 0037150: Chỉnh sửa việc gọi store Xác nhận quyền lợi bảo hiểm.
 * 20220526 #004 DatTB: Thêm chức năng Xác nhận bn hoãn/ miễn tạm ứng
 * 20220511 #005 QTD: Chỉnh func thêm biến isGetLast cho màn hình xác nhận BHYT
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using eHCMS.Configurations;
using System.Data.Common;

using eHCMS.DAL;
using eHCMS.Services.Core;

namespace aEMR.DataAccessLayer.Providers
{
    public class PaymentProvider : DataProviderBase
    {
        static private PaymentProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public PaymentProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PaymentProvider();
                }
                return _instance;
            }
        }

        public PaymentProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }

        public virtual PatientTransactionPayment getPatientTransactionPaymentObjectFromReader(IDataReader reader)
        {
            PatientTransactionPayment p = base.GetPatientTransactionPaymentFromReader(reader);

            return p;
        }

        public  List<PatientTransactionPayment> GetAllPayments_New(long transactionID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {                
                cn.Open();

                SqlCommand cmd = cn.CreateCommand();
                cmd.Transaction = null;
                cmd.CommandText = "spGetAllPayments_New";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

                IDataReader reader = ExecuteReader(cmd);
                if (reader == null)
                {
                    return null;
                }
                List<PatientTransactionPayment> retVal = new List<PatientTransactionPayment>();
                while (reader.Read())
                {
                    PatientTransactionPayment temp = base.GetPatientTransactionPaymentFromReader(reader);
                    retVal.Add(temp);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public  List<PatientTransactionPayment> GetAllPayments_InPt(long transactionID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.Transaction = null;
                cmd.CommandText = "spGetAllPayments_InPt";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

                IDataReader reader = ExecuteReader(cmd);
                if (reader == null)
                {
                    return null;
                }
                List<PatientTransactionPayment> retVal = new List<PatientTransactionPayment>();
                while (reader.Read())
                {
                    PatientTransactionPayment temp = base.GetPatientTransactionPaymentFromReader(reader);

                    if (reader["PaymentModeLookupID"] != DBNull.Value)
                    {
                        //Get them thong tin tu bang lookup.
                        Lookup paymentModeInfo = new Lookup();

                        paymentModeInfo.LookupID = (long)reader["PaymentModeLookupID"];
                        paymentModeInfo.ObjectTypeID = (long)reader["PaymentModeObjectTypeID"];
                        paymentModeInfo.ObjectName = reader["PaymentModeObjectName"] as string;
                        paymentModeInfo.ObjectValue = reader["PaymentModeObjectValue"] as string;
                        paymentModeInfo.ObjectNotes = reader["PaymentModeObjectNotes"] as string;

                        temp.PaymentMode = paymentModeInfo;
                    }

                    if (reader["CurrencyLookupID"] != DBNull.Value)
                    {
                        //Get them thong tin tu bang lookup.
                        Lookup currencyInfo = new Lookup();

                        currencyInfo.LookupID = (long)reader["CurrencyLookupID"];
                        currencyInfo.ObjectTypeID = (long)reader["CurrencyObjectTypeID"];
                        currencyInfo.ObjectName = reader["CurrencyObjectName"] as string;
                        currencyInfo.ObjectValue = reader["CurrencyObjectValue"] as string;
                        currencyInfo.ObjectNotes = reader["CurrencyObjectNotes"] as string;

                        temp.Currency = currencyInfo;
                    }

                    if (reader["PaymentTypeLookupID"] != DBNull.Value)
                    {
                        //Get them thong tin tu bang lookup.
                        Lookup paymentTypeInfo = new Lookup();

                        paymentTypeInfo.LookupID = (long)reader["PaymentTypeLookupID"];
                        paymentTypeInfo.ObjectTypeID = (long)reader["PaymentTypeObjectTypeID"];
                        paymentTypeInfo.ObjectName = reader["PaymentTypeObjectName"] as string;
                        paymentTypeInfo.ObjectValue = reader["PaymentTypeObjectValue"] as string;
                        paymentTypeInfo.ObjectNotes = reader["PaymentTypeObjectNotes"] as string;

                        temp.PaymentType = paymentTypeInfo;
                    }

                    retVal.Add(temp);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public  PatientTransaction GetAllPaymentByRegistrationID_InPt(long registrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetAllPaymentByRegistrationID_InPt(registrationID, cn, null);
            }
        }

        public  List<PatientTransactionPayment> GetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch, int FindPatient)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetPatientPaymentByDay_New(PatientPaymentSearch, FindPatient, cn, null);
            }
        }

        public  List<PatientTransactionPayment> GetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch, int FindPatient, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spPatientPaymentsGetByDay_New";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(PatientPaymentSearch.FromDate));
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(PatientPaymentSearch.ToDate));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPaymentSearch.StaffID));
            cmd.AddParameter("@FindPatient", SqlDbType.Int, FindPatient);

            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<PatientTransactionPayment> retVal = new List<PatientTransactionPayment>();
            while (reader.Read())
            {
                PatientTransactionPayment temp = getPatientTransactionPaymentObjectFromReader(reader);

                retVal.Add(temp);
            }
            reader.Close();

            return retVal;
        }

        public  List<ReportPaymentReceiptByStaff> GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate, bool bFilterByStaffID, Int64 nStaffID, long loggedStaffID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spReportPaymentReceiptByStaff";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@RepFromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
            cmd.AddParameter("@RepToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));

            if (bFilterByStaffID && nStaffID > 0)
            {
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(nStaffID));
            }

            cmd.AddParameter("@LoggedStaffID", SqlDbType.BigInt, loggedStaffID);

            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<ReportPaymentReceiptByStaff> retVal = new List<ReportPaymentReceiptByStaff>();
            while (reader.Read())
            {
                ReportPaymentReceiptByStaff temp = new ReportPaymentReceiptByStaff();

                temp.RepPaymentRecvID = (long)reader["RepPaymentRecvID"];
                if (reader.HasColumn("ReportDateTime") && reader["ReportDateTime"] != DBNull.Value)
                {
                    temp.ReportDateTime = (DateTime)reader["ReportDateTime"];
                }
                temp.StaffID = (long)reader["StaffID"];
                temp.ApprovedStaffID = (long)reader["ApprovedStaffID"];

                if (reader.HasColumn("RepFromDate") && reader["RepFromDate"] != DBNull.Value)
                {
                    temp.RepFromDate = (DateTime)reader["RepFromDate"];
                }
                if (reader.HasColumn("RepToDate") && reader["RepToDate"] != DBNull.Value)
                {
                    temp.RepToDate = (DateTime)reader["RepToDate"];
                }

                temp.RepTittle = reader["RepTittle"] as string;
                temp.RepNumCode = reader["RepNumCode"] as string;

                temp.ReceiptIssueMauSo = reader["ReceiptIssueMauSo"] as string;
                temp.ReceiptIssueKyHieu = reader["ReceiptIssueKyHieu"] as string;
                temp.ReceiptNumberFrom = reader["ReceiptNumberFrom"] as string;
                temp.ReceiptNumberTo = reader["ReceiptNumberTo"] as string;

                temp.staff = new Staff();
                if (reader["FullName"] != DBNull.Value)
                {
                    temp.staff.FullName = reader["FullName"] as string;
                }
                if (reader.HasColumn("IsDeleted") && reader["IsDeleted"] != DBNull.Value)
                {
                    temp.IsDeleted = (bool)reader["IsDeleted"];
                }

                retVal.Add(temp);
            }
            reader.Close();

            return retVal;
        }

        public  ReportPaymentReceiptByStaffDetails GetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spReportPaymentReceiptByStaffDetails";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@RepPaymentRecvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RepPaymentRecvID));


            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            ReportPaymentReceiptByStaffDetails RptPayReceiptByStaffDetails = new ReportPaymentReceiptByStaffDetails();
            RptPayReceiptByStaffDetails.allPatientPayment = new List<PatientTransactionPayment>();
            RptPayReceiptByStaffDetails.RepPaymentRecvID = RepPaymentRecvID;
            while (reader.Read())
            {
                PatientTransactionPayment temp = getPatientTransactionPaymentObjectFromReader(reader);
                RptPayReceiptByStaffDetails.RepPaymentRecvDetailID = (long)reader["RepPaymentRecvDetailID"];
                RptPayReceiptByStaffDetails.allPatientPayment.Add(temp);
            }
            reader.Close();

            return RptPayReceiptByStaffDetails;
        }

        public  List<ReportPaymentReceiptByStaff> GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate, bool bFilterByStaffID, Int64 nStaffID, long loggedStaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetReportPaymentReceiptByStaff(FromDate, ToDate, bFilterByStaffID, nStaffID, loggedStaffID, cn, null);
            }
        }

        public  ReportPaymentReceiptByStaffDetails GetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetReportPaymentReceiptByStaffDetails(RepPaymentRecvID, cn, null);
            }
        }


        public  IList<OutPatientCashAdvance> GetAllOutPatientCashAdvance(long PtRegistrationID, bool isGetLast = false)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetOutPatientCashAdvance", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    //▼====: #005
                    if (isGetLast)
                    {
                        cmd.AddParameter("@IsGetLast", SqlDbType.Bit, isGetLast);
                    }
                    //▲====: #005
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    IList<OutPatientCashAdvance> retVal = null;
                    retVal = GetOutPatientCashAdvanceFromReader(reader);
                    if (retVal != null && retVal.Count > 0 && reader.NextResult())
                    {
                        IList<OutPatientCashAdvanceLink> mLinks = GetOutPatientCashAdvanceLinkCollectionFromReader(reader);
                        if (mLinks != null && mLinks.Count > 0)
                        {
                            foreach (var item in retVal)
                            {
                                if (mLinks.Any(x => x.OutPtCashAdvanceID == item.OutPtCashAdvanceID))
                                {
                                    item.OutPatientCashAdvanceLinks = mLinks.Where(x => x.OutPtCashAdvanceID == item.OutPtCashAdvanceID).ToList();
                                }
                            }
                        }
                    }
                    reader.Close();
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  PatientTransaction GetAllPaymentByRegistrationID_InPt(long registrationID, DbConnection connection, DbTransaction tran)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                PatientTransaction ptTransaction = new PatientTransaction();

                SqlCommand cmd = (SqlCommand)connection.CreateCommand();
                cmd.CommandText = "spGetTransactionByRegistrationID";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationID);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)AllLookupValues.RegistrationType.NOI_TRU);

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        ptTransaction = GetPatientTransactionFromReader(reader);
                    }
                    reader.Close();
                }

                if (ptTransaction.TransactionID > 0)
                {
                    ptTransaction.PatientTransactionPayments = GetAllPayments_InPt(ptTransaction.TransactionID, connection, tran).ToObservableCollection();
                }

                return ptTransaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool AddTransactionForDrug(PatientTransactionPayment payment, long outiID, long V_TranRefType, out long PaymentID)//PatientPayment payment,
        {
            PaymentID = 0;
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("AddTransationForDrug", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@OutiID", SqlDbType.BigInt, outiID);
                cmd.AddParameter("@V_TranRefType", SqlDbType.BigInt, V_TranRefType);
                long? V_Currency = null;
                if (payment.Currency != null)
                {
                    V_Currency = payment.Currency.LookupID;
                }
                long? V_PaymentType = null;
                if (payment.PaymentType != null)
                {
                    V_PaymentType = payment.PaymentType.LookupID;
                }
                long? V_PaymentMode = null;
                if (payment.PaymentMode != null)
                {
                    V_PaymentMode = payment.PaymentMode.LookupID;
                }
                cmd.AddParameter("@V_Currency", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_Currency));
                cmd.AddParameter("@V_PaymentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PaymentType));
                cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PaymentMode));
                cmd.AddParameter("@PayAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.PayAmount));
                cmd.AddParameter("@CreditOrDebit", SqlDbType.SmallInt, ConvertNullObjectToDBNull(payment.CreditOrDebit));
                SqlParameter paremail = new SqlParameter("@PaymentID", SqlDbType.BigInt);
                paremail.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paremail);
                cn.Open();
                int count = 0;
                count = cmd.ExecuteNonQuery();
                if (count != 0)
                {
                    if (paremail.Value != DBNull.Value)
                    {
                        PaymentID = (long)paremail.Value;
                    }
                    results = true;
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public List<PatientCashAdvance> GetRefundPatientCashAdvance(long ptRegistrationID)
        {
            if (ptRegistrationID <= 0)
            {
                return null;
            }
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetRefundPatientCashAdvance", cn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptRegistrationID));
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<PatientCashAdvance> _Result = GetPatientCashAdvanceCollectionFromReader(reader);
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return _Result;
                }
            }
            catch (Exception _Ex)
            {
                throw new Exception(_Ex.Message);
            }
        }

        public  bool ThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail, long PtRegistrationID, long V_RegistrationType, out long PtTranPaymtID)//PatientPayment payment,
        {
            try
            {
                PtTranPaymtID = 0;
                bool results = false;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spThanhToanTienChoBenhNhan", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                    cmd.AddParameter("@OutwBloodInvoiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.OutwBloodInvoiceID));
                    cmd.AddParameter("@OutDMedRscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.OutDMedRscrID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.StaffID));
                    cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.PtRegDetailID));
                    cmd.AddParameter("@outiID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.outiID));
                    cmd.AddParameter("@TransactionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.TransactionID));
                    //cmd.AddParameter("@TransactionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(TrDetail.TransactionDate));
                    cmd.AddParameter("@Amount", SqlDbType.Money, ConvertNullToZero(TrDetail.Amount));
                    cmd.AddParameter("@PriceDifference", SqlDbType.Money, ConvertNullToZero(TrDetail.PriceDifference));
                    cmd.AddParameter("@AmountCoPay", SqlDbType.Money, ConvertNullToZero(TrDetail.AmountCoPay));
                    cmd.AddParameter("@HealthInsuranceRebate", SqlDbType.Money, ConvertNullToZero(TrDetail.HealthInsuranceRebate));
                    cmd.AddParameter("@Discount", SqlDbType.Money, ConvertNullToZero(TrDetail.Discount));
                    cmd.AddParameter("@Qty", SqlDbType.Float, ConvertNullToZero(TrDetail.Qty));
                    cmd.AddParameter("@RefDocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.RefDocID));
                    cmd.AddParameter("@ExchangeRate", SqlDbType.Float, ConvertNullObjectToDBNull(TrDetail.ExchangeRate));
                    cmd.AddParameter("@TransItemRemarks", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TrDetail.TransItemRemarks));
                    cmd.AddParameter("@PCLRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.PCLRequestID));
                    cmd.AddParameter("@TranRefID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.TranRefID));
                    cmd.AddParameter("@TranFinalizationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(0));
                    cmd.AddParameter("@V_TranRefType", SqlDbType.BigInt, ConvertNullObjectToDBNull(TrDetail.V_TranRefType));
                    SqlParameter pareTranItemID = new SqlParameter("@TransItemID", SqlDbType.BigInt);
                    pareTranItemID.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pareTranItemID);

                    cmd.AddParameter("@PaymentAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.PayAmount));
                    cmd.AddParameter("@V_Currency", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.Currency != null ? payment.Currency.LookupID : 0));
                    cmd.AddParameter("@V_PaymentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PaymentType != null ? payment.PaymentType.LookupID : 0));
                    cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PaymentMode != null ? payment.PaymentMode.LookupID : 0));
                    cmd.AddParameter("@CreditOrDebit", SqlDbType.TinyInt, ConvertNullObjectToDBNull(payment.CreditOrDebit));
                    cmd.AddParameter("@TranPaymtNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(payment.TranPaymtNote));
                    cmd.AddParameter("@PtPmtAccID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PtPmtAccID));
                    cmd.AddParameter("@ReceiptNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(payment.ReceiptNumber));
                    cmd.AddParameter("@PaymentDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(payment.PaymentDate));
                    cmd.AddParameter("@V_RefundPaymentReasonInPt", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.V_RefundPaymentReasonInPt != null ? payment.V_RefundPaymentReasonInPt.LookupID : 0));

                    SqlParameter parePtTranPaymtID = new SqlParameter("@PtTranPaymtID", SqlDbType.BigInt);
                    parePtTranPaymtID.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(parePtTranPaymtID);
                    SqlParameter pareNewPtCashAdvanceID = new SqlParameter("@NewPtCashAdvanceID", SqlDbType.BigInt);
                    pareNewPtCashAdvanceID.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pareNewPtCashAdvanceID);
                    cn.Open();
                    int count = 0;
                    count = cmd.ExecuteNonQuery();
                    if (count != 0 && parePtTranPaymtID.Value != DBNull.Value)
                    {
                        PtTranPaymtID = (long)parePtTranPaymtID.Value;
                        results = true;
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool RptPatientCashAdvReminder_Insert(RptPatientCashAdvReminder payment, out long RptPtCashAdvRemID)
        {
            try
            {
                RptPtCashAdvRemID = 0;
                bool results = false;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRptPatientCashAdvReminder_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PtRegistrationID));
                    cmd.AddParameter("@RemCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(payment.RemCode));
                    cmd.AddParameter("@RemDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(payment.RemDate));
                    cmd.AddParameter("@V_CashAdvanceType", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.V_CashAdvanceType));
                    cmd.AddParameter("@RemAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.RemAmount));
                    cmd.AddParameter("@RemNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(payment.RemNote));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.DepartmentSuggest != null ? payment.DepartmentSuggest.DeptID : 0));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.StaffID));
                    cmd.AddParameter("@V_PaymentReason", SqlDbType.BigInt, payment.V_PaymentReason != null ? payment.V_PaymentReason.LookupID : 0);
                    cmd.AddParameter("@V_RefundPaymentReasonInPt", SqlDbType.BigInt, payment.V_RefundPaymentReasonInPt != null ? payment.V_RefundPaymentReasonInPt.LookupID : 0);

                    SqlParameter paremail = new SqlParameter("@RptPtCashAdvRemID", SqlDbType.BigInt);
                    paremail.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paremail);
                    cn.Open();
                    int count = 0;
                    count = cmd.ExecuteNonQuery();
                    if (count > 0 && paremail.Value != DBNull.Value)
                    {
                        RptPtCashAdvRemID = (long)paremail.Value;
                        results = true;
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool RptPatientCashAdvReminder_Update(RptPatientCashAdvReminder payment)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRptPatientCashAdvReminder_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@RptPtCashAdvRemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.RptPtCashAdvRemID));
                    cmd.AddParameter("@RemDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(payment.RemDate));
                    cmd.AddParameter("@V_CashAdvanceType", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.V_CashAdvanceType));
                    cmd.AddParameter("@RemNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(payment.RemNote));
                    cmd.AddParameter("@RemAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.RemAmount));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.DepartmentSuggest != null ? payment.DepartmentSuggest.DeptID : 0));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.StaffID));
                    cmd.AddParameter("@V_PaymentReason", SqlDbType.BigInt, payment.V_PaymentReason != null ? payment.V_PaymentReason.LookupID : 0);
                    cmd.AddParameter("@V_RefundPaymentReasonInPt", SqlDbType.BigInt, payment.V_RefundPaymentReasonInPt != null ? payment.V_RefundPaymentReasonInPt.LookupID : 0);

                    cn.Open();
                    bool bRes = cmd.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(cn, cmd);

                    return bRes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool RptPatientCashAdvReminder_Delete(long RptPtCashAdvRemID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRptPatientCashAdvReminder_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@RptPtCashAdvRemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RptPtCashAdvRemID));

                    cn.Open();
                    bool bRes = cmd.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(cn, cmd);

                    return bRes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  List<RptPatientCashAdvReminder> RptPatientCashAdvReminder_GetAll(long PtRegistrationID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRptPatientCashAdvReminder_GetAllRegistrationID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<RptPatientCashAdvReminder> retVal = null;
                    retVal = GetRptPatientCashAdvReminderCollectionFromReader(reader);
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_DeptID(long DeptID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRefDepartmentReqCashAdv_GetByDeptID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<RefDepartmentReqCashAdv> results = null;
                    results = GetRefDepartmentReqCashAdvCollectionFromReader(reader);
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return results;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Noi Tru - Lay thong tin phuong thuc thanh toan dau tien
        /// </summary>
        /// <param name="ptRegistrationID"></param>
        /// <returns></returns>
        public long GetFirstPaymentMode(long ptRegistrationID)
        {
            if (0 == ptRegistrationID)
            {
                return 0;
            }
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                            @"SELECT TOP 1 V_PaymentMode
                            FROM PatientCashAdvance
                            WHERE PtRegistrationID=@PtRegistrationID
                            ORDER BY PtCashAdvanceID ASC",
                        cn);
                    cmd.AddParameter("@PtRegistrationID",
                        SqlDbType.BigInt, ConvertNullObjectToDBNull(ptRegistrationID));
                    SqlDataAdapter _Adapter = new SqlDataAdapter(cmd);
                    DataTable _Dt = new DataTable();
                    _Adapter.Fill(_Dt);
                    if (null != _Dt
                        && null != _Dt.Rows
                        && _Dt.Rows.Count > 0)
                    {
                        return long.Parse(_Dt.Rows[0][0].ToString());
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Noi Tru - Tao ma phieu thu tien tam ung
        /// </summary>
        /// <returns></returns>
        public string GetCashAdvReceiptNum()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT [dbo].[funcCreateInvDocNumber](5, 0)", cn);
                    SqlDataAdapter _Adapter = new SqlDataAdapter(cmd);
                    DataTable _Dt = new DataTable();
                    _Adapter.Fill(_Dt);
                    if (null != _Dt
                        && null != _Dt.Rows
                        && _Dt.Rows.Count > 0)
                    {
                        return _Dt.Rows[0][0].ToString();
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool PatientCashAdvance_Insert(PatientCashAdvance payment, out long PtCashAdvanceID)
        {
            try
            {
                PtCashAdvanceID = 0;
                bool results = false;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientCashAdvance_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PtRegistrationID));
                    cmd.AddParameter("@CashAdvReceiptNum", SqlDbType.VarChar, ConvertNullObjectToDBNull(payment.CashAdvReceiptNum));
                    cmd.AddParameter("@PaymentDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(payment.PaymentDate));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.StaffID));
                    cmd.AddParameter("@V_CashAdvanceType", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.V_CashAdvanceType));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.V_RegistrationType));
                    cmd.AddParameter("@PaymentAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.PaymentAmount));
                    cmd.AddParameter("@GeneralNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(payment.GeneralNote));
                    cmd.AddParameter("@RptPtCashAdvRemID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(payment.RptPtCashAdvRemID));
                    cmd.AddParameter("@V_PaymentReason", SqlDbType.BigInt, payment.V_PaymentReason != null ? payment.V_PaymentReason.LookupID : 0);
                    cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, payment.V_PaymentMode != null ? payment.V_PaymentMode.LookupID : 0);
					cmd.AddParameter("@BankingTrxId", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.BankingTrxId)); // VuTTM
					SqlParameter paremail = new SqlParameter("@PtCashAdvanceID", SqlDbType.BigInt);
                    paremail.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paremail);
                    cn.Open();
                    int count = 0;
                    count = cmd.ExecuteNonQuery();
                    if (count > 0 && paremail.Value != DBNull.Value)
                    {
                        PtCashAdvanceID = (long)paremail.Value;
                        results = true;
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //▼==== #004
        public bool ConfirmPatientPostponementAdvance(InPatientAdmDisDetails AdmissionInfo, long StaffID)
        {
            try
            {
                int _Count = 0;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spConfirmPatientPostponementAdvance", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AdmissionInfo.InPatientAdmDisDetailID));
                    cmd.AddParameter("@PostponementStaff", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cn.Open();
                    _Count = cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                }
                return _Count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲==== #004

        // VuTTM Begin
        public bool PatientCashAdvance_Refund(long PtCashAdvanceID, long bankingRefundTrxId)
		{
			try
			{
				int _Count = 0;
				using (SqlConnection cn = new SqlConnection(this.ConnectionString))
				{
					SqlCommand cmd = new SqlCommand("spPatientCashAdvance_Refund", cn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParameter("@PtCashAdvanceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtCashAdvanceID));
					cmd.AddParameter("@BankingRefundTrxId", SqlDbType.BigInt, ConvertNullObjectToDBNull(bankingRefundTrxId));
					cn.Open();
					_Count = cmd.ExecuteNonQuery();
					CleanUpConnectionAndCommand(cn, cmd);
				}
				return _Count > 0;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		// VuTTM End


        public  bool PatientCashAdvance_Delete(PatientCashAdvance payment, long staffID)
        {
            try
            {
                bool results = false;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientCashAdvance_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtCashAdvanceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PtCashAdvanceID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(staffID));
                    cn.Open();
                    int count = 0;
                    count = cmd.ExecuteNonQuery();

                    if (count > 0)
                    {
                        results = true;
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GenericPayment_FullOperation
        public  bool GenericPayment_FullOperation(GenericPayment GenPayment, out long OutGenericPaymentID)
        {
            try
            {
                OutGenericPaymentID = 0;
                bool results = false;
                if (GenPayment.V_Status == "Insert")
                {
                    GenPayment.GenericPaymentCode = new ServiceSequenceNumberProvider().GetReceiptNumber_NgoaiTru(0);
                }
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGenericPayment_InsertOrUpdate", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@GenericPaymentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GenPayment.GenericPaymentID));
                    cmd.AddParameter("@GenericPaymentCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(GenPayment.GenericPaymentCode));
                    cmd.AddParameter("@PaymentDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(GenPayment.PaymentDate));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GenPayment.StaffID));
                    cmd.AddParameter("@V_GenericPaymentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(GenPayment.V_GenericPaymentType));
                    cmd.AddParameter("@V_GenericPaymentReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GenPayment.V_GenericPaymentReason));
                    cmd.AddParameter("@LookupReasonID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GenPayment.LookupReasonID));
                    cmd.AddParameter("@PaymentAmount", SqlDbType.Money, ConvertNullObjectToDBNull(GenPayment.PaymentAmount));
                    cmd.AddParameter("@GeneralNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GenPayment.GeneralNote));
                    cmd.AddParameter("@PersonName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GenPayment.PersonName));
                    cmd.AddParameter("@PersonAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GenPayment.PersonAddress));
                    cmd.AddParameter("@PhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GenPayment.PhoneNumber));
                    cmd.AddParameter("@V_Status", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GenPayment.V_Status));
                    cmd.AddParameter("@DOB", SqlDbType.VarChar, ConvertNullObjectToDBNull(GenPayment.DOB));
                    cmd.AddParameter("@OrgName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GenPayment.OrgName));
                    //==== #001
                    cmd.AddParameter("@VATAmount", SqlDbType.Money, ConvertNullObjectToDBNull(GenPayment.VATAmount));
                    cmd.AddParameter("@VATPercent", SqlDbType.Float, ConvertNullObjectToDBNull(GenPayment.VATPercent));
                    //==== #001
                    SqlParameter parOutGenPayID = new SqlParameter("@OutGenericPaymentID", SqlDbType.BigInt);
                    parOutGenPayID.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(parOutGenPayID);
                    cn.Open();
                    int count = 0;
                    count = cmd.ExecuteNonQuery();
                    if (count != 0 && parOutGenPayID.Value != DBNull.Value)
                    {
                        OutGenericPaymentID = (long)parOutGenPayID.Value;
                        results = true;
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  GenericPayment GetGenericPaymentByID(long GenericPaymentID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGenericPayment_GetByID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@GenericPaymentID", SqlDbType.BigInt, GenericPaymentID);
                    cn.Open();
                    GenericPayment GenPayment = null;
                    IDataReader reader = ExecuteReader(cmd);

                    if (reader.Read())
                    {
                        GenPayment = GetGenericPaymentFromReader(reader);
                    }
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return GenPayment;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public  List<GenericPayment> GenericPayment_GetAll(DateTime? FromDate, DateTime? ToDate, long? V_GenericPaymentType, long? StaffID)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    mConnection.Open();
                    SqlCommand mCommand = (SqlCommand)mConnection.CreateCommand();
                    mCommand.CommandText = "spGenericPayment_GetAllByCreatedDate";
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                    mCommand.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                    mCommand.AddParameter("@V_GenericPaymentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_GenericPaymentType));
                    mCommand.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    IDataReader reader = ExecuteReader(mCommand);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<GenericPayment> retVal = null;
                    retVal = GetGenericPaymentCollectionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  GenericPayment GenericPayment_SearchByCode(string GenPaymtCode, long StaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGenericPayment_SearchByCode", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@GenericPaymentCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GenPaymtCode));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cn.Open();
                    GenericPayment GenPayment = null;
                    IDataReader reader = ExecuteReader(cmd);

                    if (reader.Read())
                    {
                        GenPayment = GetGenericPaymentFromReader(reader);
                    }
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return GenPayment;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  List<PatientCashAdvance> PatientCashAdvance_GetAll(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    return PatientCashAdvance_GetAll(PtRegistrationID, V_RegistrationType, cn, null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  List<PatientCashAdvance> PatientCashAdvance_GetAll(long PtRegistrationID, long V_RegistrationType, DbConnection connection, DbTransaction tran)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SqlCommand cmd = (SqlCommand)connection.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandText = "spPatientCashAdvance_GetAllByRegistration";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
                IDataReader reader = ExecuteReader(cmd);
                if (reader == null)
                {
                    return null;
                }
                List<PatientCashAdvance> retVal = null;
                retVal = GetPatientCashAdvanceCollectionFromReader(reader);
                reader.Close();
                return retVal;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public long GetLastBankingTrxIdByPtReg(long ptRegistrationId)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetLastBankingTrxIdByPtReg", cn)
                    {
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ptRegistrationId);
                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return 0;
                    }
                    long result = 0;
                    while (reader.Read())
                    {
                        if (reader.HasColumn("BankingTrxId")
                            && reader["BankingTrxId"] != DBNull.Value)
                        {
                            result = long.Parse(reader["BankingTrxId"].ToString());
                        }
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  List<PatientCashAdvance> GetCashAdvanceBill(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetCashAdvanceBill", cn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);

                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<PatientCashAdvance> retVal = null;
                    retVal = GetPatientCashAdvanceCollectionFromReader(reader);
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public  List<TransactionFinalization> GetPatientSettlement(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetPatientSettlement", cn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);

                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<TransactionFinalization> retVal = null;
                    retVal = GetTransactionFinalizationCollectionFromReader(reader);
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public  bool PatientTransactionPayment_UpdateNote(List<PatientTransactionPayment> allPayment)//List<PatientPayment> allPayment,
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientTransactionPayment_UpdateNote", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, ConvertListPatientPaymentToXml(allPayment));
                    cn.Open();
                    bool bRes = cmd.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return bRes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool PatientTransactionPayment_UpdateID(PatientTransactionPayment Payment)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientTransactionPayment_UpdateID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtTranPaymtID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Payment.PtTranPaymtID));
                cmd.AddParameter("@TranPaymtNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Payment.TranPaymtNote));
                cmd.AddParameter("@ManualReceiptNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Payment.ManualReceiptNumber));
                cn.Open();
                bool bRes = cmd.ExecuteNonQuery() > 0;

                CleanUpConnectionAndCommand(cn, cmd);
                return bRes;
            }
        }


        public  List<PatientTransactionPayment> PatientTransactionPayment_Load(long TransactionID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientTransactionPayment_Load", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, TransactionID);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader == null)
                {
                    return null;
                }
                List<PatientTransactionPayment> retVal = null;
                retVal = GetPatientTransactionPaymentCollectionFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public  bool AddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff,
            List<PatientTransactionPayment> allPayment, out long RepPaymentRecvID)//List<PatientPayment> allPayment,
        {
            RepPaymentRecvID = 0;
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spReportPaymentReceiptByStaffAdd", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.StaffID));
                cmd.AddParameter("@ApprovedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.ApprovedStaffID));


                cmd.AddParameter("@RepFromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.RepFromDate));
                cmd.AddParameter("@RepToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.RepToDate));

                cmd.AddParameter("@RepTittle", SqlDbType.NVarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.RepTittle));
                cmd.AddParameter("@RepNumCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.RepNumCode));
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.IsDeleted));

                cmd.AddParameter("@ReceiptIssueMauSo", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.ReceiptIssueMauSo));
                cmd.AddParameter("@ReceiptIssueKyHieu", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.ReceiptIssueKyHieu));
                cmd.AddParameter("@ReceiptNumberFrom", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.ReceiptNumberFrom));
                cmd.AddParameter("@ReceiptNumberTo", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.ReceiptNumberTo));

                cmd.AddParameter("@DataXML", SqlDbType.Xml, ConvertListPatientPaymentToXml(allPayment));

                SqlParameter paremail = new SqlParameter("@RepPaymentRecvID", SqlDbType.BigInt);
                paremail.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paremail);


                cn.Open();
                int count = 0;
                count = cmd.ExecuteNonQuery();
                if (count != 0)
                {
                    if (paremail.Value != DBNull.Value)
                    {
                        RepPaymentRecvID = (long)paremail.Value;
                    }
                    results = true;
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        private string ConvertListPatientPaymentToXml(IList<PatientTransactionPayment> lstPatientPayment)//IList<PatientPayment> lstPatientPayment
        {
            if (lstPatientPayment != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PM>");
                foreach (PatientTransactionPayment item in lstPatientPayment)
                {
                    sb.Append("<PatientPayment>");
                    sb.AppendFormat("<PtPmtID>{0}</PtPmtID>", item.PtTranPaymtID);
                    sb.AppendFormat("<TranPaymtNote>{0}</TranPaymtNote>", item.TranPaymtNote);
                    sb.AppendFormat("<ManualReceiptNumber>{0}</ManualReceiptNumber>", item.ManualReceiptNumber);
                    sb.AppendFormat("<OutPtCashAdvanceID>{0}</OutPtCashAdvanceID>", item.OutPtCashAdvanceID);
                    sb.Append("</PatientPayment>");
                }
                sb.Append("</PM>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public  bool UpdateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spReportPaymentReceiptByStaffUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.StaffID));
                cmd.AddParameter("@RepPaymentRecvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.RepPaymentRecvID));

                cmd.AddParameter("@RepTittle", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.RepTittle));
                cmd.AddParameter("@RepNumCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.RepNumCode));

                cmd.AddParameter("@ReceiptIssueMauSo", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.ReceiptIssueMauSo));
                cmd.AddParameter("@ReceiptIssueKyHieu", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.ReceiptIssueKyHieu));
                cmd.AddParameter("@ReceiptNumberFrom", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.ReceiptNumberFrom));
                cmd.AddParameter("@ReceiptNumberTo", SqlDbType.VarChar, ConvertNullObjectToDBNull(curReportPaymentReceiptByStaff.ReceiptNumberTo));

                cn.Open();
                int count = 0;
                count = cmd.ExecuteNonQuery();
                if (count != 0)
                {
                    results = true;
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public  List<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt(SearchOutwardReport Searchcriate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPaymentsGetByDay_NyCreate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Searchcriate.fromdate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Searchcriate.todate));

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Searchcriate.StaffID));
                cmd.AddParameter("@V_TradingPlaces", SqlDbType.BigInt, ConvertNullObjectToDBNull(Searchcriate.V_TradingPlaces));
                cmd.AddParameter("@IsReport", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Searchcriate.IsReport));
                cmd.AddParameter("@IsDeleted", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Searchcriate.IsDeleted));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader == null)
                {
                    return null;
                }
                List<ReportOutPatientCashReceipt_Payments> retVal = null;
                retVal = GetReportOutPatientCashReceipt_PaymentsCollectionFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public  List<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt_TongHop(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, out List<PatientTransactionPayment> OutPaymentLst)
        {
            try
            {
                OutPaymentLst = null;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientPaymentsGetByDay_TongHop_V2", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;

                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Searchcriate.fromdate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Searchcriate.todate));

                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Searchcriate.StaffID));
                    cmd.AddParameter("@V_TradingPlaces", SqlDbType.BigInt, ConvertNullObjectToDBNull(Searchcriate.V_TradingPlaces));
                    cmd.AddParameter("@IsReport", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Searchcriate.IsReport));
                    cmd.AddParameter("@IsDeleted", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Searchcriate.IsDeleted));
                    cmd.AddParameter("@IsTongHop", SqlDbType.Bit, ConvertNullObjectToDBNull(IsTongHop));
                    cmd.AddParameter("@LoggedStaffID", SqlDbType.BigInt, loggedStaffID);
                    cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, ConvertNullObjectToDBNull(Searchcriate.V_PaymentMode)); //--▼--02/02/2021 DatTB 
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<ReportOutPatientCashReceipt_Payments> retVal = null;
                    retVal = GetReportOutPatientCashReceipt_PaymentsCollectionFromReader_New(reader, out OutPaymentLst);
                    //if (reader.NextResult())
                    //{
                    //    OutPaymentLst = GetPatientTransactionPaymentCollectionFromReader(reader);
                    //}
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*▼====: #003*/
        Dictionary<int, PatientCashReceiptAsync> PatientCashReceiptMap = new Dictionary<int, PatientCashReceiptAsync>();
        int MaxFieldLength = 8000;
        public  List<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt_TongHop_Async(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey)
        {
            try
            {
                OutPaymentLst = null;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientPaymentsGetByDay_TongHop", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;

                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Searchcriate.fromdate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Searchcriate.todate));

                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Searchcriate.StaffID));
                    cmd.AddParameter("@V_TradingPlaces", SqlDbType.BigInt, ConvertNullObjectToDBNull(Searchcriate.V_TradingPlaces));
                    cmd.AddParameter("@IsReport", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Searchcriate.IsReport));
                    cmd.AddParameter("@IsDeleted", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Searchcriate.IsDeleted));
                    cmd.AddParameter("@IsTongHop", SqlDbType.Bit, ConvertNullObjectToDBNull(IsTongHop));
                    cmd.AddParameter("@LoggedStaffID", SqlDbType.BigInt, loggedStaffID);
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    AsyncKey = 0;
                    if (reader == null)
                    {
                        return null;
                    }
                    List<ReportOutPatientCashReceipt_Payments> retVal = null;
                    retVal = GetReportOutPatientCashReceipt_PaymentsCollectionFromReader(reader);
                    if (reader.NextResult())
                    {
                        OutPaymentLst = GetPatientTransactionPaymentCollectionFromReader(reader);
                    }
                    if (retVal.Count > MaxFieldLength)
                    {
                        AsyncKey = Guid.NewGuid().GetHashCode();
                        PatientCashReceiptMap.Add(AsyncKey, new PatientCashReceiptAsync { PatientCashReceipts = retVal.Skip(MaxFieldLength).ToList(), TransactionPayments = OutPaymentLst });
                        retVal = retVal.Take(MaxFieldLength).ToList();
                        OutPaymentLst = null;
                    }
                    
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  List<ReportOutPatientCashReceipt_Payments> GetMoreReportOutPatientCashReceipt_TongHop_Async(int RefAsyncKey, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey)
        {
            OutPaymentLst = null;
            var mCashReceiptAsync = PatientCashReceiptMap[RefAsyncKey];
            AsyncKey = RefAsyncKey;
            List<ReportOutPatientCashReceipt_Payments> mResult = new List<ReportOutPatientCashReceipt_Payments>();
            if (mCashReceiptAsync.PatientCashReceipts.Count > 0)
            {
                mResult = mCashReceiptAsync.PatientCashReceipts.Take(MaxFieldLength).ToList();
                mCashReceiptAsync.PatientCashReceipts = mCashReceiptAsync.PatientCashReceipts.Skip(MaxFieldLength).ToList();
            }
            else if (mCashReceiptAsync.TransactionPayments.Count > 0)
            {
                OutPaymentLst = mCashReceiptAsync.TransactionPayments.Take(MaxFieldLength).ToList();
                mCashReceiptAsync.TransactionPayments = mCashReceiptAsync.TransactionPayments.Skip(MaxFieldLength).ToList();
            }
            else
            {
                PatientCashReceiptMap.Remove(RefAsyncKey);
                AsyncKey = 0;
            }
            return mResult;
        }
        /*▲====: #003*/

        public  List<CharityOrganization> GetAllCharityOrganization()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetAllCharityOrganization", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<CharityOrganization> AllCharityOrganization = null;
                    AllCharityOrganization = GetCharityOrganizationCollectionFromReader(reader);
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return AllCharityOrganization;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //public  List<CharitySupportFund> GetCharitySupportFundForInPt(long PtRegistrationID, long? BillingInvID)
        public  List<CharitySupportFund> GetCharitySupportFundForInPt(long PtRegistrationID, long? BillingInvID, bool? IsHighTechServiceBill = null)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetAllCharitySupportFund_ByPtRegistrationID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@BillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BillingInvID));
                    /*▼====: #002*/
                    cmd.AddParameter("@IsHighTechServiceBill", SqlDbType.Bit, ConvertNullObjectToDBNull(IsHighTechServiceBill));
                    /*▲====: #002*/
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<CharitySupportFund> AllCharitySupportFund = null;
                    AllCharitySupportFund = GetCharitySupportFundCollectionFromReader(reader);
                    reader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return AllCharitySupportFund;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string ConvertSupportFundListToXml(IEnumerable<CharitySupportFund> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<SupportFunds>");
                foreach (CharitySupportFund details in items)
                {
                    sb.Append("<FundItem>");
                    sb.AppendFormat("<CharityFundID>{0}</CharityFundID>", details.CharityFundID);
                    sb.AppendFormat("<CharityOrgID>{0}</CharityOrgID>", details.CharityOrgInfo.CharityOrgID);
                    sb.AppendFormat("<IsUsedPercent>{0}</IsUsedPercent>", details.IsUsedPercent);
                    sb.AppendFormat("<PercentValue>{0}</PercentValue>", details.PercentValue);
                    sb.AppendFormat("<AmountValue>{0}</AmountValue>", details.AmountValue);
                    sb.AppendFormat("<RecordState>{0}</RecordState>", (int)details.RecordState);
                    sb.AppendFormat("<V_CharityObjectType>{0}</V_CharityObjectType>", (long)details.V_CharityObjectType);
                    sb.Append("</FundItem>");
                }
                sb.Append("</SupportFunds>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        //public  List<CharitySupportFund> SaveCharitySupportFundForInPt(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds)
        public  List<CharitySupportFund> SaveCharitySupportFundForInPt(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds, bool IsHighTechServiceBill = false)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSupportFundForInPt_InsertOrUpdate", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@SupportFundInfoXML", SqlDbType.Xml, ConvertSupportFundListToXml(SupportFunds));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@BillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BillingInvID));
                    /*▼====: #002*/
                    cmd.AddParameter("@IsHighTechServiceBill", SqlDbType.Bit, ConvertNullObjectToDBNull(IsHighTechServiceBill));
                    /*▲====: #002*/
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return null;
                    }
                    List<CharitySupportFund> AllSupportFund = null;
                    AllSupportFund = GetCharitySupportFundCollectionFromReader(reader);
                    reader.Close();
                    
                    CleanUpConnectionAndCommand(cn, cmd);
                    return AllSupportFund;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool AddCharityOrganization(string CharityOrgName)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddCharityOrganization", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@CharityOrgName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CharityOrgName));
                    cn.Open();
                    bool RelVal = cmd.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return RelVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool EditCharityOrganization(long CharityOrgID, string CharityOrgName)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spEditCharityOrganization", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@CharityOrgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CharityOrgID));
                    cmd.AddParameter("@CharityOrgName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CharityOrgName));
                    cn.Open();
                    bool RelVal = cmd.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return RelVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteCharityOrganization(long CharityOrgID, long StaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteCharityOrganization", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@CharityOrgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CharityOrgID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cn.Open();
                    bool RelVal = cmd.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return RelVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool Recal15PercentHIBenefit(long PtRegistrationID, long StaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRecal15PercentHIBenefit", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cn.Open();
                    bool bRes = cmd.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return bRes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool RecalRegistrationHIBenefit(long PtRegistrationID, long StaffID, out string OutputBalanceServicesXML)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    //▼===== #003
                    //SqlCommand cmd = new SqlCommand("spRecalRegistrationHIBenefit", cn);
                    SqlCommand cmd = new SqlCommand("spConfirmRegistrationHIBenefit_V3", cn);
                    //▲===== #003
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@BalanceServicesXML", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cn.Open();
                    bool retval = cmd.ExecuteNonQuery() > 0;
                    OutputBalanceServicesXML = null;
                    if (cmd.Parameters["@BalanceServicesXML"].Value != null && cmd.Parameters["@BalanceServicesXML"].Value != DBNull.Value)
                    {
                        OutputBalanceServicesXML = cmd.Parameters["@BalanceServicesXML"].Value.ToString();
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                    return retval;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool RecalRegistrationHIBenefit_New(long PtRegistrationID, long StaffID, out string OutputBalanceServicesXML)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    //SqlCommand cmd = new SqlCommand("spRecalRegistrationHIBenefit_New", cn);
                    SqlCommand cmd = new SqlCommand("spConfirmRegistrationHIBenefit_V3", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@BalanceServicesXML", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cn.Open();
                    bool retval = cmd.ExecuteNonQuery() > 0;
                    OutputBalanceServicesXML = null;
                    if (cmd.Parameters["@BalanceServicesXML"].Value != null && cmd.Parameters["@BalanceServicesXML"].Value != DBNull.Value)
                    {
                        OutputBalanceServicesXML = cmd.Parameters["@BalanceServicesXML"].Value.ToString();
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                    return retval;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  bool CancelConfirmHIBenefit(long PtRegistrationID, long StaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCancelConfirmHIBenefit", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cn.Open();
                    bool retval = cmd.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return retval;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  bool ConfirmRegistrationHIBenefit(long PtRegistrationID, long? StaffID, bool IsUpdateHisID, long? HisID, double PtInsuranceBenefit)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spConfirmRegistrationHIBenefit", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@IsUpdateHisID", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateHisID));
                    cmd.AddParameter("@HisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(HisID));
                    cmd.AddParameter("@PtInsuranceBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(PtInsuranceBenefit));
                    cn.Open();
                    bool bRes = cmd.ExecuteNonQuery() > 0;

                    CleanUpConnectionAndCommand(cn, cmd);
                    return bRes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool AddOutPtTransactionFinalization(OutPtTransactionFinalization TransactionFinalizationObj, bool IsUpdateToken, byte ViewCase, out long TransactionFinalizationSummaryInfoID, out long OutTranFinalizationID, long OutPtGeneralFinalizationID = 0)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    if (TransactionFinalizationObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        cmd = new SqlCommand("spAddOutPtTransactionFinalization", cn);
                        cmd.AddParameter("@OutTranFinalizationID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                        cmd.AddParameter("@ViewCase", SqlDbType.TinyInt, ViewCase);
                        cmd.AddParameter("@GenericPaymentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TransactionFinalizationObj.GenericPaymentID));
                        cmd.AddParameter("@outiID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TransactionFinalizationObj.outiID));
                    }
                    else
                    {
                        cmd = new SqlCommand("spUpdateTransactionFinalization", cn);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    if(OutPtGeneralFinalizationID > 0)
                    {
                        cmd.AddParameter("@OutPtGeneralFinalizationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OutPtGeneralFinalizationID));
                    }
                    cmd.AddParameter("@TranFinalizationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TransactionFinalizationObj.TranFinalizationID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TransactionFinalizationObj.PtRegistrationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TransactionFinalizationObj.StaffID));
                    cmd.AddParameter("@TaxMemberName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TransactionFinalizationObj.TaxMemberName));
                    cmd.AddParameter("@TaxMemberAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TransactionFinalizationObj.TaxMemberAddress));
                    cmd.AddParameter("@TaxCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(TransactionFinalizationObj.TaxCode));
                    cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, ConvertNullObjectToDBNull(TransactionFinalizationObj.V_PaymentMode));
                    cmd.AddParameter("@BankAccountNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(TransactionFinalizationObj.BankAccountNumber));
                    cmd.AddParameter("@Denominator", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TransactionFinalizationObj.Denominator));
                    cmd.AddParameter("@Symbol", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TransactionFinalizationObj.Symbol));
                    cmd.AddParameter("@InvoiceNumb", SqlDbType.VarChar, ConvertNullObjectToDBNull(TransactionFinalizationObj.InvoiceNumb));
                    cmd.AddParameter("@StaffIDModify", SqlDbType.BigInt, ConvertNullObjectToDBNull(TransactionFinalizationObj.StaffID));
                    cmd.AddParameter("@DateInvoice", SqlDbType.DateTime, ConvertNullObjectToDBNull(TransactionFinalizationObj.DateInvoice));
                    if (TransactionFinalizationObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU && TransactionFinalizationObj.PtRegistrationIDCollection != null && TransactionFinalizationObj.PtRegistrationIDCollection.Count > 0)
                    {
                        cmd.AddParameter("@PtRegistrationIDCollection", SqlDbType.VarChar, ConvertNullObjectToDBNull(string.Join(",", TransactionFinalizationObj.PtRegistrationIDCollection)));
                    }
                    if (TransactionFinalizationObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        cmd.AddParameter("@TransactionFinalizationSummaryInfoID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    }
                    cmd.AddParameter("@eInvoiceToken", SqlDbType.VarChar, ConvertNullObjectToDBNull(TransactionFinalizationObj.eInvoiceToken));
                    cmd.AddParameter("@IsUpdateToken", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateToken));
                    // 20201019 TNHX: Thêm Buyer khi quyết toán 1 ca
                    cmd.AddParameter("@Buyer", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TransactionFinalizationObj.Buyer));
                    cn.Open();
                    bool RelVal = cmd.ExecuteNonQuery() > 0;
                    TransactionFinalizationSummaryInfoID = 0;
                    OutTranFinalizationID = 0;
                    if (cmd.Parameters.Cast<SqlParameter>().Any(x => x.ParameterName == "@TransactionFinalizationSummaryInfoID" && x.Direction == ParameterDirection.Output && x.Value != null && x.Value != DBNull.Value))
                    {
                        TransactionFinalizationSummaryInfoID = (long)cmd.Parameters.Cast<SqlParameter>().First(x => x.ParameterName == "@TransactionFinalizationSummaryInfoID" && x.Direction == ParameterDirection.Output).Value;
                    }
                    if (cmd.Parameters.Cast<SqlParameter>().Any(x => x.ParameterName == "@OutTranFinalizationID" && x.Direction == ParameterDirection.Output && x.Value != null && x.Value != DBNull.Value))
                    {
                        OutTranFinalizationID = (long)cmd.Parameters.Cast<SqlParameter>().First(x => x.ParameterName == "@OutTranFinalizationID" && x.Direction == ParameterDirection.Output).Value;
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                    return RelVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  OutPtTransactionFinalization RptOutPtTransactionFinalization(long aPtRegistrationID, long V_RegistrationType, long TranFinalizationID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRptOutPtTransactionFinalization", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aPtRegistrationID));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                    cmd.AddParameter("@TranFinalizationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TranFinalizationID));
                    cn.Open();
                    var mReader = cmd.ExecuteReader();
                    var retval = GetOutPtTransactionFinalizationCollectionFromReader(mReader);
                    mReader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    if (retval == null)
                    {
                        return null;
                    }
                    return retval.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<PatientTransactionPayment> GetAllPayments_InPt(long transactionID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllPayments_InPt";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<PatientTransactionPayment> retVal = new List<PatientTransactionPayment>();
            while (reader.Read())
            {
                PatientTransactionPayment temp = base.GetPatientTransactionPaymentFromReader(reader);

                if (reader["PaymentModeLookupID"] != DBNull.Value)
                {
                    //Get them thong tin tu bang lookup.
                    Lookup paymentModeInfo = new Lookup();

                    paymentModeInfo.LookupID = (long)reader["PaymentModeLookupID"];
                    paymentModeInfo.ObjectTypeID = (long)reader["PaymentModeObjectTypeID"];
                    paymentModeInfo.ObjectName = reader["PaymentModeObjectName"] as string;
                    paymentModeInfo.ObjectValue = reader["PaymentModeObjectValue"] as string;
                    paymentModeInfo.ObjectNotes = reader["PaymentModeObjectNotes"] as string;

                    temp.PaymentMode = paymentModeInfo;
                }

                if (reader["CurrencyLookupID"] != DBNull.Value)
                {
                    //Get them thong tin tu bang lookup.
                    Lookup currencyInfo = new Lookup();

                    currencyInfo.LookupID = (long)reader["CurrencyLookupID"];
                    currencyInfo.ObjectTypeID = (long)reader["CurrencyObjectTypeID"];
                    currencyInfo.ObjectName = reader["CurrencyObjectName"] as string;
                    currencyInfo.ObjectValue = reader["CurrencyObjectValue"] as string;
                    currencyInfo.ObjectNotes = reader["CurrencyObjectNotes"] as string;

                    temp.Currency = currencyInfo;
                }

                if (reader["PaymentTypeLookupID"] != DBNull.Value)
                {
                    //Get them thong tin tu bang lookup.
                    Lookup paymentTypeInfo = new Lookup();

                    paymentTypeInfo.LookupID = (long)reader["PaymentTypeLookupID"];
                    paymentTypeInfo.ObjectTypeID = (long)reader["PaymentTypeObjectTypeID"];
                    paymentTypeInfo.ObjectName = reader["PaymentTypeObjectName"] as string;
                    paymentTypeInfo.ObjectValue = reader["PaymentTypeObjectValue"] as string;
                    paymentTypeInfo.ObjectNotes = reader["PaymentTypeObjectNotes"] as string;

                    temp.PaymentType = paymentTypeInfo;
                }

                retVal.Add(temp);
            }
            reader.Close();

            return retVal;
        }

        public List<PatientTransactionPayment> GetAllPayments_New(long transactionID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllPayments_New";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<PatientTransactionPayment> retVal = new List<PatientTransactionPayment>();
            while (reader.Read())
            {
                PatientTransactionPayment temp = base.GetPatientTransactionPaymentFromReader(reader);
                retVal.Add(temp);
            }

            reader.Close();

            return retVal;
        }
        #region Tạm ứng ngoại trú
        public bool PatientAccountTransaction_Insert(PatientAccountTransaction payment, out long PtAccountTranID)
        {
            try
            {
                PtAccountTranID = 0;
                bool results = false;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientAccountTransaction_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientAccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PatientAccountID));
                    cmd.AddParameter("@TranReceiptNum", SqlDbType.VarChar, ConvertNullObjectToDBNull(payment.TranReceiptNum));
                    cmd.AddParameter("@TransactionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(payment.TransactionDate));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.Staff.StaffID));
                    cmd.AddParameter("@CreditAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.CreditAmount));
                    cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(payment.Note));
                    cmd.AddParameter("@V_PtAccountTranType", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.V_PtAccountTranType));
                    cmd.AddParameter("@V_PaymentReason", SqlDbType.BigInt, payment.V_PaymentReason != null ? payment.V_PaymentReason.LookupID : 0);
                    cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, payment.V_PaymentMode != null ? payment.V_PaymentMode.LookupID : 0);
                    SqlParameter paremail = new SqlParameter("@PtAccountTranID", SqlDbType.BigInt);
                    paremail.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paremail);
                    cn.Open();
                    int count = 0;
                    count = cmd.ExecuteNonQuery();
                    if (count > 0 && paremail.Value != DBNull.Value)
                    {
                        PtAccountTranID = (long)paremail.Value;
                        results = true;
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<PatientAccountTransaction> PatientAccountTransaction_GetAll(long PatientID)
        {
            try
            {
                List<PatientAccountTransaction> results = new List<PatientAccountTransaction>();
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientAccountTransaction_GetAll", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.VarChar, ConvertNullObjectToDBNull(PatientID));
                    cn.Open();
                    var reader = cmd.ExecuteReader();
                    results = GetPatientAccountTransactionCollection(reader);
                    CleanUpConnectionAndCommand(cn, cmd);
                    cn.Close();
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<PatientAccount> PatientAccount_GetAll(long PatientID)
        {
            try
            {
                List<PatientAccount> results = new List<PatientAccount>();
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientAccount_GetAll", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.VarChar, ConvertNullObjectToDBNull(PatientID));
                    cn.Open();
                    var reader = cmd.ExecuteReader();
                    results = GetPatientAccountCollection(reader);
                    CleanUpConnectionAndCommand(cn, cmd);
                    cn.Close();
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool PatientAccount_Insert(long PatientID, string AccountNumber)
        {
            try
            {
                bool results = false;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientAccount_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    cmd.AddParameter("@AccountNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(AccountNumber));
                    cn.Open();
                    int count = 0;
                    count = cmd.ExecuteNonQuery();
                    if (count > 0)
                    {
                        results = true;
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int OutPatientGeneralFinalizations_Insert(long PatientID, long StaffID, out long OutPtGeneralFinalizationID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    OutPtGeneralFinalizationID = 0;
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand("spOutPatientGeneralFinalizations_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@OutPtGeneralFinalizationID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cn.Open();
                    int count = 0;
                    count = cmd.ExecuteNonQuery();
                    if (count != 0)
                    {
                        if (cmd.Parameters["@OutPtGeneralFinalizationID"].Value != null && cmd.Parameters["@OutPtGeneralFinalizationID"].Value != DBNull.Value)
                        {
                            OutPtGeneralFinalizationID = (long)cmd.Parameters["@OutPtGeneralFinalizationID"].Value;
                        }
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                    return count;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }

    public class PatientCashReceiptAsync
    {
        public List<ReportOutPatientCashReceipt_Payments> PatientCashReceipts { get; set; }
        public List<PatientTransactionPayment> TransactionPayments { get; set; }
    }

}

//public List<PatientTransactionPayment> GetAllPayments_New(long transactionID, DbConnection connection, DbTransaction tran)
//{
//    if (connection.State != ConnectionState.Open)
//    {
//        connection.Open();
//    }
//    SqlCommand cmd = (SqlCommand)connection.CreateCommand();
//    cmd.Transaction = (SqlTransaction)tran;
//    cmd.CommandText = "spGetAllPayments_New";
//    cmd.CommandType = CommandType.StoredProcedure;

//    cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

//    IDataReader reader = ExecuteReader(cmd);
//    if (reader == null)
//    {
//        return null;
//    }
//    List<PatientTransactionPayment> retVal = new List<PatientTransactionPayment>();
//    while (reader.Read())
//    {
//        PatientTransactionPayment temp = base.GetPatientTransactionPaymentFromReader(reader);
//        retVal.Add(temp);
//    }

//    reader.Close();

//    return retVal;
//}

