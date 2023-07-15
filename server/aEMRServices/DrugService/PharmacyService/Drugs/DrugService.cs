using System;
using System.Collections.Generic;
using System.Data.Services;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
using eHCMS.Configurations;
using ErrorLibrary;
using System.Runtime.Serialization;
using AxLogging;
using ErrorLibrary.Resources;
using System.Linq;
using System.IO;
using System.Text;
using Service.Core.Common;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;
using System.Collections.ObjectModel;
/*
 * 20190620 #001 TTM:   BM 0011857: Lấy dữ liệu cho màn hình Thông tin thuốc
 * 20200218 #002 TNHX: [] Thêm điều kiện lọc danh mục NT + Kho BHYT Ngoại trú
 */
namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class DrugService : eHCMS.WCFServiceCustomHeader, IDrugs
    {
        //private string _ModuleName = "Drug Service";
        public DrugService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        #region IDrugService Members

        [SingleResult]
        public RefGenericDrugDetail GetDrugByID(long drugID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefDrugGenericDetailsByID(drugID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefDrugGenericDetailsByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteDrug(RefGenericDrugDetail drug)
        {
            return true;
        }

        public bool DeleteDrugByID(long drugID) //Nếu thuốc chưa sử dụng
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteRefDrugGenericDetailByID(drugID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteRefDrugGenericDetailByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▼===== 25072018 TTM
        public bool DeleteDrugByID_New(long drugID) //Nếu thuốc chưa sử dụng
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteRefDrugGenericDetailByID_New(drugID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteRefDrugGenericDetailByID_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== 25072018 TTM

        public bool UpdateActiveDrugByID(long drugID) //tuong ung voi delete nếu thuốc đó đã được sử dụng rồi
        {
            throw new Exception(eHCMSResources.Z1842_G1_NotImplementedOperation);
        }
        public IList<RefGenericDrugDetail> GetAllDrugs()
        {
            throw new Exception(eHCMSResources.Z1842_G1_NotImplementedOperation);
        }


        #endregion

        #region IDrugService Members
        public IList<RefGenericDrugDetail> GetDrugsJoin(int IsMedDept, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefDrugGenericDetails(IsMedDept, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefDrugGenericDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_NOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string brandName, long? SupplierID, int pageIndex, int pageSize, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRefDrugGenericDetails_AutoPaging(IsCode, brandName, SupplierID, pageIndex, pageSize, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRefDrugGenericDetails_AutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_NOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▼====: #002
        public IList<RefGenericDrugSimple> SearchRefGenericDrugName_SimpleAutoPaging(bool? IsCode, string BrandName, int pageIndex, int pageSize, bool IsHIStore, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRefGenericDrugName_SimpleAutoPaging(IsCode, BrandName, pageIndex, pageSize, IsHIStore, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRefGenericDrugName_SimpleAutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_NOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #002

        public IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_RefAutoPaging(bool? IsCode, string BrandName, int pageIndex, int pageSize, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRefDrugGenericDetails_RefAutoPaging(IsCode, BrandName, pageIndex, pageSize, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRefDrugGenericDetails_RefAutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_NOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenericDrugDetail> SearchDrugs(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRefDrugGenericDetails(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRefDrugGenericDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_NOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_Simple(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRefDrugGenericDetails_Simple(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRefDrugGenericDetails_RefAutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_NOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼===== 25072018 TTM
        public IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_Simple_New(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRefDrugGenericDetails_Simple_New(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRefDrugGenericDetails_RefAutoPaging_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_NOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== 25072018 TTM

        public IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_ItemPrice(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchRefDrugGenericDetails_ItemPrice(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRefDrugGenericDetails_RefAutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_NOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool UpdateDrug(RefGenericDrugDetail DrugRecord)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateRefDrugGenericDetail(DrugRecord);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateRefDrugGenericDetail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼===== 25072018 TTM
        public bool UpdateDrug_New(RefGenericDrugDetail DrugRecord)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateRefDrugGenericDetail_New(DrugRecord);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateRefDrugGenericDetail_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== 25072018 TTM

        public bool AddNewDrug(RefGenericDrugDetail newDrug, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType, out long DrugID)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.InsertRefDrugGenericDetail(newDrug, out DrugID);
                if (DrugID > 0 && lstRefMedicalConditionType.Count > 0)
                {
                    //InsertConIndicatorDrugsRelToMedCond(lstRefMedicalConditionType, DrugID);
                }
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertRefDrugGenericDetail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▼===== 25072018 TTM
        public bool AddNewDrug_New(RefGenericDrugDetail newDrug, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType, out long DrugID)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.InsertRefDrugGenericDetail_New(newDrug, out DrugID);
                if (DrugID > 0 && lstRefMedicalConditionType.Count > 0)
                {
                    InsertConIndicatorDrugsRelToMedCond_New(lstRefMedicalConditionType, DrugID);
                }
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertRefDrugGenericDetail_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== 25072018 TTM
        #endregion

        #region DrugClass member
        public bool AddNewFamilyTherapy(DrugClass newfamily, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddNewFamilyTherapy(newfamily, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewFamilyTherapy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_AddNewFamilyTherapy);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateFamilyTherapy(DrugClass updatefamily, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateFamilyTherapy(updatefamily, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateFamilyTherapy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteFamilyTherapy(long deletefamily, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteFamilyTherapy(deletefamily, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteFamilyTherapy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public DrugClass GetFamilyTherapyByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetFamilyTherapyByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapyByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public DrugClass GetFamilyTherapyByID(Nullable<long> ID)
        {
            try
            {
                long value = 0;
                if (ID != null)
                {
                    value = (long)ID;
                }
                return RefDrugGenericDetailsProvider.Instance.GetFamilyTherapyByID(value);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapyByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DrugClass> GetFamilyTherapies(long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetFamilyTherapies(V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<DrugClass> GetFamilyTherapyParent(long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetFamilyTherapyParent(V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapyParent. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<DrugClass> GetSearchFamilyTherapies(string faname, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSearchFamilyTherapies(faname, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetSearchFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<TherapyTree> list = new List<TherapyTree>();
        public List<TherapyTree> GetTreeView(long V_MedProductType)
        {
            try
            {
                list.Clear();
                IList<DrugClass> familytherapies = RefDrugGenericDetailsProvider.Instance.GetFamilyTherapies(V_MedProductType);
                List<TherapyTree> results = new List<TherapyTree>();
                foreach (DrugClass item in familytherapies)
                {
                    TherapyTree genericItem = new TherapyTree(item.FaName, item.DrugClassID, item.ParDrugClassID, item.FaDescription, item.DrugClassCode, GetFamilyTherapyByID(item.ParDrugClassID));
                    this.list.Add(genericItem);
                }

                var rootNodes = this.list.Where(x => x.ParentID == x.NodeID);

                foreach (TherapyTree node in rootNodes)
                {
                    this.FindChildren(node);
                    results.Add(node);
                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        private void FindChildren(TherapyTree item)
        {
            try
            {
                // find all the children of the item
                var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);

                // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
                foreach (TherapyTree child in children)
                {
                    item.Children.Add(child);
                    FindChildren(child);
                }
            }
            catch
            { }
        }
        public List<TherapyTree> GetTreeViewFamilyTherapyParent(long V_MedProductType)
        {
            try
            {
                list.Clear();
                IList<DrugClass> familytherapies = GetFamilyTherapies(V_MedProductType);
                List<TherapyTree> results = new List<TherapyTree>();
                foreach (DrugClass item in familytherapies)
                {
                    TherapyTree genericItem = new TherapyTree(item.FaName, item.DrugClassID, item.ParDrugClassID, item.FaDescription, item.DrugClassCode, GetFamilyTherapyByID(item.ParDrugClassID));
                    this.list.Add(genericItem);
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                var rootNodes = this.list.Where(x => x.ParentID == x.NodeID);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works
                foreach (TherapyTree node in rootNodes)
                {
                    results.Add(node);
                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<TherapyTree> GetSearchTreeView(string faname, long V_MedProductType)
        {
            try
            {
                list.Clear();
                IList<DrugClass> familytherapies = GetSearchFamilyTherapies(faname, V_MedProductType);
                List<TherapyTree> results = new List<TherapyTree>();
                foreach (DrugClass item in familytherapies)
                {
                    TherapyTree genericItem = new TherapyTree(item.FaName, item.DrugClassID, item.ParDrugClassID, item.FaDescription, item.DrugClassCode, GetFamilyTherapyByID(item.ParDrugClassID));
                    this.list.Add(genericItem);
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                var rootNodes = this.list.Where(x => x.ParentID == x.NodeID);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works

                foreach (TherapyTree node in rootNodes)
                {
                    this.FindChildren(node);
                    results.Add(node);
                }
                return results;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region DrugDeptClass member
        public bool AddNewDrugDeptClasses(DrugClass newfamily, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddNewDrugDeptClasses(newfamily, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewFamilyTherapy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_AddNewFamilyTherapy);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateDrugDeptClasses(DrugClass updatefamily, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateDrugDeptClasses(updatefamily, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateFamilyTherapy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteDrugDeptClasses(DrugClass deletefamily, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteDrugDeptClasses(deletefamily, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteFamilyTherapy. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public DrugClass GetDrugDeptClassesByID(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugDeptClassesByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapyByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public DrugClass GetDrugDeptClassesByID(Nullable<long> ID)
        {
            try
            {
                long value = 0;
                if (ID != null)
                {
                    value = (long)ID;
                }
                return RefDrugGenericDetailsProvider.Instance.GetDrugDeptClassesByID(value);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapyByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DrugClass> GetDrugDeptClasses(long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugDeptClasses(V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DrugClass> GetDrugDeptClassesParent(long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugDeptClassesParent(V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapyParent. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DrugClass> GetSearchDrugDeptClasses(string faname, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSearchDrugDeptClasses(faname, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetSearchFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<TherapyTree> GetTreeView_DrugDept(long V_MedProductType)
        {
            try
            {
                list.Clear();
                IList<DrugClass> familytherapies = RefDrugGenericDetailsProvider.Instance.GetDrugDeptClasses(V_MedProductType);
                List<TherapyTree> results = new List<TherapyTree>();
                foreach (DrugClass item in familytherapies)
                {
                    TherapyTree genericItem = new TherapyTree(item.FaName, item.DrugClassID, item.ParDrugClassID, item.FaDescription, item.DrugClassCode, GetDrugDeptClassesByID(item.ParDrugClassID));
                    this.list.Add(genericItem);
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                var rootNodes = this.list.Where(x => x.ParentID == x.NodeID);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works
                foreach (TherapyTree node in rootNodes)
                {
                    this.FindChildren(node);
                    results.Add(node);
                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<TherapyTree> GetTreeViewDrugDeptClassesParent(long V_MedProductType)
        {
            try
            {
                list.Clear();
                IList<DrugClass> familytherapies = GetDrugDeptClasses(V_MedProductType);
                List<TherapyTree> results = new List<TherapyTree>();
                foreach (DrugClass item in familytherapies)
                {
                    TherapyTree genericItem = new TherapyTree(item.FaName, item.DrugClassID, item.ParDrugClassID, item.FaDescription, item.DrugClassCode, GetDrugDeptClassesByID(item.ParDrugClassID));
                    this.list.Add(genericItem);
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                var rootNodes = this.list.Where(x => x.ParentID == x.NodeID);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works
                foreach (TherapyTree node in rootNodes)
                {
                    results.Add(node);
                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<TherapyTree> GetSearchTreeView_DrugDept(string faname, long V_MedProductType)
        {
            try
            {
                list.Clear();
                IList<DrugClass> familytherapies = GetSearchDrugDeptClasses(faname, V_MedProductType);
                List<TherapyTree> results = new List<TherapyTree>();
                foreach (DrugClass item in familytherapies)
                {
                    TherapyTree genericItem = new TherapyTree(item.FaName, item.DrugClassID, item.ParDrugClassID, item.FaDescription, item.DrugClassCode, familytherapies.FirstOrDefault(x => x.DrugClassID == item.ParDrugClassID));
                    this.list.Add(genericItem);
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                var rootNodes = this.list.Where(x => x.ParentID == x.NodeID);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works

                foreach (TherapyTree node in rootNodes)
                {
                    this.FindChildren(node);
                    results.Add(node);
                }
                return results;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapies. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DrugClass> GetAllRefGeneric()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllRefGeneric();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRefGeneric. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<RefGeneric> GetParRefGeneric()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetParRefGeneric();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetParRefGeneric. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InsertRefGenericRelation(TherapyTree CurrentTherapyTree, ObservableCollection<TherapyTree> ObsRefGenericRelation)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InsertRefGenericRelation(CurrentTherapyTree, ObsRefGenericRelation);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertRefGenericRelation. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "InsertRefGenericRelation Failed");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<TherapyTree> GetRefGenericRelation_ForGenericID(long GenericID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefGenericRelation_ForGenericID(GenericID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefGenericRelation_ForGenericID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "GetRefGenericRelation_ForGenericID Failed");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        public IList<PharmaceuticalCompany> GetPharmaceuticalCompanyCbx()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetPharmaceuticalCompanyCbx();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPharmaceuticalCompanyCbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_PHARMACEUTICAL_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #region Pharmacy Member

        public IList<RefGenMedProductDetails> GetRefGenMedProductDetails_Auto(bool? IsCode, string BrandName, long V_MedProductType, int PageSize, int PageIndex, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefGenMedProductDetails_Auto(IsCode, BrandName, V_MedProductType, PageSize, PageIndex, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefGenMedProductDetails_Auto. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetRefGenMedProductDetails_Auto);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetAllRefGenMedProductDetail(long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllRefGenMedProductDetail(V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRefGenMedProductDetail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetRefGenMedProductDetails_Auto);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region
        public List<RefMedContraIndicationTypes> GetRefMedicalConditionTypesAllPaging(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefMedicalConditionTypesAllPaging(PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                Total = 0;
                AxLogger.Instance.LogInfo("End of GetRefMedicalConditionTypesAllPaging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetRefMedicalConditionTypesAllPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool GetConIndicatorDrugsRelToMedCondAll(int MCTypeID, long DrugID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetConIndicatorDrugsRelToMedCondAll(MCTypeID, DrugID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetConIndicatorDrugsRelToMedCondAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetConIndicatorDrugsRelToMedCondAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //public bool InsertConIndicatorDrugsRelToMedCond(IList<RefMedicalConditionType> lstRefMedicalCondition, long DrugID)
        //{
        //    try
        //    {                
        //        RefDrugGenericDetailsProvider.Instance.InsertConIndicatorDrugsRelToMedCond(lstRefMedicalCondition, DrugID);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of InsertConIndicatorDrugsRelToMedCond. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_InsertConIndicatorDrugsRelToMedCond);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        //▼===== 25072018 TTM
        public bool InsertConIndicatorDrugsRelToMedCond_New(IList<RefMedContraIndicationTypes> lstRefMedicalCondition, long DrugID)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.InsertConIndicatorDrugsRelToMedCond_New(lstRefMedicalCondition, DrugID);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertConIndicatorDrugsRelToMedCond_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_InsertConIndicatorDrugsRelToMedCond);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== 25072018 TTM

        public bool InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(lstInsert
                                                    , lstDelete
                                                    , lstUpdate);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertConIndicatorDrugsRelToMedCond. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_InsertConIndicatorDrugsRelToMedCond);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼===== 25072018 TTM
        public bool InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML_New(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML_New(lstInsert
                                                     , lstDelete
                                                     , lstUpdate);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertConIndicatorDrugsRelToMedCond_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_InsertConIndicatorDrugsRelToMedCond);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== 25072018 TTM

        //public bool InsertConIndicatorDrugsRelToMedCondEx(IList<RefGenericDrugDetail> lstRefGenericDrugDetail, long MCTypeID)
        //{
        //    try
        //    {                
        //        RefDrugGenericDetailsProvider.Instance.InsertConIndicatorDrugsRelToMedCondEx(lstRefGenericDrugDetail, MCTypeID);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of InsertConIndicatorDrugsRelToMedCondEx. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_InsertConIndicatorDrugsRelToMedCondEx);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        public IList<ContraIndicatorDrugsRelToMedCond> GetConIndicatorDrugsRelToMedCond(IList<long> lstMCTpe, long DrugID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetConIndicatorDrugsRelToMedCond(lstMCTpe, DrugID);

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetConIndicatorDrugsRelToMedCond. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetConIndicatorDrugsRelToMedCond);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<DrugAndConTra> GetAllDrugsContrainIndicator()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllDrugsContrainIndicator();

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllDrugsContrainIndicator. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetAllContrainIndicatorDrugs);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<ContraIndicatorDrugsRelToMedCond> GetAllContrainIndicatorDrugs()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllContrainIndicatorDrugs();

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllContrainIndicatorDrugs. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetAllContrainIndicatorDrugs);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<ContraIndicatorDrugsRelToMedCond> GetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetContraIndicatorDrugsRelToMedCondList(MCTypeID, DrugID);

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetContraIndicatorDrugsRelToMedCondList. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetContraIndicatorDrugsRelToMedCondList);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Dictionary<long, List<RefGenericRelation>> GetAllRefGenericRelation()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllRefGenericRelation();

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRefGenericRelation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "GetAllRefGenericRelation Failed");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼===== 25072018 TTM
        public List<ContraIndicatorDrugsRelToMedCond> GetContraIndicatorDrugsRelToMedCondList_New(int MCTypeID, long DrugID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetContraIndicatorDrugsRelToMedCondList_New(MCTypeID, DrugID);

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetContraIndicatorDrugsRelToMedCondList_New. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetContraIndicatorDrugsRelToMedCondList);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== 25072018 TTM

        public List<RefMedContraIndicationTypes> GetRefMedCondType()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefMedCondType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefMedCondType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetRefMedCondType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<ContraIndicatorDrugsRelToMedCond> GetContraIndicatorDrugsRelToMedCondPaging(
                                                long MCTypeID
                                                , int PageSize
                                                , int PageIndex
                                                , string OrderBy
                                                , bool CountTotal
                                                , out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetContraIndicatorDrugsRelToMedCondPaging(MCTypeID
                                                , PageSize
                                                , PageIndex
                                                , OrderBy
                                                , CountTotal
                                                , out Total);

            }
            catch (Exception ex)
            {
                Total = 0;
                AxLogger.Instance.LogInfo("End of GetContraIndicatorDrugsRelToMedCondPaging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetContraIndicatorDrugsRelToMedCondPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteConIndicatorDrugsRelToMedCond(long DrugsMCTypeID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteConIndicatorDrugsRelToMedCond(DrugsMCTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteConIndicatorDrugsRelToMedCond. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DeleteConIndicatorDrugsRelToMedCond);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion


        #region RefDisposableMedicalResources
        public IList<RefDisposableMedicalResource> RefDisposableMedicalResources_Paging(RefDisposableMedicalResourceSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefDisposableMedicalResources_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDisposableMedicalResources_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_RefDisposableMedicalResources_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RefDisposableMedicalResourcesInsertUpdate(RefDisposableMedicalResource Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.RefDisposableMedicalResourcesInsertUpdate(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDisposableMedicalResourcesInsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_RefDisposableMedicalResourcesInsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RefDisposableMedicalResources_MarkDelete(long DMedRscrID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefDisposableMedicalResources_MarkDelete(DMedRscrID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDisposableMedicalResources_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_RefDisposableMedicalResources_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public RefDisposableMedicalResource RefDisposableMedicalResources_ByDMedRscrID(long DMedRscrID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefDisposableMedicalResources_ByDMedRscrID(DMedRscrID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDisposableMedicalResources_ByDMedRscrID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_RefDisposableMedicalResources_ByDMedRscrID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region DisposableMedicalResourceTypes
        public IList<DisposableMedicalResourceType> DisposableMedicalResourceTypes_GetAll()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DisposableMedicalResourceTypes_GetAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DisposableMedicalResourceTypes_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DisposableMedicalResourceTypes_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion



        #region 16*. Medical Condition

        public List<RefMedContraIndicationTypes> GetRefMedicalConditionTypes()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefMedicalConditionTypes();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefMedicalConditionTypes. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetRefMedicalConditionTypes);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteRefMedicalConditionTypes(int MCTypeID, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteRefMedicalConditionTypes(MCTypeID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteRefMedicalConditionTypes. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DeleteRefMedicalConditionTypes);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool InsertRefMedicalConditionTypes(string MedConditionType, int Idx, int? AgeFrom, int? AgeTo, long V_AgeUnit)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InsertRefMedicalConditionTypes(MedConditionType, Idx, AgeFrom, AgeTo, V_AgeUnit);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertRefMedicalConditionTypes. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_InsertRefMedicalConditionTypes);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool UpdateRefMedicalConditionTypes(int MCTypeID, string MedConditionType, int? AgeFrom, int? AgeTo, long V_AgeUnit)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateRefMedicalConditionTypes(MCTypeID, MedConditionType, AgeFrom, AgeTo, V_AgeUnit);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateRefMedicalConditionTypes. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_UpdateRefMedicalConditionTypes);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public List<RefMedContraIndicationICD> GetRefMedicalConditions(int MCTypeID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefMedicalConditions(MCTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefMedicalConditions. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetRefMedicalConditions);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteRefMedicalConditions(int MCID, int MCTypeID, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteRefMedicalConditions(MCID, MCTypeID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteRefMedicalConditions. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DeleteRefMedicalConditions);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool InsertRefMedicalConditions(int MCTypeID, string ICD10Code, string DiseaseNameVN)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InsertRefMedicalConditions(MCTypeID, ICD10Code, DiseaseNameVN);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertRefMedicalConditions. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_InsertRefMedicalConditions);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateRefMedicalConditions(int MCID, int MCTypeID, string MCDescription)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateRefMedicalConditions(MCID, MCTypeID, MCDescription);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateRefMedicalConditions. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_UpdateRefMedicalConditions);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        //<Giá Nhà Thuốc>
        #region Giá Bán Thuốc Của Nhà Thuốc PharmacySellingItemPrices
        public List<PharmacySellingItemPrices> PharmacySellingItemPrices_ByDrugID_Paging(
        PharmacySellingItemPricesSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacySellingItemPrices_ByDrugID_Paging(
                    SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingItemPrices_ByDrugID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingItemPrices_ByDrugID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PharmacySellingItemPrices_Save(PharmacySellingItemPrices Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.PharmacySellingItemPrices_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingItemPrices_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingItemPrices_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PharmacySellingItemPrices_Item_Save(PharmacySellingItemPrices Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.PharmacySellingItemPrices_Item_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingItemPrices_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingItemPrices_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacySellingItemPrices_SaveRow(PharmacySellingItemPrices Obj)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacySellingItemPrices_SaveRow(Obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingItemPrices_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingItemPrices_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PharmacySellingItemPrices_MarkDelete(Int64 PharmacySellingItemPriceID, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.PharmacySellingItemPrices_MarkDelete(PharmacySellingItemPriceID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingItemPrices_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingItemPrices_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //KMx: Sau khi kiểm tra, thấy hàm này không còn sử dụng nữa (31/05/2014 17:18)
        //public PharmacySellingItemPrices PharmacySellingItemPrices_ByPharmacySellingItemPriceID(Int64 PharmacySellingItemPriceID)
        //{
        //    try
        //    {
        //        return RefDrugGenericDetailsProvider.Instance.PharmacySellingItemPrices_ByPharmacySellingItemPriceID(PharmacySellingItemPriceID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of PharmacySellingItemPrices_ByPharmacySellingItemPriceID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingItemPrices_ByPharmacySellingItemPriceID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        #endregion

        #region Bảng Giá Bán Thuốc Của Nhà Thuốc PharmacySellingPriceList
        public void PharmacySellingPriceList_CheckCanAddNew(out bool CanAddNew)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.PharmacySellingPriceList_CheckCanAddNew(out CanAddNew);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_CheckCanAddNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingPriceList_CheckCanAddNew);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<PharmacySellingItemPrices> PharmacySellingPriceList_AutoCreate(out string Result)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacySellingPriceList_AutoCreate(out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_AutoCreate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingPriceList_AutoCreate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacySellingItemPrices> PharmacySellingPriceList_AutoCreate_V2(out string Result)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacySellingPriceList_AutoCreate_V2(out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_AutoCreate_V2. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingPriceList_AutoCreate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacyReferenceItemPrice> PharmacyRefPriceList_AutoCreate()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyRefPriceList_AutoCreate();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_AutoCreate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacyRefPriceList_AutoCreate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacyRefPriceList_AddNew(PharmacyReferencePriceList Obj, out long ReferencePriceListID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyRefPriceList_AddNew(Obj, out ReferencePriceListID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyRefPriceList_AddNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacyRefPriceList_AddNew);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PharmacyRefPriceList_Update(PharmacyReferencePriceList Obj)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacyRefPriceList_Update(Obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacyRefPriceList_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacyRefPriceList_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacyReferencePriceList> GetReferencePriceList(PharmacySellingPriceListSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetReferencePriceList(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetReferencePriceList);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacyReferenceItemPrice> GetPharmacyRefItemPrice(Int64 ReferencePriceListID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetPharmacyRefItemPrice(ReferencePriceListID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPharmacyRefItemPrice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetPharmacyRefItemPrice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PharmacySellingPriceList_AddNew(PharmacySellingPriceList Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.PharmacySellingPriceList_AddNew(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_AddNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingPriceList_AddNew);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public Nullable<DateTime> PharmacySellingItemPrices_EffectiveDateMax()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacySellingItemPrices_EffectiveDateMax();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingItemPrices_EffectiveDateMax. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingItemPrices_EffectiveDateMax);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacySellingPriceList> PharmacySellingPriceList_GetList_Paging(
            PharmacySellingPriceListSearchCriteria SearchCriteria,
                                     int PageIndex,
                                     int PageSize,
                                     string OrderBy,
                                     bool CountTotal,
                                     out int Total
                                        , out DateTime curDate)
        {
            try
            {
                curDate = DateTime.Now;
                return RefDrugGenericDetailsProvider.Instance.PharmacySellingPriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total, out curDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingPriceList_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PharmacySellingPriceList_Delete(Int64 PharmacySellingPriceListID, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.PharmacySellingPriceList_Delete(PharmacySellingPriceListID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingPriceList_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<PharmacySellingItemPrices> PharmacySellingPriceList_Detail(Int64 PharmacySellingPriceListID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacySellingPriceList_Detail(PharmacySellingPriceListID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_Detail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingPriceList_Detail);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmacySellingItemPrices> PharmacySellingPriceList_Detail_V2(Int64 PharmacySellingPriceListID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacySellingPriceList_Detail_V2(PharmacySellingPriceListID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_Detail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingPriceList_Detail);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }



        public void PharmacySellingPriceList_Update(PharmacySellingPriceList Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.PharmacySellingPriceList_Update(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellingPriceList_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellingPriceList_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        private static string FormatField(string data, string format)
        {
            switch (format)
            {
                case "xls":
                    return String.Format("<Cell><Data ss:Type=\"String" +
                       "\">{0}</Data></Cell>", data);
                case "CSV":
                    return String.Format("\"{0}\"",
                      data.Replace("\"", "\"\"\"").Replace("\n",
                      "").Replace("\r", ""));
            }
            return data;
        }

        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields, string strFormat)
        {
            switch (strFormat)
            {
                case "xls":
                    strBuilder.AppendLine("<Row>");
                    strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
                    strBuilder.AppendLine("</Row>");
                    break;
                case "CSV":
                    strBuilder.AppendLine(String.Join(",", lstFields.ToArray()));
                    break;
            }
        }

        private string ExportAll(List<List<string>> listData, string sheetName, string fileName, string fileExt)
        {
            try
            {
                if (listData != null)
                {
                    string strFormat = fileExt.Substring(fileExt.IndexOf('.') + 1).ToLower();
                    StringBuilder strBuilder = new StringBuilder();
                    List<string> lstFields = new List<string>();
                    //BuildStringOfRow(strBuilder, columnNames);
                    foreach (List<string> data in listData)
                    {
                        lstFields.Clear();
                        foreach (var col in data)
                        {
                            lstFields.Add(FormatField(col, strFormat));
                        }
                        BuildStringOfRow(strBuilder, lstFields, strFormat);
                    }
                    fileName += fileExt;
                    StreamWriter sw = new StreamWriter(fileName);
                    if (strFormat == "xls")
                    {
                        //Let us write the headers for the Excel XML
                        sw.WriteLine("<?xml version=\"1.0\" " +
                                        "encoding=\"utf-8\"?>");
                        sw.WriteLine("<?mso-application progid" +
                                        "=\"Excel.Sheet\"?>");
                        sw.WriteLine("<Workbook xmlns=\"urn:" +
                                        "schemas-microsoft-com:office:spreadsheet\">");
                        sw.WriteLine("<DocumentProperties " +
                                        "xmlns=\"urn:schemas-microsoft-com:" +
                                        "office:office\">");
                        //sw.WriteLine("<Author>" + Globals.LoggedUserAccount.Staff.FullName.Trim() + "</Author>");
                        sw.WriteLine("<Created>" +
                                        DateTime.Now.ToLocalTime().ToLongDateString() +
                                        "</Created>");
                        sw.WriteLine("<LastSaved>" +
                                        DateTime.Now.ToLocalTime().ToLongDateString() +
                                        "</LastSaved>");
                        sw.WriteLine("<Company>Viện Tim</Company>");
                        sw.WriteLine("<Version>12.00</Version>");
                        sw.WriteLine("</DocumentProperties>");
                        sw.WriteLine("<Worksheet ss:Name=\"" + sheetName + "\" " +
                            "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                        sw.WriteLine("<Table>");
                    }
                    sw.Write(strBuilder.ToString());
                    if (strFormat == "xls")
                    {
                        sw.WriteLine("</Table>");
                        sw.WriteLine("</Worksheet>");
                        sw.WriteLine("</Workbook>");
                    }
                    sw.Close();
                }
                return fileName;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return "";
            }
        }
        //KMx: Vì sao xuất excel cho bảng giá DV và CLS mà nằm trong DrugService.cs (nhà thuốc) (27/03/2014 10:56).
        //     Function xuất excel này được viết lần đầu tiên cho bảng giá nhà thuốc.
        //     Nhưng sau đó BV yêu cầu thêm là xuất excel cho bảng giá DV và CLS. Cho nên bảng giá DV và CLS xài chung function với nhà thuốc.
        //     Vì vậy: bảng giá DV và CLS gọi vào DrugService.cs này.
        //Hàm này không còn dùng nữa, chuyển sang dùng chung hàm ExportToExcellAllGeneric() của TransactionService.cs (07/08/2014 09:49)
        public Stream ExportToExcelAllItemsPriceList(Int64 PriceListID, int PriceListType, string StoreName, string ShowTitle)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Export To Excel All Items Price List.", CurrentUser);

                var res = RefDrugGenericDetailsProvider.Instance.ExportToExcelAllItemsPriceList(PriceListID, PriceListType);

                string folderPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(folderPath);
                }

                string SheetName = "BANG_GIA";
                switch (PriceListType)
                {
                    case (int)AllLookupValues.PriceListType.BANG_GIA_THUOC:
                        SheetName = "BANG_GIA_THUOC";
                        break;
                    case (int)AllLookupValues.PriceListType.BANG_GIA_DV:
                        SheetName = "BANG_GIA_DV";
                        break;
                    case (int)AllLookupValues.PriceListType.BANG_GIA_PCL:
                        SheetName = "BANG_GIA_PCL";
                        break;
                }

                var filePath = ExportAll(res, SheetName, folderPath + "\\" + StoreName, ShowTitle);
                return CommonFunction.GetVideoAndImage(filePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Export To Excel All Items Price List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_ExportPriceListToExcel, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Thang Giá Bán Thuốc Của Nhà Thuốc PharmacySellPriceProfitScale
        public IList<PharmacySellPriceProfitScale> PharmacySellPriceProfitScale_GetList_Paging(bool IsActive,
                                                                                                int PageIndex,
                                                                                                int PageSize,
                                                                                                string OrderBy,
                                                                                                bool CountTotal,
                                                                                                out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmacySellPriceProfitScale_GetList_Paging(IsActive,
                    PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellPriceProfitScale_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellPriceProfitScale_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PharmacySellPriceProfitScale_AddEdit(PharmacySellPriceProfitScale Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.PharmacySellPriceProfitScale_AddEdit(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellPriceProfitScale_AddEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellPriceProfitScale_AddEdit);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public void PharmacySellPriceProfitScale_IsActive(Int64 PharmacySellPriceProfitScaleID, Boolean IsActive, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.PharmacySellPriceProfitScale_IsActive(PharmacySellPriceProfitScaleID, IsActive, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PharmacySellPriceProfitScale_IsActive. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_PharmacySellPriceProfitScale_IsActive);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion
        //<Giá Nhà Thuốc>

        //<Giá Khoa Dược>
        #region Giá Bán Thuốc Của Khoa Dược DrugDeptSellingItemPrices
        public List<DrugDeptSellingItemPrices> DrugDeptSellingItemPrices_ByDrugID_Paging(
        DrugDeptSellingItemPricesSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSellingItemPrices_ByDrugID_Paging(
                    SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingItemPrices_ByDrugID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingItemPrices_ByDrugID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="genMedProductId"></param>
        /// <returns></returns>
        public DrugDeptSellingItemPrices GetDrugDeptSellingItemPriceDetails(long genMedProductId)
        {
            if (genMedProductId == 0)
            {
                return null;
            }
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugDeptSellingItemPriceDetails(genMedProductId);
            }
            catch (Exception _Ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugDeptSellingItemPriceDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(_Ex,
                    "GetDrugDeptSellingItemPriceDetails. Status: Failed.");
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void DrugDeptSellingItemPrices_Save(DrugDeptSellingItemPrices Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.DrugDeptSellingItemPrices_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingItemPrices_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingItemPrices_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void DrugDeptSellingItemPrices_MarkDelete(Int64 DrugDeptSellingItemPriceID, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.DrugDeptSellingItemPrices_MarkDelete(DrugDeptSellingItemPriceID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingItemPrices_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingItemPrices_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public DrugDeptSellingItemPrices DrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(Int64 DrugDeptSellingItemPriceID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(DrugDeptSellingItemPriceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Bảng Giá Bán Thuốc Của Khoa Dược DrugDeptSellingPriceList
        public void DrugDeptSellingPriceList_CheckCanAddNew(out bool CanAddNew)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.DrugDeptSellingPriceList_CheckCanAddNew(out CanAddNew);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingPriceList_CheckCanAddNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingPriceList_CheckCanAddNew);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptSellingItemPrices> DrugDeptSellingPriceList_AutoCreate(long V_MedProductType, out string Result)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSellingPriceList_AutoCreate(V_MedProductType, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingPriceList_AutoCreate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingPriceList_AutoCreate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void DrugDeptSellingPriceList_AddNew(DrugDeptSellingPriceList Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.DrugDeptSellingPriceList_AddNew(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingPriceList_AddNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingPriceList_AddNew);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Nullable<DateTime> DrugDeptSellingItemPrices_EffectiveDateMax()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSellingItemPrices_EffectiveDateMax();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingItemPrices_EffectiveDateMax. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingItemPrices_EffectiveDateMax);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptSellingPriceList> DrugDeptSellingPriceList_GetList_Paging(
            DrugDeptSellingPriceListSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total, out DateTime curDate)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSellingPriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total, out curDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingPriceList_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingPriceList_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public void DrugDeptSellingPriceList_Delete(Int64 DrugDeptSellingPriceListID, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.DrugDeptSellingPriceList_Delete(DrugDeptSellingPriceListID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingPriceList_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingPriceList_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<DrugDeptSellingItemPrices> DrugDeptSellingPriceList_Detail(Int64 DrugDeptSellingPriceListID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSellingPriceList_Detail(DrugDeptSellingPriceListID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingPriceList_Detail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingPriceList_Detail);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public void DrugDeptSellingPriceList_Update(DrugDeptSellingPriceList Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.DrugDeptSellingPriceList_Update(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellingPriceList_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellingPriceList_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Thang Giá Bán Thuốc Của Khoa Dược DrugDeptSellPriceProfitScale
        public IList<DrugDeptSellPriceProfitScale> DrugDeptSellPriceProfitScale_GetList_Paging(long V_MedProductType, bool IsActive, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSellPriceProfitScale_GetList_Paging(V_MedProductType, IsActive,
                    PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellPriceProfitScale_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellPriceProfitScale_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void DrugDeptSellPriceProfitScale_AddEdit(DrugDeptSellPriceProfitScale Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.DrugDeptSellPriceProfitScale_AddEdit(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellPriceProfitScale_AddEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellPriceProfitScale_AddEdit);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public void DrugDeptSellPriceProfitScale_IsActive(Int64 DrugDeptSellPriceProfitScaleID, Boolean IsActive, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.DrugDeptSellPriceProfitScale_IsActive(DrugDeptSellPriceProfitScaleID, IsActive, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DrugDeptSellPriceProfitScale_IsActive. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_DrugDeptSellPriceProfitScale_IsActive);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion
        //</Giá Khoa Dược>

        //▼===== #001
        public PrescriptionDetail GetDrugInformation(long? DrugID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugInformation(DrugID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugInformation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetAllContrainIndicatorDrugs);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== #001

        public List<BidDetail> GetAutoBidDetailCollection(long V_MedProductType,
            DateTime FromDate,
            DateTime ToDate, long StoreID,
            float Factor)
        {
            try
            {
                System.Data.DataTable mTable = RefDrugGenericDetailsProvider.Instance.OutwardDrugMedDeptInvoices_ListGenMedProductIDNhap_Ton(V_MedProductType, FromDate, ToDate, StoreID, true, 1);
                List<BidDetail> mBidDetailCollection = new List<BidDetail>();
                if (mTable != null && mTable.Rows != null && mTable.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow aRow in mTable.Rows)
                    {
                        BidDetail aBidDetail = new BidDetail();
                        if (mTable.Columns.Contains("OutQty") && aRow["OutQty"] != null && aRow["OutQty"] != DBNull.Value)
                        {
                            int mOutQty = Convert.ToInt32(aRow["OutQty"]);
                            if (mOutQty == 0)
                            {
                                continue;
                            }
                            aBidDetail.ApprovedQty = Convert.ToInt32(mOutQty * Factor);
                        }
                        else
                        {
                            continue;
                        }
                        if (mTable.Columns.Contains("GenMedProductID") && aRow["GenMedProductID"] != null && aRow["GenMedProductID"] != DBNull.Value)
                        {
                            aBidDetail.DrugID = Convert.ToInt64(aRow["GenMedProductID"]);
                        }
                        else
                        {
                            continue;
                        }
                        if (mTable.Columns.Contains("Code") && aRow["Code"] != null && aRow["Code"] != DBNull.Value)
                        {
                            aBidDetail.DrugCode = aRow["Code"].ToString();
                        }
                        if (mTable.Columns.Contains("HICode") && aRow["HICode"] != null && aRow["HICode"] != DBNull.Value)
                        {
                            aBidDetail.HICode = aRow["HICode"].ToString();
                        }
                        if (mTable.Columns.Contains("BrandName") && aRow["BrandName"] != null && aRow["BrandName"] != DBNull.Value)
                        {
                            aBidDetail.ReportBrandName = aRow["BrandName"].ToString();
                        }
                        if (mTable.Columns.Contains("Visa") && aRow["Visa"] != null && aRow["Visa"] != DBNull.Value)
                        {
                            aBidDetail.Visa = aRow["Visa"].ToString();
                        }
                        mBidDetailCollection.Add(aBidDetail);
                    }
                }
                return mBidDetailCollection;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutwardDrugMedDeptInvoices_ListGenMedProductIDNhap_Ton. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SERVICE_GetAllContrainIndicatorDrugs);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
    }
}