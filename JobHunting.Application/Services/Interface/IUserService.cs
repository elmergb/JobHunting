using JobHunting.Application.Common;
using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Services.Interface
{
    public interface IUserService
    {
        Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
        Task<Result<UserResponse>> GetByIdAsync(Guid userId, CancellationToken ct = default);
    }
}
