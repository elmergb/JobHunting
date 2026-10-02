using JobHunting.Domain;
using JobHunting.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Responses
{
    public record ApplicationResponse(
        Guid Id,
        string CompanyName,
        string JobTitle,
        string? JobDescription,
        Money? SalaryExpectation,
        Money? PostedSalaryRange,
        ApplicationSource Source,
        WorkType WorkType,
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
