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
using System.Linq;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;
using System.Data.SqlClient;

/*
 * 20180412 #001 TTM: BM 0005324: Nhập thuốc từ khoa dược vào kho BHYT - Nhà thuốc.
 * 20181219 #002 TTM: BM 0005443: Lập phiếu lĩnh thuốc cho kho BHYT - Nhà thuốc.
 * 20201026 #003 TNHX: BM: Thêm func Cancel Duyệt phiếu lĩnh cho Khoa Dược
 * 20210109 #004 TNHX: BM: Hoàn thiện chức năng quản lý suất ăn
 * 20220112 #005 BLQ: Thêm loại đăng ký để tìm phiếu lĩnh ngoại trú
 * 20230508 #006 DatTB: Thêm nút xuất Excel danh sách các mẫu lĩnh
 * 20230511 #007 QTD: Thêm chức năng Tab Vật tư y tế kèm DVKT
 */
namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single)]
    [KnownType(typeof(AxException))]
    public class InCLinicDeptService : eHCMS.WCFServiceCustomHeader, IInClinicDept
    {
        public InCLinicDeptService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        #region 17. Inward Drug For Clinic Dept

        public IList<InwardDrugClinicDeptInvoice> SearchInwardDrugClinicDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardDrugClinicDeptInvoice(criteria, TypID, V_MedProductType, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardDrugClinicDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchInwardDrugClinicDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InwardDrugClinicDeptInvoice GetInwardDrugClinicDeptInvoice_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugClinicDeptInvoice_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugClinicDeptInvoice_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetInwardDrugClinicDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InwardDrugClinicDeptInvoice GetInwardDrugClinicDeptInvoice_ByID_V2(long ID, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugClinicDeptInvoice_ByID_V2(ID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugClinicDeptInvoice_ByID_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetInwardDrugClinicDeptInvoice_ByID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public InwardDrugClinicDeptInvoice GetInwardDrugMedDeptInvoice_ByID(long ID, long V_MedProductType)//--02/01/2021 DatTB Thêm biến V_MedProductType
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugMedDeptInvoice_ByID_New(ID, V_MedProductType);//--02/01/2021 DatTB Thêm biến V_MedProductType
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugMedDeptInvoice_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetInwardDrugClinicDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDrugClinicDept> GetInwardDrugClinicDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
        , out decimal TongTienSPChuaVAT
        , out decimal CKTrenSP
        , out decimal TongTienTrenSPDaTruCK
        , out decimal TongCKTrenHoaDon
        , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugClinicDept_ByIDInvoice(inviID, pageSize, pageIndex, bCountTotal, out totalCount
                                                                                                    , out TongTienSPChuaVAT
                                                                                                    , out CKTrenSP
                                                                                                    , out TongTienTrenSPDaTruCK
                                                                                                    , out TongCKTrenHoaDon
                                                                                                    , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugClinicDept_ByIDInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetInwardDrugClinicDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugClinicDept> GetInwardDrugClinicDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoVAT)
        {

            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugClinicDept_ByIDInvoiceNotPaging(inviID
                    , out TongTienSPChuaVAT
                    , out CKTrenSP
                    , out TongTienTrenSPDaTruCK
                    , out TongCKTrenHoaDon
                    , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugClinicDept_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetInwardDrugClinicDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugClinicDept> GetInwardDrugClinicDept_ByIDInvoiceNotPaging_V2(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoVAT
            , long? V_MedProductType
            , bool IsMedDeptSubStorage)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugClinicDept_ByIDInvoiceNotPaging_V2(inviID
                    , out TongTienSPChuaVAT
                    , out CKTrenSP
                    , out TongTienTrenSPDaTruCK
                    , out TongCKTrenHoaDon
                    , out TongTienHoaDonCoVAT
                    , V_MedProductType
                    , IsMedDeptSubStorage);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugClinicDept_ByIDInvoiceNotPaging_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetInwardDrugClinicDept_ByIDInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int AddInwardDrugClinicDeptInvoice(InwardDrugClinicDeptInvoice InvoiceDrug, out long inwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardDrugClinicDeptInvoice(InvoiceDrug, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddInwardDrugClinicDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_AddInwardDrugClinicDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int DeleteInwardDrugClinicDept(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardDrugClinicDept(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardDrugClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_DeleteInwardDrugClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int DeleteInwardDrugClinicDeptInvoice(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardDrugClinicDeptInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInwardDrugClinicDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_DeleteInwardDrugClinicDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int UpdateInwardDrugClinicDept(InwardDrugClinicDept invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardDrugClinicDept(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardDrugClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_UpdateInwardDrugClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int UpdateInwardDrugClinicDeptInvoice(InwardDrugClinicDeptInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInwardDrugClinicDeptInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInwardDrugClinicDeptInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_UpdateInwardDrugClinicDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefGenMedProductDetails> spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID(GenMedProductID, V_MedProductType, StoreID, IsHIPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InwardDrugClinicDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID)
        {
            OrderDetails = OrderDetails.Where(x => x.GenMedProductID > 0).ToList();
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                RefDrugGenericDetailsProvider.Instance.AddInwardDrugClinicDept(OrderDetails[i], inviID);
            }
            return true;
        }

        public bool ExistInwardDrug(InwardDrugClinicDeptInvoice invoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ExistInwardDrug(invoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ExistInwardDrug. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int InwardDrugClinicDeptInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugClinicDeptInvoice_SaveXML(InvoiceDrug, DSPTModifiedDate_Outward, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardDrugClinicDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //--▼--02/01/2021 DatTB
        public int InwardDrugInternalMedDeptInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugInternalMedDeptInvoice_SaveXML(InvoiceDrug, DSPTModifiedDate_Outward, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardDrugClinicDeptInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //--▲--02/01/2021 DatTB
        public bool AcceptAutoUpdateInwardClinicInvoice(long inviID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AcceptAutoUpdateInwardClinicInvoice(inviID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AcceptAutoUpdateInwardClinicInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_AcceptAutoUpdateInwardClinicInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        public bool SaveOutwardDrugClinicDeptTemplate(OutwardDrugClinicDeptTemplate OutwardTemplate, out OutwardDrugClinicDeptTemplate OutwardTemplateOut)
        {
            try
            {

                AxLogger.Instance.LogInfo("Start of SaveOutwardDrugClinicDeptTemplate.", CurrentUser);
                OutwardTemplateOut = new OutwardDrugClinicDeptTemplate();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.SaveOutwardDrugClinicDeptTemplate(OutwardTemplate, out id);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    OutwardTemplateOut = GetOutwardTemplateByID(id);
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveOutwardDrugClinicDeptTemplate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SaveOutwardDrugClinicDeptTemplate);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public OutwardDrugClinicDeptTemplate GetOutwardTemplateByID(long OutwardTemplateID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of GetOutwardTemplateByID.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugClinicDeptTemplate(OutwardTemplateID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetOutwardTemplateByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetOutwardTemplateByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<OutwardDrugClinicDeptTemplate> GetAllOutwardTemplate(long V_MedProductType, long DeptID, long? V_OutwardTemplateType, bool? IsShared)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of GetAllOutwardTemplate.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.GetAllOutwardTemplate(V_MedProductType, DeptID, V_OutwardTemplateType, IsShared);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllOutwardTemplate. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetAllOutwardTemplate);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public bool DeleteOutwardDrugClinicDeptTemplate(long OutwardTemplateID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteOutwardDrugClinicDeptTemplate.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteOutwardDrugClinicDeptTemplate(OutwardTemplateID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteOutwardDrugClinicDeptTemplate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_DeleteOutwardDrugClinicDeptTemplate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool FullOperatorRequestDrugInwardClinicDept(RequestDrugInwardClinicDept Request,
                                                                 long V_MedProductType, out RequestDrugInwardClinicDept OutRequest)
        {
            try
            {
                OutRequest = new RequestDrugInwardClinicDept();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.FullOperatorRequestDrugInwardClinicDept(Request, V_MedProductType, out id);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptByID(id);
                    if (OutRequest != null)
                    {
                        OutRequest.RequestDetails = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptDetailByID(id).ToObservableCollection();
                    }
                }
                else
                {
                    if (Request.ReqDrugInClinicDeptID > 0)
                    {
                        OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptByID(Request.ReqDrugInClinicDeptID);
                        if (OutRequest != null)
                        {
                            OutRequest.RequestDetails = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptDetailByID(Request.ReqDrugInClinicDeptID).ToObservableCollection();
                        }
                    }
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardClinicDeptDetailByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool FullOperatorRequestDrugInwardClinicDeptNew(RequestDrugInwardClinicDept Request, long V_MedProductType
            //▼====: #005
            , long V_RegistrationType, out RequestDrugInwardClinicDept OutRequest)
            //▲====: #005
        {
            try
            {
                OutRequest = new RequestDrugInwardClinicDept();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.FullOperatorRequestDrugInwardClinicDeptNew(Request, V_MedProductType, out id);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptByID(id);
                    if (OutRequest != null)
                    {
                        OutRequest.ReqOutwardDetails = RefDrugGenericDetailsProvider.Instance. GetReqOutwardDrugClinicDeptPatientByID(id, false
                            //▼====: #005
                            , V_RegistrationType).ToObservableCollection();
                            //▲====: #005
                    }
                }
                else
                {
                    if (Request.ReqDrugInClinicDeptID > 0)
                    {
                        OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptByID(Request.ReqDrugInClinicDeptID);
                        if (OutRequest != null)
                        {
                            OutRequest.ReqOutwardDetails = RefDrugGenericDetailsProvider.Instance.GetReqOutwardDrugClinicDeptPatientByID(Request.ReqDrugInClinicDeptID
                                //▼====: #005
                                , false, V_RegistrationType).ToObservableCollection();
                                //▲====: #005
                        }
                    }
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardClinicDeptDetailByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public RequestDrugInwardClinicDept GetRequestDrugInwardClinicDeptByID(long ReqDrugInClinicDeptID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptByID(ReqDrugInClinicDeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardClinicDeptByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ReqOutwardDrugClinicDeptPatient> GetRemainingQtyForInPtRequestDrug(long? StoreID, long V_MedProductType, long RefGenDrugCatID_1)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRemainingQtyForInPtRequestDrug(StoreID, V_MedProductType, RefGenDrugCatID_1);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardClinicDeptDetailByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<RequestDrugInwardClinicDeptDetail> GetRequestDrugInwardClinicDeptDetailByID(long ReqDrugInClinicDeptID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDeptDetailByID(ReqDrugInClinicDeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardClinicDeptDetailByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ReqOutwardDrugClinicDeptPatient> GetReqOutwardDrugClinicDeptPatientByID(long ReqDrugInClinicDeptID, bool bGetExistingReqToCreateNew
            //▼====: #005
            , long V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU)
            //▲====: #005
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetReqOutwardDrugClinicDeptPatientByID(ReqDrugInClinicDeptID, bGetExistingReqToCreateNew
                    //▼====: #005
                    , V_RegistrationType);
                    //▲====: #005
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetReqOutwardDrugClinicDeptPatientByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ReqOutwardDrugClinicDeptPatient> ReqOutwardDrugClinicFromInstruction(DateTime? FromDate, DateTime? ToDate, long RefGenDrugCatID_1, long DeptID, long V_MedProductType, bool IsInstructionFuture)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ReqOutwardDrugClinicFromInstruction(FromDate, ToDate, RefGenDrugCatID_1, DeptID, V_MedProductType, IsInstructionFuture);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ReqOutwardDrugClinicFromInstruction. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RequestDrugInwardClinicDept_Approved(RequestDrugInwardClinicDept Request)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RequestDrugInwardClinicDept_Approved(Request);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RequestDrugInwardClinicDept_Approved. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #003
        public bool RequestDrugInwardClinicDept_Cancel(RequestDrugInwardClinicDept Request, long CancelStaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RequestDrugInwardClinicDept_Cancel(Request, CancelStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RequestDrugInwardClinicDept_Cancel. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                //throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #003


        public List<ReqOutwardDrugClinicDeptPatient> GetReqOutwardDrugClinicDeptPatientSumByID(long ReqDrugInClinicDeptID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetReqOutwardDrugClinicDeptPatientSumByID(ReqDrugInClinicDeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetReqOutwardDrugClinicDeptPatientSumByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RequestDrugInwardClinicDept> SearchRequestDrugInwardClinicDept(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRequestDrugInwardClinicDept(Criteria, V_MedProductType, pageIndex, pageSize, bCount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRequestDrugInwardClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RequestDrugInwardClinicDept> GetRequestDrugInwardClinicDept_ByRegistrationID(long PtRegistrationID, long V_MedProductType, long StoreID, long? outiID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardClinicDept_ByRegistrationID(PtRegistrationID, V_MedProductType, StoreID, outiID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardClinicDept_ByRegistrationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteRequestDrugInwardClinicDept(long ReqDrugInClinicDeptID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteRequestDrugInwardClinicDept(ReqDrugInClinicDeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteRequestDrugInwardClinicDept. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_DeleteRequestDrugInwardClinicDept);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestDrugList, IsCode, PtRegistrationID, IsHIPatient, OutputID /*--28/01/2021 DatTB Thêm biến*/);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugForSellVisitorAutoComplete_ForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region 18. Outward Drug For Clinic Dept


        public List<OutwardDrugClinicDept> spGetInBatchNumberAndPrice_ListForRequestClinicDept(bool? IsCost,
                                                                                        List<RequestDrugInwardClinicDept> ReqDrugInClinicDeptID,
                                                                                        long OutwardTemplateID,
                                                                                        long StoreID,
                                                                                        long V_MedProductType, long PtRegistrationID, bool? IsHIPatient, DateTime OutDate)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAndPrice_ListForRequestClinicDept(IsCost, ReqDrugInClinicDeptID, OutwardTemplateID, StoreID, V_MedProductType, PtRegistrationID, IsHIPatient, OutDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of spGetInBatchNumberAndPrice_ListForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<OutwardDrugClinicDept> GetInBatchNumberAndPrice_ListForRequestClinicDeptFromInstruction(long[] InstructionIDCollection
            , long StoreID
            , long V_MedProductType
            , long PtRegistrationID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInBatchNumberAndPrice_ListForRequestClinicDeptFromInstruction(InstructionIDCollection, StoreID, V_MedProductType, PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInBatchNumberAndPrice_ListForRequestClinicDeptFromInstruction. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<OutwardDrugClinicDept> OutwardDrugClinicDeptInvoices_SearchTKPaging(SearchOutwardInfo SearchCriteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugClinicDeptInvoices_SearchTKPaging(SearchCriteria, V_MedProductType, pageIndex, pageSize, bCount, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugClinicDeptInvoices_SearchTKPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool OutwardDrugClinicDeptInvoice_SaveByType(OutwardDrugClinicDeptInvoice Invoice, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugClinicDeptInvoice_SaveByType(Invoice, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugClinicDeptInvoice_SaveByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool OutwardDrugClinicDeptInvoice_UpdateByType(OutwardDrugClinicDeptInvoice Invoice, out long ID, out string StrError)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of OutwardDrugClinicDeptInvoice_UpdateByType.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugClinicDeptInvoice_UpdateByType(Invoice, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugClinicDeptInvoice_UpdateByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_UpdateByType);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public OutwardDrugClinicDeptInvoice GetOutwardDrugClinicDeptInvoice(long ID)
        {
            return GetOutwardDrugClinicDeptInvoice_V2(ID, null);
        }
        public OutwardDrugClinicDeptInvoice GetOutwardDrugClinicDeptInvoice_V2(long ID, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugClinicDeptInvoice_V2(ID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetOutwardDrugClinicDeptInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<OutwardDrugClinicDept> GetOutwardDrugClinicDeptDetailByInvoice(long ID)
        {
            return GetOutwardDrugClinicDeptDetailByInvoice_V2(ID, null);
        }
        public List<OutwardDrugClinicDept> GetOutwardDrugClinicDeptDetailByInvoice_V2(long ID, long? V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetOutwardDrugClinicDeptDetailByInvoice_V2(ID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetOutwardDrugClinicDeptDetailByInvoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<OutwardDrugClinicDeptInvoice> OutwardDrugClinicDeptInvoice_SearchByType(MedDeptInvoiceSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugClinicDeptInvoice_SearchByType(Criteria, pageIndex, pageSize, bCount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugClinicDeptInvoice_SearchByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefGenMedProductDetails> spGetInBatchNumberAllClinicDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.spGetInBatchNumberAllClinicDept_ByGenMedProductID(GenMedProductID, V_MedProductType, StoreID, IsHIPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of spGetInBatchNumberAllClinicDept_ByGenMedProductID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public StaffDeptPresence GetAllStaffDeptPresenceInfo(long DeptID, DateTime StaffCountDate)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllStaffDeptPresenceInfo(DeptID, StaffCountDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllStaffDeptPresenceInfo. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public StaffDeptPresence SaveAllStaffDeptPresenceInfo(StaffDeptPresence CurStaffDeptPresence, bool IsUpdateRequiredNumber)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SaveAllStaffDeptPresenceInfo(CurStaffDeptPresence, IsUpdateRequiredNumber);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveAllStaffDeptPresenceInfo. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        //▼====== #001
        public int InwardDrugInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, out long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrugInvoice_SaveXML(InvoiceDrug, DSPTModifiedDate_Outward, out id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InwardDrugInvoice_SaveXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<InwardDrugClinicDeptInvoice> SearchInwardDrugInvoiceForPharmacy(InwardInvoiceSearchCriteria criteria, long? TypID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchInwardDrugInvoiceForPharmacy(criteria, TypID, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchInwardDrugInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchInwardDrugClinicDeptInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<InwardDrugClinicDept> GetInwardDrugPharmacy_ByIDInvoiceNotPaging(long inviID
         , out decimal TongTienSPChuaVAT
         , out decimal CKTrenSP
         , out decimal TongTienTrenSPDaTruCK
         , out decimal TongCKTrenHoaDon
         , out decimal TongTienHoaDonCoVAT)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInwardDrugPharmacy_ByIDInvoiceNotPaging(inviID
        , out TongTienSPChuaVAT
        , out CKTrenSP
        , out TongTienTrenSPDaTruCK
        , out TongCKTrenHoaDon
        , out TongTienHoaDonCoVAT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInwardDrugPharmacy_ByIDInvoiceNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetInwardDrugClinicDept_ByIDInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #001
        //▼====== #002
        public bool RequestDrugInwardHIStore_Save(RequestDrugInwardForHiStore Request, long V_MedProductType, out RequestDrugInwardForHiStore OutRequest)
        {
            try
            {
                OutRequest = new RequestDrugInwardForHiStore();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.RequestDrugInwardHIStore_Save(Request, V_MedProductType, out id);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardHIStoreByID(id);
                    if (OutRequest != null)
                    {
                        OutRequest.ReqOutwardDetails = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardHIStoreDetailByID(id, false).ToObservableCollection();
                    }
                }
                else
                {
                    if (Request.RequestDrugInwardHiStoreID > 0)
                    {
                        OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardHIStoreByID(Request.RequestDrugInwardHiStoreID);
                        if (OutRequest != null)
                        {
                            OutRequest.ReqOutwardDetails = RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardHIStoreDetailByID(Request.RequestDrugInwardHiStoreID, false).ToObservableCollection();
                        }
                    }

                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RequestDrugInwardHIStore_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RequestDrugInwardForHiStore> SearchRequestDrugInwardHIStore(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRequestDrugInwardHIStore(Criteria, V_MedProductType, pageIndex, pageSize, bCount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRequestDrugInwardHIStore. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<RequestDrugInwardForHiStoreDetails> GetRequestDrugInwardHIStoreDetailByID(long ReqDrugInHIStoreID, bool bGetExistingReqToCreateNew)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardHIStoreDetailByID(ReqDrugInHIStoreID, bGetExistingReqToCreateNew);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetReqOutwardDrugHIStoreByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public RequestDrugInwardForHiStore GetRequestDrugInwardHIStoreByID(long RequestDrugInwardHiStoreID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugInwardHIStoreByID(RequestDrugInwardHiStoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardHIStoreByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RequestDrugInwardHIStore_Approved(RequestDrugInwardForHiStore Request)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RequestDrugInwardHIStore_Approved(Request);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RequestDrugInwardHIStore_Approved. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteRequestDrugInwardHIStore(long RequestDrugInwardHiStoreID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteRequestDrugInwardHIStore(RequestDrugInwardHiStoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteRequestDrugInwardClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_DeleteRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #002
        #region AutoComplete Xuất khác
        public IList<RefGenMedProductDetails> GetVTYTTHForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetVTYTTHForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestDrugList, IsCode, PtRegistrationID, IsHIPatient, OutputID /*--28/01/2021 DatTB Thêm biến*/);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetVTYTTHForSellVisitorAutoComplete_ForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetBloodForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetBloodForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestDrugList, IsCode, PtRegistrationID, IsHIPatient, OutputID /*--28/01/2021 DatTB Thêm biến*/);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetBloodForSellVisitorAutoComplete_ForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetThanhTrungForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetThanhTrungForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestDrugList, IsCode, PtRegistrationID, IsHIPatient, OutputID /*--28/01/2021 DatTB Thêm biến*/);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetThanhTrungForSellVisitorAutoComplete_ForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetChemicalForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetChemicalForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestDrugList, IsCode, PtRegistrationID, IsHIPatient, OutputID /*--28/01/2021 DatTB Thêm biến*/);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetChemicalForSellVisitorAutoComplete_ForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetVPPForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetVPPForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestDrugList, IsCode, PtRegistrationID, IsHIPatient, OutputID /*--28/01/2021 DatTB Thêm biến*/);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetVPPForSellVisitorAutoComplete_ForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetTiemNguaForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetTiemNguaForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestDrugList, IsCode, PtRegistrationID, IsHIPatient, OutputID /*--28/01/2021 DatTB Thêm biến*/);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetTiemNguaForSellVisitorAutoComplete_ForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<RefGenMedProductDetails> GetVTTHForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient, long OutputID /*--28/01/2021 DatTB Thêm biến*/)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetVTTHForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestDrugList, IsCode, PtRegistrationID, IsHIPatient, OutputID /*--28/01/2021 DatTB Thêm biến*/);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetVTTHForSellVisitorAutoComplete_ForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        #region Other Function
        public List<RequestDrugInwardClinicDept> SearchRequestDrugInwardClinicDept_NotPaging(long V_MedProductType, long StoreID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRequestDrugInwardClinicDept_NotPaging(V_MedProductType, StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRequestDrugInwardClinicDept_NotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        public List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByIDList(long StoreID, long V_MedProductType, List<long> GenIDCollection)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllInwardDrugClinicDeptByIDList(StoreID, V_MedProductType, GenIDCollection);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllInwardDrugClinicDeptByIDList. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_DeleteRequestDrugInwardClinicDept);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void OutwardDrugClinicDeptInvoices_SaveByItemCollection(IList<OutwardDrugClinicDeptInvoice> InvoiceCollection, long StaffID, DateTime OutDate, long V_MedProductType)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.OutwardDrugClinicDeptInvoices_SaveByItemCollection(InvoiceCollection, StaffID, OutDate, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugClinicDeptInvoices_SaveByItemCollection. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RefGenMedProductDetails> ReqOutwardDrugClinicFromPrescription(long PtRegistrationID, long StoreProvided, long RefGenDrugCatID_1)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ReqOutwardDrugClinicFromPrescription(PtRegistrationID, StoreProvided, RefGenDrugCatID_1);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ReqOutwardDrugClinicFromPrescription. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool OutwardDrugClinicDeptInvoice_SaveByType_Balance(OutwardDrugClinicDeptInvoice Invoice, int ViewCase, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugClinicDeptInvoice_SaveByType_Balance(Invoice, ViewCase, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugClinicDeptInvoice_SaveByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_OutwardDrugMedDeptInvoice_SaveByType);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<RefGenMedProductDetails> GetDrugClinicDeptForBalance_FromCategory(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugClinicDeptForBalance_FromCategory(IsCost, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestDrugList, IsCode, PtRegistrationID, IsHIPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugForSellVisitorAutoComplete_ForRequestClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDDEPT_SERVICE_GetDrugForSellVisitorAutoComplete_ForRequestDrugDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #004
        #region Quản lý Suất ăn
        public List<ReqFoodClinicDeptDetail> LoadReqFoodClinicFromInstruction(DateTime? FromDate, DateTime? ToDate, long DeptID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.LoadReqFoodClinicFromInstruction(FromDate, ToDate, DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of LoadReqFoodClinicFromInstruction. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RequestFoodClinicDept> SearchRequestFoodClinicDept(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRequestFoodClinicDept(Criteria, pageIndex, pageSize, bCount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRequestFoodClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SaveRequestFoodClinicDept(RequestFoodClinicDept Request, out RequestFoodClinicDept OutRequest)
        {
            try
            {
                OutRequest = new RequestFoodClinicDept();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.SaveRequestFoodClinicDept(Request, out id);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestFoodClinicDeptByID(id);
                    if (OutRequest != null)
                    {
                        OutRequest.RequestDetails = RefDrugGenericDetailsProvider.Instance.GetReqFoodClinicDeptDetailByID(id).ToObservableCollection();
                    }
                }
                else
                {
                    if (Request.ReqFoodClinicDeptID > 0)
                    {
                        OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestFoodClinicDeptByID(Request.ReqFoodClinicDeptID);
                        if (OutRequest != null)
                        {
                            OutRequest.RequestDetails = RefDrugGenericDetailsProvider.Instance.GetReqFoodClinicDeptDetailByID(Request.ReqFoodClinicDeptID).ToObservableCollection();
                        }
                    }
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveRequestFoodClinicDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<ReqFoodClinicDeptDetail> GetReqFoodClinicDeptDetailsByID(long ReqFoodClinicDeptID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetReqFoodClinicDeptDetailByID(ReqFoodClinicDeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetReqOutwardDrugClinicDeptPatientByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #004
        #endregion

        //▼==== #006
        public List<List<string>> ExportExcelOutwardClinicDeptTemplates(long V_MedProductType, long V_OutwardTemplateType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ExportExcelOutwardClinicDeptTemplates(V_MedProductType, V_OutwardTemplateType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ExportExcelOutwardClinicDeptTemplates. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetAllOutwardTemplate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #006

        //▼====: #007
        public RequestDrugForTechnicalService GetRequestDrugForTechnicalServicePtRegDetailID(long PtRegDetailID, long V_RegistrationType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRequestDrugForTechnicalServicePtRegDetailID(PtRegDetailID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRequestDrugInwardClinicDeptByPtRegDetailID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetInwardDrugClinicDeptInvoice_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SaveRequestDrugForTechnicalService(RequestDrugForTechnicalService Request, long V_MedProductType, long V_RegistrationType, out RequestDrugForTechnicalService OutRequest)
        {
            try
            {
                OutRequest = new RequestDrugForTechnicalService();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.SaveRequestDrugForTechnicalService(Request, V_MedProductType, V_RegistrationType, out id);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugForTechnicalServicePtRegDetailID(Request.PtRegDetailID);
                    if (OutRequest != null)
                    {
                        OutRequest.ReqOutwardDetails = RefDrugGenericDetailsProvider.Instance.GetReqOutwardDrugClinicDeptPatientByReqService(id, V_RegistrationType).ToObservableCollection();
                    }
                }
                else
                {
                    if (Request.ReqForTechID > 0)
                    {
                        OutRequest = RefDrugGenericDetailsProvider.Instance.GetRequestDrugForTechnicalServicePtRegDetailID(Request.PtRegDetailID);
                        if (OutRequest != null)
                        {
                            OutRequest.ReqOutwardDetails = RefDrugGenericDetailsProvider.Instance.GetReqOutwardDrugClinicDeptPatientByReqService(Request.ReqForTechID, V_RegistrationType).ToObservableCollection();
                        }
                    }
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveRequestDrugForTechnicalService. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<ReqOutwardDrugClinicDeptPatient> GetReqOutwardDrugClinicDeptPatientByReqService(long ReqForTechID, long V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetReqOutwardDrugClinicDeptPatientByReqService(ReqForTechID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetReqOutwardDrugClinicDeptPatientByReqService. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_GetRequestDrugInwardClinicDeptDetailByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteRequestDrugForTechnicalService(long ReqForTechID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteRequestDrugForTechnicalService(ReqForTechID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteRequestDrugForTechnicalService. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_DeleteRequestDrugInwardClinicDept);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #007
    }
}