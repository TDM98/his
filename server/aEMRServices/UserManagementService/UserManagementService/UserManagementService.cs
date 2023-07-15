using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using DataEntities;
using eHCMS;

using AxLogging;
using aEMR.DataAccessLayer.Providers;
using ErrorLibrary;
using eHCMS.Configurations;

namespace UserManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UserManagementService : eHCMS.WCFServiceCustomHeader, IUserManagementService, IClientAccessPolicy
    {
        public UserManagementService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            
        }

        #region IUserManagementService Members
        public string GetCustomerIP()
        {
            return "";
        }

        public UserAccount AuthenticateUser(string userName, string password,string hostname, bool IsConfirmForSecretary, out List<refModule> lstModules, out bool? IsOutOfSegments)
        {
            AxLogger.Instance.LogInfo("AuthenticateUser - Testing ONLY: ===> ", CurrentUser);
            UserAccount ua = new UserAccount();
            ua=UserProvider.Instance.AuthenticateUser(userName, password);
            if (ua != null)
            {
                IsOutOfSegments = true;
                //Kiem tra quyen cua nguoi dang nhap
                lstModules = new List<refModule>();
                lstModules = UserAccounts.Instance.GetAllUserGroupByAccountID(ua.AccountID);
                ua.ConsultationTimeSegmentsList = UserAccounts.Instance.GetAllConsultationTimeSegmentsTodayByAccountID(ua.Staff.StaffID, out IsOutOfSegments);
                if (!Globals.AxServerSettings.CommonItems.IsApplyTimeSegments)
                {
                    IsOutOfSegments = false;
                }
                //▼===== 20191011 TTM: Bổ sung thêm điều kiện nếu là xác nhận đăng nhập thư ký y khoa của
                //                     Ra toa thì không cần ghi lích sử login
                if (!IsConfirmForSecretary)
                {
                    //Ghi vao log file
                    OperationContext context = OperationContext.Current;
                    MessageProperties properties = context.IncomingMessageProperties;
                    RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as
                                                                    RemoteEndpointMessageProperty;
                    string IpAddress = "";
                    string hostName = "";

                 
                    IPAddress ipAddress = IPAddress.Parse(endpoint.Address);
                    try
                    {
                        IPHostEntry ipHostEntry = Dns.GetHostEntry(ipAddress);

                        hostName = ipHostEntry.HostName;
                        foreach (IPAddress address in ipHostEntry.AddressList)
                        {
                            if (address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                IpAddress = address.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AxLogger.Instance.LogError(ex);
                        IpAddress = ipAddress.ToString();
                    }
                    //insert vao table
                    UserLoginHistory Ulh = new UserLoginHistory();
                    Ulh.LogDateTime = DateTime.Now;
                    Ulh.HostIPV4 = IpAddress;
                    Ulh.HostName = hostname;
                    Ulh.AccountID = ua.AccountID;
                    ua.LoggedHistoryID = AddUserLoginHistory(Ulh);

                    //-------------------------------------------
                }
                //▲===== 
                return ua;
            }
            IsOutOfSegments = null;
            lstModules = null;
            return null;            
        }

        public UserAccount AuthorizationUser(long AccountAuthoID,long AccountSubID, string password, out List<refModule> lstModules)
        {
            var res = UserProvider.Instance.GetUserSubAuthorization(new UserSubAuthorization
            {
                AccountIDAuth= AccountAuthoID,AccountIDSub= AccountSubID, AuthPwd=password});
            if(res!=null )
            {
                lstModules = new List<refModule>();
                lstModules = UserAccounts.Instance.GetAllUserGroupByAccountID(AccountAuthoID);

                return res.AccountAuth;
            }
                
            lstModules = null;
            return null;
        }

        public UserAccount AuthenticateUserEx(string userName, string password,string HostName,string IPAdress ,out List<refModule> lstModules)
        {
            UserAccount ua = new UserAccount();
            ua = UserProvider.Instance.AuthenticateUser(userName, password);
            if (ua != null)
            {
                //Kiem tra quyen cua nguoi dang nhap
                lstModules = new List<refModule>();
                lstModules = UserAccounts.Instance.GetAllUserGroupByAccountID(ua.AccountID);
                //Ghi vao log file
                //-------------------------------------------
                return ua;
            }
            lstModules = null;
            return null;
        }

        public UserAccount GetUserByID(long userID)
        {
            throw new NotImplementedException();
        }
        public UserAccount GetUserWithGroupInfo(long userID)
        {
            return UserProvider.Instance.GetUserWithGroupInfo(userID);
        }

        public UserAccount GetUserByName(string userName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(UserAccount info)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUserByID(long userID)
        {
            return UserProvider.Instance.DeleteUserByID(userID);
        }

        public IList<UserAccount> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public IList<UserAccount> GetUsers(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            throw new NotImplementedException();
        }

        public IList<UserAccount> SearchUsers(UserSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            return UserProvider.Instance.SearchUsers(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
        }

        public void AddNewUser(UserAccount newUser, out long userID)
        {
            UserProvider.Instance.AddUser(newUser, out userID);
        }

        public bool CheckIfUserExists(string userName)
        {
            return UserProvider.Instance.CheckIfUserNameExists(userName);
        }

        public void AssignUserToGroups(long userID, List<int> groupIDs)
        {
            UserProvider.Instance.AssignUserToGroups(userID, groupIDs);
        }
        #endregion

        #region IUserManagementService Members


        public Group GetGroupByID(int groupID)
        {
            throw new NotImplementedException();
        }

        public Group GetGroupByName(string groupName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteGroup(Group info)
        {
            throw new NotImplementedException();
        }

        public bool DeleteGroupByID(int groupID)
        {
            return UserProvider.Instance.DeleteGroupByID(groupID);
        }

        public IList<Group> GetAllGroups()
        {
            return UserProvider.Instance.GetAllGroups();
        }

        public IList<Group> GetGroups(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            throw new NotImplementedException();
        }

        public IList<Group> SearchGroups(UserGroupSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            throw new NotImplementedException();
        }

        public void AddNewGroup(Group newGroup, out int groupID)
        {
            UserProvider.Instance.AddGroup(newGroup, out groupID);
        }

        public bool CheckIfGroupExists(string groupName)
        {
            return UserProvider.Instance.CheckIfGroupNameExists(groupName);
        }

        #endregion

        #region Manage Roles


        public Role GetRoleByID(int roleID)
        {
            throw new NotImplementedException();
        }

        public Role GetRoleByName(string roleName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRole(Role info)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRoleByID(int roleID)
        {
            return UserProvider.Instance.DeleteRoleByID(roleID);
        }

        public IList<Role> GetAllRoles()
        {
            return UserProvider.Instance.GetAllRoles();
        }

        public void AddNewRole(Role newRole, out int roleID)
        {
            UserProvider.Instance.AddRole(newRole, out roleID);
        }

        public bool CheckIfRoleExists(string roleName)
        {
            return UserProvider.Instance.CheckIfRoleNameExists(roleName);
        }

        public Role GetRoleWithOperationInfo(int roleID)
        {
            return UserProvider.Instance.GetRoleWithOperationInfo(roleID);
        }

        public void AssignRoleToOperations(int roleID, List<int> operationIDs)
        {
            UserProvider.Instance.AssignRoleToOperations(roleID, operationIDs);
        }

        #endregion

        #region IUserManagementService Members


        public Group GetGroupWithRoleInfo(int groupID)
        {
            return UserProvider.Instance.GetGroupWithRoleInfo(groupID);
        }

        public void AssignGroupToRoles(int groupID, List<int> roleIDs)
        {
            UserProvider.Instance.AssignGroupToRoles(groupID, roleIDs);
        }

        #endregion

        #region Manage Modules
        
        public IList<Module> GetAllModules()
        {
            return UserProvider.Instance.GetAllModules();
        }
        #endregion

        #region Manage Operations

        public IList<Operation> GetAllOperations()
        {
            return UserProvider.Instance.GetAllOperations();
        }
        #endregion
        #region Manage Staffs
                public IList<Staff> SearchStaffs(StaffSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
                {
                    return UserProvider.Instance.SearchStaffs(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                }
        #endregion
        #region UserLoginHistory

        public long AddUserLoginHistory(UserLoginHistory Ulh)
        {
            return UserProvider.Instance.AddUserLoginHistory(Ulh);
        }

        public bool UpdateUserLoginHistory(UserLoginHistory Ulh)
        {
            return UserProvider.Instance.UpdateUserLoginHistory(Ulh);
        }

        public bool DeleteUserLoginHistory(long LoggedHistoryID)
        {
            return UserProvider.Instance.DeleteUserLoginHistory(LoggedHistoryID);
        }

        #endregion
        #region IClientAccessPolicy Members

        public Stream GetClientAccessPolicy()
        {
            string str = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                <access-policy>
                                  <cross-domain-access>
                                    <policy>
                                      <allow-from http-request-headers=""*"">
                                        <domain uri=""*""/>
                                      </allow-from>
                                      <grant-to>
                                        <resource path=""/"" include-subpaths=""true""/>
                                      </grant-to>
                                    </policy>
                                  </cross-domain-access>
                                </access-policy>";
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        #endregion

        #region UserSubAuthorization
        public List<UserSubAuthorization> GetUserSubAuthorizationPaging(UserSubAuthorization Usa
            , int pageIndex, int pageSize, bool bCountTotal, out int totalCount) 
        {
            totalCount = 0;
            try
            {
                return UserProvider.Instance.GetUserSubAuthorizationPaging(Usa,pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return null;
            }
        }

        public bool AddUserSubAuthorization(UserSubAuthorization Usa)
        {            
            try
            {
                return UserProvider.Instance.AddUserSubAuthorization(Usa);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return false;
            }
        }

        public bool UpdateUserSubAuthorization(UserSubAuthorization Usa)
        {
            try
            {
                return UserProvider.Instance.UpdateUserSubAuthorization(Usa);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return false;
            }
        }

        public bool DeleteUserSubAuthorization(long SubUserAuthorizationID)
        {
            try
            {
                return UserProvider.Instance.DeleteUserSubAuthorization(SubUserAuthorizationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return false;
            }
        }

        #endregion
        #region User account official
        public long AddUserOfficialHistory(UserOfficialHistory Usa)
        {
            try
            {
                return UserProvider.Instance.AddUserOfficialHistory(Usa);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return 0;
            }
        }

        public List<UserOfficialHistory> GetUserOfficialHistoryPaging(long LoggedStaffID
            , int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            try
            {
                return UserProvider.Instance.GetUserOfficialHistoryPaging(LoggedStaffID, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return null;
            }
        }

        public long AddManagementUserOfficial(ManagementUserOfficial Usa)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of AddManagementUserOfficial.", CurrentUser);
                return UserProvider.Instance.AddManagementUserOfficial(Usa);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddManagementUserOfficial. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "AddManagementUserOfficial");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteManagementUserOfficial(long ManagementUserOfficialID, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of DeleteManagementUserOfficial.", CurrentUser);
                return UserProvider.Instance.DeleteManagementUserOfficial(ManagementUserOfficialID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteManagementUserOfficial. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "DeleteManagementUserOfficial");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<ManagementUserOfficial> GetManagementUserOfficialPaging(long LoginUserID, long UserOfficialID
            , int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            try
            {
                return UserProvider.Instance.GetManagementUserOfficialPaging(LoginUserID, UserOfficialID, pageIndex, pageSize, bCountTotal
                    , out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return null;
            }
        }
        #endregion
    }
}
