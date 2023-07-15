/*
 * 20170821 #001 CMN: Added AdjustClinicPrice Service
 * 20181124 #002 TTM: BM 0005309: Hàm lấy dữ liệu cho cbb Phiếu xuất kho BHYT - màn hình nhập trả khoa dược
 * 20180412 #003 TTM: BM 0005324: Hàm lấy dữ liệu cho cbb Phiếu nhập kho BHYT - màn hình nhập từ khoa dược
 * 20200807 #004 TNHX: Thêm loại hóa chất để nhập hàng theo thầu
 * 20220106 #005 TNHX: Thêm điều kiện filter theo danh mục COVID
 * 20230211 #006 QTD:  Thêm DTDT Nhà thuốc
 */
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using AxLogging;
using ErrorLibrary.Resources;
using System.Linq;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;
using System.Data;

namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single)]
    [KnownType(typeof(AxException))]
    public class InMedDeptService : eHCMS.WCFServiceCustomHeader, IInMedDept
    {
        public InMedDeptService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        #region 1. Inward Drug

        public List<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, bool IsFromClinic)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoice_Cbx(StoreID, V_MedProductType, IsFromClinic);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddInwardDrugMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, bool IsNotCheckInvalid, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardDrugMedDeptInvoice(InvoiceDrug, IsNotCheckInvalid, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddInwardDrugMedDeptInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼====== #004
        public int UpdateInwardDrugMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardDrugMedDeptInvoice(InvoiceDrug, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardDrugMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardDrugMedDeptInvoice(long ID, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardDrugMedDeptInvoice(ID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardDrugMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InwardDrugMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, long? V_MedProductType)
        {
            OrderDetails = OrderDetails.Where(x => x.GenMedProductID > 0).ToList();
            RefDrugGenericDetailsProvider.Instance.AddInwardDrugMedDept(OrderDetails, inviID);
            //for (int i = 0; i < OrderDetails.Count; i++)
            //{
            //    RefDrugGenericDetailsProvider.Instance.AddInwardDrugMedDept(OrderDetails[i], inviID);
            //}
            RefDrugGenericDetailsProvider.Instance.InwardDrugMedDeptInvoice_UpdateCost(inviID, V_MedProductType);
            if (DrugDeptPoID != null && DrugDeptPoID > 0)
            {
                RefDrugGenericDetailsProvider.Instance.UpdateStatusDrugDeptPurchaseOrder(DrugDeptPoID.GetValueOrDefault());
            }
            return true;
        }

        public int UpdateInwardDrugMedDept(InwardDrugMedDept invoicedrug, long StaffID, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardDrugMedDept(invoicedrug, StaffID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardDrugMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardDrugMedDept(long invoicedrug, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardDrugMedDept(invoicedrug, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardDrugMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== #004

        public bool DeleteInwardDrugMedDeptTemp(long InID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteInwardDrugMedDeptTemp.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardDrugMedDeptTemp(InID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardDrugMedDeptTemp. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptTemp);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<OutwardDrugClinicDeptInvoice> OutwardDrugClinicDeptInvoice_Cbx(long? StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugClinicDeptInvoice_Cbx(StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugClinicDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▼====== #002
        public List<OutwardDrugInvoice> OutwardDrugPharmacyInvoice_Cbx(long? StoreID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardPharmacyDeptInvoice_Cbx(StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardPharmacyDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #003
        public IList<InwardDrugMedDeptInvoice> SearchInwardDrugMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, bool IsConsignment, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardDrugMedDeptInvoice(criteria, TypID, V_MedProductType, pageIndex, pageSize, bCountTotal, IsConsignment, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardDrugMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====== #004
        public IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount
            , long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugMedDept_ByIDInvoice(inviID, pageSize, pageIndex, bCountTotal, out totalCount
                    , out TongTienSPChuaVAT
                    , out CKTrenSP
                    , out TongTienTrenSPDaTruCK
                    , out TongCKTrenHoaDon
                    , out TongTienHoaDonCoThueNK
                    , out TongTienHoaDonCoVAT
                    , out TotalVATDifferenceAmount
                    , V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDept_ByIDInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDDrugDeptInIDOrig(long inviID, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugMedDept_ByIDDrugDeptInIDOrig(inviID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDept_ByIDDrugDeptInIDOrig. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== #004

        public IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return GetInwardDrugMedDept_ByIDInvoiceNotPaging_V2(inviID
                    , out TongTienSPChuaVAT
                    , out CKTrenSP
                    , out TongTienTrenSPDaTruCK
                    , out TongCKTrenHoaDon
                    , out TongTienHoaDonCoThueNK
                    , out TongTienHoaDonCoVAT
                    , null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDInvoiceNotPaging_V2(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugMedDept_ByIDInvoiceNotPaging_V2(inviID
                    , out TongTienSPChuaVAT
                    , out CKTrenSP
                    , out TongTienTrenSPDaTruCK
                    , out TongCKTrenHoaDon
                    , out TongTienHoaDonCoThueNK
                    , out TongTienHoaDonCoVAT
                    , V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDept_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<RefGenMedProductDetails> spGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAllDrugDept_ByGenMedProductID(GenMedProductID, V_MedProductType, StoreID, IsHIPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of spGetInBatchNumberAllDrugDept_ByGenMedProductID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAllDrugDept_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====== #004
        public InwardDrugMedDeptInvoice GetInwardDrugMedDeptInvoice_ByID(long ID, long? V_MedProductType)
        {
            try
            {
                return GetInwardDrugMedDeptInvoice_ByID_V2(ID, V_MedProductType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //▲===== #004

        public InwardDrugMedDeptInvoice GetInwardDrugMedDeptInvoice_ByID_V2(long ID, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugMedDeptInvoice_ByID_V2(ID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDeptInvoice_ByID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDeptInvoice_ByID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int InwardDrugMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugMedDeptInvoice_SaveXML(InvoiceDrug, DSPTModifiedDate_Outward, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardDrugMedDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public RefGenMedProductDetails GetRefGenMedProductDetails(long genMedProductID, long? genMedVersionID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefGenMedProductDetails(genMedProductID, genMedVersionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefGenMedProductDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetRefGenMedProductDetails);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        #region 2. Outward Drug

        public IList<OutwardDrugMedDept> spGetInBatchNumberAndPrice_ListForRequest(bool? IsCost, long ReqDrugInClinicDeptID, long StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_ListForRequest(IsCost, ReqDrugInClinicDeptID, StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of spGetInBatchNumberAndPrice_ListForRequest. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<OutwardDrugMedDept> GetRequestDrugDeptList_ForDepositGoods(long ReqDrugInClinicDeptID, long StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugDeptList_ForDepositGoods(ReqDrugInClinicDeptID, StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugDeptList_ForDepositGoods. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetRequestDrugDeptList_ForDepositGoods);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<OutwardDrugMedDept> GetOutwardDrugMedDeptDetailByInvoice(long ID, long V_MedProductType, bool FromClinicDept)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugMedDeptDetailByInvoice(ID, V_MedProductType, FromClinicDept);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetOutwardDrugMedDeptDetailByInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetOutwardDrugMedDeptDetailByInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<OutwardDrugMedDept> GetOutwardDrugMedDeptDetailByInvoice_HangKyGoi(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugMedDeptDetailByInvoice_HangKyGoi(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetOutwardDrugMedDeptDetailByInvoice_HangKyGoi. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetOutwardDrugMedDeptDetailByInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<OutwardDrugMedDept> GetListDrugExpiryDate_DrugDept(long? StoreID, int Type, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetListDrugExpiryDate_DrugDept(StoreID, Type, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetListDrugExpiryDate_DrugDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetOutwardDrugMedDeptDetailByInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RequestDrugInwardClinicDeptDetail> GetRequestDrugInwardClinicDeptDetailByRequestID(long ReqDrugInClinicDeptID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptDetailByRequestID(ReqDrugInClinicDeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardClinicDeptDetailByRequestID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetRequestDrugInwardClinicDeptDetailByRequestID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<OutwardDrugMedDept> GetRequestDrugInwardClinicDeptDetailByRequestIDNew(long ReqDrugInClinicDeptID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptDetailByRequestIDNew(ReqDrugInClinicDeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardClinicDeptDetailByRequestIDNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetRequestDrugInwardClinicDeptDetailByRequestIDNew);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public OutwardDrugMedDeptInvoice GetOutwardDrugMedDeptInvoice(long ID, long V_MedProductType)
        {
            try
            {
                OutwardDrugMedDeptInvoice result = RefDrugGenericDetailsProvider.Instance.GetOutwardDrugMedDeptInvoice(ID, V_MedProductType);
                if (result != null && result.ReturnID.GetValueOrDefault(0) > 0)
                {
                    result.OutInvoice = RefDrugGenericDetailsProvider.Instance.GetOutwardDrugMedDeptInvoice(result.ReturnID.GetValueOrDefault(0), result.V_MedProductType);
                }
                return result;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetOutwardDrugMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetOutwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool DeleteOutwardDrugMedDeptInvoice(long outiID, long StaffID, long V_MedProductType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of OutwardDrugMedDept_Delete.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDept_Delete(outiID, StaffID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDept_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDept_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public bool OutwardDrugMedDept_Delete(long id, long StaffID, long V_MedProductType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of OutwardDrugMedDept_Delete.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDept_Delete(id, StaffID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDept_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDept_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoice_SearchByType(MedDeptInvoiceSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoice_SearchByType(Criteria, pageIndex, pageSize, bCount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoice_SearchByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SearchByType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetRefGenMedProductDetailsAuto_ByRequestID(string BrandName, long V_MedProductType, long? RequestID, int pageIndex, int pageSize, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefGenMedProductDetailsAuto_ByRequestID(BrandName, V_MedProductType, RequestID, pageIndex, pageSize, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefGenMedProductDetailsAuto_ByRequestID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetRefGenMedProductDetailsAuto_ByRequestID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoComplete_ForRequestDrugDept(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugForSellVisitorAutoComplete_ForRequestDrugDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(bool IsSearchByGenericName, bool? IsCost
            , string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1
            , long? RequestID, bool? IsCode, long? OutputToStoreID, long? RefGenDrugCatID_2
            , long? RequestTemplateID, bool IsGetOnlyRemain
            //▼====: #005
            , bool IsCOVID
            //▲====: #005
            )
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(IsSearchByGenericName, IsCost
                    , BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, OutputToStoreID, RefGenDrugCatID_2
                    , RequestTemplateID, IsGetOnlyRemain
                    //▼====: #005
                    , IsCOVID
                    //▲====: #005
                    );
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<GetGenMedProductForSell> GetGenMedProductForSellAutoComplete_ForPrescriptionByID(byte HI, bool? IsHIPatient, long StoreID, long V_MedProductType, long? IssueID, long RefGenDrugCatID_1)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetGenMedProductForSellAutoComplete_ForPrescriptionByID.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetGenMedProductForSellAutoComplete_ForPrescriptionByID(HI, IsHIPatient, StoreID, V_MedProductType, IssueID, RefGenDrugCatID_1);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetGenMedProductForSellAutoComplete_ForPrescriptionByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<OutwardDrugMedDept> spGetInBatchNumberAndPrice_ByPresciption_InPt(long PrescriptID, long StoreID, Int16 IsObject, long V_MedProductType, long RefGenDrugCatID_1)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving spGetInBatchNumberAndPrice_ByPresciption_InPt.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_ByPresciption_InPt(PrescriptID, StoreID, IsObject, V_MedProductType, RefGenDrugCatID_1);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spGetInBatchNumberAndPrice_ByPresciption_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SaveOutwardInvoice(OutwardDrugMedDeptInvoice OutwardInvoice, out OutwardDrugMedDeptInvoice SavedOutwardInvoice)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of SaveOutwardInvoice.", CurrentUser);

                long outiID = 0;

                SavedOutwardInvoice = null;
                bool result = false;
                result = RefDrugGenericDetailsProvider.Instance.SaveOutwardInvoice(OutwardInvoice, out outiID);
                if (result && outiID > 0)
                {
                    SavedOutwardInvoice = RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceByID_InPt(outiID);
                }
                else
                {
                    return false;
                }

                return result;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveOutwardInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public bool UpdateOutwardInvoice(OutwardDrugMedDeptInvoice OutwardInvoice, out OutwardDrugMedDeptInvoice SavedOutwardInvoice)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of UpdateOutwardInvoice.", CurrentUser);

                long outiID_New = 0;

                SavedOutwardInvoice = null;
                bool result = false;
                result = RefDrugGenericDetailsProvider.Instance.UpdateOutwardInvoice(OutwardInvoice, out outiID_New);
                if (result && outiID_New > 0)
                {
                    SavedOutwardInvoice = RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceByID_InPt(outiID_New);
                }
                else
                {
                    return false;
                }

                return result;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateOutwardInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public IList<OutwardDrugMedDeptInvoice> GetOutWardDrugInvoiceSearchAllByStatus_InPt(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetOutWardDrugInvoiceSearchAllByStatus_InPt.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceSearchAllByStatus_InPt(SearchCriteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutWardDrugInvoiceSearchAllByStatus_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<OutwardDrugMedDept> GetOutwardDrugDetailsByOutwardInvoice_InPt(long OutiID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetOutwardDrugDetailsByOutwardInvoice_InPt.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugDetailsByOutwardInvoice_InPt(OutiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutwardDrugDetailsByOutwardInvoice_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<GetGenMedProductForSell> GetGenMedProductForSellAutoComplete_ForPrescription(byte HI, bool? IsHIPatient, string BrandName, long StoreID, long? IssueID, bool? IsCode, long V_MedProductType, long RefGenDrugCatID_1)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetGenMedProductForSellAutoComplete_ForPrescription.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetGenMedProductForSellAutoComplete_ForPrescription(HI, IsHIPatient, BrandName, StoreID, IssueID, IsCode, V_MedProductType, RefGenDrugCatID_1);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetGenMedProductForSellAutoComplete_ForPrescription. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteOutwardInvoice(long outiID, long staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving DeleteOutwardInvoice.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteOutwardInvoice(outiID, staffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteOutwardInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INVOICE_NOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public OutwardDrugMedDeptInvoice GetOutWardDrugInvoiceByID_InPt(long outiID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetOutWardDrugInvoiceByID_InPt.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceByID_InPt(outiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutWardDrugInvoiceByID_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INVOICE_NOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<GetGenMedProductForSell> GetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of GetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(GenMedProductID, V_MedProductType, StoreID, IsHIPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAllDrugDept_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool CreateBillForOutwardFromPrescription(OutwardDrugMedDeptInvoice OutwardInvoice, long StaffID, out long InPatientBillingInvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of CreateBillForOutwardFromPrescription.", CurrentUser);
                bool result = RefDrugGenericDetailsProvider.Instance.CreateBillForOutwardFromPrescription(OutwardInvoice, StaffID, out InPatientBillingInvID);

                return result;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CreateBillForOutwardFromPrescription. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INVOICE_NOT_CREATE_BILL);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteBillForOutwardFromPrescription(long InPatientBillingInvID, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteBillForOutwardFromPrescription.", CurrentUser);
                bool result = RefDrugGenericDetailsProvider.Instance.DeleteBillForOutwardFromPrescription(InPatientBillingInvID, StaffID);

                return result;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteBillForOutwardFromPrescription. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INVOICE_NOT_DELETE_BILL);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1,
                                                                                                             long? RequestID, bool? IsCode, int PageSize, int PageIndex, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, PageSize, PageIndex, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool OutwardDrugMedDeptInvoice_SaveByType(OutwardDrugMedDeptInvoice Invoice, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoice_SaveByType(Invoice, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoice_SaveByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool OutwardDrugMedDeptInvoice_Update(OutwardDrugMedDeptInvoice Invoice, List<OutwardDrugMedDept> NewOutwardDrugMedDepts,
                                                    List<OutwardDrugMedDept> UpdateOutwardDrugMedDepts, List<OutwardDrugMedDept> DeleteOutwardDrugMedDepts, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoice_Update(Invoice, NewOutwardDrugMedDepts, UpdateOutwardDrugMedDepts, DeleteOutwardDrugMedDepts, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoice_SaveByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool RequireUnlockOutMedDeptInvoice(long outiID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RequireUnlockOutMedDeptInvoice(outiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RequireUnlockOutMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_RequireUnlockOutMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool OutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(OutwardDrugMedDeptInvoice Invoice, List<OutwardDrugMedDept> NewOutwardDrugMedDepts,
                                                                List<OutwardDrugMedDept> UpdateOutwardDrugMedDepts, List<OutwardDrugMedDept> DeleteOutwardDrugMedDepts, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(Invoice, NewOutwardDrugMedDepts, UpdateOutwardDrugMedDepts, DeleteOutwardDrugMedDepts, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoice_SaveByType_HangKyGoi. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool OutwardDrugMedDeptInvoices_HangKyGoi_Delete(long outiID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of OutwardDrugMedDeptInvoices_HangKyGoi_Delete.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoices_HangKyGoi_Delete(outiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoices_HangKyGoi_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoices_HangKyGoi_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RefOutputType> RefOutputType_Get(bool? All)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefOutputType_Get(All);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefOutputType_Get. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion


        #region Outward From Prescription

        public IList<Prescription> SearchPrescription_InPt(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            AxLogger.Instance.LogInfo("Start of retrieving SearchPrescription.", CurrentUser);
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchPrescription_InPt(Criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchPrescription. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion


        #region 30.1 DrugDept StockTakes member

        public List<DrugDeptStockTakes> DrugDeptStockTakes_Search(DrugDeptStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptStockTakes_Search(SearchCriteria, PageIndex, PageSize, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptStockTakes_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptStockTakeDetails> DrugDeptStockTakeDetails_Get(long StoreID, long V_MedProductType, DateTime StockTakeDate)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptStockTakeDetails_Get(StoreID, V_MedProductType, StockTakeDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptStockTakeDetails_Get. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //--▼--02-- 11/12/2020 DatTB 
        public List<DrugDeptStockTakeDetails> ReGetDrugDeptStockTakeDetails(long StoreID, long V_MedProductType, DateTime StockTakeDate)
        {
            if (0 == StoreID
                || 0 == V_MedProductType
                || null == StockTakeDate)
            {
                return null;
            }
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ReGetDrugDeptStockTakeDetails(StoreID, V_MedProductType, StockTakeDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ReGetDrugDeptStockTakeDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //--▲--02-- 11/12/2020 DatTB 

        public List<DrugDeptStockTakeDetails> DrugDeptStockTakeDetails_Load(long DrugDeptStockTakeID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptStockTakeDetails_Load(DrugDeptStockTakeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptStockTakeDetails_Load. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DrugDeptStockTakeDetails> GetProductForDrugDeptStockTake(string BrandName, long StoreID, bool IsCode, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetProductForDrugDeptStockTake(BrandName, StoreID, IsCode, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetProductForDrugDeptStockTake. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetProductForDrugDeptStockTake);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DrugDeptStockTake_Save(DrugDeptStockTakes StockTake, bool IsConfirmFinished, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptStockTake_Save(StockTake, IsConfirmFinished, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptStockTake_Save. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //--▼--01-- 11/12/2020 DatTB 
        public bool DrugDeptStockTake_Resave(DrugDeptStockTakes StockTake, bool IsConfirmFinished, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptStockTake_Resave(StockTake, IsConfirmFinished, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptStockTake_Resave. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //--▲--01-- 11/12/2020 DatTB 

        public bool DrugDeptStockTake_Remove(long DrugDeptStockTakeID, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptStockTake_Remove(DrugDeptStockTakeID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptStockTake_Remove. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool KetChuyenTonKho_DrugDept(long StoreID, long StaffID, string CheckPointName, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.KetChuyenTonKho_DrugDept(StoreID, StaffID, CheckPointName, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of KetChuyenTonKho_DrugDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 30.2 ClinicDept StockTakes member

        public List<ClinicDeptStockTakes> ClinicDeptStockTakes_Search(ClinicDeptStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ClinicDeptStockTakes_Search(SearchCriteria, PageIndex, PageSize, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ClinicDeptStockTakes_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ClinicDeptStockTakeDetails> ClinicDeptStockTakeDetails_Get(long StoreID, long V_MedProductType, DateTime StockTakeDate)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ClinicDeptStockTakeDetails_Get(StoreID, V_MedProductType, StockTakeDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ClinicDeptStockTakeDetails_Get. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="vMedProductType"></param>
        /// <param name="stockTakeDate"></param>
        /// <returns></returns>
        public List<ClinicDeptStockTakeDetails> ReGetClinicDeptStockTakeDetails(long storeID, long vMedProductType, DateTime stockTakeDate)
        {
            if (0 == storeID
                || 0 == vMedProductType
                || null == stockTakeDate)
            {
                return null;
            }
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ReGetClinicDeptStockTakeDetails(storeID, vMedProductType, stockTakeDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ReGetClinicDeptStockTakeDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Get the last clinic dept stock takes
        /// VuTTM
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="vMedProductType"></param>
        /// <returns></returns>
        public ClinicDeptStockTakes GetLastClinicDeptStockTakes(long storeId, long vMedProductType)
        {
            if (0 == storeId || 0 == vMedProductType)
            {
                return null;
            }

            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetLastClinicDeptStockTakes(storeId, vMedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLastClinicDeptStockTakes. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Could not get the last clinic dept stock takes.");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="StoreID"></param>
        /// <param name="V_MedProductType"></param>
        /// <param name="staffID"></param>
        /// <param name="IsLock"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool IsLockedClinicWarehouse(long StoreID, long vMedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.IsLockedClinicWarehouse(StoreID, vMedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of IsLockedClinicWarehouse. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_ClinicDeptLockAndUnlockStore);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool ClinicDeptLockAndUnlockStore(long StoreID, long V_MedProductType, long staffID, bool IsLock, out string msg)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ClinicDeptLockAndUnlockStore(StoreID, V_MedProductType, staffID, IsLock, out msg);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ClinicDeptLockAndUnlockStore. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_ClinicDeptLockAndUnlockStore);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //--▼--01-- 12/12/2020 DatTB
        public DrugDeptStockTakes GetLastDrugDeptStockTakes(long storeId, long vMedProductType)
        {
            if (0 == storeId || 0 == vMedProductType)
            {
                return null;
            }

            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetLastDrugDeptStockTakes(storeId, vMedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLastDrugDeptStockTakes. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Could not get the last clinic dept stock takes.");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DrugDeptLockAndUnlockStore(long StoreID, long V_MedProductType, bool IsLock)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptLockAndUnlockStore(StoreID, V_MedProductType, IsLock);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptLockAndUnlockStore. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_ClinicDeptLockAndUnlockStore);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //--▲--01-- 12/12/2020 DatTB

        public List<ClinicDeptStockTakeDetails> ClinicDeptStockTakeDetails_Load(long ClinicDeptStockTakeID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ClinicDeptStockTakeDetails_Load(ClinicDeptStockTakeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ClinicDeptStockTakeDetails_Load. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<ClinicDeptStockTakeDetails> GetProductForClinicDeptStockTake(string BrandName, long StoreID, bool IsCode, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetProductForClinicDeptStockTake(BrandName, StoreID, IsCode, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetProductForClinicDeptStockTake. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetProductForClinicDeptStockTake);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool ClinicDeptStockTake_Save(ClinicDeptStockTakes StockTake, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ClinicDeptStockTake_Save(StockTake, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ClinicDeptStockTake_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool ClinicDeptStockTake_Resave(ClinicDeptStockTakes StockTake, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ClinicDeptStockTake_Resave(StockTake, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ClinicDeptStockTake_Resave. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool KetChuyenTonKho_ClinicDept(long StoreID, long StaffID, string CheckPointName, long V_MedProductType, DateTime CheckPointDate)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.KetChuyenTonKho_ClinicDept(StoreID, StaffID, CheckPointName, V_MedProductType, CheckPointDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of KetChuyenTonKho_ClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion 

        #region 32.0 SupplierDrugDeptPaymentReqs Member

        public List<InwardDrugMedDeptInvoice> SupplierDrugDeptPaymentReqs_Details(InwardInvoiceSearchCriteria criteria, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_Details(criteria, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierDrugDeptPaymentReqs_Details. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_Details);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SupplierDrugDeptPaymentReqs_Save(SupplierDrugDeptPaymentReqs PaymentReqs, out SupplierDrugDeptPaymentReqs OutPaymentReqs)
        {
            try
            {
                OutPaymentReqs = new SupplierDrugDeptPaymentReqs();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_Save(PaymentReqs, out id);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    OutPaymentReqs = RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_ID(id);
                    if (OutPaymentReqs != null)
                    {
                        OutPaymentReqs.InwardDrugMedDeptInvoices = RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_DetailsByReqID(id).ToObservableCollection();
                    }
                }
                else
                {
                    if (PaymentReqs.DrugDeptSupplierPaymentReqID > 0)
                    {
                        OutPaymentReqs = RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_ID(PaymentReqs.DrugDeptSupplierPaymentReqID);
                        if (OutPaymentReqs != null)
                        {
                            OutPaymentReqs.InwardDrugMedDeptInvoices = RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_DetailsByReqID(PaymentReqs.DrugDeptSupplierPaymentReqID).ToObservableCollection();
                        }
                    }
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierDrugDeptPaymentReqs_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_REQUEST_CANNOT_SAVE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<InwardDrugMedDeptInvoice> SupplierDrugDeptPaymentReqs_DetailsByReqID(long DrugDeptSupplierPaymentReqID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_DetailsByReqID(DrugDeptSupplierPaymentReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierDrugDeptPaymentReqs_DetailsByReqID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_DetailsByReqID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public SupplierDrugDeptPaymentReqs SupplierDrugDeptPaymentReqs_ID(long DrugDeptSupplierPaymentReqID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_ID(DrugDeptSupplierPaymentReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierDrugDeptPaymentReqs_ID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_ID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<SupplierDrugDeptPaymentReqs> SupplierDrugDeptPaymentReqs_Search(RequestSearchCriteria Criteria, long? V_MedProductType, int PageSize, int PageIndex, bool bcount, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_Search(Criteria, V_MedProductType, PageSize, PageIndex, bcount, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierDrugDeptPaymentReqs_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_Search);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SupplierDrugDeptPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus, long? StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_UpdateStatus(ID, V_PaymentReqStatus, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierDrugDeptPaymentReqs_UpdateStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_UpdateStatus);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SupplierDrugDeptPaymentReqs_Delete(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierDrugDeptPaymentReqs_Delete(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierDrugDeptPaymentReqs_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_SupplierPharmacyPaymentReqs_Delete);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 39.CostTableForMedDeptInvoice Member

        public bool CostTableMedDept_Insert(CostTableMedDept Item, out long CoID, out string StrCoNumber)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.CostTableMedDept_Insert(Item, out CoID, out StrCoNumber);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CostTableMedDept_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<CostTableMedDept> CostTableMedDept_Search(InwardInvoiceSearchCriteria criteria, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.CostTableMedDept_Search(criteria, V_MedProductType, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CostTableMedDept_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public CostTableMedDept CostTableMedDeptDetails_ByID(long CoID)
        {
            try
            {
                CostTableMedDept result = new CostTableMedDept();
                result.CostTableMedDeptLists = RefDrugGenericDetailsProvider.Instance.CostTableMedDeptList_ByID(CoID).ToObservableCollection();
                result.InwardDrugMedDeptInvoices = RefDrugGenericDetailsProvider.Instance.CostTableForMedDeptInvoice_ByCoID(CoID).ToObservableCollection();
                return result;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CostTableMedDeptList_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====== #004
        public IList<CostTableMedDeptList> InwardDrugMedDeptInvoice_GetListCost(long inviID, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugMedDeptInvoice_GetListCost(inviID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardDrugMedDeptInvoice_GetListCost. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        //▲===== #004

        #region 40. Return MedDept Member

        public bool OutWardDrugMedDeptInvoiceReturn_Insert(OutwardDrugMedDeptInvoice InvoiceDrug, out long outwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutWardDrugMedDeptInvoiceReturn_Insert(InvoiceDrug, out outwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutWardDrugMedDeptInvoiceReturn_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_AddOutWardDrugInvoiceReturnVisitor);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion 

        #region Xáp nhập phiếu nhập tạm
        public IList<InwardDrugMedDept> InwardDrugMedDeptIsInputTemp_BySupplierID(long SupplierID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugMedDeptIsInputTemp_BySupplierID(SupplierID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardDrugMedDeptIsInputTemp_BySupplierID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_InwardDrugMedDeptIsInputTemp_BySupplierID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool InwardDrugInvoices_XapNhapInputTemp_Save(long inviIDJoin, IEnumerable<InwardDrugMedDept> ObjInwardDrugMedDeptList, out string Result)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugInvoices_XapNhapInputTemp_Save(inviIDJoin, ObjInwardDrugMedDeptList, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardDrugInvoices_XapNhapInputTemp_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_InwardDrugInvoices_XapNhapInputTemp_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        public List<DrugDeptPurchaseCheckOrder> DrugDeptPurchaseOrderDetails_CheckOrder(long GenMedProductID, long V_MedProductType, DateTime FromDate, DateTime ToDate, out List<DrugDeptPurchaseCheckOrderInward> InwardList)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrderDetails_CheckOrder(GenMedProductID, V_MedProductType, FromDate, ToDate, out InwardList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrderDetails_CheckOrder. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_InwardDrugMedDeptIsInputTemp_BySupplierID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool AddTransactionMedDept(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug, out long PaymentID, long? StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddTransactionMedDept(payment, InvoiceDrug, out PaymentID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddTransactionVisitor. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_TRANSACTION_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool AddTransactionMedDeptHoanTien(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug, out long PaymentID,
                                                  long? StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddTransactionMedDeptHoanTien(payment, InvoiceDrug, out PaymentID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddTransactionVisitor. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_TRANSACTION_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool MedDeptInvoice_UpdateInvoicePayed(OutwardDrugMedDeptInvoice Outward, out long outiID, out long PaymentID,
                                          out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.MedDeptInvoice_UpdateInvoicePayed(Outward, out outiID, out PaymentID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving MedDeptInvoice_UpdateInvoicePayed. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_TRANSACTION_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool MedDeptInvoice_UpdateInvoiceInfo(OutwardDrugMedDeptInvoice Outward)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.MedDeptInvoice_UpdateInvoiceInfo(Outward);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving MedDeptInvoice_UpdateInvoiceInfo. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_TRANSACTION_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        #region Tồn Kho

        public IList<InwardDrugMedDept> InStock_MedDept(string BrandName, long StoreID, bool IsDetail, long V_MedProductType, int PageIndex, int PageSize, out int TotalCount, out decimal TotalMoney, long? BidID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving InStock_MedDept.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.InStock_MedDept(BrandName, StoreID, IsDetail, V_MedProductType, PageIndex, PageSize, out TotalCount, out TotalMoney, BidID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InStock_MedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_InwardDrugs_TonKho);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Adjust Out Price

        public IList<InwardDrugMedDept> GetInwardDrugMedDeptForAdjustOutPrice(long StoreID, bool IsCode, string BrandName, long V_MedProductType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetInwardDrugMedDeptForAdjustOutPrice.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugMedDeptForAdjustOutPrice(StoreID, IsCode, BrandName, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetInwardDrugMedDeptForAdjustOutPrice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetInwardDrugMedDeptForAdjustOutPrice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public bool MedDeptAdjustOutPrice(ObservableCollection<InwardDrugMedDept> InwardDrugMedDeptList)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving MedDeptAdjustOutPrice.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.MedDeptAdjustOutPrice(InwardDrugMedDeptList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving MedDeptAdjustOutPrice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_MedDeptAdjustOutPrice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        /*▼====: #001*/
        #region Adjust In Clinic Price
        public IList<InwardDrugClinicDept> GetInwardDrugClinicDeptForAdjustOutPrice(long StoreID, bool IsCode, string BrandName, long V_MedProductType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetInwardDrugClinicDeptForAdjustOutPrice.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugClinicDeptForAdjustOutPrice(StoreID, IsCode, BrandName, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetInwardDrugClinicDeptForAdjustOutPrice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetInwardDrugMedDeptForAdjustOutPrice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool ClinicDeptAdjustOutPrice(IList<InwardDrugClinicDept> InwardDrugMedDeptList, long? StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving ClinicDeptAdjustOutPrice.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.ClinicDeptAdjustOutPrice(InwardDrugMedDeptList, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving ClinicDeptAdjustOutPrice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_GetInwardDrugMedDeptForAdjustOutPrice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion
        /*▲====: #001*/
        //▼====== #003
        public List<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoice_Cbx_V2(long? StoreID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoice_Cbx_V2(StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoice_Cbx_V2. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #003

        public IList<OutwardDrugMedDept> GetInBatchNumberAndPrice_ListForRequest(bool? IsCost, long RequestDrugInwardHiStoreID, long StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInBatchNumberAndPrice_ListForRequest(IsCost, RequestDrugInwardHiStoreID, StoreID, V_MedProductType);
            }

            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of spGetInBatchNumberAndPrice_ListForRequest. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region 2. Inward VTYT Tieu Hao

        public List<OutwardDrugMedDeptInvoice> OutwardVTYTTHMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardVTYTTHMedDeptInvoice_Cbx(StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardVTYTTHMedDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddInwardVTYTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardVTYTTHMedDeptInvoice(InvoiceDrug, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddInwardVTYTTHMedDeptInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int UpdateInwardVTYTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardVTYTTHMedDeptInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardVTYTTHMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardVTYTTHMedDeptInvoice(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardVTYTTHMedDeptInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardVTYTTHMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InwardVTYTTHMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID)
        {
            OrderDetails = OrderDetails.Where(x => x.GenMedProductID > 0).ToList();
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                RefDrugGenericDetailsProvider.Instance.AddInwardVTYTTHMedDept(OrderDetails[i], inviID);
            }
            RefDrugGenericDetailsProvider.Instance.InwardVTYTTHMedDeptInvoice_UpdateCost(inviID);
            if (DrugDeptPoID != null && DrugDeptPoID > 0)
            {
                RefDrugGenericDetailsProvider.Instance.UpdateStatusVTYTTHMedDeptPurchaseOrder(DrugDeptPoID.GetValueOrDefault());
            }
            return true;
        }

        public int UpdateInwardVTYTTHMedDept(InwardDrugMedDept invoicedrug, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardVTYTTHMedDept(invoicedrug, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardVTYTTHMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardVTYTTHMedDept(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardVTYTTHMedDept(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardVTYTTHMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteInwardVTYTTHMedDeptTemp(long InID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteInwardVTYTTHMedDeptTemp.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardVTYTTHMedDeptTemp(InID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardVTYTTHMedDeptTemp. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptTemp);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<InwardDrugMedDeptInvoice> SearchInwardVTYTTHMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardVTYTTHMedDeptInvoice(criteria, TypID, V_MedProductType, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardVTYTTHMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDrugMedDept> GetInwardVTYTTHMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
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
                return RefDrugGenericDetailsProvider.Instance.GetInwardVTYTTHMedDept_ByIDInvoice(inviID, pageSize, pageIndex, bCountTotal, out totalCount
                , out TongTienSPChuaVAT
                , out CKTrenSP
                , out TongTienTrenSPDaTruCK
                , out TongCKTrenHoaDon
                , out TongTienHoaDonCoThueNK
                , out TongTienHoaDonCoVAT
                , out TotalVATDifferenceAmount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardVTYTTHMedDept_ByIDInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugMedDept> GetInwardVTYTTHMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardVTYTTHMedDept_ByIDInvoiceNotPaging(inviID
                                                                                        , out TongTienSPChuaVAT
                                                                                        , out CKTrenSP
                                                                                        , out TongTienTrenSPDaTruCK
                                                                                        , out TongCKTrenHoaDon
                                                                                         , out TongTienHoaDonCoThueNK
                                                                                        , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDept_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InwardDrugMedDeptInvoice GetInwardVTYTTHMedDeptInvoice_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardVTYTTHMedDeptInvoice_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardVTYTTHMedDeptInvoice_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int InwardVTYTTHMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardVTYTTHMedDeptInvoice_SaveXML(InvoiceDrug, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardVTYTTHMedDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<CostTableMedDeptList> InwardVTYTTHMedDeptInvoice_GetListCost(long inviID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardVTYTTHMedDeptInvoice_GetListCost(inviID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardVTYTTHMedDeptInvoice_GetListCost. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 2. Inward Vaccine

        public List<OutwardDrugMedDeptInvoice> OutwardTiemNguaMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardTiemNguaMedDeptInvoice_Cbx(StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardTiemNguaMedDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddInwardTiemNguaMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardTiemNguaMedDeptInvoice(InvoiceDrug, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddInwardTiemNguaMedDeptInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int UpdateInwardTiemNguaMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardTiemNguaMedDeptInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardTiemNguaMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardTiemNguaMedDeptInvoice(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardTiemNguaMedDeptInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardTiemNguaMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InwardTiemNguaMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID)
        {
            OrderDetails = OrderDetails.Where(x => x.GenMedProductID > 0).ToList();
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                RefDrugGenericDetailsProvider.Instance.AddInwardTiemNguaMedDept(OrderDetails[i], inviID);
            }
            RefDrugGenericDetailsProvider.Instance.InwardTiemNguaMedDeptInvoice_UpdateCost(inviID);
            if (DrugDeptPoID != null && DrugDeptPoID > 0)
            {
                RefDrugGenericDetailsProvider.Instance.UpdateStatusTiemNguaMedDeptPurchaseOrder(DrugDeptPoID.GetValueOrDefault());
            }
            return true;
        }

        public int UpdateInwardTiemNguaMedDept(InwardDrugMedDept invoicedrug, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardTiemNguaMedDept(invoicedrug, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardTiemNguaMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardTiemNguaMedDept(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardTiemNguaMedDept(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardTiemNguaMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteInwardTiemNguaMedDeptTemp(long InID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteInwardTiemNguaMedDeptTemp.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardTiemNguaMedDeptTemp(InID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardTiemNguaMedDeptTemp. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptTemp);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<InwardDrugMedDeptInvoice> SearchInwardTiemNguaMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardTiemNguaMedDeptInvoice(criteria, TypID, V_MedProductType, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardTiemNguaMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDrugMedDept> GetInwardTiemNguaMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
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
                return RefDrugGenericDetailsProvider.Instance.GetInwardTiemNguaMedDept_ByIDInvoice(inviID, pageSize, pageIndex, bCountTotal, out totalCount
                , out TongTienSPChuaVAT
                , out CKTrenSP
                , out TongTienTrenSPDaTruCK
                , out TongCKTrenHoaDon
                , out TongTienHoaDonCoThueNK
                , out TongTienHoaDonCoVAT
                , out TotalVATDifferenceAmount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardTiemNguaMedDept_ByIDInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugMedDept> GetInwardTiemNguaMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardTiemNguaMedDept_ByIDInvoiceNotPaging(inviID
                                                                                        , out TongTienSPChuaVAT
                                                                                        , out CKTrenSP
                                                                                        , out TongTienTrenSPDaTruCK
                                                                                        , out TongCKTrenHoaDon
                                                                                         , out TongTienHoaDonCoThueNK
                                                                                        , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDept_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InwardDrugMedDeptInvoice GetInwardTiemNguaMedDeptInvoice_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardTiemNguaMedDeptInvoice_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardTiemNguaMedDeptInvoice_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int InwardTiemNguaMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardTiemNguaMedDeptInvoice_SaveXML(InvoiceDrug, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardTiemNguaMedDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<CostTableMedDeptList> InwardTiemNguaMedDeptInvoice_GetListCost(long inviID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardTiemNguaMedDeptInvoice_GetListCost(inviID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardTiemNguaMedDeptInvoice_GetListCost. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 2. Inward Chemical

        public List<OutwardDrugMedDeptInvoice> OutwardChemicalMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardChemicalMedDeptInvoice_Cbx(StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardChemicalMedDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddInwardChemicalMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardChemicalMedDeptInvoice(InvoiceDrug, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddInwardChemicalMedDeptInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int UpdateInwardChemicalMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardChemicalMedDeptInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardChemicalMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardChemicalMedDeptInvoice(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardChemicalMedDeptInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardChemicalMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InwardChemicalMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID)
        {
            OrderDetails = OrderDetails.Where(x => x.GenMedProductID > 0).ToList();
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                RefDrugGenericDetailsProvider.Instance.AddInwardChemicalMedDept(OrderDetails[i], inviID);
            }
            RefDrugGenericDetailsProvider.Instance.InwardChemicalMedDeptInvoice_UpdateCost(inviID);
            if (DrugDeptPoID != null && DrugDeptPoID > 0)
            {
                RefDrugGenericDetailsProvider.Instance.UpdateStatusChemicalMedDeptPurchaseOrder(DrugDeptPoID.GetValueOrDefault());
            }
            return true;
        }

        public int UpdateInwardChemicalMedDept(InwardDrugMedDept invoicedrug, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardChemicalMedDept(invoicedrug, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardChemicalMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardChemicalMedDept(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardChemicalMedDept(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardChemicalMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteInwardChemicalMedDeptTemp(long InID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteInwardChemicalMedDeptTemp.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardChemicalMedDeptTemp(InID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardChemicalMedDeptTemp. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptTemp);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<InwardDrugMedDeptInvoice> SearchInwardChemicalMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardChemicalMedDeptInvoice(criteria, TypID, V_MedProductType, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardChemicalMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDrugMedDept> GetInwardChemicalMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
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
                return RefDrugGenericDetailsProvider.Instance.GetInwardChemicalMedDept_ByIDInvoice(inviID, pageSize, pageIndex, bCountTotal, out totalCount
                , out TongTienSPChuaVAT
                , out CKTrenSP
                , out TongTienTrenSPDaTruCK
                , out TongCKTrenHoaDon
                , out TongTienHoaDonCoThueNK
                , out TongTienHoaDonCoVAT
                , out TotalVATDifferenceAmount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardChemicalMedDept_ByIDInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugMedDept> GetInwardChemicalMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardChemicalMedDept_ByIDInvoiceNotPaging(inviID
                                                                                        , out TongTienSPChuaVAT
                                                                                        , out CKTrenSP
                                                                                        , out TongTienTrenSPDaTruCK
                                                                                        , out TongCKTrenHoaDon
                                                                                         , out TongTienHoaDonCoThueNK
                                                                                        , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDept_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InwardDrugMedDeptInvoice GetInwardChemicalMedDeptInvoice_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardChemicalMedDeptInvoice_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardChemicalMedDeptInvoice_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int InwardChemicalMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardChemicalMedDeptInvoice_SaveXML(InvoiceDrug, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardChemicalMedDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<CostTableMedDeptList> InwardChemicalMedDeptInvoice_GetListCost(long inviID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardChemicalMedDeptInvoice_GetListCost(inviID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardChemicalMedDeptInvoice_GetListCost. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        #region 2. Inward Blood

        public List<OutwardDrugMedDeptInvoice> OutwardBloodMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardBloodMedDeptInvoice_Cbx(StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardBloodMedDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddInwardBloodMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardBloodMedDeptInvoice(InvoiceDrug, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddInwardBloodMedDeptInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int UpdateInwardBloodMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardBloodMedDeptInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardBloodMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardBloodMedDeptInvoice(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardBloodMedDeptInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardBloodMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InwardBloodMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID)
        {
            OrderDetails = OrderDetails.Where(x => x.GenMedProductID > 0).ToList();
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                RefDrugGenericDetailsProvider.Instance.AddInwardBloodMedDept(OrderDetails[i], inviID);
            }
            RefDrugGenericDetailsProvider.Instance.InwardBloodMedDeptInvoice_UpdateCost(inviID);
            if (DrugDeptPoID != null && DrugDeptPoID > 0)
            {
                RefDrugGenericDetailsProvider.Instance.UpdateStatusBloodMedDeptPurchaseOrder(DrugDeptPoID.GetValueOrDefault());
            }
            return true;
        }

        public int UpdateInwardBloodMedDept(InwardDrugMedDept invoicedrug, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardBloodMedDept(invoicedrug, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardBloodMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardBloodMedDept(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardBloodMedDept(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardBloodMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteInwardBloodMedDeptTemp(long InID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteInwardBloodMedDeptTemp.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardBloodMedDeptTemp(InID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardBloodMedDeptTemp. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptTemp);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<InwardDrugMedDeptInvoice> SearchInwardBloodMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardBloodMedDeptInvoice(criteria, TypID, V_MedProductType, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardBloodMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDrugMedDept> GetInwardBloodMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
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
                return RefDrugGenericDetailsProvider.Instance.GetInwardBloodMedDept_ByIDInvoice(inviID, pageSize, pageIndex, bCountTotal, out totalCount
                , out TongTienSPChuaVAT
                , out CKTrenSP
                , out TongTienTrenSPDaTruCK
                , out TongCKTrenHoaDon
                , out TongTienHoaDonCoThueNK
                , out TongTienHoaDonCoVAT
                , out TotalVATDifferenceAmount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardBloodMedDept_ByIDInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugMedDept> GetInwardBloodMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardBloodMedDept_ByIDInvoiceNotPaging(inviID
                                                                                        , out TongTienSPChuaVAT
                                                                                        , out CKTrenSP
                                                                                        , out TongTienTrenSPDaTruCK
                                                                                        , out TongCKTrenHoaDon
                                                                                         , out TongTienHoaDonCoThueNK
                                                                                        , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDept_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InwardDrugMedDeptInvoice GetInwardBloodMedDeptInvoice_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardBloodMedDeptInvoice_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardBloodMedDeptInvoice_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int InwardBloodMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardBloodMedDeptInvoice_SaveXML(InvoiceDrug, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardBloodMedDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<CostTableMedDeptList> InwardBloodMedDeptInvoice_GetListCost(long inviID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardBloodMedDeptInvoice_GetListCost(inviID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardBloodMedDeptInvoice_GetListCost. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 2. Inward ThanhTrung

        public List<OutwardDrugMedDeptInvoice> OutwardThanhTrungMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardThanhTrungMedDeptInvoice_Cbx(StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardThanhTrungMedDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddInwardThanhTrungMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardThanhTrungMedDeptInvoice(InvoiceDrug, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddInwardThanhTrungMedDeptInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int UpdateInwardThanhTrungMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardThanhTrungMedDeptInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardThanhTrungMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardThanhTrungMedDeptInvoice(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardThanhTrungMedDeptInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardThanhTrungMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InwardThanhTrungMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID)
        {
            OrderDetails = OrderDetails.Where(x => x.GenMedProductID > 0).ToList();
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                RefDrugGenericDetailsProvider.Instance.AddInwardThanhTrungMedDept(OrderDetails[i], inviID);
            }
            RefDrugGenericDetailsProvider.Instance.InwardThanhTrungMedDeptInvoice_UpdateCost(inviID);
            if (DrugDeptPoID != null && DrugDeptPoID > 0)
            {
                RefDrugGenericDetailsProvider.Instance.UpdateStatusThanhTrungMedDeptPurchaseOrder(DrugDeptPoID.GetValueOrDefault());
            }
            return true;
        }

        public int UpdateInwardThanhTrungMedDept(InwardDrugMedDept invoicedrug, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardThanhTrungMedDept(invoicedrug, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardThanhTrungMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardThanhTrungMedDept(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardThanhTrungMedDept(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardThanhTrungMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteInwardThanhTrungMedDeptTemp(long InID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteInwardThanhTrungMedDeptTemp.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardThanhTrungMedDeptTemp(InID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardThanhTrungMedDeptTemp. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptTemp);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<InwardDrugMedDeptInvoice> SearchInwardThanhTrungMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardThanhTrungMedDeptInvoice(criteria, TypID, V_MedProductType, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardThanhTrungMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDrugMedDept> GetInwardThanhTrungMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
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
                return RefDrugGenericDetailsProvider.Instance.GetInwardThanhTrungMedDept_ByIDInvoice(inviID, pageSize, pageIndex, bCountTotal, out totalCount
                , out TongTienSPChuaVAT
                , out CKTrenSP
                , out TongTienTrenSPDaTruCK
                , out TongCKTrenHoaDon
                , out TongTienHoaDonCoThueNK
                , out TongTienHoaDonCoVAT
                , out TotalVATDifferenceAmount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardThanhTrungMedDept_ByIDInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugMedDept> GetInwardThanhTrungMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardThanhTrungMedDept_ByIDInvoiceNotPaging(inviID
                                                                                        , out TongTienSPChuaVAT
                                                                                        , out CKTrenSP
                                                                                        , out TongTienTrenSPDaTruCK
                                                                                        , out TongCKTrenHoaDon
                                                                                         , out TongTienHoaDonCoThueNK
                                                                                        , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDept_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InwardDrugMedDeptInvoice GetInwardThanhTrungMedDeptInvoice_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardThanhTrungMedDeptInvoice_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardThanhTrungMedDeptInvoice_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int InwardThanhTrungMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardThanhTrungMedDeptInvoice_SaveXML(InvoiceDrug, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardThanhTrungMedDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<CostTableMedDeptList> InwardThanhTrungMedDeptInvoice_GetListCost(long inviID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardThanhTrungMedDeptInvoice_GetListCost(inviID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardThanhTrungMedDeptInvoice_GetListCost. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 2. Inward VPP

        public List<OutwardDrugMedDeptInvoice> OutwardVPPMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardVPPMedDeptInvoice_Cbx(StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardVPPMedDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddInwardVPPMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardVPPMedDeptInvoice(InvoiceDrug, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddInwardVPPMedDeptInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int UpdateInwardVPPMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardVPPMedDeptInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardVPPMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardVPPMedDeptInvoice(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardVPPMedDeptInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardVPPMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InwardVPPMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID)
        {
            OrderDetails = OrderDetails.Where(x => x.GenMedProductID > 0).ToList();
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                RefDrugGenericDetailsProvider.Instance.AddInwardVPPMedDept(OrderDetails[i], inviID);
            }
            RefDrugGenericDetailsProvider.Instance.InwardVPPMedDeptInvoice_UpdateCost(inviID);
            if (DrugDeptPoID != null && DrugDeptPoID > 0)
            {
                RefDrugGenericDetailsProvider.Instance.UpdateStatusVPPMedDeptPurchaseOrder(DrugDeptPoID.GetValueOrDefault());
            }
            return true;
        }

        public int UpdateInwardVPPMedDept(InwardDrugMedDept invoicedrug, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardVPPMedDept(invoicedrug, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardVPPMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardVPPMedDept(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardVPPMedDept(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardVPPMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteInwardVPPMedDeptTemp(long InID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardVPPMedDeptTemp(InID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardVPPMedDeptTemp. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptTemp);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<InwardDrugMedDeptInvoice> SearchInwardVPPMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardVPPMedDeptInvoice(criteria, TypID, V_MedProductType, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardVPPMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDrugMedDept> GetInwardVPPMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
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
                return RefDrugGenericDetailsProvider.Instance.GetInwardVPPMedDept_ByIDInvoice(inviID, pageSize, pageIndex, bCountTotal, out totalCount
                , out TongTienSPChuaVAT
                , out CKTrenSP
                , out TongTienTrenSPDaTruCK
                , out TongCKTrenHoaDon
                , out TongTienHoaDonCoThueNK
                , out TongTienHoaDonCoVAT
                , out TotalVATDifferenceAmount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardVPPMedDept_ByIDInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugMedDept> GetInwardVPPMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardVPPMedDept_ByIDInvoiceNotPaging(inviID
                                                                                        , out TongTienSPChuaVAT
                                                                                        , out CKTrenSP
                                                                                        , out TongTienTrenSPDaTruCK
                                                                                        , out TongCKTrenHoaDon
                                                                                         , out TongTienHoaDonCoThueNK
                                                                                        , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDept_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InwardDrugMedDeptInvoice GetInwardVPPMedDeptInvoice_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardVPPMedDeptInvoice_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardVPPMedDeptInvoice_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int InwardVPPMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardVPPMedDeptInvoice_SaveXML(InvoiceDrug, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardVPPMedDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<CostTableMedDeptList> InwardVPPMedDeptInvoice_GetListCost(long inviID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardVPPMedDeptInvoice_GetListCost(inviID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardVPPMedDeptInvoice_GetListCost. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        #region 3. Inward VTTH
        public List<OutwardDrugMedDeptInvoice> OutwardVTTHMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardVTTHMedDeptInvoice_Cbx(StoreID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardVTTHMedDeptInvoice_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddInwardVTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardVTTHMedDeptInvoice(InvoiceDrug, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddInwardVTTHMedDeptInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int UpdateInwardVTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardVTTHMedDeptInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardVTTHMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardVTTHMedDeptInvoice(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardVTTHMedDeptInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardVTTHMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InwardVTTHMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID)
        {
            OrderDetails = OrderDetails.Where(x => x.GenMedProductID > 0).ToList();
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                RefDrugGenericDetailsProvider.Instance.AddInwardVTTHMedDept(OrderDetails[i], inviID);
            }
            RefDrugGenericDetailsProvider.Instance.InwardVTTHMedDeptInvoice_UpdateCost(inviID);
            if (DrugDeptPoID != null && DrugDeptPoID > 0)
            {
                RefDrugGenericDetailsProvider.Instance.UpdateStatusVTTHMedDeptPurchaseOrder(DrugDeptPoID.GetValueOrDefault());
            }
            return true;
        }

        public int UpdateInwardVTTHMedDept(InwardDrugMedDept invoicedrug, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardVTTHMedDept(invoicedrug, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardVTTHMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_UpdateInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int DeleteInwardVTTHMedDept(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardVTTHMedDept(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardVTTHMedDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDept);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteInwardVTTHMedDeptTemp(long InID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteInwardVTTHMedDeptTemp.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardVTTHMedDeptTemp(InID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardVTTHMedDeptTemp. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_DeleteInwardDrugMedDeptTemp);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<InwardDrugMedDeptInvoice> SearchInwardVTTHMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardVTTHMedDeptInvoice(criteria, TypID, V_MedProductType, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardVTTHMedDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_SearchInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDrugMedDept> GetInwardVTTHMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
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
                return RefDrugGenericDetailsProvider.Instance.GetInwardVTTHMedDept_ByIDInvoice(inviID, pageSize, pageIndex, bCountTotal, out totalCount
                , out TongTienSPChuaVAT
                , out CKTrenSP
                , out TongTienTrenSPDaTruCK
                , out TongCKTrenHoaDon
                , out TongTienHoaDonCoThueNK
                , out TongTienHoaDonCoVAT
                , out TotalVATDifferenceAmount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardVTTHMedDept_ByIDInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugMedDept> GetInwardVTTHMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardVTTHMedDept_ByIDInvoiceNotPaging(inviID
                                                                                            , out TongTienSPChuaVAT
                                                                                            , out CKTrenSP
                                                                                            , out TongTienTrenSPDaTruCK
                                                                                            , out TongCKTrenHoaDon
                                                                                             , out TongTienHoaDonCoThueNK
                                                                                            , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardVTTHMedDept_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InwardDrugMedDeptInvoice GetInwardVTTHMedDeptInvoice_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardVTTHMedDeptInvoice_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardVTTHMedDeptInvoice_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetInwardDrugMedDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int InwardVTTHMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardVTTHMedDeptInvoice_SaveXML(InvoiceDrug, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardVTTHMedDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<CostTableMedDeptList> InwardVTTHMedDeptInvoice_GetListCost(long inviID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardVTTHMedDeptInvoice_GetListCost(inviID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardVTTHMedDeptInvoice_GetListCost. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Báo giá
        public List<RefGenMedProductDetails> GetGenMedProductAndPrice(bool IsCode, string BrandName, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetGenMedProductAndPrice(IsCode, BrandName, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetGenMedProductAndPrice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        #region Cân bằng
        public bool DrugMedDeptInvoice_SaveByType_Balance(OutwardDrugMedDeptInvoice Invoice, int ViewCase, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugMedDeptInvoice_SaveByType_Balance(Invoice, ViewCase, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoice_SaveByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<RefGenMedProductDetails> GetDrugForBalanceCompleteFromCategory(bool IsSearchByGenericName, bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode, long? OutputToStoreID, long? RefGenDrugCatID_2,
           long? RequestTemplateID,
           bool IsGetOnlyRemain)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForBalanceCompleteFromCategory(IsSearchByGenericName, IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, OutputToStoreID, RefGenDrugCatID_2, RequestTemplateID, IsGetOnlyRemain);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region
        public bool DrugDeptStockTake_SaveNew(DrugDeptStockTakes StockTake, bool IsConfirmFinished, bool IsAlreadyRefresh, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptStockTake_SaveNew(StockTake, IsConfirmFinished, IsAlreadyRefresh, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptStockTake_Save. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_spGetInBatchNumberAndPrice_ListForRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        #region
        public List<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoiceConsignment_Cbx(long? StoreID, long V_MedProductType, bool IsFromClinic)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoiceConsignment_Cbx(StoreID, V_MedProductType, IsFromClinic);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoiceConsignment_Cbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateReceipt(InwardDrugMedDeptInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateReceipt(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateReceipt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_AddInwardDrugMedDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_ForConsignment(bool IsSearchByGenericName, bool? IsCost
          , string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode, long? OutputToStoreID, long? RefGenDrugCatID_2
          , long? RequestTemplateID, bool IsGetOnlyRemain, bool IsCOVID, bool IsReturn)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_ForConsignment(IsSearchByGenericName, IsCost
                    , BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, OutputToStoreID, RefGenDrugCatID_2, RequestTemplateID, IsGetOnlyRemain
                    , IsCOVID, IsReturn);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        #region Đơn thuốc điện tử
        public IList<PatientRegistration> SearchRegistrationsForElectronicPrescription(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading SearchRegistrationsForElectronicPrescription.", CurrentUser);
                IList<PatientRegistration> bRet;

                bRet = RefDrugGenericDetailsProvider.Instance.SearchRegistrationsForElectronicPrescription(aSeachPtRegistrationCriteria, ViewCase
                     , PageIndex, PageSize, OrderBy, CountTotal, out Total);
                AxLogger.Instance.LogInfo("End loading SearchRegistrationsForElectronicPrescription.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading SearchRegistrationsForElectronicPrescription. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<DTDT_don_thuoc> PreviewElectronicPrescription(long PtRegistrationID, long V_RegistrationType, long? OutPtTreatmentProgramID, out string ErrText)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.PreviewElectronicPrescription(PtRegistrationID, V_RegistrationType, OutPtTreatmentProgramID, out ErrText);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading PreviewElectronicPrescription. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, "Error PreviewElectronicPrescription", CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<DTDT_don_thuoc> PreviewElectronicPrescription_Cancel(long PtRegistrationID, long IssueID, long V_RegistrationType, long? OutPtTreatmentProgramID, out string ErrText)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.PreviewElectronicPrescription_Cancel(PtRegistrationID, IssueID, V_RegistrationType, OutPtTreatmentProgramID, out ErrText);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading PreviewElectronicPrescription_Cancel. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, "Error PreviewElectronicPrescription_Cancel", CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion
        //▼====: #006
        public IList<PatientRegistration> SearchRegistrationsForElectronicPrescriptionPharmacy(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, bool IsTotalTab, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;
            try
            {
                AxLogger.Instance.LogInfo("Start loading SearchRegistrationsForElectronicPrescriptionPharmacy.", CurrentUser);
                IList<PatientRegistration> bRet;

                bRet = RefDrugGenericDetailsProvider.Instance.SearchRegistrationsForElectronicPrescriptionPharmacy(aSeachPtRegistrationCriteria, IsTotalTab, pageIndex, pageSize, out totalCount);
                AxLogger.Instance.LogInfo("End loading SearchRegistrationsForElectronicPrescriptionPharmacy.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading SearchRegistrationsForElectronicPrescriptionPharmacy. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<DQG_don_thuoc> PreviewElectronicPrescriptionPharmacy(long PtRegistrationID, long V_RegistrationType, long IssueID, long? OutPtTreatmentProgramID, out string ErrText)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.RefDrugGenericDetailsProvider.Instance.PreviewElectronicPrescriptionPharmacy(PtRegistrationID, V_RegistrationType, IssueID, OutPtTreatmentProgramID, out ErrText);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End loading PreviewElectronicPrescriptionPharmacy. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, "Error PreviewElectronicPrescriptionPharmacy", CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #006
    }
}
