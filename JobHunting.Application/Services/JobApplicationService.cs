using JobHunting.Application.Common;
using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Dtos.Responses;
using JobHunting.Application.Services.Interface;
using JobHunting.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

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


        public Task<Result<ApplicationResponse>> CreateAsync(CreateApplicationRequest request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        public Task<Result<InterviewResponse>> ScheduleInterviewAsync(Guid applicationId, ScheduleInterviewRequest request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        public Task<Result> MoveStatusAsync(Guid applicationId, MoveStatusRequest request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        public Task<Result<ApplicationResponse>> GetByIdAsync(Guid applicationId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        public Task<Result<IReadOnlyList<ApplicationResponse>>> GetUserPipelineAsync(string userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
