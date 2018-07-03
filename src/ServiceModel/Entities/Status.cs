using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo
{
    [Serializable]
    public enum Status
    {
        StockReserved,
        Submitted,
        Paymented,
        Shipped, 
        Processed,
    }
}
