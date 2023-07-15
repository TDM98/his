using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
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
    public class StorageService : eHCMS.WCFServiceCustomHeader, IStorages
    {
        // private string _ModuleName = "Storage Service";

        public StorageService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public RefStorageWarehouseLocation GetStorageByID(long storageID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetStorageByID(storageID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetStorageByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int DeleteStorageByID(long storageID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteStorageByID(storageID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteStorageByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int UpdateStorage(RefStorageWarehouseLocation storage)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateStorage(storage);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateStorage. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_UPDATE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int AddNewStorage(RefStorageWarehouseLocation storage)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddNewStorage(storage);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddNewStorage. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_ADD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefStorageWarehouseLocation> GetAllStorages(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllStorages(pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllStorages. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<RefStorageWarehouseLocation> GetAllStoragesNotPaging(long? type, bool? bNo, long? DeptID, bool? IsNotSubStorage)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllStorages(type, bNo, DeptID, IsNotSubStorage);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllStoragesNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefStorageWarehouseLocation> GetAllStorages_ForRespon(List<long> allListStoreID, long? type,
                                                                           bool? bNo)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllStorages_ForRespon(allListStoreID, type, bNo);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllStorages_ForRespon. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefStorageWarehouseLocation> SearchStorage(string StorageName, long V_MedProductType, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchStorage(StorageName, V_MedProductType, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchStorage. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<GetDrugForSellVisitor> GetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex, int IsMedDept)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugForPrescription_Auto(BrandName, IsInsurance, PageSize, PageIndex, IsMedDept);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForPrescription_Auto. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<RefStorageWarehouseLocation> GetStoragesByStaffID(long StaffID, long? StoreTypeID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetStoragesByStaffID(StaffID, StoreTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetStoragesByStaffID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefStorageWarehouseType> GetRefStorageWarehouseType_All()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefStorageWarehouseType_All();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefStorageWarehouseType_All. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<RefDepartment> GetRefDepartment_ByDeptType(long DeptType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefDepartment_ByDeptType(DeptType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefDepartment_ByDeptType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_STORAGE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
    }
}
