/*
 * 201470803 #001 CMN: Add HI Store Service
 * 20181124 #002 TTM: BM 0005309: Load dữ liệu chi tiết phiếu xuất kho BHYT màn hình Nhập trả khoa dược
 * 20190928 #003 TNHX: BM 0016380: Get OutwardDrugInvoices from InternalExport
 * 20200902 #004 TNHX: BM : Thêm điều kiện tìm thuốc theo hoạt chất
*/
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;
using System.Runtime.Serialization;
using AxLogging;
using ErrorLibrary.Resources;
using System.Data.SqlClient;
using System.Linq;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;
using System.Collections.ObjectModel;

namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class InwardDrugService : eHCMS.WCFServiceCustomHeader, IInwardDrug
    {
        public InwardDrugService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        #region 0. Supplier member

        public IList<Supplier> GetAllSupplierCbx(int supplierType = (int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllSupplierCbx(supplierType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllSupplierCbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SUPPLIER_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 1. RefShelfDrugLocation member

        public IList<RefShelfDrugLocation> GetRefShelfDrugLocationAutoComplete(string Name, int pageIndex, int pageSize)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefShelfDrugLocationAutoComplete(Name, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefShelfDrugLocationAutoComplete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_LOCATION_CANNOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InsertRefShelfDrugLocation(RefShelfDrugLocation Location, out long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InsertRefShelfDrugLocation(Location, out ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InsertRefShelfDrugLocation. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_LOCATION_CANNOT_ADD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 2. Inwarddrug member
        public InwardDrugInvoice GetInwardInvoiceDrugByID(long invoiceID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugInvoiceByID(invoiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetInwardInvoiceDrugByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARDINV_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public IList<InwardDrug> GetspInwardDrugDetailsByID(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetspInwardDrugDetailsByID(inviID, pageSize, pageIndex, bCountTotal, out totalCount, out TongTienSPChuaVAT
, out CKTrenSP
, out TongTienTrenSPDaTruCK
, out TongCKTrenHoaDon
, out TongTienHoaDonCoThueNK
, out TongTienHoaDonCoVAT
, out TotalVATDifferenceAmount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetspInwardDrugDetailsByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public List<InwardDrug> InwardDrugDetails_ByID(long inviID
                                                                                  , out decimal TongTienSPChuaVAT
                                                                                  , out decimal CKTrenSP
                                                                                  , out decimal TongTienTrenSPDaTruCK
                                                                                  , out decimal TongCKTrenHoaDon
                                                                                  , out decimal TongTienHoaDonCoThueNK
                                                                                  , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugDetails_ByID(inviID, out TongTienSPChuaVAT
                , out CKTrenSP
                , out TongTienTrenSPDaTruCK
                , out TongCKTrenHoaDon
                , out TongTienHoaDonCoThueNK
                , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InwardDrugDetails_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public bool InwardDrugInvoice_Save(InwardDrugInvoice InvoiceDrug, out long inwardid, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugInvoice_Save(InvoiceDrug, out inwardid, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddInwardInvoiceDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARDINV_CANNOT_ADD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddInwardInvoiceDrug(InwardDrugInvoice InvoiceDrug, bool IsNotCheckInvalid, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardDrugInvoice(InvoiceDrug, IsNotCheckInvalid, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddInwardInvoiceDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARDINV_CANNOT_ADD);


                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }
        public int UpdateInwardInvoiceDrug(InwardDrugInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardDrugInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateInwardInvoiceDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARDINV_CANNOT_UPDATE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public int DeleteInwardInvoiceDrug(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardDrugInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateInwardInvoiceDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARDINV_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InwardDrug_Insert(PharmacyPurchaseOrderDetail invoicedrug, long inviID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrug_Insert(invoicedrug, inviID);
            }
            catch (SqlException ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InwardDrug_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARD_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InwardDrug_InsertList(List<PharmacyPurchaseOrderDetail> OrderDetails, long inviID)
        {
            OrderDetails = OrderDetails.Where(x => x.DrugID > 0).ToList();
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                RefDrugGenericDetailsProvider.Instance.InwardDrug_Insert(OrderDetails[i], inviID);
            }
            return true;
        }

        public int InwardDrug_Update(InwardDrug invoicedrug, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrug_Update(invoicedrug, StaffID);
            }
            catch (SqlException ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InwardDrug_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARD_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool AddInwardDrug(InwardDrug invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardDrug(invoicedrug);
            }
            catch (SqlException ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InwardDrug_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARD_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int DeleteInwardDrug(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardDrug(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteInwardDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARD_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugInvoice> GetAllInwardInvoiceDrug(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllInwardDrugInvoice(pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllInwardInvoiceDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARDINV_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDrugInvoice> SearchInwardInvoiceDrug(InwardInvoiceSearchCriteria criteria, long? TypID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardDrugInvoice(criteria, TypID, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchInwardInvoiceDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARDINV_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public RefGenericDrugDetail GetRefGenericDrugDetail(long drugID, long? drugVersionID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefGenericDrugDetail(drugID, drugVersionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefGenericDrugDetail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_GET_REFGENERICDRUGDETAIL);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int InwardDrugInvoice_SaveXML(InwardDrugInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward,out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugInvoice_SaveXML(InvoiceDrug, DSPTModifiedDate_Outward,out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardDrugInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion

        #region 3. Outwarddrug member
        //KMx: Hàm này không còn sử dụng nữa, chuyển sang hàm AddOutWardDrugInvoiceReturnVisitor_Pst (SaleAndOutward.cs) (26/02/2014 10:58)
        public bool AddOutWardDrugInvoiceReturnVisitor(OutwardDrugInvoice InvoiceDrug, long ReturnStaffID, out long outwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddOutWardDrugInvoiceReturnVisitor(InvoiceDrug, ReturnStaffID, out outwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddOutWardDrugInvoiceReturnVisitor. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_AddOutWardDrugInvoiceReturnVisitor);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int OutWardDrugInvoiceVisitor_Cancel(OutwardDrugInvoice InvoiceDrug, out long TransItemID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutWardDrugInvoiceVisitor_Cancel(InvoiceDrug, out TransItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutWardDrugInvoiceVisitor_Cancel. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_OutWardDrugInvoiceVisitor_Cancel);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int OutWardDrugInvoicePrescriptChuaThuTien_Cancel(OutwardDrugInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutWardDrugInvoicePrescriptChuaThuTien_Cancel(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutWardDrugInvoicePrescriptChuaThuTien_Cancel. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_OutWardDrugInvoiceVisitor_Cancel);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public int OutWardDrugInvoice_Delete(long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutWardDrugInvoice_Delete(id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutWardDrugInvoice_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int OutWardDrugInvoice_UpdateStatus(OutwardDrugInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutWardDrugInvoice_UpdateStatus(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutWardDrugInvoice_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public OutwardDrugInvoice GetOutWardDrugInvoiceVisitorByID(long outwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceVisitorByID(outwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutWardDrugInvoiceVisitorByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //KMx: Sau khi kiểm tra toàn bộ chương trình, thấy hàm này không được gọi (không được sử dụng) nữa.
        public OutwardDrugInvoice GetOutWardDrugInvoiceReturnByID(long? OutiID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceReturnByID(OutiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutWardDrugInvoiceReturnByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public OutwardDrugInvoice GetOutWardDrugInvoiceByID(long? OutiID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceByID(OutiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutWardDrugInvoiceByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //==== #001
        public OutwardDrugInvoice GetHIOutWardDrugInvoiceByID(long? OutiID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetHIOutWardDrugInvoiceByID(OutiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetHIOutWardDrugInvoiceByID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //==== #001

        public IList<OutwardDrugInvoice> GetOutWardDrugInvoiceSearchAllByStatus(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, bool? bFlagStoreHI, bool bFlagPaidTime, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceSearchAllByStatus(SearchCriteria, pageIndex, pageSize, bCountTotal, bFlagStoreHI, bFlagPaidTime, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutWardDrugInvoiceSearchAllByStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<OutwardDrugInvoice> GetOutWardDrugInvoiceSearchReturn(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceSearchReturn(SearchCriteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutWardDrugInvoiceSearchReturn. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #004
        public IList<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_New(string BrandName, long StoreID, bool? IsCode,bool? IsSearchByGenericName)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoComplete_New(BrandName, StoreID, IsCode, IsSearchByGenericName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForSellVisitorAutoComplete_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<GetDrugForSellVisitor> RefGennericDrugDetails_GetRemaining_Paging(string BrandName, long StoreID
            , bool? IsCode, int PageSize, int PageIndex, bool? IsSearchByGenericName, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGennericDrugDetails_GetRemaining_Paging(BrandName, StoreID, IsCode, PageSize, PageIndex, IsSearchByGenericName, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForSellVisitorAutoComplete_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #004

        public IList<GetDrugForSellVisitor> GetDrugForSellVisitorBatchNumber(long DrugID, long StoreID, bool? IsHIPatient)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorBatchNumber(DrugID, StoreID, IsHIPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForSellVisitorBatchNumber. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<GetDrugForSellVisitor> GetDrugForSellVisitorBatchNumber_V2(long DrugID, long StoreID, bool? IsHIPatient, bool? InsuranceCover)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorBatchNumber_V2(DrugID, StoreID, IsHIPatient, InsuranceCover);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForSellVisitorBatchNumber. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_ForPrescription(byte HI, bool? IsHIPatient, string BrandName, long StoreID, long? PrescriptID, bool? IsCode)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoComplete_ForPrescription(HI, IsHIPatient, BrandName, StoreID, PrescriptID, IsCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForSellVisitorAutoComplete_ForPrescription. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_ForPrescriptionByID(byte HI, bool? IsHIPatient, long StoreID, long? PrescriptID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoComplete_ForPrescriptionByID(HI, IsHIPatient, StoreID, PrescriptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForSellVisitorAutoComplete_ForPrescription. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<OutwardDrug> GetOutwardDrugDetailsByOutwardInvoice(long OutiID)
        {
            return GetOutwardDrugDetailsByOutwardInvoice_V2(OutiID, false, null, null);
        }

        public IList<OutwardDrug> GetOutwardDrugDetailsByOutwardInvoice_V2(long OutiID, bool HI, long[] OutiIDArray, long? PtRegistrationID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugDetailsByOutwardInvoice(OutiID, HI, OutiIDArray, PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutwardDrugDetailsByOutwardInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<OutwardDrug> spGetInBatchNumberAndPrice_List(long StoreID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_List(StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spGetInBatchNumberAndPrice_List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciption(long PrescriptID, long StoreID, Int16 IsObject)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_ByPresciption(PrescriptID, StoreID, IsObject);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spGetInBatchNumberAndPrice_ByPresciption. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //==== #001
        public IList<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciptionHI(long PrescriptID, long StoreID, Int16 IsObject)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_ByPresciptionHI(PrescriptID, StoreID, IsObject);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spGetInBatchNumberAndPrice_ByPresciption. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //==== #001

        public IList<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciption_V2(long PrescriptID, long StoreID, Int16 IsObject, bool HI, bool IsIssueID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_ByPresciption(PrescriptID, StoreID, IsObject, HI, IsIssueID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spGetInBatchNumberAndPrice_ByPresciption_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<OutwardDrug> GetInBatchNumberAndPrice_ByPresciption_V3(long PrescriptID, long StoreID, Int16 IsObject, bool HI, bool IsIssueID, long? PtRegistrationID = null)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_ByPresciption(PrescriptID, StoreID, IsObject, HI, IsIssueID, PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetInBatchNumberAndPrice_ByPresciption_V3. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciption_NotSave(Prescription ObjPrescription, long StoreID, Int16 RegistrationType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_ByPresciption_NotSave(ObjPrescription, StoreID, RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spGetInBatchNumberAndPrice_ByPresciption_NotSave. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CALC_NOTSAVE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool UpdateInvoicePayed(OutwardDrugInvoice Outward, out long outiID, out long PaymentID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInvoicePayed(Outward, out outiID, out PaymentID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateInvoicePayed. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateInvoiceInfo(OutwardDrugInvoice Outward)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInvoiceInfo(Outward);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateInvoiceInfo. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<OutwardDrugInvoice> OutwardDrugInvoice_CollectMultiDrug(int top, DateTime FromDate, DateTime ToDate, long StoreID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugInvoice_CollectMultiDrug(top, FromDate, ToDate, StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutwardDrugInvoice_CollectMultiDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool OutwardDrugInvoice_UpdateCollectMulti(IEnumerable<OutwardDrugInvoice> lst)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugInvoice_UpdateCollectMulti(lst);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutwardDrugInvoice_UpdateCollectMulti. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====== #003
        public List<OutwardDrugInvoice> OutwardInternalExportPharmacyInvoice_Cbx(long? StoreID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardInternalExportPharmacyInvoice_Cbx(StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardInternalExportPharmacyInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #003

        #endregion

        #region 5. Add Full OutWardDrug and Transaction member

        public bool CountMoneyForVisitorPharmacy(long outiID, out decimal AmountPaid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.CountMoneyForVisitorPharmacy(outiID, out AmountPaid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving CountMoneyForVisitorPharmacy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_TRANSACTION_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug, out long PaymentID, long? StaffID, long? CollectorDeptLocID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddTransactionVisitor(payment, InvoiceDrug, out PaymentID, StaffID, CollectorDeptLocID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddTransactionVisitor. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_TRANSACTION_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug, out long PaymentID, long? StaffID, long? CollectorDeptLocID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddTransactionHoanTien(payment, InvoiceDrug, out PaymentID, StaffID, CollectorDeptLocID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddTransactionVisitor. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_TRANSACTION_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 21. Drug Expiry
        public IList<OutwardDrug> GetListDrugExpiryDate(long? StoreID, int Type)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetListDrugExpiryDate(StoreID, Type);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetListDrugExpiryDate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetListDrugExpiryDate);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool ListDrugExpiryDate_Save(OutwardDrugInvoice Invoice, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ListDrugExpiryDate_Save(Invoice, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving ListDrugExpiryDate_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_ListDrugExpiryDate_Save);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<OutwardDrugInvoice> OutWardDrugInvoice_SearchByType(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutWardDrugInvoice_SearchByType(SearchCriteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutWardDrugInvoice_SearchByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_OutWardDrugInvoice_SearchByType);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 6. Report member


        public IList<InwardDrug> InwardDrugs_TonKho(string BrandName, long StoreID, bool IsDetail, int PageIndex, int PageSize, out int TotalCount, out decimal TotalMoney)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugs_TonKho(BrandName, StoreID, IsDetail, PageIndex, PageSize, out TotalCount, out TotalMoney);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InwardDrugs_TonKho. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_InwardDrugs_TonKho);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<OutwardDrug> OutwardDrugs_SellOnDate(string BrandName, long StoreID, DateTime FromDate, DateTime ToDate, bool IsDetail, int PageIndex, int PageSize, out int TotalCount, out decimal TotalMoney)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugs_SellOnDate(BrandName, StoreID, FromDate, ToDate, IsDetail, PageIndex, PageSize, out TotalCount, out TotalMoney);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutwardDrugs_SellOnDate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_OutwardDrugs_SellOnDate);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 30. StockTakes member

        public IList<PharmacyStockTakes> PharmacyStockTakes_Search(PharmacyStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyStockTakes_Search(SearchCriteria, PageIndex, PageSize, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmacyStockTakes_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_PharmacyStockTakes_Search);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PharmacyStockTakeDetails> PharmacyStockTakeDetails_Get(long StoreID, DateTime StockTakeDate)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyStockTakeDetails_Get(StoreID, StockTakeDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmacyStockTakeDetails_Get. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_PharmacyStockTakeDetails_Get);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PharmacyStockTakeDetails> PharmacyStockTakeDetails_Load(long PharmacyStockTakeID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyStockTakeDetails_Load(PharmacyStockTakeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmacyStockTakeDetails_Load. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_PharmacyStockTakeDetails_Load);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PharmacyStockTakeDetails> GetDrugForAutoCompleteStockTake(string BrandName, long StoreID, bool IsCode)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForAutoCompleteStockTake(BrandName, StoreID, IsCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForAutoCompleteStockTake. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacyStockTake_Save(PharmacyStockTakes StockTake, bool IsConfirmFinished, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyStockTake_Save(StockTake, IsConfirmFinished, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmacyStockTake_Save. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_PharmacyStockTake_Save);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool KetChuyenTonKho(long StoreID, long StaffID, string CheckPointName)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.KetChuyenTonKho(StoreID, StaffID, CheckPointName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving KetChuyenTonKho. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_KetChuyenTonKho);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        
        public bool PharmacyLockAndUnlockStore(long StoreID, long V_MedProductType, bool IsLock)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyLockAndUnlockStore(StoreID, IsLock);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ClinicDeptLockAndUnlockStore. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_ClinicDeptLockAndUnlockStore);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<PharmacyStockTakeDetails> ReGetPharmacyStockTakeDetails(long storeID, DateTime stockTakeDate)
        {
            if (0 == storeID
                //|| 0 == vMedProductType
                || null == stockTakeDate)
            {
                return null;
            }
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ReGetPharmacyStockTakeDetails(storeID, stockTakeDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ReGetClinicDeptStockTakeDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public PharmacyStockTakes GetLastPharmacyStockTakes(long storeId)
        {
            if (0 == storeId )
            {
                return null;
            }

            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetLastPharmacyStockTakes(storeId);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLastClinicDeptStockTakes. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Could not get the last clinic dept stock takes.");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool PharmacyStockTake_Resave(PharmacyStockTakes StockTake, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyStockTake_Resave(StockTake, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ClinicDeptStockTake_Resave. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        #region 31. Request Form Drug For Pharmacy

        public bool FullOperatorRequestDrugInward(RequestDrugInward Request, out RequestDrugInward OutRequest)
        {
            try
            {
                OutRequest = new RequestDrugInward();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.FullOperatorRequestDrugInward(Request, out id);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardByID(id);
                    if (OutRequest != null)
                    {
                        OutRequest.RequestDetails = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardDetailByID(id).ToObservableCollection();
                    }
                }
                else
                {
                    if (Request.ReqDrugInID > 0)
                    {
                        OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardByID(Request.ReqDrugInID);
                        if (OutRequest != null)
                        {
                            OutRequest.RequestDetails = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardDetailByID(Request.ReqDrugInID).ToObservableCollection();
                        }
                    }
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving FullOperatorRequestDrugInward. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_REQUEST_CANNOT_SAVE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public RequestDrugInward GetRequestDrugInwardByID(long ReqDrugInID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardByID(ReqDrugInID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRequestDrugInwardByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetRequestDrugInwardByID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RequestDrugInwardDetail> GetRequestDrugInwardDetailByID(long ReqDrugInID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardDetailByID(ReqDrugInID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRequestDrugInwardByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetRequestDrugInwardDetailByID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RequestDrugInwardDetail> GetRequestDrugInwardDetailByRequestID(long ReqDrugInID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardDetailByRequestID(ReqDrugInID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRequestDrugInwardDetailByRequestID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetRequestDrugInwardDetailByRequestID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<OutwardDrug> GetRequestDrugInwardDetailByRequestIDNew(long ReqDrugInID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardDetailByRequestIDNew(ReqDrugInID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRequestDrugInwardDetailByRequestIDNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetRequestDrugInwardDetailByRequestIDNew);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RequestDrugInward> SearchRequestDrugInward(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRequestDrugInward(Criteria, pageIndex, pageSize, bCount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchRequestDrugInward. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SearchRequestDrugInward);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteRequestDrugInward(long ReqDrugInID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteRequestDrugInward(ReqDrugInID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteRequestDrugInward. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_DeleteRequestDrugInward);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<OutwardDrug> spGetInBatchNumberAndPrice_ByRequestPharmacy(bool? IsCost, long RequestID, long StoreID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_ByRequestPharmacy(IsCost, RequestID, StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spGetInBatchNumberAndPrice_ByRequestPharmacy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_spGetInBatchNumberAndPrice_ByRequestPharmacy);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #004
        public List<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_ForRequestPharmacy(bool? IsCost, string BrandName, long StoreID
            , long? RequestID, bool? IsCode, bool? IsSearchByGenericName)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoComplete_ForRequestPharmacy(IsCost, BrandName, StoreID, RequestID, IsCode, IsSearchByGenericName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForSellVisitorAutoComplete_ForRequestPharmacy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestPharmacy);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #004

        public DateTime? OutwardDrug_GetMaxDayBuyInsurance(long PatientID, long outiID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrug_GetMaxDayBuyInsurance(PatientID, outiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutwardDrug_GetMaxDayBuyInsurance. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestPharmacy);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool OutwardDrugInvoice_SaveByType(OutwardDrugInvoice Invoice, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugInvoice_SaveByType(Invoice, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutwardDrugInvoice_SaveByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_OutwardDrugInvoice_SaveByType);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 32. SupplierPharmacyPaymentReqs Member

        public List<InwardDrugInvoice> SupplierPharmacyPaymentReqs_Details(InwardInvoiceSearchCriteria criteria)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_Details(criteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierPharmacyPaymentReqs_Details. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_Details);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SupplierPharmacyPaymentReqs_Save(SupplierPharmacyPaymentReqs PaymentReqs, out SupplierPharmacyPaymentReqs OutPaymentReqs)
        {
            try
            {
                OutPaymentReqs = new SupplierPharmacyPaymentReqs();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_Save(PaymentReqs, out id);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    OutPaymentReqs = RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_ID(id);
                    if (OutPaymentReqs != null)
                    {
                        OutPaymentReqs.InwardDrugInvoices = RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_DetailsByReqID(id).ToObservableCollection();
                    }
                }
                else
                {

                    if (PaymentReqs.PharmacySupplierPaymentReqID > 0)
                    {
                        OutPaymentReqs = RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_ID(PaymentReqs.PharmacySupplierPaymentReqID);
                        if (OutPaymentReqs != null)
                        {
                            OutPaymentReqs.InwardDrugInvoices = RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_DetailsByReqID(PaymentReqs.PharmacySupplierPaymentReqID).ToObservableCollection();
                        }
                    }
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierPharmacyPaymentReqs_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_REQUEST_CANNOT_SAVE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<InwardDrugInvoice> SupplierPharmacyPaymentReqs_DetailsByReqID(long PharmacySupplierPaymentReqID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_DetailsByReqID(PharmacySupplierPaymentReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierPharmacyPaymentReqs_DetailsByReqID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_DetailsByReqID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public SupplierPharmacyPaymentReqs SupplierPharmacyPaymentReqs_ID(long PharmacySupplierPaymentReqID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_ID(PharmacySupplierPaymentReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierPharmacyPaymentReqs_ID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_ID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<SupplierPharmacyPaymentReqs> SupplierPharmacyPaymentReqs_Search(RequestSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_Search(Criteria, PageSize, PageIndex, bcount, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierPharmacyPaymentReqs_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_Search);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SupplierPharmacyPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus, long? StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_UpdateStatus(ID, V_PaymentReqStatus, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierPharmacyPaymentReqs_UpdateStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_UpdateStatus);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SupplierPharmacyPaymentReqs_Delete(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierPharmacyPaymentReqs_Delete(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierPharmacyPaymentReqs_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_Delete);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 33. Hopital member
        public List<Hospital> Hopital_IsFriends()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.Hopital_IsFriends();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Hopital_IsFriends. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_Hopital_IsFriends);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 43. PharmacyOutwardDrugReport Member

        public IList<PharmacyOutwardDrugReportDetail> PharmacyOutwardDrugReportDetail_GetReport(PharmacyOutwardDrugReport para, long loggedStaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyOutwardDrugReportDetail_GetReport(para, loggedStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmacyOutwardDrugReportDetail_GetReport. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_Hopital_IsFriends);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacyOutwardDrugReport_Save(PharmacyOutwardDrugReport para, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyOutwardDrugReport_Save(para, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmacyOutwardDrugReport_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_Hopital_IsFriends);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PharmacyOutwardDrugReport> PharmacyOutwardDrugReport_Search(SearchOutwardReport para, long loggedStaffID, int PageIndex, int PageSize, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyOutwardDrugReport_Search(para, loggedStaffID, PageIndex, PageSize, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmacyOutwardDrugReport_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_Hopital_IsFriends);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PharmacyOutwardDrugReportDetail> PharmacyOutwardDrugReportDetail_GetID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyOutwardDrugReportDetail_GetID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmacyOutwardDrugReportDetail_GetID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_Hopital_IsFriends);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        public List<PharmacyPurchaseCheckOrder> PharmacyPurchaseOrderDetails_CheckOrder(long DrugID, DateTime FromDate, DateTime ToDate, out List<PharmacyPurchaseCheckOrderInward> InwardList)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrderDetails_CheckOrder(DrugID, FromDate, ToDate, out InwardList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmacyPurchaseOrderDetails_CheckOrder. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_Hopital_IsFriends);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▼====== #002
        public List<OutwardDrug> GetOutwardDrugDetailsByOutwardInvoiceForDrugDept(long OutiID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugDetailsByOutwardInvoiceForDrugDept(OutiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutwardDrugDetailsByOutwardInvoiceForDrugDept. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #002

        public IList<OutwardDrug> GetOutwardDrugAndPrice_ByListPresciption(ObservableCollection<Prescription> ListPrescription, long aStoreID, short aIsObject, bool IsHIOutPt, long PtRegistrationID, out DateTime LastDaySoldHI, out ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListByPrescriptIDForConfirm)
        {
            try
            {
                LastDaySoldHI = DateTime.MinValue;
                GetDrugForSellVisitorListByPrescriptIDForConfirm = new ObservableCollection<GetDrugForSellVisitor>();
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugAndPrice_ByListPresciption(ListPrescription, aStoreID, aIsObject, IsHIOutPt, PtRegistrationID, out LastDaySoldHI, out GetDrugForSellVisitorListByPrescriptIDForConfirm);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetInBatchNumberAndPrice_ByPresciption_V3. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<OutwardDrug> GetOutwardDrugForOutPtTreatment(long IssueID, long StoreID, Int16 IsObject, bool HI, bool IsIssueID, long? PtRegistrationID = null, bool SecondTimesSell = false)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugForOutPtTreatment(IssueID, StoreID, IsObject, HI, IsIssueID, PtRegistrationID, SecondTimesSell);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutwardDrugForOutPtTreatment. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<OutwardDrug> GetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID(long PtRegistrationID, Int16 RegistrationType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID(PtRegistrationID, RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spGetInBatchNumberAndPrice_ByPresciption_NotSave. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CALC_NOTSAVE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<GetDrugForSellVisitor> GetDrugForSellVisitorAutoCompleteFromCategory(bool? IsCost, string BrandName, long StoreID
            , long? RequestID, bool? IsCode, bool? IsSearchByGenericName)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoCompleteFromCategory(IsCost, BrandName, StoreID, RequestID, IsCode, IsSearchByGenericName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForSellVisitorAutoComplete_ForRequestPharmacy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestPharmacy);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
    }
}
