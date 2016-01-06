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
    /// <summary>
    /// A base class for mapping Elasticsearch values that can lower-case all field values
    /// (and respects <see cref="NotAnalyzedAttribute"/> to opt-out of the lower-casing), 
    /// camel-case field names, and camel-case and pluralize type names.
    /// </summary>
    public class ElasticMapping : IElasticMapping
    {
        readonly bool camelCaseFieldNames;
        readonly bool camelCaseTypeNames;
        readonly CultureInfo conversionCulture;
        readonly bool lowerCaseAnalyzedFieldValues;
        readonly bool pluralizeTypeNames;
        readonly EnumFormat enumFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticMapping"/> class.
        /// </summary>
        /// <param name="camelCaseFieldNames">Pass <c>true</c> to automatically camel-case field names (for <see cref="GetFieldName(Type, MemberInfo)"/>).</param>
        /// <param name="camelCaseTypeNames">Pass <c>true</c> to automatically camel-case type names (for <see cref="GetDocumentType"/>).</param>
        /// <param name="pluralizeTypeNames">Pass <c>true</c> to automatically pluralize type names (for <see cref="GetDocumentType"/>).</param>
        /// <param name="lowerCaseAnalyzedFieldValues">Pass <c>true</c> to automatically convert field values to lower case (for <see cref="FormatValue"/>).</param>
        /// <param name="enumFormat">Pass <c>EnumFormat.String</c> to format enums as strings or <c>EnumFormat.Integer</c> to use integers (defaults to string).</param>
        /// <param name="conversionCulture">The culture to use for the lower-casing, camel-casing, and pluralization operations. If <c>null</c>,
        /// uses <see cref="CultureInfo.CurrentCulture"/>.</param>
        public ElasticMapping(bool camelCaseFieldNames = true,
                              bool camelCaseTypeNames = true,
                              bool pluralizeTypeNames = true,
                              bool lowerCaseAnalyzedFieldValues = true,
                              EnumFormat enumFormat = EnumFormat.String,
                              CultureInfo conversionCulture = null)
        {
            this.camelCaseFieldNames = camelCaseFieldNames;
            this.camelCaseTypeNames = camelCaseTypeNames;
            this.pluralizeTypeNames = pluralizeTypeNames;
            this.lowerCaseAnalyzedFieldValues = lowerCaseAnalyzedFieldValues;
            this.conversionCulture = conversionCulture ?? CultureInfo.CurrentCulture;
            this.enumFormat = enumFormat;
        }

        /// <inheritdoc/>
        public virtual JToken FormatValue(MemberInfo member, object value)
        {
            Argument.CheckNotNull("member", member);

            if (value == null)
                return new JValue((string)null);

            if (this.enumFormat == EnumFormat.String)
                value = ReformatValueIfEnum(member, value);

            var result = JToken.FromObject(value);

            if (this.lowerCaseAnalyzedFieldValues && result.Type == JTokenType.String && !this.IsNotAnalyzed(member))
            {
                var loweredString = this.conversionCulture.TextInfo.ToLower(result.Value<string>());
                result = new JValue(loweredString);
            }

            return result;
        }

        static object ReformatValueIfEnum(MemberInfo member, object value)
        {
            var returnType = TypeHelper.GetReturnType(member);
            if (!returnType.GetTypeInfo().IsEnum) return value;

            var nameValue = Enum.GetName(returnType, value);
            if (nameValue == null)
                throw new ArgumentOutOfRangeException("value",
                    string.Format("Value '{0}' is not defined for enum type '{1}'.", value, returnType.FullName));

            return nameValue;
        }

        /// <inheritdoc/>
        public virtual string GetFieldName(Type type, MemberExpression memberExpression)
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

        /// <summary>
        /// Get the Elasticsearch field name for a given member.
        /// </summary>
        /// <param name="type">The prefix to put in front of this field name, if the field is
        /// an ongoing part of the document search.</param>
        /// <param name="memberInfo">The member whose field name is required.</param>
        /// <returns>The Elasticsearch field name that matches the member.</returns>
        public virtual string GetFieldName(Type type, MemberInfo memberInfo)
        {
            Argument.CheckNotNull("type", type);
            Argument.CheckNotNull("memberInfo", memberInfo);

            return this.GetMemberName(memberInfo);
        }

        /// <summary>
        /// Get the name of the member to be used as a field name.
        /// </summary>
        /// <param name="memberInfo">The member whose field name is required.</param>
        /// <returns>Name of the member to be used as a field name.</returns>
        protected string GetMemberName(MemberInfo memberInfo)
        {
            var jsonPropertyAttribute = memberInfo.GetCustomAttribute<JsonPropertyAttribute>(inherit: true);

            if (jsonPropertyAttribute != null)
                return jsonPropertyAttribute.PropertyName;

            return this.camelCaseFieldNames
                ? memberInfo.Name.ToCamelCase(this.conversionCulture)
                : memberInfo.Name;
        }

        /// <inheritdoc/>
        public virtual string GetDocumentType(Type type)
        {
            Argument.CheckNotNull("type", type);

            var result = type.Name;
            if (this.pluralizeTypeNames)
                result = result.ToPlural(this.conversionCulture);
            if (this.camelCaseTypeNames)
                result = result.ToCamelCase(this.conversionCulture);

            return result;
        }

        /// <inheritdoc/>
        public virtual ICriteria GetTypeSelectionCriteria(Type type)
        {
            Argument.CheckNotNull("docType", type);
            return null;
        }

        /// <summary>
        /// Determine whether a field is "not analyzed". By default, looks for the member to be
        /// decorated with the <see cref="NotAnalyzedAttribute" />.
        /// </summary>
        /// <param name="member">The member to evaluate.</param>
        /// <returns>
        /// Returns <c>true</c> if the field is not analyzed; <c>false</c>, otherwise.
        /// </returns>
        protected virtual bool IsNotAnalyzed(MemberInfo member)
        {
            return member.GetCustomAttribute<NotAnalyzedAttribute>(inherit: true) != null;
        }

        /// <summary>
        /// Materialize the JObject hit object from Elasticsearch to a CLR object.
        /// </summary>
        /// <param name="sourceDocument">JSON source document.</param>
        /// <param name="sourceType">Type of CLR object to materialize to.</param>
        /// <returns>
        /// Freshly materialized CLR object version of the source document.
        /// </returns>
        /// <inheritedDoc />
        public virtual object Materialize(JToken sourceDocument, Type sourceType)
        {
            return sourceDocument.ToObject(sourceType);
        }
    }
}
