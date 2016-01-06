﻿// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using ElasticSearch.Linq.Utility;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Response.Materializers
{
    [DebuggerDisplay("Field {Name,nq}.{Operation,nq} = {Token}")]
    public class AggregateField
    {
        readonly string name;
        readonly string operation;
        readonly JToken token;

        public AggregateField(string name, string operation, JToken token)
        {
            this.name = name;
            this.operation = operation;
            this.token = token;
        }

        public string Name { get { return name; } }
        public string Operation { get { return operation; } }
        public JToken Token { get { return token; } }
    }

    public abstract class AggregateRow
    {
        static readonly DateTime EpocStartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        static readonly DateTimeOffset EpocStartDateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        internal static object GetValue(AggregateRow row, string name, string operation, Type valueType)
        {
            return row.GetValue(name, operation, valueType);
        }

        internal abstract object GetValue(string name, string operation, Type valueType);

        internal static object GetKey(AggregateRow row)
        {
            if (row is AggregateTermRow)
                return ((AggregateTermRow)row).Key;

            if (row is AggregateStatisticalRow)
                return ((AggregateStatisticalRow)row).Key;

            return null;
        }

        internal static object ParseValue(JToken token, Type valueType)
        {
            switch (token.ToString())
            {
                case "Infinity":
                case "∞":
                    {
                        if (valueType == typeof(double))
                            return double.PositiveInfinity;

                        if (valueType == typeof(float))
                            return float.PositiveInfinity;

                        if (valueType == typeof(decimal?))
                            return null;

                        break;
                    }

                case "-Infinity":
                case "-∞":
                    {
                        if (valueType == typeof(double))
                            return double.NegativeInfinity;

                        if (valueType == typeof(float))
                            return float.NegativeInfinity;

                        if (valueType == typeof(decimal?))
                            return null;

                        break;
                    }
            }

            // Elasticsearch returns dates as milliseconds since epoch when using Facets
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            {
                if (valueType == typeof(DateTime))
                    return EpocStartDateTime.AddMilliseconds((double)token);
                if (valueType == typeof(DateTimeOffset))
                    return EpocStartDateTimeOffset.AddMilliseconds((double)token);
            }

            return token.ToObject(valueType);
        }
    }

    [DebuggerDisplay("Statistical Row")]
    class AggregateStatisticalRow : AggregateRow
    {
        readonly object key;
        readonly JObject facets;

        public AggregateStatisticalRow(object key, JObject facets)
        {
            this.key = key;
            this.facets = facets;
        }

        internal override object GetValue(string name, string operation, Type valueType)
        {
            JToken facetObject, operationObject;
            return facets.TryGetValue(name, out facetObject)
                   && facetObject is JObject
                   && ((JObject)facetObject).TryGetValue(operation, out operationObject)
                ? ParseValue(operationObject, valueType)
                : TypeHelper.CreateDefault(valueType);
        }

        public object Key { get { return key; } }
    }

    [DebuggerDisplay("Term Row {Key} Fields({Fields.Count})")]
    class AggregateTermRow : AggregateRow
    {
        readonly object key;
        readonly ReadOnlyCollection<AggregateField> fields;

        public AggregateTermRow(object key, IEnumerable<AggregateField> fields)
        {
            this.key = key;
            this.fields = new ReadOnlyCollection<AggregateField>(fields.ToArray());
        }

        public object Key
        {
            get { return key; }
        }

        public ReadOnlyCollection<AggregateField> Fields
        {
            get { return fields; }
        }

        internal override object GetValue(string name, string operation, Type valueType)
        {
            var field = fields.FirstOrDefault(f => f.Name == name && f.Operation == operation);

            return field == null
                ? TypeHelper.CreateDefault(valueType)
                : ParseValue(field.Token, valueType);
        }
    }
}