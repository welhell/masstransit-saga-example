using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo
{
    [Serializable]
    public class ShoppingCart
    {
        public List<Product> Products { get; set; }
    }
}
