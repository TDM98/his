using System;
using System.ServiceModel;
using aEMR.DataContracts;
using DataEntities;
using System.Collections.Generic;
using eHCMS.Services.Core;
using System.IO;
using DataEntities.MedicalInstruction;

/*
 * 20181113 #001 TTM: Thêm hàm lấy dữ liệu Phường xã.
 * 20210924 #002 TNHX: 571 Thêm Quản lý điều dưỡng nhập y lệnh
 * 20230316 #003 QTD: Thêm dữ liệu 130
 * 20230601 #004 QTD: Lấy lại dữ liệu Lookup cho quản lý danh mục
 */

namespace CommonService_V2Proxy
{
    [ServiceContract]
    public interface ICommonService_V2
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllPayments_New(long transactionID, AsyncCallback callback, object state);
        List<PatientTransactionPayment> EndGetAllPayments_New(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch, int FindPatient, AsyncCallback callback, object state);
        List<PatientTransactionPayment> EndGetPatientPaymentByDay_New(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff, IList<PatientTransactionPayment> allPayment, AsyncCallback callback, object state);
        bool EndAddReportPaymentReceiptByStaff(out long RepPaymentRecvID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate, bool bFilterByStaffID, Int64 nStaffID, long loggedStaffID, AsyncCallback callback, object state);
        List<ReportPaymentReceiptByStaff> EndGetReportPaymentReceiptByStaff(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID, AsyncCallback callback, object state);
        ReportPaymentReceiptByStaffDetails EndGetReportPaymentReceiptByStaffDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff, AsyncCallback callback, object state);
        bool EndUpdateReportPaymentReceiptByStaff(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetMaxExamDate(AsyncCallback callback, object state);
        DateTime EndGetMaxExamDate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreateBlankTransferFormByRegID(long PtRegistrationID, int PatientFindBy, long V_TransferFormType, AsyncCallback callback, object state);
        TransferForm EndCreateBlankTransferFormByRegID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDate(AsyncCallback callback, object state);
        DateTime EndGetDate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateDrAndDiagTrmtForPCLReq(long ServiceRecID, long PCLReqID, long DoctorStaffID, AsyncCallback callback, object state);
        PatientPCLRequest EndUpdateDrAndDiagTrmtForPCLReq(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeletePCLRequestWithDetails(Int64 PatientPCLReqID, AsyncCallback callback, object state);
        void EndDeletePCLRequestWithDetails(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteInPtPCLRequestWithDetails(Int64 PatientPCLReqID, AsyncCallback callback, object state);
        void EndDeleteInPtPCLRequestWithDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchHospitals(string HospitalName, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<Hospital> EndSearchHospitals(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchHospitalsNew(HospitalSearchCriteria entity, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<Hospital> EndSearchHospitalsNew(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchHospitalByHICode(string HiCode, AsyncCallback callback, object state);
        Hospital EndSearchHospitalByHICode(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginLoadCrossRegionHospitals(AsyncCallback callback, object state);
        List<Hospital> EndLoadCrossRegionHospitals(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreateForm02(InPatientRptForm02 CurrentRptForm02, IList<InPatientBillingInvoice> billingInvoices, AsyncCallback callback, object state);
        void EndCreateForm02(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetForm02(long PtRegistrationID, AsyncCallback callback, object state);
        IList<InPatientRptForm02> EndGetForm02(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginInPatientSettlement(long ptRegistrationID, long staffID, AsyncCallback callback, object state);
        void EndInPatientSettlement(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllPatientPaymentAccounts(AsyncCallback callback, object state);
        List<PatientPaymentAccount> EndGetAllPatientPaymentAccounts(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRefDepartmentReqCashAdv_DeptID(long DeptID, AsyncCallback callback, object state);
        List<RefDepartmentReqCashAdv> EndRefDepartmentReqCashAdv_DeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientCashAdvance_GetAll(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        List<PatientCashAdvance> EndPatientCashAdvance_GetAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGenericPayment_FullOperation(GenericPayment GenPayment, AsyncCallback callback, object state);
        bool EndGenericPayment_FullOperation(out GenericPayment OutGenericPayment, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGenericPayment_GetAll(DateTime? FromDate, DateTime? ToDate, long? V_GenericPaymentType, long? StaffID, AsyncCallback callback, object state);
        List<GenericPayment> EndGenericPayment_GetAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGenericPayment_SearchByCode(string GenPaymtCode, long StaffID, AsyncCallback callback, object state);
        GenericPayment EndGenericPayment_SearchByCode(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientSettlement(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        List<TransactionFinalization> EndGetPatientSettlement(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllPaymentByRegistrationID_InPt(long registrationID, AsyncCallback callback, object state);
        PatientTransaction EndGetAllPaymentByRegistrationID_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRptPatientCashAdvReminder_GetAll(long PtRegistrationID, AsyncCallback callback, object state);
        List<RptPatientCashAdvReminder> EndRptPatientCashAdvReminder_GetAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetReportOutPatientCashReceipt(SearchOutwardReport Searchcriate, AsyncCallback callback, object state);
        IList<ReportOutPatientCashReceipt_Payments> EndGetReportOutPatientCashReceipt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientTransactionPayment_UpdateNote(IList<PatientTransactionPayment> allPayment, AsyncCallback callback, object state);
        bool EndPatientTransactionPayment_UpdateNote(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientTransactionPayment_UpdateID(PatientTransactionPayment Payment, AsyncCallback callback, object state);
        bool EndPatientTransactionPayment_UpdateID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientTransactionPayment_Load(long TransactionID, AsyncCallback callback, object state);
        List<PatientTransactionPayment> EndPatientTransactionPayment_Load(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetReportOutPatientCashReceipt_TongHop(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, AsyncCallback callback, object state);
        IList<ReportOutPatientCashReceipt_Payments> EndGetReportOutPatientCashReceipt_TongHop(out List<PatientTransactionPayment> OutPaymentLst, IAsyncResult asyncResult);

        #region Export excel from database
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginExportToExcellAll_ListRefGenericDrug(DrugSearchCriteria criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_ListRefGenericDrug(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginExportToExcellAll_ListRefGenMedProductDetail(RefGenMedProductDetailsSearchCriteria criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_ListRefGenMedProductDetail(IAsyncResult asyncResult);
        #endregion

        #region InPatientInstruction
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddInPatientInstruction(PatientRegistration ptRegistration, bool IsUpdateDiagConfirmInPT, long WebIntPtDiagDrInstructionID, AsyncCallback callback, object state);
        void EndAddInPatientInstruction(out long IntPtDiagDrInstructionID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientInstruction(PatientRegistration ptRegistration, AsyncCallback callback, object state);
        InPatientInstruction EndGetInPatientInstruction(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientInstructionCollection(PatientRegistration aRegistration, AsyncCallback callback, object state);
        List<InPatientInstruction> EndGetInPatientInstructionCollection(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientInstructionCollectionForCreateOutward(long PtRegistrationID, bool? IsCreatedOutward, long V_MedProductType, long StoreID, AsyncCallback callback, object state);
        IList<InPatientInstruction> EndGetInPatientInstructionCollectionForCreateOutward(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientInstructionCollectionForTransmissionMonitor(long PtRegistrationID//, bool? IsCreatedOutward, long V_MedProductType, long StoreID
            , AsyncCallback callback, object state);
        IList<InPatientInstruction> EndGetInPatientInstructionCollectionForTransmissionMonitor(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTransmissionMonitorByInPatientInstructionID(long InPatientInstructionID, AsyncCallback callback, object state);
        IList<TransmissionMonitor> EndGetTransmissionMonitorByInPatientInstructionID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveTransmissionMonitor(TransmissionMonitor CurTransmissionMonitor, long StaffID, AsyncCallback callback, object state);
        bool EndSaveTransmissionMonitor(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientInstructionByInstructionID(long aIntPtDiagDrInstructionID, AsyncCallback callback, object state);
        InPatientInstruction EndGetInPatientInstructionByInstructionID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetIntravenousPlan_InPt(long IntPtDiagDrInstructionID, AsyncCallback callback, object state);
        List<Intravenous> EndGetIntravenousPlan_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAntibioticTreatmentUsageHistory(long PtRegistrationID, AsyncCallback callback, object state);
        IList<ReqOutwardDrugClinicDeptPatient> EndGetAntibioticTreatmentUsageHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllDrugFromDoctorInstruction(long PtRegistrationID, DateTime CurrentDate, AsyncCallback callback, object state);
        IList<ReqOutwardDrugClinicDeptPatient> EndGetAllDrugFromDoctorInstruction(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllItemsByInstructionID(long IntPtDiagDrInstructionID, AsyncCallback callback, object state);
        void EndGetAllItemsByInstructionID(out List<PatientRegistrationDetail> AllRegistrationItems, out List<PatientPCLRequestDetail> AllPCLRequestItems, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveInstructionMonitoring(InPatientInstruction InPatientInstruction, long StaffID, AsyncCallback callback, object state);
        bool EndSaveInstructionMonitoring(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllLookupValuesByType(LookupValues lookUpType, AsyncCallback callback, object state);
        IList<Lookup> EndGetAllLookupValuesByType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRefGenericDrugCategory_2_Get(long V_MedProductType, AsyncCallback callback, object state);
        IList<RefGenericDrugCategory_2> EndRefGenericDrugCategory_2_Get(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllGenders(AsyncCallback callback, object state);
        IList<Gender> EndGetAllGenders(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllCountries(AsyncCallback callback, object state);
        IList<RefCountry> EndGetAllCountries(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllProvinces(AsyncCallback callback, object state);
        IList<CitiesProvince> EndGetAllProvinces(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetVideoAndImage(string path, AsyncCallback callback, object state);
        byte[] EndGetVideoAndImage(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetVideoAndImage_V2(string aPath, bool aMapPath, AsyncCallback callback, object state);
        byte[] EndGetVideoAndImage_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllDepartments(bool bIncludeDeleted, AsyncCallback callback, object state);
        IList<RefDepartment> EndGetAllDepartments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetServerConfiguration(AsyncCallback callback, object state);
        AxServerConfigSection EndGetServerConfiguration(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllSuburbNames(AsyncCallback callback, object state);
        List<SuburbNames> EndGetAllSuburbNames(IAsyncResult asyncResult);

        //▼====== #001
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllWardNames(AsyncCallback callback, object state);
        List<WardNames> EndGetAllWardNames(IAsyncResult asyncResult);
        //▲====== #001

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllStaffPosition(AsyncCallback callback, object state);
        List<StaffPosition> EndGetAllStaffPosition(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDepartments(long locationID, AsyncCallback callback, object state);
        IList<RefDepartment> EndGetDepartments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllDepartmentsByV_DeptTypeOperation(long V_DeptTypeOperation, AsyncCallback callback, object state);
        IList<RefDepartment> EndGetAllDepartmentsByV_DeptTypeOperation(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllDocumentTypeOnHold(AsyncCallback callback, object state);
        IList<Lookup> EndGetAllDocumentTypeOnHold(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginHospital_ByCityProvinceID(long? CityProvinceID, AsyncCallback callback, object state);
        IList<Hospital> EndHospital_ByCityProvinceID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRefGenericDrugCategory_1_Get(long V_MedProductType, AsyncCallback callback, object state);
        IList<RefGenericDrugCategory_1> EndRefGenericDrugCategory_1_Get(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllDocTypeRequire(AsyncCallback callback, object state);
        IList<DeptTransferDocReq> EndGetAllDocTypeRequire(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTransactionSum(long TransactionID, AsyncCallback callback, object state);
        List<PatientTransactionDetail> EndGetTransactionSum(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTransactionSum_InPt(long TransactionID, AsyncCallback callback, object state);
        List<PatientTransactionDetail> EndGetTransactionSum_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetMoreReportOutPatientCashReceipt_TongHop_Async(int RefAsyncKey, AsyncCallback callback, object state);
        IList<ReportOutPatientCashReceipt_Payments> EndGetMoreReportOutPatientCashReceipt_TongHop_Async(out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateRefApplicationConfigs(string ConfigItemKey, string ConfigItemValue, string ConfigItemNotes, AsyncCallback callback, object state);
        bool EndUpdateRefApplicationConfigs(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDoUpload(string filename, byte[] content, bool append, string dir, AsyncCallback callback, object state);
        bool EndDoUpload(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDoListUpload(List<PCLResultFileStorageDetail> lst, AsyncCallback callback, object state);
        bool EndDoListUpload(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllCurrency(bool IsActive, AsyncCallback callback, object state);
        IList<Currency> EndGetAllCurrency(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllEthnics(AsyncCallback callback, object state);
        IList<Lookup> EndGetAllEthnics(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllReligion(AsyncCallback callback, object state);
        IList<Lookup> EndGetAllReligion(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllMaritalStatus(AsyncCallback callback, object state);
        IList<Lookup> EndGetAllMaritalStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginLoadRefPharmacyDrugCategory(AsyncCallback callback, object state);
        IList<RefPharmacyDrugCategory> EndLoadRefPharmacyDrugCategory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllStaffContain(AsyncCallback callback, object state);
        IList<Staff> EndGetAllStaffContain(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllBankName(AsyncCallback callback, object state);
        IList<Lookup> EndGetAllBankName(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveImageCapture(byte[] byteArray, AsyncCallback callback, object state);
        bool EndSaveImageCapture(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveImageCapture1(MemoryStream ms, AsyncCallback callback, object state);
        bool EndSaveImageCapture1(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllLookupValuesForTransferForm(AsyncCallback callback, object state);
        IList<Lookup> EndGetAllLookupValuesForTransferForm(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDrugDeptProductGroupReportTypes(AsyncCallback callback, object state);
        IList<DrugDeptProductGroupReportType> EndGetDrugDeptProductGroupReportTypes(IAsyncResult asyncResult);

        #region Xáp nhập Nhà thuốc - khoa dược
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginExportToExcellAll_ListRefGenericDrug_New(DrugSearchCriteria criteria, AsyncCallback callback, object state);
        List<List<string>> EndExportToExcellAll_ListRefGenericDrug_New(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginLoadRefPharmacyDrugCategory_New(AsyncCallback callback, object state);
        IList<RefPharmacyDrugCategory> EndLoadRefPharmacyDrugCategory_New(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditRefMedicalServiceGroup(RefMedicalServiceGroups aRefMedicalServiceGroup, AsyncCallback callback, object state);
        long EndEditRefMedicalServiceGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRefMedicalServiceGroups(string MedicalServiceGroupCode, AsyncCallback callback, object state);
        IList<RefMedicalServiceGroups> EndGetRefMedicalServiceGroups(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRefMedicalServiceGroupItemsByID(long MedicalServiceGroupID, AsyncCallback callback, object state);
        IList<RefMedicalServiceGroupItem> EndGetRefMedicalServiceGroupItemsByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetShortHandDictionariesByStaffID(long StaffID, AsyncCallback callback, object state);
        IList<ShortHandDictionary> EndGetShortHandDictionariesByStaffID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateOrderNumberRegistration(List<string> orderNumLst, AsyncCallback callback, object state);
        bool EndUpdateOrderNumberRegistration(IAsyncResult asyncResult);

        #region Quản lý điều dưỡng thực hiện y lệnh
        //▼====: #002
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveTicketCare(TicketCare gTicketCare, long CreatedStaffID, AsyncCallback callback, object state);
        TicketCare EndSaveTicketCare(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTicketCare(long IntPtDiagDrInstructionID, AsyncCallback callback, object state);
        TicketCare EndGetTicketCare(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTicketCareListForRegistration(long PtRegistrationID, AsyncCallback callback, object state);
        List<TicketCare> EndGetTicketCareListForRegistration(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetExecuteDrugListForRegistration(long PtRegistrationID, AsyncCallback callback, object state);
        List<ExecuteDrug> EndGetExecuteDrugListForRegistration(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveExecuteDrug(long ExecuteDrugID, long ExecuteDrugDetailID, long StaffID, long CreatedStaffID, DateTime DateExecute
            , AsyncCallback callback, object state);
        bool EndSaveExecuteDrug(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteExecuteDrug(long ExecuteDrugDetailID, long CreatedStaffID, AsyncCallback callback, object state);
        bool EndDeleteExecuteDrug(IAsyncResult asyncResult);
        //▲====: #002
        #endregion

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllDiseaseProgression(bool UseInConfig, AsyncCallback callback, object state);
        IList<DiseaseProgression> EndGetAllDiseaseProgression(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]

        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllDiseaseProgressionDetails( AsyncCallback callback, object state);
        IList<DiseaseProgressionDetails> EndGetAllDiseaseProgressionDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateIsBlockBill(bool IsBlock,long PtRegistrationID, long DeptID, AsyncCallback callback, object state);
        bool EndUpdateIsBlockBill(IAsyncResult asyncResult);

        //▼==== #003
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllNationalities(AsyncCallback callback, object state);
        IList<RefNationality> EndGetAllNationalities(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllJobs(AsyncCallback callback, object state);
        List<RefJob> EndGetAllJobs(IAsyncResult asyncResult);
        //▲==== #003

        //▼==== #004
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllLookupValuesByTypeForMngt(LookupValues lookUpType, string SearchCriteria, AsyncCallback callback, object state);
        IList<Lookup> EndGetAllLookupValuesByTypeForMngt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTreeView_LookupForMngt(AsyncCallback callback, object state);
        IList<LookupTree> EndGetTreeView_LookupForMngt(IAsyncResult asyncResult);
        //▲==== #004
    }
}
