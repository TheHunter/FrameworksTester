using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ElasticSearch.Linq.Request.Criteria;
using ElasticSearch.Linq.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Converter
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    /// <seealso cref="ElasticSearch.Linq.Converter.ICriteriaConverter" />
    public class CriteriaJsonConverter
        : JsonConverter, ICriteriaConverter
    {
        private static readonly Type CriteriaType = typeof(ICriteria);

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The serializer.</param>
        /// <exception cref="System.InvalidOperationException">The given argument cannot be serialized because is not a ICriteria type.</exception>
        /// <exception cref="System.ArgumentException">Error on writting the given criteria, see inner exception for details.</exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!this.CanConvert(value.GetType()))
                throw new InvalidOperationException("The given argument cannot be serialized because is not a ICriteria type.");

            try
            {
                JObject result = this.MakeToken(value as ICriteria);
                using (var reader = new JTokenReader(result))
                {
                    writer.WriteToken(reader);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error on writting the given criteria, see inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">Method not invokable...</exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Method not invokable...");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return CriteriaType.IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Makes the token.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Error on translate the given criteria into jtoken, see inner exception for details.</exception>
        public virtual JObject MakeToken(ICriteria criteria)
        {
            try
            {
                return this.Build(criteria as dynamic);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error on translate the given criteria into jtoken, see inner exception for details.", ex);
            }
        }

        #region
        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">criteria;Criteria must be referenced.</exception>
        /// <exception cref="System.NotImplementedException"></exception>
        protected JObject Build(ICriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria", "Criteria must be referenced.");

            throw new NotImplementedException(string.Format("The given criteria is not managed by this builder, type: {0}", criteria.GetType().FullName));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(RangeCriteria criteria)
        {
            return new JObject(
                new JProperty(criteria.Name,
                    new JObject(new JProperty(criteria.Field,
                        new JObject(criteria.Specifications.Select(s =>
                            new JProperty(s.Name, this.FormatValue(criteria.Member, s.Value))).ToList())))));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(RegexpCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Regexp))));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(PrefixCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Prefix))));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(TermCriteria criteria)
        {
            return new JObject(
                new JProperty(criteria.Name, new JObject(
                    new JProperty(criteria.Field, this.FormatValue(criteria.Member, criteria.Value)))));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(TermsCriteria criteria)
        {
            var termsCriteria = new JObject(
                new JProperty(criteria.Field,
                    new JArray(criteria.Values.Select(x => this.FormatValue(criteria.Member, x)).ToArray())));

            if (criteria.ExecutionMode.HasValue)
                termsCriteria.Add(new JProperty("execution", criteria.ExecutionMode.GetValueOrDefault().ToString()));

            return new JObject(new JProperty(criteria.Name, termsCriteria));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(NotCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, this.Build(criteria.Criteria as dynamic)));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(QueryStringCriteria criteria)
        {
            // We do not reformat query_string
            var unformattedValue = criteria.Value;

            var queryStringCriteria = new JObject(new JProperty("query", unformattedValue));

            if (criteria.Fields.Any())
                queryStringCriteria.Add(new JProperty("fields", new JArray(criteria.Fields)));

            return new JObject(new JProperty(criteria.Name, queryStringCriteria));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(MatchAllCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(BoolCriteria criteria)
        {
            return new JObject(
                new JProperty(criteria.Name,
                    new JObject(
                        new JProperty("must", new JArray(criteria.Must.Select(token => this.Build(token as dynamic)))),
                        new JProperty("must_not", new JArray(criteria.MustNot.Select(token => this.Build(token as dynamic)))),
                        new JProperty("should", new JArray(criteria.Should.Select(token => this.Build(token as dynamic)))),
                        new JProperty("minimum_should_match", 1)
                        )
                    )
                );
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(SingleFieldCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty("field", criteria.Field))));
        }

        /// <summary>
        /// Builds the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected JObject Build(CompoundCriteria criteria)
        {
            return criteria.Criteria.Count == 1
                ? this.Build(criteria.Criteria.First() as dynamic)
                : new JObject(new JProperty(criteria.Name, new JArray(criteria.Criteria.Select(token => this.Build(token as dynamic)).ToList())));
        }

        /// <summary>
        /// Formats the value.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected object FormatValue(MemberInfo member, object value)
        {
            Argument.CheckNotNull("member", member);

            if (value == null)
                return new JValue((string)null);

            var strVal = value as string;
            return !member.IsNotAnalyzed() && strVal != null
                ? strVal.ToLower(CultureInfo.CurrentCulture)
                : value;
        }

        #endregion
    }
}
