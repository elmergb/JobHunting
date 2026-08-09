using JobHunting.Domain.Primatives;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Domain.Entities
{
    public class StatusChange : Entity<Guid>
    {
        public ApplicationId ApplicationId { get; private set; }
        public ApplicationStatus FromStatus { get; private set; }
        public ApplicationStatus ToStatus { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public string? Reason { get; private set; }

        private StatusChange() { }

        public StatusChange(ApplicationId applicationId, ApplicationStatus from, ApplicationStatus to, string? reason = null)
        {
            Id = Guid.NewGuid();
            ApplicationId = applicationId;
            FromStatus = from;
            ToStatus = to;
            ChangedAt = DateTime.UtcNow;
            Reason = reason;
        }
    }
}
