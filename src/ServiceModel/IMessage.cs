using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo
{
    public interface IMessage
    {
        Guid CorrelationId { get;  }

        Order Order { get;  }


    }
}
