using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Linq
{
    /// <summary>
    /// Used to mark serialized fields as being "not analayzed" in Elasticsearch
    /// (and therefore not subject to value transformations like lower-casing).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class NotAnalyzedAttribute : Attribute
    {
    }
}
