using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        /// <param name="dynamicProperties">The dynamic properties.</param>
        /// <returns></returns>
        public static dynamic AsDynamic(this object value, params KeyValuePair<string, object>[] dynamicProperties)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            foreach (var dynamicProperty in dynamicProperties)
            {
                expando.Add(dynamicProperty.Key, dynamicProperty.Value);
            }

            return expando;
        }

        public static PropertyInfo PropertyName<TInstance>(Expression<Func<TInstance, object>> expression)
        {
            MemberExpression memberExpr = null;
            var exp = expression as LambdaExpression;
            if (exp == null)
                return null;

            switch (exp.Body.NodeType)
            {
                case ExpressionType.Convert:
                {
                    memberExpr = ((UnaryExpression)exp.Body).Operand as MemberExpression;
                    break;
                }
                case ExpressionType.MemberAccess:
                {
                    memberExpr = exp.Body as MemberExpression;
                    break;
                }
            }

            if (memberExpr == null)
                return null;

            return memberExpr.Member as PropertyInfo;
        }
    }
}
