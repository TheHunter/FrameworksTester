using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ElasticSearch.Linq.Test
{
    public class VeryElasticContractResolver
        : DefaultContractResolver
    {
        private static readonly Type DynamicType;
        private readonly List<MemberInfo> members;

        /// <summary>
        /// Initializes static members of the <see cref="VeryElasticContractResolver"/> class. 
        /// </summary>
        static VeryElasticContractResolver()
        {
            DynamicType = typeof(IDynamicMetaObjectProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VeryElasticContractResolver"/> class.
        /// </summary>
        public VeryElasticContractResolver()
        {
            this.members = new List<MemberInfo>();
        }

        /// <summary>
        /// Determines which contract type is created for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// A <see cref="T:Newtonsoft.Json.Serialization.JsonContract" /> for the given type.
        /// </returns>
        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (!DynamicType.IsAssignableFrom(objectType))
                return contract;

            dynamic ctr = contract;

            if (contract is JsonDynamicContract)
            {
                foreach (var prop in ctr.Properties)
                {
                    prop.HasMemberAttribute = true;
                }
            }
            else if (contract is JsonDictionaryContract)
            {
                Func<string, string> func = this.ResolvePropertyName;
                ctr.PropertyNameResolver = func;
            }

            return contract;
        }

        /// <summary>
        /// Gets the serializable members for the type.
        /// </summary>
        /// <param name="objectType">The type to get serializable members for.</param>
        /// <returns>
        /// The serializable members for the type.
        /// </returns>
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var origMembers = base.GetSerializableMembers(objectType);
            this.members.AddRange(origMembers);
            return origMembers;
        }

        /// <summary>
        /// Resolves the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// Resolved name of the property.
        /// </returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            if (this.members.Any(info => info.Name.Equals(propertyName)))
                return base.ResolvePropertyName(propertyName);

            return propertyName;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.
        /// </summary>
        /// <param name="member">The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.</param>
        /// <param name="memberSerialization">The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.</param>
        /// <returns>
        /// A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.
        /// </returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (!(member is PropertyInfo))
                return property;

            dynamic prp = member;
            property.Writable = prp.GetSetMethod(true) != null;
            property.Readable = prp.GetGetMethod(true) != null;

            return property;
        }
    }
}
