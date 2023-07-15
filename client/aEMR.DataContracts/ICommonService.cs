/*
20170218 #001 CMN: Add Checkbox AllDept for InPtBills
 * 20170522 #002 CMN: Added variable to check InPt 5 year HI without paid enough
 * 20170609 #003 CMN: Fix SupportFund About TT04 with TT04
 * 20170619 #004 CMN: Service for payment report OutPt with large data
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using DataEntities;
//using eHCMS.Services.Core;
using aEMR.DataContracts;

namespace CommonServiceProxy
{
    [ServiceContract]
    public interface ICommonService
    {
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginDoListUpload(List<PCLResultFileStorageDetail> lst, AsyncCallback callback, object state);
        //bool EndDoListUpload(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginDoUpload(string filename, byte[] content, bool append, string dir, AsyncCallback callback, object state);
        //bool EndDoUpload(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSaveImageCapture(byte[] byteArray, AsyncCallback callback, object state);
        //bool EndSaveImageCapture(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSaveImageCapture1(MemoryStream ms, AsyncCallback callback, object state);
        //bool EndSaveImageCapture1(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginUpdateRefApplicationConfigs(string ConfigItemKey, string ConfigItemValue,
        //                                              string ConfigItemNotes, AsyncCallback callback, object state);
        //bool EndUpdateRefApplicationConfigs(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllSuburbNames(AsyncCallback callback, object state);
        //List<SuburbNames> EndGetAllSuburbNames(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllGenders(AsyncCallback callback, object state);
        //IList<Gender> EndGetAllGenders(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllRegistrationStatus(AsyncCallback callback, object state);
        //IList<AllLookupValues.RegistrationStatus> EndGetAllRegistrationStatus(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllLookupValuesByType(LookupValues lookUpType, AsyncCallback callback, object state);
        //IList<Lookup> EndGetAllLookupValuesByType(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllLookupValuesForTransferForm(AsyncCallback callback, object state);
        //IList<Lookup> EndGetAllLookupValuesForTransferForm(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllReligion(AsyncCallback callback, object state);
        //IList<Lookup> EndGetAllReligion(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllBankName(AsyncCallback callback, object state);
        //IList<Lookup> EndGetAllBankName(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllMaritalStatus(AsyncCallback callback, object state);
        //IList<Lookup> EndGetAllMaritalStatus(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllEthnics(AsyncCallback callback, object state);
        //IList<Lookup> EndGetAllEthnics(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllFamilyRelationships(AsyncCallback callback, object state);
        //IList<Lookup> EndGetAllFamilyRelationships(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllCountries(AsyncCallback callback, object state);
        //IList<RefCountry> EndGetAllCountries(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllCurrency(bool IsActive, AsyncCallback callback, object state);
        //IList<Currency> EndGetAllCurrency(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllProvinces(AsyncCallback callback, object state);
        //IList<CitiesProvince> EndGetAllProvinces(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetStaffByID(Int64 ID, AsyncCallback callback, object state);
        //Staff EndGetStaffByID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllStaffContain(AsyncCallback callback, object state);
        //IList<Staff> EndGetAllStaffContain(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllStaffPosition(AsyncCallback callback, object state);
        //List<StaffPosition> EndGetAllStaffPosition(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllDepartments(bool bIncludeDeleted, AsyncCallback callback, object state);
        //IList<RefDepartment> EndGetAllDepartments(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllDepartmentsByV_DeptTypeOperation(long V_DeptTypeOperation, AsyncCallback callback, object state);
        //IList<RefDepartment> EndGetAllDepartmentsByV_DeptTypeOperation(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllDocumentTypeOnHold(AsyncCallback callback, object state);
        //IList<Lookup> EndGetAllDocumentTypeOnHold(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetVideoAndImage(string path, AsyncCallback callback, object state);
        //byte[] EndGetVideoAndImage(IAsyncResult asyncResult);

        ////[OperationContract(AsyncPattern = true)]
        ////[FaultContractAttribute(typeof(AxException))]
        ////IAsyncResult BeginGetAllConfigItemValues(AsyncCallback callback, object state);
        ////IList<string> EndGetAllConfigItemValues(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetServerConfiguration(AsyncCallback callback, object state);
        //AxServerConfigSection EndGetServerConfiguration(IAsyncResult asyncResult);

        ////[OperationContract(AsyncPattern = true)]
        ////[FaultContractAttribute(typeof(AxException))]
        ////IAsyncResult BeginGetConfigItemsValueBySerialNumber(int sNumber, AsyncCallback callback, object state);
        ////string EndGetConfigItemsValueBySerialNumber(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetDepartments(long locationID, AsyncCallback callback, object state);
        //IList<RefDepartment> EndGetDepartments(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCalcRegistration(PatientRegistration reg, AsyncCallback callback, object state);
        //void EndCalcRegistration(out PatientRegistration calculatedReg, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetTransactionSum(long TransactionID, AsyncCallback callback, object state);
        //List<PatientTransactionDetail> EndGetTransactionSum(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetTransactionSum_InPt(long TransactionID, AsyncCallback callback, object state);
        //List<PatientTransactionDetail> EndGetTransactionSum_InPt(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllPayments_New(long transactionID, AsyncCallback callback, object state);
        //List<PatientTransactionPayment> EndGetAllPayments_New(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch, int FindPatient, AsyncCallback callback, object state);
        //List<PatientTransactionPayment> EndGetPatientPaymentByDay_New(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff,
        //                                    IList<PatientTransactionPayment> allPayment, AsyncCallback callback, object state);
        //bool EndAddReportPaymentReceiptByStaff(out long RepPaymentRecvID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginUpdateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff
        //                                    , AsyncCallback callback, object state);
        //bool EndUpdateReportPaymentReceiptByStaff(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContract(typeof(AxException))]
        //IAsyncResult BeginGetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate, bool bFilterByStaffID, Int64 nStaffID, long loggedStaffID, AsyncCallback callback, object state);
        //List<ReportPaymentReceiptByStaff> EndGetReportPaymentReceiptByStaff(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContract(typeof(AxException))]
        //IAsyncResult BeginGetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID, AsyncCallback callback, object state);
        //ReportPaymentReceiptByStaffDetails EndGetReportPaymentReceiptByStaffDetails(IAsyncResult asyncResult);



        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginUpdatePatientRegistration(long PtRegistrationID, long V_RegistrationStatus, AsyncCallback callback, object state);
        //bool EndUpdatePatientRegistration(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSearchPtRegistration(SeachPtRegistrationCriteria criteria, int pageSize, int pageIndex, bool bcount, AsyncCallback callback, object state);
        //IList<PatientRegistration> EndSearchPtRegistration(out int Totalcount, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetMaxExamDate(AsyncCallback callback, object state);
        //DateTime EndGetMaxExamDate(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetPatientInfoByPtRegistration(long? PtRegistrationID, long? PatientID, int FindPatient, AsyncCallback callback, object state);
        //Patient EndGetPatientInfoByPtRegistration(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCalcOutwardDrugInvoice(long registrationID, OutwardDrugInvoice drugInvoice, AsyncCallback callback, object state);
        //void EndCalcOutwardDrugInvoice(out OutwardDrugInvoice CalculatedDrugInvoice, out PayableSum PayableSum, out decimal TotalPaid, IAsyncResult asyncResult);

        ////[OperationContract(AsyncPattern = true)]
        ////[FaultContractAttribute(typeof(AxException))]
        ////IAsyncResult BeginSaveInPatientRegistration(PatientRegistration registrationInfo, long billingType , AsyncCallback callback, object state);
        ////void EndSaveInPatientRegistration(out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out InPatientBillingInvoice AddedBillingInvoice, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCreateSuggestCashAdvance(long InPatientBillingInvID, long StaffID, AsyncCallback callback, object state);
        //bool EndCreateSuggestCashAdvance(out long RptPtCashAdvRemID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddInPatientBillingInvoice(int? Apply15HIPercent, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay, AsyncCallback callback, object state);
        //void EndAddInPatientBillingInvoice(out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginUpdateInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv
        //     , List<PatientRegistrationDetail> newRegDetails
        //    , List<PatientRegistrationDetail> deletedRegDetails
        //    , List<PatientPCLRequest> newPclRequests
        //    , List<PatientPCLRequestDetail> newPclRequestDetails
        //    , List<PatientPCLRequestDetail> deletedPclRequestDetails
        //    , List<OutwardDrugClinicDeptInvoice> newOutwardDrugClinicDeptInvoices
        //    , List<OutwardDrugClinicDeptInvoice> savedOutwardDrugClinicDeptInvoices
        //    , List<OutwardDrugClinicDeptInvoice> modifiedOutwardDrugClinicDeptInvoices
        //    , List<OutwardDrugClinicDeptInvoice> deleteOutwardDrugClinicDeptInvoices
        //    , List<PatientRegistrationDetail> modifiedRegDetails
        //     , List<PatientPCLRequestDetail> modifiedPclRequestDetails
        //    , AsyncCallback callback, object state);
        //void EndUpdateInPatientBillingInvoice(out Dictionary<long, List<long>> DrugIDList_Error, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginInsertAdditionalFee(long InPatientBillingInvID, long StaffID, AsyncCallback callback, object state);
        //bool EndInsertAdditionalFee(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSaveEmptyRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo, long V_RegistrationType, AsyncCallback callback, object state);
        //void EndSaveEmptyRegistration(out PatientRegistration SavedRegistration, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSaveRegistrationAndPay(PatientRegistration registrationInfo, PatientTransactionPayment paymentDetails, AsyncCallback callback, object state);
        //void EndSaveRegistrationAndPay(out PatientRegistration SavedRegistration, out PatientTransactionPayment PaymentInfo, out List<OutwardDrugInvoice> NewInvoiceList, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginPayForRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient, PatientTransactionPayment paymentDetails,
        //                                        IList<PatientRegistrationDetail> colPaidRegDetails,
        //                                        IList<PatientPCLRequest> colPaidPclRequests,
        //                                        IList<OutwardDrugInvoice> colPaidDrugInvoice,
        //                                        IList<InPatientBillingInvoice> colBillingInvoices,
        //    bool checkBeforePay,
        //     AsyncCallback callback, object state);
        //void EndPayForRegistration(out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList,
        //    out V_RegistrationError error, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginPayForInPatientRegistration(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,
        //                                        IList<InPatientBillingInvoice> billingInvoices, AsyncCallback callback, object state);
        //void EndPayForInPatientRegistration(out PatientTransaction Transaction, out PatientTransactionPayment PaymentInfo, out long PtCashAdvanceID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginFinalizeInPatientBillingInvoices(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,
        //                                        IList<InPatientBillingInvoice> billingInvoices, AsyncCallback callback, object state);
        //void EndFinalizeInPatientBillingInvoices(out PatientTransaction Transaction, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCreateForm02(InPatientRptForm02 CurrentRptForm02, IList<InPatientBillingInvoice> billingInvoices, AsyncCallback callback, object state);
        //void EndCreateForm02(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetForm02(long PtRegistrationID, AsyncCallback callback, object state);
        //IList<InPatientRptForm02> EndGetForm02(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginInPatientPayForBill(PatientRegistration registration, IList<InPatientBillingInvoice> billingInvoices, decimal payAmount, long staffID, AsyncCallback callback, object state);
        //void EndInPatientPayForBill(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginInPatientSettlement(long ptRegistrationID, long staffID, AsyncCallback callback, object state);
        //void EndInPatientSettlement(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetRegistrationInfo(long registrationID, int FindPatient, bool loadFromAppointment, AsyncCallback callback, object state);
        //PatientRegistration EndGetRegistrationInfo(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetRegistrationSimple(long registrationID, int PatientFindBy, AsyncCallback callback, object state);
        //PatientRegistration EndGetRegistrationSimple(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCreateBlankTransferFormByRegID(long PtRegistrationID, int PatientFindBy, long V_TransferFormType, AsyncCallback callback, object state);
        //TransferForm EndCreateBlankTransferFormByRegID(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetInPatientAdmissionByRegistrationID(long regID, AsyncCallback callback, object state);
        //InPatientAdmDisDetails EndGetInPatientAdmissionByRegistrationID(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetRegistrationInfo_InPt(long registrationID, int FindPatient, LoadRegistrationSwitch LoadRegisSwitch, bool loadFromAppointment, AsyncCallback callback, object state);
        //PatientRegistration EndGetRegistrationInfo_InPt(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetRegistration(long registrationID, int FindPatient, AsyncCallback callback, object state);
        //PatientRegistration EndGetRegistration(IAsyncResult asyncResult);


        ////lay ben pharmacy qua ne
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSaveDrugs(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, AsyncCallback callback, object state);
        //bool EndSaveDrugs(out OutwardDrugInvoice SavedOutwardInvoice, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCalcInvoiceForSaveAndPayButton(OutwardDrugInvoice OutwardInvoice, AsyncCallback callback, object state);
        //void EndCalcInvoiceForSaveAndPayButton(out OutwardDrugInvoice SavedOutwardInvoice, out PatientRegistration RegistrationInfo, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSavePCLRequest(long RegistrationID, PatientPCLRequest request, AsyncCallback callback, object state);
        //void EndSavePCLRequest(out long RequestID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginChangeRegistrationType(PatientRegistration regInfo, AllLookupValues.RegistrationType newType, AsyncCallback callback, object state);
        //PatientRegistration EndChangeRegistrationType(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetDate(AsyncCallback callback, object state);
        //DateTime EndGetDate(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginHospitalize(PatientRegistration regInfo, BedPatientAllocs selectedBed, DateTime? admissionDate, int ExpectedStayingDays, AsyncCallback callback, object state);
        //bool EndHospitalize(out AllLookupValues.RegistrationType NewRegType, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddServicesAndPCLRequests(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo, IList<PatientRegistrationDetail> regDetailList, IList<PatientPCLRequest> pclRequestList, IList<PatientRegistrationDetail> deletedRegDetailList, IList<PatientPCLRequest> deletedPclRequestList, DateTime modifiedDate = default(DateTime), AsyncCallback callback = null, object state = null);
        //PatientRegistration EndAddServicesAndPCLRequests(out long NewRegistrationID, out IList<PatientRegistrationDetail> SavedRegistrationDetailList, out IList<PatientPCLRequest> SavedPclRequestList, out V_RegistrationError error, IAsyncResult asyncResult);

        ////HPT 01/10/2016: Hàm mới cập nhật tên bác sỹ chỉ định cho chẩn đoán
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginUpdateDrAndDiagTrmtForPCLReq(long ServiceRecID, long PCLReqID, long DoctorStaffID, AsyncCallback callback = null, object state = null);
        //PatientPCLRequest EndUpdateDrAndDiagTrmtForPCLReq(IAsyncResult asyncResult);
        
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddPCLRequestsForInPt(long StaffID, long ReqFromDeptLocID, long ReqFromDeptID, PatientRegistration regInfo, PatientPCLRequest pclRequest, PatientPCLRequest deletedPclRequest, DateTime modifiedDate = default(DateTime), AsyncCallback callback = null, object state = null);
        //void EndAddPCLRequestsForInPt(out IList<PatientPCLRequest> SavedPclRequestList, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginUpdatePCLRequest(long StaffID, PatientPCLRequest request, DateTime modifiedDate = default(DateTime), AsyncCallback callback = null, object state = null);
        //void EndUpdatePCLRequest(out List<PatientPCLRequest> listPclSave, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginDeletePCLRequestWithDetails(Int64 PatientPCLReqID, AsyncCallback callback, object state);
        //void EndDeletePCLRequestWithDetails(out string Result, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginDeleteInPtPCLRequestWithDetails(Int64 PatientPCLReqID, AsyncCallback callback, object state);
        //void EndDeleteInPtPCLRequestWithDetails(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRemovePaidRegItems(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient,
        //                List<PatientRegistrationDetail> colPaidRegDetails,
        //                List<PatientPCLRequest> colPaidPclRequests,
        //                List<OutwardDrugInvoice> colPaidDrugInvoice,
        //                List<OutwardDrugClinicDeptInvoice> colPaidMedItemList,
        //                List<OutwardDrugClinicDeptInvoice> colPaidChemicalItem, AsyncCallback callback, object state);
        //void EndRemovePaidRegItems(out V_RegistrationError error, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddOutwardDrugReturn(long StaffID, int? Apply15HIPercent, OutwardDrugInvoice Outward, List<OutwardDrug> Details, AsyncCallback callback, object state);
        //bool EndAddOutwardDrugReturn(out long outiID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCancelOutwardDrugInvoice(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice invoice, AsyncCallback callback, object state);
        //bool EndCancelOutwardDrugInvoice(IAsyncResult asyncResult);

        ////HPT 24/08/2015: Thêm parameter IsHICard_FiveYearsCont vào hàm tính bảo hiểm để xét quyền lợi 100% cho người tham gia bảo hiểm 5 năm liên tiếp
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCalculateHiBenefit(HealthInsurance healthInsurance, PaperReferal paperReferal, bool IsEmergency, long V_RegistrationType, bool IsEmergInPtReExamination, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsAllowCrossRegion, AsyncCallback callback, object state);
        //double EndCalculateHiBenefit(out bool isConsideredAsCrossRegion, IAsyncResult asyncResult);

        ///*==== #002 ====*/
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCalculateHiBenefit_V2(HealthInsurance healthInsurance, PaperReferal paperReferal, bool IsEmergency, long V_RegistrationType, bool IsEmergInPtReExamination, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsAllowCrossRegion, bool IsHICard_FiveYearsCont_NoPaid, AsyncCallback callback, object state);
        //double EndCalculateHiBenefit_V2(out bool isConsideredAsCrossRegion, IAsyncResult asyncResult);
        ///*==== #002 ====*/

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSearchHospitals(string HospitalName, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        //List<Hospital> EndSearchHospitals(out int totalCount, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSearchHospitalsNew(HospitalSearchCriteria entity, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        //List<Hospital> EndSearchHospitalsNew(out int totalCount, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSearchHospitalByHICode(string HiCode, AsyncCallback callback, object state);
        //Hospital EndSearchHospitalByHICode(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginLoadCrossRegionHospitals(AsyncCallback callback, object state);
        //List<Hospital> EndLoadCrossRegionHospitals(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCancelRegistration(PatientRegistration registrationInfo, AsyncCallback callback, object state);
        //bool EndCancelRegistration(out PatientRegistration cancelledRegistration, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetInPatientBillingInvoiceDetails(long billingInvoiceID, AsyncCallback callback, object state);
        //void EndGetInPatientBillingInvoiceDetails(out List<PatientRegistrationDetail> regDetails
        //                                                , out List<PatientPCLRequest> PCLRequestList
        //                                                , out List<OutwardDrugClinicDeptInvoice> allInPatientInvoices, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetInPatientBillingInvoice(long billingInvoiceID, AsyncCallback callback, object state);
        //InPatientBillingInvoice EndGetInPatientBillingInvoice(IAsyncResult asyncResult);
        //#region Ny them member

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddTransactionForDrug(PatientTransactionPayment payment, long outiID, long V_TranRefType, AsyncCallback callback, object state);
        //bool EndAddTransactionForDrug(out long PaymentID, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginHospital_ByCityProvinceID(long? CityProvinceID, AsyncCallback callback, object state);
        //IList<Hospital> EndHospital_ByCityProvinceID(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRefGenDrugBHYT_Category_Get(AsyncCallback callback, object state);
        //IList<RefGenDrugBHYT_Category> EndRefGenDrugBHYT_Category_Get(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRefGenericDrugCategory_1_Get(long V_MedProductType, AsyncCallback callback, object state);
        //IList<RefGenericDrugCategory_1> EndRefGenericDrugCategory_1_Get(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRefGenericDrugCategory_2_Get(long V_MedProductType, AsyncCallback callback, object state);
        //IList<RefGenericDrugCategory_2> EndRefGenericDrugCategory_2_Get(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginLoadRefPharmacyDrugCategory(AsyncCallback callback, object state);
        //IList<RefPharmacyDrugCategory> EndLoadRefPharmacyDrugCategory(IAsyncResult asyncResult);

        //#endregion


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCreateNewRegistrationVIP(PatientRegistration regInfo, AsyncCallback callback, object state);
        //void EndCreateNewRegistrationVIP(out long newRegistrationID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginReturnInPatientDrug(long registrationID, List<RefGenMedProductSummaryInfo> returnedItems, long? DeptID, long? StaffID, AsyncCallback callback, object state);
        //bool EndReturnInPatientDrug(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddInPatientAdmission(PatientRegistration registrationInfo, InPatientDeptDetail deptDetail, long StaffID, long Staff_DeptLocationID, AsyncCallback callback, object state);
        //void EndAddInPatientAdmission(out long RegistrationID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginUpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc, AsyncCallback callback, object state);
        //void EndUpdateInPatientAdmDisDetails(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginChangeInPatientDept(InPatientDeptDetail entity, AsyncCallback callback, object state);
        //bool EndChangeInPatientDept(out long inPatientDeptDetailID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCreateBillingInvoiceFromExistingItems(PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, AsyncCallback callback, object state);
        //void EndCreateBillingInvoiceFromExistingItems(out long NewBillingInvoiceID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginLoadInPatientRegItemsIntoBill(long registrationID, long? DeptID, long StoreID, long StaffID, bool IsAdmin, AsyncCallback callback, object state);
        //InPatientBillingInvoice EndLoadInPatientRegItemsIntoBill(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCloseRegistration(long registrationID, int FindPatient, AsyncCallback callback, object state);
        //bool EndCloseRegistration(out List<string> errorMessages, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddInPatientDischarge(InPatientAdmDisDetails admissionInfo, long StaffID, AsyncCallback callback, object state);
        //bool EndAddInPatientDischarge(out List<string> errorMessages, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRevertDischarge(InPatientAdmDisDetails admissionInfo, long staffID, AsyncCallback callback, object state);
        //bool EndRevertDischarge(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCheckBeforeDischarge(long registrationID, long DischargeDeptID, bool ConfirmNotTreatedAsInPt, AsyncCallback callback, object state);
        //bool EndCheckBeforeDischarge(out string errorMessages, out string confirmMessages, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetInPatientRegistrationAndPaymentInfo(long registrationID, bool GetSumOfCashAdvBalanceOnly, AsyncCallback callback, object state);
        ////PatientRegistration EndGetInPatientRegistrationAndPaymentInfo(out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, IAsyncResult asyncResult);
        //bool EndGetInPatientRegistrationAndPaymentInfo(out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount,
        //                                                out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRecalcInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv, AsyncCallback callback, object state);
        //void EndRecalcInPatientBillingInvoice(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRegistrations_UpdateStatus(PatientRegistration regInfo, long V_RegistrationStatus, AsyncCallback callback, object state);
        //bool EndRegistrations_UpdateStatus(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllInPatientBillingInvoices(long PtRegistrationID, long? DeptID, AsyncCallback callback, object state);
        //List<InPatientBillingInvoice> EndGetAllInPatientBillingInvoices(IAsyncResult asyncResult);

        ////==== #001
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllInPatientBillingInvoicesByDeptArray(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray, AsyncCallback callback, object state);
        //List<InPatientBillingInvoice> EndGetAllInPatientBillingInvoicesByDeptArray(IAsyncResult asyncResult);
        ////==== #001

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllInPatientBillingInvoices_FromListDeptID(long PtRegistrationID, List<long> ListDeptIDs, AsyncCallback callback, object state);
        //List<InPatientBillingInvoice> EndGetAllInPatientBillingInvoices_FromListDeptID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllInPatientBillingInvoices_ForCreateForm02(long RptForm02_InPtID, long PtRegistrationID, List<long> ListDeptIDs, AsyncCallback callback, object state);
        //List<InPatientBillingInvoice> EndGetAllInPatientBillingInvoices_ForCreateForm02(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllPatientPaymentAccounts(AsyncCallback callback, object state);
        //List<PatientPaymentAccount> EndGetAllPatientPaymentAccounts(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail, long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        //bool EndThanhToanTienChoBenhNhan(out long PtTranPaymtID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRefDepartmentReqCashAdv_DeptID(long DeptID, AsyncCallback callback, object state);
        //List<RefDepartmentReqCashAdv> EndRefDepartmentReqCashAdv_DeptID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginPatientCashAdvance_Insert(PatientCashAdvance payment, AsyncCallback callback, object state);
        //bool EndPatientCashAdvance_Insert(out long PtCashAdvanceID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginPatientCashAdvance_Delete(PatientCashAdvance payment, long staffID, AsyncCallback callback, object state);
        //bool EndPatientCashAdvance_Delete(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginPatientCashAdvance_GetAll(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        //List<PatientCashAdvance> EndPatientCashAdvance_GetAll(IAsyncResult asyncResult);

        ////HPT: phiếu thu tiền khác
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGenericPayment_FullOperation(GenericPayment GenPayment, AsyncCallback callback, object state);
        //bool EndGenericPayment_FullOperation(out GenericPayment OutGenericPayment, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGenericPayment_GetAll(DateTime? FromDate, DateTime? ToDate, long? V_GenericPaymentType, long? StaffID, AsyncCallback callback, object state);
        //List<GenericPayment> EndGenericPayment_GetAll(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGenericPayment_SearchByCode(string GenPaymtCode, long StaffID, AsyncCallback callback, object state);
        //GenericPayment EndGenericPayment_SearchByCode(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetCashAdvanceBill(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        //List<PatientCashAdvance> EndGetCashAdvanceBill(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetPatientSettlement(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        //List<TransactionFinalization> EndGetPatientSettlement(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllPaymentByRegistrationID_InPt(long registrationID, AsyncCallback callback, object state);
        //PatientTransaction EndGetAllPaymentByRegistrationID_InPt(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginCloseRegistrationAll(long PtRegistrationID, int FindPatient, AsyncCallback callback, object state);
        //bool EndCloseRegistrationAll(out string Error, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRptPatientCashAdvReminder_Insert(RptPatientCashAdvReminder payment, AsyncCallback callback, object state);
        //bool EndRptPatientCashAdvReminder_Insert(out long RptPtCashAdvRemID, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRptPatientCashAdvReminder_Update(RptPatientCashAdvReminder payment, AsyncCallback callback, object state);
        //bool EndRptPatientCashAdvReminder_Update(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRptPatientCashAdvReminder_Delete(long RptPtCashAdvRemID, AsyncCallback callback, object state);
        //bool EndRptPatientCashAdvReminder_Delete(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginRptPatientCashAdvReminder_GetAll(long PtRegistrationID, AsyncCallback callback, object state);
        //List<RptPatientCashAdvReminder> EndRptPatientCashAdvReminder_GetAll(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetReportOutPatientCashReceipt(SearchOutwardReport Searchcriate, AsyncCallback callback, object state);
        //IList<ReportOutPatientCashReceipt_Payments> EndGetReportOutPatientCashReceipt(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginPatientTransactionPayment_UpdateNote(IList<PatientTransactionPayment> allPayment, AsyncCallback callback, object state);
        //bool EndPatientTransactionPayment_UpdateNote(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginPatientTransactionPayment_UpdateID(PatientTransactionPayment Payment, AsyncCallback callback, object state);
        //bool EndPatientTransactionPayment_UpdateID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginPatientTransactionPayment_Load(long TransactionID, AsyncCallback callback, object state);
        //List<PatientTransactionPayment> EndPatientTransactionPayment_Load(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetReportOutPatientCashReceipt_TongHop(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, AsyncCallback callback, object state);
        //IList<ReportOutPatientCashReceipt_Payments> EndGetReportOutPatientCashReceipt_TongHop(out List<PatientTransactionPayment> OutPaymentLst, IAsyncResult asyncResult);

        ///*▼====: #004*/
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetReportOutPatientCashReceipt_TongHop_Async(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, AsyncCallback callback, object state);
        //IList<ReportOutPatientCashReceipt_Payments> EndGetReportOutPatientCashReceipt_TongHop_Async(out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey, IAsyncResult asyncResult);
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetMoreReportOutPatientCashReceipt_TongHop_Async(int RefAsyncKey, AsyncCallback callback, object state);
        //IList<ReportOutPatientCashReceipt_Payments> EndGetMoreReportOutPatientCashReceipt_TongHop_Async(out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey, IAsyncResult asyncResult);
        ///*▲====: #004*/

        ////export excel all
        //#region Export excel from database

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginExportToExcellAll_ListRefGenericDrug(DrugSearchCriteria criteria, AsyncCallback callback, object state);
        //List<List<string>> EndExportToExcellAll_ListRefGenericDrug(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginExportToExcellAll_ListRefGenMedProductDetail(RefGenMedProductDetailsSearchCriteria criteria, AsyncCallback callback, object state);
        //List<List<string>> EndExportToExcellAll_ListRefGenMedProductDetail(IAsyncResult asyncResult);
        //#endregion

        ////HPT_20160619: About Quỹ hỗ trợ bệnh nhân sử dụng dịch vụ kỹ thuật cao
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllCharityOrganization(AsyncCallback callback, object state);
        //IList<CharityOrganization> EndGetAllCharityOrganization(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetCharitySupportFundForInPt(long PtRegistrationID, long? BillingInvID, AsyncCallback callback, object state);
        //IList<CharitySupportFund> EndGetCharitySupportFundForInPt(IAsyncResult asyncResult);

        ///*▼====: #003*/
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetCharitySupportFundForInPt_V2(long PtRegistrationID, long? BillingInvID, bool? IsHighTechServiceBill, AsyncCallback callback, object state);
        //IList<CharitySupportFund> EndGetCharitySupportFundForInPt_V2(IAsyncResult asyncResult);
        ///*▲====: #003*/

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSaveCharitySupportFundForInPt(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds, AsyncCallback callback, object state);
        //IList<CharitySupportFund> EndSaveCharitySupportFundForInPt(IAsyncResult asyncResult);

        ///*▼====: #003*/
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSaveCharitySupportFundForInPt_V2(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds, bool IsHighTechServiceBill, AsyncCallback callback, object state);
        //IList<CharitySupportFund> EndSaveCharitySupportFundForInPt_V2(IAsyncResult asyncResult);
        ///*▲====: #003*/

        ////HPT 2/07/2016: Load danh sách các tài liệu yêu cầu khi chuyển khoa
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllDocTypeRequire(AsyncCallback callback, object state);
        //IList<DeptTransferDocReq> EndGetAllDocTypeRequire(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddInPatientInstruction(PatientRegistration ptRegistration, AsyncCallback callback, object state);
        //void EndAddInPatientInstruction(out long IntPtDiagDrInstructionID, IAsyncResult asyncResult);
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetInPatientInstruction(PatientRegistration ptRegistration, AsyncCallback callback, object state);
        //InPatientInstruction EndGetInPatientInstruction(IAsyncResult asyncResult);
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetIntravenousPlan_InPt(long IntPtDiagDrInstructionID, AsyncCallback callback, object state);
        //List<Intravenous> EndGetIntravenousPlan_InPt(IAsyncResult asyncResult);
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllItemsByInstructionID(long IntPtDiagDrInstructionID, AsyncCallback callback, object state);
        //void EndGetAllItemsByInstructionID(out List<PatientRegistrationDetail> AllRegistrationItems, out List<PatientPCLRequestDetail> AllPCLRequestItems, IAsyncResult asyncResult);
    }

}