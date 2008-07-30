using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace SoftwareFg.Framework.Serialization
{
    /// <summary>
    /// This Attribute indicates that the NetDataContractSerializer should be used when using WCF instead of the
    /// default DataContractSerializer class.
    /// Specify this Attribute on the methods of the OperationContract for which you want to use the NetDataContractSerializer.
    /// Note that the NetDataContractSerializer will include CLR type information in the serialized data; this means that both
    /// server and client need to be .NET applications.  In other words: using this Serializer means that your server app will not
    /// be interoperable with non-.NET applications.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UseNetDataContractSerializerAttribute : Attribute, IOperationBehavior
    {
        
        public void AddBindingParameters( OperationDescription description, BindingParameterCollection parameters )
        {
        }

        public void ApplyClientBehavior( OperationDescription description,
                                        System.ServiceModel.Dispatcher.ClientOperation proxy )
        {
            ReplaceDataContractSerializerOperationBehavior (description);
        }

        public void ApplyDispatchBehavior( OperationDescription description,
                                          System.ServiceModel.Dispatcher.DispatchOperation dispatch )
        {
            ReplaceDataContractSerializerOperationBehavior (description);
        }

        public void Validate( OperationDescription description )
        {
        }

        private static void ReplaceDataContractSerializerOperationBehavior( OperationDescription description )
        {
            DataContractSerializerOperationBehavior dcsOperationBehavior =
				description.Behaviors.Find<DataContractSerializerOperationBehavior> ();

            if( dcsOperationBehavior != null )
            {
                description.Behaviors.Remove (dcsOperationBehavior);
                description.Behaviors.Add (new NetDataContractOperationBehavior (description));
            }
        }
        
    }
}
