using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace SagasDemo.OrderProcessor
{
    public static class TypeExtensions
    {
        public static string ToFriendlyName(this Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, nameof(type));
            if (!type.IsGenericType)
            {
                return type.FullName;
            }

            return type.FormatGenericType();
        }

        private static string FormatGenericType(this Type type)
        {
            var name = type.GetGenericTypeDefinition().FullName;
            name = name.Substring(0, name.IndexOf('`'));
           
            name += "<";
            var first = true;
            foreach (var argument in type.GetGenericArguments())
            {
                if (!first)
                {
                    name += ",";
                }
                else
                {
                    first = false;
                }
                name += argument.Name;

            }
            name += ">";
            return name;
        }
}
