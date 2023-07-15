/*
 * 20170517 #001 CMN: Thêm giá trị để cập nhật tên bệnh nhân không giới hạn với tài khoản quản trị
 * 20170522 #002 CMN: Added variable to check InPt 5 year HI without paid enough
 * 20181119 #003 TTM:   BM 0005257: Tạo mới hàm lấy dữ liệu cho Out Standing Task bệnh nhân nội trú nằm tại khoa.
 * 20190828 #004 TTM:   BM 0013230: Cảnh báo: khi bệnh nhân còn chỉ định hoặc phiếu xuất chưa tạo bill.
 * 20201211 #005 TNHX:   BM: Thêm func cập nhật phiếu chỉ định đã đẩy qua PAC
 * 20210315 #006 BaoLQ:   Task 237 Lấy dữ liệu bệnh nhân xuất viện dựa theo khoa
 * 20220530 #007 BLQ: Kiểm tra thời gian thao tác của bác sĩ
 * 20220625 #008 BLQ: 
 *  + Lấy ICD chính cuối cùng của đợt điều trị ngoại trú
 *  + Lấy danh sách ICD cấu hình cho điều trị ngoại trú
 * 20220812 #009 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
 * + Thêm hàm check toa thuốc để kiểm tra chỉnh sửa loại hình ĐTNT
 * 20220823 #010 QTD:  Cập nhật lại tình trạng cấp cứu khi xác nhận BHYT
 * 20221005 #011 BLQ: Thêm chức năng thẻ khách hàng
 * 20221114 #012 DatTB: Thêm biến để nhận biết gửi HSBA so với điều dưỡng xác nhận xuất viện ra viện.
 * 20230210 #013 BLQ: Thêm hàm check trước khi sát nhập vào nội trú
 * 20230503 #014 BLQ: Thêm hàm lưu giấy kiểm điểm tử vong 
 * 20230624 #015 DatTB: Thay đổi điều kiện gàng buộc ICD
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using DataEntities;
using eHCMS.Services.Core;
using aEMR.DataContracts;

namespace PatientServiceProxy
{
    [ServiceContract]
    public interface IPatientRegistrationService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientByID(long patientID, bool ToRegisOutPt, AsyncCallback callback, object state);
        Patient EndGetPatientByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientByID_InPt(long patientID, bool bGetOutPtRegInfo, bool ToRegisOutPt, AsyncCallback callback, object state);
        Patient EndGetPatientByID_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientByAppointment(PatientAppointment patientApp, AsyncCallback callback, object state);
        PatientRegistration EndGetPatientByAppointment(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientByIDFullInfo(long patientID, bool ToRegisOutPt, AsyncCallback callback, object state);
        Patient EndGetPatientByIDFullInfo(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientByCode(string patientCode, AsyncCallback callback, object state);
        Patient EndGetPatientByCode(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeletePatient(Patient patient, AsyncCallback callback, object state);
        bool EndDeletePatient(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeletePatientByID(long patientID, long StaffID, AsyncCallback callback, object state);
        bool EndDeletePatientByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllPatients(AsyncCallback callback, object state);
        IList<Patient> EndGetAllPatients(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatients(int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<Patient> EndGetPatients(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchPatients(PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<Patient> EndSearchPatients(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddNewPatient(Patient newPatient, AsyncCallback callback, object state);
        bool EndAddNewPatient(out Patient AddedPatient, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllClassifications(AsyncCallback callback, object state);
        IList<PatientClassification> EndGetAllClassifications(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdatePatient(Patient patient, AsyncCallback callback, object state);
        bool EndUpdatePatient(out Patient UpdatedPatient, IAsyncResult asyncResult);

        /*==== #001 ====*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdatePatientAdmin(Patient patient, bool IsAdmin, AsyncCallback callback, object state);
        bool EndUpdatePatientAdmin(out Patient UpdatedPatient, IAsyncResult asyncResult);
        /*==== #001 ====*/

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdatePatientBloodType(long PatientID, int? BloodTypeID, AsyncCallback callback, object state);
        bool EndUpdatePatientBloodType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllBloodTypes(AsyncCallback callback, object state);
        List<BloodType> EndGetAllBloodTypes(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllClassHistories(long patientID, AsyncCallback callback, object state);
        IList<PatientClassHistory> EndGetAllClassHistories(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegistrationTypes(AsyncCallback callback, object state);
        IList<RegistrationType> EndGetAllRegistrationTypes(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllLocations(long? departmentID, AsyncCallback callback, object state);
        IList<Location> EndGetAllLocations(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetLocationsByServiceID(long medServiceID, AsyncCallback callback, object state);
        IList<DeptLocation> EndGetLocationsByServiceID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllMedicalServiceTypes(AsyncCallback callback, object state);
        IList<RefMedicalServiceType> EndGetAllMedicalServiceTypes(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetMedicalServiceTypesByInOutType(List<long> RefMedicalServiceInOutOthersTypes, AsyncCallback callback, object state);
        IList<RefMedicalServiceType> EndGetMedicalServiceTypesByInOutType(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetActiveHICard(long patientID, AsyncCallback callback, object state);
        HealthInsurance EndGetActiveHICard(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetMedicalServiceItems(long? departmentID, long? serviceTypeID, AsyncCallback callback, object state);
        IList<RefMedicalServiceItem> EndGetMedicalServiceItems(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllMedicalServiceItemsByType(long? serviceTypeID, long? MedServiceItemPriceListID, long? V_RefMedicalServiceTypes, AsyncCallback callback, object state);
        IList<RefMedicalServiceItem> EndGetAllMedicalServiceItemsByType(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginAddHICard(HealthInsurance hi,  AsyncCallback callback, object state);
        //HealthInsurance EndAddHICard(out long HIID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginActivateHICard(long hiCardID, AsyncCallback callback, object state);
        void EndActivateHICard(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateHICard(HealthInsurance hi, AsyncCallback callback, object state);
        HealthInsurance EndUpdateHICard(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCalculatePaymentSummary(IList<PatientRegistrationDetail> services, InsuranceBenefit benefit, AsyncCallback callback, object state);
        AxResponse EndCalculatePaymentSummary(out PayableSum PayableSum, out IList<PatientRegistrationDetail> CalculatedServices, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetLatestClassificationAndActiveHICard(long patientID, AsyncCallback callback, object state);
        AxResponse EndGetLatestClassificationAndActiveHICard(out PatientClassification classification, out HealthInsurance activeHI, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllUnqueuedPatients(long locationID, PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        AxResponse EndGetAllUnqueuedPatients(out int totalCount, out List<PatientQueue> allUnqueueItems, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEnqueue(PatientQueue queueItem, AsyncCallback callback, object state);
        AxResponse EndEnqueue(out bool Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllQueuedPatients(long locationID, long queueType, AsyncCallback callback, object state);
        AxResponse EndGetAllQueuedPatients(out List<PatientQueue> QueueItems, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegistrations(long patientID, bool IsInPtRegistration, AsyncCallback callback, object state);
        List<PatientRegistration> EndGetAllRegistrations(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegistrationDetails_ForGoToKhamBenh(long registrationID, AsyncCallback callback, object state);
        List<PatientRegistrationDetail> EndGetAllRegistrationDetails_ForGoToKhamBenh(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegistrationDetails(long registrationID, AsyncCallback callback, object state);
        List<PatientRegistrationDetail> EndGetAllRegistrationDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegistrationDetails_ByV_ExamRegStatus(Int64 PtRegistrationID, Int64 V_ExamRegStatus, AsyncCallback callback, object state);
        List<PatientRegistrationDetail> EndGetAllRegistrationDetails_ByV_ExamRegStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCalculatePaymentForRegistration(PatientRegistration tran, AsyncCallback callback, object state);
        PatientRegistration EndCalculatePaymentForRegistration(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCalculateRealHIBenefit(HealthInsurance hi, AsyncCallback callback, object state);
        HealthInsurance EndCalculateRealHIBenefit(IAsyncResult asyncResult);

        // Txd 23/12/2014 : Commented out because NOT USED
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSaveHIItems(long patientID, ObservableCollection<HealthInsurance> allHiItems, bool ReloadPatientHI, bool IncludeDeletedItems, AsyncCallback callback, object state);
        //bool EndSaveHIItems(out ObservableCollection<HealthInsurance> PatientHIItems, out long? ActiveHICardID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllHealthInsurances(long patientID, bool IncludeDeletedItems, AsyncCallback callback, object state);
        List<HealthInsurance> EndGetAllHealthInsurances(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveReferalItems(long patientID, IList<PaperReferal> allReferalItems, bool Reload, bool IncludeDeletedItems, AsyncCallback callback, object state);
        bool EndSaveReferalItems(out List<PaperReferal> PatientReferalItems, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllPaperReferals(long HIID, bool IncludeDeletedItems, AsyncCallback callback, object state);
        List<PaperReferal> EndGetAllPaperReferals(out PaperReferal LatestUsedItem, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddPaperReferal(PaperReferal addedReferal, AsyncCallback callback, object state);
        bool EndAddPaperReferal(out PaperReferal PaperReferal, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdatePaperReferal(PaperReferal Referal, AsyncCallback callback, object state);
        bool EndUpdatePaperReferal(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeletePaperReferal(PaperReferal Referal, AsyncCallback callback, object state);
        bool EndDeletePaperReferal(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginConfirmHI(long patientID, HealthInsurance hiItem, out HealthInsurance ConfirmedHiItem, AsyncCallback callback, object state);
        bool EndConfirmHI(out HealthInsurance ConfirmedHiItem, IAsyncResult asyncResult);

        // Txd 23/12/2014 : Commented out because NOT USED
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginSaveHIItemsAndConfirm(long patientID, List<HealthInsurance> allHiItems, bool ReloadPatientHI, bool IncludeDeletedItems, AsyncCallback callback, object state);
        //bool EndSaveHIItemsAndConfirm(out HealthInsurance ConfirmedHIItem, out bool ConfirmItemValid, out List<HealthInsurance> PatientHIItems, out long? ActiveHICardID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginConfirmPaperReferal(PaperReferal referal, out PaperReferal ConfirmedPaperReferal, AsyncCallback callback, object state);
        bool EndConfirmPaperReferal(out PaperReferal ConfirmedPaperReferal, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSavePaperReferalsAndConfirm(long patientID, List<PaperReferal> allReferalItems, bool ReloadPatientHI, bool IncludeDeletedItems, AsyncCallback callback, object state);
        bool EndSavePaperReferalsAndConfirm(out PaperReferal ConfirmedPaperReferal, out bool ConfirmItemValid, out List<PaperReferal> PaperReferals, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRegisterAndPay(PatientRegistration registrationInfo, PatientTransactionPayment payment, AsyncCallback callback, object state);
        bool EndRegisterAndPay(out long PatientRegistrationID, out int SequenceNo, out long TransationID, out long PaymentID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreateNewPCLRequest(long medServiceID, AsyncCallback callback, object state);
        void EndCreateNewPCLRequest(out PatientPCLRequest NewRequest, out PatientPCLRequest ExternalRequest, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCloseRegistration(long PtRegistrationID, int FindPatient, AsyncCallback callback, object state);
        AllLookupValues.RegistrationPaymentStatus EndCloseRegistration(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegisDeptLoc(SeachPtRegistrationCriteria criteria, IList<DeptLocation> lstDeptLocation, AsyncCallback callback, object state);
        List<DeptLocInfo> EndGetAllRegisDeptLoc(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegisDeptLocS(IList<DeptLocation> lstDeptLocation, AsyncCallback callback, object state);
        List<DeptLocInfo> EndGetAllRegisDeptLocS(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegisDeptLoc_ForXML(IList<DeptLocation> lstDeptLocation, AsyncCallback callback, object state);
        List<DeptLocInfo> EndGetAllRegisDeptLoc_ForXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegisDeptLocStaff(IList<DeptLocation> lstDeptLocation, int timeSegment, AsyncCallback callback, object state);
        IList<DeptLocInfo> EndGetAllRegisDeptLocStaff(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetAllRegisDetail(IList<DeptLocInfo> lstDeptLocation, AsyncCallback callback, object state);
        IList<PatientRegistrationDetail> EndGetAllRegisDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSumRegistrations(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize,
                                              bool bCountTotal, AsyncCallback callback, object state);
        List<PatientRegistration> EndGetSumRegistrations(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientRegistrationByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        PatientRegistration EndGetPatientRegistrationByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCheckRegistrationStatus(long PtRegistrationID, AsyncCallback callback, object state);
        long EndCheckRegistrationStatus(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrations(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchRegistrations(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationsInPt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, bool bCalledFromSearchInPtPopup, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchRegistrationsInPt(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationsForDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<PatientRegistrationDetail> EndSearchRegistrationsForDiag(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationsForProcess(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<PatientRegistrationDetail> EndSearchRegistrationsForProcess(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationsForDiagConfirm(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<PatientRegistrationDetail> EndSearchRegistrationsForDiagConfirm(out int totalCount, out ObservableCollection<Prescription> PrescriptionCollectionForConfirm, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegisPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchRegisPrescription(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegisDetailPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<PatientRegistrationDetail> EndSearchRegisDetailPrescription(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationListForOST(long DeptID, long DeptLocID, long StaffID, long ExamRegStatus, long V_OutPtEntStatus, AsyncCallback callback, object state);
        IList<PatientRegistrationDetail> EndSearchRegistrationListForOST(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientQueue_GetListPaging(PatientQueueSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<PatientQueue> EndPatientQueue_GetListPaging(out int totalCount, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchMedDrugs(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefGenMedDrugDetails> EndSearchMedDrugs(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchMedProducts(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndSearchMedProducts(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchInwardDrugClinicDept(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugClinicDept> EndSearchInwardDrugClinicDept(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllInwardDrugClinicDeptByIDList(List<long> inwardDrugIdList, AsyncCallback callback, object state);
        IList<InwardDrugClinicDept> EndGetAllInwardDrugClinicDeptByIDList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetGenMedProductsRemainingInStore(Dictionary<long, List<long>> drugIdList, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetGenMedProductsRemainingInStore(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchMedServices(MedicalServiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefMedicalServiceItem> EndSearchMedServices(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchPCLItems(PCLItemSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount, AsyncCallback callback, object state);
        IList<PCLItem> EndSearchPCLItems(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetStaffsHaveRegistrations(byte Type, AsyncCallback callback, object state);
        IList<Staff> EndGetStaffsHaveRegistrations(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetStaffsHavePayments(long V_TradingPlaces, AsyncCallback callback, object state);
        IList<Staff> EndGetStaffsHavePayments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateHiItem(HealthInsurance hiItem, long StaffID, bool IsEditAfterRegistration, string Reason, AsyncCallback callback, object state);
        bool EndUpdateHiItem(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddHiItem(HealthInsurance hiItem, long StaffID, AsyncCallback callback, object state);
        bool EndAddHiItem(out long HIID, IAsyncResult asyncResult);

        //#region Dị Ứng/Cảnh Báo
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetAllergiesString_ByPatientID(Int64 PatientID, AsyncCallback callback, object state);
        //void EndGetAllergiesString_ByPatientID(out string Result, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginGetWarningString_ByPatientID(Int64 PatientID, AsyncCallback callback, object state);
        //void EndGetWarningString_ByPatientID(out string Result, IAsyncResult asyncResult);
        //#endregion


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationSummaryList(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<RegistrationSummaryInfo> EndSearchRegistrationSummaryList(out int totalCount, out RegistrationsTotalSummary totalSummaryInfo, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientRegistrationNonFinalizedLiabilities(long registrationID, AsyncCallback callback, object state);
        bool EndGetInPatientRegistrationNonFinalizedLiabilities(out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRefGenMedProductSummaryByRegistration(long registrationID, AllLookupValues.MedProductType medProductType, long? DeptID, AsyncCallback callback, object state);
        List<RefGenMedProductSummaryInfo> EndGetRefGenMedProductSummaryByRegistration(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllBedPatientRegDetailsByBedPatientID(long BedPatientId, bool IncludeDeletedItems, AsyncCallback callback, object state);
        List<BedPatientRegDetail> EndGetAllBedPatientRegDetailsByBedPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPCLExamTypesByMedServiceID(long medServiceID, AsyncCallback callback, object state);
        List<PCLExamType> EndGetPCLExamTypesByMedServiceID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDeptLocationsByExamType(long examTypeId, AsyncCallback callback, object state);
        List<DeptLocation> EndGetDeptLocationsByExamType(IAsyncResult asyncResult);

        // TxD 28/01/2018: Commented OUT the following because It has NEVER been USED 
        //[OperationContract(AsyncPattern = true)]
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginConfirmHIBenefit(long staffID, long patientID, long hiid, float? benefit, AsyncCallback callback, object state);
        //bool EndConfirmHIBenefit(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRegistraionVIPByPatientID(long PatientID, AsyncCallback callback, object state);
        PatientRegistration EndGetRegistraionVIPByPatientID(IAsyncResult asyncResult);

        //HPT 24/08/2015: Thêm parameter IsHICard_FiveYearsCont vào hàm xác nhận bảo hiểm để xét quyền lợi 100% cho người tham gia bảo hiểm 5 năm liên tiếp
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginApplyHiToInPatientRegistration(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient, bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, TypesOf_ConfirmHICardForInPt eConfirmType, AsyncCallback callback, object state);
        bool EndApplyHiToInPatientRegistration(IAsyncResult asyncResult);

        /*==== #002 ====*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginApplyHiToInPatientRegistration_V2(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient, bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsHICard_FiveYearsCont_NoPaid, TypesOf_ConfirmHICardForInPt eConfirmType, AsyncCallback callback, object state);
        bool EndApplyHiToInPatientRegistration_V2(IAsyncResult asyncResult);
        /*==== #002 ====*/

        //▼====: #003
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginApplyHiToInPatientRegistration_V3(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient
            , bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsHICard_FiveYearsCont_NoPaid
            , DateTime? FiveYearsAppliedDate, DateTime? FiveYearsARowDate, TypesOf_ConfirmHICardForInPt eConfirmType, bool IsAllowCrossRegion, AsyncCallback callback, object state);
        bool EndApplyHiToInPatientRegistration_V3(IAsyncResult asyncResult);
        //▲====: #003

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRemoveHiFromInPatientRegistration(long RegistrationID, bool bIsEmergency, long RemoveHiStaffID, long? OldPaperReferalID, AsyncCallback callback, object state);
        bool EndRemoveHiFromInPatientRegistration(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginInPatientAdmDisDetailsSearch(InPatientAdmDisDetailSearchCriteria p, AsyncCallback callback, object state);
        IList<InPatientAdmDisDetails> EndInPatientAdmDisDetailsSearch(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginInPatientAdmDisDetailsSearchPaging(InPatientAdmDisDetailSearchCriteria p,
                                                                        int pageIndex, int pageSize, bool bCountTotal
                                                                        , AsyncCallback callback, object state);
        IList<InPatientAdmDisDetails> EndInPatientAdmDisDetailsSearchPaging(out int totalCount, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddHospital(Hospital entity, AsyncCallback callback, object state);
        bool EndAddHospital(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateHospital(Hospital entity, AsyncCallback callback, object state);
        bool EndUpdateHospital(IAsyncResult asyncResult);


        #region StaffDeptResponsibility
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInPatientTransferDeptReq(InPatientTransferDeptReq p, AsyncCallback callback, object state);
        List<InPatientTransferDeptReq> EndGetInPatientTransferDeptReq(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertInPatientTransferDeptReq(InPatientTransferDeptReq p, AsyncCallback callback, object state);
        bool EndInsertInPatientTransferDeptReq(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInPatientTransferDeptReq(InPatientTransferDeptReq p, AsyncCallback callback, object state);
        bool EndUpdateInPatientTransferDeptReq(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteInPatientTransferDeptReq(long InPatientTransferDeptReqID, AsyncCallback callback, object state);
        bool EndDeleteInPatientTransferDeptReq(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInPatientTransferDeptReqXML(IList<InPatientTransferDeptReq> lstInPtTransDeptReq,
                                                            AsyncCallback callback, object state);
        bool EndDeleteInPatientTransferDeptReqXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInPatientDeptDetailsTranfer(InPatientDeptDetail p, long InPatientTransferDeptReqID
            , bool IsAutoCreateOutDeptDiagnosis
            , AsyncCallback callback, object state);
        bool EndInPatientDeptDetailsTranfer(out long InPatientDeptDetailID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDeleteInPatientDeptDetails(InPatientDeptDetail inDeptDetailToDelete, InPatientDeptDetail inDeptDetailToUpdate,
                                                            AsyncCallback callback, object state);
        bool EndUpdateDeleteInPatientDeptDetails(out InPatientDeptDetail inDeptDetailUpdated, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutDepartment(InPatientDeptDetail InPtDeptDetail, AsyncCallback callback, object state);
        bool EndOutDepartment(out InPatientDeptDetail InPtDeptDetail, IAsyncResult asyncResult);

        #endregion

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPtRegDetailNewByPatientID(long PatientID, AsyncCallback callback, object state);
        PatientRegistrationDetail EndGetPtRegDetailNewByPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginPatientServiceRecordsFromPatientID(long PatientID, AsyncCallback callback, object state);
        List<PatientServiceRecord> EndPatientServiceRecordsFromPatientID(IAsyncResult asyncResult);


        #region can lam sang ngoai vien

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddPCLRequestExtWithDetails(PatientPCLRequest_Ext request, AsyncCallback callback, object state);
        PatientPCLRequest_Ext EndAddPCLRequestExtWithDetails(out long PatientPCLReqExtID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginPCLRequestExtUpdate(PatientPCLRequest_Ext request, AsyncCallback callback, object state);
        bool EndPCLRequestExtUpdate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeletePCLRequestDetailExtList(List<PatientPCLRequestDetail_Ext> requestDetailList, AsyncCallback callback, object state);
        bool EndDeletePCLRequestDetailExtList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetPCLRequestListExtByRegistrationID(long RegistrationID, AsyncCallback callback, object state);
        List<PatientPCLRequest_Ext> EndGetPCLRequestListExtByRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginPatientPCLRequestExtByID(long PatientPCLReqExtID, AsyncCallback callback, object state);
        PatientPCLRequest_Ext EndPatientPCLRequestExtByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetPCLRequestExtPK(long PatientPCLReqExtID, AsyncCallback callback, object state);
        PatientPCLRequest_Ext EndGetPCLRequestExtPK(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginPatientPCLRequestExtPaging(PatientPCLRequestExtSearchCriteria SearchCriteria,
                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<PatientPCLRequest_Ext> EndPatientPCLRequestExtPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddNewPCLRequestDetailsExt(PatientPCLRequest_Ext request, AsyncCallback callback, object state);
        bool EndAddNewPCLRequestDetailsExt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeletePCLRequestExt(long PatientPCLReqExtID, AsyncCallback callback, object state);
        bool EndDeletePCLRequestExt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetPCLRequestDetailListExtByRegistrationID(long RegistrationID, AsyncCallback callback, object state);
        List<PatientPCLRequestDetail_Ext> EndGetPCLRequestDetailListExtByRegistrationID(IAsyncResult asyncResult);

        #endregion


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdatePclRequestInfo(PatientPCLRequest p, AsyncCallback callback, object state);
        bool EndUpdatePclRequestInfo(IAsyncResult asyncResult);

        /*TMA*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCloseRegistrationAll(long PtRegistrationID, int FindPatient, AsyncCallback callback, object state);
        bool EndCloseRegistrationAll(out string Error, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCancelRegistration(PatientRegistration registrationInfo, AsyncCallback callback, object state);
        bool EndCancelRegistration(out PatientRegistration cancelledRegistration, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRegistrationInfo(long registrationID, int FindPatient, bool loadFromAppointment, bool IsProcess, AsyncCallback callback, object state);
        PatientRegistration EndGetRegistrationInfo(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRegistrationInfo_InPt(long registrationID, int FindPatient, LoadRegistrationSwitch LoadRegisSwitch, bool loadFromAppointment, AsyncCallback callback, object state);
        PatientRegistration EndGetRegistrationInfo_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientInfoByPtRegistration(long? PtRegistrationID, long? PatientID, int FindPatient, AsyncCallback callback, object state);
        Patient EndGetPatientInfoByPtRegistration(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveEmptyRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo, long V_RegistrationType, AsyncCallback callback, object state);
        void EndSaveEmptyRegistration(out PatientRegistration SavedRegistration, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveEmptyRegistration_V2(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo, long V_RegistrationType, IList<DiagnosisIcd10Items> Icd10Items, bool IsHospitalizationProposal, AsyncCallback callback, object state);
        void EndSaveEmptyRegistration_V2(out PatientRegistration SavedRegistration, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateDateStarted_ForPatientRegistrationDetails(long PtRegDetailID, AsyncCallback callback, object state);
        void EndUpdateDateStarted_ForPatientRegistrationDetails(out DateTime? DateStart, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddInPatientAdmission(PatientRegistration registrationInfo, InPatientDeptDetail deptDetail, long StaffID, long Staff_DeptLocationID, AsyncCallback callback, object state);
        void EndAddInPatientAdmission(out long RegistrationID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc, AsyncCallback callback, object state);
        void EndUpdateInPatientAdmDisDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInPatientAdmissionByRegistrationID(long regID, AsyncCallback callback, object state);
        InPatientAdmDisDetails EndGetInPatientAdmissionByRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRegistration(long registrationID, int FindPatient, AsyncCallback callback, object state);
        PatientRegistration EndGetRegistration(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRegistrationSimple(long registrationID, int PatientFindBy, AsyncCallback callback, object state);
        PatientRegistration EndGetRegistrationSimple(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginChangeRegistrationType(PatientRegistration regInfo, AllLookupValues.RegistrationType newType, AsyncCallback callback, object state);
        PatientRegistration EndChangeRegistrationType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginHospitalize(PatientRegistration regInfo, BedPatientAllocs selectedBed, DateTime? admissionDate, int ExpectedStayingDays, AsyncCallback callback, object state);
        bool EndHospitalize(out AllLookupValues.RegistrationType NewRegType, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreateNewRegistrationVIP(PatientRegistration regInfo, AsyncCallback callback, object state);
        void EndCreateNewRegistrationVIP(out long newRegistrationID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddInPatientDischarge(InPatientAdmDisDetails admissionInfo, long StaffID, long? ConfirmedDTItemID, AsyncCallback callback, object state);
        bool EndAddInPatientDischarge(out List<string> errorMessages, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRevertDischarge(InPatientAdmDisDetails admissionInfo, long staffID, AsyncCallback callback, object state);
        bool EndRevertDischarge(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCheckBeforeDischarge(long registrationID, long DischargeDeptID, bool ConfirmNotTreatedAsInPt
            , DateTime? DischargeDate
            //▼==== #012
            , bool IsSendingCheckMedicalFiles
            //▲==== #012
            , AsyncCallback callback, object state);
        bool EndCheckBeforeDischarge(out string errorMessages, out string confirmMessages, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginChangeInPatientDept(InPatientDeptDetail entity, AsyncCallback callback, object state);
        bool EndChangeInPatientDept(out long inPatientDeptDetailID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRegistrations_UpdateStatus(PatientRegistration regInfo, long V_RegistrationStatus, AsyncCallback callback, object state);
        bool EndRegistrations_UpdateStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginSearchPtRegistration(SeachPtRegistrationCriteria criteria, int pageSize, int pageIndex, bool bcount, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchPtRegistration(out int Totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdatePatientRegistration(long PtRegistrationID, long V_RegistrationStatus, AsyncCallback callback, object state);
        bool EndUpdatePatientRegistration(IAsyncResult asyncResult);

        // 20180816 TNHX: New Patient-HI-PaperReferral TiepNhan-DangKy
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddPatientAndHIDetails(Patient newPatient, HealthInsurance newHICard, PaperReferal newPaperReferal, AsyncCallback callback, object state);
        bool EndAddPatientAndHIDetails(out long PatientID, out string PatientCode, out string PatientBarCode, out long HIID, out PaperReferal PaperReferal, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]

        IAsyncResult BeginAddConfirmPatientAndHIDetails(Patient newPatientAndHiDetails, bool IsEmergency, long V_RegistrationType, bool _isEmergInPtReExamination,
            bool _isHICard_FiveYearsCont, bool _isChildUnder6YearsOld, bool _isAllowCrossRegion, bool IsHICard_FiveYearsCont_NoPaid, AsyncCallback callback, object state);
        bool EndAddConfirmPatientAndHIDetails(out long PatientID, out string PatientCode, out string PatientBarCode, out HealthInsurance newHICard, out PaperReferal newPaperReferal, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdatePatientAndHIDetails(Patient patient, bool IsAdmin, long StaffID, bool IsEditAfterRegistration,
            AsyncCallback callback, object state);
        bool EndUpdatePatientAndHIDetails(out Patient UpdatedPatient, out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddConfirmNewPatientAndHIDetails(Patient newPatientAndHiDetails, bool IsEmergency, long V_RegistrationType,
                                                          bool isEmergInPtReExamination, bool isHICard_FiveYearsCont, bool _isChildUnder6YearsOld,
                                                          bool _isAllowCrossRegion, bool IsHICard_FiveYearsCont_NoPaid,
                                                          AsyncCallback callback, object state);

        bool EndAddConfirmNewPatientAndHIDetails(out long PatientID, out string PatientCode, out long HIID, out string HIPCode, out int? IBID, out long PaperReferralID,
                                                 out double RebatePercentage, out double CalculatedHiBenefit, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateConfirmPatientAndHIDetails(Patient updatedPatientAndHiDetails, bool IsAdmin, long StaffID, bool IsEditAfterRegistration, bool IsEmergency, long V_RegistrationType, bool _isEmergInPtReExamination,
            bool isHICard_FiveYearsCont, bool isChildUnder6YearsOld, bool _isAllowCrossRegion, bool IsHICard_FiveYearsCont_NoPaid, AsyncCallback callback, object state);
        bool EndUpdateConfirmPatientAndHIDetails(out long PatientID, out string PatientCode, out long HIID, out string HIPCode, out int? IBID, out long PaperReferralID,
                                                 out double RebatePercentage, out double CalculatedHiBenefit, out string Result, IAsyncResult asyncResult);

        //▼====== #003
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchInPatientRegistrationListForOST(long DeptID, bool IsSearchForListPatientCashAdvance, AsyncCallback callback, object state);
        IList<PatientRegistrationDetail> EndSearchInPatientRegistrationListForOST(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchInPatientRequestAdmissionListForOST(DateTime? FromDate, DateTime? ToDate, AsyncCallback callback, object state);
        IList<PatientRegistrationDetail> EndSearchInPatientRequestAdmissionListForOST(IAsyncResult asyncResult);
        //▲====== #003

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateBasicDiagTreatment(PatientRegistration regInfo, DiseasesReference aAdmissionICD10, Staff gSelectedDoctorStaff, AsyncCallback callback, object state);
        bool EndUpdateBasicDiagTreatment(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditPromoDiscountProgram(PromoDiscountProgram aPromoDiscountProgram, long PtRegistrationID, AsyncCallback callback, object state);
        PromoDiscountProgram EndEditPromoDiscountProgram(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSetEkipForService(PatientRegistration CurrentRegistration, AsyncCallback callback, object state);
        bool EndSetEkipForService(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationListForCheckDiagAndPrescription(CheckDiagAndPrescriptionSearchCriteria SearchCriteria, AsyncCallback callback, object state);
        List<PatientRegistration> EndSearchRegistrationListForCheckDiagAndPrescription(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationForCirculars56(DateTime FromDate, DateTime ToDate, long PatientFindBy,string PatientCode, AsyncCallback callback, object state);
        List<PatientRegistration> EndSearchRegistrationForCirculars56(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSummaryMedicalRecords_ByPtRegID(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        SummaryMedicalRecords EndGetSummaryMedicalRecords_ByPtRegID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientTreatmentCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        PatientTreatmentCertificates EndGetPatientTreatmentCertificates_ByPtRegID(out int LastCode,IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInjuryCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType,  AsyncCallback callback, object state);
        InjuryCertificates EndGetInjuryCertificates_ByPtRegID(out int LastCode, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetBirthCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        List<BirthCertificates> EndGetBirthCertificates_ByPtRegID(out int LastCode, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetVacationInsuranceCertificates_ByPtRegID(long PtRegistrationID,bool IsPrenatal, AsyncCallback callback, object state);
        VacationInsuranceCertificates EndGetVacationInsuranceCertificates_ByPtRegID(out int LastCode,IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveSummaryMedicalRecords(SummaryMedicalRecords CurrentSummaryMedicalRecords, long UserID, string Date, AsyncCallback callback, object state);
        SummaryMedicalRecords EndSaveSummaryMedicalRecords(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSavePatientTreatmentCertificates(PatientTreatmentCertificates CurrentPatientTreatmentCertificates, long UserID, string Date, AsyncCallback callback, object state);
        PatientTreatmentCertificates EndSavePatientTreatmentCertificates(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveInjuryCertificates(InjuryCertificates CurrentInjuryCertificates, long UserID, string Date, AsyncCallback callback, object state);
        InjuryCertificates EndSaveInjuryCertificates(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveBirthCertificates(BirthCertificates CurrentBirthCertificates, long UserID, string Date, AsyncCallback callback, object state);
        bool EndSaveBirthCertificates(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveVacationInsuranceCertificates(VacationInsuranceCertificates CurrentVacationInsuranceCertificates,bool IsPrenatal, long UserID, string Date, AsyncCallback callback, object state);
        VacationInsuranceCertificates EndSaveVacationInsuranceCertificates(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSummaryPCLResultByPtRegistrationID(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        string EndGetSummaryPCLResultByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPCLResultForInjuryCertificatesByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        string EndGetPCLResultForInjuryCertificatesByPtRegistrationID(out string PCLResultFromAdmissionExamination,IAsyncResult asyncResult);

        //▼===== #006
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationListForCheckMedicalFiles(CheckMedicalFilesSearchCriteria SearchCriteria
            , AsyncCallback callback, object state);
        List<PatientRegistration> EndSearchRegistrationListForCheckMedicalFiles(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientPCLReq(long PtRegistrationID, AsyncCallback callback, object state);
        bool EndGetPatientPCLReq(out List<PatientPCLRequestDetail> curPatientPCLRequestDetail, out bool bPCLOK, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetMedicalFilesByPtRegistrationID(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        CheckMedicalFiles EndGetMedicalFilesByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveMedicalFiles(CheckMedicalFiles CheckMedicalFile, bool Is_KHTH, long V_RegistrationType, long StaffID, AsyncCallback callback, object state);
        bool EndSaveMedicalFiles(out long CheckMedicalFileIDNew, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientRegistrationDetails(long PtRegistrationID, AsyncCallback callback, object state);
        bool EndGetPatientRegistrationDetails(out List<PatientRegistrationDetail> curPatientRegistrationDetail, out bool bPCLOK, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetListCheckMedicalFilesByPtID(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        List<CheckMedicalFiles> EndGetListCheckMedicalFilesByPtID(IAsyncResult asyncResult);
        //▲===== #006

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveNutritionalRating(long PtRegistrationID, NutritionalRating curNutritionalRating, long StaffID, DateTime Date, AsyncCallback callback, object state);
        bool EndSaveNutritionalRating(out long NutritionalRatingIDNew, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetNutritionalRatingByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        List<NutritionalRating> EndGetNutritionalRatingByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteNutritionalRating(long NutritionalRatingID, long StaffID, DateTime Date, AsyncCallback callback, object state);
        bool EndDeleteNutritionalRating(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPatientPrescriptionAndPCLReq(long PtRegistrationID, long DoctorID, AsyncCallback callback, object state);
        bool EndGetPatientPrescriptionAndPCLReq(out List<PrescriptionDetail> curPrescriptionDetail, out List<PatientPCLRequestDetail> curPatientPCLRequestDetail, out bool bPreOK, out bool bPCLOK, IAsyncResult asyncResult);

        //▼===== #004
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCheckBeforeChangeDept(long registrationID, long DeptID, AsyncCallback callback, object state);
        bool EndCheckBeforeChangeDept(out string errorMessages, out string confirmMessages, IAsyncResult asyncResult);
        //▲===== #004

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCheckOldConsultationPatient(long PatientID, AsyncCallback callback, object state);
        Prescription EndCheckOldConsultationPatient(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddNewListPatient(List<APIPatient> ListPatient, string ContractNo, AsyncCallback callback, object state);
        List<Patient> EndAddNewListPatient(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginLoadDeptAdmissionRequest(long PtRegistrationID, AsyncCallback callback, object state);
        RefDepartment EndLoadDeptAdmissionRequest(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInfectionCaseByPtRegID(long PtRegistrationID, AsyncCallback callback, object state);
        IList<InfectionCase> EndGetInfectionCaseByPtRegID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInfectionCaseByPtID(long PatientID, AsyncCallback callback, object state);
        IList<InfectionCase> EndGetInfectionCaseByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInfectionCaseDetail(InfectionCase aInfectionCase, AsyncCallback callback, object state);
        InfectionCase EndGetInfectionCaseDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllInfectionVirus(AsyncCallback callback, object state);
        IList<InfectionVirus> EndGetAllInfectionVirus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditInfectionCase(InfectionCase aInfectionCase, AsyncCallback callback, object state);
        long EndEditInfectionCase(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditAntibioticTreatment(AntibioticTreatment aAntibioticTreatment, AsyncCallback callback, object state);
        void EndEditAntibioticTreatment(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAntibioticTreatmentMedProductDetails(long AntibioticTreatmentID, long V_AntibioticTreatmentType, AsyncCallback callback, object state);
        IList<AntibioticTreatmentMedProductDetail> EndGetAntibioticTreatmentMedProductDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDrugsInUseOfAntibioticTreatment(AntibioticTreatment aAntibioticTreatment, long DeptID, long PtRegistrationID, AsyncCallback callback, object state);
        IList<OutwardDrugClinicDept> EndGetDrugsInUseOfAntibioticTreatment(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInfectionCaseAllContentInfo(DateTime StartDate, DateTime EndDate, long V_InfectionCaseStatus, string DrugName, AsyncCallback callback, object state);
        IList<InfectionCase> EndGetInfectionCaseAllContentInfo(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllContentInfoOfInfectionCaseCollection(DateTime StartDate, DateTime EndDate, long V_InfectionCaseStatus, string DrugName, AsyncCallback callback, object state);
        IList<AntibioticTreatmentMedProductDetailSummaryContent> EndGetAllContentInfoOfInfectionCaseCollection(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAntibioticTreatmentsByPtRegID(long PtRegistrationID, AsyncCallback callback, object state);
        IList<AntibioticTreatment> EndGetAntibioticTreatmentsByPtRegID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginInsertAntibioticTreatmentIntoInstruction(long AntibioticTreatmentID, long InfectionCaseID, AsyncCallback callback, object state);
        void EndInsertAntibioticTreatmentIntoInstruction(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationAndGetPrescription(SeachPtRegistrationCriteria regCriteria, int regPageIndex, int regPageSize, bool bregCountTotal,
                            PrescriptionSearchCriteria presCriteria, int presPageIndex, int presPageSize, bool bpresCountTotal, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchRegistrationAndGetPrescription(out int presTotalCount, out IList<Prescription> lstPrescriptions, out int regTotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistrationsForMedicalExaminationDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<PatientRegistrationDetail> EndSearchRegistrationsForMedicalExaminationDiag(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginMergerPatientRegistration(PatientRegistration PtRegistration, long StaffID, AsyncCallback callback, object state);
        void EndMergerPatientRegistration(IAsyncResult asyncResult);

        #region
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRegistrationForSettlement(long PatientID, AsyncCallback callback, object state);
        IList<PatientRegistration> EndGetAllRegistrationForSettlement(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRegistration_ByServiceID(long MedServiceID, DateTime? FromDate, DateTime? ToDate, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchRegistration_ByServiceID(out List<DiagnosisTreatment> DiagnosisList, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetResourcesForMedicalServices(AsyncCallback callback, object state);
        IList<Resources> EndGetResourcesForMedicalServices(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetMedicalServiceItemByHIRepResourceCode(string HIRepResourceCode, AsyncCallback callback, object state);
        IList<RefMedicalServiceItem> EndGetMedicalServiceItemByHIRepResourceCode(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]

        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveSmallProcedureAutomatic(ObservableCollection<SmallProcedure> SmallProcedureList, AsyncCallback callback, object state);
        bool EndSaveSmallProcedureAutomatic(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginConfirmEmergencyOutPtByPtRegistrionID(long PtRegistrationID, long StaffID, long V_RegistrationType, AsyncCallback callback, object state);
        bool EndConfirmEmergencyOutPtByPtRegistrionID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginConfirmSpecialRegistrationByPtRegistrationID(PatientRegistration PatientRegistration, long StaffID, AsyncCallback callback, object state);
        bool EndConfirmSpecialRegistrationByPtRegistrationID(IAsyncResult asyncResult);

        //▼====: #005
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdatePclRequestTransferToPAC(ObservableCollection<PatientPCLRequest> PatientPCLRequestList, long V_PCLRequestType, AsyncCallback callback, object state);
        bool EndUpdatePclRequestTransferToPAC(IAsyncResult asyncResult);
        //▲====: #005
        //▼====: #006
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetListMedServiceInSurgeryDept(long PtRegistrationID, AsyncCallback callback, object state);
        List<PatientRegistrationDetail> EndGetListMedServiceInSurgeryDept(IAsyncResult asyncResult);
        //▲====: #006
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCreateRequestDrugInwardClinicDept_ByPCLRequest(PatientPCLRequest PatientPCLRequest, AsyncCallback callback, object state);
        bool EndCreateRequestDrugInwardClinicDept_ByPCLRequest(out long ReqDrugInClinicDeptID, IAsyncResult asyncResult);
        //▼====: #007
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddUpdateDoctorContactPatientTime(DoctorContactPatientTime doctorContactPatientTime, AsyncCallback callback, object state);
        bool EndAddUpdateDoctorContactPatientTime(IAsyncResult asyncResult);
        //▲====: #007
        //▼====: #008
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetLastIDC10MainTreatmentProgram_ByPtRegID(long PtRegistrationID, long PtRegDetailID, AsyncCallback callback, object state);
        bool EndGetLastIDC10MainTreatmentProgram_ByPtRegID(out long LastOutpatientTreatmentTypeID, IAsyncResult asyncResult); //▼==== #015
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllOutpatientTreatmentType(AsyncCallback callback, object state);
        List<OutpatientTreatmentType> EndGetAllOutpatientTreatmentType( IAsyncResult asyncResult);
        //▲====: #008
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRefundInPatientCost(PatientRegistration PtRegistration, long StaffID, AsyncCallback callback, object state);
        bool EndRefundInPatientCost(IAsyncResult asyncResult);
        //▼==== #009
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPrescriptionIssueHistory_ByPtRegDetailID(long PtRegDetailID, long V_RegistrationType, AsyncCallback callback, object state);
        List<PrescriptionIssueHistory> EndGetPrescriptionIssueHistory_ByPtRegDetailID(IAsyncResult asyncResult);
        //▲==== #009
        //▼====: #010
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateIsConfirmedEmergencyPatient(long PtRegistrationID, bool IsConfirmedEmergencyPatient, AsyncCallback callback, object state);
        bool EndUpdateIsConfirmedEmergencyPatient(IAsyncResult asyncResult);
        //▲====: #010

        //▼====: #011
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetCardDetailByPatientID(long PatientID, AsyncCallback callback, object state);
        PatientCardDetail EndGetCardDetailByPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveCardDetail(PatientCardDetail Obj, bool IsExtendCard, AsyncCallback callback, object state);
        bool EndSaveCardDetail(IAsyncResult asyncResult);
        //▲====: #011
        //▼==== #013
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCheckBeforeMergerPatientRegistration(long registrationID, AsyncCallback callback, object state);
        bool EndCheckBeforeMergerPatientRegistration(out string errorMessages, out string confirmMessages, IAsyncResult asyncResult);
        //▲==== #013
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDeathCheckRecordByPtRegID(long PtRegistrationID, AsyncCallback callback, object state);
        DeathCheckRecord EndGetDeathCheckRecordByPtRegID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveDeathCheckRecord(DeathCheckRecord CurDeathCheckRecord, AsyncCallback callback, object state);
        bool EndSaveDeathCheckRecord(IAsyncResult asyncResult);
    }
}