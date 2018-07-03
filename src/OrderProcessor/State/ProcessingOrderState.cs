using Automatonymous;
using MassTransit.MongoDbIntegration.Saga;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagasDemo.OrderProcessor.State
{
    public class ProcessingOrderState: SagaStateMachineInstance, IVersionedSaga
    {
        public ProcessingOrderState(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }
        public ObjectId Id { get; set; }
        public Order Order { get; set; }      
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid CorrelationId { get; set; }

        public string State { get; set; }
        public int Version { get; set; }
    }
}
