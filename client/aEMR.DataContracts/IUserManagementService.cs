using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;

namespace UserServiceProxy
{
    [ServiceContract]
    public interface IUserManagementService
    {

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginAuthenticateUser(string userName, string password, ref List<refModule> lstModules, AsyncCallback callback, object state);
        //UserAccount EndAuthenticateUser(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCustomerIP(AsyncCallback callback, object state);
        string EndGetCustomerIP(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAuthenticateUser(string userName, string password,string hostname, bool IsConfirmForSecretary, AsyncCallback callback, object state);
        UserAccount EndAuthenticateUser(out List<refModule> lstModules, out bool? IsOutOfSegments, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAuthorizationUser(long AccountAuthoID, long AccountSubID, string password, AsyncCallback callback, object state);
        UserAccount EndAuthorizationUser(out List<refModule> lstModules, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAuthenticateUserEx(string userName, string password,string HostName,string IPAdress, AsyncCallback callback, object state);
        UserAccount EndAuthenticateUserEx(out List<refModule> lstModules, IAsyncResult asyncResult);

        #region UserSubAuthorization
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetUserSubAuthorizationPaging(UserSubAuthorization Usa
            , int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<UserSubAuthorization> EndGetUserSubAuthorizationPaging(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddUserSubAuthorization(UserSubAuthorization Usa, AsyncCallback callback, object state);
        bool EndAddUserSubAuthorization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateUserSubAuthorization(UserSubAuthorization Usa, AsyncCallback callback, object state);
        bool EndUpdateUserSubAuthorization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteUserSubAuthorization(long SubUserAuthorizationID, AsyncCallback callback, object state);
        bool EndDeleteUserSubAuthorization(IAsyncResult asyncResult);

        #endregion
        #region DoctorUserOfficial
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddUserOfficialHistory(UserOfficialHistory Usa, AsyncCallback callback, object state);
        long EndAddUserOfficialHistory(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetUserOfficialHistoryPaging(long LoggedStaffID
           , int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<UserOfficialHistory> EndGetUserOfficialHistoryPaging(out int totalCount, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddManagementUserOfficial(ManagementUserOfficial Usa, AsyncCallback callback, object state);
        long EndAddManagementUserOfficial(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetManagementUserOfficialPaging(long LoginUserID, long UserOfficialID
            , int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<ManagementUserOfficial> EndGetManagementUserOfficialPaging(out int totalCount, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteManagementUserOfficial(long ManagementUserOfficialID, long StaffID
            , AsyncCallback callback, object state);
        bool EndDeleteManagementUserOfficial(IAsyncResult asyncResult);
        #endregion
    }
}
