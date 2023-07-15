using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
using ErrorLibrary;
using System.Runtime.Serialization;
using AxLogging;
using aEMR.DataAccessLayer.Providers;
using ErrorLibrary.Resources;
using System.Data.SqlClient;
using eHCMSLanguage;

namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class Unit : eHCMS.WCFServiceCustomHeader, IUnit
    {
        public Unit()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        #region Unit member

        public IList<RefUnit> GetUnits()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetUnits();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<RefUnit> GetPagingUnits(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetPagingUnits(pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPagingUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<RefUnit> SearchUnit(string UnitName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchUnit(UnitName, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchUnit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteUnitByID(long unitID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteUnitByID(unitID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteUnitByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateUnit(RefUnit unit)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateUnit(unit);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateUnit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_UPDATE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewUnit(RefUnit newunit)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddNewUnit(newunit);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddNewUnit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_ADD);
                if (ex is SqlException)
                { throw new Exception(ex.Message); }
                else { throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError)); }



            }
        }

        public RefUnit GetUnitByID(long UnitID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetUnitByID(UnitID);
            }

            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetUnitByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region DrugDeptUnit member

        public IList<RefUnit> GetDrugDeptUnits()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugDeptUnits();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        // --------- DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học

        //public IList<RefGeneralUnits> LoadRefGeneralUnits()
        //{
        //    try
        //    { 
        //        return RefDrugGenericDetailsProvider.Instance.LoadRefGeneralUnits();
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public IList<ScientificResearchActivities> GetScientificResearchActivityList(ScientificResearchActivities Activity)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetScientificResearchActivityList(Activity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<TrainingForSubOrg> GetTrainingForSubOrgList(TrainingForSubOrg Training)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetTrainingForSubOrgList(Training);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Lookup> GetTrainningTypeList()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetTrainningTypeList();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InsertUpdateScientificResearchActivity(bool ISAdd, ScientificResearchActivities objActivity, out int Result)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InsertUpdateScientificResearchActivity(ISAdd, objActivity, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool InsertUpdateTrainingForSubOrg(bool ISAdd, TrainingForSubOrg objTraining, out int Result)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InsertUpdateTrainingForSubOrg(ISAdd, objTraining, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteScientificResearchActivity(long ActivityID, out int Result)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteScientificResearchActivity(ActivityID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteTrainingForSubOrg(long TrainingID, out int Result)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteTrainingForSubOrg(TrainingID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<ActivityClasses> ActivityClassListAll(long ClassID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ActivityClassListAll(ClassID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //  DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học----------------------------------

        public IList<RefUnit> GetPagingDrugDeptUnits(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetPagingDrugDeptUnits(pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPagingUnits. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<RefUnit> SearchDrugDeptUnit(string UnitName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchDrugDeptUnit(UnitName, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchUnit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_FOUND);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteDrugDeptUnitByID(long unitID, long? StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteDrugDeptUnitByID(unitID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteUnitByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateDrugDeptUnit(RefUnit unit)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateDrugDeptUnit(unit);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateUnit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_UPDATE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewDrugDeptUnit(RefUnit newunit)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddNewDrugDeptUnit(newunit);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddNewUnit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_ADD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public RefUnit GetDrugDeptUnitByID(long UnitID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugDeptUnitByID(UnitID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetUnitByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_UNIT_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Selling Price Formuler
        public SellingPriceFormular GetSellingPriceFormularByID(long ItemID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSellingPriceFormularByID(ItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSellingPriceFormularByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_FORPRICE_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public SellingPriceFormular GetSellingPriceFormularIsActive()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSellingPriceFormularIsActive();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSellingPriceFormularIsActive. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_FORPRICE_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<SellingPriceFormular> GetSellingPriceFormularAll(DateTime? FromDate, DateTime? Todate, int PageIndex, int PageSize, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSellingPriceFormularAll(FromDate, Todate, PageIndex, PageSize, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSellingPriceFormularAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_FORPRICE_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddNewSellingPriceFormular(SellingPriceFormular p)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddNewSellingPriceFormular(p);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddNewSellingPriceFormular. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_FORPRICE_CANNOT_ADD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateSellingPriceFormular(SellingPriceFormular p)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateSellingPriceFormular(p);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateSellingPriceFormular. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_FORPRICE_CANNOT_UPDATE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public bool DeleteSellingPriceFormular(long ItemID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteSellingPriceFormular(ItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateSellingPriceFormular. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_FORPRICE_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
    }
}
