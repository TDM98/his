using System;
using System.ServiceModel.Activation;
using DataEntities;



namespace SessionManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UserManagementService : eHCMS.WCFServiceCustomHeader, ISessionManager
    {
        public UserManagementService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;

        }


        #region ISessionManager Members

        public UserAccount GetUserByID(decimal userID)
        {
            throw new NotImplementedException();
        }

        public UserAccount AuthenticateUser(string userName, string password)
        {
            throw new NotImplementedException();
        }

        #endregion

        
    }
}