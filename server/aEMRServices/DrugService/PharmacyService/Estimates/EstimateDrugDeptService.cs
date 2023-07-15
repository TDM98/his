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
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;

namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class EstimateDrugDeptService : eHCMS.WCFServiceCustomHeader, IEstimateDrugDeptService
    {
        public EstimateDrugDeptService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        #region 0. Estimation For Drug Dept Member
        public IList<DrugDeptEstimationForPoDetail> GetEstimationForMonth(long V_MedProductType, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetEstimationForMonth(V_MedProductType, CurrentDrugDeptEstimationForPO);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetEstimationForMonth. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_GetEstimationForMonth);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<DrugDeptEstimationForPoDetail> GetEstimationForMonthByBid(long V_MedProductType, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetEstimationForMonth(V_MedProductType, CurrentDrugDeptEstimationForPO, true);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetEstimationForMonth. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_GetEstimationForMonth);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public DrugDeptEstimationForPoDetail DrugDeptEstimationForPoDetail_GenMedProductID(long GenMedProductID, string Code, DateTime EstimateDate, long? V_EstimateType, long V_MedProductType, long? RefGenDrugCatID_1)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPoDetail_GenMedProductID(GenMedProductID, Code, EstimateDate, V_EstimateType, V_MedProductType, RefGenDrugCatID_1);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPoDetail_GenMedProductID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_GetEstimationForMonth);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public long DrugDeptEstimationForPO_FullOperator(long V_MedProductType, DrugDeptEstimationForPO Estimate, out DrugDeptEstimationForPO EstimateOut)
        {
            try
            {
                EstimateOut = new DrugDeptEstimationForPO();
                long id = 0;
                id = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_FullOperator(V_MedProductType, Estimate);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    EstimateOut = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_ByID(id);
                    EstimateOut.EstimationDetails = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPoDetail_ByParentID(id).ToObservableCollection();
                }
                else
                {
                    if (Estimate.DrugDeptEstimatePoID > 0)
                    {
                        EstimateOut = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_ByID(Estimate.DrugDeptEstimatePoID);
                        EstimateOut.EstimationDetails = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPoDetail_ByParentID(Estimate.DrugDeptEstimatePoID).ToObservableCollection();
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPoDetail_ByParentID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPoDetail_ByParentID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public long DrugDeptEstimationForPO_FullOperator_ByBid(long V_MedProductType, DrugDeptEstimationForPO Estimate, out DrugDeptEstimationForPO EstimateOut)
        {
            try
            {
                EstimateOut = new DrugDeptEstimationForPO();
                long id = 0;
                id = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_FullOperator(V_MedProductType, Estimate, true);
                bool BOK;
                BOK = (id > 0);
                if (BOK)
                {
                    EstimateOut = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_ByID(id);
                    EstimateOut.EstimationDetails = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPoDetail_ByParentID(id, true).ToObservableCollection();
                }
                else
                {
                    if (Estimate.DrugDeptEstimatePoID > 0)
                    {
                        EstimateOut = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_ByID(Estimate.DrugDeptEstimatePoID);
                        EstimateOut.EstimationDetails = RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPoDetail_ByParentID(Estimate.DrugDeptEstimatePoID, true).ToObservableCollection();
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPoDetail_ByParentID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPoDetail_ByParentID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public DrugDeptEstimationForPO DrugDeptEstimationForPO_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPO_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPO_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DrugDeptEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_CheckExists(V_EstimateType, DateOfEstimation, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPO_CheckExists. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPO_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptEstimationForPoDetail> DrugDeptEstimationForPoDetail_ByParentID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPoDetail_ByParentID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPoDetail_ByParentID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPoDetail_ByParentID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptEstimationForPoDetail> DrugDeptEstimationForPoDetail_ByParentIDAndByBid(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPoDetail_ByParentID(ID, true);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPoDetail_ByParentID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPoDetail_ByParentID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<DrugDeptEstimationForPO> DrugDeptEstimationForPO_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_Search(Criteria, V_MedProductType, pageIndex, pageSize, bCount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPO_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPO_Search);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptEstimationForPO> DrugDeptEstimationForPO_SearchByBid(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_Search(Criteria, V_MedProductType, pageIndex, pageSize, bCount, out totalcount, true);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPO_Search. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPO_Search);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<DrugDeptEstimationForPO> DrugDeptEstimationForPO_Load(long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_Load(V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPO_Load. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPO_Load);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<DrugDeptEstimationForPoDetail> DrugDeptEstimationForPoDetail_AllDrugAuto(long V_MedProductType, DateTime EstimationDate, long? V_EstimateType, long? RefGenDrugCatID_1, string BrandName, int pageIndex, int pageSize, out int totalcount, bool? IsCode)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPoDetail_AllDrugAuto(V_MedProductType, EstimationDate, V_EstimateType, RefGenDrugCatID_1, BrandName, pageIndex, pageSize, out totalcount, IsCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPoDetail_AllDrugAuto. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPoDetail_AllDrugAuto);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DrugDeptEstimationForPO_Delete(long DrugDeptEstimatePoID, bool IsFromRequest)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPO_Delete(DrugDeptEstimatePoID, IsFromRequest);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPO_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptEstimationForPoDetail_AllDrugAuto);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion

        #region 1. Estimation For Pharmacy Member

        public IList<PharmacyEstimationForPODetail> GetEstimationForMonthPharmacy(long V_MedProductType, DateTime EstimateDate, long? V_EstimateType, bool IsHIStorage)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetEstimationForMonthPharmacy(V_MedProductType, EstimateDate, V_EstimateType, IsHIStorage);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetEstimationForMonthPharmacy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_GetEstimationForMonthPharmacy);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public long PharmacyEstimationForPO_FullOperator(long V_MedProductType, PharmacyEstimationForPO Estimate, bool IsHIStorage, out PharmacyEstimationForPO EstimateOut)
        {
            try
            {
                EstimateOut = new PharmacyEstimationForPO();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPO_FullOperator(V_MedProductType, Estimate, IsHIStorage, out id);
                if (id > 0)
                {
                    EstimateOut = RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPO_ByID(id);
                    EstimateOut.EstimationDetails = RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPODetail_ByParentID(id).ToObservableCollection();
                }
                else if (id == 0)
                {
                    if (Estimate.PharmacyEstimatePoID > 0)
                    {
                        EstimateOut = RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPO_ByID(Estimate.PharmacyEstimatePoID);
                        EstimateOut.EstimationDetails = RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPODetail_ByParentID(Estimate.PharmacyEstimatePoID).ToObservableCollection();
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyEstimationForPODetail_ByParentID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyEstimationForPODetail_ByParentID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacyEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation, bool IsHIStorage)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPO_CheckExists(V_EstimateType, DateOfEstimation, IsHIStorage);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyEstimationForPO_CheckExists. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyEstimationForPO_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PharmacyEstimationForPO PharmacyEstimationForPO_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPO_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyEstimationForPO_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyEstimationForPO_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacyEstimationForPODetail> PharmacyEstimationForPODetail_ByParentID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPODetail_ByParentID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyEstimationForPODetail_ByParentID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyEstimationForPODetail_ByParentID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacyEstimationForPO> PharmacyEstimationForPO_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, bool IsHIStorage, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPO_Search(Criteria, V_MedProductType, pageIndex, pageSize, bCount, IsHIStorage, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyEstimationForPO_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyEstimationForPO_Search);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PharmacyEstimationForPO> PharmacyEstimationForPO_Load(long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPO_Load(V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyEstimationForPO_Load. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyEstimationForPO_Load);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PharmacyEstimationForPODetail PharmacyEstimationForPODetail_ByDrugID(DateTime EstimationDate, long DrugID, string DrugCode, long? V_EstimateType, bool IsHIStorage)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPODetail_ByDrugID(EstimationDate, DrugID, DrugCode, V_EstimateType, IsHIStorage);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyEstimationForPODetail_ByDrugID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyEstimationForPO_Load);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacyEstimationForPODetail> PharmacyEstimationForPODetail_AllDrugAuto(long V_MedProductType, DateTime EstimationDate, long? V_EstimateType, string BrandName, int pageIndex, int pageSize, out int totalcount, bool? IsCode)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPODetail_AllDrugAuto(V_MedProductType, EstimationDate, V_EstimateType, BrandName, pageIndex, pageSize, out totalcount, IsCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyEstimationForPODetail_AllDrugAuto. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyEstimationForPODetail_AllDrugAuto);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacyEstimationForPO_Delete(long PharmacyEstimatePoID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyEstimationForPO_Delete(PharmacyEstimatePoID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyEstimationForPO_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyEstimationForPO_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 2. Purchase Order For Drug

        public List<PharmacyPurchaseOrderDetail> PharmacyPurchaseOrderDetail_GetFirst(long? PharmacyEstimatePoID, long? SupplierID, long? PCOID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrderDetail_GetFirst(PharmacyEstimatePoID, SupplierID, PCOID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyPurchaseOrderDetail_GetFirst. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyPurchaseOrderDetail_GetFirst);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacyPurchaseOrderDetail_UpdateNoWaiting(long PharmacyPoDetailID, bool? NoWaiting)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrderDetail_UpdateNoWaiting(PharmacyPoDetailID, NoWaiting);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyPurchaseOrderDetail_UpdateNoWaiting. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyPurchaseOrderDetail_UpdateNoWaiting);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public long PharmacyPurchaseOrder_FullOperator(PharmacyPurchaseOrder PurchaseOrder, byte IsInput, out PharmacyPurchaseOrder OutPurchaseOrder)
        {
            try
            {
                OutPurchaseOrder = new PharmacyPurchaseOrder();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrder_FullOperator(PurchaseOrder, out id);
                if (id > 0)
                {
                    OutPurchaseOrder = RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrder_ByID(id);
                    OutPurchaseOrder.PurchaseOrderDetails = RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrderDetail_ByParentID(id, IsInput).ToObservableCollection();
                }
                else if (id == 0)
                {
                    if (PurchaseOrder.PharmacyPoID > 0)
                    {
                        OutPurchaseOrder = RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrder_ByID(PurchaseOrder.PharmacyPoID);
                        OutPurchaseOrder.PurchaseOrderDetails = RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrderDetail_ByParentID(PurchaseOrder.PharmacyPoID, IsInput).ToObservableCollection();
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyPurchaseOrderDetail_ByParentID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyPurchaseOrderDetail_ByParentID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PharmacyPurchaseOrder PharmacyPurchaseOrder_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrder_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyPurchaseOrder_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyPurchaseOrder_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacyPurchaseOrder_UpdateStatus(long ID, long V_PurchaseOrderStatus)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrder_UpdateStatus(ID, V_PurchaseOrderStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyPurchaseOrder_UpdateStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyPurchaseOrder_UpdateStatus);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacyPurchaseOrderDetail> PharmacyPurchaseOrderDetail_ByParentID(long ID, byte IsInput)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrderDetail_ByParentID(ID, IsInput);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyPurchaseOrderDetail_ByParentID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyPurchaseOrderDetail_ByParentID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacyPurchaseOrder> PharmacyPurchaseOrder_Search(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrder_Search(Criteria, pageIndex, pageSize, bCount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyPurchaseOrder_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyPurchaseOrder_Search);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacyPurchaseOrder> PharmacyPurchaseOrder_BySupplierID(long SupplierID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrder_BySupplierID(SupplierID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyPurchaseOrder_BySupplierID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyPurchaseOrder_BySupplierID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacyPurchaseOrders_Delete(long PharmacyPoID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyPurchaseOrders_Delete(PharmacyPoID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyPurchaseOrders_Delete. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_PharmacyPurchaseOrders_Delete);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefGenericDrugDetail> RefGenericDrugDetail_AutoRequest(string BrandName, long? PharmacyEstimatePoID, long? SupplierID, int PageIndex, int PageSize, out int totalcount, bool? IsCode)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenericDrugDetail_AutoRequest(BrandName, PharmacyEstimatePoID, SupplierID, PageIndex, PageSize, out totalcount, IsCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefGenericDrugDetail_AutoRequest. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_RefGenericDrugDetail_AutoRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<RefGenericDrugDetail> RefGenericDrugDetail_WarningOrder(int PageIndex, int PageSize, long? SupplierID, bool IsAll, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenericDrugDetail_WarningOrder(PageIndex, PageSize, SupplierID, IsAll, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefGenericDrugDetail_WarningOrder. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_RefGenericDrugDetail_WarningOrder);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 3.DrugDept Purchase Member

        public List<DrugDeptPurchaseOrder> DrugDeptPurchaseOrder_BySupplierID(long SupplierID, long V_MedProductType, long BidID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrder_BySupplierID(SupplierID, V_MedProductType, BidID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrder_BySupplierID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptPurchaseOrder_BySupplierID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptPurchaseOrderDetail> DrugDeptPurchaseOrderDetail_ByParentID(long ID, byte IsInput, long? BidID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrderDetail_ByParentID(ID, IsInput, BidID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrderDetail_ByParentID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptPurchaseOrderDetail_ByParentID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptPurchaseOrder> DrugDeptPurchaseOrder_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrder_Search(Criteria, V_MedProductType, pageIndex, pageSize, bCount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrder_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptPurchaseOrder_Search);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptPurchaseOrderDetail> DrugDeptPurchaseOrderDetail_GetFirst(long? DrugDeptEstimatePoID, long? SupplierID, long? PCOID, long V_MedProductType, long? BidID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrderDetail_GetFirst(DrugDeptEstimatePoID, SupplierID, PCOID, V_MedProductType, BidID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrderDetail_GetFirst. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptPurchaseOrderDetail_GetFirst);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public long DrugDeptPurchaseOrder_FullOperator(DrugDeptPurchaseOrder PurchaseOrder, byte IsInput, long? BidID, out DrugDeptPurchaseOrder OutPurchaseOrder)
        {
            try
            {
                OutPurchaseOrder = new DrugDeptPurchaseOrder();
                long id = 0;
                RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrder_FullOperator(PurchaseOrder, out id);
                if (id > 0)
                {
                    OutPurchaseOrder = RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrder_ByID(id);
                    OutPurchaseOrder.PurchaseOrderDetails = RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrderDetail_ByParentID(id, IsInput, BidID).ToObservableCollection();
                }
                else if (id == 0)
                {
                    if (PurchaseOrder.DrugDeptPoID > 0)
                    {
                        OutPurchaseOrder = RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrder_ByID(PurchaseOrder.DrugDeptPoID);
                        OutPurchaseOrder.PurchaseOrderDetails = RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrderDetail_ByParentID(PurchaseOrder.DrugDeptPoID, IsInput, BidID).ToObservableCollection();
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrderDetail_ByParentID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptPurchaseOrderDetail_ByParentID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public DrugDeptPurchaseOrder DrugDeptPurchaseOrder_ByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrder_ByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrder_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptPurchaseOrder_ByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefGenMedProductDetails> RefGenMedProductDetails_AutoPurchaseOrder(string BrandName, long? DrugDeptEstimatePoID, long V_MedProductType, long? SupplierID, int PageIndex, int PageSize, out int totalcount, bool? IsCode, long? BidID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_AutoPurchaseOrder(BrandName, DrugDeptEstimatePoID, V_MedProductType, SupplierID, PageIndex, PageSize, out totalcount, IsCode, BidID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefGenMedProductDetails_AutoPurchaseOrder. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_RefGenMedProductDetails_AutoPurchaseOrder);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<RefGenMedProductDetails> RefGenMedProductDetails_WarningOrder(long V_MedProductType, int PageIndex, int PageSize, long? SupplierID, bool IsAll, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_WarningOrder(V_MedProductType, PageIndex, PageSize, SupplierID, IsAll, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefGenMedProductDetails_WarningOrder. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_RefGenMedProductDetails_WarningOrder);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DrugDeptPurchaseOrders_Delete(long DrugDeptPoID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrders_Delete(DrugDeptPoID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrders_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptPurchaseOrders_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DrugDeptPurchaseOrderDetail_UpdateNoWaiting(long DrugDeptPoDetailID, bool? NoWaiting)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrderDetail_UpdateNoWaiting(DrugDeptPoDetailID, NoWaiting);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrderDetail_UpdateNoWaiting. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptPurchaseOrderDetail_UpdateNoWaiting);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DrugDeptPurchaseOrders_UpdateStatus(long ID, long V_PurchaseOrderStatus)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPurchaseOrders_UpdateStatus(ID, V_PurchaseOrderStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptPurchaseOrders_UpdateStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_DrugDeptPurchaseOrders_UpdateStatus);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        //Chức năng dự trù 20210830
        public IList<DrugDeptEstimationForPoDetail> GetEstimation_V2(long V_MedProductType, DateTime FromDate, DateTime ToDate, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO, bool IsByBid = false, bool IsFromRequest = false)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetEstimation_V2(V_MedProductType, FromDate, ToDate, CurrentDrugDeptEstimationForPO, IsByBid, IsFromRequest);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetEstimation_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_GetEstimationForMonth);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public DrugDeptEstimationForPoDetail DrugDeptEstimationForPoDetail_GenMedProductID_V2(long GenMedProductID, string Code, DateTime FromDate, DateTime ToDate, long V_MedProductType, long? RefGenDrugCatID_1)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptEstimationForPoDetail_GenMedProductID_V2(GenMedProductID, Code, FromDate, ToDate, V_MedProductType, RefGenDrugCatID_1);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptEstimationForPoDetail_GenMedProductID_V2. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ESTIMATE_DRUG_GetEstimationForMonth);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
    }
}
