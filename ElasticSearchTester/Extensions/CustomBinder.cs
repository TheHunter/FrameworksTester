using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ElasticSearchTester.Extensions
{
    public class CustomBinder
        : SerializationBinder
    {
        private Dictionary<string, Type> keyTypes;

        public CustomBinder(IEnumerable<KeyValuePair<string, Type>> binder)
        {
            this.keyTypes = new Dictionary<string, Type>();
            foreach (var keyValuePair in binder)
            {
                this.keyTypes.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            var ret = this.keyTypes[typeName];
            return ret;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.IsGenericType ? serializedType.FullName : serializedType.Name;
            if (!this.keyTypes.ContainsKey(typeName))
                this.keyTypes.Add(typeName, serializedType);
        }
    }
}
