using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo
{
    [Serializable]
    public class Product
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
