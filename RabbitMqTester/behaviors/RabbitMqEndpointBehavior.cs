using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using Newtonsoft.Json;
using WcfJsonFormatter;
using WcfJsonFormatter.Configuration;
using WcfJsonFormatter.Ns;

namespace RabbitMqTester.behaviors
{
    public class RabbitMqEndpointBehavior
        : IEndpointBehavior
    {
        public RabbitMqEndpointBehavior(IEnumerable<Type> knownTypes)
        {
            this.ConfigRegister = ConfigurationManager.GetSection("serviceTypeRegister") as ServiceTypeRegister
                            ?? new ServiceTypeRegister();

            SerializerSettings serializerInfo = this.ConfigRegister.SerializerConfig;

            CustomContractResolver resolver = new CustomContractResolver(true, false, this.ConfigRegister.TryToNormalize)
            {
                DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            };

            this.Serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                ContractResolver = resolver
            };

            if (!serializerInfo.OnlyPublicConstructor)
                Serializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;

            if (serializerInfo.EnablePolymorphicMembers)
            {
                Serializer.Binder = new OperationTypeBinder(this.ConfigRegister);
                Serializer.TypeNameHandling = TypeNameHandling.Objects;
            }
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //endpoint.Behaviors.Add();
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                DispatchOperation dop = null;
                if (endpointDispatcher.DispatchRuntime.Operations.Contains(od.Name))
                {
                    dop = endpointDispatcher.DispatchRuntime.Operations[od.Name];
                }
                else if (endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.Name == od.Name)
                {
                    dop = endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation;
                }

                if (dop == null)
                    continue;

                dop.Formatter = new DispatchJsonNetMessageFormatter(od, this.Serializer, this.ConfigRegister);
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                if (clientRuntime.Operations.Contains(od.Name))
                {
                    ClientOperation cop = clientRuntime.Operations[od.Name];
                    
                    cop.Formatter = new ClientJsonNetMessageFormatter(od, endpoint, this.Serializer, this.ConfigRegister);
                    cop.SerializeRequest = true;
                }
            }
        }

        public JsonSerializer Serializer { get; private set; }

        public IServiceRegister ConfigRegister { get; private set; }
    }
}
