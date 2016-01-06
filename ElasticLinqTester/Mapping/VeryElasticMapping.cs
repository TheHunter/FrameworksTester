using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Mapping
{
    public class VeryElasticMapping
        : IElasticMapping
    {
        private readonly JsonSerializer serializer;

        public VeryElasticMapping(JsonSerializerSettings jsonSettings)
        {
            this.serializer = JsonSerializer.Create(jsonSettings);
        }

        public JToken FormatValue(MemberInfo member, object value)
        {
            Argument.CheckNotNull("member", member);

            if (value == null)
                return new JValue((string) null);

            var strVal = value as string;
            return !member.IsNotAnalyzed() && strVal != null
                ? new JValue(strVal.ToLower(CultureInfo.CurrentCulture))
                : JToken.FromObject(value, this.serializer);
        }

        public string GetDocumentType(Type type)
        {
            return type.Name.ToCamelCase(CultureInfo.CurrentCulture);
        }

        public string GetFieldName(Type type, MemberExpression memberExpression)
        {
            Argument.CheckNotNull("memberExpression", memberExpression);

            switch (memberExpression.Expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return this.GetFieldName(type, (MemberExpression)memberExpression.Expression) + "." + this.GetFieldName(type, memberExpression.Member);

                case ExpressionType.Parameter:
                    return this.GetFieldName(type, memberExpression.Member);

                default:
                    throw new NotSupportedException(string.Format("Unknown expression type {0} for left hand side of expression {1}", memberExpression.Expression.NodeType, memberExpression));
            }
        }

        public virtual string GetFieldName(Type type, MemberInfo memberInfo)
        {
            Argument.CheckNotNull("type", type);
            Argument.CheckNotNull("memberInfo", memberInfo);

            return this.GetMemberName(memberInfo);
        }

        protected string GetMemberName(MemberInfo memberInfo)
        {
            var jsonPropertyAttribute = memberInfo.GetCustomAttribute<JsonPropertyAttribute>(inherit: true);

            if (jsonPropertyAttribute != null)
                return jsonPropertyAttribute.PropertyName;

            return memberInfo.Name.ToCamelCase(CultureInfo.CurrentCulture);
        }

        public ICriteria GetTypeSelectionCriteria(Type type)
        {
            Argument.CheckNotNull("docType", type);
            return null;
        }

        public object Materialize(JToken sourceDocument, Type sourceType)
        {
            return sourceDocument.ToObject(sourceType, this.serializer);
        }
    }
}
