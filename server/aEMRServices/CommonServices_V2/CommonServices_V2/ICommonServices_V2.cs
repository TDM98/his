using System;
using System.Collections.Generic;
using System.ServiceModel;
using eHCMS.Services.Core;
using DataEntities;
using ErrorLibrary;
using System.IO;
using eHCMS.Configurations;
using DataEntities.MedicalInstruction;
/*
* 20180606 #001 CMN: Added enum for LabSoft API
* 20181113 #002 TTM: BM 0005228: Thêm hàm lấy danh sách phường xã
* 20210117 #003 TNHX: Thêm service cho PAC
* 20210217 #004 TNHX: Thêm mã SID - SampleCode cho LIS
* 20210923 #005 TNHX: 571 Quản lý điều dưỡng thực hiện y lệnh
* 20230316 #006 QTD:  Dữ liệu 130
* 20230601 #007 QTD:  Lấy lại dữ liệu quản lý danh mục Lookup
*/
namespace CommonService_V2
{
    [ServiceContract]
    public interface ICommonService_V2
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientTransactionPayment> GetAllPayments_New(long transactionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientTransactionPayment> GetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch, int FindPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff, List<PatientTransactionPayment> allPayment, out long RepPaymentRecvID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReportPaymentReceiptByStaff> GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate, bool bFilterByStaffID, Int64 nStaffID, long loggedStaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        ReportPaymentReceiptByStaffDetails GetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime GetMaxExamDate();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        TransferForm CreateBlankTransferFormByRegID(long PtRegistrationID, int PatientFindBy, long V_TransferFormType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime GetDate();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLRequest UpdateDrAndDiagTrmtForPCLReq(long ServiceRecID, long PCLReqID, long DoctorStaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DeletePCLRequestWithDetails(Int64 PatientPCLReqID, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DeleteInPtPCLRequestWithDetails(Int64 PatientPCLReqID);

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
        List<PatientPaymentAccount> GetAllPatientPaymentAccounts();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_DeptID(long DeptID);

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
        List<TransactionFinalization> GetPatientSettlement(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientTransaction GetAllPaymentByRegistrationID_InPt(long registrationID);

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

        #region Export excel from database
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_ListRefGenericDrug(DrugSearchCriteria criteria);
        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_ListRefGenericDrug_New(DrugSearchCriteria criteria);
        //▲===== 25072018 TTM


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportToExcellAll_ListRefGenMedProductDetail(RefGenMedProductDetailsSearchCriteria criteria);
        #endregion

        #region InPatientInstruction
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddInPatientInstruction(PatientRegistration ptRegistration, bool IsUpdateDiagConfirmInPT, long WebIntPtDiagDrInstructionID, out long IntPtDiagDrInstructionID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientInstruction GetInPatientInstruction(PatientRegistration ptRegistration);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InPatientInstruction> GetInPatientInstructionCollection(PatientRegistration aRegistration);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InPatientInstruction> GetInPatientInstructionCollectionForCreateOutward(long PtRegistrationID, bool? IsCreatedOutward, long V_MedProductType, long StoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InPatientInstruction> GetInPatientInstructionCollectionForTransmissionMonitor(long PtRegistrationID);//, bool? IsCreatedOutward, long V_MedProductType, long StoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<TransmissionMonitor> GetTransmissionMonitorByInPatientInstructionID(long InPatientInstructionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveTransmissionMonitor(TransmissionMonitor CurTransmissionMonitor, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientInstruction GetInPatientInstructionByInstructionID(long aIntPtDiagDrInstructionID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Intravenous> GetIntravenousPlan_InPt(long IntPtDiagDrInstructionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ReqOutwardDrugClinicDeptPatient> GetAntibioticTreatmentUsageHistory(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ReqOutwardDrugClinicDeptPatient> GetAllDrugFromDoctorInstruction(long PtRegistrationID, DateTime CurrentDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GetAllItemsByInstructionID(long IntPtDiagDrInstructionID, out List<PatientRegistrationDetail> AllRegistrationItems, out List<PatientPCLRequestDetail> AllPCLRequestItems);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveInstructionMonitoring(InPatientInstruction InPatientInstruction, long StaffID);
        #endregion

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllLookupValuesByType(LookupValues lookUpType);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefGenericDrugCategory_2> RefGenericDrugCategory_2_Get(long V_MedProductType);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Gender> GetAllGenders();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefCountry> GetAllCountries();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<CitiesProvince> GetAllProvinces();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        Stream GetVideoAndImage(string path);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        Stream GetVideoAndImage_V2(string aPath, bool aMapPath);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefDepartment> GetAllDepartments(bool bIncludeDeleted);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        AxServerConfigSection GetServerConfiguration();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        List<SuburbNames> GetAllSuburbNames();

        //▼====== #002
        [FaultContract(typeof(AxException))]
        [OperationContract]
        List<WardNames> GetAllWardNames();
        //▲====== #002

        [FaultContract(typeof(AxException))]
        [OperationContract]
        List<StaffPosition> GetAllStaffPosition();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefDepartment> GetDepartments(long locationID);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefDepartment> GetAllDepartmentsByV_DeptTypeOperation(long V_DeptTypeOperation);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllDocumentTypeOnHold();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Hospital> Hospital_ByCityProvinceID(long? CityProvinceID);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefGenericDrugCategory_1> RefGenericDrugCategory_1_Get(long V_MedProductType);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<DeptTransferDocReq> GetAllDocTypeRequire();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientTransactionDetail> GetTransactionSum(long TransactionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientTransactionDetail> GetTransactionSum_InPt(long TransactionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReportOutPatientCashReceipt_Payments> GetMoreReportOutPatientCashReceipt_TongHop_Async(int RefAsyncKey, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        bool DoUpload(string filename, byte[] content, bool append, string dir);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        bool DoListUpload(List<PCLResultFileStorageDetail> lst);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Currency> GetAllCurrency(bool? IsActive);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllEthnics();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllReligion();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllMaritalStatus();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory();

        //▼===== 25072018 TTM
        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory_New();
        //▼===== 25072018 TTM

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Staff> GetAllStaffContain();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllBankName();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        bool SaveImageCapture(byte[] byteArray);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        bool SaveImageCapture1(MemoryStream byteArray);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllLookupValuesForTransferForm(LookupValues lookUpType);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<DrugDeptProductGroupReportType> GetDrugDeptProductGroupReportTypes();
        [OperationContract]
        long EditRefMedicalServiceGroup(RefMedicalServiceGroups aRefMedicalServiceGroup);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceGroups> GetRefMedicalServiceGroups(string MedicalServiceGroupCode);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceGroupItem> GetRefMedicalServiceGroupItemsByID(long MedicalServiceGroupID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ShortHandDictionary> GetShortHandDictionariesByStaffID(long StaffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateOrderNumberRegistration(List<string> orderNumLst);

        #region Quản lý điều dưỡng thực hiện y lệnh
        //▼====: #005
        [OperationContract]
        [FaultContract(typeof(AxException))]
        TicketCare SaveTicketCare(TicketCare gTicketCare, long CreatedStaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        TicketCare GetTicketCare(long IntPtDiagDrInstructionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<TicketCare> GetTicketCareListForRegistration(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ExecuteDrug> GetExecuteDrugListForRegistration(long PtRegistrationID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveExecuteDrug(long ExecuteDrugID, long ExecuteDrugDetailID, long StaffID, long CreatedStaffID, DateTime DateExecute);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteExecuteDrug(long ExecuteDrugDetailID, long CreatedStaffID);
        //▲====: #005
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiseaseProgression> GetAllDiseaseProgression(bool UseInConfig);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiseaseProgressionDetails> GetAllDiseaseProgressionDetails();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateIsBlockBill(bool IsBlock, long PtRegistrationID, long DeptID);

        //▼==== #006
        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<RefNationality> GetAllNationalities();

        [FaultContract(typeof(AxException))]
        [OperationContract]
        List<RefJob> GetAllJobs();
        //▲==== #006

        //▼==== #007
        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<Lookup> GetAllLookupValuesByTypeForMngt(LookupValues lookUpType, string SearchCriteria);

        [FaultContract(typeof(AxException))]
        [OperationContract]
        IList<LookupTree> GetTreeView_LookupForMngt();
        //▲==== #007
    }
}
//▼====: #001
namespace LabSoftService
{
    [ServiceContract]
    public interface ILabSoftService
    {
        [OperationContract]
        List<LIS_Staff> GetBacSi();
        [OperationContract]
        List<LIS_Department> GetKhoaPhong();
        [OperationContract]
        List<LIS_Object> GetDoiTuong();
        [OperationContract]
        List<LIS_PCLItem> GetXetNghiem();
        [OperationContract]
        List<LIS_Device> GetThietBi();
        [OperationContract]
        List<LIS_User> GetNguoiDung();
        [OperationContract]
        List<LIS_PCLRequest> GetDanhSachChiDinh(string TuNgay, string DenNgay, int TrangThai);
        [OperationContract]
        List<LIS_PCLRequest> GetChiDinh(string SoPhieuChiDinh, int TrangThai);
        [OperationContract]
        bool CapNhatTrangThaiNhanMau(string SoPhieuChiDinh, string MaDichVu, int TrangThai, string NgayTiepNhan, out string TrangThaiCapNhat);

        //▼====: #004
        [OperationContract]
        bool NhanKetQuaXetNghiem(string SoPhieuChiDinh, string MaDichVu, string KetQua, string CSBT
            , string DonViTinh, bool BatThuong, int TrangThai, string MaNV_XacNhan
            , string ThoiGianXacNhan, string MaThietBi, string SampleCode);
        //▲====: #004

        [OperationContract]
        List<LIS_PCLRequest> KiemKetQuaXetNghiem(string SoPhieuChiDinh);      
    }
}
//▲====: #001
namespace RISService
{
    [ServiceContract]
    public interface IRISService
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        APIPatientPCLRequest GetPCLRequestDetail(string PCLRequestNumID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<APIStaff> GetAllStaffs();
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPCLResultFileStorageDetails(APIPatientPCLRequest aAPIPatientPCLRequest, long StaffID, string DiagnoseOnPCLExam
            , string HIRepResourceCode
            , string TemplateResultString
            , string TemplateResultDescription
            , string TemplateResult
            , long PerformStaffID
            , string Suggest);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime GetDate();
    }
}

//▼====: #003
namespace PACService
{
    [ServiceContract]
    public interface IPACService
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        APIPatientPCLRequest GetPCLRequestDetail(string PCLRequestNumID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<APIStaff> GetAllStaffs();
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPCLResultFileStorageDetails(long PatientPCLReqID, long PCLExamTypeID, long V_PCLRequestType
            , long StaffID, string DiagnoseOnPCLExam
            , string HIRepResourceCode
            , string TemplateResultString
            , string TemplateResultDescription
            , string TemplateResult
            , long PerformStaffID
            , string Suggest);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime GetDate();
    }
}
//▲====: #003
