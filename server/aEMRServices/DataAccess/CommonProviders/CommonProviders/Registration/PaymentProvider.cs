/*
 * 20170609 #001 CMN: Fix SupportFund About TT04 with TT04
 * 20170619 #002 CMN: Service for payment report OutPt with large data
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using eHCMS.Configurations;
using System.Data.Common;

namespace eHCMS.DAL
{
    public abstract class PaymentProvider : DataProviderBase
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
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Common.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.Common.Payment.ProviderType);
                    _instance = (PaymentProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }

        public PaymentProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }


        public abstract List<PatientTransactionPayment> GetAllPayments_New(long transactionID);

        public abstract List<PatientTransactionPayment> GetAllPayments_New(long transactionID, DbConnection connection, DbTransaction tran);

        public abstract IList<OutPatientCashAdvance> GetAllOutPatientCashAdvance(long PtRegistrationID);

        public abstract List<PatientTransactionPayment> GetAllPayments_InPt(long transactionID);

        public abstract List<PatientTransactionPayment> GetAllPayments_InPt(long transactionID, DbConnection connection, DbTransaction tran);


        public abstract PatientTransaction GetAllPaymentByRegistrationID_InPt(long registrationID);

        public abstract PatientTransaction GetAllPaymentByRegistrationID_InPt(long registrationID, DbConnection connection, DbTransaction tran);


        public abstract List<PatientTransactionPayment> GetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch,int FindPatient);

        public abstract List<PatientTransactionPayment> GetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch,int FindPatient,
                                                                    DbConnection connection, DbTransaction tran);

        public abstract List<ReportPaymentReceiptByStaff> GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate,
                                                                                 bool bFilterByStaffID, Int64 nStaffID,
                                                                                 long loggedStaffID,
                                                                                 DbConnection connection,
                                                                                 DbTransaction tran);

        public abstract ReportPaymentReceiptByStaffDetails GetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID,
                                                                                        DbConnection connection,
                                                                                        DbTransaction tran);

        public abstract List<ReportPaymentReceiptByStaff> GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate, bool bFilterByStaffID, Int64 nStaffID, long loggedStaffID);

        public abstract ReportPaymentReceiptByStaffDetails GetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID);

        public abstract bool AddTransactionForDrug(PatientTransactionPayment payment, long outiID, long V_TranRefType, out long PaymentID);//PatientPayment payment,

        public abstract bool ThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail, long PtRegistrationID, long V_RegistrationType, out long PtTranPaymtID);

        public abstract List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_DeptID(long DeptID);

        public abstract bool PatientCashAdvance_Insert(PatientCashAdvance payment, out long PtCashAdvanceID);

        public abstract bool PatientCashAdvance_Delete(PatientCashAdvance payment, long staffID);

        //HPT: Hàm lưu phiếu thu khác (Phòng tài vụ)
        public abstract bool GenericPayment_FullOperation(GenericPayment GenPayment, out long OutGenericPaymentID);

        public abstract GenericPayment GetGenericPaymentByID(long GenericPaymentID);

        public abstract List<GenericPayment> GenericPayment_GetAll(DateTime? FromDate, DateTime? ToDate, long? V_GenericPaymentType, long? StaffID);

        public abstract GenericPayment GenericPayment_SearchByCode(string GenPaymtCode, long StaffID);
        
        public abstract bool RptPatientCashAdvReminder_Insert(RptPatientCashAdvReminder payment, out long RptPtCashAdvRemID);

        public abstract bool RptPatientCashAdvReminder_Update(RptPatientCashAdvReminder payment);

        public abstract bool RptPatientCashAdvReminder_Delete( long RptPtCashAdvRemID);

        public abstract List<PatientCashAdvance> PatientCashAdvance_GetAll(long PtRegistrationID,long V_RegistrationType);

        public abstract List<PatientCashAdvance> PatientCashAdvance_GetAll(long PtRegistrationID,long V_RegistrationType, DbConnection connection, DbTransaction tran);

        public abstract List<PatientCashAdvance> GetCashAdvanceBill(long PtRegistrationID, long V_RegistrationType);

        public abstract List<TransactionFinalization> GetPatientSettlement(long PtRegistrationID, long V_RegistrationType);

        public abstract List<RptPatientCashAdvReminder> RptPatientCashAdvReminder_GetAll(long PtRegistrationID);

        public abstract bool PatientTransactionPayment_UpdateNote(List<PatientTransactionPayment> allPayment);

        public abstract bool PatientTransactionPayment_UpdateID(PatientTransactionPayment Payment);

        public abstract List<PatientTransactionPayment> PatientTransactionPayment_Load(long TransactionID);

        public abstract bool AddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff,
                                                    List<PatientTransactionPayment> allPayment, out long RepPaymentRecvID);// List<PatientPayment> allPayment,



        public virtual PatientTransactionPayment getPatientTransactionPaymentObjectFromReader(IDataReader reader)
        {
            PatientTransactionPayment p = base.GetPatientTransactionPaymentFromReader(reader);
          
            return p;
        }

        public abstract bool UpdateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff);

        public abstract List<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt(SearchOutwardReport Searchcriate);

        public abstract List<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt_TongHop(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, out List<PatientTransactionPayment> OutPaymentLst);

        /*▼====: #002*/
        public abstract List<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt_TongHop_Async(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey);
        public abstract List<ReportOutPatientCashReceipt_Payments> GetMoreReportOutPatientCashReceipt_TongHop_Async(int RefAsyncKey, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey);
        /*▲====: #002*/

        public abstract List<CharityOrganization> GetAllCharityOrganization();

        /*▼====: #001*/
        //public abstract List<CharitySupportFund> GetCharitySupportFundForInPt(long PtregistrationID, long? BillingInvID);
        public abstract List<CharitySupportFund> GetCharitySupportFundForInPt(long PtregistrationID, long? BillingInvID, bool? IsHighTechServiceBill = null);

        //public abstract List<CharitySupportFund> SaveCharitySupportFundForInPt(long PtregistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds);
        public abstract List<CharitySupportFund> SaveCharitySupportFundForInPt(long PtregistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds, bool IsHighTechServiceBill = false);
        /*▲====: #001*/

        public abstract bool Recal15PercentHIBenefit(long PtRegistrationID, long StaffID);
        public abstract bool RecalRegistrationHIBenefit(long PtRegistrationID, long StaffID, out string OutputBalanceServicesXML);
        public abstract bool ConfirmRegistrationHIBenefit(long PtRegistrationID, long? StaffID, bool IsUpdateHisID, long? HisID, double PtInsuranceBenefit);
        public abstract bool AddOutPtTransactionFinalization(OutPtTransactionFinalization TransactionFinalizationObj, out long TransactionFinalizationSummaryInfoID);
        public abstract OutPtTransactionFinalization RptOutPtTransactionFinalization(long aPtRegistrationID, long V_RegistrationType, long TranFinalizationID);
        public abstract bool CancelConfirmHIBenefit(long PtRegistrationID, long StaffID);
    }
}