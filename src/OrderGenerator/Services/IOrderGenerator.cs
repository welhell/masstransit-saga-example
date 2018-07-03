using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo.Generator.Services
{
    public interface IOrderGenerator
    {
        Order Generate();
    }
}
