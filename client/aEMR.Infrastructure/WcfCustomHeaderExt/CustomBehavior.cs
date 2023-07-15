using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace aEMR.Infrastructure.ServiceCore
{
    public class CustomBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint,
                                            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint,
                                        ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new MyMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
                                            EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(
            ServiceEndpoint endpoint)
        {
        }
        #endregion
    }
}
