using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using DevExpress.XtraReports.Service;
using System.IO;
using ErrorLibrary;
/*
 * 20181030 #001 TNHX: [BM0002176] Add params HospitalName, DepartmentOfHealth for PhieuChiDinh and Receipt
 * 20181201 #002 TNHX: [BM0005312] Print report PhieuMienGiamNgoaiTru
 * 20181219 #003 TNHX: [BM0005447] Fix print silent of PhieuChiDinh, PhieuMienGiam
 * 20190228 #005 TNHX: [] Add thermal report for BienLaiThuTien
 * 20190422 #006 TNHX: [BM0006716] Apply thermal report for Cashier at BHYT, change services at Pharmacy
 * 20190917 #007 TNHX: [BM0013247] Apply search for XRptInOutStockValueDrugDept_TV base on config "AllowSearchInReport"
 * 20191029 #008 TNHX: [] Apply search for XRptInOutStock base on config "AllowSearchInReport" + XRptInOutStockValue
 * 20200128 #009 TNHX: [] Apply search for XRptInOutStockGeneral base on config "AllowSearchInReport"
 * 20200816 #010 TNHX: [] Change method PrintSlient from export PDF to using PrintDriect of DevExpress
 * 20210122 #011 TNHX: [] Add parameter for Prescriptions
 * 20210911 #012 QTD: Add new report BH_KHO_TH
 * 20220211 #013 QTD: Add new report
 * 20220215 #014 QTD:  Add report XRpttPCLDepartmentLaboratoryResultReportModel_TV3
 * 20220613 #015 QTD: Add search in BC nhap NCC
 * 20220823 #016 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
 * 20221007 #017 DatTB: Thêm nút in trực tiếp phiếu soạn thuốc
 * 20221021 #018 TNHX: Func lấy danh sách kết quả xét nghiệm thành PDF để chờ ký số
 */
namespace ReportService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IAxReportService : IReportService
    {
        [OperationContract]
        string WhoAmI();

        [OperationContract]
        Stream GetReportDataInPdfFormat_Test();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetPaymentReportInPdfFormat(decimal paymentID, int FindPatient, long CashAdvanceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetPatientCashAdvanceReportInPdfFormat(decimal paymentID, int FindPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetRegisteredServiceReportInPdfFormat(string patientName, string address, string gender, string deptLocation, string registrationCode, string service, string sequenceNumber);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetPclItemsReportInPdfFormat(string patientName, string address, string gender, string deptLocation, string diagnosis, long pclRequestId, List<long> requestDetailsIdList );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientReceiptReportInPdfFormat(long paymentId);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientReceiptReportXMLInPdfFormat(string paymentIdxml);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientReceiptReportXMLInPdfFormat_V2(string paymentIdxml);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GetOutPatientReceiptReportXMLInPdfFormat_V3(string patientPaymentIDXML, string hiPaymentIDXML, out byte[] ReportForPatient, out byte[] ReportForHI);

        /*▼====: #004*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientReceiptReportXMLInPdfFormat_V4(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth
            , string parLogoUrl, string parDeptLocIDQMS, bool ApplyNewMethod);
        /*▲====: #004*/

        /*▼====: #005*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientReceiptReportXMLInPdfFormat_V4_Thermal(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth, bool ApplyNewMethod);
        /*▲====: #005*/

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientReceiptTestPrintInPdfFormat();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetConfirmHIReportInPdfFormat(long ptRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetConfirmHITestPrintInPdfFormat();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientReceiptReport_RegistrationDetails_InPdfFormat(long paymentId, List<long> regDetailIDs);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientReceiptReport_PclRequests_InPdfFormat(long paymentId, List<long> pclRequestIDs);

        #region Pharmacies

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetCollectionDrugInPdfFormat(long outID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutwardInternalInPdfFormat(long outID);

        //▼====: #006
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetCollectionDrugForThermalInPdfFormat(long outID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetCollectionDrugBHYTForThermalInPdfFormat(long outID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetCollectionDrugForThermalSummaryInPdfFormat(string outID, string parHospitalName);
        //▲====: #006

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetPharmacyDemageDrugInPdfFormat(long outID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetReturnDrugInPdfFormat(long outiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetCardStorageInPdfFormat(long DrugID, string DrugName, long StoreID, string StorageName, string UnitName,DateTime Fromdate, DateTime ToDate, string dateshow);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetGroupStorageInPdfFormat(long DrugID, string DrugName, string UnitName, DateTime Fromdate, DateTime ToDate, string dateshow);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetInOutStocksInPdfFormat(DateTime FromDate, DateTime ToDate, string StorageName, long StoreID,string dateshow);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetInWardDrugSupplierInPdfFormat(long InvID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetRequestPharmacyInPdfFormat(long RequestID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetRequestInPdfFormat(long RequestID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetDrugExpiryInPdfFormat(long? StoreID,int Type,string Message,string showdate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetEstimationPharmacyInPdfFormat(long PharmacyEstimatePoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetBaoCaoNopTienHangNgayInPdfFormat(DateTime Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetBaoCaoPhatThuocHangNgayInPdfFormat(DateTime Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetPhieuKiemKeInPdfFormat(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetSupplierTemplateInPdfFormat();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream BaocaoBanThuocTongHopPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream BaocaoTraThuocTongHopPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream BaocaoXuatThuocChoBHPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream BaocaoNhapThuocHangThangPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag,string StoreName,long? StoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream BaocaoNhapThuocHangThangInvoicePdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream BaocaoXuatNoiBoTheoNguoiMuaPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID,int OutTo,long TypID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream BaocaoXuatNoiBoTheoTenThuocPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag, string StoreName, long? StoreID, int OutTo, long TypID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream BangKeChungTuThanhToanPdfFormat(long? ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream PhieuDeNghiThanhToanPdfFormat(long? ID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetDuTruDuaVaoHeSoAnToanPdfFormat();
        #endregion

        #region Transactions

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetPaymentVisistorReportInPdfFormat(long paymentID,long OutiID,string LyDo);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetTemplate25aInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetTemplate25aTHInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetTemplate26aInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetTemplate26aTHInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetTemplate20NgoaiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetTemplate20NoiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetTemplate21NgoaiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetTemplate21NoiTruInPdfFormat(DateTime FromDate, DateTime ToDate, string ShowDate, int Quarter, int Month, int Year, int flag);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetTemplate38aInPdfFormat(long transactionID, long ptRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ThongKeDoanhThu(DateTime fromDate, DateTime toDate, string showDate, int quarter, int month, int year, int flag);

        #endregion

        //▼====: #011
        #region consultations
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXRptEPrescriptionInPdfFormat(long parIssueID, bool IsPsychotropicDrugs, bool IsFuncfoodsOrCosmetics, int parTypeOfForm
            , int parOrganizationUseSoftware, int nNumCopies, string parHospitalCode
            , int PrescriptionOutPtVersion, string PrescriptionMainRightHeader, string PrescriptionSubRightHeader
            , string DepartmentOfHealth, string HospitalName, string HospitalAddress, string KBYTLink, bool HasPharmacyDrug , bool IsSeparatePrescription, string ReportHospitalPhone
            );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXRptEPrescriptionInPdfFormat_InPt(long parIssueID, int parTypeOfForm, int nNumCopies, string parHospitalCode
            , int PrescriptionInPtVersion, string PrescriptionMainRightHeader, string PrescriptionSubRightHeader
            , string DepartmentOfHealth, string HospitalName, string HospitalAddress, string KBYTLink, string ReportHospitalPhone
            );
        //▲====: #011

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXRptPCLFormInPdfFormat(string parDoctorName, string parDoctorPhone, long parPatientID, long parPCLFormID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXRptPCLFormRequestInPdfFormat(long parPCLFormID, long parPtPCLAppID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXRptPCLLabResultInPdfFormat(bool parAllHistories, long parPtPCLLabReqID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXRptPMRInPdfFormat(long parPatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetAllXRptAppointment(PatientAppointment parPatientAppointment);


        #endregion


        #region RptNhapXuatDenKhoaPhong
        [OperationContract]
        [FaultContract(typeof(AxException))]
        String RptNhapXuatDenKhoaPhong(long vMedProductType, DateTime fromDate, DateTime toDate, long storeID, long? StoreClinicID, bool? IsShowHave, bool? IsShowHaveMedProduct);



        #endregion
        
        //▼====: #001
        // 20181005 TNHX: [BM0000034] Phieu Chi Dinh
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientPhieuChiDinhReportXMLInPdfFormat(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth, string parLogoUrl, bool ApplyNewMethod);
        //▲====: #001
        //▼====: #003
        //▼====: #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetOutPatientPhieuMienGiamNgoaiTruReportInPdfFormat(long PtRegistrationID, long PromoDiscProgID, long totalMienGiam, string parHospitalName, string parHospitalAddress);
        //▲====: #002
        //▲====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetInWardVTYTTHInPdfFormat(long InvID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetInWardVaccineInPdfFormat(long InvID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetInWardChemicalInPdfFormat(long InvID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetInWardBloodInPdfFormat(long InvID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetInWardThanhTrungInPdfFormat(long InvID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetInWardVPPInPdfFormat(long InvID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetInWardVTTHInPdfFormat(long InvID);

        //▼====: #007
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockValueDrugDept_TV(string ReportTitle, DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, long SelectedDrugDeptProductGroupReportType
            , string ReportHospitalName, string ReportLogoUrl, long BidID);
        //▲====: #007
        //▼====: #009
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockGeneral(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, string ReportHospitalName);
        //▲====: #009

        //▼====: QTD NXT Thuốc toàn Khoa DƯỢC
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockGeneral_BHYT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, bool IsBHYT, string ReportHospitalName);
        //▲====:

        //▼====: #NXT KT
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockValueDrugDept_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName);
        //▲====

        //▼====: #NXT KT - KHOAPHONG
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockValueClinicDept_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName);
        //▲====

        //▼====: #NXT KT - NHATHUOC
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockValue_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, string ReportHospitalName);
        //▲====

        //▼====: #NXT CT KT
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockValueDrugDeptDetails_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1,  string ReportHospitalName);
        //▲====

        //▼====: #NXT CT KT - KHOAPHONG
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockClinicDeptDetails_KT(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName);
        //▲====

        //▼====: #NX Theo muc dich KT - DUOC
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRpt_MedDeptInOutStatisticsDetail(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string DepartmentOfHealth);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRpt_MedDeptInOutStatisticsDetail_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string DepartmentOfHealth);

        //▼====: #NX Theo muc dich KT - KHOAPHONG
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRpt_ClinicDeptInOutStatisticsDetail(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string DepartmentOfHealth);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRpt_ClinicDeptInOutStatisticsDetail_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, long V_MedProductType, string ReportHospitalName, string DepartmentOfHealth);

        //▼====: #NX Theo muc dich KT - NHATHUOC
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRpt_DrugInOutStatisticsDetail(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, string ReportHospitalName, string DepartmentOfHealth);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRpt_DrugInOutStatisticsDetail_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
        , long StoreID, long StoreIDIn, long StoreIDOut, long PurposeInID, long PurposeOutID, string ReportHospitalName, string DepartmentOfHealth);

        //▼====: #008
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetPharmacyXRptInOutStocks_TV(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, string ReportLogoUrl);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetPharmacyXRptInOutStockValue_TV(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, string ReportLogoUrl, string ReportHospitalName);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockValueClinicDept_TV(string ReportTitle, DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, long SelectedDrugDeptProductGroupReportType
            , string ReportHospitalName, string ReportLogoUrl, long BidID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockClinicDept_TV(string ReportTitle, DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, bool ViewBefore20150331
            , string ReportLogoUrl);
        //▲====: #008
        //▼====: #010
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetOutPatientReceiptReportXML_V4(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth, string parLogoUrl, bool ApplyNewMethod);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetOutPatientPhieuChiDinhReportXML(string paymentIdxml, string parHospitalName, string parDepartmentOfHealth, string parLogoUrl, bool ApplyNewMethod);
        //▲====: #010

        //20210806 QTD New method for GetCardStorage
        //Accountant
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptDrugDeptCardStorage(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptClinicDeptCardStorage(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptPharmacyCardStorage(long StoreID, long GenMedProductID, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl);
        //End Accountant

        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptCardStorageDrugDept(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptCardStorage(long StoreID, long GenMedProductID, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptCardStorageClinicDept(long StoreID, long GenMedProductID, long V_MedProductType, string BrandName, string StorageName, string UnitName, DateTime? FromDate, DateTime? ToDate
            , string dateshow, string Code, string parLogoUrl);

        //▼====: #011
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptGeneralInOutStatistics_V3(DateTime? FromDate, DateTime? ToDate, string DateShow, string ReportHospitalName, string ReportHospitalAddress);
        //▲====: #011

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetPCLImagingResultInPdfFormat(long V_ReportForm, long PCLImgResultID, long V_PCLRequestType, string reportHospitalName, string hospitalCode,
            string reportDepartmentOfHealth, string reportHospitalAddress, bool isApplyPCRDual);

        //▼====: #013
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptInOutStockValueClinicDept_KT_V2(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, long RefGenDrugCatID_1, string ReportHospitalName);
        //▲====: #013
        //▼====: #014
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXRptPCLDepartmentLaboratoryResultReportModel_TV3PdfFormat(string parPrescriptionMainRightHeader, string parPrescriptionSubRightHeader, string parHeadOfLaboratoryFullName
            , string parHeadOfLaboratoryPositionName, string parDepartmentOfHealth, string parHospitalAddress, string parHospitalName, string parHospitalCode, int parPatientID, int parPatientPCLReqID
            , int parV_PCLRequestType, int parPatientFindBy, string parStaffName);
        //▲====: #014
        //▼====: #015
        [OperationContract]
        [FaultContract(typeof(AxException))]
        byte[] GetXRptBCNhapTuNCC(DateTime? FromDate, DateTime? ToDate, string StorageName
            , long StoreID, string DateShow, long V_MedProductType, string ReportHospitalName);
        //▲====: #015
               
        //▼====: #016
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream InHuongDanSuDungThuocWithoutView(long PtRegistrationID, bool IsInsurance);
        //▲====: #016

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXRpt_Temp12InPdfFormat(long TransactionID, long PtRegistrationID, DateTime? FromDate, DateTime? ToDate, bool ViewByDate, string StaffName
            , long DeptID, string DeptName, long RegistrationType, string parHospitalAdress, string parHospitalName, string parDepartmentOfHealth, bool IsPatientCOVID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream GetXRpt_Temp12_6556InPdfFormat(long TransactionID, long PtRegistrationID, DateTime? FromDate, DateTime? ToDate, bool ViewByDate, string StaffName
            , long DeptID, string DeptName, long RegistrationType, string parHospitalAdress, string parHospitalName, string parDepartmentOfHealth);

        //▼==== #017
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream InPhieuChoSoanThuocWithoutView(long PtRegistrationID);
        //▲==== #017
        //▼==== #018
        // step 1: lấy kết quả pdf từ danh sách gửi qua.
        [OperationContract]
        [FaultContract(typeof(AxException))]
        string GetPDFResulstToSign(string ListSendTransactionXml, int LaboratoryResultVersion, string StaffName
            , string PrescriptionMainRightHeader, string PrescriptionSubRightHeader, string parHeadOfLaboratoryFullName, string parHeadOfLaboratoryPositionName
            , string reportDepartmentOfHealth, string reportHospitalAddress, string reportHospitalName, string HospitalCode
            , string ServicePool, string PDFFileResultToSignPath, string PDFFileResultSignedPath
            , string FtpUsername, string FtpPassword, string FtpUrl);
        //▲==== #018
    }
}
