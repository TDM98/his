using System;
using System.ServiceModel;
using aEMR.DataContracts;
using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
/*
* 20191102 #001 TNHX: BM 0017411: Add new func BeginRecalcInPatientBillingInvoiceWithPriceList with MedPrice & PCLPrice
* 20220121 #002 TNHX: 848 thêm func cho màn hình tổng hợp tất cả bill
* 20220526 #003 DatTB: Thêm chức năng Xác nhận bn hoãn/ miễn tạm ứng
* 20220531 #004 DatTB: Thêm biến IsWithOutBill Hủy quyết toán bỏ qua HDDT và phân quyền
* 20220511 #005 QTD:  Thêm func get OutCashAdvanceID cuối cho màn hình xác nhận BHYT để tự in biên lai
*/
namespace BillingPaymentWcfServiceLibProxy
{
    [ServiceContract]
    public interface IBillingPaymentWcfServiceLib
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCalcOutwardDrugInvoice(long registrationID, OutwardDrugInvoice drugInvoice, AsyncCallback callback, object state);
        void EndCalcOutwardDrugInvoice(out OutwardDrugInvoice CalculatedDrugInvoice, out PayableSum PayableSum, out decimal TotalPaid, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreateSuggestCashAdvance(long InPatientBillingInvID, long StaffID, AsyncCallback callback, object state);
        bool EndCreateSuggestCashAdvance(out long RptPtCashAdvRemID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddInPatientBillingInvoice(int? Apply15HIPercent, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay, bool IsNotCheckInvalid, AsyncCallback callback, object state);
        void EndAddInPatientBillingInvoice(out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPayForRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient, PatientTransactionPayment paymentDetails,// PatientPayment paymentDetails,
            IList<PatientRegistrationDetail> colPaidRegDetails,
            IList<PatientPCLRequest> colPaidPclRequests,
            IList<OutwardDrugInvoice> colPaidDrugInvoice,
            IList<InPatientBillingInvoice> colBillingInvoices, bool checkBeforePay, AsyncCallback callback, object state);
        void EndPayForRegistration(out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList, out V_RegistrationError error, out string responseMsg, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPayForRegistration_V2(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient, PatientTransactionPayment paymentDetails,
                                                IList<PatientRegistrationDetail> colPaidRegDetails,
                                                IList<PatientPCLRequest> colPaidPclRequests,
                                                IList<OutwardDrugInvoice> colPaidDrugInvoice,
                                                IList<InPatientBillingInvoice> colBillingInvoices, PromoDiscountProgram PromoDiscountProgramObj, bool checkBeforePay, AsyncCallback callback, object state);
        void EndPayForRegistration_V2(out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList, out V_RegistrationError error, out string responseMsg, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPayForRegistration_V3(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient, PatientTransactionPayment paymentDetails
            , IList<PatientRegistrationDetail> colPaidRegDetails
            , IList<PatientPCLRequest> colPaidPclRequests
            , IList<OutwardDrugInvoice> colPaidDrugInvoice
            , IList<InPatientBillingInvoice> colBillingInvoices, PromoDiscountProgram PromoDiscountProgramObj, bool checkBeforePay, long? ConfirmHIStaffID, string OutputBalanceServicesXML
            , bool IsReported
            , bool IsUpdateHisID
            , long? HIID
            , double? PtInsuranceBenefit
            , bool IsNotCheckInvalid
            , PatientRegistration existingPtRegisInfo
            , bool IsRefundBilling, bool IsProcess
            , string TranPaymtNote, long? V_PaymentMode
            , AsyncCallback callback, object state);
        void EndPayForRegistration_V3(out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList, out V_RegistrationError error, out string responseMsg, IAsyncResult asyncResult);

		// VuTTM Start
		[OperationContract(AsyncPattern = true)]
		[FaultContractAttribute(typeof(AxException))]
		IAsyncResult BeginHasCard(long patientID, long staffID,
			AsyncCallback callback, object state);
		TransactionResponse EndHasCard(IAsyncResult asyncResult);

		[OperationContract(AsyncPattern = true)]
		[FaultContractAttribute(typeof(AxException))]
		IAsyncResult BeginMapCard(bool isHospital, long patientID, string acctNo,
			string pan, string issueDate, long staffID, AsyncCallback callback, object state);
		TransactionResponse EndMapCard(IAsyncResult asyncResult);

		[OperationContract(AsyncPattern = true)]
		[FaultContractAttribute(typeof(AxException))]
		IAsyncResult BeginUnmapCard(bool isHospital, long patientID,
			long staffID, AsyncCallback callback, object state);
		TransactionResponse EndUnmapCard(IAsyncResult asyncResult);

		[OperationContract(AsyncPattern = true)]
		[FaultContractAttribute(typeof(AxException))]
		IAsyncResult BeginDeposit(bool isHospital, long patientID, decimal settlementAmount,
			long staffID, AsyncCallback callback, object state);
		TransactionResponse EndDeposit(IAsyncResult asyncResult);
		// VuTTM End

		[OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv
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
            , bool IsNotCheckInvalid, AsyncCallback callback, object state);
        void EndUpdateInPatientBillingInvoice(out Dictionary<long, List<long>> DrugIDList_Error, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginInsertAdditionalFee(long InPatientBillingInvID, long StaffID, AsyncCallback callback, object state);
        bool EndInsertAdditionalFee(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientBillingInvoiceDetails(long billingInvoiceID
            , bool IsUsePriceByNewCer
            , bool IsRecalInSecondTime
            , bool IsPassCheckNonBlockValidPCLExamDate
            , AsyncCallback callback, object state);
        void EndGetInPatientBillingInvoiceDetails(out List<PatientRegistrationDetail> regDetails
            , out List<PatientPCLRequest> PCLRequestList
            , out List<OutwardDrugClinicDeptInvoice> allInPatientInvoices, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientBillingInvoice(long billingInvoiceID, AsyncCallback callback, object state);
        InPatientBillingInvoice EndGetInPatientBillingInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveDrugs(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, AsyncCallback callback, object state);
        bool EndSaveDrugs(out OutwardDrugInvoice SavedOutwardInvoice, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCalcInvoiceForSaveAndPayButton(OutwardDrugInvoice OutwardInvoice, AsyncCallback callback, object state);
        void EndCalcInvoiceForSaveAndPayButton(out OutwardDrugInvoice SavedOutwardInvoice, out PatientRegistration RegistrationInfo, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddServicesAndPCLRequests(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo
            , IList<PatientRegistrationDetail> regDetailList, IList<PatientPCLRequest> pclRequestList, IList<PatientRegistrationDetail> deletedRegDetailList
            , IList<PatientPCLRequest> deletedPclRequestList, bool IsNotCheckInvalid, bool IsCheckPaid, DateTime modifiedDate, bool IsProcess
            , bool? IsFromRequestDoctor, long? V_ReceiveMethod, AsyncCallback callback, object state);
        PatientRegistration EndAddServicesAndPCLRequests(out long NewRegistrationID, out IList<PatientRegistrationDetail> SavedRegistrationDetailList, out IList<PatientPCLRequest> SavedPclRequestList, out V_RegistrationError error, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddPCLRequestsForInPt(long StaffID, long ReqFromDeptLocID, long ReqFromDeptID, PatientRegistration regInfo, PatientPCLRequest pclRequest, PatientPCLRequest deletedPclRequest, bool IsNotCheckInvalid, DateTime modifiedDate, AsyncCallback callback, object state);
        void EndAddPCLRequestsForInPt(out IList<PatientPCLRequest> SavedPclRequestList, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddPCLRequest(long StaffID, long patientID, long registrationID, PatientPCLRequest pclRequest, AsyncCallback callback, object state);
        void EndAddPCLRequest(out PatientPCLRequest SavedPclRequest, IAsyncResult asyncResult);

        /// <summary>
        /// Xóa những dịch vụ đã tính tiền rồi.
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="colPaidRegDetails"></param>
        /// <param name="colPaidPclRequests"></param>
        /// <param name="colPaidDrugInvoice"></param>
        /// <param name="colPaidMedItemList"></param>
        /// <param name="colPaidChemicalItem"></param>
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRemovePaidRegItems(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient,
                                List<PatientRegistrationDetail> colPaidRegDetails,
                                List<PatientPCLRequest> colPaidPclRequests,
                                List<OutwardDrugInvoice> colPaidDrugInvoice,
                                List<OutwardDrugClinicDeptInvoice> colPaidMedItemList,
                                List<OutwardDrugClinicDeptInvoice> colPaidChemicalItem, AsyncCallback callback, object state);
        void EndRemovePaidRegItems(out V_RegistrationError error, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddOutwardDrugReturn(long StaffID, int? Apply15HIPercent, OutwardDrugInvoice Outward, List<OutwardDrug> Details, AsyncCallback callback, object state);
        bool EndAddOutwardDrugReturn(out long outiID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCancelOutwardDrugInvoice(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice invoice, AsyncCallback callback, object state);
        bool EndCancelOutwardDrugInvoice(IAsyncResult asyncResult);

        //HPT 24/08/2015: Thêm parameter IsHICard_FiveYearsCont vào hàm CalculateHiBenefit để tính quyền lợi bảo hiểm áp dụng luật bảo hiểm 5 năm liên tiếp
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCalculateHiBenefit(HealthInsurance healthInsurance, PaperReferal paperReferal, bool IsEmergency, long V_RegistrationType, bool IsEmergInPtReExamination, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsAllowCrossRegion, AsyncCallback callback, object state);
        double EndCalculateHiBenefit(out bool isConsideredAsCrossRegion, IAsyncResult asyncResult);

        /*==== #002 ====*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCalculateHiBenefit_V2(HealthInsurance healthInsurance, PaperReferal paperReferal, bool IsEmergency, long V_RegistrationType, bool IsEmergInPtReExamination, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsAllowCrossRegion, bool IsHICard_FiveYearsCont_NoPaid, AsyncCallback callback, object state);
        double EndCalculateHiBenefit_V2(out bool isConsideredAsCrossRegion, IAsyncResult asyncResult);
        /*==== #002 ====*/

        #region Ny them member
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddTransactionForDrug(PatientTransactionPayment payment, long outiID, long V_TranRefType, AsyncCallback callback, object state);
        bool EndAddTransactionForDrug(out long PaymentID, IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPayForInPatientRegistration(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,
                                                IList<InPatientBillingInvoice> billingInvoices, AsyncCallback callback, object state);
        void EndPayForInPatientRegistration(out PatientTransaction Transaction, out PatientTransactionPayment PaymentInfo, out long PtCashAdvanceID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginFinalizeInPatientBillingInvoices(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,
            IList<InPatientBillingInvoice> billingInvoices, decimal totalPaymentAmount, AsyncCallback callback, object state);
        void EndFinalizeInPatientBillingInvoices(out string msg, out PatientTransaction Transaction, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginInPatientPayForBill(PatientRegistration registration, IList<InPatientBillingInvoice> billingInvoices, decimal payAmount, long staffID, AsyncCallback callback, object state);
        void EndInPatientPayForBill(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteTransactionFinalization(string FinalizedReceiptNum, long StaffID, long V_RegistrationType, long? PtRegistrationID, bool IsWithOutBill, AsyncCallback callback, object state); //<==== #004
        string EndDeleteTransactionFinalization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteTranacsionPayment_InPt(string ReceiptNumber, long StaffID, AsyncCallback callback, object state);
        void EndDeleteTranacsionPayment_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginReturnInPatientDrug(long registrationID, List<RefGenMedProductSummaryInfo> returnedItems, long? DeptID, long? StaffID, AsyncCallback callback, object state);
        bool EndReturnInPatientDrug(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreateBillingInvoiceFromExistingItems(PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, AsyncCallback callback, object state);
        void EndCreateBillingInvoiceFromExistingItems(out long NewBillingInvoiceID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginLoadInPatientRegItemsIntoBill(long registrationID, long? DeptID, long StoreID, long StaffID
            , bool IsAdmin, DateTime? FromDate, DateTime? ToDate, int FindPatientType, int LoadBillType, DateTime? DischargeDate
            , AsyncCallback callback, object state);
        InPatientBillingInvoice EndLoadInPatientRegItemsIntoBill(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCheckForPreloadingBills(long registrationID, AsyncCallback callback, object state);
        int EndCheckForPreloadingBills(out string errorMsg, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientRegistrationAndPaymentInfo(long registrationID, bool GetSumOfCashAdvBalanceOnly, AsyncCallback callback, object state);
        bool EndGetInPatientRegistrationAndPaymentInfo(out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                    , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRecalcInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv, bool IsUsePriceByNewCer, bool IsAutoCheckCountHI, bool ReplaceMaxHIPay
            , bool ReCalBillingInv
            , bool IsNotCheckInvalid
            , bool IsPassCheckNonBlockValidPCLExamDate
            , AsyncCallback callback, object state);
        void EndRecalcInPatientBillingInvoice(IAsyncResult asyncResult);

        //▼====: #002
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRecalcInPatientBillingInvoiceWithPriceList(long? StaffID, InPatientBillingInvoice billingInv, bool IsUsePriceByNewCer, bool IsAutoCheckCountHI, bool ReplaceMaxHIPay
            , long MedServiceItemPriceListID, long PCLExamTypePriceListID, bool ReCalBillingInv, AsyncCallback callback, object state);
        void EndRecalcInPatientBillingInvoiceWithPriceList(IAsyncResult asyncResult);
        //▲====: #002

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllInPatientBillingInvoices(long PtRegistrationID, long? DeptID, long V_RegistrationType, AsyncCallback callback, object state);
        List<InPatientBillingInvoice> EndGetAllInPatientBillingInvoices(IAsyncResult asyncResult);

        //==== #001
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllInPatientBillingInvoicesByDeptArray(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray, long V_RegistrationType, AsyncCallback callback, object state);
        List<InPatientBillingInvoice> EndGetAllInPatientBillingInvoicesByDeptArray(IAsyncResult asyncResult);
        //==== #001

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllInPatientBillingInvoices_FromListDeptID(long PtRegistrationID, List<long> ListDeptIDs, AsyncCallback callback, object state);
        List<InPatientBillingInvoice> EndGetAllInPatientBillingInvoices_FromListDeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllInPatientBillingInvoices_ForCreateForm02(long RptForm02_InPtID, long PtRegistrationID, List<long> ListDeptIDs, AsyncCallback callback, object state);
        List<InPatientBillingInvoice> EndGetAllInPatientBillingInvoices_ForCreateForm02(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientCashAdvance_Insert(PatientCashAdvance payment, long patientId, AsyncCallback callback, object state);
        bool EndPatientCashAdvance_Insert(out long PtCashAdvanceID, out string msg, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientCashAdvance_Delete(PatientCashAdvance payment, long patientId, long staffID, AsyncCallback callback, object state);
        bool EndPatientCashAdvance_Delete(out string msg, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetCashAdvanceBill(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        List<PatientCashAdvance> EndGetCashAdvanceBill(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail, long PtRegistrationID, long patientId,
            long V_RegistrationType, AsyncCallback callback, object state);
        bool EndThanhToanTienChoBenhNhan(out long PtTranPaymtID, out string msg, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRptPatientCashAdvReminder_Insert(RptPatientCashAdvReminder payment, AsyncCallback callback, object state);
        bool EndRptPatientCashAdvReminder_Insert(out long RptPtCashAdvRemID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRptPatientCashAdvReminder_Update(RptPatientCashAdvReminder payment, AsyncCallback callback, object state);
        bool EndRptPatientCashAdvReminder_Update(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRptPatientCashAdvReminder_Delete(long RptPtCashAdvRemID, AsyncCallback callback, object state);
        bool EndRptPatientCashAdvReminder_Delete(IAsyncResult asyncResult);

        #region Quỹ hỗ trợ bệnh nhân sử dụng dịch vụ kỹ thuật cao
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllCharityOrganization(AsyncCallback callback, object state);
        List<CharityOrganization> EndGetAllCharityOrganization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetCharitySupportFundForInPt(long PtRegistrationID, long? BillingInvID, AsyncCallback callback, object state);
        List<CharitySupportFund> EndGetCharitySupportFundForInPt(IAsyncResult asyncResult);

        /*▼====: #003*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetCharitySupportFundForInPt_V2(long PtRegistrationID, long? BillingInvID, bool? IsHighTechServiceBill, AsyncCallback callback, object state);
        List<CharitySupportFund> EndGetCharitySupportFundForInPt_V2(IAsyncResult asyncResult);
        /*▲====: #003*/

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveCharitySupportFundForInPt(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds, AsyncCallback callback, object state);
        List<CharitySupportFund> EndSaveCharitySupportFundForInPt(IAsyncResult asyncResult);

        /*▼====: #003*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveCharitySupportFundForInPt_V2(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds, bool IsHighTechServiceBill, AsyncCallback callback, object state);
        List<CharitySupportFund> EndSaveCharitySupportFundForInPt_V2(IAsyncResult asyncResult);
        /*▲====: #003*/

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddCharityOrganization(string CharityOrgName, AsyncCallback callback, object state);
        bool EndAddCharityOrganization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditCharityOrganization(long CharityOrgID, string CharityOrgName, AsyncCallback callback, object state);
        bool EndEditCharityOrganization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteCharityOrganization(long CharityOrgID, long StaffID, AsyncCallback callback, object state);
        bool EndDeleteCharityOrganization(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRecal15PercentHIBenefit(long PtRegistrationID, long StaffID, AsyncCallback callback, object state);
        bool EndRecal15PercentHIBenefit(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRecalRegistrationHIBenefit(long PtRegistrationID, long StaffID
            , IList<InPatientBillingInvoice> OutPtBillingCollection
            , double? PtInsuranceBenefit
            , AsyncCallback callback, object state);
        bool EndRecalRegistrationHIBenefit(out string OutputBalanceServicesXML, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRecalRegistrationHIBenefit_New(long PtRegistrationID, long StaffID, AsyncCallback callback, object state);
        bool EndRecalRegistrationHIBenefit_New(out string OutputBalanceServicesXML, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginConfirmRegistrationHIBenefit(long PtRegistrationID, long? StaffID, bool IsUpdateHisID, long? aHIID, double PtInsuranceBenefit, AsyncCallback callback, object state);
        bool EndConfirmRegistrationHIBenefit(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPromoDiscountProgramByPromoDiscProgID(long PromoDiscProgID, long V_RegistrationType, AsyncCallback callback, object state);
        PromoDiscountProgram EndGetPromoDiscountProgramByPromoDiscProgID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddOutPtTransactionFinalization(OutPtTransactionFinalization TransactionFinalizationObj, bool IsUpdateToken, byte ViewCase, AsyncCallback callback, object state);
        bool EndAddOutPtTransactionFinalization(out long TransactionFinalizationSummaryInfoID, out long OutTranFinalizationID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRptOutPtTransactionFinalization(long aPtRegistrationID, long V_RegistrationType, long TranFinalizationID, AsyncCallback callback, object state);
        OutPtTransactionFinalization EndRptOutPtTransactionFinalization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCancelConfirmHIBenefit(long PtRegistrationID, long StaffID, AsyncCallback callback, object state);
        bool EndCancelConfirmHIBenefit(out string msg, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveThenPayForServicesAndPCLReqs(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo,
            List<PatientRegistrationDetail> regDetailList, List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList,
            List<PatientPCLRequest> deletedPclRequestList, PatientTransactionPayment paymentDetails, PromoDiscountProgram PromoDiscountProgramObj,
            DateTime modifiedDate,
            bool checkBeforePay,
            long? ConfirmHIStaffID,
            string OutputBalanceServicesXML,
            bool IsReported,
            bool IsUpdateHisID,
            long? HIID,
            double? PtInsuranceBenefit, bool IsNotCheckInvalid, bool IsProcess
            , string TranPaymtNote, long? V_PaymentMode, long? V_ReceiveMethod, AsyncCallback callback, object state);

        PatientRegistration EndSaveThenPayForServicesAndPCLReqs(
            out long NewRegistrationID, out List<PatientRegistrationDetail> SavedRegistrationDetailList, out List<PatientPCLRequest> SavedPclRequestList,
            out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList,
            out V_RegistrationError SaveRegisError, out V_RegistrationError PayError, out string responseMsg, IAsyncResult asyncResult);

        #region Tạm ứng ngoại trú
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientAccountTransaction_Insert(PatientAccountTransaction payment, AsyncCallback callback, object state);
        bool EndPatientAccountTransaction_Insert(out long PtAccountTranID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientAccountTransaction_GetAll(long PatientID, AsyncCallback callback, object state);
        List<PatientAccountTransaction> EndPatientAccountTransaction_GetAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientAccount_GetAll(long PatientID, AsyncCallback callback, object state);
        List<PatientAccount> EndPatientAccount_GetAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientAccount_Insert(long PatientID, string AccountNumber, AsyncCallback callback, object state);
        bool EndPatientAccount_Insert(IAsyncResult asyncResult);

        //Quyết toán ngoại trú
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDoSettlementForListOutPatient(List<PatientRegistration> ListPtRegistration, long PatientID, long StaffID, AsyncCallback callback, object state);
        bool EndDoSettlementForListOutPatient(IAsyncResult asyncResult);


        #endregion

        #region Báo giá
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveQuotation(InPatientBillingInvoice aBillingInvoice, string QuotationTitle, long? PatientID, AsyncCallback callback, object state);
        void EndSaveQuotation(out long OutQuotationID, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRemoveQuotation(long InPatientBillingInvID, AsyncCallback callback, object state);
        void EndRemoveQuotation(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetQuotationCollection(short ViewCase, AsyncCallback callback, object state);
        IList<InPatientBillingInvoice> EndGetQuotationCollection(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetQuotationAllDetail(long InPatientBillingInvID, AsyncCallback callback, object state);
        InPatientBillingInvoice EndGetQuotationAllDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreatePatientQuotation(long InPatientBillingInvID, long PatientID, string QuotationTitle, AsyncCallback callback, object state);
        void EndCreatePatientQuotation(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateQuotation(InPatientBillingInvoice aBillingInvoice, string QuotationTitle, AsyncCallback callback, object state);
        bool EndUpdateQuotation(IAsyncResult asyncResult);
        #endregion

        //▼====: #002
        #region Quản lý tất cả bill viện phí nội trú
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientAllBillingInvoiceSummary(long PtRegistrationID, long DeptID, DateTime FromDate, DateTime ToDate
           , bool IsPassCheckNonBlockValidPCLExamDate, AsyncCallback callback, object state);
        ObservableCollection<MedRegItemBase> EndGetInPatientAllBillingInvoiceSummary(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateInPatientBillingInvoiceByPtRegistrationID(long? StaffID, long PtRegistrationID
            , List<PatientRegistrationDetail> deletedRegDetails
            , List<PatientPCLRequestDetail> deletedPclRequestDetails
            , List<OutwardDrugClinicDept> deleteOutwardDrugClinicDepts
            , List<PatientRegistrationDetail> modifiedRegDetails
            , List<PatientPCLRequestDetail> modifiedPclRequestDetails
            , List<OutwardDrugClinicDept> modifiedOutwardDrugClinicDepts
            , bool IsNotCheckInvalid, AsyncCallback callback, object state);
        void EndUpdateInPatientBillingInvoiceByPtRegistrationID(out Dictionary<long, List<long>> DrugIDList_Error, IAsyncResult asyncResult);

        #endregion
        //▲====: #002

        //▼====: #003
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginConfirmPatientPostponementAdvance(InPatientAdmDisDetails AdmissionInfo, long StaffID, AsyncCallback callback, object state);
        bool EndConfirmPatientPostponementAdvance(IAsyncResult asyncResult);
        //▲====: #003
        //▼====: #005
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetLastOutPatientCashAdvance(long PtRegistrationID, bool isGetLast, AsyncCallback asyncCallback, object state);
        OutPatientCashAdvance EndGetLastOutPatientCashAdvance(IAsyncResult asyncResult);
        //▲====: #005
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute(Name = "TransactionResponse",
	Namespace = "http://schemas.datacontract.org/2004/07/BankingPaymentService.Dto.DataContract")]
	[System.SerializableAttribute()]
	public partial class TransactionResponse :
	object, System.Runtime.Serialization.IExtensibleDataObject,
	System.ComponentModel.INotifyPropertyChanged
	{

		[System.NonSerializedAttribute()]
		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

		[System.Runtime.Serialization.OptionalFieldAttribute()]
		private bool IsSucceedField;

		[System.Runtime.Serialization.OptionalFieldAttribute()]
		private string PatientCodeField;

		[System.Runtime.Serialization.OptionalFieldAttribute()]
		private long PatientIdField;

		[System.Runtime.Serialization.OptionalFieldAttribute()]
		private string ResponseCodeField;

		[System.Runtime.Serialization.OptionalFieldAttribute()]
		private string ResponseDescriptionField;

		[System.Runtime.Serialization.OptionalFieldAttribute()]
		private decimal SettlementAmountField;

		[System.Runtime.Serialization.OptionalFieldAttribute()]
		private System.Nullable<long> TransactionIdField;

		[global::System.ComponentModel.BrowsableAttribute(false)]
		public System.Runtime.Serialization.ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public bool IsSucceed
		{
			get
			{
				return this.IsSucceedField;
			}
			set
			{
				if ((this.IsSucceedField.Equals(value) != true))
				{
					this.IsSucceedField = value;
					this.RaisePropertyChanged("IsSucceed");
				}
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public string PatientCode
		{
			get
			{
				return this.PatientCodeField;
			}
			set
			{
				if ((object.ReferenceEquals(this.PatientCodeField, value) != true))
				{
					this.PatientCodeField = value;
					this.RaisePropertyChanged("PatientCode");
				}
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public long PatientId
		{
			get
			{
				return this.PatientIdField;
			}
			set
			{
				if ((this.PatientIdField.Equals(value) != true))
				{
					this.PatientIdField = value;
					this.RaisePropertyChanged("PatientId");
				}
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public string ResponseCode
		{
			get
			{
				return this.ResponseCodeField;
			}
			set
			{
				if ((object.ReferenceEquals(this.ResponseCodeField, value) != true))
				{
					this.ResponseCodeField = value;
					this.RaisePropertyChanged("ResponseCode");
				}
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public string ResponseDescription
		{
			get
			{
				return this.ResponseDescriptionField;
			}
			set
			{
				if ((object.ReferenceEquals(this.ResponseDescriptionField, value) != true))
				{
					this.ResponseDescriptionField = value;
					this.RaisePropertyChanged("ResponseDescription");
				}
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public decimal SettlementAmount
		{
			get
			{
				return this.SettlementAmountField;
			}
			set
			{
				if ((this.SettlementAmountField.Equals(value) != true))
				{
					this.SettlementAmountField = value;
					this.RaisePropertyChanged("SettlementAmount");
				}
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public System.Nullable<long> TransactionId
		{
			get
			{
				return this.TransactionIdField;
			}
			set
			{
				if ((this.TransactionIdField.Equals(value) != true))
				{
					this.TransactionIdField = value;
					this.RaisePropertyChanged("TransactionId");
				}
			}
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if ((propertyChanged != null))
			{
				propertyChanged(this,
					new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
}