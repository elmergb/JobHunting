using JobHunting.Application.Common;
using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Services.Interface
{
    public interface IJobApplicationService
    {
        Task<Result<ApplicationResponse>> CreateAsync(CreateApplicationRequest request, CancellationToken ct = default);
        Task<Result<InterviewResponse>> ScheduleInterviewAsync(Guid applicationId, ScheduleInterviewRequest request, CancellationToken ct = default);
        Task<Result> MoveStatusAsync(Guid applicationId, MoveStatusRequest request, CancellationToken ct = default);
        Task<Result<ApplicationResponse>> GetByIdAsync(Guid applicationId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<ApplicationResponse>>> GetUserPipelineAsync(string userId, CancellationToken ct = default);
    }
}
