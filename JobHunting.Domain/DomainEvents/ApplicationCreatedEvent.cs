using JobHunting.Domain.ValueObjects;
using JobHunting.Domain.Primatives;

using System;
using System.Collections.Generic;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;
using InterviewId = JobHunting.Domain.Primatives.InterviewId;

namespace JobHunting.Domain.DomainEvents
{
    // Domain/DomainEvents/ApplicationCreatedEvent.cs
    public record ApplicationCreatedEvent(ApplicationId ApplicationId) : DomainEvent;
    public record StatusChangedEvent(
        ApplicationId ApplicationId,
        ApplicationStatus From,
        ApplicationStatus To) : DomainEvent;

    public record InterviewScheduledEvent(
        ApplicationId ApplicationId,
        InterviewId InterviewId,
        DateTime ScheduledAt) : DomainEvent;

    public record FollowUpDueEvent(
        ApplicationId ApplicationId,
        string CompanyName,
        int DaysSinceApplied) : DomainEvent;
}
