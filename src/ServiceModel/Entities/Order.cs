using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo
{
    [Serializable]
    public class Order
    {
        public Customer Customer { get; set; }
        public ShoppingCart Cart { get; set; }
        public Status Status { get; set; }
    }
}
