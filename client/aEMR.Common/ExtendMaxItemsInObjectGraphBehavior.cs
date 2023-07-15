using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;

namespace aEMR.Common
{
    public class ExtendMaxItemsInObjectGraphBehavior : IEndpointBehavior
    {

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            ModifyDataContractSerializerBehavior(endpoint);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            ModifyDataContractSerializerBehavior(endpoint);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        private static void ModifyDataContractSerializerBehavior(ServiceEndpoint endpoint)
        {
            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                var behavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (behavior != null)
                {
                    behavior.MaxItemsInObjectGraph = int.MaxValue;
                }
            }

        }
    }
}
