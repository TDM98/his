using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using DataEntities;
using eHCMS.Services.Core;
using aEMR.DataContracts;
using System.IO;
/* 
 * 20181030 #001 TNHX: [BM0002176] Add params HospitalName, DepartmentOfHealth.
 * 20181201 #002 TNHX: [BM0005312] Add report PhieuMienGiam & silent print
 * 20181219 #003 TNHX: [BM0005447] Fix print silent of PhieuChiDinh, PhieuMienGiam
 * 20190228 #004 TNHX: [] Add thermal report for BienLaiThuTien
 * 20190422 #005 TNHX: [BM0006716] Apply thermal report for Cashier at BHYT, change services at Pharmacy
 * 20190917 #006 TNHX: [BM0013247] Apply search for XRptInOutStockValueDrugDept_TV base on config "AllowSearchInReport"
 * 20191029 #007 TNHX: [] Apply search for XRptInOutStock_TV base on config "AllowSearchInReport"
 * 20200128 #008 TNHX: [] Apply search for XRptInOutStockGeneral base on config "AllowSearchInReport"
 * 20200906 #009 TNHX: [] Change method PrintSlient from export PDF to using PrintDriect of DevExpress
 * 20200915 #010 TNHX: [] Add parameter for print direct precription
 * 20210911 #011 QTD: Add new Report BC_KHO_TH
 * 20220613 #012 QTD: Add Search for BC NCC
 * 20220823 #013 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
 * 20221007 #014 DatTB: Thêm nút in trực tiếp phiếu soạn thuốc
 */
namespace ReportServiceProxy
{
    [ServiceContract]
    public interface IAxReportService
    {

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPaymentReportInPdfFormat(decimal paymentID, int FindPatient, long CashAdvanceID, AsyncCallback callback, object state);
        byte[] EndGetPaymentReportInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientCashAdvanceReportInPdfFormat(decimal paymentID,int FindPatient, AsyncCallback callback, object state);
        byte[] EndGetPatientCashAdvanceReportInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRegisteredServiceReportInPdfFormat(string patientName, string address, string gender,
                                                     string deptLocation, string registrationCode, string service, string sequenceNumber, AsyncCallback callback, object state);

        byte[] EndGetRegisteredServiceReportInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptReportInPdfFormat(long paymentId, AsyncCallback callback, object state);
        byte[] EndGetOutPatientReceiptReportInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptReportXMLInPdfFormat(string paymentIdxml, AsyncCallback callback, object state);
        byte[] EndGetOutPatientReceiptReportXMLInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptReportXMLInPdfFormat_V2(string paymentIdxml, AsyncCallback callback, object state);
        byte[] EndGetOutPatientReceiptReportXMLInPdfFormat_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptReportXMLInPdfFormat_V3(string patientPaymentIDXML, string hiPaymentIDXML, AsyncCallback callback, object state);
        void EndGetOutPatientReceiptReportXMLInPdfFormat_V3(out byte[] ReportForPatient, out byte[] ReportForHI, IAsyncResult asyncResult);
        
        /*▼====: #001*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptReportXMLInPdfFormat_V4(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth
            , string parLogoUrl, string parDeptLocIDQMS, AsyncCallback callback, object state);
        byte[] EndGetOutPatientReceiptReportXMLInPdfFormat_V4(IAsyncResult asyncResult);
        /*▲====: #001*/

        /*▼====: #004*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptReportXMLInPdfFormat_V4_Thermal(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth, AsyncCallback callback, object state);
        byte[] EndGetOutPatientReceiptReportXMLInPdfFormat_V4_Thermal(IAsyncResult asyncResult);
        /*▲====: #004*/

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptTestPrintInPdfFormat(AsyncCallback callback, object state);
        byte[] EndGetOutPatientReceiptTestPrintInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetConfirmHIReportInPdfFormat(long ptRegistrationID, AsyncCallback callback, object state);
        byte[] EndGetConfirmHIReportInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetConfirmHITestPrintInPdfFormat(AsyncCallback callback, object state);
        byte[] EndGetConfirmHITestPrintInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptReport_RegistrationDetails_InPdfFormat(long paymentId, List<long> regDetailIDs, AsyncCallback callback, object state);
        byte[] EndGetOutPatientReceiptReport_RegistrationDetails_InPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptReport_PclRequests_InPdfFormat(long paymentId, List<long> pclRequestIDs, AsyncCallback callback, object state);
        byte[] EndGetOutPatientReceiptReport_PclRequests_InPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPclItemsReportInPdfFormat(string patientName, string address, string gender, string deptLocation,
                                     string diagnosis, long PclRequestId, List<long> requestDetailsIdList, AsyncCallback callback, object state);

        byte[] EndGetPclItemsReportInPdfFormat(IAsyncResult asyncResult);

        #region Pharmacies

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCollectionDrugInPdfFormat(long outID, AsyncCallback callback, object state);
        byte[] EndGetCollectionDrugInPdfFormat(IAsyncResult asyncResult);

        //▼====: #005
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCollectionDrugForThermalInPdfFormat(long outID, AsyncCallback callback, object state);
        byte[] EndGetCollectionDrugForThermalInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCollectionDrugBHYTForThermalInPdfFormat(long outID, AsyncCallback callback, object state);
        byte[] EndGetCollectionDrugBHYTForThermalInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCollectionDrugForThermalSummaryInPdfFormat(string outID, string parHospitalName, AsyncCallback callback, object state);
        byte[] EndGetCollectionDrugForThermalSummaryInPdfFormat(IAsyncResult asyncResult);
        //▲====: #005

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardInternalInPdfFormat(long outID, AsyncCallback callback, object state);
        byte[] EndGetOutwardInternalInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPharmacyDemageDrugInPdfFormat(long outID, AsyncCallback callback, object state);
        byte[] EndGetPharmacyDemageDrugInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetReturnDrugInPdfFormat(long outiID, AsyncCallback callback, object state);
        byte[] EndGetReturnDrugInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCardStorageInPdfFormat(long DrugID, string DrugName, long StoreID, string StorageName, string UnitName, AsyncCallback callback, object state);
        byte[] EndGetCardStorageInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetGroupStorageInPdfFormat(long DrugID, string DrugName, string UnitName, DateTime Fromdate, DateTime ToDate, string dateshow, AsyncCallback callback, object state);
        byte[] EndGetGroupStorageInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInOutStocksInPdfFormat(DateTime FromDate, DateTime ToDate, string StorageName, long StoreID, string dateshow, AsyncCallback callback, object state);
        byte[] EndGetInOutStocksInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInWardDrugSupplierInPdfFormat(long InvID, AsyncCallback callback, object state);
        byte[] EndGetInWardDrugSupplierInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestPharmacyInPdfFormat(long RequestID, AsyncCallback callback, object state);
        byte[] EndGetRequestPharmacyInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestInPdfFormat(long RequestID, AsyncCallback callback, object state);
        byte[] EndGetRequestInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugExpiryInPdfFormat(long? StoreID, int Type, string Message, string showdate, AsyncCallback callback, object state);
        byte[] EndGetDrugExpiryInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetBaoCaoNopTienHangNgayInPdfFormat(DateTime Date, AsyncCallback callback, object state);
        byte[] EndGetBaoCaoNopTienHangNgayInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetBaoCaoPhatThuocHangNgayInPdfFormat(DateTime Date, AsyncCallback callback, object state);
        byte[] EndGetBaoCaoPhatThuocHangNgayInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPhieuKiemKeInPdfFormat(long ID, AsyncCallback callback, object state);
        byte[] EndGetPhieuKiemKeInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBaocaoNhapThuocHangThangPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID, AsyncCallback callback, object state);
        byte[] EndBaocaoNhapThuocHangThangPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBaocaoNhapThuocHangThangInvoicePdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID, AsyncCallback callback, object state);
        byte[] EndBaocaoNhapThuocHangThangInvoicePdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBaocaoXuatNoiBoTheoNguoiMuaPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID, int OutTo, long TypID, AsyncCallback callback, object state);
        byte[] EndBaocaoXuatNoiBoTheoNguoiMuaPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBaocaoXuatNoiBoTheoTenThuocPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID, int OutTo, long TypID, AsyncCallback callback, object state);
        byte[] EndBaocaoXuatNoiBoTheoTenThuocPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBangKeChungTuThanhToanPdfFormat(long? ID, AsyncCallback callback, object state);
        byte[] EndBangKeChungTuThanhToanPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPhieuDeNghiThanhToanPdfFormat(long? ID, AsyncCallback callback, object state);
        byte[] EndPhieuDeNghiThanhToanPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBaocaoXuatThuocChoBHPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID, AsyncCallback callback, object state);
        byte[] EndBaocaoXuatThuocChoBHPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSupplierTemplateInPdfFormat(AsyncCallback callback, object state);
        byte[] EndGetSupplierTemplateInPdfFormat(IAsyncResult asyncResult);
        #endregion

        #region Transactions

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPaymentVisistorReportInPdfFormat(long paymentID, long OutiID, AsyncCallback callback, object state);
        byte[] EndGetPaymentVisistorReportInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTemplate25aInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, AsyncCallback callback, object state);
        byte[] EndGetTemplate25aInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTemplate25aTHInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, AsyncCallback callback, object state);
        byte[] EndGetTemplate25aTHInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTemplate26aInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, AsyncCallback callback, object state);
        byte[] EndGetTemplate26aInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTemplate26aTHInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, AsyncCallback callback, object state);
        byte[] EndGetTemplate26aTHInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTemplate20NgoaiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, AsyncCallback callback, object state);
        byte[] EndGetTemplate20NgoaiTruInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTemplate20NoiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, AsyncCallback callback, object state);
        byte[] EndGetTemplate20NoiTruInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTemplate21NgoaiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, AsyncCallback callback, object state);
        byte[] EndGetTemplate21NgoaiTruInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTemplate21NoiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, AsyncCallback callback, object state);
        byte[] EndGetTemplate21NoiTruInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTemplate38aInPdfFormat(long TransactionID, long PtRegistrationID, AsyncCallback callback, object state);
        byte[] EndGetTemplate38aInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginThongKeDoanhThu(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, AsyncCallback callback, object state);
        byte[] EndThongKeDoanhThu(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetEstimationPharmacyInPdfFormat(long PharmacyEstimatePoID, AsyncCallback callback, object state);
        byte[] EndGetEstimationPharmacyInPdfFormat(IAsyncResult asyncResult);

        #endregion

        #region consultations
        //▼===== #010
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptEPrescriptionInPdfFormat(long parIssueID, bool IsPsychotropicDrugs, bool IsFuncfoodsOrCosmetics, int parTypeOfForm
            , int parOrganizationUseSoftware, int nNumCopies, string parHospitalCode
            , int PrescriptionOutPtVersion, string PrescriptionMainRightHeader, string PrescriptionSubRightHeader
            , string DepartmentOfHealth, string HospitalName, string HospitalAddress, string KBYTLink, bool HasPharmacyDrug, bool IsSeparatePrescription, string ReportHospitalPhone
            , AsyncCallback callback, object state);
        byte[] EndGetXRptEPrescriptionInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptEPrescriptionInPdfFormat_InPt(long parIssueID, int parTypeOfForm, int nNumCopies, string parHospitalCode
            , int PrescriptionInPtVersion, string PrescriptionMainRightHeader, string PrescriptionSubRightHeader
            , string DepartmentOfHealth, string HospitalName, string HospitalAddress, string KBYTLink, string ReportHospitalPhone
            , AsyncCallback callback, object state);
        byte[] EndGetXRptEPrescriptionInPdfFormat_InPt(IAsyncResult asyncResult);
        //▲===== #010

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptPCLFormInPdfFormat(string parDoctorName, string parDoctorPhone, long parPatientID, long parPCLFormID, AsyncCallback callback, object state);
        byte[] EndGetXRptPCLFormInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptPCLFormRequestInPdfFormat(long parPCLFormID, long parPtPCLAppID, AsyncCallback callback, object state);
        byte[] EndGetXRptPCLFormRequestInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptPCLLabResultInPdfFormat(bool parAllHistories, long parPtPCLLabReqID, AsyncCallback callback, object state);
        byte[] EndGetXRptPCLLabResultInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptPMRInPdfFormat(long parPatientID, AsyncCallback callback, object state);
        byte[] EndGetXRptPMRInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllXRptAppointment(PatientAppointment parPatientAppointment, AsyncCallback callback, object state);
        byte[] EndGetAllXRptAppointment(IAsyncResult asyncResult);

        #endregion

        #region OutwardDrugMedDept

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDept_ByoutiID_Rpt_InPdfFormat(long outiID, AsyncCallback callback, object state);
        byte[] EndOutwardDrugMedDept_ByoutiID_Rpt_InPdfFormat(IAsyncResult asyncResult);
        #endregion

        #region RptNhapXuatDenKhoaPhong

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRptNhapXuatDenKhoaPhong(long vMedProductType, DateTime fromDate, DateTime toDate, long storeID,long? StoreClinicID, bool? IsShowHave, bool? IsShowHaveMedProduct, AsyncCallback callback, object state);
        String EndRptNhapXuatDenKhoaPhong(IAsyncResult asyncResult);

        #endregion
        //▼====: #001
        // 20180824 TNHX: [BM0000034] Add service for PhieuChiDinh
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientPhieuChiDinhReportXMLInPdfFormat(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth, string parLogoUrl, AsyncCallback callback, object state);
        byte[] EndGetOutPatientPhieuChiDinhReportXMLInPdfFormat(IAsyncResult asyncResult);
        //▲====: #001
        //▼====: #002
        //▼====: #003
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientPhieuMienGiamNgoaiTruReportInPdfFormat(long PtRegistrationID, long PromoDiscProgID, long totalMienGiam, string parHospitalName, string parHospitalAddress, AsyncCallback callback, object state);
        byte[] EndGetOutPatientPhieuMienGiamNgoaiTruReportInPdfFormat(IAsyncResult asyncResult);
        //▲====: #002
        //▲====: #003

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInWardVTYTTHInPdfFormat(long InvID, AsyncCallback callback, object state);
        byte[] EndGetInWardVTYTTHInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInWardVaccineInPdfFormat(long InvID, AsyncCallback callback, object state);
        byte[] EndGetInWardVaccineInPdfFormat(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInWardChemicalInPdfFormat(long InvID, AsyncCallback callback, object state);
        byte[] EndGetInWardChemicalInPdfFormat(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInWardBloodInPdfFormat(long InvID, AsyncCallback callback, object state);
        byte[] EndGetInWardBloodInPdfFormat(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInWardThanhTrungInPdfFormat(long InvID, AsyncCallback callback, object state);
        byte[] EndGetInWardThanhTrungInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInWardVPPInPdfFormat(long InvID, AsyncCallback callback, object state);
        byte[] EndGetInWardVPPInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInWardVTTHInPdfFormat(long InvID, AsyncCallback callback, object state);
        byte[] EndGetInWardVTTHInPdfFormat(IAsyncResult asyncResult);
        
        //▼====: #006
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockValueDrugDept_TV(string ReportTitle, DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, long SelectedDrugDeptProductGroupReportType
            , string ReportHospitalName, string ReportLogoUrl, long BidID, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockValueDrugDept_TV(IAsyncResult asyncResult);
        //▲====: #006        
        //▼====: #008
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockGeneral(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockGeneral(IAsyncResult asyncResult);
        //▲====: #008

        //▼====: QTD NTX Thuốc toàn Khoa DƯỢC
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockGeneral_BHYT(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, string DateShow, long V_MedProductType, bool IsBHYT, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockGeneral_BHYT(IAsyncResult asyncResult);

        //▼====: QTD NTX KT - DUOC
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockValueDrugDept_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockValueDrugDept_KT(IAsyncResult asyncResult);

        //▼====: QTD NTX KT - KHOAPHONG
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockValueClinicDept_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockValueClinicDept_KT(IAsyncResult asyncResult);

        //▼====: QTD NTX KT - NHATHUOC
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockValue_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, string DateShow, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockValue_KT(IAsyncResult asyncResult);

        //▼====: QTD NTX Chitiet KT - DUOC
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockValueDrugDeptDetails_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockValueDrugDeptDetails_KT(IAsyncResult asyncResult);

        //▼====: QTD NTX Chitiet KT - KHOAPHONG
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockClinicDeptDetails_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockClinicDeptDetails_KT(IAsyncResult asyncResult);

        //▼====: QTD NX Theo muc dich KT - DUOC
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRpt_MedDeptInOutStatisticsDetail(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string parDepartmentOfHealth, AsyncCallback callback, object state);
        byte[] EndGetXRpt_MedDeptInOutStatisticsDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRpt_MedDeptInOutStatisticsDetail_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string parDepartmentOfHealth, AsyncCallback callback, object state);
        byte[] EndGetXRpt_MedDeptInOutStatisticsDetail_V2(IAsyncResult asyncResult);

        //▼====: QTD NX Theo muc dich KT - KHOAPHONG
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRpt_ClinicDeptInOutStatisticsDetail(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string parDepartmentOfHealth, AsyncCallback callback, object state);
        byte[] EndGetXRpt_ClinicDeptInOutStatisticsDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRpt_ClinicDeptInOutStatisticsDetail_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string parDepartmentOfHealth, AsyncCallback callback, object state);
        byte[] EndGetXRpt_ClinicDeptInOutStatisticsDetail_V2(IAsyncResult asyncResult);

        //▼====: QTD NX Theo muc dich KT - NHATHUOC
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRpt_DrugInOutStatisticsDetail(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, string ReportHospitalName, string parDepartmentOfHealth, AsyncCallback callback, object state);
        byte[] EndGetXRpt_DrugInOutStatisticsDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRpt_DrugInOutStatisticsDetail_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, string ReportHospitalName, string parDepartmentOfHealth, AsyncCallback callback, object state);
        byte[] EndGetXRpt_DrugInOutStatisticsDetail_V2(IAsyncResult asyncResult);

        //▼====: #007
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPharmacyXRptInOutStocks_TV(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, string ReportLogoUrl, AsyncCallback callback, object state);
        byte[] EndGetPharmacyXRptInOutStocks_TV(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPharmacyXRptInOutStockValue_TV(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, string ReportLogoUrl, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetPharmacyXRptInOutStockValue_TV(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockValueClinicDept_TV(string ReportTitle, DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, long SelectedDrugDeptProductGroupReportType
            , string ReportHospitalName, string ReportLogoUrl, long BidID, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockValueClinicDept_TV(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockClinicDept_TV(string ReportTitle, DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, bool ViewBefore20150331
            , string ReportLogoUrl, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockClinicDept_TV(IAsyncResult asyncResult);
        //▲====: #007

        //▼====: #009
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientPhieuChiDinhReportXML(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth, string parLogoUrl, AsyncCallback callback, object state);
        byte[] EndGetOutPatientPhieuChiDinhReportXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPatientReceiptReportXML_V4(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth, string parLogoUrl, AsyncCallback callback, object state);
        byte[] EndGetOutPatientReceiptReportXML_V4(IAsyncResult asyncResult);
        //▲====: #009

        //20210806 QTD Add new Method XRptDrugDeptCardStorage
        //Accountant
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptDrugDeptCardStorage(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl, AsyncCallback asyncCallback, object state);
        byte[] EndGetXRptDrugDeptCardStorage(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptClinicDeptCardStorage(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl, AsyncCallback asyncCallback, object state);
        byte[] EndGetXRptClinicDeptCardStorage(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptPharmacyCardStorage(long StoreID, long GenMedProductID, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl, AsyncCallback asyncCallback, object state);
        byte[] EndGetXRptPharmacyCardStorage(IAsyncResult asyncResult);
        //End Accountant

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptCardStorageDrugDept(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl, AsyncCallback asyncCallback, object state);
        byte[] EndGetXRptCardStorageDrugDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptCardStorage(long StoreID, long GenMedProductID, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl, AsyncCallback asyncCallback, object state);
        byte[] EndGetXRptCardStorage(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptCardStorageClinicDept(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl, AsyncCallback asyncCallback, object state);
        byte[] EndGetXRptCardStorageClinicDept(IAsyncResult asyncResult);

        //▼====: #011
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptGeneralInOutStatistics_V3(DateTime? FromDate, DateTime? ToDate, string DateShow, string ReportHospitalName, string ReportHospitalAddress, AsyncCallback callback, object state);
        byte[] EndGetXRptGeneralInOutStatistics_V3(IAsyncResult asyncResult);
        //▲====: #011
      
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLImagingResultInPdfFormat(long V_ReportForm,long PCLImgResultID, long V_PCLRequestType, string reportHospitalName, string hospitalCode,
            string reportDepartmentOfHealth,string reportHospitalAddress, bool isApplyPCRDual, AsyncCallback callback, object state);
        byte[] EndGetPCLImagingResultInPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptInOutStockValueClinicDept_KT_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetXRptInOutStockValueClinicDept_KT_V2(IAsyncResult asyncResult);
		
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptPCLDepartmentLaboratoryResultReportModel_TV3PdfFormat(string parPrescriptionMainRightHeader, string parPrescriptionSubRightHeader, string parHeadOfLaboratoryFullName 
            , string parHeadOfLaboratoryPositionName, string parDepartmentOfHealth, string parHospitalAddress, string parHospitalName, string parHospitalCode, int parPatientID, int parPatientPCLReqID
            , int parV_PCLRequestType, int parPatientFindBy, string parStaffName, AsyncCallback callback, object state);
        byte[] EndGetXRptPCLDepartmentLaboratoryResultReportModel_TV3PdfFormat(IAsyncResult asyncResult);
        //▼====: #012
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXRptBCNhapTuNCC(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, string DateShow, long V_MedProductType, string ReportHospitalName, AsyncCallback callback, object state);
        byte[] EndGetXRptBCNhapTuNCC(IAsyncResult asyncResult);
        //▲====: #12

        //▼====: #013
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginInHuongDanSuDungThuocWithoutView(long PtRegistrationID, bool IsInsurance, AsyncCallback callback, object state);
        byte[] EndInHuongDanSuDungThuocWithoutView(IAsyncResult asyncResult);
        //▲====: #013
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetXRpt_Temp12InPdfFormat(long TransactionID, long PtRegistrationID, DateTime? FromDate, DateTime? ToDate, bool ViewByDate, string StaffName
            , long DeptID, string DeptName, long RegistrationType, string parHospitalAdress, string parHospitalName, string parDepartmentOfHealth, bool IsPatientCOVID
           , AsyncCallback callback, object state);
        byte[] EndGetXRpt_Temp12InPdfFormat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetXRpt_Temp12_6556InPdfFormat(long TransactionID, long PtRegistrationID, DateTime? FromDate, DateTime? ToDate, bool ViewByDate, string StaffName
            , long DeptID, string DeptName, long RegistrationType, string parHospitalAdress, string parHospitalName, string parDepartmentOfHealth
           , AsyncCallback callback, object state);
        byte[] EndGetXRpt_Temp12_6556InPdfFormat(IAsyncResult asyncResult);
        //▼==== #014
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginInPhieuChoSoanThuocWithoutView(long PtRegistrationID, AsyncCallback callback, object state);
        byte[] EndInPhieuChoSoanThuocWithoutView(IAsyncResult asyncResult);
        //▲==== #014
        //▼==== #014
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPDFResulstToSign(string ListSendTransactionXml, int LaboratoryResultVersion, string StaffName
            , string PrescriptionMainRightHeader, string PrescriptionSubRightHeader, string parHeadOfLaboratoryFullName, string parHeadOfLaboratoryPositionName
            , string reportDepartmentOfHealth, string reportHospitalAddress, string reportHospitalName, string HospitalCode
            , string ServicePool, string PDFFileResultToSignPath, string PDFFileResultSignedPath
            , string FtpUsername, string FtpPassword, string FtpUrl
            , AsyncCallback callback, object state);
        string EndGetPDFResulstToSign(IAsyncResult asyncResult);
        //▲==== #014
    }
}
