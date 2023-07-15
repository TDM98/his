using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace WCFServiceBase
{
    public class PreserveDataContractReferenceSerializerBehavior : DataContractSerializerOperationBehavior
    {
        public PreserveDataContractReferenceSerializerBehavior(OperationDescription operation):base(operation)
        {}
        public PreserveDataContractReferenceSerializerBehavior(OperationDescription operation, DataContractFormatAttribute dataContractFormatAttribute)
            : base(operation,dataContractFormatAttribute)
        { }
        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<System.Type> knownTypes)
        {
            return base.CreateSerializer(type, name, ns, knownTypes);
        }
        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<System.Type> knownTypes)
        {
            //return base.CreateSerializer(type, name, ns, knownTypes);
            return new DataContractSerializer(type, name, ns, knownTypes,
                int.MaxValue /*maxItemsInObjectGraph*/,
                false/*ignoreExtensionDataObject*/,
                true/*preserveObjectReferences*/,
                null/*dataContractSurrogate*/);
        }
    }

    public class PreserveDataContractReferenceAttribute:Attribute,IOperationBehavior
    {
        public void Validate(OperationDescription operationDescription)
        {
            
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            //IOperationBehavior innerBehavior = new PreserveDataContractReferenceSerializerBehavior(operationDescription);
            //innerBehavior.ApplyDispatchBehavior(operationDescription, dispatchOperation);
            this.ReplaceSerializerOperationBehavior(operationDescription);
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            //IOperationBehavior innerBehavior = new PreserveDataContractReferenceSerializerBehavior(operationDescription);
            //innerBehavior.ApplyClientBehavior(operationDescription, clientOperation);
            this.ReplaceSerializerOperationBehavior(operationDescription);
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
            
        }
        private void ReplaceSerializerOperationBehavior(OperationDescription operationDescription)
        {
            foreach (var od in operationDescription.DeclaringContract.Operations)
            {
                for (int i = 0; i < od.Behaviors.Count; i++)
                {
                    if(od.Behaviors[i] is DataContractSerializerOperationBehavior)
                    {
                        od.Behaviors[i] = new PreserveDataContractReferenceSerializerBehavior(od);
                    }
                }
            }
        }
    }
}
