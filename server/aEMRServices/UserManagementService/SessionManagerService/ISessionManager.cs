using System.ServiceModel;
using DataEntities;

namespace SessionManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISessionManager
    {
        [OperationContract]
        UserAccount GetUserByID(decimal userID);

        [OperationContract]
        UserAccount AuthenticateUser(string userName, string password);

    }
}