/*
 * 20170517 #001 CMN:   Thêm giá trị để cập nhật tên bệnh nhân không giới hạn với tài khoản quản trị
 * 20170522 #002 CMN:   Added variable to check InPt 5 year HI without paid enough
 * 20180814 #003 TTM:   Tạo mới method để thực hiện thêm mới và cập nhật bệnh nhân, thẻ BHYT, giấy CV 1 lần.
 * 20181119 #004 TTM:   BM 0005257: Tạo hàm lấy dữ liệu bệnh nhân đang nằm tại khoa.
 * 20190623 #005 TTM:   BM 0011797: Lấy dữ liệu đăng ký của bệnh nhân theo bác sĩ và ngày. Đọc dữ liệu toa thuốc và CLS 
 * 20190831 #006 TTM:   BM 0013214: Thêm chức năng Kiểm tra Ngày thuốc
 * 20190908 #007 TTM:   BM 0013139: [Khám sức khoẻ] Import Bệnh nhân vào chương trình 
 * 20191119 #008 TTM:   BM 0019591: Thêm 1 danh sách BN chờ nhập viện bên OutstandingTask
 * 20200807 #009 TNHX: Them Cau hinh xac nhan BN cap cuu ngoai tru
 * 20201211 #010 TNHX: Them func cap nhat da day dữ liệu qua PAC
 * 20210315 #011 BAOLQ: Lấy dữ liệu bệnh nhân xuất viện dựa vào khoa phòng
 * 20211203 #012 TNHX: Lấy dsach PTTT từ phòng mổ
 * 20220530 #013 BLQ: Kiểm tra thời gian thao tác của bác sĩ
 * 20220625 #014 BLQ: 
 *  + Lấy ICD chính cuối cùng của đợt điều trị ngoại trú
 *  + Lấy danh sách ICD cấu hình cho điều trị ngoại trú
 * 20220812 #015 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
 * + Thêm hàm check toa thuốc để kiểm tra chỉnh sửa loại hình ĐTNT
 * 20220823 #016 QTD:  Cập nhật lại tình trạng cấp cứu khi xác nhận BHYT
 * 20221005 #017 BLQ: Thêm chức năng thẻ khách hàng
 * 20221114 #018 DatTB: Thêm biến để nhận biết gửi HSBA so với điều dưỡng xác nhận xuất viện ra viện.
 * 20230210 #019 BLQ: Thêm hàm check trước khi sát nhập
 * 20230624 #020 DatTB: Thay đổi điều kiện gàng buộc ICD
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using ErrorLibrary;
using System.Data.Common;
using System.Collections.ObjectModel;

namespace PatientRegistrationService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IPatientRegistrationService
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Patient GetPatientByID(long patientID, bool ToRegisOutPt);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Patient GetPatientByID_InPt(long patientID, bool bGetOutPtRegInfo, bool ToRegisOutPt);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Patient> GetPatientAll();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetPatientByAppointment(PatientAppointment patientApp);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> GetPCLExamTypesByMedServiceID(long medServiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllDeptLocForServicesByInOutType(List<long> RefMedicalServiceInOutOthersTypes);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Patient GetPatientByIDFullInfo(long patientID, bool ToRegisOutPt);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Patient GetPatientByCode(string patientCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePatient(Patient patient, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePatientByID(long patientID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Patient> GetAllPatients();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Patient> GetPatients(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Patient> SearchPatients(PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewPatient(Patient newPatient, out Patient AddedPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientClassification> GetAllClassifications();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePatient(Patient patient, out Patient UpdatedPatient);

        /*==== #001 ====*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePatientAdmin(Patient patient, bool IsAdmin, out Patient UpdatedPatient);
        /*==== #001 ====*/

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePatientBloodType(long PatientID, int? BloodTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BloodType> GetAllBloodTypes();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientClassHistory> GetAllClassHistories(long patientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RegistrationType> GetAllRegistrationTypes();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Location> GetAllLocations(long? departmentID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DeptLocation> GetLocationsByServiceID(long medServiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceType> GetAllMedicalServiceTypes();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceType> GetMedicalServiceTypesByInOutType(List<long> RefMedicalServiceInOutOthersTypes);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        HealthInsurance GetActiveHICard(long patientID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceItem> GetMedicalServiceItems(long? departmentID, long? serviceTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceItem> GetAllMedicalServiceItemsByType(long? serviceTypeID, long? MedServiceItemPriceListID, long? V_RefMedicalServiceTypes);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //HealthInsurance AddHICard(HealthInsurance hi, out long HIID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void ActivateHICard(long hiCardID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        HealthInsurance UpdateHICard(HealthInsurance hi);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AxResponse CalculatePaymentSummary(IList<PatientRegistrationDetail> services, InsuranceBenefit benefit, out PayableSum PayableSum, out IList<PatientRegistrationDetail> CalculatedServices);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AxResponse GetLatestClassificationAndActiveHICard(long patientID, out PatientClassification classification, out HealthInsurance activeHI);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AxResponse GetAllUnqueuedPatients(long locationID, PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount, out List<PatientQueue> allUnqueueItems);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AxResponse Enqueue(PatientQueue queueItem, out bool Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AxResponse GetAllQueuedPatients(long locationID, long queueType, out List<PatientQueue> QueueItems);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistration> GetAllRegistrations(long patientID, bool IsInPtRegistration);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> GetAllRegistrationDetails_ForGoToKhamBenh(long registrationID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> GetAllRegistrationDetails(long registrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> GetAllRegistrationDetails_ByV_ExamRegStatus(Int64 PtRegistrationID, Int64 V_ExamRegStatus);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration CalculatePaymentForRegistration(PatientRegistration tran);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        HealthInsurance CalculateRealHIBenefit(HealthInsurance hi);


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool SaveHIItems(long patientID, List<HealthInsurance> allHiItems, out List<HealthInsurance> PatientHIItems, out long? ActiveHICardID, bool ReloadPatientHI = false, bool IncludeDeletedItems = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<HealthInsurance> GetAllHealthInsurances(long patientID, bool IncludeDeletedItems = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveReferalItems(long patientID, List<PaperReferal> allReferalItems, out List<PaperReferal> PatientReferalItems, bool Reload = false, bool IncludeDeletedItems = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PaperReferal> GetAllPaperReferals(long HIID, out PaperReferal LatestUsedItem, bool IncludeDeletedItems = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPaperReferal(PaperReferal addedReferal, out PaperReferal PaperReferal);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePaperReferal(PaperReferal Referal);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePaperReferal(PaperReferal Referal);

        /// <summary>
        /// Xác nhận hợp lệ một thẻ bảo hiểm.
        /// Tương úng với thao tác nhân viên xác nhận đã kiểm tra thẻ.
        /// </summary>
        /// <param name="hiItem">Thẻ cần xác nhận</param>
        /// <param name="ConfirmedHiItem">Thẻ đã được xác nhận</param>
        /// <returns>Trả về kết quả thẻ có hợp lệ hay không.</returns>
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ConfirmHI(long patientID, HealthInsurance hiItem, out HealthInsurance ConfirmedHiItem);

        /// <summary>
        /// Lưu tất cả các thay đổi trên danh sách thẻ bảo hiểm.
        /// Xác nhận hợp lệ (kiểm tra thẻ bằng mắt thường) thẻ đang active.
        /// </summary>
        /// <param name="patientID">Mã số bệnh nhân</param>
        /// <param name="allHiItems">Danh sách các thẻ bảo hiểm đã thay đổi (thêm, xóa, sửa)</param>
        /// <param name="ConfirmedHIItem">Thẻ bảo hiểm đã được xác nhận.</param>
        /// <param name="ConfirmItemValid">Thẻ bảo hiểm cần confirm có valid hay không.</param>
        /// <returns>Trả về true nếu update danh sách OK.</returns>
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool SaveHIItemsAndConfirm(long patientID, List<HealthInsurance> allHiItems, out HealthInsurance ConfirmedHIItem, out bool ConfirmItemValid, out List<HealthInsurance> PatientHIItems, out long? ActiveHICardID, bool ReloadPatientHI = false, bool IncludeDeletedItems = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ConfirmPaperReferal(PaperReferal referal, out PaperReferal ConfirmedPaperReferal);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SavePaperReferalsAndConfirm(long patientID, List<PaperReferal> allReferalItems, out PaperReferal ConfirmedPaperReferal, out bool ConfirmItemValid, out List<PaperReferal> PaperReferals, bool ReloadPatientHI = false, bool IncludeDeletedItems = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RegisterAndPay(PatientRegistration registrationInfo, PatientTransactionPayment payment, out long PatientRegistrationID, out int SequenceNo, out long TransationID, out long PaymentID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CreateNewPCLRequest(long medServiceID, out PatientPCLRequest NewRequest, out PatientPCLRequest ExternalRequest);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void SaveRegistration(PatientRegistration registrationInfo, out long PatientRegistrationID, out int SequenceNo, out PatientRegistration SavedRegistration);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void SaveRegistrationAndProcessPayment(PatientRegistration registrationInfo, PatientPayment payment, out long PatientRegistrationID, out int SequenceNo
        //    , out long TransactionID, out long PaymentID, out PatientRegistration SavedRegistration);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void ProcessPayment(PatientPayment payment, long registrationID, out long PaymentID, out PatientPayment ProcessedPayment, out List<PatientPayment> AllPayments, bool ReloadPayments = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AllLookupValues.RegistrationPaymentStatus CloseRegistration(long PtRegistrationID, int FindPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> GetPatientRegistrationDetailsByDay(long DeptLocID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> GetPatientRegistrationDetailsByDay_ForXML(IList<DeptLocation> lstDeptLoc);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> GetPtRegisDetailsByConsultTimeSegment(long DeptLocID
                                                                                            , long ConsultationTimeSegmentID
                                                                                            , long StartSequenceNumber
                                                                                            , long EndSequenceNumber);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DeptLocInfo> GetAllRegisDeptLoc(SeachPtRegistrationCriteria criteria, IList<DeptLocation> lstDeptLocation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistrationDetail> GetAllRegisDetail(IList<DeptLocInfo> lstDeptLocation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DeptLocInfo> GetAllRegisDeptLocS(IList<DeptLocation> lstDeptLocation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DeptLocInfo> GetAllRegisDeptLoc_ForXML(IList<DeptLocation> lstDeptLocation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DeptLocInfo> GetAllRegisDeptLocStaff(IList<DeptLocation> lstDeptLocation, int timeSegment);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> GetSumRegistrations(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize,
                                                       bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RegistrationSummaryInfo> SearchRegistrationSummaryList(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount, out RegistrationsTotalSummary totalSummaryInfo);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetPatientRegistrationByPtRegistrationID(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        long CheckRegistrationStatus(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegistrations(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegistrationsInPt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, bool bCalledFromSearchInPtPopup, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegisPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistrationDetail> SearchRegisDetailPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistrationDetail> SearchRegistrationListForOST(long DeptID, long DeptLocID, long StaffID, long ExamRegStatus, long V_OutPtEntStatus);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegistrationsDiagTrmt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> SearchRegistrationsForDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> SearchRegistrationsForProcess(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> SearchRegistrationsForMedicalExaminationDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        #region Outstanding Task
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientQueue> PatientQueue_GetListPaging(PatientQueueSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        #endregion


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedDrugDetails> SearchMedDrugs(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> SearchMedProducts(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugClinicDept> SearchInwardDrugClinicDept(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByIDList(List<long> inwardDrugIdList);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetGenMedProductsRemainingInStore(Dictionary<long, List<long>> drugIdList);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceItem> SearchMedServices(MedicalServiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLItem> SearchPCLItems(PCLItemSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Staff> GetStaffsHaveRegistrations(byte Type);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Staff> GetStaffsHavePayments(long V_TradingPlaces);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateHiItem(HealthInsurance hiItem, long StaffID, bool IsEditAfterRegistration, string Reason, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddHiItem(HealthInsurance hiItem, long StaffID, out long HIID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddHospitalByHiCode(string hospitalName, string hiCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddHospital(Hospital entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateHospital(Hospital entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GetInPatientRegistrationNonFinalizedLiabilities(long registrationID, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient
                                                            , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized);
        /// <summary>
        /// Lấy danh sách thuốc (hay y cụ, hóa chất tùy theo loại) đã sử dụng của một đăng ký nội trú
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="medProductType"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductSummaryInfo> GetRefGenMedProductSummaryByRegistration(long registrationID, AllLookupValues.MedProductType medProductType, long? DeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BedPatientRegDetail> GetAllBedPatientRegDetailsByBedPatientID(long BedPatientId, bool IncludeDeletedItems);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetDeptLocationsByExamType(long examTypeId);

        // TxD 28/01/2018: Commented OUT the following because It has NEVER been USED 
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool ConfirmHIBenefit(long staffID, long patientID, long hiid, float? benefit);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetRegistraionVIPByPatientID(long PatientID);

        //HPT 24/08/2015: Thêm parameter cho hàm xác nhận bảo hiểm (ApplyHiToInPatientRegistration - nội trú) để tính quyền lợi thẻ bảo hiểm khi xác nhận có tính đến điều kiện người tham gia bảo hiểm 5 năm liên tiếp
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ApplyHiToInPatientRegistration(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient, bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, TypesOf_ConfirmHICardForInPt eConfirmType);

        /*==== #002 ====*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ApplyHiToInPatientRegistration_V2(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient, bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsHICard_FiveYearsCont_NoPaid, TypesOf_ConfirmHICardForInPt eConfirmType);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ApplyHiToInPatientRegistration_V3(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient
            , bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsHICard_FiveYearsCont_NoPaid
            , DateTime? FiveYearsAppliedDate = null, DateTime? FiveYearsARowDate = null, TypesOf_ConfirmHICardForInPt eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmNew
            , bool IsAllowCrossRegion = false);
        /*==== #002 ====*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RemoveHiFromInPatientRegistration(long RegistrationID, bool bIsEmergency, long RemoveHiStaffID, long? OldPaperReferalID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InPatientAdmDisDetails> InPatientAdmDisDetailsSearch(InPatientAdmDisDetailSearchCriteria p);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientAdmDisDetails> InPatientAdmDisDetailsSearchPaging(InPatientAdmDisDetailSearchCriteria p, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePclRequestInfo(PatientPCLRequest p);

        #region StaffDeptResponsibility
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientTransferDeptReq> GetInPatientTransferDeptReq(InPatientTransferDeptReq p);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertInPatientTransferDeptReq(InPatientTransferDeptReq p);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateInPatientTransferDeptReq(InPatientTransferDeptReq p);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInPatientTransferDeptReq(long InPatientTransferDeptReqID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInPatientTransferDeptReqXML(IList<InPatientTransferDeptReq> lstInPtTransDeptReq);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InPatientDeptDetailsTranfer(InPatientDeptDetail p, long InPatientTransferDeptReqID
            , bool IsAutoCreateOutDeptDiagnosis
            , out long InPatientDeptDetailID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDeleteInPatientDeptDetails(InPatientDeptDetail inDeptDetailToDelete, InPatientDeptDetail inDeptDetailToUpdate,
                out InPatientDeptDetail inDeptDetailUpdated);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutDepartment(InPatientDeptDetail InPtDeptDetail);

        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistrationDetail GetPtRegDetailNewByPatientID(long PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientServiceRecord> PatientServiceRecordsFromPatientID(long PatientID);

        #region can lam sang ngoai vien

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLRequest_Ext AddPCLRequestExtWithDetails(PatientPCLRequest_Ext request, out long PatientPCLReqExtID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLRequestExtUpdate(PatientPCLRequest_Ext request);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePCLRequestDetailExtList(List<PatientPCLRequestDetail_Ext> requestDetailList);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientPCLRequest_Ext> GetPCLRequestListExtByRegistrationID(long RegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLRequest_Ext GetPCLRequestExtPK(long PatientPCLReqExtID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLRequest_Ext PatientPCLRequestExtByID(long PatientPCLReqExtID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientPCLRequest_Ext> PatientPCLRequestExtPaging(PatientPCLRequestExtSearchCriteria SearchCriteria,
                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewPCLRequestDetailsExt(PatientPCLRequest_Ext request);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePCLRequestExt(long PatientPCLReqExtID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientPCLRequestDetail_Ext> GetPCLRequestDetailListExtByRegistrationID(long RegistrationID);

        #endregion


        #region Txd_New The following methods were moved from the Good Old Common Service

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
        // CMN: Checked: CloseRegistration not working anywhere in eHCMSCal then remove one of this
        /*
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CloseRegistration(long registrationID, int FindPatient, out List<string> errorMessages);
        */
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CloseRegistrationAll(long PtRegistrationID, int FindPatient, out string Error);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CancelRegistration(PatientRegistration registrationInfo, out PatientRegistration cancelledRegistration);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetRegistrationInfo(long registrationID, int FindPatient, bool loadFromAppointment = false, bool IsProcess = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetRegistrationInfo_InPt(long registrationID, int FindPatient, LoadRegistrationSwitch LoadRegisSwitch, bool loadFromAppointment = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Patient GetPatientInfoByPtRegistration(long? PtRegistrationID, long? PatientID, int FindPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SaveEmptyRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo, long V_RegistrationType, out PatientRegistration SavedRegistration);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SaveEmptyRegistration_V2(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo, long V_RegistrationType, out PatientRegistration SavedRegistration, IList<DiagnosisIcd10Items> Icd10Items, bool IsHospitalizationProposal);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateDateStarted_ForPatientRegistrationDetails(long PtRegDetailID,out DateTime? DateStart);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddInPatientAdmission(PatientRegistration registrationInfo, InPatientDeptDetail deptDetail, long StaffID, long Staff_DeptLocationID, out long RegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientAdmDisDetails GetInPatientAdmissionByRegistrationID(long regID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetRegistration(long registrationID, int FindPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration GetRegistrationSimple(long registrationID, int PatientFindBy);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientRegistration ChangeRegistrationType(PatientRegistration regInfo, AllLookupValues.RegistrationType newType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Hospitalize(PatientRegistration regInfo, BedPatientAllocs selectedBed, DateTime? admissionDate, int ExpectedStayingDays, out AllLookupValues.RegistrationType NewRegType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CreateNewRegistrationVIP(PatientRegistration regInfo, out long newRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddInPatientDischarge(InPatientAdmDisDetails admissionInfo, long StaffID, long? ConfirmedDTItemID, out List<string> errorMessages);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RevertDischarge(InPatientAdmDisDetails admissionInfo, long staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckBeforeDischarge(long registrationID, long DischargeDeptID, bool ConfirmNotTreatedAsInPt
            , DateTime? DischargeDate
            //▼==== #018
            , bool IsSendingCheckMedicalFiles
            //▲==== #018
            , out string errorMessages, out string confirmMessages);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ChangeInPatientDept(InPatientDeptDetail entity, out long inPatientDeptDetailID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Registrations_UpdateStatus(PatientRegistration regInfo, long V_RegistrationStatus);

        //▼====== #004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistrationDetail> SearchInPatientRegistrationListForOST(long DeptID, bool IsSearchForListPatientCashAdvance);
        //▲====== #004
        //▼===== #008
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistrationDetail> SearchInPatientRequestAdmissionListForOST(DateTime? FromDate, DateTime? ToDate);
        //▲===== #008
        #endregion
        //▼===== #003
        #region Thông tin hành chính, BHYT, CV
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPatientAndHIDetails(Patient newPatient, HealthInsurance newHICard, PaperReferal newPaperReferal, out long PatientID, out string PatientCode, out string PatientBarCode, out long HIID, out PaperReferal PaperReferal);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePatientAndHIDetails(Patient patient, bool IsAdmin, long StaffID, bool IsEditAfterRegistration, out Patient UpdatedPatient, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddConfirmNewPatientAndHIDetails(Patient newPatientAndHiDetails, bool IsEmergency, long V_RegistrationType, bool _isEmergInPtReExamination,
            bool _isHICard_FiveYearsCont, bool _isChildUnder6YearsOld, bool _isAllowCrossRegion, bool IsHICard_FiveYearsCont_NoPaid,
            out long PatientID, out string PatientCode, out long HIID, out string HIPCode, out int? IBID, out long PaperReferralID,
                                                 out double RebatePercentage, out double CalculatedHiBenefit);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateConfirmPatientAndHIDetails(Patient updatedPatientAndHiDetails, bool IsAdmin, long StaffID, bool IsEditAfterRegistration,
            bool IsEmergency, long V_RegistrationType, bool _isEmergInPtReExamination, bool _isHICard_FiveYearsCont, bool _isChildUnder6YearsOld, bool _isAllowCrossRegion,
            bool IsHICard_FiveYearsCont_NoPaid, out long PatientID, out string PatientCode, out long HIID, out string HIPCode, out int? IBID, out long PaperReferralID,
                                                 out double RebatePercentage, out double CalculatedHiBenefit, out string Result);


        //▲===== #003
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateBasicDiagTreatment(PatientRegistration regInfo, DiseasesReference aAdmissionICD10, Staff gSelectedDoctorStaff);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PromoDiscountProgram EditPromoDiscountProgram(PromoDiscountProgram aPromoDiscountProgram, long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SetEkipForService(PatientRegistration CurrentRegistration);

        //▼===== #005
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistration> SearchRegistrationListForCheckDiagAndPrescription(CheckDiagAndPrescriptionSearchCriteria SearchCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistration> SearchRegistrationForCirculars56(DateTime FromDate, DateTime ToDate, long PatientFindBy,string PatientCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SummaryMedicalRecords GetSummaryMedicalRecords_ByPtRegID(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientTreatmentCertificates GetPatientTreatmentCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType, out int LastCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InjuryCertificates GetInjuryCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType, out int LastCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BirthCertificates> GetBirthCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType, out int LastCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        VacationInsuranceCertificates GetVacationInsuranceCertificates_ByPtRegID(long PtRegistrationID, bool IsPrenatal, out int LastCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SummaryMedicalRecords SaveSummaryMedicalRecords(SummaryMedicalRecords CurrentSummaryMedicalRecords,long UserID,string Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientTreatmentCertificates SavePatientTreatmentCertificates(PatientTreatmentCertificates CurrentPatientTreatmentCertificates, long UserID, string Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InjuryCertificates SaveInjuryCertificates(InjuryCertificates CurrentInjuryCertificates, long UserID, string Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveBirthCertificates(BirthCertificates CurrentBirthCertificates, long UserID, string Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        VacationInsuranceCertificates SaveVacationInsuranceCertificates(VacationInsuranceCertificates CurrentVacationInsuranceCertificates,bool IsPrenatal, long UserID, string Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        string GetSummaryPCLResultByPtRegistrationID(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        string GetPCLResultForInjuryCertificatesByPtRegistrationID(long PtRegistrationID, out string PCLResultFromAdmissionExamination);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GetPatientPrescriptionAndPCLReq(long PtRegistrationID, long DoctorID, out List<PrescriptionDetail> curPrescriptionDetail, out List<PatientPCLRequestDetail> curPatientPCLRequestDetail, out bool bPreOK, out bool bPCLOK);
        //▲===== #005 

        //▼===== #011
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistration> SearchRegistrationListForCheckMedicalFiles(CheckMedicalFilesSearchCriteria SearchCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GetPatientPCLReq(long PtRegistrationID, out List<PatientPCLRequestDetail> curPatientPCLRequestDetail, out bool bPCLOK);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GetPatientRegistrationDetails(long PtRegistrationID, out List<PatientRegistrationDetail> curPatientRegistrationDetail, out bool bPCLOK);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        CheckMedicalFiles GetMedicalFilesByPtRegistrationID(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveMedicalFiles(CheckMedicalFiles CheckMedicalFile, bool Is_KHTH, long V_RegistrationType, long StaffID, out long CheckMedicalFileIDNew);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<CheckMedicalFiles> GetListCheckMedicalFilesByPtID(long PtRegistrationID, long V_RegistrationType);
        //▲===== #011

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveNutritionalRating(long PtRegistrationID, NutritionalRating curNutritionalRating, long StaffID, DateTime Date, out long NutritionalRatingIDNew);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<NutritionalRating> GetNutritionalRatingByPtRegistrationID(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteNutritionalRating(long NutritionalRatingID, long StaffID, DateTime Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckBeforeChangeDept(long registrationID, long DeptID
                                    , out string errorMessages, out string confirmMessages);

        //▼===== #006
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Prescription CheckOldConsultationPatient(long PatientID);
        //▲===== #006
        //▼===== #007
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Patient> AddNewListPatient(List<APIPatient> ListPatient, string ContractNo);
        //▲===== #007
        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefDepartment LoadDeptAdmissionRequest(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InfectionCase> GetInfectionCaseByPtRegID(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InfectionCase> GetInfectionCaseByPtID(long PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InfectionCase GetInfectionCaseDetail(InfectionCase aInfectionCase);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InfectionCase GetInfectionCaseAllDetails(InfectionCase aInfectionCase);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InfectionVirus> GetAllInfectionVirus();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        long EditInfectionCase(InfectionCase aInfectionCase);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void EditAntibioticTreatment(AntibioticTreatment aAntibioticTreatment);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<AntibioticTreatmentMedProductDetail> GetAntibioticTreatmentMedProductDetails(long AntibioticTreatmentID, long V_AntibioticTreatmentType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugClinicDept> GetDrugsInUseOfAntibioticTreatment(AntibioticTreatment aAntibioticTreatment, long DeptID, long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InfectionCase> GetInfectionCaseAllContentInfo(DateTime StartDate, DateTime EndDate, long V_InfectionCaseStatus, string DrugName);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<AntibioticTreatmentMedProductDetailSummaryContent> GetAllContentInfoOfInfectionCaseCollection(DateTime StartDate, DateTime EndDate, long V_InfectionCaseStatus, string DrugName);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<AntibioticTreatment> GetAntibioticTreatmentsByPtRegID(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void InsertAntibioticTreatmentIntoInstruction(long AntibioticTreatmentID, long InfectionCaseID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegistrationAndGetPrescription(SeachPtRegistrationCriteria regCriteria, int regPageIndex, int regPageSize, bool bregCountTotal,
            PrescriptionSearchCriteria presCriteria, int presPageIndex, int presPageSize, bool bpresCountTotal, out int presTotalCount, out IList<Prescription> lstPrescriptions,
            out int regTotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> GetAllRegistrationForSettlement(long PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MergerPatientRegistration(PatientRegistration PtRegistration, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegistration_ByServiceID(long MedServiceID, DateTime? FromDate, DateTime? ToDate, out List<DiagnosisTreatment> DiagnosisList);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Resources> GetResourcesForMedicalServices();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceItem> GetMedicalServiceItemByHIRepResourceCode(string HIRepResourceCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveSmallProcedureAutomatic(ObservableCollection<SmallProcedure> SmallProcedureList);

        //▼===== #009
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ConfirmEmergencyOutPtByPtRegistrionID(long PtRegistrationID, long StaffID, long V_RegistrationType);
        //▲===== #009

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ConfirmSpecialRegistrationByPtRegistrationID(PatientRegistration PatientRegistration, long StaffID);

        //▼===== #010
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePclRequestTransferToPAC(ObservableCollection<PatientPCLRequest> PatientPCLRequestList, long V_PCLRequestType);
        //▲===== #010
        //▼===== #012
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetail> GetListMedServiceInSurgeryDept(long PtRegistrationID);
        //▲===== #012

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateRequestDrugInwardClinicDept_ByPCLRequest(PatientPCLRequest PatientPCLRequest, out long ReqDrugInClinicDeptID);
        //▼====: #013
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddUpdateDoctorContactPatientTime(DoctorContactPatientTime doctorContactPatientTime);
        //▲====: #013
        //▼====: #014
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GetLastIDC10MainTreatmentProgram_ByPtRegID(long PtRegistrationID, long PtRegDetailID, out long LastOutpatientTreatmentTypeID); //==== #020

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutpatientTreatmentType> GetAllOutpatientTreatmentType();
        //▲====: #014

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RefundInPatientCost(PatientRegistration PtRegistration, long StaffID);
        //▼==== #015
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PrescriptionIssueHistory> GetPrescriptionIssueHistory_ByPtRegDetailID(long PtRegDetailID, long V_RegistrationType);
        //▲==== #015
        //▼====: #016
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateIsConfirmedEmergencyPatient(long PtRegistrationID, bool IsConfirmedEmergencyPatient);
        //▲====: #016 
        //▼====: #017
        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientCardDetail GetCardDetailByPatientID(long PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveCardDetail(PatientCardDetail Obj, bool IsExtendCard);
        //▲====: #017
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckBeforeMergerPatientRegistration(long registrationID, out string errorMessages, out string confirmMessages);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DeathCheckRecord GetDeathCheckRecordByPtRegID(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveDeathCheckRecord(DeathCheckRecord CurDeathCheckRecord);
    }
}
