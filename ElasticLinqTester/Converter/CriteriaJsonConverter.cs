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
    public class CriteriaJsonConverter
        : JsonConverter, ICriteriaConverter
    {
        private static readonly Type CriteriaType = typeof(ICriteria);

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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Method not invokable...");
        }

        public override bool CanConvert(Type objectType)
        {
            return CriteriaType.IsAssignableFrom(objectType);
        }

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
        protected JObject Build(ICriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria", "Criteria must be referenced.");

            throw new NotImplementedException(string.Format("The given criteria is not managed by this builder, type: {0}", criteria.GetType().FullName));
        }

        protected JObject Build(RangeCriteria criteria)
        {
            return new JObject(
                new JProperty(criteria.Name,
                    new JObject(new JProperty(criteria.Field,
                        new JObject(criteria.Specifications.Select(s =>
                            new JProperty(s.Name, this.FormatValue(criteria.Member, s.Value))).ToList())))));
        }

        protected JObject Build(RegexpCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Regexp))));
        }

        protected JObject Build(PrefixCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty(criteria.Field, criteria.Prefix))));
        }

        protected JObject Build(TermCriteria criteria)
        {
            return new JObject(
                new JProperty(criteria.Name, new JObject(
                    new JProperty(criteria.Field, this.FormatValue(criteria.Member, criteria.Value)))));
        }

        protected JObject Build(TermsCriteria criteria)
        {
            var termsCriteria = new JObject(
                new JProperty(criteria.Field,
                    new JArray(criteria.Values.Select(x => this.FormatValue(criteria.Member, x)).ToArray())));

            if (criteria.ExecutionMode.HasValue)
                termsCriteria.Add(new JProperty("execution", criteria.ExecutionMode.GetValueOrDefault().ToString()));

            return new JObject(new JProperty(criteria.Name, termsCriteria));
        }

        protected JObject Build(NotCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, this.Build(criteria.Criteria as dynamic)));
        }

        protected JObject Build(QueryStringCriteria criteria)
        {
            // We do not reformat query_string
            var unformattedValue = criteria.Value;

            var queryStringCriteria = new JObject(new JProperty("query", unformattedValue));

            if (criteria.Fields.Any())
                queryStringCriteria.Add(new JProperty("fields", new JArray(criteria.Fields)));

            return new JObject(new JProperty(criteria.Name, queryStringCriteria));
        }

        protected JObject Build(MatchAllCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name));
        }

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

        protected JObject Build(SingleFieldCriteria criteria)
        {
            return new JObject(new JProperty(criteria.Name, new JObject(new JProperty("field", criteria.Field))));
        }

        protected JObject Build(CompoundCriteria criteria)
        {
            return criteria.Criteria.Count == 1
                ? this.Build(criteria.Criteria.First() as dynamic)
                : new JObject(new JProperty(criteria.Name, new JArray(criteria.Criteria.Select(token => this.Build(token as dynamic)).ToList())));
        }

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
