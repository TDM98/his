using System.Collections.Generic;
using System.ServiceModel;
using DataEntities;

namespace UserManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IUserManagementService
    {
        [OperationContract]
        UserAccount GetUserByID(long userID);

        [OperationContract]
        string GetCustomerIP();
        

        [OperationContract]
        UserAccount AuthenticateUser(string userName, string password, string hostname, bool IsConfirmForSecretary, out List<refModule> lstModules, out bool? IsOutOfSegments);

        [OperationContract]
        UserAccount AuthorizationUser(long AccountAuthoID, long AccountSubID, string password, out List<refModule> lstModules);

        [OperationContract]
        UserAccount AuthenticateUserEx(string userName, string password,string HostName,string IPAdress, out List<refModule> lstModules);
        
        [OperationContract]
        UserAccount GetUserWithGroupInfo(long userID);


        [OperationContract]
        UserAccount GetUserByName(string userName);

        [OperationContract]
        bool DeleteUser(UserAccount info);

        [OperationContract]
        bool DeleteUserByID(long userID);

        [OperationContract]
        IList<UserAccount> GetAllUsers();

        [OperationContract]
        IList<UserAccount> GetUsers(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        IList<UserAccount> SearchUsers(UserSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        void AddNewUser(UserAccount newUser, out long userID);

        [OperationContract]
        bool CheckIfUserExists(string userName);

        [OperationContract]
        void AssignUserToGroups(long userID, List<int> groupIDs);

#region Manage UserGroups
        [OperationContract]
        Group GetGroupByID(int groupID);

        [OperationContract]
        Group GetGroupWithRoleInfo(int groupID);

        [OperationContract]
        Group GetGroupByName(string groupName);

        [OperationContract]
        bool DeleteGroup(Group info);

        [OperationContract]
        bool DeleteGroupByID(int groupID);

        [OperationContract]
        IList<Group> GetAllGroups();

        [OperationContract]
        IList<Group> GetGroups(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        IList<Group> SearchGroups(UserGroupSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        void AddNewGroup(Group newGroup, out int groupID);

        [OperationContract]
        bool CheckIfGroupExists(string groupName);

        [OperationContract]
        void AssignGroupToRoles(int groupID, List<int> roleIDs);
#endregion

#region Manage Roles
        [OperationContract]
        Role GetRoleByID(int roleID);

        [OperationContract]
        Role GetRoleByName(string roleName);

        [OperationContract]
        bool DeleteRole(Role info);

        [OperationContract]
        bool DeleteRoleByID(int groupID);

        [OperationContract]
        IList<Role> GetAllRoles();

        [OperationContract]
        void AddNewRole(Role newRole, out int groupID);

        [OperationContract]
        bool CheckIfRoleExists(string roleName);

        [OperationContract]
        Role GetRoleWithOperationInfo(int roleID);

        [OperationContract]
        void AssignRoleToOperations(int roleID, List<int> operationIDs);
#endregion
#region Manage Modules
        [OperationContract]
        IList<Module> GetAllModules();
#endregion

#region Manage Operations
        [OperationContract]
        IList<Operation> GetAllOperations();
#endregion
#region Manage Staffs
        [OperationContract]
        IList<Staff> SearchStaffs(StaffSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

#endregion
        #region UserLoginHistory
        

        [OperationContract]
        long AddUserLoginHistory(UserLoginHistory Ulh);

        [OperationContract]
        bool UpdateUserLoginHistory(UserLoginHistory Ulh);

        [OperationContract]
        bool DeleteUserLoginHistory(long LoggedHistoryID);

        #endregion

        #region UserSubAuthorization
        [OperationContract]
        List<UserSubAuthorization> GetUserSubAuthorizationPaging(UserSubAuthorization Usa
            , int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        //[OperationContract]
        //UserSubAuthorization GetUserSubAuthorization(UserSubAuthorization Usa);

        [OperationContract]
        bool AddUserSubAuthorization(UserSubAuthorization Usa);

        [OperationContract]
        bool UpdateUserSubAuthorization(UserSubAuthorization Usa);

        [OperationContract]
        bool DeleteUserSubAuthorization(long SubUserAuthorizationID);

        #endregion
        #region UserOfficial
        [OperationContract]
        long AddUserOfficialHistory(UserOfficialHistory Usa);
        [OperationContract]
        List<UserOfficialHistory> GetUserOfficialHistoryPaging(long LoggedStaffID
            , int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        [OperationContract]
        long AddManagementUserOfficial(ManagementUserOfficial Usa);
        [OperationContract]
        List<ManagementUserOfficial> GetManagementUserOfficialPaging(long LoginUserID, long UserOfficialID
            , int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        [OperationContract]
        bool DeleteManagementUserOfficial(long ManagementUserOfficialID, long StaffID);
        #endregion
    }
}
