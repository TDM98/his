using System;
using System.Collections.Generic;
using DataEntities;
using eHCMSLanguage;



namespace SessionManagementService
{
    public class AxSession
    {
        private UserAccount _UserIdentity;
        public UserAccount UserIdentity
        {
            get { return _UserIdentity; }
            set { _UserIdentity = value; }
        }
        public AxSession(UserAccount account)
        {
            if(account == null)
            {
                throw new Exception(string.Format("{0}.",eHCMSResources.Z1781_G1_NullValueIsUnaceptable));
            }
            _UserIdentity = account;
        }
        public decimal SessionID
        {
            get
            {
                return _UserIdentity.AccountID;
            }
        }
    }
    public class SessionManager
    {
        public SessionManager()
        {
            _Sessions = new Dictionary<decimal, AxSession>();
        }
        public static SessionManager Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly SessionManager instance = new SessionManager();
        }

        Dictionary<decimal, AxSession> _Sessions;
        public Dictionary<decimal, AxSession> Sessions
        {
            get
            {
                return _Sessions;
            }
            set
            {
                _Sessions = value;
            }
        }

        public bool AddNewSession(AxSession session)
        {
            if (CheckIfSessionExist(session))
            {
                return false;
            }
            _Sessions[session.SessionID] = session;
            return true;
        }
        public bool RemoveSession(AxSession session)
        {
            _Sessions.Remove(session.SessionID);
            return true;
        }
        public bool CheckIfSessionExist(AxSession session)
        {
            if(_Sessions == null)
            {
                return false;
            }
            return _Sessions.ContainsKey(session.SessionID);
        }
    }
}