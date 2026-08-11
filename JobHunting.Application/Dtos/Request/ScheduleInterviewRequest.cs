using JobHunting.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Request
{
    public record ScheduleInterviewRequest(
        InterviewType Type,
        DateTime ScheduledAt,
        int DurationMinutes,
        string? InterviewerName,
        string? InterviewerRole,
        string? InterviewerEmail
    );
}
