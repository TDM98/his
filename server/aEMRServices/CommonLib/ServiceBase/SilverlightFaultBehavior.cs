using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WCFServiceBase
{
    public class SilverlightFaultBehavior : BehaviorExtensionElement, IEndpointBehavior
    {

        private static bool? _testBusyIndicatorEnabled;

        protected static bool TestBusyIndicatorEnabled
        {
            get
            {
                if (_testBusyIndicatorEnabled == null)
                {
                    string keyStrVal = ConfigurationManager.AppSettings["TestBusyIndicatorEnabled"] as string;
                    if (!string.IsNullOrEmpty(keyStrVal))
                    {
                        _testBusyIndicatorEnabled = keyStrVal.ToLower() == "true";
                    }
                    else
                    {
                        //Gán giá trị mặc định.
                        _testBusyIndicatorEnabled = true;
                    }
                }

                return _testBusyIndicatorEnabled.Value;
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            SilverlightFaultMessageInspector inspector = new SilverlightFaultMessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }
        public class SilverlightFaultMessageInspector : IDispatchMessageInspector
        {
            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                if (reply.IsFault)
                {
                    HttpResponseMessageProperty property = new HttpResponseMessageProperty();

                    // Here the response code is changed to 200.
                    property.StatusCode = System.Net.HttpStatusCode.OK;

                    reply.Properties[HttpResponseMessageProperty.Name] = property;
                }
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
#if DEBUG
                if(TestBusyIndicatorEnabled)
                {
                    System.Threading.Thread.Sleep(1000);  
                }
#endif
                
                // Do nothing to the incoming message.
                return null;
            }
        }

        // The following methods are stubs and not relevant. 
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public override System.Type BehaviorType
        {
            get { return typeof(SilverlightFaultBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SilverlightFaultBehavior();
        }

    }


    //public class SilverlightFaultBehavior:BehaviorExtensionElement,IEndpointBehavior
    //{
    //    public override Type BehaviorType
    //    {
    //        get
    //        {
    //            return typeof(SilverlightFaultBehavior);
    //        }
    //    }

    //    protected override object CreateBehavior()
    //    {
    //        return new SilverlightFaultBehavior();
    //    }

    //    #region IEndpointBehavior Members

    //    public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
    //    {
            
    //    }

    //    public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
    //    {
            
    //    }

    //    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
    //    {
    //        SilverlightFaultMessageInspector inspector = new SilverlightFaultMessageInspector();
    //        endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
    //    }

    //    public void Validate(ServiceEndpoint endpoint)
    //    {
            
    //    }

    //    #endregion

    //    public class SilverlightFaultMessageInspector:IDispatchMessageInspector
    //    {

    //        #region IDispatchMessageInspector Members

    //        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
    //        {
    //            return null;
    //        }

    //        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
    //        {
    //            if(reply.IsFault)
    //            {
    //                HttpResponseMessageProperty prop = new HttpResponseMessageProperty();
    //                //Chuyển response code thành 200 => Cho phép client (browser) truy xuất được tới SOAP fault.
    //                prop.StatusCode = System.Net.HttpStatusCode.OK;
    //                reply.Properties[HttpResponseMessageProperty.Name] = prop;
    //            }
    //        }

    //        #endregion
    //    }
    //}
}
