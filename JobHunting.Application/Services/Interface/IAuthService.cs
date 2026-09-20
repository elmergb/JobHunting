using JobHunting.Application.Common;
using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Services.Interface
{
    public interface IAuthService
    {
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<Result<UserResponse>> GetCurrentUser(CancellationToken ct = default);
    }
}
