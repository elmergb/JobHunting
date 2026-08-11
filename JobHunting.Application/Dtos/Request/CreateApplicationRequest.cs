using JobHunting.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Request
{
    public record CreateApplicationRequest(
        string UserId,
        Guid CompanyId,
        string CompanyName,
        string JobTitle,
        string? JobDescription,
        decimal? SalaryExpectation,
        string? SalaryCurrency,
        string SourceType,     
        string? SourceUrl,
        string? ReferralName,
        WorkType WorkType
    );
}
