using JobHunting.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Responses
{
    public record ApplicationResponse(
        Guid Id,
        Guid CompanyId,
        string CompanyName,
        string JobTitle,
        ApplicationStatus Status,
        DateTime AppliedDate,
        DateTime CreatedAt,
        IReadOnlyList<InterviewResponse> Interviews
    );

    public record InterviewResponse(
        Guid Id,
        int RoundNumber,
        InterviewType Type,
        DateTime ScheduledAt,
        InterviewStatus Status,
        string? InterviewerName
    );
}
