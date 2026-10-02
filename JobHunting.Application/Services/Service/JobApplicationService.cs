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
using Microsoft.Extensions.Logging;

namespace JobHunting.Application.Services.Service
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _applicationRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<JobApplicationService> _logger;
        private readonly ICurrentUserService _currentUser;
   
        public JobApplicationService(
            IJobApplicationRepository applicationRepository,
            ICompanyRepository companyRepository,
            ILogger<JobApplicationService> logger,
            ICurrentUserService currentUser)
        {
            _applicationRepository = applicationRepository;
            _companyRepository = companyRepository;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<Result<ApplicationResponse>> CreateAsync(CreateJobApplicationRequest request, CancellationToken ct = default)
        {
            var userId = _currentUser.UserId!;

            if (string.IsNullOrWhiteSpace(request.Application.JobTitle))
                return Result<ApplicationResponse>.Failure(Error.Invalid("JobTitle is required"));

            if (string.IsNullOrWhiteSpace(request.Application.SourceType))
                return Result<ApplicationResponse>.Failure(Error.Invalid("SourceType is required"));

            if (string.IsNullOrWhiteSpace(request.Application.WorkType))
                return Result<ApplicationResponse>.Failure(Error.Invalid("WorkType is required"));

            //var companyId = new CompanyId(request.Application.CompanyId);
            //var companyExists = await _companyRepository.ExistsAsync(companyId, ct);

            //if (!companyExists)
            //    return Result<ApplicationResponse>.Failure(Error.NotFound("Company not found"));

            Money? salaryExpectation = null;
            if (request.Application.SalaryExpectation.HasValue && request.Application.SalaryExpectation.Value > 0)
            {
                var currency = string.IsNullOrWhiteSpace(request.Application.SalaryCurrency) ? "PHP" : request.Application.SalaryCurrency;
                salaryExpectation = new Money(request.Application.SalaryExpectation.Value, currency);
            }

            if (!Enum.TryParse<SourceType>(request.Application.SourceType, ignoreCase: true, out var sourceType))
            {
                return Result<ApplicationResponse>.Failure(
                    Error.Invalid($"Invalid SourceType: {request.Application.SourceType}"));
            }

            if (!Enum.TryParse<WorkType>(request.Application.WorkType, ignoreCase: true, out var workType))
            {
                return Result<ApplicationResponse>.Failure(
                    Error.Invalid($"Invalid WorkType: {request.Application.WorkType}"));
            }

            ApplicationSource source = sourceType switch
            {
                SourceType.LinkedIn => ApplicationSource.LinkedIn(request.Application.SourceUrl ?? ""),
                SourceType.Referral => ApplicationSource.Referral(request.Application.ReferralName ?? ""),
                _ => new ApplicationSource 
                { 
                    Type = sourceType,
                    Url = request.Application.SourceUrl,
                    ReferralContactName = request.Application.ReferralName
                }
            };

            var company = Company.Create(
                name: request.Company.Name, 
                location: request.Company.Location
            );

            var application = JobApplication.Create(
                userId: userId,
                companyId: company.Id,
                jobTitle: request.Application.JobTitle,
                JobDescription: request.Application.JobDescription,
                source: source,
                salaryExpectation: salaryExpectation,
                workType: workType
            );

            await _companyRepository.AddAsync(company);
            await _applicationRepository.AddAsync(application, ct);

            var response = MapToResponse(application);

            return Result<ApplicationResponse>.Success(response);
        }

        public async Task<Result<ApplicationResponse>> GetByIdAsync(Guid applicationId, CancellationToken ct = default)
        {
            var appId = new ApplicationId(applicationId);
            var application = await _applicationRepository.GetByIdAsync(appId, ct);

            if (application is null)
                return Result<ApplicationResponse>.Failure(Error.NotFound("Application not found"));

            _logger.LogInformation($"application: {application.Id} - {application.JobTitle}");
            var response = MapToResponse(application);
            return Result<ApplicationResponse>.Success(response);
        }

        public async Task<Result<IReadOnlyList<ApplicationResponse>>> GetUserPipelineAsync(CancellationToken ct = default)
        {
            var userId = _currentUser.UserId;

            if (userId is null)
                return Result<IReadOnlyList<ApplicationResponse>>.Failure(Error.NotFound("Application not found"));

            var applications = await _applicationRepository.GetByUserIdAsync(userId, ct);
            var responses = applications.Select(MapToResponse).ToList().AsReadOnly();

            var result = responses.Select(x => x.CompanyName).First();
            Console.WriteLine($"Console: {result}");
            _logger.LogInformation(result);
            return Result<IReadOnlyList<ApplicationResponse>>.Success(responses);
        }

        public async Task<Result<InterviewResponse>> ScheduleInterviewAsync(Guid applicationId, ScheduleInterviewRequest request, CancellationToken ct = default)
        {
            if (request.ScheduledAt <= DateTime.UtcNow)
                return Result<InterviewResponse>.Failure(Error.Invalid("Scheduled date must be in the future"));

            var appId = new ApplicationId(applicationId);
            var application = await _applicationRepository.GetByIdAsync(appId, ct);

            if (application is null)
                return Result<InterviewResponse>.Failure(Error.NotFound("Application not found"));

            // DomainException from ScheduleInterview bubbles up to GlobalExceptionMiddleware
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

        public async Task<Result> MoveStatusAsync(Guid applicationId, MoveStatusRequest request, CancellationToken ct = default)
        {
            var appId = new ApplicationId(applicationId);
            var application = await _applicationRepository.GetByIdAsync(appId, ct);

            if (application is null)
                return Result.Failure(Error.NotFound("Application not found"));

            application.MoveToStatus(request.NewStatus, request.Reason);

            await _applicationRepository.UpdateAsync(application, ct);

            return Result.Success();
        }

        private static ApplicationResponse MapToResponse(JobApplication application)
        {
            return new ApplicationResponse(
                Id: application.Id.Value,
                CompanyName: "",
                JobTitle: application.JobTitle,
                JobDescription: application.JobDescription,
                SalaryExpectation: application.SalaryExpectation,
                PostedSalaryRange: application.PostedSalaryRange,
                Source: application.Source,
                WorkType: application.WorkType,
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
