/*
 * 20170218 #001 CMN: Add Checkbox AllDept for InPtBills
 * 20170522 #002 CMN: Added variable to check InPt 5 year HI without paid enough
 * 20170609 #003 CMN: Fix SupportFund About TT04 with TT04
 * 20191031 #004 TNHX: 	0017411 Add new func for calInPatientBillingInvoice With Pricelist
 * 20210717 #005 TNHX: 	Truyền phương thức thanh toán + mã code thanh toán online
 * 20220121 #006 TNHX: 	Thêm func cập nhật chi tiết đăng ký nội trú
 * 20220526 #007 DatTB: Thêm chức năng Xác nhận bn hoãn/ miễn tạm ứng
 * 20220531 #008 DatTB: Thêm biến IsWithOutBill Hủy quyết toán bỏ qua HDDT và phân quyền
 * 20220511 #009 QTD:   Thêm func lấy lần thanh toán cuối cho màn hình xác nhận BHYT
 * 20230109 #010 QTD:   Thêm điều kiện đánh dấu lưu chỉ định từ màn hình chỉ định dịch vụ
 */
using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataEntities;
using ErrorLibrary;
using BillingPaymentWcfServiceLib.BankingPaymentServiceReference;
using System.Collections.ObjectModel;

namespace BillingPaymentWcfServiceLib
{
    [ServiceContract]
    public interface IBillingPaymentWcfServiceLib
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CalcOutwardDrugInvoice(long registrationID, OutwardDrugInvoice drugInvoice, out OutwardDrugInvoice CalculatedDrugInvoice, out PayableSum PayableSum, out decimal TotalPaid);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateSuggestCashAdvance(long InPatientBillingInvID, long StaffID, out long RptPtCashAdvRemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddInPatientBillingInvoice(int? Apply15HIPercent, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay, out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID, bool IsNotCheckInvalid = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PayForRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient, PatientTransactionPayment paymentDetails,// PatientPayment paymentDetails,
                                                List<PatientRegistrationDetail> colPaidRegDetails,
                                                List<PatientPCLRequest> colPaidPclRequests,
                                                List<OutwardDrugInvoice> colPaidDrugInvoice,
                                                List<InPatientBillingInvoice> colBillingInvoices
            , out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList
            , out V_RegistrationError error, out string responseMsg, bool checkBeforePay = false);//out PatientPayment PaymentInfo

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PayForRegistration_V2(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient, PatientTransactionPayment paymentDetails,// PatientPayment paymentDetails,
            List<PatientRegistrationDetail> colPaidRegDetails,
            List<PatientPCLRequest> colPaidPclRequests,
            List<OutwardDrugInvoice> colPaidDrugInvoice,
            List<InPatientBillingInvoice> colBillingInvoices,
            PromoDiscountProgram PromoDiscountProgramObj
            , out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList
            , out V_RegistrationError error, out string responseMsg, bool checkBeforePay = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PayForRegistration_V3(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient, PatientTransactionPayment paymentDetails,// PatientPayment paymentDetails,
            List<PatientRegistrationDetail> colPaidRegDetails,
            List<PatientPCLRequest> colPaidPclRequests,
            List<OutwardDrugInvoice> colPaidDrugInvoice,
            List<InPatientBillingInvoice> colBillingInvoices,
            PromoDiscountProgram PromoDiscountProgramObj
            , out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList
            , out V_RegistrationError error, out string responseMsg, bool checkBeforePay = false
            , long? ConfirmHIStaffID = null
            , string OutputBalanceServicesXML = null
            , bool IsReported = false
            , bool IsUpdateHisID = false
            , long? HIID = null
            , double? PtInsuranceBenefit = null
            , PatientRegistration existingPtRegisInfo = null
            , bool IsNotCheckInvalid = false
            //▼====: #005
            , bool IsRefundBilling = false, bool IsProcess = false
            , string TranPaymtNote = null, long? V_PaymentMode = null);
            //▲====: #005

        [OperationContract]
		[FaultContract(typeof(AxException))]
		TransactionResponse HasCard(long patientID, long staffID);

		[OperationContract]
		[FaultContract(typeof(AxException))]
		TransactionResponse MapCard(bool isHospital, long patientID, string acctNo,
			string pan, string issueDate, long staffID);

		[OperationContract]
		[FaultContract(typeof(AxException))]
		TransactionResponse UnmapCard(bool isHospital, long patientID, long staffID);

		[OperationContract]
		[FaultContract(typeof(AxException))]
		TransactionResponse Deposit(bool isHospital, long patientID, decimal settlementAmount,
			long staffID);

		[OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv
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
            , bool IsNotCheckInvalid
            , out Dictionary<long, List<long>> DrugIDList_Error);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertAdditionalFee(long InPatientBillingInvID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GetInPatientBillingInvoiceDetails(long billingInvoiceID
            , out List<PatientRegistrationDetail> regDetails
            , out List<PatientPCLRequest> PCLRequestList
            , out List<OutwardDrugClinicDeptInvoice> allInPatientInvoices
            , bool IsUsePriceByNewCer
            , bool IsRecalInSecondTime
            , bool IsPassCheckNonBlockValidPCLExamDate);

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
        PatientRegistration AddServicesAndPCLRequests(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList, List<PatientPCLRequest> deletedPclRequestList, out long NewRegistrationID, out List<PatientRegistrationDetail> SavedRegistrationDetailList
            , out List<PatientPCLRequest> SavedPclRequestList, out V_RegistrationError error, DateTime modifiedDate = default(DateTime)
            //▼====: #005
            , bool IsNotCheckInvalid = false, bool IsCheckPaid = false, bool IsProcess = false, string TranPaymtNote = null, long? V_PaymentMode = null
            //▲====: #005
            //▼====: #010
            , bool IsFromRequestDoctor = false
            //▲====: #010
            , long? V_ReceiveMethod = null
            );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ConfirmRegistrationHIBenefit(long PtRegistrationID, long? StaffID, bool IsUpdateHisID, long? aHIID, double PtInsuranceBenefit);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddPCLRequestsForInPt(long StaffID, long ReqFromDeptLocID, long ReqFromDeptID, PatientRegistration regInfo, PatientPCLRequest pclRequest, PatientPCLRequest deletedPclRequest, bool IsNotCheckInvalid
            , out List<PatientPCLRequest> SavedPclRequestList, DateTime modifiedDate = default(DateTime));

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddPCLRequest(long StaffID, long patientID, long registrationID, PatientPCLRequest pclRequest, out PatientPCLRequest SavedPclRequest);

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
        [FaultContract(typeof(AxException))]
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

        #region Ny them member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddTransactionForDrug(PatientTransactionPayment payment, long outiID, long V_TranRefType, out long PaymentID);//PatientPayment payment,
        #endregion 

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PayForInPatientRegistration(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,
                                                List<InPatientBillingInvoice> billingInvoices
                                                , out PatientTransaction Transaction, out PatientTransactionPayment PaymentInfo, out long PtCashAdvanceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void FinalizeInPatientBillingInvoices(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,
            List<InPatientBillingInvoice> billingInvoices, decimal totalPaymentAmount, out string msg, out PatientTransaction Transaction);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        string DeleteTransactionFinalization(string FinalizedReceiptNum, long StaffID, long V_RegistrationType, long? PtRegistrationID, bool IsWithOutBill); //<=== #008
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DeleteTranacsionPayment_InPt(string ReceiptNumber, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void InPatientPayForBill(PatientRegistration registration, IList<InPatientBillingInvoice> billingInvoices, decimal payAmount, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ReturnInPatientDrug(long registrationID, List<RefGenMedProductSummaryInfo> returnedItems, long? DeptID, long? StaffID = null);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CreateBillingInvoiceFromExistingItems(PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, out long NewBillingInvoiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientBillingInvoice LoadInPatientRegItemsIntoBill(long registrationID, long? DeptID, long StoreID, long StaffID, bool IsAdmin
            , DateTime? FromDate, DateTime? ToDate, int FindPatientType, int LoadBillType, DateTime? DischargeDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int CheckForPreloadingBills(long registrationID, out string errorMsg);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        //PatientRegistration GetInPatientRegistrationAndPaymentInfo(long registrationID, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient);
        bool GetInPatientRegistrationAndPaymentInfo(long registrationID, bool GetSumOfCashAdvBalanceOnly, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                    , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RecalcInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv, bool IsUsePriceByNewCer, bool IsAutoCheckCountHI, bool ReplaceMaxHIPay
            , bool ReCalBillingInv
            , bool IsNotCheckInvalid
            , bool IsPassCheckNonBlockValidPCLExamDate);

        //▼====: 004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RecalcInPatientBillingInvoiceWithPriceList(long? StaffID, InPatientBillingInvoice billingInv, bool IsUsePriceByNewCer, bool IsAutoCheckCountHI, bool ReplaceMaxHIPay
            , long MedServiceItemPriceListID, long PCLExamTypePriceListID, bool ReCalBillingInv);
        //▲====: 004

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientBillingInvoice> GetAllInPatientBillingInvoices(long PtRegistrationID, long? DeptID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PromoDiscountProgram GetPromoDiscountProgramByPromoDiscProgID(long PromoDiscProgID, long V_RegistrationType);

        //==== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientBillingInvoice> GetAllInPatientBillingInvoicesByDeptArray(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray, long V_RegistrationType);
        //==== #001

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_FromListDeptID(long PtRegistrationID, List<long> ListDeptIDs);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_ForCreateForm02(long RptForm02_InPtID, long PtRegistrationID, List<long> ListDeptIDs);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientCashAdvance_Insert(PatientCashAdvance payment, long patientId, out long PtCashAdvanceID, out string msg);
        
        //▼==== #007
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ConfirmPatientPostponementAdvance(InPatientAdmDisDetails AdmissionInfo, long StaffID);
        //▲==== #007

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientCashAdvance_Delete(PatientCashAdvance payment, long patientId, long staffID, out string msg);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientCashAdvance> GetCashAdvanceBill(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail,
            long PtRegistrationID, long patientId, long V_RegistrationType, out long PtTranPaymtID, out string msg);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RptPatientCashAdvReminder_Insert(RptPatientCashAdvReminder payment, out long RptPtCashAdvRemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RptPatientCashAdvReminder_Update(RptPatientCashAdvReminder payment);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RptPatientCashAdvReminder_Delete(long RptPtCashAdvRemID);

        #region Quỹ hỗ trợ bệnh nhân sử dụng dịch vụ kỹ thuật cao
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
        bool AddCharityOrganization(string CharityOrgName);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool EditCharityOrganization(long CharityOrgID, string CharityOrgName);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteCharityOrganization(long CharityOrgID, long StaffID);
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Recal15PercentHIBenefit(long PtRegistrationID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RecalRegistrationHIBenefit(long PtRegistrationID, long StaffID, out string OutputBalanceServicesXML
            , IList<InPatientBillingInvoice> OutPtBillingCollection
            , double? PtInsuranceBenefit);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RecalRegistrationHIBenefit_New(long PtRegistrationID, long StaffID, out string OutputBalanceServicesXML);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddOutPtTransactionFinalization(OutPtTransactionFinalization TransactionFinalizationObj, bool IsUpdateToken, byte ViewCase, out long TransactionFinalizationSummaryInfoID, out long OutTranFinalizationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutPtTransactionFinalization RptOutPtTransactionFinalization(long aPtRegistrationID, long V_RegistrationType, long TranFinalizationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CancelConfirmHIBenefit(long PtRegistrationID, long StaffID, out string msg);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration SaveThenPayForServicesAndPCLReqs(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo,
            List<PatientRegistrationDetail> regDetailList, List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList,
            List<PatientPCLRequest> deletedPclRequestList, PatientTransactionPayment paymentDetails, PromoDiscountProgram PromoDiscountProgramObj,
            out long NewRegistrationID, out List<PatientRegistrationDetail> SavedRegistrationDetailList, out List<PatientPCLRequest> SavedPclRequestList,
            out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList,
            out V_RegistrationError SaveRegisError,
            out V_RegistrationError PayError, out string responseMsg,
			DateTime modifiedDate = default(DateTime), bool checkBeforePay = false,
            long? ConfirmHIStaffID = null,
            string OutputBalanceServicesXML = null,
            bool IsReported = false,
            bool IsUpdateHisID = false,
            long? HIID = null,
            //▼====: #005
            double? PtInsuranceBenefit = null, bool IsNotCheckInvalid = false, bool IsProcess = false
            , string TranPaymtNote = null, long? V_PaymentMode = null, long? V_ReceiveMethod = null);
            //▲====: #005

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientAccountTransaction_Insert(PatientAccountTransaction payment, out long PtAccountTranID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientAccountTransaction> PatientAccountTransaction_GetAll(long PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientAccount> PatientAccount_GetAll(long PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientAccount_Insert(long PatientID, string AccountNumber);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DoSettlementForListOutPatient(List<PatientRegistration> ListPtRegistration, long PatientID, long StaffID);

        #region Báo giá
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SaveQuotation(InPatientBillingInvoice aBillingInvoice, out long OutQuotationID, string QuotationTitle, long? PatientID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientBillingInvoice GetQuotationAllDetail(long InPatientBillingInvID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InPatientBillingInvoice> GetQuotationCollection(short ViewCase);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RemoveQuotation(long InPatientBillingInvID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CreatePatientQuotation(long InPatientBillingInvID, long PatientID, string QuotationTitle);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateQuotation(InPatientBillingInvoice aBillingInvoice, string QuotationTitle);
        #endregion

        //▼====: #006
        #region Quản lý
        [OperationContract]
        [FaultContract(typeof(AxException))]
        ObservableCollection<MedRegItemBase> GetInPatientAllBillingInvoiceSummary(long PtRegistrationID, long DeptID, DateTime FromDate, DateTime ToDate
           , bool IsPassCheckNonBlockValidPCLExamDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateInPatientBillingInvoiceByPtRegistrationID(long? StaffID, long PtRegistrationID
            , List<PatientRegistrationDetail> deletedRegDetails
            , List<PatientPCLRequestDetail> deletedPclRequestDetails
            , List<OutwardDrugClinicDept> deleteOutwardDrugClinicDepts
            , List<PatientRegistrationDetail> modifiedRegDetails
            , List<PatientPCLRequestDetail> modifiedPclRequestDetails
            , List<OutwardDrugClinicDept> modifiedOutwardDrugClinicDepts
            , bool IsNotCheckInvalid
            , out Dictionary<long, List<long>> DrugIDList_Error);
        #endregion
        //▲====: #006

        //▼====: #009
        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutPatientCashAdvance GetLastOutPatientCashAdvance(long PtRegistrationID, bool isGetLast);
        //▲====: #009
    }
}
