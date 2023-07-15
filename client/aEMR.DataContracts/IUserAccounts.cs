using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
/*
 * #001 20210107 TNHX: Add staffeditID
 */
namespace UserAccountsProxy
{
    [ServiceContract]
    public interface IUserAccounts
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupOperationType(AsyncCallback callback, object state);
        List<Lookup> EndGetLookupOperationType(IAsyncResult asyncResult);

        #region user account
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllUserAccount(long AccountID, AsyncCallback callback, object state);
        List<UserAccount> EndGetAllUserAccount(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllUserAccountsPaging(string AccountName, int PageSize, int PageIndex, string OrderBy,
                             bool CountTotal, AsyncCallback callback, object state);
        List<UserAccount> EndGetAllUserAccountsPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewUserAccount(long StaffID, string AccountName, string AccountPassword, bool IsActivated, AsyncCallback callback, object state);
        bool EndAddNewUserAccount(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateUserAccount(int AccountID, long StaffID, string AccountName, string AccountPassword, bool IsActivated, AsyncCallback callback, object state);
        bool EndUpdateUserAccount(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteUserAccount(int AccountID, AsyncCallback callback, object state);
        bool EndDeleteUserAccount(IAsyncResult asyncResult);

        #endregion

        #region Group
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllGroupByGroupID(long GroupID, AsyncCallback callback, object state);
        List<Group> EndGetAllGroupByGroupID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllGroupAllPaging(long GroupID, int PageSize, int PageIndex, string OrderBy,
                             bool CountTotal, AsyncCallback callback, object state);
        List<Group> EndGetAllGroupAllPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewGroup(string GroupName, string Description, AsyncCallback callback, object state);
        bool EndAddNewGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateGroup(int GroupID, string GroupName, string Description, AsyncCallback callback, object state);
        bool EndUpdateGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteGroup(int GroupID, AsyncCallback callback, object state);
        bool EndDeleteGroup(IAsyncResult asyncResult);
        #endregion
        #region User Group
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllUserGroupGetByID(long AccountID, long GroupID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal,  AsyncCallback callback, object state);
        List<UserGroup> EndGetAllUserGroupGetByID(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewUserGroup(long AccountID, int GroupID, AsyncCallback callback, object state);
        bool EndAddNewUserGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewUserGroupXML(IList<UserGroup> lstUserGroup, AsyncCallback callback, object state);
        bool EndAddNewUserGroupXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateUserGroup(long UGID, long AccountID, int GroupID, AsyncCallback callback, object state);
        bool EndUpdateUserGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteUserGroup(int UGID, AsyncCallback callback, object state);
        bool EndDeleteUserGroup(IAsyncResult asyncResult);
        #endregion

        #region Group Role
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllGroupRolesGetByID(long GroupID, long RoleID
                                        , int PageSize, int PageIndex, string OrderBy, bool CountTotal,AsyncCallback callback, object state);
        List<GroupRole> EndGetAllGroupRolesGetByID(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewGroupRoles(int GroupID, int RoleID, AsyncCallback callback, object state);
        bool EndAddNewGroupRoles(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewGroupRolesXML(IList<GroupRole> lstGroupRole, AsyncCallback callback, object state);
        bool EndAddNewGroupRolesXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateGroupRoles(long GroupRoleID, int GroupID, int RoleID, AsyncCallback callback, object state);
        bool EndUpdateGroupRoles(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteGroupRoles(long GroupRoleID, AsyncCallback callback, object state);
        bool EndDeleteGroupRoles(IAsyncResult asyncResult);
        #endregion

        #region Permission
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllPermissions_GetByID(long RoleID, long OperationID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<Permission> EndGetAllPermissions_GetByID(out int Total, IAsyncResult asyncResult);

        #endregion

        #region Modules
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllModules(AsyncCallback callback, object state);
        List<Module> EndGetAllModules(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewModules(string ModuleName, int eNum, string Description, int Idx, AsyncCallback callback, object state);
        bool EndAddNewModules(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateModules(long ModuleID, int eNum, string ModuleName, string Description, int Idx, AsyncCallback callback, object state);
        bool EndUpdateModules(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateModulesEnum(long ModuleID, int eNum, AsyncCallback callback, object state);
        bool EndUpdateModulesEnum(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteModules(long ModuleID, AsyncCallback callback, object state);
        bool EndDeleteModules(IAsyncResult asyncResult);

        #endregion

        #region Functions
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllFunction(long ModuleID, AsyncCallback callback, object state);
        List<Function> EndGetAllFunction(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllFunctionsPaging(long ModuleID, int PageSize, int PageIndex, string OrderBy,
                                                             bool CountTotal, AsyncCallback callback, object state);
        List<Function> EndGetAllFunctionsPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewFunctions(int ModuleID, int eNum, string FunctionName, string FunctionDescription, int Idx, AsyncCallback callback, object state);
        bool EndAddNewFunctions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateFunctions(long FunctionID, int eNum, int ModuleID, string FunctionName, string FunctionDescription, byte Idx, AsyncCallback callback, object state);
        bool EndUpdateFunctions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateFunctionsEnum(long FunctionID, int eNum, AsyncCallback callback, object state);
        bool EndUpdateFunctionsEnum(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteFunctions(long FunctionID, AsyncCallback callback, object state);
        bool EndDeleteFunctions(IAsyncResult asyncResult);


        #endregion

        #region Operations
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllOperations(long OperationID, AsyncCallback callback, object state);
        List<Operation> EndGetAllOperations(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllOperationsByFuncID(long FunctionID, AsyncCallback callback, object state);
        List<Operation> EndGetAllOperationsByFuncID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllOperationsByFuncIDPaging(long FunctionID, int PageSize, int PageIndex
                                            , string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<Operation> EndGetAllOperationsByFuncIDPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewOperations(long FunctionID, string OperationName, string Description, AsyncCallback callback, object state);
        bool EndAddNewOperations(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateOperations(long OperationID, int Enum, string OperationName, string Description, AsyncCallback callback, object state);
        bool EndUpdateOperations(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteOperations(long OperationID, AsyncCallback callback, object state);
        bool EndDeleteOperations(IAsyncResult asyncResult);

        #endregion

        #region Permission
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllPermissions(long PermissionItemID, AsyncCallback callback, object state);
        List<Permission> EndGetAllPermissions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewPermissions(int RoleID, long OperationID, bool FullControl
                                                , bool pView
                                                , bool pAdd
                                                , bool pUpdate
                                                , bool pDelete
                                                , bool pReport
                                                , bool pPrint, AsyncCallback callback, object state);
        bool EndAddNewPermissions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePermissions(long PermissionItemID, int RoleID, long OperationID, AsyncCallback callback, object state);
        bool EndUpdatePermissions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePermissionsFull(long PermissionItemID, bool FullControl
                                                , bool pView
                                                , bool pAdd
                                                , bool pUpdate
                                                , bool pDelete
                                                , bool pReport
                                                , bool pPrint, AsyncCallback callback, object state);
        bool EndUpdatePermissionsFull(IAsyncResult asyncResult);
       

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePermissions(long PermissionItemID, AsyncCallback callback, object state);
        bool EndDeletePermissions(IAsyncResult asyncResult);


        #endregion

        #region Role
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllRoles(AsyncCallback callback, object state);
        List<Role> EndGetAllRoles(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllRolesAllPaging(long RoleID, int PageSize, int PageIndex, string OrderBy,
                             bool CountTotal, AsyncCallback callback, object state);
        List<Role> EndGetAllRolesAllPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewRoles(string RoleName, string Description, AsyncCallback callback, object state);
        bool EndAddNewRoles(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRoles(int RoleID, string RoleName, string Description, AsyncCallback callback, object state);
        bool EndUpdateRoles(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRoles(int RoleID, AsyncCallback callback, object state);
        bool EndDeleteRoles(IAsyncResult asyncResult);

        #endregion

        #region modules tree
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetModulesTreeView(AsyncCallback callback, object state);
        List<ModulesTree> EndGetModulesTreeView(IAsyncResult asyncResult);

        #endregion
        #region User Login

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetUserLogHisGByUserAccount(AsyncCallback callback, object state);
        List<UserLoginHistory> EndGetUserLogHisGByUserAccount(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetUserLogHisGByHostName(AsyncCallback callback, object state);
        List<UserLoginHistory> EndGetUserLogHisGByHostName(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllUserLoginHistoryPaging(UserLoginHistory ulh, int PageSize, int PageIndex, string OrderBy,
                                                  bool CountTotal, AsyncCallback callback, object state);
        List<UserLoginHistory> EndGetAllUserLoginHistoryPaging(out int Total, IAsyncResult asyncResult);
        
        
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddUserLoginHistory(UserLoginHistory Ulh,AsyncCallback callback, object state);
        bool EndAddUserLoginHistory(IAsyncResult asyncResult);
        
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateUserLoginHistory(UserLoginHistory Ulh, AsyncCallback callback, object state);
        bool EndUpdateUserLoginHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteUserLoginHistory(long LoggedHistoryID, AsyncCallback callback, object state);
        bool EndDeleteUserLoginHistory(IAsyncResult asyncResult);
        #endregion

        #region staff
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetStaffByID(long StaffID, AsyncCallback callback, object state);
        Staff EndGetStaffByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllStaffAllPaging(Staff searchStaff, int PageSize, int PageIndex, string OrderBy,
                                                         bool CountTotal, AsyncCallback callback, object state);
        List<Staff> EndGetAllStaffAllPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewStaff(Staff newStaff, AsyncCallback callback, object state);
        bool EndAddNewStaff(IAsyncResult asyncResult);

        //▼====: #001
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateStaff(Staff newStaff, long? StaffEditID, AsyncCallback callback, object state);
        bool EndUpdateStaff(IAsyncResult asyncResult);
        //▲====: #001

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteStaff(int StaffID, AsyncCallback callback, object state);
        bool EndDeleteStaff(IAsyncResult asyncResult);

        #endregion

        #region StaffDeptResponsibility
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetStaffDeptResponsibilitiesByDeptID(StaffDeptResponsibilities p, bool isHis, AsyncCallback callback, object state);
        List<StaffDeptResponsibilities> EndGetStaffDeptResponsibilitiesByDeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertStaffDeptResponsibilitiesXML(List<StaffDeptResponsibilities> lstStaffDeptRes, AsyncCallback callback, object state);
        bool EndInsertStaffDeptResponsibilitiesXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes, AsyncCallback callback, object state);
        bool EndUpdateStaffDeptResponsibilitiesXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes, AsyncCallback callback, object state);
        bool EndDeleteStaffDeptResponsibilitiesXML(IAsyncResult asyncResult);

        #endregion

        #region StaffStoreDeptResponsibilities
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetStaffStoreDeptResponsibilitiesByDeptID(StaffStoreDeptResponsibilities p, bool isHis, AsyncCallback callback, object state);
        List<StaffStoreDeptResponsibilities> EndGetStaffStoreDeptResponsibilitiesByDeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertStaffStoreDeptResponsibilitiesXML(List<StaffStoreDeptResponsibilities> lstStaffDeptRes, AsyncCallback callback, object state);
        bool EndInsertStaffStoreDeptResponsibilitiesXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes, AsyncCallback callback, object state);
        bool EndUpdateStaffStoreDeptResponsibilitiesXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes, AsyncCallback callback, object state);
        bool EndDeleteStaffStoreDeptResponsibilitiesXML(IAsyncResult asyncResult);

        #endregion

       
    }
}


