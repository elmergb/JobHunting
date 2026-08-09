using JobHunting.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using JobHunting.Domain.Primatives;
using JobHunting.Domain.DomainEvents;
using JobHunting.Domain.Exceptions;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Domain.Entities
{
    // Domain/Entities/JobApplication.cs
    public class JobApplication : AggregateRoot<ApplicationId>
    {
        // Identity reference (outside our bounded context)
        public string UserId { get; private set; }

        // Reference to another aggregate (by ID only, never hold Company object)
        public CompanyId CompanyId { get; private set; }

        public string JobTitle { get; private set; }
        public string? JobDescription { get; private set; }
        public Money? SalaryExpectation { get; private set; }
        public Money? PostedSalaryRange { get; private set; }
        public DateTime AppliedDate { get; private set; }
        public ApplicationSource Source { get; private set; }
        public WorkType WorkType { get; private set; }
        public ApplicationStatus Status { get; private set; }
        public string? Notes { get; private set; }
        public bool IsArchived { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation to child entities (same aggregate)
        private readonly List<Interview> _interviews = new();
        public IReadOnlyCollection<Interview> Interviews => _interviews.AsReadOnly();

        private readonly List<StatusChange> _history = new();
        public IReadOnlyCollection<StatusChange> History => _history.AsReadOnly();

        public Offer? Offer { get; private set; }

        private JobApplication() { } // EF Core

        public static JobApplication Create(
            string userId,
            CompanyId companyId,
            string jobTitle,
            ApplicationSource source,
            Money? salaryExpectation = null,
            WorkType workType = WorkType.Hybrid)
        {
            var application = new JobApplication
            {
                Id = ApplicationId.New(),
                UserId = userId,
                CompanyId = companyId,
                JobTitle = jobTitle,
                Source = source,
                SalaryExpectation = salaryExpectation,
                WorkType = workType,
                Status = ApplicationStatus.Wishlist, // Default before applied
                AppliedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            application.AddDomainEvent(new ApplicationCreatedEvent(application.Id));
            return application;
        }

        public void Apply(DateTime appliedDate)
        {
            if (Status != ApplicationStatus.Wishlist)
                throw new DomainException("Can only apply from Wishlist status");

            var oldStatus = Status;
            Status = ApplicationStatus.Applied;
            AppliedDate = appliedDate;

            _history.Add(new StatusChange(Id, oldStatus, Status, "Application submitted"));
            AddDomainEvent(new StatusChangedEvent(Id, oldStatus, Status));
        }

        public void MoveToStatus(ApplicationStatus newStatus, string? reason = null)
        {
            if (!CanTransitionTo(newStatus))
                throw new DomainException($"Cannot transition from {Status} to {newStatus}");

            var oldStatus = Status;
            Status = newStatus;

            _history.Add(new StatusChange(Id, oldStatus, newStatus, reason));
            AddDomainEvent(new StatusChangedEvent(Id, oldStatus, newStatus));

            if (newStatus == ApplicationStatus.OfferReceived && Offer is null)
                throw new DomainException("Cannot move to OfferReceived without an Offer entity");
        }

        public Interview ScheduleInterview(
            InterviewType type,
            DateTime scheduledAt,
            TimeSpan duration,
            ContactInfo? interviewer = null)
        {
            var roundNumber = _interviews.Count + 1;
            var interview = Interview.Create(Id, roundNumber, type, scheduledAt, duration, interviewer);
            _interviews.Add(interview);

            AddDomainEvent(new InterviewScheduledEvent(Id, interview.Id, scheduledAt));
            return interview;
        }

        public void AddOffer(Offer offer)
        {
            if (Offer is not null)
                throw new DomainException("Application already has an offer");

            Offer = offer;
            MoveToStatus(ApplicationStatus.OfferReceived, "Offer received");
        }

        public void Archive()
        {
            IsArchived = true;
        }

        private bool CanTransitionTo(ApplicationStatus newStatus)
        {
            // Define valid state machine transitions
            return (Status, newStatus) switch
            {
                (ApplicationStatus.Wishlist, ApplicationStatus.Applied) => true,
                (ApplicationStatus.Applied, ApplicationStatus.PhoneScreen) => true,
                (ApplicationStatus.Applied, ApplicationStatus.Rejected) => true,
                (ApplicationStatus.PhoneScreen, ApplicationStatus.Technical) => true,
                (ApplicationStatus.PhoneScreen, ApplicationStatus.Rejected) => true,
                (ApplicationStatus.Technical, ApplicationStatus.Onsite) => true,
                (ApplicationStatus.Technical, ApplicationStatus.Rejected) => true,
                (ApplicationStatus.Onsite, ApplicationStatus.OfferReceived) => true,
                (ApplicationStatus.Onsite, ApplicationStatus.Rejected) => true,
                (ApplicationStatus.OfferReceived, ApplicationStatus.Accepted) => true,
                (ApplicationStatus.OfferReceived, ApplicationStatus.Declined) => true,
                _ => false
            };
        }
    }
}
