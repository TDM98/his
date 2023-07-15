using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Runtime.Serialization;
using ErrorLibrary;
using DataEntities;
using eHCMS.DAL;
using AxLogging;
using ErrorLibrary.Resources;
using eHCMSLanguage;

namespace ConfigurationManagerService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class UserAccountsService : eHCMS.WCFServiceCustomHeader, IUserAccounts
    {

        public UserAccountsService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public List<Lookup> GetLookupOperationType() 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetLookupOperationType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupOperationType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_LOOKUP_OPERATION_TYPE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region user account
        public List<UserAccount> GetAllUserAccount(long AccountID)
        {
            {
                try
                {
                    return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllUserAccount(AccountID);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of GetAllUserAccount. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_USERACCOUNT);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }
        public List<UserAccount> GetAllUserAccountsPaging(string AccountName, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllUserAccountsPaging(AccountName, PageSize, PageIndex, OrderBy,
                                                         CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllUserAccountsPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_USERACCOUNTS_PAGING);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddNewUserAccount(long StaffID, string AccountName, string AccountPassword, bool IsActivated) 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewUserAccount(StaffID, AccountName, AccountPassword, IsActivated);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewUserAccount. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_USERACCOUNT);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateUserAccount(int AccountID, long StaffID, string AccountName, string AccountPassword, bool IsActivated) 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateUserAccount(AccountID, StaffID, AccountName, AccountPassword, IsActivated) ;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateUserAccount. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_USERACCOUNT);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteUserAccount(int AccountID) 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteUserAccount(AccountID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteUserAccount. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_USERACCOUNT);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Group

        public List<Group> GetAllGroupAllPaging(long GroupID, int PageSize, int PageIndex, string OrderBy,
                                                         bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllGroupAllPaging(GroupID,PageSize, PageIndex, OrderBy,
                                                         CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllGroupAllPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_GROUP_ALLPAGING);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<Group> GetAllGroupByGroupID(long GroupID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllGroupByGroupID(GroupID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllGroupByGroupID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_GROUP_BY_GROUPID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewGroup(string GroupName, string Description) 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewGroup(GroupName, Description) ;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewGroup. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_GROUP);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateGroup(int GroupID, string GroupName, string Description) 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateGroup(GroupID, GroupName, Description) ;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateGroup. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_GROUP);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteGroup(int GroupID) 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteGroup(GroupID) ;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteGroup. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_GROUP);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region user group
        public List<UserGroup> GetAllUserGroupGetByID(long AccountID, long GroupID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllUserGroupGetByID(AccountID, GroupID
                    , PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllUserGroupGetByID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_USERGROUP_GET_BYID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewUserGroup(long AccountID, int GroupID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewUserGroup(AccountID, GroupID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewUserGroup. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_USERGROUP);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddNewUserGroupXML(IList<UserGroup> lstUserGroup)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewUserGroupXML(lstUserGroup);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewUserGroupXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_GROUP);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateUserGroup(long UGID, long AccountID, int GroupID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateUserGroup(UGID, AccountID, GroupID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateUserGroup. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_USERGROUP);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteUserGroup(int UGID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteUserGroup(UGID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteUserGroup. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_USERGROUP);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Group Role

        public List<GroupRole> GetAllGroupRolesGetByID(long GroupID, long RoleID
                                        , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total) 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllGroupRolesGetByID(GroupID, RoleID
                                        , PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllGroupRolesGetByID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_GROUPROLES_GET_BYID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewGroupRoles(int GroupID, int RoleID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewGroupRoles(GroupID, RoleID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewGroupRoles. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_GROUPROLES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewGroupRolesXML(IList<GroupRole> lstGroupRole)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewGroupRolesXML(lstGroupRole);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewGroupRoles. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_GROUPROLES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateGroupRoles(long GroupRoleID, int GroupID, int RoleID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateGroupRoles(GroupRoleID, GroupID, RoleID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateGroupRoles. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_GROUPROLES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool DeleteGroupRoles(long GroupRoleID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteGroupRoles(GroupRoleID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteGroupRoles. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_GROUPROLES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Permission

        public List<Permission> GetAllPermissions_GetByID(long RoleID, long OperationID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total) 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllPermissions_GetByID(RoleID, OperationID
                                    , PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllPermissions_GetByID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_PERMISSIONS_GETBYID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Modules
        public List<DataEntities.Module> GetAllModules()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllModules();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllModules. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_MODULES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewModules(string ModuleName,int eNum, string Description, int Idx)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewModules(ModuleName, eNum,Description, Idx);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewModules. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_MODULES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateModules(long ModuleID, int eNum, string ModuleName, string Description, int Idx)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateModules(ModuleID, eNum,ModuleName, Description, Idx);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateModules. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_MODULES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateModulesEnum(long ModuleID, int eNum)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateModulesEnum(ModuleID, eNum);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateModulesEnum. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_MODULES_ENUM);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteModules(long ModuleID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteModules(ModuleID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteModules. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_MODULES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Functions
        public List<Function> GetAllFunction(long ModuleID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllFunction(ModuleID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllFunction. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_FUNCTION);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Function> GetAllFunctionsPaging(long ModuleID, int PageSize, int PageIndex, string OrderBy,
                                                             bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllFunctionsPaging(ModuleID, PageSize, PageIndex, OrderBy,
                                                                  CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllFunctionsPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_FUNCTIONS_PAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }
        }
        public bool AddNewFunctions(int ModuleID, int eNum, string FunctionName, string FunctionDescription, int Idx)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewFunctions(ModuleID, eNum,FunctionName, FunctionDescription, Idx);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewFunctions. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_FUNCTIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateFunctionsEnum(long FunctionID, int eNum)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateFunctionsEnum(FunctionID, eNum);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateFunctionsEnum. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_FUNCTIONS_ENUM);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateFunctions(long FunctionID, int eNum, int ModuleID, string FunctionName, string FunctionDescription, byte Idx)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateFunctions(FunctionID,eNum, ModuleID, FunctionName, FunctionDescription, Idx);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateFunctions. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_FUNCTIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteFunctions(long FunctionID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteFunctions(FunctionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteFunctions. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_FUNCTIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #endregion

        #region Operations
        public List<Operation> GetAllOperations(long OperationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllOperations(OperationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllOperations. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_OPERATIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<Operation> GetAllOperationsByFuncID(long FunctionID) 
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllOperationsByFuncID(FunctionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllOperationsByFuncID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_OPERATIONS_BY_FUNCID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Operation> GetAllOperationsByFuncIDPaging(long FunctionID
                                                       , int PageSize, int PageIndex, string OrderBy, bool CountTotal,
                                                       out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllOperationsByFuncIDPaging(FunctionID, PageSize, PageIndex, OrderBy,
                                                                  CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllOperationsByFuncIDPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_OPERATIONS_BY_FUNCID_PAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }
        }

        public bool AddNewOperations(long FunctionID, string OperationName, string Description)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewOperations(FunctionID, OperationName, Description);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewOperations. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_OPERATIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateOperations(long OperationID, int Enum, string OperationName, string Description)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateOperations(OperationID, Enum, OperationName, Description);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateOperations. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_OPERATIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteOperations(long OperationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteOperations(OperationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteOperations. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_OPERATIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Permission

        public List<Permission> GetAllPermissions(long PermissionItemID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllPermissions(PermissionItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllPermissions. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_PERMISSIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddNewPermissions(int RoleID, long OperationID, bool FullControl
                                                , bool pView
                                                , bool pAdd
                                                , bool pUpdate
                                                , bool pDelete
                                                , bool pReport
                                                , bool pPrint)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewPermissions(RoleID, OperationID,FullControl
			                                    ,pView  
			                                    ,pAdd  
			                                    ,pUpdate  
			                                    ,pDelete  
			                                    ,pReport
                                                ,pPrint);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewPermissions. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_PERMISSIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdatePermissions(long PermissionItemID, int RoleID, long OperationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdatePermissions(PermissionItemID, RoleID, OperationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdatePermissions. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_PERMISSIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdatePermissionsFull(long PermissionItemID
                                                ,bool FullControl
			                                    ,bool pView  
			                                    ,bool pAdd  
			                                    ,bool pUpdate  
			                                    ,bool pDelete  
			                                    ,bool pReport
                                                , bool pPrint)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdatePermissionsFull( PermissionItemID
                                                , FullControl
			                                    , pView  
			                                    , pAdd  
			                                    , pUpdate  
			                                    , pDelete  
			                                    , pReport
                                                ,  pPrint);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdatePermissions. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_PERMISSIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeletePermissions(long PermissionItemID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeletePermissions(PermissionItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeletePermissions. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_PERMISSIONS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #endregion

        #region Role

        public List<Role> GetAllRolesAllPaging(long RoleID, int PageSize, int PageIndex, string OrderBy,
                                                         bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllRolesAllPaging(RoleID, PageSize, PageIndex, OrderBy,
                                                                  CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRolesAllPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_ROLES_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }
        }

        public List<Role> GetAllRoles()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllRoles();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRoles. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_ROLES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddNewRoles(string RoleName, string Description)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewRoles(RoleName, Description);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewRoles. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_ROLES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateRoles(int RoleID, string RoleName, string Description)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateRoles(RoleID, RoleName, Description);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateRoles. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_ROLES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteRoles(int RoleID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteRoles(RoleID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteRoles. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_ROLES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Modules tree
        // 20180921 TNHX: Edit method for get all detail Modules tree
        public List<ModulesTree> GetModulesTreeView()
        {
            try
            {
                List<ModulesTree> results = new List<ModulesTree>();
                List<Um_ModuleFunctionOperations> listAll = aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllDetails_ModuleFunctionOperations();
                long curModuleID = 0;
                long curFunctionID = 0;
                ModulesTree theCurModTreeNode = null;
                ModulesTree theCurFuncTreeNode = null;
                foreach (Um_ModuleFunctionOperations theLine in listAll)
                {                    
                    if (theLine.ModuleID != curModuleID)
                    {
                        curModuleID = theLine.ModuleID;
                        curFunctionID = theLine.FunctionID;
                        theCurModTreeNode = new ModulesTree(theLine.ModuleID, theLine.ModuleName, theLine.ModuleDescription, 1, theLine.eNumModule, null, null, "");
                        theCurFuncTreeNode = new ModulesTree(theLine.FunctionID, theLine.FunctionName, theLine.FunctionDescription, 2, theLine.eNumFunction, null, null, "");
                        ModulesTree theOpTreeNode = new ModulesTree(theLine.OperationID, theLine.OperationName, theLine.OperationDescription, 3, theLine.EnumOperation, null, null, "");
                        theCurFuncTreeNode.Children.Add(theOpTreeNode);
                        theCurModTreeNode.Children.Add(theCurFuncTreeNode);
                        results.Add(theCurModTreeNode);
                    }
                    else
                    {
                        if (theLine.FunctionID != curFunctionID)
                        {
                            curFunctionID = theLine.FunctionID; 
                            theCurFuncTreeNode = new ModulesTree(theLine.FunctionID, theLine.FunctionName, theLine.FunctionDescription, 2, theLine.eNumFunction, null, null, "");
                            ModulesTree theOpTreeNode = new ModulesTree(theLine.OperationID, theLine.OperationName, theLine.OperationDescription, 3, theLine.EnumOperation, null, null, "");
                            theCurFuncTreeNode.Children.Add(theOpTreeNode);
                            theCurModTreeNode.Children.Add(theCurFuncTreeNode);
                        }
                        else
                        {
                            ModulesTree theOpTreeNode = new ModulesTree(theLine.OperationID, theLine.OperationName, theLine.OperationDescription, 3, theLine.EnumOperation, null, null, "");
                            theCurFuncTreeNode.Children.Add(theOpTreeNode);
                        }
                    }
                }
                return results;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllModules. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_MODULES);
                
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region UserLoginHistory
        public List<UserLoginHistory> GetUserLogHisGByUserAccount()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetUserLogHisGByUserAccount();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUserLogHisGByUserAccount. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_USERLOGHISG_BY_USERACCOUNT);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }
            
        }
        public List<UserLoginHistory> GetUserLogHisGByHostName()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetUserLogHisGByHostName();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUserLogHisGByHostName. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_USERLOGHISG_BY_HOSTNAME);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }
            
        }

        public List<UserLoginHistory> GetAllUserLoginHistoryPaging(UserLoginHistory ulh, int PageSize,
                                                                            int PageIndex, string OrderBy,
                                                                            bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllUserLoginHistoryPaging( ulh, PageSize,
                                                                            PageIndex, OrderBy,
                                                                            CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllUserLoginHistoryPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_USERLOGIN_HISTORY_PAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }
        }
        
        public long AddUserLoginHistory(UserLoginHistory Ulh)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddUserLoginHistory(Ulh);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddUserLoginHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADD_USERLOGIN_HISTORY);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateUserLoginHistory(UserLoginHistory Ulh)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateUserLoginHistory(Ulh);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateUserLoginHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_USERLOGIN_HISTORY);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteUserLoginHistory(long LoggedHistoryID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteUserLoginHistory(LoggedHistoryID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteUserLoginHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_USERLOGIN_HISTORY);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        
        #endregion

        #region staff

        public Staff GetStaffByID(long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetStaffsByID(StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllStaffAllPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }
            
        }
        
        public List<Staff> GetAllStaffAllPaging(Staff searchStaff, int PageSize, int PageIndex, string OrderBy,
                                                         bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetAllStaffAllPaging(searchStaff, PageSize,
                                                                            PageIndex, OrderBy,
                                                                            CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllStaffAllPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }
        }

        public bool AddNewStaff(Staff newStaff)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.AddNewStaff(newStaff);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewStaff. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_ADDNEW_STAFF);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼====: #001
        public bool UpdateStaff(Staff newStaff, long? StaffEditID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateStaff(newStaff, StaffEditID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateStaff. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_UPDATE_STAFF);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #001

        public bool DeleteStaff(int StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteStaff(StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteStaff. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_DELETE_STAFF);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region StaffDeptResponsibility
        public List<StaffDeptResponsibilities> GetStaffDeptResponsibilitiesByDeptID(StaffDeptResponsibilities p, bool isHis)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetStaffDeptResponsibilitiesByDeptID(p, isHis);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetStaffDeptResponsibilitiesByDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }

        }

        public bool InsertStaffDeptResponsibilitiesXML(List<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.InsertStaffDeptResponsibilitiesXML(lstStaffDeptRes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertStaffDeptResponsibilitiesXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }

        }

        public bool UpdateStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateStaffDeptResponsibilitiesXML(lstStaffDeptRes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateStaffDeptResponsibilitiesXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }

        }

        public bool DeleteStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteStaffDeptResponsibilitiesXML(lstStaffDeptRes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteStaffDeptResponsibilitiesXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }

        }

        #endregion

        #region StaffStoreDeptResponsibility
        public List<StaffStoreDeptResponsibilities> GetStaffStoreDeptResponsibilitiesByDeptID(StaffStoreDeptResponsibilities p, bool isHis)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.GetStaffStoreDeptResponsibilitiesByDeptID(p, isHis);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetStaffStoreDeptResponsibilitiesByDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }

        }

        public bool InsertStaffStoreDeptResponsibilitiesXML(List<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.InsertStaffStoreDeptResponsibilitiesXML(lstStaffDeptRes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertStaffStoreDeptResponsibilitiesXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }

        }

        public bool UpdateStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.UpdateStaffStoreDeptResponsibilitiesXML(lstStaffDeptRes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateStaffStoreDeptResponsibilitiesXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }

        }

        public bool DeleteStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.UserAccounts.Instance.DeleteStaffStoreDeptResponsibilitiesXML(lstStaffDeptRes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteStaffStoreDeptResponsibilitiesXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_STAFF_ALLPAGING);

                throw new FaultException<AxException>(axErr,
                                                      new FaultReason(
                                                          "An error occurred during a WCF call, see inner Detail for more details"));
            }

        }

        #endregion
    }
}
