using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace SagasDemo.Generator.Services
{
    public static class OrderGenerator
    {
        private static IOrderGenerator current;
        public static IOrderGenerator Currrent
        {
            get
            {
                current = current ?? new AutoFixtureOrderGenerator();
                return current;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null,nameof(value));
                current = value;
            }
        }

        public static void Reset() => current = null;
    }
}
