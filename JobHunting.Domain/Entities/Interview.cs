using JobHunting.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using JobHunting.Domain.Exceptions;
using JobHunting.Domain.Primatives;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Domain.Entities
{
    // Domain/Entities/Interview.cs
    public class Interview : Entity<InterviewId>
    {
        public ApplicationId ApplicationId { get; private set; }
        public int RoundNumber { get; private set; }
        public InterviewType Type { get; private set; }
        public DateTime ScheduledAt { get; private set; }
        public TimeSpan Duration { get; private set; }
        public ContactInfo? Interviewer { get; private set; }
        public InterviewStatus Status { get; private set; }
        public string? Notes { get; private set; }
        public int? Rating { get; private set; } // 1-5

        private Interview() { }

        public static Interview Create(
            ApplicationId applicationId,
            int roundNumber,
            InterviewType type,
            DateTime scheduledAt,
            TimeSpan duration,
            ContactInfo? interviewer)
        {
            if (scheduledAt < DateTime.UtcNow.AddMinutes(-1))
                throw new DomainException("Interview cannot be scheduled in the past");

            return new Interview
            {
                Id = InterviewId.New(),
                ApplicationId = applicationId,
                RoundNumber = roundNumber,
                Type = type,
                ScheduledAt = scheduledAt,
                Duration = duration,
                Interviewer = interviewer,
                Status = InterviewStatus.Scheduled
            };
        }

        public void Complete(string? notes, int? rating)
        {
            if (rating is < 1 or > 5)
                throw new DomainException("Rating must be between 1 and 5");

            Status = InterviewStatus.Completed;
            Notes = notes;
            Rating = rating;
        }

        public void Cancel(string reason)
        {
            Status = InterviewStatus.Cancelled;
            Notes = reason;
        }
    }

}
