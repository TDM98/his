
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel;
using DataEntities;

namespace aEMR.Common
{
    /// <summary>
    /// Use this class to add logged user into the header of a wcf service call.
    /// </summary>
    public class UserLoginInspector:IClientMessageInspector,IEndpointBehavior
    {
        public static UserLoginInspector Instance
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

            internal static readonly UserLoginInspector instance = new UserLoginInspector();
        }

        private UserAccount _user = null;
        public void SetUserInspector(UserAccount u)
        {
            _user = u;
        }
        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            MessageHeader header = MessageHeader.CreateHeader("AuthenticatedUser", "eHCMS", _user);
            foreach (MessageHeader h in request.Headers)
            {
                if(header.Name == h.Name && header.Namespace == h.Namespace)
                {
                    return null;
                }
            }
            request.Headers.Add(header);
            return null;
        }

        #endregion

        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}
