using System.Collections.Generic;
using System.ServiceModel;

namespace EventManagementService
{
    [ServiceContract(CallbackContract = typeof(IEventManagerCallback), SessionMode = SessionMode.Required)]
    public interface IEventManagement
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        bool Subscribe();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        bool SubscribeEvents(List<AxEvent> evtList);


        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void Unsubscribe();

        [OperationContract(IsOneWay = true)]
        void Notify(AxEvent evt);

        [OperationContract(IsOneWay = false)]
        void KeepConnection();
    }
    public interface IEventManagerCallback
    {
        [OperationContract(IsOneWay = true)]
        void Receive(AxEvent evt);
    }
}
