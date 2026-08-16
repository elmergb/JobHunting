using JobHunting.Application.Common;
using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Dtos.Responses;
using JobHunting.Application.Services.Interface;
using JobHunting.Domain;
using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using JobHunting.Domain.Repositories;
using JobHunting.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;
using CompanyId = JobHunting.Domain.Primatives.CompanyId;

namespace JobHunting.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _applicationRepository;
        private readonly ICompanyRepository _companyRepository;

        public JobApplicationService(
            IJobApplicationRepository applicationRepository,
            ICompanyRepository companyRepository)
        {
            _applicationRepository = applicationRepository;
            _companyRepository = companyRepository;
        }

        public async Task<Result<ApplicationResponse>> CreateAsync(CreateApplicationRequest request, CancellationToken ct = default)
        {
            // 1️⃣ Validate required fields
            if (string.IsNullOrWhiteSpace(request.UserId))
                return Result<ApplicationResponse>.Failure(Error.Invalid("UserId is required"));

            if (string.IsNullOrWhiteSpace(request.JobTitle))
                return Result<ApplicationResponse>.Failure(Error.Invalid("JobTitle is required"));

            if (string.IsNullOrWhiteSpace(request.SourceType))
                return Result<ApplicationResponse>.Failure(Error.Invalid("SourceType is required"));

            if (string.IsNullOrWhiteSpace(request.WorkType))
                return Result<ApplicationResponse>.Failure(Error.Invalid("WorkType is required"));

            // 2️⃣ Check if company exists
            var companyId = new CompanyId(request.CompanyId);
            var companyExists = await _companyRepository.ExistsAsync(companyId, ct);

            if (!companyExists)
                return Result<ApplicationResponse>.Failure(Error.NotFound("Company not found"));

            // 3️⃣ Parse salary expectation if provided
            Money? salaryExpectation = null;
            if (request.SalaryExpectation.HasValue && request.SalaryExpectation.Value > 0)
            {
                var currency = string.IsNullOrWhiteSpace(request.SalaryCurrency) ? "PHP" : request.SalaryCurrency;
                salaryExpectation = new Money(request.SalaryExpectation.Value, currency);
            }

            // 4️⃣ Parse and validate SourceType
            if (!Enum.TryParse<SourceType>(request.SourceType, ignoreCase: true, out var sourceType))
            {
                return Result<ApplicationResponse>.Failure(
                    Error.Invalid($"Invalid SourceType: {request.SourceType}"));
            }

            // 5️⃣ Parse and validate WorkType
            if (!Enum.TryParse<WorkType>(request.WorkType, ignoreCase: true, out var workType))
            {
                return Result<ApplicationResponse>.Failure(
                    Error.Invalid($"Invalid WorkType: {request.WorkType}"));
            }

            // 6️⃣ Create application source based on type
            ApplicationSource source = sourceType switch
            {
                SourceType.LinkedIn => ApplicationSource.LinkedIn(request.SourceUrl ?? ""),
                SourceType.Referral => ApplicationSource.Referral(request.ReferralName ?? ""),
                _ => new ApplicationSource 
                { 
                    Type = sourceType,
                    Url = request.SourceUrl,
                    ReferralContactName = request.ReferralName
                }
            };

            // 7️⃣ Create the domain entity
            var application = JobApplication.Create(
                userId: request.UserId,
                companyId: companyId,
                jobTitle: request.JobTitle,
                source: source,
                salaryExpectation: salaryExpectation,
                workType: workType
            );

            // 8️⃣ Save to repository
            await _applicationRepository.AddAsync(application, ct);

            // 9️⃣ Map to response DTO
            var response = MapToResponse(application);

            return Result<ApplicationResponse>.Success(response);
        }

        public async Task<Result<ApplicationResponse>> GetByIdAsync(Guid applicationId, CancellationToken ct = default)
        {
            var appId = new ApplicationId(applicationId);
            var application = await _applicationRepository.GetByIdAsync(appId, ct);

            if (application is null)
                return Result<ApplicationResponse>.Failure(Error.NotFound("UserId Not Found"));


            var response = MapToResponse(application);
            return Result<ApplicationResponse>.Success(response);
        }

        public async Task<Result<IReadOnlyList<ApplicationResponse>>> GetUserPipelineAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<IReadOnlyList<ApplicationResponse>>.Failure(Error.NotFound("UserId Not Found"));


            var applications = await _applicationRepository.GetByUserIdAsync(userId, ct);
            var responses = applications.Select(MapToResponse).ToList().AsReadOnly();

            return Result<IReadOnlyList<ApplicationResponse>>.Success(responses);
        }

        public async Task<Result<InterviewResponse>> ScheduleInterviewAsync(Guid applicationId, ScheduleInterviewRequest request, CancellationToken ct = default)
        {
            if (request.ScheduledAt <= DateTime.UtcNow)
                return Result<InterviewResponse>.Failure(Error.NotFound("UserId Not Found"));


            var appId = new ApplicationId(applicationId);
            var application = await _applicationRepository.GetByIdAsync(appId, ct);

            if (application is null)
                return Result<InterviewResponse>.Failure(Error.NotFound("UserId Not Found"));


            try
            {
                var interview = application.ScheduleInterview(
                    type: request.Type,
                    scheduledAt: request.ScheduledAt,
                    duration: TimeSpan.FromMinutes(request.DurationMinutes),
                    interviewer: request.InterviewerName != null 
                        ? new ContactInfo { Name = request.InterviewerName, Role = request.InterviewerRole, Email = request.InterviewerEmail }
                        : null
                );

                await _applicationRepository.UpdateAsync(application, ct);

                var response = MapToInterviewResponse(interview);
                return Result<InterviewResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<InterviewResponse>.Failure(Error.NotFound("UserId Not Found"));
            }
        }

        public async Task<Result> MoveStatusAsync(Guid applicationId, MoveStatusRequest request, CancellationToken ct = default)
        {
            var appId = new ApplicationId(applicationId);
            var application = await _applicationRepository.GetByIdAsync(appId, ct);

            if (application is null)
                return Result.Failure(Error.NotFound("Not Found"));

            try
            {
                application.MoveToStatus(request.NewStatus, request.Reason);

                await _applicationRepository.UpdateAsync(application, ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.NotFound("Not Found"));
            }
        }

        private static ApplicationResponse MapToResponse(JobApplication application)
        {
            return new ApplicationResponse(
                Id: application.Id.Value,
                CompanyId: application.CompanyId.Value,
                CompanyName: "",
                JobTitle: application.JobTitle,
                Status: application.Status,
                AppliedDate: application.AppliedDate,
                CreatedAt: application.CreatedAt,
                Interviews: application.Interviews
                    .Select(MapToInterviewResponse)
                    .ToList()
                    .AsReadOnly()
            );
        }

        private static InterviewResponse MapToInterviewResponse(Interview interview)
        {
            return new InterviewResponse(
                Id: interview.Id.Value,
                RoundNumber: interview.RoundNumber,
                Type: interview.Type,
                ScheduledAt: interview.ScheduledAt,
                Status: interview.Status,
                InterviewerName: interview.Interviewer?.Name
            );
        }
    }
}
