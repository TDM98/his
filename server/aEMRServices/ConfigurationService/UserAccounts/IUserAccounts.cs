using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ErrorLibrary;
using DataEntities;
/*
 * #001 20210107 TNHX: Add staffeditID
 */
namespace ConfigurationManagerService
{
    [ServiceContract]
    public interface IUserAccounts
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Lookup> GetLookupOperationType();

        #region user account
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<UserAccount> GetAllUserAccount(long AccountID);

        [OperationContract]
        List<UserAccount> GetAllUserAccountsPaging(string AccountName, int PageSize, int PageIndex, string OrderBy,
                                                                   bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewUserAccount(long StaffID, string AccountName, string AccountPassword, bool IsActivated);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateUserAccount(int AccountID, long StaffID, string AccountName, string AccountPassword, bool IsActivated);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteUserAccount(int AccountID);

        #endregion

        #region Group
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Group> GetAllGroupByGroupID(long GroupID);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        List<Group> GetAllGroupAllPaging(long GroupID, int PageSize, int PageIndex, string OrderBy,
                                                bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewGroup(string GroupName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateGroup(int GroupID, string GroupName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteGroup(int GroupID);
        #endregion
        #region User Group
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<UserGroup> GetAllUserGroupGetByID(long AccountID, long GroupID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewUserGroup(long AccountID, int GroupID);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        bool AddNewUserGroupXML(IList<UserGroup> lstUserGroup);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateUserGroup(long UGID, long AccountID, int GroupID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteUserGroup(int UGID);
        #endregion

        #region Group Role
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<GroupRole> GetAllGroupRolesGetByID(long GroupID, long RoleID
                                        , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewGroupRoles(int GroupID, int RoleID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewGroupRolesXML(IList<GroupRole> lstGroupRole);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateGroupRoles(long GroupRoleID, int GroupID, int RoleID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteGroupRoles(long GroupRoleID);
        #endregion

        #region Permission
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Permission> GetAllPermissions_GetByID(long RoleID, long OperationID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        #endregion

        #region Modules
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DataEntities.Module> GetAllModules();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewModules(string ModuleName, int eNum, string Description, int Idx);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateModules(long ModuleID, int eNum, string ModuleName, string Description, int Idx);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateModulesEnum(long ModuleID, int eNum);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteModules(long ModuleID);        

        #endregion

        #region Functions
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Function> GetAllFunction(long ModuleID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Function> GetAllFunctionsPaging(long ModuleID, int PageSize, int PageIndex, string OrderBy,
                                                             bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewFunctions(int ModuleID, int eNum, string FunctionName, string FunctionDescription, int Idx);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateFunctions(long FunctionID, int eNum, int ModuleID, string FunctionName, string FunctionDescription, byte Idx);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateFunctionsEnum(long FunctionID, int eNum);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteFunctions(long FunctionID);
        

        #endregion

        #region Operations
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Operation> GetAllOperations(long OperationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Operation> GetAllOperationsByFuncID(long FunctionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Operation> GetAllOperationsByFuncIDPaging(long FunctionID
                                                       , int PageSize, int PageIndex, string OrderBy, bool CountTotal,
                                                       out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewOperations(long FunctionID, string OperationName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateOperations(long OperationID, int Enum, string OperationName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteOperations(long OperationID);        

        #endregion

        #region Permission
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Permission> GetAllPermissions(long PermissionItemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewPermissions(int RoleID, long OperationID, bool FullControl
                                                , bool pView
                                                , bool pAdd
                                                , bool pUpdate
                                                , bool pDelete
                                                , bool pReport
                                                , bool pPrint);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePermissions(long PermissionItemID, int RoleID, long OperationID);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        bool UpdatePermissionsFull(long PermissionItemID
                                   , bool FullControl
                                   , bool pView
                                   , bool pAdd
                                   , bool pUpdate
                                   , bool pDelete
                                   , bool pReport
                                   , bool pPrint);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePermissions(long PermissionItemID);       
       

        #endregion

        #region Role
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Role> GetAllRoles();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Role> GetAllRolesAllPaging(long RoleID, int PageSize, int PageIndex, string OrderBy,
                                                        bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewRoles(string RoleName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateRoles(int RoleID, string RoleName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRoles(int RoleID);       

        #endregion

        #region modules tree
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ModulesTree> GetModulesTreeView();

        #endregion

        
        #region UserLoginHistory

        [OperationContract]
        List<UserLoginHistory> GetUserLogHisGByUserAccount();

        [OperationContract]
        List<UserLoginHistory> GetUserLogHisGByHostName();

        [OperationContract]
        List<UserLoginHistory> GetAllUserLoginHistoryPaging(UserLoginHistory ulh, int PageSize,
                                                                            int PageIndex, string OrderBy,
                                                                            bool CountTotal, out int Total);

        [OperationContract]
        long AddUserLoginHistory(UserLoginHistory Ulh);

        [OperationContract]
        bool UpdateUserLoginHistory(UserLoginHistory Ulh);

        [OperationContract]
        bool DeleteUserLoginHistory(long LoggedHistoryID);

        #endregion

        #region staff

        [OperationContract]
        Staff GetStaffByID(long StaffID);

        [OperationContract]
        List<Staff> GetAllStaffAllPaging(Staff searchStaff, int PageSize, int PageIndex, string OrderBy,
                                                         bool CountTotal, out int Total);
        [OperationContract]
        bool AddNewStaff(Staff newStaff);

        //▼====: #001
        [OperationContract]
        bool UpdateStaff(Staff newStaff, long? StaffEditID);
        //▲====: #001

        [OperationContract]
        bool DeleteStaff(int StaffID);
        
        #endregion

        #region StaffDeptResponsibility
        [OperationContract]
        List<StaffDeptResponsibilities> GetStaffDeptResponsibilitiesByDeptID(
            StaffDeptResponsibilities p, bool isHis);
        [OperationContract]
        bool InsertStaffDeptResponsibilitiesXML(List<StaffDeptResponsibilities> lstStaffDeptRes);
        [OperationContract]
        bool UpdateStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes);

        [OperationContract]
        bool DeleteStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes);
        
        #endregion

        #region StaffStoreDeptResponsibilities
        [OperationContract]
        List<StaffStoreDeptResponsibilities> GetStaffStoreDeptResponsibilitiesByDeptID(
            StaffStoreDeptResponsibilities p, bool isHis);
        [OperationContract]
        bool InsertStaffStoreDeptResponsibilitiesXML(List<StaffStoreDeptResponsibilities> lstStaffDeptRes);
        [OperationContract]
        bool UpdateStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes);

        [OperationContract]
        bool DeleteStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes);

        #endregion
    }
}
