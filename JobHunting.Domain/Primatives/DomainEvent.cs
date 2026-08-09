using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Domain.Primatives
{
    public abstract record DomainEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
