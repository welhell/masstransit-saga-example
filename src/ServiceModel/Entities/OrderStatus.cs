using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo
{
    [Serializable]
    public class OrderStatus
    {
        public Status Status { get; set; }
        public string Reason { get; set; }
    }
}
