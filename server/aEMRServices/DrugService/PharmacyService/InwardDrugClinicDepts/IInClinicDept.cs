using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using System.Xml.Linq;
using ErrorLibrary;
/*
 * 20190806 #001 TTM: BM 0013080: Sửa ý số 1 của Bugmantis: Cho phép lựa chọn ngày load y lệnh.
 * 20201026 #002 TNHX: BM: Thêm func Cancel Duyệt phiếu lĩnh cho Khoa Dược
 * 20210109 #003 TNHX: BM: Hoàn thiện quản lý suất ăn
 * 20220112 #004 BLQ: Thêm loại đăng ký để tìm phiếu lĩnh ngoại trú
 * 20230508 #005 DatTB: Thêm nút xuất Excel danh sách các mẫu lĩnh
 * 20230511 #006 QTD: Thêm chức năng Tab Vật tư y tế kèm DVKT
 */

namespace PharmacyService
{
    [ServiceContract]
    public interface IInClinicDept
    {
        #region 17. Inward Drug For Clinic Dept

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugClinicDeptInvoice> SearchInwardDrugClinicDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugClinicDeptInvoice GetInwardDrugClinicDeptInvoice_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugClinicDeptInvoice GetInwardDrugClinicDeptInvoice_ByID_V2(long ID, long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugClinicDeptInvoice GetInwardDrugMedDeptInvoice_ByID(long ID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugClinicDept> GetInwardDrugClinicDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
        , out decimal TongTienSPChuaVAT
        , out decimal CKTrenSP
        , out decimal TongTienTrenSPDaTruCK
        , out decimal TongCKTrenHoaDon
        , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugClinicDept> GetInwardDrugClinicDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugClinicDept> GetInwardDrugClinicDept_ByIDInvoiceNotPaging_V2(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoVAT
            , long? V_MedProductType
            , bool IsMedDeptSubStorage);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardDrugClinicDeptInvoice(InwardDrugClinicDeptInvoice InvoiceDrug, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardDrugClinicDept(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardDrugClinicDeptInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardDrugClinicDept(InwardDrugClinicDept invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardDrugClinicDeptInvoice(InwardDrugClinicDeptInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardDrugClinicDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardDrugClinicDeptInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ExistInwardDrug(InwardDrugClinicDeptInvoice invoiceDrug);

        //--▼--02/01/2021 DatTB
        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardDrugInternalMedDeptInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, out long id);
        //--▲--02/01/2021 DatTB

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AcceptAutoUpdateInwardClinicInvoice(long inviID);


        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveOutwardDrugClinicDeptTemplate(OutwardDrugClinicDeptTemplate OutwardTemplate, out OutwardDrugClinicDeptTemplate OutwardTemplateOut);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutwardDrugClinicDeptTemplate GetOutwardTemplateByID(long OutwardTemplateID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugClinicDeptTemplate> GetAllOutwardTemplate(long V_MedProductType, long DeptID, long? V_OutwardTemplateType, bool? IsShared);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteOutwardDrugClinicDeptTemplate(long OutwardTemplateID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool FullOperatorRequestDrugInwardClinicDept(RequestDrugInwardClinicDept Request, long V_MedProductType, out RequestDrugInwardClinicDept OutRequest);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool FullOperatorRequestDrugInwardClinicDeptNew(RequestDrugInwardClinicDept Request,long V_MedProductType
            //▼====: #004
            , long V_RegistrationType, out RequestDrugInwardClinicDept OutRequest);
            //▲====: #004

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RequestDrugInwardClinicDept GetRequestDrugInwardClinicDeptByID(long ReqDrugInClinicDeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReqOutwardDrugClinicDeptPatient> GetRemainingQtyForInPtRequestDrug(long? StoreID, long V_MedProductType, long RefGenDrugCatID_1);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestDrugInwardClinicDeptDetail> GetRequestDrugInwardClinicDeptDetailByID(long ReqDrugInClinicDeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReqOutwardDrugClinicDeptPatient> GetReqOutwardDrugClinicDeptPatientByID(long ReqDrugInClinicDeptID, bool bGetExistingReqToCreateNew
            //▼====: #004
            , long V_RegistrationType);
            //▲====: #004

        //▼===== #001: Bổ sung FromDate và ToDate để load y lệnh theo ngày.
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReqOutwardDrugClinicDeptPatient> ReqOutwardDrugClinicFromInstruction(DateTime? FromDate, DateTime? ToDate, long RefGenDrugCatID_1, long DeptID, long V_MedProductType, bool IsInstructionFuture);
        //▲===== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RequestDrugInwardClinicDept_Approved(RequestDrugInwardClinicDept Request);
        //▼====: #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RequestDrugInwardClinicDept_Cancel(RequestDrugInwardClinicDept Request, long CancelStaffID);
        //▲====: #002

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReqOutwardDrugClinicDeptPatient> GetReqOutwardDrugClinicDeptPatientSumByID(long ReqDrugInClinicDeptID);



        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestDrugInwardClinicDept> SearchRequestDrugInwardClinicDept(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestDrugInwardClinicDept> GetRequestDrugInwardClinicDept_ByRegistrationID(long PtRegistrationID, long V_MedProductType, long StoreID,long? outiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRequestDrugInwardClinicDept(long ReqDrugInClinicDeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/);

        #region 18. Outward Drug For Clinic Dept

    
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugClinicDept> spGetInBatchNumberAndPrice_ListForRequestClinicDept(bool? IsCost,
                                                                                        List<RequestDrugInwardClinicDept> ReqDrugInClinicDeptID,
                                                                                        long OutwardTemplateID,
                                                                                        long StoreID,
                                                                                        long V_MedProductType, long PtRegistrationID, bool? IsHIPatient, DateTime OutDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugClinicDept> GetInBatchNumberAndPrice_ListForRequestClinicDeptFromInstruction(long[] InstructionIDCollection
            , long StoreID
            , long V_MedProductType
            , long PtRegistrationID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugClinicDept> OutwardDrugClinicDeptInvoices_SearchTKPaging(SearchOutwardInfo SearchCriteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugClinicDeptInvoice_SaveByType(OutwardDrugClinicDeptInvoice Invoice, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugClinicDeptInvoice_UpdateByType(OutwardDrugClinicDeptInvoice Invoice, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutwardDrugClinicDeptInvoice GetOutwardDrugClinicDeptInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutwardDrugClinicDeptInvoice GetOutwardDrugClinicDeptInvoice_V2(long ID, long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugClinicDept> GetOutwardDrugClinicDeptDetailByInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugClinicDept> GetOutwardDrugClinicDeptDetailByInvoice_V2(long ID, long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugClinicDeptInvoice> OutwardDrugClinicDeptInvoice_SearchByType(MedDeptInvoiceSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> spGetInBatchNumberAllClinicDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        StaffDeptPresence GetAllStaffDeptPresenceInfo(long DeptID, DateTime StaffCountDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        StaffDeptPresence SaveAllStaffDeptPresenceInfo(StaffDeptPresence CurStaffDeptPresence, bool IsUpdateRequiredNumber);

        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardDrugInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugClinicDeptInvoice> SearchInwardDrugInvoiceForPharmacy(InwardInvoiceSearchCriteria criteria, long? TypID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugClinicDept> GetInwardDrugPharmacy_ByIDInvoiceNotPaging(long inviID
        , out decimal TongTienSPChuaVAT
        , out decimal CKTrenSP
        , out decimal TongTienTrenSPDaTruCK
        , out decimal TongCKTrenHoaDon
        , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RequestDrugInwardHIStore_Save(RequestDrugInwardForHiStore Request, long V_MedProductType, out RequestDrugInwardForHiStore OutRequest);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestDrugInwardForHiStore> SearchRequestDrugInwardHIStore(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestDrugInwardForHiStoreDetails> GetRequestDrugInwardHIStoreDetailByID(long RequestDrugInwardHiStoreID, bool bGetExistingReqToCreateNew);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RequestDrugInwardForHiStore GetRequestDrugInwardHIStoreByID(long RequestDrugInwardHiStoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RequestDrugInwardHIStore_Approved(RequestDrugInwardForHiStore Request);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRequestDrugInwardHIStore(long RequestDrugInwardHiStoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetVTYTTHForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetBloodForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetThanhTrungForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetChemicalForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetVPPForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetTiemNguaForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetVTTHForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/);

        #region Other Function
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestDrugInwardClinicDept> SearchRequestDrugInwardClinicDept_NotPaging(long V_MedProductType, long StoreID);

        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByIDList(long StoreID, long V_MedProductType, List<long> GenIDCollection);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void OutwardDrugClinicDeptInvoices_SaveByItemCollection(IList<OutwardDrugClinicDeptInvoice> InvoiceCollection, long StaffID, DateTime OutDate, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> ReqOutwardDrugClinicFromPrescription(long PtRegistrationID, long StoreProvided, long RefGenDrugCatID_1);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugClinicDeptInvoice_SaveByType_Balance(OutwardDrugClinicDeptInvoice Invoice, int ViewCase, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetDrugClinicDeptForBalance_FromCategory(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient);
        
        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestFoodClinicDept> SearchRequestFoodClinicDept(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReqFoodClinicDeptDetail> LoadReqFoodClinicFromInstruction(DateTime? FromDate, DateTime? ToDate, long DeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveRequestFoodClinicDept(RequestFoodClinicDept Request, out RequestFoodClinicDept OutRequest);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReqFoodClinicDeptDetail> GetReqFoodClinicDeptDetailsByID(long ReqFoodClinicDeptID);
        //▲====: #004

        //▼==== #005
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportExcelOutwardClinicDeptTemplates(long V_MedProductType, long V_OutwardTemplateType);
        //▲==== #005

        //▼====: #006
        [OperationContract]
        [FaultContract(typeof(AxException))]
        RequestDrugForTechnicalService GetRequestDrugForTechnicalServicePtRegDetailID(long PtRegDetailID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveRequestDrugForTechnicalService(RequestDrugForTechnicalService Request, long V_MedProductType, long V_RegistrationType, out RequestDrugForTechnicalService OutRequest);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRequestDrugForTechnicalService(long ReqForTechID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ReqOutwardDrugClinicDeptPatient> GetReqOutwardDrugClinicDeptPatientByReqService(long ReqForTechID, long V_RegistrationType);
    }
}
