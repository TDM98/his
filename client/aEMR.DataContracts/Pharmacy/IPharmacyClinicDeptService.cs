using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using aEMR.DataContracts;
/*
 * 20180412 #001 TTM: BM 0005324: Cho phép nhập hàng từ khoa dược vào kho BHYT - Nhà thuốc.
 * 20181219 #002 TTM: BM 0005443: Xây dựng chức năng lập phiếu lĩnh nhà thuốc (Kho -BHYT), tìm kiếm, xóa phiếu lĩnh ...
 * 20190806 #003 TTM: BM 0013080: Sửa ý số 1 của Bugmantis: Cho phép lựa chọn ngày load y lệnh.
 * 20200708 #004 TTM: BM 0039353: Bổ sung chức năng load dữ liệu phiếu yêu cầu từ toa thuốc xuất viện.
 * 20201030 #005 TNHX: BM: Thêm chức năng hủy phiếu lĩnh đã được duyệt
 * 20201111 #006 TNHX: BM: Thêm chức năng lập phiếu suất ăn
 * 20210109 #007 TNHX: BM: hoàn thiện chức năng lập phiếu suất ăn
 * 20220112 #008 BLQ: Thêm loại đăng ký để tìm phiếu lĩnh ngoại trú
 * 20230508 #009 DatTB: Thêm nút xuất Excel danh sách các mẫu lĩnh
 * 20230511 #010 QTD: Thêm chức năng cho Tab Vật tư kèm DVKT
 */
namespace ClinicDeptProxy
{
    [ServiceContract]
    public interface IInClinicDept
    {
        #region 17. Inward Drug For Clinic Dept

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardDrugClinicDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugClinicDeptInvoice> EndSearchInwardDrugClinicDeptInvoice(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugClinicDeptInvoice_ByID(long ID, AsyncCallback callback, object state);
        InwardDrugClinicDeptInvoice EndGetInwardDrugClinicDeptInvoice_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugMedDeptInvoice_ByID(long ID,long V_MedProductType, AsyncCallback callback, object state);//--02/01/2021 DatTB Thêm biến V_MedProductType
        InwardDrugClinicDeptInvoice EndGetInwardDrugMedDeptInvoice_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugClinicDeptInvoice_ByID_V2(long ID, long? V_MedProductType, AsyncCallback callback, object state);
        InwardDrugClinicDeptInvoice EndGetInwardDrugClinicDeptInvoice_ByID_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugClinicDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugClinicDept> EndGetInwardDrugClinicDept_ByIDInvoice(out int totalCount
        , out decimal TongTienSPChuaVAT
        , out decimal CKTrenSP
        , out decimal TongTienTrenSPDaTruCK
        , out decimal TongCKTrenHoaDon
        , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugClinicDept_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);
        IList<InwardDrugClinicDept> EndGetInwardDrugClinicDept_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugClinicDept_ByIDInvoiceNotPaging_V2(long inviID, long? V_MedProductType, bool IsMedDeptSubStorage, AsyncCallback callback, object state);
        IList<InwardDrugClinicDept> EndGetInwardDrugClinicDept_ByIDInvoiceNotPaging_V2(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardDrugClinicDeptInvoice(InwardDrugClinicDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndAddInwardDrugClinicDeptInvoice(out long inwardid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardDrugClinicDept(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardDrugClinicDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardDrugClinicDeptInvoice(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardDrugClinicDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardDrugClinicDept(InwardDrugClinicDept invoicedrug, AsyncCallback callback, object state);
        int EndUpdateInwardDrugClinicDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardDrugClinicDeptInvoice(InwardDrugClinicDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndUpdateInwardDrugClinicDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndspGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugClinicDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, AsyncCallback callback, object state);
        bool EndInwardDrugClinicDept_InsertList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugClinicDeptInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, AsyncCallback callback, object state);
        int EndInwardDrugClinicDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExistInwardDrug(InwardDrugClinicDeptInvoice invoiceDrug, AsyncCallback callback, object state);
        bool EndExistInwardDrug(IAsyncResult asyncResult);

        //--▼--02/01/2021 DatTB
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugInternalMedDeptInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, AsyncCallback callback, object state);
        int EndInwardDrugInternalMedDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);
        //--▲--02/01/2021 DatTB

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAcceptAutoUpdateInwardClinicInvoice(long inviID, AsyncCallback callback, object state);
        bool EndAcceptAutoUpdateInwardClinicInvoice(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveOutwardDrugClinicDeptTemplate(OutwardDrugClinicDeptTemplate OutwardTemplate, AsyncCallback callback, object state);
        bool EndSaveOutwardDrugClinicDeptTemplate(out OutwardDrugClinicDeptTemplate OutwardTemplateOut, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardTemplateByID(long OutwardTemplateID, AsyncCallback callback, object state);
        OutwardDrugClinicDeptTemplate EndGetOutwardTemplateByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllOutwardTemplate(long V_MedProductType, long DeptID, long? V_OutwardTemplateType, bool? IsShared, AsyncCallback callback, object state);
        List<OutwardDrugClinicDeptTemplate> EndGetAllOutwardTemplate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteOutwardDrugClinicDeptTemplate(long OutwardTemplateID, AsyncCallback callback, object state);
        bool EndDeleteOutwardDrugClinicDeptTemplate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginFullOperatorRequestDrugInwardClinicDept(RequestDrugInwardClinicDept Request, long V_MedProductType, AsyncCallback callback, object state);
        bool EndFullOperatorRequestDrugInwardClinicDept(out RequestDrugInwardClinicDept OutRequest, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginFullOperatorRequestDrugInwardClinicDeptNew(RequestDrugInwardClinicDept Request, long V_MedProductType
            //▼====: #008
            , long V_RegistrationType, AsyncCallback callback, object state);
            //▲====: #008
        bool EndFullOperatorRequestDrugInwardClinicDeptNew(out RequestDrugInwardClinicDept OutRequest, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRemainingQtyForInPtRequestDrug(long? StoreID, long V_MedProductType, long RefGenDrugCatID_1, AsyncCallback callback, object state);
        List<ReqOutwardDrugClinicDeptPatient> EndGetRemainingQtyForInPtRequestDrug(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardClinicDeptByID(long ReqDrugInClinicDeptID, AsyncCallback callback, object state);
        RequestDrugInwardClinicDept EndGetRequestDrugInwardClinicDeptByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardClinicDeptDetailByID(long ReqDrugInClinicDeptID, AsyncCallback callback, object state);
        List<RequestDrugInwardClinicDeptDetail> EndGetRequestDrugInwardClinicDeptDetailByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetReqOutwardDrugClinicDeptPatientByID(long ReqDrugInClinicDeptID, bool bGetExistingReqToCreateNew
            //▼====: #008
            , long V_RegistrationType , AsyncCallback callback, object state);
            //▲====: #008
        List<ReqOutwardDrugClinicDeptPatient> EndGetReqOutwardDrugClinicDeptPatientByID(IAsyncResult asyncResult);

        //▼===== #003: Bổ sung thêm từ ngày đến ngày vào cho hàm để load y lệnh theo ngày
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginReqOutwardDrugClinicFromInstruction(DateTime? FromDate, DateTime? ToDate, long RefGenDrugCatID_1, long DeptID, long V_MedProductType, bool IsInstructionFuture, AsyncCallback callback, object state);
        List<ReqOutwardDrugClinicDeptPatient> EndReqOutwardDrugClinicFromInstruction(IAsyncResult asyncResult);
        //▲===== #003

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRequestDrugInwardClinicDept_Approved(RequestDrugInwardClinicDept Request, AsyncCallback callback, object state);
        bool EndRequestDrugInwardClinicDept_Approved(IAsyncResult asyncResult);

        //▼====: #005
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginRequestDrugInwardClinicDept_Cancel(RequestDrugInwardClinicDept Request, long CancelStaffID, AsyncCallback callback, object state);
        bool EndRequestDrugInwardClinicDept_Cancel(IAsyncResult asyncResult);
        //▲====: #005

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetReqOutwardDrugClinicDeptPatientSumByID(long ReqDrugInClinicDeptID, AsyncCallback callback, object state);
        List<ReqOutwardDrugClinicDeptPatient> EndGetReqOutwardDrugClinicDeptPatientSumByID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRequestDrugInwardClinicDept(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        List<RequestDrugInwardClinicDept> EndSearchRequestDrugInwardClinicDept(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardClinicDept_ByRegistrationID(long PtRegistrationID, long V_MedProductType, long StoreID, long? outiID, AsyncCallback callback, object state);
        List<RequestDrugInwardClinicDept> EndGetRequestDrugInwardClinicDept_ByRegistrationID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRequestDrugInwardClinicDept(long ReqDrugInClinicDeptID, AsyncCallback callback, object state);
        bool EndDeleteRequestDrugInwardClinicDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, IList<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetDrugForSellVisitorAutoComplete_ForRequestClinicDept(IAsyncResult asyncResult);



        #region 18. Outward Drug For Clinic Dept

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAndPrice_ListForRequestClinicDept(bool? IsCost,
                                                                                      IList<RequestDrugInwardClinicDept> ReqDrugInClinicDeptID,
                                                                                       long OutwardTemplateID,
                                                                                      long StoreID,
                                                                                      long V_MedProductType, long PtRegistrationID, bool? IsHIPatient, DateTime OutDate, AsyncCallback callback, object state);
        List<OutwardDrugClinicDept> EndspGetInBatchNumberAndPrice_ListForRequestClinicDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetInBatchNumberAndPrice_ListForRequestClinicDeptFromInstruction(long[] InstructionIDCollection
            , long StoreID
            , long V_MedProductType
            , long PtRegistrationID
            , AsyncCallback callback, object state);
        List<OutwardDrugClinicDept> EndGetInBatchNumberAndPrice_ListForRequestClinicDeptFromInstruction(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugClinicDeptInvoices_SearchTKPaging(SearchOutwardInfo SearchCriteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        List<OutwardDrugClinicDept> EndOutwardDrugClinicDeptInvoices_SearchTKPaging(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugClinicDeptInvoice_SaveByType(OutwardDrugClinicDeptInvoice Invoice, AsyncCallback callback, object state);
        bool EndOutwardDrugClinicDeptInvoice_SaveByType(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugClinicDeptInvoice_UpdateByType(OutwardDrugClinicDeptInvoice Invoice, AsyncCallback callback, object state);
        bool EndOutwardDrugClinicDeptInvoice_UpdateByType(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugClinicDeptInvoice(long ID, AsyncCallback callback, object state);
        OutwardDrugClinicDeptInvoice EndGetOutwardDrugClinicDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugClinicDeptInvoice_V2(long ID, long? V_MedProductType, AsyncCallback callback, object state);
        OutwardDrugClinicDeptInvoice EndGetOutwardDrugClinicDeptInvoice_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugClinicDeptDetailByInvoice(long ID, AsyncCallback callback, object state);
        IList<OutwardDrugClinicDept> EndGetOutwardDrugClinicDeptDetailByInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugClinicDeptDetailByInvoice_V2(long ID, long? V_MedProductType, AsyncCallback callback, object state);
        IList<OutwardDrugClinicDept> EndGetOutwardDrugClinicDeptDetailByInvoice_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugClinicDeptInvoice_SearchByType(MedDeptInvoiceSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        IList<OutwardDrugClinicDeptInvoice> EndOutwardDrugClinicDeptInvoice_SearchByType(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAllClinicDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndspGetInBatchNumberAllClinicDept_ByGenMedProductID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllStaffDeptPresenceInfo(long DeptID, DateTime StaffCountDate, AsyncCallback callback, object state);
        StaffDeptPresence EndGetAllStaffDeptPresenceInfo(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveAllStaffDeptPresenceInfo(StaffDeptPresence CurStaffDeptPresence, bool IsUpdateRequiredNumber, AsyncCallback callback, object state);
        StaffDeptPresence EndSaveAllStaffDeptPresenceInfo(IAsyncResult asyncResult);


        #endregion

        //▼====== #001
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward,AsyncCallback callback, object state);
        int EndInwardDrugInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardDrugInvoiceForPharmacy(InwardInvoiceSearchCriteria criteria, long? TypID, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugClinicDeptInvoice> EndSearchInwardDrugInvoiceForPharmacy(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugPharmacy_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);

        IList<InwardDrugClinicDept> EndGetInwardDrugPharmacy_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
        , out decimal CKTrenSP
        , out decimal TongTienTrenSPDaTruCK
        , out decimal TongCKTrenHoaDon
        , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);
        //▲====== #001
        //▼====== #002
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRequestDrugInwardHIStore(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        List<RequestDrugInwardForHiStore> EndSearchRequestDrugInwardHIStore(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRequestDrugInwardHIStore_Save(RequestDrugInwardForHiStore Request, long V_MedProductType, AsyncCallback callback, object state);
        bool EndRequestDrugInwardHIStore_Save(out RequestDrugInwardForHiStore OutRequest, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardHIStoreDetailByID(long RequestDrugInwardHiStoreID, bool bGetExistingReqToCreateNew, AsyncCallback callback, object state);
        List<RequestDrugInwardForHiStoreDetails> EndGetRequestDrugInwardHIStoreDetailByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardHIStoreByID(long RequestDrugInwardHiStoreID, AsyncCallback callback, object state);
        RequestDrugInwardForHiStore EndGetRequestDrugInwardHIStoreByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRequestDrugInwardHIStore_Approved(RequestDrugInwardForHiStore Request, AsyncCallback callback, object state);
        bool EndRequestDrugInwardHIStore_Approved(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRequestDrugInwardHIStore(long RequestDrugInwardHiStoreID, AsyncCallback callback, object state);
        bool EndDeleteRequestDrugInwardHIStore(IAsyncResult asyncResult);
        //▲====== #002

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetVTYTTHForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, IList<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetVTYTTHForSellVisitorAutoComplete_ForRequestClinicDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetBloodForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, IList<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetBloodForSellVisitorAutoComplete_ForRequestClinicDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetThanhTrungForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, IList<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetThanhTrungForSellVisitorAutoComplete_ForRequestClinicDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetChemicalForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, IList<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetChemicalForSellVisitorAutoComplete_ForRequestClinicDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetVPPForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, IList<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetVPPForSellVisitorAutoComplete_ForRequestClinicDept(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]

        IAsyncResult BeginGetTiemNguaForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, IList<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetTiemNguaForSellVisitorAutoComplete_ForRequestClinicDept(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]

        IAsyncResult BeginGetVTTHForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, IList<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetVTTHForSellVisitorAutoComplete_ForRequestClinicDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRequestDrugInwardClinicDept_NotPaging(long V_MedProductType, long StoreID , AsyncCallback callback, object state);
        List<RequestDrugInwardClinicDept> EndSearchRequestDrugInwardClinicDept_NotPaging(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllInwardDrugClinicDeptByIDList(long StoreID, long V_MedProductType, List<long> GenIDCollection, AsyncCallback callback, object state);
        List<InwardDrugClinicDept> EndGetAllInwardDrugClinicDeptByIDList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugClinicDeptInvoices_SaveByItemCollection(IList<OutwardDrugClinicDeptInvoice> InvoiceCollection, long StaffID, DateTime OutDate, long V_MedProductType, AsyncCallback callback, object state);
        void EndOutwardDrugClinicDeptInvoices_SaveByItemCollection(IAsyncResult asyncResult);

        //▼===== #004
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginReqOutwardDrugClinicFromPrescription(long PtRegistrationID, long StoreProvided, long RefGenDrugCatID_1, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndReqOutwardDrugClinicFromPrescription(IAsyncResult asyncResult);
        //▲===== #004
        //▼====: #007
        //▼===== #006
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginLoadReqFoodClinicFromInstruction(DateTime? FromDate, DateTime? ToDate, long DeptID, AsyncCallback callback, object state);
        List<ReqFoodClinicDeptDetail> EndLoadReqFoodClinicFromInstruction(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchRequestFoodClinicDept(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        List<RequestFoodClinicDept> EndSearchRequestFoodClinicDept(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveRequestFoodClinicDept(RequestFoodClinicDept Request, AsyncCallback callback, object state);
        bool EndSaveRequestFoodClinicDept(out RequestFoodClinicDept OutRequest, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetReqFoodClinicDeptDetailsByID(long ReqFoodClinicDeptID, AsyncCallback callback, object state);
        List<ReqFoodClinicDeptDetail> EndGetReqFoodClinicDeptDetailsByID(IAsyncResult asyncResult);
        //▲===== #006

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugClinicDeptInvoice_SaveByType_Balance(OutwardDrugClinicDeptInvoice Invoice, int ViewCase, AsyncCallback callback, object state);
        bool EndOutwardDrugClinicDeptInvoice_SaveByType_Balance(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugClinicDeptForBalance_FromCategory(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, IList<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetDrugClinicDeptForBalance_FromCategory(IAsyncResult asyncResult);
        //▲====: #007

        //▼===== #009
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginExportExcelOutwardClinicDeptTemplates(long V_MedProductType, long V_OutwardTemplateType, AsyncCallback callback, object state);
        List<List<string>> EndExportExcelOutwardClinicDeptTemplates(IAsyncResult asyncResult);
        //▲==== #009

        //▼====: #010
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugForTechnicalServicePtRegDetailID(long PtRegDetailID, long V_RegistrationType, AsyncCallback callback, object state);
        RequestDrugForTechnicalService EndGetRequestDrugForTechnicalServicePtRegDetailID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveRequestDrugForTechnicalService(RequestDrugForTechnicalService Request, long V_MedProductType, long V_RegistrationType, AsyncCallback callback, object state);
        bool EndSaveRequestDrugForTechnicalService(out RequestDrugForTechnicalService OutRequest, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRequestDrugForTechnicalService(long ReqForTechID, AsyncCallback callback, object state);
        bool EndDeleteRequestDrugForTechnicalService(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetReqOutwardDrugClinicDeptPatientByReqService(long ReqForTechID, long V_RegistrationType, AsyncCallback callback, object state);
        List<ReqOutwardDrugClinicDeptPatient> EndGetReqOutwardDrugClinicDeptPatientByReqService(IAsyncResult asyncResult);
        //▲====: #010
    }
}