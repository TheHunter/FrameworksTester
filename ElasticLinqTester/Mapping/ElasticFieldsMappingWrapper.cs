using System;
using System.Linq.Expressions;
using System.Reflection;
using ElasticSearch.Linq.Request.Criteria;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Mapping
{
    /// <summary>
    /// Wraps an elastic mapping with one that also handles the built-in
    /// ElasticFields class that contains properties for _score etc.
    /// </summary>
    class ElasticFieldsMappingWrapper : IElasticMapping
    {
        readonly IElasticMapping wrapped;

        /// <inheritdoc/>
        public ElasticFieldsMappingWrapper(IElasticMapping wrapped)
        {
            this.wrapped = wrapped;
        }

        /// <inheritdoc/>
        public JToken FormatValue(MemberInfo member, object value)
        {
            return this.wrapped.FormatValue(member, value);
        }

        /// <inheritdoc/>
        public string GetDocumentType(Type type)
        {
            return this.wrapped.GetDocumentType(type);
        }

        /// <inheritdoc/>
        public string GetFieldName(Type type, MemberExpression memberExpression)
        {
            return
                memberExpression.Member.DeclaringType == typeof(ElasticFields)
                    ? "_" + memberExpression.Member.Name.ToLowerInvariant()
                    : this.wrapped.GetFieldName(type, memberExpression);
        }

        /// <inheritdoc/>
        public ICriteria GetTypeSelectionCriteria(Type type)
        {
            return this.wrapped.GetTypeSelectionCriteria(type);
        }

        /// <inheritdoc/>
        public object Materialize(JToken sourceDocument, Type objectType)
        {
            return this.wrapped.Materialize(sourceDocument, objectType);
        }
    }
}
