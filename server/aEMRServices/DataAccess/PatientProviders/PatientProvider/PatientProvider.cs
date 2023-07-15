/*
 * 20170218 #001 CMN: Add Checkbox AllDept for InPtBills
 * 20170517 #002 CMN: Thêm giá trị để cập nhật tên bệnh nhân không giới hạn với tài khoản quản trị
 * 20170522 #003 CMN: Added variable to check InPt 5 year HI without paid enough\
 * 20180523 #004 TBLD: Added parameter CreatedDate
 * 20180814 #005 TTM: Tạo mới method để thực hiện thêm mới và cập nhật bệnh nhân, thẻ BHYT, giấy CV 1 lần.
 * 20181119 #006 TTM: BM 0005257: Tạo hàm lấy dữ liệu bệnh nhân đang nằm tại khoa.
 * 20181211 #007 TTM: BM 0004207: Thêm hàm lấy danh sách định dạng thẻ BHYT động.
 * 20181212 #008 TTM: Tạo hàm cập nhật Chẩn đoán ban đầu (BasicDiagTreatment)
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using DataEntities;
using eHCMS.Configurations;

namespace eHCMS.DAL
{
    public abstract class PatientProvider : DataProviderBase //DataAccess
    {
        static private PatientProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public PatientProvider Instance
        {
            get
            {
                lock (typeof(PatientProvider))
                {
                    if (_instance == null)
                    {
                        string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                        if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                            tempPath = AppDomain.CurrentDomain.BaseDirectory;
                        else
                            tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                        string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Patients.Assembly + ".dll");
                        Assembly assem = Assembly.LoadFrom(assemblyPath);
                        Type t = assem.GetType(Globals.Settings.Patients.ProviderType);
                        _instance = (PatientProvider)Activator.CreateInstance(t);
                    }
                }
                return _instance;
            }
        }

        public PatientProvider()
        {
            this.ConnectionString = Globals.Settings.Patients.ConnectionString;

        }
        public abstract Patient GetPatientByID(long patientID, bool ToRegisOutPt = false);
        
        // TxD 20/12/2014: Added the following Method for InPt usage because the existing above is a really messy
        public abstract Patient GetPatientByID_InPt(long patientID, bool bGetOutPtRegInfo = false, bool ToRegisOutPt = false);
        
        public abstract List<Patient> GetPatientAll();
        public abstract Patient GetPatientByID_Full(long patientID, bool ToRegisOutPt = false);
        public abstract bool DeletePatientByID(long patientID);
        /*==== #002 ====*/
        //public abstract bool UpdatePatient(Patient patient);
        public abstract bool UpdatePatient(Patient patient, bool IsAdmin = false);
        /*==== #002 ====*/
        public abstract bool UpdatePatientBloodTypeID(long PatientID, int? BloodTypeID);
        public abstract List<BloodType> GetAllBloodTypes();
        public abstract bool AddPatient(Patient newPatient, out long PatientID, out string PatientCode, out string PatientBarCode);
        public abstract List<Patient> GetAllPatients();
        public abstract List<Patient> SearchPatients(PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<Patient> GetPatients(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<PatientClassification> GetAllClassifications();
        public abstract List<PatientClassHistory> GetAllClassificationHistories(long patientID);
        public abstract List<HealthInsurance> GetAllHealthInsurances(long patientID, bool IncludeDeletedItems = false);
        public abstract HealthInsuranceHIPatientBenefit GetActiveHIBenefit(long HIID);
        public abstract PatientClassification GetLatestClassification(long patientID);
        public abstract void GetLatestClassificationAndActiveHICard(long patientID, out PatientClassification classification, out HealthInsurance activeHI);
        public abstract HealthInsurance GetLatestHealthInsurance(long patientID);
        public abstract List<RegistrationType> GetAllRegistrationTypes();
        public abstract bool RegisterPatient(PatientRegistration info, out long PatientRegistrationID, out int SequenceNo);
        public abstract bool RegisterPatient(PatientRegistration info,DbConnection conn,DbTransaction tran, out long PatientRegistrationID, out int SequenceNo);

        public abstract bool CreateSuggestCashAdvance(long InPatientBillingInvID, long StaffID, out long RptPtCashAdvRemID);

        public abstract bool AddInPatientBillingInvoice(InPatientBillingInvoice inv, DbConnection conn, DbTransaction tran, out long ID);
        public abstract List<InPatientBillingInvoice> GetAllInPatientBillingInvoices(long registrationID,long? DeptID);
        public abstract PromoDiscountProgram GetPromoDiscountProgramByRegID(long PtRegistrationID);

        public abstract PromoDiscountProgram GetPromoDiscountProgramByPromoDiscProgID(long PromoDiscProgID);
        //==== #001
        public abstract List<InPatientBillingInvoice> GetAllInPatientBillingInvoicesByDeptArray(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray);
        //==== #001
        public abstract List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_FromListDeptID(long registrationID, List<long> ListDeptIDs);
        public abstract List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_ForCreateForm02(long RptForm02_InPtID, long PtRegistrationID, List<long> ListDeptIDs);
        public abstract bool GetBalanceInPatientBillingInvoice(long StaffID,long PtRegistrationID, long V_RegistrationType,string BillingInvoices, DbConnection connection, DbTransaction tran);
        public abstract List<OutwardDrugClinicDeptInvoice> GetInPatientInvoicesHasMedProducts(long registrationID, List<long> MedProductIDs,long? DeptID);
        public abstract List<OutwardDrugClinicDeptInvoice> GetInPatientInvoicesHasMedProducts(long registrationID, List<long> MedProductIDs,long? DeptID, DbConnection conn, DbTransaction tran);

        public abstract bool AddPaperReferal(PaperReferal referal, out long PaperReferalID);

        public abstract bool UpdatePaperReferal(PaperReferal referal);
        public abstract bool DeletePaperReferal(PaperReferal referal);
        public abstract bool UpdatePaperReferalRegID(PaperReferal referal);
        public abstract Hospital SearchHospitalByHICode(string HiCode);
        public abstract List<Hospital> LoadCrossRegionHospitals();
        public abstract List<Hospital> SearchHospitals(string HospitalName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<Hospital> SearchHospitalsNew(HospitalSearchCriteria entity, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<RegistrationSummaryInfo> SearchRegistrationSummaryList(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount, out RegistrationsTotalSummary totalSummaryInfo);
        public abstract List<PaperReferal> GetAllPaperReferalsByHealthInsurance(long hiid, bool IncludeDeletedItems = false);
        public abstract bool SaveChangesOnRegistration(PatientRegistration info);
        public abstract bool SaveChangesOnRegistration(PatientRegistration info, DbConnection conn, DbTransaction tran);
        public abstract bool SaveChangesOnTransaction(PatientTransaction info);
        public abstract bool SaveChangesOnTransaction(PatientTransaction info, DbConnection connection, DbTransaction tran);
      
        public abstract bool ProcessPayment_New(string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID);
        public abstract bool ProcessPayment_New(string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID, DbConnection connection, DbTransaction tran);

        public abstract bool CreatePaymentAndPayAdvance(long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID,out long PtCashAdvanceID);
        public abstract bool CreatePaymentAndPayAdvance(long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID, out long PtCashAdvanceID, DbConnection connection, DbTransaction tran);

        public abstract bool FinializeBillingInv_CreateTranPaymt_ConsolidateTranDetailsAndAdvPaymt(bool bCreateNewTran, PatientTransaction newTranInfo, List<InPatientBillingInvoice> colBillingInvoices,
                    DateTime paidTime, List<PatientTransactionDetail> balancedTranDetails, long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long NewTransactionIDRet);
        public abstract bool FinializeBillingInv_CreateTranPaymt_ConsolidateTranDetailsAndAdvPaymt(bool bCreateNewTran, PatientTransaction newTranInfo, List<InPatientBillingInvoice> colBillingInvoices,
                    DateTime paidTime, List<PatientTransactionDetail> balancedTranDetails, long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long NewTransactionIDRet, DbConnection connection, DbTransaction tran);

        public abstract bool InPatientPayForBill(PatientRegistration registration, IList<InPatientBillingInvoice> billingInvoices, decimal payAmount, long staffID);

        public abstract bool InPatientSettlement(long ptRegistrationID, long staffID);

        public abstract bool CheckIfTransactionExists(long registrationID, out long TransactionID);
        public abstract bool OpenTransaction(PatientTransaction info, out long TransactionID, DbConnection connection, DbTransaction tran);
        public abstract bool OpenTransaction_InPt(PatientTransaction info, out long TransactionID, DbConnection connection, DbTransaction tran);

        public abstract bool SaveChangesOnOutwardDrugClinicInvoice(OutwardDrugClinicDeptInvoice info, out List<long> InwardDrugIDList_Error);
        public abstract bool SaveChangesOnOutwardDrugClinicInvoice(OutwardDrugClinicDeptInvoice info, DbConnection conn, DbTransaction tran, out List<long> InwardDrugIDList_Error);
        public abstract bool AddOutwardDrugClinicInvoice(OutwardDrugClinicDeptInvoice info, List<InwardDrugClinicDept> updatedInwardItems, DbConnection connection, DbTransaction tran, out List<long> InwardDrugIDList_Error);

        public abstract List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices(long registrationID);
        public abstract List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices(long registrationID, DbConnection conn, DbTransaction tran);

        public abstract List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoicesByBillingInvoiceID(long billingInvoiceID, DbConnection conn, DbTransaction tran);
        public abstract List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices_NoBill(long registrationId, long? DeptID, long StoreID, long StaffID, bool IsAdmin, DbConnection conn, DbTransaction tran, DateTime? FromDate = null, DateTime? ToDate = null);

        public abstract void UpdatePaidTime(List<PatientRegistrationDetail> paidRegDetailsList,
                                        List<PatientPCLRequest> paidPclRequestList,
                                        List<OutwardDrugInvoice> paidDrugInvoice,
                                        List<InPatientBillingInvoice> billingInvoiceList, DbConnection conn, DbTransaction tran);
        public abstract void UpdatePaidTimeForBillingInvoice(List<InPatientBillingInvoice> billingInvoiceList, DateTime paidTime, DbConnection conn, DbTransaction tran);
        public abstract bool UpdateRegistrationStatus(PatientRegistration registration, DbConnection conn, DbTransaction tran);

        public abstract void RemovePaidRegItems(List<PatientRegistrationDetail> paidRegDetailsList,
                                List<PatientPCLRequest> paidPclRequestList,
                                List<OutwardDrugInvoice> paidDrugInvoice,
                                List<OutwardDrugClinicDeptInvoice> paidMedItemList,
                                List<OutwardDrugClinicDeptInvoice> paidChemicalItemList, DbConnection conn, DbTransaction tran);
        public abstract OutwardDrugInvoice GetDrugInvoiceByID(long outiID, DbConnection conn, DbTransaction tran);
        /// <summary>
        /// Luu lai tat ca cac the bao hiem cua benh nhan (hoac them moi, hoac xoa)
        /// </summary>
        /// <param name="patientID"></param>
        /// <param name="allHiItems"></param>
        /// <returns></returns>
        public abstract bool SaveHIItems(long patientID, List<HealthInsurance> allHiItems, out long? activeHICardID, long StaffID=0);
        public abstract bool AddHospitalByHiCode(string hospitalName,string hiCode);
        public abstract bool AddHospital(Hospital entity);
        public abstract bool UpdateHospital(Hospital entity);
        public abstract bool AddHiItem(HealthInsurance hiItem, out long HIID, long StaffID = 0);
        
        public abstract bool UpdateHiItem(out string Result, HealthInsurance hiItem, bool IsEditAfterRegistration, long StaffID = 0 );
        //public abstract ObservableCollection<HICardType> GetHICardTypeList();



        public abstract List<Location> GetAllLocations(long? departmentID);
        public abstract List<DeptLocation> GetLocationsByServiceID(long medServiceID);
        public abstract List<DeptLocation> GetAllDeptLocForServicesByInOutType(List<long> RefMedicalServiceInOutOthersTypes);
        public abstract List<RefMedicalServiceType> GetAllMedicalServiceTypes();
        public abstract List<RefMedicalServiceType> GetMedicalServiceTypesByInOutType(List<long> RefMedicalServiceInOutOthersTypes);
        public abstract HealthInsurance GetActiveHICard(long patientID);

        public abstract List<RefMedicalServiceItem> GetMedicalServiceItems(long? departmentID, long? serviceTypeID);
        public abstract List<RefMedicalServiceItem> GetAllMedicalServiceItemsByType(long? serviceTypeID);
        public abstract void AddHICard(HealthInsurance hi, out long HIID);
        public abstract void ActivateHICard(long hiCardID);

        // TxD 28/01/2018: Commented OUT the following because It has NEVER been USED 
        //public abstract bool ConfirmHIBenefit(long staffID, long patientID, long hiid, float? benefit);

        public abstract bool UpdateHICard(HealthInsurance hi);
        public abstract bool OpenTransaction(PatientTransaction tran, out long transactionID);
        public abstract bool OpenTransaction_InPt(PatientTransaction tran, out long transactionID);
        //public abstract bool OpenTransactionAndProcessPayment(PatientTransaction info, PatientPayment payment, out long TransactionID, out long PaymentID);
        public abstract PatientTransaction CreateTransaction(PatientRegistration registration, PatientType patientType, out bool HIServiceUsed);
        public abstract List<PatientRegistration> GetAllRegistrations(long patientID, bool IsInPtRegistration);
        
        public abstract List<PatientRegistrationDetail> GetAllRegistrationDetails_ForGoToKhamBenh(long registrationID);
        public abstract List<PatientRegistrationDetail> GetAllRegistrationDetails(long registrationID);
        public abstract List<PatientRegistrationDetail> GetAllRegistrationDetails(long registrationID,int FindPatient, DbConnection conn,DbTransaction tran);
        public abstract List<PatientRegistrationDetail> GetAllRegistrationDetailsByBillingInvoiceID(long billingInvoiceID, DbConnection conn, DbTransaction tran);
        public abstract List<PatientRegistrationDetail> GetAllInPatientRegistrationDetails_NoBill(long registrationId,long? DeptID, DbConnection conn, DbTransaction tran, DateTime? FromDate = null, DateTime? ToDate = null);

        public abstract InPatientBillingInvoice GetInPatientBillingInvoice(long billingInvoiceID, DbConnection conn, DbTransaction tran);

        public abstract List<PatientRegistrationDetail> GetAllRegistrationDetails_ByV_ExamRegStatus(Int64 PtRegistrationID, Int64 V_ExamRegStatus);
        public abstract List<PatientRegistrationDetail> GetAllRegistrationDetails_ByV_ExamRegStatus(Int64 PtRegistrationID, Int64 V_ExamRegStatus, DbConnection conn, DbTransaction tran);

        public abstract PatientRegistration GetRegistration(long registrationID,int FindPatient);
        public abstract PatientRegistration GetRegistration(long registrationID, int FindPatient, DbConnection conn, DbTransaction tran);
        public abstract PatientRegistration GetRegistrationSimple(long registrationID, int PatientFindBy);
        //HPT: Lấy thông tin bệnh nhân và chẩn đoán để khởi tạo form giấy chuyển tuyến
        public abstract TransferForm CreateBlankTransferFormByRegID(long PtRegistrationID, int PatientFindBy, long V_TransferFormType);
        
        public abstract PatientRegistration GetRegistraionVIPByPatientID(long PatientID);
        public abstract List<PatientTransactionDetail> GetAlltransactionDetails(long transactionID);
        public abstract List<PatientTransactionDetail> GetAlltransactionDetails_InPt(long transactionID);
        public abstract List<PatientTransactionDetail> GetAlltransactionDetailsSum(long transactionID);
        public abstract bool SaveOutwardDrugInvoice(OutwardDrugInvoice invoice, out long outiID);
        public abstract List<PatientTransactionDetail> GetAlltransactionDetailsSum_InPt(long transactionID);
        public abstract List<OutwardDrugInvoice> GetDrugInvoiceListByRegistrationID(long registrationID);
        public abstract List<OutwardDrugInvoice> GetDrugInvoiceListByRegistrationID(long registrationID, DbConnection conn, DbTransaction tran);
        
        public abstract bool SaveOutwardDrugInvoice(OutwardDrugInvoice invoice, out long outiID, DbConnection conn, DbTransaction tran);
        public abstract bool SaveChangesOnOutwardDrugInvoice(OutwardDrugInvoice invoice);
        public abstract bool SaveChangesOnOutwardDrugInvoice(OutwardDrugInvoice invoice, DbConnection connection, DbTransaction tran);
        public abstract PatientTransaction GetTransactionByID(long TransactionID);
        public abstract PatientTransaction GetTransactionByRegistrationID(long RegistrationID);
        public abstract List<PatientPCLRequest> GetPCLRequestListByRegistrationID(long RegistrationID);
        public abstract List<PatientPCLRequest> GetPCLRequestListByRegistrationID(long RegistrationID, DbConnection connection, DbTransaction tran);
        public abstract List<PatientPCLRequest> GetPCLRequestListByRegistrationID_InPt(long InPtRegistrationID);

        public abstract List<PatientPCLRequest> GetPCLRequestListByBillingInvoiceID(long billingInvoiceID, DbConnection connection, DbTransaction tran);
        public abstract List<PatientPCLRequest> spGetInPatientPCLRequest_NoBill(long registrationId,long? DeptID, DbConnection connection, DbTransaction tran, DateTime? FromDate = null, DateTime? ToDate = null);
        public abstract List<PCLItem> GetPCLItemsByMedServiceID(long MedicalServiceID, DbConnection connection, DbTransaction tran);
        public abstract List<PCLItem> GetPCLItemsByMedServiceID(long MedicalServiceID);

        public abstract List<PCLExamType> GetPclExamTypesByMedServiceID(long MedicalServiceID, DbConnection connection, DbTransaction tran);
        public abstract List<PCLExamType> GetPclExamTypesByMedServiceID(long MedicalServiceID);

        public abstract PCLExamType GetPclExamTypeByID(long ExamTypeID);

        public abstract List<PatientPCLRequest> GetPCLRequestListByIDList(List<long> PclIDList, long V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU);

        public abstract List<PatientPCLRequest> GetPCLRequestListByIDList(List<long> PclIDList, DbConnection connection,
                                                                          DbTransaction tranlong, long V_RegistrationType);

        public abstract AxServerConfigSection GetApplicationConfigValues();
        /// <summary>
        /// Cập nhật các thuộc tính của 1 danh sách các chi tiết đăng ký của một đăng ký.
        /// Các thuộc tính được update bao gồm:
        ///                                V_ExamRegStatus
        ///                                MarkedAsDeleted
        ///                                PaidTime
        ///                                RefundTime
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="regDetailList"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public abstract bool UpdateRegistrationDetailsStatus(long registrationID, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran);
        public abstract bool UpdatePclRequestStatus(long registrationID, List<PatientPCLRequest> regPclList, DbConnection conn, DbTransaction tran);
        public abstract bool UpdatePclRequestInfo(PatientPCLRequest p);
        public abstract long ActiveHisID(HealthInsurance aHealthInsurance);
        public abstract bool AddRegistration(PatientRegistration info, DbConnection conn, DbTransaction tran, out long PatientRegistrationID, out int SequenceNo);

        public abstract bool AddRegistrationDetails(long registrationID,int FindPatient, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran, out List<long> newItemIDList);
        public abstract bool UpdateRegistrationDetails(IList<PatientRegistrationDetail> regDetailList,int FindPatient, DbConnection conn, DbTransaction tran);

        public abstract bool UpdateBillingInvoice(InPatientBillingInvoice billingInv);

        public abstract bool DeleteRegistrationDetailList(List<PatientRegistrationDetail> registrationDetailList, DbConnection connection, DbTransaction tran);
        public abstract bool AddPCLRequestWithDetails(long registrationID, long V_RegistrationType, long ptMedicalRecordID, PatientPCLRequest request , DbConnection conn, DbTransaction tran, out long PCLRequestID);
        public abstract bool AddPCLRequestDetails(long pclRequestID, List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran);
        public abstract bool AddPCLRequestDetails(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran);
        public abstract bool UpdatePCLRequestDetailList(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran);
        public abstract bool DeletePCLRequestDetailList(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran);
        public abstract void DeletePCLRequestWithDetails(Int64 PatientPCLReqID, out string Result);
        public abstract void DeleteInPtPCLRequestWithDetails(Int64 PatientPCLReqID);
        //public abstract void DeletePCLRequestWithDetails(Int64 PatientPCLReqID, DbConnection conn, DbTransaction tran, out string Result);
        public abstract bool DeleteOutwardDrugClinicDeptList(List<OutwardDrugClinicDept> outwardDrugClinicDeptList, DbConnection conn, DbTransaction tran);
        public abstract bool DeleteOutwardDrugClinicDeptInvoices(List<OutwardDrugClinicDeptInvoice> outwardDrugClinicDeptClinicDeptInvoices, DbConnection conn, DbTransaction tran);
        public abstract bool AddOutwardDrugClinicDeptIntoBill(long PtRegistrationID, long InPatientBillingInvID, List<OutwardDrugClinicDeptInvoice> outwardDrugClinicDeptClinicDeptInvoices, DbConnection conn, DbTransaction tran);

        public abstract bool CalculateBillInvoice(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, DbConnection conn, DbTransaction tran);
        /*==== #003 ====*/
        //public abstract bool CalAdditionalFeeAndTotalBill(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, DbConnection conn, DbTransaction tran);
        /*▼====: #004*/
        public abstract bool CalAdditionalFeeAndTotalBill(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, DbConnection conn, DbTransaction tran, DateTime CreatedDate, double? HIBenefit = 1, bool IsHICard_FiveYearsCont_NoPaid = false);
        /*▲====: #004*/
        /*==== #003 ====*/

        //---can lam sang ngoai vien
        public abstract bool AddPCLRequestExtWithDetails(PatientPCLRequest_Ext request, out long PatientPCLReqExtID);
        public abstract bool PCLRequestExtUpdate(PatientPCLRequest_Ext request);
        public abstract bool AddNewPCLRequestDetailsExt(PatientPCLRequest_Ext request);
        public abstract bool DeletePCLRequestDetailExtList(List<PatientPCLRequestDetail_Ext> requestDetailList);
        public abstract bool DeletePCLRequestExt(long PatientPCLReqExtID);
        public abstract List<PatientPCLRequest_Ext> GetPCLRequestListExtByRegistrationID(long RegistrationID);
        public abstract PatientPCLRequest_Ext GetPCLRequestExtPK(long PatientPCLReqExtID);

        public abstract PatientPCLRequest_Ext PatientPCLRequestExtByID(long PatientPCLReqExtID);

        public abstract List<PatientPCLRequest_Ext> PatientPCLRequestExtPaging(PatientPCLRequestExtSearchCriteria SearchCriteria,
                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        public abstract List<PatientPCLRequestDetail_Ext> GetPCLRequestDetailListExtByRegistrationID(long RegistrationID);

        /// <summary>
        /// Cập nhật lại số lượng thuốc.
        /// </summary>
        /// <param name="outwardDrugClinicDeptList"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public abstract bool UpdateOutwardDrugClinicDeptList(IList<OutwardDrugClinicDept> outwardDrugClinicDeptList,long? StaffID, DbConnection conn, DbTransaction tran);
        public abstract List<PatientRegistrationDetail> GetAllRegistrationDetailsByIDList(List<long> regDetailIdList);
        public abstract List<PatientRegistrationDetail> GetAllRegistrationDetailsByIDList(List<long> regDetailIdList, DbConnection conn, DbTransaction tran);

        /// <summary>
        /// Lấy tổng số công nợ của những bill của đăng ký nội trú chưa được QUYẾT TOÁN
        /// </summary>
        /// <param name="registrationID">Mã đăng ký</param>
        /// <param name="conn">Connection tới database</param>
        /// <param name="tran">DB transaction</param>
        /// <param name="TotalLiabilities">Tổng tiền công nợ</param>
        /// <param name="SumOfAdvance">Tổng tiền Bệnh nhân ứng trước</param>
        /// <param name="TotalPatientPayment_PaidInvoice">Tổng tiền Bệnh nhân cần trả cho những bill chưa được QUYẾT TOÁN và đã trả tiền rồi</param>
        /// <returns></returns>
        public abstract bool GetInPatientRegistrationNonFinalizedLiabilities(long registrationID, bool GetSumOfCashAdvBalanceOnly, DbConnection conn, DbTransaction tran, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                                            , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized);
        /// <summary>
        /// Lấy tổng số công nợ của những bill của đăng ký nội trú chưa được QUYẾT TOÁN
        /// </summary>
        /// <param name="registrationID">Mã đăng ký</param>
        /// <param name="TotalLiabilities">Tổng tiền công nợ</param>
        /// <param name="SumOfAdvance">Tổng tiền Bệnh nhân ứng trước</param>
        /// <param name="TotalPatientPayment_PaidInvoice">Tổng tiền Bệnh nhân cần trả cho những bill chưa được QUYẾT TOÁN và đã trả tiền rồi</param>
        /// <returns></returns>
        public abstract bool GetInPatientRegistrationNonFinalizedLiabilities(long registrationID, bool GetSumOfCashAdvBalanceOnly, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                                            , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized );
        public abstract List<RefGenMedProductSummaryInfo> GetRefGenMedProductSummaryByRegistration(long registrationID, long medProductType,long? DeptID);

        //---Dinh them-----
        public abstract List<PatientRegistrationDetail> GetPatientRegistrationDetailsByDay(long DeptLocID);

        public abstract List<PatientRegistrationDetail> GetPtRegisDetailsByConsultTimeSegment(long DeptLocID
                                                                                            , long V_TimeSegment
                                                                                            , long StartSequenceNumber
                                                                                            , long EndSequenceNumber);

        public abstract List<PatientRegistrationDetailEx> GetPatientRegistrationDetailsByRoom(out int totalCount, SeachPtRegistrationCriteria SeachRegCriteria);

        public abstract List<List<string>> ExportToExcelBangKeChiTietKhamBenh(SeachPtRegistrationCriteria SeachRegCriteria);

        public abstract List<PatientRegistrationDetail> SearchRegistrationsForDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        /// <summary>
        /// Cập nhật tổng giá tiền vượt quá 15% tháng lương tối thiểu.
        /// Nếu có chỉ định giá trị progSumMinusMinHi thì update giá trị này
        /// Nếu chưa chỉ định giá trị progSumMinusMinHi thì đọc hết trong database rồi cập nhật
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="minHi"></param>
        /// <param name="progSumMinusMinHi"></param>
        /// <param name="curProgSumMinusMinHi"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public abstract bool UpdateProgSumMinusMinHIForRegistration(long registrationID, decimal minHi, decimal progSumMinusMinHi, out decimal curProgSumMinusMinHi, DbConnection conn, DbTransaction tran);
        /// <summary>
        /// Cân bằng đăng ký lại
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="hiMinPayExceeded">Tong gia tien cua dang ky nay da vuot qua 15% thang luong toi thieu hay chua.</param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public abstract bool CorrectHiRegistration(long registrationID, bool hiMinPayExceeded, DbConnection conn, DbTransaction tran, long? returnedOutInvoiceID = null);
        public abstract bool CorrectRegistrationDetails(long registrationID, DbConnection conn, DbTransaction tran);
        public abstract bool CloseRegistration(long registrationID, AllLookupValues.RegistrationClosingStatus closingStatus, DbConnection conn, DbTransaction tran);
        
#region Outstanding Task
        /// <summary>
        /// Lấy danh sách các bệnh nhân đã trả tiền nhưng chưa được xếp vào phòng khám.
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        public abstract List<PatientQueue> GetAllUnqueuedPatients(long locationID, PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        
        /// <summary>
        /// Lấy tất cả các bệnh nhân đã được sắp xếp vào phòng khám, và có QueueID > markerQueueID.
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="markerQueueID"></param>
        /// <returns></returns>
        public abstract List<PatientQueue> GetAllQueuedPatients(long locationID, long queueType);

        public abstract bool Enqueue(PatientQueue queueItem);

#endregion

        //public abstract PayableSum CalculatePaymentSummary(IList<PatientRegistrationDetail> services, InsuranceBenefit benefit);






        /// <summary>
        /// Add danh sách transaction details vào bảng Transaction.
        /// </summary>
        /// <param name="transactionId">ID của transaction</param>
        /// <param name="tranDetailList">Danh sách transaction details</param>
        /// <param name="connection"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public abstract bool AddTransactionDetailList(long StaffID,long transactionId, List<PatientTransactionDetail> tranDetailList, out string ListOutID, DbConnection connection, DbTransaction tran);
        public abstract bool AddTransactionDetailList_InPt(long StaffID, long transactionId, List<PatientTransactionDetail> tranDetailList, out string ListOutID, DbConnection connection, DbTransaction tran);
        //Cac ham moi them:
        public abstract bool AddNewRegistration(PatientRegistration info, out long PatientRegistrationID, out List<long> newRegistrationDetailIdList
                                    , out List<long> newPclRequestIdList);
        public abstract bool AddNewRegistration(PatientRegistration info, out long PatientRegistrationID, out List<long> newRegistrationDetailIdList
            , out List<long> newPclRequestIdList
            , IList<DiagnosisIcd10Items> Icd10Items);

        //sp_AddBedPatientRegDetail
        public abstract bool AddBedPatientRegDetail(BedPatientRegDetail bedPatientDetail, out long bedPatientDetailId, DbConnection connection, DbTransaction tran);
        public abstract bool AddNewRegistrationDetails(PatientRegistrationDetail regDetails, out long RegDetailsID, DbConnection connection, DbTransaction tran);
        public abstract bool AddNewRegistrationDetails(PatientRegistrationDetail regDetails, out long RegDetailsID);

        public abstract bool AddNewPCLRequest(PatientPCLRequest info, out long PCLRequestID, DbConnection connection, DbTransaction tran);
        public abstract bool AddNewPCLRequest(PatientPCLRequest info, out long PCLRequestID);

        //public abstract bool UpdatePCLRequest(PatientPCLRequest info, DbConnection connection, DbTransaction tran);
        //public abstract bool UpdatePCLRequest(PatientPCLRequest info);

        public abstract bool AddNewPCLRequestDetails(long PCLRequestID, PatientPCLRequestDetail PCLRequestDetails, out long PCLRequestDetailsID, DbConnection connection, DbTransaction tran);
        public abstract bool AddNewPCLRequestDetails(long PCLRequestID, PatientPCLRequestDetail PCLRequestDetails, out long PCLRequestDetailsID);

        public abstract bool UpdatePCLRequestDetails(PatientPCLRequestDetail PCLRequestDetails, DbConnection connection, DbTransaction tran);
        public abstract bool UpdatePCLRequestDetails(PatientPCLRequestDetail PCLRequestDetails);


        public abstract bool UpdateAppointmentStatus(long appointmentID, long createdPtRegistrationID, AllLookupValues.ApptStatus newStatus);

        public abstract bool AddNewPatientMedicalRecord(PatientMedicalRecord info, out long PatientMedicalRecordID, DbConnection connection, DbTransaction tran);
        public abstract bool AddNewPatientMedicalRecord(PatientMedicalRecord info, out long PatientMedicalRecordID);

        public abstract bool AddNewPatientServiceRecord(long PatientMedicalRecordID,long? PatientMedicalFileID, PatientServiceRecord PtServiceRecord, out long PatientServiceRecordID, DbConnection connection, DbTransaction tran);
        public abstract bool AddNewPatientServiceRecord(long PatientMedicalRecordID, long? PatientMedicalFileID, PatientServiceRecord PtServiceRecord, out long PatientServiceRecordID);

        public abstract bool CheckIfMedicalRecordExists(long PatientID, out long PatientMedicalRecordID, out long? PatientMedicalFileID, DbConnection connection, DbTransaction tran);
        public abstract bool CheckIfMedicalRecordExists(long PatientID, out long PatientMedicalRecordID, out long? PatientMedicalFileID);

        public abstract Patient GetPatientByID_Simple(long patientID, DbConnection connection, DbTransaction tran);
        public abstract Patient GetPatientByID_Simple(long patientID);

        public abstract PatientRegistration GetPatientRegistrationByPtRegistrationID(long PtRegistrationID);

        public abstract long CheckRegistrationStatus(long PtRegistrationID);

        public abstract List<PatientRegistration> SearchRegistrations(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<PatientRegistration> SearchRegistrationsInPt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, bool bCalledFromSearchInPtPopup, out int totalCount);

        public abstract List<PatientRegistration> SearchRegisPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<PatientRegistrationDetail> SearchRegisDetailPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<PatientRegistrationDetail> SearchRegistrationListForOST(long DeptID, long DeptLocID);

        public abstract List<PatientRegistration> SearchRegistrationsDiagTrmt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<RefGenMedDrugDetails> SearchMedDrugs(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<RefGenMedProductDetails> SearchMedProducts(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<InwardDrugClinicDept> SearchInwardDrugClinicDept(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        /// <summary>
        /// Lấy danh sách inwardDrug tương ứng với các loại thuốc trong medProductList
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="medProductType"></param>
        /// <param name="medProductList"></param>
        /// <returns></returns>
        public abstract List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByProductList(long storeID, long medProductType, List<long> medProductList, DbConnection connection, DbTransaction tran);
        public abstract List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByProductList(long storeID, long medProductType, List<long> medProductList);
        public abstract List<RefMedicalServiceItem> SearchMedServices(MedicalServiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByIDList(List<long> inwardDrugIdList );
        public abstract List<RefGenMedProductDetails> GetGenMedProductsRemainingInStore(Dictionary<long, List<long>> drugIdList);
        public abstract List<PCLItem> SearchPCLItems(PCLItemSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract decimal GetTotalPatientPayForTransaction(long transactionID);
        public abstract decimal GetTotalPatientPayForTransaction(long transactionID, DbConnection conn, DbTransaction tran);

        public abstract decimal GetTotalPatientPayForTransaction_InPt(long transactionID);
        public abstract decimal GetTotalPatientPayForTransaction_InPt(long transactionID, DbConnection conn, DbTransaction tran);
        #region Outstanding Task
        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
        //public abstract bool PatientQueue_Insert(PatientQueue ObjPatientQueue, DbConnection connection, DbTransaction tran);
        //public abstract bool PatientQueue_InsertList(List<PatientQueue> lstPatientQueue,ref List<string> lstPatientQueueFail);
        //public abstract bool PatientQueue_Insert(PatientQueue ObjPatientQueue);
        public abstract bool PatientQueue_MarkDelete(Int64 RegistrationID, Int64 IDType, Int64 PatientID, Int64 V_QueueType, Int64 DeptLocID, DbConnection connection, DbTransaction tran);
        public abstract List<PatientQueue> PatientQueue_GetListPaging(PatientQueueSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        #endregion

        public abstract bool ChangeRegistrationType(PatientRegistration regInfo,AllLookupValues.RegistrationType newType);

        public abstract bool Registrations_UpdateStatus(PatientRegistration regInfo, long V_RegistrationStatus);

        public abstract List<Staff> GetStaffsHaveRegistrations(byte Type);
        public abstract List<Staff> GetStaffsHavePayments(long V_TradingPlaces);

        public abstract PaperReferal GetLatestPaperReferal(long hiid);
        public abstract InPatientBillingInvoice GetInPatientBillingInvoiceByIdWithoutDetails(long InPatientBillingInvID);
        public abstract InPatientBillingInvoice GetInPatientBillingInvoiceByIdWithoutDetails(long InPatientBillingInvID, DbConnection connection, DbTransaction tran);
        public abstract bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity, long StaffID, long Staff_DeptLocationID, DbConnection conn, DbTransaction tran, out long admissionID);
        public abstract bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity, long StaffID, long Staff_DeptLocationID, out long admissionID);


        public abstract InPatientAdmDisDetails GetInPatientAdmissionByID(long admissionId);
        public abstract InPatientAdmDisDetails GetInPatientAdmissionByID(long admissionId, DbConnection conn, DbTransaction tran);
        public abstract InPatientAdmDisDetails GetInPatientAdmissionByRegistrationID(long registration, DbConnection conn, DbTransaction tran);

        public abstract List<InPatientAdmDisDetails> InPatientAdmDisDetailsSearch(InPatientAdmDisDetailSearchCriteria p,
                                                                          DbConnection conn, DbTransaction tran);
        public abstract List<InPatientAdmDisDetails> InPatientAdmDisDetailsSearchPaging(InPatientAdmDisDetailSearchCriteria p
            , int pageIndex, int pageSize, bool bCountTotal, out int totalCount, DbConnection conn, DbTransaction tran);

        public abstract bool InsertInPatientDeptDetails(InPatientDeptDetail entity, DbConnection conn, DbTransaction tran, out long inPatientDeptDetailID);
        //public abstract bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc, DbConnection conn, DbTransaction tran);
        public abstract bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc);
        public abstract bool InsertInPatientDeptDetails(InPatientDeptDetail entity, out long inPatientDeptDetailID);

        public abstract bool ChangeInPatientDept(InPatientDeptDetail entity, out long inPatientDeptDetailID);
        public abstract bool ChangeInPatientDept(InPatientDeptDetail entity, DbConnection conn, DbTransaction tran, out long inPatientDeptDetailID);

        public abstract bool UpdateRegItemsToBill(long InPatientBillingInvID, List<long> regDetailIds, List<long> pclRequestIds, List<long> outDrugInvIds, DbConnection connection, DbTransaction tran);
        public abstract DateTime? BedPatientAlloc_GetLatestBillToDate(long BedPatientAllocID, DbConnection connection, DbTransaction tran);
        public abstract bool UpdatePatientRegistrationStatus(long registrationId,int FindPatient, AllLookupValues.RegistrationStatus newStatus, DbConnection connection, DbTransaction tran);

        public abstract List<BedPatientRegDetail> GetAllBedPatientRegDetailsByBedPatientID(long BedPatientId, bool IncludeDeletedItems, DbConnection connection, DbTransaction tran);
        public abstract List<BedPatientRegDetail> GetAllBedPatientRegDetailsByBedPatientID(long BedPatientId, bool IncludeDeletedItems);

        public abstract bool UpdateDeceaseInfo(DeceasedInfo info, DbConnection conn, DbTransaction tran);
        
        public abstract bool AddDeceaseInfo(DeceasedInfo info, DbConnection conn, DbTransaction tran, out long DeceaseId);
        public abstract bool DeleteDeceaseInfo(long deceaseId, DbConnection conn, DbTransaction tran);
        public abstract bool UpdateInPatientDischargeInfo(InPatientAdmDisDetails entity, long StaffID, DbConnection conn, DbTransaction tran);

        public abstract bool RevertDischarge(InPatientAdmDisDetails entity, long staffID);


        public abstract bool CheckBeforeDischarge(long registrationID, long DischargeDeptID, bool ConfirmNotTreatedAsInPt, out string errorMessages, out string confirmMessages);

        public abstract void CreateForm02(InPatientRptForm02 CurrentRptForm02, List<InPatientBillingInvoice> billingInvoices);

        public abstract IList<InPatientRptForm02> GetForm02(long PtRegistrationID);

        public abstract long? GetActiveHisID(long HIID);
        /// <summary>
        /// Thêm mới, cập nhật, xóa những dịch vụ (KCB, CLS, thuốc...)
        /// </summary>
        /// <param name="registrationID">ID của đăng ký</param>
        /// <param name="newRegistrationDetailList">Những dịch vụ KCB cần thêm mới</param>
        /// <param name="modifiedRegistrationDetailList">Những dịch vụ KCB cần cập nhật</param>
        /// <param name="deletedRegistrationDetailList">Những dịch vụ KCB cần xóa</param>
        /// <param name="newPclRequestList">Những yêu cầu CLS cần thêm mới</param>
        /// <param name="modifiedPclRequestList">Những yêu cầu CLS cần cập nhật. Trong mỗi yêu cầu CLS này, 
        /// các chi tiết CLS có thể có những trạng thái như: DETACHED(thêm mới), MODIFIED(cập nhật), DELETED(xóa)</param>
        /// <param name="deletedPclRequestList">Những yêu cầu CLS cần xóa</param>
        /// <param name="newInvoiceList">...Tương tự CLS</param>
        /// <param name="modifiedInvoiceList">...Tương tự CLS</param>
        /// <param name="deletedInvoiceList">...Tương tự CLS</param>
        /// <returns></returns>
        public abstract bool AddUpdateServiceForRegistration(long registrationID,long StaffID,
            long CollectorDeptLocID,
            bool ProgSumMinusMinHIModified, decimal? ProgSumMinusMinHINewValue, 
            long PatientMedicalRecordID
            , List<PatientRegistrationDetail> newRegistrationDetailList
            , List<PatientRegistrationDetail> modifiedRegistrationDetailList
            , List<PatientRegistrationDetail> deletedRegistrationDetailList
            , List<PatientPCLRequest> newPclRequestList
            , List<PatientPCLRequest> modifiedPclRequestList
            , List<PatientPCLRequest> deletedPclRequestList
            , List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum
            , List<OutwardDrugInvoice> newInvoiceList
            , List<OutwardDrugInvoice> modifiedInvoiceList
            , List<OutwardDrugInvoice> deletedInvoiceList
            , string modifiedTransactionString
            , long V_RegistrationType
            , out List<long> newRegistrationDetailIdList
            , out List<long> newPclRequestIdList
            , out long newPatientTransactionID
            , out List<long> newPaymentIDList
            , out List<long> newOutwardDrugInvoiceIDList
            , out string PaymentIDListNy
            , PromoDiscountProgram aPromoDiscountProgram
            , long? ConfirmHIStaffID = null
            , string OutputBalanceServicesXML = null
            , bool IsZeroValueHIConfirm = false
            , bool IsReported = false
            , bool IsUpdateHisID = false
            , long? HIID = null
            , double? PtInsuranceBenefit = null);

        public abstract PatientPCLRequest UpdateDrAndDiagTrmtForPCLReq(long ServiceRecID, long PCLReqID, long DoctorStaffID);


        public abstract bool SavePCLRequestsForInPt(long registrationID, long StaffID
                            ,long PatientMedicalRecordID
                            , long ReqFromDeptLocID
                            , long ReqFromDeptID
                            , List<PatientPCLRequest> newPclRequestList
                            , PatientPCLRequest deletedPclRequest
                            , List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum
                            , long V_RegistrationType
                            , out List<long> newPclRequestIdList);
        public abstract bool AddUpdateBillingInvoices(long registrationID
                                   , long patientMedicalRecordID
                                   , List<InPatientBillingInvoice> newInvoiceList
                                   , List<InPatientBillingInvoice> modifiedInvoiceList
                                   , List<InPatientBillingInvoice> deletedInvoiceList
                                   , string modifiedTransactionString
                                   , long V_RegistrationType
                                   , out List<long> newBillingInvoiceList
                                   , out long newPatientTransactionID
                                   , out List<long> newPaymentIDList);

        public abstract bool AddReportOutPatientCashReceiptList(List<ReportOutPatientCashReceipt> items);
        //HPT 24/08/2015: Thêm parameter vào hàm ApplyHiToInPatientRegistration để xác nhận bảo hiểm có xét đến điều kiện tham gia bảo hiểm năm năm liên tiếp
        /*==== #003 ====*/
        //* TxD 11/01/2018: Added new parameter ConfirmHICardForInPt_Types to enable joining HI Card
        public abstract bool ApplyHiToInPatientRegistration(long RegistrationID, long HIID, long HisID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient,
                        bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsHICard_FiveYearsCont_NoPaid = false, DateTime? FiveYearsAppliedDate = null, TypesOf_ConfirmHICardForInPt eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmNew);
        /*==== #003 ====*/
        public abstract bool RemoveHiFromInPatientRegistration(long RegistrationID, bool bIsEmergency, long RemoveHiStaffID, long? OldPaperReferalID);

        public abstract bool InsertAdditionalFee(long InPatientBillingInvID, long StaffID);

        #region StaffDeptResponsibility

        public abstract List<InPatientTransferDeptReq> GetInPatientTransferDeptReq(InPatientTransferDeptReq p);
        public abstract bool InsertInPatientTransferDeptReq(InPatientTransferDeptReq p);

        public abstract bool OutDepartment(InPatientDeptDetail InPtDeptDetail);

        public abstract bool UpdateInPatientTransferDeptReq(InPatientTransferDeptReq p);
        public abstract bool DeleteInPatientTransferDeptReq(long InPatientTransferDeptReqID);

        public abstract bool InPatientDeptDetailsTranfer(InPatientDeptDetail p, long InPatientTransferDeptReqID
                                                 , out long InPatientDeptDetailID);

        public abstract bool UpdateDeleteInPatientDeptDetails(InPatientDeptDetail inDeptDetailToDelete, InPatientDeptDetail inDeptDetailToUpdate, out InPatientDeptDetail inDeptDetailUpdated);

        public abstract bool DeleteInPatientTransferDeptReqXML(IList<InPatientTransferDeptReq> lstInPtTransDeptReq);

        protected virtual InPatientTransferDeptReq GetInPatientTransferDeptReqFromReader(IDataReader reader)
        {
            InPatientTransferDeptReq p = new InPatientTransferDeptReq();
            p.reqStaff = new Staff();
            p.ReqDeptLoc = new DeptLocation();
            p.ReqDeptLoc.RefDepartment=new RefDepartment();
            p.CurDept=new DeptLocation();
            p.CurDept.RefDepartment=new RefDepartment();
            p.InPatientAdmDisDetails=new InPatientAdmDisDetails();
            p.InPatientAdmDisDetails.PatientRegistration=new PatientRegistration();
            p.InPatientAdmDisDetails.PatientRegistration.Patient=new Patient();
            try
            {
                if (reader.HasColumn("PtRegistrationID"))
                {
                    p.PtRegistrationID = (long)reader["PtRegistrationID"];
                }
                if (reader.HasColumn("InPatientTransferDeptReqID") && reader["InPatientTransferDeptReqID"] != DBNull.Value)
                {
                    p.InPatientTransferDeptReqID = (long)(reader["InPatientTransferDeptReqID"]);
                }
                
	
                if (reader.HasColumn("InPatientAdmDisDetailID") && reader["InPatientAdmDisDetailID"] != DBNull.Value)
                {
                    p.InPatientAdmDisDetailID = (long)(reader["InPatientAdmDisDetailID"]);
                    p.InPatientAdmDisDetails.InPatientAdmDisDetailID = (long)(reader["InPatientAdmDisDetailID"]);
                }

             

                if (reader.HasColumn("ReqDeptLocID") && reader["ReqDeptLocID"] != DBNull.Value)
                {
                    p.ReqDeptLocID = (long)(reader["ReqDeptLocID"]);
                    p.ReqDeptLoc.DeptLocationID = (long)(reader["ReqDeptLocID"]);
                }
	
                if (reader.HasColumn("ReqDate") && reader["ReqDate"] != DBNull.Value)
                {
                    p.ReqDate = (DateTime)(reader["ReqDate"]);
                }
	
                if (reader.HasColumn("ReqStaffID") && reader["ReqStaffID"] != DBNull.Value)
                {
                    p.ReqStaffID = (long)(reader["ReqStaffID"]);
                    p.reqStaff.StaffID = (long)(reader["ReqStaffID"]);
                }



                if (reader.HasColumn("CurDeptID") && reader["CurDeptID"] != DBNull.Value)
                {
                    p.CurDept.DeptID = ((long)reader["CurDeptID"]);
                    p.CurDept.RefDepartment.DeptID = ((long)reader["CurDeptID"]);
                }

                if (reader.HasColumn("curDeptName") && reader["curDeptName"] != DBNull.Value)
                {
                    p.CurDept.RefDepartment.DeptName = (reader["curDeptName"].ToString());
                }


                if (reader.HasColumn("ReqDeptID") && reader["ReqDeptID"] != DBNull.Value)
                {
                    p.ReqDeptID = (long)(reader["ReqDeptID"]);
                    p.ReqDeptLoc.RefDepartment.DeptID = (long)(reader["ReqDeptID"]);
                }

                if (reader.HasColumn("ReqDeptName") && reader["ReqDeptName"] != DBNull.Value)
                {
                    p.ReqDeptLoc.RefDepartment.DeptName = (reader["ReqDeptName"].ToString());
                }

                if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                {
                    p.reqStaff.FullName = (reader["FullName"].ToString());
                }

                if (reader.HasColumn("PtRegistrationID") && reader["PtRegistrationID"] != DBNull.Value)
                {
                    p.InPatientAdmDisDetails.PatientRegistration.PtRegistrationID = (long)(reader["PtRegistrationID"]);
                }
                
                if (reader.HasColumn("PtFullName") && reader["PtFullName"] != DBNull.Value)
                {
                    p.InPatientAdmDisDetails.PatientRegistration.Patient.FullName = (reader["PtFullName"].ToString());
                }
                if (reader.HasColumn("PatientCode") && reader["PatientCode"] != DBNull.Value)
                {
                    p.InPatientAdmDisDetails.PatientRegistration.Patient.PatientCode = (reader["PatientCode"].ToString());
                }

                if (reader.HasColumn("IsAccepted") && reader["IsAccepted"] != DBNull.Value)
                {
                    p.IsAccepted = Convert.ToBoolean(reader["IsAccepted"]);
                }

            }
            catch
            { return null; }
            return p;
        }
        protected virtual List<InPatientTransferDeptReq> GetInPatientTransferDeptReqCollectionFromReader(IDataReader reader)
        {
            List<InPatientTransferDeptReq> InPatientTransferDeptReq = new List<InPatientTransferDeptReq>();
            while (reader.Read())
            {
                InPatientTransferDeptReq.Add(GetInPatientTransferDeptReqFromReader(reader));
            }
            return InPatientTransferDeptReq;
        }

        #endregion


        public abstract PatientPCLRequestDetail PatientPCLRequestDetails_GetDeptLocID(long PatientPCLReqID);

        public abstract PatientRegistrationDetail GetPtRegDetailNewByPatientID(long PatientID);

        public abstract bool BuildPCLExamTypeDeptLocMap();

        public abstract bool BuildPclDeptLocationList();

        public abstract bool BuildAllServiceIdDeptLocMap();

        public abstract bool TestDatabaseConnectionOK();

        public abstract bool AddInPatientInstruction(PatientRegistration ptRegistration, out long IntPtDiagDrInstructionID);
        public abstract InPatientInstruction GetInPatientInstruction(PatientRegistration ptRegistration);
        public abstract List<InPatientInstruction> GetInPatientInstructionCollection(PatientRegistration aRegistration);
        public abstract InPatientInstruction GetInPatientInstructionByInstructionID(long aIntPtDiagDrInstructionID);
        public abstract List<Intravenous> GetIntravenousPlan_InPt(long IntPtDiagDrInstructionID);
        public abstract bool GetAllItemsByInstructionID(long IntPtDiagDrInstructionID, out List<PatientRegistrationDetail> AllRegistrationItems, out List<PatientPCLRequestDetail> AllPCLRequestItems);
        //▼===== #005
        #region Thông tin hành chính, BHYT, CV
        public abstract bool AddPatientAndHIDetails(Patient newPatient, HealthInsurance newHICard, PaperReferal newPaperReferal, out long PatientID, out string PatientCode, out string PatientBarCode, out long HIID, out long paperReferalID);

        public abstract bool UpdatePatientAndHIDetails(Patient patient, bool IsAdmin, long StaffID, bool IsEditAfterRegistration, out string Result);

        public abstract bool AddNewPatientAndHIDetails(Patient newPatientAndHiDetails, out long PatientID, out long HIID, out long PaperReferralID);
        public abstract bool UpdateNewPatientAndHIDetails(Patient updatedPatientAndHiDetails, bool IsAdmin, long StaffID, bool IsEditAfterRegistration,
                                                           out long PatientID, out long HIID, out long PaperReferralID, out double RebatePercentage, out string Result);

        #endregion
        //▲===== #005
        //▼===== #006
        public abstract List<PatientRegistrationDetail> SearchInPatientRegistrationListForOST(long DeptID);
        //▲===== #006
        //▼====== #007
        public abstract List<InsuranceBenefitCategories> GetInsuranceBenefitCategoriesValues();
        //▲====== #007
        //▼====== #008
        public abstract bool UpdateBasicDiagTreatment(PatientRegistration regInfo, string newBasicDiagTreatment);
        //▲====== #008
    }

    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> root, IEnumerable<T> collection)
        {
            if(collection == null)
            {
                return;
            }
            foreach (var item in collection)
            {
                root.Add(item);
            }
        }

    }
}