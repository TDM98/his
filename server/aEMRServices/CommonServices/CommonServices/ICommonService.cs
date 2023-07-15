/*
 * 20170218 #001 CMN: Add Checkbox AllDept for InPtBills
 * 20170522 #002 CMN: Added variable to check InPt 5 year HI without paid enough
 * 20170609 #003 CMN: Fix SupportFund About TT04 with TT04
 * 20170619 #004 CMN: Service for payment report OutPt with large data
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using eHCMS.Configurations;
using eHCMS.Services.Core;
using DataEntities;
using ErrorLibrary;
using System.IO;

namespace CommonServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    [Obsolete("This Class has been DEPRECATED. CommonService has been REPLACED by other Classes ie. PatientRegistrationService, CommonServices_V2 etc...", true)]
    public interface ICommonService
    {
        [FaultContract(typeof(AxException))]
        [OperationContract]
        bool DoListUpload(List<PCLResultFileStorageDetail> lst);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        bool DoUpload(string filename, byte[] content, bool append,string dir);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        bool SaveImageCapture(byte[] byteArray);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        bool SaveImageCapture1(MemoryStream byteArray);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        bool UpdateRefApplicationConfigs(string ConfigItemKey, string ConfigItemValue, string ConfigItemNotes);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        List<SuburbNames> GetAllSuburbNames();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Gender> GetAllGenders();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<AllLookupValues.RegistrationStatus> GetAllRegistrationStatus();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllLookupValuesByType(LookupValues lookUpType);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllLookupValuesForTransferForm(LookupValues lookUpType);


        // TODO: Add your service operations here

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllMaritalStatus();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllEthnics();

        [FaultContract(typeof (AxException))]
        [OperationContract]
        IList<Lookup> GetAllReligion();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllBankName();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllFamilyRelationships();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefCountry> GetAllCountries();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Currency> GetAllCurrency(bool? IsActive);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<CitiesProvince> GetAllProvinces();


        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Hospital> Hospital_ByCityProvinceID(long? CityProvinceID);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Get();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefGenericDrugCategory_1> RefGenericDrugCategory_1_Get(long V_MedProductType);

  	    [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefGenericDrugCategory_2> RefGenericDrugCategory_2_Get(long V_MedProductType);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        Staff GetStaffByID(Int64 ID);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Staff> GetAllStaffContain();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        List<StaffPosition> GetAllStaffPosition();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<DeptTransferDocReq> GetAllDocTypeRequire();


        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefDepartment> GetAllDepartments(bool bIncludeDeleted);


        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefDepartment> GetAllDepartmentsByV_DeptTypeOperation(long V_DeptTypeOperation);

   
        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllDocumentTypeOnHold();

        //[OperationContract]
        //byte[] GetImage(string path);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        Stream GetVideoAndImage(string path);

        //[FaultContract(typeof(AxException))]
        //[OperationContract]
        //IList<string> GetAllConfigItemValues();

        [FaultContract(typeof (AxException))]
        [OperationContract]
        AxServerConfigSection GetServerConfiguration();

        //[FaultContract(typeof(AxException))]
        //[OperationContract]
        //string GetConfigItemsValueBySerialNumber(int sNumber);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefDepartment> GetDepartments(long locationID);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        List<PatientTransactionDetail> GetTransactionSum(long TransactionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientTransactionDetail> GetTransactionSum_InPt(long TransactionID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientTransactionPayment> GetAllPayments_New(long transactionID);

        //[OperationContract]
        //[FaultContract(typeof (AxException))]
        //List<PatientPayment> GetPatientPaymentByDay(PatientPaymentSearchCriteria PatientPaymentSearch);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientTransactionPayment> GetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch,int FindPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff,
                                            List<PatientTransactionPayment> allPayment, out long RepPaymentRecvID);//List<PatientPayment> allPayment,

        [OperationContract]
        [FaultContract(typeof (AxException))]
        List<ReportPaymentReceiptByStaff> GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate, bool bFilterByStaffID, Int64 nStaffID, long loggedStaffID);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        ReportPaymentReceiptByStaffDetails GetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff);

        //--------------------------------------


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePatientRegistrationPayStatus(long PtRegistrationID, long V_RegistrationPaymentStatus);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePatientRegistration(long PtRegistrationID, long V_RegistrationStatus);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientRegistration_Close(long PtRegistrationID, int FindPatient, long V_RegistrationClosingStatus);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchPtRegistration(SeachPtRegistrationCriteria criteria, int pageSize, int pageIndex, bool bcount, out int Totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime GetMaxExamDate();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Patient GetPatientInfoByPtRegistration(long? PtRegistrationID,long? PatientID,int FindPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CalcOutwardDrugInvoice(long registrationID, OutwardDrugInvoice drugInvoice, out OutwardDrugInvoice CalculatedDrugInvoice, out PayableSum PayableSum, out decimal TotalPaid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SaveEmptyRegistration(long StaffID, long CollectorDeptLocID,int? Apply15HIPercent, PatientRegistration registrationInfo, long V_RegistrationType, out PatientRegistration SavedRegistration);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddInPatientAdmission(PatientRegistration registrationInfo, InPatientDeptDetail deptDetail, long StaffID, long Staff_DeptLocationID, out long RegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc);

        

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CancelRegistration(PatientRegistration registrationInfo, out PatientRegistration cancelledRegistration);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void UpdatePCLRequest(long StaffID,PatientPCLRequest request, out List<PatientPCLRequest> listPclSave, DateTime modifiedDate = default(DateTime));

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void SaveInPatientRegistration(PatientRegistration registrationInfo, long billingType, out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out InPatientBillingInvoice AddedBillingInvoice);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateSuggestCashAdvance(long InPatientBillingInvID, long StaffID, out long RptPtCashAdvRemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddInPatientBillingInvoice(int? Apply15HIPercent,PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay, out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID);
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void SaveRegistrationAndPay(PatientRegistration registrationInfo, PatientPayment paymentDetails, out PatientRegistration SavedRegistration, out PatientPayment PaymentInfo, out List<OutwardDrugInvoice> NewInvoiceList);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PayForRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient, PatientTransactionPayment paymentDetails,// PatientPayment paymentDetails,
                                                List<PatientRegistrationDetail> colPaidRegDetails,
                                                List<PatientPCLRequest> colPaidPclRequests,
                                                List<OutwardDrugInvoice> colPaidDrugInvoice,
                                                List<InPatientBillingInvoice> colBillingInvoices
            , out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList
            , out V_RegistrationError error, bool checkBeforePay = false);//out PatientPayment PaymentInfo

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetRegistrationInfo(long registrationID, int FindPatient,bool loadFromAppointment = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientAdmDisDetails GetInPatientAdmissionByRegistrationID(long regID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetRegistrationInfo_InPt(long registrationID, int FindPatient, LoadRegistrationSwitch LoadRegisSwitch, bool loadFromAppointment = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetRegistration(long registrationID,int FindPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetRegistrationSimple(long registrationID, int PatientFindBy);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        TransferForm CreateBlankTransferFormByRegID(long PtRegistrationID, int PatientFindBy, long V_TransferFormType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateInPatientBillingInvoice(long? StaffID,InPatientBillingInvoice billingInv
             , List<PatientRegistrationDetail> newRegDetails
            , List<PatientRegistrationDetail> deletedRegDetails
            , List<PatientPCLRequest> newPclRequests
            , List<PatientPCLRequestDetail> newPclRequestDetails
            , List<PatientPCLRequestDetail> deletedPclRequestDetails
            , List<OutwardDrugClinicDeptInvoice> newOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> savedOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> modifiedOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> deleteOutwardDrugClinicDeptInvoices
            , List<PatientRegistrationDetail> modifiedRegDetails
             , List<PatientPCLRequestDetail> modifiedPclRequestDetails
            , out Dictionary<long, List<long>> DrugIDList_Error);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertAdditionalFee(long InPatientBillingInvID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GetInPatientBillingInvoiceDetails(long billingInvoiceID
                                                        , out List<PatientRegistrationDetail> regDetails
                                                        , out List<PatientPCLRequest> PCLRequestList
                                                        , out List<OutwardDrugClinicDeptInvoice> allInPatientInvoices);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientBillingInvoice GetInPatientBillingInvoice(long billingInvoiceID);
        //lay ben pharmacy qua ne
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveDrugs(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice SavedOutwardInvoice);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CalcInvoiceForSaveAndPayButton(OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice SavedOutwardInvoice, out PatientRegistration RegistrationInfo);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration ChangeRegistrationType(PatientRegistration regInfo, AllLookupValues.RegistrationType newType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime GetDate();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Hospitalize(PatientRegistration regInfo, BedPatientAllocs selectedBed, DateTime? admissionDate, int ExpectedStayingDays, out AllLookupValues.RegistrationType NewRegType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLRequest UpdateDrAndDiagTrmtForPCLReq(long ServiceRecID, long PCLReqID, long DoctorStaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration AddServicesAndPCLRequests(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList, List<PatientPCLRequest> deletedPclRequestList, out long NewRegistrationID, out List<PatientRegistrationDetail> SavedRegistrationDetailList, out List<PatientPCLRequest> SavedPclRequestList
            , out V_RegistrationError error, DateTime modifiedDate = default(DateTime));

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddPCLRequestsForInPt(long StaffID, long ReqFromDeptLocID, long ReqFromDeptID, PatientRegistration regInfo, PatientPCLRequest pclRequest, PatientPCLRequest deletedPclRequest, out List<PatientPCLRequest> SavedPclRequestList
            , DateTime modifiedDate = default(DateTime));


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DeletePCLRequestWithDetails(Int64 PatientPCLReqID,  out string Result);
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DeleteInPtPCLRequestWithDetails(Int64 PatientPCLReqID);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        void AddPCLRequest(long StaffID,long patientID, long registrationID, PatientPCLRequest pclRequest, out PatientPCLRequest SavedPclRequest);



        /// <summary>
        /// Xóa những dịch vụ đã tính tiền rồi.
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="colPaidRegDetails"></param>
        /// <param name="colPaidPclRequests"></param>
        /// <param name="colPaidDrugInvoice"></param>
        /// <param name="colPaidMedItemList"></param>
        /// <param name="colPaidChemicalItem"></param>
        [OperationContract]
        [FaultContract(typeof (AxException))]
        void RemovePaidRegItems(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient,
                                List<PatientRegistrationDetail> colPaidRegDetails,
                                List<PatientPCLRequest> colPaidPclRequests,
                                List<OutwardDrugInvoice> colPaidDrugInvoice,
                                List<OutwardDrugClinicDeptInvoice> colPaidMedItemList,
                                List<OutwardDrugClinicDeptInvoice> colPaidChemicalItem
                                , out V_RegistrationError error);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddOutwardDrugReturn(long StaffID, int? Apply15HIPercent, OutwardDrugInvoice Outward, List<OutwardDrug> Details, out long outiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CancelOutwardDrugInvoice(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice invoice);

        //HPT 24/08/2015: Thêm parameter IsHICard_FiveYearsCont vào hàm CalculateHiBenefit để tính quyền lợi bảo hiểm áp dụng luật bảo hiểm 5 năm liên tiếp
        [OperationContract]
        [FaultContract(typeof(AxException))]
        double CalculateHiBenefit(HealthInsurance healthInsurance, PaperReferal paperReferal, out bool isConsideredAsCrossRegion, bool IsEmergency = false, long V_RegistrationType = 0, bool IsEmergInPtReExamination = false, bool IsHICard_FiveYearsCont = false, bool IsChildUnder6YearsOld = false, bool IsAllowCrossRegion = false);

        /*==== #002 ====*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        double CalculateHiBenefit_V2(HealthInsurance healthInsurance, PaperReferal paperReferal, out bool isConsideredAsCrossRegion, bool IsEmergency = false, long V_RegistrationType = 0, bool IsEmergInPtReExamination = false, bool IsHICard_FiveYearsCont = false, bool IsChildUnder6YearsOld = false, bool IsAllowCrossRegion = false, bool IsHICard_FiveYearsCont_NoPaid = false);
        /*==== #002 ====*/

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Hospital> SearchHospitals(string HospitalName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Hospital> SearchHospitalsNew(HospitalSearchCriteria entity, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Hospital SearchHospitalByHICode(string HiCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Hospital> LoadCrossRegionHospitals();


        #region Ny them member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddTransactionForDrug(PatientTransactionPayment payment, long outiID, long V_TranRefType, out long PaymentID);//PatientPayment payment,

        #endregion 

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PayForInPatientRegistration(long StaffID,long registrationID, PatientTransactionPayment paymentDetails,
                                                List<InPatientBillingInvoice> billingInvoices
                                                , out PatientTransaction Transaction, out PatientTransactionPayment PaymentInfo, out long PtCashAdvanceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void FinalizeInPatientBillingInvoices(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,
                                                List<InPatientBillingInvoice> billingInvoices, out PatientTransaction Transaction);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CreateForm02(InPatientRptForm02 CurrentRptForm02, List<InPatientBillingInvoice> billingInvoices);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InPatientRptForm02> GetForm02(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void InPatientSettlement(long ptRegistrationID, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void InPatientPayForBill(PatientRegistration registration, IList<InPatientBillingInvoice> billingInvoices, decimal payAmount, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CreateNewRegistrationVIP(PatientRegistration regInfo, out long newRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ReturnInPatientDrug(long registrationID, List<RefGenMedProductSummaryInfo> returnedItems,long? DeptID,long? StaffID=null);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ChangeInPatientDept(InPatientDeptDetail entity, out long inPatientDeptDetailID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CreateBillingInvoiceFromExistingItems(PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, out long NewBillingInvoiceID);
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientBillingInvoice LoadInPatientRegItemsIntoBill(long registrationID, long? DeptID, long StoreID, long StaffID, bool IsAdmin);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CloseRegistration(long registrationID, int FindPatient,out List<string> errorMessages);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CloseRegistrationAll(long PtRegistrationID, int FindPatient, out string Error);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddInPatientDischarge(InPatientAdmDisDetails admissionInfo, long StaffID, out List<string> errorMessages);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RevertDischarge(InPatientAdmDisDetails admissionInfo, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckBeforeDischarge(long registrationID, long DischargeDeptID, bool ConfirmNotTreatedAsInPt, out string errorMessages, out string confirmMessages);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        //PatientRegistration GetInPatientRegistrationAndPaymentInfo(long registrationID, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient);
        bool GetInPatientRegistrationAndPaymentInfo(long registrationID, bool GetSumOfCashAdvBalanceOnly, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                    , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RecalcInPatientBillingInvoice(long? StaffID,InPatientBillingInvoice billingInv);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Registrations_UpdateStatus(PatientRegistration regInfo, long V_RegistrationStatus);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientBillingInvoice> GetAllInPatientBillingInvoices(long PtRegistrationID,long? DeptID);

        //==== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientBillingInvoice> GetAllInPatientBillingInvoicesByDeptArray(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray);
        //==== #001

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_FromListDeptID(long PtRegistrationID, List<long> ListDeptIDs);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_ForCreateForm02(long RptForm02_InPtID, long PtRegistrationID, List<long> ListDeptIDs);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientPaymentAccount> GetAllPatientPaymentAccounts();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_DeptID(long DeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientCashAdvance_Insert(PatientCashAdvance payment, out long PtCashAdvanceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientCashAdvance_Delete(PatientCashAdvance payment, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientCashAdvance> PatientCashAdvance_GetAll(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GenericPayment_FullOperation(GenericPayment GenPayment, out GenericPayment OutGenericPayment);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<GenericPayment> GenericPayment_GetAll(DateTime? FromDate, DateTime? ToDate, long? V_GenericPaymentType, long? StaffID);
      
        [OperationContract]
        [FaultContract(typeof(AxException))]
        GenericPayment GenericPayment_SearchByCode(string GenPaymtCode, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientCashAdvance> GetCashAdvanceBill(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<TransactionFinalization> GetPatientSettlement(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientTransaction GetAllPaymentByRegistrationID_InPt(long registrationID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail, long PtRegistrationID, long V_RegistrationType, out long PtTranPaymtID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RptPatientCashAdvReminder_Insert(RptPatientCashAdvReminder payment, out long RptPtCashAdvRemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RptPatientCashAdvReminder_Update(RptPatientCashAdvReminder payment);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RptPatientCashAdvReminder_Delete(long RptPtCashAdvRemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RptPatientCashAdvReminder> RptPatientCashAdvReminder_GetAll(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt(SearchOutwardReport Searchcriate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientTransactionPayment_UpdateNote(List<PatientTransactionPayment> allPayment);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientTransactionPayment_UpdateID(PatientTransactionPayment Payment);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientTransactionPayment> PatientTransactionPayment_Load(long TransactionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt_TongHop(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, out List<PatientTransactionPayment> OutPaymentLst);

        /*▼====: #004*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt_TongHop_Async(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReportOutPatientCashReceipt_Payments> GetMoreReportOutPatientCashReceipt_TongHop_Async(int RefAsyncKey, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey);
        /*▲====: #004*/

        //export excel all
        #region Export excel from database
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_ListRefGenericDrug(DrugSearchCriteria criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_ListRefGenMedProductDetail(RefGenMedProductDetailsSearchCriteria criteria);

        #endregion

        //HPT_20160619: About quỹ hỗ trợ bệnh nhân sử dụng dịch vụ kỹ thuật cao
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<CharityOrganization> GetAllCharityOrganization();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<CharitySupportFund> GetCharitySupportFundForInPt(long PtRegistrationID, long? BillingInvID);

        /*▼====: #003*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<CharitySupportFund> GetCharitySupportFundForInPt_V2(long PtRegistrationID, long? BillingInvID, bool? IsHighTechServiceBill);
        /*▲====: #003*/
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<CharitySupportFund> SaveCharitySupportFundForInPt(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds);

        /*▼====: #003*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<CharitySupportFund> SaveCharitySupportFundForInPt_V2(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds, bool IsHighTechServiceBill);
        /*▲====: #003*/

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddInPatientInstruction(PatientRegistration ptRegistration, out long IntPtDiagDrInstructionID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientInstruction GetInPatientInstruction(PatientRegistration ptRegistration);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Intravenous> GetIntravenousPlan_InPt(long IntPtDiagDrInstructionID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GetAllItemsByInstructionID(long IntPtDiagDrInstructionID, out List<PatientRegistrationDetail> AllRegistrationItems, out List<PatientPCLRequestDetail> AllPCLRequestItems);
    }
}