using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace ElasticSearchTester.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class Converter
    {

        //BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy

        /// <summary>
        /// Ases the dynamic.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static dynamic AsDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            return expando;
        }
    }
}
